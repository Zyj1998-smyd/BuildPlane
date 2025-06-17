using System.Collections.Generic;
using System.Threading;
using Common.LoadRes;
using Cysharp.Threading.Tasks;
using Data;
using Platform;
using UnityEngine;

namespace Common.GameRoot.AudioHandler
{
    public class AudioHandler : MonoBehaviour
    {
        public static AudioHandler _instance;

        private List<AudioSource> holdAudios = new List<AudioSource>();

        private AudioSource bgmAudio;

        private string bgmClipNow;

        [SerializeField] private Dictionary<string, AudioSource> onlyAudioDic = new Dictionary<string, AudioSource>();
        
        CancellationTokenSource ctsWaitUniTask = new CancellationTokenSource();

        /** 背景音乐音量 */
        public bool musicSwitch;
        /** 音效音量 */
        public bool audioSwitch;
        /** 震动开关 */
        public bool vibrateSwitch;
        /** 低能耗模式开关 */
        public bool qualitySwitch;

        private void Awake()
        {
            if (_instance != null)
            {
                Destroy(gameObject);
                return;
            }
            _instance = this;
            
            CreateBgmAudio();
        }

        private void Start()
        {
            // InitAudioSet(0); // 背景音乐
            // InitAudioSet(1); // 音效
            // InitAudioSet(2); // 震动
            // InitAudioSet(3); // 低能耗模式
        }
        private void OnDisable()
        {
            ctsWaitUniTask.Cancel();
        }

        public void PlayAudio(AudioClip audioClip)
        {
            if (!audioSwitch) return;
            
            // volumeTmp *= DataManager._instance.GameData.EffectVolume;
            // if (volumeTmp > 0)
            // {
            AudioSource tempAudio = GetFreeAudio();
            tempAudio.clip = audioClip;
            // tempAudio.volume = volumeTmp;
            tempAudio.Play();
            // }
        }

        AudioSource GetFreeAudio()
        {
            AudioSource tempAudio;
            
            for (int i = 0; i < holdAudios.Count; i++)
            {
                if (!holdAudios[i].isPlaying)
                {
                    tempAudio = holdAudios[i];
                    holdAudios.Remove(tempAudio);
                    holdAudios.Add(tempAudio);
                    return tempAudio;
                }
            }
            
            if (holdAudios.Count > 10)
            {
                return holdAudios[0];
            }
            
            tempAudio = CreateAudioSource();

            holdAudios.Add(tempAudio);
            return tempAudio;
        }

        AudioSource CreateAudioSource()
        {
            var tempAudio = gameObject.AddComponent<AudioSource>();
            tempAudio.outputAudioMixerGroup = null;
            tempAudio.mute = false;
            tempAudio.bypassEffects = false;
            tempAudio.bypassListenerEffects = false;
            tempAudio.bypassReverbZones = false;
            tempAudio.playOnAwake = false;
            tempAudio.loop = false;
            tempAudio.priority = 128;
            tempAudio.pitch = 1;
            tempAudio.panStereo = 0;
            tempAudio.spatialBlend = 0;
            tempAudio.reverbZoneMix = 1;
            
            return tempAudio;
        }
        private void StopAllAudio()
        {
            for (int i = 0; i < holdAudios.Count; i++)
            {
                if (holdAudios[i].isPlaying)
                {
                    holdAudios[i].Stop();
                }
            }

            foreach (var VARIABLE in onlyAudioDic)
            {
                if (VARIABLE.Value.isPlaying)
                {
                    VARIABLE.Value.Stop();
                }
            }
        }

        private void CreateBgmAudio()
        {
            bgmAudio = gameObject.AddComponent<AudioSource>();
            bgmAudio.outputAudioMixerGroup = null;
            bgmAudio.mute = false;
            bgmAudio.bypassEffects = false;
            bgmAudio.bypassListenerEffects = false;
            bgmAudio.bypassReverbZones = false;
            bgmAudio.playOnAwake = false;
            bgmAudio.loop = true;
            bgmAudio.priority = 128;
            bgmAudio.pitch = 1;
            bgmAudio.panStereo = 0;
            bgmAudio.spatialBlend = 0;
            bgmAudio.reverbZoneMix = 1;
            // bgmAudio.volume = DataManager._instance.GameData.MusicVolume;
        }

        public void PlayBGM(string bgmClipTmp)
        {
            if (!musicSwitch) return;
            if (bgmClipTmp == "") return;
            
            // if (bgmClipTmp == bgmClipNow) return;
            bgmClipNow = bgmClipTmp;
            
            LoadResources.XXResourcesLoad(bgmClipNow, handleTmp =>
            {
                bgmAudio.clip = handleTmp;
                bgmAudio.Play();
                
                ctsWaitUniTask.Cancel();
                ctsWaitUniTask = new CancellationTokenSource();
                _ = SetBgmV(ctsWaitUniTask.Token);
                
            }, true);
        }

        private void StopBGM()
        {
            bgmAudio.Stop();
        }

        async UniTask SetBgmV(CancellationToken ctk)
        {
            bgmAudio.volume = 0;
            while (bgmAudio.volume < 1)
            {
                bgmAudio.volume += 0.05f;
                await UniTask.DelayFrame(10,cancellationToken: ctk);
            }
        }

        #region 音乐/音效/震动设置

        /** 初始化背景音乐/音效/震动设置 */
        public void InitAudioSet(int type)
        {
            switch (type)
            {
                case 0: // 背景音乐
                    // var _musicSwitch = PlayerPrefs.GetInt("MusicVolume", 1);
                    var _musicSwitch = DataHelper.CurUserInfoData.settings[0];
                    musicSwitch = _musicSwitch == 1;
                    
                    // bgmAudio.mute = !musicSwitch;

                    if (!musicSwitch) StopBGM();
                    else PlayBGM(DataHelper.GetCurSceneBgmName());
                    break;
                case 1: // 音效
                    // var _audioSwitch = PlayerPrefs.GetInt("AudioSwitch", 1);
                    var _audioSwitch = DataHelper.CurUserInfoData.settings[1];
                    audioSwitch = _audioSwitch == 1;
                    
                    if (!audioSwitch) StopAllAudio();
                    break;
                case 2: // 震动
                    // var _vibrateSwitch = PlayerPrefs.GetInt("VibrateSwitch", 1);
                    var _vibrateSwitch = DataHelper.CurUserInfoData.settings[2];
                    vibrateSwitch = _vibrateSwitch == 1;
                    
                    if(vibrateSwitch)
                    {
                        if (GameSdkManager._instance)
                        {
                            GameSdkManager._instance._sdkScript.ShortVibrateControl();
                        }
                    }
                    break;
                case 3: // 低能耗模式
                    // var _qualitySwitch = PlayerPrefs.GetInt("QualitySwitch", 0);
                    var _qualitySwitch = DataHelper.CurUserInfoData.settings[3];
                    qualitySwitch = _qualitySwitch == 1;

                    if (qualitySwitch) QualitySettings.SetQualityLevel(1);
                    else QualitySettings.SetQualityLevel(0);
                    break;
            }
        }
        
        /** 修改背景音乐/音效/震动设置 */
        public void ModifyAudioSet(int type, bool value)
        {
            switch (type)
            {
                case 0: // 背景音乐
                    // PlayerPrefs.SetInt("MusicVolume", value ? 1 : 0);
                    DataHelper.CurUserInfoData.settings[0] = value ? 1 : 0;
                    DataHelper.ModifyLocalData(new List<string>(1) { "settings" }, () => { });
                    break;
                case 1: // 音效
                    // PlayerPrefs.SetInt("AudioSwitch", value ? 1 : 0);
                    DataHelper.CurUserInfoData.settings[1] = value ? 1 : 0;
                    DataHelper.ModifyLocalData(new List<string>(1) { "settings" }, () => { });
                    break;
                case 2: // 震动
                    // PlayerPrefs.SetInt("VibrateSwitch", value ? 1 : 0);
                    DataHelper.CurUserInfoData.settings[2] = value ? 1 : 0;
                    DataHelper.ModifyLocalData(new List<string>(1) { "settings" }, () => { });
                    break;
                case 3: // 低能耗模式
                    // PlayerPrefs.SetInt("QualitySwitch", value ? 1 : 0);
                    DataHelper.CurUserInfoData.settings[3] = value ? 1 : 0;
                    DataHelper.ModifyLocalData(new List<string>(1) { "settings" }, () => { });
                    break;
            }
        }

        #endregion
        
        /// <summary>
        /// 暂停/恢复背景音乐播放
        /// </summary>
        /// <param name="isHide">T: 暂停 F: 恢复</param>
        public void HideAudioPlay(bool isHide)
        {
            // bgmAudio.mute = isHide;
            // int _musicSwitch = PlayerPrefs.GetInt("MusicVolume", 1);
            int _musicSwitch = DataHelper.CurUserInfoData.settings[0];
            bool musicSwitchTmp = _musicSwitch == 1;
            if (isHide)
            {
                // 暂停背景音乐 如果背景音乐开关打开则暂停背景音乐 开关关闭忽略
                if (musicSwitchTmp)
                {
                    StopBGM();
                }
            }
            else
            {
                // 恢复背景音乐 如果背景音乐开关打开则播放背景音乐 开关关闭忽略
                if (musicSwitchTmp)
                {
                    AudioHandler._instance.PlayBGM(DataHelper.GetCurSceneBgmName());
                }
            }
        }
    }
}