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
using System.Text;
using System.Security.Cryptography;
using UnityEngine;
using System.Runtime.InteropServices;

namespace EP.U3D.LIBRARY.BASE
{
    public class Helper
    {
        private static StringBuilder strBuilder = new StringBuilder();

        public static string StringFormat(string format, params object[] args)
        {
            try
            {
                if (strBuilder.Length > 0)
                {
                    strBuilder.Remove(0, strBuilder.Length);
                }
                strBuilder.AppendFormat(format, args);
                return strBuilder.ToString();
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError("format error: " + e.Message);
                return string.Empty;
            }
        }

        public static long VersionToNumber(string version)
        {
            string[] strs = version.Split('.');
            if (strs == null || strs.Length == 0)
            {
                return -1;
            }
            else
            {
                long finalVersion = 0;
                int large = (strs.Length - 1) * 4;
                for (int i = 0; i < strs.Length; i++)
                {
                    int singleVersion = 0;
                    int.TryParse(strs[i], out singleVersion);
                    if (i == 0)
                    {
                        finalVersion = (large == 0 ? singleVersion : singleVersion * (long)(Math.Pow(10, large)));
                    }
                    else if (i == strs.Length - 1)
                    {
                        finalVersion += singleVersion;
                    }
                    else
                    {
                        finalVersion += singleVersion * (int)(Math.Pow(10, large - i * 4));
                    }
                }
                return finalVersion;
            }
        }

        public static string NumberToVersion(long version)
        {
            string finalVersion = string.Empty;
            string str = version.ToString();
            int singleVersion = 0;
            for (int i = str.Length - 1; i >= 0;)
            {
                int length = (i - 1) >= 0 ? 4 : 1;
                int from = i - length + 1;
                int.TryParse(str.Substring(from, length), out singleVersion);
                finalVersion = singleVersion + finalVersion;
                if (i > 3)
                {
                    finalVersion = "." + finalVersion;
                }
                i -= 4;
            }
            return finalVersion;
        }

        public static Vector3 StrToVec3(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return Vector3.zero;
            }
            string v = str.Substring(1, str.Length - 2);
            string[] values = v.Split(new string[] { "," }, StringSplitOptions.None);
            if (values.Length == 3)
            {
                return new Vector3(Convert.ToSingle(values[0]), Convert.ToSingle(values[1]), Convert.ToSingle(values[2]));
            }
            return Vector3.zero;
        }

        public static Vector4 StrToVec4(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return Vector3.zero;
            }
            string v = str.Substring(1, str.Length - 2);
            string[] values = v.Split(new string[] { "," }, StringSplitOptions.None);
            if (values.Length == 4)
            {
                return new Vector4(Convert.ToSingle(values[0]), Convert.ToSingle(values[1]), Convert.ToSingle(values[2]), Convert.ToSingle(values[3]));
            }
            return Vector4.zero;
        }

        public static string Vec3ToStr(Vector3 vec)
        {
            return StringFormat("({0},{1},{2})", vec.x, vec.y, vec.z);
        }

        public static string Vec4ToStr(Vector4 vec)
        {
            return StringFormat("({0},{1},{2},{3})", vec.x, vec.y, vec.z, vec.w);
        }

        public static string ColorToStr(Color32 color)
        {
            string tem = "({0},{1},{2},{3})";
            tem = StringFormat(tem, color.r, color.g, color.b, color.a);
            return tem;
        }

        public static Color StrToColor(string str)
        {
            if (string.IsNullOrEmpty(str)) return Color.black;
            string v = str.Substring(1, str.Length - 2);
            string[] values = v.Split(new string[] { "," }, StringSplitOptions.None);
            if (values.Length == 4)
            {
                try
                {
                    float r, g, b, a;
                    r = Convert.ToSingle(values[0]);
                    g = Convert.ToSingle(values[1]);
                    b = Convert.ToSingle(values[2]);
                    a = Convert.ToSingle(values[3]);
                    r /= 255.0f;
                    g /= 255.0f;
                    b /= 255.0f;
                    a /= 255.0f;
                    return new Color(r, g, b, a);
                }
                catch (Exception e)
                {
                    LogError("Helper.StrToColor: convert to color error:" + e.ToString());
                    return Color.black;
                }
            }
            return Color.black;
        }

        public static void Log(object format, params object[] args)
        {
            HandleLog(format, LogType.Log, args);
        }

        public static void LogError(object format, params object[] args)
        {
            HandleLog(format, LogType.Error, args);
        }

        public static void LogWarning(object format, params object[] args)
        {
            HandleLog(format, LogType.Warning, args);
        }

        private static void HandleLog(object format, LogType type, params object[] args)
        {
            string log = null;
            if (format != null && !(format is bool))
            {
                if (format is string && args != null && args.Length > 0)
                {
                    log = StringFormat(format as string, args);
                }
                else
                {
                    log = format.ToString();
                }
            }
            if (string.IsNullOrEmpty(log) == false)
            {
                if (type == LogType.Log)
                {
                    UnityEngine.Debug.Log(log);
                }
                else if (type == LogType.Warning)
                {
                    UnityEngine.Debug.LogWarning(log);
                }
                else if (type == LogType.Error)
                {
                    UnityEngine.Debug.LogError(log);
                }
            }
        }

        public static string FilePathToMD5(string path)
        {
            try
            {
                byte[] bytes = File.ReadAllBytes(path);
                MD5 md5 = new MD5CryptoServiceProvider();
                byte[] md5Bytes = md5.ComputeHash(bytes);
                md5.Clear();
                StringWriter sw = new StringWriter();
                for (int i = 0; i < md5Bytes.Length; i++)
                {
                    sw.Write(md5Bytes[i].ToString("x").PadLeft(2, '0'));
                }
                return sw.ToString();
            }
            catch (Exception e)
            {
                LogError("Helper.FilePathToMD5: error: {0}.", e.Message);
                return null;
            }
        }

        public static string GetRootPath(Transform targetTransform, Transform startTransform, string rootPath)
        {
            if (targetTransform == null || startTransform == null)
            {
                return string.Empty;
            }
            if (targetTransform != startTransform)
            {
                if (string.IsNullOrEmpty(rootPath))
                {
                    rootPath = startTransform.name;
                }
                else
                {
                    rootPath = startTransform.name + "/" + rootPath;
                }
                return GetRootPath(targetTransform, startTransform.parent, rootPath);
            }
            else
            {
                return rootPath;
            }
        }

        public static string FileMD5(string filePath)
        {
            if (File.Exists(filePath) == false)
            {
                return string.Empty;
            }
            try
            {
                FileStream fs = new FileStream(filePath, FileMode.Open);
                MD5 md5 = new MD5CryptoServiceProvider();
                byte[] retVal = md5.ComputeHash(fs);
                fs.Close();

                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < retVal.Length; i++)
                {
                    sb.Append(retVal[i].ToString("x2"));
                }
                return sb.ToString();
            }
            catch
            {
                return string.Empty;
            }
        }

        public static int FileSize(string filePath)
        {
            if (File.Exists(filePath) == false)
            {
                return 0;
            }
            try
            {
                FileStream fs = new FileStream(filePath, FileMode.Open);
                int length = (int)fs.Length;
                fs.Close();
                return length;
            }
            catch
            {
                return 0;
            }
        }

        public static string StrMD5(string str)
        {
            byte[] bytes = Encoding.Default.GetBytes(str);
            return BytesMD5(bytes);
        }

        public static string BytesMD5(byte[] bytes)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] retVal = md5.ComputeHash(bytes);

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < retVal.Length; i++)
            {
                sb.Append(retVal[i].ToString("x2"));
            }
            return sb.ToString();
        }

        public static bool HasFile(string path)
        {
            return File.Exists(path);
        }

        public static string OpenText(string path)
        {
            return Encoding.UTF8.GetString(OpenFile(path));
        }

        public static byte[] OpenFile(string path)
        {
            byte[] bytes = new byte[0];
            try
            {
                if (File.Exists(path) == false)
                {
                    return bytes;
                }
                using (var file = File.OpenRead(path))
                {
                    if (file != null)
                    {
                        bytes = new byte[file.Length];
                        file.Read(bytes, 0, (int)file.Length);
                        file.Close();
                        file.Dispose();
                        return bytes;
                    }
                }
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError(e.Message);
            }
            return bytes;
        }

        public static bool SaveText(string path, string content)
        {
            return SaveFile(path, Encoding.UTF8.GetBytes(content));
        }

        public static bool SaveFile(string path, byte[] buffer)
        {
            try
            {
                if (string.IsNullOrEmpty(path) || buffer == null)
                {
                    return false;
                }
                string directory = path.Substring(0, path.IndexOf(Path.GetFileName(path)));
                if (Directory.Exists(directory) == false)
                {
                    Directory.CreateDirectory(directory);
                }
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
                using (var file = File.Open(path, FileMode.CreateNew))
                {
                    if (file != null)
                    {
                        file.Write(buffer, 0, buffer.Length);
                        file.Close();
                        file.Dispose();
                        return true;
                    }
                }
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError(e.Message);
            }
            return false;
        }

        public static void DeleteFile(string path)
        {
            try
            {
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError(e.Message);
            }
        }

        public static void CopyFile(string src, string dst, bool overwrite = true)
        {
            File.Copy(src, dst, overwrite);
        }

        public static bool HasDirectory(string path)
        {
            return Directory.Exists(path);
        }

        public static bool DeleteDirectory(string path, bool recursive = true)
        {
            bool result = false;
            try
            {
                if (Directory.Exists(path) == true)
                {
                    Directory.Delete(path, recursive);
                }
                result = true;
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError(e.Message);
            }
            return result;
        }

        public static void CreateDirectory(string path)
        {
            try
            {
                Directory.CreateDirectory(path);
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError(e.Message);
            }
        }

        public static void CopyDirectory(string src, string dest, params string[] exclude)
        {
            try
            {
                string[] paths = Directory.GetFileSystemEntries(src);
                for (int i = 0; i < paths.Length; i++)
                {
                    string path = paths[i];
                    path = path.Replace("//", "/");
                    path = path.Replace("\\", "/");
                    bool copy = true;
                    for (int j = 0; j < exclude.Length; j++)
                    {
                        string ext = exclude[j];
                        if (path.EndsWith(ext))
                        {
                            copy = false;
                            break;
                        }
                    }
                    if (copy)
                    {
                        if (Directory.Exists(path))
                        {
                            string folder = path.Substring(path.LastIndexOf("/") + 1);
                            CopyDirectory(path, dest + folder + "/", exclude);
                        }
                        else
                        {
                            string file = path.Substring(src.Length);
                            file = dest + file;
                            if (Directory.Exists(Path.GetDirectoryName(file)) == false)
                            {
                                Directory.CreateDirectory(Path.GetDirectoryName(file));
                            }
                            File.Copy(path, file, true);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError(e.Message);
            }
        }

        public static AssetBundle LoadAssetBundle(string path)
        {
            return AssetBundle.LoadFromFile(path);
        }

        public static AssetBundleCreateRequest LoadAssetBundleAsync(string path)
        {
            return AssetBundle.LoadFromFileAsync(path);
        }

        public static UnityEngine.Object LoadAssetFromBundle(string assetName, Type type, AssetBundle bundle)
        {
            if (bundle)
            {
                return bundle.LoadAsset(assetName, type);
            }
            return null;
        }

        private static byte[] RGBIV = { 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF };

        public static string EncryptString(string str, string key = "")
        {
            try
            {
                byte[] rgb = string.IsNullOrEmpty(key) ? RGBIV : Encoding.UTF8.GetBytes(key);
                byte[] arr = Encoding.UTF8.GetBytes(str);
                DESCryptoServiceProvider dcsp = new DESCryptoServiceProvider();
                MemoryStream ms = new MemoryStream();
                CryptoStream cs = new CryptoStream(ms, dcsp.CreateEncryptor(rgb, RGBIV), CryptoStreamMode.Write);
                cs.Write(arr, 0, arr.Length);
                cs.FlushFinalBlock();
                cs.Close();
                return Convert.ToBase64String(ms.ToArray());
            }
            catch
            {
                return str;
            }
        }

        public static string DecryptString(string str, string key = "")
        {
            try
            {
                byte[] rgb = string.IsNullOrEmpty(key) ? RGBIV : Encoding.UTF8.GetBytes(key);
                byte[] arr = Convert.FromBase64String(str);
                DESCryptoServiceProvider dcsp = new DESCryptoServiceProvider();
                MemoryStream ms = new MemoryStream();
                CryptoStream cs = new CryptoStream(ms, dcsp.CreateDecryptor(rgb, RGBIV), CryptoStreamMode.Write);
                cs.Write(arr, 0, arr.Length);
                cs.FlushFinalBlock();
                cs.Close();
                return Encoding.UTF8.GetString(ms.ToArray());
            }
            catch
            {
                return str;
            }
        }

        public static byte[] EncryptBytes(byte[] src, string key = "")
        {
            try
            {
                byte[] rgb = string.IsNullOrEmpty(key) ? RGBIV : Encoding.UTF8.GetBytes(key);
                DESCryptoServiceProvider dcsp = new DESCryptoServiceProvider();
                MemoryStream ms = new MemoryStream();
                CryptoStream cs = new CryptoStream(ms, dcsp.CreateEncryptor(rgb, RGBIV), CryptoStreamMode.Write);
                cs.Write(src, 0, src.Length);
                cs.FlushFinalBlock();
                cs.Close();
                return ms.ToArray();
            }
            catch
            {
                return null;
            }
        }

        public static byte[] DecryptBytes(byte[] src, string key = "")
        {
            try
            {
                byte[] rgb = string.IsNullOrEmpty(key) ? RGBIV : Encoding.UTF8.GetBytes(key);
                DESCryptoServiceProvider dcsp = new DESCryptoServiceProvider();
                MemoryStream ms = new MemoryStream();
                CryptoStream cs = new CryptoStream(ms, dcsp.CreateDecryptor(rgb, RGBIV), CryptoStreamMode.Write);
                cs.Write(src, 0, src.Length);
                cs.FlushFinalBlock();
                cs.Close();
                return ms.ToArray();
            }
            catch
            {
                return null;
            }
        }

        public static int GenLayerMask(params string[] layers)
        {
            int mask = 0;
            for (int i = 0; i < layers.Length; i++)
            {
                mask |= 1 << LayerMask.NameToLayer(layers[i]);
            }
            return mask;
        }

        public static long ToTimestamp(DateTime time)
        {
            DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            return (long)(time - startTime).TotalSeconds;
        }

        public static byte[] StructToByte<T>(T obj)
        {
            int size = Marshal.SizeOf(typeof(T));
            byte[] buffer = new byte[size];
            IntPtr bufferIntPtr = Marshal.AllocHGlobal(size);
            try
            {
                Marshal.StructureToPtr(obj, bufferIntPtr, true);
                Marshal.Copy(bufferIntPtr, buffer, 0, size);
            }
            finally
            {
                Marshal.FreeHGlobal(bufferIntPtr);
            }
            return buffer;
        }

        public static T ByteToStruct<T>(byte[] bytes)
        {
            object obj = null;
            int size = Marshal.SizeOf(typeof(T));
            IntPtr allocIntPtr = Marshal.AllocHGlobal(size);
            try
            {
                Marshal.Copy(bytes, 0, allocIntPtr, size);
                obj = Marshal.PtrToStructure(allocIntPtr, typeof(T));
            }
            finally
            {
                Marshal.FreeHGlobal(allocIntPtr);
            }
            return (T)obj;
        }
    }
}