﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System;

namespace StudyLeaveAppraisals.Meta
{
    public class DoSQL
    {
        private readonly IConfiguration _config;

        public DoSQL(IConfiguration config)
        {
            _config = config;
        }

        private string sql = "";

        private void DoSQLCommand(string sql)
        {
            SqlConnection con = new SqlConnection(_config.GetConnectionString("ConString"));
            SqlCommand cmd = new SqlCommand(sql, con);
            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();
        }

        public void CreateNewStudyLeaveRequest(string eventName, DateTime eventDate, int travelCost, int accomCost, int eventCost, int days, int totalReq, DateTime dateRequested, string staffCode, string reqName)
        {
            sql = "insert into StudyLeaveRequests (event, dateofevent, costtravel, costaccomodation, " +
                "costevent, duration, totalrequested, daterequested, staffcode, requestername, Granted) values (' "
                + eventName + "', '" + eventDate.ToString("yyyy-MM-dd") + "', " + travelCost + ", " + accomCost + ", "
                + eventCost + ", " + days + ", " + totalReq + ", '" + dateRequested.ToString("yyyy-MM-dd") + "', '" 
                + staffCode +  "', '" + reqName + "', 'Pending')";

            DoSQLCommand(sql);
        }

        public void ApproveStudyLeaveRequest(int id, string granted, int totalGranted, string fund, int fundYear, string grantedBy) 
        {
            sql = "update StudyLeaveRequests set Granted='" + granted + "', GrantedBy='" + grantedBy + 
                "', GrantedDate='" + DateTime.Now.ToString("yyyy-MM-dd") + "', TotalGranted=" + totalGranted + 
                ", Fund='" + fund + "', FundYear=" + fundYear + " where ID = " + id;
            
            DoSQLCommand(sql);
        }

        public void CancelStudyLeaveRequest(int id)
        {
            sql = "update StudyLeaveRequests set logicaldelete = 1 where ID = " + id;

            DoSQLCommand(sql);
        }
    }
}
