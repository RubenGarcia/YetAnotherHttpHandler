using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Cysharp.Net.Http
{
    internal static class ThrowHelper
    {
        [Conditional("__VERIFY_POINTER")]
        public static unsafe void VerifyPointer(YahaNativeContext* ctx, YahaNativeRequestContext* reqCtx)
        {
            if (ctx == null) throw new ArgumentNullException(nameof(ctx));
            if (reqCtx == null) throw new ArgumentNullException(nameof(reqCtx));
        }

#if NETSTANDARD2_0
        public static unsafe void ThrowIfFailed(bool result)
#else
        public static unsafe void ThrowIfFailed([DoesNotReturnIf(false)]bool result)
#endif
        {
            if (!result)
            {
                var buf = NativeMethods.yaha_get_last_error();
                if (buf != null)
                {
                    try
                    {
                        throw new InvalidOperationException(UnsafeUtilities.GetStringFromUtf8Bytes(buf->AsSpan()));
                    }
                    finally
                    {
                        NativeMethods.yaha_free_byte_buffer(buf);
                    }
                }
                else
                {
                    throw new InvalidOperationException("Unexpected error occurred.");
                }
            }
        }
    }
}