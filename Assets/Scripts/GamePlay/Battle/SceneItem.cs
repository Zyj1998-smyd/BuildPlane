using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GamePlay.Battle
{
    public class SceneItem : MonoBehaviour
    {
        public  ItemType itemType;

        private                                     Animation ani;
        [ShowIf("itemType",ItemType.ClockIn)] public int       clockInId;
        
        private bool     useed;

        private void Awake()
        {
            switch (itemType)
            {
                case ItemType.gold:
                    ani = GetComponent<Animation>();
                    break;
            }
        }

        private void OnEnable()
        {
            switch (itemType)
            {
                case ItemType.gold:
                    AnimationState state = ani["GoldRotate"];
                    state.time = Random.Range(0f,1f);
                    break;
            }
            
            useed = false;
        }

        private void OnTriggerEnter(Collider other)
        {
            GetItem();
        }

        void GetItem()
        {
            if (useed) return;
            useed = true;

            switch (itemType)
            {
                case ItemType.speed:
                    BattleManager._instance._planeControl.ThrusterItemStart(BattleManager._instance.propetyThrusterRing);
                    break;
                case ItemType.gold:
                    useed = false;
                    BattleManager._instance.GetGold(100);
                    gameObject.SetActive(false);
                    break;
                case ItemType.ClockIn:
                    BattleManager._instance.ClockIn(clockInId);
                    BattleManager._instance._planeControl.ThrusterItemStart(BattleManager._instance.propetyThrusterRing);
                    break;
            }
            
        }


        
    }

    public enum ItemType
    {
        [LabelText("加速环")]
        speed,
        [LabelText("金币")]
        gold,
        [LabelText("打卡点")]
        ClockIn
    }
}
