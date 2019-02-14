using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;
using Photon.Pun;
using Photon.Realtime;

namespace MUVRTK
{
    /// <summary>
    /// This class is the base class for all player owned objects that are interacted with in the networked room.
    /// They send direct messages to the player who owns them, thus enabling direct interactions with specific other players.
    /// <para>Created by Katharina Ziolkowski, 2019-02-04</para>
    /// </summary>

    [RequireComponent(typeof(VRTK_InteractableObject))]
    public abstract class MUVRTK_PlayerOwnedObject : MonoBehaviourPun
    {
        #region Protected Enums

        /// <summary>
        /// Interactions with the other Player that shall trigger the reaction.
        /// For example, when you choose "Pointer", then the Interaction shall be triggered when another player points at this object.
        /// </summary>
        protected enum TriggerInteractions { Touch, Point, None };

        #endregion

        #region Public Fields

        public bool debug;

        #endregion

        #region Protected Serialize Fields

        [Header("How to trigger the interacton")]

        [Tooltip("The interaction that shall trigger the reaction on this player's side. E.g. if the audioclip shall be triggered on touch or on collision.")]
        [SerializeField]
        protected TriggerInteractions triggerInteraction;

        [Header("Interactions")]

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

        protected VRTK_InteractableObject interactableObject;
        protected Collider[] collider;
        protected GameObject[] controllerScriptAliases;

        #endregion

        #region Private Fields


        private bool triggerSetupCompleted;


        #endregion

        #region MonoBehaviour Callbacks

        private void Start()
        {
            if (!triggerSetupCompleted)
            {
                SetupTriggers();

            }

        }

        private void OnEnable()
        {
            if (!triggerSetupCompleted)
            {
                SetupTriggers();

            }
        }

        #endregion

        #region StartAction Methods

      

        protected virtual void StartAction(object o, InteractableObjectEventArgs e)
        {
            //making sure that objects that are owned by the Player (like e.g. the Controller Models) don't count as Touching.
            if (!e.interactingObject.GetPhotonView().IsMine)
            {
                //choosing the interaction option
                if (triggerHapticPulse)
                {
                    Debug.Log(name + " : Startaction(2) was called. TriggerHapticPulse selected.");

                    //Calling networked method
                    Networked_HapticPulseOnBothOwnedControllers(photonView, vibrationStrength, duration, pulseInterval);

                }
                else
                {
                    Debug.Log("Haptic Pulse not selected!");
                }
            }
            else{
                if (debug)
                    Debug.Log("Touched by own PlayerObject!");
            }
            
        }

        #endregion

        #region Setup Triggers

        protected void SetupTriggers()
        {
            if (debug)
                Debug.Log(name + " : Setting up Triggers. Current Interaction Trigger: " + triggerInteraction.ToString());

            switch (triggerInteraction)
            {
                case (TriggerInteractions.Touch):
                    SetupControllerScriptAliases();
                    SetupInteractable();
                    if (interactableObject != null)
                    {
                        interactableObject.SubscribeToInteractionEvent(VRTK_InteractableObject.InteractionType.Touch, StartAction);
                        triggerSetupCompleted = true;
                        Debug.Log(name + " Touch Trigger Setup complete!");
                    }
                    else
                    {
                        if (debug)
                            Debug.Log("Interactable Object missing! Waiting for Setup.");
                    }
                    break;


                case (TriggerInteractions.Point):
                    //Pointer-Touch is handled like Interact_Touch in VRTK.
                    SetupControllerScriptAliases();
                    SetupInteractable();
                    if(interactableObject != null)
                    {
                        interactableObject.SubscribeToInteractionEvent(VRTK_InteractableObject.InteractionType.Touch, StartAction);
                        triggerSetupCompleted = true;
                        Debug.Log(name + "Pointer Trigger Setup complete!");
                    }
                    else
                    {
                        if (debug)
                            Debug.Log("Interactable Object missing! Waiting for Setup.");
                    }
                    break;

                case (TriggerInteractions.None):
                    triggerSetupCompleted = true;
                    break;

                default:
                    triggerSetupCompleted = true;
                    break;
            }
        }

        #endregion

        #region Basic Setup Methods

        void SetupInteractable()
        {
            if (interactableObject == null)
            {
                if (GetComponent<VRTK_InteractableObject>())
                {
                    interactableObject = GetComponent<VRTK_InteractableObject>();

                    if (debug)
                        Debug.Log(name + " : Interactable found!");
                }
                else if (GetComponentInChildren<VRTK_InteractableObject>())
                {
                    interactableObject = GetComponentInChildren<VRTK_InteractableObject>();
                    if (debug)
                        Debug.Log(name + " : Interactable found in Parent!");
                }
                else
                {
                    gameObject.AddComponent<VRTK_InteractableObject>();
                    if (debug)
                        Debug.Log(name + " : Interactable not found. Added missing Component VRTK_InteractableObject.");
                }
            }
        }


        protected void SetupControllerScriptAliases()
        {
            if (GameObject.FindGameObjectsWithTag("ScriptAlias") != null)
                controllerScriptAliases = GameObject.FindGameObjectsWithTag("ScriptAlias");

            else
            {
                Debug.Log(name + " SetControllerScriptAliases: No ControllerScriptAliases found! Did you forget to set the tag?");
            }

        }


        #endregion

        #region Network Methods

        /// <summary>
        /// Calls RPC-Method on the child object. 
        /// </summary>
        /// <param name="pv"></param>
        /// <param name="vibrationStrength"></param>
        /// <param name="duration"></param>
        /// <param name="pulseInterval"></param>

        public void Networked_HapticPulseOnBothOwnedControllers(PhotonView pv, float vibrationStrength, float duration, float pulseInterval)
        {
            Player player = photonView.Owner;
            pv.RPC("HapticPulseOnBothOwnedControllers", player, vibrationStrength, duration, pulseInterval);

            Debug.Log(name + " : Networked_TriggerHapticPulseOnPlayer(5) was called on this ViewID:" + photonView.ViewID);
        }



        #endregion
    }
}


