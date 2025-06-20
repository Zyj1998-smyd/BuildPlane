using System;
using System.Text;
using Common.Event;
using Common.Event.CustomEnum;
using Common.GameRoot.AudioHandler;
using Common.LoadRes;
using Common.Tool;
using Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GamePlay.Battle.Ui
{
    public class UiBattle : MonoBehaviour
    {
        private Transform canvasTran;
        /** 结算页面 */
        private UiAccount _uiAccount;

        /** 推进中 */
        private GameObject spurtIng;
        /** 推进按钮 进度 */
        private Image spurtValue;

        /** 城市进度 */
        internal Image infoOrderValue;

        /** 距离 */
        internal TextMeshProUGUI distanceNumText;
        /** 新纪录 */
        internal GameObject disNewRecord;

        /** 速度条 */
        private Image _speedValue;
        /** 速度值 */
        private TextMeshProUGUI speedNumText;

        /** 高度值 */
        private TextMeshProUGUI heightNumText;

        private Animation goldAni;
        /** 金币数 */
        private TextMeshProUGUI goldNumText;
        
        /** 城市名称 当前站/下一站 */
        internal TextMeshProUGUI cityNameNow, cityNameNew;

        internal UiCityLogo _uiCityLogo;
        internal UiClockIn  _uiClockIn;

        /// <summary>
        /// 初始化
        /// </summary>
        internal void Initial()
        {
             canvasTran = GameObject.Find("/Canvas2D").transform;

            _uiAccount = canvasTran.Find("Account").GetComponent<UiAccount>();
            _uiAccount.Initial();
            _uiAccount.gameObject.SetActive(false);

            spurtIng = canvasTran.Find("Main/BtnSpurt/SpurtIng").gameObject;
            spurtValue = canvasTran.Find("Main/BtnSpurt/SpurtValue").GetComponent<Image>();
            spurtIng.SetActive(false);

            Transform infoObj = canvasTran.Find("Main/Info");

            infoOrderValue = infoObj.Find("OrderFrame/Value").GetComponent<Image>();
            infoOrderValue.fillAmount = 0;

            distanceNumText = infoObj.Find("Distance").GetComponent<TextMeshProUGUI>();
            distanceNumText.text =
                new StringBuilder(ToolFunManager.GetText(
                        BattleManager._instance.endDis * (DataHelper.CurLevelNum - 1), true) + "M")
                    .ToString();

            disNewRecord = infoObj.Find("Distance/NewRecord").gameObject;
            disNewRecord.SetActive(false);

            _speedValue = infoObj.Find("Speed/Value").GetComponent<Image>();
            speedNumText = infoObj.Find("Speed/Num").GetComponent<TextMeshProUGUI>();
            _speedValue.fillAmount = 0;
            speedNumText.text = "0M/H";

            heightNumText = infoObj.Find("Height/Num").GetComponent<TextMeshProUGUI>();
            heightNumText.text = "0M";

            goldAni = infoObj.Find("Gold").GetComponent<Animation>();
            goldNumText = infoObj.Find("Gold/Num").GetComponent<TextMeshProUGUI>();
            goldNumText.text = "+0";
            
            cityNameNow = infoObj.Find("OrderFrame/Text1").GetComponent<TextMeshProUGUI>();
            cityNameNow.text = BattleManager._instance.cityNames[DataHelper.CurLevelNum - 1];
            cityNameNew = infoObj.Find("OrderFrame/Text2").GetComponent<TextMeshProUGUI>();
            if (DataHelper.CurLevelNum <= BattleManager._instance.cityNames.Length)
            {
                cityNameNew.text = BattleManager._instance.cityNames[DataHelper.CurLevelNum];
            }

            GreateClockIn();
        }

        void OnEnable()
        {
            EventManager<Vector2>.Add(EnumButtonType.TouchJoystick, JoyInput);
            EventManager<EnumButtonSign,Vector2>.Add(EnumButtonType.TouchScreenDown, ScreenInputDown);
            EventManager<EnumButtonSign,Vector2>.Add(EnumButtonType.TouchScreenUp, ScreenInputUp);
        }

        void OnDisable()
        {
            EventManager<Vector2>.Remove(EnumButtonType.TouchJoystick, JoyInput);
            EventManager<EnumButtonSign,Vector2>.Remove(EnumButtonType.TouchScreenDown, ScreenInputDown);
            EventManager<EnumButtonSign,Vector2>.Remove(EnumButtonType.TouchScreenUp, ScreenInputUp);
        }

        /// <summary>
        /// 刷新飞行距离
        /// </summary>
        internal void RefreshDistance()
        {
            distanceNumText.text =
                new StringBuilder(ToolFunManager.GetText(
                        Mathf.FloorToInt(BattleManager._instance.scoreDistance) + BattleManager._instance.endDis * (DataHelper.CurLevelNum - 1), true) + "M")
                    .ToString();
            infoOrderValue.fillAmount = BattleManager._instance.scoreDistance / BattleManager._instance.endDis;

            disNewRecord.SetActive(BattleManager._instance.scoreDistance + BattleManager._instance.endDis * (DataHelper.CurLevelNum - 1) > DataHelper.CurUserInfoData.scoreDistanceMax);
        }

        /// <summary>
        /// 刷新金币
        /// </summary>
        internal void RefreshGold()
        {
            goldAni["GetGold"].normalizedTime = 0;
            goldAni.Play("GetGold");
            goldNumText.text = new StringBuilder("+" + ToolFunManager.GetText(BattleManager._instance.scoreGetGold, false)).ToString();
        }

        internal void RefreshSpeed()
        {
            speedNumText.text = new StringBuilder(BattleManager._instance.nowSpeed +"M/H").ToString();
            _speedValue.fillAmount = Mathf.Lerp(_speedValue.fillAmount, (BattleManager._instance.nowSpeed / 100f) + 0.3f, 0.25f);
        }
        
        internal void RefreshHeight()
        {
            heightNumText.text = new StringBuilder(BattleManager._instance.nowHeight + "M").ToString();
        }

        internal void RefreshSpurt(int spurtIngState, float Ratio)
        {
            switch (spurtIngState)
            {
                case 0:
                    spurtIng.SetActive(false);
                    break;
                case 1:
                    spurtIng.SetActive(true);
                    break;
            }
            spurtValue.fillAmount = Ratio;
        }

        internal void GreateCityLogo(Action cb)
        {
            LoadResources.XXResourcesLoad("CityLogo",
                handleTmp =>
                {
                    GameObject objTmp = Instantiate(handleTmp, canvasTran);
                    _uiCityLogo = objTmp.GetComponent<UiCityLogo>();
                    _uiCityLogo.Initial();
                    
                    cb();
                });
        }

        void GreateClockIn()
        {
            LoadResources.XXResourcesLoad("ClockIn",
                handleTmp =>
                {
                    GameObject objTmp = Instantiate(handleTmp,canvasTran);
                    _uiClockIn = objTmp.GetComponent<UiClockIn>();
                    _uiClockIn.Initial();
                });
        }

        // ---------------------------------------------------------- 按钮 ----------------------------------------------------------
        void JoyInput(Vector2 joyMoveVector)
        {
            BattleManager._instance._planeControl.PlayerRot(joyMoveVector);
        }

        void ScreenInputDown(EnumButtonSign buttonSign, Vector2 touchPos)
        {
            switch (buttonSign)
            {
                case EnumButtonSign.BtnSpurt:
                    BattleManager._instance._planeControl.ThrusterSpurtStart();
                    break;
            }
        }
        
        void ScreenInputUp(EnumButtonSign buttonSign, Vector2 touchPos)
        {
            switch (buttonSign)
            {
                case EnumButtonSign.BtnSpurt:
                    _ = BattleManager._instance._planeControl.ThrusterSpurtCancel();
                    break;
            }
        }

        /// <summary>
        /// 打开/关闭 结算弹窗
        /// </summary>
        internal void OpenAccount()
        {
            if (BattleManager._instance.launcherTouch) BattleManager._instance.launcherTouch.SetActive(false);
            _uiAccount.gameObject.SetActive(true);
            _uiAccount.OnOpenAccount();

            SetQuality();
        }


        void SetQuality()
        {
            if (PlayerPrefs.GetInt("QualitySwitch", 0) == 1) return;

            float fpsTmp = FPSMonitor.Instance.StopTracking();
            Debug.Log(fpsTmp);
            if (fpsTmp < 28)
            {
                AudioHandler._instance.ModifyAudioSet(3, true);
                AudioHandler._instance.InitAudioSet(3);
            }
        }


#if UNITY_EDITOR
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                ScreenInputDown(EnumButtonSign.BtnSpurt,Vector2.zero);
            }
            if (Input.GetKeyUp(KeyCode.A))
            {
                ScreenInputUp(EnumButtonSign.BtnSpurt,Vector2.zero);
            }
        }
#endif
        
        
    }
}