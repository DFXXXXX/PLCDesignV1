using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
namespace PLCDesignV1
{
    public class AFindRoute
    {
        public int GridCellWidth { get; set; }
        public int GridCellHeight { get; set; }
        private Dictionary<(int, int), Point> _gridIndex { get; set; }

        private PlcModuleManager _plcModuleManager;

        public AFindRoute(int gridCellWidth, int gridCellHeight, Dictionary<(int, int), Point> pairs,PlcModuleManager plcModuleManager)
        {
            GridCellWidth = gridCellWidth;
            GridCellHeight = gridCellHeight;
            _gridIndex = pairs;
            this._plcModuleManager = plcModuleManager;
        }


  
        public List<Point> FindPath(Point start, Point end)
        {
            var openList = new List<Node>();
            var closedList = new HashSet<Point>();
            var startNode = new Node(start);
            var endNode = new Node(end);
            openList.Add(startNode);

            while (openList.Count > 0)
            {
                var currentNode = openList.OrderBy(node => node.F).First();
                if (currentNode.Position == endNode.Position)
                {
                    var path = ReconstructPath(currentNode);
                    return path;
                }

                openList.Remove(currentNode);
                closedList.Add(currentNode.Position);

                foreach (var neighbor in GetNeighbors(currentNode))
                {
                    if (closedList.Contains(neighbor.Position))
                    {
                        continue;
                    }

                    var tentativeG = currentNode.G + GetDistance(currentNode.Position, neighbor.Position);
                    var existingNode = openList.FirstOrDefault(node => node.Position == neighbor.Position);

                    if (existingNode == null)
                    {
                        neighbor.G = tentativeG;
                        neighbor.H = GetDistance(neighbor.Position, endNode.Position);
                        openList.Add(neighbor);
                    }
                    else if (tentativeG < existingNode.G)
                    {
                        existingNode.G = tentativeG;
                        existingNode.Parent = currentNode;
                    }
                }
            }

            return null; // 未找到路径
        }

        private List<Point> ReconstructPath(Node node)
        {
            var path = new List<Point>();
            while (node != null)
            {
                path.Add(node.Position);
                node = node.Parent;
            }
            path.Reverse();
            var modules = GetFindPathPLCModule(path[0], path[path.Count-1]);
            for (int i = 1; i < path.Count -2; i++)
            {
                var result = IsLineIntersectingModule(path[i], path[i + 1], modules);
                if (result) 
                {
                    return null;
                }
            }
            return path;
        }

        private Dictionary<(int, int), PLCModuleControl> GetFindPathPLCModule(Point start, Point end)
        {
            Dictionary<(int, int), PLCModuleControl> container = new Dictionary<(int, int), PLCModuleControl>();
            var modules = _plcModuleManager.GetModule();
            foreach (var obj in modules)
            {
                var point = obj.Key;
                var module = obj.Value;
                Rect Rect = new Rect(point.Item1 * MainWindow.GridCellSize,
                                    point.Item2 * MainWindow.GridCellSize,
                                    module.CalculatedColumnSpan * MainWindow.GridCellSize,
                                    module.CalculatedRowSpan * MainWindow.GridCellSize);
                if (Rect.Contains(start))
                {
                    continue;
                }
                if (Rect.Contains(end))
                {
                    continue;
                }

                container.Add(point, module);
            }
            return container;

        }

        private IEnumerable<Node> GetNeighbors(Node node)
        {
            var directions = new List<Point>
            {
                new Point(-GridCellWidth, 0),  // 左
                new Point(GridCellWidth, 0),   // 右
                new Point(0, -GridCellHeight), // 上
                new Point(0, GridCellHeight)   // 下
            };

            foreach (var direction in directions)
            {
                var neighborPosition = new Point(node.Position.X + direction.X, node.Position.Y + direction.Y);
                if (_gridIndex.Values.Contains(neighborPosition))
                {
                    yield return new Node(neighborPosition, node);
                }
            }
        }

   

        private List<Point> RDP(List<Point> points, double epsilon)
        {
            if (points.Count < 3)
                return points;

            int index = -1;
            double dmax = 0.0;
            for (int i = 1; i < points.Count - 1; i++)
            {
                double d = PerpendicularDistance(points[i], points[0], points[points.Count - 1]);
                if (d > dmax)
                {
                    dmax = d;
                    index = i;
                }
            }

            List<Point> resultList;
            if (dmax > epsilon)
            {
                var firstSplit = points.Take(index + 1).ToList();
                var secondSplit = points.Skip(index).ToList();

                var firstResult = RDP(firstSplit, epsilon);
                var secondResult = RDP(secondSplit, epsilon);

                resultList = firstResult.Take(firstResult.Count - 1).Concat(secondResult).ToList();
            }
            else
            {
                resultList = new List<Point> { points.First(), points.Last() };
            }

            return resultList;
        }

        private double PerpendicularDistance(Point p, Point lineStart, Point lineEnd)
        {
            double area = Math.Abs(0.5 * (lineStart.X * lineEnd.Y + lineEnd.X * p.Y + p.X * lineStart.Y
                                           - lineEnd.X * lineStart.Y - p.X * lineEnd.Y - lineStart.X * p.Y));
            double bottom = Math.Sqrt(Math.Pow(lineStart.X - lineEnd.X, 2) + Math.Pow(lineStart.Y - lineEnd.Y, 2));
            return area / bottom * 2;
        }


        private double GetDistance(Point a, Point b)
        {
            double dx = Math.Abs(a.X - b.X) / GridCellWidth;
            double dy = Math.Abs(a.Y - b.Y) / GridCellHeight;
            return dx + dy; // 曼哈顿距离
        }



        public bool IsLineIntersectingModule(Point start, Point end, Dictionary<(int, int), PLCModuleControl> modules)
        {
            if(modules==null)
                return false;
            foreach (var modulePair in modules)
            {
                var point = modulePair.Key;
                var module = modulePair.Value;

                // 获取模块的矩形区域
                Rect moduleRect = new Rect(point.Item1 * MainWindow.GridCellSize,
                                           point.Item2 * MainWindow.GridCellSize,
                                           module.CalculatedColumnSpan * MainWindow.GridCellSize,
                                           module.CalculatedRowSpan * MainWindow.GridCellSize);
                // 检查该模块是否是起点或终点所在的模块，如果是则跳过
      
                // 检查线段是否与矩形相交
                if (IsLineIntersectingRectangle(start, end, moduleRect))
                {
                    return true; // 线段与某个模块相交
                }
            }
            return false;
        }
        // 检查线段是否与矩形相交
        private bool IsLineIntersectingRectangle(Point start, Point end, Rect rect)
        {
            // 检查线段的起点或终点是否在矩形内部
            if (rect.Contains(start) || rect.Contains(end))
            {
                return true;
            }

            // 定义矩形的四条边
            Point topLeft = new Point(rect.Left, rect.Top);
            Point topRight = new Point(rect.Right, rect.Top);
            Point bottomLeft = new Point(rect.Left, rect.Bottom);
            Point bottomRight = new Point(rect.Right, rect.Bottom);

            // 检查线段是否与矩形的四条边相交
            var top = DoLinesIntersect(start, end, topLeft, topRight);   // 上边
         
          
            var right = DoLinesIntersect(start, end, topRight, bottomRight);  //        右边
            var bottom = DoLinesIntersect(start, end, bottomRight, bottomLeft);      // 下边
            var left = DoLinesIntersect(start, end, bottomLeft, topLeft);  //             左边
            bool result = top || right || bottom || left;
            return result;
        }

        // 线段相交检查的辅助方法
        private bool DoLinesIntersect(Point p1, Point p2, Point p3, Point p4)
        {
            double d1 = CrossProduct(p1, p2, p3);
            double d2 = CrossProduct(p1, p2, p4);
            double d3 = CrossProduct(p3, p4, p1);
            double d4 = CrossProduct(p3, p4, p2);

            // 判断线段是否相交
            return (d1 * d2 < 0 && d3 * d4 < 0);
        }

        // 计算叉积（判断方向）
        private double CrossProduct(Point a, Point b, Point c)
        {
            return (b.X - a.X) * (c.Y - a.Y) - (b.Y - a.Y) * (c.X - a.X);
        }


    }
}
