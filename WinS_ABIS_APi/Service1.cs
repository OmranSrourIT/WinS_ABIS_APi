using R100ManagerSDKLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Web.Http;
using System.Web.Http.SelfHost;
using WinS_ABIS_APi.ABIS_API;
using WinS_ABIS_APi.HandleingCalsses;

namespace WinS_ABIS_APi
{
    public partial class Service1 : ServiceBase
    {

        private readonly string _enrollmentImagesDirectory = "Enrollment";
        private readonly string _TempImagesDirectory = "Temp";
        private readonly string _faceImagesDirectory = "Face";
        public static string m_strUserID;
        string SigBase64Left;
        string SigBase64Right;

        int m_nWhiteBalance;
        int m_nSaturation;
        int m_nBrightness;
        int m_nISO;
        int m_nSharpness;

        int m_nNumOfUser;

        int m_nEYE = 0;
        int m_nIrisType = -1;

        int m_nCaptureMode = IS_INIT;
        string txtEnrollTimeOut = "20";

        private delegate void DelProgressbarCtrl(bool bIsRun);
        DelProgressbarCtrl m_ProgressbarCtrl;

        CHostDBCtrl m_HostDBControl;

        public const int NONE = 0;
        public const int COMPARE = 1;
        public const int ALL_INSERT = 2;
        public const int ALL_DELETE = 3;
        public const int FINISH = 4;
        public const int FILE_SAVE = 5;

        public const int IS_INIT = 0;
        public const int IS_ENROLL = 1;
        public const int IS_IDENTIFY = 2;
        public const int IS_VERIFY_BY_ID = 3;
        public const int IS_CAPTURE = 4;
        public const int IS_VERIFY_BY_TEMPLATE = 5;


        int m_nStatus = NONE;

        int m_nSelectedRow = 0;


        int m_nColorOffsetX = 0;
        int m_nColorOffsetY = 0;
        ApiResult oApi = new ApiResult();
        public static  List<ImageConfigure> InserImage = new List<ImageConfigure>();
        public readonly ImageConfigure OBJIMAG = new ImageConfigure();
        public Service1()
        {

            HomeController obj = new HomeController();
          
            InitializeComponent();
            WinS_ABIS_APi.Program._iCAMR100DeviceControl_CAMERA.OnGetLiveImage += new _IR100DeviceControlEvents_OnGetLiveImageEventHandler(OnGetLiveImage); 
            WinS_ABIS_APi.Program._iCAMR100DeviceControl_CAPTURE.OnGetIrisImage += new _IR100DeviceControlEvents_OnGetIrisImageEventHandler(OnGetIrisImage);  
            WinS_ABIS_APi.Program._iCAMR100DeviceControl_CAPTURE.OnMatchReport += new _IR100DeviceControlEvents_OnMatchReportEventHandler(OnMatchReport);
            WinS_ABIS_APi.Program._iCAMR100DeviceControl_CAPTURE.OnCaptureReport += new _IR100DeviceControlEvents_OnCaptureReportEventHandler(OnCaptureReport);


        }
        ImageConverter imgcvt = new ImageConverter();
        private void OnGetLiveImage(int nImageSize, object objLiveImage)
        {
            
            var xxx = (System.Drawing.Image)imgcvt.ConvertFrom(objLiveImage);

            var Con = Convert.ToBase64String((byte[])objLiveImage);
            var ImageUrl = Con;
            //IMAGE_IRIS.Src = ImageUrl;
            OBJIMAG.IMAGE_Arr = ImageUrl;
            InserImage.Add(OBJIMAG);
        }


        private void OnCaptureReport(int nReportResult, int nFailureCode)
        {
            Console.WriteLine("OnCaptureReport");

            var labCaptureResult = string.Empty;

            m_nCaptureMode = IS_CAPTURE;
            // initFrameIrisCamera(true);

            if (nReportResult == Constants.IS_ERROR_NONE)
            {
                labCaptureResult = "[OnCaptureReport]\n  nReportResult : Success\n";

                if (m_nIrisType == Constants.IS_IRIS_IMAGE)
                {
                    // btnAddIrisImage.Enabled = true;
                }
                else if (m_nIrisType == Constants.IS_IRIS_TEMPLATE)
                {
                    //  btnAddIrisTemplate.Enabled = true;
                    // btnVerifyByTemplate.Enabled = true;
                }

            }
            else
                labCaptureResult += "[OnCaptureReport]\n  FailureCode : " + ((Constants.Error)nFailureCode).ToString() + "\n";


        }


        private void OnMatchReport(int nMatchType, int nReportResult, int nFailureCode, string strMatchedUserID)
        {
            Console.WriteLine("OnMatchReport");
            try
            {
                int nResult;

                string[] strUserInfo;
                var labIdentifyResult = "";
                var labVerifyResult = "";
                var labCaptureResult = "";

                //initFrameIrisCamera();

                if (nReportResult == Constants.IS_ERROR_NONE)
                {
                    if (nMatchType == Constants.IS_REP_IDENTIFY)
                    {
                        labIdentifyResult = "[OnMatchReport]\n  nReportResult : Success\n";
                    }
                    else if (nMatchType == Constants.IS_REP_VERIFY_ID)
                    {
                        labVerifyResult = "[OnMatchReport]\n  nReportResult : Success\n";
                    }
                    else if (nMatchType == Constants.IS_REP_VERIFY_TEMPLATE)
                    {
                        labCaptureResult = "[OnMatchReport]\n  nReportResult : Success\n";
                    }

                    WinS_ABIS_APi.Program._iCAMR100DeviceControl_CAPTURE.ControlIndicator(Constants.IS_SND_IDENTIFIED, Constants.IS_IND_SUCCESS);

                    if (nMatchType != Constants.IS_REP_VERIFY_TEMPLATE)
                    {
                        nResult = m_HostDBControl.SelectEnrolledUserInfo(strMatchedUserID, out strUserInfo);

                        if (nResult == Constants.IS_ERROR_NONE)
                        {

                            var txtUser_ID = strUserInfo[1];
                            var txtUserName = strUserInfo[2];
                            var txtCardName = strUserInfo[8];
                            var txtCardID = strUserInfo[9];


                            var labRQuality = strUserInfo[6];
                            var labLQuality = strUserInfo[7];

                            var txtInsertDate = strUserInfo[10];

                            //picEnrolledAudit.ImageLocation = strUserInfo[3];
                            //picEnrolledREye.ImageLocation = strUserInfo[4];
                            //picEnrolledLEye.ImageLocation = strUserInfo[5];
                        }
                    }


                }
                else
                {
                    if (nMatchType == Constants.IS_REP_IDENTIFY)
                    {
                        labIdentifyResult += "[OnMatchReport]\n  FailureCode : " + ((Constants.Error)nFailureCode).ToString() + "\n";
                    }
                    else if (nMatchType == Constants.IS_REP_VERIFY_ID)
                    {
                        labVerifyResult += "[OnMatchReport]\n  FailureCode : " + ((Constants.Error)nFailureCode).ToString() + "\n";
                    }
                    else if (nMatchType == Constants.IS_REP_VERIFY_TEMPLATE)
                    {
                        labCaptureResult += "[OnMatchReport]\n  FailureCode : " + ((Constants.Error)nFailureCode).ToString() + "\n";
                    }


                    if (nFailureCode != Constants.IS_FAIL_ABORT && nFailureCode != Constants.IS_FAIL_TIMEOUT)
                    {
                        WinS_ABIS_APi.Program._iCAMR100DeviceControl_CAPTURE.ControlIndicator(Constants.IS_SND_NOT_IDENTIFY, Constants.IS_IND_FAILURE);
                    }

                    //ProcessError(nFailureCode);

                }

            }
            finally
            {
                //    initFrameIrisCamera(true);
            }
        }
         
        private void OnGetIrisImage(int nRightIrisFEDStatus, int nRightIrisLensStatus, int nRightIrisImageSize, object objRightIrisImage, int nLeftIrisFEDStatus, int nLeftIrisLensStatus, int nLeftIrisImageSize, object objLeftIrisImage)
        {
            Console.WriteLine("OnGetIrisImage");

            try
            {
                m_nEYE = 0;


                if (nRightIrisImageSize != 0)
                {
                    m_nEYE += Constants.IS_EYE_RIGHT;

                    var picRightEye = Helper.RawToBitmap((byte[])objRightIrisImage, 640, 480, PixelFormat.Format8bppIndexed);
                    WinS_ABIS_APi.Program._capturedImage.RawRightIris = (byte[])objRightIrisImage;
                     
                }

                if (nLeftIrisImageSize != 0)
                {
                    m_nEYE += Constants.IS_EYE_LEFT;

                    var picLeftEye = Helper.RawToBitmap((byte[])objLeftIrisImage, 640, 480, PixelFormat.Format8bppIndexed);
                    WinS_ABIS_APi.Program._capturedImage.RawLeftIris = (byte[])objLeftIrisImage;
 
                }
                 
            }
            finally
            {
  
                WinS_ABIS_APi.Program.m_pRightIrisImage = (byte[])objRightIrisImage;
                WinS_ABIS_APi.Program.m_pLeftIrisImage = (byte[])objLeftIrisImage;


            }

        }


        protected override void OnStart(string[] args)
        {

            int nResult;
            nResult = WinS_ABIS_APi.Program._iCAMR100DeviceControl_CAPTURE.Open();
            nResult = WinS_ABIS_APi.Program._iCAMR100DeviceControl_CAMERA.Open();

            var config = new HttpSelfHostConfiguration("http://localhost:1234");
            config.EnableCors();
            config.Formatters.JsonFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/html"));

            config.MaxReceivedMessageSize = 2147483647; // use config for this value


            config.Routes.MapHttpRoute(
                name : "API",
                routeTemplate: "api/{controller}/{action}/{id}", 
                defaults : new { controller = "Home", id = RouteParameter.Optional }
                );

            HttpSelfHostServer server = new HttpSelfHostServer(config);
            server.OpenAsync().Wait();

            //WriteToFile("api http://localhost:1234 in done to calling" + DateTime.Now);

           

        }

        protected override void OnStop()
        {
            //WriteToFile("Service is stopped at " + DateTime.Now);
        }
        private void OnElapsedTime(object source, ElapsedEventArgs e)
        {
          
            //WriteToFile("Service is recall at " + DateTime.Now); 
        }

     
    }
}
