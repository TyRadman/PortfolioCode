using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TankLike
{
    public class Bootstrapper: MonoBehaviour
    {
        const string BOOTSTRAP_SCENE = "S_Bootstrap";

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Bootstrap()
        {
            // Loop through the currently loaded scenes
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                Scene scene = SceneManager.GetSceneAt(i);

                // If the bootstrap scene is already loaded, return
                if (scene.name == BOOTSTRAP_SCENE)
                {
                    return;
                }

                // Load the bootstrap scene additively
                Debug.Log("Load bootstrap scene");
                SceneManager.LoadScene(BOOTSTRAP_SCENE, LoadSceneMode.Additive);
            }
        }


        //private void Awake()
        //{
        //    DontDestroyOnLoad(gameObject);
        //}

        //        private static List<string> _scenesNotToLoad = new List<string>()
        //        {
        //            "S_Bootstrapper", 
        //            "S_Level_03 (Ty)",
        //            "S_MapMaker"
        //        };

        //        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        //        static void Init()
        //        {
        //#if UNITY_EDITOR
        //            var currentlyLoadedEditorScene = SceneManager.GetActiveScene();

        //            if (_scenesNotToLoad.Exists(s => s == currentlyLoadedEditorScene.name)) return;

        //            //if (currentlyLoadedEditorScene.name == "S_Bootstrapper" ||
        //            //    currentlyLoadedEditorScene.name == "S_Level_03 (Ty)") return;
        //#endif

        //            if (SceneManager.GetSceneByName("S_Bootstrapper").isLoaded != true)
        //                SceneManager.LoadScene("S_Bootstrapper");

        //#if UNITY_EDITOR
        //            if (currentlyLoadedEditorScene.IsValid())
        //                SceneManager.LoadSceneAsync(currentlyLoadedEditorScene.name, LoadSceneMode.Additive);
        //#else
        //        SceneManager.LoadSceneAsync("CarSelection", LoadSceneMode.Additive);
        //#endif
        //        }
    }
}
