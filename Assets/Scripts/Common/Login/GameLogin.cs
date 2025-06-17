using System.Collections.Generic;
using System.Text;
using Common.GameRoot;
using Data;
using Data.ConfigData;
using GamePlay.Globa;
using Platform;
using UnityEngine;

namespace Common.Login
{
    public class GameLogin : MonoBehaviour
    {
        /** 加载进度条 */
        private GameObject _loadProgressBar;
        
        /** 用户渠道 */
        private string userChannel;
        
        /** 配置表读取完成 */
        private bool _configLoadComplete;
        
        /** 游戏已经开始 */
        private bool _beginStart;

        /// <summary>
        /// 游戏准备开始
        /// </summary>
        /// <param name="userChannelTmp">当前运行环境</param>
        internal void ReadyStart(string userChannelTmp)
        {
            _loadProgressBar = GameObject.Find("/CanvasMain/ProgressBG").gameObject;
            _loadProgressBar.SetActive(false);
            
            _configLoadComplete = false;
            _beginStart = false;
            DataHelper.isRootLoad = true;
            
            userChannel = userChannelTmp;
            
            // 配置表读取
            ConfigManager.Instance.GetJsonConfig("MainConfig", () =>
            {
                _configLoadComplete = true;
                GameSdkManager._instance._sdkScript.PlatformLogin(GameStart);
            });
        }

        /// <summary>
        /// 游戏运行环境初始化完成
        /// </summary>
        /// <param name="openIdTmp">用户唯一ID</param>
        private void GameStart(string openIdTmp)
        {
            GameRootManager._instance.InitTouch();
            
            Debug.Log(new StringBuilder("OpenId:" + openIdTmp).ToString());
            Debug.Log(new StringBuilder("UserChannel:" + userChannel).ToString());
            
            DataHelper.CurOpenId = openIdTmp;
            DataHelper.CurUserChannel = userChannel;
            
            StartGame();
        }

        /// <summary>
        /// 游戏开始
        /// </summary>
        private void StartGame()
        {
            DataHelper.ProvinceRanks = new Dictionary<string, int>();
            for (int i = 0; i < ConfigManager.Instance.provinceConfigs.Count; i++)
            {
                ProvinceConfig config = ConfigManager.Instance.provinceConfigs[i];
                DataHelper.ProvinceRanks.Add(config.name, 0);
            }

            GameSdkManager._instance._serverScript.InitUserData((str) => { });
        }
        
        private void Update()
        {
            if (_beginStart) return;
            if (!_configLoadComplete) return;
            if (GameGlobalManager._instance._globaCanvas != null && GameGlobalManager._instance._globalOpenBox != null)
            {
                _beginStart = true;
                _loadProgressBar.SetActive(true);
            }
        }
    }
}