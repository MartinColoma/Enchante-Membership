using iTextSharp.text.pdf.draw;
using iTextSharp.text.pdf;
using iTextSharp.text;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Transactions;
using System.Windows.Forms;

namespace Enchante_Membership
{
    
    public partial class EnchanteMembership : Form
    {
        //local db connection
        public static string mysqlconn = "server=localhost;user=root;database=enchante;password=";
        public MySqlConnection connection = new MySqlConnection(mysqlconn); 
        
        
        private ParentCard ParentPanelShow;
        private Registration Registration; //Membership Type Card
        private ServiceCard Service; //Service Card
        private MemberAccountParentCard Member;

        //tool tip
        private System.Windows.Forms.ToolTip iconToolTip;

        string membercategory;

        //gender combo box
        private string[] genders = { "Male", "Female", "Prefer Not to Say" };

        private Timer timer;
        private int currentIndex = 0;
        private System.Drawing.Image[] images = { Properties.Resources.Enchante_Bldg,  Properties.Resources.Hair, 
                                    Properties.Resources.Olga_Collection, Properties.Resources.download,
                                    Properties.Resources.Green___Gold_Collection___Salon_Equipment_Centre}; // Replace with your resource names

        public string filterstaffbyservicecategory;
        public bool haschosenacategory = false;
        public bool servicecategorychanged;
        public string selectedStaffID;
        //private bool IsPrefferredTimeSchedComboBoxModified = false;
        public string membertype;

        public EnchanteMembership()
        {
            InitializeComponent();

            //icon tool tip
            iconToolTip = new System.Windows.Forms.ToolTip();
            iconToolTip.IsBalloon = true;

            // Exit MessageBox 
            this.FormClosing += new FormClosingEventHandler(MainForm_FormClosing);

            //Cardlayout Panel Manager
            ParentPanelShow = new ParentCard(EnchanteHomePage, EnchanteMemberPage);
            Registration = new Registration(MembershipPlanPanel, RegularPlanPanel, PremiumPlanPanel, SVIPPlanPanel);
            Service = new ServiceCard(ServiceType, ServiceHairStyling, ServiceFaceSkin, ServiceNailCare, ServiceSpa, ServiceMassage);
            Member = new MemberAccountParentCard(MemAccInfoPanel, MemAccApptPanel, MemAccHomePanel, MemAccReviewPanel, MemAccBillingPanel);

            //gender combobox
            RegularGenderComboText.Items.AddRange(genders);
            RegularGenderComboText.DropDownStyle = ComboBoxStyle.DropDownList;
            SVIPGenderComboText.Items.AddRange(genders);
            SVIPGenderComboText.DropDownStyle = ComboBoxStyle.DropDownList;
            PremGenderComboText.Items.AddRange(genders);
            PremGenderComboText.DropDownStyle = ComboBoxStyle.DropDownList;

            // Initialize the timer
            ScrollTimer.Start();
            PictureSlideTimer.Start();

        }

        private void EnchanteMembership_Load(object sender, EventArgs e)
        {
            HomePanelReset();

        }
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {

            if (e.CloseReason == CloseReason.UserClosing)
            {
                // Prevent the form from closing.
                e.Cancel = true;

                DialogResult result = MessageBox.Show("Do you want to close the application?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    this.Dispose();

                }


            }
        }
        private void ScrollTimer_Tick(object sender, EventArgs e)
        {
            HomeUpdateButtonColors();
        }
        #region ID Generator Methods
        public class RegularClientIDGenerator
        {
            private static Random random = new Random();

            public static string GenerateClientID()
            {
                // Get the current year and extract the last digit
                int currentYear = DateTime.Now.Year;
                int lastDigitOfYear = currentYear % 100;

                // Generate a random 6-digit number
                string randomPart = GenerateRandomNumber();

                // Format the ClientID
                string clientID = $"R-{lastDigitOfYear:D2}-{randomPart:D6}";

                return clientID;
            }

            private static string GenerateRandomNumber()
            {
                // Generate a random 6-digit number
                int randomNumber = random.Next(100000, 999999);

                return randomNumber.ToString();
            }
        }
        private void RegularAccIDGenerator()
        {
            RegularMemberIDText.Text = "";

            // Call the GenerateClientID method using the type name
            string generatedClientID = RegularClientIDGenerator.GenerateClientID();

            RegularMemberIDText.Text = generatedClientID;
        }

        public class SVIPClientIDGenerator
        {
            private static Random random = new Random();

            public static string GenerateClientID()
            {
                // Get the current year and extract the last digit
                int currentYear = DateTime.Now.Year;
                int lastDigitOfYear = currentYear % 100;

                // Generate a random 6-digit number
                string randomPart = GenerateRandomNumber();

                // Format the ClientID
                string clientID = $"SVIP-{lastDigitOfYear:D2}-{randomPart:D6}";

                return clientID;
            }

            private static string GenerateRandomNumber()
            {
                // Generate a random 6-digit number
                int randomNumber = random.Next(100000, 999999);

                return randomNumber.ToString();
            }
        }
        private void SVIPAccIDGenerator()
        {
            SVIPMemberIDText.Text = "";

            // Call the GenerateClientID method using the type name
            string generatedClientID = SVIPClientIDGenerator.GenerateClientID();

            SVIPMemberIDText.Text = generatedClientID;
        }
        public class PremClientIDGenerator
        {
            private static Random random = new Random();

            public static string GenerateClientID()
            {
                // Get the current year and extract the last digit
                int currentYear = DateTime.Now.Year;
                int lastDigitOfYear = currentYear % 100;

                // Generate a random 6-digit number
                string randomPart = GenerateRandomNumber();

                // Format the ClientID
                string clientID = $"PREM-{lastDigitOfYear:D2}-{randomPart:D6}";

                return clientID;
            }

            private static string GenerateRandomNumber()
            {
                // Generate a random 6-digit number
                int randomNumber = random.Next(100000, 999999);

                return randomNumber.ToString();
            }
        }
        private void PremAccIDGenerator()
        {
            PremMemberIDText.Text = "";

            // Call the GenerateClientID method using the type name
            string generatedClientID = PremClientIDGenerator.GenerateClientID();

            PremMemberIDText.Text = generatedClientID;
        }
        #endregion

        #region password hashers
        public class HashHelper
        {
            public static string HashString(string input)
            {
                using (SHA256 sha256 = SHA256.Create())
                {
                    byte[] inputBytes = Encoding.UTF8.GetBytes(input);
                    byte[] hashBytes = sha256.ComputeHash(inputBytes);
                    string hashedString = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
                    return hashedString;
                }
            }
        }
        public class HashHelper_Salt
        {
            public static string HashString_Salt(string input_Salt)
            {
                using (SHA256 sha256 = SHA256.Create())
                {
                    byte[] inputBytes_Salt = Encoding.UTF8.GetBytes(input_Salt);
                    byte[] hashBytes_Salt = sha256.ComputeHash(inputBytes_Salt);
                    string hashedString_Salt = BitConverter.ToString(hashBytes_Salt).Replace("-", "").ToLower();
                    return hashedString_Salt;
                }
            }
        }
        public class HashHelper_SaltperUser
        {
            public static string HashString_SaltperUser(string input_SaltperUser)
            {
                using (SHA256 sha256 = SHA256.Create())
                {
                    byte[] inputBytes_SaltperUser = Encoding.UTF8.GetBytes(input_SaltperUser);
                    byte[] hashBytes_SaltperUser = sha256.ComputeHash(inputBytes_SaltperUser);
                    string hashedString_SaltperUser = BitConverter.ToString(hashBytes_SaltperUser).Replace("-", "").ToLower();
                    return hashedString_SaltperUser;
                }
            }
        }
        #endregion
        #region Enchante Home Landing Page Starts Here
        private void EnchanteHomeScrollPanel_Click(object sender, EventArgs e)
        {
            //Reset Panel to Show Default
            HomePanelReset();
        }
        private void EnchanteHomeScrollPanel_Scroll(object sender, ScrollEventArgs e)
        {
            HomeUpdateButtonColors();
        }
        private void HomeUpdateButtonColors()
        {
            int scrollPosition = EnchanteHomeScrollPanel.VerticalScroll.Value;

            int homeThreshold = 887;
            int serviceThreshold = 1774;
            int membershipThreshold = 2661;
            int teamThreshold = 3548;
            int aboutThreshold = 4435;

            // Home Button
            EnchanteHomeBtn.ForeColor = scrollPosition < homeThreshold ?
                System.Drawing.Color.FromArgb(177, 183, 97) :
                System.Drawing.Color.FromArgb(229, 229, 221);

            // Service Button
            EnchanteServiceBtn.ForeColor = scrollPosition >= homeThreshold && scrollPosition < serviceThreshold ?
                System.Drawing.Color.FromArgb(177, 183, 97) :
                System.Drawing.Color.FromArgb(229, 229, 221);

            // Membership Button
            EnchanteMemberBtn.ForeColor = scrollPosition >= serviceThreshold && scrollPosition < membershipThreshold ?
                System.Drawing.Color.FromArgb(177, 183, 97) :
                System.Drawing.Color.FromArgb(229, 229, 221);

            // Team Button
            EnchanteTeamBtn.ForeColor = scrollPosition >= membershipThreshold && scrollPosition < teamThreshold ?
                System.Drawing.Color.FromArgb(177, 183, 97) :
                System.Drawing.Color.FromArgb(229, 229, 221);

            // About Button
            EnchanteAbtUsBtn.ForeColor = scrollPosition >= teamThreshold && scrollPosition < aboutThreshold ?
                System.Drawing.Color.FromArgb(177, 183, 97) :
                System.Drawing.Color.FromArgb(229, 229, 221);


        }
        private void HomePanelReset()
        {
            ParentPanelShow.PanelShow(EnchanteHomePage);
            Service.PanelShow(ServiceType);
            Registration.PanelShow(MembershipPlanPanel);

        }
       
        private void MemberHomePanelReset()
        {
            ParentPanelShow.PanelShow(EnchanteMemberPage);
            Member.PanelShow(MemAccHomePanel);

        }



        private void ScrollToCoordinates(int x, int y)
        {
            // Set the AutoScrollPosition to the desired coordinates
            EnchanteHomeScrollPanel.AutoScrollPosition = new Point(x, y);
        }

        private void EnchanteAppointBtn_Click(object sender, EventArgs e)
        {
            MemberLocationAndColor();
        }

        private void EnchanteHLoginBtn_Click(object sender, EventArgs e)
        {


            if (EnchanteLoginForm.Visible == false)
            {
                HomeLocationAndColor();
                EnchanteLoginForm.Visible = true;
                return;
            }
            else
            {
                HomeLocationAndColor();
                EnchanteLoginForm.Visible = false;
            }

        }

        private void EnchanteHomeBtn_Click(object sender, EventArgs e)
        {
            //Reset Panel to Show Default
            HomePanelReset();
            HomeLocationAndColor();
        }
        private void EnchanteHeaderLogo_Click(object sender, EventArgs e)
        {
            //Reset Panel to Show Default
            HomePanelReset();
            HomeLocationAndColor();
        }
        private void HomeLocationAndColor()
        {
            // Scroll to the Home position (0, 0)
            ScrollToCoordinates(0, 0);
            //Change color once clicked
            EnchanteHomeBtn.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(177)))), ((int)(((byte)(183)))), ((int)(((byte)(97)))));

            //Change back to original
            EnchanteServiceBtn.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(229)))), ((int)(((byte)(229)))), ((int)(((byte)(221)))));
            EnchanteMemberBtn.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(229)))), ((int)(((byte)(229)))), ((int)(((byte)(221)))));
            EnchanteTeamBtn.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(229)))), ((int)(((byte)(229)))), ((int)(((byte)(221)))));
            EnchanteAbtUsBtn.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(229)))), ((int)(((byte)(229)))), ((int)(((byte)(221)))));

            PictureSlideTimer.Stop();

        }

        private void EnchanteServiceBtn_Click(object sender, EventArgs e)
        {
            ServiceLocationAndColor();
        }

        private void ServiceLocationAndColor()
        {
            //Reset Panel to Show Default
            Service.PanelShow(ServiceType);

            int serviceSectionY = 1000;
            ScrollToCoordinates(0, serviceSectionY);
            //Change color once clicked
            EnchanteServiceBtn.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(177)))), ((int)(((byte)(183)))), ((int)(((byte)(97)))));
            //Change back to original
            EnchanteHomeBtn.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(229)))), ((int)(((byte)(229)))), ((int)(((byte)(221)))));
            EnchanteMemberBtn.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(229)))), ((int)(((byte)(229)))), ((int)(((byte)(221)))));
            EnchanteTeamBtn.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(229)))), ((int)(((byte)(229)))), ((int)(((byte)(221)))));
            EnchanteAbtUsBtn.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(229)))), ((int)(((byte)(229)))), ((int)(((byte)(221)))));

            PictureSlideTimer.Stop();

        }

        private void EnchanteMemberBtn_Click(object sender, EventArgs e)
        {
            MemberLocationAndColor();
        }

        private void MemberLocationAndColor()
        {
            //Reset Panel to Show Default
            Registration.PanelShow(MembershipPlanPanel);

            //location scroll
            int serviceSectionY = 1800;
            ScrollToCoordinates(0, serviceSectionY);

            //Change color once clicked
            EnchanteMemberBtn.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(177)))), ((int)(((byte)(183)))), ((int)(((byte)(97)))));
            //Change back to original
            EnchanteHomeBtn.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(229)))), ((int)(((byte)(229)))), ((int)(((byte)(221)))));
            EnchanteServiceBtn.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(229)))), ((int)(((byte)(229)))), ((int)(((byte)(221)))));
            EnchanteTeamBtn.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(229)))), ((int)(((byte)(229)))), ((int)(((byte)(221)))));
            EnchanteAbtUsBtn.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(229)))), ((int)(((byte)(229)))), ((int)(((byte)(221)))));
            PictureSlideTimer.Stop();
        }
        private void EnchanteTeamBtn_Click(object sender, EventArgs e)
        {
            TeamLocationAndColor();
        }

        private void TeamLocationAndColor()
        {
            ////location scroll
            int serviceSectionY = 2800;
            ScrollToCoordinates(0, serviceSectionY);

            //Change color once clicked
            EnchanteTeamBtn.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(177)))), ((int)(((byte)(183)))), ((int)(((byte)(97)))));
            //Change back to original
            EnchanteHomeBtn.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(229)))), ((int)(((byte)(229)))), ((int)(((byte)(221)))));
            EnchanteServiceBtn.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(229)))), ((int)(((byte)(229)))), ((int)(((byte)(221)))));
            EnchanteMemberBtn.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(229)))), ((int)(((byte)(229)))), ((int)(((byte)(221)))));
            EnchanteAbtUsBtn.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(229)))), ((int)(((byte)(229)))), ((int)(((byte)(221)))));
            PictureSlideTimer.Stop();
        }
        private void EnchanteAbtUsBtn_Click(object sender, EventArgs e)
        {
            AboutUsLocatonAndColor();

        }

        private void AboutUsLocatonAndColor()
        {
            //location scroll
            int serviceSectionY = 3800;
            ScrollToCoordinates(0, serviceSectionY);

            //Change color once clicked
            EnchanteAbtUsBtn.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(177)))), ((int)(((byte)(183)))), ((int)(((byte)(97)))));
            //Change back to original
            EnchanteHomeBtn.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(229)))), ((int)(((byte)(229)))), ((int)(((byte)(221)))));
            EnchanteServiceBtn.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(229)))), ((int)(((byte)(229)))), ((int)(((byte)(221)))));
            EnchanteMemberBtn.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(229)))), ((int)(((byte)(229)))), ((int)(((byte)(221)))));
            EnchanteTeamBtn.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(229)))), ((int)(((byte)(229)))), ((int)(((byte)(221)))));
            PictureSlideTimer.Stop();
        }
        private void EnchanteHomeBtn_MouseHover(object sender, EventArgs e)
        {
            iconToolTip.SetToolTip(EnchanteHomeBtn, "Home");
        }

        private void EnchanteServiceBtn_MouseHover(object sender, EventArgs e)
        {
            iconToolTip.SetToolTip(EnchanteServiceBtn, "Service");
        }


        private void EnchanteMemberBtn_MouseHover(object sender, EventArgs e)
        {
            iconToolTip.SetToolTip(EnchanteMemberBtn, "Membership");
        }

        private void EnchanteTeamBtn_MouseHover(object sender, EventArgs e)
        {
            iconToolTip.SetToolTip(EnchanteTeamBtn, "Our Team");
        }
        private void EnchanteAbtUsBtn_MouseHover(object sender, EventArgs e)
        {
            iconToolTip.SetToolTip(EnchanteAbtUsBtn, "About Us");
        }
        private void EnchanteHLoginBtn_MouseHover(object sender, EventArgs e)
        {
            iconToolTip.SetToolTip(EnchanteHLoginBtn, "Login");
        }

        //services part

        private void ServiceHSBtn_Click(object sender, EventArgs e)
        {
            Service.PanelShow(ServiceHairStyling);

        }

        private void ServiceFSBtn_Click(object sender, EventArgs e)
        {
            Service.PanelShow(ServiceFaceSkin);

        }

        private void ServiceNCBtn_Click(object sender, EventArgs e)
        {
            Service.PanelShow(ServiceNailCare);

        }

        private void ServiceSpaBtn_Click(object sender, EventArgs e)
        {
            Service.PanelShow(ServiceSpa);

        }

        private void ServiceMBtn_Click(object sender, EventArgs e)
        {
            Service.PanelShow(ServiceMassage);

        }



        private void ShowHidePassBtn_Click(object sender, EventArgs e)
        {
            if (LoginPassText.UseSystemPasswordChar == true)
            {
                LoginPassText.UseSystemPasswordChar = false;
                ShowHidePassBtn.IconChar = FontAwesome.Sharp.IconChar.EyeSlash;
            }
            else if (LoginPassText.UseSystemPasswordChar == false)
            {
                LoginPassText.UseSystemPasswordChar = true;
                ShowHidePassBtn.IconChar = FontAwesome.Sharp.IconChar.Eye;





            }
        }
        private void ShowHidePassBtn_MouseHover(object sender, EventArgs e)
        {

        }
        private void LoginPassReqBtn_MouseHover(object sender, EventArgs e)
        {
            string message = "Must be at least 8 character long.\n";
            message += "First character must be capital.\n";
            message += "Must include a special character and a number.";

            iconToolTip.SetToolTip(LoginPassReqBtn, message);
        }
        private void LoginBtn_Click(object sender, EventArgs e)
        {
            loginchecker();
        }
        private void LoginPassText_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                loginchecker();

                e.SuppressKeyPress = true;
                return;
            }
            else if (e.KeyCode == Keys.Up)
            {
                LoginEmailAddText.Focus();
            }

        }
        private void LoginEmailAddText_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Down)
            {
                LoginPassText.Focus();
            }

        }
        private void loginchecker()
        {
            if (LoginEmailAddText.Text == "Member" && LoginPassText.Text == "Member123")
            {
                //Test Member
                LoginEmailAddErrorLbl.Visible = false;
                LoginPassErrorLbl.Visible = false;
                MessageBox.Show("Welcome back, Member.", "Login Verified", MessageBoxButtons.OK, MessageBoxIcon.Information);
                MemberHomePanelReset();
                MemberNameLbl.Text = "Member Tester";
                MemberIDNumLbl.Text = "MT-0000-0000";
                logincredclear();

                return;
            }
            else if (LoginEmailAddText.Text != "Member" && LoginPassText.Text == "Member123")
            {
                //Test Member
                LoginEmailAddErrorLbl.Visible = true;
                LoginPassErrorLbl.Visible = false;
                LoginEmailAddErrorLbl.Text = "Email Address Does Not\nMatch Any Existing Email";

                return;
            }
            else if (LoginEmailAddText.Text == "Member" && LoginPassText.Text != "Member123")
            {
                //Test Member
                LoginEmailAddErrorLbl.Visible = false;
                LoginPassErrorLbl.Visible = true;
                LoginPassErrorLbl.Text = "INCORRECT PASSWORD";
                return;
            }

            else if (string.IsNullOrEmpty(LoginEmailAddText.Text) && string.IsNullOrEmpty(LoginPassText.Text))
            {
                //MessageBox.Show("Missing text on required fields.", "Ooooops!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                LoginEmailAddErrorLbl.Visible = true;
                LoginPassErrorLbl.Visible = true;
                LoginEmailAddErrorLbl.Text = "Missing Field";
                LoginPassErrorLbl.Text = "Missing Field";
                return;
            }
            else if (string.IsNullOrEmpty(LoginEmailAddText.Text))
            {
                LoginEmailAddErrorLbl.Visible = true;
                LoginPassErrorLbl.Visible = false;

                LoginEmailAddErrorLbl.Text = "Missing Field";
                return;
            }
            else if (string.IsNullOrEmpty(LoginPassText.Text))
            {
                LoginEmailAddErrorLbl.Visible = false;
                LoginPassErrorLbl.Visible = true;
                LoginPassErrorLbl.Text = "Missing Field";
                return;
            }
            else
            {
                //db connection query
                string email = LoginEmailAddText.Text;
                string password = LoginPassText.Text;
                string passchecker = HashHelper.HashString(password); 
                string membertype;

                try //user member login
                {
                    connection.Open();

                    string queryCheckEmail = "SELECT COUNT(*) FROM membershipaccount WHERE EmailAdd = @email";

                    using (MySqlCommand cmdCheckEmail = new MySqlCommand(queryCheckEmail, connection))
                    {
                        cmdCheckEmail.Parameters.AddWithValue("@email", email);

                        int emailCount = Convert.ToInt32(cmdCheckEmail.ExecuteScalar());

                        if (emailCount == 0)
                        {
                            // Email does not exist in the database
                            LoginEmailAddErrorLbl.Visible = true;
                            LoginPassErrorLbl.Visible = false;
                            LoginEmailAddErrorLbl.Text = "Email Address Does Not\nMatch Any Existing Email";
                            return;
                        }
                    }

                    string queryApproved = "SELECT FirstName, LastName, MemberIDNumber, MembershipType, HashedPass, AccountCreated, EmailAdd, CPNumber, Birthday, Age, MembershipType " +
                                            "FROM membershipaccount WHERE EmailAdd = @email";

                    using (MySqlCommand cmdApproved = new MySqlCommand(queryApproved, connection))
                    {
                        cmdApproved.Parameters.AddWithValue("@email", email);

                        using (MySqlDataReader readerApproved = cmdApproved.ExecuteReader())
                        {
                            if (readerApproved.Read())
                            {
                                //personal info
                                string name = readerApproved["FirstName"].ToString();
                                string lastname = readerApproved["LastName"].ToString();
                                string CPNum = readerApproved["CPNumber"].ToString();
                                string bday = readerApproved["Birthday"].ToString();
                                string age = readerApproved["Age"].ToString();
                                string type = readerApproved["MembershipType"].ToString();

                                //account settings
                                string memberEmail = readerApproved["EmailAdd"].ToString();
                                string ID = readerApproved["MemberIDNumber"].ToString();
                                string accountCreated = readerApproved["AccountCreated"].ToString();


                                membertype = readerApproved["MembershipType"].ToString();

                                if (membertype == "Regular")
                                {
                                    // Retrieve the HashedPass column
                                    string hashedPasswordFromDB = readerApproved["HashedPass"].ToString();

                                    // Check if the entered password matches
                                    bool passwordMatches = hashedPasswordFromDB.Equals(passchecker);

                                    if (passwordMatches)
                                    {
                                        MessageBox.Show($"Welcome back, Regular Client {name}.", "Account Verified", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                        MemberSubAccUserBtn.Visible = false;
                                        MemberNameLbl.Text = $"{name} {lastname}";
                                        MemberIDNumLbl.Text = ID;

                                        //personal info
                                        MemberAccInfoPersonalNameText.Text = $"{name} {lastname}";
                                        MemberAccInfoPersonalCPNumText.Text = CPNum;
                                        MemberAccInfoPersonalBdayText.Text = bday;
                                        MemberAccInfoPersonalAgeText.Text = age;

                                        //account settings
                                        MemberAccInfoPersonalEAddText.Text = memberEmail;
                                        MemberAccInfoPersonalPassText.Text = password;
                                        MemberAccInfoPersonalIDNumText.Text = ID;
                                        MemberAccInfoPersonalCreatedText.Text = accountCreated;
                                        MemberAccInfoPersonalTypeText.Text = type;

                                        MemberHomePanelReset();
                                        logincredclear();

                                    }
                                    else
                                    {
                                        LoginEmailAddErrorLbl.Visible = false;
                                        LoginPassErrorLbl.Visible = true;
                                        LoginPassErrorLbl.Text = "INCORRECT PASSWORD";
                                    }
                                    return;
                                }
                                else if (membertype == "PREMIUM")
                                {
                                    // Retrieve the HashedPass column
                                    string hashedPasswordFromDB = readerApproved["HashedPass"].ToString();

                                    // Check if the entered password matches
                                    bool passwordMatches = hashedPasswordFromDB.Equals(passchecker);

                                    if (passwordMatches)
                                    {
                                        MessageBox.Show($"Welcome back, Premium Client {name}.", "Account Verified", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                        MemberNameLbl.Text = name + " " + lastname;
                                        MemberIDNumLbl.Text = ID;


                                        //personal info
                                        MemberAccInfoPersonalNameText.Text = $"{name} {lastname}";
                                        MemberAccInfoPersonalCPNumText.Text = CPNum;
                                        MemberAccInfoPersonalBdayText.Text = bday;
                                        MemberAccInfoPersonalAgeText.Text = age;

                                        //account settings
                                        MemberAccInfoPersonalEAddText.Text = memberEmail;
                                        MemberAccInfoPersonalPassText.Text = password;
                                        MemberAccInfoPersonalIDNumText.Text = ID;
                                        MemberAccInfoPersonalCreatedText.Text = accountCreated;
                                        MemberAccInfoPersonalTypeText.Text = type;

                                        MemberHomePanelReset();
                                        logincredclear();

                                    }
                                    else
                                    {
                                        LoginEmailAddErrorLbl.Visible = false;
                                        LoginPassErrorLbl.Visible = true;
                                        LoginPassErrorLbl.Text = "INCORRECT PASSWORD";
                                    }
                                    return;
                                }
                                else if (membertype == "SVIP")
                                {
                                    // Retrieve the HashedPass column
                                    string hashedPasswordFromDB = readerApproved["HashedPass"].ToString();

                                    // Check if the entered password matches
                                    bool passwordMatches = hashedPasswordFromDB.Equals(passchecker);

                                    if (passwordMatches)
                                    {
                                        MessageBox.Show($"Welcome back, SVIP Client {name}.", "Account Verified", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                        MemberNameLbl.Text = name + " " + lastname;
                                        MemberIDNumLbl.Text = ID;


                                        //personal info
                                        MemberAccInfoPersonalNameText.Text = $"{name} {lastname}";
                                        MemberAccInfoPersonalCPNumText.Text = CPNum;
                                        MemberAccInfoPersonalBdayText.Text = bday;
                                        MemberAccInfoPersonalAgeText.Text = age;

                                        //account settings
                                        MemberAccInfoPersonalEAddText.Text = memberEmail;
                                        MemberAccInfoPersonalPassText.Text = password;
                                        MemberAccInfoPersonalIDNumText.Text = ID;
                                        MemberAccInfoPersonalCreatedText.Text = accountCreated;
                                        MemberAccInfoPersonalTypeText.Text = type;


                                        MemberHomePanelReset();
                                        logincredclear();

                                    }
                                    else
                                    {
                                        LoginEmailAddErrorLbl.Visible = false;
                                        LoginPassErrorLbl.Visible = true;
                                        LoginPassErrorLbl.Text = "INCORRECT PASSWORD";
                                    }
                                    return;
                                }
                            }

                        }


                    }

                }
                catch (Exception ex)
                {
                    string errorMessage = "An error occurred: " + ex.Message + "\n\n" + ex.StackTrace;

                    // Copy the error message to the clipboard
                    Clipboard.SetText(errorMessage);

                    // Show a message box indicating the error and informing the user that the error message has been copied to the clipboard
                    MessageBox.Show($"An error occurred. {errorMessage}.", "Login Verification Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    connection?.Close();
                }

            }
        }

        private void logincredclear()
        {
            LoginEmailAddText.Text = "";
            LoginPassText.Text = "";
            LoginEmailAddErrorLbl.Visible = false;
            LoginPassErrorLbl.Visible = false;
            LoginPassText.UseSystemPasswordChar = true;

        }

        private void LogoutChecker()
        {
            DialogResult result = MessageBox.Show("Do you want to logout user?", "Logout Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                EnchanteLoginForm.Visible = false;
                ParentPanelShow.PanelShow(EnchanteHomePage);


                MemberUserAccPanel.Visible = false;
                RecApptAnyStaffToggleSwitch.Location = new System.Drawing.Point(533, 885);
                RecApptAnyStaffLbl.Location = new System.Drawing.Point(590, 885);
            }
        }

        private void LoginRegisterHereLbl_Click(object sender, EventArgs e)
        {
            MemberLocationAndColor();

        }

        private void SM_FBBtn_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://www.facebook.com/enchantesalon2024");
        }

        private void SM_TwitterBtn_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://twitter.com/Enchante2024");
        }

        private void SM_IGBtn_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://www.instagram.com/enchantesalon2024/");
        }

        private void SM_GmailBtn_Click(object sender, EventArgs e)
        {
            string emailAddress = "enchantesalon2024@gmail.com";
            string subject = "Subject of your email";
            string body = "Body of your email";

            string mailtoLink = $"mailto:{emailAddress}?subject={Uri.EscapeDataString(subject)}&body={Uri.EscapeDataString(body)}";

            System.Diagnostics.Process.Start(mailtoLink);
        }

        private void SM_FBBtn_MouseHover(object sender, EventArgs e)
        {
            iconToolTip.SetToolTip(SM_FBBtn, "Facebook");

        }

        private void SM_TwitterBtn_MouseHover(object sender, EventArgs e)
        {
            iconToolTip.SetToolTip(SM_TwitterBtn, "Twitter");
        }

        private void SM_IGBtn_MouseHover(object sender, EventArgs e)
        {
            iconToolTip.SetToolTip(SM_IGBtn, "Instagram");

        }

        private void SM_GmailBtn_MouseHover(object sender, EventArgs e)
        {
            iconToolTip.SetToolTip(SM_GmailBtn, "Email Us Here");
        }

        private void RMemberCreateAccBtn_Click(object sender, EventArgs e)
        {
            Registration.PanelShow(RegularPlanPanel);
            RegularAccIDGenerator();
        }

        private void PMemberCreateAccBtn_Click(object sender, EventArgs e)
        {
            Registration.PanelShow(PremiumPlanPanel);
            PremAccIDGenerator();
            SetExpirationDate("monthly");
            PremMonthly();
        }

        private void SVIPMemberCreateAccBtn_Click(object sender, EventArgs e)
        {
            Registration.PanelShow(SVIPPlanPanel);
            SVIPAccIDGenerator();
            SetExpirationDate("monthly");
            SVIPMonthly();

        }


        //Regular Member Registration
        private void RegularExitBtn_Click(object sender, EventArgs e)
        {
            Registration.PanelShow(MembershipPlanPanel);

        }

        private void RegularBdayPicker_ValueChanged(object sender, EventArgs e)
        {
            DateTime selectedDate = RegularBdayPicker.Value;
            int age = DateTime.Now.Year - selectedDate.Year;

            if (DateTime.Now < selectedDate.AddYears(age))
            {
                age--; // Subtract 1 if the birthday hasn't occurred yet this year
            }

            RegularAgeText.Text = age.ToString();
            if (age < 18)
            {
                RegularAgeErrorLbl.Visible = true;
                RegularAgeErrorLbl.Text = "Must be 18 years old and above";
                return;
            }
            else
            {
                RegularAgeErrorLbl.Visible = false;

            }
        }

        private void RegularGenderComboText_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (RegularGenderComboText.SelectedItem != null)
            {
                RegularGenderComboText.Text = RegularGenderComboText.SelectedItem.ToString();
            }
        }

        private void RegularPassReqBtn_MouseHover(object sender, EventArgs e)
        {
            string message = "Must be at least 8 character long.\n";
            message += "First character must be capital.\n";
            message += "Must include a special character and a number.";

            iconToolTip.SetToolTip(RegularPassReqBtn, message);

        }
        private void RegularShowHidePassBtn_Click(object sender, EventArgs e)
        {
            if (RegularPassText.UseSystemPasswordChar == true)
            {
                RegularPassText.UseSystemPasswordChar = false;
                RegularShowHidePassBtn.IconChar = FontAwesome.Sharp.IconChar.EyeSlash;
            }
            else if (RegularPassText.UseSystemPasswordChar == false)
            {
                RegularPassText.UseSystemPasswordChar = true;
                RegularShowHidePassBtn.IconChar = FontAwesome.Sharp.IconChar.Eye;

            }
        }
        private void RegularConfirmPassText_TextChanged(object sender, EventArgs e)
        {
            if (RegularConfirmPassText.Text != RegularPassText.Text)
            {
                RegularConfirmPassErrorLbl.Visible = true;
                RegularConfirmPassErrorLbl.Text = "PASSWORD DOES NOT MATCH";
            }
            else
            {
                RegularConfirmPassErrorLbl.Visible = false;
            }
        }
        private void RegularConfirmShowHidePassBtn_Click(object sender, EventArgs e)
        {
            if (RegularConfirmPassText.UseSystemPasswordChar == true)
            {
                RegularConfirmPassText.UseSystemPasswordChar = false;
                RegularConfirmShowHidePassBtn.IconChar = FontAwesome.Sharp.IconChar.EyeSlash;
            }
            else if (RegularPassText.UseSystemPasswordChar == false)
            {
                RegularConfirmPassText.UseSystemPasswordChar = true;
                RegularConfirmShowHidePassBtn.IconChar = FontAwesome.Sharp.IconChar.Eye;

            }
        }

        private void RegularMemberIDCopyBtn_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(RegularMemberIDText.Text))
            {
                RegularMemberIDCopyLbl.Visible = true;
                RegularMemberIDCopyLbl.Text = "ID Number Copied Successfully";
                Clipboard.SetText(RegularMemberIDText.Text);

            }
        }
        private void RegularCreateAccBtn_Click(object sender, EventArgs e)
        {
            DateTime selectedDate = RegularBdayPicker.Value;
            DateTime currentDate = DateTime.Now;

            string rCreated = currentDate.ToString("MM-dd-yyyy");
            string rStatus = "Active";
            string rType = "Regular";
            string rPlanPeriod = "None";
            string rFirstname = RegularFirstNameText.Text;
            string rLastname = RegularLastNameText.Text;
            string rBday = selectedDate.ToString("MM-dd-yyyy");
            string rAge = RegularAgeText.Text;
            string rGender = RegularGenderComboText.Text;
            string rNumber = RegularMobileNumText.Text;
            string rEmailAdd = RegularEmailText.Text;
            string rMemberID = RegularMemberIDText.Text;
            string rPass = RegularPassText.Text;
            string rConfirmPass = RegularConfirmPassText.Text;

            Regex nameRegex = new Regex("^[A-Z][a-zA-Z]+(?: [a-zA-Z]+)*$");
            Regex gmailRegex = new Regex(@"^[A-Za-z0-9._%+-]*\d*@gmail\.com$");
            Regex passwordRegex = new Regex("^(?=.*[A-Z])(?=.*[a-z])(?=.*\\d)(?=.*[!@#$%^&*()_+\\-=\\[\\]{};':\"\\\\|,.<>\\/?])[A-Za-z\\d!@#$%^&*()_+\\-=\\[\\]{};':\"\\\\|,.<>\\/?]{8,}$");

            string hashedPassword = HashHelper.HashString(rPass);    // Password hashed
            string fixedSalt = HashHelper_Salt.HashString_Salt("Enchante" + rPass + "2024");    //Fixed Salt
            string perUserSalt = HashHelper_SaltperUser.HashString_SaltperUser(rPass + rMemberID);    //Per User salt

            int age = DateTime.Now.Year - selectedDate.Year;
            if (DateTime.Now < selectedDate.AddYears(age))
            {
                age--; // Subtract 1 if the birthday hasn't occurred yet this year
            }

            if (string.IsNullOrEmpty(rFirstname) || string.IsNullOrEmpty(rLastname) || string.IsNullOrEmpty(rAge) ||
                string.IsNullOrEmpty(rGender) || string.IsNullOrEmpty(rNumber) || string.IsNullOrEmpty(rEmailAdd) ||
                string.IsNullOrEmpty(rNumber) || string.IsNullOrEmpty(rPass) || string.IsNullOrEmpty(rConfirmPass))
            {
                RegularFirstNameErrorLbl.Visible = true;
                RegularGenderErrorLbl.Visible = true;
                RegularMobileNumErrorLbl.Visible = true;
                RegularEmailErrorLbl.Visible = true;
                RegularPassErrorLbl.Visible = true;
                RegularConfirmPassErrorLbl.Visible = true;
                RegularLastNameErrorLbl.Visible = true;
                RegularAgeErrorLbl.Visible = true;

                RegularFirstNameErrorLbl.Text = "Missing Field";
                RegularGenderErrorLbl.Text = "Missing Field";
                RegularMobileNumErrorLbl.Text = "Missing Field";
                RegularEmailErrorLbl.Text = "Missing Field";
                RegularPassErrorLbl.Text = "Missing Field";
                RegularConfirmPassErrorLbl.Text = "Missing Field";
                RegularLastNameErrorLbl.Text = "Missing Field";
                RegularAgeErrorLbl.Text = "Missing Field";

            }
            else if (age < 18)
            {
                RegularAgeErrorLbl.Visible = true;
                RegularAgeErrorLbl.Text = "Must be 18 years old and above";
                return;
            }
            else if (!nameRegex.IsMatch(rFirstname) && !nameRegex.IsMatch(rLastname))
            {
                RegularFirstNameErrorLbl.Visible = true;
                RegularLastNameErrorLbl.Visible = true;

                RegularFirstNameErrorLbl.Text = "First Letter Must Be Capital";
                RegularLastNameErrorLbl.Text = "First Letter Must Be Capital";

                return;
            }
            else if (!gmailRegex.IsMatch(rEmailAdd))
            {
                RegularEmailErrorLbl.Visible = true;
                RegularEmailErrorLbl.Text = "Invalid Email Format";
                return;
            }
            else if (!passwordRegex.IsMatch(rPass))
            {
                RegularPassErrorLbl.Visible = true;
                RegularPassErrorLbl.Text = "Invalid Password Format";
                return;
            }
            else if (rPass != rConfirmPass)
            {
                RegularConfirmPassErrorLbl.Visible = true;
                RegularPassErrorLbl.Text = "PASSWORD DOES NOT MATCH";
                return;
            }
            else
            {
                try
                {
                    using (MySqlConnection connection = new MySqlConnection(mysqlconn))
                    {
                        connection.Open();
                        // Check if email already exists
                        string checkEmailQuery = "SELECT COUNT(*) FROM membershipaccount WHERE EmailAdd = @email";
                        MySqlCommand checkEmailCmd = new MySqlCommand(checkEmailQuery, connection);
                        checkEmailCmd.Parameters.AddWithValue("@email", rEmailAdd);

                        int emailCount = Convert.ToInt32(checkEmailCmd.ExecuteScalar());

                        if (emailCount > 0)
                        {
                            // Email already exists, show a message or take appropriate action
                            MessageBox.Show("Email already exists. Please use a different email.", "Email Exists", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return; // Exit the method without inserting the new account
                        }
                        string insertQuery = "INSERT INTO membershipaccount (MembershipType, MemberIDNumber, AccountStatus, FirstName, " +
                            "LastName, Birthday, Age, CPNumber, EmailAdd, HashedPass, SaltedPass, UserSaltedPass, PlanPeriod, AccountCreated) " +
                            "VALUES (@type, @ID, @status, @firstName, @lastName, @bday, @age, @cpnum, @email, @hashedpass, @saltedpass, @usersaltedpass, @period, @created)";

                        MySqlCommand cmd = new MySqlCommand(insertQuery, connection);
                        cmd.Parameters.AddWithValue("@type", rType);
                        cmd.Parameters.AddWithValue("@ID", rMemberID);
                        cmd.Parameters.AddWithValue("@status", rStatus);
                        cmd.Parameters.AddWithValue("@firstName", rFirstname);
                        cmd.Parameters.AddWithValue("@lastName", rLastname);
                        cmd.Parameters.AddWithValue("@bday", rBday);
                        cmd.Parameters.AddWithValue("@age", rAge);
                        cmd.Parameters.AddWithValue("@cpnum", rNumber);
                        cmd.Parameters.AddWithValue("@email", rEmailAdd);
                        cmd.Parameters.AddWithValue("@hashedpass", hashedPassword);
                        cmd.Parameters.AddWithValue("@saltedpass", fixedSalt);
                        cmd.Parameters.AddWithValue("@usersaltedpass", perUserSalt);
                        cmd.Parameters.AddWithValue("@period", rPlanPeriod);
                        cmd.Parameters.AddWithValue("@created", rCreated);

                        cmd.ExecuteNonQuery();
                    }

                    // Successful insertion
                    MessageBox.Show("Regular Account is successfully created.", "Welcome to Enchanté", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    RegularAccIDGenerator();
                    RegularMembershipBoxClear();
                    MemberLocationAndColor();

                }
                catch (MySqlException ex)
                {
                    // Handle MySQL database exception
                    MessageBox.Show("MySQL Error: " + ex.Message, "Creating Regular Account Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    // Make sure to close the connection
                    connection.Close();
                }
            }


        }
        private void RegularMembershipBoxClear()
        {
            RegularFirstNameText.Text = "";
            RegularLastNameText.Text = "";
            RegularAgeText.Text = "";
            RegularGenderComboText.SelectedIndex = -1;
            RegularMobileNumText.Text = "";
            RegularEmailText.Text = "";
            RegularMemberIDText.Text = "";
            RegularPassText.Text = "";
            RegularConfirmPassText.Text = "";
            RegularPassText.UseSystemPasswordChar = true;
            RegularConfirmPassText.UseSystemPasswordChar = true;

        }

        //Super VIP Plan Membership
        private void SVIPExitBtn_Click(object sender, EventArgs e)
        {
            Registration.PanelShow(MembershipPlanPanel);
        }
        private void SetExpirationDate(string planType)
        {
            DateTime registrationDate = DateTime.Now; // Replace with your actual registration date

            switch (planType.ToLower())
            {
                case "monthly":
                    SVIPPlanExpirationText.Text = CalculateMonthlyExpirationDate(registrationDate);
                    PremPlanExpirationText.Text = CalculateMonthlyExpirationDate(registrationDate);

                    break;

                case "yearly":
                    SVIPPlanExpirationText.Text = CalculateYearlyExpirationDate(registrationDate);
                    PremPlanExpirationText.Text = CalculateMonthlyExpirationDate(registrationDate);

                    break;

                case "biyearly":
                    SVIPPlanExpirationText.Text = CalculateBiyearlyExpirationDate(registrationDate);
                    PremPlanExpirationText.Text = CalculateMonthlyExpirationDate(registrationDate);

                    break;

                default:
                    // Handle invalid plan type
                    break;
            }
        }

        private string CalculateMonthlyExpirationDate(DateTime registrationDate)
        {
            DateTime expirationDate = registrationDate.AddMonths(1);
            return expirationDate.ToString("MM-dd-yyyy");
        }

        private string CalculateYearlyExpirationDate(DateTime registrationDate)
        {
            DateTime expirationDate = registrationDate.AddYears(1);
            return expirationDate.ToString("MM-dd-yyyy");
        }

        private string CalculateBiyearlyExpirationDate(DateTime registrationDate)
        {
            DateTime expirationDate = registrationDate.AddYears(2);
            return expirationDate.ToString("MM-dd-yyyy");
        }
        private void SVIPMonthlyPlanBtn_Click(object sender, EventArgs e)
        {
            SVIPMonthly();
        }
        private void SVIPYearlyPlanBtn_Click(object sender, EventArgs e)
        {
            SVIPYearly();
        }

        private void SVIPBiyearlyPlanBtn_Click(object sender, EventArgs e)
        {
            SVIPBiyearly();
        }

        private void SVIPMonthlyPlanRB_CheckedChanged(object sender, EventArgs e)
        {
            //SVIPMonthly();
        }

        private void SVIPYearlyPlanRB_CheckedChanged(object sender, EventArgs e)
        {
            //SVIPYearly();
        }

        private void SVIPBiyearlyPlanRB_CheckedChanged(object sender, EventArgs e)
        {
            //SVIPBiyearly();
        }
        private void SVIPMonthly()
        {
            SetExpirationDate("monthly");

            if (SVIPMonthlyPlanRB.Checked == false)
            {
                SVIPMonthlyPlanRB.Visible = true;
                SVIPMonthlyPlanRB.Checked = true;
                SVIPPlanPeriodText.Text = "Super VIP Plan - Monthly";

                SVIPOrigPriceText.Visible = false;
                SVIPOrigPriceText.Text = "Php. 4999.00";
                SVIPNewPriceText.Text = "Php. 4999.00";

                SVIPYearlyPlanRB.Visible = false;
                SVIPBiyearlyPlanRB.Visible = false;
                SVIPYearlyPlanRB.Checked = false;
                SVIPBiyearlyPlanRB.Checked = false;
                return;
            }
            else if (SVIPMonthlyPlanRB.Checked == true)
            {
                SVIPPlanPeriodText.Text = "Super VIP Plan - Monthly";

                SVIPOrigPriceText.Visible = false;
                SVIPOrigPriceText.Text = "Php. 4999.00";
                SVIPNewPriceText.Text = "Php. 4999.00";

                SVIPYearlyPlanRB.Visible = false;
                SVIPBiyearlyPlanRB.Visible = false;
                SVIPYearlyPlanRB.Checked = false;
                SVIPBiyearlyPlanRB.Checked = false;
            }
        }
        private void SVIPYearly()
        {
            SetExpirationDate("yearly");

            if (SVIPYearlyPlanRB.Checked == false)
            {
                SVIPYearlyPlanRB.Visible = true;
                SVIPYearlyPlanRB.Checked = true;
                SVIPPlanPeriodText.Text = "Super VIP Plan - 12 Months";

                SVIPOrigPriceText.Visible = true;
                SVIPOrigPriceText.Text = "Php. 4999.00";
                SVIPNewPriceText.Text = "Php. 3499.00";

                SVIPMonthlyPlanRB.Visible = false;
                SVIPBiyearlyPlanRB.Visible = false;
                SVIPMonthlyPlanRB.Checked = false;
                SVIPBiyearlyPlanRB.Checked = false;
            }
            else
            {
                SVIPYearlyPlanRB.Visible = true;
                SVIPYearlyPlanRB.Checked = true;
            }
        }
        private void SVIPBiyearly()
        {
            SetExpirationDate("biyearly");

            if (SVIPBiyearlyPlanRB.Checked == false)
            {
                SVIPBiyearlyPlanRB.Visible = true;
                SVIPBiyearlyPlanRB.Checked = true;
                SVIPPlanPeriodText.Text = "Super VIP Plan - 24 Months";

                SVIPOrigPriceText.Visible = true;
                SVIPOrigPriceText.Text = "Php. 4999.00";
                SVIPNewPriceText.Text = "Php. 2999.00";

                SVIPMonthlyPlanRB.Visible = false;
                SVIPYearlyPlanRB.Visible = false;
                SVIPMonthlyPlanRB.Checked = false;
                SVIPYearlyPlanRB.Checked = false;
            }
            else
            {
                SVIPBiyearlyPlanRB.Visible = true;
                SVIPBiyearlyPlanRB.Checked = true;
            }
        }

        private void SVIPMemberCopyBtn_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(SVIPMemberIDText.Text))
            {
                SVIPMemberIDCopyLbl.Visible = true;
                SVIPMemberIDCopyLbl.Text = "ID Number Copied Successfully";
                Clipboard.SetText(SVIPMemberIDText.Text);

            }
        }

        private void SVIPPassReqBtn_MouseHover(object sender, EventArgs e)
        {
            string message = "Must be at least 8 character long.\n";
            message += "First character must be capital.\n";
            message += "Must include a special character and a number.";

            iconToolTip.SetToolTip(SVIPPassReqBtn, message);
        }

        private void SVIPShowHidePassBtn_Click(object sender, EventArgs e)
        {
            if (SVIPPassText.UseSystemPasswordChar == true)
            {
                SVIPPassText.UseSystemPasswordChar = false;
                SVIPShowHidePassBtn.IconChar = FontAwesome.Sharp.IconChar.EyeSlash;
            }
            else if (SVIPPassText.UseSystemPasswordChar == false)
            {
                SVIPPassText.UseSystemPasswordChar = true;
                SVIPShowHidePassBtn.IconChar = FontAwesome.Sharp.IconChar.Eye;

            }
        }

        private void SVIPShowHideConfirmPassBtn_Click(object sender, EventArgs e)
        {
            if (SVIPConfirmPassText.UseSystemPasswordChar == true)
            {
                SVIPConfirmPassText.UseSystemPasswordChar = false;
                SVIPShowHideConfirmPassBtn.IconChar = FontAwesome.Sharp.IconChar.EyeSlash;
            }
            else if (SVIPConfirmPassText.UseSystemPasswordChar == false)
            {
                SVIPConfirmPassText.UseSystemPasswordChar = true;
                SVIPShowHideConfirmPassBtn.IconChar = FontAwesome.Sharp.IconChar.Eye;

            }
        }

        private void SVIPBdayPicker_ValueChanged(object sender, EventArgs e)
        {
            DateTime selectedDate = SVIPBdayPicker.Value;
            int age = DateTime.Now.Year - selectedDate.Year;

            if (DateTime.Now < selectedDate.AddYears(age))
            {
                age--; // Subtract 1 if the birthday hasn't occurred yet this year
            }
            SVIPAgeText.Text = age.ToString();
            if (age < 18)
            {
                SVIPAgeErrorLbl.Visible = true;
                SVIPAgeErrorLbl.Text = "Must be 18 years old and above";
                return;
            }
            else
            {
                SVIPAgeErrorLbl.Visible = false;

            }
        }

        private void SVIPGenderComboText_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (SVIPGenderComboText.SelectedItem != null)
            {
                SVIPGenderComboText.Text = SVIPGenderComboText.SelectedItem.ToString();
            }
        }

        private void SVIPCCPaymentBtn_Click(object sender, EventArgs e)
        {
            if (SVIPCCPaymentRB.Checked == false)
            {
                SVIPCCPaymentRB.Visible = true;
                SVIPCCPaymentRB.Checked = true;
                SVIPPaymentTypeText.Text = "Credit Card";

                SVIPPayPPaymentRB.Visible = false;
                SVIPGCPaymentRB.Visible = false;
                SVIPPayMPaymentRB.Visible = false;
                SVIPPayPPaymentRB.Checked = false;
                SVIPGCPaymentRB.Checked = false;
                SVIPPayMPaymentRB.Checked = false;
            }
            else
            {
                SVIPCCPaymentRB.Visible = true;
                SVIPCCPaymentRB.Checked = true;
            }
        }

        private void SVIPPayPPaymentBtn_Click(object sender, EventArgs e)
        {
            if (SVIPPayPPaymentRB.Checked == false)
            {
                SVIPPayPPaymentRB.Visible = true;
                SVIPPayPPaymentRB.Checked = true;
                SVIPPaymentTypeText.Text = "Paypal";

                SVIPCCPaymentRB.Visible = false;
                SVIPGCPaymentRB.Visible = false;
                SVIPPayMPaymentRB.Visible = false;
                SVIPCCPaymentRB.Checked = false;
                SVIPGCPaymentRB.Checked = false;
                SVIPPayMPaymentRB.Checked = false;
            }
            else
            {
                SVIPPayPPaymentRB.Visible = true;
                SVIPPayPPaymentRB.Checked = true;
            }
        }

        private void SVIPGCPaymentBtn_Click(object sender, EventArgs e)
        {
            if (SVIPGCPaymentRB.Checked == false)
            {
                SVIPGCPaymentRB.Visible = true;
                SVIPGCPaymentRB.Checked = true;
                SVIPPaymentTypeText.Text = "GCash";

                SVIPCCPaymentRB.Visible = false;
                SVIPPayPPaymentRB.Visible = false;
                SVIPPayMPaymentRB.Visible = false;
                SVIPCCPaymentRB.Checked = false;
                SVIPPayPPaymentRB.Checked = false;
                SVIPPayMPaymentRB.Checked = false;
            }
            else
            {
                SVIPGCPaymentRB.Visible = true;
                SVIPGCPaymentRB.Checked = true;
            }
        }

        private void SVIPPayMPaymentBtn_Click(object sender, EventArgs e)
        {
            if (SVIPPayMPaymentRB.Checked == false)
            {
                SVIPPayMPaymentRB.Visible = true;
                SVIPPayMPaymentRB.Checked = true;
                SVIPPaymentTypeText.Text = "Paymaya";


                SVIPCCPaymentRB.Visible = false;
                SVIPPayPPaymentRB.Visible = false;
                SVIPGCPaymentRB.Visible = false;
                SVIPCCPaymentRB.Checked = false;
                SVIPPayPPaymentRB.Checked = false;
                SVIPGCPaymentRB.Checked = false;
            }
            else
            {
                SVIPPayMPaymentRB.Checked = true;
            }
        }
        private void SVIPConfirmPassText_TextChanged(object sender, EventArgs e)
        {
            if (SVIPConfirmPassText.Text != SVIPPassText.Text)
            {
                SVIPConfirmPassErrorLbl.Visible = true;
                SVIPConfirmPassErrorLbl.Text = "PASSWORD DOES NOT MATCH";
            }
            else
            {
                SVIPConfirmPassErrorLbl.Visible = false;
            }
        }

        private void SVIPCreateAccBtn_Click(object sender, EventArgs e)
        {
            DateTime selectedDate = SVIPBdayPicker.Value;
            DateTime currentDate = DateTime.Now;

            string SVCreated = currentDate.ToString("MM-dd-yyyy");
            string SVStatus = "Active";
            string SVType = "SVIP";
            string SVFirstname = SVIPFirstNameText.Text;
            string SVLastname = SVIPLastNameText.Text;
            string SVBday = selectedDate.ToString("MM-dd-yyyy");
            string SVAge = SVIPAgeText.Text;
            string SVGender = SVIPGenderComboText.Text;
            string SVNumber = SVIPCPNumText.Text;
            string SVEmailAdd = SVIPEmailText.Text;
            string SVMemberID = SVIPMemberIDText.Text;
            string SVPass = SVIPPassText.Text;
            string SVConfirmPass = SVIPConfirmPassText.Text;
            string SVPeriod = SVIPPlanPeriodText.Text;
            string SVPayment = SVIPPaymentTypeText.Text;
            string SVCardName = SVIPCardNameText.Text;
            string SVCardNum = SVIPCardNumText.Text;
            string SVCardExpire = SVIPCardExpireText.Text;
            string SVcvc = SVIPCardCVCText.Text;
            string SVPlanExpire = SVIPPlanExpirationText.Text;
            string SVPlanRenew = "";
            string SVAmount = SVIPNewPriceText.Text;


            Regex nameRegex = new Regex("^[A-Z][a-zA-Z]+(?: [a-zA-Z]+)*$");
            Regex gmailRegex = new Regex(@"^[A-Za-z0-9._%+-]*\d*@gmail\.com$");
            Regex passwordRegex = new Regex("^(?=.*[A-Z])(?=.*[a-z])(?=.*\\d)(?=.*[!@#$%^&*()_+\\-=\\[\\]{};':\"\\\\|,.<>\\/?])[A-Za-z\\d!@#$%^&*()_+\\-=\\[\\]{};':\"\\\\|,.<>\\/?]{8,}$");

            string hashedPassword = HashHelper.HashString(SVPass);    // Password hashed
            string fixedSalt = HashHelper_Salt.HashString_Salt("Enchante" + SVPass + "2024");    //Fixed Salt
            string perUserSalt = HashHelper_SaltperUser.HashString_SaltperUser(SVPass + SVMemberID);    //Per User salt

            int age = DateTime.Now.Year - selectedDate.Year;
            if (DateTime.Now < selectedDate.AddYears(age))
            {
                age--; // Subtract 1 if the birthday hasn't occurred yet this year
            }

            if (string.IsNullOrEmpty(SVFirstname) || string.IsNullOrEmpty(SVLastname) || string.IsNullOrEmpty(SVAge) ||
                string.IsNullOrEmpty(SVGender) || string.IsNullOrEmpty(SVNumber) || string.IsNullOrEmpty(SVEmailAdd) ||
                string.IsNullOrEmpty(SVNumber) || string.IsNullOrEmpty(SVPass) || string.IsNullOrEmpty(SVConfirmPass) ||
                string.IsNullOrEmpty(SVPeriod) || string.IsNullOrEmpty(SVPayment) || string.IsNullOrEmpty(SVCardName) ||
                string.IsNullOrEmpty(SVCardNum) || string.IsNullOrEmpty(SVCardExpire) || string.IsNullOrEmpty(SVcvc) || string.IsNullOrEmpty(SVAmount))
            {
                SVIPFirstNameErrorLbl.Visible = true;
                SVIPGenderErrorLbl.Visible = true;
                SVIPCPNumErrorLbl.Visible = true;
                SVIPEmailErrorLbl.Visible = true;
                SVIPPassErrorLbl.Visible = true;
                SVIPConfirmPassErrorLbl.Visible = true;
                SVIPLastNameErrorLbl.Visible = true;
                SVIPAgeErrorLbl.Visible = true;


                SVIPFirstNameErrorLbl.Text = "Missing Field";
                SVIPGenderErrorLbl.Text = "Missing Field";
                SVIPCPNumErrorLbl.Text = "Missing Field";
                SVIPEmailErrorLbl.Text = "Missing Field";
                SVIPPassErrorLbl.Text = "Missing Field";
                SVIPConfirmPassErrorLbl.Text = "Missing Field";
                SVIPLastNameErrorLbl.Text = "Missing Field";
                SVIPAgeErrorLbl.Text = "Missing Field";

            }
            else if (age < 18)
            {
                SVIPAgeErrorLbl.Visible = true;
                SVIPAgeErrorLbl.Text = "Must be 18 years old and above";
                return;
            }
            else if (!nameRegex.IsMatch(SVFirstname) && !nameRegex.IsMatch(SVLastname))
            {
                SVIPFirstNameErrorLbl.Visible = true;
                SVIPLastNameErrorLbl.Visible = true;

                SVIPFirstNameErrorLbl.Text = "First Letter Must Be Capital";
                SVIPLastNameErrorLbl.Text = "First Letter Must Be Capital";

                return;
            }
            else if (!gmailRegex.IsMatch(SVEmailAdd))
            {
                SVIPEmailErrorLbl.Visible = true;
                SVIPEmailErrorLbl.Text = "Invalid Email Format";
                return;
            }
            else if (!passwordRegex.IsMatch(SVPass))
            {
                SVIPPassErrorLbl.Visible = true;
                SVIPPassErrorLbl.Text = "Invalid Password Format";
                return;
            }
            else if (SVPass != SVConfirmPass)
            {
                SVIPConfirmPassErrorLbl.Visible = true;
                SVIPPassErrorLbl.Text = "PASSWORD DOES NOT MATCH";
                return;
            }
            else
            {
                try
                {
                    using (MySqlConnection connection = new MySqlConnection(mysqlconn))
                    {
                        connection.Open();

                        // Check if email already exists
                        string checkEmailQuery = "SELECT COUNT(*) FROM membershipaccount WHERE EmailAdd = @email";
                        MySqlCommand checkEmailCmd = new MySqlCommand(checkEmailQuery, connection);
                        checkEmailCmd.Parameters.AddWithValue("@email", SVEmailAdd);

                        int emailCount = Convert.ToInt32(checkEmailCmd.ExecuteScalar());

                        if (emailCount > 0)
                        {
                            // Email already exists, show a message or take appropriate action
                            MessageBox.Show("Email already exists. Please use a different email.", "Email Exists", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return; // Exit the method without inserting the new account
                        }

                        // Email doesn't exist, proceed with insertion
                        string insertQuery = "INSERT INTO membershipaccount (MembershipType, MemberIDNumber, AccountStatus, FirstName, " +
                            "LastName, Birthday, Age, CPNumber, EmailAdd, HashedPass, SaltedPass, UserSaltedPass, PlanPeriod, " +
                            "PaymentType, CardholderName, CardNumber, CardExpiration, CVCCode, AccountCreated, PlanExpiration, PlanRenewal, AmountPaid) " +
                            "VALUES (@type, @ID, @status, @firstName, @lastName, @bday, @age, @cpnum, @email, @hashedpass, @saltedpass, @usersaltedpass, " +
                            "@period, @payment, @cardname, @cardnumber, @cardexpiration, @cvc, @created, @planExpiration, @planRenew, @amount)";

                        MySqlCommand cmd = new MySqlCommand(insertQuery, connection);
                        cmd.Parameters.AddWithValue("@type", SVType);
                        cmd.Parameters.AddWithValue("@ID", SVMemberID);
                        cmd.Parameters.AddWithValue("@status", SVStatus);
                        cmd.Parameters.AddWithValue("@firstName", SVFirstname);
                        cmd.Parameters.AddWithValue("@lastName", SVLastname);
                        cmd.Parameters.AddWithValue("@bday", SVBday);
                        cmd.Parameters.AddWithValue("@age", SVAge);
                        cmd.Parameters.AddWithValue("@cpnum", SVNumber);
                        cmd.Parameters.AddWithValue("@email", SVEmailAdd);
                        cmd.Parameters.AddWithValue("@hashedpass", hashedPassword);
                        cmd.Parameters.AddWithValue("@saltedpass", fixedSalt);
                        cmd.Parameters.AddWithValue("@usersaltedpass", perUserSalt);
                        cmd.Parameters.AddWithValue("@period", SVPeriod);
                        cmd.Parameters.AddWithValue("@payment", SVPayment);
                        cmd.Parameters.AddWithValue("@cardname", SVCardName);
                        cmd.Parameters.AddWithValue("@cardnumber", SVCardNum);
                        cmd.Parameters.AddWithValue("@cardexpiration", SVCardExpire);
                        cmd.Parameters.AddWithValue("@cvc", SVcvc);
                        cmd.Parameters.AddWithValue("@created", SVCreated);
                        cmd.Parameters.AddWithValue("@planExpiration", SVPlanExpire);
                        cmd.Parameters.AddWithValue("@planRenew", SVPlanRenew);
                        cmd.Parameters.AddWithValue("@amount", SVAmount);

                        cmd.ExecuteNonQuery();
                    }

                    // Successful insertion
                    MessageBox.Show("SVIP Account is successfully created.", "Welcome to Enchanté", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    SVIPAccIDGenerator();
                    SVIPMembershipBoxClear();
                    MemberLocationAndColor();
                }
                catch (MySqlException ex)
                {
                    // Handle MySQL database exception
                    MessageBox.Show("MySQL Error: " + ex.Message, "Creating SVIP Account Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    // No need to close the connection here as it is in a using statement
                }



            }
        }
        private void SVIPMembershipBoxClear()
        {
            SVIPFirstNameText.Text = "";
            SVIPLastNameText.Text = "";
            SVIPAgeText.Text = "";
            SVIPGenderComboText.SelectedIndex = -1;
            SVIPCPNumText.Text = "";
            SVIPEmailText.Text = "";
            SVIPMemberIDText.Text = "";
            SVIPPassText.Text = "";
            SVIPConfirmPassText.Text = "";
            SVIPPassText.UseSystemPasswordChar = true;
            SVIPConfirmPassText.UseSystemPasswordChar = true;
            SVIPBdayPicker.Value = DateTime.Now;
            SVIPCardNameText.Text = "";
            SVIPPlanPeriodText.Text = "";
            SVIPPaymentTypeText.Text = "";
            SVIPCardNumText.Text = "";
            SVIPCardExpireText.Text = "";
            SVIPCardCVCText.Text = "";
            SVIPPlanExpirationText.Text = "";
            SVIPNewPriceText.Text = "";

        }
        //PREMIUM REGISTRATION
        private void PremiumExitBtn_Click(object sender, EventArgs e)
        {
            Registration.PanelShow(MembershipPlanPanel);
        }

        private void PremMonthlyPlanBtn_Click(object sender, EventArgs e)
        {
            PremMonthly();
        }

        private void PremYearlyPlanBtn_Click(object sender, EventArgs e)
        {
            PremYearly();
        }

        private void PremBiyearlyPlanBtn_Click(object sender, EventArgs e)
        {
            PremBiyearly();
        }

        private void PremPassReqBtn_MouseHover(object sender, EventArgs e)
        {
            string message = "Must be at least 8 character long.\n";
            message += "First character must be capital.\n";
            message += "Must include a special character and a number.";

            iconToolTip.SetToolTip(PremPassReqBtn, message);
        }

        private void PremShowHidePassBtn_MouseHover(object sender, EventArgs e)
        {
            if (PremPassText.UseSystemPasswordChar == true)
            {
                iconToolTip.SetToolTip(PremShowHidePassBtn, "Show Password");
            }
            else if (PremPassText.UseSystemPasswordChar == false)
            {
                iconToolTip.SetToolTip(PremShowHidePassBtn, "Hide Password");
            }
        }

        private void PremShowHidePassBtn_Click(object sender, EventArgs e)
        {
            if (PremPassText.UseSystemPasswordChar == true)
            {
                PremPassText.UseSystemPasswordChar = false;
                PremShowHidePassBtn.IconChar = FontAwesome.Sharp.IconChar.EyeSlash;
            }
            else if (SVIPPassText.UseSystemPasswordChar == false)
            {
                PremPassText.UseSystemPasswordChar = true;
                PremShowHidePassBtn.IconChar = FontAwesome.Sharp.IconChar.Eye;

            }
        }

        private void PremShowHideConfirmPassBtn_MouseHover(object sender, EventArgs e)
        {
            if (PremConfirmPassText.UseSystemPasswordChar == true)
            {
                iconToolTip.SetToolTip(PremShowHideConfirmPassBtn, "Show Password");
            }
            else if (PremConfirmPassText.UseSystemPasswordChar == false)
            {
                iconToolTip.SetToolTip(PremShowHideConfirmPassBtn, "Hide Password");
            }
        }

        private void PremShowHideConfirmPassBtn_Click(object sender, EventArgs e)
        {
            if (PremConfirmPassText.UseSystemPasswordChar == true)
            {
                PremConfirmPassText.UseSystemPasswordChar = false;
                PremShowHideConfirmPassBtn.IconChar = FontAwesome.Sharp.IconChar.EyeSlash;
            }
            else if (SVIPConfirmPassText.UseSystemPasswordChar == false)
            {
                PremConfirmPassText.UseSystemPasswordChar = true;
                PremShowHideConfirmPassBtn.IconChar = FontAwesome.Sharp.IconChar.Eye;

            }
        }
        private void PremConfirmPassText_TextChanged(object sender, EventArgs e)
        {
            if (PremConfirmPassText.Text != PremPassText.Text)
            {
                PremConfirmPassErrorLbl.Visible = true;
                PremConfirmPassErrorLbl.Text = "PASSWORD DOES NOT MATCH";
            }
            else
            {
                PremConfirmPassErrorLbl.Visible = false;
            }
        }
        private void PremMemberIDCopyBtn_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(SVIPMemberIDText.Text))
            {
                PremMemberIDCopyLbl.Visible = true;
                PremMemberIDCopyLbl.Text = "ID Number Copied Successfully";
                Clipboard.SetText(PremMemberIDText.Text);

            }
        }

        private void PremBdayPicker_ValueChanged(object sender, EventArgs e)
        {
            DateTime selectedDate = PremBdayPicker.Value;
            int age = DateTime.Now.Year - selectedDate.Year;

            if (DateTime.Now < selectedDate.AddYears(age))
            {
                age--; // Subtract 1 if the birthday hasn't occurred yet this year
            }
            PremAgeText.Text = age.ToString();
            if (age < 18)
            {
                PremAgeErrorLbl.Visible = true;
                PremAgeErrorLbl.Text = "Must be 18 years old and above";
                return;
            }
            else
            {
                PremAgeErrorLbl.Visible = false;

            }
        }

        private void PremGenderComboText_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (PremGenderComboText.SelectedItem != null)
            {
                PremGenderComboText.Text = PremGenderComboText.SelectedItem.ToString();
            }
        }
        private void PremMonthly()
        {
            SetExpirationDate("monthly");

            if (PremMonthlyPlanRB.Checked == false)
            {
                PremMonthlyPlanRB.Visible = true;
                PremMonthlyPlanRB.Checked = true;
                PremPlanPeriodText.Text = "Premium Plan - Monthly";

                PremOrigPriceText.Visible = false;
                PremOrigPriceText.Text = "Php. 1499.00";
                PremNewPriceText.Text = "Php. 1499.00";

                PremYearlyPlanRB.Visible = false;
                PremBiyearlyPlanRB.Visible = false;
                PremYearlyPlanRB.Checked = false;
                PremBiyearlyPlanRB.Checked = false;
                return;
            }
            else if (PremMonthlyPlanRB.Checked == true)
            {
                PremPlanPeriodText.Text = "Premium Plan - Monthly";

                PremOrigPriceText.Visible = false;
                PremOrigPriceText.Text = "Php. 1499.00";
                PremNewPriceText.Text = "Php. 1499.00";

                PremYearlyPlanRB.Visible = false;
                PremBiyearlyPlanRB.Visible = false;
                PremYearlyPlanRB.Checked = false;
                PremBiyearlyPlanRB.Checked = false;
            }
        }
        private void PremYearly()
        {
            SetExpirationDate("yearly");

            if (PremYearlyPlanRB.Checked == false)
            {
                PremYearlyPlanRB.Visible = true;
                PremYearlyPlanRB.Checked = true;
                PremPlanPeriodText.Text = "Premium Plan - 12 Months";

                PremOrigPriceText.Visible = true;
                PremOrigPriceText.Text = "Php. 1499.00";
                PremNewPriceText.Text = "Php. 1299.00";

                PremMonthlyPlanRB.Visible = false;
                PremBiyearlyPlanRB.Visible = false;
                PremMonthlyPlanRB.Checked = false;
                PremBiyearlyPlanRB.Checked = false;
            }
            else
            {
                PremYearlyPlanRB.Visible = true;
                PremYearlyPlanRB.Checked = true;
            }
        }
        private void PremBiyearly()
        {
            SetExpirationDate("biyearly");

            if (PremBiyearlyPlanRB.Checked == false)
            {
                PremBiyearlyPlanRB.Visible = true;
                PremBiyearlyPlanRB.Checked = true;
                PremPlanPeriodText.Text = "Premium Plan - 24 Months";

                PremOrigPriceText.Visible = true;
                PremOrigPriceText.Text = "Php. 1499.00";
                PremNewPriceText.Text = "Php. 999.00";

                PremMonthlyPlanRB.Visible = false;
                PremYearlyPlanRB.Visible = false;
                PremMonthlyPlanRB.Checked = false;
                PremYearlyPlanRB.Checked = false;
            }
            else
            {
                PremBiyearlyPlanRB.Visible = true;
                PremBiyearlyPlanRB.Checked = true;
            }
        }


        private void PremCCPaymentBtn_Click(object sender, EventArgs e)
        {
            if (PremCCPaymentRB.Checked == false)
            {
                PremCCPaymentRB.Visible = true;
                PremCCPaymentRB.Checked = true;
                PremPaymentTypeText.Text = "Credit Card";

                PremPayPPaymentRB.Visible = false;
                PremGCPaymentRB.Visible = false;
                PremPayMPaymentRB.Visible = false;
                PremPayPPaymentRB.Checked = false;
                PremGCPaymentRB.Checked = false;
                PremPayMPaymentRB.Checked = false;
            }
            else
            {
                PremCCPaymentRB.Visible = true;
                PremCCPaymentRB.Checked = true;
            }
        }

        private void PremPayPPaymentBtn_Click(object sender, EventArgs e)
        {
            if (PremPayPPaymentRB.Checked == false)
            {
                PremPayPPaymentRB.Visible = true;
                PremPayPPaymentRB.Checked = true;
                PremPaymentTypeText.Text = "Paypal";

                PremCCPaymentRB.Visible = false;
                PremGCPaymentRB.Visible = false;
                PremPayMPaymentRB.Visible = false;
                PremCCPaymentRB.Checked = false;
                PremGCPaymentRB.Checked = false;
                PremPayMPaymentRB.Checked = false;
            }
            else
            {
                PremPayPPaymentRB.Visible = true;
                PremPayPPaymentRB.Checked = true;
            }
        }

        private void PremGCPaymentBtn_Click(object sender, EventArgs e)
        {
            if (PremGCPaymentRB.Checked == false)
            {
                PremGCPaymentRB.Visible = true;
                PremGCPaymentRB.Checked = true;
                PremPaymentTypeText.Text = "GCash";

                PremCCPaymentRB.Visible = false;
                PremPayPPaymentRB.Visible = false;
                PremPayMPaymentRB.Visible = false;
                PremCCPaymentRB.Checked = false;
                PremPayPPaymentRB.Checked = false;
                PremPayMPaymentRB.Checked = false;
            }
            else
            {
                PremGCPaymentRB.Visible = true;
                PremGCPaymentRB.Checked = true;
            }
        }

        private void PremPayMPaymentBtn_Click(object sender, EventArgs e)
        {
            if (PremPayMPaymentRB.Checked == false)
            {
                PremPayMPaymentRB.Visible = true;
                PremPayMPaymentRB.Checked = true;
                PremPaymentTypeText.Text = "Paymaya";

                PremCCPaymentRB.Visible = false;
                PremPayPPaymentRB.Visible = false;
                PremGCPaymentRB.Visible = false;
                PremCCPaymentRB.Checked = false;
                PremPayPPaymentRB.Checked = false;
                PremGCPaymentRB.Checked = false;
            }
            else
            {
                PremPayMPaymentRB.Checked = true;
            }
        }

        private void PremCreateAccBtn_Click(object sender, EventArgs e)
        {
            DateTime selectedDate = PremBdayPicker.Value;
            DateTime currentDate = DateTime.Now;

            string PremCreated = currentDate.ToString("MM-dd-yyyy");
            string PremStatus = "Active";
            string PremType = "PREMIUM";
            string PremFirstname = PremFirstNameText.Text;
            string PremLastname = PremLastNameText.Text;
            string PremBday = selectedDate.ToString("MM-dd-yyyy");
            string PremAge = PremAgeText.Text;
            string PremGender = PremGenderComboText.Text;
            string PremNumber = PremCPNumText.Text;
            string PremEmailAdd = PremEmailText.Text;
            string PremMemberID = PremMemberIDText.Text;
            string PremPass = PremPassText.Text;
            string PremConfirmPass = PremConfirmPassText.Text;
            string PremPeriod = PremPlanPeriodText.Text;
            string PremPayment = PremPaymentTypeText.Text;
            string PremCardName = PremCardNameText.Text;
            string PremCardNum = PremCardNumText.Text;
            string PremCardExpire = PremCardExpireText.Text;
            string Premcvc = PremCardCVCText.Text;
            string PremPlanExpire = PremPlanExpirationText.Text;
            string PremPlanRenew = "";
            string PremAmount = PremNewPriceText.Text;


            Regex nameRegex = new Regex("^[A-Z][a-zA-Z]+(?: [a-zA-Z]+)*$");
            Regex gmailRegex = new Regex(@"^[A-Za-z0-9._%+-]*\d*@gmail\.com$");
            Regex passwordRegex = new Regex("^(?=.*[A-Z])(?=.*[a-z])(?=.*\\d)(?=.*[!@#$%^&*()_+\\-=\\[\\]{};':\"\\\\|,.<>\\/?])[A-Za-z\\d!@#$%^&*()_+\\-=\\[\\]{};':\"\\\\|,.<>\\/?]{8,}$");

            string hashedPassword = HashHelper.HashString(PremPass);    // Password hashed
            string fixedSalt = HashHelper_Salt.HashString_Salt("Enchante" + PremPass + "2024");    //Fixed Salt
            string perUserSalt = HashHelper_SaltperUser.HashString_SaltperUser(PremPass + PremMemberID);    //Per User salt

            int age = DateTime.Now.Year - selectedDate.Year;
            if (DateTime.Now < selectedDate.AddYears(age))
            {
                age--; // Subtract 1 if the birthday hasn't occurred yet this year
            }

            if (string.IsNullOrEmpty(PremFirstname) || string.IsNullOrEmpty(PremLastname) || string.IsNullOrEmpty(PremAge) ||
                string.IsNullOrEmpty(PremGender) || string.IsNullOrEmpty(PremNumber) || string.IsNullOrEmpty(PremEmailAdd) ||
                string.IsNullOrEmpty(PremNumber) || string.IsNullOrEmpty(PremPass) || string.IsNullOrEmpty(PremConfirmPass) ||
                string.IsNullOrEmpty(PremPeriod) || string.IsNullOrEmpty(PremPayment) || string.IsNullOrEmpty(PremCardName) ||
                string.IsNullOrEmpty(PremCardNum) || string.IsNullOrEmpty(PremCardExpire) || string.IsNullOrEmpty(Premcvc) || string.IsNullOrEmpty(PremAmount))
            {
                PremFirstNameErrorLbl.Visible = true;
                PremGenderErrorLbl.Visible = true;
                PremCPNumErrorLbl.Visible = true;
                PremEmailErrorLbl.Visible = true;
                PremPassErrorLbl.Visible = true;
                PremConfirmPassErrorLbl.Visible = true;
                PremLastNameErrorLbl.Visible = true;
                PremAgeErrorLbl.Visible = true;


                PremFirstNameErrorLbl.Text = "Missing Field";
                PremGenderErrorLbl.Text = "Missing Field";
                PremCPNumErrorLbl.Text = "Missing Field";
                PremEmailErrorLbl.Text = "Missing Field";
                PremPassErrorLbl.Text = "Missing Field";
                PremConfirmPassErrorLbl.Text = "Missing Field";
                PremLastNameErrorLbl.Text = "Missing Field";
                PremAgeErrorLbl.Text = "Missing Field";

            }
            else if (age < 18)
            {
                PremAgeErrorLbl.Visible = true;
                PremAgeErrorLbl.Text = "Must be 18 years old and above";
                return;
            }
            else if (!nameRegex.IsMatch(PremFirstname) && !nameRegex.IsMatch(PremLastname))
            {
                PremFirstNameErrorLbl.Visible = true;
                PremLastNameErrorLbl.Visible = true;

                PremFirstNameErrorLbl.Text = "First Letter Must Be Capital";
                PremLastNameErrorLbl.Text = "First Letter Must Be Capital";

                return;
            }
            else if (!gmailRegex.IsMatch(PremEmailAdd))
            {
                PremEmailErrorLbl.Visible = true;
                PremEmailErrorLbl.Text = "Invalid Email Format";
                return;
            }
            else if (!passwordRegex.IsMatch(PremPass))
            {
                PremPassErrorLbl.Visible = true;
                PremPassErrorLbl.Text = "Invalid Password Format";
                return;
            }
            else if (PremPass != PremConfirmPass)
            {
                PremConfirmPassErrorLbl.Visible = true;
                PremConfirmPassErrorLbl.Text = "PASSWORD DOES NOT MATCH";
                return;
            }
            else
            {
                try
                {
                    using (MySqlConnection connection = new MySqlConnection(mysqlconn))
                    {
                        connection.Open();

                        // Check if email already exists
                        string checkEmailQuery = "SELECT COUNT(*) FROM membershipaccount WHERE EmailAdd = @email";
                        MySqlCommand checkEmailCmd = new MySqlCommand(checkEmailQuery, connection);
                        checkEmailCmd.Parameters.AddWithValue("@email", PremEmailAdd);

                        int emailCount = Convert.ToInt32(checkEmailCmd.ExecuteScalar());

                        if (emailCount > 0)
                        {
                            // Email already exists, show a message or take appropriate action
                            MessageBox.Show("Email already exists. Please use a different email.", "Email Exists", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return; // Exit the method without inserting the new account
                        }

                        // Email doesn't exist, proceed with insertion
                        string insertQuery = "INSERT INTO membershipaccount (MembershipType, MemberIDNumber, AccountStatus, FirstName, " +
                            "LastName, Birthday, Age, CPNumber, EmailAdd, HashedPass, SaltedPass, UserSaltedPass, PlanPeriod, " +
                            "PaymentType, CardholderName, CardNumber, CardExpiration, CVCCode, AccountCreated, PlanExpiration, PlanRenewal, AmountPaid) " +
                            "VALUES (@type, @ID, @status, @firstName, @lastName, @bday, @age, @cpnum, @email, @hashedpass, @saltedpass, @usersaltedpass, " +
                            "@period, @payment, @cardname, @cardnumber, @cardexpiration, @cvc, @created, @planExpiration, @planRenew, @amount)";

                        MySqlCommand cmd = new MySqlCommand(insertQuery, connection);
                        cmd.Parameters.AddWithValue("@type", PremType);
                        cmd.Parameters.AddWithValue("@ID", PremMemberID);
                        cmd.Parameters.AddWithValue("@status", PremStatus);
                        cmd.Parameters.AddWithValue("@firstName", PremFirstname);
                        cmd.Parameters.AddWithValue("@lastName", PremLastname);
                        cmd.Parameters.AddWithValue("@bday", PremBday);
                        cmd.Parameters.AddWithValue("@age", PremAge);
                        cmd.Parameters.AddWithValue("@cpnum", PremNumber);
                        cmd.Parameters.AddWithValue("@email", PremEmailAdd);
                        cmd.Parameters.AddWithValue("@hashedpass", hashedPassword);
                        cmd.Parameters.AddWithValue("@saltedpass", fixedSalt);
                        cmd.Parameters.AddWithValue("@usersaltedpass", perUserSalt);
                        cmd.Parameters.AddWithValue("@period", PremPeriod);
                        cmd.Parameters.AddWithValue("@payment", PremPayment);
                        cmd.Parameters.AddWithValue("@cardname", PremCardName);
                        cmd.Parameters.AddWithValue("@cardnumber", PremCardNum);
                        cmd.Parameters.AddWithValue("@cardexpiration", PremCardExpire);
                        cmd.Parameters.AddWithValue("@cvc", Premcvc);
                        cmd.Parameters.AddWithValue("@created", PremCreated);
                        cmd.Parameters.AddWithValue("@planExpiration", PremPlanExpire);
                        cmd.Parameters.AddWithValue("@planRenew", PremPlanRenew);
                        cmd.Parameters.AddWithValue("@amount", PremAmount);

                        cmd.ExecuteNonQuery();
                    }

                    // Successful insertion
                    MessageBox.Show("Premium Account is successfully created.", "Welcome to Enchanté", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    PremAccIDGenerator();
                    PremMembershipBoxClear();
                    MemberLocationAndColor();
                }
                catch (MySqlException ex)
                {
                    // Handle MySQL database exception
                    MessageBox.Show("MySQL Error: " + ex.Message, "Creating Premium Account Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    // No need to close the connection here as it is in a using statement
                }



            }
        }
        private void PremMembershipBoxClear()
        {
            PremFirstNameText.Text = "";
            PremLastNameText.Text = "";
            PremAgeText.Text = "";
            PremGenderComboText.SelectedIndex = -1;
            PremCPNumText.Text = "";
            PremEmailText.Text = "";
            PremMemberIDText.Text = "";
            PremPassText.Text = "";
            PremConfirmPassText.Text = "";
            PremPassText.UseSystemPasswordChar = true;
            PremConfirmPassText.UseSystemPasswordChar = true;
            PremBdayPicker.Value = DateTime.Now;
            PremCardNameText.Text = "";
            PremPlanPeriodText.Text = "";
            PremPaymentTypeText.Text = "";
            PremCardNumText.Text = "";
            PremCardExpireText.Text = "";
            PremCardCVCText.Text = "";
            PremPlanExpirationText.Text = "";
            PremNewPriceText.Text = "";

        }
        #endregion

        #region Customer Member Dashboard Starts Here
        private void MemberAccUserBtn_Click(object sender, EventArgs e)
        {
            if (MemberUserAccPanel.Visible == false)
            {
                MemberUserAccPanel.Visible = true;

            }
            else
            {
                MemberUserAccPanel.Visible = false;
            }
        }
        private void MemberSignOut_Click(object sender, EventArgs e)
        {
            LogoutChecker();
            MemApptTransactionClear();
            //// Check if positions have been set already during logout
            //if (MemberAccInfoPersonalTypeText.Text == "Regular" && RecApptAnyStaffToggleSwitch.Visible)
            //{
            //    // No need to set positions again if already visible
            //    return;
            //}
            //else if (MemberAccInfoPersonalTypeText.Text == "PREMIUM" && RecApptAnyStaffToggleSwitch.Visible)
            //{
            //    // No need to set positions again if already visible
            //    return;
            //}
            //else if (MemberAccInfoPersonalTypeText.Text == "SVIP" && RecApptAnyStaffToggleSwitch.Visible)
            //{
            //    // No need to set positions again if already visible
            //    return;
            //}

            //// Your existing code for setting positions during logout
            //if (MemberAccInfoPersonalTypeText.Text == "Regular")
            //{
            //    RecApptAnyStaffToggleSwitch.Location = new System.Drawing.Point(749, 885);
            //    RecApptAnyStaffLbl.Location = new System.Drawing.Point(806, 885);
            //    RecApptAnyStaffToggleSwitch.Visible = true;
            //    RecApptAnyStaffLbl.Visible = true;
            //    RecApptPreferredStaffLbl.Visible = false;
            //    RecApptPreferredStaffToggleSwitch.Visible = false;
            //}
            //else if (MemberAccInfoPersonalTypeText.Text == "PREMIUM")
            //{
            //    RecApptAnyStaffToggleSwitch.Location = new System.Drawing.Point(533, 885);
            //    RecApptAnyStaffLbl.Location = new System.Drawing.Point(590, 885);
            //    RecApptAnyStaffToggleSwitch.Visible = true;
            //    RecApptAnyStaffLbl.Visible = true;
            //    RecApptPreferredStaffLbl.Visible = true;
            //    RecApptPreferredStaffToggleSwitch.Visible = true;
            //}
            //else if (MemberAccInfoPersonalTypeText.Text == "SVIP")
            //{
            //    RecApptAnyStaffToggleSwitch.Location = new System.Drawing.Point(533, 885);
            //    RecApptAnyStaffLbl.Location = new System.Drawing.Point(590, 885);
            //    RecApptAnyStaffToggleSwitch.Visible = true;
            //    RecApptAnyStaffLbl.Visible = true;
            //    RecApptPreferredStaffLbl.Visible = true;
            //    RecApptPreferredStaffToggleSwitch.Visible = true;
            //}
        }


        #endregion

        #region Staff Team 
        private void TeamHS1_MouseHover(object sender, EventArgs e)
        {
            string message = "Angela Cruz\n";
            message += "A junior hair stylist in ENCHANTÉ Salon\n";

            iconToolTip.SetToolTip(TeamHS1, message);
            TeamHS1.Text = "Angela Cruz";
        }

        private void TeamHS2_MouseHover(object sender, EventArgs e)
        {
            string message = "Maria Santos\n";
            message += "An assistant hair stylist in ENCHANTÉ Salon\n";

            iconToolTip.SetToolTip(TeamHS2, message);
            TeamHS2.Text = "Maria Santos";
        }

        private void TeamHS3_MouseHover(object sender, EventArgs e)
        {
            string message = "Rhydel Estrada\n";
            message += "A senior hair stylist in ENCHANTÉ Salon\n";

            iconToolTip.SetToolTip(TeamHS3, message);
            TeamHS3.Text = "Rhydel Estrada";
        }

        private void TeamFS1_MouseHover(object sender, EventArgs e)
        {
            string message = "Juan Dela Cruz\n";
            message += "A senior aesthetician in ENCHANTÉ Salon\n";

            iconToolTip.SetToolTip(TeamFS1, message);
            TeamFS1.Text = "Juan Dela Cruz";
        }

        private void TeamFS2_MouseHover(object sender, EventArgs e)
        {
            string message = "Katrina Reyes\n";
            message += "A nurse aesthetician in ENCHANTÉ Salon\n";

            iconToolTip.SetToolTip(TeamFS2, message);
            TeamFS2.Text = "Katrina Reyes";
        }

        private void TeamFS3_MouseHover(object sender, EventArgs e)
        {
            string message = "Miguel Fernandez\n";
            message += "A junior aesthetician in ENCHANTÉ Salon\n";

            iconToolTip.SetToolTip(TeamFS3, message);
            TeamFS3.Text = "Miguel Fernandez";
        }

        private void TeamNC1_MouseHover(object sender, EventArgs e)
        {
            string message = "Carlos Gonzales\n";
            message += "An assistant nail technician in ENCHANTÉ Salon\n";

            iconToolTip.SetToolTip(TeamNC1, message);
            TeamNC1.Text = "Carlos Gonzales";
        }

        private void TeamNC2_MouseHover(object sender, EventArgs e)
        {
            string message = "Andrea Villanueva\n";
            message += "An junior nail technician in ENCHANTÉ Salon\n";

            iconToolTip.SetToolTip(TeamNC2, message);
            TeamNC2.Text = "Andrea Villanueva";
        }

        private void TeamNC3_MouseHover(object sender, EventArgs e)
        {
            string message = "Sarah Lim\n";
            message += "A senior nail technician in ENCHANTÉ Salon\n";

            iconToolTip.SetToolTip(TeamNC3, message);
            TeamNC3.Text = "Sarah Lim";
        }

        private void TeamSPA1_MouseHover(object sender, EventArgs e)
        {
            string message = "Luis Cruz\n";
            message += "A senior spa therapist in ENCHANTÉ Salon\n";

            iconToolTip.SetToolTip(TeamSPA1, message);
            TeamSPA1.Text = "Luis Cruz";
        }

        private void TeamSPA2_MouseHover(object sender, EventArgs e)
        {
            string message = "Maricel Santos\n";
            message += "An assistant spa therapist in ENCHANTÉ Salon\n";

            iconToolTip.SetToolTip(TeamSPA2, message);
            TeamSPA2.Text = "Maricel Santos";
        }

        private void TeamSPA3_MouseHover(object sender, EventArgs e)
        {
            string message = "Rafael Garcia\n";
            message += "A junior spa therapist in ENCHANTÉ Salon\n";

            iconToolTip.SetToolTip(TeamSPA3, message);
            TeamSPA3.Text = "Rafael Garcia";
        }

        private void TeamMSG1_MouseHover(object sender, EventArgs e)
        {
            string message = "Antonio Reyes\n";
            message += "A junior massage therapist in ENCHANTÉ Salon\n";

            iconToolTip.SetToolTip(TeamMSG1, message);
            TeamMSG1.Text = "Antonio Reyes";
        }

        private void TeamMSG2_MouseHover(object sender, EventArgs e)
        {
            string message = "Jasmine Castro\n";
            message += "An assistant massage therapist in ENCHANTÉ Salon\n";

            iconToolTip.SetToolTip(TeamMSG2, message);
            TeamMSG2.Text = "Jasmine Castro";
        }

        private void TeamMSG3_MouseHover(object sender, EventArgs e)
        {
            string message = "Sofia Ramirez\n";
            message += "A senior massage therapist in ENCHANTÉ Salon\n";

            iconToolTip.SetToolTip(TeamMSG3, message);
            TeamMSG3.Text = "Sofia Ramirez";
        }

        private void TeamHS1_MouseLeave(object sender, EventArgs e)
        {
            StaffNameClear();
        }

        private void TeamHS2_MouseLeave(object sender, EventArgs e)
        {
            StaffNameClear();
        }

        private void TeamHS3_MouseLeave(object sender, EventArgs e)
        {
            StaffNameClear();
        }

        private void TeamFS1_MouseLeave(object sender, EventArgs e)
        {
            StaffNameClear();
        }

        private void TeamFS2_MouseLeave(object sender, EventArgs e)
        {
            StaffNameClear();
        }

        private void TeamFS3_MouseLeave(object sender, EventArgs e)
        {
            StaffNameClear();
        }

        private void TeamNC1_MouseLeave(object sender, EventArgs e)
        {
            StaffNameClear();
        }

        private void TeamNC2_MouseLeave(object sender, EventArgs e)
        {
            StaffNameClear();
        }

        private void TeamNC3_MouseLeave(object sender, EventArgs e)
        {
            StaffNameClear();
        }

        private void TeamSPA1_MouseLeave(object sender, EventArgs e)
        {
            StaffNameClear();
        }

        private void TeamSPA2_MouseLeave(object sender, EventArgs e)
        {
            StaffNameClear();
        }

        private void TeamSPA3_MouseLeave(object sender, EventArgs e)
        {
            StaffNameClear();
        }

        private void TeamMSG1_MouseLeave(object sender, EventArgs e)
        {
            StaffNameClear();
        }

        private void TeamMSG2_MouseLeave(object sender, EventArgs e)
        {
            StaffNameClear();
        }

        private void TeamMSG3_MouseLeave(object sender, EventArgs e)
        {
            StaffNameClear();
        }

        private void StaffNameClear()
        {
            TeamHS1.Text = "";
            TeamHS2.Text = "";
            TeamHS3.Text = "";
            TeamFS1.Text = "";
            TeamFS2.Text = "";
            TeamFS3.Text = "";
            TeamNC1.Text = "";
            TeamNC2.Text = "";
            TeamNC3.Text = "";
            TeamSPA1.Text = "";
            TeamSPA2.Text = "";
            TeamSPA3.Text = "";
            TeamMSG1.Text = "";
            TeamMSG2.Text = "";
            TeamMSG3.Text = "";
        }



        #endregion

        #region About US
        private void PictureSlideTimer_Tick(object sender, EventArgs e)
        {

            DisplayNextImage();

        }

        private void DisplayNextImage()
        {
            // Load the next image
            System.Drawing.Image image = images[currentIndex];
            AbtUsPictureBox.Image = image;
            EDP1.Image = image;

            // Increment the index, looping back to the beginning if necessary
            currentIndex = (currentIndex + 1) % images.Length;
        }


        #endregion

        #region Member Panel Starts Here

        #endregion

        private void MemberNameLbl_Click(object sender, EventArgs e)
        {
            Member.PanelShow(MemAccInfoPanel);
            MemberUserAccPanel.Visible = false;
            MemberAccInfoPersonalPassText.PasswordChar = '*';
        }

        private void MemberHomeBtn_Click(object sender, EventArgs e)
        {
            Member.PanelShow(MemAccHomePanel);
            
        }
        private bool positionsSetForRegularBtn = false;
        private bool positionsSetForPremiumBtn = false;
        private bool positionsSetForSVIPBtn = false;
        private void MemberAppointBtn_Click(object sender, EventArgs e)
        {
            Member.PanelShow(MemAccApptPanel);
            MemApptTransactionClear();
            LoadBookingTimes();
            RecApptBookingDatePicker.MinDate = DateTime.Today;
            isappointment = true;

            

            if (MemberAccInfoPersonalTypeText.Text == "Regular")
            {
                RecApptTransNumText.Text = TransactionNumberGenerator.RegAppointGenerateTransNumberDefault();

                // Check if positions have been set already
                if (!positionsSetForRegularBtn)
                {
                    RecApptAnyStaffToggleSwitch.Visible = true;
                    RecApptAnyStaffLbl.Visible = true;
                    RecApptPreferredStaffLbl.Visible = false;
                    RecApptPreferredStaffToggleSwitch.Visible = false;
                    RecApptAnyStaffLbl.Visible = false;
                    RecApptAnyStaffToggleSwitch.Visible = false;
                    RecApptAnyStaffLblRegular.Visible = true;
                    RecApptAnyStaffToggleSwitchRegular.Visible = true;

                    positionsSetForRegularBtn = true;
                }
            }
            else if (MemberAccInfoPersonalTypeText.Text == "PREMIUM")
            {
                RecApptTransNumText.Text = TransactionNumberGenerator.PremAppointGenerateTransNumberDefault();

                // Check if positions have been set already
                if (!positionsSetForPremiumBtn)
                {
                    RecApptAnyStaffToggleSwitch.Location = new System.Drawing.Point(533, 885);
                    RecApptAnyStaffLbl.Location = new System.Drawing.Point(590, 885);
                    RecApptAnyStaffToggleSwitch.Visible = true;
                    RecApptAnyStaffLbl.Visible = true;
                    RecApptPreferredStaffLbl.Visible = true;
                    RecApptPreferredStaffToggleSwitch.Visible = true;
                    RecApptAnyStaffLbl.Visible = true;
                    RecApptAnyStaffToggleSwitch.Visible = true;
                    RecApptAnyStaffLblRegular.Visible = false;
                    RecApptAnyStaffToggleSwitchRegular.Visible = false;
                    // Set the flag to indicate positions have been set
                    positionsSetForPremiumBtn = true;
                }
            }
            else if (MemberAccInfoPersonalTypeText.Text == "SVIP")
            {
                RecApptTransNumText.Text = TransactionNumberGenerator.SVIPAppointGenerateTransNumberDefault();

                // Check if positions have been set already
                if (!positionsSetForSVIPBtn)
                {
                    RecApptAnyStaffToggleSwitch.Location = new System.Drawing.Point(533, 885);
                    RecApptAnyStaffLbl.Location = new System.Drawing.Point(590, 885);
                    RecApptAnyStaffToggleSwitch.Visible = true;
                    RecApptAnyStaffLbl.Visible = true;
                    RecApptPreferredStaffLbl.Visible = true;
                    RecApptPreferredStaffToggleSwitch.Visible = true;
                    RecApptAnyStaffLbl.Visible = true;
                    RecApptAnyStaffToggleSwitch.Visible = true;
                    RecApptAnyStaffLblRegular.Visible = false;
                    RecApptAnyStaffToggleSwitchRegular.Visible = false;

                    // Set the flag to indicate positions have been set
                    positionsSetForSVIPBtn = true;
                }
            }
        }
        private void ToggleBtnLocatory()
        {
            // Define boolean flags to track if positions have been set
            
        }
        #region Receptionsit Walk-in Appointment

        //ApptMember
        private void RecApptPanelExitBtn_Click(object sender, EventArgs e)
        {
            Member.PanelShow(MemAccHomePanel);
        }

        //ApptMember
        string[] bookingTimes = new string[]
        {
            "Select a booking time", "08:00 am", "08:30 am", "09:00 am",
            "09:30 am", "10:00 am", "10:30 am", "11:00 am", "11:30 am",
            "01:00 pm", "01:30 pm", "02:00 pm", "02:30 pm", "03:00 pm",
        };

        //ApptMember
        private void RecApptCatHSBtn_Click(object sender, EventArgs e)
        {
            filterstaffbyservicecategory = "Hair Styling";
            haschosenacategory = true;
            if (RecApptPreferredStaffToggleSwitch.Checked == true)
            {
                RecApptAvailableAttendingStaffSelectedComboBox.Items.Clear();
                LoadAppointmentPreferredStaffComboBox();
            }
            LoadBookingTimes();
            RecApptHairStyle();
        }

        //ApptMember
        private void RecApptCatFSBtn_Click(object sender, EventArgs e)
        {
            filterstaffbyservicecategory = "Face & Skin";
            haschosenacategory = true;
            if (RecApptPreferredStaffToggleSwitch.Checked == true)
            {
                RecApptAvailableAttendingStaffSelectedComboBox.Items.Clear();
                LoadAppointmentPreferredStaffComboBox();
            }
            LoadBookingTimes();
            RecApptFace();
        }

        //ApptMember
        private void RecApptCatNCBtn_Click(object sender, EventArgs e)
        {
            filterstaffbyservicecategory = "Nail Care";
            haschosenacategory = true;
            if (RecApptPreferredStaffToggleSwitch.Checked == true)
            {
                RecApptAvailableAttendingStaffSelectedComboBox.Items.Clear();
                LoadAppointmentPreferredStaffComboBox();
            }
            LoadBookingTimes();
            RecApptNail();
        }

        //ApptMember
        private void RecApptCatSpaBtn_Click(object sender, EventArgs e)
        {
            filterstaffbyservicecategory = "Spa";
            haschosenacategory = true;
            if (RecApptPreferredStaffToggleSwitch.Checked == true)
            {
                RecApptAvailableAttendingStaffSelectedComboBox.Items.Clear();
                LoadAppointmentPreferredStaffComboBox();
            }
            LoadBookingTimes();
            RecApptSpa();
        }

        //ApptMember
        private void RecApptCatMassBtn_Click(object sender, EventArgs e)
        {
            filterstaffbyservicecategory = "Massage";
            haschosenacategory = true;
            if (RecApptPreferredStaffToggleSwitch.Checked == true)
            {
                RecApptAvailableAttendingStaffSelectedComboBox.Items.Clear();
                LoadAppointmentPreferredStaffComboBox();
            }
            LoadBookingTimes();
            RecApptMassage();
        }

        //ApptMember
        private void RecApptHairStyle()
        {
            if (RecApptCatHSRB.Checked == false)
            {
                RecApptCatHSRB.Visible = true;
                RecApptCatHSRB.Checked = true;
                RecApptLoadServiceTypeComboBox("Hair Styling");

                RecApptCatFSRB.Visible = false;
                RecApptCatNCRB.Visible = false;
                RecApptCatSpaRB.Visible = false;
                RecApptCatMassRB.Visible = false;
                RecApptCatFSRB.Checked = false;
                RecApptCatNCRB.Checked = false;
                RecApptCatSpaRB.Checked = false;
                RecApptCatMassRB.Checked = false;
                return;
            }
            else if (RecApptCatHSRB.Checked == true)
            {
                RecApptCatHSRB.Visible = true;
                RecApptCatHSRB.Checked = true;
                RecApptLoadServiceTypeComboBox("Hair Styling");

                RecApptCatFSRB.Visible = false;
                RecApptCatNCRB.Visible = false;
                RecApptCatSpaRB.Visible = false;
                RecApptCatMassRB.Visible = false;
                RecApptCatFSRB.Checked = false;
                RecApptCatNCRB.Checked = false;
                RecApptCatSpaRB.Checked = false;
                RecApptCatMassRB.Checked = false;
            }
        }
        //ApptMember
        private void RecApptFace()
        {
            if (RecApptCatFSRB.Checked == false)
            {
                RecApptCatFSRB.Visible = true;
                RecApptCatFSRB.Checked = true;
                RecApptLoadServiceTypeComboBox("Face & Skin");

                RecApptCatHSRB.Visible = false;
                RecApptCatNCRB.Visible = false;
                RecApptCatSpaRB.Visible = false;
                RecApptCatMassRB.Visible = false;
                RecApptCatHSRB.Checked = false;
                RecApptCatNCRB.Checked = false;
                RecApptCatSpaRB.Checked = false;
                RecApptCatMassRB.Checked = false;
                return;
            }
            else if (RecApptCatFSRB.Checked == true)
            {
                RecApptCatFSRB.Visible = true;
                RecApptCatFSRB.Checked = true;
            }
        }
        //ApptMember
        private void RecApptNail()
        {
            if (RecApptCatNCRB.Checked == false)
            {
                RecApptCatNCRB.Visible = true;
                RecApptCatNCRB.Checked = true;
                RecApptLoadServiceTypeComboBox("Nail Care");

                RecApptCatHSRB.Visible = false;
                RecApptCatFSRB.Visible = false;
                RecApptCatSpaRB.Visible = false;
                RecApptCatMassRB.Visible = false;
                RecApptCatHSRB.Checked = false;
                RecApptCatFSRB.Checked = false;
                RecApptCatSpaRB.Checked = false;
                RecApptCatMassRB.Checked = false;
                return;
            }
            else if (RecApptCatNCRB.Checked == true)
            {
                RecApptCatNCRB.Visible = true;
                RecApptCatNCRB.Checked = true;
            }
        }
        //ApptMember
        private void RecApptSpa()
        {
            if (RecApptCatSpaRB.Checked == false)
            {
                RecApptCatSpaRB.Visible = true;
                RecApptCatSpaRB.Checked = true;
                RecApptLoadServiceTypeComboBox("Spa");

                RecApptCatHSRB.Visible = false;
                RecApptCatFSRB.Visible = false;
                RecApptCatNCRB.Visible = false;
                RecApptCatMassRB.Visible = false;
                RecApptCatHSRB.Checked = false;
                RecApptCatFSRB.Checked = false;
                RecApptCatNCRB.Checked = false;
                RecApptCatMassRB.Checked = false;
                return;
            }
            else if (RecApptCatSpaRB.Checked == true)
            {
                RecApptCatSpaRB.Visible = true;
                RecApptCatSpaRB.Checked = true;
            }
        }
        //ApptMember
        private void RecApptMassage()
        {
            if (RecApptCatMassRB.Checked == false)
            {
                RecApptCatMassRB.Visible = true;
                RecApptCatMassRB.Checked = true;
                RecApptLoadServiceTypeComboBox("Massage");

                RecApptCatHSRB.Visible = false;
                RecApptCatFSRB.Visible = false;
                RecApptCatNCRB.Visible = false;
                RecApptCatSpaRB.Visible = false;
                RecApptCatHSRB.Checked = false;
                RecApptCatFSRB.Checked = false;
                RecApptCatNCRB.Checked = false;
                RecApptCatSpaRB.Checked = false;
                return;
            }
            else if (RecApptCatMassRB.Checked == true)
            {
                RecApptCatMassRB.Visible = true;
                RecApptCatMassRB.Checked = true;
            }
        }

        //ApptMember
        public void RecApptLoadHairStyleType()
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(mysqlconn))
                {
                    connection.Open();

                    // Filter and sort the data by FoodType
                    string sql = "SELECT * FROM `services` WHERE Category = 'Hair Styling' ORDER BY Category";
                    MySqlCommand cmd = new MySqlCommand(sql, connection);
                    System.Data.DataTable dataTable = new System.Data.DataTable();

                    using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd))
                    {
                        adapter.Fill(dataTable);

                        RecApptServiceTypeDGV.Columns.Clear();


                        RecApptServiceTypeDGV.DataSource = dataTable;

                        RecApptServiceTypeDGV.Columns[0].Visible = false; //service category
                        RecApptServiceTypeDGV.Columns[1].Visible = false; // service type
                        RecApptServiceTypeDGV.Columns[2].Visible = false; // service ID
                        RecApptServiceTypeDGV.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                        RecApptServiceTypeDGV.ClearSelection();
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("An error occurred: " + e.Message, "Cashier Burger Item List");
            }
            finally
            {
                connection.Close();
            }
        }
        public void RecApptFaceSkinType()
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(mysqlconn))
                {
                    connection.Open();

                    // Filter and sort the data by FoodType
                    string sql = "SELECT * FROM `services` WHERE Category = 'Face & Skin' ORDER BY Category";
                    MySqlCommand cmd = new MySqlCommand(sql, connection);
                    System.Data.DataTable dataTable = new System.Data.DataTable();

                    using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd))
                    {
                        adapter.Fill(dataTable);

                        RecApptServiceTypeDGV.Columns.Clear();


                        RecApptServiceTypeDGV.DataSource = dataTable;

                        RecApptServiceTypeDGV.Columns[0].Visible = false;
                        RecApptServiceTypeDGV.Columns[1].Visible = false;
                        RecApptServiceTypeDGV.Columns[2].Visible = false;
                        RecApptServiceTypeDGV.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                        RecApptServiceTypeDGV.ClearSelection();
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("An error occurred: " + e.Message, "Cashier Burger Item List");
            }
            finally
            {
                connection.Close();
            }
        }

        //ApptMember
        public void RecApptNailCareType()
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(mysqlconn))
                {
                    connection.Open();

                    // Filter and sort the data by FoodType
                    string sql = "SELECT * FROM `services` WHERE Category = 'Nail Care' ORDER BY Category";
                    MySqlCommand cmd = new MySqlCommand(sql, connection);
                    System.Data.DataTable dataTable = new System.Data.DataTable();

                    using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd))
                    {
                        adapter.Fill(dataTable);

                        RecApptServiceTypeDGV.Columns.Clear();


                        RecApptServiceTypeDGV.DataSource = dataTable;

                        RecApptServiceTypeDGV.Columns[0].Visible = false;
                        RecApptServiceTypeDGV.Columns[1].Visible = false;
                        RecApptServiceTypeDGV.Columns[2].Visible = false;
                        RecApptServiceTypeDGV.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                        RecApptServiceTypeDGV.ClearSelection();
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("An error occurred: " + e.Message, "Cashier Burger Item List");
            }
            finally
            {
                connection.Close();
            }
        }

        //ApptMember
        public void RecApptSpaType()
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(mysqlconn))
                {
                    connection.Open();

                    // Filter and sort the data by FoodType
                    string sql = "SELECT * FROM `services` WHERE Category = 'Spa' ORDER BY Category";
                    MySqlCommand cmd = new MySqlCommand(sql, connection);
                    System.Data.DataTable dataTable = new System.Data.DataTable();

                    using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd))
                    {
                        adapter.Fill(dataTable);

                        RecApptServiceTypeDGV.Columns.Clear();


                        RecApptServiceTypeDGV.DataSource = dataTable;

                        RecApptServiceTypeDGV.Columns[0].Visible = false;
                        RecApptServiceTypeDGV.Columns[1].Visible = false;
                        RecApptServiceTypeDGV.Columns[2].Visible = false;
                        RecApptServiceTypeDGV.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                        RecApptServiceTypeDGV.ClearSelection();
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("An error occurred: " + e.Message, "Cashier Burger Item List");
            }
            finally
            {
                connection.Close();
            }
        }

        //ApptMember
        public void RecApptMassageType()
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(mysqlconn))
                {
                    connection.Open();

                    // Filter and sort the data by FoodType
                    string sql = "SELECT * FROM `services` WHERE Category = 'Massage' ORDER BY Category";
                    MySqlCommand cmd = new MySqlCommand(sql, connection);
                    System.Data.DataTable dataTable = new System.Data.DataTable();

                    using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd))
                    {
                        adapter.Fill(dataTable);

                        RecApptServiceTypeDGV.Columns.Clear();


                        RecApptServiceTypeDGV.DataSource = dataTable;

                        RecApptServiceTypeDGV.Columns[0].Visible = false;
                        RecApptServiceTypeDGV.Columns[1].Visible = false;
                        RecApptServiceTypeDGV.Columns[2].Visible = false;
                        RecApptServiceTypeDGV.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                        RecApptServiceTypeDGV.ClearSelection();
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("An error occurred: " + e.Message, "Cashier Burger Item List");
            }
            finally
            {
                connection.Close();
            }
        }

        //ApptMember
        private void RecApptLoadServiceTypeComboBox(string selectedCategory)
        {
            // Filter and add the relevant service types based on the selected category
            switch (selectedCategory)
            {
                case "Hair Styling":
                    RecApptLoadHairStyleType();
                    break;
                case "Nail Care":
                    RecApptNailCareType();
                    break;
                case "Face & Skin":
                    RecApptFaceSkinType();
                    break;
                case "Massage":
                    RecApptMassageType();
                    break;
                case "Spa":
                    RecApptSpaType();
                    break;
                default:
                    break;
            }

        }

        //ApptMember
        

        private void RecApptServiceTypeDGV_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            RecApptAddService();
        }

        //ApptMember
        private void RecApptSelectServiceAndStaffBtn_Click(object sender, EventArgs e)
        {
            RecApptAddService();
        }

        //ApptMember
        private void RecApptAnyStaffToggleSwitch_CheckedChanged(object sender, EventArgs e)
        {
            
            //if (MemberAccInfoPersonalTypeText.Text == "Regular")
            //{

            //    if (haschosenacategory == false)
            //    {
            //        ShowNoServiceCategoryChosenWarningMessage();
            //        RecApptAnyStaffToggleSwitch.CheckedChanged -= RecApptAnyStaffToggleSwitch_CheckedChanged;
            //        RecApptAnyStaffToggleSwitch.Checked = false;
            //        RecApptAttendingStaffLbl.Visible = false;
            //        RecApptAvailableAttendingStaffSelectedComboBox.Visible = false;
            //        RecApptAnyStaffToggleSwitch.CheckedChanged += RecApptAnyStaffToggleSwitch_CheckedChanged;
            //        return;
            //    }
            //    else
            //    {
            //        if (RecApptAnyStaffToggleSwitch.Checked)
            //        {
            //            RecApptPreferredStaffToggleSwitch.Checked = false;
            //            RecApptAvailableAttendingStaffSelectedComboBox.Enabled = false;
            //            RecApptAttendingStaffLbl.Visible = false;
            //            RecApptAvailableAttendingStaffSelectedComboBox.Visible = false;
            //            selectedStaffID = "Anyone";
            //            RecApptAvailableAttendingStaffSelectedComboBox.Items.Clear();
            //            RecApptPreferredStaffLbl.Visible = false;
            //            RecApptPreferredStaffToggleSwitch.Visible = false;
            //        }
            //    }

            //    return;
            //}
            //else
            if (MemberAccInfoPersonalTypeText.Text == "PREMIUM")
            {
                if (haschosenacategory == false)
                {
                    ShowNoServiceCategoryChosenWarningMessage();
                    RecApptAnyStaffToggleSwitch.CheckedChanged -= RecApptAnyStaffToggleSwitch_CheckedChanged;
                    RecApptAnyStaffToggleSwitch.Checked = false;
                    RecApptAttendingStaffLbl.Visible = false;
                    RecApptAvailableAttendingStaffSelectedComboBox.Visible = false;
                    RecApptAnyStaffToggleSwitch.CheckedChanged += RecApptAnyStaffToggleSwitch_CheckedChanged;
                    return;
                }
                else
                {
                    if (RecApptAnyStaffToggleSwitch.Checked)
                    {
                        RecApptPreferredStaffToggleSwitch.Checked = false;
                        RecApptAvailableAttendingStaffSelectedComboBox.Enabled = false;
                        RecApptAttendingStaffLbl.Visible = false;
                        RecApptAvailableAttendingStaffSelectedComboBox.Visible = false;
                        selectedStaffID = "Anyone";
                        RecApptAvailableAttendingStaffSelectedComboBox.Items.Clear();
                        RecApptPreferredStaffLbl.Visible = true;
                        RecApptPreferredStaffToggleSwitch.Visible = true;
                    }
                }
                return;
            }
            else if (MemberAccInfoPersonalTypeText.Text == "SVIP")
            {
                if (haschosenacategory == false)
                {
                    ShowNoServiceCategoryChosenWarningMessage();
                    RecApptAnyStaffToggleSwitch.CheckedChanged -= RecApptAnyStaffToggleSwitch_CheckedChanged;
                    RecApptAnyStaffToggleSwitch.Checked = false;
                    RecApptAttendingStaffLbl.Visible = false;
                    RecApptAvailableAttendingStaffSelectedComboBox.Visible = false;
                    RecApptAnyStaffToggleSwitch.CheckedChanged += RecApptAnyStaffToggleSwitch_CheckedChanged;
                    return;
                }
                else
                {
                    if (RecApptAnyStaffToggleSwitch.Checked)
                    {
                        RecApptPreferredStaffToggleSwitch.Checked = false;
                        RecApptAvailableAttendingStaffSelectedComboBox.Enabled = false;
                        RecApptAttendingStaffLbl.Visible = false;
                        RecApptAvailableAttendingStaffSelectedComboBox.Visible = false;
                        selectedStaffID = "Anyone";
                        RecApptAvailableAttendingStaffSelectedComboBox.Items.Clear();
                    }
                }
                return;

            }
        }
        private void RecApptAnyStaffToggleSwitchRegular_CheckedChanged(object sender, EventArgs e)
        {
            

                if (haschosenacategory == false)
                {
                    ShowNoServiceCategoryChosenWarningMessage();
                    RecApptAnyStaffToggleSwitch.CheckedChanged -= RecApptAnyStaffToggleSwitchRegular_CheckedChanged;
                    RecApptAnyStaffToggleSwitch.Checked = false;
                    RecApptAttendingStaffLbl.Visible = false;
                    RecApptAvailableAttendingStaffSelectedComboBox.Visible = false;
                    RecApptAnyStaffToggleSwitch.CheckedChanged += RecApptAnyStaffToggleSwitchRegular_CheckedChanged;
                    return;
                }
                else
                {
                    if (RecApptAnyStaffToggleSwitchRegular.Checked)
                    {
                        RecApptPreferredStaffToggleSwitch.Checked = false;
                        RecApptAvailableAttendingStaffSelectedComboBox.Enabled = false;
                        RecApptAttendingStaffLbl.Visible = false;
                        RecApptAvailableAttendingStaffSelectedComboBox.Visible = false;
                        selectedStaffID = "Anyone";
                        RecApptAvailableAttendingStaffSelectedComboBox.Items.Clear();
                    }
                }

                return;
            

        }
        //ApptMember
        private void RecApptPreferredStaffToggleSwitch_CheckedChanged(object sender, EventArgs e)
        {
            if (haschosenacategory == false)
            {
                ShowNoServiceCategoryChosenWarningMessage();
                RecApptPreferredStaffToggleSwitch.CheckedChanged -= RecApptPreferredStaffToggleSwitch_CheckedChanged;
                RecApptPreferredStaffToggleSwitch.Checked = false;
                RecApptAttendingStaffLbl.Visible = false;
                RecApptAvailableAttendingStaffSelectedComboBox.Visible = false;
                RecApptPreferredStaffToggleSwitch.CheckedChanged += RecApptPreferredStaffToggleSwitch_CheckedChanged;
                return;
            }
            else
            {
                if (RecApptPreferredStaffToggleSwitch.Checked && RecApptAvailableAttendingStaffSelectedComboBox.SelectedText != "Select a Preferred Staff")
                {
                    RecApptAnyStaffToggleSwitch.Checked = false;
                    RecApptAvailableAttendingStaffSelectedComboBox.Enabled = true;
                    RecApptAttendingStaffLbl.Visible = true;
                    RecApptAvailableAttendingStaffSelectedComboBox.Visible = true;
                    LoadAppointmentPreferredStaffComboBox();
                }
                else
                {
                    selectedStaffID = "Anyone";
                    RecApptAvailableAttendingStaffSelectedComboBox.Enabled = false;
                    RecApptAttendingStaffLbl.Visible = false;
                    RecApptAvailableAttendingStaffSelectedComboBox.Visible = false;
                    RecApptAvailableAttendingStaffSelectedComboBox.Items.Clear();
                }
            }
        }

        private void RecApptAttendingStaffSelectedComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        private void RecApptAddService()
        {


            if (RecApptServiceTypeDGV.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a service.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (string.IsNullOrEmpty(selectedStaffID))
            {
                MessageBox.Show("Please select a prefered staff or toggle anyone ", "No Selected Staff ID", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (RecApptBookingTimeComboBox.SelectedIndex == 0 || RecApptBookingTimeComboBox.SelectedItem == null || RecApptBookingTimeComboBox.SelectedItem.ToString() == "Cutoff Time")
            {
                MessageBox.Show("Please select a booking time", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            DataGridViewRow selectedRow = RecApptServiceTypeDGV.SelectedRows[0];

            string SelectedCategory = selectedRow.Cells[0].Value.ToString();
            string ServiceID = selectedRow.Cells[2].Value.ToString();
            string ServiceName = selectedRow.Cells[3].Value.ToString();
            string ServicePrice = selectedRow.Cells[6].Value.ToString();
            string ServiceTime = RecApptBookingTimeComboBox.SelectedItem.ToString();
            string serviceID = selectedRow.Cells[2]?.Value?.ToString(); // Use null-conditional operator to avoid NullReferenceException


            if (RecApptServiceTypeDGV.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a service.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (selectedRow == null)
            {
                MessageBox.Show("Selected row is null.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (string.IsNullOrEmpty(serviceID))
            {
                MessageBox.Show("Service ID is null or empty.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (RecApptAvailableAttendingStaffSelectedComboBox.SelectedItem?.ToString() == "Select a Preferred Staff") // 4942
            {
                MessageBox.Show("Please select a preferred staff or toggle anyone.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            foreach (DataGridViewRow row in RecApptSelectedServiceDGV.Rows)
            {
                string existingServiceID = row.Cells["RecApptServiceID"]?.Value?.ToString(); // Use null-conditional operator

                if (serviceID == existingServiceID)
                {
                    MessageBox.Show("This service is already selected.", "Duplicate Service", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }



            DialogResult result = MessageBox.Show("Are you sure you want to add this service?", "Confirm Service Selection", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                // Add the row
                DataGridViewRow NewSelectedServiceRow = RecApptSelectedServiceDGV.Rows[RecApptSelectedServiceDGV.Rows.Add()];

                string appointmentDate = RecApptBookingDatePicker.Value.ToString("MM-dd-yyyy dddd");
                string serviceCategory = SelectedCategory;
                int latestprioritynumber = GetLargestPriorityNum(appointmentDate, serviceCategory);

                NewSelectedServiceRow.Cells["RecApptServicePrice"].Value = ServicePrice;
                NewSelectedServiceRow.Cells["RecApptServiceCategory"].Value = SelectedCategory;
                NewSelectedServiceRow.Cells["RecApptSelectedService"].Value = ServiceName;
                NewSelectedServiceRow.Cells["RecApptServiceID"].Value = ServiceID;
                NewSelectedServiceRow.Cells["RecApptTimeSelected"].Value = ServiceTime;
                NewSelectedServiceRow.Cells["RecApptPriorityNumber"].Value = latestprioritynumber;
                NewSelectedServiceRow.Cells["RecApptStaffSelected"].Value = selectedStaffID;
                QueTypeIdentifier(NewSelectedServiceRow.Cells["RecApptQueType"]);

                RecApptServiceTypeDGV.ClearSelection();

            }
        }
        //ApptMember
        private void RecApptDeleteSelectedServiceAndStaffBtn_Click(object sender, EventArgs e)
        {
            if (RecApptSelectedServiceDGV.SelectedRows.Count > 0)
            {
                DialogResult result = MessageBox.Show("Are you sure you want to delete this row?", "Confirm Deletion", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    DataGridViewRow selectedRow = RecApptSelectedServiceDGV.SelectedRows[0];
                    RecApptSelectedServiceDGV.Rows.Remove(selectedRow);
                }
            }
        }


        //ApptMember
        private void RecApptBookTransactBtn_Click(object sender, EventArgs e)
        {
            DateTime selectedDate = RecApptBookingDatePicker.Value.Date;
            DateTime currentDate = DateTime.Today;
            if (RecApptBookingTimeComboBox.SelectedItem == null || RecApptBookingTimeComboBox.SelectedIndex == 0)
            {
                MessageBox.Show("Please select a booking time.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (RecApptSelectedServiceDGV != null && RecApptSelectedServiceDGV.Rows.Count == 0)
            {
                MessageBox.Show("Select a service first to proceed on booking a transaction.", "Ooooops!", MessageBoxButtons.OK, MessageBoxIcon.Hand);
            }
            else
            {
                RecApptServiceHistoryDB(RecApptSelectedServiceDGV); //service history db
                ReceptionistAppointmentDB(); //appointment transaction db
                RecApptTransactNumRefresh();
                MemApptTransactionClear();            }
        }

        //ApptMember
        private void RecApptServiceHistoryDB(DataGridView RecApptSelectedServiceDGV)
        {
            DateTime pickedDate = RecApptBookingDatePicker.Value;
            string transactionNum = RecApptTransNumText.Text;
            string transactionType = "Walk-in Appointment Transaction";
            string serviceStatus = "Pending";

            //booked values
            string bookedDate = pickedDate.ToString("MM-dd-yyyy dddd"); //bookedDate

            //basic info
            string CustomerName = MemberAccInfoPersonalNameText.Text; //client name

            if (RecApptSelectedServiceDGV.Rows.Count > 0)
            {
                try
                {
                    using (MySqlConnection connection = new MySqlConnection(mysqlconn))
                    {
                        connection.Open();

                        foreach (DataGridViewRow row in RecApptSelectedServiceDGV.Rows)
                        {
                            if (row.Cells["RecApptSelectedService"].Value != null)
                            {
                                string serviceName = row.Cells["RecApptSelectedService"].Value.ToString();
                                string serviceCat = row.Cells["RecApptServiceCategory"].Value.ToString();
                                string serviceID = row.Cells["RecApptServiceID"].Value.ToString();
                                decimal servicePrice = Convert.ToDecimal(row.Cells["RecApptServicePrice"].Value);
                                string selectedStaff = row.Cells["RecApptStaffSelected"].Value.ToString();
                                string quepriorityNumber = row.Cells["RecApptPriorityNumber"].Value.ToString();
                                string queType = row.Cells["RecApptQueType"].Value.ToString();
                                string bookedTime = row.Cells["RecApptTimeSelected"].Value.ToString();

                                string insertQuery = "INSERT INTO servicehistory (TransactionNumber, TransactionType, ServiceStatus, AppointmentDate, AppointmentTime, ClientName, " +
                                                     "ServiceCategory, ServiceID, SelectedService, ServicePrice, PreferredStaff, PriorityNumber," +
                                                     "QueType" +
                                                     ") VALUES (@Transact, @TransactType, @status, @appointDate, @appointTime, @name, @serviceCat, @ID, @serviceName, @servicePrice, " +
                                                     "@preferredstaff, @queprioritynumber, @quetype)";

                                MySqlCommand cmd = new MySqlCommand(insertQuery, connection);
                                cmd.Parameters.AddWithValue("@Transact", transactionNum);
                                cmd.Parameters.AddWithValue("@TransactType", transactionType);
                                cmd.Parameters.AddWithValue("@status", serviceStatus);
                                cmd.Parameters.AddWithValue("@appointDate", bookedDate);
                                cmd.Parameters.AddWithValue("@appointTime", bookedTime);
                                cmd.Parameters.AddWithValue("@name", CustomerName);
                                cmd.Parameters.AddWithValue("@serviceCat", serviceCat);
                                cmd.Parameters.AddWithValue("@ID", serviceID);
                                cmd.Parameters.AddWithValue("@serviceName", serviceName);
                                cmd.Parameters.AddWithValue("@servicePrice", servicePrice);
                                cmd.Parameters.AddWithValue("@preferredstaff", selectedStaff);
                                cmd.Parameters.AddWithValue("@queprioritynumber", quepriorityNumber);
                                cmd.Parameters.AddWithValue("@quetype", queType);

                                cmd.ExecuteNonQuery();

                                if (selectedStaff != "Anyone")
                                {
                                    string insertScheduleQuery = "INSERT INTO staffappointmentschedule (EmployeeID, AppointmentDate, AppointmentTime,TransactionNumber,ServiceName,ServiceCategory,ServiceID) VALUES (@EmployeeID, @AppointmentDate, @AppointmentTime, @Transact, " +
                                                                 "@serviceName, @serviceCat, @ID )";
                                    MySqlCommand insertScheduleCommand = new MySqlCommand(insertScheduleQuery, connection);
                                    insertScheduleCommand.Parameters.AddWithValue("@EmployeeID", selectedStaff);
                                    insertScheduleCommand.Parameters.AddWithValue("@AppointmentDate", bookedDate);
                                    insertScheduleCommand.Parameters.AddWithValue("@AppointmentTime", bookedTime);
                                    insertScheduleCommand.Parameters.AddWithValue("@Transact", transactionNum);
                                    insertScheduleCommand.Parameters.AddWithValue("@serviceName", serviceName);
                                    insertScheduleCommand.Parameters.AddWithValue("@serviceCat", serviceCat);
                                    insertScheduleCommand.Parameters.AddWithValue("@ID", serviceID);
                                    insertScheduleCommand.ExecuteNonQuery();
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occurred: " + ex.Message, "Receptionist Service failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    connection.Close();
                }
            }
            else
            {
                MessageBox.Show("No items to insert into the database.", "Service");
            }

        }

        //ApptMember
        private void ReceptionistAppointmentDB()
        {
            DateTime appointmentdate = RecApptBookingDatePicker.Value;
            string transactionNum = RecApptTransNumText.Text;
            DateTime currentDate = DateTime.Today;
            string serviceStatus = "Pending";
            string transactType = "Walk-in Appointment";
            string appointmentStatus = "Unconfirmed";

            //basic info
            string CustomerName = MemberAccInfoPersonalNameText.Text; //client name
            string CustomerMobileNumber = MemberAccInfoPersonalCPNumText.Text; //client cp num

            //booked values
            string appointmentbookedDate = appointmentdate.ToString("MM-dd-yyyy dddd"); //bookedDate
            string appointmentbookedTime = RecApptBookingTimeComboBox.SelectedItem?.ToString(); //bookedTime
            string bookedDate = currentDate.ToString("MM-dd-yyyy dddd"); //bookedDate
            string bookedTime = currentDate.ToString("hh:mm tt"); //bookedTime
            string bookedBy = MemberAccInfoPersonalNameText.Text; //booked by


            try
            {
                using (MySqlConnection connection = new MySqlConnection(mysqlconn))
                {
                    connection.Open();
                    string insertQuery = "INSERT INTO appointment (TransactionNumber, TransactionType, ServiceStatus, AppointmentDate, AppointmentTime, AppointmentStatus, " +
                                        "ClientName, ClientCPNum, ServiceDuration, BookedBy, BookedDate, BookedTime)" +
                                        "VALUES (@Transact, @TransactType, @status, @appointDate, @appointTime, @appointStatus, @clientName, @clientCP, @duration, @bookedBy, @bookedDate, @bookedTime)";

                    MySqlCommand cmd = new MySqlCommand(insertQuery, connection);
                    cmd.Parameters.AddWithValue("@Transact", transactionNum);
                    cmd.Parameters.AddWithValue("@TransactType", transactType);
                    cmd.Parameters.AddWithValue("@status", serviceStatus);
                    cmd.Parameters.AddWithValue("@appointDate", appointmentbookedDate);
                    cmd.Parameters.AddWithValue("@appointTime", appointmentbookedTime);
                    cmd.Parameters.AddWithValue("@appointStatus", appointmentStatus);
                    cmd.Parameters.AddWithValue("@clientName", CustomerName);
                    cmd.Parameters.AddWithValue("@clientCP", CustomerMobileNumber);
                    cmd.Parameters.AddWithValue("@duration", "00:00:00");
                    cmd.Parameters.AddWithValue("@bookedBy", bookedBy);
                    cmd.Parameters.AddWithValue("@bookedDate", bookedDate);
                    cmd.Parameters.AddWithValue("@bookedTime", bookedTime);


                    cmd.ExecuteNonQuery();
                }
                MessageBox.Show("Service successfully booked.", "Hooray!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Member.PanelShow(MemAccHomePanel);
                //RecWalkinServiceHistoryDB();
            }
            catch (MySqlException ex)
            {
                // Handle MySQL database exception
                MessageBox.Show("An error occurred: " + ex.Message, "Manager booked transaction failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                // Make sure to close the connection
                connection.Close();
            }
        }

        //ApptMember
        private int GetLargestPriorityNum(string appointmentDate, string serviceCategory)
        {
            using (MySqlConnection connection = new MySqlConnection(mysqlconn))
            {
                connection.Open();

                using (MySqlCommand command = connection.CreateCommand())
                {
                    string query = "SELECT MAX(CAST(PriorityNumber AS UNSIGNED)) FROM servicehistory WHERE AppointmentDate = @AppointmentDate AND ServiceCategory = @ServiceCategory";
                    command.CommandText = query;

                    command.Parameters.AddWithValue("@AppointmentDate", appointmentDate);
                    command.Parameters.AddWithValue("@ServiceCategory", serviceCategory);

                    object result = command.ExecuteScalar();
                    int latestprioritynumber = result != DBNull.Value ? Convert.ToInt32(result) : 0;

                    if (latestprioritynumber > 0)
                    {
                        latestprioritynumber++;
                    }
                    else
                    {
                        latestprioritynumber = 1;
                    }

                    return latestprioritynumber;
                }
            }
        }


        //ApptMember
        private void RecApptTransactNumRefresh()
        {
            if (MemberAccInfoPersonalTypeText.Text == "Regular")
            {
                RecApptTransNumText.Text = TransactionNumberGenerator.RegAppointGenerateTransNumberInc();
                return;
            }
            else if (MemberAccInfoPersonalTypeText.Text == "PREMIUM")
            {
                RecApptTransNumText.Text = TransactionNumberGenerator.PremAppointGenerateTransNumberInc();
                return;
            }
            else if (MemberAccInfoPersonalTypeText.Text == "SVIP")
            {
                RecApptTransNumText.Text = TransactionNumberGenerator.SVIPAppointGenerateTransNumberInc();
                return;
            }
        }
        public class TransactionNumberGenerator
        {
            private static int transactNumber = 1; // Starting order number

            public static string RegAppointGenerateTransNumberDefault()
            {
                string datePart = DateTime.Now.ToString("MMddhhmm");

                string orderPart = transactNumber.ToString("D3");

                string ordersessionNumber = $"R-A-{datePart}-{orderPart}";

                return ordersessionNumber;
            }
            //ApptMember
            public static string RegAppointGenerateTransNumberInc()
            {
                string datePart = DateTime.Now.ToString("MMddhhmm");

                // Use only the order number
                string orderPart = transactNumber.ToString("D3");

                // Increment the order number for the next order
                transactNumber++;
                string ordersessionNumber = $"R-A-{datePart}-{orderPart}";

                return ordersessionNumber;
            }
            public static string PremAppointGenerateTransNumberDefault()
            {
                string datePart = DateTime.Now.ToString("MMddhhmm");

                string orderPart = transactNumber.ToString("D3");

                string ordersessionNumber = $"PREM-A-{datePart}-{orderPart}";

                return ordersessionNumber;
            }
            //ApptMember
            public static string PremAppointGenerateTransNumberInc()
            {
                string datePart = DateTime.Now.ToString("MMddhhmm");

                // Use only the order number
                string orderPart = transactNumber.ToString("D3");

                // Increment the order number for the next order
                transactNumber++;
                string ordersessionNumber = $"PREM-A-{datePart}-{orderPart}";

                return ordersessionNumber;
            }
            public static string SVIPAppointGenerateTransNumberDefault()
            {
                string datePart = DateTime.Now.ToString("MMddhhmm");

                string orderPart = transactNumber.ToString("D3");

                string ordersessionNumber = $"SVIP-A-{datePart}-{orderPart}";

                return ordersessionNumber;
            }
            //ApptMember
            public static string SVIPAppointGenerateTransNumberInc()
            {
                string datePart = DateTime.Now.ToString("MMddhhmm");

                // Use only the order number
                string orderPart = transactNumber.ToString("D3");

                // Increment the order number for the next order
                transactNumber++;
                string ordersessionNumber = $"SVIP-A-{datePart}-{orderPart}";

                return ordersessionNumber;
            }
        }
        //ApptMember
        private void RecApptTransactionClear()
        {
            RecApptCatHSRB.Checked = false;
            RecApptCatFSRB.Checked = false;
            RecApptCatNCRB.Checked = false;
            RecApptCatSpaRB.Checked = false;
            RecApptCatMassRB.Checked = false;
            RecApptSelectedServiceDGV.Rows.Clear();
            RecApptBookingTimeComboBox.Items.Clear();
            RecApptBookingDatePicker.Value = DateTime.Today;
            RecApptPreferredStaffToggleSwitch.Checked = false;
            RecApptAnyStaffToggleSwitch.Checked = false;
            isappointment = false;
        }

        //ApptMember
        private void RecApptBookingDatePicker_ValueChanged(object sender, EventArgs e)
        {
            LoadBookingTimes();
        }

        //ApptMember
        private void LoadBookingTimes()
        {
            DateTime selectedDate = RecApptBookingDatePicker.Value.Date;
            string selectedDateString = selectedDate.ToString("MM-dd-yyyy dddd");
            string serviceCategory = filterstaffbyservicecategory;

            // Retrieve matching appointment times based on selected date and service category
            List<string> matchingTimes = RetrieveMatchingAppointmentTimes(selectedDateString, serviceCategory);

            // Clear existing items in the ComboBox
            RecApptBookingTimeComboBox.Items.Clear();

            // Check if the selected date is today and if it's past 3 PM
            if (selectedDate == DateTime.Today && DateTime.Now.TimeOfDay > new TimeSpan(15, 0, 0))
            {
                // Add "Cutoff Time" to ComboBox and disable it
                RecApptBookingTimeComboBox.Items.Add("Cutoff Time");
                RecApptBookingTimeComboBox.SelectedIndex = 0;
                RecApptBookingTimeComboBox.Enabled = false;
            }
            else
            {
                // Add regular booking times for the selected date and service category
                foreach (string time in bookingTimes)
                {
                    // Add the time to the ComboBox
                    RecApptBookingTimeComboBox.Items.Add(time);
                    RecApptBookingTimeComboBox.SelectedIndex = 0;

                }

                // Remove booked times beyond the limit
                Dictionary<string, int> timeCount = new Dictionary<string, int>();
                foreach (string time in matchingTimes)
                {
                    if (!timeCount.ContainsKey(time))
                    {
                        timeCount[time] = 0;
                    }
                    timeCount[time]++;
                }

                foreach (var pair in timeCount)
                {
                    if (pair.Value >= 3)
                    {
                        RecApptBookingTimeComboBox.Items.Remove(pair.Key);
                    }
                }

                RecApptBookingTimeComboBox.Enabled = true;
            }
        }

        //ApptMember
        private List<string> RetrieveMatchingAppointmentTimes(string selectedDate, string serviceCategory)
        {
            List<string> matchingTimes = new List<string>();

            string query = "SELECT AppointmentTime FROM servicehistory WHERE AppointmentDate = @SelectedDate AND ServiceCategory = @ServiceCategory AND (QueType = 'AnyonePriority' OR QueType = 'PreferredPriority')";

            using (MySqlConnection connection = new MySqlConnection(mysqlconn))
            {
                connection.Open();

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@SelectedDate", selectedDate);
                    command.Parameters.AddWithValue("@ServiceCategory", serviceCategory);

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string appointmentTime = reader.GetString("AppointmentTime");
                            matchingTimes.Add(appointmentTime);
                        }
                    }
                }
            }
            return matchingTimes;
        }
        //ApptMember

        public void LoadAppointmentPreferredStaffComboBox()
        {
            using (MySqlConnection connection = new MySqlConnection(mysqlconn))
            {
                string bookedtime = RecApptBookingTimeComboBox.SelectedItem.ToString();
                string appointmentDate = RecApptBookingDatePicker.Value.ToString("MM-dd-yyyy dddd");

                connection.Open();

                string query = "SELECT EmployeeID, Gender, LastName, FirstName FROM systemusers WHERE EmployeeCategory = @FilterValue";
                MySqlCommand command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@FilterValue", filterstaffbyservicecategory);

                List<string> employeeIDs = new List<string>();
                List<string> genders = new List<string>();
                List<string> lastNames = new List<string>();
                List<string> firstNames = new List<string>();

                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string employeeID = reader.GetString("EmployeeID");
                        string gender = reader.GetString("Gender");
                        string lastName = reader.GetString("LastName");
                        string firstName = reader.GetString("FirstName");

                        employeeIDs.Add(employeeID);
                        genders.Add(gender);
                        lastNames.Add(lastName);
                        firstNames.Add(firstName);
                    }
                }

                RecApptAvailableAttendingStaffSelectedComboBox.Items.Clear();
                RecApptAvailableAttendingStaffSelectedComboBox.Items.Add("Select a Preferred Staff");

                for (int i = 0; i < employeeIDs.Count; i++)
                {
                    string employeeID = employeeIDs[i];
                    string gender = genders[i];
                    string lastName = lastNames[i];
                    string firstName = firstNames[i];

                    string comboBoxItem = $"{employeeID}-{gender}-{lastName}, {firstName}";

                    string scheduleQuery = "SELECT 1 FROM staffappointmentschedule WHERE EmployeeID = @EmployeeID AND AppointmentDate = @AppointmentDate AND AppointmentTime = @AppointmentTime LIMIT 1";
                    MySqlCommand scheduleCommand = new MySqlCommand(scheduleQuery, connection);
                    scheduleCommand.Parameters.AddWithValue("@EmployeeID", employeeID);
                    scheduleCommand.Parameters.AddWithValue("@AppointmentDate", appointmentDate);
                    scheduleCommand.Parameters.AddWithValue("@AppointmentTime", bookedtime);
                    object result = scheduleCommand.ExecuteScalar();

                    if (result == null)
                    {
                        RecApptAvailableAttendingStaffSelectedComboBox.Items.Add(comboBoxItem);
                    }
                }
            }

            RecApptAvailableAttendingStaffSelectedComboBox.SelectedIndex = 0;
        }

        public bool isappointment;
        public void QueTypeIdentifier(DataGridViewCell QueType)
        {


            if (isappointment == true && RecApptAnyStaffToggleSwitch.Checked)
            {
                QueType.Value = "AnyonePriority";
            }
            else if (isappointment == true && RecApptPreferredStaffToggleSwitch.Checked)
            {
                QueType.Value = "PreferredPriority";
            }
            else if (selectedStaffID == "Anyone")
            {
                QueType.Value = "GeneralQue";
            }
            else
            {
                QueType.Value = "Preferred";
            }
        }
        private void ShowNoServiceCategoryChosenWarningMessage()
        {
            RecApptNoServiceCategoryChosenWarningLbl.Visible = true;
            AnimateShakeEffect(RecApptNoServiceCategoryChosenWarningLbl);

            Timer timer = new Timer();
            timer.Interval = 1500; // 1 seconds
            timer.Tick += (s, e) =>
            {
                RecApptNoServiceCategoryChosenWarningLbl.Visible = false;

                timer.Stop();
            };
            timer.Start();
        }
        //ApptMember
        private void AnimateShakeEffect(System.Windows.Forms.Control control)
        {
            int originalX = control.Location.X;
            Random rand = new Random();
            System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
            timer.Interval = 30; // 
            timer.Tick += (s, e) =>
            {
                int newX = originalX + rand.Next(-4, 4);
                control.Location = new System.Drawing.Point(newX, control.Location.Y);
            };
            timer.Start();
        }
        private bool IsCardNameValid(string name)
        {
            foreach (char c in name)
            {
                if (!char.IsLetter(c) && c != ' ')
                {
                    return false;
                }
            }
            return true;
        }
        private bool IsNumeric(string input)
        {
            foreach (char c in input)
            {
                if (!char.IsDigit(c))
                {
                    return false;
                }
            }
            return true;
        }
        #endregion

        
        private byte[] GetImageBytesFromResource(string resourceName)
        {
            try
            {
                using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
                {
                    if (stream != null)
                    {
                        using (MemoryStream memoryStream = new MemoryStream())
                        {
                            stream.CopyTo(memoryStream);
                            return memoryStream.ToArray();
                        }
                    }
                    else
                    {
                        MessageBox.Show($"Resource stream for '{resourceName}' is null.", "Member Appointment Form Receipt Generator Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Member Appointment Form Receipt Generator Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }
        private void MemApptFormGenerator()
        {
            DateTime currentDate = RecDateTimePicker.Value;
            string datetoday = currentDate.ToString("MM-dd-yyyy dddd");
            string timePrinted = currentDate.ToString("hh:mm tt");
            string timePrintedFile = currentDate.ToString("hh-mm-ss");
            string transactNum = RecApptTransNumText.Text;
            string clientName = MemberAccInfoPersonalNameText.Text;
            string legal = "Thank you for trusting Enchanté Salon for your beauty needs." +
                " This receipt will serve as your sales invoice of any services done in Enchanté Salon." +
                " Any concerns about your services please ask and show this receipt in the frontdesk of Enchanté Salon.";
            // Increment the file name

            // Generate a unique filename for the PDF
            string fileName = $"Enchanté-Receipt-{transactNum}-{timePrintedFile}.pdf";

            // Create a SaveFileDialog to choose the save location
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "PDF Files|*.pdf";
            saveFileDialog.FileName = fileName;

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                string filePath = saveFileDialog.FileName;

                // Create a new document with custom page size (8.5"x4.25" in landscape mode)
                Document doc = new Document(new iTextSharp.text.Rectangle(Utilities.MillimetersToPoints(133f), Utilities.MillimetersToPoints(203f)));

                try
                {
                    // Create a PdfWriter instance
                    PdfWriter writer = PdfWriter.GetInstance(doc, new FileStream(filePath, FileMode.Create));

                    // Open the document for writing
                    doc.Open();

                    //string imagePath = "C:\\Users\\Pepper\\source\\repos\\Enchante\\Resources\\Enchante Logo (200 x 200 px) (1).png"; // Replace with the path to your logo image
                    // Load the image from project resources
                    //if (File.Exists(imagePath))
                    //{
                    //    //iTextSharp.text.Image logo = iTextSharp.text.Image.GetInstance(imagePath);
                    //}

                    // Load the image from project resources
                    byte[] imageBytes = GetImageBytesFromResource("EnchanteMembership.Resources.Enchante Logo (200 x 200 px) (1).png");

                    if (imageBytes != null)
                    {
                        iTextSharp.text.Image logo = iTextSharp.text.Image.GetInstance(imageBytes);
                        logo.ScaleAbsolute(50f, 50f);
                        logo.Alignment = Element.ALIGN_CENTER;
                        doc.Add(logo);
                    }
                    else
                    {
                        MessageBox.Show("Error loading image from resources.", "Appointment Form Image Loading Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    };

                    iTextSharp.text.Font headerFont = FontFactory.GetFont("Courier", 16, iTextSharp.text.Font.BOLD);
                    iTextSharp.text.Font boldfont = FontFactory.GetFont("Courier", 10, iTextSharp.text.Font.BOLD);
                    iTextSharp.text.Font font = FontFactory.GetFont("Courier", 10, iTextSharp.text.Font.NORMAL);
                    iTextSharp.text.Font italic = FontFactory.GetFont("Courier", 10, iTextSharp.text.Font.ITALIC);

                    // Create a centered alignment for text
                    iTextSharp.text.Paragraph centerAligned = new Paragraph();
                    centerAligned.Alignment = Element.ALIGN_CENTER;

                    // Add centered content to the centerAligned Paragraph
                    centerAligned.Add(new Chunk("Enchanté Salon", headerFont));
                    centerAligned.Add(new Chunk("\n69th flr. Enchanté Bldg. Ortigas Extension Ave. \nManggahan, Pasig City 1611 Philippines", font));
                    centerAligned.Add(new Chunk("\nTel. No.: (1101) 111-1010", font));
                    centerAligned.Add(new Chunk($"\nDate: {datetoday} Time: {timePrinted}", font));

                    // Add the centered content to the document
                    doc.Add(centerAligned);
                    doc.Add(new Chunk("\n")); // New line

                    doc.Add(new Paragraph($"Transaction No.: {transactNum}", font));
                    //doc.Add(new Paragraph($"Order Date: {today}", font));
                    doc.Add(new Chunk("\n")); // New line

                    doc.Add(new LineSeparator()); // Dotted line

                    PdfPTable columnHeaderTable = new PdfPTable(4);
                    columnHeaderTable.SetWidths(new float[] { 10f, 10f, 5f, 5f }); // Column widths
                    columnHeaderTable.DefaultCell.Border = PdfPCell.NO_BORDER;
                    columnHeaderTable.DefaultCell.VerticalAlignment = Element.ALIGN_CENTER;
                    columnHeaderTable.DefaultCell.HorizontalAlignment = Element.ALIGN_CENTER;

                    columnHeaderTable.AddCell(new Phrase("Attending\nStaff ID", boldfont));
                    columnHeaderTable.AddCell(new Phrase("Service(s)", boldfont));
                    columnHeaderTable.AddCell(new Phrase("Qty.", boldfont));
                    columnHeaderTable.AddCell(new Phrase("Total Price", boldfont));
                    doc.Add(columnHeaderTable);

                    doc.Add(new LineSeparator()); // Dotted line
                    // Iterate through the rows of your 

                    foreach (DataGridViewRow row in RecApptSelectedServiceDGV.Rows)
                    {
                        try
                        {
                            string serviceName = row.Cells["RecApptSelectedService"].Value?.ToString();
                            if (string.IsNullOrEmpty(serviceName))
                            {
                                continue; // Skip empty rows
                            }

                            string staffID = row.Cells["RecApptStaffSelected"].Value?.ToString();
                            string itemTotalcost = row.Cells["RecApptServicePrice"].Value?.ToString();

                            // Add cells to the item table
                            PdfPTable serviceTable = new PdfPTable(4);
                            serviceTable.SetWidths(new float[] { 5f, 5f, 3f, 3f }); // Column widths
                            serviceTable.DefaultCell.Border = PdfPCell.NO_BORDER;
                            serviceTable.DefaultCell.VerticalAlignment = Element.ALIGN_CENTER;
                            serviceTable.DefaultCell.HorizontalAlignment = Element.ALIGN_CENTER;

                            serviceTable.AddCell(new Phrase(staffID, font));
                            serviceTable.AddCell(new Phrase(serviceName, font));
                            serviceTable.AddCell(new Phrase("1", font));
                            serviceTable.AddCell(new Phrase(itemTotalcost, font));
                            doc.Add(serviceTable);

                        }
                        catch (Exception ex)
                        {
                            // Handle or log any exceptions that occur while processing DataGridView data
                            MessageBox.Show("An error occurred: " + ex.Message, "Receipt Generator Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }

                    doc.Add(new Chunk("\n")); // New line
                    doc.Add(new LineSeparator()); // Dotted line
                    doc.Add(new Chunk("\n")); // New line





                    // Add the "Served To" section
                    doc.Add(new Chunk("\n")); // New line
                    doc.Add(new Paragraph($"Served To: {clientName}", italic));
                    doc.Add(new Paragraph("Address:_______________________________", italic));
                    doc.Add(new Paragraph("TIN No.:_______________________________", italic));

                    // Add the legal string with center alignment
                    Paragraph paragraph_footer = new Paragraph($"\n\n{legal}", italic);
                    paragraph_footer.Alignment = Element.ALIGN_CENTER;
                    doc.Add(paragraph_footer);
                }
                catch (DocumentException de)
                {
                    MessageBox.Show("An error occurred: " + de.Message, "Receipt Generator Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch (IOException ioe)
                {
                    MessageBox.Show("An error occurred: " + ioe.Message, "Receipt Generator Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    // Close the document
                    doc.Close();
                }

                //MessageBox.Show($"Receipt saved as {filePath}", "Receipt Saved", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void RecApptAvailableAttendingStaffSelectedComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (RecApptAvailableAttendingStaffSelectedComboBox.SelectedItem != null)
            {
                string selectedValue = RecApptAvailableAttendingStaffSelectedComboBox.SelectedItem.ToString();
                selectedStaffID = selectedValue.Substring(0, 11);
            }
        }

        private void MemApptTransactionClear()
        {
            RecApptCatHSRB.Visible = false;
            RecApptCatHSRB.Checked = false;
            RecApptCatFSRB.Visible = false;
            RecApptCatNCRB.Visible = false;
            RecApptCatSpaRB.Visible = false;
            RecApptCatMassRB.Visible = false;
            RecApptCatFSRB.Checked = false;
            RecApptCatNCRB.Checked = false;
            RecApptCatSpaRB.Checked = false;
            RecApptCatMassRB.Checked = false;

            RecApptSelectedServiceDGV.Rows.Clear();
            RecApptBookingTimeComboBox.Items.Clear();
            RecApptBookingDatePicker.Value = DateTime.Today;
            RecApptPreferredStaffToggleSwitch.Checked = false;
            RecApptAnyStaffToggleSwitch.Checked = false;
            isappointment = false;

            RecApptAnyStaffToggleSwitch.Checked = false;
            RecApptPreferredStaffToggleSwitch.Checked = false;
            RecApptAnyStaffToggleSwitchRegular.Checked = false;
            RecApptAttendingStaffLbl.Visible = false;
            RecApptAvailableAttendingStaffSelectedComboBox.Visible = false;
        }

        private void TestPrint_Click(object sender, EventArgs e)
        {
            MemApptFormGenerator();
        }
    }
}
