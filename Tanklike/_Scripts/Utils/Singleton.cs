using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.Utils
{
    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T instance;
        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    // If an instance already exists
                    instance = FindObjectOfType<T>();

                    //if (instance == null)
                    //{
                    //    GameObject obj = new GameObject();
                    //    obj.name = typeof(T).Name;
                    //    instance = obj.AddComponent<T>();
                    //}

                    //if (instance == null)
                    //{
                    //    GameObject obj = new GameObject();
                    //    obj.name = typeof(T).Name;
                    //    instance = obj.AddComponent<T>();
                    //}
                }
                return instance;
            }
        }

        //public virtual void Awake()
        //{
        //    if (instance != null && instance != this as T)
        //    {
        //        Destroy(gameObject);
        //    }
        //    else
        //    {
        //        instance = this as T;
        //        DontDestroyOnLoad(this.gameObject);
        //    }
        //}
    }
}
