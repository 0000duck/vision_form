using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HalconDotNet;

namespace vision_form
{
    public abstract class VisionUnitBase
    {
        protected string m_strCfgDir;
        protected string m_strName;

        //protected VisionUnitBase();
        public string[] str_in_parm;
        public string all_parm;
        public HTuple[] Result_Array;
        public string ConfigDir { get; set; }
        public string Name { get; set; }
        public abstract string Type { get; }

        public abstract void DrawRecord(HTuple wnd);
        public abstract bool LoadConfig(string str_parm_all);
        public abstract bool process(HTuple window, HObject img);
        public abstract void SaveConfig();
        public abstract bool SetTrainImage(string filename);
        //public abstract DialogResult ShowConfigForm();
    }
}
