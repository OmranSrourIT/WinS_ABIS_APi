using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ABISAPi.Controllers
{
    public class ValuesController : ApiController
    {
       
        [HttpGet]
        public string GetName(string Name)
        {
            return "value of " + Name;
        }
         
    }
}
