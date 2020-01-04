
namespace MUVRTK{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEditor;
    using System.IO;                      
    using System;                         
    using System.Globalization;           
    using UnityEngine.SceneManagement; 
    using Photon.Pun;   

    /// <summary>
    /// collects session data and writes it into a csv-file
    /// <para>Created by Katharina Ziolkowski, 2019-02-20</para>
    /// </summary>
    public class MUVRTK_Evaluation : MonoBehaviour
    {
        public bool trackTimeSpentInScene;
        public bool trackPositionData;
        public PhotonView photonViewOfTrackedObject;
        public GameObject nonNetworkedGameObjectToTrack;
        public int numberOfFrames = 50;

        private string path;
        private int tempNumberOfFrames;
        
        // Start is called before the first frame update            
        void Start()
        {
            SetupFile();
        }

        // Update is called once per frame
        void Update()
        {
            if (trackPositionData)
            {
                 if (photonViewOfTrackedObject)                                                                                                                                                                                                     
                    {                                                                                                                                                                                                                                  
                     if (tempNumberOfFrames == 0)                                                                                                                                                                                                   
                      {                                                                                                                                                                                                                             
                          if (photonViewOfTrackedObject.IsMine)                                                                                                                                                                                     
                          {                                                                                                                                                                                                                         
                              WriteString(string.Format("Position of tracked object: {0}  at : {1} is {2}", photonViewOfTrackedObject.ViewID, Time.realtimeSinceStartup, photonViewOfTrackedObject.gameObject.transform.position.ToString()), path);
                          }                                                                                                                                                                                                                         
                                                                                                                                                                                                                                                    
                          tempNumberOfFrames = numberOfFrames;                                                                                                                                                                                      
                      }                                                                                                                                                                                                                             
                      else                                                                                                                                                                                                                          
                      {                                                                                                                                                                                                                             
                          tempNumberOfFrames--;                                                                                                                                                                                                     
                      }                                                                                                                                                                                                                             
                 }

                if (nonNetworkedGameObjectToTrack)
                {
                    if (tempNumberOfFrames == 0)                                                                                                                                                                                                   
                      {                                                                                                                                                                                                                             
                                                                                                                                                                                                                       
                          WriteString(string.Format("Position of tracked object: {0}  at : {1} is {2}", nonNetworkedGameObjectToTrack.name, Time.realtimeSinceStartup, nonNetworkedGameObjectToTrack.transform.position.ToString()), path);                                                                                                                                                                                                                        
                                                                                                                                                                                                                                                    
                          tempNumberOfFrames = numberOfFrames;                                                                                                                                                                                      
                      }                                                                                                                                                                                                                             
                      else                                                                                                                                                                                                                          
                      {                                                                                                                                                                                                                             
                          tempNumberOfFrames--;                                                                                                                                                                                                     
                      }                   
                }
            }                                                                                                                                                                                                                                                                                                                                                                                                            
        }

        private void OnDestroy()
        {
            if (trackTimeSpentInScene)
            {
                WriteString(string.Format("The time since Level load of Level {0} was : {1}", SceneManager.GetActiveScene().name, Time.realtimeSinceStartup), path);
            }
        }

        /// <summary>
        /// create new Eval File. If it already exists, override it.
        /// </summary>
        private void SetupFile()
        {
            if (photonViewOfTrackedObject)
            {
                path  = string.Format("Assets/MUVRTK/Evaluation/eval_{0}_{1}_{2}_{3}_{4}.csv", photonViewOfTrackedObject.ViewID, 
                    DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute);     
            }

            if (nonNetworkedGameObjectToTrack)
            {
                path  = string.Format("Assets/MUVRTK/Evaluation/eval_{0}_{1}_{2}_{3}_{4}.csv", nonNetworkedGameObjectToTrack.GetInstanceID(), 
                    DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute);     
            }
            
           
        }
       
        
        static void WriteString(string text, string path)                                                                                                               
        {                                                                                                                                                                                     
                                                                                                                                                
            //Write some text to the test.txt file   
            StreamWriter writer = new StreamWriter(path, true);   
            writer.WriteLine(text);                                                                                                           
            writer.Close();

#if UNITY_EDITOR
            //Re-import the file to update the reference in the editor                                                                          
            AssetDatabase.ImportAsset(path);
#endif
            TextAsset asset = (TextAsset) Resources.Load(path);
                                                                                                                                                                                                                        

        }

    }

}
