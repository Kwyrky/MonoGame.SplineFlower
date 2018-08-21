﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.SplineFlower
{
    public static class Setup
    {
        public static float SplineMarkerResolution = 1000f;
        public static bool ShowBezierSpline { get; set; } = true;
        public static bool ShowSplineWalker { get; set; } = true;

        public static Color BaseLineColor { get; set; } = Color.White;
        public static float BaseLineThickness { get; set; } = 1f;
        public static bool ShowBaseLine { get; set; } = true;

        public static Color CurveLineColor { get; set; } = Color.Yellow;
        public static float CurveLineThickness { get; set; } = 1.5f;
        public static bool ShowCurves { get; set; } = true;

        public static Color DirectionLineColor { get; set; } = Color.LightGreen;
        public static float DirectionLineThickness { get; set; } = 1f;
        public static float DirectionLineLength { get; set; } = 50f;
        public static bool ShowDirectionVectors { get; set; } = true;

        public static Color StartPointColor { get; set; } = Color.Magenta;
        public static float StartPointThickness { get; set; } = 1.3f;

        public static Color TriggerEventColor { get; set; } = Color.Magenta;
        public static float TriggerEventThickness { get; set; } = 1f;
        public static bool ShowTriggers { get; set; } = true;

        public static Color PointColor { get; set; } = Color.Red;
        public static int PointThickness { get; set; } = 10;

        public static bool MovePointAxis { get; set; } = true;

        internal static Texture2D Pixel { get; set; }
        internal static Texture2D Circle { get; set; }

        public static void Initialize(GraphicsDevice graphicsDevice)
        {
            if (Pixel == null)
            {
                Pixel = new Texture2D(graphicsDevice, 1, 1);
                Pixel.SetData(new[] { Color.White });
            }

            if (Circle == null)
            {
                Circle = CreateCircleTexture(graphicsDevice, 8);
            }
        }
        private static Texture2D CreateCircleTexture(GraphicsDevice device, int radius)
        {
            Texture2D texture = new Texture2D(device, radius, radius);
            Color[] colorData = new Color[radius * radius];

            float diam = radius / 2f;
            float diamsq = diam * diam;

            for (int x = 0; x < radius; x++)
            {
                for (int y = 0; y < radius; y++)
                {
                    int index = x * radius + y;
                    Vector2 pos = new Vector2(x - diam, y - diam);
                    if (pos.LengthSquared() <= diamsq)
                    {
                        colorData[index] = Color.White;
                    }
                    else
                    {
                        colorData[index] = Color.Transparent;
                    }
                }
            }

            texture.SetData(colorData);
            return texture;
        }
    }
}
