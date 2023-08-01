using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LineArtVectorization.Models.Data.ConnectedComponents
{
    public class Strip : IConnectedComponent
    {
        private List<Series> _series;
        public IReadOnlyList<Series> Series => _series;


        public int Length => Series.Count;

        public int Weight => Series.Sum(s => s.End - s.Begin + 1);


        public Strip(List<Series> series)
        {
            _series = series;
        }

        public Strip(Series series)
        {
            _series = new List<Series>() { series };
        }


        public bool Contains(Series series)
        {
            return Series.Contains(series);
        }

        public void AddSeries(Series series)
        {
            _series.Add(series);
        }
        public Series GetFirstSeries()
        {
            return Series.FirstOrDefault();
        }

        public Series GetLastSeries()
        {
            return Series.LastOrDefault();
        }

        public bool ShouldBeClosed()
        {
            return Length * Length > Weight;
        }
    }
}
