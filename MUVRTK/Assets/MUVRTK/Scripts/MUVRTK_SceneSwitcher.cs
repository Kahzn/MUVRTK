

using WebSocketSharp;

namespace MUVRTK
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using Photon.Pun;
    using UnityEngine.UI;

    public class MUVRTK_SceneSwitcher : MonoBehaviour
    {
        public bool debug;

        public MUVRTK_Launcher launcher;


        
        [SerializeField]
        private string sceneName;

        private void OnEnable()
        {
            sceneName = gameObject.GetComponentInChildren<Text>().text.ToString();
        }

        public void SwitchScene()
        {
            if (!sceneName.IsNullOrEmpty())
            {
                if (launcher)
                {
                    launcher.sceneName = sceneName;
                    launcher.ConnectToRoom();
                }

                
                if(debug)
                    Debug.Log(name + " SwitchScene was called. Switching to: " + sceneName);
            }
            else
            {
                if(debug)
                    Debug.Log(name + " : SwitchScene was called, but the scene name is empty!");
            }
        }
        
    }
}

