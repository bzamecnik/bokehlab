namespace BokehLab.RayTracing.Test.GUI
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using System.Windows.Forms;
    using BokehLab.Math;
    using BokehLab.RayTracing;
    using BokehLab.RayTracing.Lens;
    using OpenTK;

    public partial class ComplexLensLrtfForm : Form
    {
        private IList<ComplexLens> complexLenses;
        private LensRayTransferFunction lrtf;
        private int sampleCount;

        private IList<LensRayTransferFunction.Parameters> outgoingRays;

        bool initialized = false;

        public ComplexLensLrtfForm()
        {
            InitializeComponent();

            SetDoubleBuffered(posThetaPanel);
            SetDoubleBuffered(posPhiPanel);
            SetDoubleBuffered(dirThetaPanel);
            SetDoubleBuffered(dirPhiPanel);

            variableParameterComboBox.DataSource = Enum.GetValues(
                typeof(LensRayTransferFunction.VariableParameter));
            variableParameterComboBox.SelectedItem = LensRayTransferFunction.VariableParameter.DirectionTheta;

            complexLenses = CreateLenses();
            lrtf = new LensRayTransferFunction(complexLenses[0]);

            lensComboBox.Items.AddRange(new[] { "Double Gauss", "Petzval", "Biconvex" });
            lensComboBox.SelectedIndex = 0;

            sampleCountNumeric.Value = 256;

            initialized = true;
            Recompute();
        }

        private IList<ComplexLens> CreateLenses()
        {
            return new List<ComplexLens>(
                new[] {
                ComplexLens.CreateDoubleGaussLens(Materials.Fixed.AIR, 4.0),
                ComplexLens.CreatePetzvalLens(Materials.Fixed.AIR, 4.0),
                ComplexLens.CreateBiconvexLens(150, 100, 0)}
            );
        }

        private void Recompute()
        {
            if (!initialized)
            {
                return;
            }

            double positionTheta = (double)positionThetaNumeric.Value;
            double positionPhi = (double)positionPhiNumeric.Value;
            double directionTheta = (double)directionThetaNumeric.Value;
            double directionPhi = (double)directionPhiNumeric.Value;
            var inputRay = new LensRayTransferFunction.Parameters(
                positionTheta, positionPhi, directionTheta, directionPhi);

            var variableParam = (LensRayTransferFunction.VariableParameter)
                variableParameterComboBox.SelectedValue;

            outgoingRays = lrtf.SampleLrtf1D(inputRay, variableParam, sampleCount);

            InvalidatePanels();
        }

        private void InvalidatePanels()
        {
            posThetaPanel.Invalidate();
            posPhiPanel.Invalidate();
            dirThetaPanel.Invalidate();
            dirPhiPanel.Invalidate();
        }

        /// <summary>
        /// Create a one-dimensional list of value of particular dimension
        /// from the list with all dimensions. Eg. select only list of
        /// direction phi's.
        /// </summary>
        /// <param name="rays"></param>
        /// <param name="?"></param>
        /// <returns></returns>
        private IEnumerable<double?> SelectRayParameterDimension(
            IList<LensRayTransferFunction.Parameters> rays,
            LensRayTransferFunction.VariableParameter dimension)
        {
            return rays.Select((ray) => (ray != null) ? (double?)ray[dimension] : null);
        }

        private void PlotPanel(PaintEventArgs e, LensRayTransferFunction.VariableParameter shownParam)
        {
            base.OnPaint(e);

            Graphics g = e.Graphics;
            // scale drawing area to [0; sampleCount-1] x [0; 1]
            g.ScaleTransform(e.ClipRectangle.Width / (float)sampleCount, -e.ClipRectangle.Height);
            g.TranslateTransform(0, -1);

            g.Clear(Color.White);

            var values = SelectRayParameterDimension(outgoingRays, shownParam).ToList();
            double? lastValue = null;
            Pen linePen = new Pen(Color.Black, 40 / (float)e.ClipRectangle.Height);
            for (int i = 0; i < values.Count; i++)
            {
                var value = values[i];
                if (value.HasValue)
                {
                    g.FillRectangle(Brushes.LightBlue, i, 0, 1, (float)value.Value);
                }
                else
                {
                    g.FillRectangle(Brushes.LightSalmon, i, 0, 1, 1);
                }
                if ((lastValue != null) && (value != null))
                {
                    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                    g.DrawLine(linePen, i - 0.5f, (float)lastValue.Value, i + 0.5f, (float)value.Value);
                    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
                }
                lastValue = value;
            }
        }

        private void SceneControlsValueChanged(object sender, EventArgs e)
        {
            Recompute();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Recompute();
        }

        private void complexLensLrtfForm_Resize(object sender, EventArgs e)
        {
            InvalidatePanels();
        }

        private void variableParameterComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            var varParam = (LensRayTransferFunction.VariableParameter)
                variableParameterComboBox.SelectedValue;
            foreach (var control in new[] {
                positionThetaNumeric, positionPhiNumeric,
                directionThetaNumeric, directionPhiNumeric})
            {
                control.Enabled = true;
            }
            switch (varParam)
            {
                case LensRayTransferFunction.VariableParameter.PositionTheta:
                    positionThetaNumeric.Enabled = false;
                    break;
                case LensRayTransferFunction.VariableParameter.PositionPhi:
                    positionPhiNumeric.Enabled = false;
                    break;
                case LensRayTransferFunction.VariableParameter.DirectionTheta:
                    directionThetaNumeric.Enabled = false;
                    break;
                case LensRayTransferFunction.VariableParameter.DirectionPhi:
                    directionPhiNumeric.Enabled = false;
                    break;
                default:
                    break;
            }
            Recompute();
        }

        private void sampleCountNumeric_ValueChanged(object sender, EventArgs e)
        {
            sampleCount = (int)sampleCountNumeric.Value;
            Recompute();
        }

        public static void SetDoubleBuffered(System.Windows.Forms.Control c)
        {
            // source: http://stackoverflow.com/questions/76993/how-to-double-buffer-net-controls-on-a-form

            //Taxes: Remote Desktop Connection and painting
            //http://blogs.msdn.com/oldnewthing/archive/2006/01/03/508694.aspx
            if (System.Windows.Forms.SystemInformation.TerminalServerSession)
                return;

            System.Reflection.PropertyInfo aProp =
                  typeof(System.Windows.Forms.Control).GetProperty(
                        "DoubleBuffered",
                        System.Reflection.BindingFlags.NonPublic |
                        System.Reflection.BindingFlags.Instance);

            aProp.SetValue(c, true, null);
        }

        private void posThetaPanel_Paint(object sender, PaintEventArgs e)
        {
            PlotPanel(e, LensRayTransferFunction.VariableParameter.PositionTheta);
        }

        private void posPhiPanel_Paint(object sender, PaintEventArgs e)
        {
            PlotPanel(e, LensRayTransferFunction.VariableParameter.PositionPhi);
        }

        private void dirThetaPanel_Paint(object sender, PaintEventArgs e)
        {
            PlotPanel(e, LensRayTransferFunction.VariableParameter.DirectionTheta);
        }

        private void dirPhiPanel_Paint(object sender, PaintEventArgs e)
        {
            PlotPanel(e, LensRayTransferFunction.VariableParameter.DirectionPhi);
        }

        private void DrawingPanel_Resize(object sender, EventArgs e)
        {
            ((Control)sender).Invalidate();
        }

        private void lensComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            int index = lensComboBox.SelectedIndex;
            if ((index >= 0) && (index < complexLenses.Count))
            {
                lrtf.Lens = complexLenses[index];
            }
            Recompute();
        }
    }
}
