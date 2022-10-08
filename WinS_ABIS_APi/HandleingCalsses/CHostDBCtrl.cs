using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinS_ABIS_APi.HandleingCalsses
{
    class CHostDBCtrl
    {
        OleDbConnection m_Conn = null;

        string m_ConnStr;

        bool m_bIsNewDB = false;

        public bool IsNewDB()
        {
            return m_bIsNewDB;
        }


        public bool IsAvailable()
        {
            bool bResult = false;

            if (m_Conn != null)
                bResult = true;

            return bResult;
        }
        public int Open(string strSerialNumber)
        {
            int nResult = Constants.IS_ERROR_NONE;

            //m_ConnStr = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + strSerialNumber + ".mdb;User Id=admin;Password=;";

            m_ConnStr = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + strSerialNumber + ".mdb;User Id=admin;Password=;";

            try
            {
                m_Conn = new OleDbConnection(m_ConnStr);

                m_Conn.Open();
                m_bIsNewDB = false;

            }
            catch (OleDbException e)
            {
                if (e.ErrorCode == -2147467259)
                {
                    nResult = CreateDatabase();
                    m_bIsNewDB = true;
                }
                else
                {
                    nResult = Constants.IS_ERROR_UNKNOWN;
                    m_Conn = null;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Msg " + e.Message);
                nResult = Constants.IS_ERROR_UNKNOWN;
                m_Conn = null;
            }

            if (m_Conn != null)
            {
                if (m_Conn.State == ConnectionState.Open)
                    m_Conn.Close();
            }

            return nResult;
        }

        private int CreateDatabase()
        {
            int nResult = Constants.IS_ERROR_UNKNOWN;

            Type objClassType = Type.GetTypeFromProgID("ADOX.Catalog");

            if (objClassType != null)
            {
                try
                {
                    object obj = Activator.CreateInstance(objClassType);

                    // Create MDB file 
                    obj.GetType().InvokeMember("Create", System.Reflection.BindingFlags.InvokeMethod, null, obj,
                                new object[] { m_ConnStr });

                    // Clean up
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(obj);
                    obj = null;

                    CreateTable();

                    nResult = Constants.IS_ERROR_NONE;

                }
                catch (Exception e)
                {
                    Console.WriteLine("Exception  " + e.Message);
                    m_Conn = null;
                }
            }

            return nResult;
        }

        private int CreateTable()
        {

            OleDbCommand command;
            string strSql;


            strSql = "CREATE TABLE EnrolledUserInfo (EXT_UUID AUTOINCREMENT PRIMARY KEY, USER_ID TEXT(40) NOT NULL, EXT_NAME TEXT(255)," +
                "EXT_FACE_IMAGE TEXT(255), EXT_IRIS_IMAGE_RIGHT TEXT(255), EXT_IRIS_IMAGE_LEFT TEXT(255), EXT_IRIS_QUALITY_RIGHT int, EXT_IRIS_QUALITY_LEFT int," +
                "EXT_CARD_NAME TEXT(255), EXT_CARD_ID TEXT(255), EXT_INSERT_DATE TEXT(14) NOT NULL, EXT_UPDATE_DATE TEXT(14) NOT NULL)";


            m_Conn.Open();

            command = new OleDbCommand(strSql, m_Conn);

            command.ExecuteNonQuery();


            if (m_Conn.State == ConnectionState.Open)
                m_Conn.Close();

            return Constants.IS_ERROR_NONE;

        }

        public int InsertUserInfo(string strUserID, int nRIrisQuality, int nLIrisQuality, string strFacePath, string strRPath, string strLPath, string strInsertDate, string strUpdateDate)
        {
            if (m_Conn == null)
                return Constants.IS_ERROR_UNKNOWN;

            OleDbCommand command;
            string strSql;

            strSql = "INSERT INTO EnrolledUserInfo (USER_ID, EXT_IRIS_QUALITY_RIGHT, EXT_IRIS_QUALITY_LEFT, EXT_INSERT_DATE, EXT_UPDATE_DATE, EXT_IRIS_IMAGE_RIGHT, EXT_IRIS_IMAGE_LEFT, EXT_FACE_IMAGE) " +
                "VALUES ('" + strUserID + "', " + nRIrisQuality + "," + nLIrisQuality + ", '" + strInsertDate + "', '" + strUpdateDate + "', '" + strRPath + "', '" + strLPath + "', '" + strFacePath + "')";

            m_Conn.Open();

            command = new OleDbCommand(strSql, m_Conn);

            command.ExecuteNonQuery();


            if (m_Conn.State == ConnectionState.Open)
                m_Conn.Close();

            return Constants.IS_ERROR_NONE;
        }

        public int InsertDownloadedghadoUserInfo(int nNumofRecord, string[,] strUserInfo)
        {
            if (m_Conn == null)
                return Constants.IS_ERROR_UNKNOWN;

            try
            {
                OleDbCommand command = new OleDbCommand();

                m_Conn.Open();

                for (int i = 0; i < nNumofRecord; i++)
                {
                    command.Connection = m_Conn;

                    command.CommandText = "INSERT INTO EnrolledUserInfo (USER_ID, EXT_INSERT_DATE, EXT_UPDATE_DATE) " +
                    "VALUES ('" + strUserInfo[i, 0] + "', '" + strUserInfo[i, 1] + "', '" + strUserInfo[i, 2] + "')";


                    command.ExecuteNonQuery();

                }

            }
            catch (OleDbException e)
            {
                Console.WriteLine("OleDbException  " + e.Message);
            }

            if (m_Conn.State == ConnectionState.Open)
                m_Conn.Close();


            return Constants.IS_ERROR_NONE;
        }

        public int DeleteAllUserInfo()
        {
            if (m_Conn == null)
                return Constants.IS_ERROR_UNKNOWN;

            OleDbCommand command;
            string strSql;

            strSql = "DELETE FROM EnrolledUserInfo";

            m_Conn.Open();

            command = new OleDbCommand(strSql, m_Conn);

            command.ExecuteNonQuery();


            if (m_Conn.State == ConnectionState.Open)
                m_Conn.Close();

            return Constants.IS_ERROR_NONE;
        }

        public int DeleteUserInfo(string strUserID)
        {
            if (m_Conn == null)
                return Constants.IS_ERROR_UNKNOWN;

            try
            {
                OleDbCommand command;
                string strSql;

                strSql = "DELETE FROM EnrolledUserInfo WHERE USER_ID = '" + strUserID + "'";

                m_Conn.Open();

                command = new OleDbCommand(strSql, m_Conn);

                command.ExecuteNonQuery();

            }
            catch (OleDbException e)
            {
                Console.WriteLine("OleDbException  " + e.Message);
            }

            if (m_Conn.State == ConnectionState.Open)
                m_Conn.Close();

            return Constants.IS_ERROR_NONE;
        }



        public int LoadEnrolledUserID(out int nNumofRecord, out string[,] strUserInfo)
        {
            nNumofRecord = 0;
            strUserInfo = null;

            if (m_Conn == null)
                return Constants.IS_ERROR_UNKNOWN;

            try
            {
                OleDbCommand command = new OleDbCommand();

                m_Conn.Open();

                command.Connection = m_Conn;
                command.CommandText = "SELECT COUNT(*) as NumofUser From EnrolledUserInfo";

                OleDbDataReader reader = command.ExecuteReader();


                while (reader.Read())
                {
                    nNumofRecord = reader.GetInt32(0);
                }

                reader.Close();

                int nIndex = 0;

                if (nNumofRecord > 0)
                {
                    strUserInfo = new string[nNumofRecord, 2];

                    command.Connection = m_Conn;
                    command.CommandText = "SELECT USER_ID, EXT_INSERT_DATE From EnrolledUserInfo";

                    reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        strUserInfo[nIndex, 0] = reader["USER_ID"].ToString();
                        strUserInfo[nIndex, 1] = reader["EXT_INSERT_DATE"].ToString();
                        nIndex++;
                    }

                    reader.Close();
                }

            }
            catch (OleDbException e)
            {

                Console.WriteLine("OleDbException  " + e.Message);
            }

            if (m_Conn.State == ConnectionState.Open)
                m_Conn.Close();



            return Constants.IS_ERROR_NONE;
        }

        public int UpdateEnrolledFacePath(string strUserID, string strFacePath)
        {
            if (m_Conn == null)
                return Constants.IS_ERROR_UNKNOWN;

            try
            {
                int nNumofRecord = 0;

                OleDbCommand command = new OleDbCommand();

                m_Conn.Open();

                command.Connection = m_Conn;
                command.CommandText = "SELECT COUNT(*) From EnrolledUserInfo WHERE USER_ID = '" + strUserID + "'";

                OleDbDataReader reader = command.ExecuteReader();


                while (reader.Read())
                {
                    nNumofRecord = reader.GetInt32(0);
                }

                reader.Close();

                if (nNumofRecord <= 0)
                {
                    if (m_Conn.State == ConnectionState.Open)
                        m_Conn.Close();
                    return Constants.IS_ERROR_NOT_EXIST_USER_ID;
                }

                command.CommandText = "UPDATE EnrolledUserInfo Set EXT_FACE_IMAGE='" + strFacePath + "' WHERE USER_ID='" + strUserID + "'";


                command.ExecuteNonQuery();
            }
            catch (OleDbException e)
            {

                Console.WriteLine("OleDbException  " + e.Message);
            }

            if (m_Conn.State == ConnectionState.Open)
                m_Conn.Close();

            return Constants.IS_ERROR_NONE;
        }

        public int UpdateEnrolledUserInfo(string strUserID, string strName, string strCardName, string strCardID)
        {
            int nNumofRecord = 0;

            if (m_Conn == null)
                return Constants.IS_ERROR_UNKNOWN;

            try
            {
                OleDbCommand command = new OleDbCommand();

                m_Conn.Open();

                command.Connection = m_Conn;
                command.CommandText = "SELECT COUNT(*) From EnrolledUserInfo WHERE USER_ID = '" + strUserID + "'";

                OleDbDataReader reader = command.ExecuteReader();


                while (reader.Read())
                {
                    nNumofRecord = reader.GetInt32(0);
                }

                reader.Close();

                if (nNumofRecord <= 0)
                {
                    if (m_Conn.State == ConnectionState.Open)
                        m_Conn.Close();


                    return Constants.IS_ERROR_NOT_EXIST_USER_ID;
                }

                command.CommandText = "UPDATE EnrolledUserInfo Set EXT_NAME='" + strName + "', EXT_CARD_NAME='" + strCardName + "', EXT_CARD_ID='" + strCardID + "' WHERE USER_ID='" + strUserID + "'";



                command.ExecuteNonQuery();
            }
            catch (OleDbException e)
            {

                Console.WriteLine("OleDbException  " + e.Message);
            }

            if (m_Conn.State == ConnectionState.Open)
                m_Conn.Close();

            return Constants.IS_ERROR_NONE;
        }

        public int LoadEnrolledUserInfo(out int nNumofRecord, out string[,] strUserInfo)
        {

            nNumofRecord = 0;
            strUserInfo = null;

            if (m_Conn == null)
                return Constants.IS_ERROR_UNKNOWN;

            try
            {
                OleDbCommand command = new OleDbCommand();

                m_Conn.Open();

                command.Connection = m_Conn;
                command.CommandText = "SELECT COUNT(*) as NumofUser From EnrolledUserInfo";

                OleDbDataReader reader = command.ExecuteReader();


                while (reader.Read())
                {
                    nNumofRecord = reader.GetInt32(0);
                }

                reader.Close();

                int nIndex = 0;

                if (nNumofRecord > 0)
                {
                    strUserInfo = new string[nNumofRecord, 3];

                    command.Connection = m_Conn;
                    command.CommandText = "SELECT EXT_UUID, USER_ID, EXT_NAME From EnrolledUserInfo order by EXT_UUID asc";

                    reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        strUserInfo[nIndex, 0] = reader["EXT_UUID"].ToString();
                        strUserInfo[nIndex, 1] = reader["USER_ID"].ToString();
                        strUserInfo[nIndex, 2] = reader["EXT_NAME"].ToString();

                        nIndex++;
                    }

                    reader.Close();
                }

            }
            catch (OleDbException e)
            {

                Console.WriteLine("OleDbException  " + e.Message);
            }

            if (m_Conn.State == ConnectionState.Open)
                m_Conn.Close();



            return Constants.IS_ERROR_NONE;
        }

        public int SelectEnrolledUserInfo(string strUserID, out string[] strUserInfo)
        {
            strUserInfo = null;

            if (m_Conn == null)
                return Constants.IS_ERROR_UNKNOWN;

            try
            {
                OleDbCommand command = new OleDbCommand();

                m_Conn.Open();

                command.Connection = m_Conn;
                command.CommandText = command.CommandText = "SELECT * From EnrolledUserInfo WHERE USER_ID='" + strUserID + "'";

                OleDbDataReader reader = command.ExecuteReader();


                while (reader.Read())
                {
                    strUserInfo = new string[12];

                    strUserInfo[0] = reader["EXT_UUID"].ToString();
                    strUserInfo[1] = reader["USER_ID"].ToString();
                    strUserInfo[2] = reader["EXT_NAME"].ToString();
                    strUserInfo[3] = reader["EXT_FACE_IMAGE"].ToString();
                    strUserInfo[4] = reader["EXT_IRIS_IMAGE_RIGHT"].ToString();
                    strUserInfo[5] = reader["EXT_IRIS_IMAGE_LEFT"].ToString();
                    strUserInfo[6] = reader["EXT_IRIS_QUALITY_RIGHT"].ToString();
                    strUserInfo[7] = reader["EXT_IRIS_QUALITY_LEFT"].ToString();
                    strUserInfo[8] = reader["EXT_CARD_NAME"].ToString();
                    strUserInfo[9] = reader["EXT_CARD_ID"].ToString();
                    strUserInfo[10] = reader["EXT_INSERT_DATE"].ToString();
                    strUserInfo[11] = reader["EXT_UPDATE_DATE"].ToString();

                }

                reader.Close();


            }
            catch (OleDbException e)
            {

                Console.WriteLine("OleDbException  " + e.Message);
            }

            if (m_Conn.State == ConnectionState.Open)
                m_Conn.Close();



            return Constants.IS_ERROR_NONE;
        }
    }
}
