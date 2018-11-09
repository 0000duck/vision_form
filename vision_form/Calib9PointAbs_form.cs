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

namespace vision_form
{
    public partial class Calib9PointAbs_form : Form
    {
        HObject image_show = null;
        HTuple Hwin = null;
        HTuple Hwidth, Hheight;

        private UnitCalib9PointAbs calibData;



        public Calib9PointAbs_form(UnitCalib9PointAbs calib, HObject image)
        {
            InitializeComponent();

            this.calibData = calib;
            this.image_show = image;
        }

        private void Calib9PointAbs_form_Load(object sender, EventArgs e)
        {
            HOperatorSet.OpenWindow(0, 0, pictureBox1.Width, pictureBox1.Height, pictureBox1.Handle, "", "", out Hwin);
            HDevWindowStack.Push(Hwin);

            HOperatorSet.SetSystem("clip_region", "false");
            HOperatorSet.SetDraw(Hwin, "margin");
            HOperatorSet.SetColor(Hwin, "red");
            load_parm();
            try
            {
                HOperatorSet.GetImageSize(image_show, out Hwidth, out Hheight);
                HOperatorSet.SetPart(Hwin, 0, 0, Hheight, Hwidth);
                HOperatorSet.DispObj(image_show, Hwin);
            }
            catch
            {

            }
        }

        private void chkCalibRC_CheckedChanged(object sender, EventArgs e)
        {
            if (chkCalibRC.Checked)
            {
                //calibData.EnableRotateCenter = true;
                txtAngleRange.Enabled = true;
            }
            else
            {
                //calibData.EnableRotateCenter = false;
                txtAngleRange.Enabled = false;
            }
        }

        private void btnDir_Click(object sender, EventArgs e)
        {
            //if (txtName.Text == "")
            //{
            //    MessageBox.Show("数据名称不能为空");
            //    return;
            //}

            FolderBrowserDialog fbd = new FolderBrowserDialog();

            if (fbd.ShowDialog() == DialogResult.OK)
            {
                txtDir.Text = fbd.SelectedPath + "\\";
                //calibData.CalibDataFileName = txtDir.Text + txtName.Text;
            }
        }

        private void txtOffsetXY_TextChanged(object sender, EventArgs e)
        {
            //calibData.OffsetXY = Convert.ToInt32(txtOffsetXY.Text);
        }

        private void txtAngleRange_TextChanged(object sender, EventArgs e)
        {
            //calibData.AngleRange = Convert.ToInt32(txtAngleRange.Text);
        }

        private void txtName_TextChanged(object sender, EventArgs e)
        {
            //calibData.CalibDataFileName = txtDir.Text + txtName.Text;
        }

        private void button_save_Click(object sender, EventArgs e)
        {
            if (txtOffsetXY.Text.Trim() == "" || txtOffsetXY.Text.Trim() == "0")
            {
                MessageBox.Show("XY偏移值不能为空");
                return;
            }

            if (chkCalibRC.Checked && (txtAngleRange.Text.Trim() == "" || txtAngleRange.Text.Trim() == "0"))
            {
                MessageBox.Show("角度范围不能为空");
                return;
            }

            if (txtName.Text.Trim() == "")
            {
                MessageBox.Show("数据名称不能为空");
                return;
            }

            
            
            if (calibData.EnableRotateCenter = chkCalibRC.Checked)
            {
                calibData.AngleRange = Convert.ToInt32(txtAngleRange.Text.Trim());
            }
            calibData.OffsetXY = Convert.ToInt32(txtOffsetXY.Text.Trim());
            calibData.CalibDataFileName = txtDir.Text.Trim() + txtName.Text.Trim();

            Close();
        }

        private void button_exit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnTest_Click(object sender, EventArgs e)
        {
            int length = dataGridView1.Rows.Count - 1;

            if (length == 9)
            {
                calibData.EnableRotateCenter = false;
            }
            else if (length == 15)
            {
                calibData.EnableRotateCenter = true;
            }
            else
            {
                MessageBox.Show("数据量有误");
                return;
            }

            calibData.ClearData();

            for (int i = 0; i < length; i++)
            {
                calibData.in_pixel_column.Append(Convert.ToDouble(dataGridView1.Rows[i].Cells[1].Value));
                calibData.in_pixel_row.Append(Convert.ToDouble(dataGridView1.Rows[i].Cells[2].Value));
                calibData.in_world_x.Append(Convert.ToDouble(dataGridView1.Rows[i].Cells[3].Value));
                calibData.in_world_y.Append(Convert.ToDouble(dataGridView1.Rows[i].Cells[4].Value));

            }

            calibData.PointCount = 9;
            calibData.process(null, null);
            calibData.PointCount = 16;
            calibData.process(null, null);

            load_parm();
        }

        private void load_parm()
        {
            dataGridView1.Rows.Clear();

            int length = calibData.in_pixel_row.Length;
            for (int i = 0; i < length; i++)
            {
                int row = dataGridView1.Rows.Add();

                dataGridView1.Rows[row].Cells[0].Value = row + 1;
                dataGridView1.Rows[row].Cells[1].Value = calibData.in_pixel_column.ToDArr()[i];
                dataGridView1.Rows[row].Cells[2].Value = calibData.in_pixel_row.ToDArr()[i];
                dataGridView1.Rows[row].Cells[3].Value = calibData.in_world_x.ToDArr()[i];
                dataGridView1.Rows[row].Cells[4].Value = calibData.in_world_y.ToDArr()[i];

            }
            
            txtOffsetXY.Text = calibData.OffsetXY.ToString();
            txtAngleRange.Text = calibData.AngleRange.ToString();
            chkCalibRC.Checked = calibData.EnableRotateCenter;

            if (chkCalibRC.Checked)
            {
                txtAngleRange.Enabled = true;
            }
            else
            {
                txtAngleRange.Enabled = false;
            }

            int index = calibData.CalibDataFileName.LastIndexOf("\\");

            if (index >= 0)
            {
                txtName.Text = calibData.CalibDataFileName.Substring(index + 1);
                txtDir.Text = calibData.CalibDataFileName.Substring(0, index + 1);
            }
            else
            {
                txtName.Text = calibData.CalibDataFileName;
                txtDir.Text = "";
            }

            double[] homMat2D = calibData.HomMat2D.ToDArr();

            labHomMat2D.Text = "HomMat2D:\r\n" + homMat2D[0] + "  " + homMat2D[1] + "  " + homMat2D[2] + "\r\n" +
                homMat2D[3] + "  " + homMat2D[4] + "  " + homMat2D[5] + "\r\n" + "0  0  1";

            labMaxDeviation.Text = "最大偏差(x, y)：(" + calibData.XMaxDeviation + ", " + calibData.YMaxDeviation + ")";

            labRotateCenter.Text = "旋转中心(row, column)：(" + calibData.CenterRow + ", " + calibData.CenterColumn + ")";
        }
    }
}
