using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LineArtVectorization.Models
{
    public class Series
    {
        public Direction Direction { get; init; }

        public int Position { get; init; }

        public int Begin { get; init; }

        public int End { get; init; }

        public Series(Direction direction, int columnOrRowNumber, int firstPixelNumber, int lastPixelNumber)
        {
            Direction = direction;
            Position = columnOrRowNumber;
            Begin = firstPixelNumber;
            End = lastPixelNumber;
        }

        public bool IsAdjacent(Series series)
        {
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

