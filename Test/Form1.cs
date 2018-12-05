using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HalconDotNet;
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

        private void button5_Click(object sender, EventArgs e)
        {
            double[] row = new double[] { 300, 500, 700, 300, 500, 700, 300, 500, 700 };
            double[] column = new double[] { 100, 100, 100, 200, 200, 200, 300, 300, 300 };

            double[] x = new double[] { 1000, 3000, 5000, 1000, 3000, 5000, 1000, 3000, 5000 };
            double[] y = new double[] { 1000, 1000, 1000, 2000, 2000, 2000, 3000, 3000, 3000 };

            double[] r = new double[] { 50, 100, 150 };
            double[] c = new double[] { 100, 50, 100 };

            calib1.CalibNinePoint(row, column);
            calib1.CalibRotationCenter(r, c);

            calib2.CalibNinePoint(row, column, x, y);

            //HTuple ro, co, qx, qy, qx2, qy2, angle2;
            //calib1.GetRotatedPose(0, 0, 0, 90, out ro, out co);
            //calib1.ImageToWorldPose(300, 100, out qx, out qy);

            //calib2.GetWorldPose(700, 300, 0, null, null, 0, out qx2, out qy2, out angle2);
        }
    }
}
