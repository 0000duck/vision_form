using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
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

        // 旋转中心和数据偏差
        public double CenterRow = 0;
        public double CenterColumn = 0;
        public double CenterDev = 0;

        // 仿射变换矩阵和偏差
        public HTuple HomMat2D = new HTuple(1, 0, 0, 0, 1, 0);
        public double HomMat2DDevX = 0;
        public double HomMat2DDevY = 0;

        // 像素比和比例偏差
        public double PixelRatioColumn = 0;
        public double PixelRatioRow = 0;
        public double PixelRatioDevCol = 0;
        public double PixelRatioDevRow = 0;

        // 文件名称
        private string filename = "Calibration.xml";

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
                CenterDev = distanceMax;

                SaveConfig();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }


        /// <summary>
        /// 绝对坐标9点标定
        /// </summary>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public bool CalibNinePointAbsolute(double[] row, double[] column, double[] x, double[] y)
        {
            try
            {
                if (row.Length < 3 || column.Length < 3 || x.Length < 3 || y.Length < 3)
                {
                    return false;
                }

                HTuple qx, qy;
                HOperatorSet.VectorToHomMat2d(column, row, x, y, out HomMat2D);
                HOperatorSet.AffineTransPoint2d(HomMat2D, column, row, out qx, out qy);

                HomMat2DDevX = qx.TupleSub(x).TupleAbs().TupleMax();
                HomMat2DDevY = qy.TupleSub(y).TupleAbs().TupleMax();

                SaveConfig();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }


        /// <summary>
        /// 相对坐标9点标定
        /// </summary>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        public bool CalibNinePointRelative(double[] row, double[] column)
        {
            try
            {
                int length = row.Length;
                int count = 0;

                double[] px = new double[length];
                double[] py = new double[length];
                double[] qx = new double[length];
                double[] qy = new double[length];
                double[] kx = new double[length];
                double[] ky = new double[length];

                for (int i = 0; i < length; i++)
                {
                    px[i] = column[i] - column[4];
                    py[i] = row[i] - row[4];
                    qx[i] = (i % 3 - 1) * TranslationStep;
                    qy[i] = (i / 3 - 1) * TranslationStep;

                    if (px[i] != 0 && py[i] != 0 && qx[i] != 0 && qy[i] != 0)
                    {
                        kx[i] = qx[i] / px[i];
                        ky[i] = qy[i] / py[i];
                        count++;
                    }
                }

                double kxMean = kx.Sum() / count;
                double kyMean = ky.Sum() / count;
                double kxDev = 0;
                double kyDev = 0;

                for (int i = 0; i < count; i++)
                {
                    double value = Math.Abs(kx[i] - kxMean);
                    if (kxDev < value)
                    {
                        kxDev = value;
                    }

                    value = Math.Abs(ky[i] - kyMean);
                    if (kyDev < value)
                    {
                        kyDev = value;
                    }
                }

                PixelRatioColumn = kxMean;
                PixelRatioRow = kyMean;

                PixelRatioDevCol = kxDev;
                PixelRatioDevRow = kyDev;

                SaveConfig();

                return true;
            }
            catch (Exception)
            {
                return false;
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
            string xpath = "//unit[@name=\"" + CalibName + "\"]";
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
