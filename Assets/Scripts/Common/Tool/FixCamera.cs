using UnityEngine;
using UnityEngine.Rendering;

namespace Common.Tool
{
    public class FixCamera : MonoBehaviour
    {
        void OnEnable()
        {
            RenderPipelineManager.beginCameraRendering += OnBeginCameraRendering;
            RenderPipelineManager.endCameraRendering += OnEndCameraRendering;
        }
        void OnDisable()
        {
            RenderPipelineManager.beginCameraRendering -= OnBeginCameraRendering;
            RenderPipelineManager.endCameraRendering -= OnEndCameraRendering;
        }
        void OnBeginCameraRendering(ScriptableRenderContext ctx, Camera cam)
        {
            cam.cullingMatrix = Matrix4x4.Ortho(-99, 99, -99, 99, 0.3f, 200) * cam.worldToCameraMatrix;
        }
        void OnEndCameraRendering(ScriptableRenderContext ctx, Camera cam)
        {
            cam.ResetCullingMatrix();
        }
    }
}
