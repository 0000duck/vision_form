using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HalconDotNet;

namespace vision_form
{
    public class UnitRandom : VisionUnitBase
    {
        
        private VisionUnitBase[] VUB;
        
        public int MinValue = 0;
        public int MaxValue = 1000;

        public UnitRandom(VisionUnitBase[] vision_step)
        {
            VUB = vision_step;
            Result_Array = new HTuple[2];
            str_in_parm = new string[2];
        }

        public override string Type
        {
            get
            {
                return "Random";
            }
        }

        public override void DrawRecord(HTuple wnd)
        {
            throw new NotImplementedException();
        }

        public override bool LoadConfig(string str_parm_all)
        {
            string[] sArray = str_parm_all.Split('_');
            MinValue = Convert.ToInt32(sArray[0]);
            MaxValue = Convert.ToInt32(sArray[1]);
            return true;
        }

        public override bool process(HTuple window, HObject img)
        {
            Random r = new Random();

            Result_Array[0] = r.Next(MinValue, MaxValue) * 1.0;
            Result_Array[1] = r.Next(MinValue, MaxValue) * 1.0;

            return true;
        }

        public override void SaveConfig()
        {
            all_parm = MinValue + "_" + MaxValue;
        }

        public override bool SetTrainImage(string filename)
        {
            throw new NotImplementedException();
        }
    }
}
