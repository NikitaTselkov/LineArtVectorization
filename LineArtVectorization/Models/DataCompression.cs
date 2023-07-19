using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LineArtVectorization.Models
{
    public class DataCompression<T> : IDataCompression<T>
    {
        public T[] EncodeRLE(T[] data)
        {
            var result = new List<T>();

            dynamic prevValue = null;
            int counter = 1;
            T prevValueT;
            T counterT;

            for (int i = 0; i <= data.Length; i++)
            {
                if (prevValue != null)
                {
                    counterT = (T)Convert.ChangeType(counter, typeof(T));
                    prevValueT = (T)prevValue;

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

            return result.ToArray<T>();
        }
        
        public T[] DecodeRLE(T[] data)
        {
            return null;
        }
    }
}
