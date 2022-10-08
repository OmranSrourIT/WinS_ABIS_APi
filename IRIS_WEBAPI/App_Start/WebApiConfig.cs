using R100ManagerSDKLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace IRIS_WEBAPI
{
    public static class WebApiConfig
    {

        private static R100DeviceControl _iCAMR100DeviceControl = new R100DeviceControl();
        public static void Register(HttpConfiguration config)
        {
              
        // Web API configuration and services
        _iCAMR100DeviceControl.OnEnrollReport += new _IR100DeviceControlEvents_OnEnrollReportEventHandler(OnEnrollReport);
            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }


        private static void OnEnrollReport(int nReportResult, int nFailureCode, int nRightIrisQualityValue, int nLeftIrisQualityValue, string strMatchedUserID)
        {
            Console.WriteLine("OnEnrollReport");
            try
            {
                int nResult;
                stUSERINFO stEnrolledUserInfo;
                string[] strUserInfo;
                string strRPath = string.Empty;
                string strLPath = string.Empty;
                string strFacePath = string.Empty;
                int prgRQuality = 0;
                string labRightQualityValue = "";
                string labEnrollResult = "";
                string labLeftQualityValue = "";


                if (nRightIrisQualityValue != 0)
                {
                    prgRQuality = nRightIrisQualityValue;
                    labRightQualityValue = nRightIrisQualityValue.ToString();
                }

                if (nLeftIrisQualityValue != 0)
                {
                    prgRQuality = nLeftIrisQualityValue;
                    labLeftQualityValue = nLeftIrisQualityValue.ToString();
                }


            }
            finally
            {

            }


        }
    }
}
