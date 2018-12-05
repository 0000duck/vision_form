using System;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Xml;
using HalconDotNet;

namespace vision_form
{
    public class Calibration
    {
        // 旋转步长和平移步长
        public double RotationStep = 1000;
        public double TranslationStep = 3000;

        // 旋转中心和误差
        public double CenterRow = 0;
        public double CenterColumn = 0;
        public double CenterError = 0;

        // 仿射变换矩阵和误差
        public HTuple HomMat2D = new HTuple(1, 0, 0, 0, 1, 0);
        public double HomMat2DError = 0;

        public bool IsRelative = false;

        // 文件名称
        private readonly string filename = "Calibration.xml";


        // 标定数据名称
        public string CalibName { get; set; } = "default";


        public Calibration(string name)
        {
            CalibName = name;
            LoadConfig();
        }


        /// <summary>
        /// 旋转中心标定
        /// </summary>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        public bool CalibRotationCenter(double[] row, double[] column)
        {
            try
            {
                if (row.Length < 3 || column.Length < 3)
                {
                    return false;
                }

                HObject contour;
                HTuple r = 0, c = 0, radius, startPhi, endPhi, pointOrder;
                HTuple distanceMin, distanceMax = 0;

                // 拟合圆
                HOperatorSet.GenContourPolygonXld(out contour, row, column);
                HOperatorSet.FitCircleContourXld(contour, "algebraic", -1, 0, 0, 3, 2,
                    out r, out c, out radius, out startPhi, out endPhi, out pointOrder);

                // 误差
                HOperatorSet.GenCircleContourXld(out contour, r, c, radius, 0, 6.28318, "positive", 1);
                HOperatorSet.DistancePc(contour, row, column, out distanceMin, out distanceMax);

                CenterRow = r;
                CenterColumn = c;
                CenterError = distanceMin.TupleMax();

                SaveConfig();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }


        /// <summary>
        /// 9点标定
        /// </summary>
        /// <param name="row">像素坐标</param>
        /// <param name="column">像素坐标</param>
        /// <param name="x">世界坐标，相对坐标置null</param>
        /// <param name="y">世界坐标，相对坐标置null</param>
        /// <returns></returns>
        public bool CalibNinePoint(double[] row, double[] column, double[] x = null, double[] y = null)
        {
            try
            {
                if ((row.Length < 3 || column.Length < 3) || (x == null && row.Length != 9))
                {
                    return false;
                }

                IsRelative = false;

                if (x == null || y == null)
                {
                    IsRelative = true;

                    double[] px = new double[9];
                    double[] py = new double[9];
                    double[] wx = new double[9];
                    double[] wy = new double[9];

                    for (int i = 0; i < 9; i++)
                    {
                        px[i] = row[i] - row[4];
                        py[i] = column[i] - column[4];
                        wx[i] = (i % 3 - 1) * TranslationStep;
                        wy[i] = (i / 3 - 1) * TranslationStep;
                    }

                    row = px;
                    column = py;
                    x = wx;
                    y = wy;
                }


                // 获取矩阵
                HOperatorSet.VectorToHomMat2d(row, column, x, y, out HomMat2D);

                // 获取最大误差
                HTuple qx, qy;
                HOperatorSet.AffineTransPoint2d(HomMat2D, row, column, out qx, out qy);
                double ex = qx.TupleSub(x).TupleAbs().TupleMax();
                double ey = qy.TupleSub(y).TupleAbs().TupleMax();
                HomMat2DError = ex > ey ? ex : ey;

                SaveConfig();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }





        /// <summary>
        /// 获取旋转角度后的位置
        /// </summary>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <param name="rotateAngle"></param>
        /// <param name="rotatedRow"></param>
        /// <param name="rotatedColumn"></param>
        public void GetRotatedPose(HTuple row, HTuple column, HTuple rotateAngle,
            out HTuple rotatedRow, out HTuple rotatedColumn)
        {
            try
            {
                HTuple homMat2DIdentity, homMat2DRotate;
                HTuple rad = rotateAngle.TupleRad();

                HOperatorSet.HomMat2dIdentity(out homMat2DIdentity);
                HOperatorSet.HomMat2dRotate(homMat2DIdentity, rad, CenterRow, CenterColumn, out homMat2DRotate);
                HOperatorSet.AffineTransPoint2d(homMat2DRotate, row, column, out rotatedRow, out rotatedColumn);
            }
            catch (Exception)
            {
                rotatedRow = rotatedColumn = null;
            }
        }


        /// <summary>
        /// 获取旋转角度后的位置
        /// </summary>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <param name="startAngle"></param>
        /// <param name="endAngle"></param>
        /// <param name="rotatedRow"></param>
        /// <param name="rotatedColumn"></param>
        public void GetRotatedPose(HTuple row, HTuple column, HTuple startAngle, HTuple endAngle,
            out HTuple rotatedRow, out HTuple rotatedColumn)
        {
            try
            {
                HTuple homMat2DIdentity, homMat2DRotate;
                HTuple rad = endAngle.TupleRad() - startAngle.TupleRad();

                HOperatorSet.HomMat2dIdentity(out homMat2DIdentity);
                HOperatorSet.HomMat2dRotate(homMat2DIdentity, rad, CenterRow, CenterColumn, out homMat2DRotate);
                HOperatorSet.AffineTransPoint2d(homMat2DRotate, row, column, out rotatedRow, out rotatedColumn);
            }
            catch (Exception)
            {
                rotatedRow = rotatedColumn = null;
            }
        }


        /// <summary>
        /// 获取指定旋转中心旋转后的位置
        /// </summary>
        /// <param name="centerRow">旋转中心Row</param>
        /// <param name="centerColumn">旋转中心Column</param>
        /// <param name="row">当前位置Row</param>
        /// <param name="column">当前位置Column</param>
        /// <param name="startAngle">当前角度</param>
        /// <param name="endAngle">旋转后的角度</param>
        /// <param name="rotatedRow">旋转后的位置Row</param>
        /// <param name="rotatedColumn">旋转后的位置Column</param>
        public void GetRotatedPose(HTuple centerRow, HTuple centerColumn, HTuple row, HTuple column,
            HTuple startAngle, HTuple endAngle, out HTuple rotatedRow, out HTuple rotatedColumn)
        {
            try
            {
                HTuple homMat2DIdentity, homMat2DRotate;
                HTuple rad = endAngle.TupleRad() - startAngle.TupleRad();

                HOperatorSet.HomMat2dIdentity(out homMat2DIdentity);
                HOperatorSet.HomMat2dRotate(homMat2DIdentity, rad, centerRow, centerColumn, out homMat2DRotate);
                HOperatorSet.AffineTransPoint2d(homMat2DRotate, row, column, out rotatedRow, out rotatedColumn);
            }
            catch (Exception)
            {
                rotatedRow = rotatedColumn = null;
            }
        }


        /// <summary>
        /// 图像坐标系到世界坐标系
        /// </summary>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void ImageToWorldPose(HTuple row, HTuple column, out HTuple x, out HTuple y)
        {
            HOperatorSet.AffineTransPoint2d(HomMat2D, row, column, out x, out y);
        }


        /// <summary>
        /// 获取通过指定矩阵变换后的世界坐标
        /// </summary>
        /// <param name="homMat2D">矩阵</param>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void ImageToWorldPose(HTuple homMat2D, HTuple row, HTuple column, out HTuple x, out HTuple y)
        {
            HOperatorSet.AffineTransPoint2d(homMat2D, row, column, out x, out y);
        }


        /// <summary>
        /// 获取世界坐标系位置
        /// </summary>
        /// <param name="startRow">当前位置Row</param>
        /// <param name="startColumn">当前位置Column</param>
        /// <param name="startAngle">当前角度</param>
        /// <param name="endRow">相对坐标有效，绝对坐标置null</param>
        /// <param name="endColumn">相对坐标有效，绝对坐标置null</param>
        /// <param name="endAngle">目标角度</param>
        /// <param name="x">世界坐标系X</param>
        /// <param name="y">世界坐标系Y</param>
        /// <param name="angle">相对角度</param>
        public void GetWorldPose(HTuple startRow, HTuple startColumn, HTuple startAngle,
                                HTuple endRow, HTuple endColumn, HTuple endAngle,
                                out HTuple x, out HTuple y, out HTuple angle)
        {
            try
            {
                HTuple row, column;
                GetRotatedPose(startRow, startColumn, startAngle, endAngle, out row, out column);
                angle = endAngle - startAngle;

                if (IsRelative)
                {
                    row = endRow - row;
                    column = endColumn - column;
                }

                HOperatorSet.AffineTransPoint2d(HomMat2D, row, column, out x, out y);

            }
            catch (Exception)
            {
                x = y = angle = null;
            }
        }


        /// <summary>
        /// 通过指定的矩阵和旋转中心获取世界坐标
        /// </summary>
        /// <param name="homMat2D"></param>
        /// <param name="centerRow"></param>
        /// <param name="centerColumn"></param>
        /// <param name="startRow"></param>
        /// <param name="startColumn"></param>
        /// <param name="startAngle"></param>
        /// <param name="endRow"></param>
        /// <param name="endColumn"></param>
        /// <param name="endAngle"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="angle"></param>
        public void GetWorldPose(HTuple homMat2D, HTuple centerRow, HTuple centerColumn,
                                HTuple startRow, HTuple startColumn, HTuple startAngle,
                                HTuple endRow, HTuple endColumn, HTuple endAngle,
                                out HTuple x, out HTuple y, out HTuple angle)
        {
            try
            {
                HTuple row, column;
                GetRotatedPose(centerRow, centerColumn, startRow, startColumn, startAngle, endAngle, out row, out column);
                angle = endAngle - startAngle;

                if (IsRelative)
                {
                    row = endRow - row;
                    column = endColumn - column;
                }

                HOperatorSet.AffineTransPoint2d(homMat2D, row, column, out x, out y);

            }
            catch (Exception)
            {
                x = y = angle = null;
            }
        }






        /// <summary>
        /// 保存配置和数据
        /// </summary>
        public void SaveConfig()
        {
            XmlDocument doc = new XmlDocument();
            XmlElement root = doc.CreateElement("Calibration");
            XmlElement unit = doc.CreateElement("unit");
            unit.SetAttribute("name", CalibName);

            if (!File.Exists(filename))
            {
                XmlDeclaration dec = doc.CreateXmlDeclaration("1.0", "UTF-8", "yes");
                doc.AppendChild(dec);
                doc.AppendChild(root);
            }
            else
            {
                doc.Load(filename);
                root = doc.DocumentElement;
            }

            AppendCalibNode(doc, unit);
            string xpath = "/Calibration/unit[@name=\"" + CalibName + "\"]";
            XmlNode oldNode = root.SelectSingleNode(xpath);
            if (oldNode == null)
            {
                root.AppendChild(unit);
            }
            else
            {
                root.ReplaceChild(unit, oldNode);
            }

            doc.Save(filename);
        }


        /// <summary>
        /// 添加实例到标定节点
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="unit"></param>
        private void AppendCalibNode(XmlDocument doc, XmlElement unit)
        {
            Type type = GetType();
            FieldInfo[] fields = type.GetFields();

            foreach (var item in fields)
            {
                XmlElement field = doc.CreateElement("field");
                field.SetAttribute("name", item.Name);
                field.SetAttribute("value", item.GetValue(this).ToString());
                unit.AppendChild(field);
            }
        }


        /// <summary>
        /// 加载配置和数据
        /// </summary>
        public void LoadConfig()
        {
            if (!File.Exists(filename))
            {
                return;
            }

            XmlDocument doc = new XmlDocument();
            doc.Load(filename);
            XmlElement root = doc.DocumentElement;
            string xpath = "/Calibration/unit[@name=\"" + CalibName + "\"]";
            XmlNode nuit = root.SelectSingleNode(xpath);

            if (nuit != null)
            {
                AppendCalibData(nuit);
            }
        }


        /// <summary>
        /// 加载标定节点数据到实例
        /// </summary>
        /// <param name="unit"></param>
        private void AppendCalibData(XmlNode unit)
        {
            Type type = GetType();
            FieldInfo[] fields = type.GetFields();

            foreach (var item in fields)
            {
                string xpath = "/Calibration/unit[@name=\"" + CalibName + "\"]/field[@name=\"" + item.Name + "\"]";
                XmlNode node = unit.SelectSingleNode(xpath);

                if (node == null)
                {
                    continue;
                }

                object value = null;
                string strValue = ((XmlElement)node).GetAttribute("value");

                // 类型转换
                TypeConverter converter = TypeDescriptor.GetConverter(item.FieldType);
                if (converter.CanConvertFrom(strValue.GetType()))
                {
                    value = converter.ConvertFrom(strValue);
                }
                else
                {
                    converter = TypeDescriptor.GetConverter(strValue);
                    if (converter.CanConvertTo(item.FieldType))
                    {
                        value = converter.ConvertTo(item, item.FieldType);
                    }
                }

                if (value != null)
                {
                    item.SetValue(this, value);
                }
                else
                {
                    // 解析无法进行类型转换的类型
                    if (((XmlElement)node).GetAttribute("name") == nameof(HomMat2D))
                    {
                        HomMat2D = new HTuple();
                        string[] array = strValue.Replace("[", "").Replace("]", "").Split(',');
                        foreach (var i in array)
                        {
                            HomMat2D.Append(Convert.ToDouble(i.Trim()));
                        }
                    }
                }
            }
        }


        /// <summary>
        /// 显示设置对话框
        /// </summary>
        public void ShowDialog()
        {
            FormSet fs = new FormSet(this);
            fs.ShowDialog();
        }
    }
}
