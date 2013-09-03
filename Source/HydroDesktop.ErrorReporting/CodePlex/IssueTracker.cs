using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace HydroDesktop.ErrorReporting.CodePlex
{
    internal class IssueTracker
    {
        private readonly string _projectName;
        private WebBrowser _browser;

        public bool IsSignedIn { get; private set; }

        public IssueTracker(string projectName)
        {
            _projectName = projectName;
        }

        public void SignIn(string userName, string password)
        {
            if (String.IsNullOrWhiteSpace(userName))
            {
                throw new ArgumentOutOfRangeException("userName", "User name should be not empty");
            }
            if (String.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentOutOfRangeException("password", "Password should be not empty");
            }

            _browser = new WebBrowser("Mozilla/5.0 (Windows NT 6.1) AppleWebKit/535.19 (KHTML, like Gecko) Chrome/18.0.1025.151 Safari/535.19");
            const string loginUrl = @"https://www.codeplex.com/site/login";
            var page = _browser.Get(loginUrl);
            var form = HtmlForm.GetForm(page, "aspnetForm");

            foreach (var t in form.Elements)
            {
                if (t.Name == null) continue;
                switch (t.Name.ToLower())
                {
                    case "username":
                        t.Value = userName;
                        break;
                    case "password":
                        t.Value = password;
                        break;
                }
            }

            var doc = _browser.Post(loginUrl, form);
            if (!doc.Contains("Redirect"))
            {
                throw new Exception("Invalid user name or password.");
            }
            IsSignedIn = true;
        }

        /// <summary>
        /// Creates issue on Codeplex
        /// </summary>
        /// <param name="issue">Issue to create</param>
        /// <returns>Link to created issue.</returns>
        public string CreateIssue(Issue issue)
        {
            if (issue == null) throw new ArgumentNullException("issue");
            if (!IsSignedIn)
            {
                throw new Exception("Not signed in into codeplex.");
            }

            var createIssueUrl = string.Format(@"https://{0}.codeplex.com/WorkItem/Create", _projectName);

            var createIssuePage = _browser.Get(createIssueUrl);
            var form = HtmlForm.GetForm(createIssuePage, "aspnetForm");
            var elements = new List<HtmlElement>(form.Elements);
            form.Elements.Clear();

            var file = issue.FileToAttach;
            byte[] fileData = null;
            if (file != null)
            {
                using (var fs = new FileStream(file.FullName, FileMode.Open, FileAccess.Read))
                {
                    fileData = new byte[fs.Length];
                    fs.Read(fileData, 0, fileData.Length);
                }
            }
            form.Elements.Add(new HtmlElement("__RequestVerificationToken", elements.First(t => t.Name == "__RequestVerificationToken").Value));
            form.Elements.Add(new HtmlElement("searchsite", "Search all projects"));
            form.Elements.Add(new HtmlElement("WorkItem.Summary", issue.Summary));
            form.Elements.Add(new HtmlElement("Text", issue.Description));

            if (fileData != null)
            {
                form.Elements.Add(new HtmlElement("PostedFile_text", @"C:\fakepath\" + file.Name));
                form.Elements.Add(new HtmlElement("PostedFile", null)
                    {
                        IsFile = true,
                        File = fileData,
                        ContentType = "application/octet-stream",
                        FileName = file.Name
                    });
            }
            else
            {
                form.Elements.Add(new HtmlElement("PostedFile_text", null));
                form.Elements.Add(new HtmlElement("PostedFile", null) {IsFile = true,});
            }   
            
            form.Elements.Add(new HtmlElement("SubscribeCheckBox", "false"));
            form.Elements.Add(new HtmlElement("EmailSubscriptionTypeList", "1"));
            form.Elements.Add(new HtmlElement("StopNotificationsCheckBox", "false"));
            form.Elements.Add(new HtmlElement("SelectedSubscriptionItem", "1"));
            form.Elements.Add(new HtmlElement("SelectedStatus", "Proposed"));
            form.Elements.Add(new HtmlElement("SelectedType", "Issue"));
            form.Elements.Add(new HtmlElement("SelectedPriority", "Low"));
            form.Elements.Add(new HtmlElement("WorkItem.PlannedForRelease", "Unassigned"));
            form.Elements.Add(new HtmlElement("WorkItem.AssignedTo", "Unassigned"));
            form.Elements.Add(new HtmlElement("SelectedComponent", "No Component Selected"));
            form.Elements.Add(new HtmlElement("WorkItem.Custom", null));

            var res = _browser.Post(createIssueUrl, form, createIssueUrl, true);
            // Check that issue was created
            string issueLink = null;
            var itemNumberSection = res.IndexOf(@">Item number:</td>", StringComparison.Ordinal);
            if (itemNumberSection >= 0)
            {
                var itemNumberStarts = res.IndexOf(">", itemNumberSection + @">Item number:</td>".Length + 1,
                                                   StringComparison.Ordinal);
                if (itemNumberStarts >= 0)
                {
                    var itemNumberEnds = res.IndexOf("</td>", itemNumberStarts + 1, StringComparison.Ordinal);
                    if (itemNumberEnds >= 0)
                    {
                        var itemNumber = res.Substring(itemNumberStarts + 1, itemNumberEnds - itemNumberStarts - 1);
                        int number;
                        if (Int32.TryParse(itemNumber, out number))
                        {
                            issueLink = string.Format("http://{0}.codeplex.com/workitem/{1}", _projectName, number);
                        }
                    }
                }
            }
            if (issueLink == null)
            {
                throw new Exception("Unable to post issue on Codeplex.");
            }
            return issueLink;
        }
    }
}
