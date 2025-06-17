using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace GamePlay.Battle
{
    public class AirWall : MonoBehaviour
    {
        private Material wallMatL, wallMatR;

        private static readonly int PlanePoint = Shader.PropertyToID("_PlanePoint");

        private CancellationTokenSource _cancellationToken;

        private void Start()
        {
            wallMatL = transform.Find("AirWallL").GetComponent<MeshRenderer>().material;
            wallMatR = transform.Find("AirWallR").GetComponent<MeshRenderer>().material;
            _cancellationToken = new CancellationTokenSource();
            CheckAirWall().Forget();
        }

        private void OnDisable()
        {
            _cancellationToken?.Cancel();
            _cancellationToken?.Dispose();
            _cancellationToken = null;
        }


        async UniTask CheckAirWall()
        {
            while (!_cancellationToken.IsCancellationRequested)
            {
                if (BattleManager._instance.bodyCenter)
                {
                    transform.position = Vector3.forward * BattleManager._instance.bodyCenter.position.z;
                    if (BattleManager._instance.bodyCenter.position.x < -20 || BattleManager._instance.bodyCenter.position.x > 20)
                    {
                        wallMatL.SetVector(PlanePoint, BattleManager._instance.bodyCenter.position);
                        wallMatR.SetVector(PlanePoint, BattleManager._instance.bodyCenter.position);
                    }
                }

                await UniTask.Yield(cancellationToken: _cancellationToken.Token);
            }
        }



    }
}
