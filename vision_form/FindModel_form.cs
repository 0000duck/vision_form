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
    public partial class FindModel_form : Form
    {
        //缩放移动窗口变量
        HTuple Hrow1, Hcol1, Hrow2, Hcol2;
        HTuple Hwidth, Hheight;
        HTuple HmouseX, HmouseY;
        HTuple Hdata;
        //绘制bool
        bool draw_falg = false;
        bool isMouseDown = false;
        //移动图片用
        System.Drawing.Point p1 = new System.Drawing.Point();
        System.Drawing.Point p2 = new System.Drawing.Point();
        double row_rate_hight, col_rate_width;
        //
        HObject image_show = null, Htemp_roi = null, Hdraw_roi = null, Hshow_xld1 = null, Hshow_xld2 = null;
        HObject m_hModelXLD;
        HTuple Hwin = null, Hwin_tool = null;
        UnitFindModel FM_data;
        //起始角度，角度范围，最小放大比例，最大放大比例，
        //对比度（低），对比度（高），最小组件尺寸，最小对比度
        //金字塔层数，角度步长，放大比例步长，最优化，度量，//默认值，不让调整
        HTuple HangleStart, HangleExtent, HscaleMin, HscaleMax;
        HTuple Hcontrast, HminContrast, ContrastMin, ContrastMax, MinSize;//第一个是，最后三个的集合
        HTuple H_NumLevels, H_AngleStep, H_ScaleStep, H_Optimization, H_Metric;//默认值，不让调整,现在可以不用,方便以后开发应用

       

        //最小分数，最大重叠，贪婪度
        //查找个数，亚像素，//默认值，不让调整
        HTuple HminScore, HmaxOverlap, Hgreediness;

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            UpDown1.Value = trackBar1.Value;
            UpDown1.Refresh();
        }

        private void trackBar2_Scroll(object sender, EventArgs e)
        {
            UpDown2.Value = trackBar2.Value;
            UpDown2.Refresh();
        }

        private void trackBar3_Scroll(object sender, EventArgs e)
        {
            UpDown3.Value = trackBar3.Value;
            UpDown3.Refresh();
        }

        private void trackBar4_Scroll(object sender, EventArgs e)
        {
            UpDown4.Value = trackBar4.Value;
            UpDown4.Refresh();
        }

        private void trackBar5_Scroll(object sender, EventArgs e)
        {
            UpDown5.Value = trackBar5.Value;
            UpDown5.Refresh();
        }

        private void trackBar6_Scroll(object sender, EventArgs e)
        {
            UpDown6.Value = trackBar6.Value;
            UpDown6.Refresh();
        }

        private void trackBar7_Scroll(object sender, EventArgs e)
        {
            UpDown7.Value = trackBar7.Value;
            UpDown7.Refresh();
        }

        private void trackBar8_Scroll(object sender, EventArgs e)
        {
            UpDown8.Value = trackBar8.Value;
            UpDown8.Refresh();
        }

        private void trackBar9_Scroll(object sender, EventArgs e)
        {
            UpDown9.Value = (decimal)(trackBar9.Value*0.1);
            UpDown9.Refresh();
        }

        private void trackBar10_Scroll(object sender, EventArgs e)
        {
            UpDown10.Value = (decimal)(trackBar10.Value*0.1);
            UpDown10.Refresh();
        }

        private void trackBar11_Scroll(object sender, EventArgs e)
        {
            UpDown11.Value = (decimal)(trackBar11.Value*0.1);
            UpDown11.Refresh();
        }

        private void UpDown1_ValueChanged(object sender, EventArgs e)
        {
            HangleStart = (int)UpDown1.Value;
            trackBar1.Value = (int)UpDown1.Value;
            if (Hdraw_roi.Key != (IntPtr)0)
            {
                create_display_model();
            }
        }

        private void UpDown2_ValueChanged(object sender, EventArgs e)
        {
            HangleExtent = (int)UpDown2.Value;
            trackBar2.Value = (int)UpDown2.Value;
            if (Hdraw_roi.Key != (IntPtr)0)
            {
                create_display_model();
            }
        }

        private void UpDown3_ValueChanged(object sender, EventArgs e)
        {
            HscaleMin = (double)UpDown3.Value;
            trackBar3.Value = (int)(UpDown3.Value*10);
            if (Hdraw_roi.Key != (IntPtr)0)
            {
                create_display_model();
            }
        }

        private void UpDown4_ValueChanged(object sender, EventArgs e)
        {
            HscaleMax = (double)UpDown4.Value;
            trackBar4.Value = (int)(UpDown4.Value * 10);
            if (Hdraw_roi.Key != (IntPtr)0)
            {
                create_display_model();
            }
        }

        private void UpDown5_ValueChanged(object sender, EventArgs e)
        {
            ContrastMin = (int)UpDown5.Value;
            trackBar5.Value = (int)(UpDown5.Value);
            Hcontrast = new HTuple();
            Hcontrast = Hcontrast.TupleConcat(ContrastMin);
            Hcontrast = Hcontrast.TupleConcat(ContrastMax);
            Hcontrast = Hcontrast.TupleConcat(MinSize);
            if (Hdraw_roi.Key != (IntPtr)0)
            {
                create_display_model();
            }
        }

        private void UpDown6_ValueChanged(object sender, EventArgs e)
        {
            ContrastMax = (int)UpDown6.Value;
            trackBar6.Value = (int)(UpDown6.Value);
            Hcontrast = new HTuple();
            Hcontrast = Hcontrast.TupleConcat(ContrastMin);
            Hcontrast = Hcontrast.TupleConcat(ContrastMax);
            Hcontrast = Hcontrast.TupleConcat(MinSize);
            if (Hdraw_roi.Key != (IntPtr)0)
            {
                create_display_model();
            }
        }

        private void UpDown7_ValueChanged(object sender, EventArgs e)
        {
            MinSize = (int)UpDown7.Value;
            trackBar7.Value = (int)(UpDown7.Value);
            Hcontrast = new HTuple();
            Hcontrast = Hcontrast.TupleConcat(ContrastMin);
            Hcontrast = Hcontrast.TupleConcat(ContrastMax);
            Hcontrast = Hcontrast.TupleConcat(MinSize);
            if (Hdraw_roi.Key != (IntPtr)0)
            {
                create_display_model();
            }
        }

        private void UpDown8_ValueChanged(object sender, EventArgs e)
        {
            HminContrast = (int)UpDown8.Value;
            trackBar8.Value = (int)(UpDown8.Value);
            if (Hdraw_roi.Key != (IntPtr)0)
            {
                create_display_model();
            }
        }

        private void UpDown9_ValueChanged(object sender, EventArgs e)
        {
            HminScore = (double)UpDown9.Value;
            trackBar9.Value = (int)(UpDown9.Value * 10);
            if (Refresh_parm(FM_data))
            {
                if (FM_data.FindModel(image_show))
                {
                    FM_data.DrawRecord(Hwin);
                }
            }
        }

        private void UpDown10_ValueChanged(object sender, EventArgs e)
        {
            HmaxOverlap = (double)UpDown10.Value;
            trackBar10.Value = (int)(UpDown10.Value * 10);
            if (Refresh_parm(FM_data))
            {
                if (FM_data.FindModel(image_show))
                {
                    FM_data.DrawRecord(Hwin);
                }
            }
        }

        private void UpDown11_ValueChanged(object sender, EventArgs e)
        {
            Hgreediness = (double)UpDown11.Value;
            trackBar11.Value = (int)(UpDown11.Value * 10);
            if (Refresh_parm(FM_data))
            {
                if (FM_data.FindModel(image_show))
                {
                    FM_data.DrawRecord(Hwin);
                }
            }
        }

        HTuple H_NumMatches, H_SubPixel;//默认值，不让调整,现在可以不用,方便以后开发应用
        HTuple HShapeModelID = null;
        string modelpath = null;
        //
        HTuple Hrow, Hcol, Hradius;
        HTuple Hphi, HL1, HL2;
        HObject[] OTemp = new HObject[20];
        private void draw_box_Click(object sender, EventArgs e)
        {
            draw_box.Enabled = false;
            draw_falg = true;
        }

        private void button10_Click(object sender, EventArgs e)
        {
            if (draw_falg)
            {
                button10.Enabled = false;
                this.pictureBox1.Focus();
                HDevWindowStack.SetActive(Hwin);
                Htemp_roi.Dispose();
                HOperatorSet.DrawCircle(Hwin, out Hrow, out Hcol, out Hradius);
                HOperatorSet.TupleInt(Hrow, out Hrow);
                HOperatorSet.TupleInt(Hcol, out Hcol);
                HOperatorSet.GenCircle(out Htemp_roi, Hrow, Hcol, Hradius);

                display_draw(Htemp_roi, ref Hdraw_roi);
                create_display_model();
            }
            draw_falg = false;
            draw_box.Enabled = true;
            button10.Enabled = true;
        }

        private void button9_Click(object sender, EventArgs e)
        {
            if (draw_falg)
            {
                button9.Enabled = false;
                this.pictureBox1.Focus();
                HDevWindowStack.SetActive(Hwin);
                Htemp_roi.Dispose();
                HOperatorSet.DrawRectangle1(Hwin, out Hrow1, out Hcol1, out Hrow2, out Hcol2);
                HOperatorSet.TupleInt(Hrow1, out Hrow1);
                HOperatorSet.TupleInt(Hcol1, out Hcol1);
                HOperatorSet.TupleInt(Hrow2, out Hrow2);
                HOperatorSet.TupleInt(Hcol2, out Hcol2);
                HOperatorSet.GenRectangle1(out Htemp_roi, Hrow1, Hcol1, Hrow2, Hcol2);

                display_draw(Htemp_roi, ref Hdraw_roi);
                create_display_model();
            }
            draw_falg = false;
            draw_box.Enabled = true;
            button9.Enabled = true;
        }

        private void button8_Click(object sender, EventArgs e)
        {
            if (draw_falg)
            {
                button8.Enabled = false;
                this.pictureBox1.Focus();
                HDevWindowStack.SetActive(Hwin);
                Htemp_roi.Dispose();
                HOperatorSet.DrawRectangle2(Hwin, out Hrow, out Hcol, out Hphi, out HL1, out HL2);
                HOperatorSet.TupleInt(Hrow, out Hrow);
                HOperatorSet.TupleInt(Hcol, out Hcol);
                HOperatorSet.TupleInt(HL1, out HL1);
                HOperatorSet.TupleInt(HL2, out HL2);
                HOperatorSet.GenRectangle2(out Htemp_roi, Hrow, Hcol, Hphi, HL1, HL2);

                display_draw(Htemp_roi, ref Hdraw_roi);
                create_display_model();
            }
            draw_falg = false;
            draw_box.Enabled = true;
            button8.Enabled = true;
        }

        private void delete_roi_Click(object sender, EventArgs e)
        {
            Htemp_roi.Dispose();
            Hdraw_roi.Dispose();
            Hshow_xld1.Dispose();
            Hshow_xld2.Dispose();
            try
            {
                HOperatorSet.ClearWindow(Hwin);
                HOperatorSet.DispObj(image_show, Hwin);
            }
            catch
            {

            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            saveFileDialog1.InitialDirectory = "D:\\";
            saveFileDialog1.Filter = "SHM文件|*.shm";
            if (modelpath != null)
            {
                saveFileDialog1.InitialDirectory = modelpath;
            }
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                modelpath = saveFileDialog1.FileName;
                try
                {
                    HOperatorSet.WriteShapeModel(HShapeModelID, modelpath);
                    MessageBox.Show("模板保存成功");
                }
                catch
                {
                    MessageBox.Show("模板保存失败");
                }
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            openFileDialog1.InitialDirectory = "D:\\";
            openFileDialog1.Filter = "模板|*.shm";
            if (modelpath != null)
            {
                openFileDialog1.InitialDirectory = modelpath;
            }
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                textBox3.Text = openFileDialog1.FileName; ;
                modelpath = textBox3.Text;
                try
                {
                    HOperatorSet.ReadShapeModel(modelpath, out HShapeModelID);
                    if (Refresh_parm(FM_data))
                    {
                        if (FM_data.FindModel(image_show))
                        {
                            FM_data.DrawRecord(Hwin);
                        }
                    }
                }
                catch (System.Exception ex)
                {
                	
                }
            }
        }
        void display_draw(HObject Htemp_roi, ref HObject Hdraw_roi)//, out HObject Hdraw_roi, out HObject Hshow_xld2)
        {
            Hshow_xld1.Dispose();
            HOperatorSet.GenContourRegionXld(Htemp_roi, out Hshow_xld1, "border");
            if (radioButton1.Checked)
            {
                if (Hdraw_roi.Key == (IntPtr)0)
                {
                    HOperatorSet.CopyObj(Htemp_roi, out Hdraw_roi, 1, 1);
                    HOperatorSet.CopyObj(Hshow_xld1, out Hshow_xld2, 1, 1);
                }
                else
                {
                    HOperatorSet.Union2(Hdraw_roi, Htemp_roi, out Hdraw_roi);
                    HOperatorSet.ConcatObj(Hshow_xld2, Hshow_xld1, out OTemp[0]);
                    Hshow_xld2.Dispose();
                    Hshow_xld2 = OTemp[0];
                }


            }
            else if (radioButton2.Checked)
            {
                if (Hdraw_roi.Key == (IntPtr)0)
                {
                    HOperatorSet.CopyObj(Htemp_roi, out Hdraw_roi, 1, 1);
                    HOperatorSet.CopyObj(Hshow_xld1, out Hshow_xld2, 1, 1);
                }
                else
                {
                    HOperatorSet.Intersection(Hdraw_roi, Htemp_roi, out Hdraw_roi);
                    HOperatorSet.ConcatObj(Hshow_xld2, Hshow_xld1, out OTemp[0]);
                    Hshow_xld2.Dispose();
                    Hshow_xld2 = OTemp[0];
                }
            }
            else if (radioButton3.Checked)
            {
                if (Hdraw_roi.Key == (IntPtr)0)
                {
                    HOperatorSet.CopyObj(Htemp_roi, out Hdraw_roi, 1, 1);
                    HOperatorSet.CopyObj(Hshow_xld1, out Hshow_xld2, 1, 1);
                }
                else
                {
                    HOperatorSet.Difference(Hdraw_roi, Htemp_roi, out Hdraw_roi);
                    HOperatorSet.ConcatObj(Hshow_xld2, Hshow_xld1, out OTemp[0]);
                    Hshow_xld2.Dispose();
                    Hshow_xld2 = OTemp[0];
                }
            }
            else
            {
                return;
            }
            try
            {
                HOperatorSet.ClearWindow(Hwin);
                HOperatorSet.DispObj(image_show, Hwin);

                HOperatorSet.SetColor(Hwin, "green");
                HOperatorSet.DispObj(Hshow_xld2, Hwin);

                HOperatorSet.SetColor(Hwin, "red");
                HOperatorSet.DispObj(Hdraw_roi, Hwin);
            }
            catch (System.Exception ex)
            {

            }
        }
        void create_display_model()
        {
            HObject HImageReduced = null, HModelContours = null;
            HTuple hv_Area, Htemprow, Htempcol, hv_HomMat2D;
            HOperatorSet.GenEmptyObj(out HImageReduced);
            HOperatorSet.GenEmptyObj(out HModelContours);
            try
            {
                HImageReduced.Dispose();
                HOperatorSet.ReduceDomain(image_show, Hdraw_roi, out HImageReduced);
                HOperatorSet.AreaCenter(Hdraw_roi, out hv_Area, out Htemprow, out Htempcol);

                HOperatorSet.CreateScaledShapeModel(HImageReduced, H_NumLevels, HangleStart * 3.1415926 / 180, HangleExtent * 3.1415926 / 180,
                    H_AngleStep, HscaleMin, HscaleMax, H_ScaleStep, H_Optimization, H_Metric,
                    Hcontrast, HminContrast, out HShapeModelID);
                HModelContours.Dispose();
                HOperatorSet.GetShapeModelContours(out HModelContours, HShapeModelID, 1);
                HOperatorSet.VectorAngleToRigid(0, 0, 0, Htemprow, Htempcol, 0, out hv_HomMat2D);
                m_hModelXLD.Dispose();
                HOperatorSet.AffineTransContourXld(HModelContours, out m_hModelXLD, hv_HomMat2D);
                //HOperatorSet.SetColor(Hwin, "blue");
                //HOperatorSet.DispObj(m_hModelXLD, Hwin);
                show_pic_roi();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("创建模板失败");
            }
        }

        public FindModel_form(UnitFindModel FM, HObject image)
        {
            HOperatorSet.GenEmptyObj(out image_show);
            HOperatorSet.GenEmptyObj(out m_hModelXLD);
            HOperatorSet.GenEmptyObj(out Htemp_roi);
            HOperatorSet.GenEmptyObj(out Hdraw_roi);
            HOperatorSet.GenEmptyObj(out Hshow_xld1);
            HOperatorSet.GenEmptyObj(out Hshow_xld2);

            Hdraw_roi.Dispose();
            Hshow_xld2.Dispose();

            image_show = image;
            FM_data = FM;

            HangleStart = -20;
            HangleExtent = 40;
            HscaleMin = 0.9;
            HscaleMax = 1.1;
            ContrastMin = 11;
            ContrastMax = 15;
            MinSize = 10;
            Hcontrast = new HTuple();
            Hcontrast = Hcontrast.TupleConcat(ContrastMin);
            Hcontrast = Hcontrast.TupleConcat(ContrastMax);
            Hcontrast = Hcontrast.TupleConcat(MinSize);
            HminContrast = 9;

            H_NumLevels = 4;
            H_AngleStep = "auto";
            H_ScaleStep = "auto";
            H_Optimization = (new HTuple("none")).TupleConcat("no_pregeneration");
            H_Metric = "use_polarity";

            HminScore = 0.5;
            HmaxOverlap = 0.5;
            Hgreediness = 0.8;

            H_NumMatches = 1;
            H_SubPixel = "least_squares";

            
            

             InitializeComponent();
        }
        private void FindModel_form_Load(object sender, EventArgs e)
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

                if (Refresh_parm(FM_data))
                {
                    if (FM_data.FindModel(image_show))
                    {
                        FM_data.DrawRecord(Hwin);
                    }
                }
            }
            catch
            {

            }
        }
        private void load_parm()
        {
            HangleStart = FM_data.HangleStart;
            HangleExtent = FM_data.HangleExtent;
            HscaleMin = FM_data.HscaleMin;
            HscaleMax = FM_data.HscaleMax;
            ContrastMin = FM_data.ContrastMin;
            ContrastMax = FM_data.ContrastMax;
            MinSize = FM_data.MinSize;
            Hcontrast = new HTuple();
            Hcontrast = Hcontrast.TupleConcat(ContrastMin);
            Hcontrast = Hcontrast.TupleConcat(ContrastMax);
            Hcontrast = Hcontrast.TupleConcat(MinSize);
            HminContrast = FM_data.HminContrast;

            HminScore = FM_data.HminScore;
            HmaxOverlap = FM_data.HmaxOverlap;
            Hgreediness = FM_data.Hgreediness;

            HShapeModelID = FM_data.HShapeModelID;
            modelpath = FM_data.modelpath;

            //int temp = HangleStart[0].I;
            UpDown1.Value = (decimal)HangleStart[0].D;
            trackBar1.Value = (int)UpDown1.Value;
            UpDown2.Value = (decimal)HangleExtent[0].D;
            trackBar2.Value = (int)UpDown2.Value;
            UpDown3.Value = (decimal)HscaleMin[0].D;
            trackBar3.Value = (int)(UpDown3.Value * 10);
            UpDown4.Value = (decimal)HscaleMax[0].D;
            trackBar4.Value = (int)(UpDown4.Value * 10);
            UpDown5.Value = (decimal)ContrastMin[0].I;
            trackBar5.Value = (int)UpDown5.Value;
            UpDown6.Value = (decimal)ContrastMax[0].I;
            trackBar6.Value = (int)UpDown6.Value;
            UpDown7.Value = (decimal)MinSize[0].I;
            trackBar7.Value = (int)UpDown7.Value;
            UpDown8.Value = (decimal)HminContrast[0].I;
            trackBar8.Value = (int)UpDown8.Value;

            UpDown9.Value = (decimal)HminScore[0].D;
            trackBar9.Value = (int)(UpDown9.Value * 10);
            UpDown10.Value = (decimal)HmaxOverlap[0].D;
            trackBar10.Value = (int)(UpDown10.Value * 10);
            UpDown11.Value = (decimal)Hgreediness[0].D;
            trackBar11.Value = (int)(UpDown11.Value * 10);

            textBox3.Text = modelpath;
        }
        private bool Refresh_parm(UnitFindModel FM_data)
        {
            FM_data.HangleStart = HangleStart;
            FM_data.HangleExtent = HangleExtent;
            FM_data.HscaleMin = HscaleMin;
            FM_data.HscaleMax = HscaleMax;
            FM_data.ContrastMin = ContrastMin;
            FM_data.ContrastMax = ContrastMax;
            FM_data.MinSize = MinSize;
            Hcontrast = new HTuple();
            Hcontrast = Hcontrast.TupleConcat(ContrastMin);
            Hcontrast = Hcontrast.TupleConcat(ContrastMax);
            Hcontrast = Hcontrast.TupleConcat(MinSize);
            FM_data.HminContrast = HminContrast;

            FM_data.HminScore = HminScore;
            FM_data.HmaxOverlap = HmaxOverlap;
            FM_data.Hgreediness = Hgreediness;

            FM_data.HShapeModelID = HShapeModelID;
            FM_data.modelpath = modelpath;
            if (HShapeModelID == null)
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
                HOperatorSet.SetColor(Hwin, "red");
                HOperatorSet.DispObj(Hdraw_roi, Hwin);
                HOperatorSet.SetColor(Hwin, "blue");
                HOperatorSet.DispObj(Hshow_xld2, Hwin);
                HOperatorSet.SetColor(Hwin, "green");
                HOperatorSet.DispObj(m_hModelXLD, Hwin);
                HOperatorSet.SetColor(Hwin, "red");
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
                        HOperatorSet.DispObj(Hdraw_roi, Hwin);
                        HOperatorSet.DispObj(Hshow_xld2, Hwin);
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
                    HOperatorSet.DispObj(Hdraw_roi, Hwin);
                    HOperatorSet.DispObj(Hshow_xld2, Hwin);
                }
            }
            catch
            {

            }
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
                        HOperatorSet.DispObj(Hdraw_roi, Hwin);
                        HOperatorSet.DispObj(Hshow_xld2, Hwin);
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
                        HOperatorSet.DispObj(Hdraw_roi, Hwin);
                        HOperatorSet.DispObj(Hshow_xld2, Hwin);
                    }
                    catch { }
                }
            }

        }

    }
}
