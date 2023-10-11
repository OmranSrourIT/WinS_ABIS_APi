using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinS_ABIS_APi.HandleingCalsses
{
    public class ObjImagesDeviceSystem
    {

        public byte[] RightEyeFromDevice { get; set; }
        public string RightFromSystem { get; set; }
        public byte[] LeftEyeFromDevice { get; set; }
        public string LeftEyeFromSystem { get; set; }
        public string QualityValue { get; set; }

    }
}
