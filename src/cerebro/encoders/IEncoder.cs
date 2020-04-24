using System.Collections.Generic;
namespace cerebro.encoders
{
    using cerebro.math;

    public interface IEncoder<T>
    {
        SparseBitArray Encode(T input);
    }
}