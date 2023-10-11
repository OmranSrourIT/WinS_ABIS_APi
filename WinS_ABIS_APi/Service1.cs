using R100ManagerSDKLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
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
using IRIS_WinService.ABIS_API;
using IRIS_WinService.HandleingCalsses;
using IRIS_WinService;
using System.Runtime.InteropServices;

namespace IRIS_WinService
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

            try
            {
                HomeController obj = new HomeController();
                m_HostDBControl = new CHostDBCtrl();

                InitializeComponent();
             
                IRIS_WinService.Program._iCAMR100DeviceControl_CAMERA.OnGetLiveImage += new _IR100DeviceControlEvents_OnGetLiveImageEventHandler(OnGetLiveImage);
                IRIS_WinService.Program._iCAMR100DeviceControl_CAPTURE.OnEnrollReport += new _IR100DeviceControlEvents_OnEnrollReportEventHandler(OnEnrollReport);
                IRIS_WinService.Program._iCAMR100DeviceControl_CAPTURE.OnGetIrisImage += new _IR100DeviceControlEvents_OnGetIrisImageEventHandler(OnGetIrisImage);
                IRIS_WinService.Program._iCAMR100DeviceControl_CAPTURE.OnGetIrisTemplate += new _IR100DeviceControlEvents_OnGetIrisTemplateEventHandler(OnGetIrisTemplate);
                IRIS_WinService.Program._iCAMR100DeviceControl_CAPTURE.OnMatchReport += new _IR100DeviceControlEvents_OnMatchReportEventHandler(OnMatchReport);
                IRIS_WinService.Program._iCAMR100DeviceControl_CAPTURE.OnCaptureReport += new _IR100DeviceControlEvents_OnCaptureReportEventHandler(OnCaptureReport);
                IRIS_WinService.Program._iCAMR100DeviceControl_CAPTURE.OnUserDB += new _IR100DeviceControlEvents_OnUserDBEventHandler(OnUserDB);


            }
            catch (Exception ex)
            {
                var stackTrace = new StackTrace(ex, true);

               Logger.WriteLog("ErrorMessage" + Environment.NewLine + ex.Message + Environment.NewLine + stackTrace);

            }

            

        }
        ImageConverter imgcvt = new ImageConverter();

        private void OnUserDB(int nNumOfUser, int nSizeOfUserDB, object pUserDB)
        {
            int nResult = 0;

            Console.WriteLine("OnUserDB ");

            if (m_nStatus == FILE_SAVE)
            {
                if (nNumOfUser == 0)
                {
                   // MessageBox.Show("Database is empty!", Constants.TITLE, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    if (Helper.ByteArrayToFile(".", "DB.dat", (byte[])pUserDB))
                    {
                        

                      //  if (MessageBox.Show("Download complete.( DB.dat ) \n Do you want synchronization?", Constants.TITLE, MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                   //     {
                            nResult = IRIS_WinService.Program._iCAMR100DeviceControl_CAPTURE.DownloadUserDB(out m_nNumOfUser);

                            if (nResult == Constants.IS_ERROR_NONE)
                            {
                                if (m_nNumOfUser > 0)
                                    m_nStatus = COMPARE;
                                else
                                    m_nStatus = ALL_DELETE;
                            }
                       // }
                    }
                }


            }
            else
            {
                stUSERINFO st;
                string[,] strUserDB;
                int nSize;
                int nNumOfUser_Device = nNumOfUser;
                st = default(stUSERINFO);
                nSize = Marshal.SizeOf(st);


                strUserDB = new string[nNumOfUser_Device, 3];

                int dw = System.Environment.TickCount; //tick

                for (int i = 0; i < nNumOfUser_Device; i++)
                {
                    //    ListViewItem item;

                    IntPtr iPtr = Marshal.AllocHGlobal(nSize);
                    Marshal.Copy((byte[])pUserDB, i * nSize, iPtr, nSize);

                    st = (stUSERINFO)Marshal.PtrToStructure(iPtr, typeof(stUSERINFO));


                    strUserDB[i, 0] = Convert.ToString(st.pID);
                    strUserDB[i, 1] = Convert.ToString(st.pInsertDate);
                    strUserDB[i, 2] = Convert.ToString(st.pUpdateDate);

                }


                if (!m_HostDBControl.IsAvailable())
                {
                    //SetUIOnConnect();
                    return;
                }

                bool[] bIsExist_Host;
                bool[] bIsExist_Device;

                if (m_nStatus == COMPARE)
                {
                    if (m_HostDBControl.IsNewDB())
                        m_nStatus = ALL_INSERT;
                    else
                    {

                        int nNumOfUser_Host;
                        string[,] strUserDB_Host;

                        nResult = m_HostDBControl.LoadEnrolledUserID(out nNumOfUser_Host, out strUserDB_Host);

                        if (nNumOfUser_Host == 0)
                        {
                            m_nStatus = ALL_INSERT;
                        }
                        else
                        {
                            int nInsertUser = 0;
                            string[,] strInsertUserInfo = new string[nNumOfUser, 3];

                            bIsExist_Host = new bool[nNumOfUser_Host];
                            bIsExist_Device = new bool[nNumOfUser];

                            for (int i = 0; i < nNumOfUser; i++)
                            {
                                for (int j = 0; j < nNumOfUser_Host; j++)
                                {
                                    if ((strUserDB[i, 0] == strUserDB_Host[j, 0]))
                                    {
                                        if ((strUserDB[i, 1] == strUserDB_Host[j, 1]))
                                        {
                                            bIsExist_Device[i] = true;
                                            bIsExist_Host[j] = true;

                                            break;
                                        }
                                    }
                                }

                                if (bIsExist_Device[i] == false)
                                {
                                    strInsertUserInfo[nInsertUser, 0] = strUserDB[i, 0];
                                    strInsertUserInfo[nInsertUser, 1] = strUserDB[i, 1];
                                    strInsertUserInfo[nInsertUser, 2] = strUserDB[i, 2];
                                    nInsertUser++;
                                }
                            }


                            int nCheck = 0;

                            for (int i = 0; i < nNumOfUser_Host; i++)
                            {
                                if (bIsExist_Host[i] == false)
                                {
                                    nCheck++;
                                    m_HostDBControl.DeleteUserInfo(strUserDB_Host[i, 0]);
                                }
                            }


                            if (nInsertUser > 0)
                            {
                                m_HostDBControl.InsertDownloadedghadoUserInfo(nInsertUser, strInsertUserInfo);
                            }

                            m_nStatus = FINISH;

                        }

                    }

                    if (m_nStatus == ALL_INSERT)
                    {
                        m_HostDBControl.InsertDownloadedghadoUserInfo(nNumOfUser, strUserDB);
                        m_nStatus = FINISH;
                    }
                }
                else if (m_nStatus == ALL_DELETE)
                {
                    if (!m_HostDBControl.IsNewDB())
                    {
                        m_HostDBControl.DeleteAllUserInfo();

                    }

                    m_nStatus = FINISH;

                }

             //   LoadEnrolledUserInfo();

              //  SetUIOnConnect();

            }

          

        }


        private void OnEnrollReport(int nReportResult, int nFailureCode, int nRightIrisQualityValue, int nLeftIrisQualityValue, string strMatchedUserID)
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
                int m_nEYE = 0;


                //initFrameIrisCamera();

                //  labEnrollResult.Text = string.Empty;

                //  labQuality.Show();

                if (nRightIrisQualityValue != 0)
                {
                    //     prgRQuality.Show();
                    //    prgRQuality.Value = nRightIrisQualityValue;
                    //    labRightQualityValue.Text = nRightIrisQualityValue.ToString();
                }

                if (nLeftIrisQualityValue != 0)
                {
                    //     prgLQuality.Show();
                    //    prgLQuality.Value = nLeftIrisQualityValue;
                    //  labLeftQualityValue.Text = nLeftIrisQualityValue.ToString();
                }
                if (nReportResult == Constants.IS_RST_SUCCESS)
                {

                    IRIS_WinService.Program._iCAMR100DeviceControl_CAPTURE.ClearUserDB();
                    nResult = m_HostDBControl.DeleteAllUserInfo();

                    if (nResult == Constants.IS_ERROR_NONE)
                    {
                        // LoadEnrolledUserInfo();
                        // MessageBox.Show(this, " Delete complete.", Constants.TITLE, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        Logger.WriteLog("ErrorMessage" + nResult); 
                    }

                    //    labEnrollResult.Text = "[OnEnrollReport]\n  nReportResult : Success\n";

                    // IRIS_WinService.Program._iCAMR100DeviceControl_CAPTURE.ControlIndicator(Constants.IS_SND_FINISH_IRIS_CAPTURE, Constants.IS_IND_SUCCESS);

                    stEnrolledUserInfo = new stUSERINFO();

                    nResult = IRIS_WinService.Program._iCAMR100DeviceControl_CAPTURE.GetUserInfo("1", out stEnrolledUserInfo);

                    if (nResult == Constants.IS_ERROR_NONE)
                    {
                        string guid = Guid.NewGuid().ToString();

                        if (m_nEYE != Constants.IS_EYE_LEFT)
                        {
                            nResult = 0; //SaveIrisImage(_enrollmentImagesDirectory, "R" + m_strUserID + "_" + guid, _capturedImage.RawRightIris, out strRPath);

                            if (nResult != Constants.IS_RST_SUCCESS)
                            {
                               // ProcessError(nResult);
                                return;
                            }
                        }
                        if (m_nEYE != Constants.IS_EYE_RIGHT)
                        {
                           //SaveIrisImage(_enrollmentImagesDirectory, "L" + m_strUserID + "_" + guid, _capturedImage.RawLeftIris, out strLPath);

                            if (nResult != Constants.IS_RST_SUCCESS)
                            {
                              //  ProcessError(nResult);
                                return;
                            }
                        }

                        

                        if (m_HostDBControl.IsAvailable())
                        {
                            m_HostDBControl.InsertUserInfo(m_strUserID, nRightIrisQualityValue, nLeftIrisQualityValue, strFacePath, strRPath, strLPath, Convert.ToString(stEnrolledUserInfo.pInsertDate), Convert.ToString(stEnrolledUserInfo.pUpdateDate));

                            m_HostDBControl.SelectEnrolledUserInfo(m_strUserID, out strUserInfo);

                           // ListViewItem item = new ListViewItem(strUserInfo[0]);
                          //  item.SubItems.Add(strUserInfo[1]);
                           // item.SubItems.Add(strUserInfo[2]);


                         //   lstEnrolledUserInfo.Items.Add(item);

                          //  txtUser_ID.Text = strUserInfo[1];

                           // labRQuality.Text = strUserInfo[6];
                           // labLQuality.Text = strUserInfo[7];

                           // txtInsertDate.Text = strUserInfo[10];

                           // picEnrolledAudit.ImageLocation = strUserInfo[3];
                          //  picEnrolledREye.ImageLocation = strUserInfo[4];
                         //   picEnrolledLEye.ImageLocation = strUserInfo[5];

                        }

                    }
                    else
                    {
                        //ProcessError(nResult);
                    }

                }
                else
                {
                  //  labEnrollResult.Text += "[OnEnrollReport]\n  FailureCode : " + ((Constants.Error)nFailureCode).ToString() + "\n";

                    if (nFailureCode == Constants.IS_FAIL_ALREADY_EXIST)
                    {
                      //  labEnrollResult.Text += "  Already exist user. (User ID : " + strMatchedUserID + ")\n";
                    }


                    IRIS_WinService.Program._iCAMR100DeviceControl_CAPTURE.ControlIndicator(Constants.IS_SND_NONE, Constants.IS_IND_FAILURE);


                }

            }
            finally
            {
                if (nReportResult != Constants.IS_RST_FAIL_STATUS)
                {
                    var cccc = nReportResult;
                }
                   // initFrameIrisCamera(true);
            }


        }





        private void OnGetIrisTemplate(int nRightIrisFEDStatus, int nRightIrisLensStatus, int nRightIrisTemplateSize, object objRightIrisTemplate, int nLeftIrisFEDStatus, int nLeftIrisLensStatus, int nLeftIrisTemplateSize, object objLeftIrisTemplate)
        {
            
            try
            {
                if (nRightIrisTemplateSize != 0)
                {
                    IRIS_WinService.Program.m_pRightIrisTemplate = (byte[])objRightIrisTemplate;
                    
                }

                if (nLeftIrisTemplateSize != 0)
                {
                    IRIS_WinService.Program.m_pLeftIrisTemplate = (byte[])objLeftIrisTemplate;
                }

            }
            catch (Exception ex)
            {
                var stackTrace = new StackTrace(ex, true);

                Logger.WriteLog("ErrorMessage" + ex.Message + Environment.NewLine + stackTrace);
            }
             
        }


        private void OnGetLiveImage(int nImageSize, object objLiveImage)
        {
            try
            {
                var xxx = (System.Drawing.Image)imgcvt.ConvertFrom(objLiveImage);

                var Con = Convert.ToBase64String((byte[])objLiveImage);
                var ImageUrl = Con;
                //IMAGE_IRIS.Src = ImageUrl;
                OBJIMAG.IMAGE_Arr = ImageUrl;
                InserImage.Add(OBJIMAG);

            }
            catch (Exception ex)
            {
                var stackTrace = new StackTrace(ex, true);

                Logger.WriteLog("ErrorMessage" + Environment.NewLine + ex.Message + Environment.NewLine + stackTrace);

            }

            
        }


        private void OnCaptureReport(int nReportResult, int nFailureCode)
        {
           
            try
            {

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
            catch (Exception ex)
            {
                var stackTrace = new StackTrace(ex, true);

                Logger.WriteLog("ErrorMessage" + Environment.NewLine + ex.Message + Environment.NewLine + stackTrace);

            }



        }


        private void OnMatchReport(int nMatchType, int nReportResult, int nFailureCode, string strMatchedUserID)
        {
            
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

                    IRIS_WinService.Program._iCAMR100DeviceControl_CAPTURE.ControlIndicator(Constants.IS_SND_IDENTIFIED, Constants.IS_IND_SUCCESS);

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
                        IRIS_WinService.Program._iCAMR100DeviceControl_CAPTURE.ControlIndicator(Constants.IS_SND_NOT_IDENTIFY, Constants.IS_IND_FAILURE);
                    }

                    //ProcessError(nFailureCode);

                }

            }
            catch (Exception ex)
            {
                var stackTrace = new StackTrace(ex, true);

              Logger.WriteLog("ErrorMessage" + Environment.NewLine + ex.Message + Environment.NewLine + stackTrace);

            }
        }
         

        private void OnGetIrisImage(int nRightIrisFEDStatus, int nRightIrisLensStatus, int nRightIrisImageSize, object objRightIrisImage, int nLeftIrisFEDStatus, int nLeftIrisLensStatus, int nLeftIrisImageSize, object objLeftIrisImage)
        {
            

            try
            {
                m_nEYE = 0;


                if (nRightIrisImageSize != 0)
                {
                    m_nEYE += Constants.IS_EYE_RIGHT;

                    var picRightEye = Helper.RawToBitmap((byte[])objRightIrisImage, 640, 480, PixelFormat.Format8bppIndexed);
                    IRIS_WinService.Program._capturedImage.RawRightIris = (byte[])objRightIrisImage;
                     
                }

                if (nLeftIrisImageSize != 0)
                {
                    m_nEYE += Constants.IS_EYE_LEFT;

                    var picLeftEye = Helper.RawToBitmap((byte[])objLeftIrisImage, 640, 480, PixelFormat.Format8bppIndexed);
                    IRIS_WinService.Program._capturedImage.RawLeftIris = (byte[])objLeftIrisImage;
 
                }
                 
            }
            catch (Exception ex)
            {
                var stackTrace = new StackTrace(ex, true);

               Logger.WriteLog("ErrorMessage" + Environment.NewLine + ex.Message + Environment.NewLine + stackTrace);

            }
            finally
            {

                IRIS_WinService.Program.m_pRightIrisImage = (byte[])objRightIrisImage;
                IRIS_WinService.Program.m_pLeftIrisImage = (byte[])objLeftIrisImage;


            }

        }


        protected override void OnStart(string[] args)
        {

            int nResult;
            try
            {
                string ServiceURLPath = ConfigurationManager.AppSettings["UrlSelfHosting"];

                //Logger.WriteLog("تم تشغيل الخدمة بصمات العين بنجاح");
                
                //nResult = WinS_ABIS_APi.Program._iCAMR100DeviceControl_CAPTURE.Open();
                //nResult = WinS_ABIS_APi.Program._iCAMR100DeviceControl_CAMERA.Open();


                var config = new HttpSelfHostConfiguration(ServiceURLPath);
                config.EnableCors();
                config.Formatters.JsonFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/html"));

                config.MaxReceivedMessageSize = 2147483647; // use config for this value


                config.Routes.MapHttpRoute(
                    name: "API",
                    routeTemplate: "api/{controller}/{action}/{id}",
                    defaults: new { controller = "Home", id = RouteParameter.Optional }
                    );

                HttpSelfHostServer server = new HttpSelfHostServer(config);
                server.OpenAsync().Wait();

                //WriteToFile("api http://localhost:1234 in done to calling" + DateTime.Now);

            }
            catch (Exception ex)
            {
                var stackTrace = new StackTrace(ex, true);
               
               Logger.WriteLog("ErrorMessage" + Environment.NewLine + ex.Message + Environment.NewLine  + stackTrace);
           
            }
            
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
