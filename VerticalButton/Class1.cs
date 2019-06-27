using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;



namespace VerticalButton
{
    public class VerticalButton : Button
    {
        public string VerticalText { get; set; }
        private StringFormat fmt = new StringFormat();

        public VerticalButton()
            : base()
        {
            fmt.Alignment = StringAlignment.Center;
            fmt.LineAlignment = StringAlignment.Center;
        }

        protected override void OnPaint(PaintEventArgs pevent)
        {
            base.OnPaint(pevent);
            pevent.Graphics.TranslateTransform(Width, 0);
            pevent.Graphics.RotateTransform(90);
            pevent.Graphics.DrawString(VerticalText, Font, Brushes.Black, new Rectangle(0, 0, Height, Width), fmt);
        }

    }
}
