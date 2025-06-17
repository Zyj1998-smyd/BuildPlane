using GamePlay.Globa;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(GlobaMaterialSet))]
    public class GlobaMaterialSetEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            GlobaMaterialSet script = (GlobaMaterialSet)target;
            
            GUILayout.BeginHorizontal();
            GUI.color = Color.green;
            if (GUILayout.Button("开启环形地图显示"))
            {
                script.SetBend(true);
            }
            GUI.color = Color.red;
            if (GUILayout.Button("关闭环形地图显示"))
            {
                script.SetBend(false);
            }
            GUI.color = Color.white;
            GUILayout.EndHorizontal();
        }
        
        
        
    }
}
