using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IRIS_WinService.HandleingCalsses
{
    class Constants
    {

        //Voice Language file constants
        public enum Language
        {
            Other = 0,
            English,
            Korean,
            Chinese,
            Spanish,
            Arabic,
            Portuguese,
            Russian,
            German,
            French,
            Turkish,
            Japanese,
            Hindi
        };

        // Application Title.
        public const string TITLE = "R100_SDK_Sample_Device_C#";

        public enum Error
        {
            IS_ERROR_NONE = 0,
            IS_ERROR_UNOPEN = -1000000,
            IS_ERROR_ALREADY_OPEN = -1000001,
            IS_ERROR_CLOSE = -1000002,
            IS_ERROR_COMMUNICATION = -1000003,
            IS_ERROR_AUTHENTICATION = -1000004,
            IS_ERROR_ICAM_FAILURE = -1000005,
            IS_ERROR_PARAMETER = -1000006,
            IS_ERROR_ICAM_RETURN = -1000007,
            IS_ERROR_FEATURE_NOT_SUPPORT = -1000013,
            IS_ERROR_NO_UPDATE_FILE = -1000014,
            IS_ERROR_WRONG_SIZE_UPDATE_FILE = -1000015,
            IS_ERROR_VOICE_FILES_NOT_FOUND = -1000021,
            IS_ERROR_VOICE_FILES_DIR_EMPTY = -1000022,
            IS_ERROR_UPDATE_PACKAGE = -1000023,
            IS_ERROR_ICAM = -1000024,
            IS_TIMEOUT = -1000025,
            IS_ERROR_FAIL_TO_OPEN_CONFIG = -1000026,
            IS_ERROR_INVALID_ORDER = -1000027,
            IS_ERROR_INVALID_VALUE = -1000028,
            IS_ERROR_SYSTEM_BUSY = -1000030,
            IS_ERROR_UNKNOWN = -1000033,
            IS_FAIL_ALREADY_EXIST = -1001,
            IS_FAIL_CAPTURE = -1002,
            IS_FAIL_TIMEOUT = -1003,
            IS_FAIL_ABORT = -1004,
            IS_FAIL_MATCH = -1005,
            IS_FAIL_LOW_QULITY = -1006,
            IS_FAIL_CREATE_TEMPLATE = -1007,
            IS_FAIL_IGNORE = -1008,
            IS_FAIL_FAKE = -1009,
            IS_ERRPR_FAIL_OPEN_DB = -1100000,
            IS_ERROR_ALREADY_EXIST_USER_ID = -1100001,
            IS_ERROR_NOT_EXIST_USER_ID = -1100002,
            IS_ERROR_NOT_EMPTY_DB = -1100003,
            IS_ERROR_EXCEED_DB = -1100004,
            IS_ERROR_ALREADY_EXIST = -1100005,
            IS_ERROR_LOW_QUALITY = -1100006,
            IS_ERROR_CREATE_TEMPLATE = -1100007
        }


        public const int IS_RST_FAILURE = -1;
        public const int IS_RST_SUCCESS = 0;
        public const int IS_RST_FAIL_STATUS = 1;

        // SDK errors
        public const int IS_ERROR_NONE = 0;
        public const int IS_ERROR_UNOPEN = -1000000;
        public const int IS_ERROR_ALREADY_OPEN = -1000001;
        public const int IS_ERROR_CLOSE = -1000002;
        public const int IS_ERROR_COMMUNICATION = -1000003;
        public const int IS_ERROR_AUTHENTICATION = -1000004;
        public const int IS_ERROR_ICAM_FAILURE = -1000005;
        public const int IS_ERROR_PARAMETER = -1000006;
        public const int IS_ERROR_ICAM_RETURN = -1000007;
        public const int IS_ERROR_FEATURE_NOT_SUPPORT = -1000013;
        public const int IS_ERROR_NO_UPDATE_FILE = -1000014;
        public const int IS_ERROR_WRONG_SIZE_UPDATE_FILE = -1000015;
        public const int IS_ERROR_VOICE_FILES_NOT_FOUND = -1000021;
        public const int IS_ERROR_VOICE_FILES_DIR_EMPTY = -1000022;
        public const int IS_ERROR_UPDATE_PACKAGE = -1000023;
        public const int IS_ERROR_ICAM = -1000024;
        public const int IS_TIMEOUT = -1000025;
        public const int IS_ERROR_UNKNOWN = -1000033;
        public const int IS_ERROR_SYSTEM_BUSY = -1000030;

        public const int IS_ERROR_NOT_EXIST_USER_ID = -1100002;
        public const int IS_ERROR_NOT_EMPTY_DB = -1100003;
        public const int IS_ERROR_EXCEED_DB = -1100004;

        public const int IS_ERROR_ALREADY_EXIST = -1100005;
        public const int IS_ERROR_LOW_QUALITY = -1100006;
        public const int IS_ERROR_CREATE_TEMPLATE = -1100007;


        public const int IS_FAIL_ALREADY_EXIST = -1001;
        public const int IS_FAIL_CAPTURE = -1002;
        public const int IS_FAIL_TIMEOUT = -1003;
        public const int IS_FAIL_ABORT = -1004;
        public const int IS_FAIL_MATCH = -1005;
        public const int IS_FAIL_LOW_QUALITY = -1006;
        public const int IS_FAIL_CREATE_TEMPLATE = -1007;

        public const int IS_FAIL_IGNORE = -1008;
        public const int IS_FAIL_FAKE = -1009;

        // iCAM Status
        public const int IS_STAT_DISCONNECT = 1;
        public const int IS_STAT_UPGRADE_STATUS = 6;
        public const int IS_STAT_UPSIDEDOWN = 7;

        public const int IS_NORMAL = 1;
        public const int IS_UPSIDEDOWN = 0;

        // Face Image (Captured) Type
        public const int IS_FI_CAPTURED_JPEG_480X640 = 1;
        public const int IS_FI_CAPTURED_JPEG_1200X1600 = 2;
        public const int IS_FI_CAPTURED_JPEG_1920X2560 = 3;
        public const int IS_FI_CAPTURED_FAIL = -1;

        public const int IS_COLOR_STROBE_OFF = 0;
        public const int IS_COLOR_STROBE_LOW = 1;
        public const int IS_COLOR_STROBE_MIDDLE = 2;
        public const int IS_COLOR_STROBE_HIGH = 3;

        // Eye Selection (Iris Capture Mode)
        public const int IS_EYE_RIGHT = 1;
        public const int IS_EYE_LEFT = 2;
        public const int IS_EYE_BOTH = 3;
        public const int IS_EYE_EITHER = 4;

        public const int IS_FED_LEVEL_1 = 0;
        public const int IS_FED_LEVEL_2 = 1;

        public const int IS_DISABLE = 0;
        public const int IS_ENABLE = 1;

        public const int IS_FACE_AUDIT_OFF = 0;
        public const int IS_FACE_AUDIT_ON = 1;

        public const int IS_ENROLLMENT = 1;
        public const int IS_RECOGNITION = 2;


        public const int IS_IRIS_IMAGE = 0;
        public const int IS_IRIS_TEMPLATE = 1;


        // Iris Status
        public const int IS_IRIS_STAT_NONE = 0;
        public const int IS_IRIS_STAT_LIVE = 1;
        public const int IS_IRIS_STAT_FAKE = 2;
        public const int IS_IRIS_STAT_FAIL = 3;

        // Iris Status - Lens detection
        public const int IS_LENS_STAT_NONE = 0;
        public const int IS_LENS_STAT_HARD = 1;
        public const int IS_LENS_STAT_PATTERN = 2;


        // Version Type
        public const int IS_DEV_VER_SDK = 0;
        public const int IS_DEV_VER_ICAMSW = 1;
        public const int IS_DEV_VER_FS = 2;
        public const int IS_DEV_VER_ICAM_CMDCENTER = 3;
        public const int IS_DEV_VER_ICAM_MANAGER = 4;
        public const int IS_DEV_VER_OS = 5;
        public const int IS_DEV_VER_LIB_CAPTURE = 6;
        public const int IS_DEV_VER_LIB_RECOG = 7;
        public const int IS_DEV_VER_LIB_EYESEEK = 8;
        public const int IS_DEV_VER_LIB_COUNTERMEASURE = 9;
        public const int IS_DEV_VER_LIB_LENS = 10;
        public const int IS_DEV_VER_LIB_TWOPI = 11;


        // Sound Message
        public const int IS_SND_NONE = 0;
        public const int IS_SND_CAMERA_SHUTTER = 1;
        public const int IS_SND_MOVE_FORWARD = 2;
        public const int IS_SND_MOVE_BACKWARD = 3;
        public const int IS_SND_CENTER_EYES_IN_MIRROR = 4;
        public const int IS_SND_IDENTIFIED = 5;
        public const int IS_SND_NOT_IDENTIFY = 6;
        public const int IS_SND_VERIFIED = 7;
        public const int IS_SND_NOT_VERIFY = 8;
        public const int IS_SND_PRESENT_CARD = 9;
        public const int IS_SND_FINISH_IRIS_CAPTURE = 10;
        public const int IS_SND_OPERATION_BEEP = 11;
        public const int IS_SND_SMARTCARD_READ_SUCCESS = 12;
        public const int IS_SND_TRY_AGAIN = 13;

        //Voice Language Indexes
        public const int IS_VOICE_LANGUAGE_OTHER = 0;
        public const int IS_VOICE_LANGUAGE_ENGLISH = 1;
        public const int IS_VOICE_LANGUAGE_KOREAN = 2;
        public const int IS_VOICE_LANGUAGE_CHINESE = 3;
        public const int IS_VOICE_LANGUAGE_SPANISH = 4;
        public const int IS_VOICE_LANGUAGE_ARABIC = 5;
        public const int IS_VOICE_LANGUAGE_PORTUGUESE = 6;
        public const int IS_VOICE_LANGUAGE_RUSSIAN = 7;
        public const int IS_VOICE_LANGUAGE_GERMAN = 8;
        public const int IS_VOICE_LANGUAGE_FRENCH = 9;
        public const int IS_VOICE_LANGUAGE_TURKISH = 10;
        public const int IS_VOICE_LANGUAGE_JAPANESE = 11;
        public const int IS_VOICE_LANGUAGE_HINDI = 12;


        // Indicator Type
        public const int IS_IND_NONE = 0;
        public const int IS_IND_SUCCESS = 1;
        public const int IS_IND_FAILURE = 2;
        public const int IS_IND_BUSY = 3;
        public const int IS_IND_ERROR = 4;
        public const int IS_IND_TURN_ON = 5;
        public const int IS_IND_TURN_OFF = 6;


        public const int IS_LENS_CHECK_NONE = 0;
        public const int IS_LENS_CHECK_HARD = 1;
        public const int IS_LENS_CHECK_PATTERN = 2;

        public const int COLOR_EFFECT_WHITE_BALANCE = 1;
        public const int COLOR_EFFECT_BRIGHTNESS = 2;
        public const int COLOR_EFFECT_SHARPNESS = 3;
        public const int COLOR_EFFECT_SATURATION = 4;
        public const int COLOR_EFFECT_ISO = 5;

        public const int BRIGHTNESS_MINUS_5 = 0;
        public const int BRIGHTNESS_MINUS_4 = 1;
        public const int BRIGHTNESS_MINUS_3 = 2;
        public const int BRIGHTNESS_MINUS_2 = 3;
        public const int BRIGHTNESS_MINUS_1 = 4;
        public const int BRIGHTNESS_DEFAULT = 5;
        public const int BRIGHTNESS_PLUS_1 = 6;
        public const int BRIGHTNESS_PLUS_2 = 7;
        public const int BRIGHTNESS_PLUS_3 = 8;
        public const int BRIGHTNESS_PLUS_4 = 9;
        public const int BRIGHTNESS_PLUS_5 = 10;

        public const int SHARPNESS_MINUS_5 = 0;
        public const int SHARPNESS_MINUS_4 = 1;
        public const int SHARPNESS_MINUS_3 = 2;
        public const int SHARPNESS_MINUS_2 = 3;
        public const int SHARPNESS_MINUS_1 = 4;
        public const int SHARPNESS_DEFAULT = 5;
        public const int SHARPNESS_PLUS_1 = 6;
        public const int SHARPNESS_PLUS_2 = 7;
        public const int SHARPNESS_PLUS_3 = 8;
        public const int SHARPNESS_PLUS_4 = 9;
        public const int SHARPNESS_PLUS_5 = 10;

        public const int SATURATION_MINUS_5 = 0;
        public const int SATURATION_MINUS_4 = 1;
        public const int SATURATION_MINUS_3 = 2;
        public const int SATURATION_MINUS_2 = 3;
        public const int SATURATION_MINUS_1 = 4;
        public const int SATURATION_DEFAULT = 5;
        public const int SATURATION_PLUS_1 = 6;
        public const int SATURATION_PLUS_2 = 7;
        public const int SATURATION_PLUS_3 = 8;
        public const int SATURATION_PLUS_4 = 9;
        public const int SATURATION_PLUS_5 = 10;

        public const int WHITE_BALANCE_AUTO = 0;
        public const int WHITE_BALANCE_DAYLIGHT = 1;
        public const int WHITE_BALANCE_CLOUDY = 2;
        public const int WHITE_BALANCE_INCANDESCENT = 3;
        public const int WHITE_BALANCE_FLUORESCENT = 4;

        public const int ISO_AUTO = 0;
        public const int ISO_100 = 1;
        public const int ISO_200 = 2;
        public const int ISO_400 = 3;


        public const int IS_FACE_AUDIT = 1;
        public const int IS_FACE_CAPTRUE = 2;

        public const int IS_REP_IDENTIFY = 1;
        public const int IS_REP_VERIFY_ID = 2;
        public const int IS_REP_VERIFY_TEMPLATE = 3;
    }
}
