

namespace MUVRTK
{
    
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using Photon.Pun;
    using Photon.Realtime;
    using VRTK;

    [RequireComponent(typeof(PhotonView))]
    public class MUVRTK_BodyCollision : MonoBehaviourPun
    {
        public bool debug;

        [Tooltip("Enter the name of the tag this Collision shall listen for. It will only trigger the reaction if the colliding object has this tag.")]
        [SerializeField]
        protected string collisionTag;

        // Audio
        
        [Tooltip("Tick this if the Action shall trigger an Audio Clip.")]
        [SerializeField]
        protected bool playAudioClip;

        [SerializeField]
        protected AudioClip audioClip;
        
        // Haptics

        [Tooltip("Tick this if the Action shall trigger a Haptic Pulse on all Controllers.")]
        [SerializeField]
        protected bool triggerHapticPulse;

        [Tooltip("Defines the duration of the Controller Vibration")]
        [SerializeField]
        [Range(0, 10)]
        protected float duration = 1f;

        [Tooltip("Defines the pulse Interval of the controller Vibration")]
        [SerializeField]
        [Range(0f, 10f)]
        protected float pulseInterval = 0.1f;

        [Tooltip("Defines the strength of the controller Vibration")]
        [SerializeField]
        [Range(0f, 20f)]
        protected float vibrationStrength = 10f;

        private PhotonView pv;
        private AudioSource audioSource;

        #region MonoBehaviour Callbacks

        // Start is called before the first frame update
        void Start()
        {
            pv = PhotonView.Get(this);

            if (!audioSource)
            {
                if (GetComponent<AudioSource>() != null)
                {
                    audioSource = GetComponent<AudioSource>();
                }
                else
                {
                    audioSource = gameObject.AddComponent<AudioSource>();
                }

            }
        }

        
        private void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.tag.Equals(collisionTag))
            {
                Player otherPlayer = other.gameObject.GetPhotonView().Owner;
                if(playAudioClip)
                pv.RPC("PlayAudioClip", otherPlayer);
                
                if(triggerHapticPulse)
                    pv.RPC("HapticPulseOnCollidedController", otherPlayer);
            }
            
        }
        
        #endregion
        
        #region PUN RPCs

        /// <summary>
        /// Implementation of "HapticPulseOnBothOwnedControllers". 
        /// This method is called on Interaction with a Player owned Object and triggers a haptic pulse on all active Controllers owned by this player.
        /// The vibrationStrength, duration and pulseInterval are set in the PlayerOwnedObject-Script.
        /// </summary>
        /// <param name="vibrationStrength"></param>
        /// <param name="duration"></param>
        /// <param name="pulseInterval"></param>
        [PunRPC]
        void HapticPulseOnCollidedController()
        {
                    VRTK_ControllerHaptics.TriggerHapticPulse(VRTK_ControllerReference.GetControllerReference(pv.gameObject), vibrationStrength, duration, pulseInterval);
                    Debug.Log(name + " : HapticPulseOnCollidedController-RPC worked as it should. Haptic Pulse Triggered on ViewID:" + pv.ViewID);
           
        }

        /// <summary>
        /// Implementation of "PlayAudioClip". Necessary for all PlayerOwnedObjects!
        /// </summary>
        [PunRPC]
        void PlayAudioClip()
        {
            if (audioSource)
            {
                if (audioClip.isReadyToPlay)
                {
                    audioSource.clip = audioClip;
                    audioSource.Play();
                    
                    if(debug)
                        Debug.Log(pv.ViewID + " : Playing Audio file!");
                }
            }
            else
            {
                Debug.LogWarning(pv.ViewID + "missing AudioSource / AudioClip!");
            }
        }

        #endregion
    }
}


