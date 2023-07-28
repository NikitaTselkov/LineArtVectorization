using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LineArtVectorization.Models.Data
{
    public class Series
    {
        public Direction Direction { get; init; }

        public int Position { get; init; }

        public int Begin { get; init; }

        public int End { get; init; }

        public Series(Direction direction, int position, int begin, int end)
        {
            Direction = direction;
            Position = position;
            Begin = begin;
            End = end;
        }

        public bool IsAdjacent(Series series)
        {
            if(series == null)
                return false;

            if (Direction != series.Direction)
                return false;

            if (Math.Abs(Position - series.Position) != 1)
                return false;

            if (Begin > series.End + 1)
                return false;

            if (End < series.Begin - 1)
                return false;

            return true;
        }
    }
}

