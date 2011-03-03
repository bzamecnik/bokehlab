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

namespace spreading
{
  public partial class Form1 : Form
  {
    protected Bitmap inputImage = null;
    protected Bitmap outputImage = null;

    public Form1 ()
    {
      InitializeComponent();
    }

    private void buttonLoad_Click ( object sender, EventArgs e )
    {
      OpenFileDialog ofd = new OpenFileDialog();

      ofd.Title = "Open Image File";
      ofd.Filter = "Bitmap Files|*.bmp" +
          "|Gif Files|*.gif" +
          "|JPEG Files|*.jpg" +
          "|PNG Files|*.png" +
          "|TIFF Files|*.tif" +
          "|All image types|*.bmp;*.gif;*.jpg;*.png;*.tif";

      ofd.FilterIndex = 6;
      ofd.FileName = "";
      if ( ofd.ShowDialog() != DialogResult.OK )
        return;

      pictureBox1.Image =
      inputImage  = (Bitmap)Image.FromFile( ofd.FileName );
      outputImage = null;
    }

    private void buttonRecode_Click ( object sender, EventArgs e )
    {
        filterImage();
    }

    

    private void buttonSave_Click ( object sender, EventArgs e )
    {
      if ( outputImage == null ) return;

      SaveFileDialog sfd = new SaveFileDialog();
      sfd.Title = "Save PNG file";
      sfd.Filter = "PNG Files|*.png";
      sfd.AddExtension = true;
      sfd.FileName = "";
      if ( sfd.ShowDialog() != DialogResult.OK )
        return;

      outputImage.Save( sfd.FileName, System.Drawing.Imaging.ImageFormat.Png );
    }

    private void blurRadiusNumeric_ValueChanged(object sender, EventArgs e)
    {
        filterImage();
    }



    private void filterImage()
    {
        if (inputImage == null) return;
        Cursor.Current = Cursors.WaitCursor;

        Stopwatch sw = new Stopwatch();
        sw.Start();

        RectangleSpreadingFilter filter = new RectangleSpreadingFilter()
        {
            BlurRadius = (int)blurRadiusNumeric.Value
        };
        outputImage = filter.SpreadPSF(inputImage, outputImage);

        sw.Stop();
        labelElapsed.Text = String.Format("Elapsed time: {0:f}s", 1.0e-3 * sw.ElapsedMilliseconds);

        if (outputImage != null)
        {
            pictureBox1.Image = outputImage;
        }
        else
        {
            pictureBox1.Image = null;
            outputImage = null;
        }

        Cursor.Current = Cursors.Default;
    }

    private void imageTypeComboBox_SelectedIndexChanged(object sender, EventArgs e)
    {

    }
  }
}
