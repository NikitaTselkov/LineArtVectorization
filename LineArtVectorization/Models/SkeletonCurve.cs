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

        // алгоритм упрощения кривой
        public void Simplify()
        {
            if (_points.Count < 2) return;
          
            var x1 = _points.Where((p, i) => i % 2 == 0).Select(p => new { p.X, p.Y }).Average(p => p.X);
            var y1 = _points.Where((p, i) => i % 2 == 0).Select(p => new { p.X, p.Y }).Average(p => p.Y);
            
            var x2 = _points.Where((p, i) => i % 2 != 0).Select(p => new { p.X, p.Y }).Average(p => p.X);
            var y2 = _points.Where((p, i) => i % 2 != 0).Select(p => new { p.X, p.Y }).Average(p => p.Y);

            _points = new List<Point> { new Point(x1, y1), new Point(x2, y2) };
        }
    }
}
