using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HalconDotNet;


namespace vision_form
{
    public class UnitFindLine : VisionUnitBase
    {
        /// <summary>
        /// 找线窗口
        /// </summary>
        //Form_FindLine frm;
        /// <summary>
        /// 委托回调显示函数
        /// </summary>
       // public FindLineDelegate NotifyIconObserver;
        /// <summary>
        /// 训练图像
        /// </summary>
        public HObject m_hImageTrain = null;
        /// <summary>
        /// 运行图像
        /// </summary>
        public string m_strRunImage = string.Empty;
        /// <summary>
        /// 训练图像的存储路径
        /// </summary>
        public string m_strTrainImage = string.Empty;
        /// <summary>
        /// 用于查找的ROI
        /// </summary>
        // public ROILine m_roiLine;
        public HTuple in_row, in_col, in_phi, data_from_phi;
        public HTuple data_row, data_col, data_phi;
        public HTuple L1, L2;
        public HTuple Hnum, Hwid, Hamp, Hsomth;
        public HTuple Htransition, Hselect;
        public HTuple m_nNumDropPoints;

        //public HTuple IN_ROW = null, IN_COL = null, IN_PHI = null;
        
        /// <summary>
        /// 显示参数
        /// </summary>
        public bool m_bShowLine = true;
        /// <summary>
        /// 是否显示找到的边缘点
        /// </summary>
        public bool m_bShowCross = true;
        /// <summary>
        /// 是否显示忽略的边缘点
        /// </summary>
        public bool m_bShowCrossDrop = true;
        /// <summary>
        /// 是否显示用于查找的小矩形区
        /// </summary>
        public bool m_bShowRectang2 = true;
        /// <summary>
        /// 查找的结果
        /// </summary>
        public bool m_bResult;
        /// <summary>
        /// 
        /// </summary>
        public HTuple m_rowBegin = new HTuple();
        /// <summary>
        /// 
        /// </summary>
        public HTuple m_colBegin = new HTuple();
        /// <summary>
        /// 
        /// </summary>
        public HTuple m_rowEnd = new HTuple();
        /// <summary>
        /// 
        /// </summary>
        public HTuple m_colEnd = new HTuple();

        
        /// <summary>
        /// 要显示的图形对像
        /// </summary>
        public HXLDCont m_hLine = new HXLDCont();
        /// <summary>
        /// 查找到的有效边缘点
        /// </summary>
        public HXLDCont m_hCross = new HXLDCont();
        /// <summary>
        /// 忽略的无效边缘点
        /// </summary>
        public HXLDCont m_hCrossDrop = new HXLDCont();
        /// <summary>
        /// 用于查找的小矩形区
        /// </summary>
        public HXLDCont m_hRectang2 = new HXLDCont();
        /// <summary>
        /// 图像的宽度
        /// </summary>
        public HTuple m_imageWidth = new HTuple();
        /// <summary>
        /// 图像的高度
        /// </summary>
        public HTuple m_imageHeight = new HTuple();

        VisionUnitBase[] VUB;
        /// <summary>
        /// 构造函数
        /// </summary>
        public UnitFindLine(VisionUnitBase[] Vision_step = null)
        {
            data_from_phi = 0;
            Result_Array = new HTuple[4];
            str_in_parm = new string[7];
            VUB = Vision_step;
            data_row = 0;
            data_col = 0;
            data_phi = 0;
            m_hImageTrain = null;
            //m_roiLine = null;
            Hnum = 10;

            //m_fProjectiveLength = 20;
            in_phi = 0.0;
            Hamp = 15;
            Hsomth = 1;
            Hwid = 30;
            Htransition = "positive";
            Hselect = "first";
            m_nNumDropPoints = 0;

            //m_strAlgorithm = "tukey";
            //m_nIterations = 15;
            //m_fClippingFactory = 6.0;

            m_strRunImage = string.Empty;
            m_strTrainImage = string.Empty;

            //m_roiLine = new ROILine();
        }
        /// <summary>
        /// 获取当前视觉单元的类型
        /// </summary>
        public override string Type { get { return "FindLine"; } }

        /// <summary>
        /// 设置查找点的个数
        /// </summary>
        /// <param name="nNumPoints"></param>
        
        /// <summary>
        /// 设置投影长度
        /// </summary>
        /// <param name="nLen"></param>
       
        /// <summary>
        /// 设置查找方向
        /// </summary>
       
        /// <summary>
        /// 设置边缘的选择顺序
        /// </summary>
        /// <param name="strSelect"></param>
     
        /// <summary>
        /// 设置忽略点的个数
        /// </summary>
        /// <param name="nNumDropPoints"></param>
        public void SetNumDropPoints(int nNumDropPoints)
        {
            if (m_nNumDropPoints != nNumDropPoints)
            {
                m_nNumDropPoints = nNumDropPoints;
                FindLine(m_hImageTrain);
                //NotifyIconObserver(1);
            }
        }
        /// <summary>
        /// 设置算法的迭代次数
        /// </summary>
        /// <param name="nIterations"></param>
       
        /// <summary>
        /// 设置拟合的算法
        /// </summary>
        /// <param name="strAlgorithm"></param>
     
        /// <summary>
        /// 设置算法的剪裁因子
        /// </summary>
        /// <param name="fClippingFactory"></param>
     

        /// <summary>
        /// 保存配置
        /// </summary>
        public override void SaveConfig()
        {
            //all_parm = in_row[0].D.ToString();
            all_parm = in_row[0].D.ToString() + "_"+ in_col[0].D.ToString() + "_" +in_phi[0].D.ToString() + "_" + L1[0].D.ToString()
                    + "_" +L2[0].D.ToString() + "_" + Hnum[0].I.ToString() + "_" + Hwid[0].I.ToString() + "_" + Hamp[0].I.ToString()
                    + "_" + Hsomth[0].D.ToString() + "_" + Htransition[0].S + "_" + Hselect[0].S
                    + "_" + data_row[0].I.ToString() + "_" + data_col[0].I.ToString() + "_" + data_phi[0].D.ToString()
                    + "_" + m_nNumDropPoints[0].I.ToString();
            

        }
        /// <summary>
        /// 读取配置
        /// </summary>
        /// <param name="strDir">目录</param>
        /// <param name="strName">文件名</param>
        public override bool LoadConfig(string str_parm_all)
        {
            try
            {
                string[] sArray = str_parm_all.Split('_');
                in_row = Convert.ToDouble(sArray[0]);
                in_col = Convert.ToDouble(sArray[1]);
                in_phi = Convert.ToDouble(sArray[2]);
                L1 = Convert.ToDouble(sArray[3]);
                L2 = Convert.ToDouble(sArray[4]);
                Hnum = Convert.ToInt32(sArray[5]);
                Hwid = Convert.ToInt32(sArray[6]);
                Hamp = Convert.ToInt32(sArray[7]);
                Hsomth = Convert.ToDouble(sArray[8]);
                Htransition = sArray[9];
                Hselect = sArray[10];
                data_row = Convert.ToInt32(sArray[11]);
                data_col = Convert.ToInt32(sArray[12]);
                data_phi = Convert.ToDouble(sArray[13]);
                m_nNumDropPoints = Convert.ToInt32(sArray[14]);
                return true;
            }
            catch (System.Exception ex)
            {
                return false;
            }          
        }
        
        /// <summary>
        /// 设置训练的图像
        /// </summary>
        /// <param name="filename">图像的文件名</param>
        /// <returns></returns>
        public override bool SetTrainImage(string filename)
        {
            try
            {
                if (m_hImageTrain != null)
                    m_hImageTrain.Dispose();
                else
                    m_hImageTrain = new HImage();

                m_strTrainImage = filename;
                m_hImageTrain = new HImage(m_strTrainImage);
            }
            catch (HOperatorException e)
            {
                MessageBox.Show(e.Message, "提示");
                return false;
            }
            return true;
        }


        /// <summary>
        /// 在当前图像上按单元配置查找直线
        /// </summary>
        /// <param name="img"></param>
        /// <param name="hFixTool"></param> 
        /// <returns></returns>
        public bool FindLine(HObject img)
        {
            m_rowBegin = new HTuple();
            m_colBegin = new HTuple();
            m_rowEnd = new HTuple();
            m_colEnd = new HTuple();

            m_hLine.Dispose();
            m_hCross.Dispose();
            m_hCrossDrop.Dispose();
            if (img == null)
                return false;

            string strSelect = string.Empty;
            if (Hselect == "strongest")
                strSelect = "all";
            else
                strSelect = Hselect;

            HTuple imgWidth, imgHeight;
            HOperatorSet.GetImageSize(img, out imgWidth, out imgHeight);
            
            m_imageWidth = imgWidth;
            m_imageHeight = imgHeight;

            double x1, y1, x2, y2;
            HTuple in_phi_t = in_phi + data_from_phi + data_phi * 3.1415926 / 180;
            double angle = in_phi_t[0].D + 3.1415926/2;

            x1 = (in_col + data_col)[0].D + L2 * Math.Cos(angle);
            x2 = (in_col + data_col)[0].D - L2 * Math.Cos(angle);
            y1 = (in_row + data_row)[0].D - L2 * Math.Sin(angle);
            y2 = (in_row + data_row)[0].D + L2 * Math.Sin(angle);

            HHomMat2D hom = new HHomMat2D();
            hom.HomMat2dIdentity();
            hom = hom.HomMat2dTranslate((y2 - y1) / (Hnum - 1), (x2 - x1) / (Hnum - 1));

            double rect_row = y1;
            double rect_col = x1;
            double rect_phi = in_phi_t[0].D;
            double rect_len1 = L1;
            double rect_len2 = (double)Hwid/2.0;
            HTuple hms = null;
            HOperatorSet.GenMeasureRectangle2(rect_row, rect_col, rect_phi, rect_len1,
                rect_len2, imgWidth, imgHeight, "nearest_neighbor",out hms);

            HTuple Row = new HTuple();
            HTuple Col = new HTuple();
            HTuple Distance = new HTuple();
            HTuple outRow = new HTuple();
            HTuple outCol = new HTuple();
            HTuple outAmp = new HTuple();
            HTuple outDistance = new HTuple();
            HRegion regionCount = new HRegion();
            regionCount.GenEmptyRegion();
            HXLDCont contour = new HXLDCont();

            for (int i = 0; i < Hnum[0].I; ++i)
            {
                if (m_bShowRectang2)
                {
                    contour.Dispose();
                    contour.GenRectangle2ContourXld(rect_row, rect_col, rect_phi, rect_len1, rect_len2);
                    HRegion region;
                    region = contour.GenRegionContourXld("margin");
                    regionCount = regionCount.Union2(region);
                }
                try
                {
                    HOperatorSet.MeasurePos(img, hms, Hsomth, Hamp, Htransition, strSelect,
                                     out outRow, out outCol, out outAmp, out outDistance);
                }
                catch { continue; }

                if (strSelect == "all" && outRow.Length > 1)
                {
                    HTuple hIndex = outAmp.TupleSortIndex();
                    int nMax = hIndex[hIndex.Length - 1];
                    Row.Append(outRow[nMax]);
                    Col.Append(outCol[nMax]);
                }
                else
                {
                    Row.Append(outRow);
                    Col.Append(outCol);
                }
                hom.AffineTransPixel(rect_row, rect_col, out rect_row, out rect_col);
                HOperatorSet.TranslateMeasure(hms, rect_row, rect_col);
                
            }
            HOperatorSet.CloseMeasure(hms);
            if (m_bShowRectang2)
            {
                m_hRectang2 = regionCount.GenContoursSkeletonXld(1, "filter");

            }

            if (Row.Length > 1)
            {
                HXLDCont counter = new HXLDCont();
                counter.GenContourPolygonXld(Row, Col);

                HTuple nr, nc, dist;
                counter.FitLineContourXld("tukey", -1, 0, 5, 2,
                    out m_rowBegin, out m_colBegin, out m_rowEnd, out m_colEnd, out nr, out nc, out dist);

                HTuple rowDrop = new HTuple();
                HTuple colDrop = new HTuple();
                if (Row.Length - 3 > m_nNumDropPoints && m_nNumDropPoints > 0)
                {
                    HTuple distance = new HTuple();

                    for (int i = 0; i < Row.Length; ++i)
                    {
                        double dis = HMisc.DistancePl(Row[i], Col[i], m_rowBegin, m_colBegin, m_rowEnd, m_colEnd);
                        distance.Append(dis);
                    }
                    HTuple index = distance.TupleSortIndex();
                    index = index.TupleInverse();
                    for (int i = 0; i < m_nNumDropPoints; ++i)
                    {
                        int n = index[i];
                        rowDrop.Append(Row[n]);
                        colDrop.Append(Col[n]);
                    }
                    index = index.TupleFirstN(m_nNumDropPoints - 1);
                    Row = Row.TupleRemove(index);
                    Col = Col.TupleRemove(index);

                    counter.GenContourPolygonXld(Row, Col);
                    counter.FitLineContourXld("tukey", -1, 0, 5, 2,
                        out m_rowBegin, out m_colBegin, out m_rowEnd, out m_colEnd, out nr, out nc, out dist);
                }

                if (m_bShowLine)
                {
                    HTuple rrow = new HTuple(m_rowBegin);
                    HTuple ccol = new HTuple(m_colBegin);
                    rrow.Append(m_rowEnd);
                    ccol.Append(m_colEnd);
                    m_hLine.GenContourPolygonXld(rrow, ccol);
                }

                if (m_bShowCross)
                {
                    m_hCross.GenEmptyObj();
                    m_hCross.GenCrossContourXld(Row, Col, 17, (new HTuple(45)).TupleRad());
                }

                if (m_bShowCrossDrop)
                {
                    m_hCrossDrop.GenEmptyObj();
                    m_hCrossDrop.GenCrossContourXld(rowDrop, colDrop, 17, (new HTuple(45)).TupleRad());
                }
                
                Result_Array[0] = m_rowBegin;
                Result_Array[1] = m_colBegin;
                Result_Array[2] = m_rowEnd;
                Result_Array[3] = m_colEnd;
                return true;
            }
            return false;
        }

        public bool Show_object(HObject img, HTuple Hwin)
        {
            try
            {
                HOperatorSet.DispObj(img, Hwin);
                HOperatorSet.DispObj(m_hLine, Hwin);
                HOperatorSet.DispObj(m_hCross, Hwin);
                HOperatorSet.DispObj(m_hRectang2, Hwin);

                
            }
            catch (System.Exception ex)
            {
            	
            }
            return true;
        }


        /// <summary>
        /// 在指定窗口上显示查找结果
        /// </summary>
        /// <param name="wnd"></param>
        public override void DrawRecord(HTuple wnd)
        {
            
            if (m_bShowCross && m_hCross.IsInitialized())
            {
                HOperatorSet.SetColor(wnd, "blue");
                HOperatorSet.SetLineWidth(wnd, 1);
                HOperatorSet.DispObj(m_hCross, wnd);
            }
            //if (m_bShowCrossDrop && m_hCrossDrop.IsInitialized())
            //{
            //    HOperatorSet.SetColor(wnd, "red");
            //    HOperatorSet.DispObj(m_hCrossDrop, wnd);
            //}

            if (m_bShowRectang2 && m_hRectang2.IsInitialized())
            {
                HOperatorSet.SetColor(wnd, "green");
                HOperatorSet.SetLineWidth(wnd, 1);
                HOperatorSet.DispObj(m_hRectang2, wnd);
            }
            if (m_bShowLine && m_hLine.IsInitialized())
            {
                HOperatorSet.SetColor(wnd, "red");
                HOperatorSet.SetLineWidth(wnd, 2);
                HOperatorSet.DispObj(m_hLine, wnd);
                HOperatorSet.SetLineWidth(wnd, 1);
            }
        }
        /// <summary>
        /// 在指定图像，指定窗口上查找直线
        /// </summary>
        /// <param name="window"></param>
        /// <param name="img"></param>
        /// <param name="hFixTool"></param>
        /// <returns></returns>
        public override bool process(HTuple window, HObject img)
        {
            m_bResult = false;
            if (img != null)
            {
                //HTuple pointer, type, width, height;
                //HOperatorSet.GetImagePointer1(img, out pointer, out type, out width, out height);
                //HImage imgTest = new HImage();
                //imgTest.GenImage1(type, width, height, pointer);
                try
                {

                    Refresh_in();
                    m_bResult = FindLine(img);
                    //HOperatorSet.DispObj(img, window);
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
                    in_row = VUB[i].Result_Array[j];
                }
                sArray = str_in_parm[1].Split('_');
                if (sArray[0] == "from")
                {
                    int i, j;
                    i = Convert.ToInt32(sArray[1]);
                    j = Convert.ToInt32(sArray[2]);
                    in_col = VUB[i].Result_Array[j];
                }
                sArray = str_in_parm[2].Split('_');
                if (sArray[0] == "from")
                {
                    int i, j;
                    i = Convert.ToInt32(sArray[1]);
                    j = Convert.ToInt32(sArray[2]);
                    data_from_phi = VUB[i].Result_Array[j];
                }
            }
            catch (System.Exception ex)
            {
            	
            }
                       
        }
    }
}

