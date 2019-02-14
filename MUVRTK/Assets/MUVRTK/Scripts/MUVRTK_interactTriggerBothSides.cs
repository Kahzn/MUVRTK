namespace MUVRTK
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using VRTK;
    using Photon.Pun;

    public class MUVRTK_interactTriggerBothSides : MUVRTK_InteractHaptics
    {

        void Start()
        {
            triggerSenderAndReceiver = true;
        }
        private void OnEnable()
        {
            triggerSenderAndReceiver = true;
        }
    }
}


