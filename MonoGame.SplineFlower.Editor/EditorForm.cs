﻿using Microsoft.Xna.Framework;
using MonoGame.SplineFlower.Samples;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Collections.Generic;
using MonoGame.SplineFlower.Content;

namespace MonoGame.SplineFlower.Editor
{
    public partial class FormEditor : Form
    {
        public static JsonHandling GetJsonHandling { get; set; }
        public static JsonSerializerSettings JsonSerializerSetup { get; set; }
        private void InitializeJsonSerializer()
        {
            JsonSerializerSetup = new JsonSerializerSettings();
            JsonSerializerSetup.ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor;
            JsonSerializerSetup.NullValueHandling = NullValueHandling.Ignore;
            JsonSerializerSetup.DefaultValueHandling = DefaultValueHandling.Include;
            JsonSerializerSetup.TypeNameHandling = TypeNameHandling.Auto;
            JsonSerializerSetup.Converters.Add(new StringEnumConverter());
        }

        #region Diagnostics

        private void toolStripMenuItemCenterSpline_Click(object sender, EventArgs e)
        {
            if (splineControl != null) splineControl.MoveSplineToScreenCenter();
        }

        private void toolStripMenuItemDrawSpline_CheckedChanged(object sender, EventArgs e)
        {
            Setup.ShowSpline = toolStripMenuItemDrawSpline.Checked;
        }

        private void toolStripMenuItemDrawBaseLine_CheckedChanged(object sender, EventArgs e)
        {
            Setup.ShowBaseLine = toolStripMenuItemDrawBaseLine.Checked;
        }

        private void toolStripMenuItemDrawCurves_CheckedChanged(object sender, EventArgs e)
        {
            Setup.ShowCurves = toolStripMenuItemDrawCurves.Checked;
        }

        private void toolStripMenuItemDrawDirections_CheckedChanged(object sender, EventArgs e)
        {
            Setup.ShowDirectionVectors = toolStripMenuItemDrawDirections.Checked;
        }

        private void toolStripMenuItemDrawTrigger_CheckedChanged(object sender, EventArgs e)
        {
            Setup.ShowTriggers = toolStripMenuItemDrawTrigger.Checked;
        }

        private void toolStripMenuItemDrawCenterSpline_CheckedChanged(object sender, EventArgs e)
        {
            Setup.ShowCenterSpline = toolStripMenuItemDrawCenterSpline.Checked;
        }

        private void toolStripMenuItemDrawSplineWalker_CheckedChanged(object sender, EventArgs e)
        {
            Setup.ShowSplineWalker = toolStripMenuItemDrawSplineWalker.Checked;
        }

        private void toolStripMenuItemDrawMarker_Click(object sender, EventArgs e)
        {
            Marker.ShowSplineMarker = toolStripMenuItemDrawMarker.Checked;
        }

        private void toolStripMenuItemDrawCar_CheckedChanged(object sender, EventArgs e)
        {
            Car.ShowCar = toolStripMenuItemDrawCar.Checked;
        }

        private void toolStripMenuItemShowFPS_CheckedChanged(object sender, EventArgs e)
        {
            if (splineControl != null) splineControl.Editor.ShowFPS = toolStripMenuItemShowFPS.Checked;
        }

        private void toolStripMenuItemShowCursorPosition_CheckedChanged(object sender, EventArgs e)
        {
            if (splineControl != null) splineControl.Editor.ShowCursorPosition = toolStripMenuItemShowCursorPosition.Checked;
        }

        #endregion

        #region Spline Actions

        private void toolStripButtonAddCurveLeft_Click(object sender, EventArgs e)
        {
            if (splineControl != null && splineControl.MySpline != null) splineControl.MySpline.AddCurveLeft();
        }

        private void toolStripButtonAddCurveRight_Click(object sender, EventArgs e)
        {
            if (splineControl != null && splineControl.MySpline != null) splineControl.MySpline.AddCurveRight();
        }

        private void toolStripButtonTrackLoop_Click(object sender, EventArgs e)
        {
            if (splineControl != null && splineControl.MySpline != null)
            {
                splineControl.MySpline.Loop = true;
                toolStripButtonTrackLoop.Enabled = false;
            }
        }

        private void toolStripButtonCatMulRom_Click(object sender, EventArgs e)
        {
            if (splineControl != null && splineControl.MySpline != null)
            {
                splineControl.MySpline.CatMulRom = true;
                toolStripButtonCatMulRom.Enabled = false;
            }
        }

        #endregion

        #region Spline Walker

        private void toolStripButtonResetSplineWalker_Click(object sender, EventArgs e)
        {
            if (splineControl != null && splineControl.MySpline != null) splineControl.MySplineWalker.Reset();
        }

        private void toolStripComboBoxWalkerMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (splineControl != null && splineControl.MySpline != null) splineControl.MySplineWalker.WalkerMode = (SplineWalker.SplineWalkerMode)toolStripComboBoxWalkerMode.SelectedIndex;
        }

        private void toolStripNumericUpDownDuration_ValueChanged(object sender, EventArgs e)
        {
            if (splineControl != null && splineControl.MySpline != null) splineControl.MySplineWalker.Duration = (int)toolStripNumericUpDownDuration.Value;
        }

        #endregion

        #region TriggerEvents

        private void toolStripComboBoxSelectedTrigger_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (splineControl != null && splineControl.MySplineMarker != null)
            {
                if (toolStripComboBoxSelectedTrigger.SelectedItem.ToString() != "Marker")
                {
                    splineControl.MySplineMarker.MarkerSelected = false;

                    string[] splitted = toolStripComboBoxSelectedTrigger.SelectedItem.ToString().Split('_');
                    splineControl.MySplineMarker.SelectedTrigger = splitted[1];
                    UpdateTriggerInterface(splineControl.MySplineWalker.GetTrigger(splitted[1]));
                }
                else splineControl.MySplineMarker.MarkerSelected = true;

                splineControl.MySplineMarker.SetPosition(GetSplinePosition());
            }
        }

        private void toolStripButtonAddEvent_Click(object sender, EventArgs e)
        {
            if (splineControl != null && splineControl.MySpline != null)
            {
                Guid triggerID = splineControl.MySplineWalker.AddTrigger(
                toolStripComboBoxEvents.SelectedItem.ToString(),
                GetSplinePosition(),
                (int)toolStripNumericUpDownTriggerRange.Value);

                toolStripComboBoxSelectedTrigger.Items.Add(GetSelectedTriggerString(toolStripComboBoxEvents.SelectedItem.ToString(), triggerID.ToString()));
            }
        }

        private void toolStripNumericUpDownTriggerRange_ValueChanged(object sender, EventArgs e)
        {
            if (splineControl != null && splineControl.MySplineMarker != null)
            {
                if (!splineControl.MySplineMarker.MarkerSelected)
                {
                    Trigger trigger = splineControl.MySplineMarker.GetTrigger();
                    trigger.TriggerRange = (int)toolStripNumericUpDownTriggerRange.Value;
                }
            }
        }

        private void UpdateTriggerInterface(Trigger trigger)
        {
            trackBarMarker.Value = (int)(trigger.GetPlainProgress * Setup.SplineMarkerResolution);
            toolStripNumericUpDownTriggerRange.Value = (int)(trigger.TriggerRange * Setup.SplineMarkerResolution);
        }

        private string GetSelectedTriggerString(string itemName, string itemID)
        {
            return itemName + "_" + itemID;
        }

        private void toolStripComboBoxCenterTransformMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (splineControl != null)
            {
                splineControl.SetCenterTransformMode = (Samples.Controls.TransformControl.CenterTransformMode)toolStripComboBoxCenterTransformMode.SelectedIndex;
            }
        }

        #endregion

        #region TrackBar

        private void trackBarMarker_Scroll(object sender, EventArgs e)
        {
            if (splineControl != null && splineControl.MySpline != null) splineControl.MySplineMarker.SetPosition(GetSplinePosition());
        }

        private void trackBarMarker_MouseDown(object sender, MouseEventArgs e)
        {
            if (splineControl != null &&
                splineControl.MySplineMarker != null)
            {
                if (!splineControl.MySplineMarker.MarkerSelected) splineControl.MySplineWalker.Stop = true;
            }
        }

        private void trackBarMarker_MouseUp(object sender, MouseEventArgs e)
        {
            if (splineControl != null &&
                splineControl.MySplineMarker != null)
            {
                if (!splineControl.MySplineMarker.MarkerSelected)
                {
                    splineControl.ReorderTriggerList();
                    splineControl.MySplineWalker.Stop = false;
                }
            }
        }

        private float GetSplinePosition()
        {
            return (float)trackBarMarker.Value / Setup.SplineMarkerResolution;
        }

        #endregion

        #region JsonHandling

        #region Export JSON

        private void toolStripMenuItemExportJson_Click(object sender, EventArgs e)
        {
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                GetJsonHandling.GetBezierSplineData.SplineMarkerResolution = Setup.SplineMarkerResolution;
                GetJsonHandling.GetBezierSplineData.SplineWalkerDuration = splineControl.MySplineWalker.Duration;
                GetJsonHandling.GetBezierSplineData.CatMulRom = splineControl.MySpline.CatMulRom;
                GetJsonHandling.GetBezierSplineData.Loop = splineControl.MySpline.Loop;

                CreateJsonReadyPointData();
                CreateJsonReadyPointModeData();
                CreateJsonReadyTriggerData();

                Array.Resize(ref GetJsonHandling.GetBezierSplineData.TriggerNames, toolStripComboBoxEvents.Items.Count);
                for (int i = 0; i < toolStripComboBoxEvents.Items.Count; i++)
                {
                    GetJsonHandling.GetBezierSplineData.TriggerNames[i] = toolStripComboBoxEvents.Items[i].ToString();
                }

                string json = JsonConvert.SerializeObject(GetJsonHandling.GetBezierSplineData, JsonSerializerSetup);
                File.WriteAllText(@Path.Combine(saveFileDialog.FileName), json, Encoding.UTF8);
            }
        }
        private void CreateJsonReadyPointData()
        {
            Transform[] pointsToExport = splineControl.MySpline.GetAllPoints();
            Array.Resize(ref GetJsonHandling.GetBezierSplineData.PointData, pointsToExport.Length);

            for (int i = 0; i < pointsToExport.Length; i++)
            {
                GetJsonHandling.GetBezierSplineData.PointData[i] =
                    new TransformDummy(pointsToExport[i].Index, pointsToExport[i].Position);
            }
        }
        private void CreateJsonReadyPointModeData()
        {
            BezierSpline.BezierControlPointMode[] modePointsToExport = splineControl.MySpline.GetAllPointModes();
            Array.Resize(ref GetJsonHandling.GetBezierSplineData.PointModeData, modePointsToExport.Length);

            for (int i = 0; i < modePointsToExport.Length; i++)
            {
                GetJsonHandling.GetBezierSplineData.PointModeData[i] =
                    new BezierControlPointModeDummy(modePointsToExport[i].ToString());
            }
        }
        private void CreateJsonReadyTriggerData()
        {
            Trigger[] triggerToExport = splineControl.MySpline.GetAllTrigger().ToArray();
            Array.Resize(ref GetJsonHandling.GetBezierSplineData.TriggerData, triggerToExport.Length);

            for (int i = 0; i < triggerToExport.Length; i++)
            {
                GetJsonHandling.GetBezierSplineData.TriggerData[i] =
                    new TriggerDummy(
                        triggerToExport[i].Name,
                        triggerToExport[i].ID.ToString(),
                        triggerToExport[i].GetPlainProgress,
                        triggerToExport[i].TriggerRange * Setup.SplineMarkerResolution);
            }
        }

        #endregion

        #region Import JSON

        private void toolStripMenuItemImportJson_Click(object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                ResetSplineWalkerMode();
                ResetTrackBarMarker();

                GetJsonHandling.GetBezierSplineData =
                    JsonConvert.DeserializeObject<BezierSplineData>(
                        File.ReadAllText(openFileDialog.FileName), JsonSerializerSetup);

                Setup.SplineMarkerResolution = GetJsonHandling.GetBezierSplineData.SplineMarkerResolution;
                splineControl.MySplineWalker.Duration = GetJsonHandling.GetBezierSplineData.SplineWalkerDuration;
                toolStripNumericUpDownDuration.Value = GetJsonHandling.GetBezierSplineData.SplineWalkerDuration;
                splineControl.MySpline.CatMulRom = GetJsonHandling.GetBezierSplineData.CatMulRom;
                splineControl.MySpline.Loop = GetJsonHandling.GetBezierSplineData.Loop;
                toolStripButtonCatMulRom.Enabled = !GetJsonHandling.GetBezierSplineData.CatMulRom;
                toolStripButtonTrackLoop.Enabled = !GetJsonHandling.GetBezierSplineData.Loop;

                Trigger[] loadedTrigger;
                splineControl.MySpline.LoadJsonBezierSplineData(
                    GetJsonHandling.GetBezierSplineData.PointData,
                    GetJsonHandling.GetBezierSplineData.PointModeData,
                    GetJsonHandling.GetBezierSplineData.TriggerData,
                    out loadedTrigger);

                toolStripComboBoxEvents.Items.Clear();
                for (int i = 0; i < GetJsonHandling.GetBezierSplineData.TriggerNames.Length; i++)
                {
                    toolStripComboBoxEvents.Items.Add(GetJsonHandling.GetBezierSplineData.TriggerNames[i]);
                }
                toolStripComboBoxEvents.SelectedIndex = 0;

                ResetComboBoxSelectedTrigger();
                foreach (Trigger trigger in loadedTrigger)
                {
                    toolStripComboBoxSelectedTrigger.Items.Add(GetSelectedTriggerString(trigger.Name, trigger.ID.ToString()));
                }

                splineControl.MySplineWalker.Reset(SplineWalker.ResetLocation.End);
                splineControl.MoveSplineToScreenCenter();
            }
        }

        #endregion

        #endregion

        #region File Menu

        // New BezierSpline
        private void toolStripMenuItemNew_Click(object sender, EventArgs e)
        {
            // Reset the SplineWalkerMode so that the SplineWalker stops when creating a new BezierSpline.
            ResetSplineWalkerMode();

            //Reset the TrackBarMarker to make sure the actual marker position is up to date.
            ResetTrackBarMarker();

            // Reset the BezierSpline (which creates a new one).
            splineControl.MySpline.Reset();

            // Adding a small value to refresh the point positions.
            splineControl.MySpline.GetAllPoints()[0].Translate(Vector2.One);

            // Recalculating the new BezierCenter.
            splineControl.SplineControl_RecalculateBezierCenter();

            // Revert the translation.
            splineControl.MySpline.GetAllPoints()[0].Translate(-Vector2.One);

            // Move the BezierSpline to the center of the screen.
            splineControl.MoveSplineToScreenCenter();

            // Reset the SelectedTrigger of the SplineMarker.
            ResetSplineMarkerSelectedTrigger();

            // Reset the SelectedTrigger of the ComboBox.
            ResetComboBoxSelectedTrigger();

            // Reset the Looped status of the BezierSpline.
            ResetLoop();
        }
        private void ResetTrackBarMarker()
        {
            trackBarMarker.Value = 0;
            splineControl.MySplineMarker.Reset();
        }
        private void ResetSplineMarkerSelectedTrigger()
        {
            splineControl.MySplineMarker.SelectedTrigger = "";
            splineControl.MySplineMarker.MarkerSelected = true;
        }
        private void ResetComboBoxSelectedTrigger()
        {
            toolStripComboBoxSelectedTrigger.Items.Clear();
            toolStripComboBoxSelectedTrigger.Items.Add("Marker");
            toolStripComboBoxSelectedTrigger.SelectedIndex = 0;
        }
        private void ResetSplineWalkerMode()
        {
            toolStripComboBoxWalkerMode.SelectedIndex = 0;
        }
        private void ResetLoop()
        {
            toolStripButtonTrackLoop.Enabled = true;
            splineControl.MySpline.Loop = false;
        }

        // Close Application
        private void toolStripMenuItemExit_Click(object sender, EventArgs e)
        {
            Close();
        }

        #endregion

        #region Tools Menu
        
        // Trigger Editor
        private void toolStripMenuItemTriggerEditor_Click(object sender, EventArgs e)
        {
            List<string> trigger = new List<string>();
            foreach (var item in toolStripComboBoxEvents.Items) trigger.Add(item.ToString());

            TriggerEditor triggerEditor = new TriggerEditor(trigger);
            triggerEditor.UpdateTriggerNames += TriggerEditor_UpdateTriggerNames;
            triggerEditor.Show();
        }
        private void TriggerEditor_UpdateTriggerNames(List<string> obj)
        {
            toolStripComboBoxEvents.Items.Clear();
            for (int i = 0; i < obj.Count; i++) toolStripComboBoxEvents.Items.Add(obj[i].ToString());
            toolStripComboBoxEvents.SelectedIndex = 0;
        }

        #endregion

        public FormEditor()
        {
            InitializeComponent();
            InitializeJsonSerializer();

            GetJsonHandling = new JsonHandling();
            GetJsonHandling.GetBezierSplineData = new BezierSplineData();

            saveFileDialog.InitialDirectory = @Path.Combine(Application.StartupPath, "Content", "Splines");
            openFileDialog.InitialDirectory = saveFileDialog.InitialDirectory;
        }

        private void FormEditor_Load(object sender, EventArgs e)
        {
            toolStripComboBoxEvents.SelectedIndex = 0;
            toolStripComboBoxSelectedTrigger.SelectedIndex = 0;
            toolStripComboBoxWalkerMode.SelectedIndex = 0;
            toolStripComboBoxCenterTransformMode.SelectedIndex = 3;
        }

        private void FormEditor_Resize(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Maximized) MoveSplineToCenterOfScreen();
        }

        private void FormEditor_ResizeEnd(object sender, EventArgs e)
        {
            MoveSplineToCenterOfScreen();
        }

        private void splineControl_MouseUp(object sender, MouseEventArgs e)
        {
            if (splineControl != null && splineControl.SelectedTrigger != null)
            {
                if (splineControl.MySplineMarker != null && splineControl.MySplineMarker.Initialized)
                {
                    splineControl.MySplineMarker.SelectedTrigger = splineControl.SelectedTrigger.ID.ToString();
                    toolStripComboBoxSelectedTrigger.SelectedItem = GetSelectedTriggerString(
                        splineControl.SelectedTrigger.Name,
                        splineControl.SelectedTrigger.ID.ToString());
                }

                UpdateTriggerInterface(splineControl.SelectedTrigger);
                splineControl.SelectedTrigger = null;
            }
        }

        private void MoveSplineToCenterOfScreen()
        {
            if (splineControl != null)
            {
                splineControl.SplineControl_RecalculateBezierCenter();
                splineControl.MoveSplineToScreenCenter();
            }
        }
    }
}
