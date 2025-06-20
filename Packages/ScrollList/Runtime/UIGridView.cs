﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace Plugins.ScrollList
{
    // 轴方向和开始排布的角落，可以确定：
    // 1、滑动方向（Contetnt的起始中心点和锚点）（4种）
    // 2、元素排布轨迹（2*4种）
    public sealed class UIGridView : UIScrollRect
    {
        public enum Corner
        {
            UpperLeft = 0,      //左上
            UpperRight = 1,     //右上
            LowerLeft = 2,      //左下
            LowerRight = 3      //右下
        }

        public enum Constraint
        {
            Flexible = 0,           // 不限定行或列数（灵活自适应）
            FixedRowCount = 1,      // 限定行数（水平滑动时）
            FixedColumnCount = 2    // 限定列数（竖直滑动时）
        }

        public enum Alignment
        {
            LeftOrUpper = 0,
            CenterOrMiddle = 1,
            RightOrLower = 2,
        }

        private MovementAxis startAxis { get { return (MovementAxis)(1 - (int)m_MovementAxis); } }  //对m_MovementAxis取反

        [SerializeField] private Corner m_StartCorner = Corner.UpperLeft;
        public Corner startCorner { get { return m_StartCorner; } set { SetProperty(ref m_StartCorner, value); } }

        [SerializeField] private Constraint m_Constraint = Constraint.Flexible;
        public Constraint constraint { get { return m_Constraint; } set { SetProperty(ref m_Constraint, value); } }

        [SerializeField] private int m_ConstraintCount = 2;
        public int constraintCount { get { return m_ConstraintCount; } set { SetProperty(ref m_ConstraintCount, Mathf.Max(1, value)); } }

        [SerializeField] private Vector2 m_Spacing = Vector2.zero;
        public Vector2 spacing { get { return m_Spacing; } set { SetProperty(ref m_Spacing, value); } }
        
        [SerializeField] private Alignment m_ChildAlignment = Alignment.LeftOrUpper;
        public Alignment childAlignment { get { return m_ChildAlignment; } set { SetProperty(ref m_ChildAlignment, value); } }

        [SerializeField] private RectOffset m_Padding = new RectOffset();
        public RectOffset padding { get { return m_Padding; } set { SetProperty(ref m_Padding, value); } }

        private DrivenRectTransformTracker m_Tracker;
        
        private int m_CellsPerMainAxis;
        private int m_ActualCellCountX;
        private int m_ActualCellCountY;
        private Vector2 m_RequiredSpace;
        private Vector2 m_StartOffset;

        private Dictionary<int, RectTransform> cellRTDict;    //index-Cell字典    
        private Stack<RectTransform> unUseCellRTStack;        //空闲Cell堆栈
        private List<KeyValuePair<int, RectTransform>> cellRTListForSort;     //Cell列表用于辅助Sbling排序

        private List<int> oldIndexes;             //旧的索引集合
        private List<int> newIndexes;             //新的索引集合
        private List<int> appearIndexes;          //将要出现的索引集合   //使用List而非单个，可以支持Content位置跳变
        private List<int> disAppearIndexes;       //将要消失的索引集合   //使用List而非单个，可以支持Content位置跳变
        
        private RectTransform m_TemplateCellRT;     //Cell模板
        private int m_CellCount;                    //实例化数量

        private Action<int> m_OnCellCreated;        //创建时回调
        private Action<int> m_OnCellAppear;         //出现时回调

        public void StartShow(RectTransform cellRT, int count, Action<int> onCellCreated = null, Action<int> onCellAppeared = null)
        {
            ReSetData();
            
            this.m_TemplateCellRT = cellRT;
            this.m_CellCount = count;
            this.m_OnCellCreated = onCellCreated;
            this.m_OnCellAppear = onCellAppeared;

            ResetContent();
            ResetTracker();

            CalcCellCountOnNaturalAxis();
            CalculateRequiredSpace();
            SetContentSizeOnMovementAxis();
            CalculateStartOffset();

            CalcIndexes();
            DisAppearCells();
            AppearCells();
            CalcAndSetCellsSblingIndex();
        }

        private void ReSetData()
        {
            for (var i = 0; i < m_Content.childCount; i++)
            {
                Destroy(m_Content.GetChild(i).gameObject);
            }
            
            cellRTDict = new Dictionary<int, RectTransform>();
            unUseCellRTStack = new Stack<RectTransform>();
            cellRTListForSort = new List<KeyValuePair<int, RectTransform>>();

            oldIndexes = new List<int>();
            newIndexes = new List<int>();
            appearIndexes = new List<int>();
            disAppearIndexes = new List<int>();
        }

        public void CustomRefresh(Vector2 delta)
        {
            OnScrollValueChanged(delta);
        }

        //获取该索引对应的CellRT
        public RectTransform GetCellRT(int index)
        {
            Debug.Assert(cellRTDict.ContainsKey(index));
            return cellRTDict[index];
        }

        //尝试获取该索引对应的CellRT，若未在显示则返回 null
        public RectTransform TryGetCellRT(int index)
        {
            return cellRTDict.ContainsKey(index) ? cellRTDict[index] : null;
        }

        //该索引当前是否正在显示
        public bool IsThisIndexShowing(int index)
        {
            return cellRTDict.ContainsKey(index);
        }

        protected override void Awake()
        {
            cellRTDict = new Dictionary<int, RectTransform>();
            unUseCellRTStack = new Stack<RectTransform>();
            cellRTListForSort = new List<KeyValuePair<int, RectTransform>>();

            oldIndexes = new List<int>();
            newIndexes = new List<int>();
            appearIndexes = new List<int>();
            disAppearIndexes = new List<int>();

            this.onValueChanged.AddListener(OnScrollValueChanged);
        }

        protected override void OnDisable()
        {
            m_Tracker.Clear();
            base.OnDisable();
        }

        private void OnScrollValueChanged(Vector2 delta)
        {
            if (m_CellCount <= 0) { return; }

            CalcIndexes();
            DisAppearCells();
            AppearCells();
            CalcAndSetCellsSblingIndex();
        }

        //轴向修改后需要重置
        private void ResetContent()
        {
            // 根据轴向和起始角落设置锚点、中心点
            if (m_MovementAxis == MovementAxis.Horizontal)
            {
                int cornerX = (int)m_StartCorner % 2;  //0：左， 1右
                m_Content.anchorMin = new Vector2(cornerX, 0);
                m_Content.anchorMax = new Vector2(cornerX, 1);
                m_Content.pivot = new Vector2(cornerX, 0.5f);
            }
            else
            {
                int cornerY = (int)m_StartCorner / 2;  //0：上， 1下
                m_Content.anchorMin = new Vector2(0, 1 - cornerY);
                m_Content.anchorMax = new Vector2(1, 1 - cornerY);
                m_Content.pivot = new Vector2(0.5f, 1 - cornerY);
            }

            // 位置归0
            m_Content.anchoredPosition = Vector2.zero;

            // 重置为Viewport的大小
            m_Content.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, m_Viewport.rect.width);
            m_Content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, m_Viewport.rect.height);
        }

        private void ResetTracker()
        {
            m_Tracker.Clear();
        }

        //计算直观行列数（自然坐标轴上）
        public void CalcCellCountOnNaturalAxis()
        {
            int cellCountX = 1;  //默认最小1
            int cellCountY = 1;  //默认最小1

            if (startAxis == MovementAxis.Horizontal)
            {
                Debug.Assert(m_Constraint == Constraint.FixedColumnCount || m_Constraint == Constraint.Flexible); //由编辑器限制选项

                if (m_Constraint == Constraint.FixedColumnCount)
                {
                    cellCountX = m_ConstraintCount;
                }
                else if (m_Constraint == Constraint.Flexible)
                {
                    // 自适应时：
                    if (m_TemplateCellRT.rect.size.x + spacing.x <= 0)
                        //处理参数不合法的情况
                        cellCountX = int.MaxValue;
                    else
                    {
                        //列数 = 能放下的最大列数
                        float width = m_Content.rect.width;
                        cellCountX = Mathf.Max(1, Mathf.FloorToInt((width - padding.horizontal + spacing.x + 0.001f) / (m_TemplateCellRT.rect.size.x + spacing.x)));
                    }
                }

                if (m_CellCount > cellCountX)   //多于一列时
                    cellCountY = m_CellCount / cellCountX + (m_CellCount % cellCountX > 0 ? 1 : 0); //行数 = 整除（总数/列数） 有余数+1，没余数则不+
            }
            else
            {
                Debug.Assert(m_Constraint == Constraint.FixedRowCount || m_Constraint == Constraint.Flexible); //由编辑器限制选项

                if (m_Constraint == Constraint.FixedRowCount)
                {
                    cellCountY = m_ConstraintCount;
                }
                else if (m_Constraint == Constraint.Flexible)
                {
                    if (m_TemplateCellRT.rect.size.y + spacing.y <= 0)
                        //处理参数不合法的情况
                        cellCountY = int.MaxValue;
                    else
                    {
                        //行数 = 能放下的最大行数
                        float height = m_Content.rect.height;
                        cellCountY = Mathf.Max(1, Mathf.FloorToInt((height - padding.vertical + spacing.y + 0.001f) / (m_TemplateCellRT.rect.size.y + spacing.y)));
                    }
                }
                if (m_CellCount > cellCountY)   //多于一行时
                    cellCountX = m_CellCount / cellCountY + (m_CellCount % cellCountY > 0 ? 1 : 0); //列数 = 整除（总数/行数） 有余数+1，没余数则不+

            }

            //行列数约束至合法范围
            int cellsPerMainAxis;  //沿startAxis轴的格子数
            int actualCellCountX;  //实际列数
            int actualCellCountY;  //实际行数

            if (startAxis == MovementAxis.Horizontal)
            {
                cellsPerMainAxis = cellCountX;
                actualCellCountX = Mathf.Clamp(cellCountX, 1, m_CellCount);  //注意，这里Mathf.Clamp是因为上面自适应中非法时，将行列数设为了Int最大值。
                actualCellCountY = Mathf.Clamp(cellCountY, 1, Mathf.CeilToInt(m_CellCount / (float)cellsPerMainAxis));
            }
            else
            {
                cellsPerMainAxis = cellCountY;
                actualCellCountY = Mathf.Clamp(cellCountY, 1, m_CellCount);
                actualCellCountX = Mathf.Clamp(cellCountX, 1, Mathf.CeilToInt(m_CellCount / (float)cellsPerMainAxis));
            }

            this.m_CellsPerMainAxis = cellsPerMainAxis;
            this.m_ActualCellCountX = actualCellCountX;
            this.m_ActualCellCountY = actualCellCountY;
        }
        
        //计算实际需要的空间大小（不含padding） 及 在这个空间上第一个元素所在的位置
        private void CalculateRequiredSpace()
        {
            Vector2 requiredSpace = new Vector2(
                m_ActualCellCountX * m_TemplateCellRT.rect.size.x + (m_ActualCellCountX - 1) * spacing.x,
                m_ActualCellCountY * m_TemplateCellRT.rect.size.y + (m_ActualCellCountY - 1) * spacing.y
            );

            this.m_RequiredSpace = requiredSpace;
        }

        //设置滑动轴方向的Content大小
        private void SetContentSizeOnMovementAxis()
        {
            RectTransform.Axis axis;
            float size;
            if (m_MovementAxis == MovementAxis.Horizontal)
            {
                axis = RectTransform.Axis.Horizontal;
                size = m_RequiredSpace.x + padding.horizontal;
            }
            else
            {
                axis = RectTransform.Axis.Vertical;
                size = m_RequiredSpace.y + padding.vertical;
            }

            m_Content.SetSizeWithCurrentAnchors(axis, size);
        }

        //计算起始Offset
        private void CalculateStartOffset()
        {
            Vector2 startOffset = new Vector2(
                GetStartOffset(0, m_RequiredSpace.x),
                GetStartOffset(1, m_RequiredSpace.y)
            );
            this.m_StartOffset = startOffset;
        }
        
        //计算应出现的索引 和 应消失的索引
        private void CalcIndexes()
        {
            int cornerX = (int)m_StartCorner % 2;  //0：左， 1右
            int cornerY = (int)m_StartCorner / 2;  //0：上， 1下

            int outCountFromStart = 0;  //完全滑出起始边界的数量
            int outCountFromEnd = 0;    //完全滑出结束边界的数量

            if (m_MovementAxis == MovementAxis.Horizontal)
            {
                //content起始边界 相对于 viewport起始边界的位移（滑动轴延伸方向为正方向）：
                float outWidthFromStart = m_Content.anchoredPosition.x * (cornerX == 0 ? 1 : -1);
                //content结束边界 相对于 viewport结束边界的位移（滑动轴延伸方向为正方向）：
                float outWidthFromEnd = (m_Content.anchoredPosition.x + (m_Content.rect.width - m_Viewport.rect.width) * (cornerX == 0 ? 1 : -1)) * (cornerX == 0 ? 1 : -1);

                if (outWidthFromStart < 0)
                {
                    float startPadding = cornerX == 0 ? padding.left : padding.right;
                    //滑出的列数，要向下取整，即尽量认为其没滑出，以保证可视区域内的正确性。
                    int outColFromStart = Mathf.FloorToInt((-outWidthFromStart - startPadding + spacing.x) / (m_TemplateCellRT.rect.size.x + spacing.x));
                    outCountFromStart = Mathf.Clamp(outColFromStart * m_ActualCellCountY, 0, m_CellCount);
                }
                if (outWidthFromEnd > 0)
                {
                    float endPadding = cornerX == 0 ? padding.right : padding.left;
                    //滑出的列数，要向下取整，即尽量认为其没滑出，以保证可视区域内的正确性。
                    int outColFromEnd = Mathf.FloorToInt((outWidthFromEnd - endPadding + spacing.x) / (m_TemplateCellRT.rect.size.x + spacing.x));
                    //若最后一列未满，则从总数中减去。
                    int theLastColOffsetCount = m_CellCount % m_ActualCellCountY != 0 ? (m_ActualCellCountY - m_CellCount % m_ActualCellCountY) : 0;
                    outCountFromEnd = Mathf.Clamp(outColFromEnd * m_ActualCellCountY - theLastColOffsetCount, 0, m_CellCount);
                }
            }
            else
            {
                //content起始边界 相对于 viewport起始边界的位移（滑动轴延伸方向为正方向）：
                float outHeightFromStart = m_Content.anchoredPosition.y * (cornerY == 0 ? -1 : 1);
                //content结束边界 相对于 viewport结束边界的位移（滑动轴延伸方向为正方向）：
                float outHeightFromEnd = (m_Content.anchoredPosition.y + (m_Content.rect.height - m_Viewport.rect.height) * (cornerY == 0 ? -1 : 1)) * (cornerY == 0 ? -1 : 1);
                
                if (outHeightFromStart < 0)
                {
                    float startPadding = cornerY == 0 ? padding.top : padding.bottom;
                    //滑出的行数，要向下取整，即尽量认为其没滑出，以保证可视区域内的正确性。
                    int outRowFromStart = Mathf.FloorToInt((-outHeightFromStart - startPadding + spacing.y) / (m_TemplateCellRT.rect.size.y + spacing.y));
                    outCountFromStart = Mathf.Clamp(outRowFromStart * m_ActualCellCountX, 0, m_CellCount);
                }
                if (outHeightFromEnd > 0)
                {
                    float endPadding = cornerY == 0 ? padding.bottom : padding.top;
                    //滑出的行数，要向下取整，即尽量认为其没滑出，以保证可视区域内的正确性。
                    int outRowFromEnd = Mathf.FloorToInt((outHeightFromEnd - endPadding + spacing.y) / (m_TemplateCellRT.rect.size.y + spacing.y));
                    //若最后一行未满，则从总数中减去。
                    int theLastRowOffsetCount = m_CellCount % m_ActualCellCountX != 0 ? (m_ActualCellCountX - m_CellCount % m_ActualCellCountX) : 0;
                    outCountFromEnd = Mathf.Clamp(outRowFromEnd * m_ActualCellCountX - theLastRowOffsetCount, 0, m_CellCount);
                }
            }

            //应该显示的开始索引和结束索引
            int startIndex = (outCountFromStart); // 省略了先+1再-1。 从滑出的下一个开始，索引从0开始;
            int endIndex = (m_CellCount - 1 - outCountFromEnd);

            //Debug.Log("startIndex, endIndex: " + startIndex + ", " + endIndex);

            for (int index = startIndex; index <= endIndex; index++)
            {
                newIndexes.Add(index);
            }

            ////新旧索引列表输出调试
            //string Str1 = "";
            //foreach (int index in newIndexes)
            //{
            //    Str1 += index + ",";
            //}
            //string Str2 = "";
            //foreach (int index in oldIndexes)
            //{
            //    Str2 += index + ",";
            //}
            //Debug.Log("Str1: " + Str1);
            //Debug.Log("Str2: " + Str2);
            //Debug.Log("-------------------------");

            //找出出现的和消失的
            //出现的：在新列表中，但不在老列表中。
            appearIndexes.Clear();
            foreach (int index in newIndexes)
            {
                if (oldIndexes.IndexOf(index) < 0)
                {
                    //Debug.Log("出现：" + index);
                    appearIndexes.Add(index);
                }
            }

            //消失的：在老列表中，但不在新列表中。
            disAppearIndexes.Clear();
            foreach (int index in oldIndexes)
            {
                if (newIndexes.IndexOf(index) < 0)
                {
                    //Debug.Log("消失：" + index);
                    disAppearIndexes.Add(index);
                }
            }

            //oldIndexes保存当前帧索引数据。
            List<int> temp;
            temp = oldIndexes;
            oldIndexes = newIndexes;
            newIndexes = temp;
            newIndexes.Clear();
        }

        //该消失的消失
        private void DisAppearCells()
        {
            foreach (int index in disAppearIndexes)
            {
                RectTransform cellRT = cellRTDict[index];
                cellRTDict.Remove(index);
                cellRT.gameObject.SetActive(false);
                unUseCellRTStack.Push(cellRT);
            }
        }

        //该出现的出现
        private void AppearCells()
        {
            foreach (int index in appearIndexes)
            {
                RectTransform cellRT = GetOrCreateCell(index, out bool isNew);
                cellRTDict[index] = cellRT;
                cellRT.anchoredPosition = GetCellPos(index);    //设置Cell位置

                if (isNew) { m_OnCellCreated?.Invoke(index); }      //Cell创建回调
                m_OnCellAppear?.Invoke(index);                              //Cell出现回调
            }
        }
        
        //计算并设置Cells的SblingIndex
        //调用时机：有新的Cell出现时
        //Cell可能重叠时必须
        //若无需求，可去掉以节省性能
        private void CalcAndSetCellsSblingIndex()
        {
            if (appearIndexes.Count <= 0) { return; }

            cellRTListForSort.Clear();
            foreach (KeyValuePair<int, RectTransform> kvp in cellRTDict)
            {
                cellRTListForSort.Add(kvp);
            }
            cellRTListForSort.Sort((x, y) =>
            {
                //按index升序
                return x.Key - y.Key;
            });

            foreach (KeyValuePair<int, RectTransform> kvp in cellRTListForSort)
            {
                //索引大的在上
                //kvp.Value.SetAsLastSibling();
                //索引大的在下
                kvp.Value.SetAsFirstSibling();
            }
        }
        
        private float GetStartOffset(int axis, float requiredSpaceWithoutPadding)
        {
            float requiredSpace = requiredSpaceWithoutPadding + (axis == 0 ? padding.horizontal : padding.vertical);  //该轴上子元素需要的总尺寸 + 边距
            float availableSpace = m_Content.rect.size[axis];   //该轴上 LayoutGroup 的实际有效尺寸
            float surplusSpace = availableSpace - requiredSpace;  //剩余尺寸（可以是负的）
            float alignmentOnAxis = GetAlignmentOnAxis(axis);   //获取小数形式的子元素对齐方式

            //水平方向从左开始，竖直方向从上开始。
            // 要计入剩余尺寸。以水平方向为例，
            // 若对齐方式为居左，则 alignmentOnAxis 为 0， 结果为 padding.left + 0，可以达到居左效果；
            // 若对齐方式为居中，则 alignmentOnAxis 为 0.5， 结果为 padding.left + 0.5*剩余距离，可以达到居中效果；
            // 若对齐方式为居右，则 alignmentOnAxis 为 1， 结果为 padding.left + 1*剩余距离，可以达到居右效果。
            return (axis == 0 ? padding.left : padding.top) + surplusSpace * alignmentOnAxis;
        }

        // Returns the alignment on the specified axis as a fraction where 0 is left/top, 0.5 is middle, and 1 is right/bottom.
        // 以小数形式返回指定轴上的对齐方式，其中0为左/上，0.5为中，1为右/下。（水平方向：0左，0.5中，1右）（竖直方向：0上，0.5中，1下）
        // 参数 "axis"：The axis to get alignment along. 0 is horizontal and 1 is vertical.    //轴索引，0是水平的，1是垂直的。
        // 返回值：The alignment as a fraction where 0 is left/top, 0.5 is middle, and 1 is right/bottom. //小数形式的对齐方式
        private float GetAlignmentOnAxis(int axis)
        {
            return (axis == (int)m_MovementAxis) ? 0.5f : (int)childAlignment * 0.5f;
        }

        private Vector2 GetCellPos(int index)
        {
            //一、计算索引
            int cornerX = (int)m_StartCorner % 2;  //0：左， 1右
            int cornerY = (int)m_StartCorner / 2;  //0：上， 1下

            int posIndexX;   //X位置索引
            int posIndexY;   //Y位置索引
            if (startAxis == MovementAxis.Horizontal)
            {
                posIndexX = index % m_CellsPerMainAxis;
                posIndexY = index / m_CellsPerMainAxis;
            }
            else
            {
                posIndexX = index / m_CellsPerMainAxis;
                posIndexY = index % m_CellsPerMainAxis;
            }

            //根据起始角进行转置
            if (cornerX == 1)  //如果是从右往左
                posIndexX = m_ActualCellCountX - 1 - posIndexX;
            if (cornerY == 1) //如果是从下往上
                posIndexY = m_ActualCellCountY - 1 - posIndexY;

            //二、计算坐标
            Vector2 scaleFactor = Vector2.one;  //不考虑元素缩放

            // x轴：初始位置+宽度*中心点偏移*缩放系数 (x轴是向正方向)(从左上到右下)
            float anchoredPosX = (m_StartOffset.x + (m_TemplateCellRT.rect.size.x + spacing.x) * posIndexX) + m_TemplateCellRT.rect.size.x * m_TemplateCellRT.pivot.x * scaleFactor.x;

            // y轴：-初始位置-宽度*(1-中心点偏移)*缩放系数 (y轴是向负方向)(从左上到右下)
            float anchoredPosY = -(m_StartOffset.y + (m_TemplateCellRT.rect.size.y + spacing.y) * posIndexY) - m_TemplateCellRT.rect.size.y * (1f - m_TemplateCellRT.pivot.y) * scaleFactor.y;

            return new Vector2(anchoredPosX, anchoredPosY);
        }

        private void SetProperty<T>(ref T currentValue, T newValue)
        {
            if ((currentValue == null && newValue == null) || (currentValue != null && currentValue.Equals(newValue)))  //过滤无效和未变
                return;
            currentValue = newValue;
            //Refresh();
        }

        private RectTransform GetOrCreateCell(int index, out bool isNew)
        {
            RectTransform cellRT;
            if (unUseCellRTStack.Count > 0)
            {
                isNew = false;
                cellRT = unUseCellRTStack.Pop();
                cellRT.gameObject.SetActive(true);
            }
            else
            {
                isNew = true;

                cellRT = GameObject.Instantiate<GameObject>(m_TemplateCellRT.gameObject).GetComponent<RectTransform>();
                cellRT.SetParent(m_Content, false);

                //驱动子物体的锚点和位置
                m_Tracker.Add(this, cellRT, DrivenTransformProperties.Anchors | DrivenTransformProperties.AnchoredPosition | DrivenTransformProperties.SizeDelta);

                //强制设置Cell的anchor
                cellRT.anchorMin = Vector2.up;
                cellRT.anchorMax = Vector2.up;

                cellRT.sizeDelta = m_TemplateCellRT.rect.size;
            }

            return cellRT;
        }

        //protected override void OnRectTransformDimensionsChange()
        //{
        //    base.OnRectTransformDimensionsChange();
        //    Refresh();
        //}

        //#if UNITY_EDITOR
        //        protected override void OnValidate()
        //        {
        //            base.OnValidate();
        //            Refresh();
        //        }
        //#endif
    }
}
