using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KYM
{
    public class SingletonBase<T> : MonoBehaviour where T : class
    {
        public static T Singleton
        {
            get
            {
                return _instance.Value;
            }
        }

        private static readonly Lazy<T> _instance =
            new Lazy<T>(() => 
            {
                T instance = UnityEngine.Object.FindObjectOfType(typeof(T)) as T;

                if (instance == null)
                {
                    GameObject obj = new GameObject(typeof(T).ToString());
                    instance = obj.AddComponent(typeof(T)) as T;
#if UNITY_EDITOR
                    if (UnityEditor.EditorApplication.isPlaying)
                    {
                        DontDestroyOnLoad(obj); // 씬 전환 시 파괴되지 않도록 설정
                    }
#else
                    DontDestroyOnLoad(obj);
#endif
                }
                return instance;
            });

        protected virtual void Awake() 
        {
            DontDestroyOnLoad(gameObject);
        }
    }
}
