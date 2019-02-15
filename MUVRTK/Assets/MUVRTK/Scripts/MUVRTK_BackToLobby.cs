



namespace MUVRTK{

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;
    
public class MUVRTK_BackToLobby : MonoBehaviour
{

    public MUVRTK_GameManager_base gameManager;

    private VRTK_InteractableObject interactable;

    void Start()
    {
        interactable = GetComponent<VRTK_InteractableObject>();
        interactable.SubscribeToInteractionEvent(VRTK_InteractableObject.InteractionType.Grab, Leave);
    }

    public void Leave(object sender, InteractableObjectEventArgs args)
    {
        gameManager.LeaveRoom();
    }

}

}


