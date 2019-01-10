
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
            if (roomList.Count.Equals(0))
            {
                lobbyOutput.text = "No Rooms available at the moment";
            }
            else if(roomList.Count.Equals(1))
            {
                Debug.Log("room length : " + roomList.Count);
                lobbyOutput.text = "";
                lobbyOutput.text += roomList.First().Name + " " + roomList.First().PlayerCount;
            }
            else
            {
                Debug.Log("room length : " + roomList.Count);
                foreach (RoomInfo ri in roomList)
                {
                    lobbyOutput.text += "\n" + ri.Name + " " + ri.PlayerCount;
                }
            }
            
            
        }

        #endregion
    }

}
