using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using vision_form;

namespace Test
{
    public partial class Form1 : Form
    {
        Calibration calib1 = new Calibration("MainCamera");
        Calibration calib2 = new Calibration("UpDownCalib");
        Calibration calib3 = new Calibration("SideCalib");

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            calib1.ShowDialog();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            calib1.SaveConfig();
            calib2.SaveConfig();
            calib3.SaveConfig();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            calib2.ShowDialog();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            calib3.ShowDialog();
        }
    }
}
