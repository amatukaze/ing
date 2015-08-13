using System;
using System.Runtime.CompilerServices;

namespace Sakuno.SystemInterop
{
    public static class NativeUtils
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Succeeded(int rpResult)
        {
            return rpResult >= 0;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Failed(int rpResult)
        {
            return rpResult < 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort HiWord(IntPtr rpValue)
        {
            return (ushort)(rpValue.ToInt64() >> 0x10 & 0xFFFFL);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort LoWord(IntPtr rpValue)
        {
            return (ushort)(rpValue.ToInt64() & 0xFFFFL);
        }
    }
}
