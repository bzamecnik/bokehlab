using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using System.Drawing.Imaging;
using libpfm;

namespace spreading
{
  public partial class Form1 : Form
  {
    protected Bitmap inputLdrImage = null;
    protected Bitmap outputLdrImage = null;

    protected PFMImage inputHdrImage = null;
    protected PFMImage outputHdrImage = null;

    public Form1 ()
    {
      InitializeComponent();
    }

    private void buttonLoad_Click ( object sender, EventArgs e )
    {
      OpenFileDialog ofd = new OpenFileDialog();

      ofd.Title = "Open Image File";
      ofd.Filter = "PNG Files|*.png" +
          "|PFM Files|*.pfm" +
          "|Bitmap Files|*.bmp" +
          "|Gif Files|*.gif" +
          "|JPEG Files|*.jpg" +
          "|TIFF Files|*.tif" +
          "|All Image types|*.png;*.pfm;*.bmp;*.gif;*.jpg;*.tif";

      ofd.FilterIndex = 7;
      ofd.FileName = "";
      if ( ofd.ShowDialog() != DialogResult.OK )
        return;

        if (ofd.FileName.EndsWith(".pfm")) {
            inputHdrImage = PFMImage.LoadImage(ofd.FileName);
            inputLdrImage = inputHdrImage.ToLdr();
        } else {
            inputLdrImage = (Bitmap)Image.FromFile(ofd.FileName);
            inputHdrImage = PFMImage.FromLdr(inputLdrImage);
        }
      pictureBox1.Image = inputLdrImage;

      outputHdrImage = null;
      outputLdrImage = null;
    }

    private void buttonRecode_Click ( object sender, EventArgs e )
    {
        filterImage();
    }

    private void buttonSave_Click ( object sender, EventArgs e )
    {
        if ((outputLdrImage == null) || (outputHdrImage == null)) return;

      SaveFileDialog sfd = new SaveFileDialog();
      sfd.Title = "Save PNG/PFM file";
      sfd.Filter = "PNG Files|*.png|PFM Files|*.pfm";
      sfd.AddExtension = true;
      sfd.FileName = "";
      if ( sfd.ShowDialog() != DialogResult.OK )
        return;

      if (sfd.FileName.EndsWith(".pfm"))
      {
          outputHdrImage.SaveImage(sfd.FileName);
      }
      else
      {
          outputLdrImage.Save(sfd.FileName, System.Drawing.Imaging.ImageFormat.Png);
      }
    }

    private void blurRadiusNumeric_ValueChanged(object sender, EventArgs e)
    {
        filterImage();
    }

    private void filterImage()
    {
        if ((inputLdrImage == null) || (inputLdrImage == null)) return;
        Cursor.Current = Cursors.WaitCursor;

        Stopwatch sw = new Stopwatch();
        sw.Start();

        RectangleSpreadingFilter filter = new RectangleSpreadingFilter()
        {
            BlurRadius = (int)blurRadiusNumeric.Value
        };
        outputHdrImage = filter.SpreadPSF(inputHdrImage, outputHdrImage);
        outputLdrImage = outputHdrImage.ToLdr();
        
        //// stub:
        //outputHdrImage = inputHdrImage;
        ////outputLdrImage = inputLdrImage;
        //outputLdrImage = inputHdrImage.ToLdr();

        sw.Stop();
        labelElapsed.Text = String.Format("Elapsed time: {0:f}s", 1.0e-3 * sw.ElapsedMilliseconds);

        if (outputLdrImage != null)
        {
            pictureBox1.Image = outputLdrImage;
        }
        else
        {
            pictureBox1.Image = null;
            outputLdrImage = null;
        }

        Cursor.Current = Cursors.Default;
    }

    private void imageTypeComboBox_SelectedIndexChanged(object sender, EventArgs e)
    {

    }
  }
}
