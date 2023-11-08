using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace SysTrayApp
{

    public class StatusLamp
    {
        private Bitmap Bm;
        private Size mSize;
        private Graphics g;


        public StatusLamp(Size size)
        {
            mSize = size;
            Bm = new Bitmap(size.Width, size.Height);
            g = Graphics.FromImage(Bm);
        }


        public Bitmap SetColor(Color color)
        {
            try
            {
                Brush B = new System.Drawing.Drawing2D.LinearGradientBrush(new Point(0, 0), new Point(mSize.Width, mSize.Height), System.Drawing.Color.White, color);
                g.FillEllipse(B, 0, 0, mSize.Height - 2, mSize.Height - 2);
                g.DrawEllipse(new System.Drawing.Pen(color), 0, 0, mSize.Height - 2, mSize.Height - 2);
                return Bm;
            }
            catch
            {
                return Bm;
            }
        }

    }

}
