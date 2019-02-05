namespace MUVRTK
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// Manages the lifespan and Behaviour of the POI.
    /// The parent class MUVRTK_SceneObject contains different actions that can be triggered on object spawn.
    /// <para>Created by Katharina Ziolkowski, 2019-02-04</para>
    /// </summary>
    public class MUVRTK_PoiManager : MUVRTK_SceneObject
    {


        private MeshRenderer meshRenderer;

        private void Start(){

            meshRenderer = GetComponent<MeshRenderer>();

        }


    }

}


