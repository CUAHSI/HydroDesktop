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

        //click the CANCEL button
        private void button2_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("This will delete all information in this form. Are you sure you want to proceed?", "Caution", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
            {
                this.Close();
            }
            else
            {
                //do nothing
            }
        }

        private void nextButton_Click_1(object sender, EventArgs e)
        {
            title = this.titleTB.Text;
            name = this.nameTB.Text;
            email = this.emailTB.Text;
            sSubject = this.sourceSubjectTB.Text;

            //if either title OR email OR name OR subject text boxes are blank
            //then the user is informed that they must fix this error
            if ((title.Length == 0) || (email.Length == 0) || (name.Length == 0) || (sSubject.Length == 0))
            {
                if (title.Length == 0)
                {
                    pictureBox1.Visible = false;
                    titleX.Visible = true;
                } 
                if (email.Length == 0)
                {
                    pictureBox2.Visible = false;
                    emailX.Visible = true;
                } 
                if (name.Length == 0)
                {
                    pictureBox3.Visible = false;
                    nameX.Visible = true;
                } 
                if (sSubject.Length == 0)
                {
                    pictureBox4.Visible = false;
                    subjectX.Visible = true;
                }

                MessageBox.Show("Please fill in all required fields (marked with an * ).", "Error", MessageBoxButtons.OK);
            }

            else
            {
                //store text boxes in local variables
                //these variables will then be permanently stored in an array
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
                sName = this.sourceNameTB.Text;
                sComments = this.sourceCommentsTB.Text;
                sID = this.sourceIDTB.Text;

                //store all of the variables in an array
                //this array will be passed down to uploadForm2
                //this way we can close this form, but retain its contents
                string[] inputs = {title, bodyEditSummary, name, nickname, email, organization, address, cityState, zipCode, country, phoneNumber,
                                  cName, cNickname, cContribution, cEmail, cOrganization, cCountry, cPhoneNumber,
                                  sSubject, sName, sComments, sID};

                //close this form
                this.Visible = false;

                //instantiate form2...which takes the array of inputs as a parameter
                uploadForm2 form2 = new uploadForm2(inputs);
                form2.StartPosition = FormStartPosition.CenterScreen;
                form2.Visible = true;
            }
        }

        private void makeVisible()
        {
            this.Visible = true;
        }

        private void titleTB_TextChanged(object sender, EventArgs e)
        {
            if (titleTB.Text.Length != 0)
            {
                pictureBox1.Visible = true;
                titleX.Visible = false;
            }
        }

        private void nameTB_TextChanged(object sender, EventArgs e)
        {
            if (nameTB.Text.Length != 0)
            {
                pictureBox2.Visible = true;
                nameX.Visible = false;
            }
        }

        private void emailTB_TextChanged(object sender, EventArgs e)
        {
            if (emailTB.Text.Length != 0)
            {
                pictureBox3.Visible = true;
                emailX.Visible = false;
            }
        }

        private void sourceSubjectTB_TextChanged(object sender, EventArgs e)
        {
            if (sourceSubjectTB.Text.Length != 0)
            {
                pictureBox4.Visible = true;
                subjectX.Visible = false;
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }
    }
}
