
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

        // Start is called before the first frame update
        void Start()
        {
            lobbyOutput.text = "no Rooms available";
            Debug.Log(this + ": No Rooms available at the moment.");

        }
        


        #endregion

        #region Photon Callbacks
        
        public override void OnRoomListUpdate(List<RoomInfo> roomList)
        {
            Debug.Log("room length : " + roomList.Count);
            lobbyOutput.text += "\n Number of Rooms: " + roomList.Count;
        }

        #endregion
    }

}
