using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

namespace MUVRTK
{
    /// <summary>
    /// This class fixes the synchronization issue when Grabbing objects in VRTK by requesting ownership of the item On Grab.
    /// Just add this script to your interactable object and make sure it has got a <see cref="VRTK_InteractableObject"/> and <see cref="PhotonView"/>-component attached to it.
    /// <para> Created by Katharina Ziolkowski, 2020-01-04</para>
    /// </summary>
    /// 
    [RequireComponent(typeof(PhotonView))]
    [RequireComponent(typeof(VRTK_InteractableObject))]
    public class MUVRTK_NetworkGrab : MonoBehaviour
    {
        private PhotonView photonView;
        private VRTK_InteractableObject interactableObject;


        // Start is called before the first frame update
        void Start()
        {
            photonView = GetComponent<PhotonView>();
            interactableObject = GetComponent<VRTK_InteractableObject>();

            interactableObject.SubscribeToInteractionEvent(VRTK_InteractableObject.InteractionType.Grab, GetOwnership);

        }

        private void GetOwnership(object sender, InteractableObjectEventArgs e)
        {
            photonView.TransferOwnership(PhotonNetwork.LocalPlayer);
        }

    }
}

