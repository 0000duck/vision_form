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
    public partial class FindLine_form : Form
    {
        HObject image_show = null;
        HTuple Hwin = null, Hwin_tool = null;
        HTuple Hrow, Hcol, Hphi, L1, L2;
        HTuple data_row, data_col, data_phi;
        HTuple Hnum, Hwid, Hamp, Hsomth;
        HTuple Htransition, Hselect;
        HTuple Hrow1, Hcol1, Hrow2, Hcol2;
        HTuple Hwidth, Hheight;
        HTuple HmouseX, HmouseY;
        //移动图片用
        System.Drawing.Point p1 = new System.Drawing.Point();
        System.Drawing.Point p2 = new System.Drawing.Point();
        double row_rate_hight, col_rate_width;
        bool isMouseDown = false;
        HTuple Hdata;
        UnitFindLine FL_data;

        
        //绘制bool
        bool draw_falg = false;
        HObject Htemp_roi = null;
        public FindLine_form(UnitFindLine FL, HObject image)
        {
            HOperatorSet.GenEmptyObj(out image_show);
            HOperatorSet.GenEmptyObj(out Htemp_roi);
            image_show = image;
            FL_data = FL;
            InitializeComponent();
            
        }
        private void FindLine_form_Load(object sender, EventArgs e)
        {
            
            HOperatorSet.OpenWindow(0, 0, pictureBox1.Width, pictureBox1.Height, pictureBox1.Handle, "", "", out Hwin_tool);
            Hwin = Hwin_tool;
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
            try
            {
                
                if (Refresh_parm(FL_data))
                {
                    if(FL_data.FindLine(image_show))
                    {
                        show_pic_roi();
                        FL_data.DrawRecord(Hwin);
                    }  
                }
            }
            catch
            {

            }
        }
        private void draw_box_Click(object sender, EventArgs e)
        {
            draw_box.Enabled = false;
            draw_falg = true;       
        }
        private void button8_Click(object sender, EventArgs e)
        {
            if (draw_falg)
            {
                button8.Enabled = false;
                this.pictureBox1.Focus();
                HDevWindowStack.SetActive(Hwin);
                Htemp_roi.Dispose();
                HOperatorSet.DrawRectangle2(Hwin, out Hrow, out Hcol, out Hphi, out L1, out L2);
                HOperatorSet.TupleInt(Hrow, out Hrow);
                HOperatorSet.TupleInt(Hcol, out Hcol);
                //HOperatorSet.TupleInt(Hphi, out Hphi);
                HOperatorSet.TupleInt(L1, out L1);
                HOperatorSet.TupleInt(L2, out L2);
                HOperatorSet.GenRectangle2(out Htemp_roi, Hrow, Hcol, Hphi, L1, L2);
                HOperatorSet.DispObj(Htemp_roi, Hwin);
            }
            if (Refresh_parm(FL_data))
            {
                if (FL_data.FindLine(image_show))
                {
                    show_pic_roi();
                    FL_data.DrawRecord(Hwin);
                }
            }
            draw_falg = false;
            draw_box.Enabled = true;
            button8.Enabled = true;
        }
        private void delete_roi_Click(object sender, EventArgs e)
        {
            Htemp_roi.Dispose();
            try
            {
                HOperatorSet.ClearWindow(Hwin);
                HOperatorSet.DispObj(image_show, Hwin);
            }
            catch
            {

            }
        }
        
        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            NUM_UpDown.Value = trackBar1.Value;
            NUM_UpDown.Refresh();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            try
            {
                data_row = Convert.ToInt32(textBox1.Text);
            }
            catch (System.Exception ex)
            {
            	
            }
            

            if (Refresh_parm(FL_data))
            {
                if (FL_data.FindLine(image_show))
                {
                    show_pic_roi();
                    FL_data.DrawRecord(Hwin);
                }
            }
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            try
            {
                data_col = Convert.ToInt32(textBox2.Text);
            }
            catch (System.Exception ex)
            {

            }

            if (Refresh_parm(FL_data))
            {
                if (FL_data.FindLine(image_show))
                {
                    show_pic_roi();
                    FL_data.DrawRecord(Hwin);
                }
            }
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            try
            {
                data_phi = Convert.ToInt32(textBox3.Text);
            }
            catch (System.Exception ex)
            {

            }
            if (Refresh_parm(FL_data))
            {
                if (FL_data.FindLine(image_show))
                {
                    show_pic_roi();
                    FL_data.DrawRecord(Hwin);
                }
            }
        }

        

        private void trackBar2_Scroll(object sender, EventArgs e)
        {
            Wid_UpDown.Value = trackBar2.Value;
            Wid_UpDown.Refresh();
        }

        private void trackBar3_Scroll(object sender, EventArgs e)
        {
            Amp_UpDown.Value = trackBar3.Value;
            Amp_UpDown.Refresh();
        }

        private void trackBar4_Scroll(object sender, EventArgs e)
        {
            Som_UpDown.Value = (decimal)0.1 * trackBar4.Value;
            Som_UpDown.Refresh();
        }
        private void NUM_UpDown_ValueChanged(object sender, EventArgs e)
        {
            Hnum = (int)NUM_UpDown.Value;
            trackBar1.Value = (int)NUM_UpDown.Value;
            if (Refresh_parm(FL_data))
            {
                if (FL_data.FindLine(image_show))
                {
                    show_pic_roi();
                    FL_data.DrawRecord(Hwin);
                }
            }
            
        }

        private void Wid_UpDown_ValueChanged(object sender, EventArgs e)
        {
            Hwid = (int)Wid_UpDown.Value;
            trackBar2.Value = (int)Wid_UpDown.Value;
            if (Refresh_parm(FL_data))
            {
                if (FL_data.FindLine(image_show))
                {
                    show_pic_roi();
                    FL_data.DrawRecord(Hwin);
                }
            }
        }

        private void Amp_UpDown_ValueChanged(object sender, EventArgs e)
        {
            Hamp = (int)Amp_UpDown.Value;
            trackBar3.Value = (int)Amp_UpDown.Value;
            if (Refresh_parm(FL_data))
            {
                if (FL_data.FindLine(image_show))
                {
                    show_pic_roi();
                    FL_data.DrawRecord(Hwin);
                }
            }
        }

        private void Som_UpDown_ValueChanged(object sender, EventArgs e)
        {
            Hsomth = (double)Som_UpDown.Value;
            trackBar4.Value = (int)(Som_UpDown.Value*10);
            if (Refresh_parm(FL_data))
            {
                if (FL_data.FindLine(image_show))
                {
                    show_pic_roi();
                    FL_data.DrawRecord(Hwin);
                }
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            Htransition = comboBox1.Text;
            if (Refresh_parm(FL_data))
            {
                if (FL_data.FindLine(image_show))
                {
                    show_pic_roi();
                    FL_data.DrawRecord(Hwin);
                }
            }
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            Hselect = comboBox2.Text;
            if (Refresh_parm(FL_data))
            {
                if (FL_data.FindLine(image_show))
                {
                    show_pic_roi();
                    FL_data.DrawRecord(Hwin);
                }
            }
        }

        private void load_parm()
        {
            if(FL_data.in_row != null)
            {
                Hrow = FL_data.in_row;
                Hcol = FL_data.in_col;
                Hphi = FL_data.in_phi;
                L1 = FL_data.L1;
                L2 = FL_data.L2;               
            }
            Hnum = FL_data.Hnum;
            Hwid = FL_data.Hwid;
            Hamp = FL_data.Hamp;
            Hsomth = FL_data.Hsomth;
            Htransition = FL_data.Htransition;
            Hselect = FL_data.Hselect;
            data_row = FL_data.data_row;
            data_col = FL_data.data_col;
            data_phi = FL_data.data_phi;

            NUM_UpDown.Value = (decimal)Hnum[0].I;
            trackBar1.Value = (int)NUM_UpDown.Value;
            Wid_UpDown.Value = (decimal)Hwid[0].I;
            trackBar2.Value = (int)Wid_UpDown.Value;
            Amp_UpDown.Value = (decimal)Hamp[0].I;
            trackBar3.Value = (int)Amp_UpDown.Value;
            Som_UpDown.Value = (decimal)Hsomth[0].D;
            trackBar4.Value = (int)(Som_UpDown.Value * 10);
            comboBox1.Text = Htransition;
            comboBox2.Text = Hselect;
            textBox1.Text = data_row.ToString();
            textBox2.Text = data_col.ToString();
            textBox3.Text = data_phi.ToString();
        }
        private bool Refresh_parm(UnitFindLine FL_data)
        {
            FL_data.data_row = data_row;
            FL_data.data_col = data_col;
            FL_data.data_phi = data_phi;
            FL_data.in_row = Hrow;
            FL_data.in_col = Hcol;
            FL_data.in_phi = Hphi;
            FL_data.L1 = L1;
            FL_data.L2 = L2;
            

            FL_data.Hnum = Hnum;
            FL_data.Hwid = Hwid;
            FL_data.Hamp = Hamp;
            FL_data.Hsomth = Hsomth;
            FL_data.Htransition = Htransition;
            FL_data.Hselect = Hselect;
            if (Hrow == null)
            {
                return false;
            }
            return true;
        }

        private void show_pic_roi()
        {
            try
            {
                HOperatorSet.DispObj(image_show, Hwin);
                Htemp_roi.Dispose();
                HOperatorSet.GenRectangle2(out Htemp_roi, Hrow, Hcol, Hphi, L1, L2);
                HOperatorSet.DispObj(Htemp_roi, Hwin);
            }
            catch (System.Exception ex)
            {
            	
            }
            
        }
        private void pictureBox1_MouseEnter(object sender, EventArgs e)
        {
            this.pictureBox1.Focus();
        }
        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (!draw_falg)
                {
                    try
                    {
                        HOperatorSet.GetImageSize(image_show, out Hwidth, out Hheight);
                        HOperatorSet.SetPart(Hwin, 0, 0, Hheight, Hwidth);
                        HOperatorSet.DispObj(image_show, Hwin);
                        HOperatorSet.DispObj(Htemp_roi, Hwin);
                        //HOperatorSet.DispObj(H_show_roi2, Hwin);
                    }
                    catch
                    {

                    }
                }

            }
            else
            {
                isMouseDown = true;
                HOperatorSet.GetPart(Hwin, out Hrow1, out Hcol1, out Hrow2, out Hcol2);
                row_rate_hight = 1.0 * (Hrow2 - Hrow1) / pictureBox1.Height;
                col_rate_width = 1.0 * (Hcol2 - Hcol1) / pictureBox1.Width;
                p1 = PointToClient(Control.MousePosition);//记录鼠标坐标
                //p2 = pictureBox1.Location;                //记录图片坐标
            }
        }
        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            isMouseDown = false;
        }
        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            //鼠标坐标的相对改变值
            int a = (int)((PointToClient(Control.MousePosition).X - p1.X) * col_rate_width);
            int b = (int)((PointToClient(Control.MousePosition).Y - p1.Y) * row_rate_hight);
            //图片坐标计算&赋值
            //图片新的坐标 = 图片起始坐标 + 鼠标相对位移
            try
            {
                if (isMouseDown && !draw_falg)
                {
                    HOperatorSet.SetPart(Hwin, Hrow1 - b, Hcol1 - a, Hrow2 - b, Hcol2 - a);
                    HOperatorSet.DispObj(image_show, Hwin);
                    HOperatorSet.DispObj(Htemp_roi, Hwin);
                }
            }
            catch
            {

            }
            //pictureBox1.Location = new System.Drawing.Point(p2.X + a, p2.Y + b);
        }
        private void pictureBox1_MouseWheel(object sender, MouseEventArgs e)
        {


            if (!draw_falg)
            {
                HDevWindowStack.SetActive(Hwin);
                HOperatorSet.GetPart(Hwin, out Hrow1, out Hcol1, out Hrow2, out Hcol2);
                HOperatorSet.GetImageSize(image_show, out Hwidth, out Hheight);

                HmouseX = Hcol1 + e.X * (Hcol2 - Hcol1) / pictureBox1.Width;
                HmouseY = Hrow1 + e.Y * (Hrow2 - Hrow1) / pictureBox1.Height;
                if (e.Delta > 0)//向上滚
                {

                    try
                    {
                        Hdata = 1.2 * (Hrow2 - Hrow1);
                        Hrow1 = HmouseY - e.Y * Hdata / pictureBox1.Height; ;
                        Hrow2 = Hrow1 + Hdata;

                        Hdata = 1.2 * (Hcol2 - Hcol1);
                        Hcol1 = HmouseX - e.X * Hdata / pictureBox1.Width;
                        Hcol2 = Hcol1 + Hdata;
                        if (Hrow1.D >= 0 && Hcol1.D >= 0 && Hrow2.D <= Hheight && Hcol2.D <= Hwidth)
                        {
                            HOperatorSet.SetPart(Hwin, Hrow1, Hcol1, Hrow2, Hcol2);

                        }
                        else
                        {
                            HOperatorSet.SetPart(Hwin, 0, 0, Hheight, Hwidth);
                        }
                        HOperatorSet.DispObj(image_show, Hwin);
                        HOperatorSet.DispObj(Htemp_roi, Hwin);
                        //HOperatorSet.DispObj(H_show_roi2, Hwin);
                    }
                    catch { }

                }
                else
                {
                    try
                    {
                        Hdata = 0.8 * (Hrow2 - Hrow1);

                        Hrow1 = HmouseY - e.Y * Hdata / pictureBox1.Height;
                        Hrow2 = Hrow1 + Hdata;

                        Hdata = 0.8 * (Hcol2 - Hcol1);
                        Hcol1 = HmouseX - e.X * Hdata / pictureBox1.Width;
                        Hcol2 = Hcol1 + Hdata;
                        if (Hrow2.D - Hrow1.D >= 10 && Hcol2.D - Hcol1.D >= 10)
                        {
                            HOperatorSet.SetPart(Hwin, Hrow1, Hcol1, Hrow2, Hcol2);
                        }
                        HOperatorSet.DispObj(image_show, Hwin);
                        HOperatorSet.DispObj(Htemp_roi, Hwin);
                        //HOperatorSet.DispObj(H_show_roi2, Hwin);
                    }
                    catch { }
                }
            }

        }
    }
}
