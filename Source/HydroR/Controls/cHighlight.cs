using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.Design;


namespace HydroR
{
    public class cHighlight : Control
    {
        
        private int opacity = 100;
        private int alpha;

        public cHighlight()
        {
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            SetStyle(ControlStyles.Opaque, true);
            BackColor = Color.Transparent;           
            this.MouseUp += new MouseEventHandler(this.h_MouseUp);
        }
        public event EventHandler Changed;
        public delegate void EventHandler(Point Location);


        private void h_MouseUp(object sender, MouseEventArgs e)
        {
            if (Changed != null)
            {
                Changed(e.Location);
            }
        }
        public int Opacity
        {
            get
            {
                if (opacity > 100) { opacity = 100; }
                else if (opacity < 1) { opacity = 1; }
                return opacity;
            }
            set
            {
                opacity = value;                
            }
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x20;
                return cp;
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            Rectangle bounds = new Rectangle(0, 0, this.Width - 1, this.Height - 1);

            Color frmColor = this.Parent.BackColor;

            Brush bckColor;

            alpha = (opacity * 255) / 100;
            bckColor = new SolidBrush(Color.FromArgb(alpha, BackColor));

            if (BackColor != Color.Transparent )
            {
                g.FillRectangle(bckColor, bounds);
            }
           
            bckColor.Dispose();
            g.Dispose();
        }

    }
    

}
