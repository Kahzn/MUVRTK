using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

namespace MUVRTK
{
    public class MUVRTK_GameManager_base : MonoBehaviourPunCallbacks
    {
        
       #region Private Serialize Fields
        [Tooltip("Log the debug messages for this script in the console.")]
        [SerializeField]
        private bool debug;
        
        [SerializeField]
        private string gameVersion;

        [SerializeField] 
        private MUVRTK_Instantiate instantiator;
        
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
            else
            {
                if(instantiator)
                    instantiator.Instantiate_GameObjects();
                else
                {
                    if (debug)
                        Debug.Log(name + " : No Instantiator found!");
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
                Debug.Log(name + ": OnJoinedRoom() called by PUN. Now this client is in the Room.");
            if(instantiator)
                instantiator.Instantiate_GameObjects();
            else
            {
                if (debug)
                    Debug.Log(name + " : No Instantiator found!");
            }
        }
        
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
        
        #region Public Methods


        public void LeaveRoom()
        {
            PhotonNetwork.LeaveRoom();
        }


        #endregion
        
    } 

}

