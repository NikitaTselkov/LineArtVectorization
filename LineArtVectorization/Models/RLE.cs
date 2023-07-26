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

        public IList<Series>[] GetMCC(T[,] image, Direction direction)
        {
            int width = image.GetLength(0);
            int height = image.GetLength(1);

            var columnsOrRows = new List<Series>[direction == Direction.Vertical ? width : height];

            for (int i = 0; i < columnsOrRows.Length; i++)
            {
                columnsOrRows[i] = new List<Series>();
            }

            byte black = 1;

            if (direction == Direction.Vertical)
            {
                for (int y = 0; y < height; y++)
                {
                    ProcessPixels(image, width, y, direction);
                }
            }
            else
            {
                for (int x = 0; x < width; x++)
                {
                    ProcessPixels(image, height, x, direction);
                }
            }

            return columnsOrRows;

            void ProcessPixels(T[,] image, int start, int i, Direction direction)
            {
                int startPos = -1;
                byte pixel;

                for (int j = 0; j < start; j++)
                {
                    if (direction == Direction.Vertical)
                        pixel = Convert.ToByte(image[j, i]);
                    else
                        pixel = Convert.ToByte(image[i, j]);

                    if (pixel == black && startPos == -1)
                        startPos = j;

                    if (pixel != black && startPos != -1)
                    {
                        var series = new Series(direction, i, startPos, j - 1);
                        columnsOrRows[startPos].Add(series);
                        startPos = -1;
                    }
                }

                if (startPos != -1)
                {
                    var series = new Series(direction, i, startPos, start - 1);
                    columnsOrRows[startPos].Add(series);
                }
            }
        }
    }
}
