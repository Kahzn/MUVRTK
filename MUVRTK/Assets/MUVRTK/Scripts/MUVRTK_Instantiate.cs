using System.Numerics;
using UnityEngine;
using Photon.Pun;
using UnityEngine.Serialization;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

namespace MUVRTK
{

    public class MUVRTK_Instantiate: MonoBehaviour
    {
        
        #region Private Serialize Fields
        
        [Header("VR Manager Section")]
        
        [Tooltip("The pure VR-Manager-Object without the Player Models. This needs to be instantiated locally because it is a singleton instance.")]
        [SerializeField] 
        private GameObject vr_Manager;

        [Tooltip("Optional: The position where you want your VR Manager (and thus: Player) to spawn. If nothing is set, they will spawn at 0,0,0.")]
        [SerializeField] 
        private Transform spawnPoint;
        
        [Tooltip("The Playermodel that you need attached to your VR-Manager.")]
        [SerializeField] 
        private GameObject playerModel;
       
        [Tooltip("The SDK-Setup-Switcher Panel. Mandatory!")]
        [SerializeField] 
        private GameObject sdkSetupSwitcher;
        
        [FormerlySerializedAs("objectsToInstantiate")]
        [Tooltip("All networked player objects. NOTE: Every networked Prefab needs a Photon View!")]
        [SerializeField]
        private GameObject [] objectsToInstantiateOverTheNetwork;
    
        #endregion
        
        #region Private Fields

        private bool cameraLoaded;

        private GameObject vrmInstance;
        private GameObject modelInstance;
        
        
        #endregion
        
        #region MonoBehaviour Callbacks

        void Start()
        {
            if (spawnPoint == null)
            {
                spawnPoint = transform;
            }
        }

        void Update()
        {
            if (!cameraLoaded)
            {
                if (vrmInstance != null)
                {
                    if (vrmInstance.GetComponentInChildren<Camera>() != null)
                    {
                        if (modelInstance != null)
                        {
                            BindModelToVRM(modelInstance, vrmInstance);
                            cameraLoaded = true;
                        }
                    }
                }
            }
        }
        
        #endregion

        
        #region Public Methods
        
        public void Instantiate_GameObjects()
        {
            vrmInstance = Instantiate(vr_Manager, spawnPoint.position, Quaternion.identity);
           Instantiate(sdkSetupSwitcher, transform.position, transform.rotation);

            if (playerModel != null)
            {
                modelInstance = PhotonNetwork.Instantiate(playerModel.name, transform.position, transform.rotation);
            }
    
            if (PhotonNetwork.IsConnected)
            {
                foreach (GameObject go in objectsToInstantiateOverTheNetwork)
                {
                    PhotonNetwork.Instantiate(go.name, new Vector3(0, 0, 0), Quaternion.identity);
                }
            }
            else
            {
                foreach (GameObject go in objectsToInstantiateOverTheNetwork)
                {
                    Instantiate(go, new Vector3(0, 0, 0), Quaternion.identity);
                }
            }
        }
        
        #endregion
        
        #region Private Methods

        private void BindModelToVRM(GameObject model, GameObject vrm)
        {
            Camera headCamera = vrm.GetComponentInChildren<Camera>();
            model.transform.parent = headCamera.transform;
            
            model.transform.localPosition = new Vector3(0,0,0);
            model.transform.rotation = Quaternion.Euler(0,0,0);
        }
        
        #endregion
    }
}

