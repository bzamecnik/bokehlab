using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace LightTracing
{
    public partial class Form1 : Form
    {
        private LightTracer lightTracer;

        public Form1()
        {
            InitializeComponent();
            lightTracer = new LightTracer();
            pictureBox1.Image = lightTracer.TraceLight();
        }
    }
}
