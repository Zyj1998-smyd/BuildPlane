using System;
using System.Threading;
using Common.GameRoot.AudioHandler;
using Common.Tool;
using Cysharp.Threading.Tasks;
using Data;
using UnityEngine;

namespace GamePlay.Battle
{
    public class CupControl : MonoBehaviour
    {
        private TeaModle _teaModle;
        public float nowHight;
        public Color color;
        public int cupLogoId;

        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        /** 杯子动画 */
        private Animation _animation;
        
        /** 倒水水流颜色 */
        private static readonly int BaseColor = Shader.PropertyToID("_BaseColor");
        /** 倒水水流 */
        private Material _waterFlow;

        /** 水花 */
        private Transform _effectWater;

        /** 品牌Logo图片 */
        private static readonly int LogoTex = Shader.PropertyToID("_BaseMap");
        /** 品牌Logo */
        private Material _logo;

        /** 杯口颜色 */
        private static readonly int CupTopColor = Shader.PropertyToID("_MainColor");
        /** 杯口 */
        private Material _cupTop;

        /** 杯垫 */
        private Material _cupPlane;
        /** 杯垫颜色 */
        private static readonly int CupPlaneColor = Shader.PropertyToID("_MatColor");
        
        /** 杯型 0: 中杯 1: 大杯 */
        public int type;
        /** 颜色ID */
        public int colorId;
        
        public AudioClip pourOutAudio;

        /** 订单完成回调 */
        private Action _orderCompleteCb;
        /** 订单切换回调 */
        private Action _orderChangeCb;
        /** 创建新的订单回调 */
        private Action _createNewCb;
        /** 订单进入回调 */
        private Action _orderInCb;

        /** 订单号 */
        public int orderNum;

        /** 订单杯动画操作节点 */
        public Transform cupAniPoint;
        
        private void Awake()
        {
            // 动画组件
            _animation = gameObject.GetComponent<Animation>();
            
            // 备料杯
            if (transform.Find("CentrePoint/Tea"))
                _teaModle = transform.Find("CentrePoint/Tea").GetComponent<TeaModle>();
            if (transform.Find("CentrePoint/WaterFlow"))
                _waterFlow = transform.Find("CentrePoint/WaterFlow").GetComponent<Renderer>().material;
            if (transform.Find("EffectWater"))
            {
                _effectWater = transform.Find("EffectWater");
                _effectWater.gameObject.SetActive(false);
            }
            
            // 订单杯
            if (transform.Find("Cup/Tea"))
                _teaModle = transform.Find("Cup/Tea").GetComponent<TeaModle>();
            if (transform.Find("Cup/Mat"))
                _cupPlane = transform.Find("Cup/Mat").GetComponent<Renderer>().material;
            if (transform.Find("Cup/EffectWater"))
            {
                _effectWater = transform.Find("Cup/EffectWater");
                _effectWater.gameObject.SetActive(false);
            }

            if (transform.Find("Cup/CupL/CupLLogo"))
                _logo = transform.Find("Cup/CupL/CupLLogo").GetComponent<Renderer>().material;
            if (transform.Find("Cup/CupL"))
                _cupTop = transform.Find("Cup/CupL").GetComponent<Renderer>().material;
            if (transform.Find("Cup"))
                cupAniPoint = transform.Find("Cup");
        }

        private void Start()
        {
            // nowHight = 1;
            // _teaModle.SetColor(color);
            // _teaModle.SetTopColor(color);
        }

        private void Update()
        {
            // 大杯：0，0.4，0.7，1；中杯：0，0.5，1，小杯(备料)：0，1；
            // if (Input.GetKey(KeyCode.O))
            // {
            //     PourOutStart(0f);
            // }
            // if (Input.GetKey(KeyCode.A))
            // {
            //     PourAddStart(1f);
            // }
        }

        private void OnDestroy()
        {
            _cancellationTokenSource.Cancel();
        }

        public void PourOutStart(float targetHight)
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource = new CancellationTokenSource();
            _ = PourOut(targetHight);
            
            if (gameObject.name != "Cup")
            {
                _animation.Play("CupSPourOut");
                _waterFlow.SetColor(BaseColor, color);
            }
                
            AudioHandler._instance.PlayAudio(pourOutAudio);
        }
        
        public void PourAddStart(float targetHight)
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource = new CancellationTokenSource();
            _ = PourAdd(targetHight);

            if (gameObject.name != "Cup")
            {
                _animation.Play("CupSStandAdd");
            }
        }
        
        async UniTask PourOut(float targetHight)
        {
            while (nowHight > targetHight)
            {
                nowHight -= (Time.deltaTime * 3);
                if (gameObject.name == "Cup")
                    _teaModle.RefreshModle(nowHight);

                await UniTask.DelayFrame(1, cancellationToken: _cancellationTokenSource.Token);
            }

            nowHight = targetHight;
        }
        
        async UniTask PourAdd(float targetHight)
        {
            while (nowHight < targetHight)
            {
                // var offset = _animation != null ? -0.4f : 0f;
                var offset = -0.5f;
                var posTmp = transform.position;
                _effectWater.position = new Vector3(posTmp.x, offset + nowHight, posTmp.z);
                _effectWater.gameObject.SetActive(false);
                _effectWater.gameObject.SetActive(true);

                nowHight += (Time.deltaTime * 3); // 可以处理成固定总时间
                if (gameObject.name == "Cup")
                    _teaModle.RefreshModle(nowHight);

                await UniTask.DelayFrame(1, cancellationToken: _cancellationTokenSource.Token);
            }

            nowHight = targetHight;
            if (gameObject.name != "Cup")
                _animation.Play("CupSStandFull");
        }

        /// <summary>
        /// 初始化杯子
        /// </summary>
        public void Init()
        {
            nowHight = 0;
            _teaModle.SetColor(color);
            _teaModle.SetTopColor(color);

            if (gameObject.name == "Cup")
            {
                // 订单杯
                _teaModle.RefreshModle(nowHight);
                _logo.SetTexture(LogoTex, BattleManager._instance.OrderCupBodyLogos[cupLogoId]);
                _cupTop.SetColor(CupTopColor, color);
                _cupPlane.SetColor(CupPlaneColor, color);
            }
            else
            {
                // 备料杯
                _animation.Play("CupSStandEmpty");
            }
        }

        /// <summary>
        /// 设置颜色
        /// </summary>
        /// <param name="colorIdTmp">颜色ID</param>
        public void SetColor(int colorIdTmp)
        {
            colorId = colorIdTmp;
            color = ToolFunManager.HexToColor(GlobalValueManager.ColorList[colorIdTmp]);
            _teaModle.SetColor(color);
            _teaModle.SetTopColor(color);
        }

        /// <summary>
        /// 订单完成
        /// <param name="cb">订单完成动画播放完成回调</param>
        /// <param name="createNewCb">创建新的订单回调</param>
        /// </summary>
        public void OrderComplete(Action cb, Action createNewCb)
        {
            _orderCompleteCb = cb;
            _createNewCb = createNewCb;
            _animation.Play("CupLDone");
        }

        /// <summary>
        /// 订单杯切换
        /// </summary>
        /// <param name="cb">订单杯切换回调</param>
        public void OrderCupChange(Action cb)
        {
            _orderChangeCb = cb;
            _animation.Play("CupLOut");
        }

        /// <summary>
        /// 订单杯进入
        /// </summary>
        /// <param name="cb">订单杯进入回调</param>
        public void OrderCupIn(Action cb)
        {
            _orderInCb = cb;
            _animation.Play("CupLIn");
        }

        public void AniEventStartPourOut()
        {
            BattleManager._instance.PrepareCupPourStart();
        }
        public void AniEventEndPourOut()
        {
        }

        public void AniEventReSet()
        {
            BattleManager._instance.PrepareCupPourReSet();
        }

        /// <summary>
        /// 动画事件 订单完成动画播放结束
        /// </summary>
        public void AniEventOrderComplete()
        {
            _orderCompleteCb?.Invoke();
        }

        /// <summary>
        /// 动画事件 订单完成动画第40帧 创建新的订单
        /// </summary>
        public void AniEventCreateNewOrderCup()
        {
            _createNewCb?.Invoke();
        }

        /// <summary>
        /// 动画事件 订单切换动画播放结束
        /// </summary>
        public void AniEventOrderChange()
        {
            _orderChangeCb?.Invoke();
        }

        /// <summary>
        /// 动画事件 订单杯子进入动画播放结束
        /// </summary>
        public void AniEventOrderIn()
        {
            _orderInCb?.Invoke();
        }
    }
}