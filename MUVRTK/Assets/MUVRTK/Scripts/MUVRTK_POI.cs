namespace MUVRTK
{
    using Photon.Pun;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// This class defines takes an object prefab from the resources foulder and spawns it in the scene visible to everyone.
    /// This script has to be added onto the ControllerScript Alias and manages the Prefab instantiation over the network.
    /// <para>Created by Katharina Ziolkowski, 2019-02-04</para>
    /// </summary>
    public class MUVRTK_POI : MonoBehaviour
    {
        #region private Serializable Fields

        [SerializeField]
        private GameObject poi;


        #endregion

        #region Public Methods

        public void SpawnPOI()
        {
            PhotonNetwork.Instantiate(poi.name, transform.position, transform.rotation);
        }

        #endregion
    }
}


