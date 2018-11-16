using System;
using System.Windows.Forms;

namespace vision_form
{
    public partial class FormSet : Form
    {
        Calibration calib;

        public FormSet(Calibration calibration)
        {
            InitializeComponent();
            calib = calibration;
        }

        private void FormSet_Load(object sender, EventArgs e)
        {
            this.Text += "——" + calib.CalibName;
            txtTranslation.Text = calib.TranslationStep.ToString();
            txtRotation.Text = calib.RotationStep.ToString();
        }

        private void butOK_Click(object sender, EventArgs e)
        {
            if (txtTranslation.Text == "" || txtTranslation.Text == "0" ||
               txtRotation.Text == "" || txtRotation.Text == "0")
            {
                MessageBox.Show("数据不能为空");
                return;
            }

            calib.TranslationStep = Convert.ToDouble(txtTranslation.Text);
            calib.RotationStep = Convert.ToDouble(txtRotation.Text);
            calib.SaveConfig();

            Close();
        }

        private void butCancel_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
