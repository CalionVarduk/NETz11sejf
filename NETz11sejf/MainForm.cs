using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NETz11sejf
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();

            GameResolution.Width = pBox.Width;
            GameResolution.Height = pBox.Height;
            PlayerControls.setKeyboardEvents(this);
            GameController.OnEscape += Close;

            timer.Start();
        }

        private void pBox_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            GameController.drawGame(e.Graphics);
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            if (!Focused) Focus();
            GameController.updateGame();
            pBox.Invalidate();
        }
    }
}
