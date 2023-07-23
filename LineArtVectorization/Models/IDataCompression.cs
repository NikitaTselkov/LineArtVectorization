﻿using System.Collections;
using System.Collections.Generic;

namespace LineArtVectorization.Models
{
    public interface IDataCompression<T>
    {
        public O[] Compress<O>(T[] data);

        public T[] Decompress<O>(O[] data);
    }
}