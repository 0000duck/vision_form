using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HalconDotNet;

namespace vision_form
{
    public class UnitCalib9PointAbs : VisionUnitBase
    {
        private VisionUnitBase[] VUB;
        
        public HTuple in_pixel_row = new HTuple();
        public HTuple in_pixel_column = new HTuple();
        public HTuple in_world_x = new HTuple();
        public HTuple in_world_y = new HTuple();

        public HTuple HomMat2D = new HTuple(0, 0, 0, 0, 0, 0);//矩阵
        public HTuple CenterRow = 0;//旋转中心
        public HTuple CenterColumn = 0;

        public HTuple XMaxDeviation = 0;
        public HTuple YMaxDeviation = 0;


        public int PointCount = 0;

        public string CalibDataFileName = "CalibData.cal";


        //public string CalibDataFileName = @"C:\Users\Administrator\Desktop\HelloWorld\CalibData.clb";

        //public HTuple out_offset_x;
        //public HTuple out_offset_y;


        //public enum CalibrationMode { AbsoluteCoord, RelativeCoord }

        //public CalibrationMode CalibMode { get; set; } = CalibrationMode.AbsoluteCoord;


        public bool EnableRotateCenter { get; set; } = false;
        
        public int OffsetXY { get; set; } = 10;

        public int AngleRange { get; set; } = 10;


        
        public UnitCalib9PointAbs(VisionUnitBase[] vision_step)
        {
            this.VUB = vision_step;
            Result_Array = new HTuple[4];
            str_in_parm = new string[8];
            //SaveConfig();
        }

        public override string Type
        {
            get
            {
                return "Calib9PointAbs";
            }
        }

        public override void DrawRecord(HTuple wnd)
        {
            throw new NotImplementedException();
        }

        public override bool LoadConfig(string str_parm_all)
        {
            string[] sArray = str_parm_all.Split('_');

            OffsetXY = Convert.ToInt32(sArray[0]);
            AngleRange = Convert.ToInt32(sArray[1]);
            EnableRotateCenter = Convert.ToBoolean(sArray[2]);
            CalibDataFileName = sArray[3];

            XMaxDeviation = Convert.ToDouble(sArray[4]);
            YMaxDeviation = Convert.ToDouble(sArray[5]);

            CenterRow = Convert.ToDouble(sArray[6]);
            CenterColumn = Convert.ToDouble(sArray[7]);


            int count = Convert.ToInt32(sArray[8]) * 4 + 8;

            for (int i = 9; i <= count; )
            {
                in_pixel_column.Append(Convert.ToDouble(sArray[i++]));
                in_pixel_row.Append(Convert.ToDouble(sArray[i++]));
                in_world_x.Append(Convert.ToDouble(sArray[i++]));
                in_world_y.Append(Convert.ToDouble(sArray[i++]));
            }
            
            return true;
        }

        public override bool process(HTuple window, HObject img)
        {
            try
            {
                #region 9点标定

                if (PointCount <= 9)
                {
                    if (PointCount != 9)
                    {
                        Result_Array[0] = (PointCount % 3 - 1) * OffsetXY;//x偏移量
                        Result_Array[1] = (PointCount / 3 - 1) * OffsetXY;//y偏移量
                        Result_Array[2] = 0;//angle偏移量
                        Result_Array[3] = 1;//标定状态  -1——标定失败，0——标定成功，1——9点标定，2——旋转中心标定

                    }

                    if (PointCount++ == 0)
                    {
                        ClearData();
                        return true;
                    }

                    Refresh_in();

                    if (in_pixel_row.Length == 9)
                    {
                        HTuple qx, qy;
                        HOperatorSet.VectorToHomMat2d(in_pixel_column, in_pixel_row, in_world_x, in_world_y, out HomMat2D);
                        HOperatorSet.AffineTransPoint2d(HomMat2D, in_pixel_column, in_pixel_row, out qx, out qy);

                        XMaxDeviation = qx.TupleSub(in_world_x).TupleAbs().TupleMax();
                        YMaxDeviation = qy.TupleSub(in_world_y).TupleAbs().TupleMax();

                        if (!EnableRotateCenter)
                        {
                            HomMat2D.WriteTuple(CalibDataFileName.Replace("\\", "/"));

                            SaveConfig();

                            Result_Array[0] = 0;
                            Result_Array[1] = 0;
                            Result_Array[2] = 0;
                            Result_Array[3] = 0;

                            PointCount = 0;

                            return true;
                        }
                    }

                }

                #endregion


                #region 旋转中心标定

                if (EnableRotateCenter && PointCount > 9)
                {
                    if (PointCount <= 15)
                    {
                        Result_Array[0] = 0;
                        Result_Array[1] = 0;
                        Result_Array[2] = (AngleRange * 1.0 / 5) * (PointCount - 10);
                        Result_Array[3] = 2;

                        if (PointCount++ == 10)
                        {
                            return true;
                        }

                        Refresh_in();
                    }

                    if (in_pixel_row.Length == 15)
                    {
                        HObject contour;
                        HTuple radius, startPhi, endPhi, pointOrder;

                        HOperatorSet.GenContourPolygonXld(out contour,
                            in_pixel_row.TupleSelectRange(9, 14), in_pixel_column.TupleSelectRange(9, 14));

                        HOperatorSet.FitCircleContourXld(contour, "algebraic", -1, 0, 0, 3, 2,
                            out CenterRow, out CenterColumn, out radius, out startPhi, out endPhi, out pointOrder);

                        HomMat2D.Append(CenterRow);
                        HomMat2D.Append(CenterColumn);
                        HomMat2D.WriteTuple(CalibDataFileName.Replace("\\", "/"));

                        SaveConfig();

                        Result_Array[0] = 0;
                        Result_Array[1] = 0;
                        Result_Array[2] = 0;
                        Result_Array[3] = 0;

                        PointCount = 0;

                        return true;

                    }
                }

                #endregion

                return true;
            }
            catch (Exception)
            {
                PointCount = 0;

                Result_Array[0] = 0;
                Result_Array[1] = 0;
                Result_Array[2] = 0;
                Result_Array[3] = -1;

                return false;
            }
        }


        public override void SaveConfig()
        {
            all_parm = OffsetXY + "_" + AngleRange + "_" + EnableRotateCenter + "_" + CalibDataFileName + "_";

            all_parm += XMaxDeviation + "_" + YMaxDeviation + "_";
            all_parm += CenterRow + "_" + CenterColumn + "_";
            

            if (in_pixel_row != null)
            {
                int length = in_pixel_row.Length;

                all_parm += length + "_";

                for (int i = 0; i < length; i++)
                {
                    all_parm += in_pixel_column.ToDArr()[i] + "_" + in_pixel_row.ToDArr()[i] + "_" + in_world_x.ToDArr()[i] + "_" + in_world_y.ToDArr()[i] + "_";
                }
            }
            
            all_parm.Remove(all_parm.LastIndexOf("_"));
        }

        public override bool SetTrainImage(string filename)
        {
            throw new NotImplementedException();
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
                    in_pixel_row.Append(VUB[i].Result_Array[j]);
                }

                sArray = str_in_parm[1].Split('_');
                if (sArray[0] == "from")
                {
                    int i, j;
                    i = Convert.ToInt32(sArray[1]);
                    j = Convert.ToInt32(sArray[2]);
                    in_pixel_column.Append(VUB[i].Result_Array[j]);
                }

                sArray = str_in_parm[2].Split('_');
                if (sArray[0] == "from")
                {
                    int i, j;
                    i = Convert.ToInt32(sArray[1]);
                    j = Convert.ToInt32(sArray[2]);
                    in_world_x.Append(VUB[i].Result_Array[j]);
                }

                sArray = str_in_parm[3].Split('_');
                if (sArray[0] == "from")
                {
                    int i, j;
                    i = Convert.ToInt32(sArray[1]);
                    j = Convert.ToInt32(sArray[2]);
                    in_world_y.Append(VUB[i].Result_Array[j]);
                }
            }
            catch (System.Exception ex)
            {

            }

        }


        public void ClearData()
        {
            //in_point_index = 0;
            in_pixel_row = new HTuple();
            in_pixel_column = new HTuple();
            in_world_x = new HTuple();
            in_world_y = new HTuple();

            XMaxDeviation = 0;
            YMaxDeviation = 0;

            CenterRow = 0;
            CenterColumn = 0;
        }
    }
}
