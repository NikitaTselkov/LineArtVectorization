using System.Collections.Generic;
using System.Linq;
using LineArtVectorization.Models.Data;

namespace LineArtVectorization.Models.Utils
{
    public class Skeletonization
    {
        public List<SkeletonCurve> PartialSkeletonization(byte[,] pixels)
        {
            var rleByteEncoder = new RLE<byte>();
            var skeletonCurves = new List<SkeletonCurve>();
            var strips = new List<Strip>();

            // вертикальное сканирование
            var MCC = rleByteEncoder.GetMCC(pixels, Direction.Vertical);
            strips.AddRange(GetStrips(MCC));

            // горизонтальное сканирование 
            MCC = rleByteEncoder.GetMCC(pixels, Direction.Horizontal);
            strips.AddRange(GetStrips(MCC));

            // обработка областей соединений

            foreach (var strip in strips)
            {
                // замена серий в полосе на скелетную кривую 
                var skeleton = BuildOptimizedCurve(strip);

                if (skeleton != null)
                    skeletonCurves.Add(skeleton);
            }

            return skeletonCurves;
        }

        private List<Strip> GetStrips(Series[,] seriesList)
        {
            HashSet<Series> branches = new();
            List<Strip> strips = new();

            for (int i = 0; i < seriesList.GetLength(0); i++)
            {
                for (int j = 0; j < seriesList.GetLength(1); j++)
                {
                    Series series = seriesList[i, j];

                    if (series == null) continue;

                    var strip = strips?.Where(w => w.GetLastSeries().IsAdjacent(series)).ToList();

                    List<Series> p = GetNearStrip(seriesList, i - 1, series);
                    List<Series> c = GetNearStrip(seriesList, i + 1, series);

                    if (branches.Contains(series))
                    {
                        strips.Add(new Strip(series));
                        branches.Remove(series);
                    }
                    else if (strip?.Count == 0)
                    {
                        strips.Add(new Strip(series));
                    }
                    else if (strip?.Count == 1)
                    {
                        if (p.Count > 1)
                            continue;

                        if (c.Count > 1)
                            c.ForEach(f => branches.Add(f));

                        strip.Single().AddSeries(series);
                    }
                }
            }

            for (int i = 0; i < strips.Count; i++)
            {
                if (!strips[i].ShouldBeClosed())
                    strips[i] = null;
            }

            return strips;

            List<Series> GetNearStrip(Series[,] seriesList, int index, Series series)
            {
                var result = new List<Series>();
                Series s;

                if (index > 0 && index < seriesList.GetLength(0))
                {
                    for (int k = 0; k < seriesList.GetLength(1); k++)
                    {
                        s = seriesList[index, k];

                        if (series.IsAdjacent(s))
                            result.Add(s);
                    }
                }

                return result;
            }
        }

        //private SkeletonCurve BuildOptimizedCurve(Strip strip)
        //{
        //    if (strip == null) return null;

        //    var curve = new SkeletonCurve();
        //    Series s = null;
        //    Series s2 = null;

        //    for (int i = 1; i < strip?.Length; i++)
        //    {
        //        s = strip.Series[i - 1];
        //        s2 = strip.Series[i];

        //        if (i == 1)
        //            curve.AddPoint(s.Direction == Direction.Vertical ? new Point(s.Position, (s.Begin + s.End) / 2) : new Point((s.Begin + s.End) / 2, s.Position));

        //        // Если направление изменилось
        //        if (s.Direction != s2.Direction || s.IsAdjacent(s2) && s.Begin != s2.Begin && s.End != s2.End)
        //            curve.AddPoint(s.Direction == Direction.Vertical ? new Point(s.Position, (s.Begin + s.End) / 2) : new Point((s.Begin + s.End) / 2, s.Position));
        //    }

        //    if (curve.Points.Length % 2 != 0)
        //        curve.AddPoint(s.Direction == Direction.Vertical ? new Point(s.Position, (s.Begin + s.End) / 2) : new Point((s.Begin + s.End) / 2, s.Position));

        //    return curve;
        //}

        private SkeletonCurve BuildOptimizedCurve(Strip strip)
        {
            var curve = new SkeletonCurve();

            for (int i = 0; i < strip?.Length; i++)
            {
                var s = strip.Series[i];

                if (s.Direction == Direction.Vertical)
                {
                    curve.AddPoint(s.Position, (s.Begin + s.End) / 2);
                }
                else
                {
                    curve.AddPoint((s.Begin + s.End) / 2, s.Position);
                }
            }

            if (curve.Points.Length < 3)
                curve = null;

            return curve;
        }
    }
}
