using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HalconDotNet;
using System.Windows.Forms;

namespace vision_form
{
    public class UnitFindModel : VisionUnitBase
    {
        public bool m_bShowModel = true;
        bool m_bResult = false;
        HObject m_hModelXLD;
        VisionUnitBase[] VUB;

        //起始角度，角度范围，最小放大比例，最大放大比例，
        //对比度（低），对比度（高），最小组件尺寸，最小对比度
        //金字塔层数，角度步长，放大比例步长，最优化，度量，//默认值，不让调整
        public HTuple HangleStart, HangleExtent, HscaleMin, HscaleMax;
        public HTuple Hcontrast, HminContrast,ContrastMin, ContrastMax, MinSize;//第一个是，最后三个的集合
        public HTuple H_NumLevels, H_AngleStep, H_ScaleStep, H_Optimization, H_Metric;//默认值，不让调整,现在可以不用,方便以后开发应用

        //最小分数，最大重叠，贪婪度
        //查找个数，亚像素，//默认值，不让调整
        public HTuple HminScore, HmaxOverlap, Hgreediness;
        public HTuple H_NumMatches, H_SubPixel;//默认值，不让调整,现在可以不用,方便以后开发应用
        public HTuple HShapeModelID = null;
        public string modelpath =null;
        HTuple out_row, out_col, out_phi, out_scale, out_score;
        public UnitFindModel(VisionUnitBase[] Vision_step = null)
        {
            Result_Array = new HTuple[3];
            str_in_parm = new string[4];
            VUB = Vision_step;

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
            //HOperatorSet.GenEmptyObj(out m_hCross);
            //HOperatorSet.GenEmptyObj(out m_hRectang2);
            //HOperatorSet.GenEmptyObj(out m_hCircle);
            HOperatorSet.GenEmptyObj(out m_hModelXLD);
        }
        public override string Type { get { return "FindModel"; } }
        public override void SaveConfig()
        {
            //all_parm = in_row[0].D.ToString();Hdirection[0].S,Hwid[0].I.ToString() 
            all_parm = HangleStart[0].D.ToString() + "_" + HangleExtent[0].D.ToString() + "_" + HscaleMin[0].D.ToString()
                    + "_" + HscaleMax[0].D.ToString() + "_" + ContrastMin[0].I.ToString() + "_" + ContrastMax[0].I.ToString()
                    + "_" + MinSize[0].I.ToString() + "_" + HminContrast[0].I.ToString()
                    + "_" + HminScore[0].D.ToString() + "_" + HmaxOverlap[0].D.ToString()+ "_" + Hgreediness[0].D.ToString()
                    + "_" + modelpath;


        }
        public override bool LoadConfig(string str_parm_all)
        {
            string[] sArray = str_parm_all.Split('_');
            HangleStart = Convert.ToDouble(sArray[0]);
            HangleExtent = Convert.ToDouble(sArray[1]);
            HscaleMin = Convert.ToDouble(sArray[2]);
            HscaleMax = Convert.ToDouble(sArray[3]);
            ContrastMin = Convert.ToInt32(sArray[4]);
            ContrastMax = Convert.ToInt32(sArray[5]);
            MinSize = Convert.ToInt32(sArray[6]);
            HminContrast = Convert.ToInt32(sArray[7]);
            HminScore = Convert.ToDouble(sArray[8]);
            HmaxOverlap = Convert.ToDouble(sArray[9]);
            Hgreediness = Convert.ToDouble(sArray[10]);
            modelpath = sArray[11];
            if (sArray.Length>12)
                modelpath = modelpath + sArray[12];
            try
            {
                //"E:\\c#学习\\visionform\\test.shm"
                //HOperatorSet.ReadShapeModel("E:/c#学习/visionform/test.shm", out HShapeModelID);
                HOperatorSet.ReadShapeModel(modelpath.Replace("\\", "/"), out HShapeModelID);

                return true;
            }
            catch (Exception)
            {
                return false;
                //MessageBox.Show("载入");
                //throw;
            }
            
        }

        public override bool SetTrainImage(string filename)
        {
            return true;
        }

        public override void DrawRecord(HTuple wnd)
        {
            if (m_bShowModel && m_hModelXLD.IsInitialized())
            {
                HOperatorSet.SetColor(wnd, "blue");
                HOperatorSet.SetLineWidth(wnd, 2);
                HOperatorSet.DispObj(m_hModelXLD, wnd);
                HOperatorSet.SetColor(wnd, "red");
                HOperatorSet.SetLineWidth(wnd, 1);
            }            
        }
        public override bool process(HTuple window, HObject img)
        {
            m_bResult = false;
            if (img != null)
            {
                try
                {
                    Refresh_in();
                    m_bResult = FindModel(img);
                    DrawRecord(window);
                    return m_bResult;
                }
                catch (HOperatorException e)
                {
                    System.Diagnostics.Debug.WriteLine(e.ToString());
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        public bool FindModel(HObject img)
        {
            HTuple hv_HomMat2D;
            HObject HModelContours;
            HOperatorSet.GenEmptyObj(out HModelContours);
            try
            {
                HModelContours.Dispose();
                HOperatorSet.GetShapeModelContours(out HModelContours, HShapeModelID, 1);
                HOperatorSet.FindScaledShapeModel(img, HShapeModelID, HangleStart*3.1415926/180, HangleExtent * 3.1415926 / 180,
                    HscaleMin, HscaleMax, 0.2, H_NumMatches, HmaxOverlap, H_SubPixel, H_NumLevels,
                    Hgreediness, out out_row, out out_col, out out_phi, out out_scale, out out_score);
                if (out_score[0].D > HminScore)
                {
                    HOperatorSet.VectorAngleToRigid(0, 0, 0, out_row, out_col, out_phi, out hv_HomMat2D);
                    m_hModelXLD.Dispose();
                    HOperatorSet.AffineTransContourXld(HModelContours, out m_hModelXLD, hv_HomMat2D);
                    Result_Array[0] = out_row;
                    Result_Array[1] = out_col;
                    Result_Array[2] = out_phi;
                    return true;
                }
                else
                {
                    MessageBox.Show("匹配分值太低");
                    return false;
                }
            }
            catch (System.Exception ex)
            {
                return false;
            }
        }
        public void Refresh_in()
        {

        }
    }
}
