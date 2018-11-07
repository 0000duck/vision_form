using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HalconDotNet;

namespace vision_form
{
    public class UnitFindCircle : VisionUnitBase
    {
        public HTuple in_row, in_col, in_radius;
        public HTuple data_row, data_col;
        
        public HTuple Hnum, Hwid, Hamp, Hsomth, HLengh;
        public HTuple Htransition, Hselect, Hdirection;
        public HTuple m_nNumDropPoints;

        HObject m_hCross, m_hRectang2, m_hCircle, m_centerCross;
        public bool m_bShowCircle = true;
        public bool m_bShowCross = true;
        public bool m_bShowRectang2 = true;
        public bool m_bResult;
        public HTuple out_row, out_col, out_radius;

        VisionUnitBase[] VUB;
        public UnitFindCircle(VisionUnitBase[] Vision_step = null)
        {
            Result_Array = new HTuple[3];
            str_in_parm = new string[6];
            VUB = Vision_step;
            data_row = 0;
            data_col = 0;

            Hnum = 10;
            Hamp = 15;
            Hsomth = 1;
            Hwid = 30;
            HLengh = 30;
            Htransition = "positive";
            Hselect = "first";
            Hdirection = "in_to_out";
            m_nNumDropPoints = 0;
            HOperatorSet.GenEmptyObj(out m_hCross);
            HOperatorSet.GenEmptyObj(out m_hRectang2);
            HOperatorSet.GenEmptyObj(out m_hCircle);
            HOperatorSet.GenEmptyObj(out m_centerCross);
        }
        public override string Type { get { return "FindCircle"; } }
        public override void SaveConfig()
        {
            //all_parm = in_row[0].D.ToString();
            all_parm = in_row[0].D.ToString() + "_" + in_col[0].D.ToString() + "_" + in_radius[0].D.ToString()
                    + "_" + Hnum[0].I.ToString() + "_" + Hwid[0].I.ToString() + "_" + Hamp[0].I.ToString()
                    + "_" + Hsomth[0].D.ToString() + "_" + HLengh[0].D.ToString()
                    + "_" + Htransition[0].S + "_" + Hselect[0].S + "_" + Hdirection[0].S
                    + "_" + data_row[0].I.ToString() + "_" + data_col[0].I.ToString()
                    + "_" + m_nNumDropPoints[0].I.ToString();


        }
        public override bool LoadConfig(string str_parm_all)
        {
            try
            {
                string[] sArray = str_parm_all.Split('_');
                in_row = Convert.ToDouble(sArray[0]);
                in_col = Convert.ToDouble(sArray[1]);
                in_radius = Convert.ToDouble(sArray[2]);
                Hnum = Convert.ToInt32(sArray[3]);
                Hwid = Convert.ToInt32(sArray[4]);
                Hamp = Convert.ToInt32(sArray[5]);
                Hsomth = Convert.ToDouble(sArray[6]);
                HLengh = Convert.ToInt32(sArray[7]);
                Htransition = sArray[8];
                Hselect = sArray[9];
                Hdirection = sArray[10];
                data_row = Convert.ToInt32(sArray[11]);
                data_col = Convert.ToInt32(sArray[12]);
                m_nNumDropPoints = Convert.ToInt32(sArray[13]);
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
            if (m_bShowCircle && m_hCircle.IsInitialized())
            {
                HOperatorSet.SetColor(wnd, "red");
                HOperatorSet.SetLineWidth(wnd, 2);
                HOperatorSet.DispObj(m_hCircle, wnd);
                HOperatorSet.SetColor(wnd, "green");
                HOperatorSet.SetLineWidth(wnd, 2);
                HOperatorSet.DispObj(m_centerCross, wnd);
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
                    m_bResult = FindCircle(img);
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
        public bool FindCircle(HObject img)
        {
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

            if (img == null)
                return false;
            HTuple imgWidth, imgHeight;
            HTuple hms = null;
            
            HOperatorSet.GetImageSize(img, out imgWidth, out imgHeight);

            double xCenter, yCenter, Center_radius, angle, angleSplit;
            
            angle = 0;
            //分割角度
            angleSplit = 2*3.1415926 / (Hnum);
            xCenter = in_col[0].D + data_col;
            yCenter = in_row[0].D + data_row;
            Center_radius = in_radius[0].D;

            HHomMat2D hom = new HHomMat2D();
            hom.HomMat2dIdentity();
            hom = hom.HomMat2dRotate(angleSplit, yCenter,xCenter);
            double rect_row, rect_col, angleStart, L1, L2;
            //分割矩形的中心坐标
            rect_row = yCenter ;
            rect_col = xCenter + Center_radius;
            L1 = HLengh[0].D / 2;
            L2 = Hwid[0].D / 2;
            if (Hdirection != "in2out")
            {
                //由外向内
                angleStart = angle + (new HTuple(180)).TupleRad();
            }
            else
            {
                angleStart = angle;
            }
            
            for (int i = 0; i < Hnum[0].I; ++i)
            {
                if (m_bShowRectang2)
                {
                    contour.Dispose();
                    contour.GenRectangle2ContourXld(rect_row, rect_col, angleStart + angleSplit * i, L1, L2);
                    HRegion region;
                    region = contour.GenRegionContourXld("margin");
                    regionCount = regionCount.Union2(region);
                    region.Dispose();
                }
                try
                {//out_row, out_col, out_radius;
                    HOperatorSet.GenMeasureRectangle2(rect_row, rect_col, angleStart + angleSplit * i, L1,
                                                L2, imgWidth, imgHeight, "nearest_neighbor", out hms);
                    HOperatorSet.MeasurePos(img, hms, Hsomth, Hamp, Htransition, Hselect,
                                     out outRow, out outCol, out outAmp, out outDistance);

                }
                catch { continue; }
                if (Hselect == "all" && outRow.Length > 1)
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
            }
            HOperatorSet.CloseMeasure(hms);
            if (m_bShowRectang2)
            {
                m_hRectang2.Dispose();
                m_hRectang2 = regionCount.GenContoursSkeletonXld(1, "filter");
            }
            if (Row.Length > Hnum[0].D / 3)
            {
                HXLDCont counter = new HXLDCont();
                counter.GenContourPolygonXld(Row, Col);

                HTuple startphi, endphi;
                HTuple pointorder;
                counter.FitCircleContourXld("atukey", -1, 0, 0, 3, 2,
                    out out_row, out out_col, out out_radius, out startphi, out endphi, out pointorder);
                
                HTuple rowDrop = new HTuple();
                HTuple colDrop = new HTuple();
                if (Row.Length - 5 > m_nNumDropPoints && m_nNumDropPoints > 0)
                {
                    HTuple distance = new HTuple();
                    for (int i = 0; i < Row.Length; ++i)
                    {
                        double dis = HMisc.DistancePp(Row[i], Col[i], xCenter, yCenter);
                        distance.Append(Math.Abs(dis - Center_radius));
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
                    counter.FitCircleContourXld("atukey", -1, 0, 0, 3, 2,
                     out out_row, out out_col, out out_radius, out startphi, out endphi, out pointorder);
                }
                m_hCircle.Dispose();
                m_centerCross.Dispose();
                if (m_bShowCircle)
                {
                    HOperatorSet.GenCrossContourXld(out m_centerCross, out_row, out_col, (out_radius / 2) + 1, (new HTuple(0)).TupleRad());
                    HOperatorSet.GenCircleContourXld(out m_hCircle, out_row, out_col, out_radius,
                        0, 3.1415926 * 2, "positive", 1);
                }   
                m_hCross.Dispose();
                if (m_bShowCross)
                {
                    HOperatorSet.GenCrossContourXld(out m_hCross, Row, Col, 17, (new HTuple(45)).TupleRad());
                }
                
                Result_Array[0] = out_row;
                Result_Array[1] = out_col;
                Result_Array[2] = out_radius;
                return true;
            }
            return false;
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
                    in_radius = VUB[i].Result_Array[j];
                }
            }
            catch (System.Exception ex)
            {

            }

        }
    }
}
