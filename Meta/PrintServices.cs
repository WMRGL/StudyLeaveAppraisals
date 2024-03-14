using Microsoft.AspNetCore.Mvc.RazorPages;
using PdfSharpCore.Pdf;
using PdfSharpCore.Drawing;
using PdfSharpCore.Drawing.Layout;
using StudyLeaveAppraisals.Data;
using StudyLeaveAppraisals.Models;
using System.Globalization;
using System.Net.Http;
using Azure;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using Microsoft.AspNetCore.Hosting.Server;
using System.Net;

namespace StudyLeaveAppraisals.Meta
{
    public class PrintServices
    {
        private readonly DataContext _context;

        public PrintServices() {}
        public void PrintReport(List<Appointments> totalAppts, string? clinicianName, DateTime? dStart, DateTime? dEnd)
        {            
            PdfDocument document = new PdfDocument();
            document.Info.Title = "My PDF";
            PdfPage page = document.AddPage();
            XGraphics gfx = XGraphics.FromPdfPage(page);
            var tf = new XTextFormatter(gfx);
            //set the fonts used for the letters
            XFont font = new XFont("Arial", 12, XFontStyle.Regular);
            XFont fontSmall = new XFont("Arial", 10, XFontStyle.Regular);
            XFont fontBold = new XFont("Arial", 12, XFontStyle.Bold);
            XFont fontBoldSmall = new XFont("Arial", 10, XFontStyle.Bold);
            XFont fontItalic = new XFont("Arial", 12, XFontStyle.Bold);
            //Load the image for the letter head
            XImage image = XImage.FromFile(@"wwwroot\Letterhead.jpg");
            gfx.DrawImage(image, 350, 20, image.PixelWidth / 2, image.PixelHeight / 2);
            //tf.Alignment = XParagraphAlignment.Right;
            tf.DrawString("Clinic summary for " + clinicianName, fontBold, XBrushes.Black, new XRect(20, 150, page.Width, 200));

            //Somehow trying to get a breakdown per clinic
            List<string> clinicsList = new List<string>();
            List<string> monthsList = new List<string>();

            foreach (var a in totalAppts)
            {
                clinicsList.Add(a.Clinic);
                monthsList.Add(DateTime.Parse(a.BOOKED_DATE.ToString()).Month + "/" + DateTime.Parse(a.BOOKED_DATE.ToString()).Year);
            }

            clinicsList = clinicsList.Distinct().ToList();
            monthsList = monthsList.Distinct().ToList();

            int para = 200;
            int iAtt;
            int iDNA;
            int iCPat;
            int iCPro;
            int iNE;
            int iTot;

            tf.DrawString("Attendance by Month", fontBold, XBrushes.Black, new XRect(20, para, page.Width, 200));
            para = para + 30;
            tf.Alignment = XParagraphAlignment.Center;
            tf.DrawString("Month", fontBold, XBrushes.DarkBlue, new XRect(20, para, 20, 40));
            tf.DrawString("Attended", fontBoldSmall, XBrushes.DarkBlue, new XRect(200, para, 20, 40));
            tf.DrawString("Did not attend", fontBoldSmall, XBrushes.DarkBlue, new XRect(270, para, 20, 40));
            tf.DrawString("Cancelled by patient", fontBoldSmall, XBrushes.DarkBlue, new XRect(340, para, 20, 40));
            tf.DrawString("Cancelled by professional", fontBoldSmall, XBrushes.DarkBlue, new XRect(410, para, 20, 40));
            tf.DrawString("Not recorded", fontBoldSmall, XBrushes.DarkBlue, new XRect(490, para, 20, 40));
            tf.DrawString("Total", fontBoldSmall, XBrushes.DarkBlue, new XRect(560, para, 20, 20));
            tf.Alignment = XParagraphAlignment.Left;
            para = para + 20;

            foreach (var m in monthsList)
            {
                int iMonth = DateTime.Parse(m.ToString()).Month;
                int iYear = DateTime.Parse(m.ToString()).Year;

                string month = CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(iMonth) + " " + iYear;

                para = para + 15;
                tf.DrawString(month, font, XBrushes.Black, new XRect(20, para, 100, 20));
                iAtt = totalAppts.Where(a => Convert.ToDateTime(a.BOOKED_DATE).Month == iMonth & Convert.ToDateTime(a.BOOKED_DATE).Year == iYear & a.Attendance == "Attended" & a.SeenByClinician == clinicianName).Count();
                tf.DrawString(iAtt.ToString(), font, XBrushes.Black, new XRect(220, para, 20, 20));
                iDNA = totalAppts.Where(a => Convert.ToDateTime(a.BOOKED_DATE).Month == iMonth & Convert.ToDateTime(a.BOOKED_DATE).Year == iYear & a.Attendance.Contains("Did")).Count();
                tf.DrawString(iDNA.ToString(), font, XBrushes.Black, new XRect(290, para, 20, 20));
                iCPat = totalAppts.Where(a => Convert.ToDateTime(a.BOOKED_DATE).Month == iMonth & Convert.ToDateTime(a.BOOKED_DATE).Year == iYear & a.Attendance.Contains("patient")).Count();
                tf.DrawString(iCPat.ToString(), font, XBrushes.Black, new XRect(360, para, 20, 20));
                iCPro = totalAppts.Where(a => Convert.ToDateTime(a.BOOKED_DATE).Month == iMonth & Convert.ToDateTime(a.BOOKED_DATE).Year == iYear & a.Attendance.Contains("professional")).Count();
                tf.DrawString(iCPro.ToString(), font, XBrushes.Black, new XRect(430, para, 20, 20));
                iNE = totalAppts.Where(a => Convert.ToDateTime(a.BOOKED_DATE).Month == iMonth & Convert.ToDateTime(a.BOOKED_DATE).Year == iYear & a.Attendance.Contains("NOT")).Count();
                tf.DrawString(iNE.ToString(), font, XBrushes.Black, new XRect(500, para, 20, 20));
                iTot = totalAppts.Where(a => Convert.ToDateTime(a.BOOKED_DATE).Month == iMonth & Convert.ToDateTime(a.BOOKED_DATE).Year == iYear & (a.SeenByClinician == clinicianName || a.SeenByClinician == null)).Count();
                tf.DrawString(iTot.ToString(), font, XBrushes.Black, new XRect(570, para, 20, 20));
            }
            para = para + 15;
            tf.DrawString("Grand Totals", fontSmall, XBrushes.Black, new XRect(20, para, 20, 40));
            iAtt = totalAppts.Where(a => a.Attendance == "Attended" & a.SeenByClinician == clinicianName).Count();
            tf.DrawString(iAtt.ToString(), font, XBrushes.Black, new XRect(220, para, 20, 20));
            iDNA = totalAppts.Where(a => a.Attendance.Contains("Did")).Count();
            tf.DrawString(iDNA.ToString(), font, XBrushes.Black, new XRect(290, para, 20, 20));
            iCPat = totalAppts.Where(a => a.Attendance.Contains("patient")).Count();
            tf.DrawString(iCPat.ToString(), font, XBrushes.Black, new XRect(360, para, 20, 20));
            iCPro = totalAppts.Where(a => a.Attendance.Contains("professional")).Count();
            tf.DrawString(iCPro.ToString(), font, XBrushes.Black, new XRect(430, para, 20, 20));
            iNE = totalAppts.Where(a => a.Attendance.Contains("NOT")).Count();
            tf.DrawString(iNE.ToString(), font, XBrushes.Black, new XRect(500, para, 20, 20));
            iTot = totalAppts.Where(a => a.SeenByClinician == clinicianName || a.SeenByClinician == null).Count();
            tf.DrawString(iTot.ToString(), font, XBrushes.Black, new XRect(570, para, 20, 20));

            para = para + 50;

            tf.DrawString("Attendance by Clinic", fontBold, XBrushes.Black, new XRect(20, para, page.Width, 200));
            para = para + 30;
            tf.Alignment = XParagraphAlignment.Center;
            tf.DrawString("Clinic", fontBold, XBrushes.DarkBlue, new XRect(20, para, 20, 40));
            tf.DrawString("Attended", fontBoldSmall, XBrushes.DarkBlue, new XRect(200, para, 20, 40));
            tf.DrawString("Did not attend", fontBoldSmall, XBrushes.DarkBlue, new XRect(270, para, 20, 40));
            tf.DrawString("Cancelled by patient", fontBoldSmall, XBrushes.DarkBlue, new XRect(340, para, 20, 40));
            tf.DrawString("Cancelled by professional", fontBoldSmall, XBrushes.DarkBlue, new XRect(410, para, 20, 40));
            tf.DrawString("Not recorded", fontBoldSmall, XBrushes.DarkBlue, new XRect(490, para, 20, 40));
            tf.DrawString("Total", fontBoldSmall, XBrushes.DarkBlue, new XRect(560, para, 20, 20));
            tf.Alignment = XParagraphAlignment.Left;
            para = para + 20;

            foreach (var c in clinicsList)
            {
                para = para + 15;
                tf.DrawString(c, font, XBrushes.Black, new XRect(20, para, 400, 20));
                iAtt = totalAppts.Where(a => a.Clinic == c & a.Attendance == "Attended" & a.SeenByClinician == clinicianName).Count();
                tf.DrawString(iAtt.ToString(), font, XBrushes.Black, new XRect(220, para, 20, 20));
                iDNA = totalAppts.Where(a => a.Clinic == c & a.Attendance.Contains("Did")).Count();
                tf.DrawString(iDNA.ToString(), font, XBrushes.Black, new XRect(290, para, 20, 20));
                iCPat = totalAppts.Where(a => a.Clinic == c & a.Attendance.Contains("patient")).Count();
                tf.DrawString(iCPat.ToString(), font, XBrushes.Black, new XRect(360, para, 20, 20));
                iCPro = totalAppts.Where(a => a.Clinic == c & a.Attendance.Contains("professional")).Count();
                tf.DrawString(iCPro.ToString(), font, XBrushes.Black, new XRect(430, para, 20, 20));
                iNE = totalAppts.Where(a => a.Clinic == c & a.Attendance.Contains("NOT")).Count();
                tf.DrawString(iNE.ToString(), font, XBrushes.Black, new XRect(500, para, 20, 20));
                iTot = totalAppts.Where(a => a.Clinic == c & (a.SeenByClinician == clinicianName || a.SeenByClinician == null)).Count();
                tf.DrawString(iTot.ToString(), font, XBrushes.Black, new XRect(570, para, 20, 20));
            }

            para = para + 15;
            tf.DrawString("Grand Totals", fontSmall, XBrushes.Black, new XRect(20, para, 20, 40));
            iAtt = totalAppts.Where(a => a.Attendance == "Attended" & a.SeenByClinician == clinicianName).Count();
            tf.DrawString(iAtt.ToString(), font, XBrushes.Black, new XRect(220, para, 20, 20));
            iDNA = totalAppts.Where(a => a.Attendance.Contains("Did")).Count();
            tf.DrawString(iDNA.ToString(), font, XBrushes.Black, new XRect(290, para, 20, 20));
            iCPat = totalAppts.Where(a => a.Attendance.Contains("patient")).Count();
            tf.DrawString(iCPat.ToString(), font, XBrushes.Black, new XRect(360, para, 20, 20));
            iCPro = totalAppts.Where(a => a.Attendance.Contains("professional")).Count();
            tf.DrawString(iCPro.ToString(), font, XBrushes.Black, new XRect(430, para, 20, 20));
            iNE = totalAppts.Where(a => a.Attendance.Contains("NOT")).Count();
            tf.DrawString(iNE.ToString(), font, XBrushes.Black, new XRect(500, para, 20, 20));
            iTot = totalAppts.Where(a => a.SeenByClinician == clinicianName || a.SeenByClinician == null).Count();
            tf.DrawString(iTot.ToString(), font, XBrushes.Black, new XRect(570, para, 20, 20));

            string fileName = "wwwroot\\" + clinicianName.Replace(" ", "_") + "_" + dStart.Value.ToString("yyyy-MM-dd") + "_" + dEnd.Value.ToString("yyyy-MM-dd") + ".pdf";
            string fileNameLocal = @"C:\CGU_DB\" + clinicianName + " " + dStart.Value.ToString("yyyy-MM-dd") + " to " + dEnd.Value.ToString("yyyy-MM-dd") + ".pdf";
            document.Save(fileName);
            
            
            //var uri = new System.Uri(new Uri("file://"), fileName);
            //var converted = uri.AbsoluteUri;

            using (var client = new WebClient())
            {
                //using (var s = client.GetStreamAsync(converted))
                //{
                //    using (var fs = new FileStream(fileNameLocal, FileMode.OpenOrCreate))
                //    {
                //        s.Result.CopyTo(fs);
                //    }
                //}
                client.DownloadFile(fileName, fileNameLocal);
            }

            //File.Delete(fileName);
        }

        
    }
}
