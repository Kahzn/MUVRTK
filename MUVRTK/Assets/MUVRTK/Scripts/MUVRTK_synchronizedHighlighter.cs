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

        private Color color;

        private void Start()
        {
            renderer = gameObject.GetComponent<MeshRenderer>();
            color = new Color();
        }


        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                stream.SendNext(renderer.material.color.r);
                stream.SendNext(renderer.material.color.g);
                stream.SendNext(renderer.material.color.b);
                stream.SendNext(renderer.material.color.a);

                if (debug)
                    Debug.Log(this.name + ": Stream is Writing this Color: " + renderer.material.color);
            }
            else
            {
                color.r = (float)stream.ReceiveNext();
                color.g = (float)stream.ReceiveNext();
                color.b = (float)stream.ReceiveNext();
                color.a = (float)stream.ReceiveNext();

                if (debug)
                    Debug.Log(this.name + ": Stream is Receiving this Color: " + renderer.material.color);

                renderer.material.color = color;
            }
        }

    }
}


