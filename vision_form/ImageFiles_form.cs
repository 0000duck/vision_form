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
    public partial class ImageFiles_form : Form
    {
        private UnitImageFiles files_data;
        private HObject image_show;

        public ImageFiles_form(UnitImageFiles files, HObject image)
        {
            InitializeComponent();
            files_data = files;
            image_show = image;
        }

        private void ImageFiles_form_Load(object sender, EventArgs e)
        {
            load_parm();
        }

        private void load_parm()
        {
            txtDir.Text = files_data.Directory;
        }

        private void button_save_Click(object sender, EventArgs e)
        {
            files_data.Directory = txtDir.Text;
            Close();
        }

        private void but_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();

            if (fbd.ShowDialog() == DialogResult.OK)
            {
                txtDir.Text = fbd.SelectedPath.Replace("\\", "/");
            }
        }

        private void button_exit_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
