﻿using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.IO;

namespace CUESYSv._01
{
    public partial class Form1 : Form
    {
        ///// NOTES START //////////////////////////////////////////////////////////
        // Should include log items stored on database
        // Bookings only in single hour "slots", would be better to custom set
        // Cannot search for booking (by room, date or customer)
        // Only view and edit upcoming x days
        // User+Pass check, insecure - later versions should use a database lookup
        // formatting odd on maximize/resize/different screen resolutions
        // menu is shown when software ran, this allows modification of customer entries before login - not secure
        // devlog out of sync with actions
        // autoscroll devlogs
        // need to hide menu bar on start (good for debug though)
        ///// NOTES END ////////////////////////////////////////////////////////////


        ///// VARIABLES START //////////////////////////////////////////////////////
        dbConn mysqlConn = new dbConn();
        private string varFloor;
        private string varRoom;
        ///// VARIABLES END ////////////////////////////////////////////////////////

        ///// METHODS START ////////////////////////////////////////////////////////
        public void devLogs(string logItem)
        {//Write development log to DevLog
            using (StreamWriter devlog = new StreamWriter("DevLog.txt", append: true))
            { devlog.WriteLine(DateTime.Now + " --- " + logItem); }//Concat current time and logItem and write to DevLog file
        }
        public bool dbConfig()
        {
            try
            {
                mysqlConn.varConfigServer = "gbupot.cucstudents.org";
                mysqlConn.varConfigDatabase = "gbupot_cuesys";
                mysqlConn.varConfigUser = "gbupot_cueadmin";
                mysqlConn.varConfigPass = "Gas9ymzd!SZa";
                return true;
            }
            catch { return false; }
        }



        public void resetControls(string newFocus)
        {//Hide all controls and only show those needed
            devLogs("resetControls triggered");
            foreach (Control control in this.Controls) { control.Visible = false; }//Hide all controls
            lbCueSys.Visible = true;//Show logo
            panClock.Visible = true;//Show clock panel
            mainMenu.Visible = true;//Show menu
            foreach (var clockLbl in panClock.Controls.OfType<Label>()){ clockLbl.Visible = true; };//Show clock in panel
            switch (newFocus)//Use control statement to selectively show controls based on newFocus argument
            {
                case "Program started":
                    lbUserName.Visible = lbUserPass.Visible = tbUserName.Visible = tbUserPass.Visible = btLogin.Visible = true;//make login controls visible
                    mainMenu.Visible = false; //makes menu not visible in login page
                    devLogs("Login controls visible");
                    break;
                case "landing":
                    dgRoomBookingsSummary.Visible = true;
                    dbReturn("SELECT  bookingID as 'Booking ID', custContact as Contact, bookingBuilding as Building, bookingFloor as Floor,  bookingroom as Room,  bookingDateTime as 'Date/Time',  bookingCost as Cost,  bookingPaid as Paid FROM `tblBookings` WHERE `bookingDateTime` >= CURDATE()");
                    break;
                case "Book Room":
                    panFloorLayout.Visible = true;
                    cbBuilding.Visible = true;
                    cbFloor.Visible = true;
                    foreach (var x in panFloorLayout.Controls.OfType<Button>())
                    {//Make each button transparent
                        x.Parent = panFloorLayout;
                        x.Visible = true;
                        x.BackColor = Color.Transparent;
                        x.FlatAppearance.MouseDownBackColor = Color.Transparent;
                        x.FlatAppearance.MouseOverBackColor = Color.Transparent;
                        x.FlatStyle = FlatStyle.Flat;
                        x.ForeColor = BackColor;
                        x.UseVisualStyleBackColor = true;
                        x.FlatAppearance.BorderSize = 0;
                    };
                    break;
                case "create customer":
                    lbCustAdd1.Visible = true;
                    lbCustAdd2.Visible = true;
                    lbCustContact.Visible = true;
                    lbCustEmail.Visible = true;
                    lbCustPostcode.Visible = true;
                    lbCustTel.Visible = true;
                    lbCustTitle.Visible = true;
                    lbCustTownCity.Visible = true;
                    lbCustNationlity.Visible = true; //Added Customer Nationality Label, maded it visible
                    cbNationality.Visible = true;//Nationality ComboBox added and made visible
                    cbNationality.SelectedIndex = 0;
                    tbCustAdd1.Visible = true;
                    tbCustAdd2.Visible = true;
                    tbCustContact.Visible = true;
                    tbCustContact.Select();
                    tbCustEmail.Visible = true;
                    tbCustPostcode.Visible = true;
                    tbCustTel.Visible = true;
                    tbCustTownCity.Visible = true;
                    btCustSave.Visible = true;
                    lbCustTitle.Text = "Create Customer";
                    break;
                case "view customers":
                    //show all customers
                    dgRoomBookingsSummary.Visible = true;
                    dbReturn("SELECT custID as 'Customer ID', custContact as Contact, custEmail as 'Email Address', custTel as Telephone, custAddr1 as Address1, custAddr2 as Address2, custTownCity as 'Town/City', custPostcode as Postcode, custNationality as Nationality FROM `tblCustomer`");
                    break;
                case "Exit":
                    Application.Exit();
                    break;
                default:
                    devLogs("resetControls default case triggered, no controls visible");
                    break;
            }
            devLogs("Focus changed to " + newFocus);
        }
        public void dbReturn(string returnWhat)
        {
            devLogs(returnWhat + " sql run");
            if (mysqlConn.connOpen() == true)
            {
                dgRoomBookingsSummary.DataSource = mysqlConn.qry(returnWhat).Tables[0];
            }
        }
        public void createBooking(string room)
        {
            resetControls("");
            switch (cbFloor.Text)
            {
                case "Ground":
                    varFloor = "G";
                    break;
                case "First":
                    varFloor = "1";
                    break;
                case "Second":
                    varFloor = "2";
                    break;
                case "Third":
                    varFloor = "3";
                    break;
                case "Fourth":
                    varFloor = "4";
                    break;
                default:
                    break;
            }
            lbBookingInfo.Visible = true;
            label1.Visible = true;
            label2.Visible = true;
            label3.Visible = true;
            label4.Visible = true;
            label5.Visible = true;
            tbCustomer.Visible = true;
            mcDate.Visible = true;
            tbTime.Visible = true;
            tbCost.Visible = true;
            cbPaid.Visible = true;
            btBook.Visible = true;
            mcDate.MaxSelectionCount = 1;
            lbBookingInfo.Text = cbBuilding.Text + " - " + varFloor + room;
            varRoom = room;
        }
        ///// METHODS END //////////////////////////////////////////////////////////


        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        public Form1()
        {
            InitializeComponent();
            
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            File.WriteAllText("DevLog.txt", String.Empty);//Clear contents of DevLog
            lbCueSys.Font = new Font("Comic Sans MS", 40, FontStyle.Bold);
            this.ActiveControl = tbUserName;
            dbConfig();
            mysqlConn.connect();
            resetControls("Program started");
            devLogs("Program started");
        }

        ///// EVENTS START /////////////////////////////////////////////////////////
        private void timeClock_Tick(object sender, EventArgs e)
        {//Timer to control clock
            lbClockTime.Text = DateTime.Now.ToString("HH:mm");
            lbClockSeconds.Text = DateTime.Now.ToString("ss");
            lbClockDate.Text = DateTime.Now.ToString("ddd")+"  "+DateTime.Now.ToString("dd/MM/yyyy");
        }


        private void btLogin_Click(object sender, EventArgs e)
        {
            devLogs("Login button clicked");
            //User+Pass check, not secure and only allows one login
            if (tbUserName.Text == "admin" && tbUserPass.Text == "admin")
            { resetControls("landing"); devLogs("Login success for user " + tbUserName.Text); }//Login success
            else
            {
                //MessageBox.Show("Sorry, wrong password/user combo!");
                label6text.Visible = true;
                label6text.Text = "Incorrect login details";
                label6text.ForeColor = Color.Red;
                devLogs("Login failure for user " + tbUserName.Text); }//Login failure
            tbUserName.Text = ""; tbUserPass.Text = ""; //Clear logon credentials
        }
        private void tbUserName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {//Change focus to password box on enter key
                this.ActiveControl = tbUserPass;
                devLogs("enter key detected in tbUserName");
            }
        }
        private void tbUserPass_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {//Trigger login button on enter key
                btLogin_Click(this, new EventArgs());
                devLogs("enter key detected in tbUserPass");
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {//Generic keyboard shortcuts
            if (keyData == (Keys.Alt | Keys.L))
            {
                devLogs("alt-l shortcut intercepted");
                resetControls("Program started");
                return true;
            }
            if (keyData == (Keys.Alt | Keys.X))
            {
                devLogs("alt-x shortcut intercepted");
                resetControls("Exit");
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void viewDevLogsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form devForm = new Form();
            devForm.Text = "DevLogs";
            RichTextBox rtbDevLogs = new RichTextBox();
            Timer timerRefreshDevLogs = new Timer();
            timerRefreshDevLogs.Interval = 2500;
            timerRefreshDevLogs.Tick += new EventHandler(devRefreshTimer_Tick);
            timerRefreshDevLogs.Start();
            rtbDevLogs.Location = new Point(0, 0);
            rtbDevLogs.Size = new Size(300, 380);
            rtbDevLogs.Anchor = (AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right);
            devForm.Size = new Size(300, 400);
            devForm.Controls.Add(rtbDevLogs);
            devLogs("devlogs viewed");
            void devRefreshTimer_Tick(object timer, EventArgs args)
            {
                rtbDevLogs.Text = "";
                string line;
                try
                {
                    StreamReader sr = new StreamReader("DevLog.txt");
                    line = sr.ReadLine();
                    while (line != null)
                    {
                        rtbDevLogs.Text += line + "\r\n";
                        line = sr.ReadLine();
                    }
                    sr.Close();
                }
                catch (Exception ex) { devLogs("error reading devlogs"); }
            }
            devForm.Show();
        }

        private void logoutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            resetControls("Program started");devLogs("user logged out");
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            resetControls("Exit"); devLogs("application exit request");
        }

        private void bookRoomToolStripMenuItem_Click(object sender, EventArgs e)
        {
            resetControls("Book Room");devLogs("book room request");
        }

        private void btRoomA_Click(object sender, EventArgs e)
        {
            createBooking("1");
            devLogs("room a clicked");
        }

        private void btRoomB_Click(object sender, EventArgs e)
        {
            createBooking("2");
            devLogs("room b clicked");
        }

        private void btRoomC_Click(object sender, EventArgs e)
        {
            createBooking("3");
            devLogs("room c clicked");
        }

        private void btRoomD_Click(object sender, EventArgs e)
        {
            createBooking("4");
            devLogs("room d clicked");
        }

        private void btRoomE_Click(object sender, EventArgs e)
        {
            createBooking("5");
            devLogs("room e clicked");
        }

        private void btRoomF_Click(object sender, EventArgs e)
        {
            createBooking("6");
            devLogs("room f clicked");
        }

        private void btRoomG_Click(object sender, EventArgs e)
        {
            createBooking("7");
            devLogs("room g clicked");
        }

        private void btRoomH_Click(object sender, EventArgs e)
        {
            createBooking("8");
            devLogs("room h clicked");
        }

        private void createCustomerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            resetControls("create customer"); devLogs("create customer request");
        }
        
        private void viewBookingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            resetControls("landing"); devLogs("show bookings");
        }

        private void viewCustomersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            resetControls("view customers"); devLogs("show customers");
        }

        bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
        private void btCustSave_Click(object sender, EventArgs e)
        {
             
            var emptyFieldCount = 0;
            foreach (var control in this.Controls.OfType<TextBox>())
            {
                if (control.Visible) { 
                if (control.Text == string.Empty)
                {

                    control.BackColor = Color.Red;
                    emptyFieldCount++;
                }

                else
                {
                    
                    control.BackColor = Color.White;
                }

                
            }
            }
            if (emptyFieldCount == 0 && cbNationality.SelectedIndex > 0)
            {
               
                devLogs("insert new customer");
                if (mysqlConn.connOpen() == true)
                {
                    mysqlConn.insertCustomer(tbCustContact.Text, tbCustEmail.Text, tbCustTel.Text, tbCustAdd1.Text, tbCustAdd2.Text, tbCustTownCity.Text, tbCustPostcode.Text, cbNationality.SelectedItem.ToString());// tbCustNationality is added
                }
                tbCustContact.Text = tbCustEmail.Text = tbCustTel.Text = tbCustAdd1.Text = tbCustAdd2.Text = tbCustTownCity.Text = tbCustPostcode.Text = "";// "" clears existing data from the fields after saving
                cbNationality.SelectedIndex = 0;//returns to default after save
                resetControls("view customers");

            }

                
            

        }

        private void dgRoomBookingsSummary_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (dgRoomBookingsSummary.Columns[0].Name == "Booking ID") {
                DialogResult dialogResult = MessageBox.Show("Are you sure you want to delete this booking?", "Delete booking", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    mysqlConn.deleteBooking(Convert.ToString(dgRoomBookingsSummary.CurrentRow.Cells[0].Value));
                }
                resetControls("landing");
            }
            if (dgRoomBookingsSummary.Columns[0].Name == "Customer ID")
            {
                DialogResult dialogResult = MessageBox.Show("Are you sure you want to delete this customer?", "Delete customer", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    mysqlConn.deleteCustomer(Convert.ToString(dgRoomBookingsSummary.CurrentRow.Cells[0].Value));
                }
                resetControls("view customers");
            }
        }

        private void btBook_Click(object sender, EventArgs e)
        {
            string varDateTime = mcDate.SelectionRange.Start.ToString("yyyy-MM-dd") + " " + tbTime.Text + ":00"; ;
            string varPaid;
            if (cbPaid.Checked == true) { varPaid = "Y"; }
            else { varPaid = "N"; }
            if (mysqlConn.connOpen() == true)
            {
                mysqlConn.insertBooking(tbCustomer.Text, cbBuilding.Text, varFloor, varRoom, varDateTime, tbCost.Text, varPaid);
            }
            resetControls("landing");
        }

        private void mcDate_DateChanged(object sender, DateRangeEventArgs e)
        {

        }

        private void tbCustPostcode_TextChanged(object sender, EventArgs e)
        {
            if (tbCustPostcode.BackColor == Color.Red) { 
            tbCustPostcode.BackColor = Color.White;

            }
        }

        private void tbCustEmail_TextChanged(object sender, EventArgs e)
        {
            if (tbCustEmail.BackColor == Color.Red)
            {
                tbCustEmail.BackColor = Color.White;

            }
        }

        private void tbCustAdd1_TextChanged(object sender, EventArgs e)
        {
            if (tbCustAdd1.BackColor == Color.Red)
            {
                tbCustAdd1.BackColor = Color.White;

            }
        }

        private void tbCustAdd2_TextChanged(object sender, EventArgs e)
        {
            if (tbCustAdd2.BackColor == Color.Red)
            {
                tbCustAdd2.BackColor = Color.White;

            }
        }

        private void tbCustTel_TextChanged(object sender, EventArgs e)
        {
            if (tbCustTel.BackColor == Color.Red)
            {
                tbCustTel.BackColor = Color.White;

            }
        }

        private void tbCustTownCity_TextChanged(object sender, EventArgs e)
        {
            if (tbCustTownCity.BackColor == Color.Red)
            {
                tbCustTownCity.BackColor = Color.White;

            }
        }

        private void tbCustNationality_TextChanged(object sender, EventArgs e)
        {
          
        }

        private void tbCustContact_TextChanged(object sender, EventArgs e)
        {
            if (tbCustContact.BackColor == Color.Red)
            {
                tbCustContact.BackColor = Color.White;

            }
        }

        private void tbCustEmail_Leave(object sender, EventArgs e)
        {
            if(tbCustEmail.Text == "")
            {
                lbEmailErrorMsg.Visible = true;
                lbEmailErrorMsg.ForeColor = Color.Red;
                lbEmailErrorMsg.Text = "You must enter a valid email address.";

            }
            else if (!IsValidEmail(tbCustEmail.Text))
            {

                lbEmailErrorMsg.Visible = true;
                lbEmailErrorMsg.ForeColor = Color.Red;
                lbEmailErrorMsg.Text = "Invalid Email Address.";


            }

            else
            {

                lbEmailErrorMsg.Visible = false;
            }
        }
        ///// EVENTS END ///////////////////////////////////////////////////////////
    }
}
