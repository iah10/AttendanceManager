/**
* Issa Haddar
* 201307043
* Cmps 258
* section 1
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Office.Interop.Excel;
using ExcelEmail;
using System.IO;
using System.Drawing;

namespace ExcelEmail
{
    /**
     * The Excel Reader Class 
     */
    public class ExcelReader
    {
        /*---Instance Fields----*/
        private string courseName;
        private string term;
        private string path;
        private int year;
        private SMTPEmail user;
        /*---------------------*/

        /**
         * The Constructor
         */ 
        public ExcelReader(String path, string courseName, string term, int year, SMTPEmail user)
        {
            this.user = user;
            this.year = year;
            this.term = term;
            this.path = @path;
            this.courseName = courseName;
        }

        /**
         * Process th excel file and sets the message body
         */
        public void ExcelEmail()
        {
            //setting the report file Destination My Documents
            var reportPath = Path.Combine(Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments), "AttendanceReport.doc");
            using (StreamWriter report = new StreamWriter(reportPath))
            {
                report.WriteLine("\t\t\t\tAttendance Report\n\n");
                var app = new Application();
                app.Visible = true;
                var book = app.Workbooks.Open(path);        //open the file
                Worksheet sheet = book.Sheets[1];           //getting the excel sheet
                int rowNb = 3;
                while(true)      //looping over the students
                {
                    Range row = sheet.Rows[rowNb];              //getting the row
                    if (string.IsNullOrWhiteSpace(sheet.Cells[rowNb, 1].Text))     //we reached the end
                        break;
                    if (row.Hidden)     //hidden row
                        continue;
                    string email = sheet.Cells[rowNb, 3].Text;  //student's email
                    string name = string.Format("{0} {1}", sheet.Cells[rowNb, 1].Text, sheet.Cells[rowNb, 2].Text);   //student's name

                    StringBuilder htmlWriter = new StringBuilder();
                    int absent = int.Parse(sheet.Cells[rowNb, 9].Text);    //number of absences
                    int penalty = (7 * absent) / 5;
                    int stop = 0;  int count = 11;      //loop conditions
                    htmlWriter.AppendFormat("<h5>Dear {0},</h5> <p>This is your latest attendance record for {1}{2}{3}:</p>", name, courseName, term, year);
                    htmlWriter.Append("<table border='1' cellpadding='3'>");   //html Table
                    htmlWriter.Append("<tr bgcolor='#9acd32'><th style='text-align:left'>Date</th><th style='text-align:left'>Attendance Status</th></tr>");
                    while (stop != 2)
                    {
                        var cell = sheet.Cells[rowNb, count];       //get the cell

                        int colorNumber = System.Convert.ToInt32(((Range)sheet.Cells[rowNb, count]).Interior.Color);    //get the color
                        Color color = System.Drawing.ColorTranslator.FromOle(colorNumber);

                        if (cell.Text == "" && color.Equals(Color.White)) {    //check whether to stop or not
                            stop++;
                            count++;
                            continue;
                        }
	                    if (stop == 1) {
		                    stop = 0;
	                    }

	                    string status = string.Empty;   //the status of the student
                        htmlWriter.Append("<tr>");
                        htmlWriter.AppendFormat("<td>{0}</td>", sheet.Cells[2, count].Text);
                        if ((color).Equals(Color.Black)) {                                   //no Lecture was held-Black 
                            status = "No lecture was held";
                            htmlWriter.AppendFormat("<td bgcolor='#000000'>{0}</td>", status);
                        } else if ((color).Equals(Color.Yellow)) {                            // Holiday-Yellow
                            status = "University Holiday – no classes";
                            htmlWriter.AppendFormat("<td bgcolor='#ffff00'>{0}</td>", status);
                        } else if (int.Parse(sheet.Cells[rowNb, count].Text) != 0) {      //Absent
                            status = "Absent";
                            htmlWriter.AppendFormat("<td bgcolor='ff0000' >{0}</td>", status);
                        } else {                                                        //present
                            status = "Present";
                            htmlWriter.AppendFormat("<td>{0}</td>", status);
                        }
                        
                        htmlWriter.Append("</tr>");
                        count++;
                    }
                    htmlWriter.AppendFormat("</table>");
                    htmlWriter.AppendFormat("<p> In total you have missed {0} lectures and according to the class syllabus, you have incurred a penalty of ", absent);
                    htmlWriter.AppendFormat("{0}% which means the maximum grade you can receive for this course if you do 100% on all work is {1}%.</p>", penalty, (100-penalty));
                    user.SendEmail(name, email, courseName +"Attendance Report", htmlWriter.ToString());
                    report.WriteLine("{0}- {1}\n\t{1} has missed {2} lecture{3}, Penalty: {4}\n", (rowNb - 2), name, absent, (absent != 1) ? "s" : "",penalty);
                    rowNb++;
                }
            }

            //setting the Emials report file Destination to My Documents
            var EmailsReportsPath = Path.Combine(Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments), "EmailsReport.doc");
            using (StreamWriter emailReport = new StreamWriter(EmailsReportsPath))
            {
                emailReport.WriteLine("\t\t\t\t\tEmails Report\n\n");
                emailReport.WriteLine(user.getReport());
            }
        }
    }
}
