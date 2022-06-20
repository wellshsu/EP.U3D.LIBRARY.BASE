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
using System.IO;
using System.Net.NetworkInformation;
using UnityEngine;
using System.Collections.Generic;

namespace EP.U3D.LIBRARY.BASE
{
    public class Constants
    {
        public static string PREF_STEAMING_FILE = STREAMING_PATH + "PREF.txt";

        public static string PLAT_STEAMING_FILE = STREAMING_PATH + "PLAT.txt";

        public static string PROJ_NAME = "EFrame";

        public static string APP_NAME = "EFrame";

        public static string CHANNEL_NAME = "eframe";

        public static string JSON_URL = "http://localhost";

        public static bool CHECK_MODE;

        public static bool RELEASE_MODE;

        public static bool LIVE_MODE;

        public static string JSON_FILE
        {
            get
            {
                string platform = PLATFORM_NAME;
                if (Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.OSXPlayer || Application.isEditor)
                {
                    platform = "Android";
                }
                string url = Helper.StringFormat("{0}/json/{1}/{2}/{3}/config.json?t={4}", JSON_URL, CHANNEL_NAME, platform, BINARY_VERSION, DateTime.Now.Ticks);
                return url;
            }
        }

        public static string JSON_DATA { get; set; }

        public static string CONN_SERVER_IP;

        public static int CONN_SERVER_PORT;

        public static int CONN_SERVER_UID;

        public static string CGI_SERVER_URL;

        public static string CGI_ACCESS_TOKEN;

        public static string CGI_REFRESH_TOKEN;

        public static int CGI_SERVER_UID;

        public static string BINARY_FILE_URL;

        public static int BINARY_FILE_SIZE;

        public static string REPORT_URL; // 问题反馈地址

#if EFRAME_ILR || EFRAME_LUA
        public static bool SCRIPT_BUNDLE_MODE = Application.isEditor ? Preferences.Instance.ScriptBundle : true;
#else
        public static bool SCRIPT_BUNDLE_MODE = false;
#endif

#if EFRAME_ILR || EFRAME_LUA
        public static bool ASSET_BUNDLE_MODE = Application.isEditor ? Preferences.Instance.AssetBundle : true;
#else
        public static bool ASSET_BUNDLE_MODE = false;
#endif
        public static bool FORCE_UPDATE { get; set; }

        public static bool CHECK_UPDATE { get; set; }

        public static List<string> UPDATE_WHITELIST { get; set; }

        public static List<string> LOG_WHITELIST { get; set; }

        public static string NEWEST_VERSION;

        public static string BINARY_VERSION = Application.version;

        public static string LOCAL_VERSION = $"{BINARY_VERSION}.{PATCH_VERSION}";

        public static string PATCH_VERSION
        {
            get
            {
                string versionFile = Helper.StringFormat("{0}{1}", CONFIG_PATH, BINARY_VERSION);
                if (File.Exists(versionFile))
                {
                    try
                    {
                        string[] lines = File.ReadAllLines(versionFile);
                        string versionStr = lines[0];
                        return versionStr;
                    }
                    catch
                    {
                        return "0";
                    }
                }
                else
                {
                    return "0";
                }
            }
            set
            {
                try
                {
                    string versionFile = Helper.StringFormat("{0}{1}", CONFIG_PATH, BINARY_VERSION);
                    if (File.Exists(versionFile) == false)
                    {
                        File.Delete(versionFile);
                    }
                    using (var file = File.Open(versionFile, FileMode.Create))
                    {
                        StreamWriter sw = new StreamWriter(file);
                        sw.WriteLine(value);
                        sw.Close();
                        file.Close();
                    }
                }
                catch { }
            }
        }

        public static string DATA_PATH
        {
            get
            {
                if (Application.isEditor)
                {
                    string path = Helper.StringFormat("{0}Local/", DOCS_PATH);
                    if (Directory.Exists(path) == false)
                    {
                        Helper.CreateDirectory(path);
                    }
                    return path;
                }
                else if (Application.platform == RuntimePlatform.WindowsPlayer)
                {
                    int i = Application.streamingAssetsPath.LastIndexOf('/');
                    string path = Application.streamingAssetsPath.Substring(0, i + 1) + "Local/";
                    if (Directory.Exists(path) == false)
                    {
                        Helper.CreateDirectory(path);
                    }
                    return path;
                }
                else
                {
                    return Application.persistentDataPath + "/";
                }
            }
        }

        public static string CONFIG_PATH
        {
            get
            {
                string path = DATA_PATH + "Config/";
                if (Directory.Exists(path) == false)
                {
                    Helper.CreateDirectory(path);
                }
                return path;
            }
        }

        public static string LOG_PATH
        {
            get
            {
                string path = DATA_PATH + "Log/";
                if (Directory.Exists(path) == false)
                {
                    Helper.CreateDirectory(path);
                }
                return path;
            }
        }

        public static string TEMP_PATH
        {
            get
            {
                string path = DATA_PATH + "Temp/";
                if (Directory.Exists(path) == false)
                {
                    Helper.CreateDirectory(path);
                }
                return path;
            }
        }

        public static string PROJ_PATH
        {
            get
            {
                string path = Application.dataPath.Substring(0, Application.dataPath.LastIndexOf('/') + 1);
                return path;
            }
        }

        public static string DOCS_PATH
        {
            get
            {
                string path = Application.dataPath.Substring(0, Application.dataPath.IndexOf("/Assets")) + "/Docs/";
                if (Directory.Exists(path) == false)
                {
                    Helper.CreateDirectory(path);
                }
                return path;
            }
        }

        public static string ASSET_BUNDLE_MANIFEST_FILE = "Assets";


        public static string MANIFEST_FILE = "manifest.txt";

        public static string STREAMING_PATH
        {
            get
            {
                string path;
                if (Application.isEditor)
                {
                    path = Application.dataPath + "/StreamingAssets/";
                }
                else
                {
                    switch (Application.platform)
                    {
                        case RuntimePlatform.Android:
                            path = "jar:file://" + Application.dataPath + "!/assets/";
                            break;
                        case RuntimePlatform.IPhonePlayer:
                            path = Application.dataPath + "/Raw/";
                            break;
                        default:
                            path = Application.dataPath + "/StreamingAssets/";
                            break;
                    }
                }
                return path;
            }
        }

        public static string PLATFORM_NAME
        {
            get
            {
#if UNITY_STANDALONE
                return "Windows";
#elif UNITY_ANDROID
                return "Android";
#elif UNITY_IPHONE
                return "IOS";
#else
                return string.Empty;        
#endif
            }
        }

        public const string ASSET_BUNDLE_FILE_EXTENSION = ".asset";

        public const string ILR_BUNDLE_FILE_EXTENSION = ".ilr";

        public const string LUA_BUNDLE_FILE_EXTENSION = ".lua";

        public static string STREAMING_ASSET_BUNDLE_PATH = STREAMING_PATH + "Assets/";

        public static string STREAMING_SCRIPT_BUNDLE_ROOT = STREAMING_PATH + "Scripts/";

        public static string STREAMING_ILR_BUNDLE_PATH = STREAMING_PATH + "Scripts/ILR/";

        public static string STREAMING_LUA_BUNDLE_PATH = IntPtr.Size == 4 ? STREAMING_PATH + "Scripts/LUA/x86/" : STREAMING_PATH + "Scripts/LUA/x64/";

        public static string LOCAL_ASSET_BUNDLE_PATH = DATA_PATH + "Assets/";  // 本地资源包目录

        public static string LOCAL_ILR_BUNDLE_PATH = DATA_PATH + "Scripts/ILR/";  // 本地ILR脚本目录

        public static string LOCAL_LUA_BUNDLE_PATH = DATA_PATH + "Scripts/LUA/";   // 本地LUA脚本目录

        public static string REMOTE_ASSET_BUNDLE_PATH; // 远端资源目录

        public static string REMOTE_ILR_BUNDLE_PATH; // 远端ILR脚本目录

        public static string REMOTE_LUA_BUNDLE_PATH; // 远端LUA脚本目录

        public static string REMOTE_FILE_BUNDLE_ROOT; // 远端文件根目录

        public static string DEVICE_ID { get { return SystemInfo.deviceUniqueIdentifier; } }

        public static string MAC_ADDRESS
        {
            get
            {
                try
                {

                    NetworkInterface[] nis = NetworkInterface.GetAllNetworkInterfaces();
                    if (nis.Length <= 0)
                    {
                        return "no mac address.";
                    }
                    else
                    {
                        return nis[0].GetPhysicalAddress().ToString();
                    }
                }
                catch
                {
                    return "get mac address error.";
                }
            }
        }
    }
}
