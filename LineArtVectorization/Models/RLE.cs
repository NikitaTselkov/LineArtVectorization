using System;
using System.Collections.Generic;
using System.Linq;

namespace LineArtVectorization.Models
{
    public class RLE<T> : IDataCompression<T>
    {
        public O[] Compress<O>(T[] data)
        {
            var result = new List<O>();

            dynamic prevValue = null;
            int counter = 1;
            O prevValueT;
            O counterT;

            for (int i = 0; i <= data.Length; i++)
            {
                if (prevValue != null)
                {
                    counterT = (O)Convert.ChangeType(counter, typeof(O));
                    prevValueT = (O)prevValue;

                    if (i == data.Length)
                    {
                        result.Add(counterT);
                        result.Add(prevValueT);
                        break;
                    }

                    if (prevValue.Equals(data[i])) counter++;
                    else
                    {
                        result.Add(counterT);
                        result.Add(prevValueT);

                        counter = 1;
                    }
                }

                prevValue = data[i];
            }

            return result.ToArray<O>();
        }

        public T[] Decompress<O>(O[] data)
        {
            var result = new List<T>();

            for (int i = 0; i < data.Length; i += 2)
            {
                int count = (int)Convert.ChangeType(data[i], typeof(int));

                for (int j = 0; j < count; j++)
                {
                    result.Add((T)Convert.ChangeType(data[i + 1], typeof(T)));
                }
            }

            return result.ToArray();
        }

        public IList<Series>[] GetMCC(T[,] image)
        {
            int width = image.GetLength(0);
            int height = image.GetLength(1);

            var columns = new List<Series>[width];

            for (int i = 0; i < columns.Length; i++)
            {
                columns[i] = new List<Series>();
            }

            byte black = 1;

            for (int y = 0; y < height; y++)
            {
                int startX = -1;

                for (int x = 0; x < width; x++)
                {
                    byte pixel = Convert.ToByte(image[x, y]);

                    if (pixel == black && startX == -1)
                        startX = x;

                    if (pixel != black && startX != -1)
                    {
                        var series = new Series(Direction.Vertical, y, startX, x - 1);
                        columns[startX].Add(series);
                        startX = -1;
                    }
                }

                if (startX != -1)
                {
                    var series = new Series(Direction.Vertical, y, startX, width - 1);
                    columns[startX].Add(series);
                }
            }

            return columns;
        }
    }
}
