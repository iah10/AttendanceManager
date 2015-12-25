/**
* Issa Haddar
* 201307043
* Cmps 258
* section 1
*/

using System;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace ExcelEmail
{
    /**
     * The Browser Of The Porgram
     */
    public partial class Browser : Form
    {
        /*---Instance Fields----*/
        private SMTPEmail user;
        private AboutBox1 about;
        /*---------------------*/

        /**
         * The Main Method
         */ 
        [STAThread]
        public static void Main(string[] args)
        {
            Browser b = new Browser();
            b.ShowDialog();
        }

        /**
         * The Constructor
         */ 
        public Browser()
        {
            InitializeComponent();
            about = new AboutBox1();
        }

        /**
         * The exit Menu Item
         */
        private void exitToolStripMenuItem_Click(object sender, EventArgs e) {
            this.Close();
        }

        /**
         * The login in button
         */
        private void loginbtn_Click(object sender, EventArgs e)
        {
            user = new SMTPEmail();
            if (txPassword.Text == "" || txEmail.Text == "" || txtName.Text == "")
            {
                MessageBox.Show("All Fields Are Required", "Caption", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (!user.setEmail(txEmail.Text) && !user.setPassword(txPassword.Text))
            {
                MessageBox.Show("The Email And the Password you Entered are Syntatically Incorrect", "Caption", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txEmail.Text = "";
                txPassword.Text = "";
            }
            else if (!user.setEmail(txEmail.Text))
            {
                MessageBox.Show("The Email you Entered is Syntatically Incorrect", "Caption", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txEmail.Text = "";
            }
            else if (!user.setPassword(txPassword.Text))
            {
                MessageBox.Show("The Password you Entered is Syntatically Incorrect", "Caption", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txPassword.Text = "";
            }
            else if (!user.setName(txtName.Text))
            {
                MessageBox.Show("In Vlaid Name. Example of a valid Names: Issa Haddar", "Caption", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtName.Text = "";
            }
            else
            {
                panel1.Visible = true;
                lblInfo.Text = "Enter Your Course Name, Term, Year, And The Excel Sheet You Want To Process";
            }
        }

        /**
         * The year input
         */
        private void dtpYear_ValueChanged(object sender, EventArgs e) {
            dtpYear.Value = dtpYear.Value;
        }

        /**
         * The Browse button
         */
        private void btnBrowse_Click(object sender, EventArgs e)
        {
            if (excelFilePath.ShowDialog() == DialogResult.OK)
                txtExcelPath.Text = (excelFilePath.FileName);
        }

        /**
         * The process button
         */
        private void btnProcess_Click(object sender, EventArgs e)
        {
            string courseName = String.Empty, term = String.Empty, path = String.Empty;
            int year;
            bool toCreate = true;
            if (!Regex.IsMatch(txtCourseName.Text, @"^[A-Z]{4,6}[0-9]{3}[L]?$")) {
                MessageBox.Show("In Valid ourse Name. Example of a valid Names: CMPS258, PHYS228L", "Caption", MessageBoxButtons.OK, MessageBoxIcon.Error);
                toCreate = false;
            }
            else
                courseName = txtCourseName.Text;
            string selected = this.cmbBxTerm.GetItemText(this.cmbBxTerm.SelectedItem);  //the combo box selected string
            switch (selected)
            {
                case "":
                    MessageBox.Show("Term Is Not Selected", "Caption", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    toCreate = false;
                    break;
                default:
                    term = selected;
                    break;
            }
            if (String.IsNullOrEmpty(txtExcelPath.Text)) {
                MessageBox.Show("Choose Excel File Location", "Caption", MessageBoxButtons.OK, MessageBoxIcon.Error);
                toCreate = false;
            }
            else
                path = @txtExcelPath.Text;
            year = int.Parse(dtpYear.Value.Year.ToString());
            if (toCreate)
            {
                ExcelReader reader = new ExcelReader(path, courseName, term, year, user);
                lblInfo.Text = "Please Wait. The Process Is On Going";
                reader.ExcelEmail();
                lblInfo.Text = "Done. You Can Choose Another Excel Sheet to Progress";
                MessageBox.Show("Done. Please check out the reports generated in your docements folder", "Caption", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Console.WriteLine("Sucess");
            }
        }

        /**
         * The About Window
         */ 
        private void aboutMenu_Click(object sender, EventArgs e) {
            about.ShowDialog();
        }
    }
}
