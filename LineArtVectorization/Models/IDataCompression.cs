using System.Collections;
using System.Collections.Generic;

namespace LineArtVectorization.Models
{
    public interface IDataCompression<T>
    {
        public T[] EncodeRLE(T[] data);

        public T[] DecodeRLE(T[] data);
    }
}