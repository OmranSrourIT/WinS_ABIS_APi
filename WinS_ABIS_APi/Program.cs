using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using R100ManagerSDKLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using IRIS_WinService.ABIS_API;

namespace IRIS_WinService
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        /// 
        public static R100DeviceControl _iCAMR100DeviceControl_CAMERA = new R100DeviceControl();
        public static R100DeviceControl _iCAMR100DeviceControl_CAPTURE = new R100DeviceControl();

        public static CapturedImages _capturedImage = new CapturedImages();

        public static byte[] m_pLeftIrisImage;
        public static byte[] m_pRightIrisImage;
         

        //To Quailty Eyes GET For both eyes
        public static int IMAGE_RightIrisQualityValue;
        public static int IMAGE_LeftIrisQualityValue;
        static void Main()
        {
           
            ServiceBase[] ServicesToRun;
           
            ServicesToRun = new ServiceBase[]
            { 
                new Service1()
            };

            ServiceBase.Run(ServicesToRun);
             
      
        }

         
        public class CapturedImages
        {
            public byte[] FaceImage;

            public byte[] RawLeftIris;
            public byte[] RawRightIris;

        }

    }
}
