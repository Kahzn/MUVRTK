namespace MUVRTK
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using VRTK;
    using Photon.Pun;

    /// <summary>
    /// This class takes an input from a Touch/Point-Interaction and lets the controllers of the player who has been touched vibrate and/or plays an Audio Clip.
    /// <para> Created by Katharina Ziolkowski, 2019-02-14</para>
    /// </summary>
    public class MUVRTK_InteractHaptics : MUVRTK_PlayerOwnedObject
    {
        private bool controllerScriptAliasesSetup;
        private bool audioSourceSetup;

        private void Update()
        {
            if (!controllerScriptAliasesSetup)
            {
                if (controllerScriptAliases.Length == 0)
                {
                    SetupControllerScriptAliases();
                }
                else
                {
                    controllerScriptAliasesSetup = true;
                }
            }

            if (!audioSourceSetup)
            {
                SetupAudioSource();
                audioSourceSetup = true;
            }
        }

        #region PUN RPCs

        /// <summary>
        /// Implementation of "HapticPulseOnBothOwnedControllers". Mandatory for all PlayerOwnedObjects!
        /// This method is called on Interaction with a Player owned Object and triggers a haptic pulse on all active Controllers owned by this player.
        /// The vibrationStrength, duration and pulseInterval are set in the PlayerOwnedObject-Script.
        /// </summary>
        /// <param name="vibrationStrength"></param>
        /// <param name="duration"></param>
        /// <param name="pulseInterval"></param>
        [PunRPC]
        void HapticPulseOnBothOwnedControllers(float vibrationStrength, float duration, float pulseInterval)
        {
            if (controllerScriptAliases.Length > 0)
            {
                foreach(GameObject go in controllerScriptAliases)
                {
                    VRTK_ControllerHaptics.TriggerHapticPulse(VRTK_ControllerReference.GetControllerReference(PhotonView.Find(go.GetPhotonView().ViewID).gameObject), vibrationStrength, duration, pulseInterval);
                    Debug.Log(name + " : HapticPulseOnBothOwnedControllers-RPC worked as it should. Haptic Pulse Triggered on ViewID:" + go.GetPhotonView().ViewID);
                }
            }
            else
            {
                SetupControllerScriptAliases();

                Debug.Log(name + " : HapticPulseOnBothOwnedControllers-RPC: too few controllerScripAliases!");
            }
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
                }
            }
            else
            {
                Debug.LogWarning(viewId + "missing AudioSource / AudioClip!");
            }
        }

        #endregion

    }
}

