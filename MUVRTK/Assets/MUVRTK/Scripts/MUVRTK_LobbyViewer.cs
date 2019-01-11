
using System.Linq;

namespace MUVRTK
{
    using System.Collections;
    using UnityEngine;
    using UnityEngine.UI;
    using Photon.Pun;
    using Photon.Realtime;
    using System.Collections.Generic;

    public class MUVRTK_LobbyViewer : MonoBehaviourPunCallbacks
    {
        #region Private Fields
        [Tooltip("Add a UI-Text-Component from a Scroll View here. ")]
        [SerializeField]
        private Text lobbyOutput;

        [SerializeField]
        private bool debug;

        #endregion

      

        #region Photon Callbacks
        
        public override void OnRoomListUpdate(List<RoomInfo> roomList)
        {
            if (roomList.Count == 0)
            {
                lobbyOutput.text = "No Rooms available at the moment";
            }

            if(roomList.Count > 0 )
            {
                if (debug)
                {
                    Debug.Log("MUVRTK_LobbyViewer Rooms Count : " + roomList.Count);
                    Debug.Log("Number of Rooms from Statistics:" + PhotonNetwork.CountOfRooms);
                    Debug.Log("Number of Players on Master: " + PhotonNetwork.CountOfPlayersOnMaster);
                    Debug.Log("Number of Players in Rooms: " + PhotonNetwork.CountOfPlayersInRooms);
                    Debug.Log("Total number of Players: " + PhotonNetwork.CountOfPlayers);
                    Debug.Log("Current Lobby name: " + PhotonNetwork.CurrentLobby.Name);
                }
                
                lobbyOutput.text = "";
                foreach (RoomInfo ri in roomList)
                {
                    lobbyOutput.text += "\n" + ri.Name + " " + ri.PlayerCount;
                }
            }
            
            
        }

        #endregion
    }

}
