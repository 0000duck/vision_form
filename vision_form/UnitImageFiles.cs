using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HalconDotNet;

namespace vision_form
{
    public class UnitImageFiles : VisionUnitBase
    {
        private VisionUnitBase[] VUB;

        public HTuple Directory = new HTuple();
        private HTuple hv_ImageFiles = new HTuple();
        private int index = 0;
        private int length = -1;

        public UnitImageFiles(VisionUnitBase[] vision_step)
        {
            VUB = vision_step;
            Result_Array = new HTuple[1];
            str_in_parm = new string[1];
        }

        public override string Type
        {
            get
            {
                return "ImageFiles";
            }
        }

        public override void DrawRecord(HTuple wnd)
        {
            throw new NotImplementedException();
        }

        public override bool LoadConfig(string str_parm_all)
        {
            Directory = str_in_parm;
            return true;
        }

        public override bool process(HTuple window, HObject img)
        {
            if (hv_ImageFiles.Length <= 0 && Directory.Length > 0)
            {
                HOperatorSet.ListFiles(Directory, (new HTuple("files")).TupleConcat("follow_links"), out hv_ImageFiles);
                HOperatorSet.TupleRegexpSelect(hv_ImageFiles,
                    (new HTuple("\\.(tif|tiff|gif|bmp|jpg|jpeg|jp2|png|pcx|pgm|ppm|pbm|xwd|ima|hobj)$")).TupleConcat("ignore_case"),
                    out hv_ImageFiles);

                length = hv_ImageFiles.TupleLength();
            }


            if (index >= length)
            {
                index = 0;
            }

            Result_Array[0] = hv_ImageFiles.ToSArr()[index++];
            
            return true;

        }

        public override void SaveConfig()
        {
            all_parm = Directory;
        }

        public override bool SetTrainImage(string filename)
        {
            throw new NotImplementedException();
        }
    }
}
