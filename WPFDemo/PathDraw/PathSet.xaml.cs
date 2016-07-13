﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WPFDemo.PathDraw
{
    /// <summary>
    /// 路径设置
    /// </summary>
    public partial class PathSet : UserControl
    {
        private List<NodeModel> tmpNodeModels = new List<NodeModel>();
        private List<RunLineModel> tmpLineModels = new List<RunLineModel>();


        private List<NodeBorder> nodeList = new List<NodeBorder>();
        private List<RunLine> runLineList = new List<RunLine>();
        /// <summary>
        /// 控制选择的控件
        /// </summary>
        private ControlType controlType = ControlType.None;
        /// <summary>
        /// 当前操作状态
        /// </summary>
        private OperateStatus _operateStatus = OperateStatus.None;
        /// <summary>
        /// 当前属性面板
        /// </summary>
        public ControlType CurrentPropertyPanel
        {
            get
            {
                if (nodePropertyPanel.Visibility == Visibility.Visible) return ControlType.Node;
                else if (linePropertyPanel.Visibility == Visibility.Visible) return ControlType.RunLine;
                else return 0;
            }
            set
            {
                nodePropertyPanel.Visibility = Visibility.Collapsed;
                linePropertyPanel.Visibility = Visibility.Collapsed;
                if (value == ControlType.Node)
                    nodePropertyPanel.Visibility = Visibility.Visible;
                else if (value == ControlType.RunLine)
                    linePropertyPanel.Visibility = Visibility.Visible;
            }
        }


        public PathSet()
        {
            InitializeComponent();
            this.Loaded += PathSet_Loaded;
            canvasPanel.SizeChanged += canvasPanel_SizeChanged;
        }

        private void PathSet_Loaded(object sender, RoutedEventArgs e)
        {
            LoadData();
            MakeUI();
        }

        private void LoadData()
        {
            var data = Util.Util.ReadData<Tuple<List<NodeModel>, List<RunLineModel>>>("PathSetData.dat");
            if (data != null)
            {
                if (data.Item1 != null)
                    tmpNodeModels = data.Item1;
                if (data.Item2 != null)
                    tmpLineModels = data.Item2;
            }
        }

        public void SaveData()
        {
            foreach (var item in nodeList)
            {
                item.X = item.X / canvasPanel.ActualWidth;
                item.Y = item.Y / canvasPanel.ActualHeight;
            }

            var nodeModes = nodeList.Select(a => a.Model).ToList();
            var lineModes = runLineList.Select(a => a.Model).ToList();

            var data = new Tuple<List<NodeModel>, List<RunLineModel>>(nodeModes, lineModes);
            if (data != null)
            {
                Util.Util.SaveData(data, "PathSetData.dat");
            }

        }


        private void MakeUI()
        {
            foreach (var item in tmpNodeModels)
            {
                var node = MakeNode(item);
                if (node != null)
                {
                    canvasPanel.Children.Add(node);
                    nodeList.Add(node);
                }
            }

            foreach (var item in tmpLineModels)
            {
                var line = MakeLine(item);
                if (line != null)
                {
                    line.Model.StartNode = tmpNodeModels.FirstOrDefault(a => a.Name.Equals(line.Model.StartNode.Name));
                    line.Model.EndNode = tmpNodeModels.FirstOrDefault(a => a.Name.Equals(line.Model.EndNode.Name));
                    canvasPanel.Children.Add(line);
                    runLineList.Add(line);
                    AutoOptimizeLine(line);
                }
            }
        }



        private void lvSelector_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (lvSelector.SelectedIndex < 0) return;

            ControlType selectControlType = ControlType.None;
            switch (lvSelector.SelectedIndex)
            {
                case 0:
                    selectControlType = ControlType.Node;
                    _operateStatus = OperateStatus.NewNode;
                    break;
                case 1:
                    selectControlType = ControlType.RunLine;
                    _operateStatus = OperateStatus.NewLine;
                    break;
                default:
                    break;
            }

            if (selectControlType == controlType)
            {
                lvSelector.SelectedItem = null;
                controlType = ControlType.None;
                _operateStatus = OperateStatus.None;
            }
            else
                controlType = selectControlType;
        }

        private void canvasPanel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void canvasPanel_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Point point = e.GetPosition(canvasPanel);
            var ctrlState = Keyboard.GetKeyStates(Key.LeftCtrl);
            if (_operateStatus == OperateStatus.NewNode)
            {
                AddNode(new Point(point.X / canvasPanel.ActualWidth, point.Y / canvasPanel.ActualHeight));
                //若检测到用户按住ctrl键可连续点击添加多个节点
                if ((ctrlState & KeyStates.Down) != KeyStates.Down)
                {
                    controlType = ControlType.None;
                    _operateStatus = OperateStatus.None;
                    lvSelector.SelectedIndex = -1;
                }
                else
                    _operateStatus = OperateStatus.NewNode;
            }
            else if (_operateStatus == OperateStatus.NewLine)
            {
                //添加一条行驶路线
                if (SelectedNodeBorder != null)
                {
                    SelectedRunLine = AddLine(SelectedNodeBorder);
                    _operateStatus = OperateStatus.SelectedSouceNode;
                }
            }
            else if (_operateStatus == OperateStatus.SelectedSouceNode)
            {
                //完成路线目标节点的选择
                if (SelectedRunLine != null && SelectedNodeBorder != null)
                {
                    if (!SelectedNodeBorder.Model.Equals(SelectedRunLine.Model.StartNode))
                    {
                        SelectedRunLine.Model.EndNode = SelectedNodeBorder.Model;
                        _operateStatus = OperateStatus.SelectedObjNode;

                        //设置目标路径自动优化连接点
                        var sourceNode = nodeList.FirstOrDefault(a => a.Model.Equals(SelectedRunLine.Model.StartNode));
                        if (sourceNode != null)
                        {
                            var border = SelectedNodeBorder;
                            var suitPoint = GetConnectPoint(border, new Point(sourceNode.CircleCenterPoint.X, sourceNode.CircleCenterPoint.Y));
                            SelectedRunLine.EndPoint = suitPoint;
                        }

                        //若检测到用户按住ctrl键可连续添加多个路线
                        if ((ctrlState & KeyStates.Down) != KeyStates.Down)
                        {
                            controlType = ControlType.None;
                            _operateStatus = OperateStatus.None;
                            lvSelector.SelectedIndex = -1;
                        }
                        else
                            _operateStatus = OperateStatus.NewLine;
                    }
                }
            }
            else if (_operateStatus == OperateStatus.MoveSourceNode)
            {
                //完成路线源节点的移动
                if (SelectedRunLine != null && SelectedNodeBorder != null)
                {
                    SelectedRunLine.Model.StartNode = SelectedNodeBorder.Model;
                    _operateStatus = OperateStatus.SelectedLine;

                    //设置源路径自动优化连接点    
                    var objectNode = nodeList.FirstOrDefault(a => a.Model.Equals(SelectedRunLine.Model.EndNode));
                    if (objectNode != null)
                    {
                        var suitPoint = GetConnectPoint(SelectedNodeBorder, new Point(objectNode.CircleCenterPoint.X, objectNode.CircleCenterPoint.Y));
                        SelectedRunLine.StartPoint = suitPoint;
                    }
                }
            }
            else if (_operateStatus == OperateStatus.MoveObjNode)
            {
                //完成路线目标节点的移动
                if (SelectedNodeBorder != null && SelectedRunLine != null)
                {
                    if (!SelectedNodeBorder.Model.Equals(SelectedRunLine.Model.StartNode))
                    {
                        SelectedRunLine.Model.EndNode = SelectedNodeBorder.Model;
                        _operateStatus = OperateStatus.SelectedLine;

                        //设置目标路径自动优化连接点    
                        var sourceNode = nodeList.FirstOrDefault(a => a.Model.Equals(SelectedRunLine.Model.StartNode));
                        if (sourceNode != null)
                        {
                            var suitPoint = GetConnectPoint(SelectedNodeBorder, new Point(sourceNode.CircleCenterPoint.X, sourceNode.CircleCenterPoint.Y));
                            SelectedRunLine.EndPoint = suitPoint;
                        }

                    }
                }
            }
        }
        /// <summary>
        /// 获取该节点所有相关联的线
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private List<RunLine> GetNodeRelativeLine(NodeBorder node)
        {
            if (node == null || node.Model == null) return null;
            var model = node.Model;
            var list = runLineList.Where(a => model.Equals(a.Model.StartNode) || model.Equals(a.Model.EndNode));
            return list.ToList();
        }

        private void canvasPanel_MouseMove(object sender, MouseEventArgs e)
        {
            var point = e.GetPosition(canvasPanel);
            point.X += 5;
            point.Y -= 5;


            if ((_operateStatus == OperateStatus.MoveNode) && SelectedNodeBorder != null)
            {
                SelectedNodeBorder.X = point.X - SelectedNodeBorder.Width / 2;
                SelectedNodeBorder.Y = point.Y - SelectedNodeBorder.Height / 2;

                var list = GetNodeRelativeLine(SelectedNodeBorder);
                if (list != null)
                    foreach (var item in list)
                    {
                        AutoOptimizeLine(item);
                    }
            }
            else if ((_operateStatus == OperateStatus.SelectedSouceNode ||
                 _operateStatus == OperateStatus.MoveObjNode) &&
                SelectedRunLine != null)
            {
                var x = point.X;
                var y = point.Y;
                SelectedRunLine.EndPoint = new Point(x, y);

                //设置源路径自动优化连接点
                if (SelectedRunLine.Model.StartNode != null)
                {
                    var border = nodeList.FirstOrDefault(a => a.Model.Equals(SelectedRunLine.Model.StartNode));
                    var suitPoint = GetConnectPoint(border, point);
                    SelectedRunLine.StartPoint = suitPoint;
                }

            }
            else if ((_operateStatus == OperateStatus.MoveSourceNode) &&
               SelectedRunLine != null)
            {
                var x = point.X;
                var y = point.Y;
                SelectedRunLine.StartPoint = new Point(x, y);

                //设置目标路径自动优化连接点
                if (SelectedRunLine.Model.EndNode != null)
                {
                    var border = nodeList.FirstOrDefault(a => a.Model.Equals(SelectedRunLine.Model.EndNode));
                    var suitPoint = GetConnectPoint(border, point);
                    SelectedRunLine.EndPoint = suitPoint;
                }
            }
        }
        /// <summary>
        /// 自动优化线的连接位置
        /// </summary>
        /// <param name="line"></param>
        private void AutoOptimizeLine(RunLine line)
        {
            var startNode = nodeList.FirstOrDefault(a => a.Model.Name.Equals(line.Model.StartNode.Name));
            var endNode = nodeList.FirstOrDefault(a => a.Model.Name.Equals(line.Model.EndNode.Name));

            if (startNode == null || endNode == null) return;
            line.StartPoint = GetConnectPoint(startNode, endNode.CircleCenterPoint);
            line.EndPoint = GetConnectPoint(endNode, startNode.CircleCenterPoint);            

        }

        /// <summary>
        /// 获取节点的最佳连接点
        /// </summary>
        /// <param name="border"></param>
        /// <param name="relativePoint"></param>
        /// <returns></returns>
        private Point GetConnectPoint(NodeBorder border, Point relativePoint)
        {
            Point point = new Point();
            if (border != null)
            {
                double x, y;
                var centerPoint = border.CircleCenterPoint;

                //使用圆的角度解决,由圆弧公式可求出xy值
                double rad = Math.Atan2((relativePoint.Y - centerPoint.Y), (relativePoint.X - centerPoint.X));// 圆弧     
                x = centerPoint.X + border.CircleR * Math.Cos(rad);
                y = centerPoint.Y + border.CircleR * Math.Sin(rad);

                point.X = x;
                point.Y = y;
            }
            return point;
        }


        /// <summary>
        /// 添加节点
        /// </summary>
        /// <param name="point"></param>
        private void AddNode(Point point)
        {
            var node = new NodeModel();
            //自动生成 节点名
            int i = 0;
            while (true)
            {
                node.Name = "节点" + (++i);
                if (nodeList.FirstOrDefault(a => a.Model != null && node.Name.Equals(a.Model.Name)) == null)
                    break;
            }
            node.Position = point;
            var border = MakeNode(node);
            if (border != null)
            {
                canvasPanel.Children.Add(border);
                nodeList.Add(border);
            }
        }

        private NodeBorder MakeNode(NodeModel mode)
        {
            var border = new NodeBorder();
            border.Cursor = Cursors.Hand;
            border.X = mode.Position.X * canvasPanel.ActualWidth - border.Width / 2;
            border.Y = mode.Position.Y * canvasPanel.ActualHeight - border.Height / 2;
            border.MouseLeftButtonDown += Border_MouseLeftButtonDown;
            border.MouseLeftButtonUp += Border_MouseLeftButtonUp;
            border.Model = mode;
            return border;
        }

        /// <summary>
        /// 当前选择的节点
        /// </summary>
        private NodeBorder _selectedNodeBorder = null;

        /// <summary>
        /// 当前选择的节点
        /// </summary>
        private NodeBorder SelectedNodeBorder
        {
            get
            {
                return _selectedNodeBorder;
            }
            set
            {
                //if (value != _selectedNodeBorder)
                //{
                    if (_selectedNodeBorder != null)
                        _selectedNodeBorder.IsSelected = false;
                    _selectedNodeBorder = value;
                    if (value != null)
                    {
                        value.IsSelected = true;
                        CurrentPropertyPanel = ControlType.Node;
                        if (_selectedRunLine != null)
                        {
                            _selectedRunLine.IsSelected = false;
                            //_selectedRunLine = null;
                        }
                    }
                //}
            }
        }



        private void Border_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var border = sender as NodeBorder;
            if (border == null) return;
            if (border != SelectedNodeBorder || border.IsSelected != true)
            {
                //变更选择的节点
                border.IsSelected = true;
                if (SelectedNodeBorder != null)
                    SelectedNodeBorder.IsSelected = false;
                SelectedNodeBorder = border;
            }
            if (_operateStatus == OperateStatus.None ||
                _operateStatus == OperateStatus.MoveNode ||
                _operateStatus == OperateStatus.SelectedLine)
                _operateStatus = OperateStatus.SelectedNode;

        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var border = sender as NodeBorder;
            if (border != null && (border == SelectedNodeBorder) && (_operateStatus == OperateStatus.SelectedNode))
            {
                //若该节点已选择,鼠标处于按下状态则可以移动节点
                _operateStatus = OperateStatus.MoveNode;
            }
        }



        private RunLine _selectedRunLine = null;
        private RunLine SelectedRunLine
        {
            get
            {
                return _selectedRunLine;
            }
            set
            {
                if (value != _selectedRunLine)
                {
                    if (_selectedRunLine != null)
                        _selectedRunLine.IsSelected = false;
                    _selectedRunLine = value;
                    if (value != null)
                    {
                        value.IsSelected = true;
                        CurrentPropertyPanel = ControlType.RunLine;
                        if (_selectedNodeBorder != null)
                        {
                            _selectedNodeBorder.IsSelected = false;
                        }
                    }
                }

            }
        }

        /// <summary>
        /// 添加节点与节点间的连线
        /// </summary>
        /// <param name="sourceNode"></param>
        /// <param name="objNode"></param>
        private void AddLine(NodeBorder sourceNode, NodeBorder objNode)
        {
            var lineMode = new RunLineModel();

            var line = new RunLine();
            var x = sourceNode.X * canvasPanel.ActualWidth - sourceNode.Width / 2;
            var y = sourceNode.Y * canvasPanel.ActualHeight - sourceNode.Height / 2;
            line.StartPoint = new Point(x, y);
            if (objNode != null)
            {
                x = objNode.X * canvasPanel.ActualWidth - objNode.Width / 2;
                y = objNode.Y * canvasPanel.ActualHeight - objNode.Height / 2;
                line.EndPoint = new Point(x, y);
            }
            line.MouseLeftButtonUp += Line_MouseLeftButtonUp;
            canvasPanel.Children.Add(line);
            line.Model = lineMode;
            runLineList.Add(line);
        }



        /// <summary>
        /// 添加节点与节点间的连线
        /// </summary>
        /// <param name="sourceNode"></param>
        /// <param name="objNode"></param>
        private RunLine AddLine(NodeBorder sourceNode)
        {
            var lineMode = new RunLineModel();
            lineMode.StartNode = sourceNode.Model;

            var line = MakeLine(lineMode);
            line.Height = 15;
            var x = sourceNode.X - sourceNode.Width / 2;
            var y = sourceNode.Y - sourceNode.Height / 2;
            line.StartPoint = new Point(x, y);
            line.EndPoint = new Point(x, y);
            canvasPanel.Children.Add(line);
            runLineList.Add(line);
            return line;
        }

        private RunLine MakeLine(RunLineModel mode)
        {
            var line = new RunLine();
            line.Height = 15;
            line.Cursor = Cursors.Hand;
            line.MouseLeftButtonUp += Line_MouseLeftButtonUp;
            line.Model = mode;
            return line;
        }


        private void Line_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var line = sender as RunLine;
            if (line == null) return;

            if (SelectedRunLine != null && SelectedRunLine.IsSelected == false)
            {
                //防止更改线的节点后，选择了节点而SelectedRunLine没有重置的问题
                SelectedRunLine = null;
                SelectedRunLine = line;
                _operateStatus = OperateStatus.SelectedLine;
            }

            else if (line != SelectedRunLine &&
                _operateStatus != OperateStatus.MoveSourceNode &&
                _operateStatus != OperateStatus.MoveObjNode
                )
            {
                //变更选择的行驶路线
                line.IsSelected = true;
                if (SelectedRunLine != null)
                    SelectedRunLine.IsSelected = false;
                SelectedRunLine = line;
                _operateStatus = OperateStatus.SelectedLine;
            }
            else if (_operateStatus == OperateStatus.SelectedLine)
            {
                var point = e.GetPosition(canvasPanel);

                //若首尾x坐标相差很大(>30)则使用x区分首尾，否则使用y区分
                if (Math.Abs(SelectedRunLine.EndPoint.X - SelectedRunLine.StartPoint.X) > 30)
                {
                    if (Math.Abs(point.X - SelectedRunLine.StartPoint.X) < Math.Abs(point.X - SelectedRunLine.EndPoint.X))
                        _operateStatus = OperateStatus.MoveSourceNode;
                    else
                        _operateStatus = OperateStatus.MoveObjNode;
                }
                else
                {
                    if (Math.Abs(point.Y - SelectedRunLine.StartPoint.Y) < Math.Abs(point.Y - SelectedRunLine.EndPoint.Y))
                        _operateStatus = OperateStatus.MoveSourceNode;
                    else
                        _operateStatus = OperateStatus.MoveObjNode;
                }

                if (SelectedNodeBorder != null)
                {
                    SelectedNodeBorder.IsSelected = false;
                    SelectedNodeBorder = null;
                }
            }

        }

        private void canvasPanel_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            foreach (FrameworkElement item in canvasPanel.Children)
            {
                var node = item as NodeBorder;
                if (node != null)
                {
                    node.X = node.X / e.PreviousSize.Width * e.NewSize.Width;
                    node.Y = node.Y / e.PreviousSize.Height * e.NewSize.Height;
                }
            }
            foreach (var item in runLineList)
            {
                AutoOptimizeLine(item);
            }
        }
    }

    /// <summary>
    /// 操作进行中的状态
    /// </summary>
    public enum OperateStatus
    {
        /// <summary>
        /// 无
        /// </summary>
        None,
        #region 节点状态
        /// <summary>
        /// 等待创建新节点
        /// </summary>
        NewNode,
        /// <summary>
        /// 移动节点
        /// </summary>
        MoveNode,
        /// <summary>
        /// 选择了节点
        /// </summary>
        SelectedNode,
        #endregion
        #region 线状态
        /// <summary>
        /// 等待创建新路线
        /// </summary>
        NewLine,
        /// <summary>
        /// 选择了源节点
        /// </summary>
        SelectedSouceNode,
        /// <summary>
        /// 选择了目标节点
        /// </summary>
        SelectedObjNode,
        /// <summary>
        /// 选择了一条线
        /// </summary>
        SelectedLine,
        /// <summary>
        /// 移动线的源节点
        /// </summary>
        MoveSourceNode,
        /// <summary>
        /// 移动线的目标节点
        /// </summary>
        MoveObjNode
        #endregion
    }

    /// <summary>
    /// 节点
    /// </summary>
    public class NodeModel
    {
        /// <summary>
        /// 节点名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 相对位置坐标
        /// </summary>
        public Point Position { get; set; }
        /// <summary>
        /// 堆场真实坐标
        /// </summary>
        public Point RealPosition { get; set; }
        /// <summary>
        /// 是否可用
        /// </summary>

        public bool IsEnable { get; set; } = true;

        public override string ToString()
        {
            return Name;
        }
    }

    /// <summary>
    /// 行驶路线
    /// </summary>
    public class RunLineModel
    {
        /// <summary>
        /// 车道类型(0-2)
        /// </summary>
        public int LaneType { get; set; }
        /// <summary>
        /// 行驶方向(0-2)
        /// </summary>
        public int RunDirection { get; set; }
        /// <summary>
        /// 源节点
        /// </summary>
        public NodeModel StartNode { get; set; }
        /// <summary>
        /// 目标节点
        /// </summary>
        public NodeModel EndNode { get; set; }

        //public Point StartPoint { get; set; }
        //public Point EndPoint { get; set; }

    }
    /// <summary>
    /// 控件类型
    /// </summary>
    public enum ControlType : byte
    {
        /// <summary>
        /// 无
        /// </summary>
        None,
        /// <summary>
        /// 节点
        /// </summary>
        Node,
        /// <summary>
        /// 行驶路线
        /// </summary>
        RunLine
    }

}
