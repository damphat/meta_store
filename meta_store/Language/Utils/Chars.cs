using System.Runtime.CompilerServices;

namespace meta_store.Language.Utils
{
    internal static class Chars
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsDigit(char c) => c >= '0' && c <= '9';

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsIdentifierStart(char c) => c >= 'a' && c <= 'z' || c >= 'A' && c <= 'Z' || c == '_';

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsIdentifierPart(char c) => IsIdentifierStart(c) || IsDigit(c);
    }
}