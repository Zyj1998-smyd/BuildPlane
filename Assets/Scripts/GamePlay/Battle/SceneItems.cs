using System.Collections.Generic;
using UnityEngine;

namespace GamePlay.Battle
{
    public class SceneItems : MonoBehaviour
    {
        private List<GameObject> subItems = new List<GameObject>();
        private void Awake()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                subItems.Add(transform.GetChild(i).gameObject);
            }
        }

        private void OnEnable()
        {
            for (int i = 0; i < subItems.Count; i++)
            {
                subItems[i].SetActive(true);
            }
        }

        void FixedUpdate()
        {
            if (BattleManager._instance.bodyCenter.transform.position.z - transform.position.z > 30)
            {
                gameObject.SetActive(false);
            }
        }


        
    }
}
