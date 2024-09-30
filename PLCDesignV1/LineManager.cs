using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace PLCDesignV1
{
    public class LineManager<T, V> where T : Shape where V : Shape
    {
        public Dictionary<(int, int), (V ellipse, int count)> _EllipseDic { get; set; }
        public HashSet<T> _lines { get; set; }

        public event Action<Point> EllipseClicked;
        private Canvas _lineCanvas { get; set; }

        public Point SelectPoint { get; set; }
        private Grid _gridCanvas { get; set; }
        public LineManager(Canvas canvas,Grid gridCanvas)
        {
            _lines = new HashSet<T>() ;
            _lineCanvas = canvas;
            _EllipseDic = new Dictionary<(int, int), (V ellipse, int count)>();
            _gridCanvas = gridCanvas;
        }
        public HashSet<T> GetLines()
        {
            return _lines;
        }
        public void RemoveLine(T polyline)
        {
            _lines.Remove(polyline);
            _lineCanvas.Children.Remove(polyline);
            RemoveLineEllipses(polyline);
        }
        private void RemoveLineEllipses(T polyline)
        {
            var polyline1 = polyline as Polyline;
            foreach (var point in polyline1.Points)
            {
                (int, int) po = ((int)point.X, (int)point.Y);
                if (_EllipseDic.ContainsKey(po))
                {
                    _EllipseDic[po] = (_EllipseDic[po].ellipse, _EllipseDic[po].count - 1);
                    if (_EllipseDic[po].count == 0)
                    {
                        _gridCanvas.Children.Remove(_EllipseDic[po].ellipse);
                        _EllipseDic.Remove(po);
                    }
                }
            }
       
        }
        private void AddLineEllipses(T polyline)
        {
            var p = polyline as Polyline;
            foreach (var point in p.Points)
            {
                (int, int) po = ((int)point.X, (int)point.Y);
                if (point.X < 0 || point.Y < 0)
                    break;
                if (!_EllipseDic.ContainsKey(po))
                {
                    Ellipse dot = new Ellipse
                    {
                        Width = 6,
                        Height = 6,
                        Fill = Brushes.Blue,
                        Visibility = Visibility.Hidden,
                        IsHitTestVisible = true,
                    
                        HorizontalAlignment = HorizontalAlignment.Left,
                        VerticalAlignment = VerticalAlignment.Center
                        
                    };
                    dot.Tag = po;
                    _EllipseDic[po] = (dot as V, 1); // Explicitly cast to V and set count to 1
                    _gridCanvas.Children.Add(dot);
                    dot.MouseLeftButtonDown += Ellipse_MouseLeftButtonDown;
                    dot.MouseEnter += Dot_MouseEnter;
                    dot.MouseLeave += Dot_MouseLeave;

                    

                    Grid.SetColumn(dot, (int)point.X/MainWindow.GridCellSize);
                    Grid.SetRow(dot, (int)point.Y / MainWindow.GridCellSize);
                    //Canvas.SetLeft(dot, point.X);
                    //Canvas.SetTop(dot, point.Y);
                }
                else
                {
                    _EllipseDic[po] = (_EllipseDic[po].ellipse, _EllipseDic[po].count + 1);
                }
            }
        }


        public void Ellipse_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Debug.WriteLine("Ellipse clicked");
            if (sender is Ellipse ellipse && ellipse.Tag is ValueTuple<int, int> po)
            {
                Point point = new Point(po.Item1, po.Item2);
                EllipseClicked?.Invoke(point);
            }
           
        }

        public void AddLine(T polyline)
        {
            _lines.Add(polyline);
            _lineCanvas.Children.Add(polyline);
            AddLineEllipses(polyline);
        }

        public void Dot_MouseEnter(object sender, MouseEventArgs e)
        {
            Debug.WriteLine("Dot_MouseLeave");
            if (sender is Ellipse dot)
            {
                dot.Fill = Brushes.Yellow; // Set the fill color to yellow for highlighting
            }
        }

        public void ToggleEllipsesHidden()
        {
            //Canvas.SetZIndex(_lineCanvas, _lineCanvas_ZIndex);
            foreach (var ellipse in _EllipseDic.Values)
            {
              
                ellipse.ellipse.Visibility =  Visibility.Hidden ;
            }
        }

        private int _lineCanvas_ZIndex;
        public void ToggleEllipsesVisibility()
        {

            //_lineCanvas_ZIndex = Canvas.GetZIndex(_lineCanvas);
            //Canvas.SetZIndex(_lineCanvas, 100);
            foreach (var ellipse in _EllipseDic.Values)
            {

                ellipse.ellipse.Visibility = Visibility.Visible;
               
            }

        }

        public void Dot_MouseLeave(object sender, MouseEventArgs e)
        {
            Debug.WriteLine("Dot_MouseLeave");
            if (sender is Ellipse dot)
            { 
                dot.Fill =  Brushes.Blue; // Restore the original fill color
            }
        }

    }
}
