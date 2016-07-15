namespace WPFDemo.PathDraw
{
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Shapes;

    /// <summary>
    /// ��ͷ����
    /// </summary>
    public abstract class ArrowBase : Shape
    {
        #region Fields

        #region DependencyProperty

        /// <summary>
        /// ��ͷ���߼нǵ���������
        /// </summary>
        public static readonly DependencyProperty ArrowAngleProperty = DependencyProperty.Register(
            "ArrowAngle",
            typeof(double),
            typeof(ArrowBase),
            new FrameworkPropertyMetadata(45.0, FrameworkPropertyMetadataOptions.AffectsMeasure));

        /// <summary>
        /// ��ͷ���ȵ���������
        /// </summary>
        public static readonly DependencyProperty ArrowLengthProperty = DependencyProperty.Register(
            "ArrowLength",
            typeof(double),
            typeof(ArrowBase),
            new FrameworkPropertyMetadata(12.0, FrameworkPropertyMetadataOptions.AffectsMeasure));

        /// <summary>
        /// ��ͷ���ڶ˵���������
        /// </summary>
        public static readonly DependencyProperty ArrowEndsProperty = DependencyProperty.Register(
            "ArrowEnds",
            typeof(ArrowEnds),
            typeof(ArrowBase),
            new FrameworkPropertyMetadata(ArrowEnds.End, FrameworkPropertyMetadataOptions.AffectsMeasure));

        /// <summary>
        /// ��ͷ�Ƿ�պϵ���������
        /// </summary>
        public static readonly DependencyProperty IsArrowClosedProperty = DependencyProperty.Register(
            "IsArrowClosed",
            typeof(bool),
            typeof(ArrowBase),
            new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsMeasure));

        /// <summary>
        /// ��ʼ��
        /// </summary>
        public static readonly DependencyProperty StartPointProperty = DependencyProperty.Register(
            "StartPoint",
            typeof(Point),
            typeof(ArrowBase),
            new FrameworkPropertyMetadata(default(Point), FrameworkPropertyMetadataOptions.AffectsMeasure));

        #endregion DependencyProperty

        /// <summary>
        /// ������״(������ͷ�;�����״)
        /// </summary>
        private readonly PathGeometry geometryWhole = new PathGeometry();

        /// <summary>
        /// ��ȥ��ͷ��ľ�����״
        /// </summary>
        private readonly PathFigure figureConcrete = new PathFigure();

        /// <summary>
        /// ��ʼ���ļ�ͷ�߶�
        /// </summary>
        private readonly PathFigure figureStart = new PathFigure();

        /// <summary>
        /// �������ļ�ͷ�߶�
        /// </summary>
        private readonly PathFigure figureEnd = new PathFigure();

        #endregion Fields

        #region Constructor

        /// <summary>
        /// ���캯��
        /// </summary>
        protected ArrowBase()
        {
            var polyLineSegStart = new PolyLineSegment();
            this.figureStart.Segments.Add(polyLineSegStart);

            var polyLineSegEnd = new PolyLineSegment();
            this.figureEnd.Segments.Add(polyLineSegEnd);

            this.geometryWhole.Figures.Add(this.figureConcrete);
            this.geometryWhole.Figures.Add(this.figureStart);
            this.geometryWhole.Figures.Add(this.figureEnd);
        }

        #endregion Constructor

        #region Properties

        /// <summary>
        /// ��ͷ���߼н�
        /// </summary>
        public double ArrowAngle
        {
            get { return (double)this.GetValue(ArrowAngleProperty); }
            set { this.SetValue(ArrowAngleProperty, value); }
        }

        /// <summary>
        /// ��ͷ���ߵĳ���
        /// </summary>
        public double ArrowLength
        {
            get { return (double)this.GetValue(ArrowLengthProperty); }
            set { this.SetValue(ArrowLengthProperty, value); }
        }

        /// <summary>
        /// ��ͷ���ڶ�
        /// </summary>
        public ArrowEnds ArrowEnds
        {
            get { return (ArrowEnds)this.GetValue(ArrowEndsProperty); }
            set { this.SetValue(ArrowEndsProperty, value); }
        }

        /// <summary>
        /// ��ͷ�Ƿ�պ�
        /// </summary>
        public bool IsArrowClosed
        {
            get { return (bool)this.GetValue(IsArrowClosedProperty); }
            set { this.SetValue(IsArrowClosedProperty, value); }
        }

        /// <summary>
        /// ��ʼ��
        /// </summary>
        public Point StartPoint
        {
            get { return (Point)this.GetValue(StartPointProperty); }
            set { this.SetValue(StartPointProperty, value); }
        }

        /// <summary>
        /// ������״
        /// </summary>
        protected override Geometry DefiningGeometry
        {
            get
            {
                this.figureConcrete.StartPoint = this.StartPoint;

                // ��վ�����״,�����ظ����
                this.figureConcrete.Segments.Clear();
                var segements = this.FillFigure();
                if (segements != null)
                {
                    foreach (var segement in segements)
                    {
                        this.figureConcrete.Segments.Add(segement);
                    }
                }

                // ���ƿ�ʼ���ļ�ͷ
                if ((this.ArrowEnds & ArrowEnds.Start) == ArrowEnds.Start)
                {
                    this.CalculateArrow(this.figureStart, this.GetStartArrowEndPoint(), this.StartPoint);
                }

                // ���ƽ������ļ�ͷ
                if ((this.ArrowEnds & ArrowEnds.End) == ArrowEnds.End)
                {
                    this.CalculateArrow(this.figureEnd, this.GetEndArrowStartPoint(), this.GetEndArrowEndPoint());
                }

                return this.geometryWhole;
            }
        }

        #endregion Properties

        #region Protected Methods

        /// <summary>
        /// ��ȡ������״�ĸ�����ɲ���
        /// </summary>
        /// <returns>PathSegment����</returns>
        protected abstract PathSegmentCollection FillFigure();

        /// <summary>
        /// ��ȡ��ʼ��ͷ���Ľ�����
        /// </summary>
        /// <returns>��ʼ��ͷ���Ľ�����</returns>
        protected abstract Point GetStartArrowEndPoint();

        /// <summary>
        /// ��ȡ������ͷ���Ŀ�ʼ��
        /// </summary>
        /// <returns>������ͷ���Ŀ�ʼ��</returns>
        protected abstract Point GetEndArrowStartPoint();

        /// <summary>
        /// ��ȡ������ͷ���Ľ�����
        /// </summary>
        /// <returns>������ͷ���Ľ�����</returns>
        protected abstract Point GetEndArrowEndPoint();

        #endregion  Protected Methods

        #region Private Methods

        /// <summary>
        /// ����������֮��������ͷ
        /// </summary>
        /// <param name="pathfig">��ͷ���ڵ���״</param>
        /// <param name="startPoint">��ʼ��</param>
        /// <param name="endPoint">������</param>
        private void CalculateArrow(PathFigure pathfig, Point startPoint, Point endPoint)
        {
            var polyseg = pathfig.Segments[0] as PolyLineSegment;
            if (polyseg != null)
            {
                var matx = new Matrix();
                Vector vect = startPoint - endPoint;

                // ��ȡ��λ����
                vect.Normalize();
                vect *= this.ArrowLength;

                // ��ת�нǵ�һ��
                matx.Rotate(this.ArrowAngle / 2);

                // �����ϰ�μ�ͷ�ĵ�
                pathfig.StartPoint = endPoint + (vect * matx);

                polyseg.Points.Clear();
                polyseg.Points.Add(endPoint);

                matx.Rotate(-this.ArrowAngle);

                // �����°�μ�ͷ�ĵ�
                polyseg.Points.Add(endPoint + (vect * matx));
            }

            pathfig.IsClosed = this.IsArrowClosed;
        }

        #endregion Private Methods
    }
}
