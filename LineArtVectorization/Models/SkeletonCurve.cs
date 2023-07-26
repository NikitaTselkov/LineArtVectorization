using LineArtVectorization.Core;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace LineArtVectorization.Models
{
    public class SkeletonCurve
    {
        private List<Point> _points;

        public Point[] Points => _points.ToArray();

        public SkeletonCurve()
        {
            _points = new List<Point>();
        }

        public void AddPoint(int x, int y)
        {
            _points.Add(new Point(x, y));
        }

        public Line GetLine()
        {
            if(_points != null) return new Line
            {
                Start = new Point(_points.First().X, _points.First().Y),
                End = new Point(_points.Last().X, _points.Last().Y)
            };

            return null;
        }
    }
}
