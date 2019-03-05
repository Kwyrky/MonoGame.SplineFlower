﻿using Microsoft.Xna.Framework;
using MonoGame.Forms.Controls;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;

namespace MonoGame.SplineFlower.Samples.Controls
{
    public abstract class TransformControl : UpdateWindow
    {
        public enum CenterTransformMode
        {
            Rotate,
            Scale,
            ScaleRotate
        }
        public CenterTransformMode SetCenterTransformMode { get; set; } = CenterTransformMode.ScaleRotate;

        protected bool ScalePointClick = false;
        protected bool RotatePointClick = false;
        protected bool TranslatePointClick = false;
        protected bool TranslateAllPointsClick = false;
        protected System.Drawing.Point TranslatePointFirstClick;
        protected Transform SelectedTransform;
        public Trigger SelectedTrigger { get; set; }

        [Browsable(false)]
        public BezierSpline GetSpline { get; set; }

        private Vector2 _BezierPosition;
        private Vector2 _OldBezierDistance;

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            ScalePointClick = false;
            RotatePointClick = false;
            TranslatePointClick = false;
            TranslateAllPointsClick = false;
            SelectedTransform = null;

            GetSpline.CalculateSplineCenter(GetSpline.GetAllPoints());
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (Editor.IsMouseInsideControl)
            {
                if (e.Button == MouseButtons.Left)
                {
                    SelectedTransform = GetSpline.TryGetTransformFromPosition(new Vector2(e.X, e.Y));
                    if (SelectedTransform != null)
                    {
                        TranslatePointFirstClick = e.Location;

                        if (SelectedTransform.IsCenterSpline)
                        {
                            if (SetCenterTransformMode == CenterTransformMode.Rotate) RotatePointClick = true;
                            else if (SetCenterTransformMode == CenterTransformMode.Scale) ScalePointClick = true;
                            else if (SetCenterTransformMode == CenterTransformMode.ScaleRotate)
                            {
                                RotatePointClick = true;
                                ScalePointClick = true;
                            }
                        }
                        else TranslatePointClick = true;
                    }
                    else SelectedTrigger = GetSpline.TryGetTriggerFromPosition(new Vector2(e.X, e.Y));
                }
                else if (e.Button == MouseButtons.Middle)
                {
                    TranslatePointFirstClick = e.Location;
                    TranslateAllPointsClick = true;
                }
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            int xDiff = TranslatePointFirstClick.X - e.Location.X;
            int yDiff = TranslatePointFirstClick.Y - e.Location.Y;

            if (TranslatePointClick || TranslateAllPointsClick)
            {
                if (SelectedTransform != null && TranslatePointClick)
                {
                    SelectedTransform.Translate(new Vector2(-xDiff, -yDiff));
                    GetSpline.MoveAxis(SelectedTransform.Index, new Vector2(-xDiff, -yDiff));
                    GetSpline.GetAllTrigger().ForEach(x => x.UpdateTriggerRotation());
                }
                else if (TranslateAllPointsClick) TranslateAllPoints(new Vector2(-xDiff, -yDiff));

                GetSpline.CalculateSplineCenter(GetSpline.GetAllPoints());
            }
            else if (RotatePointClick && !ScalePointClick) GetSpline.Rotate(yDiff);
            else if (ScalePointClick && !RotatePointClick) GetSpline.Scale(yDiff);
            else if (ScalePointClick && RotatePointClick) GetSpline.ScaleRotate(yDiff);

            TranslatePointFirstClick.X = e.Location.X;
            TranslatePointFirstClick.Y = e.Location.Y;
        }
        

        public void TranslateAllPoints(Vector2 amount)
        {
            Transform[] allPoints = GetSpline.GetAllPoints().Distinct().ToArray();
            for (int i = 0; i < allPoints.Length; i++)
            {
                allPoints[i].Translate(new Vector2(amount.X, amount.Y));
            }

            _BezierPosition += amount;
        }

        protected void TranslateAllPointsToScreenCenter(Vector2 bezierCenter)
        {
            Vector2 centerScreen = new Vector2(ClientSize.Width / 2, ClientSize.Height / 2);
            Vector2 distance = centerScreen - bezierCenter;

            if (_OldBezierDistance != distance) TranslateAllPoints(distance);

            _OldBezierDistance = distance;

            GetSpline.CalculateSplineCenter(GetSpline.GetAllPoints());
        }
    }
}
