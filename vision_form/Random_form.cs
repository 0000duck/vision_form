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
    public partial class Random_form : Form
    {
        private HObject image_show;
        private UnitRandom random_data;

        public Random_form(UnitRandom rand, HObject image)
        {
            InitializeComponent();
            random_data = rand;
            image_show = image;
        }

        private void Random_form_Load(object sender, EventArgs e)
        {
            load_parm();
        }

        private void load_parm()
        {
            txtMin.Text = random_data.MinValue.ToString();
            txtMax.Text = random_data.MaxValue.ToString();
        }

        private void button_save_Click(object sender, EventArgs e)
        {
            random_data.MinValue = Convert.ToInt32(txtMin.Text);
            random_data.MaxValue = Convert.ToInt32(txtMax.Text);

            Close();
        }

        private void button_exit_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
