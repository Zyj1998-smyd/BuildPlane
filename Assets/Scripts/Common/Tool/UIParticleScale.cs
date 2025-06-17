using System.Collections.Generic;
using UnityEngine;

namespace Common
{
    public class UIParticleScale : MonoBehaviour {
        private List<float> m_initialSizes = new List<float>();

        public void Awake() {
            // Save off all the initial scale values at start.
            ParticleSystem[] particles = gameObject.GetComponentsInChildren<ParticleSystem>();
            for (int i=0;i<particles.Length;i++) {
                m_initialSizes.Add(particles[i].main.startSize.constant);
            
                ParticleSystemRenderer ParticleSystemRenderer = particles[i].GetComponent<ParticleSystemRenderer>();
                if (ParticleSystemRenderer) {
                    m_initialSizes.Add(ParticleSystemRenderer.lengthScale);
                    m_initialSizes.Add(ParticleSystemRenderer.velocityScale);
                }
            }
        }

        public void Start() {
            float designWidth = 720;//开发时分辨率宽
            float designHeight = 1280;//开发时分辨率高
            float designScale = designWidth / designHeight;
            float scaleRate = (float)Screen.width / (float)Screen.height;
            float scaleFactor = scaleRate / designScale;

            // Scale all the particle components based on parent.
            int arrayIndex = 0;
            ParticleSystem[] particles = gameObject.GetComponentsInChildren<ParticleSystem>();
            for (int i = 0; i < particles.Length; i++) {
                float rate;
                if (scaleRate < designScale) {
                    rate = scaleFactor;
                }
                else {
                    rate = 1;
                }

                var mainTmp = particles[i].main;
                mainTmp.startSize = m_initialSizes[arrayIndex++] * rate;
                ParticleSystemRenderer ParticleSystemRenderer = particles[i].GetComponent<ParticleSystemRenderer>();
                if (ParticleSystemRenderer) {
                    ParticleSystemRenderer.lengthScale = m_initialSizes[arrayIndex++] * rate;
                    ParticleSystemRenderer.velocityScale = m_initialSizes[arrayIndex++] * rate;
                }
            }
        }
    }
}
// using System.Collections.Generic;
// using UnityEngine;
//
// namespace Common
// {
//     public class UIParticleScale : MonoBehaviour
//     {
//         private List<ScaleData> scaleDatas;
//
//         void Awake()
//         {
//             scaleDatas = new List<ScaleData>();
//             foreach (ParticleSystem p in transform.GetComponentsInChildren<ParticleSystem>(true))
//             {
//                 scaleDatas.Add(new ScaleData() {transform = p.transform, beginScale = p.transform.localScale});
//             }
//         }
//
//         void Start()
//         {
//             float designWidth = 720; //开发时分辨率宽
//             float designHeight = 1600; //开发时分辨率高
//             float designScale = designWidth / designHeight;
//             float scaleRate = Screen.width / (float) Screen.height;
//
//             foreach (ScaleData scale in scaleDatas)
//             {
//                 if (scale.transform != null)
//                 {
//                     if (scaleRate < designScale)
//                     {
//                         float scaleFactor = scaleRate / designScale;
//                         scale.transform.localScale = scale.beginScale * scaleFactor;
//                     }
//                     else
//                     {
//                         scale.transform.localScale = scale.beginScale;
//                     }
//                 }
//             }
//         }
//
//         class ScaleData
//         {
//             public Transform transform;
//             public Vector3 beginScale = Vector3.one;
//         }
//     }
// }