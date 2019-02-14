using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using VRTK;
using UnityEngine;

/// <summary>
/// Collection of static methods to retrieve another object's View ID.
/// <para>Created by Katharina Ziolkowski, 2019-02-14</para>
/// </summary>
public static class MUVRTK_PhotonViewIDGetter 
{
    public static int GetViewIDOfCollision(Collider other)
    {
        return other.gameObject.GetPhotonView().ViewID;
    }

    public static int GetViewIDOfTouchedObject(VRTK_InteractTouch interactTouch)
    {
        return interactTouch.GetTouchedObject().GetPhotonView().ViewID;
    }
}
