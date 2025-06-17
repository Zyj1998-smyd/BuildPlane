using System;
using System.Collections.Generic;
using Common.GameRoot.AudioHandler;
using Common.Tool;
using Cysharp.Threading.Tasks;
using Data;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GamePlay.Battle
{
    public class VesselControl : MonoBehaviour
    {
        private VesselModle _vesselModle;
        public float nowHight;
        public Color colorA, colorB, colorC;

        public List<int> colorIds = new List<int>(3);

        private Animation _animation;
        
        /** 倒水音效 */
        public AudioClip pourOutAudio;
        /** 不可使用音效 */
        public AudioClip vesselErrorAudio;

        private static readonly int BaseColor = Shader.PropertyToID("_BaseColor");
        private Material _waterFlow;

        public int vesselId;

        public int type;

        /** 不可使用动画回调 */
        private Action _errorCallBack;

        private void Awake()
        {
            _vesselModle = transform.Find("CentrePoint/Tea").GetComponent<VesselModle>();
            
            _animation = gameObject.GetComponent<Animation>();
            _waterFlow = transform.Find("CentrePoint/WaterFlow").GetComponent<Renderer>().material;
        }

        private void Start()
        {
            // nowHight = 1;
            // _vesselModle.SetColor(colorA, colorB, colorC);
            // _vesselModle.SetTopColor(colorA);
        }

        private void Update()
        {
            // if (Input.GetKey(KeyCode.P))
            // {
            //     PourOutStart();
            // }
        }

        public void PourOutStart()
        {
            switch (nowHight)
            {
                case > 0.75f:
                    _animation.Play("VesselPourOut1");
                    _waterFlow.SetColor(BaseColor, colorA);
                    nowHight = 0.75f;
                    break;
                case > 0.4f:
                    _animation.Play("VesselPourOut2");
                    _waterFlow.SetColor(BaseColor, colorB);
                    nowHight = 0.4f;
                    break;
                case > 0f:
                    _animation.Play("VesselPourOut3");
                    _waterFlow.SetColor(BaseColor, colorC);
                    nowHight = 0;
                    break;
            }
            
            AudioHandler._instance.PlayAudio(pourOutAudio);
        }

        /// <summary>
        /// 初始化瓶子
        /// </summary>
        public void Init(int typeTmp)
        {
            type = typeTmp;
            switch (typeTmp)
            {
                case 0:
                    nowHight = 1.15f;
                    _vesselModle.SetColor(colorA, colorB, colorC);
                    _vesselModle.SetTopColor(colorA);
                    InitAni("VesselStand1");
                    break;
                case 1:
                    nowHight = 0.75f;
                    _vesselModle.SetColor(colorA, colorB, colorC);
                    _vesselModle.SetTopColor(colorB);
                    InitAni("VesselStand2");
                    break;
                case 2:
                    nowHight = 0.4f;
                    _vesselModle.SetColor(colorA, colorB, colorC);
                    _vesselModle.SetTopColor(colorC);
                    InitAni("VesselStand3");
                    break;
            }
        }

        /// <summary>
        /// 初始化瓶子动画
        /// </summary>
        private void InitAni(string aniName)
        {
            _animation.Play(aniName);
        }

        /// <summary>
        /// 原料瓶递补移动完成
        /// </summary>
        public void MoveSupplyComplete()
        {
            _animation.Play("VesselMove");
        }

        /// <summary>
        /// 原料瓶不可使用
        /// <param name="isGuide">是否新手引导</param>
        /// <param name="cb">回调</param>
        /// </summary>
        public void NoCanUseError(bool isGuide, Action cb)
        {
            if (!isGuide)
            {
                // 不是新手引导 原逻辑不变
                if (BattleManager._instance.vesselErrorAniPlaying) return;
                BattleManager._instance.vesselErrorAniPlaying = true;
                _errorCallBack = () =>
                {
                    cb();
                    DelayTime.DelayFrame(() =>
                    {
                        BattleManager._instance.vesselErrorAniPlaying = false;
                    }, 1 * GlobalValueManager.VesselErrorTime, this.GetCancellationTokenOnDestroy());
                };
                _animation.Play("VesselError");
                AudioHandler._instance.PlayAudio(vesselErrorAudio);
            }
            else
            {
                // 是新手引导
                _errorCallBack = cb;
                _animation.Play("VesselError");
                AudioHandler._instance.PlayAudio(vesselErrorAudio);
            }
        }

        /// <summary>
        /// 刷新原料瓶
        /// </summary>
        public void ChangeVessel()
        {
            _animation.Play("VesselChange");
        }

        public void AniEventStartPourOut()
        {
            BattleManager._instance.CupPourStart();
        }

        public void AniEventEndPourOut()
        {
            BattleManager._instance.CupPourEnd();
            
            switch (nowHight)
            {
                case > 0.4f:
                    _vesselModle.SetTopColor(colorB);
                    break;
                case > 0f:
                    _vesselModle.SetTopColor(colorC);
                    break;
            }
        }

        public void AniEventReSet()
        {
        }

        public void AniEventMoveSupplyComplete()
        {
            // BattleManager._instance.VesselUseEndMoveComplete();
        }
        
        /// <summary>
        /// 随机变颜色
        /// </summary>
        public void AniEventChangeColor()
        {
            var colorTmpA = ToolFunManager.HexToColor(GlobalValueManager.ColorList[Random.Range(1, GlobalValueManager.ColorList.Count)]);
            var colorTmpB = ToolFunManager.HexToColor(GlobalValueManager.ColorList[Random.Range(1, GlobalValueManager.ColorList.Count)]);
            var colorTmpC = ToolFunManager.HexToColor(GlobalValueManager.ColorList[Random.Range(1, GlobalValueManager.ColorList.Count)]);
            _vesselModle.SetColor(colorTmpA, colorTmpB, colorTmpC);
            _vesselModle.SetTopColor(colorTmpA);
        }
        
        /// <summary>
        /// 随机变颜色结束，最终颜色
        /// </summary>
        public void AniEventChangeColorEnd()
        {
            var colorTmpA = DataHelper.VesselDatas[vesselId][0];
            var colorTmpB = DataHelper.VesselDatas[vesselId][1];
            var colorTmpC = DataHelper.VesselDatas[vesselId][2];
            DataHelper.VesselStatus[vesselId] = true;

            colorIds = new List<int>(3) { colorTmpA, colorTmpB, colorTmpC };
            colorA = colorTmpA == 0 ? Color.white : ToolFunManager.HexToColor(GlobalValueManager.ColorList[colorIds[0]]);
            colorB = colorTmpB == 0 ? Color.white : ToolFunManager.HexToColor(GlobalValueManager.ColorList[colorIds[1]]);
            colorC = colorTmpC == 0 ? Color.white : ToolFunManager.HexToColor(GlobalValueManager.ColorList[colorIds[2]]);
            
            Init(type);

            if (BattleManager._instance.itemUsing)
            {
                ConfigManager.Instance.ConsoleLog(0, "退出刷新原料道具使用状态...");
                BattleManager._instance.itemUsing = false;
            }
        }

        /// <summary>
        /// 不可使用动画播放完成
        /// </summary>
        public void AniEventErrorEnd()
        {
            _errorCallBack?.Invoke();
        }
    }
}