using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            List<T> result = new List<T>();

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
    }
}
