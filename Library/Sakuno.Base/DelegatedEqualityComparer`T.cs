using System;
using System.Collections.Generic;

namespace Sakuno
{
    public class DelegatedEqualityComparer<T> : IEqualityComparer<T>
    {
        Func<T, T, bool> r_Equals;
        Func<T, int> r_HashCode;

        public DelegatedEqualityComparer(Func<T, T, bool> rpEquals, Func<T, int> rpHashCode)
        {
            r_Equals = rpEquals;
            r_HashCode = rpHashCode;
        }

        public bool Equals(T x, T y) => r_Equals(x, y);

        public int GetHashCode(T rpObject) => r_HashCode(rpObject);
    }
}
