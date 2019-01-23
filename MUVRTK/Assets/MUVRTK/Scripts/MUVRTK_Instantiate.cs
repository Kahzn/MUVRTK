using UnityEngine;
using Photon.Pun;

namespace MUVRTK
{

    public class MUVRTK_Instantiate: MonoBehaviour
    {
        [SerializeField]
        private GameObject [] objectsToInstantiate;
    
        public void Instantiate_GameObjects()
        {
    
            if (PhotonNetwork.IsConnected)
            {
                foreach (GameObject go in objectsToInstantiate)
                {
                    PhotonNetwork.Instantiate(go.name, new Vector3(0, 0, 0), Quaternion.identity);
                }
            }
            else
            {
                foreach (GameObject go in objectsToInstantiate)
                {
                    Instantiate(go, new Vector3(0, 0, 0), Quaternion.identity);
                }
            }
        }
    }
}

