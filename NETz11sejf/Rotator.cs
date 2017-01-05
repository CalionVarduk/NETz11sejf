using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NETz11sejf
{
    public static class Rotator
    {
        public static void Point(float dAngle, ref float x, ref float y)
        {
            float rad = dAngle * 0.0174532925f;
            float sin = (float)Math.Sin(rad);
            float cos = (float)Math.Cos(rad);

            float tx = x;
            x = (x * cos) - (y * sin);
            y = (tx * sin) + (y * cos);
        }
    }
}
