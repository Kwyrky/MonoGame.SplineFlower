﻿using Microsoft.Xna.Framework;
using System.ComponentModel;

namespace SplineSharp.Samples.Controls
{
    public class CurveControl : TransformControl
    {
        public enum BezierType
        {
            Quadratic,
            Cubic
        }
        [DisplayName("BezierType")]
        [DefaultValue(BezierType.Quadratic)]
        public BezierType GetBezierType { get; set; } = BezierType.Quadratic;

        public BezierCurve MyCurve;

        protected override void Initialize()
        {
            base.Initialize();
            Setup.Initialize(Editor.graphics);

            MyCurve = new BezierCurve();

            if (GetBezierType == BezierType.Cubic) MyCurve.CreateCubic();
            else MyCurve.CreateQuadratic();

            TryGetTransformFromPosition = MyCurve.TryGetTransformFromPosition;
            GetAllPoints = MyCurve.GetAllPoints;
            RecalculateBezierCenter += CurveControl_RecalculateBezierCenter;

            MoveSplineToScreenCenter();
        }

        public void CurveControl_RecalculateBezierCenter()
        {
            if (MyCurve != null) MyCurve.CalculateBezierCenter(MyCurve.GetAllPoints());
        }

        public void MoveSplineToScreenCenter()
        {
            if (MyCurve != null) TranslateAllPointsToScreenCenter(MyCurve.GetBezierCenter);
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        protected override void Draw()
        {
            base.Draw();

            Editor.spriteBatch.Begin();

            if (MyCurve != null) MyCurve.DrawCurve(Editor.spriteBatch);

            Editor.spriteBatch.End();
        }
    }
}