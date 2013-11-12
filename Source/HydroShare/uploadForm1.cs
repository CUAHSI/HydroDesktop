using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;


namespace HydroShare
{
    public partial class uploadForm1 : Form
    {
        string title, bodyEditSummary, name, nickname, email, organization, address, cityState, zipCode, country, phoneNumber;
        string cName, cNickname, cContribution, cEmail, cOrganization, cCountry, cPhoneNumber;
        string sSubject, sName, sComments, sID;

        public uploadForm1()
        {
            InitializeComponent();
        }

        private void uploadForm_Load(object sender, EventArgs e)
        {
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void nextButton_Click_1(object sender, EventArgs e)
        {
            title = this.titleTB.Text;
            name = this.nameTB.Text;
            email = this.emailTB.Text;

            if ((title.Length == 0) || (email.Length == 0) || (name.Length == 0))
            {
                errorRequiredInformation error1 = new errorRequiredInformation();
                error1.Visible = true;
            }

            else
            {
                bodyEditSummary = this.bodyTB.Text;
                nickname = this.nicknameTB.Text;
                organization = this.organizationTB.Text;
                address = this.addressTB.Text;
                cityState = this.citystateTB.Text;
                zipCode = this.zipcodeTB.Text;
                country = this.countryTB.Text;
                phoneNumber = this.phonenumberTB.Text;
                cName = this.contribNameTB.Text;
                cNickname = this.contribNicknameTB.Text;
                cContribution = this.contribContributionTB.Text;
                cEmail = this.contribEmailTB.Text;
                cOrganization = this.contribOrganizationTB.Text;
                cCountry = this.contribCountryTB.Text;
                cPhoneNumber = this.contribPhonenumberTB.Text;
                sSubject = this.sourceSubjectTB.Text;
                sName = this.sourceNameTB.Text;
                sComments = this.sourceCommentsTB.Text;
                sID = this.sourceIDTB.Text;

                this.Close();
                uploadForm2 form2 = new uploadForm2();
                form2.Visible = true;
            }
        }
        

    
    }
}
