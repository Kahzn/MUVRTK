using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;
using VRTK.UnityEventHelper;

namespace MUVRTK
{
    /// <summary>
    /// This class is the base class for all scene objects that are interacted with in the networked room.
    /// Scene Objects can broadcast their effects to all other Players in the room.
    /// <para>Created by Katharina Ziolkowski, 2019-02-04</para>
    /// </summary>
   
    [RequireComponent(typeof(PhotonView))]
    [RequireComponent(typeof(VRTK_InteractableObject))]
    public abstract class MUVRTK_SceneObject : MonoBehaviourPun
    {
        #region Public Enums

        // Interactions that shall be possible to use on an object in order to destroy it. For example, if I choose "Touch" the object shall be destroyed when my Player touches it with his controller.
        public enum DestroyInteractions { Touch, Point, Select,  Use, Grab, Collide, None};

        // Interactions with the object that shall trigger a broadcast reaction on all Players in a room. 
        //For example, when you choose "Grab", then all Players get a haptic feedback on their controllers as soon as you grab that object.
        // "Spawn" and "Destroy" are Interactions that are triggered when the object is spawned or destroyed (this applies to the automatic self-destruct as well).
        public enum TriggerInteractions { Touch, Point, Select, Use, Grab, Collide, Spawn, Destroy, None };

        public enum pointerRenderer { Straight, Bezier};

        #endregion

        #region Public Fields

        public bool debug;

        #endregion

        #region Protected Serialize Fields
        [Header("Object values")]

        [Tooltip("Lifetime of the object in the scene. As soon as this time has passed, the object destroys itself. If you don't want your object to destroy itself automatically, set this value to a whole negative number.")]
        [SerializeField]
        protected int lifetimeInSeconds = -99;

        [Tooltip("Points a Player can earn by destroying this object. Also a blueprint for values that are increased on the Player's side.")]
        [SerializeField]
        protected int points = 100;

        [Tooltip("The object health parameter. Also a blueprint for values that are decreased on the object's side.")]
        [SerializeField]
        protected float health = 1;

        [Header("Destroy Interactions")]

        [Tooltip("The interaction that shall be used to destroy this object.")]
        [SerializeField]
        protected DestroyInteractions destroyInteraction = DestroyInteractions.Use;

        [Header("Interaction triggered by the object - controls")]

        [Tooltip("The interaction with this object that shall trigger a broadcast action on all other players. ")]
        [SerializeField]
        protected TriggerInteractions triggerInteractions = TriggerInteractions.Spawn;

        [Header("Pointer Rendering Options")]
        [SerializeField]
        protected pointerRenderer pointerRendering = pointerRenderer.Bezier;


        [Header("Broadcast Interaction - Audio")]

        // Audio

        [Tooltip("Tick this if the Action shall trigger an Audio Clip.")]
        [SerializeField]
        protected bool playAudioClip;

        [SerializeField]
        protected AudioClip audioClip;

        [Header("Broadcast Interaction - Haptics")]

        // Haptics

        [Tooltip("Tick this if the Action shall trigger a Haptic Pulse on all Controllers.")]
        [SerializeField]
        protected bool triggerHapticPulse;

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

        [Header("Broadcast Interaction - Highlight")]

        // Highlight

        [Tooltip("Tick this if the Action shall trigger a Highlighting on the Object.")]
        [SerializeField]
        protected bool highlightObject;

        [SerializeField]
        protected MUVRTK_InteractObjectHighlighter interactObjectHighlighterComponent;

        // TODO: UI Notification Option!


        #endregion

        #region Private Fields

        private VRTK_InteractableObject interactable;
        private VRTK_Pointer[] pointers;
        private VRTK_BasePointerRenderer[] pointerRenderers;
        private MUVRTK_Instantiate instantiate;
        private MUVRTK_ControllerHaptics controllerHaptics;
        private Collider[] collider;
        private AudioSource audioSource;
        private float timeSinceSpawn = 0f;
        private GameObject[] controllerScriptAliases;
        private object[] triggerInteractionEvents;
        private bool destructionSetupCompleted;
        private bool interactionSetupCompleted;


        #endregion


        #region Monobehaviour Callbacks

        //setting up all components
        void OnEnable()
        {
            SetupControllerScriptAliases();

            SetupInteractableObject();

            SetupInteractionTrigger();

            SetupDestructionTrigger();

            SetupBroadcastActions();


        }

        private void Update()
        {
            timeSinceSpawn += Time.deltaTime;

            if(lifetimeInSeconds > 0)
            {
                if(lifetimeInSeconds < timeSinceSpawn)
                {
                    if (debug)
                        Debug.Log(name + " : Lifespan of this object has passed! Destroying it now.");

                    DestroyObject();
                }
            }

            if (!destructionSetupCompleted)
            {
                SetupDestructionTrigger();
            }

            if (!interactionSetupCompleted)
            {
                SetupInteractionTrigger();
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if(destroyInteraction == DestroyInteractions.Collide)
            {
                if (collision.gameObject.tag.Equals("Floor"))
                {
                    DestroyObject();
                }
            }

        }

        #endregion

        #region Private Methods

        private void SetupControllerScriptAliases()
        {
            if(GameObject.FindGameObjectsWithTag("ScriptAlias") != null)
            controllerScriptAliases = GameObject.FindGameObjectsWithTag("ScriptAlias");

            else
            {
                Debug.Log(name + " SetControllerScriptAliases: No ControllerScriptAliases found! Did you forget to set the tag?");
            }

        }



        /// <summary>
        /// Called in Start-Method.
        /// Binds the chosen destruction trigger to the interaction.
        /// </summary>
        private void SetupDestructionTrigger()
        {
            switch (destroyInteraction)
            { 
                case DestroyInteractions.Touch:

                        if(interactable != null)
                        {
                            interactable.SubscribeToInteractionEvent(VRTK_InteractableObject.InteractionType.Touch, DestroyObject);

                            destructionSetupCompleted = true;
                        }
                        else
                        {
                            if (debug)
                                Debug.Log("Interactable Object missing! Waiting for Setup.");
                        }
                    break;
                case DestroyInteractions.Point:
                    /// WIP: Script-side setup postponed due to bug that I cannot fix at the moment.
                    /// workaround: Add a Pointer Renderer Manually to your ScriptAliases and do the setup you need manually (interactwithObjects = true, etc.)
                    /// 
                    //Controller-side: Setup the pointer.

                    /**
                    SetupPointer();
                    foreach(VRTK_Pointer point in pointers)
                    {
                        point.interactWithObjects = true;
                    }**/

                    //Object-side: Setup the behaviour on the object.
                    if (interactable != null)
                    {
                        interactable.SubscribeToInteractionEvent(VRTK_InteractableObject.InteractionType.Touch, DestroyObject);

                        destructionSetupCompleted = true;
                    }
                    else
                    {
                        if (debug)
                            Debug.Log("Interactable Object missing! Waiting for Setup.");
                    }
                    break;
                case DestroyInteractions.Select:
                    //SetupPointer();

                    destructionSetupCompleted = true;
                    break;
                
                case DestroyInteractions.Use:
                    if (interactable != null)
                    {
                        interactable.SubscribeToInteractionEvent(VRTK_InteractableObject.InteractionType.Use, DestroyObject);

                        destructionSetupCompleted = true;
                    }
                    else
                    {
                        if (debug)
                            Debug.Log("Interactable Object missing! Waiting for Setup.");
                    }
                    break;
                case DestroyInteractions.Grab:
                    if (interactable != null)
                    {
                        interactable.SubscribeToInteractionEvent(VRTK_InteractableObject.InteractionType.Grab, DestroyObject);

                        destructionSetupCompleted = true;
                    }
                    else
                    {
                        if (debug)
                            Debug.Log("Interactable Object missing! Waiting for Setup.");
                    }
                    break;
                case DestroyInteractions.Collide:
                    SetupCollider();

                    Rigidbody rb = gameObject.AddComponent<Rigidbody>();
                    rb.useGravity = false;

                    destructionSetupCompleted = true;
                    break;
                default:
                    break;

            }
        }

        private void SetupCollider()
        {
            if (GetComponents<Collider>() != null)
                collider = GetComponents<Collider>();
            if (GetComponentsInChildren<Collider>() != null)
                collider = GetComponentsInChildren<Collider>();
            else collider[0] = gameObject.AddComponent<Collider>();
        }


        /// <summary>
        /// Retrieves the VRTK_InteractableObject Component of the SceneObject this script is on.
        /// Needed for all kinds of VRTK-Interactions: Touch, Grab, Use, Point.
        /// </summary>
        private void SetupInteractableObject()
        {
            if (GetComponent<VRTK_InteractableObject>())
            {
                interactable = GetComponent<VRTK_InteractableObject>();

                if (debug)
                    Debug.Log(name + " : Interactable found!");
            }


            else if (GetComponentInParent<VRTK_InteractableObject>())
            {
                interactable = GetComponentInParent<VRTK_InteractableObject>();
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

        //TODO: Fix Pointer Setup  Bug! (Nullreference-Exception)

        /// <summary>
        /// Retrieves or Sets the Pointer and PointerRenderer Components from the ControllerScriptAliases of both Controllers.
        /// </summary>
        private void SetupPointer()
        {
            if (controllerScriptAliases != null)
            {
                for (int i = 0; i < controllerScriptAliases.Length; i++)
                {
                    if (controllerScriptAliases[i].GetComponent<VRTK_Pointer>() != null)
                    {
                        /// BUG: causes Nullreference-Exceptions at the moment. No idea why.
                        pointers[i] = controllerScriptAliases[i].GetComponent<VRTK_Pointer>();
                    }
                    else
                    {
                        pointers[i] = controllerScriptAliases[i].AddComponent<VRTK_Pointer>();
                    }

                    if (controllerScriptAliases[i].GetComponent<VRTK_BasePointerRenderer>() != null)
                    {
                        pointerRenderers[i] = controllerScriptAliases[i].GetComponent<VRTK_BasePointerRenderer>();
                    }
                    else
                    {
                        if(pointerRendering == pointerRenderer.Bezier)
                        {
                            pointerRenderers[i] = controllerScriptAliases[i].AddComponent<VRTK_BezierPointerRenderer>();
                        }
                        else
                        {
                            pointerRenderers[i] = controllerScriptAliases[i].AddComponent<VRTK_StraightPointerRenderer>();
                        }
                        
                    }
                }
            }
            else
            {
                Debug.Log(name + " SetupInteractionTrigger: No ControllerScriptAliases found! Did you forget to set the tag?");
            }
        }
       

        private void SetupInteractionTrigger()
        {
            switch (triggerInteractions)
            {
                case TriggerInteractions.Collide:
                    SetupCollider();

                    Rigidbody rb = gameObject.AddComponent<Rigidbody>();
                    rb.useGravity = false;

                    interactionSetupCompleted = true;
                    break;

                case TriggerInteractions.Destroy:
                    //handled in DestroyObject-Method.
                    interactionSetupCompleted = true;
                    break;

                case TriggerInteractions.Grab:
                    if (interactable != null)
                    {
                        interactable.SubscribeToInteractionEvent(VRTK_InteractableObject.InteractionType.Grab, StartAction);

                        interactionSetupCompleted = true;
                    }
                    else
                    {
                        if (debug)
                            Debug.Log("Interactable Object missing! Waiting for Setup.");
                    }
                    break;

                case TriggerInteractions.Point:
                    if (interactable != null)
                    {
                        interactable.SubscribeToInteractionEvent(VRTK_InteractableObject.InteractionType.Touch, StartAction);

                        interactionSetupCompleted = true;
                    }
                    else
                    {
                        if (debug)
                            Debug.Log("Interactable Object missing! Waiting for Setup.");
                    }
                    break;

                case TriggerInteractions.Select:
                    interactionSetupCompleted = true;
                    break;

                case TriggerInteractions.Spawn:
                    StartAction();
                    break;

                case TriggerInteractions.Touch:
                    if (interactable != null)
                    {
                        interactable.SubscribeToInteractionEvent(VRTK_InteractableObject.InteractionType.Touch, StartAction);

                        interactionSetupCompleted = true;
                    }
                    else
                    {
                        if (debug)
                            Debug.Log("Interactable Object missing! Waiting for Setup.");
                    }
                    break;

                case TriggerInteractions.Use:
                    if (interactable != null)
                    {
                        interactable.SubscribeToInteractionEvent(VRTK_InteractableObject.InteractionType.Use, StartAction);

                        interactionSetupCompleted = true;
                    }
                    else
                    {
                        if (debug)
                            Debug.Log("Interactable Object missing! Waiting for Setup.");
                    }
                    break;

                case TriggerInteractions.None:
                    interactionSetupCompleted = true;
                    break;

                default:
                    break;
            }
            
        }

        private void SetupBroadcastActions()
        {
            if (playAudioClip)
            {
                if (audioClip == null)
                {
                    Debug.LogWarning(name + ": Play Audio Clip was selected, but no AudioClip was found.");
                }

                if (GetComponent<AudioSource>())
                {
                    audioSource = GetComponent<AudioSource>();

                    if (debug)
                        Debug.Log(name + " : AudioSource found and Set.");
                }
                else
                {
                    audioSource = gameObject.AddComponent<AudioSource>();

                    if (debug)
                        Debug.Log(name + " : AudioSource not found. Added.");
                }

            }
            /**
            if (triggerHapticPulse)
            {

                foreach(GameObject go in controllerScriptAliases)
                {
                    if (go.GetComponent<MUVRTK_ControllerHaptics>() == null)
                    {
                        controllerHaptics = go.AddComponent<MUVRTK_ControllerHaptics>();
                    }
                    else controllerHaptics = go.GetComponent<MUVRTK_ControllerHaptics>();

                }

            }**/

            if (highlightObject)
            {
                if (interactObjectHighlighterComponent == null)
                {
                    if (GetComponent<MUVRTK_InteractObjectHighlighter>())
                    {
                        interactObjectHighlighterComponent = GetComponent<MUVRTK_InteractObjectHighlighter>();
                        if (debug)
                            Debug.Log(name + " : interactObjectHighlighterComponent found.");
                    }

                    else
                    {
                        interactObjectHighlighterComponent = gameObject.AddComponent<MUVRTK_InteractObjectHighlighter>();
                        if (debug)
                            Debug.Log(name + " : interactObjectHighlighterComponent added.");
                    }
                }
            }

            else
            {
                Debug.Log(name + " : No Action selected!");
            }
        }

       

        private void StartAction()
        {
            if (playAudioClip)
            {
                if (audioClip.isReadyToPlay)
                {
                    audioSource.clip = audioClip;
                    audioSource.Play();
                }
            }

            if (triggerHapticPulse)
            {
                foreach(GameObject go in controllerScriptAliases)
                {
                    Networked_TriggerHapticPulse(photonView, go.GetPhotonView().ViewID, vibrationStrength, duration, pulseInterval);
                }
               
            }

            if (highlightObject)
            {
                //TODO implementation (maybe new Simple_Highlighter-Method?)
            }
        }

        /// <summary>
        /// Overload for use with VRTK Interaction Events
        /// </summary>
        private void StartAction(object o, InteractableObjectEventArgs e)
        {
            if (playAudioClip)
            {
                if (audioClip.isReadyToPlay)
                {
                    audioSource.clip = audioClip;
                    audioSource.Play();
                }
            }

            if (triggerHapticPulse)
            {
                foreach (GameObject go in controllerScriptAliases)
                {
                    Networked_TriggerHapticPulse(photonView, go.GetPhotonView().ViewID, vibrationStrength, duration, pulseInterval);
                }

            }

            if (highlightObject)
            {
                //TODO implementation (maybe new Simple_Highlighter-Method?)
            }
        }

        private void DestroyObject()
        {
            if (triggerInteractions.Equals(TriggerInteractions.Destroy))
            {
                StartAction();
            }

            PhotonNetwork.Destroy(GetComponent<PhotonView>());

            if (debug)
                Debug.Log(name + " : DestroyObject was called.");
        }


        /// <summary>
        /// Overload for use with VRTK Interaction Events
        /// </summary>
        private void DestroyObject(object o, InteractableObjectEventArgs e)
        {

            if (triggerInteractions.Equals(TriggerInteractions.Destroy))
            {
                StartAction();
            }

            PhotonNetwork.Destroy(GetComponent<PhotonView>());

            if (debug)
                Debug.Log(name + " : DestroyObject was called.");
        }


        #endregion

        #region Network Methods


        public void Networked_TriggerHapticPulse(PhotonView pv, int viewIDOfController, float vibrationStrength, float duration, float pulseInterval)
        {
            pv.RPC("BroadcastHapticPulseOnViewID", RpcTarget.All, viewIDOfController, vibrationStrength, duration, pulseInterval);

            Debug.Log("MUVRTK_StaticControllerHaptics : Networked_TriggerHapticPulse(5) was called.");
        }

        #endregion

       
    }
}


