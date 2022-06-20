//---------------------------------------------------------------------//
//                    GNU GENERAL PUBLIC LICENSE                       //
//                       Version 2, June 1991                          //
//                                                                     //
// Copyright (C) Wells Hsu, wellshsu@outlook.com, All rights reserved. //
// Everyone is permitted to copy and distribute verbatim copies        //
// of this license document, but changing it is not allowed.           //
//                  SEE LICENSE.md FOR MORE DETAILS.                   //
//---------------------------------------------------------------------//
using System;
using System.Collections;
using UnityEngine;

namespace EP.U3D.LIBRARY.BASE
{
    [Serializable]
    public class Platform
    {
        private static Platform mInstance;
        public static Platform Instance
        {
            get
            {
                if (mInstance == null) mInstance = new Platform(Constants.PLAT_STEAMING_FILE);
                return mInstance;
            }
            set { mInstance = value; }
        }

        [SerializeField] public string Project = string.Empty;
        [SerializeField] public string AppName = string.Empty;
        [SerializeField] public string Channel = string.Empty;
        [SerializeField] public string Version = string.Empty;
        [SerializeField] public string JsonUrl = string.Empty;

        [NonSerialized] public string Error;

#if UNITY_EDITOR
        [UnityEditor.InitializeOnLoadMethod]
        private static void OnLoad()
        {
            Instance = new Platform(Constants.PLAT_STEAMING_FILE);
        }
#endif

        public Platform(string path = null, Action callback = null)
        {
            Read(path, callback);
        }

        public virtual void Read(string path, Action callback = null)
        {
            if (Application.platform == RuntimePlatform.Android)
            {
                Loom.StartCR(ReadCO(path, callback));
            }
            else
            {
                try
                {
                    JsonUtility.FromJsonOverwrite(Helper.OpenText(path), this);
                }
                catch (Exception e)
                {
                    Error = e.ToString();
                }
                if (callback != null)
                {
                    callback();
                }
            }
        }

        private IEnumerator ReadCO(string path, Action callback)
        {
            Error = string.Empty;
            using (WWW www = new WWW(path))
            {
                yield return www;
                if (string.IsNullOrEmpty(www.error) && www.isDone)
                {
                    try
                    {
                        JsonUtility.FromJsonOverwrite(www.text, this);
                    }
                    catch (Exception e)
                    {
                        Error = e.ToString();
                    }
                    if (callback != null)
                    {
                        callback();
                    }
                    yield return 0;
                }
                else
                {
                    Error = "www error is " + www.error;
                    if (callback != null)
                    {
                        callback();
                    }
                    yield return null;
                }
            }
        }
    }
}