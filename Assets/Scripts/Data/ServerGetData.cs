using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using Random = UnityEngine.Random;

namespace Data
{
    /// <summary>
    /// 远程访问管理类
    /// </summary>
    public static class ServerGetData
    {
        /// <summary>
        /// 加载远程图片
        /// </summary>
        /// <param name="url">远程图片地址</param>
        /// <param name="cb">回调</param>
        public static IEnumerator GetRemoteImg(string url, Action<Sprite> cb)
        {
            using var request = UnityWebRequest.Get(url);
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                var texture = new Texture2D(200, 200);
                texture.LoadImage(request.downloadHandler.data);
                var sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                cb(sprite);
            }
            else
            {
                Debug.LogError(new StringBuilder("error = " + request.error).ToString());
            }
        }
        
        /// <summary>
        /// 服务器通讯
        /// </summary>
        /// <param name="url"></param>
        /// <param name="json"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public static IEnumerator RequestServer(string url, string json, Action<string, bool> callback)
        {
            UnityWebRequest www = new UnityWebRequest(url, UnityWebRequest.kHttpVerbPOST);
            www.disposeDownloadHandlerOnDispose = true;
            www.disposeUploadHandlerOnDispose = true;
            www.SetRequestHeader("Refere", "http://127.0.0.1");
            www.SetRequestHeader("Content-Type", "application/json");
            byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new DownloadHandlerBuffer();
            yield return www.SendWebRequest();
            if (www.result == UnityWebRequest.Result.Success)
            {
                callback?.Invoke(www.downloadHandler.text, true);
            }
            else
            {
                callback?.Invoke(www.error, false);
                Debug.LogError("服务器请求出错: " + www.error);
            }
            www.Dispose();
        }
        
        /// <summary>
        /// 读取远程配置
        /// </summary>
        /// <param name="configName">配置名称</param>
        /// <param name="cb">回调</param>
        public static IEnumerator LoadRemoteConfig(string configName, Action<string> cb)
        {
            string url = new StringBuilder(GlobalValueManager.RemoteConfigUrl + configName + ".json?" + Random.Range(0, 1000000)).ToString();
            UnityWebRequest www = UnityWebRequest.Get(url);
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                ConfigManager.Instance.ConsoleLog(2, new StringBuilder("远程配置[" + configName + "]加载失败:" + www.error).ToString());
                cb(null);
            }
            else
            {
                ConfigManager.Instance.ConsoleLog(0, new StringBuilder("远程配置[" + configName + "]加载成功").ToString());
                cb(www.downloadHandler.text);
            }
        }
        
        /// <summary>
        /// 读取远程配置
        /// </summary>
        /// <param name="configName">配置名称</param>
        /// <param name="cb">回调</param>
        public static IEnumerator LoadRemoteConfigPublic(string configName, Action<string> cb)
        {
            string url = new StringBuilder(GlobalValueManager.RemoteConfigUrl_Public + configName + ".json?" + Random.Range(0, 1000000)).ToString();
            UnityWebRequest www = UnityWebRequest.Get(url);
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                ConfigManager.Instance.ConsoleLog(2, new StringBuilder("远程配置[" + configName + "]加载失败:" + www.error).ToString());
                cb(null);
            }
            else
            {
                ConfigManager.Instance.ConsoleLog(0, new StringBuilder("远程配置[" + configName + "]加载成功").ToString());
                cb(www.downloadHandler.text);
            }
        }
    }
}