using System.Runtime.InteropServices;

namespace MathLibrary.Core;

public static partial class MathHelper
{
    // Вспомогательная структура для преобразования float <-> int без небезопасного кода.
    [StructLayout(LayoutKind.Explicit)]
    private struct FloatIntUnion
    {
        [FieldOffset(0)]
        public float F;
        [FieldOffset(0)]
        public int I;
    }
}
