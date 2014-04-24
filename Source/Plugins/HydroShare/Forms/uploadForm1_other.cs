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
    public partial class uploadForm1_other : Form
    {
        public string title, bodyEditSummary, name, nickname, email, organization, address, cityState, zipCode, country, phoneNumber;
        public string cName, cNickname, cContribution, cEmail, cOrganization, cCountry, cPhoneNumber;
        public string sSubject, sName, sComments, sID;
        public string references, rights, coverageSpatial, tags, dataSourceDriver, driver, style, webservice, style_ID, layer_ID, driverConfig,
            revisionInfo, authoringInfo, publishingOptions, file;


        public uploadForm1_other()
        {

            InitializeComponent();
            string[] items = new string[] { "N/A", "SHAPEFILE", "KMZ", "GEOTIFF", "POSTGIS", "OGR" };
            dataSourceDriverCB.DataSource = items;
            dataSourceDriverCB.SelectedIndex = 0;

            pBar.Visible = true;
            pBar.Minimum = 1;
            pBar.Maximum = items.Length;
            pBar.Value = 1;
            pBar.Step = 1;
        }
        private void makeVisible()
        {
            this.Visible = true;
        }


        private void dataSourceDriverCB_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        private void button1_Click(object sender, EventArgs e)
        {
            gotoWeb browser = new gotoWeb();
            browser.StartPosition = FormStartPosition.CenterScreen;
            browser.Visible = true;
        }

        private void finishUploadButton_Click_1(object sender, EventArgs e)
        {
            //System.IO.File.WriteAllLines(@"C:\Users\Temp\Desktop\WriteLines.txt", prevInputs);
            /*
            string url = "http://dev.hyrdroshare.org";
            string file = fileTB.Text;
            WebClient client = new WebClient();
            client.UploadFile(url, "post", file);
            */


            title = this.titleTB.Text;
            pBar.PerformStep();
            name = this.nameTB.Text;
            pBar.PerformStep();
            email = this.emailTB.Text;
            pBar.PerformStep();
            sSubject = this.sourceSubjectTB.Text;
            pBar.PerformStep();
            rights = this.rightsTB.Text;
            pBar.PerformStep();
            coverageSpatial = this.coverageSpatialTB.Text;
            pBar.PerformStep();
            file = this.fileTB.Text;
            pBar.PerformStep();


            //if either title OR email OR name OR subject text boxes are blank
            //then the user is informed that they must fix this error
            if ((title.Length == 0) || (email.Length == 0) || (name.Length == 0) || (sSubject.Length == 0) || (rights.Length == 0) || (coverageSpatial.Length == 0) || (file.Length == 0))
            {
                MessageBox.Show("Please fill in all required fields (marked with an * ).", "Error", MessageBoxButtons.OK);
            }

            else
            {
                //store text boxes in local variables
                //these variables will then be permanently stored in an array
                bodyEditSummary = this.bodyTB.Text;
                pBar.PerformStep();
                nickname = this.nicknameTB.Text;
                pBar.PerformStep();
                organization = this.organizationTB.Text;
                pBar.PerformStep();
                address = this.addressTB.Text;
                pBar.PerformStep();
                cityState = this.citystateTB.Text;
                pBar.PerformStep();
                zipCode = this.zipcodeTB.Text;
                pBar.PerformStep();
                country = this.countryTB.Text;
                pBar.PerformStep();
                phoneNumber = this.phonenumberTB.Text;
                pBar.PerformStep();
                cName = this.contribNameTB.Text;
                pBar.PerformStep();
                cNickname = this.contribNicknameTB.Text;
                pBar.PerformStep();
                cContribution = this.contribContributionTB.Text;
                pBar.PerformStep();
                cEmail = this.contribEmailTB.Text;
                pBar.PerformStep();
                cOrganization = this.contribOrganizationTB.Text;
                pBar.PerformStep();
                cCountry = this.contribCountryTB.Text;
                pBar.PerformStep();
                cPhoneNumber = this.contribPhonenumberTB.Text;
                pBar.PerformStep();
                sName = this.sourceNameTB.Text;
                pBar.PerformStep();
                sComments = this.sourceCommentsTB.Text;
                pBar.PerformStep();
                sID = this.sourceIDTB.Text;
                pBar.PerformStep();
                references = this.referencesTB.Text;
                pBar.PerformStep();
                tags = this.tagsTB.Text;
                pBar.PerformStep();
                dataSourceDriver = this.dataSourceDriverCB.Text;
                pBar.PerformStep();
                driver = this.driverTB.Text;
                pBar.PerformStep();
                style = this.styleTB.Text;
                pBar.PerformStep();
                webservice = this.webserviceTB.Text;
                pBar.PerformStep();
                style_ID = this.style_IDTB.Text;
                pBar.PerformStep();
                layer_ID = this.layer_IDTB.Text;
                pBar.PerformStep();
                driverConfig = this.driverConfigTB.Text;
                pBar.PerformStep();
                revisionInfo = this.revisionInfoTB.Text;
                pBar.PerformStep();
                authoringInfo = this.authoringInfoTB.Text;
                pBar.PerformStep();
                publishingOptions = this.publishingOptionsTB.Text;
                pBar.PerformStep();

                //store all of the variables in an array
                //this array will be passed down to uploadForm2
                //this way we can close this form, but retain its contents
                string[] inputs = {title, bodyEditSummary, name, nickname, email, organization, address, cityState, zipCode, country, 
                                  phoneNumber, cName, cNickname, cContribution, cEmail, cOrganization, cCountry, cPhoneNumber,
                                  sSubject, sName, sComments, sID, references, tags, dataSourceDriver, driver, style, webservice,
                                  style_ID, layer_ID, driverConfig, revisionInfo, authoringInfo, publishingOptions};

                //close this form
                this.Visible = false;

                printForm print = new printForm(this);
                print.StartPosition = FormStartPosition.CenterScreen;
                print.Visible = true;
            }
        }

        private void browseButton_Click_1(object sender, EventArgs e)
        {
            /*
          OpenFileDialog fDialog = new OpenFileDialog();
          fDialog.Title = "Open File";
          fDialog.Filter = "XML Files|*.xml|UML Files|*.uml";
          fDialog.InitialDirectory = @"C:\";
          if (fDialog.ShowDialog() == DialogResult.OK)
              MessageBox.Show(fDialog.FileName.ToString());
          */

            FolderBrowserDialog folder = new FolderBrowserDialog();
            if (folder.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string folderPath = folder.SelectedPath;
                file = fileTB.Text = folderPath;
            }
        }

        private void backButton_Click_1(object sender, EventArgs e)
        {
            if (MessageBox.Show("This will delete all information in this form. Are you sure you want to proceed?", "Caution", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
            {
                this.Close();
                chooseUploadType uploadType = new chooseUploadType();
                uploadType.StartPosition = FormStartPosition.CenterScreen;
                uploadType.Visible = true;
            }
            else
            {
                //do nothing
            }
        }

        private void Cancel_button_Click(object sender, EventArgs e)
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
    }
}
