using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HalconDotNet;

namespace vision_form
{
    class UnitIntersectionLL : VisionUnitBase
    {
        VisionUnitBase[] VUB;
        public HTuple in_line1_row1, in_line1_col1, in_line1_row2, in_line1_col2;
        public HTuple in_line2_row1, in_line2_col1, in_line2_row2, in_line2_col2;
        public bool m_bResult;
        public HObject m_hCross;
        public HTuple m_hRow, m_hCol, hv_IsParallel;
        public UnitIntersectionLL(VisionUnitBase[] Vision_step = null)
        {
            VUB = Vision_step;
            Result_Array = new HTuple[2];
            str_in_parm = new string[10];
            in_line1_row1 = 0;
            in_line1_col1 = 0;
            in_line1_row2 = 0;
            in_line1_col2 =100;
            in_line2_row1 = 10;
            in_line2_col1 = 10;
            in_line2_row2 = 100;
            in_line2_col2 = 10;
            HOperatorSet.GenEmptyObj(out m_hCross);
        }
        public override string Type { get { return "IntersectionLL"; } }
        public override void SaveConfig()
        {
            try
            {
                all_parm = in_line1_row1[0].D.ToString() + "_" + in_line1_col1[0].D.ToString() + "_" +
                       in_line1_row2[0].D.ToString() + "_" + in_line1_col2[0].D.ToString() + "_" +
                       in_line2_row1[0].D.ToString() + "_" + in_line2_col1[0].D.ToString() + "_" +
                       in_line2_row2[0].D.ToString() + "_" + in_line2_col2[0].D.ToString();
            }
            catch (System.Exception ex)
            {
            	
            }
            
        }
        public override bool LoadConfig(string str_parm_all)
        {
            try
            {
                string[] sArray = str_parm_all.Split('_');
                in_line1_row1 = Convert.ToDouble(sArray[0]);
                in_line1_col1 = Convert.ToDouble(sArray[1]);
                in_line1_row2 = Convert.ToDouble(sArray[2]);
                in_line1_col2 = Convert.ToDouble(sArray[3]);
                in_line2_row1 = Convert.ToDouble(sArray[4]);
                in_line2_col1 = Convert.ToDouble(sArray[5]);
                in_line2_row2 = Convert.ToDouble(sArray[6]);
                in_line2_col2 = Convert.ToDouble(sArray[7]);
                return true;
            }
            catch (System.Exception ex)
            {
                return false;
            }                        
        }
        public override bool SetTrainImage(string filename)
        {
            return true;
        }
        public override void DrawRecord(HTuple wnd)
        {
            if (m_hCross.IsInitialized())
            {
                HOperatorSet.SetColor(wnd, "green");
                HOperatorSet.SetLineWidth(wnd, 2);
                HOperatorSet.DispObj(m_hCross, wnd);
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
                    m_bResult = IntersectionLL(img);
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
        public bool IntersectionLL(HObject img)
        {
            try
            {
                HOperatorSet.IntersectionLl(in_line1_row1, in_line1_col1, in_line1_row2, in_line1_col2,
                                        in_line2_row1, in_line2_col1, in_line2_row2, in_line2_col2,
                                        out m_hRow, out m_hCol, out hv_IsParallel);
                m_hCross.Dispose();
                HOperatorSet.GenCrossContourXld(out m_hCross, m_hRow, m_hCol, 27, (new HTuple(45)).TupleRad());
                Result_Array[0] = m_hRow;
                Result_Array[1] = m_hCol;
                return true;
            }
            catch (System.Exception ex)
            {
                return false;
            }
            
            
            
        }

        public void Refresh_in()
        {
            try
            {
                string[] sArray = str_in_parm[0].Split('_');
                if (sArray[0] == "from")
                {
                    int i, j;
                    i = Convert.ToInt32(sArray[1]);
                    j = Convert.ToInt32(sArray[2]);
                    in_line1_row1 = VUB[i].Result_Array[j];
                }
                sArray = str_in_parm[1].Split('_');
                if (sArray[0] == "from")
                {
                    int i, j;
                    i = Convert.ToInt32(sArray[1]);
                    j = Convert.ToInt32(sArray[2]);
                    in_line1_col1 = VUB[i].Result_Array[j];
                }
                sArray = str_in_parm[2].Split('_');
                if (sArray[0] == "from")
                {
                    int i, j;
                    i = Convert.ToInt32(sArray[1]);
                    j = Convert.ToInt32(sArray[2]);
                    in_line1_row2 = VUB[i].Result_Array[j];
                }
                sArray = str_in_parm[3].Split('_');
                if (sArray[0] == "from")
                {
                    int i, j;
                    i = Convert.ToInt32(sArray[1]);
                    j = Convert.ToInt32(sArray[2]);
                    in_line1_col2 = VUB[i].Result_Array[j];
                }

                sArray = str_in_parm[4].Split('_');
                if (sArray[0] == "from")
                {
                    int i, j;
                    i = Convert.ToInt32(sArray[1]);
                    j = Convert.ToInt32(sArray[2]);
                    in_line2_row1 = VUB[i].Result_Array[j];
                }
                sArray = str_in_parm[5].Split('_');
                if (sArray[0] == "from")
                {
                    int i, j;
                    i = Convert.ToInt32(sArray[1]);
                    j = Convert.ToInt32(sArray[2]);
                    in_line2_col1 = VUB[i].Result_Array[j];
                }
                sArray = str_in_parm[6].Split('_');
                if (sArray[0] == "from")
                {
                    int i, j;
                    i = Convert.ToInt32(sArray[1]);
                    j = Convert.ToInt32(sArray[2]);
                    in_line2_row2 = VUB[i].Result_Array[j];
                }
                sArray = str_in_parm[7].Split('_');
                if (sArray[0] == "from")
                {
                    int i, j;
                    i = Convert.ToInt32(sArray[1]);
                    j = Convert.ToInt32(sArray[2]);
                    in_line2_col2 = VUB[i].Result_Array[j];
                }
            }
            catch (System.Exception ex)
            {

            }

        }
    }
}
