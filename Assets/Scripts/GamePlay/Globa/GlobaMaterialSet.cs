using UnityEngine;

namespace GamePlay.Globa
{
    public class GlobaMaterialSet : MonoBehaviour
    {
        private static readonly int OffstZ = Shader.PropertyToID("_OffstZ");
        private static readonly int BendZ  = Shader.PropertyToID("_BendZ");
        private static readonly int OffstY = Shader.PropertyToID("_OffstY");
        private static readonly int BendY  = Shader.PropertyToID("_BendY");

        public void SetBend(bool switchTmp)
        {
            if (switchTmp)
            {
                Shader.SetGlobalFloat(OffstZ, 20f);
                Shader.SetGlobalFloat(BendZ, -0.15f);
                // Shader.SetGlobalFloat(OffstY, 10f);
                // Shader.SetGlobalFloat(BendY, 0.5f);
                Shader.SetGlobalFloat(OffstY, 0);
                Shader.SetGlobalFloat(BendY, 0);
            }
            else
            {
                Shader.SetGlobalFloat(OffstZ, 0);
                Shader.SetGlobalFloat(BendZ, 0);
                Shader.SetGlobalFloat(OffstY, 0);
                Shader.SetGlobalFloat(BendY, 0);
            }
        }




    }
}
