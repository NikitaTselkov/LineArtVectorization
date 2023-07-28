using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;
using System.Xml.Linq;
using LineArtVectorization.Models.Data;
using LineArtVectorization.Models.Data.Graph;

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
            List<Strip> strips = new();
            Strip currentStrip = null;

            for (int i = 0; i < seriesList.GetLength(0); i++)
            { 
                for (int j = 0; j < seriesList.GetLength(1); j++)
                {
                    Series series = seriesList[i, j];

                    if (series == null) continue;

                    var strip = strips?.Where(w => w.GetLastSeries().IsAdjacent(series)).ToList();

                    if (strip?.Count() == 1)
                    {
                        strip.Single().AddSeries(series);
                    }
                    else
                    {
                        strips.Add(currentStrip = new Strip(series));
                    }
                }
            }

            // добавить последнюю незаконченную полосу
            //if (currentStrip != null && !currentStrip.ShouldBeClosed())
            //    strips.Add(currentStrip);
            
            return strips;
        }

        private SkeletonCurve BuildOptimizedCurve(Strip strip)
        {
            var curve = new SkeletonCurve();

            Series s = strip.GetFirstSeries();

            if (s != null)
            {
                for (int i = 0; i < strip.Length; i++)
                {
                    s = strip.Series[i];
                    curve.AddPoint(s.Position, (s.Begin + s.End) / 2);
                }

                //if (s1.Direction == Direction.Vertical)
                //{
                //    curve.AddPoint((s1.Position + s2.Position) / 2, s1.Begin);
                //    curve.AddPoint((s1.Position + s2.Position) / 2, s2.End);
                //}
                //else
                //{
                //    curve.AddPoint(s1.Begin, (s1.Position + s2.Position) / 2);
                //    curve.AddPoint(s2.End, (s1.Position + s2.Position) / 2);
                //}
            }

            if (curve.Points.Length < 2)
                curve = null;

            return curve;
        }
    }
}
