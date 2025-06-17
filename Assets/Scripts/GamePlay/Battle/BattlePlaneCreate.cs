using System.Collections.Generic;
using System.Text;
using Common.Event;
using Common.Event.CustomEnum;
using Common.LoadRes;
using Common.Tool;
using Data;
using UnityEngine;

namespace GamePlay.Battle
{
    public class BattlePlaneCreate : MonoBehaviour
    {
        private int loadDoneNum;
        
        private Transform planeTram;

        private          Transform        launcherPoint;
        private readonly List<GameObject> effectThrusterSpurts = new List<GameObject>();
        private readonly List<GameObject> effectThrusterItems = new List<GameObject>();
        private readonly List<GameObject> effectTrails         = new List<GameObject>();
        
        private readonly List<GameObject> _planeSubs = new List<GameObject>();

        internal void CreatePlane(Transform planeTramTmp)
        {
            planeTram = planeTramTmp;

            CreatePlaneBody();
        }

        void CreatePlaneBody()
        {
            List<string> colorInfoTmp =DataHelper.GetPlaneEquipmentColor(DataHelper.CurUserInfoData.equipEquipments[0]);
            
            LoadResources.XXResourcesLoad(new StringBuilder("PlaneBody" + DataHelper.CurUserInfoData.equipEquipments[0]).ToString(),
                handleTmp =>
                {
                    GameObject objTmp = Instantiate(handleTmp, planeTram.transform.position,Quaternion.identity,planeTram);
                    SetPlaneSubs(objTmp, colorInfoTmp);
                    
                    launcherPoint = objTmp.transform.Find("LauncherPoint");
                    planeTram.transform.position = new Vector3(0, 0.5f, -launcherPoint.position.z);

                    Transform bodyCenter = objTmp.transform.Find("BodyCenter");
                    _ = BattleManager._instance.bodyCenter = bodyCenter;
                    _ = BattleManager._instance._camControl.ResetCam(bodyCenter);
                        
                    CreatePlaneWingL(objTmp.transform.Find("Point_WingL").position);
                    CreatePlaneWingR(objTmp.transform.Find("Point_WingR").position);
                    CreatePlanePropeller(objTmp.transform.Find("Point_Propeller").position);
                    CreatePlaneFin(objTmp.transform.Find("Point_Fin").position);

                });
        }

        private void CreatePlaneWingL(Vector3 pointTmp)
        {
            if (DataHelper.CurUserInfoData.equipEquipments[2] > -1)
            {
                List<string> colorInfoTmp =DataHelper.GetPlaneEquipmentColor(DataHelper.CurUserInfoData.equipEquipments[2]);
                
                LoadResources.XXResourcesLoad(new StringBuilder("PlaneWingL" + DataHelper.CurUserInfoData.equipEquipments[2]).ToString(),
                    handleTmp =>
                    {
                        GameObject objTmp = Instantiate(handleTmp, pointTmp, Quaternion.identity, planeTram);
                        SetPlaneSubs(objTmp, colorInfoTmp);
                        LoadDoneNum(1);

                        CreatePlaneSpurtL(objTmp.transform.Find("Point_Spurt").position);

                        CreateEffectTrail(objTmp.transform.Find("Point_Trail"));
                    });
            }
            else
                LoadDoneNum(4);
        }

        private void CreatePlaneWingR(Vector3 pointTmp)
        {
            if (DataHelper.CurUserInfoData.equipEquipments[3] > -1)
            {
                List<string> colorInfoTmp =DataHelper.GetPlaneEquipmentColor(DataHelper.CurUserInfoData.equipEquipments[3]);
                
                LoadResources.XXResourcesLoad(new StringBuilder("PlaneWingR" + DataHelper.CurUserInfoData.equipEquipments[3]).ToString(),
                    handleTmp =>
                    {
                        GameObject objTmp = Instantiate(handleTmp, pointTmp, Quaternion.identity, planeTram);
                        SetPlaneSubs(objTmp, colorInfoTmp);
                        LoadDoneNum(1);

                        CreatePlaneSpurtR(objTmp.transform.Find("Point_Spurt").position);

                        CreateEffectTrail(objTmp.transform.Find("Point_Trail"));
                    });
            }
            else
                LoadDoneNum(4);
        }

        void CreatePlaneSpurtL(Vector3 pointTmp)
        {
            if (DataHelper.CurUserInfoData.equipEquipments[5] > -1)
            {
                List<string> colorInfoTmp = DataHelper.GetPlaneEquipmentColor(DataHelper.CurUserInfoData.equipEquipments[5]);

                LoadResources.XXResourcesLoad(new StringBuilder("PlaneSpurtL" + DataHelper.CurUserInfoData.equipEquipments[5]).ToString(),
                    handleTmp =>
                    {
                        GameObject objTmp = Instantiate<GameObject>(handleTmp, pointTmp, Quaternion.identity, planeTram);
                        SetPlaneSubs(objTmp, colorInfoTmp);
                        LoadDoneNum(1);

                        CreateEffectThruster(objTmp.transform.Find("Effect"));
                    });
            }
            else
                LoadDoneNum(2);
        }

        void CreatePlaneSpurtR(Vector3 pointTmp)
        {
            if (DataHelper.CurUserInfoData.equipEquipments[5] > -1)
            {
                List<string> colorInfoTmp = DataHelper.GetPlaneEquipmentColor(DataHelper.CurUserInfoData.equipEquipments[5]);

                LoadResources.XXResourcesLoad(new StringBuilder("PlaneSpurtR" + DataHelper.CurUserInfoData.equipEquipments[5]).ToString(),
                    handleTmp =>
                    {
                        GameObject objTmp = Instantiate<GameObject>(handleTmp, pointTmp, Quaternion.identity, planeTram);
                        SetPlaneSubs(objTmp, colorInfoTmp);
                        LoadDoneNum(1);

                        CreateEffectThruster(objTmp.transform.Find("Effect"));
                    });
            }
            else
                LoadDoneNum(2);
        }
        void CreatePlanePropeller(Vector3 pointTmp)
        {
            if (DataHelper.CurUserInfoData.equipEquipments[1] > -1)
            {
                List<string> colorInfoTmp = DataHelper.GetPlaneEquipmentColor(DataHelper.CurUserInfoData.equipEquipments[1]);

                LoadResources.XXResourcesLoad(new StringBuilder("PlanePropeller" + DataHelper.CurUserInfoData.equipEquipments[1]).ToString(),
                    handleTmp =>
                    {
                        GameObject objTmp = Instantiate<GameObject>(handleTmp, pointTmp, Quaternion.identity, planeTram);
                        SetPlaneSubs(objTmp, colorInfoTmp);
                        LoadDoneNum(1);
                    });
            }
            else
                LoadDoneNum(1);
        }
        void CreatePlaneFin(Vector3 pointTmp)
        {
            if (DataHelper.CurUserInfoData.equipEquipments[4] > -1)
            {
                List<string> colorInfoTmp =DataHelper.GetPlaneEquipmentColor(DataHelper.CurUserInfoData.equipEquipments[4]);
                
                LoadResources.XXResourcesLoad(new StringBuilder("PlaneFin" + DataHelper.CurUserInfoData.equipEquipments[4]).ToString(),
                    handleTmp =>
                    {
                        GameObject objTmp = Instantiate<GameObject>(handleTmp, pointTmp, Quaternion.identity, planeTram);
                        SetPlaneSubs(objTmp, colorInfoTmp);
                        LoadDoneNum(1);
                    });
            }
            else
                LoadDoneNum(1);
        }


        void CreateEffectTrail(Transform parentTmp)
        {
            LoadResources.XXResourcesLoad("Effect_Trail",
                handleTmp =>
                {
                    GameObject objTmp = Instantiate(handleTmp, parentTmp.position, Quaternion.identity, parentTmp);
                    objTmp.SetActive(false);
                    effectTrails.Add(objTmp);
                    
                    LoadDoneNum(1);
                });
        }

        void CreateEffectThruster(Transform parentTmp)
        {
            LoadResources.XXResourcesLoad("Effect_Thruster",
                handleTmp =>
                {
                    Transform objTmp = Instantiate(handleTmp, parentTmp.position, Quaternion.identity, parentTmp).transform;
                    GameObject objSpurtsTmp = objTmp.Find("ThrusterSpurts").gameObject;
                    GameObject objItemTmp = objTmp.Find("ThrusterItems").gameObject;
                    objSpurtsTmp.SetActive(false);
                    objItemTmp.SetActive(false);
                    effectThrusterSpurts.Add(objSpurtsTmp);
                    effectThrusterItems.Add(objItemTmp);
                    
                    LoadDoneNum(1);
                });
        }

        void LoadDoneNum(int doneNum)
        {
            for (int i = 0; i < doneNum; i++)
            {
                EventManager.Send(CustomEventType.ResLoadDone);
            }

            loadDoneNum += doneNum;
            if (loadDoneNum != 10) return;

            BattleManager._instance.ModleDone(planeTram.gameObject, launcherPoint, effectThrusterSpurts, effectThrusterItems, effectTrails,_planeSubs);
            Destroy(this);
        }

        
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
        void SetPlaneSubs(GameObject objTmp, List<string> colorInfoTmp)
        {
            foreach (var t in objTmp.GetComponentsInChildren<MeshRenderer>(true))
            {
                _planeSubs.Add(t.gameObject);
                
                // LED灯组
                t.material.SetColor(LightColor, ToolFunManager.HexToColor(colorInfoTmp[0]));
                // 贴图
                LoadResources.XXResourcesLoad(colorInfoTmp[1], texture =>
                {
                    t.material.SetTexture(MainTexA, texture);
                });
                LoadResources.XXResourcesLoad(colorInfoTmp[2], texture =>
                {
                    t.material.SetTexture(MainTexB, texture);
                });
                LoadResources.XXResourcesLoad(colorInfoTmp[3], texture =>
                {
                    t.material.SetTexture(MatcapTexA, texture);
                });
                LoadResources.XXResourcesLoad(colorInfoTmp[4], texture =>
                {
                    t.material.SetTexture(MatcapTexB, texture);
                });
            }
        }

    }
}
