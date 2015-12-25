/**
 * Issa Haddar
 * 201307043
 * iah10
 */

    /*---- Global Variables ----*/
    var courseName = "Cmps258";
    var term = "Spring";
    var year = 2014;
    /*----------------------------*/

/**
 * Adds a custom menu to the active spreadsheet, containing a single menu item
 * for invoking the readRows() function specified above.
 * The onOpen() function, when defined, is automatically invoked whenever the
 * spreadsheet is opened.
 */
function onOpen() 
{
  var sheet = SpreadsheetApp.getActiveSpreadsheet();
  var entries = [{
    name : "Send Attendance Emails",
    functionName : "readRows"
  }];
  sheet.addMenu("Manage", entries);
};

/**
 * This function is called when the Send Emails button is clicked
 */
function readRows() 
{
  /**
   * Retrieves all the rows in the active spreadsheet that contain data and logs the
   * values for each row.
   */
  var sheet = SpreadsheetApp.getActiveSheet();
  var rows = sheet.getDataRange();
  var numRows = rows.getNumRows();
  var values = rows.getValues();     //Returns a two-dimensional array of values, indexed by row, then by column

  for (var i = 2; i < 4; i++)    //traverse the rows
  {
  
    var row = values[i];                                     //get the row
    var studentsEmail = values[i][2];                        //students email
    var studentsName = values[i][0] + " " + values[i][1];    //students name
    var studentsAbsences = values[i][8];                     //number of absences
    var studentsPenalty = (7 * studentsAbsences) / 5;        //students penalty
    Logger.log(studentsPenalty);
    var emailBody = "<h5>Dear "+ studentsName+ ",</h5> <p>This is your latest attendance record for "+courseName+term+year+":</p>";        //the email body
    emailBody = emailBody.concat("<table border='1' cellpadding='3'>");   //html Table
    emailBody = emailBody.concat("<tr bgcolor='#9acd32'><th style='text-align:left'>Date</th><th style='text-align:left'>Attendance Status</th></tr>");
   
   /***************************************************** Loop Over Each Student *********************************************************************/
    var stop=0;                        
    var cellCount=10;
    while(stop !=2)
    {
        var date = values[1][cellCount];
        var cell = values[i][cellCount];                           //get the cell text
        var range = sheet.getRange(i, cellCount).getA1Notation();   //get the cell notation ex: A1
        var cellColor = sheet.getRange(range).getBackground();     //get the cell color 
        //Logger.log(cell);
        if(cell === "" && cellColor=="white") {    //check whether to stop or not
            stop++;
            cellCount++;
            continue;
       } 
       else if(stop==1)
           stop=0;
           
       var status = "";
       emailBody = emailBody.concat("<tr>");
       emailBody = emailBody.concat("<td>"+date+"</td>");
       
        if (cellColor == "#000000") {                                      //no Lecture was held-Black
			status = "No lecture was held";
			emailBody = emailBody.concat("<td bgcolor='#000000'>"+status+"</td>");
        } else if (cellColor == "#FFFF00") {                               // Holiday-Yellow
			status = "University Holiday – no classes";
			emailBody = emailBody.concat("<td bgcolor='#ffff00'>"+status+"</td>");
        } else if (cell =="1") {             //Absent
			status = "Present";
			emailBody = emailBody.concat("<td>"+status+"</td>");
        } else {                                                                  //present
          status = "Absent";
          emailBody = emailBody.concat("<td bgcolor='ff0000'>"+status+"</td>");
        }
        emailBody = emailBody.concat("</tr>");
        Logger.log(status);
        cellCount++;
    }//end while
    
    emailBody = emailBody.concat("</table>");
    emailBody = emailBody.concat("<p> In total you have missed "+studentsAbsences+" lectures and according to the class syllabus, you have incurred a penalty of ");
    emailBody = emailBody.concat(studentsPenalty+"% which means the maximum grade you can receive for this course if you do 100% on all work is "+(100-studentsPenalty)+"%.</p>");
   
   /**** Send Mail ****/
    MailApp.sendEmail({
     to: studentsEmail,
     subject: "Attendence Record",
     htmlBody: emailBody
   });
   
  }// end for
};
