using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace StudyLeaveAppraisals.Meta
{
    public class DoSQL
    {
        private readonly IConfiguration _config;

        public DoSQL(IConfiguration config)
        {
            _config = config;
        }

        private string strSQL = "";

        private void DoSQLCommand(string strSQL)
        {
            SqlConnection con = new SqlConnection(_config.GetConnectionString("ConString"));
            SqlCommand cmd = new SqlCommand(strSQL, con);
            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();
        }

        public void CreateNewStudyLeaveRequest(string sEvent, DateTime dEventDate, int iTravelCost, int iAccomCost, int iEventCost, int iDays, int iTotalReq, DateTime dDateRequested, string sStaffCode, string sReqName)
        {
            strSQL = "insert into StudyLeaveRequests (event, dateofevent, costtravel, costaccomodation, " +
                "costevent, duration, totalrequested, daterequested, staffcode, requestername, Granted) values (' "
                + sEvent + "', '" + dEventDate.ToString("yyyy-MM-dd") + "', " + iTravelCost + ", " + iAccomCost + ", "
                + iEventCost + ", " + iDays + ", " + iTotalReq + ", '" + dDateRequested.ToString("yyyy-MM-dd") + "', '" 
                + sStaffCode +  "', '" + sReqName + "', 'Pending')";

            DoSQLCommand(strSQL);
        }

        public void ApproveStudyLeaveRequest(int ID, string sGranted, int iTotalGranted, string sFund, int iFundYear, string sGrantedBy) 
        {
            strSQL = "update StudyLeaveRequests set Granted='" + sGranted + "', GrantedBy='" + sGrantedBy + 
                "', GrantedDate='" + DateTime.Now.ToString("yyyy-MM-dd") + "', TotalGranted=" + iTotalGranted + 
                ", Fund='" + sFund + "', FundYear=" + iFundYear + " where ID = " + ID;
            
            DoSQLCommand(strSQL);
        }
    }
}
