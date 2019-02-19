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
    
    #region Private Serialized Fields

    [SerializeField]
    private GameObject personalSpaceShape;

    [SerializeField] private bool visibleToOtherPlayers;
    
    private GameObject personalSpaceInstance;
    private GameObject personalSpaceInstanceCopyOnDummy;
    
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
            personalSpaceInstance = PhotonNetwork.Instantiate(personalSpaceShape.name, transform.position, transform.rotation);

            if (!visibleToOtherPlayers)
            {
                personalSpaceInstance.layer = 8;
                personalSpaceInstance.transform.GetChild(0).gameObject.layer = 8;
            }
            
            personalSpaceInstanceCopyOnDummy = Instantiate(personalSpaceShape, dummyAvatar.transform.position,
                dummyAvatar.transform.rotation);
            alreadyCreated = true;
        }
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

