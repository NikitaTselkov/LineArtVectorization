using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LineArtVectorization.Models
{
    public class Skeletonization
    {
        public List<SkeletonCurve> PartialSkeletonization(byte[,] pixels)
        {
            var rleByteEncoder = new RLE<byte>();
            var skeletonCurves = new List<SkeletonCurve>();

            // вертикальное сканирование
            var MCC = rleByteEncoder.GetMCC(pixels, Direction.Vertical);
            CreateSkeletonCurves(skeletonCurves, MCC, Direction.Vertical);

            // горизонтальное сканирование 
            MCC = rleByteEncoder.GetMCC(pixels, Direction.Horizontal);
            CreateSkeletonCurves(skeletonCurves, MCC, Direction.Horizontal);

            // обработка областей соединений

            return skeletonCurves;

            void CreateSkeletonCurves(List<SkeletonCurve> skeletonCurves, IList<Series>[] MCC, Direction direction)
            {
                for (int x = 0; x < MCC.Length; x++)
                {
                    var verticalStrips = GetStrips(MCC[x]);
                    foreach (var strip in verticalStrips)
                    {
                        // замена серий в полосе на скелетную кривую 
                        var skeleton = BuildOptimizedCurve(strip, direction);

                        if (skeleton != null)
                            skeletonCurves.Add(skeleton);
                    }
                }
            }
        }

        private List<Strip> GetStrips(IList<Series> seriesList)
        {
            List<Strip> strips = new List<Strip>();

            List<Series> currentStrip = null;

            for (int i = 0; i < seriesList.Count; i++)
            {
                Series series = seriesList[i];

                if (currentStrip == null)
                {
                    // начало новой полосы
                    currentStrip = new List<Series> { series };
                }
                else if (series.IsAdjacent(currentStrip.Last())
                    && series.Begin == currentStrip.Last().Begin
                    && series.End == currentStrip.Last().End)
                {
                    // серия принадлежит текущей полосе
                    currentStrip.Add(series);
                }
                else
                {
                    // конец текущей полосы
                    strips.Add(new Strip(currentStrip));
                    currentStrip = new List<Series> { series };
                }
            }

            if (currentStrip != null)
            {
                // добавить последнюю незаконченную полосу
                strips.Add(new Strip(currentStrip));
            }

            return strips;
        }

        private SkeletonCurve BuildOptimizedCurve(Strip strip, Direction direction)
        {
            var curve = new SkeletonCurve();

            Series s1 = strip.Series.FirstOrDefault();
            Series s2 = strip.Series.LastOrDefault();

            if (s1 != null && s2 != null)
            {
                if (!strip.ShouldBeClosed())
                {
                    if (direction == Direction.Vertical)
                    {
                        curve.AddPoint((s1.Position + s2.Position) / 2, s1.Begin);
                        curve.AddPoint((s1.Position + s2.Position) / 2, s2.End);
                    }
                    else
                    {
                        curve.AddPoint(s1.Begin, (s1.Position + s2.Position) / 2);
                        curve.AddPoint(s2.End, (s1.Position + s2.Position) / 2);
                    }
                }
            }

            if (curve.Points.Length < 1)
                curve = null;

            return curve;
        }
    }
}
