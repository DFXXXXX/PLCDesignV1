using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace PLCDesignV1
{
    public class PLCDesignGridServices
    {
        Dictionary<(int, int), Point> _gridIndex = new Dictionary<(int, int), Point>();
        public Grid DesignCanvas { get; set; }
        public int Rows { get; set; }

        private AFindRoute aFind;
        public int Columns { get; set; }

        public Rectangle _selectedRectangle;

        public int CurrentRectangeRow { get; set; }
        public int CurrentRectangeCloumn { get; set; }

        private PlcModuleManager _plcModuleManager;
        public PLCDesignGridServices(int rows, int columns, Grid DesignCanvas, PlcModuleManager plcModuleManager)
        {
            this.DesignCanvas = DesignCanvas;
            this.Rows = rows;
            this.Columns = columns;
            CreateGrid(rows, columns);
            _plcModuleManager = plcModuleManager;
            aFind = new AFindRoute(MainWindow.GridCellRectanglWidth, MainWindow.GridCellSize, _gridIndex, _plcModuleManager);
        }
        public Point GetGridIndexPoint(double X, double Y)
        {
            Point point = new Point();
            double Xmod = X % MainWindow.GridCellSize;
            double Ymod = Y % MainWindow.GridCellSize;
            if (Xmod > MainWindow.GridCellSize / MainWindow.GridCellRectanglWidth)
                X = (X + MainWindow.GridCellSize - Xmod);
            if (Ymod > 0)
                Y = (Y + MainWindow.GridCellSize - Ymod);
            int column = (int)Math.Round(X / ((MainWindow.GridCellSize)));
            int row = (int)Math.Round(Y / (MainWindow.GridCellSize));
            if (column < 0 || row < 0)
            {
                MessageBox.Show("error point！！！！");
                return point;
            }


            point = _gridIndex[(column, row)];
            return point;

        }
        private void CreateGrid(int rows, int columns)
        {
            DesignCanvas.RowDefinitions.Clear();
            DesignCanvas.ColumnDefinitions.Clear();

            DesignCanvas.Width = MainWindow.GridCellSize * columns;
            DesignCanvas.Height = MainWindow.GridCellSize * rows;
            DesignCanvas.VerticalAlignment = VerticalAlignment.Center;
            _gridIndex = new Dictionary<(int, int), Point>();
            for (int i = 0; i < columns; i++)
            {
                DesignCanvas.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(MainWindow.GridCellSize) });
            }

            for (int i = 0; i < rows; i++)
            {
                DesignCanvas.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(MainWindow.GridCellSize) });
            }


            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    var point = new Point(MainWindow.GridCellSize * i, MainWindow.GridCellSize * j + 0.5* MainWindow.GridCellSize);
                    var key = (i, j);
                    _gridIndex[key] = point;

                }
            }
            AddGridRectangles(rows, columns);
        }

        private void AddGridRectangles(int rows, int columns)
        {
            for (int i = 0; i < columns; i += MainWindow.GridCellWidthRate)
            {
                for (int j = 0; j < rows; j += MainWindow.GridCellHeightRate)
                {
                    var rectangle = CreateGridRectangle();
                    rectangle.Height = MainWindow.GridCellRectangleHeight;
                    rectangle.Width = MainWindow.GridCellRectanglWidth;
                    //Ellipse linedot = new Ellipse()
                    //{
                    //    Fill = Brushes.Blue,
                    //    Width = 6,
                    //    Height= 6,
                    //    VerticalAlignment = VerticalAlignment.Top,
                    //    HorizontalAlignment = HorizontalAlignment.Left,
                    //};
                    //Grid.SetColumn(linedot, i);
                    //Grid.SetRow(linedot, j);
                    //DesignCanvas.Children.Add(linedot);
                    Grid.SetColumn(rectangle, i);
                    Grid.SetRow(rectangle, j);
                    DesignCanvas.Children.Add(rectangle);

                }
            }
        }

        private Rectangle CreateGridRectangle()
        {
            var rectangle = new Rectangle
            {
                Stroke = Brushes.Gray,
                StrokeThickness = 0.3,
                StrokeDashArray = new DoubleCollection { 2, 2 },
                Fill = Brushes.Transparent
            };
            Grid.SetRowSpan(rectangle, MainWindow.GridCellHeightRate);
            Grid.SetColumnSpan(rectangle, MainWindow.GridCellWidthRate);
            rectangle.MouseLeftButtonDown += Rectangle_MouseLeftButtonDown;
            rectangle.LostFocus += Rectangle_LostFocus;
            return rectangle;
        }
        private void Rectangle_LostFocus(object sender, RoutedEventArgs e)
        {
            Rectangle rectangle = sender as Rectangle;
            if (rectangle != null)
            {
                //// 恢复原来的边框样式
                //rectangle.Stroke = Brushes.Gray;
                //rectangle.StrokeThickness = 0.5;
                //rectangle.StrokeDashArray = new DoubleCollection { 2, 2 };
                //rectangle.Fill = Brushes.Transparent;

            }
        }

        private void UpdateRectangleStyle(Rectangle rectangle)
        {
            if (rectangle == null) return;

            // 恢复之前选中矩形的样式
            if (_selectedRectangle != null && _selectedRectangle != rectangle)
            {
                _selectedRectangle.Stroke = Brushes.Gray;
                _selectedRectangle.StrokeThickness = 0.5;
                _selectedRectangle.StrokeDashArray = new DoubleCollection { 2, 2 };
                _selectedRectangle.Fill = Brushes.Transparent;
            }

            // 设置当前选中矩形的样式
            rectangle.Stroke = Brushes.Blue;
            rectangle.StrokeThickness = 2;
            // 更新选中的矩形
            _selectedRectangle = rectangle;
        }

        private void Rectangle_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var clickedRectangle = sender as Rectangle;
            CurrentRectangeRow = Grid.GetRow(clickedRectangle);
            CurrentRectangeCloumn = Grid.GetColumn(clickedRectangle);
            UpdateRectangleStyle(clickedRectangle);
            //DesignCanvas.Focus();
            //AdjustRectangleSize(clickedRectangle, 4, 3);
            foreach (UIElement element in DesignCanvas.Children)
            {
                if (Grid.GetRow(element) == CurrentRectangeRow && Grid.GetColumn(element) == CurrentRectangeCloumn && element is PLCModuleControl)
                {
                    // PLC模块处理逻辑
                    return;
                }
            }
        }

        //左上角的点的坐标
        public Point GetRectangleLeftTopPoint()
        {
            var topLeft = _selectedRectangle.TranslatePoint(new Point(0, 0), DesignCanvas);
            topLeft.X = Math.Ceiling(topLeft.X);
            topLeft.Y = Math.Ceiling(topLeft.Y);
            Debug.WriteLine($"Top Left: {topLeft.X}, {topLeft.Y}");
            return topLeft;
        }


        public Point GetRectangleLeftHalfPoint()
        {
            var topLeft = _selectedRectangle.TranslatePoint(new Point(0, 0), DesignCanvas);
            topLeft.X = Math.Ceiling(topLeft.X);
            topLeft.Y = (MainWindow.GridCellRectangleHeight / 2 )+ Math.Ceiling(topLeft.Y);
            Debug.WriteLine($"Top Left: {topLeft.X}, {topLeft.Y}");
            return topLeft;
        }

        //顶部的水平线
        public (Point, Point) GetSelectRectangeTopHorizontalLine()
        {
            var topLeft = GetRectangleLeftTopPoint();
            var end = topLeft;
            end.X += MainWindow.GridCellRectanglWidth;
            return (topLeft, end);
        }

        //底部的水平线
        public (Point, Point) GetSelectRectangeBottomHorizontalLine()
        {
            var topLeft = GetRectangleBottomLeftPoint();
            var end = topLeft;
            end.X += MainWindow.GridCellRectanglWidth;
            return (topLeft, end);
        }

        //中点的水平线
        public (Point, Point) GetSelectRectangeHorizontalLine()
        {
            var topLeft = GetRectangleLeftTopPoint();
            topLeft.Y += 1.5* MainWindow.GridCellSize;
            var end = topLeft;
            end.X += MainWindow.GridCellRectanglWidth;
            return (topLeft, end);
        }

        //上部分的垂直线
        public (Point, Point) GetSelectRectangeTopVerticaLine()
        {
            var topLeft = GetRectangleLeftHalfPoint();
            var end = topLeft;
            end.Y -= MainWindow.GridCellRectangleHeight;
            return (topLeft, end);
        }

        //下部分的垂直线
        public (Point, Point) GetSelectRectangeBottomVerticaLine()
        {
            var topLeft = GetRectangleLeftHalfPoint();
            var end = topLeft;
            end.Y += MainWindow.GridCellRectangleHeight;
            return (topLeft, end);
        }

        public Point GetRectangleBottomLeftPoint()
        {
            var bottomLeft = _selectedRectangle.TranslatePoint(new Point(0, _selectedRectangle.ActualHeight), DesignCanvas);
            bottomLeft.X = Math.Ceiling(bottomLeft.X);
            bottomLeft.Y = Math.Ceiling(bottomLeft.Y);
            Debug.WriteLine($"Bottom Left: {bottomLeft.X}, {bottomLeft.Y}");
            return bottomLeft;
        }

        public Point GetRectangleRightTopPoint()
        {
            var topRight = _selectedRectangle.TranslatePoint(new Point(_selectedRectangle.ActualWidth, 0), DesignCanvas);
            Debug.WriteLine($"topRight : {topRight.X}, {topRight.Y}");
            return topRight;
        }
        public Point GetRectangleBottomRightPoint()
        {
            var bottomRight = _selectedRectangle.TranslatePoint(new Point(_selectedRectangle.ActualWidth, _selectedRectangle.ActualHeight), DesignCanvas);
            Debug.WriteLine($"topRight : {bottomRight.X}, {bottomRight.Y}");
            return bottomRight;
        }


        public Polyline DrawLine(Point startPoint, Point endPoint)
        {
            //添加输入
            Debug.WriteLine($"StartPoint {startPoint.X},{startPoint.Y} ");
            Debug.WriteLine($"EndPoint {endPoint.X},{endPoint.Y} ");

            double adjustX = endPoint.X - MainWindow.GridCellRectanglWidth;
            if (endPoint.X < startPoint.X)
                adjustX = endPoint.X + MainWindow.GridCellRectanglWidth;


            var adjustedEndPoint = new Point(adjustX, endPoint.Y);
            var path = aFind.FindPath(startPoint, endPoint);
            if (path != null && path.Count > 1)
            {
                //path.Insert(0, startPoint);
                //path.Add(strp);

                //删除和结束点x一样的点
                //path.Add(adjustedEndPoint);
                //path.RemoveAll(x => x.X == endPoint.X);
                //path.Add(endPoint);
                Polyline polyline = new Polyline
                {
                    Stroke = Brushes.Black,
                    StrokeThickness = 2
                };
                PointCollection points = new PointCollection(path);
                polyline.Points = points;
                return polyline;
            }
            return null;
        }


    }
}
