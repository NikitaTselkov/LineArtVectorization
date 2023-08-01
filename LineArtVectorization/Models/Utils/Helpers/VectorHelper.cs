using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using LineArtVectorization.Models.Data.Vectors;

namespace LineArtVectorization.Models.Utils.Helpers
{
    public class VectorHelper
    {
        public static Vector GetVectorDirectionFromPoints(IEnumerable<Point> points)
        {
            Vector vector = new()
            {
                X = points.Last().X - points.First().X,
                Y = points.Last().Y - points.First().Y
            };

            vector.Normalize();

            return vector;
        }


        public static VectorDirection GetDirection(Vector normalized)
        {
            if (normalized.X == 0 && normalized.Y == 0)
                return VectorDirection.None;

            var angle = Math.Atan2(normalized.Y, normalized.X) / Math.PI * 180f;

            if (angle > -22.5f && angle <= 22.5f)
                return VectorDirection.Right;

            if (angle > 22.5f && angle <= 67.5f)
                return VectorDirection.UpRight;

            if (angle > 67.5f && angle <= 112.5f)
                return VectorDirection.Up;

            if (angle > 112.5f && angle <= 157.5f)
                return VectorDirection.UpLeft;

            if (angle > 157.5f || angle <= -157.5f)
                return VectorDirection.Left;

            if (angle > -157.5f && angle <= -112.5f)
                return VectorDirection.DownLeft;

            if (angle > -112.5f && angle <= -67.5f)
                return VectorDirection.Down;

            if (angle > -67.5f && angle <= -22.5f)
                return VectorDirection.DownRight;

            return VectorDirection.None;
        }
    }
}
