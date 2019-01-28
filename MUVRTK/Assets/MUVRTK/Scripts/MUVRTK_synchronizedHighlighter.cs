using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace MUVRTK
{
    [RequireComponent(typeof(PhotonView))]
    [RequireComponent(typeof(MeshRenderer))]
    public class MUVRTK_synchronizedHighlighter : MonoBehaviour, IPunObservable
    {
        public bool debug;

        private MeshRenderer renderer;

        private void Start()
        {
            renderer = gameObject.GetComponent<MeshRenderer>();
        }


        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                stream.SendNext(renderer.material.color);

                if (debug)
                    Debug.Log(this.name + ": Stream is Writing this Color: " + renderer.material.color);
            }
            else
            {
                renderer.material.color = (Color)stream.ReceiveNext();
                if (debug)
                    Debug.Log(this.name + ": Stream is Receiving this Color: " + renderer.material.color);
            }
        }
    }
}


