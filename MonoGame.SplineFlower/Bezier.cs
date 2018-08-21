﻿using Microsoft.Xna.Framework;

namespace MonoGame.SplineFlower
{
    internal static class Bezier
    {
        #region Quadratic Bezier

        internal static Vector2 GetPoint(Vector2 p0, Vector2 p1, Vector2 p2, float t)
        {
            t = MathHelper.Clamp(t, 0f, 1f);
            float oneMinusT = 1f - t;

            return
                oneMinusT * oneMinusT * p0 +
                2f * oneMinusT * t * p1 +
                t * t * p2;
        }

        internal static Vector2 GetFirstDerivative(Vector2 p0, Vector2 p1, Vector2 p2, float t)
        {
            return
                2f * (1f - t) * (p1 - p0) +
                2f * t * (p2 - p1);
        }

        #endregion

        #region Cubic Bezier

        internal static Vector2 GetPoint(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3, float t)
        {
            t = MathHelper.Clamp(t, 0f, 1f);
            float oneMinusT = 1f - t;
            return
                oneMinusT * oneMinusT * oneMinusT * p0 +
                3f * oneMinusT * oneMinusT * t * p1 +
                3f * oneMinusT * t * t * p2 +
                t * t * t * p3;
        }

        internal static Vector2 GetFirstDerivative(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3, float t)
        {
            t = MathHelper.Clamp(t, 0f, 1f);
            float oneMinusT = 1f - t;
            return
                3f * oneMinusT * oneMinusT * (p1 - p0) +
                6f * oneMinusT * t * (p2 - p1) +
                3f * t * t * (p3 - p2);
        }

        #endregion
    }
}