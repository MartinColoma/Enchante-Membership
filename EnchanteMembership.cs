using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
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
        //tool tip
        private System.Windows.Forms.ToolTip iconToolTip;

        string membercategory;

        //gender combo box
        private string[] genders = { "Male", "Female", "Prefer Not to Say" };

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

            //gender combobox
            RegularGenderComboText.Items.AddRange(genders);
            RegularGenderComboText.DropDownStyle = ComboBoxStyle.DropDownList;
            SVIPGenderComboText.Items.AddRange(genders);
            SVIPGenderComboText.DropDownStyle = ComboBoxStyle.DropDownList;
            PremGenderComboText.Items.AddRange(genders);
            PremGenderComboText.DropDownStyle = ComboBoxStyle.DropDownList;

            // Initialize the timer
            ScrollTimer.Start();
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
            // Update button colors based on scroll position
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
            EnchanteReviewBtn.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(229)))), ((int)(((byte)(229)))), ((int)(((byte)(221)))));
            EnchanteTeamBtn.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(229)))), ((int)(((byte)(229)))), ((int)(((byte)(221)))));
            EnchanteAbtUsBtn.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(229)))), ((int)(((byte)(229)))), ((int)(((byte)(221)))));


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
            EnchanteReviewBtn.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(229)))), ((int)(((byte)(229)))), ((int)(((byte)(221)))));
            EnchanteTeamBtn.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(229)))), ((int)(((byte)(229)))), ((int)(((byte)(221)))));
            EnchanteAbtUsBtn.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(229)))), ((int)(((byte)(229)))), ((int)(((byte)(221)))));


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
            EnchanteReviewBtn.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(229)))), ((int)(((byte)(229)))), ((int)(((byte)(221)))));
            EnchanteTeamBtn.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(229)))), ((int)(((byte)(229)))), ((int)(((byte)(221)))));
            EnchanteAbtUsBtn.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(229)))), ((int)(((byte)(229)))), ((int)(((byte)(221)))));
        }
        private void EnchanteReviewBtn_Click(object sender, EventArgs e)
        {
            ReviewLocationAndColor();
        }

        private void ReviewLocationAndColor()
        {
            //Reset Panel to Show Default
            HomePanelReset();

            ////location scroll
            //int serviceSectionY = 1800;
            //ScrollToCoordinates(0, serviceSectionY);

            //Change color once clicked
            EnchanteReviewBtn.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(177)))), ((int)(((byte)(183)))), ((int)(((byte)(97)))));
            //Change back to original
            EnchanteHomeBtn.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(229)))), ((int)(((byte)(229)))), ((int)(((byte)(221)))));
            EnchanteServiceBtn.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(229)))), ((int)(((byte)(229)))), ((int)(((byte)(221)))));
            EnchanteMemberBtn.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(229)))), ((int)(((byte)(229)))), ((int)(((byte)(221)))));
            EnchanteTeamBtn.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(229)))), ((int)(((byte)(229)))), ((int)(((byte)(221)))));
            EnchanteAbtUsBtn.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(229)))), ((int)(((byte)(229)))), ((int)(((byte)(221)))));
        }
        private void EnchanteTeamBtn_Click(object sender, EventArgs e)
        {
            TeamLocationAndColor();
        }

        private void TeamLocationAndColor()
        {
            //Reset Panel to Show Default
            HomePanelReset();

            ////location scroll
            //int serviceSectionY = 1800;
            //ScrollToCoordinates(0, serviceSectionY);

            //Change color once clicked
            EnchanteTeamBtn.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(177)))), ((int)(((byte)(183)))), ((int)(((byte)(97)))));
            //Change back to original
            EnchanteHomeBtn.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(229)))), ((int)(((byte)(229)))), ((int)(((byte)(221)))));
            EnchanteServiceBtn.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(229)))), ((int)(((byte)(229)))), ((int)(((byte)(221)))));
            EnchanteMemberBtn.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(229)))), ((int)(((byte)(229)))), ((int)(((byte)(221)))));
            EnchanteReviewBtn.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(229)))), ((int)(((byte)(229)))), ((int)(((byte)(221)))));
            EnchanteAbtUsBtn.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(229)))), ((int)(((byte)(229)))), ((int)(((byte)(221)))));
        }
        private void EnchanteAbtUsBtn_Click(object sender, EventArgs e)
        {
            AboutUsLocatonAndColor();
        }

        private void AboutUsLocatonAndColor()
        {
            //Reset Panel to Show Default
            HomePanelReset();

            ////location scroll
            //int serviceSectionY = 1800;
            //ScrollToCoordinates(0, serviceSectionY);

            //Change color once clicked
            EnchanteAbtUsBtn.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(177)))), ((int)(((byte)(183)))), ((int)(((byte)(97)))));
            //Change back to original
            EnchanteHomeBtn.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(229)))), ((int)(((byte)(229)))), ((int)(((byte)(221)))));
            EnchanteServiceBtn.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(229)))), ((int)(((byte)(229)))), ((int)(((byte)(221)))));
            EnchanteMemberBtn.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(229)))), ((int)(((byte)(229)))), ((int)(((byte)(221)))));
            EnchanteReviewBtn.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(229)))), ((int)(((byte)(229)))), ((int)(((byte)(221)))));
            EnchanteTeamBtn.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(229)))), ((int)(((byte)(229)))), ((int)(((byte)(221)))));
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

        private void EnchanteReviewBtn_MouseHover(object sender, EventArgs e)
        {
            iconToolTip.SetToolTip(EnchanteReviewBtn, "Reviews");
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
                LoginPassErrorLbl.Visible = true;
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

                    string queryApproved = "SELECT FirstName, LastName, MemberIDNumber, MembershipType, HashedPass FROM membershipaccount WHERE EmailAdd = @email";

                    using (MySqlCommand cmdApproved = new MySqlCommand(queryApproved, connection))
                    {
                        cmdApproved.Parameters.AddWithValue("@email", email);

                        using (MySqlDataReader readerApproved = cmdApproved.ExecuteReader())
                        {
                            if (readerApproved.Read())
                            {
                                string name = readerApproved["FirstName"].ToString();
                                string lastname = readerApproved["LastName"].ToString();
                                string ID = readerApproved["MemberIDNumber"].ToString();
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
                                        MemberNameLbl.Text = name + " " + lastname;
                                        MemberIDNumLbl.Text = ID;
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
                membercategory = "";


                MemberUserAccPanel.Visible = false;

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
            iconToolTip.SetToolTip(SM_FBBtn1, "Facebook");

        }

        private void SM_TwitterBtn_MouseHover(object sender, EventArgs e)
        {
            iconToolTip.SetToolTip(SM_TwitterBtn, "Twitter");
            iconToolTip.SetToolTip(SM_TwitterBtn1, "Twitter");
        }

        private void SM_IGBtn_MouseHover(object sender, EventArgs e)
        {
            iconToolTip.SetToolTip(SM_IGBtn, "Instagram");
            iconToolTip.SetToolTip(SM_IGBtn1, "Instagram");

        }

        private void SM_GmailBtn_MouseHover(object sender, EventArgs e)
        {
            iconToolTip.SetToolTip(SM_GmailBtn, "Email Us Here");
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
        }
        #endregion


    }
}
