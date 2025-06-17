using Common.GameRoot.AudioHandler;
using GamePlay.Globa;
using UnityEngine;
using UnityEngine.UI;

namespace GamePlay.Battle.Ui
{
    public class UiPause : MonoBehaviour
    {
        /** 音乐/音效/震动 开/关 */
        private Button _btnMusicOn, _btnMusicOff, _btnEffectOn, _btnEffectOff, _btnVibrateOn, _btnVibrateOff;
        /** 音乐/音效/震动 开/关 */
        private bool _musicOn, _audioOn, _vibrateOn;
        
        /// <summary>
        /// 初始化UI
        /// </summary>
        public void AwakeOnUi()
        {
            transform.Find("UseItemFrame/BtnClose").GetComponent<Button>().onClick.AddListener(OnBtnContinue);
            transform.Find("UseItemFrame/BtnHome").GetComponent<Button>().onClick.AddListener(OnBtnHome);
            transform.Find("UseItemFrame/BtnBack").GetComponent<Button>().onClick.AddListener(OnBtnContinue);
            
            _btnMusicOn = transform.Find("UseItemFrame/Set1/BtnOn").GetComponent<Button>();
            _btnEffectOn = transform.Find("UseItemFrame/Set2/BtnOn").GetComponent<Button>();
            _btnVibrateOn = transform.Find("UseItemFrame/Set3/BtnOn").GetComponent<Button>();
            _btnMusicOn.onClick.AddListener(delegate { OnBtnSwitchMusic(1); });
            _btnEffectOn.onClick.AddListener(delegate { OnBtnSwitchEffect(1); });
            _btnVibrateOn.onClick.AddListener(delegate { OnBtnSwitchVibrate(1); });
            
            _btnMusicOff = transform.Find("UseItemFrame/Set1/BtnOff").GetComponent<Button>();
            _btnEffectOff = transform.Find("UseItemFrame/Set2/BtnOff").GetComponent<Button>();
            _btnVibrateOff = transform.Find("UseItemFrame/Set3/BtnOff").GetComponent<Button>();
            _btnMusicOff.onClick.AddListener(delegate { OnBtnSwitchMusic(1); });
            _btnEffectOff.onClick.AddListener(delegate { OnBtnSwitchEffect(1); });
            _btnVibrateOff.onClick.AddListener(delegate { OnBtnSwitchVibrate(1); });
        }

        /// <summary>
        /// 打开暂停页面
        /// </summary>
        public void OpenPauseUi()
        {
            // 刷新设置
            RefreshSet();
        }
        
        /** 刷新设置 */
        private void RefreshSet()
        {
            _musicOn = !AudioHandler._instance.musicSwitch;
            _audioOn = !AudioHandler._instance.audioSwitch;
            _vibrateOn = !AudioHandler._instance.vibrateSwitch;
            OnBtnSwitchMusic(0);
            OnBtnSwitchEffect(0);
            OnBtnSwitchVibrate(0);
        }
        
        // ---------------------------------------------- 按钮 ----------------------------------------------
        /** 按钮 返回主页面 */
        private void OnBtnHome()
        {
            AudioHandler._instance.PlayAudio(BattleManager._instance.BtnClickAudio);
            UiBattle._instance.OnBtnOpenPause(false);
            GameGlobalManager._instance.LoadScene("MainScene");
        }

        /** 按钮 继续游戏 */
        private void OnBtnContinue()
        {
            AudioHandler._instance.PlayAudio(BattleManager._instance.BtnClickAudio);
            UiBattle._instance.OnBtnOpenPause(false);
        }
        
        /** 按钮 开关 音乐 */
        private void OnBtnSwitchMusic(int type)
        {
            if (type == 1)
                AudioHandler._instance.PlayAudio(BattleManager._instance.BtnClickAudio);
            _musicOn = !_musicOn;
            _btnMusicOn.gameObject.SetActive(_musicOn);
            _btnMusicOff.gameObject.SetActive(!_musicOn);
            if (type == 1)
            {
                AudioHandler._instance.ModifyAudioSet(0, _musicOn);
                AudioHandler._instance.InitAudioSet(0);
            }
        }
        
        /** 按钮 开关 音效 */
        private void OnBtnSwitchEffect(int type)
        {
            if (type == 1)
                AudioHandler._instance.PlayAudio(BattleManager._instance.BtnClickAudio);
            _audioOn = !_audioOn;
            _btnEffectOn.gameObject.SetActive(_audioOn);
            _btnEffectOff.gameObject.SetActive(!_audioOn);
            if (type == 1)
            {
                AudioHandler._instance.ModifyAudioSet(1, _audioOn);
                AudioHandler._instance.InitAudioSet(1);
            }
        }
        
        /** 按钮 开关 震动 */
        private void OnBtnSwitchVibrate(int type)
        {
            if (type == 1)
                AudioHandler._instance.PlayAudio(BattleManager._instance.BtnClickAudio);
            _vibrateOn = !_vibrateOn;
            _btnVibrateOn.gameObject.SetActive(_vibrateOn);
            _btnVibrateOff.gameObject.SetActive(!_vibrateOn);
            if (type == 1)
            {
                AudioHandler._instance.ModifyAudioSet(2, _vibrateOn);
                AudioHandler._instance.InitAudioSet(2);
            }
        }
    }
}