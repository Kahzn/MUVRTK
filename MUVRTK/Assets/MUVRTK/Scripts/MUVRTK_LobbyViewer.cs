
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
        #region Public Fields

        public Text lobbyOutput;

        #endregion

        

        #region Monobehaviour Callbacks


        #endregion

        #region Photon Callbacks
        
        public override void OnRoomListUpdate(List<RoomInfo> roomList)
        {
            if (roomList.Count == 0)
            {
                lobbyOutput.text = "No Rooms available at the moment";
            }
            if(roomList.Count == 1)
            {
                Debug.Log("MUVRTK_LobbyViewer Rooms Count : " + roomList.Count);
                lobbyOutput.text = "";
                lobbyOutput.text += roomList.First().Name + " " + roomList.First().PlayerCount;
            }
            if(roomList.Count> 1)
            {
                Debug.Log("MUVRTK_LobbyViewer Rooms Number : " + roomList.Count);
                foreach (RoomInfo ri in roomList)
                {
                    lobbyOutput.text += "\n" + ri.Name + " " + ri.PlayerCount;
                }
            }
            
            
        }

        #endregion
    }

}
