﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IRIS_WinService.ABIS_API
{
   public class ApiResult
    {
        public object Data { get; set; }
        public string ErrorMessage { get; set; }
        public Boolean IsError { get; set; }
    }
}
