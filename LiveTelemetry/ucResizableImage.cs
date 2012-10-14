﻿using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace LiveTelemetry
{
    public partial class ucResizableImage : UserControl
    {
        public string Caption { get; set; }
        private string imagePath = "";
        private Bitmap imageBMP;
        private Size bmpSize;
        public bool Disabled = false;

        private ImageAttributes grayscaleAttributes;

        public Size PictureSize
        {
            get
            {
                return bmpSize;
            }
        }

        public ucResizableImage(string image)
        {
            Caption = "";
            InitializeComponent();

            imagePath = image;
            if (image.ToLower().EndsWith(".tga"))
                imageBMP = Paloma.TargaImage.LoadTargaImage(image); // http://www.codeproject.com/Articles/31702/NET-Targa-Image-Reader
            else
                imageBMP = (Bitmap)Bitmap.FromFile(image);

            SetStyle(
              ControlStyles.AllPaintingInWmPaint |
              ControlStyles.UserPaint |
              ControlStyles.DoubleBuffer, true);


            //create the grayscale ColorMatrix
            ColorMatrix colorMatrix = new ColorMatrix(
                new float[][]
                    {
                        new float[] {.3f, .3f, .3f, 0, 0},
                        new float[] {.59f, .59f, .59f, 0, 0},
                        new float[] {.11f, .11f, .11f, 0, 0},
                        new float[] {0, 0, 0, 1, 0},
                        new float[] {0, 0, 0, 0, 1}
                    });

            //create some image attributes
            grayscaleAttributes = new ImageAttributes();

            //set the color matrix attribute
            grayscaleAttributes.SetColorMatrix(colorMatrix);
        }

        public void Crop(int w, int h)
        {
            Crop(w, h, true);
        }

        public void Crop(int w, int h, bool resize)
        {
            if (h > imageBMP.Size.Height)
                h = imageBMP.Size.Height;

            Size = new Size(w, h);
            if (w > imageBMP.Size.Width)
                w = imageBMP.Size.Width;
            
            bmpSize = new Size(w, h);
            
            if (w < imageBMP.Size.Height)
            {
                bmpSize = new Size(h * imageBMP.Size.Width / imageBMP.Size.Height, h);
            }
            
            if (imageBMP.Size.Width > w || w < imageBMP.Size.Width)
            {
                bmpSize = new Size(w, w * imageBMP.Size.Height / imageBMP.Size.Width);
            }
            
            if (this.bmpSize.Height > h)
            {
                // back to original..
                bmpSize = new Size(h * imageBMP.Size.Width / imageBMP.Size.Height, h);
            }

            if (resize)
            {
                Invalidate();
                OnResize(null);
            }
        }

        protected override void OnResize(EventArgs e)
        {
            if (this.bmpSize.Width == 0)
            {
                Crop(this.Size.Width, this.Size.Height, false);
            }
            Bitmap b = new Bitmap(this.Size.Width, this.Size.Height);
            Graphics g = Graphics.FromImage((Image)b);

            Rectangle s = new Rectangle((this.Width - bmpSize.Width)/2, (this.Height - bmpSize.Height)/2,
                                        this.bmpSize.Width, this.bmpSize.Height);
            g.DrawImage(imageBMP, (this.Width - bmpSize.Width) / 2, (this.Height - bmpSize.Height) / 2, this.bmpSize.Width, this.bmpSize.Height);
            g.DrawString(Caption, new Font("Tahoma", 10.0f, FontStyle.Underline), Brushes.White, 5, this.Height-15);
            g.Dispose();
            this.BackgroundImage = (Image)b;
            if (e != null) base.OnResize(e);
        }

    }

}