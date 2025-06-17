using System.Collections.Generic;
using Common.GameRoot.AudioHandler;
using Data;
using GamePlay.Globa;
using Platform;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GamePlay.Battle.Ui
{
    public class UiAccount : MonoBehaviour
    {
        private GameObject winObj, failObj;
        
        /** 记录本次需要刷新的数据Key */
        private List<string> _modifyKeys;
        
        /// <summary>
        /// 初始化UI
        /// </summary>
        public void AwakeOnUi()
        {
            winObj = transform.Find("Win").gameObject;
            winObj.SetActive(false);
            failObj = transform.Find("Fail").gameObject;
            failObj.SetActive(false);

            var btnHomeWin = winObj.transform.Find("Win/Button/BtnHome").GetComponent<Button>();
            var btnNextWin = winObj.transform.Find("Win/Button/BtnNext").GetComponent<Button>();
            btnHomeWin.onClick.AddListener(OnBtnHome);
            btnNextWin.onClick.AddListener(OnBtnNext);

            var btnHomeFail = failObj.transform.Find("Fail/Button/BtnHome").GetComponent<Button>();
            var btnAgainFail = failObj.transform.Find("Fail/Button/BtnAgain").GetComponent<Button>();
            btnHomeFail.onClick.AddListener(OnBtnHome);
            btnAgainFail.onClick.AddListener(OnBtnAgain);
        }

        /// <summary>
        /// 打开结算页
        /// </summary>
        /// <param name="isWin">结算结果 胜利/失败</param>
        public void OpenAccountUi(bool isWin)
        {
            if (isWin) BattleWin();
            else BattleFail();
        }

        /** 胜利结算 */
        private void BattleWin()
        {
            AudioHandler._instance.PlayAudio(BattleManager._instance.AccountWinAudio);
            winObj.SetActive(true);

            // 保存数据
            DataHelper.CurUserInfoData.levelNum += 1;
            DataHelper.ModifyLocalData(new List<string>(1) { "LevelNum" }, () =>
            {
                if (DataHelper.CurUserInfoData.userProvince != "")
                {
                    DataHelper.ProvinceRanks[DataHelper.CurUserInfoData.userProvince] += 1;
                    DataHelper.ModifyAdminCloudData(() => { });
                }
            });
            GameSdkManager.Instance._sdkScript.ReportRankData();
        }

        /** 失败结算 */
        private void BattleFail()
        {
            AudioHandler._instance.PlayAudio(BattleManager._instance.AccountFailAudio);
            failObj.SetActive(true);
        }

        // ---------------------------------------------- 按钮 ----------------------------------------------
        /// <summary>
        /// 按钮 返回主页
        /// </summary>
        private void OnBtnHome()
        {
            AudioHandler._instance.PlayAudio(BattleManager._instance.BtnClickAudio);
            GameGlobalManager._instance.LoadScene("MainScene");
        }

        /// <summary>
        /// 按钮 重新挑战
        /// </summary>
        private void OnBtnAgain()
        {
            AudioHandler._instance.PlayAudio(BattleManager._instance.BtnClickAudio);
            GameGlobalManager._instance.LoadScene("BattleScene");
        }

        /// <summary>
        /// 按钮 下一关
        /// </summary>
        private void OnBtnNext()
        {
            AudioHandler._instance.PlayAudio(BattleManager._instance.BtnClickAudio);
            GameGlobalManager._instance.LoadScene("BattleScene");
        }
    }
}