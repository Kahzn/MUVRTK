using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

namespace MUVRTK
{
    /// <summary>
    /// This class is the base class for all player owned objects that are interacted with in the networked room.
    /// They send direct messages to the player who owns them, thus enabling direct interactions with specific other players.
    /// <para>Created by Katharina Ziolkowski, 2019-02-04</para>
    /// </summary>

    [RequireComponent(typeof(VRTK_InteractableObject))]
    public abstract class MUVRTK_PlayerOwnedObject : MonoBehaviour
    {

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

        [Tooltip("Tick this if the Action shall be triggered on Collision.")]
        [SerializeField]
        protected bool collision;

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

        [SerializeField]
        protected MUVRTK_ControllerHaptics controllerHapticsComponent;

        // Highlight

        [Tooltip("Tick this if the Action shall trigger a Highlighting on the Object.")]
        [SerializeField]
        protected bool highlightObject;

        [SerializeField]
        protected MUVRTK_InteractObjectHighlighter interactObjectHighlighterComponent;

        // TODO: UI Notification Option!


        #endregion
    }
}


