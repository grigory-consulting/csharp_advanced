using System;
using System.Buffers;


class Program
{

    static void Main()
    {


    }


    static void HybridStackallocPool(int size)
    {
        byte[]? bytes = null;

        Span<byte> buf = size <= 1024
            ? stackalloc byte[size]
            : (bytes = ArrayPool<byte>.Shared.Rent(size)).AsSpan(0, size);

        try
        {
            for (int i = 0; i < size; i++)
            {
                buf[i] = (byte)i;   
            }
        }
        finally
        {
            if (bytes != null)
                ArrayPool<byte>.Shared.Return(bytes);
        }
    }
}
