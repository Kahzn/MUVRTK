

namespace MUVRTK
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using Photon.Pun;
    using Photon.Realtime;


    public class MUVRTK_Launcher : MonoBehaviourPunCallbacks
    {
        #region Public Fields

        public bool debug;

        public string sceneName;

        #endregion


        #region Private Serializable Fields

        /// <summary>
        /// The maximum number of players per room. When a room is full, it can't be joined by new players, and so new room will be created.
        /// </summary>
        [Tooltip("The maximum number of players per room. When a room is full, it can't be joined by new players, and so new room will be created")]
        [SerializeField]
        private byte maxPlayersPerRoom = 4;

        [Tooltip("The Ui Panel to let the user enter name, connect and play")]
        [SerializeField]
        private GameObject controlPanel;
        [Tooltip("The UI Label to inform the user that the connection is in progress")]
        [SerializeField]
        private GameObject progressLabel;
        [Tooltip("The UI input field for creating a new room")]
        [SerializeField]
        private GameObject enterRoomName;
        [Tooltip("The viewport for the room lobby and UI input field for joining a room")]
        [SerializeField]
        private GameObject joinRoomPanel;

        #endregion

        #region Private Fields

        /// <summary>
        /// Keep track of the current process. Since connection is asynchronous and is based on several callbacks from Photon,
        /// we need to keep track of this to properly adjust the behavior when we receive call back by Photon.
        /// Typically this is used for the OnConnectedToMaster() callback.
        /// </summary>
        bool isConnectingToRoom;


        bool joinRandomRoom = true;


        private string roomName;

        /// <summary>
        /// This client's version number. Users are separated from each other by gameVersion (which allows you to make breaking changes).
        /// </summary>
        string gameVersion = "1";


        #endregion

        #region MonoBehaviour Callbacks

        void Awake()
        {
            // #Critical
            // this makes sure we can use PhotonNetwork.LoadLevel() on the master client and all clients in the same room sync their level automatically
            PhotonNetwork.AutomaticallySyncScene = true;
        }

        /// <summary>
        /// MonoBehaviour method called on GameObject by Unity during initialization phase.
        /// </summary>
        void Start()
        {

            ControlUI();
            ConnectToMaster();

        }

        #endregion

        #region MonobehaviourPunCallbacks Callbacks

        public override void OnConnectedToMaster()
        {
            if(debug)
            Debug.Log("MUVRTK_Launcher: OnConnectedToMaster() was called by PUN");

            if (isConnectingToRoom)
            {
                // #Critical: The first we try to do is to join a potential existing room. If there is, good, else, we'll be called back with OnJoinRandomFailed()
                // PhotonNetwork.JoinRandomRoom();
                ConnectToRoom();
            }
            else
            {
                PhotonNetwork.JoinLobby();
                if(debug)
                    Debug.Log("MUVRTK_Launcher: JoinLobby() was called by PUN");
            }
            

        }


        public override void OnDisconnected(DisconnectCause cause)
        {

            progressLabel.SetActive(false);
            controlPanel.SetActive(true);
            enterRoomName.SetActive(false);
            joinRoomPanel.SetActive(false);

            if (debug)
            Debug.LogWarningFormat("MUVRTK_Launcher: OnDisconnected() was called by PUN with reason {0}", cause);
        }

        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            if(debug)
            Debug.Log("MUVRTK_Launcher:OnJoinRandomFailed() was called by PUN. No random room available, so we create one.\nCalling: PhotonNetwork.CreateRoom");

            // #Critical: we failed to join a random room, maybe none exists or they are all full. No worries, we create a new room.
            PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = maxPlayersPerRoom }, TypedLobby.Default);
        }

        public override void OnJoinedRoom()
        {

            // #Critical
            // Load the Room Level.
            PhotonNetwork.LoadLevel(sceneName);

            if(debug)
            Debug.Log("MUVRTK_Launcher: OnJoinedRoom() called by PUN. Now this client is in a room.");
        }


        #endregion

        #region Public Methods

        public void ConnectToRoom()
        {
            isConnectingToRoom = true;

            progressLabel.SetActive(true);
            controlPanel.SetActive(false);
            enterRoomName.SetActive(false);
            joinRoomPanel.SetActive(false);

            // we check if we are connected or not, we join if we are , else we initiate the connection to the server.
            if (PhotonNetwork.IsConnected)
            {
                // #Critical we need at this point to attempt joining a Random Room. If it fails, we'll get notified in OnJoinRandomFailed() and we'll create one.

                if (joinRandomRoom)
                {
                    PhotonNetwork.JoinRandomRoom();
                    if (debug)
                        Debug.Log("MUVRTK_Launcher: JoinRandomRoom() called by PUN.");

                }

                else
                {
                    PhotonNetwork.JoinOrCreateRoom(roomName, new RoomOptions(), TypedLobby.Default);

                    if (debug)
                        Debug.Log("MUVRTK_Launcher: JoinOrCreateRoom() called by PUN with Room Name: " + roomName);
                }
                
                
            }
            else
            {
               ConnectToMaster();
            }
        }

        public void ConnectToMaster()
        {
            if (!PhotonNetwork.IsConnected)
            {
                // #Critical, we must first and foremost connect to Photon Online Server.
                PhotonNetwork.GameVersion = gameVersion;
                PhotonNetwork.ConnectUsingSettings();
            }
            else
            {
                PhotonNetwork.JoinLobby();
            }
        }

        public void SetRoomName(string value)
        {
            roomName = value;
        }
        
        /// <summary>
        /// Called when the CreateRoomPanel is toggled. Open
        /// </summary>
        public void CreateRoomUI()
        {
            progressLabel.SetActive(false);
            controlPanel.SetActive(false);
            enterRoomName.SetActive(true);
            joinRoomPanel.SetActive(false);

            joinRandomRoom = false;
        }
        
        /// <summary>
        /// Called when the JoinRoomPanel is toggled. Opens the LobbyView.
        /// </summary>
        public void JoinRoomUI()
        {
            progressLabel.SetActive(false);
            controlPanel.SetActive(false);
            enterRoomName.SetActive(false);
            joinRoomPanel.SetActive(true);

            joinRandomRoom = false;
        }

        /// <summary>
        /// Called when the Control Panel is toggled.
        /// </summary>
        public void ControlUI()
        {
            progressLabel.SetActive(false);
            controlPanel.SetActive(true);
            enterRoomName.SetActive(false);
            joinRoomPanel.SetActive(false);

            joinRandomRoom = true;
        }

        #endregion
    }
}

