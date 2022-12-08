using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using IRIS_WinService.ABIS_API;
using R100ManagerSDKLib;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using IRIS_WinService.HandleingCalsses;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using Newtonsoft.Json;
using System.Web;
using System.Web.Mvc;
using System.Net.Http;
using System.Net;
using System.Web.Http.Cors;
using System.Net.Http.Formatting;
using IRIS_API.configrationClass;
using Newtonsoft.Json.Linq;
using System.Web.Script.Serialization;
using IRIS_IDataMain.configrationClass;
using System.Diagnostics;
using IRIS_WinService.HandleingCalsses;
using System.Configuration;

namespace IRIS_WinService
{
    public class HomeController : ApiController
    {

       public static string SigBase64Left = "";
        public static string SigBase64Right = "";

        ApiResult oApi = new ApiResult();
        List<string> ErrorMessage = new List<string>();
        int m_nIrisType = -1;

        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [System.Web.Http.HttpGet]
        public HttpResponseMessage GetAllImage_Live()
        {
            try
            {
                if (Service1.InserImage.Count > 0)
                {
                    for (var i = 0; i <= Service1.InserImage.Count; i++)
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, Service1.InserImage[i], Configuration.Formatters.JsonFormatter);

                    }

                }


            }
            catch(Exception ex)
            {
                var stackTrace = new StackTrace(ex, true);

               
                Logger.WriteLog("ErrorMessage" + Environment.NewLine + ex.Message + Environment.NewLine + stackTrace);

                return Request.CreateResponse(HttpStatusCode.OK, ex.Message, Configuration.Formatters.JsonFormatter);

            }

            return Request.CreateResponse(HttpStatusCode.OK, Service1.InserImage.Count, Configuration.Formatters.JsonFormatter);
        }

        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [System.Web.Http.HttpGet]
        public async Task<HttpResponseMessage> IrisCapture(int WhichEye , string QualityValue)
        {
             
         SigBase64Left = "";
         SigBase64Right = "";

             int nVolume = 9;

             var nResult2 = IRIS_WinService.Program._iCAMR100DeviceControl_CAPTURE.SetSoundVolume(9);
            var nResult3 = IRIS_WinService.Program._iCAMR100DeviceControl_CAMERA.SetSoundVolume(9);
             
            int nResult = 0;
            int nPurpose, nWhichEye, nCounterMeasureLevel, nLensDetectionLevel, nTimeOut, nIsAuditFace, nIsLive;
            int cmbCaptureWhichEye = WhichEye;
            int txtCaptureTimeOut = 20;
            nPurpose = Constants.IS_ENROLLMENT; //(cmbCapturePurpose.SelectedIndex == 0 ? Constants.IS_ENROLLMENT : Constants.IS_RECOGNITION);
            m_nIrisType = Constants.IS_IRIS_IMAGE; // (cmbCaptureIrisType.SelectedIndex == 0 ? Constants.IS_IRIS_IMAGE : Constants.IS_IRIS_TEMPLATE);
            nCounterMeasureLevel = Constants.IS_FED_LEVEL_1;// (cmbCaptureCounterMeasureLevel.SelectedIndex == 0 ? Constants.IS_FED_LEVEL_1 : Constants.IS_FED_LEVEL_2);
            nLensDetectionLevel = 0; // cmbCaptureLensDetection.SelectedIndex;
            nIsLive = Constants.IS_ENABLE; //(chkCaptureLiveImage.Checked ? Constants.IS_ENABLE : Constants.IS_DISABLE);
            nIsAuditFace = Constants.IS_FACE_AUDIT_OFF;  //(chkCaptureAuditFace.Checked ? Constants.IS_FACE_AUDIT_ON : Constants.IS_FACE_AUDIT_OFF);

            ImageArr imgObjRight = new ImageArr();
            ImageArr imgObjLeft = new ImageArr();
             


            List<ImageArr> ImageArrObj = new List<ImageArr>();
            try
            {
                switch (cmbCaptureWhichEye)
                {
                    case 0:
                        nWhichEye = Constants.IS_EYE_RIGHT;
                        break;
                    case 1:
                        nWhichEye = Constants.IS_EYE_LEFT;
                        break;
                    case 2:
                        nWhichEye = Constants.IS_EYE_BOTH;
                        break;
                    //case 3:
                    //    nWhichEye = Constants.IS_EYE_EITHER;
                    //    break;
                    default:
                        nWhichEye = 0;
                        break;
                }

                if (txtCaptureTimeOut <= 0)
                {
                    var ErrorMessage = Constants.TITLE;
                     
                    return  Request.CreateResponse(HttpStatusCode.OK, ErrorMessage, Configuration.Formatters.JsonFormatter);
                }

                nTimeOut = Convert.ToInt32(txtCaptureTimeOut);


                IRIS_WinService.Program.m_pRightIrisImage = null;
                IRIS_WinService.Program.m_pLeftIrisImage = null;
                 
                if(nWhichEye == 0)
                { 
                    return Request.CreateResponse(HttpStatusCode.OK, "Eyes not Found", Configuration.Formatters.JsonFormatter);
                }
                // IrisImage
                nResult = IRIS_WinService.Program._iCAMR100DeviceControl_CAPTURE.StartIrisCapture(nPurpose, m_nIrisType, nWhichEye, nCounterMeasureLevel, nLensDetectionLevel, nTimeOut, nIsAuditFace, nIsLive);


                if (nResult != Constants.IS_ERROR_NONE)
                {
                    var Error = ProcessError(nResult);
                     
                    return Request.CreateResponse(HttpStatusCode.OK, Error, Configuration.Formatters.JsonFormatter);
                }


                IRIS_WinService.Program._iCAMR100DeviceControl_CAPTURE.ControlIndicator(Constants.IS_SND_CENTER_EYES_IN_MIRROR, Constants.IS_IND_NONE);
                await Task.Delay(10000);

                 
                ////////////IRIS IMAGE ////////////////////////////////////////

                // Get Image Right
                if (IRIS_WinService.Program.m_pRightIrisImage != null)
                {

                    Image imageIris_Right = Helper.RawToBitmap(IRIS_WinService.Program.m_pRightIrisImage, 640, 480, PixelFormat.Format8bppIndexed);

                    using (var ms = new MemoryStream())
                    {

                        imageIris_Right.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);


                        SigBase64Right = Convert.ToBase64String(ms.GetBuffer()); //Get Base64


                        imgObjRight.EyePostion = "Right";
                        imgObjRight.IRIS_ImageBmp = SigBase64Right;


                        ImageArrObj.Add(imgObjRight);


                    }

                }


                // Get Image Left 

                if (IRIS_WinService.Program.m_pLeftIrisImage != null)
                {

                    Image imageIris_Left = Helper.RawToBitmap(IRIS_WinService.Program.m_pLeftIrisImage, 640, 480, PixelFormat.Format8bppIndexed);
                    using (var ms = new MemoryStream())
                    {

                        imageIris_Left.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);

                        SigBase64Left = Convert.ToBase64String(ms.GetBuffer()); //Get Base64


                        imgObjLeft.EyePostion = "Left";
                        imgObjLeft.IRIS_ImageBmp = SigBase64Left;

                        ImageArrObj.Add(imgObjLeft);

                    }



                }

                //call api Check Quailty from URL  "http://localhost:55555/api/IRISID/PassImageIRIS_IDATA1";
                //ttp://10.130.149.200/IRISIDATAWeb

                var URLCapture = ConfigurationManager.AppSettings["UrlCaptureEyes"];

                var BaseAddress = new Uri(URLCapture);


                IList<string> postData = new List<string> {
                SigBase64Left , SigBase64Right , QualityValue
                };

               
                using (var client = new HttpClient())
                { 
                    HttpResponseMessage response = client.PostAsync(BaseAddress, postData, new JsonMediaTypeFormatter()).Result;
                    var ResultResponseEyeQuality = response.Content.ReadAsStringAsync().Result;
                    JavaScriptSerializer js = new JavaScriptSerializer();
                   
                    var  ResponseImagess = js.Deserialize<List<ResponseImage>>(ResultResponseEyeQuality);
                    if(ResponseImagess.Count > 0)
                    {
                        if(ResponseImagess[0].ImageQuailtyLeft !="0" && ResponseImagess[0].ImageQuailtyRight != "0")
                        {
                            IRIS_WinService.Program._iCAMR100DeviceControl_CAPTURE.ControlIndicator(Constants.IS_SND_FINISH_IRIS_CAPTURE, Constants.IS_IND_NONE);

                        }else
                        {
                            nResult = IRIS_WinService.Program._iCAMR100DeviceControl_CAPTURE.Close();
                            nResult = IRIS_WinService.Program._iCAMR100DeviceControl_CAPTURE.Open();
                        }

                        return Request.CreateResponse(HttpStatusCode.OK, ResponseImagess, Configuration.Formatters.JsonFormatter);
                    }
                    

                }

            }
            catch (Exception ex)
            { 
                var stackTrace = new StackTrace(ex, true);
                var frame = stackTrace.GetFrame(0);
                var line = frame.GetFileLineNumber();
                Logger.WriteLog("ErrorMessage" + Environment.NewLine + ex.Message + Environment.NewLine + stackTrace + "Line" + line);
               
                return Request.CreateResponse(HttpStatusCode.OK, ex.Message, Configuration.Formatters.JsonFormatter);
            }

            

            return Request.CreateResponse(HttpStatusCode.OK, "", Configuration.Formatters.JsonFormatter);

        }

        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [System.Web.Http.HttpPost]
        // ImageIrisLeftFromSytem I Mean System Sibile Or Mendix
        public HttpResponseMessage VerifyIRISEyesCapture(List<string> eyes)
        {

            int nResult = 0;

            var LeftIMageFomSystem = eyes[0];
            var RightIMageFomSystem = eyes[1];

            var ResultResponseMatchs = "";
            try
            {

                // Localhost http://localhost:55555/api/IRISID
                // Server http://10.130.149.200/IRISIDATAWeb/api/IRISID

                var URLCapture = ConfigurationManager.AppSettings["UrlCaptureVerify"];

                var BaseAddress = new Uri(URLCapture);


                IList<string> postData = new List<string> {
                    //From Sytem
                 LeftIMageFomSystem  , RightIMageFomSystem, 
                //From IRIS Device
                  SigBase64Left , SigBase64Right
                };


                using (var client = new HttpClient())
                {
                    HttpResponseMessage response = client.PostAsync(BaseAddress, postData, new JsonMediaTypeFormatter()).Result;
                     ResultResponseMatchs = response.Content.ReadAsStringAsync().Result;
                      
                }



            }
            catch (Exception ex)
            {
                var stackTrace = new StackTrace(ex, true);

                Logger.WriteLog("ErrorMessage" + Environment.NewLine + ex.Message + Environment.NewLine + stackTrace);


                return Request.CreateResponse(HttpStatusCode.OK, "Error occoured" + ex.Message, Configuration.Formatters.JsonFormatter);

            }
             
            
            

            if(ResultResponseMatchs == "true")
            { 
                IRIS_WinService.Program._iCAMR100DeviceControl_CAPTURE.ControlIndicator(Constants.IS_SND_VERIFIED, Constants.IS_IND_SUCCESS);
            }
            else
            { 
                IRIS_WinService.Program._iCAMR100DeviceControl_CAPTURE.ControlIndicator(Constants.IS_SND_NOT_VERIFY, Constants.IS_IND_FAILURE);
            } 
            return Request.CreateResponse(HttpStatusCode.OK, ResultResponseMatchs, Configuration.Formatters.JsonFormatter);

        }

        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [System.Web.Http.HttpGet]
        public HttpResponseMessage OpenDeviceIRIS_Capture()
        {
            int nResult;

            try
            {

                nResult = IRIS_WinService.Program._iCAMR100DeviceControl_CAPTURE.Open();
                if (nResult != 0)
                {
                    nResult = IRIS_WinService.Program._iCAMR100DeviceControl_CAMERA.Close();
                    nResult = IRIS_WinService.Program._iCAMR100DeviceControl_CAPTURE.Open();
                    ErrorMessage.Add(((Constants.Error)nResult).ToString());

                    return Request.CreateResponse(HttpStatusCode.OK, ErrorMessage, Configuration.Formatters.JsonFormatter);
                }
                ErrorMessage.Add("ConnectionSuccssfult");
                nResult = IRIS_WinService.Program._iCAMR100DeviceControl_CAPTURE.StartFaceCapture();

                return Request.CreateResponse(HttpStatusCode.OK, ErrorMessage, Configuration.Formatters.JsonFormatter);

            }
            catch (Exception ex)
            {
                var stackTrace = new StackTrace(ex, true);

               Logger.WriteLog("ErrorMessage" + Environment.NewLine + ex.Message + Environment.NewLine + stackTrace);

                return Request.CreateResponse(HttpStatusCode.OK, ex.Message, Configuration.Formatters.JsonFormatter);
            }

            
        }


        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [System.Web.Http.HttpGet]
        public HttpResponseMessage OpenDeviceIRIS_CAMERA()
        {
            int nResult;

            try
            {

                nResult = IRIS_WinService.Program._iCAMR100DeviceControl_CAMERA.Open();
                if (nResult != 0)
                {
                    nResult = IRIS_WinService.Program._iCAMR100DeviceControl_CAPTURE.Close();
                    nResult = IRIS_WinService.Program._iCAMR100DeviceControl_CAMERA.Open();
                    nResult = IRIS_WinService.Program._iCAMR100DeviceControl_CAMERA.StartFaceCapture();
                    ErrorMessage.Add(((Constants.Error)nResult).ToString());

                    return Request.CreateResponse(HttpStatusCode.OK, ErrorMessage, Configuration.Formatters.JsonFormatter);
                }
                ErrorMessage.Add("ConnectionSuccssfult");
                nResult = IRIS_WinService.Program._iCAMR100DeviceControl_CAMERA.StartFaceCapture();

                return Request.CreateResponse(HttpStatusCode.OK, ErrorMessage, Configuration.Formatters.JsonFormatter);

            }
            catch(Exception ex)
            {
                var stackTrace = new StackTrace(ex, true);

               Logger.WriteLog("ErrorMessage" + Environment.NewLine + ex.Message + Environment.NewLine + stackTrace);

                return Request.CreateResponse(HttpStatusCode.OK, ex.Message, Configuration.Formatters.JsonFormatter);

            }
            
            
        }


        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [System.Web.Http.HttpGet]
        public HttpResponseMessage StopLiveImage_Capture()
        {
            int nResult;
            try
            {
               
                nResult = IRIS_WinService.Program._iCAMR100DeviceControl_CAPTURE.Close(); 
            }
            catch(Exception ex)
            {
                nResult = IRIS_WinService.Program._iCAMR100DeviceControl_CAPTURE.Close();
                var stackTrace = new StackTrace(ex, true); 
               Logger.WriteLog("ErrorMessage" + Environment.NewLine + ex.Message + Environment.NewLine + stackTrace);
                return Request.CreateResponse(HttpStatusCode.OK, ex.Message, Configuration.Formatters.JsonFormatter);
            }
            

            return Request.CreateResponse(HttpStatusCode.OK, "Closed Successfuly", Configuration.Formatters.JsonFormatter);
             
        }

        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [System.Web.Http.HttpGet]
        public HttpResponseMessage TakeLiveImage_Capture()
        {
            try
            {
                //int nResult = 0;

                //int nImageType = 2;
                //int nStrobe = 2;

                //nResult = WinS_ABIS_APi.Program._iCAMR100DeviceControl_CAMERA.StopFaceCapture(nImageType, nStrobe);
                // WinS_ABIS_APi.Program._iCAMR100DeviceControl_CAMERA.ControlIndicator(Constants.IS_SND_VERIFIED, Constants.IS_IND_SUCCESS);
                IRIS_WinService.Program._iCAMR100DeviceControl_CAMERA.ControlIndicator(Constants.IS_SND_CAMERA_SHUTTER, Constants.IS_IND_NONE);


            }
            catch (Exception ex)
            {
                var stackTrace = new StackTrace(ex, true);

               Logger.WriteLog("ErrorMessage" + Environment.NewLine + ex.Message + Environment.NewLine + stackTrace);
                return Request.CreateResponse(HttpStatusCode.OK, ex.Message, Configuration.Formatters.JsonFormatter);
            }

            return Request.CreateResponse(HttpStatusCode.OK, "Captured Successfuly", Configuration.Formatters.JsonFormatter);

        }

        
        public string ProcessError(int errorCode)
        {
            try
            {
                oApi.IsError = true;
                oApi.ErrorMessage = ((Constants.Error)errorCode).ToString() + "(" + errorCode + ")" + Constants.TITLE;

            }
            catch (Exception ex)
            {
                var stackTrace = new StackTrace(ex, true);

               Logger.WriteLog("ErrorMessage" + Environment.NewLine + ex.Message + Environment.NewLine + stackTrace);
            }
           
            return oApi.ErrorMessage;
        }





    }
}
