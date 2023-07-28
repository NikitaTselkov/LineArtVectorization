using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace LineArtVectorization.Models.Data.Graph
{
    public class Node
    {
        public Point Point { get; }
        public List<Node> Connections { get; }

        public Node(Point point)
        {
            Point = point;
            Connections = new List<Node>();
        }

        public void Connect(Node node)
        {
            Connections.Add(node);
        }
    }
}
