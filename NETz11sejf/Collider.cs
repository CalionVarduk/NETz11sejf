using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NETz11sejf
{
    public static class Collider
    {
        public static bool AsRectangles(GameEntity obj1, GameEntity obj2)
        {
            return Rectangles(obj1.Left, obj1.Top, obj1.Right, obj1.Bottom, obj2.Left, obj2.Top, obj2.Right, obj2.Bottom);
        }

        public static bool AsCircles(GameEntity obj1, GameEntity obj2)
        {
            return Circles(obj1.LocationX, obj1.LocationY, obj1.Width * 0.5f, obj2.LocationX, obj2.LocationY, obj2.Width * 0.5f);
        }

        public static bool Rectangles(float left1, float top1, float right1, float bottom1, float left2, float top2, float right2, float bottom2)
        {
            return ((right1 >= left2) && (left1 <= right2) && (top1 <= bottom2) && (bottom1 >= top2));
        }

        public static bool Circles(float x1, float y1, float r1, float x2, float y2, float r2)
        {
            float dX = x2 - x1;
            float dY = y2 - y1;
            float r = r1 + r2;
            return ((dX * dX) + (dY * dY) <= (r * r));
        }

        public static double Orientation(float px, float py, float sx, float sy, float ex, float ey)
        {
            return ((sx - (double)px) * (ey - (double)py)) - ((ex - (double)px) * (sy - (double)py));
        }
    }
}
