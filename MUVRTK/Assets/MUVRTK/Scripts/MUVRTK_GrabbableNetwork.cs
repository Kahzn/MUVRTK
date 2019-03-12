using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class MUVRTK_GrabbableNetwork : MonoBehaviour
{
    private PhotonView view;

    private Vector3 correctPos;

    private Quaternion correctRot;

    private Transform root;

    // Start is called before the first frame update
    void Start()
    {
        this.correctPos = transform.position;
        this.correctRot = transform.rotation;
        view = GetComponent<PhotonView>();
        root = transform.root;
    }

    // Update is called once per frame
    void Update()
    {
        //if grabbed - root changes due to parenting of an grabbed object to the controller
        if(transform.root != root)
        {
            view.RPC("SetTransform", RpcTarget.All, transform.position, transform.rotation);
        }

        transform.position = Vector3.Lerp(transform.position, this.correctPos, Time.deltaTime * 5f);
        transform.rotation = Quaternion.Lerp(transform.rotation, this.correctRot, Time.deltaTime * 5f);
    }

    [PunRPC]
    public void SetTransform(Vector3 pos, Quaternion rot)
    {
        this.correctPos = pos;
        this.correctRot = rot;
    }
}
