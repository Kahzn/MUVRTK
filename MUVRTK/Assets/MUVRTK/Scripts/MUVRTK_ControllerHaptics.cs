namespace MUVRTK
{
    using Photon.Pun;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using VRTK;

    /// <summary>
    /// This class combines interactions between Players regarding Controller Haptics.
    /// <para>created by Katharina Ziolkowski, 2018.01.11</para>
    /// </summary>
    public class MUVRTK_ControllerHaptics : MonoBehaviourPun
    {
        #region Public Fields

        public bool debug;

        #endregion

        #region Private Serialize Fields


        [Header("Haptics Settings")]
        [Tooltip("Defines the duration of the Controller Vibration")]
        [SerializeField]
        [Range(0, 10)]
        private float duration = 1f;

        [Tooltip("Defines the pulse Interval of the controller Vibration")]
        [SerializeField]
        [Range(0f, 10f)]
        private float pulseInterval = 0.1f;

        [Tooltip("Defines the strength of the controller Vibration")]
        [SerializeField]
        [Range(0f, 20f)]
        private float vibrationStrength = 10f;

        [Header("Controller References")]

        [Tooltip("The GameObject of the right Controller. It can be the Controller itself or the Script-Alias.")]
        [SerializeField]
        private GameObject controller_right;

        [Tooltip("The GameObject of the right Controller. It can be the Controller itself or the Script-Alias.")]
        [SerializeField]
        private GameObject controller_left;

        [Header("Interaction Settings")]

        [Header("Interact by broadcasting")]
        
        [Tooltip("Choose which Keyboard Key shall Trigger the Impulse. Enter only one letter or number. If you enter more, only the first will be registered. You can leave this empty, if you choose a Controller Event to Trigger the Function.")]
        [SerializeField]
        private string keyMapping;

        [Tooltip("Send the Impulse to all other players in the room. If this is not set, then the impulse is send to all Players in the room, including yourself.")]
        [SerializeField]
        private bool broadcastToAllOthers = false;

        [Header("Interact with specific Players")]

        [Tooltip("Send the Impulse one specific Player by touching him with your Controller.")]
        [SerializeField]
        private bool singleTriggerByTouch;

        [Tooltip("Send the Impulse to one specific Player by pointing at him with your Controller.")]
        [SerializeField]
        private bool singleTriggerByPointer = false;



        private RpcTarget rpcTarget;

        private PhotonView[] otherPlayers;

        #endregion

        #region MonoBehaviour Callbacks


        void OnEnable()
        {

            if (broadcastToAllOthers)
            {
                rpcTarget = RpcTarget.Others;
            }
            else
            {
                rpcTarget = RpcTarget.All;
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (photonView.IsMine)
            {
                if (Input.GetKeyDown(KeyCode.K))
                {
                    SetupControllersIfNecessary();
                    VRTK_ControllerHaptics.TriggerHapticPulse(VRTK_ControllerReference.GetControllerReference(controller_right), 15f, 0.5f, 0.01f);
                }

                if (Input.GetKeyDown(KeyCode.L))
                {
                    SetupControllersIfNecessary();
                    VRTK_ControllerHaptics.TriggerHapticPulse(VRTK_ControllerReference.GetControllerReference(controller_left), 10f, 1f, 0.02f);
                }

                if (!broadcastToAllOthers)
                {
                    if (Input.GetKeyDown(keyMapping[0].ToString()))
                    {
                        this.photonView.RPC("BroadcastHapticPulse", RpcTarget.All);
                    }
                }
                

                if (broadcastToAllOthers)
                {
                    if (Input.GetKeyDown(keyMapping[0].ToString()))
                    {
                        this.photonView.RPC("BroadcastHapticPulse", RpcTarget.Others);
                    }
                }
               
            }

        }

        #endregion

        #region PUN RPCs

        [PunRPC]
        void BroadcastHapticPulse()
        {
            VRTK_ControllerHaptics.TriggerHapticPulse(VRTK_ControllerReference.GetControllerReference(controller_right), 10f, 1f, 0.02f);
            VRTK_ControllerHaptics.TriggerHapticPulse(VRTK_ControllerReference.GetControllerReference(controller_left), 10f, 1f, 0.02f);
        }

        

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            throw new System.NotImplementedException();
        }

        #endregion

        #region Network Methods

        public void Networked_TriggerHapticPulse()
        {
            this.photonView.RPC("BroadcastHapticPulse", rpcTarget);

            if (debug)
                Debug.Log(name + " : Networked_TriggerHapticPulse(0) was called.");
        }

        public void Networked_TriggerHapticPulse(int viewIDOfController)
        {
            this.photonView.RPC("BroadcastHapticPulseOnViewID", rpcTarget, viewIDOfController, vibrationStrength, duration, pulseInterval);

            if (debug)
                Debug.Log(name + " : Networked_TriggerHapticPulse(1) was called.");
        }

        public void Networked_TriggerHapticPulse(int viewIDOfController, float vibrationStrength, float duration, float pulseInterval)
        {
            this.photonView.RPC("BroadcastHapticPulseOnViewID", rpcTarget, viewIDOfController, vibrationStrength, duration, pulseInterval);

            if (debug)
                Debug.Log(name + " : Networked_TriggerHapticPulse(4) was called.");
        }

        #endregion

        #region Private Methods

        private void SetupControllersIfNecessary()
        {
            if(controller_left == null)
            controller_left = GameObject.Find("Controller (left)");

            if (controller_right == null)
                controller_right = GameObject.Find("Controller (right)");
        }

        #endregion
    }
}