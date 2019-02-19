using System.Numerics;
using System.Runtime.CompilerServices;
using Photon.Pun;
using VRTK;

namespace MUVRTK
{
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class handles the behaviour of the Personalspace-bubble around the avatar.
/// <para>Created by Katharina Ziolkowski, 2019-02-19</para>
/// </summary>
public class MUVRTK_PersonalSpace : MonoBehaviour
{

    public GameObject avatarInstance;
    public GameObject dummyAvatar;
   // public MUVRTK_PanelMenuSlider panelMenuSlider;
    public MUVRTK_SliderResize sliderResize;
    public GameObject personalSpaceInstance;
    public GameObject personalSpaceInstanceCopyOnDummy;
    
    #region Private Serialized Fields

    [SerializeField]
    private GameObject personalSpaceShape;
    
    [SerializeField]
    private GameObject localPersonalSpaceShapeForDummy;

    [SerializeField] private bool visibleToOtherPlayers;

    #endregion
    
    #region Private Fields

    private bool alreadyCreated;
    private bool bindingCompleted;

    #endregion
    
    #region MonoBehaviour Callbacks

    private void Start()
    {
        alreadyCreated = false;
        dummyAvatar = GameObject.FindWithTag("Dummy");
        //panelMenuSlider = GameObject.FindWithTag("PanelMenuSlider").GetComponent<MUVRTK_PanelMenuSlider>();
        sliderResize = GameObject.FindWithTag("PanelMenuSlider").GetComponent<MUVRTK_SliderResize>();
    }

    private void Update()
    {
        if (alreadyCreated)
        {
            if (avatarInstance)
            {
                if (!bindingCompleted)
                {
                    BindPersonalSpaceToAvatar(avatarInstance);
                    bindingCompleted = true;
                }

            }
        }
    }
    
    #endregion
    
    #region Public Methods

    /// <summary>
    /// Spawns a Personal Space Bubble around the Player.
    /// Will be tunable with UI
    /// </summary>
    public void Instantiate()
    {
        if (!alreadyCreated)
        {
            //toggle visibility for the Players' Personal Space
            if (!visibleToOtherPlayers)
            {
                personalSpaceShape.layer = 8;
                personalSpaceShape.transform.GetChild(0).gameObject.layer = 8;
            }
            else
            {
                personalSpaceShape.layer = 0;
                personalSpaceShape.transform.GetChild(0).gameObject.layer = 0;
            }
            
            personalSpaceInstance = PhotonNetwork.Instantiate(personalSpaceShape.name, transform.position, transform.rotation);
            
            
            personalSpaceInstanceCopyOnDummy = Instantiate(localPersonalSpaceShapeForDummy, dummyAvatar.transform.position,
                dummyAvatar.transform.rotation);
            alreadyCreated = true;
        }

        sliderResize.objectToResize = personalSpaceInstance;
        sliderResize.referenceObject = personalSpaceInstanceCopyOnDummy;
    }

    public void BindPersonalSpaceToAvatar(GameObject avatar)
    {
            personalSpaceInstance.transform.parent = avatar.transform;
            personalSpaceInstance.transform.localPosition = new Vector3(0, 0, 0);
            personalSpaceInstance.transform.localRotation = Quaternion.Euler(0,0,0);

    }

    #endregion

   

}

}

