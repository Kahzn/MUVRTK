using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

namespace MUVRTK
{
    /// <summary>
    /// This class is the base class for all scene objects that are interacted with in the networked room.
    /// Scene Objects can broadcast their effects to all other Players in the room.
    /// <para>Created by Katharina Ziolkowski, 2019-02-04</para>
    /// </summary>
   
    [RequireComponent(typeof(PhotonView))]
    [RequireComponent(typeof(VRTK_InteractableObject))]
    public abstract class MUVRTK_SceneObject : MonoBehaviour
    {
        #region Public Enums

        public enum DestroyInteractions { Touch, Point, Select, Activate, Use, Grab, Collide};

        public enum TriggerInteractions { Touch, Point, Select, Activate, Use, Grab, Collide, Spawn };

        #endregion

        #region Public Fields

        public bool debug;

        #endregion

        #region Protected Serialize Fields
        [Header("Object values")]

        [Tooltip("Lifetime of the object in the scene. As soon as this time has passed, the object destroys itself. If you don't want your object to destroy itself automatically, set this value to -99.")]
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

       

        [Header("Broadcast Interaction")]

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

        [SerializeField]
        protected MUVRTK_ControllerHaptics[] controllerHapticsComponent;

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
        private Collider[] collider;
        private AudioSource audioSource;
        private float timeSinceSpawn = 0f;

        #endregion


        #region Monobehaviour Callbacks

        //setting up all components
        private void Start()
        {
            #region trigger interaction setup
            if (triggerInteractions == TriggerInteractions.Touch || triggerInteractions == TriggerInteractions.Grab || triggerInteractions == TriggerInteractions.Use)
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

            if(triggerInteractions == TriggerInteractions.Point || triggerInteractions == TriggerInteractions.Activate || triggerInteractions == TriggerInteractions.Select)
            {
                if(FindObjectsOfType<VRTK_Pointer>() != null)
                {
                    pointers = FindObjectsOfType<VRTK_Pointer>();
                    Debug.Log(name + " : Pointers set.");
                }
                else
                {
                    GameObject[] controllerScriptAliases = GameObject.FindGameObjectsWithTag("ScriptAlias");

                    if(controllerScriptAliases != null)
                    {
                        foreach (GameObject go in controllerScriptAliases)
                        {
                            go.AddComponent<VRTK_Pointer>();
                            go.AddComponent<VRTK_BezierPointerRenderer>();
                            Debug.Log(name + " : Pointers and Renderers could not be loaded. Setting them anew.");
                        }
                    }
                    else
                    {
                        Debug.Log(name + " : No ControllerScriptAliases found! Did you forget to set the tag?");
                    }
                }

                if (FindObjectsOfType<VRTK_BasePointerRenderer>() != null)
                {
                    pointerRenderers = FindObjectsOfType<VRTK_BasePointerRenderer>();
                    
                    if(debug)
                        Debug.Log(name + " : Pointer Renderers set.");
                }
               
            }

            if (triggerInteractions == TriggerInteractions.Spawn)
            {
                StartAction();
            }

            if (triggerInteractions == TriggerInteractions.Collide)
            {
                if (GetComponents<Collider>() != null)
                {
                    collider = GetComponents<Collider>();

                    if (debug)
                        Debug.Log(name + " : Collider set.");
                        
                }

            }

            

            #endregion

            #region Action setup

            if (playAudioClip)
            {
                if(audioClip == null)
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

            if (triggerHapticPulse)
            {
                if(controllerHapticsComponent == null)
                {
                    if (FindObjectsOfType<MUVRTK_ControllerHaptics>() != null)
                    {
                        controllerHapticsComponent = FindObjectsOfType<MUVRTK_ControllerHaptics>();

                        if (debug)
                            Debug.Log(name + " : controllerHapticsComponent found");
                    }
                    
                }

            }

            if (highlightObject)
            {
                if(interactObjectHighlighterComponent == null)
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

            #endregion


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
        }

        #endregion

        #region Private Methods

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
                //TODO implementation
            }

            if (highlightObject)
            {
                //TODO implementation (maybe new Simple_Highlighter-Method?)
            }
        }

        private void DestroyObject()
        {
            PhotonNetwork.Destroy(GetComponent<PhotonView>());

            if (debug)
                Debug.Log(name + " : DestroyObject was called.");
        }

        #endregion
    }
}


