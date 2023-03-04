using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IRIS_IDataMain.configrationClass
{
    public class ResponseImage
    {
        public string ImageRight { get; set; }
        public string ImageLeft { get; set; }
        public string ImageQuailtyRight { get; set; }
        public string ImageQuailtyLeft { get; set; } 
        public string ImageRightByte { get; set; }
        public string ImageLeftByte { get; set; }
        public string MessageQuailty { get; set; }
        public bool ResultQuailty { get; set; }
    }
}