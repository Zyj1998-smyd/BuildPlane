using System;
using System.Collections.Generic;
using System.Text;
using Common.GameRoot.AudioHandler;
using Data;
using GamePlay.Globa;
using GamePlay.Main;
using Platform;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GamePlay.Module.Set
{
    public class OpenSetPageUi : MonoBehaviour
    {
        /** 音乐开关 */
        private GameObject _btnSwitchOn_Music, _btnSwitchOff_Music;
        /** 音效开关 */
        private GameObject _btnSwitchOn_Effect, _btnSwitchOff_Effect;
        /** 震动开关 */
        private GameObject _btnSwitchOn_Vibrate, _btnSwitchOff_Vibrate;
        /** 低能耗模式开关 */
        private GameObject _btnSwitchOn_Quality, _btnSwitchOff_Quality;

        /** 音乐开/关 */
        private bool _musicOn;
        /** 音效开/关 */
        private bool _effectOn;
        /** 震动开/关 */
        private bool _vibrateOn;
        /** 低能耗模式开/关 */
        private bool _qualityOn;

        /** 清空数据弹窗 */
        private GameObject _clearDataUi;

        /** UID */
        private TextMeshProUGUI _uidText;
        
        public void OpenTanChuang()
        {
            AudioHandler._instance.PlayAudio(GameGlobalManager._instance.audioPopOpen);
            _musicOn = !AudioHandler._instance.musicSwitch;
            _effectOn = !AudioHandler._instance.audioSwitch;
            _vibrateOn = !AudioHandler._instance.vibrateSwitch;
            _qualityOn = !AudioHandler._instance.qualitySwitch;
            OnBtnSwitchMusic(0);
            OnBtnSwitchEffect(0);
            OnBtnSwitchVibrate(0);
            OnBtnSwitchQuality(0);

            _uidText.text = new StringBuilder("UID:" + DataHelper.CurOpenId).ToString();
        }

        private void CloseTanChuang()
        {
            AudioHandler._instance.PlayAudio(GameGlobalManager._instance.audioPopClose);
            MainManager._instance.OnOpenPop_Set(false);
        }
        
        // ----------------------------------------------- 按钮 -----------------------------------------------
        /// <summary>
        /// 按钮 音乐开关
        /// </summary>
        /// <param name="type">0:调用 1:点击</param>
        private void OnBtnSwitchMusic(int type)
        {
            if (type == 1) AudioHandler._instance.PlayAudio(GameGlobalManager._instance.audioBtnClick);
            _musicOn = !_musicOn;
            _btnSwitchOff_Music.SetActive(!_musicOn);
            _btnSwitchOn_Music.SetActive(_musicOn);
            if (type == 1)
            {
                AudioHandler._instance.ModifyAudioSet(0, _musicOn);
                AudioHandler._instance.InitAudioSet(0);
            }
        }

        /// <summary>
        /// 按钮 音效开关
        /// </summary>
        /// <param name="type">0:调用 1:点击</param>
        private void OnBtnSwitchEffect(int type)
        {
            if (type == 1) AudioHandler._instance.PlayAudio(GameGlobalManager._instance.audioBtnClick);
            _effectOn = !_effectOn;
            _btnSwitchOff_Effect.SetActive(!_effectOn);
            _btnSwitchOn_Effect.SetActive(_effectOn);
            if (type == 1)
            {
                AudioHandler._instance.ModifyAudioSet(1, _effectOn);
                AudioHandler._instance.InitAudioSet(1);
            }
        }

        /// <summary>
        /// 按钮 震动开关
        /// </summary>
        /// <param name="type">0:调用 1:点击</param>
        private void OnBtnSwitchVibrate(int type)
        {
            if (type == 1) AudioHandler._instance.PlayAudio(GameGlobalManager._instance.audioBtnClick);
            _vibrateOn = !_vibrateOn;
            _btnSwitchOff_Vibrate.SetActive(!_vibrateOn);
            _btnSwitchOn_Vibrate.SetActive(_vibrateOn);
            if (type == 1)
            {
                AudioHandler._instance.ModifyAudioSet(2, _vibrateOn);
                AudioHandler._instance.InitAudioSet(2);
            }
        }

        /// <summary>
        /// 按钮 低能耗模式开关
        /// </summary>
        /// <param name="type">0:调用 1:点击</param>
        private void OnBtnSwitchQuality(int type)
        {
            if (type == 1) AudioHandler._instance.PlayAudio(GameGlobalManager._instance.audioBtnClick);
            _qualityOn = !_qualityOn;
            _btnSwitchOff_Quality.SetActive(!_qualityOn);
            _btnSwitchOn_Quality.SetActive(_qualityOn);
            if (type == 1)
            {
                AudioHandler._instance.ModifyAudioSet(3, _qualityOn);
                AudioHandler._instance.InitAudioSet(3);
            }
        }

        /// <summary>
        /// 按钮 打开清除数据弹窗
        /// </summary>
        private void OnBtnOpenClearData()
        {
            _clearDataUi.SetActive(true);
        }

        /// <summary>
        /// 按钮 关闭清除数据弹窗
        /// </summary>
        private void OnBtnCloseClearData()
        {
            _clearDataUi.SetActive(false);
        }

        /// <summary>
        /// 按钮 确定清除数据
        /// </summary>
        private void OnBtnSureClearData()
        {
            _clearDataUi.SetActive(false);
            
            Debug.Log(" ================== 清空用户数据 ==================");
            GameGlobalManager._instance.ShowTips("数据已重置请重启游戏");
            GameSdkManager._instance._serverScript.ClearUserData();
        }

        /// <summary>
        /// 按钮 GM弹窗
        /// </summary>
        private void OnBtnPopGm()
        {
            MainManager._instance.OnOpenPop_Set(false);
            MainManager._instance.OnOpenPop_Gm(true);
        }

        public void Initial()
        {
            transform.Find("Mask").GetComponent<Button>().onClick.AddListener(() =>
            {
                AudioHandler._instance.PlayAudio(GameGlobalManager._instance.audioBtnClick);
                CloseTanChuang();
            });
            transform.Find("Set/Tittle/BtnClose").GetComponent<Button>().onClick.AddListener(() =>
            {
                AudioHandler._instance.PlayAudio(GameGlobalManager._instance.audioBtnClick);
                CloseTanChuang();
            });

            Transform btnMusic = transform.Find("Set/SetFrame/Set1/Switch");
            _btnSwitchOff_Music = btnMusic.Find("Off").gameObject;
            _btnSwitchOn_Music = btnMusic.Find("On").gameObject;
            btnMusic.GetComponent<Button>().onClick.AddListener(() => { OnBtnSwitchMusic(1); });
            
            Transform btnEffect = transform.Find("Set/SetFrame/Set2/Switch");
            _btnSwitchOff_Effect = btnEffect.Find("Off").gameObject;
            _btnSwitchOn_Effect = btnEffect.Find("On").gameObject;
            btnEffect.GetComponent<Button>().onClick.AddListener(() => { OnBtnSwitchEffect(1); });
            
            Transform btnVibrate = transform.Find("Set/SetFrame/Set3/Switch");
            _btnSwitchOff_Vibrate = btnVibrate.Find("Off").gameObject;
            _btnSwitchOn_Vibrate = btnVibrate.Find("On").gameObject;
            btnVibrate.GetComponent<Button>().onClick.AddListener(() => { OnBtnSwitchVibrate(1); });
            
            Transform btnQuality = transform.Find("Set/SetFrame/Set4/Switch");
            _btnSwitchOff_Quality = btnQuality.Find("Off").gameObject;
            _btnSwitchOn_Quality = btnQuality.Find("On").gameObject;
            btnQuality.GetComponent<Button>().onClick.AddListener(() => { OnBtnSwitchQuality(1); });

            transform.Find("Set/BtnClear").GetComponent<Button>().onClick.AddListener(() =>
            {
                AudioHandler._instance.PlayAudio(GameGlobalManager._instance.audioBtnClick);
                OnBtnOpenClearData();
            });
            _clearDataUi = transform.Find("ClearPlayerData").gameObject;
            _clearDataUi.SetActive(false);
            _clearDataUi.transform.Find("Frame/BtnClose").GetComponent<Button>().onClick.AddListener(() =>
            {
                AudioHandler._instance.PlayAudio(GameGlobalManager._instance.audioBtnClick);
                OnBtnCloseClearData();
            });
            _clearDataUi.transform.Find("Frame/BtnSure").GetComponent<Button>().onClick.AddListener(() =>
            {
                AudioHandler._instance.PlayAudio(GameGlobalManager._instance.audioBtnClick);
                OnBtnSureClearData();
            });

            _uidText = transform.Find("Set/UID").GetComponent<TextMeshProUGUI>();
            _uidText.GetComponent<Button>().onClick.AddListener(OnBtnPopGm);
        }
    }
}