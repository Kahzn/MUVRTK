using System.Numerics;

namespace MUVRTK
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;
/// <summary>
/// Takes two GameObjects and resizes their scale according to a slider value.
/// <para>Created by Katharina Ziolkowski, 2019-02-19</para>
/// </summary>
    public class MUVRTK_SliderResize : MonoBehaviour
    {
        public GameObject objectToResize;

        public GameObject referenceObject;

        public Slider slider;

        private void OnEnable()
        {
            slider = GetComponent<Slider>();
        }

        public void Resize()
        {
            if (objectToResize)
            {
                //the factor is an approximation to make up for differences between the localscale of the dummy and the real avatar.
                objectToResize.transform.localScale = new Vector3(slider.value*0.03f, slider.value*0.03f, slider.value*0.03f);
            }
            
            if (referenceObject)
            {   
                referenceObject.transform.localScale  = new Vector3(slider.value, slider.value, slider.value);
            }
        }
    }
}


