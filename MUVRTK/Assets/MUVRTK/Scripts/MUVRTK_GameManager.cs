namespace MUVRTK
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.SceneManagement;
    using Photon.Pun;
    using Photon.Realtime;


    public class MUVRTK_GameManager : MonoBehaviourPunCallbacks
    {
        #region Public Fields

        public bool debug;

        [Tooltip("The prefab to use for representing the player")]
        public GameObject playerPrefab;


        #endregion

        #region Private Fields

        private bool cameraLoaded;
        private Camera mainCamera;
        private GameObject instantiatedPlayer;

        #endregion

        #region Photon Callbacks


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


                LoadArena();
            }
        }


        public override void OnPlayerLeftRoom(Player other)
        {
            if(debug)
            Debug.LogFormat("OnPlayerLeftRoom() {0}", other.NickName); // seen when other disconnects

            if (PhotonNetwork.IsMasterClient)
            {
                if (debug)
                    Debug.LogFormat(this.name + "OnPlayerLeftRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient); // called before OnPlayerLeftRoom


                LoadArena();
            }
        }

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
            if (playerPrefab == null)
            {
                Debug.LogError("<Color=Red><a>Missing</a></Color> playerPrefab Reference. Please set it up in GameObject 'Game Manager'", this);
            }
            else
            {
                Debug.LogFormat("We are Instantiating LocalPlayer from {0}", Application.loadedLevelName);
                // we're in a room. spawn a character for the local player. it gets synced by using PhotonNetwork.Instantiate
                if (mainCamera != null)
                {
                   instantiatedPlayer = PhotonNetwork.Instantiate(playerPrefab.name, mainCamera.transform.position, mainCamera.transform.rotation, 0);
                }
                else
                {
                    instantiatedPlayer = PhotonNetwork.Instantiate(playerPrefab.name, new Vector3(0,0,0), Quaternion.identity, 0);
                }


            }
        }

        private void Update()
        {
            if (!cameraLoaded)
            {
                if(mainCamera == null && Camera.main != null)
                {
                    mainCamera = Camera.main;
                    instantiatedPlayer.transform.parent = mainCamera.transform;
                    cameraLoaded = true;
                }
            }
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



        #endregion

        #region Public Methods


        public void LeaveRoom()
        {
            PhotonNetwork.LeaveRoom();
        }


        #endregion

    }
}

