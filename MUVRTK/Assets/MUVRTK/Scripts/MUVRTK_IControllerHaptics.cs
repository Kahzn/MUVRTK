using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface MUVRTK_IControllerHaptics 
{
    [PunRPC]
    void BroadcastHapticPulseOnViewID(int viewIdOfController, float vibrationStrength, float duration, float pulseInterval);

    }
