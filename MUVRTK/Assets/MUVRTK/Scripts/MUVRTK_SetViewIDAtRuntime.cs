namespace MUVRTK
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using Photon.Pun;

    public class MUVRTK_SetViewIDAtRuntime : MonoBehaviourPunCallbacks
    {

        #region MonoBehaviourPunCallbacks

        // Start is called before the first frame update
        public void AddPhotonView()
        {

            PhotonView pv = gameObject.AddComponent<PhotonView>();
            PhotonTransformView ptv = gameObject.AddComponent<PhotonTransformView>();
            PhotonNetwork.AllocateViewID(pv);
            pv.ObservedComponents = new List<Component>();
            pv.ObservedComponents.Add(ptv);
        }

        #endregion


    }
}


