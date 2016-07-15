namespace WPFDemo.PathDraw
{
    using System.Windows;
    using System.Windows.Media;

    /// <summary>
    /// ����֮�����ͷ��ֱ��
    /// </summary>
    public class RunLine : ArrowBase
    {
        #region Fields

        /// <summary>
        /// ������
        /// </summary>
        public static readonly DependencyProperty EndPointProperty = DependencyProperty.Register(
            "EndPoint",
            typeof(Point),
            typeof(RunLine),
            new FrameworkPropertyMetadata(default(Point), FrameworkPropertyMetadataOptions.AffectsMeasure));

        /// <summary>
        /// �߶�
        /// </summary>
        private readonly LineSegment lineSegment = new LineSegment();

        #endregion Fields

        #region Properties

        public bool IsSelected { get; set; }

        public RunLineModel Model { get; set; }
             

        /// <summary>
        /// ������
        /// </summary>
        public Point EndPoint
        {
            get { return (Point)this.GetValue(EndPointProperty); }
            set { this.SetValue(EndPointProperty, value); }
        }

        #endregion Properties

        #region Protected Methods

        /// <summary>
        /// ���Figure
        /// </summary>
        /// <returns>PathSegment����</returns>
        protected override PathSegmentCollection FillFigure()
        {
            this.lineSegment.Point = this.EndPoint;
            return new PathSegmentCollection
            {
                this.lineSegment
            };
        }

        /// <summary>
        /// ��ȡ��ʼ��ͷ���Ľ�����
        /// </summary>
        /// <returns>��ʼ��ͷ���Ľ�����</returns>
        protected override Point GetStartArrowEndPoint()
        {
            return this.EndPoint;
        }

        /// <summary>
        /// ��ȡ������ͷ���Ŀ�ʼ��
        /// </summary>
        /// <returns>������ͷ���Ŀ�ʼ��</returns>
        protected override Point GetEndArrowStartPoint()
        {
            return this.StartPoint;
        }

        /// <summary>
        /// ��ȡ������ͷ���Ľ�����
        /// </summary>
        /// <returns>������ͷ���Ľ�����</returns>
        protected override Point GetEndArrowEndPoint()
        {
            return this.EndPoint;
        }

        #endregion  Protected Methods
    }
}
