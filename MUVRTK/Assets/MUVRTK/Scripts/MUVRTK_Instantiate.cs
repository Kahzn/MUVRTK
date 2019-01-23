using System.Numerics;
using UnityEngine;
using Photon.Pun;
using UnityEngine.Serialization;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

namespace MUVRTK
{
/// <summary>
/// Manages all Player Instantiation Issues in MUVRTK. 
/// <para>created by Katharina Ziolkowski, 2019.01.23</para>
/// </summary>
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
        
        [Tooltip("The Array of Models for the Controllers. If you want to use the same for both, add it only once. Otherwise: First Left, Second Right.")]
        [SerializeField] 
        private GameObject[] controllerModels;
        
        [FormerlySerializedAs("objectsToInstantiate")]
        [Tooltip("All networked player objects. NOTE: Every networked Prefab needs a Photon View!")]
        [SerializeField]
        private GameObject [] objectsToInstantiateOverTheNetwork;
    
        #endregion
        
        #region Private Fields

        private bool cameraLoaded;
        private bool leftControllerModelLoaded;
        private bool rightControllerModelLoaded;

        private GameObject vrmInstance;
        private GameObject playerModelInstance;
        private GameObject leftControllerInstance;
        private GameObject rightControllerInstance;
        private GameObject[] controllerModelInstances;
        
        
        #endregion
        
        #region MonoBehaviour Callbacks

        void Start()
        {
            if (spawnPoint == null)
            {
                spawnPoint = transform;
            }
            
            controllerModelInstances = new GameObject[2];
        }

        void Update()
        {
            
            // and this, ladies in gentlemen, is the most awful flight of if-stairs I've ever seen. But I'm quite too stupid to do it any better, and this works. So I guess you'll have to live with it. Sorry!
            // also: This waits for the vrm and model-instantiation and then binds the two together in holy matrimony. 
            if (!cameraLoaded)
            {
                if (vrmInstance != null)
                {
                    if (vrmInstance.GetComponentInChildren<Camera>() != null)
                    {
                        if (playerModelInstance != null)
                        {
                            BindModelToVRM(playerModelInstance, vrmInstance, "Camera");
                            cameraLoaded = true;
                            
                        }
                    }
                }
            }

            if (!leftControllerModelLoaded)
            {
                //only replace the current controller meshes if any new models have been added to the array
                if (controllerModels.Length > 0)
                {
                    if (leftControllerInstance != null)
                    {
                        if (leftControllerInstance.transform.GetChild(0).transform.childCount > 0)
                        {
                            BindModelToVRM(controllerModelInstances[0], vrmInstance, "LeftController");
                            DeactivateCurrentControllerModel(leftControllerInstance);
                            leftControllerModelLoaded = true;
                        }
 
                    }
                    else
                    {
                        leftControllerInstance = GameObject.FindWithTag("LeftController");
                    }
                    
                }
            }
            
            if (!rightControllerModelLoaded)
            {
                //only replace the current controller meshes if any new models have been added to the array
                if (controllerModels.Length > 0)
                {
                    if (rightControllerInstance != null)
                    {
                        if (rightControllerInstance.transform.GetChild(0).transform.childCount > 0)
                        {
                            BindModelToVRM(controllerModelInstances[1], vrmInstance, "RightController");
                            DeactivateCurrentControllerModel(rightControllerInstance);
                            rightControllerModelLoaded = true;
                        }  
                    }
                    else
                    {
                        rightControllerInstance = GameObject.FindWithTag("RightController");
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
                playerModelInstance = PhotonNetwork.Instantiate(playerModel.name, transform.position, transform.rotation);
            }

            if (controllerModels.Length > 0)
            {
                //If you want the same Model applied to both Controllers
                if (controllerModels.Length == 1)
                {
                    controllerModelInstances[0] = PhotonNetwork.Instantiate(controllerModels[0].name,
                        transform.position, transform.rotation);
                    controllerModelInstances[1] = PhotonNetwork.Instantiate(controllerModels[0].name,
                        transform.position, transform.rotation); 
                }

                if (controllerModels.Length == 2)
                {
                    //If you want different Models applied to both Controllers
                    controllerModelInstances[0] = PhotonNetwork.Instantiate(controllerModels[0].name,
                        transform.position, transform.rotation);
                    controllerModelInstances[1] = PhotonNetwork.Instantiate(controllerModels[1].name,
                        transform.position, transform.rotation); 
                }
                else
                {
                    Debug.LogWarning(name + ": Too many Controllermodels added to Array. Which ones shall I choose?");
                }
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
        
        /// <summary>
        /// Takes a Model Prefab, a vr-manager GameObject and a keyword and binds the model to the movements of the vrm.
        /// keyword options for component: Camera, LeftController, RightController
        ///
        /// </summary>
        /// <param name="model"></param>
        /// <param name="vrm"></param>
        /// <param name="component"></param>
        private void BindModelToVRM(GameObject model, GameObject vrm, string component)
        {
            if (component.Equals("Camera"))
            {
                Camera headCamera = vrm.GetComponentInChildren<Camera>();
                model.transform.parent = headCamera.transform;
            }

            if (component.Equals("LeftController"))
            {
                if (leftControllerInstance != null)
                {
                    //add the  networked Model to the Controller
                    model.transform.parent = leftControllerInstance.transform;
                }               
                else
                {
                    Debug.LogWarning(name + ": Controller (left) not found!");
                }
                
            }

            if (component.Equals("RightController"))
            {

                if (rightControllerInstance != null)
                {
                    //add the  networked Model to the Controller
                    model.transform.parent = rightControllerInstance.transform;
                }
                else
                {
                    Debug.LogWarning(name + ": Controller (right) not found!");
                }

            }

            model.transform.localPosition = new Vector3(0,0,0);
            model.transform.localRotation = Quaternion.Euler(0,0,0);
        }

        /// <summary>
        /// Searches the Controller Hierarchy for the Model Body and deactivates its Mesh Renderer Component sothat we don't render two models on top of each other.
        /// Only necessary if controller models need to be synchronized over network.
        /// </summary>
        /// <param name="controllerInstance"></param>
        private void DeactivateCurrentControllerModel(GameObject controllerInstance)
        {

            GameObject originalModel = controllerInstance.transform.GetChild(0).gameObject;
            originalModel.transform.GetChild(1).GetComponent<MeshRenderer>().enabled = false;
        }
        
        #endregion
    }
}

