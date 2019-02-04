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
        #region Public Fields

        public bool debug;

        #endregion

        #region Protected Serialize Fields

        [Header("How to trigger the interacton")]

        [Tooltip("Tick this if the Action shall be triggered on Touch.")]
        [SerializeField]
        protected bool touch;

        [Tooltip("Tick this if the Action shall be triggered on Pointer Touch.")]
        [SerializeField]
        protected bool pointer;

        [Tooltip("Tick this if the Action shall be triggered on Use.")]
        [SerializeField]
        protected bool use;

        [Tooltip("Tick this if the Action shall be triggered on Grab.")]
        [SerializeField]
        protected bool grab;

        [Tooltip("Tick this if the Action shall be triggered on Spawn.")]
        [SerializeField]
        protected bool spawn;

        [Tooltip("Tick this if the Action shall be triggered on Collision.")]
        [SerializeField]
        protected bool collision;

        [Header("Broadcast Action")]

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

        #endregion


        #region Monobehaviour Callbacks

        //setting up all components
        private void Start()
        {
            #region trigger interaction setup
            if (touch || use || grab || pointer)
            {

                if (GetComponent<VRTK_InteractableObject>())
                {
                    interactable = GetComponent<VRTK_InteractableObject>();

                    if (debug)
                        Debug.Log(name + " : Interactable found!");
                }


                else if (GetComponentInParent<VRTK_InteractableObject>())
                {
                    interactable = GetComponent<VRTK_InteractableObject>();
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

            if(pointer)
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

            if (spawn)
            {
                StartAction();
            }

            if (collision)
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

        #endregion
    }
}


