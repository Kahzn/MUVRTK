using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Photon.Pun;
using UnityEditor;
using UnityEngine.Serialization;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;
using VRTK;

namespace MUVRTK
{
/// <summary>
/// Manages all Player Instantiation Issues in MUVRTK. 
/// <para>created by Katharina Ziolkowski, 2019.01.23</para>
/// </summary>

    public class MUVRTK_Instantiate: MonoBehaviour
    {
        
        #region Private Serialize Fields

        [SerializeField] private bool debug;
        
        [SerializeField] private bool testOffline;
        
        [Header("VR Manager Section")]
        
        [Tooltip("The pure VR-Manager-Object without the Player Models. This needs to be instantiated locally because it is a singleton instance.")]
        [SerializeField] 
        private GameObject vr_Manager;

        [Tooltip("The SDK-Setup-Switcher Panel. Mandatory!")]
        [SerializeField]
        private GameObject sdkSetupSwitcher;

        [Tooltip("Optional: The position where you want your VR Manager (and thus: Player) to spawn. If nothing is set, they will spawn at 0,0,0.")]
        [SerializeField] 
        private Transform spawnPoint;
        
        [Tooltip("The Player Avatar that you need attached to your VR-Manager.")]
        [SerializeField] 
        private GameObject playerAvatar;
       



        [Tooltip("tick yes if you want to exchange the default controller Models for networked ones. DONT TICK IF YOU'RE USING THE SIMULATOR.")]
        [SerializeField]
        private bool showControllersOverNetwork;

        [Tooltip("The Array of Models for the Controllers. If you want to use the same for both, add it only once. Otherwise: First Left, Second Right.")]
        [SerializeField] 
        private GameObject[] controllerModels;

        [Tooltip("The Array of Scriptaliases for the Controllers. If you want to use the same for both, add it only once. Otherwise: First Left, Second Right.")]
        [SerializeField]
        private GameObject[] controllerScriptAliases;
        
        [Tooltip("tick yes if you want to Basic Teleport on your controller Script Aliases.")]
        [SerializeField]
        private bool enableTeleport;

        [Tooltip("All networked player objects. NOTE: Every networked Prefab needs a Photon View!")]
        [SerializeField]
        private GameObject [] objectsToInstantiateOverTheNetwork;

        [Tooltip("offset by which an object can be instantiated randomly from the center of the map.")]
        [SerializeField]
        private float networkedOffset;

        [Tooltip("All non-networked player objects. Photon Views not necessary. These objects can only be seen and manipulated by the local Player.")]
        [SerializeField]
        private GameObject [] objectsToInstantiateLocally;

        [Tooltip("offset by which an object can be instantiated randomly from the center of the map.")]
        [SerializeField]
        private float localOffset;

        #endregion

        #region Private Fields

        private bool cameraLoaded;
        private bool leftControllerModelLoaded;
        private bool rightControllerModelLoaded;
        private bool playareascriptloaded;

        private GameObject vrmInstance;
        private GameObject playerModelInstance;
        private GameObject leftControllerInstance;
        private GameObject rightControllerInstance;
        private GameObject[] controllerModelInstances = new GameObject[2];

        private bool controllerModelContainerinitialized;
        
        
        #endregion
        
        #region MonoBehaviour Callbacks

        void Start()
        {

            if (enableTeleport)
            {
                TeleportEnable();
            }

            //If you want to test offline, call Instantiate here. Else it will be called by the GameManager in the OnJoinedRoom-Method.
            if (testOffline)
            {
                Instantiate_GameObjects();
            }
        }

        void Update()
        {
            // This waits for the vrm and model-instantiation and then binds the two together in holy matrimony. 
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
                if (controllerModels.Length > 0 && showControllersOverNetwork)
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
                if (controllerModels.Length > 0 && showControllersOverNetwork)
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

            // first: instantiate the sdk-manager and the correlated setup-switcher
            if (spawnPoint)
            {
                vrmInstance = Instantiate(vr_Manager, spawnPoint.position, Quaternion.identity);
            }
            else vrmInstance = Instantiate(vr_Manager, new Vector3(Random.value * 5, 0f, Random.value * 5), Quaternion.identity);
           PhotonNetwork.Instantiate(sdkSetupSwitcher.name, transform.position, transform.rotation);


            //Player Model Instantiation
            if (playerAvatar != null)
            {
                if(PhotonNetwork.IsConnected)
                playerModelInstance = PhotonNetwork.Instantiate(playerAvatar.name, transform.position, transform.rotation);

                else playerModelInstance = Instantiate(playerAvatar, transform.position, transform.rotation);
            }

            //Networked Controller Model Instantiation
            if (controllerModels.Length > 0 && showControllersOverNetwork && PhotonNetwork.IsConnected)
            {
                //If you want the same Model applied to both Controllers
                if (controllerModels.Length == 1 )
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

            }


            //All other Networked Objects Instantiation, including Controller Script Aliases

            if (PhotonNetwork.IsConnected)
            {
                //Controler Script Aliases
                if (controllerScriptAliases.Length > 1)
                {
                    vrmInstance.GetComponent<VRTK_SDKManager>().scriptAliasLeftController = PhotonNetwork.Instantiate(controllerScriptAliases[0].name, transform.position, transform.rotation);
                    vrmInstance.GetComponent<VRTK_SDKManager>().scriptAliasRightController = PhotonNetwork.Instantiate(controllerScriptAliases[1].name, transform.position, transform.rotation);
                }
                if(controllerScriptAliases.Length == 1)
                {
                    vrmInstance.GetComponent<VRTK_SDKManager>().scriptAliasLeftController = PhotonNetwork.Instantiate(controllerScriptAliases[0].name, transform.position, transform.rotation);
                    vrmInstance.GetComponent<VRTK_SDKManager>().scriptAliasRightController = PhotonNetwork.Instantiate(controllerScriptAliases[0].name, transform.position, transform.rotation);
                }
                else
                {
                    Debug.LogWarning(name + ": No ControllerScriptAliases found!");
                }
               


                //All else (interactive objects and the like)
                foreach (GameObject go in objectsToInstantiateOverTheNetwork)
                {
                    PhotonNetwork.InstantiateSceneObject(go.name, new Vector3 (Random.value * networkedOffset, Random.value, Random.value * networkedOffset), Quaternion.Euler(0, 0, 0));
                }
                
            }
            else
            {
                //Controller Script Aliases
                foreach (GameObject go in controllerScriptAliases)
                {
                    Instantiate(go, transform.position, transform.rotation);

                }

                //All networked objects
                foreach (GameObject go in objectsToInstantiateOverTheNetwork)
                {
                    Instantiate(go, new Vector3(Random.value * networkedOffset, 1, Random.value * networkedOffset), Quaternion.Euler(0,0,0));
                }
                
            }

            //All local objects
            foreach (GameObject go in objectsToInstantiateLocally)
            {
                Instantiate(go, new Vector3(Random.value * localOffset, 1, Random.value * localOffset), Quaternion.Euler(0, 0, 0));
            }

        }

        public void TeleportEnable()
        {
            foreach (GameObject go in controllerScriptAliases)
            {
                if (!go.GetComponent<VRTK_ControllerEvents>())
                {
                    go.AddComponent<VRTK_ControllerEvents>();
                    
                }
                
                if (!go.GetComponent<VRTK_Pointer>())
                {
                    go.AddComponent<VRTK_Pointer>();
                }
                VRTK_Pointer pointer = go.GetComponent<VRTK_Pointer>();
                pointer.enableTeleport = true;
                pointer.holdButtonToActivate = true;
                
                if (!go.GetComponent<VRTK_StraightPointerRenderer>() && !go.GetComponent<VRTK_BezierPointerRenderer>())
                {
                    go.AddComponent<VRTK_StraightPointerRenderer>();
                }

                if (!playareascriptloaded)
                {
                    GameObject PlayArea =
                        AssetDatabase.LoadAssetAtPath<GameObject>("Assets/MUVRTK/Local_Resources/PlayAreaScript.prefab");
                    Instantiate(PlayArea, transform.position, transform.rotation);
                    playareascriptloaded = true;
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
        /// Searches the Controller Hierarchy for the Model Body and deactivates its Mesh Renderer Component so that we don't render two models on top of each other.
        /// Only necessary if controller models need to be synchronized over network.
        /// </summary>
        /// <param name="controllerInstance"></param>
        private void DeactivateCurrentControllerModel(GameObject controllerInstance)
        {
            if (controllerInstance.transform.childCount > 0)
            {
                if(debug)
                Debug.Log(controllerInstance.name + " has this number of children: " + controllerInstance.transform.childCount);

                GameObject originalModel = controllerInstance.transform.GetChild(0).gameObject;
                originalModel.transform.GetChild(1).GetComponent<MeshRenderer>().enabled = false;
            }
           
        }

        
        #endregion
    }
}

