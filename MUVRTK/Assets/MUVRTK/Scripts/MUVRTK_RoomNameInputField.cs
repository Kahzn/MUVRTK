
namespace MUVRTK
{
    using System.Collections;
    using UnityEngine;
    using UnityEngine.UI;
    using Photon.Pun;
    using Photon.Realtime;

    /// <summary>
    /// Manages the transition to specific rooms on the network.
    /// <para>Created by Katharina Ziolkowski, 2019-01-03</para>
    /// </summary>
    [RequireComponent(typeof(InputField))]
    public class MUVRTK_RoomNameInputField : MonoBehaviour
    {

        #region Private Constants

        // Store the PlayerPref Key to avoid typos
        const string roomNamePrefKey = "RoomName";


        #endregion

        #region Public Fields

        public MUVRTK_Launcher launcher;

        #endregion

        #region MonoBehaviour CallBacks


        /// <summary>
        /// MonoBehaviour method called on GameObject by Unity during initialization phase.
        /// </summary>
        void Start()
        {


            string defaultName = string.Empty;
            InputField _inputField = this.GetComponent<InputField>();
            if (_inputField != null)
            {
                if (PlayerPrefs.HasKey(roomNamePrefKey))
                {
                    defaultName = PlayerPrefs.GetString(roomNamePrefKey);
                    _inputField.text = defaultName;
                }
            }


            launcher.SetRoomName(defaultName);
        }


        #endregion


        #region Public Methods


        /// <summary>
        /// Sets the name of the player, and save it in the PlayerPrefs for future sessions.
        /// </summary>
        /// <param name="value">The name of the Player</param>
        public void SetRoomName(string value)
        {
            // #Important
            if (string.IsNullOrEmpty(value))
            {
                Debug.LogError("Room Name is null or empty");
                return;
            }
            launcher.SetRoomName(value);


            PlayerPrefs.SetString(roomNamePrefKey, value);
        }


        #endregion
    }
}


