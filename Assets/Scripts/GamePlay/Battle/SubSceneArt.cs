using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace GamePlay.Battle
{
    public class SubSceneArt : MonoBehaviour
    {
        private readonly List<GameObject> subObj = new List<GameObject>();
        private          PathSampler      _pathSampler;

        private CancellationTokenSource _cancellationToken;

        private void Start()
        {
            subObj.Add(gameObject);

            _cancellationToken = new CancellationTokenSource();
            GetAllChild(transform).Forget();
        }

        private void OnDisable()
        {
            _cancellationToken?.Cancel();
            _cancellationToken?.Dispose();
            _cancellationToken = null;
        }

        async UniTask GetAllChild(Transform tramTmp)
        {
            if (tramTmp == null) return;

            foreach (Transform child in tramTmp)
            {
                subObj.Add(child.gameObject);
                if (child.name == "ControlPoints")
                {
                    _pathSampler = child.GetComponent<PathSampler>();
                }

                GetAllChild(child).Forget();

                await UniTask.Yield(cancellationToken: _cancellationToken.Token);
            }
        }

        internal void SetLayer(int layerNum)
        {
            for (int i = 0; i < subObj.Count; i++)
            {
                subObj[i].layer = layerNum;
            }
        }

        internal void CreateSceneItem()
        {
            if (!_pathSampler) return;
            if (_pathSampler.transform.position.z - 100 < BattleManager._instance.bodyCenter.transform.position.z) return;

            float randomZ = Random.Range(_pathSampler.transform.position.z - 25, _pathSampler.transform.position.z + 25);
            BattleManager._instance.CreateSceneItem(new Vector3(_pathSampler.GetXAtZ(randomZ), 0, randomZ));
        }



    }
}
