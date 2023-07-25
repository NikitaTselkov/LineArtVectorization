using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LineArtVectorization.Models
{
    public class Skeletonization
    {
        public List<SkeletonCurve> PartialSkeletonization(IList<Series>[] mcc)
        {
            var skeletonCurves = new List<SkeletonCurve>();

            // вертикальное сканирование
            for (int x = 0; x < mcc.Length; x++)
            {
                var verticalStrips = GetStrips(mcc[x]);
                foreach (var strip in verticalStrips)
                {
                    // замена серий в полосе на скелетную кривую 
                    var skeleton = BuildOptimizedCurve(strip);

                    if(skeleton != null)
                        skeletonCurves.Add(skeleton);
                }
            }

            // горизонтальное сканирование 
            // аналогично, только сканирование по строкам

            // обработка областей соединений

            return skeletonCurves;
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
                else if (series.IsAdjacent(currentStrip.First()))
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

        private SkeletonCurve BuildOptimizedCurve(Strip strip)
        {
            var curve = new SkeletonCurve();

            // Проходим с шагом по 2 серии для проверки Y
            for (int i = 1; i < strip.Series.Count; i += 2)
            {
                Series s1 = strip.Series[i];
                Series s2 = strip.Series[i - 1];

                // Проверка ограничения по Y
                if (Math.Abs(s1.Position - s2.Position) > 1)
                    break;

                // Добавляем средние точки обеих серий  
                curve.AddPoint((s1.Position + s2.Position) / 2, (s1.Begin + s2.Begin) / 2);
                curve.AddPoint((s1.Position + s2.Position) / 2, (s1.End + s2.End) / 2);
            }

            if (curve.Points.Length < 1)
                curve = null;

            // Упрощаем кривую
            curve?.Simplify();

            return curve;
        }
    }
}
