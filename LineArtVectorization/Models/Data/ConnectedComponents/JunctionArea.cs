using LineArtVectorization.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace LineArtVectorization.Models.Data.ConnectedComponents
{
    public class JunctionArea : IConnectedComponent
    {
        public int X { get; }
        public int Y { get; }

        public int Weight { get; }
        public int Length { get; }

        private List<Strip> _parents;
        public IReadOnlyList<Strip> Parents => _parents;

        private List<Strip> _childrens;
        public IReadOnlyList<Strip> Childrens => _childrens;

        private List<Series> _series { get; set; }
        public IReadOnlyList<Series> Series => _series;

        public JunctionArea(Series series, List<Strip> parents = null, List<Strip> childrens = null)
        {
            _series = new List<Series>() { series };
            _parents = parents ?? new List<Strip>();
            _childrens = childrens ?? new List<Strip>();
        }

        public void AddSeries(Series series)
        {
            if (series != null)
                _series.Add(series);
        }

        public void AddParent(Strip parent)
        {
            if (parent != null)
                _parents.Add(parent);
        }

        public void AddChildren(Strip children)
        {
            if (children != null)
                _childrens.Add(children);
        }

        public Point GetIntersectionPoint()
        {
            // логика нахождения точки пересечения
            return new Point();
        }
    }
}
