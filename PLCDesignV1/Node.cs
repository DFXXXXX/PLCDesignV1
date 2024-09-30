using System.Windows;

namespace PLCDesignV1
{
    public class Node
    {
        public Point Position { get; set; }
        public Node Parent { get; set; }
        public double G { get; set; } // 从起点到当前节点的代价
        public double H { get; set; } // 从当前节点到终点的估计代价
        public double F => G + H; // 总代价

        public Node(Point position, Node parent = null)
        {
            Position = position;
            Parent = parent;
        }
    }
}
