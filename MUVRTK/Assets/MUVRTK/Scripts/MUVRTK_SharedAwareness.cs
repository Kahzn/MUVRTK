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
        private MeshRenderer renderer;
        private int viewerCount;

        private Color currentColor;
        private int viewId;


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
                PhotonView.Get(this).RPC("ChangeColor", RpcTarget.All, viewId,  currentColor.r + 0.2f, currentColor.g +0.2f, currentColor.b + 0.2f);
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
                PhotonView.Get(this).RPC("ChangeColor", RpcTarget.All, viewId, currentColor.r -0.2f, currentColor.g -0.2f, currentColor.b -0.2f);
            }

        }

        [PunRPC]
        public void ChangeColor(int photonView, float r, float g, float b)
        {
            if(photonView == viewId)
            {
                renderer.material.color = new Color(r, g, b, 1f);

            }
        }
    }
}


