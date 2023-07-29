using LineArtVectorization.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace LineArtVectorization.Models.Data
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

        public void AddPoint(Point point)
        {
            _points.Add(point);
        }

        public IList<Line> GetLines()
        {
            var result = new List<Line>();

            if (_points != null)
            {
                for (int i = 1; i < _points.Count; i++)
                {
                    result.Add(new Line
                    {
                        Start = new Point(_points[i - 1].X, _points[i - 1].Y),
                        End = new Point(_points[i].X, _points[i].Y)
                    });
                }
            }

            return result;
        }
    }
}
