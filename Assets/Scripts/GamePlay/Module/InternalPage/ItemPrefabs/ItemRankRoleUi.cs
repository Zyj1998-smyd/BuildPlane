using System;
using System.Text;
using Common.Tool;
using Data;
using Data.ClassData;
using Newtonsoft.Json;
using Platform;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace GamePlay.Module.InternalPage.ItemPrefabs
{
    public class ItemRankRoleUi : MonoBehaviour
    {
        /** 排名 */
        private TextMeshProUGUI _rankNumText;
        /** 昵称 */
        private Text _rankNameText;
        /** 数值 */
        private TextMeshProUGUI _rankScoreText;
        /** 头像 */
        private Image _haedImage;
        /** 我的排名 */
        private GameObject _rankMe;
        
        private void Awake()
        {
            _rankNumText = transform.Find("Num").GetComponent<TextMeshProUGUI>();
            _rankNameText = transform.Find("Name").GetComponent<Text>();
            _rankScoreText = transform.Find("Score").GetComponent<TextMeshProUGUI>();
            _haedImage = transform.Find("Head/Mask/Image").GetComponent<Image>();
            _rankMe = transform.Find("Me").gameObject;
        }

        public void SetData(int index, string[] data)
        {
            // 获取系统字体
            GameSdkManager._instance._sdkScript.GetSystemFont((font) => { _rankNameText.font = font; }, () => { });
            
            _rankNumText.gameObject.SetActive(true);
            _rankNumText.text = (index + 4).ToString();
            
            RankDisUserData rankDisUserData = JsonConvert.DeserializeObject<RankDisUserData>(data[1]);
            string nickName = rankDisUserData.nickName;
            string userAvatar = rankDisUserData.userAvatar;
            float rankValue = rankDisUserData.distance;
            _rankNameText.text = nickName != ""
                ? ToolFunManager.LongStrDeal(nickName, 16, "...")
                : ToolFunManager.LongStrDeal(new StringBuilder("游客" + Random.Range(10000, 100000)).ToString(), 16, "...");
            _rankScoreText.text = new StringBuilder(ToolFunManager.GetText(rankValue, true) + "米").ToString();
            if (userAvatar != "")
            {
                StartCoroutine(ServerGetData.GetRemoteImg(userAvatar, sprite => { _haedImage.sprite = sprite; }));
            }
            
            _rankMe.SetActive(data[2] != "false");
            _rankNameText.color = data[2] != "false" ? Color.black : Color.white;
        }
    }
}