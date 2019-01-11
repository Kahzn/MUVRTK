using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class Testscript_controllerHaptics : MonoBehaviour, IPunObservable
{
    [SerializeField]
    private GameObject controller_right;
    [SerializeField]
    private GameObject controller_left;

    private PhotonView photonView;

    // Start is called before the first frame update
    void Start()
    {
        photonView = this.gameObject.GetComponent<PhotonView>();
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.K))
        {
            VRTK_ControllerHaptics.TriggerHapticPulse(VRTK_ControllerReference.GetControllerReference(controller_right), 15f, 0.5f, 0.01f);
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            VRTK_ControllerHaptics.TriggerHapticPulse(VRTK_ControllerReference.GetControllerReference(controller_left), 10f, 1f, 0.02f);
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            photonView.RPC("CallTriggerHapticPulse", RpcTarget.All);
        }
    }

    [PunRPC]
    void CallTriggerHapticPulse()
    {
        VRTK_ControllerHaptics.TriggerHapticPulse(VRTK_ControllerReference.GetControllerReference(controller_right), 10f, 1f, 0.02f);
        VRTK_ControllerHaptics.TriggerHapticPulse(VRTK_ControllerReference.GetControllerReference(controller_left), 10f, 1f, 0.02f);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        throw new System.NotImplementedException();
    }
}
