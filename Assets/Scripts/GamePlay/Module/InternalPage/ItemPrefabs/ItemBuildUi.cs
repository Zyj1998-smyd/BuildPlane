using System.Collections.Generic;
using System.Text;
using Common.Event;
using Common.Event.CustomEnum;
using Common.GameRoot.AudioHandler;
using Data;
using Data.ConfigData;
using GamePlay.Globa;
using GamePlay.Main;
using GamePlay.Module.InternalPage.PageBuild;
using Platform;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GamePlay.Module.InternalPage.ItemPrefabs
{
    public class ItemBuildUi : MonoBehaviour
    {
        /** 部件品质 */
        private Image _frameImage;
        /** 部件图标 */
        private Image _iconImage;
        /** 部件名称 */
        private TextMeshProUGUI _nameText;
        /** 部件等级 */
        private TextMeshProUGUI _levelNumText;

        /** 碎片信息 */
        private GameObject _chipObj;
        /** 碎片收集进度 */
        private Image _chipBar;
        /** 碎片收集数量 */
        private TextMeshProUGUI _chipNumText;

        /** 升级按钮 */
        private GameObject _btnLevelUp;

        /** 重量 */
        private TextMeshProUGUI _propetyNumText_Weight;

        /** 属性 */
        private readonly GameObject[] _propetyObjs = new GameObject[5];
        /** 属性值 */
        private readonly TextMeshProUGUI[] _propetyNumTexts = new TextMeshProUGUI[5];

        /** 提示红点 */
        private GameObject _redPoint;

        /** 组装页面 */
        internal OpenBuildMainUi openBuildMainUi;
        /** 部件ID */
        internal int _id;
        /** 部件类型 */
        internal int _type;
        /** 列表索引 */
        internal int _index;

        internal RectTransform _parentRect;

        /** 升级消耗碎片数量 */
        private int _chipUpGradeNum;
        
        /// <summary>
        /// 初始化
        /// </summary>
        internal void Initial()
        {
            _parentRect = transform.parent.GetComponent<RectTransform>();
            _frameImage = transform.Find("Frame").GetComponent<Image>();
            _iconImage = transform.Find("Icon").GetComponent<Image>();
            _nameText = transform.Find("Name").GetComponent<TextMeshProUGUI>();
            _levelNumText = transform.Find("Star/Num").GetComponent<TextMeshProUGUI>();

            _chipObj = transform.Find("Chip/Chip").gameObject;
            _chipBar = transform.Find("Chip/Chip/Bar").GetComponent<Image>();
            _chipNumText = transform.Find("Chip/Chip/Num").GetComponent<TextMeshProUGUI>();

            _btnLevelUp = transform.Find("Chip/BtnLevelUp").gameObject;
            _btnLevelUp.GetComponent<Button>().onClick.AddListener(OnBtnLevelUp);

            _propetyNumText_Weight = transform.Find("Weight/Num").GetComponent<TextMeshProUGUI>();

            _redPoint = transform.Find("RedPoint").gameObject;

            for (int i = 0; i < 5; i++)
            {
                _propetyObjs[i] = transform.Find("Propertys/property" + (i + 2)).gameObject;
                _propetyNumTexts[i] = _propetyObjs[i].transform.Find("Num").GetComponent<TextMeshProUGUI>();
            }
            
            gameObject.GetComponent<Button>().onClick.AddListener(OnBtnItem);
        }

        /// <summary>
        /// 设置数据
        /// </summary>
        /// <param name="index">列表索引</param>
        /// <param name="id">ID</param>
        internal void SetData(int index, int id)
        {
            _index = index;
            _id = id;

            ComponentConfig config = ConfigManager.Instance.ComponentConfigDict[_id];

            _type = config.Type;

            _nameText.text = config.Name;
            _frameImage.sprite = openBuildMainUi._openBuildPageUi.qualityFrames[config.Quality];

            GameGlobalManager._instance.SetImage(_iconImage, new StringBuilder("IconImage" + _id).ToString());

            _propetyNumText_Weight.text = new StringBuilder(config.ZhongLiang + "g").ToString();
            
            int curLevel = DataHelper.CurUserInfoData.equipments.GetValueOrDefault(_id, GlobalValueManager.InitEquipmentLv);

            _levelNumText.text = curLevel.ToString();
            
            List<int> propetyNums = new List<int>(5) { config.FuKong, config.SuDu, config.KangZu, config.TuiJin, config.NengLiang };
            for (int i = 0; i < _propetyObjs.Length; i++)
            {
                if (propetyNums[i] > 0)
                {
                    float curNumTmp = propetyNums[i];
                    for (int j = 0; j < (curLevel - 1); j++)
                    {
                        curNumTmp *= GlobalValueManager.EquipmentUpGradeNum;
                    }

                    int curNum = Mathf.FloorToInt(curNumTmp);
                    _propetyObjs[i].SetActive(true);
                    _propetyNumTexts[i].text = curNum.ToString();
                }
                else
                {
                    _propetyObjs[i].SetActive(false);
                    _propetyNumTexts[i].text = "";
                }
            }
            
            int chipNum = DataHelper.CurUserInfoData.equipmentChips.GetValueOrDefault(_id, 0);

            float targetChipNumTmp = GlobalValueManager.EquipmentUpGradeChipNum;
            for (int i = 0; i < curLevel - 1; i++)
            {
                targetChipNumTmp *= GlobalValueManager.EquipmentUpGradeChipUpGradeNum;
            }

            int targetChipNum = Mathf.CeilToInt(targetChipNumTmp);
            
            float progress = (float)chipNum / targetChipNum;

            _chipNumText.text = new StringBuilder(chipNum + "/" + targetChipNum).ToString();
            _chipBar.fillAmount = progress;

            _chipObj.SetActive(chipNum < targetChipNum);
            _btnLevelUp.SetActive(chipNum >= targetChipNum);

            _chipUpGradeNum = targetChipNum;

            // 提示红点 条件1 可以升级
            bool isShowRedPoint_1 = chipNum >= targetChipNum;
            // 提示红点 条件2 可以查看涂装
            int isShowRedPoint_2 = DataHelper.CurUserInfoData.equipmentPaintNews.GetValueOrDefault(_id, 1);

            _redPoint.SetActive(isShowRedPoint_1 || isShowRedPoint_2 == 0);

            // 测试模式 免费升级
            if (ConfigManager.Instance.isDebug)
            {
                _btnLevelUp.SetActive(true);
                _chipUpGradeNum = 0;
            }
        }

        /// <summary>
        /// 设置选中框
        /// </summary>
        /// <param name="isSelect">是否选中</param>
        internal void SetSelect(bool isSelect)
        {
        }

        /// <summary>
        /// 按钮 Item
        /// </summary>
        private void OnBtnItem()
        {
            openBuildMainUi.OnSelectItem(this, true);
        }

        /// <summary>
        /// 按钮 升级
        /// </summary>
        private void OnBtnLevelUp()
        {
            AudioHandler._instance.PlayAudio(MainManager._instance.audioEquipmentLvUp);
            GameSdkManager._instance._sdkScript.ShortVibrateControl();
            List<string> modifyKeys = new List<string>();
            // 消耗碎片 非测试模式执行消耗 测试模式没有消耗
            if (!ConfigManager.Instance.isDebug)
            {
                DataHelper.CurUserInfoData.equipmentChips[_id] -= _chipUpGradeNum;
                modifyKeys.Add("equipmentChips");
            }
            // 升级部件
            if (DataHelper.CurUserInfoData.equipments.ContainsKey(_id))
            {
                // 正常逻辑 升级部件
                DataHelper.CurUserInfoData.equipments[_id] += 1;
                modifyKeys.Add("equipments");
            }
            else
            {
                // 保底容错 测试模式下未获取的装备 创建等级数据
                DataHelper.CurUserInfoData.equipments.Add(_id, 2);
                modifyKeys.Add("equipments");
            }
            
            // 完成日常任务 升级X次部件 TaskID:3
            DataHelper.CompleteDailyTask(3, 1, 0);
            modifyKeys.Add("taskInfo1");
            // 完成成就任务 累计升级X次部件 TaskID:4
            DataHelper.CompleteGloalTask(4, 1);
            // 完成成就任务 任意一个部件升级到X级 TaskID:16
            DataHelper.CompleteGloalTask(16, DataHelper.CurUserInfoData.equipments.GetValueOrDefault(_id, 1));
            modifyKeys.Add("taskInfo2");
            
            // 升级新获得的配件涂装状态(涂装按钮提示红点)
            if (!DataHelper.CurUserInfoData.equipmentPaintNews.ContainsKey(_id))
            {
                // 当前列表中没有当前配件的状态
                if (DataHelper.CurUserInfoData.equipEquipments[_type] == _id)
                {
                    // 当前升级的部件 == 当前装备中的部件
                    DataHelper.CurUpGradeEquipment = _id;
                    DataHelper.CurUserInfoData.equipmentPaintNews.Add(_id, 0);
                    modifyKeys.Add("equipmentPaintNews");
                }
            }

            // 保存数据
            DataHelper.ModifyLocalData(modifyKeys, () => { });
            
            // 刷新数据显示
            // SetData(_index, _id);
            
            // 刷新属性
            openBuildMainUi.RefreshPropety(false);
            // 升级完成
            openBuildMainUi.UpGradeComplete(this);
            // 刷新提示红点
            EventManager<int>.Send(CustomEventType.RefreshRedPoint, 3);
            // 刷新飞机部件涂装
            MainManager._instance.RefreshPlaneColor();
            
            // 上报自定义分析数据 事件: 装备升星
            int reportLv = DataHelper.CurUserInfoData.equipments.GetValueOrDefault(_id, 1);
            GameSdkManager._instance._sdkScript.ReportAnalytics("EquipmentUp", "EquipmentLv", reportLv);
        }
    }
}