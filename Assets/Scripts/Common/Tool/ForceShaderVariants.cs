using System.Collections.Generic;
using UnityEngine;

namespace Common.Tool
{
    public class ForceShaderVariants : MonoBehaviour
    {
        void Start()
        {
            // Shader.WarmupAllShaders();
            
            List<string> keywords = new List<string> 
            { 
                "_FOG_LINEAR", 
                "_LIGHTMAP_ON", 
                "_MAIN_LIGHT_SHADOWS" ,
                "_MAIN_LIGHT_SHADOWS_CASCADE" 
            };

            // 遍历每个关键字并启用
            foreach (string keyword in keywords)
            {
                Shader.EnableKeyword(keyword);
            }
        }
    }
}
