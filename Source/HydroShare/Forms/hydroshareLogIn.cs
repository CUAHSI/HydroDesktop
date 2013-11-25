using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HydroShare.Forms
{
    public partial class hydroshareLogIn : Form
    {
        private string username;
        private string password;
        public hydroshareLogIn(string u, string p)
        {
            InitializeComponent();
            username = u;
            password = p;
            
        }

        void SetText(string attribute, string attName, string value)
        {
            HtmlElementCollection col = webBrowser1.Document.GetElementsByTagName("input");

            foreach (HtmlElement element in col)
            {
                if (element.GetAttribute(attribute).Equals(attName))
                    element.SetAttribute("value", value);
            }
        }

        void ClickButton(string attribute, string attName)
        {
            HtmlElementCollection col = webBrowser1.Document.GetElementsByTagName("input");

            foreach (HtmlElement element in col)
            {
                if (element.GetAttribute(attribute).Equals(attName))
                    element.InvokeMember("click");
            }

        }

        private void webBrowser1_DocumentCompleted_1(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            // "q" is the name of the Google search textbox

            //SetText("type", "text", this.username);
            //SetText("type", "password", this.password);
            
            //ClickButton("type", "submit");

            SetText("name", "q", "dog");
            // "btnK" is the name of the Google search submit button
            // "go" is the name of the Wiki search submit button
            ClickButton("name", "btnK");
            //this.Close();
            return;
        }
    }
}
