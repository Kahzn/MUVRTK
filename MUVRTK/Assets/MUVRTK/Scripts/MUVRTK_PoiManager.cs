namespace MUVRTK
{
    using Photon.Pun;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using VRTK;

    /// <summary>
    /// Manages the lifespan and Behaviour of the POI.
    /// The parent class MUVRTK_SceneObject contains different actions that can be triggered on object spawn and manages the destruction of the object.
    /// <para>Created by Katharina Ziolkowski, 2019-02-04</para>
    /// </summary>
    public class MUVRTK_PoiManager : MUVRTK_SceneObject
    {


        private MeshRenderer meshRenderer;

        private void Start(){

            meshRenderer = GetComponent<MeshRenderer>();

        }

        #region PUN RPCs

    
        [PunRPC]
        void BroadcastHapticPulseOnViewID(int viewIdOfController, float vibrationStrength, float duration, float pulseInterval)
        {
            VRTK_ControllerHaptics.TriggerHapticPulse(VRTK_ControllerReference.GetControllerReference(PhotonView.Find(viewIdOfController).gameObject), vibrationStrength, duration, pulseInterval);
        }

        #endregion


    }

}


