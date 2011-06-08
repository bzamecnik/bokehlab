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
        private ComplexLens complexLens;
        private LensRayTransferFunction lrtf;
        private int sampleCount;

        private IList<LensRayTransferFunction.Parameters> outgoingRays;

        bool initialized = false;

        public ComplexLensLrtfForm()
        {
            InitializeComponent();

            SetDoubleBuffered(drawingPanel);

            variableParameterComboBox.DataSource = Enum.GetValues(
                typeof(LensRayTransferFunction.VariableParameter));
            variableParameterComboBox.SelectedItem = LensRayTransferFunction.VariableParameter.DirectionTheta;
            shownParameterComboBox.DataSource = Enum.GetValues(
                typeof(LensRayTransferFunction.VariableParameter));

            complexLens = CreateLens();
            lrtf = new LensRayTransferFunction(complexLens);

            sampleCountNumeric.Value = 256;

            initialized = true;
            Recompute();
        }

        private ComplexLens CreateLens()
        {
            //double curvatureRadius = 150;
            //double apertureRadius = 100;
            //return ComplexLens.CreateBiconvexLens(curvatureRadius, apertureRadius, 0);
            return ComplexLens.CreateDoubleGaussLens(Materials.Fixed.AIR, 4.0);
            //return ComplexLens.CreatePetzvalLens(Materials.Fixed.AIR, 4.0);
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

            outgoingRays = lrtf.SampleLrtf(inputRay, variableParam, sampleCount);

            drawingPanel.Invalidate();
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

        private void drawingPanel_Paint(object sender, PaintEventArgs e)
        {
            var shownParam = (LensRayTransferFunction.VariableParameter)
                shownParameterComboBox.SelectedValue;
            PlotPanel(e, shownParam);
        }

        private void PlotPanel(PaintEventArgs e, LensRayTransferFunction.VariableParameter shownParam)
        {
            base.OnPaint(e);

            Graphics g = e.Graphics;
            //g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            // scale drawing area to [0; sampleCount-1] x [0; 1]
            g.ScaleTransform(e.ClipRectangle.Width / (float)sampleCount, -e.ClipRectangle.Height);
            g.TranslateTransform(0, -1);

            g.Clear(Color.White);

            var values = SelectRayParameterDimension(outgoingRays, shownParam);
            int x = 0;
            foreach (var value in values)
            {
                if (value.HasValue)
                {
                    g.FillRectangle(Brushes.Blue, x, 0, 1, (float)value.Value);
                }
                else
                {
                    g.FillRectangle(Brushes.LightSalmon, x, 0, 1, 1);
                }
                x++;
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

        private void complexLensForm_Resize(object sender, EventArgs e)
        {
            Recompute();
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
        }

        private void sampleCountNumeric_ValueChanged(object sender, EventArgs e)
        {
            sampleCount = (int)sampleCountNumeric.Value;
            Recompute();
        }

        private void shownParameterComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            drawingPanel.Invalidate();
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
    }
}
