

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
        public AudioSource audioSource;
        public AudioClip audioClip;

        private PhotonView pv;


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
            Player otherPlayer = other.gameObject.GetPhotonView().Owner;
            pv.RPC("PlayAudioClip", otherPlayer);
        }
        
        #region PUN RPCs

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


