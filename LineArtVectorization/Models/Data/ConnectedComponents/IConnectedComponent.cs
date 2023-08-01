using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LineArtVectorization.Models.Data.ConnectedComponents
{
    public interface IConnectedComponent
    {
        public int Length { get; }

        public int Weight { get; }

        public IReadOnlyList<Series> Series { get; }

        public void AddSeries(Series series);
    }
}
