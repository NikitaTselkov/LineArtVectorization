using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;

namespace LineArtVectorization.Models.Data.Graph
{
    public class Graph
    {

        public List<Node> Nodes { get; }

        public Graph()
        {
            Nodes = new List<Node>();
        }

        public Node GetOrCreateNode(Point point)
        {
            // ищем по точке
            var existingNode = Nodes.FirstOrDefault(n => n.Point == point);

            if (existingNode != null)
                return existingNode;

            // не нашли - создаем новую
            var newNode = new Node(point);

            AddNode(newNode);

            return newNode;
        }

        public void AddNode(Node node)
        {
            if (!Nodes.Contains(node))
            {
                Nodes.Add(node);
            }
        }
    }
}
