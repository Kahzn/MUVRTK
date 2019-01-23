using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class getPhotonViewID : MonoBehaviour
{
    private void OnCollisionEnter(Collision other)
    {
        Debug.Log("Collision!");
        Debug.Log("The Object that last touched me had this Photon View ID:" + other.transform.GetComponentInChildren<PhotonView>().ViewID);
    }
}
