﻿using Microsoft.Xna.Framework;
using MonoGame.Forms.Controls;
using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace SplineSharp.Samples.EditorBase
{
    public abstract class TransformControl : UpdateWindow
    {
        protected bool TranslatePointClick = false;
        protected System.Drawing.Point TranslatePointFirstClick;
        protected Transform SelectedTransform;

        [Browsable(false)]
        public Func<Vector2, Transform> TryGetTransformFromPosition { get; set; }

        [Browsable(false)]
        public event Action<Vector2> MovePointDiff = delegate { };

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            TranslatePointClick = false;
            SelectedTransform = null;
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (Editor.IsMouseInsideControl)
            {
                if (e.Button == MouseButtons.Left)
                {
                    SelectedTransform = TryGetTransformFromPosition(new Vector2(e.X, e.Y));
                    if (SelectedTransform != null)
                    {
                        TranslatePointFirstClick = e.Location;
                        TranslatePointClick = true;
                    }
                }
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (SelectedTransform != null && TranslatePointClick)
            {
                int xDiff = TranslatePointFirstClick.X - e.Location.X;
                int yDiff = TranslatePointFirstClick.Y - e.Location.Y;

                SelectedTransform.Translate(new Vector2(-xDiff, -yDiff));
                MovePointDiff.Invoke(new Vector2(-xDiff, -yDiff));

                TranslatePointFirstClick.X = e.Location.X;
                TranslatePointFirstClick.Y = e.Location.Y;
            }
        }
    }
}
