using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
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
        private string calibName = "default";

        public Calibration(string name = "default")
        {
            calibName = name;
            Load();
        }

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

                Save();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }


        public bool CalibNinePointAbsolute(double[] row, double[] column, double[] x, double[] y)
        {
            try
            {
                HTuple qx, qy;
                HOperatorSet.VectorToHomMat2d(column, row, x, y, out HomMat2D);
                HOperatorSet.AffineTransPoint2d(HomMat2D, column, row, out qx, out qy);

                HomMat2DDevX = qx.TupleSub(x).TupleAbs().TupleMax();
                HomMat2DDevY = qy.TupleSub(y).TupleAbs().TupleMax();

                Save();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

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

                Save();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }


        public void Save()
        {
            XmlDocument doc = new XmlDocument();
            XmlElement root = doc.CreateElement("Calibration");
            XmlElement calibNode = doc.CreateElement(calibName);

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

            AppendCalibNode(doc, calibNode);
            root.AppendChild(calibNode);
            doc.Save(filename);
        }

        private void AppendCalibNode(XmlDocument doc, XmlElement calibNode)
        {
            // 步长
            XmlElement field = doc.CreateElement(nameof(TranslationStep));
            field.SetAttribute("value", TranslationStep.ToString());
            calibNode.AppendChild(field);

            field = doc.CreateElement(nameof(RotationStep));
            field.SetAttribute("value", RotationStep.ToString());
            calibNode.AppendChild(field);

            // 旋转中心
            field = doc.CreateElement("RotationCenter");
            XmlElement name = doc.CreateElement(nameof(CenterRow));
            name.SetAttribute("value", CenterRow.ToString());
            field.AppendChild(name);

            name = doc.CreateElement(nameof(CenterColumn));
            name.SetAttribute("value", CenterColumn.ToString());
            field.AppendChild(name);

            name = doc.CreateElement(nameof(CenterDev));
            name.SetAttribute("value", CenterDev.ToString());
            field.AppendChild(name);
            calibNode.AppendChild(field);

            // 仿射变换
            field = doc.CreateElement("HomMat2D");
            name = doc.CreateElement(nameof(HomMat2D));
            name.SetAttribute("value", HomMat2D.ToString());
            field.AppendChild(name);

            name = doc.CreateElement(nameof(HomMat2DDevX));
            name.SetAttribute("value", HomMat2DDevX.ToString());
            field.AppendChild(name);

            name = doc.CreateElement(nameof(HomMat2DDevY));
            name.SetAttribute("value", HomMat2DDevY.ToString());
            field.AppendChild(name);
            calibNode.AppendChild(field);


            // 像素比
            field = doc.CreateElement("PixelRatio");
            name = doc.CreateElement(nameof(PixelRatioRow));
            name.SetAttribute("value", PixelRatioRow.ToString());
            field.AppendChild(name);

            name = doc.CreateElement(nameof(PixelRatioColumn));
            name.SetAttribute("value", PixelRatioColumn.ToString());
            field.AppendChild(name);

            name = doc.CreateElement(nameof(PixelRatioDevRow));
            name.SetAttribute("value", PixelRatioDevRow.ToString());
            field.AppendChild(name);

            name = doc.CreateElement(nameof(PixelRatioDevCol));
            name.SetAttribute("value", PixelRatioDevCol.ToString());
            field.AppendChild(name);
            calibNode.AppendChild(field);
        }

        public void Load()
        {
            if (!File.Exists(filename))
            {
                //SaveData();
                return;
            }

            XmlDocument doc = new XmlDocument();
            doc.Load(filename);
            XmlElement root = doc.DocumentElement;
            XmlElement calibNode = root.SelectSingleNode(calibName) as XmlElement;

            if (calibNode == null)
            {
                //SaveData();
                return;
            }

            LoadCalibNode(calibNode);
        }

        private void LoadCalibNode(XmlElement calibNode)
        {
            // 步长
            TranslationStep = Convert.ToDouble(((XmlElement)calibNode.SelectSingleNode
                (nameof(TranslationStep))).GetAttribute("value"));
            RotationStep = Convert.ToDouble(((XmlElement)calibNode.SelectSingleNode
                (nameof(RotationStep))).GetAttribute("value"));

            // 旋转中心
            XmlNodeList nodes = calibNode.SelectSingleNode("RotationCenter").ChildNodes;
            XmlElement name = nodes[0] as XmlElement;
            CenterRow = Convert.ToDouble(name.GetAttribute("value"));
            name = nodes[1] as XmlElement;
            CenterColumn = Convert.ToDouble(name.GetAttribute("value"));
            name = nodes[2] as XmlElement;
            CenterDev = Convert.ToDouble(name.GetAttribute("value"));

            // 矩阵
            nodes = calibNode.SelectSingleNode("HomMat2D").ChildNodes;
            name = nodes[0] as XmlElement;
            string[] array = name.GetAttribute("value").Replace("[", "").Replace("]", "").Split(',');
            HomMat2D = new HTuple();
            foreach (var item in array)
            {
                HomMat2D.Append(Convert.ToDouble(item.Trim()));
            }
            name = nodes[1] as XmlElement;
            HomMat2DDevX = Convert.ToDouble(name.GetAttribute("value"));
            name = nodes[2] as XmlElement;
            HomMat2DDevY = Convert.ToDouble(name.GetAttribute("value"));

            // 像素比
            nodes = calibNode.SelectSingleNode("PixelRatio").ChildNodes;
            name = nodes[0] as XmlElement;
            PixelRatioRow = Convert.ToDouble(name.GetAttribute("value"));
            name = nodes[1] as XmlElement;
            PixelRatioColumn = Convert.ToDouble(name.GetAttribute("value"));
            name = nodes[2] as XmlElement;
            PixelRatioDevRow = Convert.ToDouble(name.GetAttribute("value"));
            name = nodes[3] as XmlElement;
            PixelRatioDevCol = Convert.ToDouble(name.GetAttribute("value"));
        }

        public void ShowDialog()
        {
            FormSet fs = new FormSet(this);
            fs.ShowDialog();
        }
    }
}
