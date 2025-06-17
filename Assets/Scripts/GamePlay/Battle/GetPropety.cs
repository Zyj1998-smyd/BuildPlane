using Data;
using UnityEngine;

namespace GamePlay.Battle
{
    public class GetPropety : MonoBehaviour
    {
        internal static float GetSuDu(float suDuTmp)
        {
            float returnNum = 0;
            switch (DataHelper.CurLevelNum)
            {
                case 1: //SuDuTmp:100-250
                    returnNum = 10f + 20f * (1f - Mathf.Exp(-((suDuTmp - 100f) / 100f * (1f + (suDuTmp - 100f) / 200f))));
                    break;
                case 2: //SuDuTmp:100-500
                    returnNum = (suDuTmp <= 250f)
                        ? 10f + 5f  * Mathf.Pow((suDuTmp - 100f) / 150f, 2.25f)
                        : 15f + 15f * (1f - Mathf.Exp(-((suDuTmp - 250f) / 200f * (1f + (suDuTmp - 250f) / 400f))));
                    break;
                case 3: //SuDuTmp:100-750
                    returnNum = (suDuTmp <= 500f)
                        ? 10f + 5f * (Mathf.Exp(0.02f * (suDuTmp - 100f)) - 1f) / 3000f
                        : 15f + 15f                                             * (1f - Mathf.Exp(-((suDuTmp - 500f) / 200f * (1f + (suDuTmp - 500f) / 400f))));
                    break;
                case 4: //SuDuTmp:100-1000
                    returnNum = (suDuTmp <= 750f)
                        ? 10f + 5f  * Mathf.Pow((suDuTmp - 100f) / 650f, 10f)
                        : 15f + 15f * (1f - Mathf.Exp(-(suDuTmp - 750f) / 200f * (1f + (suDuTmp - 750f) / 400f)));
                    break;
                case 5: //SuDuTmp:100-1500
                    returnNum = (suDuTmp <= 1000f)
                        ? 10f + 5f  * Mathf.Pow((suDuTmp - 100f) / 900f, 13f)
                        : 15f + 15f * (1f - Mathf.Exp(-(suDuTmp - 1000f) / 300f * (1f + (suDuTmp - 1000f) / 600f)));
                    break;
                case 6: //SuDuTmp:100-2500
                    returnNum = (suDuTmp <= 1500f)
                        ? 10f + 5f  * Mathf.Pow((suDuTmp - 100f) / 1400f, 10f)
                        : 15f + 15f * (1f - Mathf.Exp(-((suDuTmp - 1500f) / 500f * (1f + (suDuTmp - 1500f) / 1000f))));
                    break;
                case 7: //SuDuTmp:100-4000
                    returnNum = (suDuTmp <= 2500f)
                        ? 10f + 5f  * Mathf.Pow((suDuTmp - 100f) / 2400f, 10f)
                        : 15f + 15f * (1f - Mathf.Exp(-((suDuTmp - 2500f) / 750f * (1f + (suDuTmp - 2500f) / 1500f))));
                    break;
                case 8: //SuDuTmp:100-6000
                    returnNum = (suDuTmp <= 4000f)
                        ? 10f + 5f  * Mathf.Pow((suDuTmp - 100f) / 3900f, 15f)
                        : 15f + 15f * (1f - Mathf.Exp(-((suDuTmp - 4000f) / 1000f * (1f + (suDuTmp - 4000f) / 2000f))));
                    break;
                case 9: //SuDuTmp:100-8000
                    returnNum = (suDuTmp <= 6000f)
                        ? 10f + 5f  * Mathf.Pow((suDuTmp - 100f) / 5900f, 15f)
                        : 15f + 15f * (1f - Mathf.Exp(-((suDuTmp - 6000f) / 1200f * (1f + (suDuTmp - 6000f) / 2400f))));
                    break;
                case 10: //SuDuTmp:100-10000
                    returnNum = (suDuTmp <= 8000f)
                        ? 10f + 5f  * Mathf.Pow((suDuTmp - 100f) / 7900f, 20f)
                        : 15f + 15f * (1f - Mathf.Exp(-((suDuTmp - 8000f) / 1500f * (1f + (suDuTmp - 8000f) / 3000f))));
                    break;
            }

            return Mathf.Clamp(returnNum * 1.3f, 10f, 50f);
        }

        internal static float GetKangZu(float kangZuTmp)
        {
            float returnNum = 0;
            switch (DataHelper.CurLevelNum)
            {
                case 1: //KangZuTmp:100-250
                    returnNum = 10f + 190f * (1f - Mathf.Exp(-((kangZuTmp - 100f) / 100f * (1f + (kangZuTmp - 100f) / 200f))));
                    break;
                case 2: //KangZuTmp:100-500
                    returnNum = (kangZuTmp <= 250f)
                        ? 10f + 40f  * Mathf.Pow((kangZuTmp - 100f) / 150f, 2.8f)
                        : 50f + 150f * (1f - Mathf.Exp(-(kangZuTmp - 250f) / 200f * (1f + (kangZuTmp - 250f) / 400f)));
                    break;
                case 3: //KangZuTmp:100-750
                    returnNum = (kangZuTmp <= 500f)
                        ? 10f + 40f  * Mathf.Pow((kangZuTmp - 100f) / 400f, 8f)
                        : 50f + 150f * (1f - Mathf.Exp(-(kangZuTmp - 500f) / 200f * (1f + (kangZuTmp - 500f) / 400f)));
                    break;
                case 4: //KangZuTmp:100-1000
                    returnNum = (kangZuTmp <= 750f)
                        ? 10f + 40f  * Mathf.Pow((kangZuTmp - 100f) / 650f, 13f)
                        : 50f + 150f * (1f - Mathf.Exp(-(kangZuTmp - 750f) / 200f * (1f + (kangZuTmp - 750f) / 400f)));
                    break;
                case 5: //KangZuTmp:100-1500
                    returnNum = (kangZuTmp <= 1000f)
                        ? 10f + 40f  * Mathf.Pow((kangZuTmp - 100f) / 900f, 18f)
                        : 50f + 150f * (1f - Mathf.Exp(-(kangZuTmp - 1000f) / 300f * (1f + (kangZuTmp - 1000f) / 600f)));
                    break;
                case 6: //KangZuTmp:100-2500
                    returnNum = (kangZuTmp <= 1500f)
                        ? 10f + 40f  * Mathf.Pow((kangZuTmp - 100f) / 1400f, 10f)
                        : 50f + 150f * (1f - Mathf.Exp(-(kangZuTmp - 1500f) / 500f * (1f + (kangZuTmp - 1500f) / 1000f)));
                    break;
                case 7: //KangZuTmp:100-4500
                    returnNum = (kangZuTmp <= 2500f)
                        ? 10f + 40f  * Mathf.Pow((kangZuTmp - 100f) / 2400f, 12f)
                        : 50f + 150f * (1f - Mathf.Exp(-(kangZuTmp - 2500f) / 700f * (1f + (kangZuTmp - 2500f) / 1400f)));
                    break;
                case 8: //KangZuTmp:100-6500
                    returnNum = (kangZuTmp <= 4000f)
                        ? 10f + 40f  * Mathf.Pow((kangZuTmp - 100f) / 3900f, 14f)
                        : 50f + 150f * (1f - Mathf.Exp(-(kangZuTmp - 4000f) / 900f * (1f + (kangZuTmp - 4000f) / 1800f)));
                    break;
                case 9: //KangZuTmp:100-8000
                    returnNum = (kangZuTmp <= 6000f)
                        ? 10f + 40f  * Mathf.Pow((kangZuTmp - 100f) / 5900f, 16f)
                        : 50f + 150f * (1f - Mathf.Exp(-(kangZuTmp - 6000f) / 1000f * (1f + (kangZuTmp - 6000f) / 2000f)));
                    break;
                case 10: //KangZuTmp:100-10000
                    returnNum = (kangZuTmp <= 8000f)
                        ? 10f + 40f  * Mathf.Pow((kangZuTmp - 100f) / 7900f, 18f)
                        : 50f + 150f * (1f - Mathf.Exp(-(kangZuTmp - 8000f) / 1100f * (1f + (kangZuTmp - 8000f) / 2200f)));
                    break;
            }

            return Mathf.Clamp(returnNum, 10f, 200f);
        }

        internal static float GetFuKong(float fuKongTmp, float zhongLiangTmp)
        {
            float returnNum = 0;
            switch (DataHelper.CurLevelNum)
            {
                case 1: //FuKong:100-250,ZhongLiangTmp:400-200
                    returnNum = (10f + 190f * (1f - Mathf.Exp(-((fuKongTmp - 100f) / 100f * (1f + (fuKongTmp - 100f) / 200f))))) * GetZhongLiang(zhongLiangTmp);
                    break;
                case 2: //FuKong:100-500,ZhongLiangTmp:400-200
                    returnNum = (fuKongTmp <= 250f)
                        ? (10f + 40f  * Mathf.Pow((fuKongTmp - 100f) / 150f, 2.8f))                                      * GetZhongLiang(zhongLiangTmp)
                        : (50f + 150f * (1f - Mathf.Exp(-(fuKongTmp - 250f) / 200f * (1f + (fuKongTmp - 250f) / 400f)))) * GetZhongLiang(zhongLiangTmp);
                    break;
                case 3: //FuKong:100-750,ZhongLiangTmp:400-200
                    returnNum = (fuKongTmp <= 500f)
                        ? (10f + 40f  * Mathf.Pow((fuKongTmp - 100f) / 400f, 8f))                                        * GetZhongLiang(zhongLiangTmp)
                        : (50f + 150f * (1f - Mathf.Exp(-(fuKongTmp - 500f) / 200f * (1f + (fuKongTmp - 500f) / 400f)))) * GetZhongLiang(zhongLiangTmp);
                    break;
                case 4: //FuKong:100-1000,ZhongLiangTmp:400-200
                    returnNum = (fuKongTmp <= 750f)
                        ? (10f + 40f  * Mathf.Pow((fuKongTmp - 100f) / 650f, 13f))                                       * GetZhongLiang(zhongLiangTmp)
                        : (50f + 150f * (1f - Mathf.Exp(-(fuKongTmp - 750f) / 200f * (1f + (fuKongTmp - 750f) / 400f)))) * GetZhongLiang(zhongLiangTmp);
                    break;
                case 5: //FuKong:100-1500,ZhongLiangTmp:400-200
                    returnNum = (fuKongTmp <= 1000f)
                        ? (10f + 40f  * Mathf.Pow((fuKongTmp - 100f) / 900f, 18f))                                         * GetZhongLiang(zhongLiangTmp)
                        : (50f + 150f * (1f - Mathf.Exp(-(fuKongTmp - 1000f) / 300f * (1f + (fuKongTmp - 1000f) / 600f)))) * GetZhongLiang(zhongLiangTmp);
                    break;
                case 6: //FuKong:100-2500,ZhongLiangTmp:400-200
                    returnNum = (fuKongTmp <= 1500f)
                        ? (10f + 40f  * Mathf.Pow((fuKongTmp - 100f) / 1400f, 10f))                                         * GetZhongLiang(zhongLiangTmp)
                        : (50f + 150f * (1f - Mathf.Exp(-(fuKongTmp - 1500f) / 500f * (1f + (fuKongTmp - 1500f) / 1000f)))) * GetZhongLiang(zhongLiangTmp);
                    break;
                case 7: //FuKong:100-4000,ZhongLiangTmp:400-200
                    returnNum = (fuKongTmp <= 2500f)
                        ? (10f + 40f  * Mathf.Pow((fuKongTmp - 100f) / 2400f, 12f))                                         * GetZhongLiang(zhongLiangTmp)
                        : (50f + 150f * (1f - Mathf.Exp(-(fuKongTmp - 2500f) / 700f * (1f + (fuKongTmp - 2500f) / 1400f)))) * GetZhongLiang(zhongLiangTmp);
                    break;
                case 8: //FuKong:100-6000,ZhongLiangTmp:400-200
                    returnNum = (fuKongTmp <= 4000f)
                        ? (10f + 40f  * Mathf.Pow((fuKongTmp - 100f) / 3900f, 14f))                                         * GetZhongLiang(zhongLiangTmp)
                        : (50f + 150f * (1f - Mathf.Exp(-(fuKongTmp - 4000f) / 900f * (1f + (fuKongTmp - 4000f) / 1800f)))) * GetZhongLiang(zhongLiangTmp);
                    break;
                case 9: //FuKong:100-8000,ZhongLiangTmp:400-200
                    returnNum = (fuKongTmp <= 6000f)
                        ? (10f + 40f  * Mathf.Pow((fuKongTmp - 100f) / 5900f, 16f))                                          * GetZhongLiang(zhongLiangTmp)
                        : (50f + 150f * (1f - Mathf.Exp(-(fuKongTmp - 6000f) / 1000f * (1f + (fuKongTmp - 6000f) / 2000f)))) * GetZhongLiang(zhongLiangTmp);
                    break;
                case 10: //FuKong:100-10000,ZhongLiangTmp:400-200
                    returnNum = (fuKongTmp <= 8000f)
                        ? (10f + 40f  * Mathf.Pow((fuKongTmp - 100f) / 7900f, 18f))                                          * GetZhongLiang(zhongLiangTmp)
                        : (50f + 150f * (1f - Mathf.Exp(-(fuKongTmp - 8000f) / 1100f * (1f + (fuKongTmp - 8000f) / 2200f)))) * GetZhongLiang(zhongLiangTmp);
                    break;
            }

            return Mathf.Clamp(returnNum, 10f, 200f);
        }

        internal static float GetZhongLiang(float zhongLiangTmp)
        {
            float returnNum = 0.9f + ((400 - zhongLiangTmp) / 200) * 0.2f;
            return Mathf.Clamp(returnNum, 0.9f, 1.1f);
        }


        internal static float GetTuiJin(float tuiJinTmp)
        {
            float returnNum = 0;
            switch (DataHelper.CurLevelNum)
            {
                case 1: //TuiJinTmp:100-250
                    returnNum = 5f + 15f * (1f - Mathf.Exp(-((tuiJinTmp - 100f) / 100f * (1f + (tuiJinTmp - 100f) / 200f))));
                    break;
                case 2: //TuiJinTmp:100-500
                    returnNum = (tuiJinTmp <= 250f)
                        ? 5f  + 5f  * Mathf.Pow((tuiJinTmp - 100f) / 150f, 1.8f)
                        : 10f + 10f * (1f - Mathf.Exp(-(tuiJinTmp - 250f) / 200f * (1f + (tuiJinTmp - 250f) / 400f)));
                    break;
                case 3: //TuiJinTmp:100-750
                    returnNum = (tuiJinTmp <= 500f)
                        ? 5f  + 5f  * Mathf.Pow((tuiJinTmp - 100f) / 400f, 2.9f)
                        : 10f + 10f * (1f - Mathf.Exp(-(tuiJinTmp - 500f) / 200f * (1f + (tuiJinTmp - 500f) / 400f)));
                    break;
                case 4: //TuiJinTmp:100-1000
                    returnNum = (tuiJinTmp <= 750f)
                        ? 5f  + 5f  * Mathf.Pow((tuiJinTmp - 100f) / 650f, 4.5f)
                        : 10f + 10f * (1f - Mathf.Exp(-(tuiJinTmp - 750f) / 200f * (1f + (tuiJinTmp - 750f) / 400f)));
                    break;
                case 5: //TuiJinTmp:100-1500
                    returnNum = (tuiJinTmp <= 1000f)
                        ? 5f  + 5f  * Mathf.Pow((tuiJinTmp - 100f) / 900f, 5.8f)
                        : 10f + 10f * (1f - Mathf.Exp(-(tuiJinTmp - 1000f) / 300f * (1f + (tuiJinTmp - 1000f) / 600f)));
                    break;
                case 6: //TuiJinTmp:100-2500
                    returnNum = (tuiJinTmp <= 1500f)
                        ? 5f  + 5f  * Mathf.Pow((tuiJinTmp - 100f) / 1400f, 7f)
                        : 10f + 10f * (1f - Mathf.Exp(-((tuiJinTmp - 1500f) / 500f * (1f + (tuiJinTmp - 1500f) / 1000f))));
                    break;
                case 7: //TuiJinTmp:100-4000
                    returnNum = (tuiJinTmp <= 2500f)
                        ? 5f  + 5f  * Mathf.Pow((tuiJinTmp - 100f) / 2400f, 10f)
                        : 10f + 10f * (1f - Mathf.Exp(-((tuiJinTmp - 2500f) / 600f * (1f + (tuiJinTmp - 2500f) / 1200f))));
                    break;
                case 8: //TuiJinTmp:100-6000
                    returnNum = (tuiJinTmp <= 4000f)
                        ? 5f  + 5f  * Mathf.Pow((tuiJinTmp - 100f) / 3900f, 10f)
                        : 10f + 10f * (1f - Mathf.Exp(-((tuiJinTmp - 4000f) / 900f * (1f + (tuiJinTmp - 4000f) / 1800f))));
                    break;
                case 9: //TuiJinTmp:100-8000
                    returnNum = (tuiJinTmp <= 6000f)
                        ? 5f  + 5f  * Mathf.Pow((tuiJinTmp - 100f) / 5900f, 13f)
                        : 10f + 10f * (1f - Mathf.Exp(-((tuiJinTmp - 6000f) / 1000f * (1f + (tuiJinTmp - 6000f) / 2000f))));
                    break;
                case 10: //TuiJinTmp:100-10000
                    returnNum = (tuiJinTmp <= 8000f)
                        ? 5f  + 5f  * Mathf.Pow((tuiJinTmp - 100f) / 7900f, 15f)
                        : 10f + 10f * (1f - Mathf.Exp(-((tuiJinTmp - 8000f) / 1200f * (1f + (tuiJinTmp - 8000f) / 2400f))));
                    break;
            }

            return Mathf.Clamp(returnNum, 5f, 20f);
        }

        internal static float GetNengLiang(float nengLiangTmp)
        {
            float returnNum = 0;
            switch (DataHelper.CurLevelNum)
            {
                case 1: //nengLiangTmp:100-250
                    returnNum = 1f + 9f * (1f - Mathf.Exp(-((nengLiangTmp - 100f) / 100f * (1f + (nengLiangTmp - 100f) / 200f))));
                    break;
                case 2: //nengLiangTmp:100-500
                    returnNum = (nengLiangTmp <= 250f)
                        ? 1f + 2f * Mathf.Pow((nengLiangTmp - 100f) / 150f, 2.25f)
                        : 3f + 7f * (1f - Mathf.Exp(-(nengLiangTmp - 250f) / 200f * (1f + (nengLiangTmp - 250f) / 400f)));
                    break;
                case 3: //nengLiangTmp:100-750
                    returnNum = (nengLiangTmp <= 500f)
                        ? 1f + 2f * Mathf.Pow((nengLiangTmp - 100f) / 400f, 5.2f)
                        : 3f + 7f * (1f - Mathf.Exp(-(nengLiangTmp - 500f) / 200f * (1f + (nengLiangTmp - 500f) / 400f)));
                    break;
                case 4: //nengLiangTmp:100-1000
                    returnNum = (nengLiangTmp <= 750f)
                        ? 1f + 2f * Mathf.Pow((nengLiangTmp - 100f) / 650f, 8.5f)
                        : 3f + 7f * (1f - Mathf.Exp(-(nengLiangTmp - 750f) / 200f * (1f + (nengLiangTmp - 750f) / 400f)));
                    break;
                case 5: //nengLiangTmp:100-1500
                    returnNum = (nengLiangTmp <= 1000f)
                        ? 1f + 2f * Mathf.Pow((nengLiangTmp - 100f) / 900f, 11f)
                        : 3f + 7f * (1f - Mathf.Exp(-(nengLiangTmp - 1000f) / 300f * (1f + (nengLiangTmp - 1000f) / 600f)));
                    break;
                case 6: //nengLiangTmp:100-2500
                    returnNum = (nengLiangTmp <= 1500f)
                        ? 1f + 2f * Mathf.Pow((nengLiangTmp - 100f) / 1400f, 11f)
                        : 3f + 7f * (1f - Mathf.Exp(-((nengLiangTmp - 1500f) / 500f * (1f + (nengLiangTmp - 1500f) / 1000f))));
                    break;
                case 7: //nengLiangTmp:100-4000
                    returnNum = (nengLiangTmp <= 2500f)
                        ? 1f + 2f * Mathf.Pow((nengLiangTmp - 100f) / 2400f, 12f)
                        : 3f + 7f * (1f - Mathf.Exp(-((nengLiangTmp - 2500f) / 700f * (1f + (nengLiangTmp - 2500f) / 1400f))));
                    break;
                case 8: //nengLiangTmp:100-6000
                    returnNum = (nengLiangTmp <= 4000f)
                        ? 1f + 2f * Mathf.Pow((nengLiangTmp - 100f) / 3900f, 13f)
                        : 3f + 7f * (1f - Mathf.Exp(-((nengLiangTmp - 4000f) / 900f * (1f + (nengLiangTmp - 4000f) / 1800f))));
                    break;
                case 9: //nengLiangTmp:100-8000
                    returnNum = (nengLiangTmp <= 6000f)
                        ? 1f + 2f * Mathf.Pow((nengLiangTmp - 100f) / 5900f, 18f)
                        : 3f + 7f * (1f - Mathf.Exp(-((nengLiangTmp - 6000f) / 1000f * (1f + (nengLiangTmp - 6000f) / 2000f))));
                    break;
                case 10: //nengLiangTmp:100-10000
                    returnNum = (nengLiangTmp <= 8000f)
                        ? 1f + 2f * Mathf.Pow((nengLiangTmp - 100f) / 7900f, 20f)
                        : 3f + 7f * (1f - Mathf.Exp(-((nengLiangTmp - 8000f) / 1200f * (1f + (nengLiangTmp - 8000f) / 2400f))));
                    break;
            }

            return Mathf.Clamp(returnNum * 0.8f, 1f, 8f);
        }


        internal static float GetTanLi(float tanLiTmp)
        {
            float returnNum = 0;
            switch (DataHelper.CurLevelNum)
            {
                case 1: //TanLiTmp:100-200
                    returnNum = 10f + 20f * (1f - Mathf.Exp(-((tanLiTmp - 100f) /50f * (1f + (tanLiTmp - 100f) /100f))));
                    break;
                case 2: //TanLiTmp:100-300
                    returnNum = (tanLiTmp <= 200f)
                        ? 10f + 5f  * Mathf.Pow((tanLiTmp - 100f) / 100f, 5f)
                        : 15f + 15f * (1f - Mathf.Exp(-((tanLiTmp - 200f) / 50f * (1f + (tanLiTmp - 200f) / 100f))));
                    break;
                case 3: //TanLiTmp:100-450
                    returnNum = (tanLiTmp <= 300f)
                        ? 10f + 5f  * Mathf.Pow((tanLiTmp - 100f) / 200f, 8f)
                        : 15f + 15f * (1f - Mathf.Exp(-((tanLiTmp - 300f) / 50f * (1f + (tanLiTmp - 300f) / 100f))));
                    break;
                case 4: //TanLiTmp:100-500
                    returnNum = (tanLiTmp <= 400f)
                        ? 10f + 5f  * Mathf.Pow((tanLiTmp - 100f) / 300f, 15f)
                        : 15f + 15f * (1f - Mathf.Exp(-((tanLiTmp - 400f) / 50f * (1f + (tanLiTmp - 400f) / 100f))));
                    break;
                case 5: //TanLiTmp:100-600
                    returnNum = (tanLiTmp <= 500f)
                        ? 10f + 5f  * Mathf.Pow((tanLiTmp - 100f) / 400f, 17f)
                        : 15f + 15f * (1f - Mathf.Exp(-((tanLiTmp - 500f) / 50f * (1f + (tanLiTmp - 500f) / 100f))));
                    break;
                case 6: //TanLiTmp:100-700
                    returnNum = (tanLiTmp <= 600f)
                        ? 10f + 5f  * Mathf.Pow((tanLiTmp - 100f) / 500f, 22f)
                        : 15f + 15f * (1f - Mathf.Exp(-((tanLiTmp - 600f) / 50f * (1f + (tanLiTmp - 600f) / 100f))));
                    break;
                case 7: //TanLiTmp:100-800
                    returnNum = (tanLiTmp <= 700f)
                        ? 10f + 5f  * Mathf.Pow((tanLiTmp - 100f) / 600f, 25f)
                        : 15f + 15f * (1f - Mathf.Exp(-((tanLiTmp - 700f) / 50f * (1f + (tanLiTmp - 700f) / 100f))));
                    break;
                case 8: //TanLiTmp:100-900
                    returnNum = (tanLiTmp <= 800f)
                        ? 10f + 5f  * Mathf.Pow((tanLiTmp - 100f) / 700f, 30f)
                        : 15f + 15f * (1f - Mathf.Exp(-((tanLiTmp - 800f) / 50f * (1f + (tanLiTmp - 800f) / 100f))));
                    break;
                case 9: //TanLiTmp:100-1000
                    returnNum = (tanLiTmp <= 900f)
                        ? 10f + 5f  * Mathf.Pow((tanLiTmp - 100f) / 800f, 35f)
                        : 15f + 15f * (1f - Mathf.Exp(-((tanLiTmp - 900f) / 50f * (1f + (tanLiTmp - 900f) / 100f))));
                    break;
                case 10: //TanLiTmp:100-1100
                    returnNum = (tanLiTmp <= 1000f)
                        ? 10f + 5f  * Mathf.Pow((tanLiTmp - 100f) / 900f, 40f)
                        : 15f + 15f * (1f - Mathf.Exp(-((tanLiTmp - 1000f) / 50f * (1f + (tanLiTmp - 1000f) / 100f))));
                    break;
            }

            return Mathf.Clamp(returnNum, 10f, 30f);
        }


        internal static float GetTuiJinItem(float tuiJinItemTmp)
        {
            float returnNum = 0;
            switch (DataHelper.CurLevelNum)
            {
                case 1: //TuiJinItemTmp:100-200
                    returnNum = 5f + 15f * (1f - Mathf.Exp(-((tuiJinItemTmp - 100f) /50f   * (1f + (tuiJinItemTmp - 100f) /100f))));
                    break;
                case 2: //TuiJinItemTmp:100-300
                    returnNum = (tuiJinItemTmp <= 200f)
                        ? 5f  + 5f  * Mathf.Pow((tuiJinItemTmp - 100f) / 100f, 5f)
                        : 10f + 10f * (1f - Mathf.Exp(-((tuiJinItemTmp - 200f) / 50f * (1f + (tuiJinItemTmp - 200f) / 100f))));
                    break;
                case 3: //TuiJinItemTmp:100-400
                    returnNum = (tuiJinItemTmp <= 300f)
                        ? 5f  + 5f  * Mathf.Pow((tuiJinItemTmp - 100f) / 200f, 8f)
                        : 10f + 10f * (1f - Mathf.Exp(-((tuiJinItemTmp - 300f) / 50f * (1f + (tuiJinItemTmp - 300f) / 100f))));
                    break;
                case 4: //TuiJinItemTmp:100-500
                    returnNum = (tuiJinItemTmp <= 400f)
                        ? 5f  + 5f  * Mathf.Pow((tuiJinItemTmp - 100f) / 300f, 15f)
                        : 10f + 10f * (1f - Mathf.Exp(-((tuiJinItemTmp - 400f) / 50f * (1f + (tuiJinItemTmp - 400f) / 100f))));
                    break;
                case 5: //TuiJinItemTmp:100-600
                    returnNum = (tuiJinItemTmp <= 500f)
                        ? 5f  + 5f  * Mathf.Pow((tuiJinItemTmp - 100f) / 400f, 17f)
                        : 10f + 10f * (1f - Mathf.Exp(-((tuiJinItemTmp - 500f) / 50f * (1f + (tuiJinItemTmp - 500f) / 100f))));
                    break;
                case 6: //TuiJinItemTmp:100-700
                    returnNum = (tuiJinItemTmp <= 600f)
                        ? 5f  + 5f  * Mathf.Pow((tuiJinItemTmp - 100f) / 500f, 22f)
                        : 10f + 10f * (1f - Mathf.Exp(-((tuiJinItemTmp - 600f) / 50f * (1f + (tuiJinItemTmp - 600f) / 100f))));
                    break;
                case 7: //TuiJinItemTmp:100-800
                    returnNum = (tuiJinItemTmp <= 700f)
                        ? 5f  + 5f  * Mathf.Pow((tuiJinItemTmp - 100f) / 600f, 25f)
                        : 10f + 10f * (1f - Mathf.Exp(-((tuiJinItemTmp - 700f) / 50f * (1f + (tuiJinItemTmp - 700f) / 100f))));
                    break;
                case 8: //TuiJinItemTmp:100-900
                    returnNum = (tuiJinItemTmp <= 800f)
                        ? 5f  + 5f  * Mathf.Pow((tuiJinItemTmp - 100f) / 700f, 30f)
                        : 10f + 10f * (1f - Mathf.Exp(-((tuiJinItemTmp - 800f) / 50f * (1f + (tuiJinItemTmp - 800f) / 100f))));
                    break;
                case 9: //TuiJinItemTmp:100-1000
                    returnNum = (tuiJinItemTmp <= 900f)
                        ? 5f  + 5f  * Mathf.Pow((tuiJinItemTmp - 100f) / 800f, 35f)
                        : 10f + 10f * (1f - Mathf.Exp(-((tuiJinItemTmp - 900f) / 50f * (1f + (tuiJinItemTmp - 900f) / 100f))));
                    break;
                case 10: //TuiJinItemTmp:100-1100
                    returnNum = (tuiJinItemTmp <= 1000f)
                        ? 5f  + 5f  * Mathf.Pow((tuiJinItemTmp - 100f) / 900f, 40f)
                        : 10f + 10f * (1f - Mathf.Exp(-((tuiJinItemTmp - 1000f) / 50f * (1f + (tuiJinItemTmp - 1000f) / 100f))));
                    break;
            }

            return Mathf.Clamp(returnNum, 5f, 20f);
        }
    }
}