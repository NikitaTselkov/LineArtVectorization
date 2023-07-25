using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LineArtVectorization.Models
{
    public class Strip
    {
        public List<Series> Series { get; set; }

        public int Length => Series.Count;

        public int Weight => Series.Sum(s => s.End - s.Begin + 1);

        public Strip(List<Series> series)
        {
            Series = series;
        }

        public bool ShouldBeClosed()
        {
            return Length * Length > Weight;
        }

    }
}
