using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using VRTK;
using UnityEngine;

public class getPhotonViewID : MonoBehaviour
{
    private  VRTK_InteractTouch vrtk_interactTouch;

    private void Start()
    {
        vrtk_interactTouch = gameObject.GetComponent<VRTK_InteractTouch>();
    }

    /*
    private void OnTriggerEnter(Collider other)
    { 

        Debug.Log("Collision!");
        if(other.GetComponent<PhotonView>())
            Debug.Log(gameObject.name + ": The Object that last touched me had this Photon View ID:" + other.GetComponent<PhotonView>().ViewID);


        if (other.GetComponentInChildren<PhotonView>())
            Debug.Log(gameObject.name + ": The Object that last touched me had this Photon View ID:" + other.GetComponentInChildren<PhotonView>().ViewID);

        else Debug.Log(gameObject.name + ": Didn't find any PhotonView!");
    }*/

    public void GetViewIDOfTouchedObject()
    {
        Debug.Log("Touched Object!");

        Debug.Log(gameObject.name + ": The View ID of the touched object is: " + vrtk_interactTouch.GetTouchedObject().GetPhotonView().ViewID);
    }
}
