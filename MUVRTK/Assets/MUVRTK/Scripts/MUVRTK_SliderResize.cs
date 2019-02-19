using System.Numerics;
using UnityEngine.Experimental.UIElements;

namespace MUVRTK
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;

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
        
                objectToResize.transform.localScale = new Vector3(slider.value, slider.value, slider.value);
            }
            
            if (referenceObject)
            {   
                referenceObject.transform.localScale  = new Vector3(slider.value, slider.value, slider.value);
            }
        }
    }
}


