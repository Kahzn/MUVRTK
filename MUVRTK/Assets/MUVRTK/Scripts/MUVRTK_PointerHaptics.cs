namespace MUVRTK
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using VRTK;
    using Photon.Pun;

    /// <summary>
    /// This class takes an input from a Touch/Point-Interaction and lets the controllers of the player who has been touched vibrate.
    /// <para> Created by Katharina Ziolkowski, 2019-02-14</para>
    /// </summary>
    public class MUVRTK_PointerHaptics : MUVRTK_PlayerOwnedObject
    {

        #region PUN RPCs


        [PunRPC]
        void BroadcastHapticPulseOnViewID(int viewIdOfController, float vibrationStrength, float duration, float pulseInterval)
        {
            VRTK_ControllerHaptics.TriggerHapticPulse(VRTK_ControllerReference.GetControllerReference(PhotonView.Find(viewIdOfController).gameObject), vibrationStrength, duration, pulseInterval);

            Debug.Log(name + " : RPC called on this ViewID:" + viewIdOfController);
        }

        #endregion

    }
}

