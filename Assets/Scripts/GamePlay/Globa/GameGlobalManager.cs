using System;
using Common.GameRoot.AudioHandler;
using Common.LoadRes;
using Data;
using GamePlay.Globa.GlobaOpenBox;
using UnityEngine;
using UnityEngine.UI;

namespace GamePlay.Globa
{
    public class GameGlobalManager : MonoBehaviour
    {
        public static GameGlobalManager _instance;

        /** 通用音效 错误提示 */
        public AudioClip audioError;
        /** 通用音效 按钮点击 */
        public AudioClip audioBtnClick;
        /** 通用音效 弹窗打开 */
        public AudioClip audioPopOpen;
        /** 通用音效 弹窗关闭 */
        public AudioClip audioPopClose;
        /** 通用音效 恭喜获得 */
        public AudioClip audioGetItem;
        
        [HideInInspector] public GlobaCanvas.GlobaCanvas _globaCanvas;
        
        private bool isLoading;

        internal GlobalOpenBox _globalOpenBox;
        
        private void Awake()
        {
            if (_instance == null) _instance = this;
            else Destroy(gameObject);
        }
        
        private void Start()
        {
            CreateGlobaCanvas();
            CreateGlobaOpenBox();
        }
        
        private void CreateGlobaCanvas()
        {
            if (_globaCanvas != null) return;
            // var assetsPath = new StringBuilder("Prefabs/UI/Globa/CanvasGloba").ToString();
            var assetsPath = "CanvasGloba";
            LoadResources.XXResourcesLoad(assetsPath, handleTmp =>
            {
                RectTransform rectTranTmp = Instantiate(handleTmp.GetComponent<RectTransform>());
                _globaCanvas = rectTranTmp.GetComponent<GlobaCanvas.GlobaCanvas>();
                // _globaCanvas.Initialize();
                DontDestroyOnLoad(_globaCanvas);
                SetCanvasUiMain();
            },LoadResources.AssetsGroup.globa);
        }
        
        public void SetCanvasUiMain(Camera cameraTmp = null)
        {
            if (_globaCanvas.canvasMe.worldCamera) return;
            if (!cameraTmp)
                _globaCanvas.canvasMe.worldCamera = GameObject.Find("/CamUi2D").GetComponent<Camera>();
            else
                _globaCanvas.canvasMe.worldCamera = cameraTmp;
        }
        
        /** 弹出提示Tips */
        public void ShowTips(string textTmp)
        {
            AudioHandler._instance.PlayAudio(audioError);
            _globaCanvas.ShowTips(textTmp);
        }
        

        #region 加载场景

        public void LoadScene(string nextSceneName)
        {
            if (isLoading) return;

            isLoading = true;
            DataHelper.nextSceneName = nextSceneName;
            ShowLoadSceneStart();
        }

        private void ShowLoadSceneStart()
        {
            _globaCanvas.ShowLoadSceneStart();
        }
        
        public void ShowLoadSceneEnd(int loadIngNumMaxTmp, Action callback)
        {
            _globaCanvas.ShowLoadSceneEnd(loadIngNumMaxTmp, callback);
            isLoading = false;
        }
        
        #endregion


        /// <summary>
        /// 加载图片
        /// </summary>
        /// <param name="image">图片组件</param>
        /// <param name="imageName">图片名称</param>
        public void SetImage(Image image, string imageName)
        {
            var imagePath = imageName;
            LoadResources.XXResourcesLoad(imagePath, sprite =>
            {
                if (image.IsDestroyed()) return;
                image.sprite = sprite;
            });
        }

        #region 开箱

        private void CreateGlobaOpenBox()
        {
            if (_globalOpenBox != null) return;
            var assetsPath = "OpenBox";
            LoadResources.XXResourcesLoad(assetsPath, handleTmp =>
            {
                GameObject openBox = Instantiate(handleTmp);
                openBox.SetActive(false);
                _globalOpenBox = openBox.GetComponent<GlobalOpenBox>();
                DontDestroyOnLoad(_globalOpenBox);

                SetCanvasUiMainOpenBox();
            }, LoadResources.AssetsGroup.globa);
        }

        public void SetCanvasUiMainOpenBox(Camera cameraTmp = null)
        {
            Debug.Log(1);
            
            if (_globalOpenBox.canvasMe.worldCamera) return;
            if (!cameraTmp)
            {
                Debug.Log(2);
                _globalOpenBox.canvasMe.worldCamera = GameObject.Find("/CamUi2D").GetComponent<Camera>();
            }
            else
            {
                Debug.Log(3);
                _globalOpenBox.canvasMe.worldCamera = cameraTmp;
            }
           
        }

        public void OpenBox(int boxId)
        {
            _globalOpenBox.gameObject.SetActive(true);
            _globalOpenBox.InitialOpenBox(boxId);
        }

        #endregion

        #region 全局弹窗

        /// <summary>
        /// 打开/关闭 货币不足
        /// </summary>
        /// <param name="isOpen">打开/关闭</param>
        /// <param name="type">类型 1: 金币 2: 钻石</param>
        public void OpenNoMoney(bool isOpen, int type)
        {
            _globaCanvas.OpenNoMoney(isOpen, type);
        }
        
        /// <summary>
        /// 打开/关闭 恭喜获得
        /// </summary>
        /// <param name="isOpen">打开/关闭</param>
        public void OpenGetItem(bool isOpen)
        {
            _globaCanvas.OpenGetItem(isOpen);
        }

        #endregion
    }
}