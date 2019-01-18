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

        #region Private Fields


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

        [Tooltip("Send the Impulse to all players in the room, including you. Default setting.")]
        [SerializeField]
        private bool broadcastToAll = true;

        [Tooltip("Send the Impulse to all other players in the room.")]
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

        // Start is called before the first frame update
        void Start()
        {
            //Hotfix. Should be updated for Optimization.
            controller_right = GameObject.Find("Controller (right)");
            controller_left = GameObject.Find("Controller (left)");

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
                    VRTK_ControllerHaptics.TriggerHapticPulse(VRTK_ControllerReference.GetControllerReference(controller_right), 15f, 0.5f, 0.01f);
                }

                if (Input.GetKeyDown(KeyCode.L))
                {
                    VRTK_ControllerHaptics.TriggerHapticPulse(VRTK_ControllerReference.GetControllerReference(controller_left), 10f, 1f, 0.02f);
                }

                if (broadcastToAll)
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

        #region PUN Callbacks

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

        #region Public Methods

        public void RemoteCallTriggerHapticPulse()
        {
            if(broadcastToAll || broadcastToAllOthers)
            this.photonView.RPC("BroadcastHapticPulse", rpcTarget);
        }

        #endregion
    }
}