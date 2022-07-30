using UnityEngine;
using System;

namespace LeeFramework.Web
{
    public class WebMono : MonoBehaviour
    {
        private static WebMono _Instance;
        private static GameObject _GameObject;
        public static WebMono instance
        {
            get
            {
                if (_Instance == null)
                {
                    _GameObject = new GameObject(typeof(WebMono).ToString());
                    _GameObject.hideFlags = HideFlags.HideAndDontSave;
                    DontDestroyOnLoad(_GameObject);
                    _Instance = _GameObject.AddComponent<WebMono>();
                }
                return _Instance;
            }
        }

        public Action callback = null;

        private void Awake()
        {
            if (_Instance == null)
            {
                gameObject.name = (typeof(WebMono).ToString());
                DontDestroyOnLoad(this);
                if (gameObject.GetComponent<WebMono>() == null)
                {
                    _Instance = gameObject.AddComponent<WebMono>();
                }
                else
                {
                    _Instance = gameObject.GetComponent<WebMono>();
                }
            }
            else
            {
                Destroy(this);
            }
        }


        private void Update()
        {
            callback?.Invoke();
        }

        public virtual void Dispose()
        {
            _Instance = default;
            GameObject.Destroy(_GameObject.gameObject);
            GC.Collect();
        }

    } 
}

