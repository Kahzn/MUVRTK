using UnityEngine.Serialization;

namespace MUVRTK
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using Photon.Pun;
    /// <summary>
    /// This class describes the behaviour of a shared awareness object.
    /// The object changes its color brightness when it is being looked at by a player closely.
    /// This happens via a simple collision detection mechanism. For this to work, the player needs a Viewport-Collider tagged "view".
    /// <para> Created by Katharina Ziolkowski, 2019-02-14</para>
    /// </summary>
    public class MUVRTK_SharedAwareness : MUVRTK_SceneObject
    {
        #region Private Fields
        
        [FormerlySerializedAs("highlightStrength")] 
        [Range(0,1)]
        [SerializeField] private float sharedAwarenessHighlightStrength;
        
        private MeshRenderer renderer;
        private int viewerCount;
        private Color currentColor;
        private int viewId;

        private float lastR;
        private float lastG;
        private float lastB;
        
        #endregion
        
        #region MonoBehaviour Callbacks

        private void OnEnable()
        {
            viewerCount = 0;

            renderer = GetComponent<MeshRenderer>();
            

            viewId = PhotonView.Get(this).ViewID;
        }


        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag.Equals("View"))
            {
                viewerCount++;

                if (debug)
                    Debug.Log(name + " : ViewCount : " + viewerCount);
                currentColor = renderer.material.color;

                //PhotonView.Get(this).RPC("MakeColorLighter", RpcTarget.All, viewId,  currentColor.r, currentColor.g , currentColor.b );
                MakeColorLighter(viewId, currentColor.r, currentColor.g, currentColor.b);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.tag.Equals("View"))
            {
                viewerCount--;

                if (debug)
                    Debug.Log(name + " : ViewCount : " + viewerCount);

                currentColor = renderer.material.color;
                //PhotonView.Get(this).RPC("MakeColorDarker", RpcTarget.All, viewId, currentColor.r, currentColor.g, currentColor.b);
                MakeColorDarker(viewId, currentColor.r, currentColor.g, currentColor.b);
            }

        }

        #endregion
       
        #region Pun RPCs
        
        [PunRPC]
        public void MakeColorLighter(int photonView, float r, float g, float b)
        {
            if(photonView == viewId)
            {
                //if you're reaching the limit, save the difference and add only that.
                if (r + sharedAwarenessHighlightStrength > 1f)
                {
                    lastR = 1 - r;
                    
                    Debug.Log("r if");
                }
                else
                {
                    lastR = sharedAwarenessHighlightStrength;
                    Debug.Log("r else");
                }
                
                if (g + sharedAwarenessHighlightStrength > 1f)
                {
                    lastG = 1 - g;
                    Debug.Log("g if");
                }
                else
                {
                    lastG = sharedAwarenessHighlightStrength;
                    Debug.Log("g else");
                }
                
                if (b + sharedAwarenessHighlightStrength > 1f)
                {
                    lastB = 1 - b;
                    Debug.Log("b if");
                }
                else
                {
                    lastB = sharedAwarenessHighlightStrength;
                    Debug.Log("b else");
                }

                renderer.material.color = new Color(r+lastR, g+lastG, b+lastB, 1f);

            }
        }
        
        [PunRPC]
        public void MakeColorDarker(int photonView, float r, float g, float b)
        {
            if(photonView == viewId)
            {
                if (r - sharedAwarenessHighlightStrength < 0f)
                {
                    lastR = r;
                    Debug.Log("r if");
                }
                else
                {
                    lastR = sharedAwarenessHighlightStrength;
                    Debug.Log("r else");
                    
                }
                
                if (g - sharedAwarenessHighlightStrength < 0f)
                {
                    lastG = g;
                    Debug.Log("g if");
                }
                else
                {
                    lastG = sharedAwarenessHighlightStrength;
                    Debug.Log("g else");
                }
                
                if (b - sharedAwarenessHighlightStrength < 0f)
                {
                    lastB = b;
                    Debug.Log("b if");
                }
                else
                {
                    lastB = sharedAwarenessHighlightStrength;
                    Debug.Log("b else");
                }
                
                renderer.material.color = new Color(r - lastR, g - lastG, b - lastB, 1f);

            }
        }
        
        #endregion
    }
}


