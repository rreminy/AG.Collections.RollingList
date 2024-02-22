using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AG.Collections
{
    internal static class ThrowHelper
    {
        public static void ThrowArgumentOutOfRangeExceptionIfLessThan<T>(string paramName, T value, T comparand) where T : IComparable<T>
        {
#if NET8_0_OR_GREATER
            ArgumentOutOfRangeException.ThrowIfLessThan(value, comparand);
#else
            if (Comparer<T>.Default.Compare(value, comparand) <= -1) ThrowArgumentOutOfRangeException(paramName, $"{paramName} must be greater than or equal {comparand}");
#endif
        }

        public static void ThrowArgumentOutOfRangeExceptionIfGreaterThanOrEqual<T>(string paramName, T value, T comparand) where T : IComparable<T>
        {
#if NET8_0_OR_GREATER
            ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(value, comparand);
#else
            if (Comparer<T>.Default.Compare(value, comparand) >= 0) ThrowArgumentOutOfRangeException(paramName, $"{paramName} must be less than {comparand}");
#endif
        }

        [DoesNotReturn]
        public static void ThrowArgumentOutOfRangeException(string paramName, string message)
        {
            throw new ArgumentOutOfRangeException(paramName, message);
        }
    }
}
