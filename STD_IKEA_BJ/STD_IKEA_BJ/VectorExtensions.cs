/* Author : Jonathan Borel-Jaquet
 * Date : 21/10/20
 * Description : Class VectorExtensions
 */
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
        /// <summary>
        /// Method to convert a Vector2 to PointF
        /// </summary>
        /// <param name="vector"></param>
        /// <returns>PointF with the same value as the Vector2</returns>
        public static PointF ToPointF(this Vector2 vector)
        {
            return new PointF(vector.X, vector.Y);
        }
    }

}
