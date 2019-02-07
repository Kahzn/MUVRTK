using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using VRTK;
using VRTK.Highlighters;
using Photon.Realtime;
using ExitGames.Client.Photon;
using System.Text;

namespace MUVRTK
{
    /// <summary>
    /// This class manages the Highlighting of Interactable Objects and makes it visible via the network.
    /// When adding this script to a Prefab, please make sure to set the references in the following fields:
    /// - objects to monitor
    /// - objects to highlight
    /// 
    /// Additionally, make sure to add an entry to your prefabs photonview-component and set the reference to this script in it.
    /// 
    /// <para> Created by Katharina Ziolkowski,  2019-01-29</para>
    /// </summary>
    [RequireComponent(typeof(PhotonView))]
    [RequireComponent(typeof(VRTK_InteractableObject))]
    public class MUVRTK_InteractObjectHighlighter : VRTK_InteractObjectHighlighter, IPunObservable
    {
        #region Private Serialized Fields
        [SerializeField]
        private bool debug;

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

        /// <summary>
        /// Sets up the event listeners for the different VRTK-supported events.
        /// </summary>
        /// <param name="throwError"></param>
        /// <returns></returns>
        protected override bool SetupListeners(bool throwError)
        {
            objectToMonitor = (objectToMonitor != null ? objectToMonitor : GetComponentInParent<VRTK_InteractableObject>());
            if (objectToMonitor != null)
            {
                objectToMonitor.SubscribeToInteractionEvent(VRTK_InteractableObject.InteractionType.NearTouch, Networked_NearTouchHighlightObject);
                objectToMonitor.SubscribeToInteractionEvent(VRTK_InteractableObject.InteractionType.NearUntouch, Networked_UnHighlightObject);

                objectToMonitor.SubscribeToInteractionEvent(VRTK_InteractableObject.InteractionType.Touch, Networked_TouchHighlightObject);
                objectToMonitor.SubscribeToInteractionEvent(VRTK_InteractableObject.InteractionType.Untouch, Networked_UnHighlightObject);

                objectToMonitor.SubscribeToInteractionEvent(VRTK_InteractableObject.InteractionType.Grab, Networked_GrabHighlightObject);
                objectToMonitor.SubscribeToInteractionEvent(VRTK_InteractableObject.InteractionType.Ungrab, Networked_UnHighlightObject);

                objectToMonitor.SubscribeToInteractionEvent(VRTK_InteractableObject.InteractionType.Use, Networked_UseHighlightObject);
                objectToMonitor.SubscribeToInteractionEvent(VRTK_InteractableObject.InteractionType.Unuse, Networked_UnHighlightObject);
                return true;
            }
            else if (throwError)
            {
                VRTK_Logger.Error(VRTK_Logger.GetCommonMessage(VRTK_Logger.CommonMessageKeys.REQUIRED_COMPONENT_MISSING_FROM_GAMEOBJECT, "VRTK_InteractObjectHighlighter", "VRTK_InteractableObject", "the same or parent"));
            }
            return false;
        }

        /// <summary>
        /// Tears down the listeners when disabling the GameObject.
        /// </summary>
        protected override void TearDownListeners()
        {
            if (objectToMonitor != null)
            {
                objectToMonitor.UnsubscribeFromInteractionEvent(VRTK_InteractableObject.InteractionType.NearTouch, Networked_NearTouchHighlightObject);
                objectToMonitor.UnsubscribeFromInteractionEvent(VRTK_InteractableObject.InteractionType.NearUntouch, Networked_UnHighlightObject);

                objectToMonitor.UnsubscribeFromInteractionEvent(VRTK_InteractableObject.InteractionType.Touch, Networked_TouchHighlightObject);
                objectToMonitor.UnsubscribeFromInteractionEvent(VRTK_InteractableObject.InteractionType.Untouch, Networked_UnHighlightObject);

                objectToMonitor.UnsubscribeFromInteractionEvent(VRTK_InteractableObject.InteractionType.Grab, Networked_GrabHighlightObject);
                objectToMonitor.UnsubscribeFromInteractionEvent(VRTK_InteractableObject.InteractionType.Ungrab, Networked_UnHighlightObject);

                objectToMonitor.UnsubscribeFromInteractionEvent(VRTK_InteractableObject.InteractionType.Use, Networked_UseHighlightObject);
                objectToMonitor.UnsubscribeFromInteractionEvent(VRTK_InteractableObject.InteractionType.Unuse, Networked_UnHighlightObject);
            }
        }

        #endregion

        #region Networked Methods

        /**
         * this whole region contains the mediating methods that invoke the VRTK-Highlighting via RPC.
         * and yes, this is necessary (I've spent 6 hours coming to that conclusion. Prove me wrong).
         * */

        /// NEAR TOUCH
        
        private void Networked_NearTouchHighlightObject(object sender, InteractableObjectEventArgs e)
        {
            if(debug)
                Debug.Log("Networked_NearTouchHighlightObject passed");


            pv.RPC("NearTouchHighlightObject_RPC", RpcTarget.All, pv.ViewID);
        }



        /// TOUCH

        private void Networked_TouchHighlightObject(object sender, InteractableObjectEventArgs e)
        {
            if(debug)
                Debug.Log("Networked_TouchHighlightObject passed");


            pv.RPC("TouchHighlightObject_RPC", RpcTarget.All, pv.ViewID);
        }


        ///GRAB

        private void Networked_GrabHighlightObject(object sender, InteractableObjectEventArgs e)
        {
            if (debug)
                Debug.Log("Networked_GrabHighlightObject passed");



            pv.RPC("GrabHighlightObject_RPC", RpcTarget.All, pv.ViewID);
        }



        ///USE
        
        private void Networked_UseHighlightObject(object sender, InteractableObjectEventArgs e)
        {
            if (debug)
                Debug.Log("Networked_UseHighlightObject passed");


            pv.RPC("UseHighlightObject_RPC", RpcTarget.All, pv.ViewID);
        }


        // General Unhighlight
        private void Networked_UnHighlightObject(object sender, InteractableObjectEventArgs e)
        {
            if (debug)
                Debug.Log("Networked_UnhighlightObject passed");


            pv.RPC("UnHighlightObject_RPC", RpcTarget.All, pv.ViewID);
        }

        #endregion

        #region RPCs
        /**
         *  All methods in this region are RPC-mediators in order to call the same method on all client machines in a network.
         * 
         * */

        ///NEARTOUCH
        ///

        [PunRPC]
        private void NearTouchHighlightObject_RPC(int viewID)
        {
            if (debug)
                Debug.Log(name + ": New_NearTouchHighlighObject_RPC passed");


            if (pv.ViewID.Equals(viewID))
                Highlight(nearTouchHighlight);

        }


        ///TOUCH
        ///

        [PunRPC]
        private void TouchHighlightObject_RPC(int viewID)
        {
            if (debug)
                Debug.Log(name + ": New_TouchHighlighObject_RPC passed");


            if (pv.ViewID.Equals(viewID))
                Highlight(touchHighlight);

        }


        /// GRAB
        /// 
        [PunRPC]
        private void GrabHighlightObject_RPC(int viewID)
        {
            if (debug)
                Debug.Log(name + ": New_GrabHighlighObject_RPC passed");


            if (pv.ViewID.Equals(viewID))
                Highlight(grabHighlight);

        }
        
        /// USE


        [PunRPC]
        private void UseHighlightObject_RPC(int viewID)
        {
            if (debug)
                Debug.Log(name + ": New_GrabHighlighObject_RPC passed");


            if (pv.ViewID.Equals(viewID))
                Highlight(useHighlight);

        }

        // UNHIGHLIGHT FOR ALL

        [PunRPC]
        private void UnHighlightObject_RPC(int viewID)
        {
            if (debug)
                Debug.Log(name + ": UnHighlighObject_RPC passed");

            if (pv.ViewID.Equals(viewID))
            {
                Unhighlight();
            }
        }


            #endregion

            #region IPunObservable Implementation
            public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {

        }
        #endregion

    }


}


