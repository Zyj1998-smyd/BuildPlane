using System.Collections.Generic;
using System.Text;
using Common.GameRoot.AudioHandler;
using Common.Tool;
using Data;
using Data.ClassData;
using GamePlay.Globa;
using GamePlay.Main;
using GamePlay.Module.InternalPage.ScrollList;
using Newtonsoft.Json;
using Platform;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GamePlay.Module.InternalPage
{
    public class OpenRankPageUi : InternalPageScript
    {
        private ScrollListRankRole _scrollListRankRole;
        internal RectTransform _rankRoleContent;
        
        private ScrollListRankCity _scrollListRankCity;
        internal RectTransform _rankCityContent;

        private GameObject _rankMeRole;
        private GameObject _rankMeCity;

        private GameObject _rankRoleObj;
        private GameObject _rankCityObj;

        /** 我的个人排名 昵称 */
        private Text _rankMeNameTextRole;
        /** 我的地区排名 名称 */
        private TextMeshProUGUI _rankMeNameTextCity;

        /** 我的个人排名 排名 */
        private TextMeshProUGUI _rankMeNum_1;
        /** 我的地区排名 排名 */
        private TextMeshProUGUI _rankMeNum_2;
        
        /** 我的个人排名 距离 */
        private TextMeshProUGUI _rankMeScoreNum_1;
        /** 我的地区排名 距离 */
        private TextMeshProUGUI _rankMeScoreNum_2;
        
        /** 我的个人排名 头像 */
        private Image _rankMeHead_1;

        /** 个人排行榜 前三名 昵称 */
        private readonly Text[] _rankRoleTop3_Names = new Text[3];
        /** 个人排行榜 前三名 头像 */
        private readonly Image[] _rankRoleTop3_Heads = new Image[3];
        /** 个人排行榜 前三名 距离 */
        private readonly TextMeshProUGUI[] _rankRoleTop3_Score = new TextMeshProUGUI[3];
        /** 个人排行榜 前三名 地区名称 */
        private readonly TextMeshProUGUI[] _rankRoleTop3_CityName = new TextMeshProUGUI[3];
        
        /** 当前排行榜类型 0: 个人排行榜 1: 城市排行榜 */
        private int _curRankType;
        
        public override void Initial()
        {
            base.Initial();

            _rankRoleObj = transform.Find("RankRole").gameObject;
            _rankCityObj = transform.Find("RankCity").gameObject;

            _rankRoleObj.transform.Find("Button2").GetComponent<Button>().onClick.AddListener(() =>
            {
                AudioHandler._instance.PlayAudio(GameGlobalManager._instance.audioBtnClick);
                OnBtnToRankCity();
            });
            _rankCityObj.transform.Find("Button1").GetComponent<Button>().onClick.AddListener(() =>
            {
                AudioHandler._instance.PlayAudio(GameGlobalManager._instance.audioBtnClick);
                OnBtnToRankRole();
            });

            _rankMeRole = transform.Find("RankMe/RankMeRole").gameObject;
            _rankMeCity = transform.Find("RankMe/RankMeCity").gameObject;

            _rankMeNameTextRole = _rankMeRole.transform.Find("Name").GetComponent<Text>();
            _rankMeNameTextCity = _rankMeCity.transform.Find("Name").GetComponent<TextMeshProUGUI>();

            _rankMeNum_1 = _rankMeRole.transform.Find("Num").GetComponent<TextMeshProUGUI>();
            _rankMeNum_2 = _rankMeCity.transform.Find("Num").GetComponent<TextMeshProUGUI>();

            _rankMeScoreNum_1 = _rankMeRole.transform.Find("Score").GetComponent<TextMeshProUGUI>();
            _rankMeScoreNum_2 = _rankMeCity.transform.Find("Score").GetComponent<TextMeshProUGUI>();

            _rankMeHead_1 = _rankMeRole.transform.Find("Head/Mask/Image").GetComponent<Image>();

            _scrollListRankRole = transform.Find("List/RankRoleList").GetComponent<ScrollListRankRole>();
            _scrollListRankCity = transform.Find("List/RankCityList").GetComponent<ScrollListRankCity>();

            _rankRoleContent = _scrollListRankRole.transform.Find("ListSv/Viewport/Content").GetComponent<RectTransform>();
            _rankCityContent = _scrollListRankCity.transform.Find("ListSv/Viewport/Content").GetComponent<RectTransform>();

            _scrollListRankRole.OpenRankPageUi = this;
            _scrollListRankCity.OpenRankPageUi = this;

            for (int i = 0; i < 3; i++)
            {
                Transform rankItem = _scrollListRankRole.transform.Find("Top/Top" + (i + 1));
                _rankRoleTop3_Heads[i] = rankItem.Find("Head/Mask/Image").GetComponent<Image>();
                _rankRoleTop3_Names[i] = rankItem.Find("Name1").GetComponent<Text>();
                _rankRoleTop3_CityName[i] = rankItem.Find("Name2").GetComponent<TextMeshProUGUI>();
                _rankRoleTop3_Score[i] = rankItem.Find("Score").GetComponent<TextMeshProUGUI>();
            }
        }

        public override void OpenInternalPage()
        {
            base.OpenInternalPage();

            AudioHandler._instance.PlayAudio(MainManager._instance.audioPageOpen);
            MainManager._instance.SetRankCamActive(true);
            GameSdkManager._instance._sdkScript.GetSystemFont((font) =>
            {
                if (font != null)
                {
                    _rankMeNameTextRole.font = font;
                    for (int i = 0; i < _rankRoleTop3_Names.Length; i++)
                    {
                        _rankRoleTop3_Names[i].font = font;
                    }
                }
                OnBtnToRankRole();
            }, () => { });
            
            // 完成日常任务 查看X次排行榜 TaskID:8
            DataHelper.CompleteDailyTask(8, 1, 0);
            DataHelper.ModifyLocalData(new List<string>(1) { "taskInfo1" }, () => { });
            
            // 上报自定义分析数据 事件: 点击排行榜
            GameSdkManager._instance._sdkScript.ReportAnalytics("QueryRank", "", "");
        }

        public override void CloseInternalPage()
        {
            MainManager._instance.SetRankCamActive(false);
            base.CloseInternalPage();
        }
        
        /// <summary>
        /// 刷新排行榜
        /// </summary>
        private void RefreshRankList()
        {
            if (_curRankType == 0)
            {
                // 个人排行榜
                string myRankKey = GameSdkManager._instance._serverScript.GetRankAllJudgeKey();
                string rankData = DataHelper.RankAllSort(2);
                List<RankDisUserData> levelUserDatas = JsonConvert.DeserializeObject<List<RankDisUserData>>(rankData);
                
                List<string[]> rankDatas = new List<string[]>();
                int rankMeNum = -1;
                float rankMeValue = DataHelper.CurUserInfoData.scoreDistanceMax;
                string rankMeName = "";
                string rankMeHead = DataHelper.CurUserInfoData.userAvatar;
                for (int i = 0; i < levelUserDatas.Count; i++)
                {
                    string isMe = "false";
                    if (levelUserDatas[i].openId == myRankKey)
                    {
                        rankMeNum = (i + 1);
                        rankMeValue = levelUserDatas[i].distance;
                        rankMeName = levelUserDatas[i].nickName;
                        rankMeHead = levelUserDatas[i].userAvatar;
                        isMe = "true";
                    }

                    rankDatas.Add(new[] { "0", JsonConvert.SerializeObject(levelUserDatas[i]), isMe });
                }
                
                List<string[]> rankTop3Datas = new List<string[]>();
                while (rankTop3Datas.Count < 3 && rankDatas.Count > 0)
                {
                    rankTop3Datas.Add(rankDatas[0]);
                    rankDatas.Remove(rankDatas[0]);
                }
                
                _scrollListRankRole.SetList(rankDatas);
                RefreshRoleRankTop(rankTop3Datas);
                
                if (rankMeName == "")
                {
                    rankMeName = DataHelper.CurUserInfoData.userName == ""
                        ? "Unauthorized Users"
                        : DataHelper.CurUserInfoData.userName;
                }

                _rankMeNum_1.text = rankMeNum == -1 ? "---" : rankMeNum.ToString();
                _rankMeScoreNum_1.text = new StringBuilder(ToolFunManager.GetText(rankMeValue, true) + "Ms").ToString();
                _rankMeNameTextRole.text = ToolFunManager.LongStrDeal(rankMeName, 16, "...");
                if (rankMeHead != "")
                {
                    StartCoroutine(ServerGetData.GetRemoteImg(rankMeHead, sprite => _rankMeHead_1.sprite = sprite));
                }
            }
            else
            {
                // 城市排行榜
                for (int i = 0; i < DataHelper.GeneralRanks.Count; i++)
                {
                    KeyValuePair<string, int> rankTmp = DataHelper.GeneralRanks[i];
                    if (DataHelper.ProvinceRanks.ContainsKey(rankTmp.Key))
                    {
                        DataHelper.ProvinceRanks[rankTmp.Key] = rankTmp.Value;
                    }
                }
                
                List<string> provinceSort = DataHelper.GetProvinceSort();
                List<string> provinceNames = JsonConvert.DeserializeObject<List<string>>(provinceSort[0]);
                List<int> provinceNums = JsonConvert.DeserializeObject<List<int>>(provinceSort[1]);

                List<string[]> rankDatas = new List<string[]>();
                int rankMeNum = -1;
                int rankMeValue = 0;
                string rankMeName = "Unknown";
                if (DataHelper.CurUserInfoData.userProvince == "")
                {
                    // 未获取到地理位置信息
                    for (int i = 0; i < provinceNames.Count; i++)
                    {
                        rankDatas.Add(new[] { "1", provinceNames[i], provinceNums[i].ToString(), "false" });
                    }
                }
                else
                {
                    // 已获取到地理位置信息
                    for (int i = 0; i < provinceNames.Count; i++)
                    {
                        string isMe = "false";
                        if (provinceNames[i] == DataHelper.CurUserInfoData.userProvince)
                        {
                            rankMeNum = (i + 1);
                            rankMeValue = provinceNums[i];
                            rankMeName = provinceNames[i];
                            isMe = "true";
                        }

                        rankDatas.Add(new[] { "1", provinceNames[i], provinceNums[i].ToString(), isMe });
                    }
                }
                
                _scrollListRankCity.SetList(rankDatas);

                _rankMeNum_2.text = rankMeNum == -1 ? "---" : rankMeNum.ToString();
                _rankMeScoreNum_2.text = new StringBuilder(ToolFunManager.GetText(rankMeValue, true) + "M").ToString();
                _rankMeNameTextCity.text = rankMeName;
            }
        }

        /// <summary>
        /// 刷新个人排行榜前三名
        /// </summary>
        /// <param name="rankTop3Datas">数据</param>
        private void RefreshRoleRankTop(List<string[]> rankTop3Datas)
        {
            for (int i = 0; i < rankTop3Datas.Count; i++)
            {
                int iTmp = i;
                RankDisUserData rankDisUserData = JsonConvert.DeserializeObject<RankDisUserData>(rankTop3Datas[i][1]);
                string nickName = rankDisUserData.nickName;
                string userAvatar = rankDisUserData.userAvatar;
                float rankValue = rankDisUserData.distance;
                _rankRoleTop3_Names[i].text = nickName != ""
                    ? ToolFunManager.LongStrDeal(nickName, 16, "...")
                    : "";
                _rankRoleTop3_Score[i].text = new StringBuilder(ToolFunManager.GetText(rankValue, true) + "M").ToString();
                if (userAvatar != "")
                {
                    StartCoroutine(ServerGetData.GetRemoteImg(userAvatar, sprite => { _rankRoleTop3_Heads[iTmp].sprite = sprite; }));
                }

                _rankRoleTop3_CityName[i].text = rankDisUserData.userProvince;
                MainManager._instance._rankPlaneMgr[i].CreatePlane((i + 1), rankDisUserData.openId,
                    rankDisUserData.planeEquipments, rankDisUserData.planeEquipmentColor,
                    rankDisUserData.planeEquipmentLvs);
            }
        }

        // ----------------------------------------------- 按钮 -----------------------------------------------
        /// <summary>
        /// 按钮 切换到个人榜
        /// </summary>
        private void OnBtnToRankRole()
        {
            _curRankType = 0;
            _rankRoleObj.SetActive(true);
            _rankCityObj.SetActive(false);
            _rankMeRole.SetActive(true);
            _rankMeCity.SetActive(false);
            _scrollListRankRole.gameObject.SetActive(true);
            _scrollListRankCity.gameObject.SetActive(false);
            RefreshRankList();
        }

        /// <summary>
        /// 按钮 切换到地区榜
        /// </summary>
        private void OnBtnToRankCity()
        {
            _curRankType = 1;
            _rankRoleObj.SetActive(false);
            _rankCityObj.SetActive(true);
            _rankMeRole.SetActive(false);
            _rankMeCity.SetActive(true);
            _scrollListRankRole.gameObject.SetActive(false);
            _scrollListRankCity.gameObject.SetActive(true);
            RefreshRankList();
        }
    }
}