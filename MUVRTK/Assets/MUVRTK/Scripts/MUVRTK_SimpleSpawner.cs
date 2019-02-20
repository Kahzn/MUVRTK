namespace MUVRTK
{
    using Photon.Pun;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// This class defines takes an object prefab from the resources folder and spawns it in the scene visible to everyone.
    /// This script has to be added onto the ControllerScript Alias and manages the Prefab instantiation over the network.
    /// <para>Created by Katharina Ziolkowski, 2019-02-04</para>
    /// </summary>
    public class MUVRTK_SimpleSpawner : MonoBehaviour
    {
        #region Public Fields

        public bool debug;
        public GameObject objectToSpawn;
        
        #endregion


        #region Public Methods

        public void Spawn()
        {
                PhotonNetwork.Instantiate(objectToSpawn.name, new Vector3(transform.position.x, transform.position.y, transform.position.z), transform.rotation);

            if (debug)
                Debug.Log(name + " : Spawn(0) was called.");
        }

        public void Spawn(GameObject objectToSpawn)
        {

                PhotonNetwork.Instantiate(objectToSpawn.name, new Vector3(transform.position.x, transform.position.y, transform.position.z), transform.rotation);

            if (debug)
                Debug.Log(name + " : Spawn(1) was called.");
        }

        public void Spawn(Vector3 position)
        {

                PhotonNetwork.Instantiate(objectToSpawn.name, position, transform.rotation);

            if (debug)
                Debug.Log(name + " : Spawn(1) was called.");
        }

        public void Spawn(Vector3 position, Quaternion rotation)
        {

                PhotonNetwork.Instantiate(objectToSpawn.name, position, rotation);

            if (debug)
                Debug.Log(name + " : Spawn(2) was called.");
        }

        public void Spawn(GameObject go, Vector3 position, Quaternion rotation)
        {

                PhotonNetwork.Instantiate(go.name, position, rotation);
                if (debug)
                Debug.Log(name + " : Spawn(3) was called.");
        }

        #endregion
    }
}


