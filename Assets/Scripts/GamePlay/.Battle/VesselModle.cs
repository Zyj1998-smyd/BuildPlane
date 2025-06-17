using UnityEngine;

namespace GamePlay.Battle
{
    public class VesselModle : MonoBehaviour
    {
        private Renderer meshRenderer;
        private Material _material;

        private static readonly int HightMax = Shader.PropertyToID("_HightMax");
        private static readonly int HightMin = Shader.PropertyToID("_HightMin");
        private static readonly int WaterLevel = Shader.PropertyToID("_WaterLevel");
        private static readonly int ColorA = Shader.PropertyToID("_ColorA");
        private static readonly int ColorB = Shader.PropertyToID("_ColorB");
        private static readonly int ColorC = Shader.PropertyToID("_ColorC");
        private static readonly int ColorTop = Shader.PropertyToID("_ColorTop");

        void Awake()
        {
            meshRenderer = GetComponent<Renderer>();
            _material = meshRenderer.material;
        }

        public void SetColor(Color colorA, Color colorB, Color colorC)
        {
            _material.SetColor(ColorA, colorA);
            _material.SetColor(ColorB, colorB);
            _material.SetColor(ColorC, colorC);
        }
        public void SetTopColor(Color color)
        {
            _material.SetColor(ColorTop, color);
        }

        // public void RefreshModle(float targetHight)
        // {
        //     Bounds bounds = meshRenderer.bounds;
        //     _material.SetFloat(HightMax, bounds.max.y - transform.position.y);
        //     _material.SetFloat(HightMin, bounds.min.y - transform.position.y);
        //     
        //     _material.SetFloat(WaterLevel, targetHight);
        // }
        
        
        
        
    }
}