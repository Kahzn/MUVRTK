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

            MyCustomInteractableObjectEventArgs.Register();
            MyCustomInteractableObject.Register();

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


            pv.RPC("TouchHighlightObject_RPC", RpcTarget.All, pv.ViewID, sender, e);
        }

        private void Networked_TouchUnHighlightObjectRPC(object sender, InteractableObjectEventArgs e)
        {
            Debug.Log("Networked_TouchUnhighlightObject passed");


            pv.RPC("TouchUnHighlightObject_RPC", RpcTarget.All, pv.ViewID, sender, e);
        }
        #endregion

        #region RPCs
        /**
         *  All methods in this region are RPC-mediators in order to call the same method on all client machines in a network.
         * */

        [PunRPC]
        private void TouchHighlightObject_RPC(int viewID, object sender, InteractableObjectEventArgs e)
        {
            Debug.Log("TouchHighlighObject_RPC passed");


            if(this.pv.ViewID.Equals(viewID))
                TouchHighlightObject(sender, e); 
            
        }

        [PunRPC]
        private void TouchUnHighlightObject_RPC(int viewID, object sender, InteractableObjectEventArgs e)
        {
            Debug.Log("TouchUnHighlighObject_RPC passed");

            if (this.pv.ViewID.Equals(viewID)) 
                TouchUnHighlightObject(sender, e);
        }
        #endregion

        #region private Helper Classes



        #endregion


    }

    internal class MyCustomInteractableObjectEventArgs 
    {
       

        internal static void Register()
        {
            Debug.Log("MyCustomInteractableObjectEventArgs registration completed: " + PhotonPeer.RegisterType(typeof(InteractableObjectEventArgs), (byte) 'I', Serialize, Deserialize));
        }

        #region Custom De/Serializer Methods


        public byte Id { get; set; }
        public InteractableObjectEventArgs args;

        public static object Deserialize(byte[] data)
        {
            var result = new MyCustomInteractableObjectEventArgs();
            result.Id = data[0];
            return result;
        }

        public static byte[] Serialize(object customType)
        {

            var c = new MyCustomInteractableObjectEventArgs();
            c.args = (InteractableObjectEventArgs)customType;
            return new byte[] { c.Id };
        }
        #endregion
    }

    internal class MyCustomInteractableObject
    {

        internal static void Register()
        {
            Debug.Log("MyCustomInteractableObject registration completed: " + PhotonPeer.RegisterType(typeof(VRTK_InteractableObject), (byte)'J', Serialize, Deserialize));
        }

        #region Custom De/Serializer Methods


        public byte Id { get; set; }
        public VRTK_InteractableObject interactable;

        public static object Deserialize(byte[] data)
        {
            var result = new MyCustomInteractableObject();
            result.Id = data[0];
            return result;
        }

        public static byte[] Serialize(object customType)
        {

            var c = new MyCustomInteractableObject();
            c.interactable= (VRTK_InteractableObject)customType;
            return new byte[] { c.Id };
        }
        #endregion
    }

}


