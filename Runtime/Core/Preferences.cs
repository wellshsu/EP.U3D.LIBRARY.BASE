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
using System.Collections.Generic;
using UnityEngine;

namespace EP.U3D.LIBRARY.BASE
{
    [Serializable]
    public class Preferences
    {
        private static Preferences mInstance;
        public static Preferences Instance
        {
            get
            {
                if (mInstance == null) mInstance = new Preferences(Constants.PREF_STEAMING_FILE);
                return mInstance;
            }
            set { mInstance = value; }
        }

        #region Target
        [SerializeField] public string Name = string.Empty;
        [NonSerialized] public string Path = string.Empty;
        #endregion

        #region Editor
        [SerializeField] public string Developer = string.Empty;
        [SerializeField] public string ListenLog = string.Empty;
        [SerializeField] public string PushPatch = string.Empty;
        [SerializeField] public int SWidth = 960;
        [SerializeField] public int SHeight = 540;
        [SerializeField] public bool AssetBundle;
        [SerializeField] public bool ScriptBundle;
        [SerializeField] public bool Pauseable;
        [SerializeField] public bool CheckMode;
        [SerializeField] public bool CatVerbose;
        [SerializeField] public bool CatException;
        #endregion

        #region Runtime
        [SerializeField] public bool ReleaseMode;
        [SerializeField] public bool LiveMode;
        [SerializeField] public bool CheckUpdate;
        [SerializeField] public bool ForceUpdate;
        [SerializeField] public string LogServer = string.Empty;
        [SerializeField] public string PatchServer = string.Empty;
        [SerializeField] public int CgiIndex;
        [SerializeField] public List<string> CgiServer = new List<string> { "NONE" };
        [SerializeField] public int ConnIndex;
        [SerializeField] public List<string> ConnServer = new List<string> { "NONE" };
        #endregion

        [NonSerialized] public string Error;

#if UNITY_EDITOR
        [UnityEditor.InitializeOnLoadMethod]
        private static void OnLoad()
        {
            Instance = new Preferences(Constants.PREF_STEAMING_FILE);
        }
#endif

        public Preferences(string path = null, Action callback = null)
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