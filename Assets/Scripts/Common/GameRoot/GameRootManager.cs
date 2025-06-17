using Common.GameRoot.TouchHandler;
using UnityEngine;

namespace Common.GameRoot
{
    public class GameRootManager : MonoBehaviour
    {
        public static GameRootManager           _instance;
        private        AudioHandler.AudioHandler _audioHandler;

        private void Awake()
        {
            if (_instance != null)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
            DontDestroyOnLoad(gameObject);

            Application.targetFrameRate = 60;
        }

        private void Start()
        {
            // InitTouch(); // 初始化触摸
            InitAudio(); // 初始化音频
        }

        /// <summary>
        /// 初始化触摸
        /// </summary>
        internal void InitTouch()
        {
#if UNITY_EDITOR || PF_WEB
            // 当前运行环境是编辑器
            gameObject.AddComponent<Touch_Windows>();
#elif UNITY_WEBGL && PF_WEB
            // 当前运行环境是WEB
            Debug.Log("========================= 当前运行环境是WEB =========================");
            gameObject.AddComponent<Touch_Mobile>();
#elif UNITY_ANDROID
            // 当前运行环境是安卓
            gameObject.AddComponent<Touch_Mobile>();
#elif UNITY_IOS
            // 当前运行环境是IOS
            gameObject.AddComponent<Touch_Mobile>();
#elif UNITY_WEBGL && PF_TT
            // 当前运行环境是抖音
            gameObject.AddComponent<Touch_Mobile>();
#elif UNITY_WEBGL && PF_WX
            // 当前运行环境是微信
            gameObject.AddComponent<Touch_Wechat>();
#endif
        }

        /// <summary>
        /// 初始化音频
        /// </summary>
        private void InitAudio()
        {
            if (_audioHandler) return;

            var audioObj = new GameObject
            {
                transform =
                {
                    parent = transform
                },
                name = "AudioManager"
            };

            _audioHandler = audioObj.AddComponent<AudioHandler.AudioHandler>();
        }





    }
}