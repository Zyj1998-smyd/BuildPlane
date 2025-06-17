using Common.Login;
using Platform.FuncHandler;
using Platform.ServerHandler;
using UnityEngine;

namespace Platform
{
    public class GameSdkManager : MonoBehaviour
    {
        /** SDK管理类 */
        public static GameSdkManager _instance;

        /** SDK组件 */
        internal SdkHandler _sdkScript;

        /** 服务器组件 */
        internal ServerHandler.ServerHandler _serverScript;

        /** 渠道标识 */
        private string userChannel;

        private void Awake()
        {
            if (_instance == null)
                _instance = this;
            else
                Destroy(gameObject);
        }

        private void Start()
        {
            SdkFunc();
            gameObject.AddComponent<GameLogin>().ReadyStart(userChannel);
        }

        private void SdkFunc()
        {
            userChannel = "Plane_Editor";
            _sdkScript = gameObject.AddComponent<Sdk_Editor>();
            _serverScript = gameObject.AddComponent<Server_Editor>();
        }
    }
}