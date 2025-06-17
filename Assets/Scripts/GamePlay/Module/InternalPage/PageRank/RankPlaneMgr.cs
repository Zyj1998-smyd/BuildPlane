using System.Collections.Generic;
using System.Text;
using System.Threading;
using Common.LoadRes;
using Common.Tool;
using Cysharp.Threading.Tasks;
using Data;
using GamePlay.Main;
using Newtonsoft.Json;
using UnityEngine;

namespace GamePlay.Module.InternalPage.PageRank
{
    public class RankPlaneMgr : MonoBehaviour
    {
        /** 玩家ID列表 */
        private string _rankRoleId;
        /** 玩家装备飞机部件列表 */
        private readonly GameObject[] _planeEquipments = new GameObject[7];
        
        /** 排名 */
        private int _rankNum;
        /** 玩家装备飞机部件ID列表 */
        private readonly int[] _planeEquipmentIds = new int[6];
        /** 玩家装备飞机涂装ID列表 */
        private readonly int[][] _planeEquipmentColorIds = new int[6][];
        /** 玩家装备飞机部件等级列表 */
        private readonly int[] _planeEquipmentLvs = new int[6];
        
        /** 材质属性ID */
        private static readonly int MainTexA = Shader.PropertyToID("_MainTextureA");
        /** 材质属性ID */
        private static readonly int MainTexB = Shader.PropertyToID("_MainTextureB");
        /** 材质属性ID */
        private static readonly int MatcapTexA = Shader.PropertyToID("_MatcapTexA");
        /** 材质属性ID */
        private static readonly int MatcapTexB = Shader.PropertyToID("_MatcapTexB");
        /** 材质属性ID */
        private static readonly int LightColor = Shader.PropertyToID("_LightColor");
        
        /** UniTask异步信标 */
        private CancellationTokenSource _cancellationToken;

        /// <summary>
        /// 创建飞机
        /// </summary>
        /// <param name="rankNum">排名</param>
        /// <param name="rankRoleId">玩家ID</param>
        /// <param name="list">飞机部件ID列表</param>
        /// <param name="colorId">涂装ID列表</param>
        /// <param name="levels">飞机部件等级列表</param>
        internal void CreatePlane(int rankNum, string rankRoleId, List<int> list, List<int> colorId, List<int> levels)
        {
            _rankNum = rankNum - 1;
            if (_rankRoleId != rankRoleId)
            {
                _rankRoleId = rankRoleId;
                for (int i = 0; i < list.Count; i++)
                {
                    _planeEquipmentIds[i] = list[i];
                }

                _planeEquipmentColorIds[0] = new int[4];
                for (int i = 0; i < 4; i++)
                {
                    _planeEquipmentColorIds[0][i] = colorId[i];
                }

                _planeEquipmentColorIds[1] = new int[4];
                for (int i = 4; i < 8; i++)
                {
                    _planeEquipmentColorIds[1][i - 4] = colorId[i];
                }

                _planeEquipmentColorIds[2] = new int[4];
                for (int i = 8; i < 12; i++)
                {
                    _planeEquipmentColorIds[2][i - 8] = colorId[i];
                }

                _planeEquipmentColorIds[3] = new int[4];
                for (int i = 12; i < 16; i++)
                {
                    _planeEquipmentColorIds[3][i - 12] = colorId[i];
                }

                _planeEquipmentColorIds[4] = new int[4];
                for (int i = 16; i < 20; i++)
                {
                    _planeEquipmentColorIds[4][i - 16] = colorId[i];
                }

                _planeEquipmentColorIds[5] = new int[4];
                for (int i = 20; i < 24; i++)
                {
                    _planeEquipmentColorIds[5][i - 20] = colorId[i];
                }

                for (int i = 0; i < levels.Count; i++)
                {
                    _planeEquipmentLvs[i] = levels[i];
                }

                CreatePlaneBody();
            }
        }

        /// <summary>
        /// 创建机身
        /// </summary>
        private void CreatePlaneBody()
        {
            if (_planeEquipments[0]) Destroy(_planeEquipments[0]);
            int id = _planeEquipmentIds[0];
            LoadResources.XXResourcesLoad(new StringBuilder("PlaneBody" + id).ToString(), handleTmp =>
            {
                GameObject objTmp = Instantiate(handleTmp, MainManager._instance._rankPlaneTrans[_rankNum]);
                objTmp.transform.position = MainManager._instance._rankPlaneTrans[_rankNum].position;
                _planeEquipments[0] = objTmp;
                SetPlaneMatCap(_planeEquipments[0], 0);
                
                CreatePlaneWingL(objTmp.transform.Find("Point_WingL").position);
                CreatePlaneWingR(objTmp.transform.Find("Point_WingR").position);
                CreatePlanePropeller(objTmp.transform.Find("Point_Propeller").position);
                CreatePlaneFin(objTmp.transform.Find("Point_Fin").position);
            });
        }

        /// <summary>
        /// 创建机翼（左）
        /// </summary>
        private void CreatePlaneWingL(Vector3 pointTmp)
        {
            if (_planeEquipments[2]) Destroy(_planeEquipments[2]);
            int id = _planeEquipmentIds[2] == -1 ? -1 : _planeEquipmentIds[2];
            if (id > -1)
            {
                LoadResources.XXResourcesLoad(new StringBuilder("PlaneWingL" + id).ToString(), handleTmp =>
                {
                    GameObject objTmp = Instantiate(handleTmp, MainManager._instance._rankPlaneTrans[_rankNum]);
                    objTmp.transform.position = pointTmp;
                    _planeEquipments[2] = objTmp;
                    SetPlaneMatCap(_planeEquipments[2], 2);
                    
                    CreatePlaneSpurtL(objTmp.transform.Find("Point_Spurt").position);
                });
            }
        }

        /// <summary>
        /// 创建机翼（右）
        /// </summary>
        private void CreatePlaneWingR(Vector3 pointTmp)
        {
            if (_planeEquipments[3]) Destroy(_planeEquipments[3]);
            int id = _planeEquipmentIds[3] == -1 ? -1 : _planeEquipmentIds[3];
            if (id > -1)
            {
                LoadResources.XXResourcesLoad(new StringBuilder("PlaneWingR" + id).ToString(), handleTmp =>
                {
                    GameObject objTmp = Instantiate(handleTmp, MainManager._instance._rankPlaneTrans[_rankNum]);
                    objTmp.transform.position = pointTmp;
                    _planeEquipments[3] = objTmp;
                    SetPlaneMatCap(_planeEquipments[3], 3);
                    
                    CreatePlaneSpurtR(objTmp.transform.Find("Point_Spurt").position);
                });
            }
        }

        /// <summary>
        /// 创建推进器（左）
        /// </summary>
        private void CreatePlaneSpurtL(Vector3 pointTmp)
        {
            if (_planeEquipments[5]) Destroy(_planeEquipments[5]);
            int id = _planeEquipmentIds[5] == -1 ? -1 : _planeEquipmentIds[5];
            if (id > -1)
            {
                LoadResources.XXResourcesLoad(new StringBuilder("PlaneSpurtL" + id).ToString(), handleTmp =>
                {
                    GameObject objTmp = Instantiate(handleTmp, MainManager._instance._rankPlaneTrans[_rankNum]);
                    objTmp.transform.position = pointTmp;
                    _planeEquipments[5] = objTmp;
                    SetPlaneMatCap(_planeEquipments[5], 5);
                });
            }
        }

        /// <summary>
        /// 创建推进器（右）
        /// </summary>
        private void CreatePlaneSpurtR(Vector3 pointTmp)
        {
            if (_planeEquipments[6]) Destroy(_planeEquipments[6]);
            int id = _planeEquipmentIds[5] == -1 ? -1 : _planeEquipmentIds[5];
            if (id > -1)
            {
                LoadResources.XXResourcesLoad(new StringBuilder("PlaneSpurtR" + id).ToString(), handleTmp =>
                {
                    GameObject objTmp = Instantiate(handleTmp, MainManager._instance._rankPlaneTrans[_rankNum]);
                    objTmp.transform.position = pointTmp;
                    _planeEquipments[6] = objTmp;
                    SetPlaneMatCap(_planeEquipments[6], 5);
                });
            }
        }

        /// <summary>
        /// 创建机头
        /// </summary>
        private void CreatePlanePropeller(Vector3 pointTmp)
        {
            if (_planeEquipments[1]) Destroy(_planeEquipments[1]);
            int id = _planeEquipmentIds[1] == -1 ? -1 : _planeEquipmentIds[1];
            if (id > -1)
            {
                LoadResources.XXResourcesLoad(new StringBuilder("PlanePropeller" + id).ToString(), handleTmp =>
                {
                    GameObject objTmp = Instantiate(handleTmp, MainManager._instance._rankPlaneTrans[_rankNum]);
                    objTmp.transform.position = pointTmp;
                    _planeEquipments[1] = objTmp;
                    SetPlaneMatCap(_planeEquipments[1], 1);
                });
            }
        }

        /// <summary>
        /// 创建机尾
        /// </summary>
        private void CreatePlaneFin(Vector3 pointTmp)
        {
            if (_planeEquipments[4]) Destroy(_planeEquipments[4]);
            int id = _planeEquipmentIds[4] == -1 ? -1 : _planeEquipmentIds[4];
            if (id > -1)
            {
                LoadResources.XXResourcesLoad(new StringBuilder("PlaneFin" + id).ToString(), handleTmp =>
                {
                    GameObject objTmp = Instantiate(handleTmp, MainManager._instance._rankPlaneTrans[_rankNum]);
                    objTmp.transform.position = pointTmp;
                    _planeEquipments[4] = objTmp;
                    SetPlaneMatCap(_planeEquipments[4], 4);
                });
            }
        }
        
        /// <summary>
        /// 设置指定部件的涂装
        /// <param name="equipment">指定部件</param>
        /// <param name="index">列表索引</param>
        /// </summary>
        private void SetPlaneMatCap(GameObject equipment, int index)
        {
            // Debug.Log("涂装列表 = " + JsonConvert.SerializeObject(_planeEquipmentColorIds[index]));
            
            List<string> result = new List<string>(5);
            int level = _planeEquipmentLvs[index];
            if (level >= 3)
            {
                if (_planeEquipmentColorIds[index][2] == 0)
                {
                    result.Add("000000");
                }
                else
                {
                    int colorId = _planeEquipmentColorIds[index][2] % 100 - 1;
                    result.Add(GlobalValueManager.PlaneEquipLightColors[colorId]);
                }
            }
            else
            {
                result.Add("000000");
            }

            switch (level)
            {
                case 1:  // 木质 固定组合
                    result.AddRange(new List<string>(4) { "MainTex0_0", "MainTex0_0", "MatcapTex0_0", "MatcapTex0_1" });
                    break;
                case 2:  // 底漆/装饰各自读取
                case 3:  // 底漆/装饰各自读取
                    List<string> listTmp1 = new List<string>(4) { "", "", "", "" };
                    // 底漆
                    if (_planeEquipmentColorIds[index][0] == 0)
                    {
                        // 透明底漆 显示默认木质
                        listTmp1[0] = "MainTex0_0";
                        listTmp1[2] = "MatcapTex0_0";
                    }
                    else
                    {
                        // 非透明底漆 显示底漆颜色
                        int colorId_1 = _planeEquipmentColorIds[index][0] % 100 - 1;
                        string matcapTexName_1 = new StringBuilder("MatcapTex1_" + colorId_1).ToString();
                        listTmp1[0] = "MainTex1_0";
                        listTmp1[2] = matcapTexName_1;
                    }
                    // 装饰
                    if (_planeEquipmentColorIds[index][1] == 0)
                    {
                        // 透明装饰 显示默认木质
                        listTmp1[1] = "MainTex0_0";
                        listTmp1[3] = "MatcapTex0_1";
                    }
                    else
                    {
                        // 非透明装饰 显示装饰颜色
                        int colorId_2 = _planeEquipmentColorIds[index][1] % 100 - 1;
                        string matcapTexName_2 = new StringBuilder("MatcapTex1_" + colorId_2).ToString();
                        listTmp1[1] = "MainTex1_0";
                        listTmp1[3] = matcapTexName_2;
                    }

                    result.AddRange(listTmp1);
                    break;
                default: // 底漆贴纸
                    if (_planeEquipmentColorIds[index][3] == 0)
                    {
                        // 当前装备中的是透明贴纸
                        List<string> listTmp2 = new List<string>(4) { "", "", "", "" };
                        // 底漆
                        if (_planeEquipmentColorIds[index][0] == 0)
                        {
                            // 透明底漆 显示默认木质
                            listTmp2[0] = "MainTex0_0";
                            listTmp2[2] = "MatcapTex0_0";
                        }
                        else
                        {
                            // 非透明底漆 显示底漆颜色
                            int colorId_1 = _planeEquipmentColorIds[index][0] % 100 - 1;
                            string matcapTexName_1 = new StringBuilder("MatcapTex1_" + colorId_1).ToString();
                            listTmp2[0] = "MainTex1_0";
                            listTmp2[2] = matcapTexName_1;
                        }
                        // 装饰
                        if (_planeEquipmentColorIds[index][1] == 0)
                        {
                            // 透明装饰 显示默认木质
                            listTmp2[1] = "MainTex0_0";
                            listTmp2[3] = "MatcapTex0_1";
                        }
                        else
                        {
                            // 非透明装饰 显示装饰颜色
                            int colorId_2 = _planeEquipmentColorIds[index][1] % 100 - 1;
                            string matcapTexName_2 = new StringBuilder("MatcapTex1_" + colorId_2).ToString();
                            listTmp2[1] = "MainTex1_0";
                            listTmp2[3] = matcapTexName_2;
                        }
                        result.AddRange(listTmp2);
                    }
                    else
                    {
                        // 当前装备中的不是透明贴纸 显示底漆贴纸
                        List<string> listTmp2 = new List<string>(4) { "", "", "", "" };
                        // 贴纸
                        int colorId = _planeEquipmentColorIds[index][3] % 100 - 1;
                        string mainTexName = new StringBuilder("MainTex2_" + colorId).ToString();
                        listTmp2[0] = mainTexName;
                        // 底漆
                        if (_planeEquipmentColorIds[index][0] == 0)
                        {
                            // 透明底漆 显示默认木质
                            listTmp2[2] = "MatcapTex0_0";
                        }
                        else
                        {
                            // 非透明底漆 显示底漆颜色
                            int colorId_1 = _planeEquipmentColorIds[index][0] % 100 - 1;
                            string matcapTexName_1 = new StringBuilder("MatcapTex1_" + colorId_1).ToString();
                            listTmp2[2] = matcapTexName_1;
                        }
                        // 装饰
                        if (_planeEquipmentColorIds[index][1] == 0)
                        {
                            // 透明装饰 显示默认木质
                            listTmp2[1] = "MainTex0_0";
                            listTmp2[3] = "MatcapTex0_1";
                        }
                        else
                        {
                            // 非透明装饰 显示装饰颜色
                            int colorId_2 = _planeEquipmentColorIds[index][1] % 100 - 1;
                            string matcapTexName_2 = new StringBuilder("MatcapTex1_" + colorId_2).ToString();
                            listTmp2[1] = "MainTex1_0";
                            listTmp2[3] = matcapTexName_2;
                        }
                        result.AddRange(listTmp2);
                    }
                    break;
            }
            
            List<string> assests = new List<string>(4) { result[1], result[2], result[3], result[4] };

            // Debug.Log("涂装列表 = " + JsonConvert.SerializeObject(assests));
            
            _cancellationToken = new CancellationTokenSource();
            _ = LoadPlaneTexture(assests, equipment, ToolFunManager.HexToColor(result[0]));
        }
        
        /// <summary>
        /// 加载飞机部件涂装贴图
        /// </summary>
        /// <param name="assests">贴图资源名称</param>
        /// <param name="equipment">指定部件</param>
        /// <param name="lightColor">LED灯组</param>
        async UniTask LoadPlaneTexture(List<string> assests, GameObject equipment, Color lightColor)
        {
            int createOk = 0;
            Texture[] textures = new Texture[assests.Count];

            for (int i = 0; i < assests.Count; i++)
            {
                int iTmp = i;
                LoadResources.XXResourcesLoad(assests[iTmp], texture =>
                {
                    textures[iTmp] = texture;
                    createOk += 1;
                });
            }
            
            while (createOk < assests.Count)
            {
                await UniTask.Yield(_cancellationToken.Token);
            }

            foreach (var t in equipment.GetComponentsInChildren<MeshRenderer>(true))
            {
                // LED灯组
                t.material.SetColor(LightColor, lightColor);
                // 贴图
                t.material.SetTexture(MainTexA, textures[0]);
                t.material.SetTexture(MainTexB, textures[1]);
                t.material.SetTexture(MatcapTexA, textures[2]);
                t.material.SetTexture(MatcapTexB, textures[3]);
            }
        }
    }
}