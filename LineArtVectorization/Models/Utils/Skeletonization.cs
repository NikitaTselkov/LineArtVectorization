using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using LineArtVectorization.Core;
using LineArtVectorization.Models.Data;
using LineArtVectorization.Models.Data.ConnectedComponents;
using LineArtVectorization.Models.Data.Enums;
using LineArtVectorization.Models.Utils.Helpers;

namespace LineArtVectorization.Models.Utils
{
    public class Skeletonization
    {
        public async Task<List<SkeletonCurve>> PartialSkeletonization(byte[,] pixels)
        {
            var rleByteEncoder = new RLE<byte>();
            var skeletonCurves = new List<SkeletonCurve>();
            var components = new List<IConnectedComponent>();

            // вертикальное сканирование
            var verticalTask = Task.Run(() =>
            {
                var verticalMCC = rleByteEncoder.GetMCC(pixels, Direction.Vertical);
                return GetConnectedComponents(verticalMCC);
            });

            // горизонтальное сканирование 
            var horizontalTask = Task.Run(() =>
            {
                var horizontalMCC = rleByteEncoder.GetMCC(pixels, Direction.Horizontal);
                return GetConnectedComponents(horizontalMCC);
            });

            verticalTask.Await();
            horizontalTask.Await();

            components.AddRange(verticalTask.Result);
            components.AddRange(horizontalTask.Result);

            // обработка связных компонентов
            foreach (var component in components)
            {
                if (component is Strip strip)
                {
                    SkeletonCurve skeleton = BuildCurveFromStrip(strip);

                    if (skeleton != null)
                        skeletonCurves.Add(skeleton);
                }

                else if (component is JunctionArea junction)
                {
                     var skeletons = BuildCurveFromJunctionArea(junction);

                    if (skeletons != null)
                        skeletonCurves.AddRange(skeletons.Where(f => f != null));
                }
            }

            return skeletonCurves;
        }

        private async Task<List<IConnectedComponent>> GetConnectedComponents(Series[,] seriesList)
        {
            HashSet<Series> branches = new();
            List<IConnectedComponent> connectedComponents = new();

            for (int i = 0; i < seriesList.GetLength(0); i++)
            {
                for (int j = 0; j < seriesList.GetLength(1); j++)
                {
                    Series series = seriesList[i, j];

                    if (series != null)
                    {
                        var components = connectedComponents?.Where(w => w != null && series.IsAdjacent(w.Series.Last())).ToList();

                        List<Series> p = await Task.Run(() => GetNearStrip(seriesList, i - 1, series));
                        List<Series> c = await Task.Run(() => GetNearStrip(seriesList, i + 1, series));

                        if (c.Count > 1)
                        {
                            connectedComponents.Add(new JunctionArea(series));
                            c.ForEach(f => branches.Add(f));
                        }

                        if (branches.Contains(series))
                        {
                            var strip = new Strip(series);
                            connectedComponents.Add(strip);
                            branches.Remove(series);

                            var jArea = connectedComponents.LastOrDefault(l => l is JunctionArea) as JunctionArea;
                            jArea.AddChildren(strip);
                        }
                        else if (components?.Count() == 0)
                        {
                            connectedComponents.Add(new Strip(series));
                        }
                        else if (components?.Count() == 1)
                        {
                            components.Single().AddSeries(series);
                        }

                        if (p.Count > 1)
                        {
                            var junctionArea = new JunctionArea(series);
                            foreach (var item in p)
                            {
                                var parent = connectedComponents.Single(w => w.Series.LastOrDefault(l => l == item) != null) as Strip;
                                junctionArea.AddParent(parent);
                            }

                            connectedComponents.Add(junctionArea);
                        }
                    }
                }
            }

            for (int i = 0; i < connectedComponents.Count; i++)
            {
                if(connectedComponents[i] is Strip strip && !strip.ShouldBeClosed())
                    connectedComponents[i] = null;
            }

            return connectedComponents;

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

        private List<SkeletonCurve> BuildCurveFromJunctionArea(JunctionArea junction)
        {
            var parents = junction.Parents;
            var childrens = junction.Childrens;

            foreach (var child in childrens)
            {
                var t = CalculateVectorDirection(child);
            }

            foreach (var parent in parents)
            {
                var t = CalculateVectorDirection(parent);
            }


            return null;
        }

        private static Vector CalculateVectorDirection(Strip child)
        {
            var points = ConvertStripToPoints(child);
            var vector = VectorHelper.GetVectorDirectionFromPoints(points);
            var direction = VectorHelper.GetDirection(vector);

            return vector;
        }

        private static SkeletonCurve BuildCurveFromStrip(Strip strip)
        {
            var curve = new SkeletonCurve();

            curve.SetPoints(ConvertStripToPoints(strip));

            return curve.Points.Length > 2 ? curve : null;
        }

        private static List<Point> ConvertStripToPoints(Strip strip)
        {
            var points = new List<Point>();

            foreach (var series in strip.Series)
            {
                if (series.Direction == Direction.Vertical)
                {
                    points.Add(new Point(series.Position, (series.Begin + series.End) / 2));
                }
                else
                {
                    points.Add(new Point((series.Begin + series.End) / 2, series.Position));
                }
            }

            return points;
        }
    }
}
