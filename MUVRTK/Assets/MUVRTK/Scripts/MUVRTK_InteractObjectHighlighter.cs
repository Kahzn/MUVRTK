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


            // In order for the VRTK_Interactions to work via Network (as RPC-Calls), two new Custom Types had to be Created to support Serialization via Photon. 
            //You can find their respective definitions below in two internal classes.

            MyCustomInteractableObjectEventArgs.Register();
            MyCustomInteractableObject.Register();

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
                objectToMonitor.SubscribeToInteractionEvent(VRTK_InteractableObject.InteractionType.NearUntouch, Networked_NearTouchUnHighlightObject);

                objectToMonitor.SubscribeToInteractionEvent(VRTK_InteractableObject.InteractionType.Touch, Networked_TouchHighlightObject);
                objectToMonitor.SubscribeToInteractionEvent(VRTK_InteractableObject.InteractionType.Untouch, Networked_TouchUnHighlightObject);

                objectToMonitor.SubscribeToInteractionEvent(VRTK_InteractableObject.InteractionType.Grab, Networked_GrabHighlightObject);
                objectToMonitor.SubscribeToInteractionEvent(VRTK_InteractableObject.InteractionType.Ungrab, Networked_GrabUnHighlightObject);

                objectToMonitor.SubscribeToInteractionEvent(VRTK_InteractableObject.InteractionType.Use, Networked_UseHighlightObject);
                objectToMonitor.SubscribeToInteractionEvent(VRTK_InteractableObject.InteractionType.Unuse, Networked_UseUnHighlightObject);
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
                objectToMonitor.UnsubscribeFromInteractionEvent(VRTK_InteractableObject.InteractionType.NearUntouch, Networked_NearTouchUnHighlightObject);

                objectToMonitor.UnsubscribeFromInteractionEvent(VRTK_InteractableObject.InteractionType.Touch, Networked_TouchHighlightObject);
                objectToMonitor.UnsubscribeFromInteractionEvent(VRTK_InteractableObject.InteractionType.Untouch, Networked_TouchUnHighlightObject);

                objectToMonitor.UnsubscribeFromInteractionEvent(VRTK_InteractableObject.InteractionType.Grab, Networked_GrabHighlightObject);
                objectToMonitor.UnsubscribeFromInteractionEvent(VRTK_InteractableObject.InteractionType.Ungrab, Networked_GrabUnHighlightObject);

                objectToMonitor.UnsubscribeFromInteractionEvent(VRTK_InteractableObject.InteractionType.Use, Networked_UseHighlightObject);
                objectToMonitor.UnsubscribeFromInteractionEvent(VRTK_InteractableObject.InteractionType.Unuse, Networked_UseUnHighlightObject);
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


            pv.RPC("NearTouchHighlightObject_RPC", RpcTarget.All, pv.ViewID, sender, e);
        }

        private void Networked_NearTouchUnHighlightObject(object sender, InteractableObjectEventArgs e)
        {
            if (debug)
                Debug.Log("Networked_NearTouchUnhighlightObject passed");


            pv.RPC("NearTouchUnHighlightObject_RPC", RpcTarget.All, pv.ViewID, sender, e);
        }


        /// TOUCH

        private void Networked_TouchHighlightObject(object sender, InteractableObjectEventArgs e)
        {
            if(debug)
                Debug.Log("Networked_TouchHighlightObject passed");


            pv.RPC("TouchHighlightObject_RPC", RpcTarget.All, pv.ViewID, sender, e);
        }

        private void Networked_TouchUnHighlightObject(object sender, InteractableObjectEventArgs e)
        {
            if(debug)
                Debug.Log("Networked_TouchUnhighlightObject passed");


            pv.RPC("TouchUnHighlightObject_RPC", RpcTarget.All, pv.ViewID, sender, e);
        }

        ///GRAB

        private void Networked_GrabHighlightObject(object sender, InteractableObjectEventArgs e)
        {
            if (debug)
                Debug.Log("Networked_GrabHighlightObject passed");


            pv.RPC("GrabHighlightObject_RPC", RpcTarget.All, pv.ViewID, sender, e);
        }

        private void Networked_GrabUnHighlightObject(object sender, InteractableObjectEventArgs e)
        {
            if (debug)
                Debug.Log("Networked_GrabUnhighlightObject passed");


            pv.RPC("GrabUnHighlightObject_RPC", RpcTarget.All, pv.ViewID, sender, e);
        }

        ///USE
        
        private void Networked_UseHighlightObject(object sender, InteractableObjectEventArgs e)
        {
            if (debug)
                Debug.Log("Networked_UseHighlightObject passed");


            pv.RPC("UseHighlightObject_RPC", RpcTarget.All, pv.ViewID, sender, e);
        }

        private void Networked_UseUnHighlightObject(object sender, InteractableObjectEventArgs e)
        {
            if (debug)
                Debug.Log("Networked_UseUnhighlightObject passed");


            pv.RPC("UseUnHighlightObject_RPC", RpcTarget.All, pv.ViewID, sender, e);
        }

        #endregion

        #region RPCs
        /**
         *  All methods in this region are RPC-mediators in order to call the same method on all client machines in a network.
         *  Every RPC-Method has a second overload function that takes the custom type parameters as input.
         * */

        ///NEARTOUCH

        [PunRPC]
        private void NearTouchHighlightObject_RPC(int viewID, object sender, InteractableObjectEventArgs e)
        {
            if(debug)
                Debug.Log(name + ": NearTouchHighlighObject_RPC passed");


            if (pv.ViewID.Equals(viewID))
                NearTouchHighlightObject(sender, e);

        }

        [PunRPC]
        private void NearTouchHighlightObject_RPC(int viewID, MyCustomInteractableObject sender, MyCustomInteractableObjectEventArgs e)
        {
            if(debug)
                Debug.Log(name + ": NearTouchHighlighObject_RPC with Custom Types passed");


            if (pv.ViewID.Equals(viewID))
                NearTouchHighlightObject((object)sender, e.args);

        }

        [PunRPC]
        private void NearTouchUnHighlightObject_RPC(int viewID, object sender, InteractableObjectEventArgs e)
        {
            if(debug)
                Debug.Log(name + ": NearTouchUnHighlighObject_RPC passed");

            if (pv.ViewID.Equals(viewID))
                NearTouchUnHighlightObject(sender, e);


        }

        [PunRPC]
        private void NearTouchUnHighlightObject_RPC(int viewID, MyCustomInteractableObject sender, MyCustomInteractableObjectEventArgs e)
        {
            if(debug)
                Debug.Log(name + ": NearTouchUnHighlighObject_RPC with Custom Types passed");

            if (pv.ViewID.Equals(viewID))
            {
                /// Workaround: Calling the NearTouchUnHighlightObject-Method in this context would cause Nullreference-Exceptions on the sender-side.
                Unhighlight();
            }
        }

        ///TOUCH

        [PunRPC]
        private void TouchHighlightObject_RPC(int viewID, object sender, InteractableObjectEventArgs e)
        {
            if(debug)
                Debug.Log(name + ": TouchHighlighObject_RPC passed");


            if (pv.ViewID.Equals(viewID))
                TouchHighlightObject(sender, e);

        }

        [PunRPC]
        private void TouchHighlightObject_RPC(int viewID, MyCustomInteractableObject sender, MyCustomInteractableObjectEventArgs e)
        {
            if(debug)
                Debug.Log("TouchHighlighObject_RPC with Custom Types passed");


            if (pv.ViewID.Equals(viewID))
                TouchHighlightObject((object)sender, e.args);

        }

        [PunRPC]
        private void TouchUnHighlightObject_RPC(int viewID, object sender, InteractableObjectEventArgs e)
        {
            if(debug)
                Debug.Log("TouchUnHighlighObject_RPC passed");

            if (pv.ViewID.Equals(viewID))
                TouchUnHighlightObject(sender, e);

        }

        [PunRPC]
        private void TouchUnHighlightObject_RPC(int viewID, MyCustomInteractableObject sender, MyCustomInteractableObjectEventArgs e)
        {
            if(debug)
                Debug.Log("TouchUnHighlighObject_RPC with Custom Types passed");

            if (pv.ViewID.Equals(viewID))
            {
                /// Workaround: Calling the TouchUnHighlightObject-Method in this context would cause Nullreference-Exceptions on the sender-side.
                Unhighlight();
            }
        }

        /// GRAB

        [PunRPC]
        private void GrabHighlightObject_RPC(int viewID, object sender, InteractableObjectEventArgs e)
        {
            if (debug)
                Debug.Log(name + ": GrabHighlighObject_RPC passed");


            if (pv.ViewID.Equals(viewID))
                GrabHighlightObject(sender, e);

        }

        [PunRPC]
        private void GrabHighlightObject_RPC(int viewID, MyCustomInteractableObject sender, MyCustomInteractableObjectEventArgs e)
        {
            if (debug)
                Debug.Log(name + ": GrabHighlighObject_RPC with Custom Types passed");


            if (pv.ViewID.Equals(viewID))
                GrabHighlightObject((object)sender, e.args);

        }

        [PunRPC]
        private void GrabUnHighlightObject_RPC(int viewID, object sender, InteractableObjectEventArgs e)
        {
            if (debug)
                Debug.Log(name + ": GrabUnHighlighObject_RPC passed");

            if (pv.ViewID.Equals(viewID))
                GrabUnHighlightObject(sender, e);

        }

        [PunRPC]
        private void GrabUnHighlightObject_RPC(int viewID, MyCustomInteractableObject sender, MyCustomInteractableObjectEventArgs e)
        {
            if (debug)
                Debug.Log(name + ": GrabUnHighlighObject_RPC with Custom Types passed");

            if (pv.ViewID.Equals(viewID))
            {
                /// Workaround: Calling the TouchUnHighlightObject-Method in this context would cause Nullreference-Exceptions on the sender-side.
                Unhighlight();
            }
        }
        /// USE

        [PunRPC]
        private void UseHighlightObject_RPC(int viewID, object sender, InteractableObjectEventArgs e)
        {
            if (debug)
                Debug.Log(name + ": UseHighlighObject_RPC passed");


            if (pv.ViewID.Equals(viewID))
                UseHighlightObject(sender, e);

        }

        [PunRPC]
        private void UseHighlightObject_RPC(int viewID, MyCustomInteractableObject sender, MyCustomInteractableObjectEventArgs e)
        {
            if (debug)
                Debug.Log(name + ": UseHighlighObject_RPC with Custom Types passed");


            if (pv.ViewID.Equals(viewID))
                UseHighlightObject((object)sender, e.args);

        }

        [PunRPC]
        private void UseUnHighlightObject_RPC(int viewID, object sender, InteractableObjectEventArgs e)
        {
            if (debug)
                Debug.Log(name + ": UseUnHighlighObject_RPC passed");

            if (pv.ViewID.Equals(viewID))
                UseUnHighlightObject(sender, e);

        }

        [PunRPC]
        private void UseUnHighlightObject_RPC(int viewID, MyCustomInteractableObject sender, MyCustomInteractableObjectEventArgs e)
        {
            if (debug)
                Debug.Log(name + ": UseUnHighlighObject_RPC with Custom Types passed");

            if (pv.ViewID.Equals(viewID))
            {
                /// Workaround: Calling the TouchUnHighlightObject-Method in this context would cause Nullreference-Exceptions on the sender-side.
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

    internal class MyCustomInteractableObjectEventArgs
    {


        internal static void Register()
        {
            Debug.Log("MyCustomInteractableObjectEventArgs registration completed: " + PhotonPeer.RegisterType(typeof(InteractableObjectEventArgs), (byte)'I', Serialize, Deserialize));
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
            c.interactable = (VRTK_InteractableObject)customType;
            return new byte[] { c.Id };
        }
        #endregion
    }

}


