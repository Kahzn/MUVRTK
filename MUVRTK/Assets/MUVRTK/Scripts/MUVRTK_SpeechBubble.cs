
namespace MUVRTK
{
    using Photon.Pun;
    using System.Collections;
    using System.Collections.Generic;
    using TMPro;
    using UnityEngine;
    using VRTK;

    /// <summary>
    /// manages the content and behaviour of Speech Bubble Objects.
    /// <para>Created by Katharina Ziolkowski, 2019-02-07</para>
    /// </summary>
    public class MUVRTK_SpeechBubble : MUVRTK_SceneObject
    {
        #region private Serializable Fields

        [Tooltip("Text in the Speechbubble-Content.")]
        public TextMeshPro content;

        #endregion

        #region Monobehaviour Callbacks
        // Start is called before the first frame update
        void Start()
        {

            lifetimeInSeconds = 10;
            destroyInteraction = DestroyInteractions.Touch;

        }

        #endregion

        #region Public Methods

        public void SetText(string text)
        {
            content.text = text;
        }

        #endregion

        #region PUN RPCs


        [PunRPC]
        void BroadcastHapticPulseOnViewID(int viewIdOfController, float vibrationStrength, float duration, float pulseInterval)
        {
            VRTK_ControllerHaptics.TriggerHapticPulse(VRTK_ControllerReference.GetControllerReference(PhotonView.Find(viewIdOfController).gameObject), vibrationStrength, duration, pulseInterval);
        }

        #endregion
    }
}

