using System.Text;
using Common.Tool;
using TMPro;
using UnityEngine;

namespace GamePlay.Module.InternalPage.ItemPrefabs
{
    public class ItemRankCityUi : MonoBehaviour
    {
        /** 排名 前三名 */
        private readonly GameObject[] _rankNumTexts = new GameObject[3];
        /** 排名 第四名及后续排名 */
        private TextMeshProUGUI _rankNumText;
        /** 昵称 */
        private TextMeshProUGUI _rankNameText;
        /** 数值 */
        private TextMeshProUGUI _rankScoreText;
        /** 我的排名 */
        private GameObject _rankMe;
        
        private void Awake()
        {
            for (int i = 0; i < 3; i++)
            {
                _rankNumTexts[i] = transform.Find("Num" + (i + 1)).gameObject;
            }
            
            _rankNumText = transform.Find("Num").GetComponent<TextMeshProUGUI>();
            _rankNameText = transform.Find("Name").GetComponent<TextMeshProUGUI>();
            _rankScoreText = transform.Find("Score").GetComponent<TextMeshProUGUI>();
            _rankMe = transform.Find("Me").gameObject;
        }

        public void SetData(int index, string[] data)
        {
            int rankNum = index + 1;

            if (rankNum < 4)
            {
                _rankNumText.gameObject.SetActive(false);
                for (int i = 0; i < _rankNumTexts.Length; i++)
                {
                    _rankNumTexts[i].SetActive(i == index);
                }
            }
            else
            {
                for (int i = 0; i < _rankNumTexts.Length; i++)
                {
                    _rankNumTexts[i].SetActive(false);
                }
                _rankNumText.gameObject.SetActive(true);
                _rankNumText.text = rankNum.ToString();
            }
            
            string rankName = data[1];
            string rankValue = data[2];
            _rankNameText.text = rankName;
            _rankScoreText.text = new StringBuilder(ToolFunManager.GetText(int.Parse(rankValue), true) + "米").ToString();
            
            _rankMe.SetActive(data[3] != "false");
            _rankNameText.color = data[3] != "false" ? Color.black : Color.white;
        }
    }
}