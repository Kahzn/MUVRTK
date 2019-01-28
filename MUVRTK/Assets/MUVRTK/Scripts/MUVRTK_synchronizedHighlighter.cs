using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using VRTK;
using VRTK.Highlighters;

namespace MUVRTK
{
    [RequireComponent(typeof(PhotonView))]

    public class MUVRTK_synchronizedHighlighter : VRTK_InteractObjectHighlighter
    {
        #region Private Serialized Fields
        [SerializeField] private bool debug;
        
        #endregion

        #region private fields

        private PhotonView pv;

        #endregion

        #region Monobehaviour Callbacks

        private void Start()
        {
            // Get PhotonView Component. Necessary for RPCs.
            pv = PhotonView.Get(this);
        }

        #endregion

        #region VRTK_InteractObjectHighlighter Overrides

        protected override bool SetupListeners(bool throwError)
        {
            objectToMonitor = (objectToMonitor != null ? objectToMonitor : GetComponentInParent<VRTK_InteractableObject>());
            if (objectToMonitor != null)
            {
                objectToMonitor.SubscribeToInteractionEvent(VRTK_InteractableObject.InteractionType.NearTouch, NearTouchHighlightObject);
                objectToMonitor.SubscribeToInteractionEvent(VRTK_InteractableObject.InteractionType.NearUntouch, NearTouchUnHighlightObject);

                objectToMonitor.SubscribeToInteractionEvent(VRTK_InteractableObject.InteractionType.Touch, Networked_TouchHighlightObject);
                objectToMonitor.SubscribeToInteractionEvent(VRTK_InteractableObject.InteractionType.Untouch, Networked_TouchUnHighlightObjectRPC);

                objectToMonitor.SubscribeToInteractionEvent(VRTK_InteractableObject.InteractionType.Grab, GrabHighlightObject);
                objectToMonitor.SubscribeToInteractionEvent(VRTK_InteractableObject.InteractionType.Ungrab, GrabUnHighlightObject);

                objectToMonitor.SubscribeToInteractionEvent(VRTK_InteractableObject.InteractionType.Use, UseHighlightObject);
                objectToMonitor.SubscribeToInteractionEvent(VRTK_InteractableObject.InteractionType.Unuse, UseUnHighlightObject);
                return true;
            }
            else if (throwError)
            {
                VRTK_Logger.Error(VRTK_Logger.GetCommonMessage(VRTK_Logger.CommonMessageKeys.REQUIRED_COMPONENT_MISSING_FROM_GAMEOBJECT, "VRTK_InteractObjectHighlighter", "VRTK_InteractableObject", "the same or parent"));
            }
            return false;
        }

        protected override void TearDownListeners()
        {
            if (objectToMonitor != null)
            {
                objectToMonitor.UnsubscribeFromInteractionEvent(VRTK_InteractableObject.InteractionType.NearTouch, NearTouchHighlightObject);
                objectToMonitor.UnsubscribeFromInteractionEvent(VRTK_InteractableObject.InteractionType.NearUntouch, NearTouchUnHighlightObject);

                objectToMonitor.UnsubscribeFromInteractionEvent(VRTK_InteractableObject.InteractionType.Touch, Networked_TouchHighlightObject);
                objectToMonitor.UnsubscribeFromInteractionEvent(VRTK_InteractableObject.InteractionType.Untouch, Networked_TouchUnHighlightObjectRPC);

                objectToMonitor.UnsubscribeFromInteractionEvent(VRTK_InteractableObject.InteractionType.Grab, GrabHighlightObject);
                objectToMonitor.UnsubscribeFromInteractionEvent(VRTK_InteractableObject.InteractionType.Ungrab, GrabUnHighlightObject);

                objectToMonitor.UnsubscribeFromInteractionEvent(VRTK_InteractableObject.InteractionType.Use, UseHighlightObject);
                objectToMonitor.UnsubscribeFromInteractionEvent(VRTK_InteractableObject.InteractionType.Unuse, UseUnHighlightObject);
            }
        }

        #endregion

        #region Networked Methods

        /**
         * this whole Block contains the mediating methods that invoke the VRTK-Highlighting via RPC.
         * and yes, this is necessary (I've spent 6 hours coming to that conclusion. Prove me wrong).
         * */


        private void Networked_TouchHighlightObject(object sender, InteractableObjectEventArgs e)
        {
            Debug.Log("Networked_TouchHighlightObject passed");
            pv.RPC("TouchHighlightObject_RPC", RpcTarget.All, sender, e);
        }

        private void Networked_TouchUnHighlightObjectRPC(object sender, InteractableObjectEventArgs e)
        {
            Debug.Log("Networked_TouchUnhighlightObject passed");
            pv.RPC("TouchUnhighlightObject_RPC", RpcTarget.All, sender, e);
        }
        #endregion

        #region RPCs
        /**
         *  All methods in this region are RPC-mediators in order to call the same method on all client machines in a network.
         * */

        [PunRPC]
        private void TouchHighlightObject_RPC(object sender, InteractableObjectEventArgs e)
        {
            Debug.Log("TouchHighlighObject_RPC passed");
            TouchHighlightObject(sender, e);
        }

        [PunRPC]
        private void TouchUnHighlightObject_RPC(object sender, InteractableObjectEventArgs e)
        {
            Debug.Log("TouchUnhighlighObject_RPC passed");
            TouchUnHighlightObject(sender, e);
        }
        #endregion


    }
}


