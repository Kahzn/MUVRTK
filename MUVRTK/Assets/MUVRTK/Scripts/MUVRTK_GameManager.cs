using System.ComponentModel;
using System.Numerics;

namespace MUVRTK
{

    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.SceneManagement;
    using Photon.Pun;
    using Photon.Realtime;
    using VRTK;

    public class MUVRTK_GameManager : MonoBehaviourPunCallbacks
    {
        #region Private Serializable Fields

        [Tooltip("Log the debug messages for this script in the console.")]
        [SerializeField]
        private bool debug;

        [Header("Controller Section")]

        [Tooltip("Show Other Player Controllers in the scene.")]
        [SerializeField]
        private bool showControllersOfOtherPlayers;

        [Tooltip("List of Controller Model Objects. If not set, the Script will search the scene hierarchy for the models, which may be detrimental to performance.")]
        [SerializeField]
        private GameObject[] controllerModels;

        [Tooltip("Left Controller Object in the SDK-Setup. If not set, the Script will search the scene hierarchy for the models, which may be detrimental to performance.")]
        [SerializeField]
        private GameObject leftController;


        [Tooltip("Right Controller Object in the SDK-Setup. If not set, the Script will search the scene hierarchy for the models, which may be detrimental to performance.")]
        [SerializeField]
        private GameObject rightController;

        [SerializeField]
        private GameObject LeftControllerScriptAlias;

        [SerializeField]
        private GameObject RightControllerScriptAlias;

        [Header("Player Section")]

        [Tooltip("The prefab to use for representing the player")]
        [SerializeField]
        private GameObject playerPrefab;


        [SerializeField]
        private GameObject[] MUVRTK_InteractionElements;

        [SerializeField]
        private string gameVersion;


        #endregion

        #region Private Fields

        private bool cameraLoaded;
        private bool modelLoaded;
        private Camera mainCamera;
        private GameObject instantiatedPlayer;
        private GameObject networkedLeftControllerModel;
        private GameObject networkedRightControllerModel;

        #endregion

        #region MonoBehaviour Callbacks

        void Awake()
        {
            // #Critical
            // this makes sure we can use PhotonNetwork.LoadLevel() on the master client and all clients in the same room sync their level automatically
            PhotonNetwork.AutomaticallySyncScene = true;
        }

        private void Start()
        {
            if (!PhotonNetwork.IsConnected)
            {
                // #Critical, we must first and foremost connect to Photon Online Server.
                PhotonNetwork.GameVersion = gameVersion;
                PhotonNetwork.ConnectUsingSettings();
            }
        }

        private void Update()
        {
            if (!cameraLoaded)
            {
                if (Camera.main != null)
                {
                    mainCamera = Camera.main;
                    if (instantiatedPlayer != null)
                    {
                        if (debug)
                            Debug.Log("Setting Player Parent Object to Camera");

                        instantiatedPlayer.transform.parent = mainCamera.transform;

                        //resetting the Playerprefabs transform values to center it on the camera
                        instantiatedPlayer.transform.localPosition = new Vector3(0, 0, 0);
                        instantiatedPlayer.transform.localRotation = Quaternion.Euler(0, 0, 0);
                        cameraLoaded = true;
                    }
                    if (PhotonNetwork.InRoom && instantiatedPlayer == null)
                    {
                        if (debug)
                            Debug.Log("MUVRTK_GameManager: Update()-Method didn't find any Player, so InstantiatePlayer() was called.");
                        InstantiatePlayerOverNetwork();

                    }

                }
            }

            if (!modelLoaded)
            {
                // deactivate default Controller Model body Meshes in Hierarchy

                if (controllerModels.Length == 0)
                {
                    controllerModels = new GameObject[2];

                    if (debug)
                        Debug.Log(name + "No Controller Models set in Editor, creating new GameObject Array.");

                    if (leftController != null)
                    {
                        controllerModels[0] = leftController.transform.GetChild(0).gameObject;
                    }

                    if (rightController != null)
                    {
                        controllerModels[1] = rightController.transform.GetChild(0).gameObject;
                    }
                }
                else
                {
                    if (controllerModels[0].transform.childCount > 0 )
                    {
                        foreach (GameObject model in controllerModels)
                        {
                            model.transform.GetChild(1).GetComponent<MeshRenderer>().enabled = false;

                            if (debug)
                                Debug.Log(name + "Default Controller " + model.transform.GetChild(1).name + " deactivated");
                        }

                        modelLoaded = true;
                    }
                    
                }
            }
        }


        #endregion

        #region Photon Callbacks

        public override void OnConnectedToMaster()
        {
            PhotonNetwork.JoinRandomRoom();
        }

        public override void OnJoinedRoom()
        {
            if (debug)
                Debug.Log("MUVRTK_GameManager: OnJoinedRoom() called by PUN. Now this client is in the Room.");

            InstantiatePlayerOverNetwork();

            InstantiateInteractiveElements();


            // if the User chooses to show the Player Controllers via network, this method deactivates the standard models and replaces them by networked ones.
            if (showControllersOfOtherPlayers)
            {
                InstantiateNetworkedControllerModels();
            }

            //TODO: Generalize this shit!!!
            RightControllerScriptAlias.GetComponent<MUVRTK_SetViewIDAtRuntime>().AddPhotonView();
            LeftControllerScriptAlias.GetComponent<MUVRTK_SetViewIDAtRuntime>().AddPhotonView();

        }

        /// <summary>
        /// Called when the local player left the room. We need to load the launcher scene.
        /// </summary>
        public override void OnLeftRoom()
        {
            PhotonNetwork.LoadLevel("01 - Lobby");

            if (debug)
                Debug.Log("MUVRTK_GameManager: OnLeftRoom() called by PUN. Now this client is in the lobby.");

            PhotonNetwork.DestroyPlayerObjects(PhotonNetwork.LocalPlayer);
        }

        public override void OnPlayerEnteredRoom(Player other)
        {
            if (debug)
                Debug.LogFormat("OnPlayerEnteredRoom() {0}", other.NickName); // not seen if you're the player connecting

            if (PhotonNetwork.IsMasterClient)
            {
                if (debug)
                    Debug.LogFormat(this.name + "OnPlayerEnteredRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient); // called before OnPlayerLeftRoom

            }
        }


        public override void OnPlayerLeftRoom(Player other)
        {
            if (debug)
                Debug.LogFormat("OnPlayerLeftRoom() {0}", other.NickName); // seen when other disconnects

            if (PhotonNetwork.IsMasterClient)
            {
                if (debug)
                    Debug.LogFormat(this.name + "OnPlayerLeftRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient); // called before OnPlayerLeftRoom

            }
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            if (debug)
                Debug.LogWarningFormat("MUVRTK_GameManager: OnDisconnected() was called by PUN with reason {0}", cause);
        }

        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            if (debug)
                Debug.Log("MUVRTK_GameManager:OnJoinRandomFailed() was called by PUN. No random room available, so we create one.\nCalling: PhotonNetwork.CreateRoom");

            // #Critical: we failed to join a random room, maybe none exists or they are all full. No worries, we create a new room.
            PhotonNetwork.CreateRoom(null, new RoomOptions(), TypedLobby.Default);
        }

        #endregion



        #region Private Methods


        void LoadArena()
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                Debug.LogError("PhotonNetwork : Trying to Load a level but we are not the master Client");
            }
            if (debug)
                Debug.Log(this.name + " PhotonNetwork : Loading Common Room");

            PhotonNetwork.LoadLevel("02 - Common Room");

        }

        void InstantiatePlayerOverNetwork()
        {
            /// Player Instantiation
            if (playerPrefab == null)
            {
                Debug.LogError("<Color=Red><a>Missing</a></Color> playerPrefab Reference. Please set it up in GameObject 'Game Manager'", this);
            }
            else
            {
                Debug.LogFormat("We are Instantiating LocalPlayer from {0}", Application.loadedLevelName);
                // we're in a room. spawn a character for the local player. it gets synced by using PhotonNetwork.Instantiate

                instantiatedPlayer = PhotonNetwork.Instantiate(playerPrefab.name, new Vector3(0,0,0), Quaternion.Euler(0, 0, 0), 0);

            }

        }

        void InstantiateInteractiveElements()
        {
            foreach(GameObject go in MUVRTK_InteractionElements)
            {
                PhotonNetwork.Instantiate(go.name, new Vector3(0, 0, 0), Quaternion.Euler(0, 0, 0), 0);
            }
        }



        /// <summary>
        /// Deactivates the current Controller Models and replaces them by Networked ones.
        /// </summary>
        void InstantiateNetworkedControllerModels()
        {
            //Get Controller GameObjects in Hierarchy to add the new Networked Models onto them.
            if(leftController == null)
            {
                 leftController = GameObject.Find("Controller (left)");
            }
            

            if (rightController == null)
            {
                rightController = GameObject.Find("Controller (right)");
            }


            // Instantiate Networked Models
            networkedLeftControllerModel = PhotonNetwork.Instantiate("Controller_body", new Vector3(0, 0, 0), Quaternion.identity);
            networkedRightControllerModel = PhotonNetwork.Instantiate("Controller_body", new Vector3(0, 0, 0), Quaternion.identity);

            // bind their movement to the Controllers by making them their parents and resetting their local transform values.

            //left
            if(leftController != null)
            {
                networkedLeftControllerModel.transform.parent = leftController.transform;
                networkedLeftControllerModel.transform.localPosition = new Vector3(0, 0, 0);
                networkedLeftControllerModel.transform.localRotation = Quaternion.Euler(0, 180, 0);

                if (debug)
                    Debug.Log(name + "Left Controller Model Instantiation Finished!");
            }
            else
            {
                if (debug)
                    Debug.Log(this.name + "No Left Controller found!");
            }

            //right
            if (rightController != null)
            {
                networkedRightControllerModel.transform.parent = rightController.transform;
                networkedRightControllerModel.transform.localPosition = new Vector3(0, 0, 0);
                networkedRightControllerModel.transform.localRotation = Quaternion.Euler(0, 180, 0);



                if (debug)
                    Debug.Log(name + "Right Controller Model Instantiation Finished!");
            }
            else
            {
                if (debug)
                    Debug.Log(this.name + "Not Right Controller found!");
            }

        }




        #endregion

        #region Public Methods


        public void LeaveRoom()
        {
            PhotonNetwork.LeaveRoom();
        }


        #endregion

    }
}

