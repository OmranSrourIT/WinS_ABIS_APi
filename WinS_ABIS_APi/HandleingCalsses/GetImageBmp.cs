using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;

namespace IRIS_API.configrationClass
{
    public class ImageArr
    {
        // Auto-implemented properties.
        public string IRIS_ImageBmp { get; set; }

        public byte [] IRIS_Template { get; set; }
        public string EyePostion { get;set; } 

        public int IRIS_Quality_Value { get; set; }


    }
}