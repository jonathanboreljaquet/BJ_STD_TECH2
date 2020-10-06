using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace STD_IKEA_BJ
{
    public static class VectorExtensions
    {
        public static PointF ToPointF(this Vector2 vector2)
        {
            return new PointF(vector2.X, vector2.Y);
        }
    }

}
