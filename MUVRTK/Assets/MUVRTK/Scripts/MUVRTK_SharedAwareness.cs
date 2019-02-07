namespace MUVRTK
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using Photon.Pun;

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
                PhotonView.Get(this).RPC("ChangeColor", RpcTarget.All, viewId,  currentColor.r + 0.1f, currentColor.g +0.1f, currentColor.b +0.1f);
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
                PhotonView.Get(this).RPC("ChangeColor", RpcTarget.All, viewId, currentColor.r -0.1f, currentColor.g -0.1f, currentColor.b -0.1f);
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


