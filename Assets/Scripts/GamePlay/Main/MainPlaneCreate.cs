using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Common.LoadRes;
using Common.Tool;
using Cysharp.Threading.Tasks;
using Data;
using UnityEngine;

namespace GamePlay.Main
{
    public class MainPlaneCreate : MonoBehaviour
    {
        /** 飞机挂载节点 */
        private Transform planeTram;

        /** 机身 */
        private GameObject planBody;
        /** 机翼(左) */
        private GameObject planeWingL;
        /** 机翼(右) */
        private GameObject planeWingR;
        /** 机头 */
        private GameObject planePropeller;
        /** 机尾 */
        private GameObject planeFin;
        /** 推进器(左) */
        private GameObject planeSpurtL;
        /** 推进器(右) */
        private GameObject planeSpurtR;

        /** 挂载节点位置 机翼(左) */
        private Transform Point_WingL;
        /** 挂载节点位置 机翼(右) */
        private Transform Point_WingR;
        /** 挂载节点位置 机头 */
        private Transform Point_Propeller;
        /** 挂载节点位置 机尾 */
        private Transform Point_Fin;
        /** 挂载节点位置 推进器(左) */
        private Transform Point_SpurtL;
        /** 挂载节点位置 推进器(右) */
        private Transform Point_SpurtR;

        /** 创建模型名称 */
        private readonly string[] _createModle = new string[7];

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
        private CancellationTokenSource _cancellationToken = new CancellationTokenSource();

        private void OnDisable()
        {
            _cancellationToken?.Cancel();
            _cancellationToken?.Dispose();
            _cancellationToken = null;
        }

        /// <summary>
        /// 创建整机
        /// </summary>
        /// <param name="planeTramTmp">挂载节点</param>
        internal void CreatePlane(Transform planeTramTmp)
        {
            planeTram = planeTramTmp;

            CreatePlaneBody(DataHelper.CurUserInfoData.equipEquipments[0], () =>
            {
                CreatePlaneWingL(DataHelper.CurUserInfoData.equipEquipments[2],
                    () => { CreatePlaneSpurtL(DataHelper.CurUserInfoData.equipEquipments[5], () => { }); });
                CreatePlaneWingR(DataHelper.CurUserInfoData.equipEquipments[3],
                    () => { CreatePlaneSpurtR(DataHelper.CurUserInfoData.equipEquipments[5], () => { }); });
                CreatePlanePropeller(DataHelper.CurUserInfoData.equipEquipments[1]);
                CreatePlaneFin(DataHelper.CurUserInfoData.equipEquipments[4]);
            });
        }
        
        /// <summary>
        /// 创建机身
        /// </summary>
        internal void CreatePlaneBody(int id, Action cb)
        {
            if (planBody) Destroy(planBody);
            int planeBodyId = id;
            _createModle[0] = new StringBuilder("PlaneBody" + planeBodyId).ToString();
            LoadResources.XXResourcesLoad(_createModle[0], handleTmp =>
            {
                string handleTmpName = handleTmp.name.Replace("(Clone)", "");
                if (_createModle[0] != handleTmpName) return;

                GameObject objTmp = Instantiate(handleTmp, planeTram);
                objTmp.transform.position = planeTram.position;
                planBody = objTmp;
                SetPlaneMatCap(planBody);
                // 寻找点位
                Point_WingL = objTmp.transform.Find("Point_WingL");
                Point_WingR = objTmp.transform.Find("Point_WingR");
                Point_Propeller = objTmp.transform.Find("Point_Propeller");
                Point_Fin = objTmp.transform.Find("Point_Fin");
                // 刷新部件依赖点位
                if (planeWingL)
                {
                    planeWingL.transform.position = Point_WingL.position;
                    Point_SpurtL = planeWingL.transform.Find("Point_Spurt");
                    if (planeSpurtL) planeSpurtL.transform.position = Point_SpurtL.position;
                }

                if (planeWingR)
                {
                    planeWingR.transform.position = Point_WingR.position;
                    Point_SpurtR = planeWingR.transform.Find("Point_Spurt");
                    if (planeSpurtR) planeSpurtR.transform.position = Point_SpurtR.position;
                }

                if (planePropeller) planePropeller.transform.position = Point_Propeller.position;
                if (planeFin) planeFin.transform.position = Point_Fin.position;
                cb();
            });
        }
        
        /// <summary>
        /// 创建机翼（左）
        /// </summary>
        internal void CreatePlaneWingL(int id, Action cb)
        {
            if (planeWingL) Destroy(planeWingL);
            int planeWingLId = id == -1 ? -1 : id;
            if (planeWingLId > -1)
            {
                _createModle[2] = new StringBuilder("PlaneWingL" + planeWingLId).ToString();
                LoadResources.XXResourcesLoad(_createModle[2], handleTmp =>
                {
                    string handleTmpName = handleTmp.name.Replace("(Clone)", "");
                    if (_createModle[2] != handleTmpName) return;
                    
                    GameObject objTmp = Instantiate(handleTmp, planeTram);
                    objTmp.transform.position = Point_WingL.position;
                    planeWingL = objTmp;
                    SetPlaneMatCap(planeWingL);
                    // 寻找点位
                    Point_SpurtL = objTmp.transform.Find("Point_Spurt");
                    // 刷新部件依赖点位
                    if (planeSpurtL) planeSpurtL.transform.position = Point_SpurtL.position;
                    LoadDoneNum();
                    cb();
                });
            }
            else
                LoadDoneNum();
        }
        
        /// <summary>
        /// 创建机翼（右）
        /// </summary>
        internal void CreatePlaneWingR(int id, Action cb)
        {
            if (planeWingR) Destroy(planeWingR);
            int planeWingRId = id == -1 ? -1 : id;
            if (planeWingRId > -1)
            {
                _createModle[3] = new StringBuilder("PlaneWingR" + planeWingRId).ToString();
                LoadResources.XXResourcesLoad(_createModle[3], handleTmp =>
                {
                    string handleTmpName = handleTmp.name.Replace("(Clone)", "");
                    if (_createModle[3] != handleTmpName) return;
                    
                    GameObject objTmp = Instantiate(handleTmp, planeTram);
                    objTmp.transform.position = Point_WingR.position;
                    planeWingR = objTmp;
                    SetPlaneMatCap(planeWingR);
                    // 寻找点位
                    Point_SpurtR = objTmp.transform.Find("Point_Spurt");
                    // 刷新部件依赖点位
                    if (planeSpurtR) planeSpurtR.transform.position = Point_SpurtR.position;
                    LoadDoneNum();
                    cb();
                });
            }
            else
                LoadDoneNum();
        }
        
        /// <summary>
        /// 创建推进器（左）
        /// </summary>
        internal void CreatePlaneSpurtL(int id, Action cb)
        {
            if (planeSpurtL) Destroy(planeSpurtL);
            int planeSpurtId = id == -1 ? -1 : id;
            if (planeSpurtId > -1)
            {
                _createModle[5] = new StringBuilder("PlaneSpurtL" + planeSpurtId).ToString();
                LoadResources.XXResourcesLoad(_createModle[5], handleTmp =>
                {
                    string handleTmpName = handleTmp.name.Replace("(Clone)", "");
                    if (_createModle[5] != handleTmpName) return;

                    GameObject objTmp = Instantiate(handleTmp, planeTram);
                    objTmp.transform.position = Point_SpurtL.position;
                    planeSpurtL = objTmp;
                    SetPlaneMatCap(planeSpurtL);
                    LoadDoneNum();
                    cb();
                });
            }
            else
                LoadDoneNum();
        }

        /// <summary>
        /// 创建推进器（右）
        /// </summary>
        internal void CreatePlaneSpurtR(int id, Action cb)
        {
            if (planeSpurtR) Destroy(planeSpurtR);
            int planeSpurtId = id == -1 ? -1 : id;
            if (planeSpurtId > -1)
            {
                _createModle[6] = new StringBuilder("PlaneSpurtR" + planeSpurtId).ToString();
                LoadResources.XXResourcesLoad(_createModle[6], handleTmp =>
                {
                    string handleTmpName = handleTmp.name.Replace("(Clone)", "");
                    if (_createModle[6] != handleTmpName) return;

                    GameObject objTmp = Instantiate(handleTmp, planeTram);
                    objTmp.transform.position = Point_SpurtR.position;
                    planeSpurtR = objTmp;
                    SetPlaneMatCap(planeSpurtR);
                    LoadDoneNum();
                    cb();
                });
            }
            else
                LoadDoneNum();
        }
        
        /// <summary>
        /// 创建机头
        /// </summary>
        internal void CreatePlanePropeller(int id)
        {
            if (planePropeller) Destroy(planePropeller);
            int planePropellerId = id == -1 ? -1 : id;
            if (planePropellerId > -1)
            {
                _createModle[1] = new StringBuilder("PlanePropeller" + planePropellerId).ToString();
                LoadResources.XXResourcesLoad(_createModle[1], handleTmp =>
                {
                    string handleTmpName = handleTmp.name.Replace("(Clone)", "");
                    if (_createModle[1] != handleTmpName) return;

                    GameObject objTmp = Instantiate(handleTmp, planeTram);
                    objTmp.transform.position = Point_Propeller.position;
                    planePropeller = objTmp;
                    SetPlaneMatCap(planePropeller);
                    LoadDoneNum();
                });
            }
            else
                LoadDoneNum();
        }
        
        /// <summary>
        /// 创建机尾
        /// </summary>
        internal void CreatePlaneFin(int id)
        {
            if (planeFin) Destroy(planeFin);
            int planeFinId = id == -1 ? -1 : id;
            if (planeFinId > -1)
            {
                _createModle[4] = new StringBuilder("PlaneFin" + planeFinId).ToString();
                LoadResources.XXResourcesLoad(_createModle[4], handleTmp =>
                {
                    string handleTmpName = handleTmp.name.Replace("(Clone)", "");
                    if (_createModle[4] != handleTmpName) return;

                    GameObject objTmp = Instantiate(handleTmp, planeTram);
                    objTmp.transform.position = Point_Fin.position;
                    planeFin = objTmp;
                    SetPlaneMatCap(planeFin);
                    LoadDoneNum();
                });
            }
            else
                LoadDoneNum();
        }
        
        void LoadDoneNum()
        {
        }
        
        /// <summary>
        /// 设置指定部件的涂装
        /// <param name="equipment">指定部件</param>
        /// </summary>
        private void SetPlaneMatCap(GameObject equipment)
        {
            string nameTmp = equipment.name.Replace("(Clone)", "");
            int idTmp = int.Parse(nameTmp.Substring(nameTmp.Length - 4));
            // Debug.Log(idTmp);

            List<string> result = DataHelper.GetPlaneEquipmentColor(idTmp);
            List<string> assests = new List<string>(4) { result[1], result[2], result[3], result[4] };

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

        /// <summary>
        /// 显示/隐藏推进器
        /// <param name="isRight">右/左</param>
        /// <param name="isHide">隐藏/显示</param>
        /// </summary>
        internal void HidePlaneSpurt(bool isRight, bool isHide)
        {
            if (isRight)
            {
                if (planeSpurtR)
                {
                    if (Point_SpurtR) planeSpurtR.transform.position = Point_SpurtR.position;
                    planeSpurtR.SetActive(!isHide);
                }
            }
            else
            {
                if (planeSpurtL)
                {
                    if (Point_SpurtL) planeSpurtL.transform.position = Point_SpurtL.position;
                    planeSpurtL.SetActive(!isHide);
                }
            }
        }
        
        /// <summary>
        /// 刷新飞机涂装
        /// </summary>
        internal void RefreshPlaneColor()
        {
            if (planBody) SetPlaneMatCap(planBody);
            if (planeWingL) SetPlaneMatCap(planeWingL);
            if (planeWingR) SetPlaneMatCap(planeWingR);
            if (planePropeller) SetPlaneMatCap(planePropeller);
            if (planeFin) SetPlaneMatCap(planeFin);
            if (planeSpurtL) SetPlaneMatCap(planeSpurtL);
            if (planeSpurtR) SetPlaneMatCap(planeSpurtR);
        }
    }
}