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
using System.IO;
using System.Collections;

namespace vision_form
{
    public partial class Vision_Form : Form
    {
        HObject image_show = null;
        HTuple Hwin = null, Hwin_tool = null, Hpath;
        HTuple Hrow1, Hcol1, Hrow2, Hcol2;
        HTuple Hwidth, Hheight;
        HTuple HmouseX, HmouseY;
        //移动图片用
        System.Drawing.Point p1 = new System.Drawing.Point();
        System.Drawing.Point p2 = new System.Drawing.Point();
        double row_rate_hight, col_rate_width;
        bool isMouseDown = false;
        HTuple Hdata;
        //载入图片地址
        string image_path = null;
        string step_path = null;
        TreeLoadXml TreeXml;
        //支持20个步骤
        VisionUnitBase[] Vision_step = new VisionUnitBase[20];
        public string[] str_parm = new string[20];
        public string[] str_name = new string[20];
        int step_num;
        public Vision_Form()
        {
            InitializeComponent();
            TreeXml = new TreeLoadXml();
            HOperatorSet.GenEmptyObj(out image_show);
        }
        private void Vision_Form_Load(object sender, EventArgs e)
        {
            HOperatorSet.OpenWindow(0, 0, pictureBox1.Width, pictureBox1.Height, pictureBox1.Handle, "", "", out Hwin_tool);
            Hwin = Hwin_tool;
            HDevWindowStack.Push(Hwin);
            HOperatorSet.SetSystem("clip_region", "false");
            HOperatorSet.SetDraw(Hwin, "margin");
            HOperatorSet.SetColor(Hwin, "red");
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

        private void button3_Click(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode == null)
            {
                MessageBox.Show("请选择要删除的节点！");
                return;
            }
            int num = treeView1.SelectedNode.Index;
            delete_Vision_step(num);
            treeView1.SelectedNode.Remove();
        }
        private void delete_Vision_step(int num)
        {
            int I_temp = 0;
            for (int i = 0; i < 20; i++)
            {
                if (Vision_step[i] != null)
                {
                    I_temp = i;
                }
                else
                {
                    break;
                }
            }
            for (int i = num; i <= I_temp; i++)
            {
                if(i< I_temp)
                {
                    Vision_step[i] = Vision_step[i + 1];
                }
                else
                {
                    Vision_step[i] = null;
                }
            }
        }

       
    


        private void treeView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Point ClickPoint = new Point(e.X, e.Y);
                TreeNode CurrentNode = treeView1.GetNodeAt(ClickPoint);
                if (CurrentNode != null)//判断你点的是不是一个节点
                {
                    if (CurrentNode.Text == "Find_model")
                    {
                        int sel_num = treeView1.SelectedNode.Index;
                        FindModel_form FM_form = new FindModel_form((UnitFindModel)Vision_step[sel_num], image_show);
                        FM_form.ShowDialog();
                        FM_form.Dispose();
                    }
                    else if(CurrentNode.Text == "Find_line")
                    {
                        int sel_num = treeView1.SelectedNode.Index;
                        FindLine_form FL_form = new FindLine_form((UnitFindLine)Vision_step[sel_num], image_show);
                        FL_form.ShowDialog();                        
                        FL_form.Dispose();
                    }
                    else if(CurrentNode.Text == "Find_circle")
                    {
                        int sel_num = treeView1.SelectedNode.Index;
                        FindCircle_form FC_form = new FindCircle_form((UnitFindCircle)Vision_step[sel_num], image_show);
                        FC_form.ShowDialog();
                        FC_form.Dispose();
                    }
                    else if(CurrentNode.Text == "IntersectionLL")
                    {

                    }
                    else if (CurrentNode.Text == "Calib9PointAbs")
                    {
                        int sel_num = treeView1.SelectedNode.Index;
                        Calib9PointAbs_form Calib_form = new Calib9PointAbs_form((UnitCalib9PointAbs)Vision_step[sel_num], image_show);
                        Calib_form.ShowDialog();
                        Calib_form.Dispose();
                    }
                    else
                    {
                        treeView1.SelectedNode = CurrentNode;//选中这个节点
                        treeView1.LabelEdit = true;
                        treeView1.SelectedNode.BeginEdit();
                    }
                    
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            openFileDialog1.InitialDirectory = "D:\\";
            if (step_path != null)
                openFileDialog1.InitialDirectory = step_path;
            openFileDialog1.Filter = "XML文件|*.xml";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                step_path = openFileDialog1.FileName;
                textBox2.Text = step_path;
            }
            try
            {
                for (int i=0;i< treeView1.Nodes.Count;)
                {
                    Vision_step[treeView1.Nodes.Count-1] = null;
                    //treeView1.SelectedNode = i;
                    treeView1.Nodes.Remove(treeView1.Nodes[treeView1.Nodes.Count-i-1]);
                }
                TreeXml.Tree_Load(treeView1, step_path);
                //TreeXml.Xml_Load(step_path);//不画树，载入xml
                str_parm = TreeXml.str_parm; 
                str_name = TreeXml.str_name;
                get_tree_name();
                //get_xml_name();//根据xml，载入类和类参数。
            }
            catch (System.Exception ex)
            {
            	
            }
            
            //for(int i=0;i<20;i++)
            //{
            //    if (str_parm[i] == null)
            //    {
            //        break;
            //    }

            //}
            
        }

        private void button5_Click(object sender, EventArgs e)
        {
            saveFileDialog1.InitialDirectory = "D:\\";
            if (step_path != null)
                openFileDialog1.InitialDirectory = step_path;
            saveFileDialog1.Filter = "XML文件|*.xml";
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                step_path = saveFileDialog1.FileName;

            }

            for (int i = 0; i < 20; i++)
            {
                if (Vision_step[i] == null)
                {
                    //可以把最终的结果show出来。
                    //if (i > 0)
                    //{
                    //    show_Result(Vision_step[i - 1]);
                    //}
                    break;
                }
                //Vision_step[i].process(Hwin, image_show);
                Vision_step[i].SaveConfig();
                str_parm[i] = Vision_step[i].all_parm;
            }

            TreeXml.SaveXml(treeView1, step_path, str_parm);
        }

        private void button_run_Click(object sender, EventArgs e)
        {
            HOperatorSet.DispObj(image_show, Hwin);
            get_tree_name();
            for (int i = 0; i < 20; i++)
            {
                if(Vision_step[i] == null)
                {
                    //可以把最终的结果show出来。
                    if(i>0)
                    {
                        show_Result(Vision_step[i - 1]);
                    }
                    break;
                }
                Vision_step[i].process(Hwin, image_show);
                Vision_step[i].SaveConfig();
                str_parm[i] = Vision_step[i].all_parm;
            }
        }
        
        void show_Result(VisionUnitBase vision_step)
        {
            int lenght = vision_step.Result_Array.Length;
            HOperatorSet.SetColor(Hwin, "green");
            HOperatorSet.GetPart(Hwin, out Hrow1, out Hcol1, out Hrow2, out Hcol2);
            double beilv = (Hrow2 - Hrow1) / 500.0;
            HTuple start_row = 10 + Hrow1;
            HTuple start_col = 10 + Hcol1;
            HOperatorSet.SetTposition(Hwin, start_row, start_col);
            HOperatorSet.WriteString(Hwin, "Result :");
            for (int i=0;i<lenght;i++)
            {
                HOperatorSet.SetTposition(Hwin, start_row + 30 * (i+1)* beilv, start_col);
                HOperatorSet.WriteString(Hwin, vision_step.Result_Array[i]);
            }
        }
        void get_tree_name()
        {
            int tree_num = 0;
            foreach (TreeNode node in treeView1.Nodes)
            {
                //递归遍历节点                
                IEnumerator ie = node.Nodes.GetEnumerator();
                int tree_son_num = 0;
                if(Vision_step[tree_num]==null)
                {
                    if (node.Text == "Find_model")
                    {
                        Vision_step[tree_num] = new UnitFindModel(Vision_step); //str_parm
                        if (!Vision_step[tree_num].LoadConfig(str_parm[tree_num]))
                        {
                            MessageBox.Show("模板载入错误，请选择正确的模板路径");
                        }
                    }
                    if (node.Text == "Find_line")
                    {
                        Vision_step[tree_num] = new UnitFindLine(Vision_step); //str_parm
                        if (!Vision_step[tree_num].LoadConfig(str_parm[tree_num]))
                        {
                            MessageBox.Show("找直线，载入参数错误");
                        }
                    }
                    if (node.Text == "Find_circle")
                    {
                        Vision_step[tree_num] = new UnitFindCircle(Vision_step); //str_parm
                        if (!Vision_step[tree_num].LoadConfig(str_parm[tree_num]))
                        {
                            MessageBox.Show("找圆，载入参数错误");
                        }
                    }
                    if (node.Text == "IntersectionLL")
                    {
                        Vision_step[tree_num] = new UnitIntersectionLL(Vision_step); //str_parm
                        if (!Vision_step[tree_num].LoadConfig(str_parm[tree_num]))
                        {
                            MessageBox.Show("找交点，载入参数错误");
                        }
                    }
                    if (node.Text == "Calib9PointAbs")
                    {
                        Vision_step[tree_num] = new UnitCalib9PointAbs(Vision_step); //str_parm
                        if (!Vision_step[tree_num].LoadConfig(str_parm[tree_num]))
                        {
                            MessageBox.Show("9点标定，载入参数错误");
                        }
                    }

                }
                while (ie.MoveNext())
                {
                    TreeNode ctn = (TreeNode)ie.Current;
                    Vision_step[tree_num].str_in_parm[tree_son_num] = ctn.Text;
                    tree_son_num++;
                }
                tree_num++;
            }
        }

        void get_xml_name()
        {
            for (int i=0;i<20;i++)
            {
                if (Vision_step[i] == null)
                {
                    if (str_name[i] == "Find_model")
                    {
                        Vision_step[i] = new UnitFindModel(Vision_step); //str_parm
                        if (!Vision_step[i].LoadConfig(str_parm[i]))
                        {
                            MessageBox.Show("模板载入错误，请选择正确的模板路径");
                        }
                    }
                    if (str_name[i] == "Find_line")
                    {
                        Vision_step[i] = new UnitFindLine(Vision_step); //str_parm
                        if (!Vision_step[i].LoadConfig(str_parm[i]))
                        {
                            MessageBox.Show("找直线，载入参数错误");
                        }
                    }
                    if (str_name[i] == "Find_circle")
                    {
                        Vision_step[i] = new UnitFindCircle(Vision_step); //str_parm
                        if (!Vision_step[i].LoadConfig(str_parm[i]))
                        {
                            MessageBox.Show("找圆，载入参数错误");
                        }
                    }
                    if (str_name[i] == "IntersectionLL")
                    {
                        Vision_step[i] = new UnitIntersectionLL(Vision_step); //str_parm
                        if (!Vision_step[i].LoadConfig(str_parm[i]))
                        {
                            MessageBox.Show("找交点，载入参数错误");
                        }
                    }
                    if (str_name[i] == "Calib9PointAbs")
                    {
                        Vision_step[i] = new UnitCalib9PointAbs(Vision_step); //str_parm
                        if (!Vision_step[i].LoadConfig(str_parm[i]))
                        {
                            MessageBox.Show("9点标定，载入参数错误");
                        }
                    }
                    if(str_name[i] == null)
                    {
                        break;
                    }
                }               
            }
        }

        void Refresh_parm(int i)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            step_num = treeView1.Nodes.Count;
            //要添加的节点名称为空，即文本框是否为空
            if (string.IsNullOrEmpty(textBox1.Text.Trim()))
            {
                MessageBox.Show("要添加的节点名称不能为空！");
                return;
            }
            //根据不同的选择，添加不同的节点配置
            TreeNode tmp;
            if (comboBox1.Text == "Find_model")
            {
                tmp = new TreeNode(textBox1.Text);
                treeView1.SelectedNode = tmp;
                treeView1.Nodes.Add(tmp);
                treeView1.SelectedNode.Nodes.Add("in_ModelID".Trim());
                treeView1.SelectedNode.Nodes.Add("out_row".Trim());
                treeView1.SelectedNode.Nodes.Add("out_col".Trim());
                treeView1.SelectedNode.Nodes.Add("out_phi".Trim());
                textBox1.Text = "";
                Vision_step[step_num] = new UnitFindModel(Vision_step);
            }
            if (comboBox1.Text == "Find_line")
            {
                tmp = new TreeNode(textBox1.Text);
                treeView1.SelectedNode = tmp;
                treeView1.Nodes.Add(tmp);
                treeView1.SelectedNode.Nodes.Add("in_roi_row".Trim());
                treeView1.SelectedNode.Nodes.Add("in_roi_col".Trim());
                treeView1.SelectedNode.Nodes.Add("in_roi_phi".Trim());
                treeView1.SelectedNode.Nodes.Add("out_row1".Trim());
                treeView1.SelectedNode.Nodes.Add("out_col1".Trim());
                treeView1.SelectedNode.Nodes.Add("out_row2".Trim());
                treeView1.SelectedNode.Nodes.Add("out_col2".Trim());
                textBox1.Text = "";
                Vision_step[step_num] = new UnitFindLine(Vision_step);
                
                
            }
            if (comboBox1.Text == "Find_circle")
            {
                tmp = new TreeNode(textBox1.Text);
                treeView1.SelectedNode = tmp;
                treeView1.Nodes.Add(tmp);
                treeView1.SelectedNode.Nodes.Add("in_roi_row".Trim());
                treeView1.SelectedNode.Nodes.Add("in_roi_col".Trim());
                treeView1.SelectedNode.Nodes.Add("in_roi_ridius".Trim());
                treeView1.SelectedNode.Nodes.Add("out_row".Trim());
                treeView1.SelectedNode.Nodes.Add("out_col".Trim());;
                treeView1.SelectedNode.Nodes.Add("out_ridius".Trim());
                textBox1.Text = "";
                Vision_step[step_num] = new UnitFindCircle(Vision_step);
            }
            if (comboBox1.Text == "IntersectionLL")
            {
                tmp = new TreeNode(textBox1.Text);
                treeView1.SelectedNode = tmp;
                treeView1.Nodes.Add(tmp);
                treeView1.SelectedNode.Nodes.Add("in_line1_row1".Trim());
                treeView1.SelectedNode.Nodes.Add("in_line1_col1".Trim());
                treeView1.SelectedNode.Nodes.Add("in_line1_row2".Trim());
                treeView1.SelectedNode.Nodes.Add("in_line1_col2".Trim());
                treeView1.SelectedNode.Nodes.Add("in_line2_row1".Trim());
                treeView1.SelectedNode.Nodes.Add("in_line2_col1".Trim());
                treeView1.SelectedNode.Nodes.Add("in_line2_row2".Trim());
                treeView1.SelectedNode.Nodes.Add("in_line2_col2".Trim());
                treeView1.SelectedNode.Nodes.Add("out_row".Trim());
                treeView1.SelectedNode.Nodes.Add("out_col".Trim());
                textBox1.Text = "";
                Vision_step[step_num] = new UnitIntersectionLL(Vision_step);
            }
            if (comboBox1.Text == "Calib9PointAbs")
            {
                tmp = new TreeNode(textBox1.Text);
                treeView1.SelectedNode = tmp;
                treeView1.Nodes.Add(tmp);

                //treeView1.SelectedNode.Nodes.Add("in_point_index".Trim());
                treeView1.SelectedNode.Nodes.Add("in_pixel_column".Trim());
                treeView1.SelectedNode.Nodes.Add("in_pixel_row".Trim());
                treeView1.SelectedNode.Nodes.Add("in_world_x".Trim());
                treeView1.SelectedNode.Nodes.Add("in_world_y".Trim());

                treeView1.SelectedNode.Nodes.Add("out_offset_x".Trim());
                treeView1.SelectedNode.Nodes.Add("out_offset_y".Trim());
                treeView1.SelectedNode.Nodes.Add("out_offset_angle".Trim());
                treeView1.SelectedNode.Nodes.Add("out_calib_result".Trim());

                textBox1.Text = "";
                Vision_step[step_num] = new UnitCalib9PointAbs(Vision_step);
            }
            //添加根节点
            //treeView1.Nodes.Add(textBox1.Text.Trim());
            
            
        }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            textBox1.Text = comboBox1.Text;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            openFileDialog1.InitialDirectory = "D:\\";
            if (image_path != null)
                openFileDialog1.InitialDirectory = image_path;
            openFileDialog1.Filter = "图片文件|*.png;*.bmp";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                image_path = openFileDialog1.FileName;
                Hpath = image_path;
            }
            image_show.Dispose();
            try
            {
                HOperatorSet.ReadImage(out image_show, Hpath);
                HOperatorSet.GetImageSize(image_show, out Hwidth, out Hheight);
                HOperatorSet.SetPart(Hwin, 0, 0, Hheight, Hwidth);
                HOperatorSet.DispObj(image_show, Hwin);
            }
            catch
            {
                MessageBox.Show("打开图片失败");
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
                if (true)
                {
                    try
                    {
                        HOperatorSet.GetImageSize(image_show, out Hwidth, out Hheight);
                        HOperatorSet.SetPart(Hwin, 0, 0, Hheight, Hwidth);
                        HOperatorSet.DispObj(image_show, Hwin);
                        //HOperatorSet.DispObj(H_show_roi1, Hwin);
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
            if (isMouseDown)
            {
                HOperatorSet.SetPart(Hwin, Hrow1 - b, Hcol1 - a, Hrow2 - b, Hcol2 - a);
                HOperatorSet.DispObj(image_show, Hwin);
            }
            //pictureBox1.Location = new System.Drawing.Point(p2.X + a, p2.Y + b);
        }
        private void pictureBox1_MouseWheel(object sender, MouseEventArgs e)
        {


            if (true)
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
                        //HOperatorSet.DispObj(H_show_roi1, Hwin);
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
                        //HOperatorSet.DispObj(H_show_roi1, Hwin);
                        //HOperatorSet.DispObj(H_show_roi2, Hwin);
                    }
                    catch { }
                }
            }

        }
    }
}
