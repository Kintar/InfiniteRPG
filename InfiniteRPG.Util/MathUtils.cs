using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InfiniteRPG.Util
{
    public static class MathUtils
    {
        public static bool IsPowerOfTwo(int val)
        {
            return val > 0 && (val & (val - 1)) == 0;
        }
    }
}
