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
    public class MUVRTK_SimpleSpawner : MonoBehaviour
    {
        #region private Serializable Fields

        [SerializeField]
        private GameObject objectToSpawn;


        #endregion

        #region Public Methods

        public void Spawn()
        {
            PhotonNetwork.Instantiate(objectToSpawn.name, new Vector3(transform.position.x, transform.position.y, transform.position.z - 0.5f), transform.rotation);
        }

        #endregion
    }
}


