using ClinicalXPDataConnections.Data;
using ClinicalXPDataConnections.Meta;
using ClinicalXPDataConnections.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace StudyLeaveAppraisals.Meta
{
    public class ExportServices : Controller
    {
        public string dlFilePath;
        private readonly ClinicalContext _clinContext;
        private readonly IAppointmentData _appointmentData;
        private readonly IDiseaseData _diseaseData;
        
        public ExportServices(ClinicalContext context) 
        { 
            _clinContext = context;
            _appointmentData = new AppointmentData(_clinContext);
            _diseaseData = new DiseaseData(_clinContext);
        }

        public void ExportReport(List<Appointment> apptList, string username)
        {
            DataTable table = new DataTable();

            table.Columns.Add("CGU Number", typeof(string));
            table.Columns.Add("Name", typeof(string));
            table.Columns.Add("Date", typeof(string));
            table.Columns.Add("Time", typeof(string));
            table.Columns.Add("Clinic", typeof(string));
            table.Columns.Add("Clinician(s)", typeof(string));
            table.Columns.Add("Type", typeof(string));
            table.Columns.Add("Outcome", typeof(string));
            table.Columns.Add("Seen By", typeof(string));

            foreach (var app in apptList)
            {
                string bookedDate = "";
                string bookedTime = "";
                string clinicians = "";
                string seenBy = "";

                if (app.BOOKED_DATE != null)
                {
                    bookedDate = app.BOOKED_DATE.Value.ToString("dd/MM/yyyy");
                }

                if (app.BOOKED_TIME != null)
                {
                    bookedTime = app.BOOKED_TIME.Value.ToString("HH:mm");
                }

                clinicians = app.Clinician;
                if (app.Clinician2 != null) { clinicians = clinicians + ", " + app.Clinician2; }
                if (app.Clinician2 != null) { clinicians = clinicians + ", " + app.Clinician2; }

                seenBy = app.SeenBy;
                if (app.SeenByClinician2 != null) { seenBy = seenBy + ", " + app.SeenByClinician2; }
                if (app.SeenByClinician3 != null) { seenBy = seenBy + ", " + app.SeenByClinician3; }


                table.Rows.Add(app.CGU_No,
                    app.FIRSTNAME + " " + app.LASTNAME,
                    bookedDate,
                    bookedTime,
                    app.Clinic,
                    clinicians,
                    app.AppType,
                    app.Attendance,
                    app.SeenBy                    
                    );
            }

            //return table;
            ToCSV(table, username);

        }

        public void ToCSV(DataTable table, string username)
        {
            string fileName = $"clinicdata-{username}.csv";
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Downloads\\" + fileName);
            StreamWriter sw = new StreamWriter(filePath, false);
            //headers
            for (int i = 0; i < table.Columns.Count; i++)
            {
                sw.Write(table.Columns[i]);
                if (i < table.Columns.Count - 1)
                {
                    sw.Write(",");
                }
            }
            sw.Write(sw.NewLine);
            foreach (DataRow dr in table.Rows)
            {
                for (int i = 0; i < table.Columns.Count; i++)
                {
                    if (!Convert.IsDBNull(dr[i]))
                    {
                        string value = dr[i].ToString();
                        if (value.Contains(','))
                        {
                            value = String.Format("\"{0}\"", value);
                            sw.Write(value);
                        }
                        else
                        {
                            sw.Write(dr[i].ToString());
                        }
                    }
                    if (i < table.Columns.Count - 1)
                    {
                        sw.Write(",");
                    }
                }
                sw.Write(sw.NewLine);
            }
            sw.Close();

            dlFilePath = filePath;

            //DownloadFile(filePath);
        }

        [HttpGet("download")]
        //public async Task<IActionResult> DownloadFile(string filePath)
        public async Task<IActionResult> DownloadFile(string username, string? clinicianCode, string? venueCode, string? outcome, string? seenby, string? diseaseCode, DateTime? startDate, DateTime? endDate)
        {
            //ExportReport(apptList, username);
            //Yep. We actually have to generate all of this again, because we can't just pass the list directly to the URL.
            List<Appointment> appointments = _appointmentData.GetAppointmentsByClinicians(clinicianCode, startDate, endDate);
            List<Appointment> mdcs = _appointmentData.GetMDC(clinicianCode, startDate, endDate);
            List<Appointment> totalappts = appointments.Concat(mdcs).OrderBy(a => a.BOOKED_DATE).ThenBy(a => a.BOOKED_TIME).ToList();

            if (venueCode != null)
            {
                totalappts = totalappts.Where(a => a.FACILITY.ToUpper() == venueCode.ToUpper()).ToList();
            }

            if (outcome != null)
            {
                totalappts = totalappts.Where(a => a.Attendance.ToUpper() == outcome.ToUpper()).ToList();
            }

            if (seenby != null)
            {
                totalappts = totalappts.Where(a => a.SeenBy != null).ToList();
                totalappts = totalappts.Where(a => a.SeenBy.ToUpper() == seenby.ToUpper() ||
                    (a.SeenBy2 != null && a.SeenBy2.ToUpper() == seenby.ToUpper()) ||
                    (a.SeenBy3 != null && a.SeenBy3.ToUpper() == seenby.ToUpper())).ToList();
            }

            if (diseaseCode != null)
            {
                List<Patient> patients = new List<Patient>();
                IPatientData patData = new PatientData(_clinContext);
                foreach (var a in totalappts)
                {
                    patients.Add(patData.GetPatientDetails(a.MPI));
                }
                List<Diagnosis> diags = new List<Diagnosis>();

                foreach (var pat in patients)
                {
                    List<Diagnosis> diagsPerPatient = _diseaseData.GetDiseaseListByPatient(pat.MPI);
                    foreach (var diagnosis in diagsPerPatient)
                    {
                        if (diagnosis.DISEASE_CODE == diseaseCode)
                        {
                            diags.Add(diagnosis);
                        }
                    }
                }

                List<Appointment> actualAppts = new List<Appointment>();

                foreach (var app in totalappts)
                {
                    if (diags.Where(d => d.MPI == app.MPI).Count() > 0)
                    {
                        actualAppts.Add(app);
                    }
                }

                totalappts = actualAppts;

            }

            ExportReport(totalappts, username);

            if (System.IO.File.Exists(dlFilePath))
            {
                return File(System.IO.File.ReadAllBytes(dlFilePath), "text/csv", System.IO.Path.GetFileName(dlFilePath));
            }
            return Redirect("Error");
        }
    }
}
