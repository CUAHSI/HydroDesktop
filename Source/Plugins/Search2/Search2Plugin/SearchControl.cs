using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Windows.Forms;
using System.Text.RegularExpressions;

using System.Xml;
using DotSpatial.Controls;
using DotSpatial.Data;
using DotSpatial.Topology;
using System.IO;
using HydroDesktop.Configuration;
using DotSpatial.Projections;
using DotSpatial.Symbology;
using HydroDesktop.Database;
using HydroDesktop.Interfaces.ObjectModel;
using System.Globalization;
using HydroDesktop.Interfaces;
using HydroDesktop.Search.LayerInformation;
using log4net;
using HydroDesktop.Search.Download;

namespace HydroDesktop.Search
{
    public partial class SearchControl : UserControl
    {
        private static readonly log4net.ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #region Private Member Variables
        //the _mapArgs class level variable for accessing the map
        private IMapPluginArgs _mapArgs;

        //the main MapWindow map
        private Map mapMain = null;

        // this handles the list of webservices
        private WebServicesList _webServicesList;

        // this handles the rectangle drawing
        private RectangleDrawing _rectangleDrawing;

        private readonly DownloadManager _downLoadManager;
        private SearchInformer searchInformer;

        #endregion

        #region Constructor
        public SearchControl(IMapPluginArgs mapArgs)
        {
            InitializeComponent();

            //set the mapwindow variable
            _mapArgs = mapArgs;

            //set the main map
            mapMain = (Map)_mapArgs.Map; //TODO: hack

            // this manages the former webservices.xml
            _webServicesList = new WebServicesList();

            Populate_xmlcombo();

            dateTimePickStart.Value = DateTime.Now.Date.AddYears(-1);
            dateTimePickEnd.Value = DateTime.Now;

            //Rectangle drawing initialization
            _rectangleDrawing = null;

            //Layer added event that helps capturing the polygon layers
            _mapArgs.Map.MapFrame.LayerAdded += new EventHandler<DotSpatial.Symbology.LayerEventArgs>(MapFrame_LayerAdded);
            //Layer removed event
            _mapArgs.Map.MapFrame.LayerRemoved += new EventHandler<DotSpatial.Symbology.LayerEventArgs>(MapFrame_LayerRemoved);

            //listBox4.MouseUp += new MouseEventHandler(listBox4_MouseUp);
            dgvSearch.Click += new EventHandler(dataGridView1_Click);
            dgvSearch.SelectionChanged += new EventHandler(dataGridView1_SelectionChanged);

            treeViewWebServices.MouseUp += new MouseEventHandler(treeView1_MouseUp);

            // mouse up on list should ensure radiobutton existing themes get selected
            lstThemes.MouseUp += new MouseEventHandler(lstThemes_MouseUp);

            //project opening event to ensure refreshing of layers
            _mapArgs.AppManager.SerializationManager.Deserializing += new EventHandler<SerializingEventArgs>(SerializationManager_Deserializing);

            //set the default search mode
            SearchMode = Resources.SearchMode_HISCentral;

            //set the series selected event
            searchDataGridView1.SelectionChanged += new EventHandler(searchDataGridView1_SelectionChanged);


            _downLoadManager = new DownloadManager { Log = log };
        }

        #endregion


        #region Properties
        /// <summary>
        /// The main image that is shown in the top-left corner
        /// </summary>
        // MainImage is also commented out alongwith PictureBox2
        //public Image MainImage
        //{
        //    get { return PictureBox2.Image; }
        //    set { PictureBox2.Image = value; }
        //}

        /// <summary>
        /// The title text ("HIS Central" or "Metadata Cache")
        /// </summary>
        public string SearchMode
        {
            get { return this.Label3.Text; }
            set
            {
                string oldSearchMode = Label3.Text;

                Label3.Text = value;

                //if search mode is set to metadata cache -
                //change the list of web services and keywords
                //accordingly
                if (value == Resources.SearchMode_MetadataCache && value != oldSearchMode)
                {
                    FillWebServicesFromDB();
                    FillKeywordsFromDB();
                    rbList.PerformClick();
                    cboShowWebServicesPanel.Checked = true;
                    //button2_Click(null, null);
                }

                else if (value == Resources.SearchMode_HISCentral && value != oldSearchMode)
                {
                    button10_Click(null, null);
                    //try
                    //{
                    //    treeviewOntology.Nodes.Clear();
                    //    var tmpxmldoc = HdSearchOntologyHelper.ReadOntologyXmlFile();
                    //    FillTree(tmpxmldoc.DocumentElement, treeviewOntology.Nodes);
                    //}
                    //catch (Exception ex)
                    //{
                    //    tboTypeKeyword.Text = ex.Message;
                    //}
                }
            }
        }
        #endregion

        //when the current project is being opened
        void SerializationManager_Deserializing(object sender, SerializingEventArgs e)
        {
            AddPolygonLayers();
            if (cboActiveLayer.Items.Count > 0)
            {
                cboActiveLayer.SelectedIndex = 0;
            }
        }

        void lstThemes_MouseUp(object sender, MouseEventArgs e)
        {
            rbExistingTheme.Checked = true;
        }
        private void btnSaveSearch_Click(object sender, EventArgs e)
        {
            try
            {
                //string filename = "q_save.xml";
                // TODO: move filename to settings.
                //TODO: Move method to separate class
                string filename = Path.Combine(Application.StartupPath, "q_save.xml");
                XmlDocument hd_xmlDoc = new XmlDocument();
                // TODO: Check for file existing.  That should be more efficient than relying on an exception.
                try
                {
                    hd_xmlDoc.Load(filename);
                }
                catch (System.IO.FileNotFoundException)
                {
                    //file is not found and therefore lets create a new xml file here
                    XmlTextWriter hd_xmlWriter = new XmlTextWriter(filename, System.Text.Encoding.UTF8);
                    hd_xmlWriter.Formatting = Formatting.Indented;
                    hd_xmlWriter.WriteProcessingInstruction("xml", "version='1.0' encoding='UTF-8'");
                    hd_xmlWriter.WriteStartElement("Gro");
                    hd_xmlWriter.Close();
                    hd_xmlDoc.Load(filename);
                }
                // TODO: Not catching other exceptions and just proceeding???

                // TODO: Give this xml a more comprehensible schema
                XmlNode Gro = hd_xmlDoc.DocumentElement;
                XmlElement hd_childnode_1 = hd_xmlDoc.CreateElement("cNode_A");
                XmlElement hd_childnode_2 = hd_xmlDoc.CreateElement("cNode_B");
                XmlElement hd_childnode_3 = hd_xmlDoc.CreateElement("cNode_C");//n
                XmlElement hd_childnode_4 = hd_xmlDoc.CreateElement("cNode_D");//n
                XmlElement hd_childnode_5 = hd_xmlDoc.CreateElement("cNode_E");//n
                XmlElement hd_childnode_6 = hd_xmlDoc.CreateElement("cNode_F");//n
                XmlElement hd_childnode_7 = hd_xmlDoc.CreateElement("cNode_G");//n
                XmlElement hd_childnode_8 = hd_xmlDoc.CreateElement("cNode_H");//n

                //for childnode1
                XmlText hd_textNode_1 = hd_xmlDoc.CreateTextNode("tData");

                // for ChildNode2
                XmlText hd_textNode = hd_xmlDoc.CreateTextNode("tData");

                // for ChildNode3
                XmlText hd_textNode_3 = hd_xmlDoc.CreateTextNode("tData");//n

                // for ChildNode4
                XmlText hd_textNode_4 = hd_xmlDoc.CreateTextNode("tData");//n

                // for ChildNode5
                XmlText hd_textNode_5 = hd_xmlDoc.CreateTextNode("tData");//n

                // for ChildNode6
                XmlText hd_textNode_6 = hd_xmlDoc.CreateTextNode("tData");//n

                // for ChildNode7
                XmlText hd_textNode_7 = hd_xmlDoc.CreateTextNode("tData");//n

                // for ChildNode8
                XmlText hd_textNode_8 = hd_xmlDoc.CreateTextNode("tData");//n


                //XmlText hd_textNode_1;
                Gro.AppendChild(hd_childnode_1);
                hd_childnode_1.AppendChild(hd_childnode_2);
                hd_childnode_1.AppendChild(hd_childnode_3);//n
                hd_childnode_1.AppendChild(hd_childnode_4);//n
                hd_childnode_1.AppendChild(hd_childnode_5);//n
                hd_childnode_1.AppendChild(hd_childnode_6);//n
                hd_childnode_1.AppendChild(hd_childnode_7);//n
                hd_childnode_1.AppendChild(hd_childnode_8);//n


                // try here
                if (tboSearchName.Text == "")
                {
                    MessageBox.Show("Please type a name for this search.");
                    return;
                }

                //check whether name proposed for query already exists
                for (int t = 0; t < lboRestoreSearch.Items.Count; t++)//c1
                {
                    if (tboSearchName.Text == lboRestoreSearch.Items[t].ToString())//c1
                    {
                        MessageBox.Show("Search name already exists. Please rename and try again.");
                        return;
                    }
                }

                hd_childnode_1.SetAttribute("Search_Name", tboSearchName.Text);
                // try ends

                //Area
                hd_childnode_2.AppendChild(hd_textNode);
                hd_textNode.Value = lblAreaPara.Text;

                //handles no. 3 here, 
                hd_childnode_3.AppendChild(hd_textNode_3);//n
                hd_textNode_3.Value = lblWebServValue.Text;

                //handles no. 4 here, 
                hd_childnode_4.AppendChild(hd_textNode_4);//n
                hd_textNode_4.Value = lblKeywordsValue.Text;

                //handles no. 5 here,
                hd_childnode_5.AppendChild(hd_textNode_5);//n
                hd_textNode_5.Value = lblDateValue.Text;

                //handles no. 6 here, 
                hd_childnode_6.AppendChild(hd_textNode_6);//n
                hd_textNode_6.Value = lblServerValue.Text;

                //handles no. 7 here, 
                hd_childnode_7.AppendChild(hd_textNode_7);//n
                if (cboActiveLayer.Text == "")
                {
                    MessageBox.Show("Please select a valid layer name first.");
                    tabControl2.SelectedIndex = 0;
                    return;
                }

                hd_textNode_7.Value = cboActiveLayer.SelectedItem.ToString();

                //handles no. 8 here, 
                hd_childnode_8.AppendChild(hd_textNode_8);//n
                if (lbFieldsActiveLayer.SelectedItem.ToString() == "")
                {
                    MessageBox.Show("Please select a valid field name first.");
                    tabControl2.SelectedIndex = 0;
                    return;
                }
                hd_textNode_8.Value = lbFieldsActiveLayer.SelectedItem.ToString();

                //save file
                hd_xmlDoc.Save(filename);

                //clear Textbox1 and update listBox8 with new search-names
                tboSearchName.Text = "";
                lboRestoreSearch.Items.Clear();//c1
                Populate_xmlcombo();
            }
            catch (Exception ex)
            {
                WriteError(ex.ToString());
            }
            groupPreview.Visible = false;
        }

        static void WriteError(string exceptionAsString, string SuggestedUserAction)
        {

            const string errorformat =
                "Sorry an Error Occured: \n{0} \nDetails \n{1}";
            if (!String.IsNullOrEmpty(SuggestedUserAction))
            {
                MessageBox.Show(String.Format(errorformat, SuggestedUserAction, exceptionAsString));
            }
            else
            {
                MessageBox.Show(String.Format(errorformat, "", exceptionAsString));
            }
        }

        void WriteError(string exceptionAsString)
        {
            WriteError(exceptionAsString, null);
        }

        //On load populates the combo box...
        private void Populate_xmlcombo()
        {
            string fname = Application.StartupPath + "\\q_save.xml";
            log.Debug("Populate_xmlcombo: Reading  file " + fname);
            XmlDocument doc = new XmlDocument();

            try
            {
                doc.Load(fname);
            }
            catch (System.IO.FileNotFoundException)
            {
                doc = CreateNewSearchFile(fname);
            }
            catch (Exception ex)
            {
                log.Error("Populate_xmlcombo: Reading  file " + fname + "execption " + ex.Message);
                //WriteError ( ex.ToString (), "Previous searches not loaded" );
                doc = CreateNewSearchFile(fname);
            }

            XmlNodeList query_name = doc.SelectNodes("Gro/cNode_A");

            for (int i = 0; i < query_name.Count; i++)
            {
                XmlNode node = query_name[i];
                XmlAttributeCollection attcol = node.Attributes;
                lboRestoreSearch.Items.Add(attcol[0].Value);//c1
            }
        }

        public XmlDocument CreateNewSearchFile(string fname)
        {
            XmlDocument doc = new XmlDocument(); ;
            log.Debug("Populate_xmlcombo: new search file " + fname);
            //file is not found and therefore lets create a new xml file here
            XmlTextWriter hd_xmlWriter = new XmlTextWriter(fname, System.Text.Encoding.UTF8);
            hd_xmlWriter.Formatting = Formatting.Indented;
            hd_xmlWriter.WriteProcessingInstruction("xml", "version='1.0' encoding='UTF-8'");
            hd_xmlWriter.WriteStartElement("Gro");
            hd_xmlWriter.Close();
            doc.Load(fname);
            return doc;
        }

        void checkPrevious(string cname)
        {
            for (int t = 0; t < lboRestoreSearch.Items.Count; t++)//c1
            {
                if (cname == lboRestoreSearch.Items[t].ToString())//c1
                {
                    return;
                }
            }
        }
        #region xml driven restore search
        private void btnRestoreSearch_Click(object sender, EventArgs e)
        {
            //clear the items in listBox
            lbKeywords.Items.Clear();
            if (lboRestoreSearch.SelectedIndex == -1)
            {
                MessageBox.Show("Please select a valid 'Search Name' to restore.");
                return;
            }

            //Load the keywords and webservices
            button10_Click(null, null);//for keywords

            // issue 7244. If HIS central not working or slow issue with HD
            RefreshWebServices(false, false);//for webservices

            //Load all fields saved in XML to the form components

            //First, lets take care of area
            bool shapefound = false;
            bool fieldfound = false;
            for (int i = 0; i <= cboActiveLayer.Items.Count - 1; i++)
            {
                if (cboActiveLayer.Items[i].ToString() == lblRestoreALayer.Text)
                {
                    cboActiveLayer.SelectedIndex = i;
                    shapefound = true;
                    for (int j = 0; j < lbFieldsActiveLayer.Items.Count - 1; j++)
                    {
                        if (lbFieldsActiveLayer.Items[j].ToString() == lblRestoreFName.Text)
                        {
                            lbFieldsActiveLayer.SelectedIndex = j;
                            fieldfound = true;
                        }
                    }
                }

            }
            if (!shapefound)
            {
                MessageBox.Show("The target Layer (" + lblRestoreALayer.Text + ") not found; Please load the valid layer first.");
                return;
            }
            if (!fieldfound)
            {
                MessageBox.Show("The target field (" + lblRestoreFName.Text + ") in layer (" + lblRestoreALayer.Text + ") does not exist; Please check again.");
                return;
            }
            listBox4.Items.Clear();
            listBox7.Items.Clear();

            if (lblRestoreParameters.Text != "")
            {
                string areaLine = lblRestoreParameters.Text;
                string[] areaLineArray = Regex.Split(areaLine, " ::: ");

                //special case: restore search area rectangle
                if (areaLineArray.Length == 5)
                {
                    if (areaLineArray[0] == "Rectangle")
                    {
                        //set the labels for 'area rectangle'
                        btnSelectRectangle_Click(null, null);
                        double minLon = Convert.ToDouble(areaLineArray[1], CultureInfo.InvariantCulture);
                        double minLat = Convert.ToDouble(areaLineArray[2], CultureInfo.InvariantCulture);
                        double maxLon = Convert.ToDouble(areaLineArray[3], CultureInfo.InvariantCulture);
                        double maxLat = Convert.ToDouble(areaLineArray[4], CultureInfo.InvariantCulture);

                        _rectangleDrawing.RestoreSearchRectangle(minLon, minLat, maxLon, maxLat);
                    }
                }

                foreach (string aWord in areaLineArray)
                {
                    listBox7.Items.Add(aWord);
                }
            }
            //select appropriate datagrid rows

            //for clearing the existing selection
            for (int n = 0; n <= dgvSearch.Rows.Count - 1; n++)
            {
                dgvSearch.Rows[n].Selected = false;
            }
            //for applying the target selection        
            for (int n = 0; n <= dgvSearch.Rows.Count - 1; n++)
            {
                for (int m = 0; m <= listBox7.Items.Count - 1; m++)
                {
                    int lbs = lbFieldsActiveLayer.SelectedIndex;
                    if (listBox7.Items[m].ToString() == dgvSearch.Rows[n].Cells[lbs].Value.ToString())
                    {
                        //MessageBox.Show(n.ToString());
                        dgvSearch.Rows[n].Selected = true;
                    }
                }
            }

            dgvSearch.RefreshMapSelection();

            //Second take care of webservices

            if (lblRestoreWSValues.Text != "")
            {
                for (int i = 0; i < treeViewWebServices.Nodes.Count; i++)
                {
                    treeViewWebServices.Nodes[i].Checked = false;
                }
                string webservicesLine = lblRestoreWSValues.Text;
                if (webservicesLine == "All Webservices selected")
                {
                    for (int i = 0; i < treeViewWebServices.Nodes.Count; i++)
                    {
                        treeViewWebServices.Nodes[i].Checked = true;
                    }
                }
                else
                {
                    string[] webservicesLineArray = Regex.Split(webservicesLine, " ::: ");
                    foreach (string wsWord in webservicesLineArray)
                    {
                        for (int i = 0; i < treeViewWebServices.Nodes.Count; i++)
                        {
                            if (wsWord == treeViewWebServices.Nodes[i].Text)
                            {
                                treeViewWebServices.Nodes[i].Checked = true;
                            }
                        }
                    }
                }
                fillWebservicesXml();
                groupPreview.Visible = false;
                lboRestoreSearch.SelectedIndex = -1;
            }




            //third take care of Keywords
            lbSelectedKeywords.Items.Clear();
            if (label31.Text != "")
            {
                string keywordsLine = label31.Text;
                string[] keywordsLineArray = Regex.Split(keywordsLine, " ::: ");
                foreach (string kWord in keywordsLineArray)
                {
                    lbSelectedKeywords.Items.Add(kWord);
                }
            }
            //4th, take care of dates
            if (lblRestoreDates.Text != "")
            {
                string dateLine = lblRestoreDates.Text;
                string[] dateLineArray = Regex.Split(dateLine, " ::: ");
                //MessageBox.Show(dateLineArray[0].ToString());
                dateTimePickStart.Text = dateLineArray[0].ToString();
                dateTimePickEnd.Text = dateLineArray[1].ToString();

            }
            if (lblRestoreSName.Text != "")
            {
                string serverLine = lblRestoreSName.Text;
                string[] serverLineArray = Regex.Split(serverLine, " ::: ");
                lblServerValue.Text = serverLineArray[0].ToString();
            }
            //upload the current xml
            fillAreaXml();
            fillXml();

            //goback to tabView1
            tabControl2.SelectedIndex = 2;
        }

        #endregion xml driven restore search

        #region Ontology keywords - from metadata cache DB
        private void FillKeywordsFromDB()
        {
            tboTypeKeyword.Clear();
            lblKeywordRelation.Text = "";

            lbKeywords.Items.Clear();
            MetadataCacheSearcher searcher = new MetadataCacheSearcher();
            List<string> keywords = searcher.GetKeywords();
            keywords.Add("Hydrosphere");
            keywords.Sort();

            //fill the list box and the tree
            treeviewOntology.Nodes.Clear();
            TreeNode parentNode = treeviewOntology.Nodes.Add("Hydrosphere");

            foreach (string keyword in keywords)
            {
                if (!lbKeywords.Items.Contains(keyword))
                {
                    lbKeywords.Items.Add(keyword);
                }
                if (keyword != "Hydrosphere")
                {
                    parentNode.Nodes.Add(keyword);
                }
            }



        }
        #endregion ontology keywords - from metadata cache db

        private void button1_Click(object sender, EventArgs e)
        {
            //panel1.Visible = false;
            //CheckBox1.Checked = false;
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            mapMain.FunctionMode = FunctionMode.Select;
        }

        private void AddNode(XmlNode inXmlNode, TreeNode inTreeNode)
        {
            XmlNode xNode;
            TreeNode tNode;
            XmlNodeList nodeList;
            int i;

            // Loop through the XML nodes until the leaf is reached.
            // Add the nodes to the TreeView during the looping process.
            if (inXmlNode.HasChildNodes)
            {
                nodeList = inXmlNode.ChildNodes;
                for (i = 0; i <= nodeList.Count - 1; i++)
                {
                    xNode = inXmlNode.ChildNodes[i];
                    inTreeNode.Nodes.Add(new TreeNode(xNode.Name));
                    tNode = inTreeNode.Nodes[i];
                    AddNode(xNode, tNode);
                }
            }
            else
            {
                // Here you need to pull the data from the XmlNode based on the
                // type of node, whether attribute values are required, and so forth.
                inTreeNode.Text = (inXmlNode.OuterXml).Trim();
            }
        }

        private void button10_Click(object sender, EventArgs e)
        {
            try
            {
                //txtFilename.Text = filename;
                treeviewOntology.Nodes.Clear();
                //XmlDocument tmpxmldoc = new XmlDocument();
                //tmpxmldoc.Load ( Path.Combine(Application.StartupPath,_ontologyFilename ));

                var tmpxmldoc = HdSearchOntologyHelper.ReadOntologyXmlFile();
                FillTree(tmpxmldoc.DocumentElement, treeviewOntology.Nodes);

            }
            catch (Exception ex)
            {
                tboTypeKeyword.Text = ex.Message;
            }
            //adding synonyms to listbox1
            var tmpsyndoc = HdSearchOntologyHelper.ReadOntologySymbologyXmlFile();
            XmlNodeList nList = tmpsyndoc.GetElementsByTagName("SearchableKeyword");
            foreach (XmlNode nod in nList)
            {
                XmlElement elem = (XmlElement)nod;
                if (!lbKeywords.Items.Contains(elem.InnerText))
                {
                    lbKeywords.Items.Add(elem.InnerText);
                }
            }

        }

        private void FillTree(XmlNode node, TreeNodeCollection parentnode)
        {
            // End recursion if the node is a text type
            if (node == null || node.NodeType == XmlNodeType.Text || node.NodeType == XmlNodeType.CDATA)
                return;
            TreeNodeCollection tmptreenodecollection = AddNodeToTree(node, parentnode);
            // Add all the children of the current node to the treeview
            foreach (XmlNode tmpchildnode in node.ChildNodes)
            {
                if (tmpchildnode.Name == "childNodes")
                {
                    //FillTree(tmpchildnode, tmptreenodecollection);

                    foreach (XmlNode tmpchildnode2 in tmpchildnode.ChildNodes)
                    {
                        FillTree(tmpchildnode2, tmptreenodecollection);

                    }
                }
            }
        }
        private TreeNodeCollection
        AddNodeToTree(XmlNode node, TreeNodeCollection parentnode)
        {
            TreeNode newchildnode = CreateTreeNodeFromXmlNode(node);
            // if nothing to add, return the parent item
            if (newchildnode == null) return parentnode;
            // add the newly created tree node to its parent
            if (parentnode != null) parentnode.Add(newchildnode);
            return newchildnode.Nodes;
        }
        private TreeNode CreateTreeNodeFromXmlNode(XmlNode node)
        {
            TreeNode tmptreenode = new TreeNode();
            if (node.HasChildNodes)
            {
                if (node.FirstChild.InnerText != string.Empty)
                {
                    tmptreenode = new TreeNode(node.FirstChild.InnerText);
                    //for adding the keywords to the listbox
                    //checkedListBox1.Items.Add(node.FirstChild.InnerText);
                    if (!lbKeywords.Items.Contains(node.FirstChild.InnerText))
                    {
                        lbKeywords.Items.Add(node.FirstChild.InnerText);
                    }
                    //to maintain a master-list for differentiating from synonyms
                    lboKeywordSupport.Items.Add(node.FirstChild.InnerText);
                }
            }
            return tmptreenode;
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            //control display if user is in treeview mode
            if (rbTree.Checked == true)
            {
                rbList.Checked = true;
            }

            //string searchstring2;
            lbKeywords.ClearSelected();
            string searchString2 = tboTypeKeyword.Text;
            lbKeywords.SelectionMode = System.Windows.Forms.SelectionMode.MultiSimple;
            int x = -1;

            if (searchString2.Length != 0)
            {
                do
                {
                    x = lbKeywords.FindString(searchString2, x);
                    if (x != -1)
                    {
                        if (lbKeywords.SelectedIndices.Count > 0)
                        {
                            if (x == lbKeywords.SelectedIndices[0])
                                return;
                        }
                        lbKeywords.SetSelected(x, true);
                    }
                }
                while (x != -1);
            }
        }

        private void FindInTreeView(TreeNodeCollection tncoll, string strNode)
        {

            foreach (TreeNode tnode in tncoll)
            {
                if (tnode.Text.ToLower() == strNode.ToLower())
                {
                    tnode.TreeView.SelectedNode = tnode;
                    lblKeywordRelation.Text = tnode.FullPath;

                    //special case for 'hydrosphere when search mode is metadata cache
                    if (SearchMode == Properties.Settings.Default.SearchMethod_MetadataCache)
                    {
                        if (strNode.ToLower() == "hydrosphere") return;
                    }
                }

                FindInTreeView(tnode.Nodes, strNode);
            }
        }

        private void listBox1_MouseUp(object sender, MouseEventArgs e)
        {
            tboTypeKeyword.Text = "";
            int keyIndex = lbKeywords.IndexFromPoint(e.Location);
            string strNode = lbKeywords.Items[keyIndex].ToString();
            //check for synonym
            //only check for synonym when Search mode is HIS Central
            if (SearchMode == Resources.SearchMode_HISCentral)
            {
                //doc.Load(Application.StartupPath + "\\Synonyms.xml");
                var doc = HdSearchOntologyHelper.ReadOntologySymbologyXmlFile();

                XmlNodeList nList = doc.GetElementsByTagName("SearchableKeyword");

                foreach (XmlNode node in nList)
                {
                    if (node.InnerText.ToLower() == strNode.ToLower())
                    {
                        tboTypeKeyword.Text = node.NextSibling.InnerText;
                        FindInTreeView(treeviewOntology.Nodes, tboTypeKeyword.Text);
                    }
                }
            }
            //check ends
            if (tboTypeKeyword.Text == "")
            {
                tboTypeKeyword.Text = lbKeywords.Items[keyIndex].ToString();
                FindInTreeView(treeviewOntology.Nodes, tboTypeKeyword.Text);
            }
        }

        //click btnAddKeyword
        private void button12_Click(object sender, EventArgs e)
        {
            if (SearchMode == Resources.SearchMode_HISCentral)
            {

                if (lbSelectedKeywords.Items.Count < 1)
                {
                    if (lbKeywords.SelectedIndex == -1)
                    {
                        MessageBox.Show("Please select a valid Keyword to add.");
                        return;
                    }
                    else
                    {
                        if (lboKeywordSupport.Items.Contains(lbKeywords.SelectedItem) == true)
                        {
                            lbSelectedKeywords.Items.Add(lbKeywords.SelectedItem);
                            fillXml();
                        }
                        //synonym identified and therefore the related keyword needs to be loaded here
                        else
                        {
                            //adding synonyms to listbox1
                            //XmlDocument doc = new XmlDocument ();
                            //doc.Load ( Application.StartupPath + "\\Synonyms.xml" );

                            XmlDocument doc = HdSearchOntologyHelper.ReadOntologySymbologyXmlFile();

                            XmlNodeList nList = doc.GetElementsByTagName("SearchableKeyword");

                            foreach (XmlNode node in nList)
                            {
                                if (node.InnerText == lbKeywords.SelectedItem.ToString())
                                {
                                    lbSelectedKeywords.Items.Add(node.NextSibling.InnerText);
                                    fillXml();
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (lbKeywords.SelectedIndex == -1)
                    {
                        MessageBox.Show("Please select a valid Keyword.");
                        return;
                    }
                    for (int i = 0; i < lbSelectedKeywords.Items.Count; i++)
                    {
                        if (lbKeywords.SelectedItem.ToString() == lbSelectedKeywords.Items[i].ToString())
                        {
                            MessageBox.Show("This Keyword is already selected, Please select another keyword.");
                            return;
                        }
                    }
                    if (lboKeywordSupport.Items.Contains(lbKeywords.SelectedItem) == true)
                    {
                        lbSelectedKeywords.Items.Add(lbKeywords.SelectedItem);
                        fillXml();
                    }
                    //synonym identified and therefore the related keyword needs to be loaded here
                    else
                    {
                        //adding synonyms to listbox1
                        //XmlDocument doc = new XmlDocument ();
                        //doc.Load ( Application.StartupPath + "\\Synonyms.xml" );

                        XmlDocument doc = HdSearchOntologyHelper.ReadOntologySymbologyXmlFile();

                        XmlNodeList nList = doc.GetElementsByTagName("SearchableKeyword");

                        foreach (XmlNode node in nList)
                        {
                            if (node.InnerText == lbKeywords.SelectedItem.ToString())
                            {
                                if (lbSelectedKeywords.Items.Contains(node.NextSibling.InnerText))
                                {
                                    MessageBox.Show("The Keyword " + node.NextSibling.InnerText + " is already selected.");
                                    return;
                                }
                                else lbSelectedKeywords.Items.Add(node.NextSibling.InnerText);
                                fillXml();
                            }
                        }
                    }
                }
            }
            else if (SearchMode == "Local Metadata Cache")
            {
                if (lbKeywords.SelectedIndex == -1)
                {
                    MessageBox.Show("Please select a valid Keyword.");
                    return;
                }
                for (int i = 0; i < lbSelectedKeywords.Items.Count; i++)
                {
                    if (lbKeywords.SelectedItem.ToString() == lbSelectedKeywords.Items[i].ToString())
                    {
                        MessageBox.Show("This Keyword is already selected, Please select another keyword.");
                        return;
                    }
                }

                lbSelectedKeywords.Items.Add(lbKeywords.SelectedItem);
                fillXml();

            }
        }

        private void button15_Click(object sender, EventArgs e)
        {
            while (lbSelectedKeywords.SelectedItem != null)
            {
                lbSelectedKeywords.Items.Remove(lbSelectedKeywords.SelectedItem);
                fillXml();
            }
        }

        private void tvOntology_AfterSelect(object sender, TreeViewEventArgs e)
        {
            tboTypeKeyword.Text = e.Node.Text;
            FindInTreeView(treeviewOntology.Nodes, tboTypeKeyword.Text);
        }

        // getting information in Label
        private int _currentIndex = -1;
        private void ShowNextItem(ListBox listBox, Label label)
        {
            _currentIndex++;
            if (_currentIndex >= listBox.Items.Count)
            {
                _currentIndex = 0;
            }
            label.Text = listBox.Items[_currentIndex].ToString();
        }

        private void fillXml()
        {
            string sValue = "";

            for (int i = 0; i < lbSelectedKeywords.Items.Count; i++)
            {
                if (i == 0)
                {
                    sValue = Convert.ToString(lbSelectedKeywords.Items[0]);
                }
                else
                    sValue = sValue + " ::: " + Convert.ToString(lbSelectedKeywords.Items[i]);
            }
            lblKeywordsValue.Text = sValue;
        }
        private void fillAreaXml()
        {
            string sValue = String.Empty;

            //separate handling for rectnagle area
            if (_rectangleDrawing != null)
            {
                if (_rectangleDrawing.RectangleLayerIsInMap())
                {
                    sValue = "Rectangle ::: ";
                }
            }

            for (int i = 0; i < listBox4.Items.Count; i++)
            {
                if (i == 0)
                {
                    sValue = sValue + Convert.ToString(listBox4.Items[0]);
                }
                else
                    sValue = sValue + " ::: " + Convert.ToString(listBox4.Items[i]);
            }
            lblAreaPara.Text = sValue;
        }


        //filling webservices from a treeview

        private void fillWebservicesXml()
        {
            string sValue = "";
            int sCount = 0;
            lbSelectedWebServices.Items.Clear();
            lbWebservicesSupport.Items.Clear();
            for (int i = 0; i < treeViewWebServices.Nodes.Count; i++)
            {
                if (treeViewWebServices.Nodes[i].Checked)
                {
                    lbSelectedWebServices.Items.Add(treeViewWebServices.Nodes[i].Text);

                    //add the serviceID to the hidden listBox
                    lbWebservicesSupport.Items.Add(treeViewWebServices.Nodes[i].Name);
                    //create the counter for checked nodes
                    sCount = sCount + 1;
                }
            }
            if (lbSelectedWebServices.Items.Count < 1)
            {
                lblWebServValue.Text = "";
                return;
            }
            else
                for (int j = 0; j < lbSelectedWebServices.Items.Count; j++)
                {
                    if (j == 0)
                    {
                        sValue = lbSelectedWebServices.Items[j].ToString();
                    }
                    else
                        sValue = sValue + " ::: " + lbSelectedWebServices.Items[j].ToString();
                }
            if (sCount == treeViewWebServices.Nodes.Count)
            {
                lblWebServValue.Text = "All Webservices selected";
            }
            else
            {
                lblWebServValue.Text = sValue;
            }
        }




        public void dataChange()
        {
            lblDateValue.Text = dateTimePickStart.Value.ToShortDateString() + " ::: " + dateTimePickEnd.Value.ToShortDateString();
        }

        private void DateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            dataChange();
        }

        private void DateTimePicker2_ValueChanged(object sender, EventArgs e)
        {
            dataChange();
        }

        private void CloseSearchPanel()
        {
            _mapArgs.Map.MapFrame.ResetExtents();
        }
        /////trial subpart ends.....


        #region Fill webservices treeview control
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //fill webservices treeview control///////////////////////////////////

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            RefreshWebServices(true, true);
        }

        public void RefreshWebServices(bool forceRefresh, bool showMessage)
        {
            try
            {
                //different method for HIS Central vs. Metadata Cache
                if (SearchMode == Resources.SearchMode_HISCentral)
                {

                    treeViewWebServices.Nodes.Clear();
                    XmlDocument getWebServ = _webServicesList.GetWebServicesList(forceRefresh, showMessage);
                    //_webServicesList.UpdateServiceIcons();

                    FillWebTree(getWebServ.DocumentElement, treeViewWebServices.Nodes);
                    treeViewWebServices.Sort();
                }
                else
                {
                    treeViewWebServices.Nodes.Clear();
                    FillWebServicesFromDB();
                }

                CheckAllWebServices();
            }
            catch (Exception ex)
            {
                label4.Text = ex.Message;
            }
        }

        //to check all web services (default)    
        private void CheckAllWebServices()
        {
            if (treeViewWebServices.Nodes.Count > 0)
            {
                foreach (TreeNode tnode in treeViewWebServices.Nodes)
                {
                    tnode.Checked = true;
                }
                fillWebservicesXml();
            }
        }

        private void FillWebTree(XmlNode node, TreeNodeCollection parentNode)
        {
            foreach (XmlNode childNode1 in node.ChildNodes)
            {
                if (childNode1.Name == "ServiceInfo")
                {

                    TreeNode treeNode1 = new TreeNode();
                    foreach (XmlNode childNode2 in childNode1.ChildNodes)
                    {
                        if (childNode2.Name == "Title")
                        {
                            treeNode1.Text = childNode2.InnerText;
                        }
                        else if (childNode2.Name == "ServiceID")
                        {
                            //set the name of the tree node to the ServiceID
                            treeNode1.Name = childNode2.InnerText;

                            TreeNode treeNode2 = new TreeNode(childNode2.Name);
                            treeNode1.Nodes.Add(treeNode2);

                            TreeNode treeNode3 = new TreeNode(childNode2.InnerText);
                            treeNode2.Nodes.Add(treeNode3);
                        }
                        else
                        {
                            TreeNode treeNode2 = new TreeNode(childNode2.Name);
                            treeNode1.Nodes.Add(treeNode2);

                            TreeNode treeNode3 = new TreeNode(childNode2.InnerText);
                            treeNode2.Nodes.Add(treeNode3);
                        }
                    }
                    //add the 'Web Service' tree node to tree view
                    parentNode.Add(treeNode1);
                }
            }
        }

        //fills the 'web services' treeview from the metadata cache database
        private void FillWebServicesFromDB()
        {
            try
            {
                treeViewWebServices.Nodes.Clear();

                MetadataCacheSearcher searcher = new MetadataCacheSearcher();
                List<DataServiceInfo> serviceList = searcher.GetWebServices();

                foreach (DataServiceInfo service in serviceList)
                {
                    TreeNode treeNode1 = new TreeNode();
                    treeNode1.Text = service.ServiceTitle;
                    treeNode1.Name = service.Id.ToString();
                    treeViewWebServices.Nodes.Add(treeNode1);
                }
                treeViewWebServices.Sort();

                lbSelectedWebServices.Items.Clear();
            }
            catch (Exception ex)
            {
                label4.Text = ex.Message;
            }
        }

        #endregion Fill webservices treeview control

        private void SearchControl_Load(object sender, EventArgs e)
        {
            //removes existing for avoiding multiple entries...
            cboActiveLayer.Items.Clear();
            AddPolygonLayers();

            lblServerValue.Text = Resources.SearchMode_HISCentral;

            //ensure loading of webservices and keywords on form load
            // source of issue 7244. If HIS central not working or slow issue with HD
            RefreshWebServices(true, false);

            ////to ensure at least one layer is selected.
            if (cboActiveLayer.Items.Count >= 1)
            {
                cboActiveLayer.SelectedIndex = 0;

                //HACK: to find the corresponding layer
                List<IMapPolygonLayer> layerList = GetListOfPolygonLayers();
                foreach (MapPolygonLayer layer in layerList)
                {
                    if (layer.LegendText == cboActiveLayer.SelectedItem.ToString())
                    {
                        layer.Select(0);
                        //_mapArgs.Map.MapFrame.ResetBuffer();
                    }
                }
                //HACK ends
                //date time picker moved from Main (ribbon-click to form load)
                dateTimePickStart.Value = DateTime.Now.Date.AddYears(-5);//change suggested by Dan (range 5 years)
                dateTimePickEnd.Value = DateTime.Now.Date;
            }
            //re-setting datagrid row selection
        }
        #region Layer Added for Area Search

        /// <summary>
        /// If a polygon layer is added, update the list of polygon layers in the search control.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void MapFrame_LayerAdded(object sender, DotSpatial.Symbology.LayerEventArgs e)
        {
            // See if the layer is a polygon layer or a group layer containing polygon layers.
            if (e.Layer is IMapLayer)
            {
                List<IMapPolygonLayer> polygonLayerList = GetListOfPolygonLayers();
                //GetListOfPolygonLayers(e.Layer as IMapLayer, polygonLayerList);

                if (polygonLayerList.Count > 0)
                {
                    // Polygon layer added.  We need to update the combo box of layers. 

                    // Keep track of the currently selected layer, if any.
                    string selectedLayerName = "";
                    if (cboActiveLayer.SelectedItem != null)
                    {
                        selectedLayerName = cboActiveLayer.SelectedItem.ToString();
                    }

                    // Repopulate items in the combo box.  !!! Really we should just be adding the new items in the right order.
                    if (!String.IsNullOrEmpty(e.Layer.LegendText))
                    {
                        cboActiveLayer.BeginUpdate();
                        //cboActiveLayer.Items.Clear();
                        //AddPolygonLayers();
                        cboActiveLayer.Items.Add(e.Layer.LegendText);
                        cboActiveLayer.EndUpdate();
                    }

                    // Reselect the previously selected layer name, if present.
                    int index = cboActiveLayer.FindString(selectedLayerName);
                    if (index != -1)
                    {
                        // Turn off the event handler.  We don't need to fire events for reselecting the same layer that we had selected before.
                        cboActiveLayer.SelectedIndexChanged -= new System.EventHandler(this.cboActiveLayer_SelectedIndexChanged);

                        // Select the layer name.
                        cboActiveLayer.SelectedIndex = index;

                        // Turn on the event handler.
                        cboActiveLayer.SelectedIndexChanged += new System.EventHandler(this.cboActiveLayer_SelectedIndexChanged);
                    }
                }
            }
        }

        /// <summary>
        /// If a polygon layer is removed, update the list of polygon layers in the search control.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void MapFrame_LayerRemoved(object sender, DotSpatial.Symbology.LayerEventArgs e)
        {
            // See if the layer is a polygon layer or a group layer containing polygon layers.
            if (e.Layer is IMapLayer)
            {
                List<IMapPolygonLayer> polygonLayerList = GetListOfPolygonLayers();

                if (polygonLayerList.Count > 0)
                {
                    // Polygon layer removed.  We need to update the combo box of layers. 

                    // Keep track of the currently selected layer, if any.
                    string selectedLayerName = "";
                    if (cboActiveLayer.SelectedItem != null)
                    {
                        selectedLayerName = cboActiveLayer.SelectedItem.ToString();
                    }

                    // Repopulate items in the combo box.  !!! Really we should just be adding the new items in the right order.
                    cboActiveLayer.BeginUpdate();
                    cboActiveLayer.Items.Clear();
                    AddPolygonLayers();
                    cboActiveLayer.EndUpdate();

                    // Reselect the previously selected layer name, if present.
                    int index = cboActiveLayer.FindString(selectedLayerName);
                    if (index != -1)
                    {
                        // Turn off the event handler.  We don't need to fire events for reselecting the same layer that we had selected before.
                        cboActiveLayer.SelectedIndexChanged -= new System.EventHandler(this.cboActiveLayer_SelectedIndexChanged);

                        // Select the layer name.
                        cboActiveLayer.SelectedIndex = index;

                        // Turn on the event handler.
                        cboActiveLayer.SelectedIndexChanged += new System.EventHandler(this.cboActiveLayer_SelectedIndexChanged);
                    }
                }
            }
        }

        private void AddPolygonLayers()
        {
            cboActiveLayer.Items.Clear();
            List<IMapPolygonLayer> polygonLayers = this.GetListOfPolygonLayers();
            foreach (IMapPolygonLayer pg in polygonLayers)
            {
                cboActiveLayer.Items.Add(pg.LegendText);
            }
        }



        /// <summary>
        /// Finds polygon layers  in the map
        /// </summary>
        private List<IMapPolygonLayer> GetListOfPolygonLayers()
        {
            Map map1 = _mapArgs.Map as Map;
            List<IMapPolygonLayer> polygonLayers = new List<IMapPolygonLayer>();
            List<ILayer> allLayers = map1.GetAllLayers();
            foreach (ILayer lay in allLayers)
            {
                IMapPolygonLayer polyLay = lay as IMapPolygonLayer;
                if (polyLay != null)
                {
                    polygonLayers.Add(polyLay);
                }
            }
            return polygonLayers;
        }

        #endregion Layer Added for Area Search

        private void PictureBox5_Click_1(object sender, EventArgs e)
        {
            CloseSearchPanel();
        }
        #region Select Appropriate Layer from the Map
        //selecting the appropriate layer from the Map on change of combobox button//
        private void cboActiveLayer_SelectedIndexChanged(object sender, EventArgs e)
        {
            //lbFieldsActiveLayer.SelectedIndexChanged -= this.lbFieldsActiveLayer_SelectedIndexChanged;

            lbFieldsActiveLayer.Items.Clear();
            listBox4.Items.Clear();
            selectPolygonLayer();
            if (lbFieldsActiveLayer.Items.Count > 0) lbFieldsActiveLayer.SelectedIndex = 0;

            //to de-activate the rectangle drawing
            //check for rectangle select mode
            if (rectSelectMode == false)
            {
                dataGridView1_Click(null, null);
                ClearRectangle();
                mapMain.FunctionMode = FunctionMode.Select;
            }
            else
            {
                mapMain.FunctionMode = FunctionMode.None;
            }

            //lbFieldsActiveLayer.SelectedIndexChanged += this.lbFieldsActiveLayer_SelectedIndexChanged;

        }


        private void selectPolygonLayer()
        {
            _mapArgs.Map.MapFrame.IsSelected = false;
            foreach (IMapGroup grp in _mapArgs.Map.MapFrame.GetAllGroups())
            {
                grp.IsSelected = false;
            }
            foreach (IMapLayer lay in _mapArgs.Map.MapFrame.GetAllLayers())
            {
                lay.IsSelected = false;
                IMapPolygonLayer pg_s = lay as IMapPolygonLayer;
                if (pg_s != null)
                {
                    if (pg_s.LegendText == cboActiveLayer.SelectedItem.ToString())
                    {
                        pg_s.IsSelected = true;
                        pg_s.IsVisible = true;

                        _mapArgs.Legend.RefreshNodes();
                        lbFieldsActiveLayer.Items.Clear();
                        for (int i = 0; i <= pg_s.DataSet.DataTable.Columns.Count - 1; i++)
                        {
                            lbFieldsActiveLayer.Items.Add(pg_s.DataSet.DataTable.Columns[i].ColumnName);
                        }
                        dgvSearch.SetDataSource(pg_s);
                        //try to select Aruba
                        //check for rectangle select mode
                        if (rectSelectMode == false) dataGridView1_Click(null, null);
                    }
                }
            }
        }

        #endregion Select Appropriate Layer from the Map

        #region datagridview - listbox data exchange
        //selecting rows of the datagrid
        public void dataGridView1_Click(object sender, EventArgs e)
        {
            Int32 selectedRowCount =
                dgvSearch.Rows.GetRowCount(DataGridViewElementStates.Selected);


            //get the count of rows selected for reference and clear the listbox

            label16.Text = selectedRowCount != 1 ? selectedRowCount + " features selected" : "1 feature selected";

            listBox4.Items.Clear();
            int j = lbFieldsActiveLayer.SelectedIndex;
            if (selectedRowCount > 0 && lbFieldsActiveLayer.SelectedIndex >= 0)
            {

                for (int i = 0; i < selectedRowCount; i++)
                {
                    // listBox4.Items.Add(dataGridView1.SelectedRows[i].Index.ToString());
                    listBox4.Items.Add(dgvSearch.Rows[dgvSearch.SelectedRows[i].Index].Cells[j].FormattedValue.ToString());

                }

            }

            fillAreaXml();

            //to de-activate the rectangle drawing
            ClearRectangle();
        }

        public void listBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            //for redrawing the listbox as per new configuration
            dataGridView1_Click(null, null);
            //for controlling the width of datagrid on listbox column change
            for (int i = 0; i < dgvSearch.Columns.Count; i++)
            {
                //dataGridView1.Columns[i].Width = 0;
                dgvSearch.Columns[i].Visible = false;
                int j = lbFieldsActiveLayer.SelectedIndex;
                if (dgvSearch.Columns[i].Name == lbFieldsActiveLayer.Items[j].ToString())
                {
                    //dataGridView1.Columns[i].Width = 100;
                    dgvSearch.Columns[i].Visible = true;
                    DataGridViewColumn Column = dgvSearch.Columns[i];
                    Column.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

                    //Order the column selected in the "Select Field" by alphabet
                    ListSortDirection direction = ListSortDirection.Ascending;
                    dgvSearch.Sort(Column, direction);
                }
            }

            //to de-activate the rectangle drawing
            ClearRectangle();
        }
        void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            dataGridView1_Click(null, null);
        }
        #endregion datagridview - listbox data exchange

        #region TabControl button management

        private void button5_Click(object sender, EventArgs e)
        {
            tabControl2.SelectedIndex = 2;
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            tabControl2.SelectedIndex = 0;
        }

        private void Button7_Click(object sender, EventArgs e)
        {
            tabControl2.SelectedIndex = 1;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            tabControl2.SelectedIndex = 0;
        }

        #region Draw Rectangle on Map
        private bool rectSelectMode;

        private void btnSelectRectangle_Click(object sender, EventArgs e)
        {
            //bool rectangle selection mode control
            rectSelectMode = true;

            dgvSearch.ClearSelection();
            dgvSearch.RefreshMapSelection();
            //clear the listBox
            listBox4.Items.Clear();
            listBox4.Visible = true;

            if (_rectangleDrawing == null)
            {
                _rectangleDrawing = new RectangleDrawing(_mapArgs.Map);
                _rectangleDrawing.RectangleCreated += new EventHandler(rectangleDrawing_RectangleCreated);

            }
            _rectangleDrawing.Activate();

            //make 'area search' components invisible for reducing clutter
            label5.Visible = false;
            label14.Visible = false;
            label15.Visible = false;
            cboActiveLayer.Visible = false;
            lbFieldsActiveLayer.Visible = false;
            dgvSearch.Visible = false;
            label32.Visible = true;
            label47.Visible = true;
            //search summary labels
            label16.Visible = false;
        }

        /// <summary>
        /// Occurs when a rectangle is drawn
        /// </summary>
        void rectangleDrawing_RectangleCreated(object sender, EventArgs e)
        {
            if (_rectangleDrawing == null) return;

            double xMin = _rectangleDrawing.RectangleExtent.MinX;
            double yMin = _rectangleDrawing.RectangleExtent.MinY;
            double xMax = _rectangleDrawing.RectangleExtent.MaxX;
            double yMax = _rectangleDrawing.RectangleExtent.MaxY;

            double[] xy = new double[] { xMin, yMin, xMax, yMax };

            string esri = Resources.wgs_84_esri_string;
            ProjectionInfo wgs84 = new ProjectionInfo();
            wgs84.ReadEsriString(esri);

            Reproject.ReprojectPoints(xy, new double[] { 0, 0 }, _mapArgs.Map.Projection, wgs84, 0, 2);
            //populate the listbox
            listBox4.Items.Clear();
            listBox4.Items.Add(String.Format(CultureInfo.InvariantCulture, "{0:N6}", xy[0]));
            listBox4.Items.Add(String.Format(CultureInfo.InvariantCulture, "{0:N6}", xy[1]));
            listBox4.Items.Add(String.Format(CultureInfo.InvariantCulture, "{0:N6}", xy[2]));
            listBox4.Items.Add(String.Format(CultureInfo.InvariantCulture, "{0:N6}", xy[3]));
            //takecare of Summary
            fillAreaXml();

            //label17.Text = "rectangle:" + " Lat: " + yMin.ToString("N3") + " :: " +yMax.ToString("N3") +
            //  " Lon: " + xMin.ToString("N3") + " :: " + xMax.ToString("N3");
        }

        private RectangleFunction GetRectangleFunction()
        {
            RectangleFunction rf = null;
            foreach (MapFunction fun in _mapArgs.Map.MapFunctions)
            {
                rf = fun as RectangleFunction;
                if (rf != null)
                {
                    return rf;
                }
            }
            return null;
        }

        /// <summary>
        /// de-activates the rectangle drawing from the map and changes the mode back
        /// to selection mode
        /// </summary>
        private void ClearRectangle()
        {
            if (_rectangleDrawing != null)
            {
                _rectangleDrawing.Deactivate();
            }

            //RectangleFunction function = GetRectangleFunction();
            //if (function != null)
            //{
            //    function.Deactivate();
            //}
        }


        #endregion


        //Run the Search
        private SearchCriteria GetSearchParameters()
        {
            var searchCritria = new SearchCriteria();

            //set the search method
            searchCritria.SearchMethod = SearchMode;

            foreach (var item in lbSelectedKeywords.Items)
            {
                searchCritria.keywords.Add(item.ToString());
            }

            /* this region to be refactored /*/
            // Validate and refine the list of keywords.
            //string ontologyFilePath = GetOntologyFilePath ();
            //XmlDocument ontologyXml = new XmlDocument ();
            //ontologyXml.Load ( ontologyFilePath );
            if (SearchMode == Properties.Settings.Default.SearchMethod_HISCentral)
            {
                var ontologyXml = HdSearchOntologyHelper.ReadOntologyXmlFile();
                var ontologyHelper = new HdSearchOntologyHelper();
                ontologyHelper.RefineKeywordList(searchCritria.keywords, ontologyXml);
            }
            else
            {
                //in the special case of metadata cache - hydrosphere keyword
                if (searchCritria.keywords.Contains("Hydrosphere"))
                {
                    searchCritria.keywords.Clear();
                }
            }
            /* this region to be refactored /*/

            //get the start date
            searchCritria.startDate = dateTimePickStart.Value;

            //get the end date
            searchCritria.endDate = dateTimePickEnd.Value;

            //if all webservices are selected, pass an empty array
            if (IsAllWebservicesSelected() == true)
            {
                searchCritria.serviceIDs.Clear();
            }
            else
            {
                foreach (var item in lbWebservicesSupport.Items)
                {
                    searchCritria.serviceIDs.Add(Convert.ToInt32(item));
                }
            }

            //get the HIS Central URL
            searchCritria.hisCentralURL = Global.GetHISCentralURL();

            //get the selected polygons from the active layer or the rectangle
            GetSearchArea(searchCritria);

            //To pass in the search parameters
            return searchCritria;
        }

        public void GetSearchArea(SearchCriteria searchCriteria)
        {
            object areaParameter = searchCriteria.areaParameter;
            if (rectSelectMode == false)
            {

                List<IFeature> selectedPolygons = new List<IFeature>();
                IMapFeatureLayer activeLayer = dgvSearch.MapLayer;
                if (activeLayer.Selection.Count > 0)
                {
                    FeatureSet polyFs = new FeatureSet(FeatureType.Polygon);

                    foreach (IFeature f in activeLayer.Selection.ToFeatureList())
                    {
                        polyFs.Features.Add(f);
                    }

                    polyFs.Projection = _mapArgs.Map.Projection;

                    string esri = Resources.wgs_84_esri_string;
                    ProjectionInfo wgs84 = new ProjectionInfo();
                    wgs84.ReadEsriString(esri);


                    //reproject the selected polygons to WGS1984         
                    polyFs.Reproject(wgs84);

                    //the list of selected polygons passed in to the search function
                    selectedPolygons = new List<IFeature>();
                    foreach (IFeature f in polyFs.Features)
                    {
                        selectedPolygons.Add(f);
                    }
                }
                areaParameter = selectedPolygons;
            }
            else
            {

                //ArrayList rectangleCoordinates = new ArrayList();
                var bBox = new SearchCriteria.AreaRectangle
                               {
                                   xMin = Convert.ToDouble(listBox4.Items[0], CultureInfo.InvariantCulture),
                                   xMax = Convert.ToDouble(listBox4.Items[2], CultureInfo.InvariantCulture),
                                   yMin = Convert.ToDouble(listBox4.Items[1], CultureInfo.InvariantCulture),
                                   yMax = Convert.ToDouble(listBox4.Items[3], CultureInfo.InvariantCulture),
                                   /* as determined from
                                    *             searcher.GetSeriesCatalogInRectangle(
                                                               (double)coords[0], (double)coords[2], (double)coords[1], (double)coords[3],
                                   */
                               };
                areaParameter = bBox;
            }

            //to assign the area parameter
            searchCriteria.areaParameter = areaParameter;
            return;
        }

        //returns true, if the search option is 'All WebServices Selected'
        bool IsAllWebservicesSelected()
        {
            if (lbWebservicesSupport.Items.Count == treeViewWebServices.Nodes.Count)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        #endregion TabControl button management

        void treeView1_MouseUp(object sender, MouseEventArgs e)
        {
            fillWebservicesXml();
        }

        #region Search Background Worker

        /// <summary>
        /// Occurs when the user clicks 'go'
        /// This will update the progress bar
        /// </summary>
        private void btnRunSearchFinal_Click(object sender, EventArgs e)
        {
            RunSearchBackgroundWorker();
        }

        void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {

            SearchCriteria parameters;
            //  ArrayList parameters = e.Argument as ArrayList;
            if (!e.Argument.GetType().Equals(typeof(SearchCriteria)))
            {
                throw new ArgumentException("Bad Argument should be of type SearchCriteria.");
            }
            else
            {
                parameters = (SearchCriteria)e.Argument;
            }
            BackgroundWorker bgWorker = sender as BackgroundWorker;

            var searcher = new BackgroundSearchWithFailover();

            try
            {
                //separate search method is used for HIS Central and for Metadata Cache
                if (parameters.SearchMethod == Properties.Settings.Default.SearchMethod_HISCentral)
                {
                    searcher.HISCentralSearchWithFailover(e, Settings.Instance.HISCentralURLList, bgWorker);
                }
                else
                {
                    searcher.RunMetadataCacheSearch(e, bgWorker);
                }
            }

            catch (HydrodesktopSearchException searchEx)
            {
                WriteError(searchEx.ToString(), "Search Failure. Please Try Again Later.");
            }
        }

        /// <summary>
        /// When search progress is updated
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            //update progress bar and status label
            progBarSearch2.Value = e.ProgressPercentage;
            lblSearching.Text = "Searching.. " + Convert.ToString(e.UserState) + " Series Found";
        }

        /// <summary>
        /// When searching is finished
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //deactivate rectangle function
            if (_rectangleDrawing != null)
            {
                _rectangleDrawing.Deactivate();
            }

            //restore the controls to regular state
            progBarSearch2.Value = 0;
            lblSearching.Text = "";
            panelSearch.Visible = false;

            //check for cancellation
            if (e.Cancelled == true)
            {
                //check for user cancel
                MessageBox.Show("The search was cancelled.");
                tabControl2.SelectedIndex = 0;
                return;
            }

            //check for any error condition.
            else if (e.Error != null)
            {
                MessageBox.Show("Error occurred in the search. " + (e.Error as Exception).ToString());
            }
            else if (e.Result == null)
            {
                MessageBox.Show("No data series were found. Please change the search criteria.");
                tabControl2.SelectedIndex = 2;
                return;
            }
            else if (e.Result.ToString() == "Operation Cancelled")
            {
                //check for user cancel
                MessageBox.Show("The search was cancelled.");
                tabControl2.SelectedIndex = 2;
                return;
            }
            else
            {
                //The search process returns a point FeatureSet.

                //MessageBox.Show("Search is complete.");
                IFeatureSet result = e.Result as IFeatureSet;
                if (result == null)
                {
                    groupResults.Enabled = false;
                    btnDownload.Enabled = false;
                    btnReset.Enabled = false;
                    MessageBox.Show("No data series were found. Please change the search criteria.");
                }
                else if (result.Features.Count == 0)
                {
                    groupResults.Enabled = false;
                    btnDownload.Enabled = false;
                    btnReset.Enabled = false;
                    MessageBox.Show("No data series were found. Please change the search criteria.");
                }
                else
                {
                    //We need to reproject the Search results from WGS84 to the projection of the map.
                    ProjectionInfo wgs84 = KnownCoordinateSystems.Geographic.World.WGS1984;
                    result.Projection = wgs84;

                    ShowSearchResults(result);
                    this.Cursor = Cursors.Default;
                    groupResults.Enabled = true;
                    btnDownload.Enabled = true;
                    btnReset.Enabled = true;

                    //to control the height of datagrid for results
                    //panelSearch.Height = 0;

                }

                //populate the listbox 'Existing theme'.
                AddExistingThemes();
                //once download is complete, set panelsearch visibility to false
                panelSearch.Visible = false;
                //enable the groupbox5/btndownloa
                //check if searchdatagridview1 has >0 rows enable the label
                lblDataSeries.Visible = true;
            }
        }

        #region Cancel Events

        /// <summary>
        /// Call "Cancel_worker" when button click happens.
        /// </summary>
        private void pgsCancel_Click(object sender, EventArgs e)
        {
            Cancel_worker();
        }

        /// <summary>
        /// When Export Form is closed, BackgroundWorker has to stop.
        /// </summary>
        private void SearchForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (backgroundWorker1.IsBusy)
            {
                Cancel_worker();
                e.Cancel = true;
                return;
            }
        }

        /// <summary>
        /// Close the form if Cancel button is clicked before or after an export event.
        /// </summary>
        private void btnCancel_Click(object sender, EventArgs e)
        {
            //is search is in progress, cancel the search
            Cancel_worker();

            //if download is in progress, cancel the download

            //TODO: remove it
            _downLoadManager.Cancel();
        }

        /// <summary>
        /// When "Cancel" button is clicked during the exporting process, BackgroundWorker stops.
        /// </summary>
        private void Cancel_worker()
        {
            backgroundWorker1.CancelAsync();
            this.lblSearching.Text = "Cancelling...";
            this.btnCancel.Enabled = false;
        }

        #endregion

        /// <summary>
        /// When user selects some series for download
        /// </summary>
        void searchDataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            int numSelected = searchDataGridView1.SelectedRows.Count;
            lblDataSeries.Text = String.Format("{0} out of {1} series selected", numSelected, searchDataGridView1.RowCount);
        }

        private void AddExistingThemes()
        {
            lstThemes.Items.Clear();
            //show available data themes - from map legend
            foreach (IMapLayer layer in _mapArgs.Map.Layers)
            {
                if (layer.LegendText.ToLower() == "themes")
                {
                    IMapGroup themeGroup = layer as IMapGroup;
                    if (themeGroup != null)
                    {
                        foreach (IMapLayer themeLayer in themeGroup.Layers)
                        {
                            lstThemes.Items.Add(themeLayer.LegendText);
                        }
                    }
                }
            }
        }

        private void btnDownload_Click(object sender, EventArgs e)
        {
            //search datagrid view for selection of data series
            if (searchDataGridView1.SelectedRows.Count < 1)
            {
                MessageBox.Show("Please Select at least single data-series row to download.");
                return;
            }

            //the associated theme: either use a new theme or add
            //data to an existing theme
            Theme theme = null;
            if (rbNewTheme.Checked == true)
            {
                if (txtThemeName.Text == "")
                {
                    MessageBox.Show("Please write a valid theme name.");
                    return;
                }
                if (lstThemes.Items.Count <= 0)
                {
                    theme = new Theme(txtThemeName.Text, txtThemeDescription.Text);
                }
                else
                {
                    for (int i = 0; i < lstThemes.Items.Count; i++)
                    {
                        if (txtThemeName.Text == "")
                        {
                            MessageBox.Show("Please write a vaild theme name.");
                            return;
                        }
                        else if (txtThemeName.Text.ToLower() == lstThemes.Items[i].ToString().ToLower())
                        {
                            MessageBox.Show("The theme-name " + txtThemeName.Text + " already exists, Please change the theme name and try again!");
                            return;
                        }
                        else
                        {
                            theme = new Theme(txtThemeName.Text, txtThemeDescription.Text);
                        }
                    }
                }

            }
            else
            {
                if (lstThemes.SelectedIndex != -1)
                {
                    //theme = lstThemes.SelectedItem as Theme;
                    theme = new Theme(lstThemes.SelectedItem.ToString());
                }
                else
                {
                    MessageBox.Show("Please ensure that a a valid theme is selected");
                    return;
                }

            }

            //create a list of the SiteCode-VariableCode combinations for download
            var downloadList = new List<OneSeriesDownloadInfo>();
            var fileNameList = new List<String>(); //to ensure, that no duplicate files are 
            //downloaded..
            foreach (IFeature selFeature in searchDataGridView1.MapLayer.Selection.ToFeatureList())
            {
                DataRow row = selFeature.DataRow;

                var di = new OneSeriesDownloadInfo
                             {
                                 SiteName = row["SiteName"].ToString(),
                                 FullSiteCode = row["SiteCode"].ToString(),
                                 FullVariableCode = row["VarCode"].ToString(),
                                 Wsdl = row["ServiceURL"].ToString(),
                                 StartDate = dateTimePickStart.Value,
                                 EndDate = dateTimePickEnd.Value,
                                 VariableName = row["VarName"].ToString(),
                                 Latitude = Convert.ToDouble(row["Latitude"]),
                                 Longitude = Convert.ToDouble(row["Longitude"])
                             };

                string fileBaseName = di.FullSiteCode + "|" + di.FullVariableCode;
                if (!fileNameList.Contains(fileBaseName))
                {
                    fileNameList.Add(fileBaseName);
                    downloadList.Add(di);
                }
            }

            var arg = new StartDownloadArg(downloadList, theme);

            //setup the progress bar
            panelSearch.Visible = true;
            lblSearching.Visible = true;
            progBarSearch2.Visible = true;

            this.lblSearching.Text = "Downloading ..";
            //this.lblTitleDownload.Text = "Downloading .. Please wait";
            //this.txtBoxStatus.Clear();

            //this will ensure that cancel button is enabled as next download can also be cancelled by user at any time.
            this.btnCancel.Enabled = true;
            //Download the data in a background process
            if (_downLoadManager.IsBusy)
            {
                MessageBox.Show("The previous download command is still active, wait....");
                return;
            }

            _downLoadManager.ProgressChanged += _downLoadManager_ProgressChanged;
            _downLoadManager.Completed += _downLoadManager_Completed;
            _downLoadManager.Start(arg);
        }

        private void progBarSearch2_Click(object sender, EventArgs e)
        {
            if (_downLoadManager.IsUIVisible)
                _downLoadManager.HideUI();
            else
                _downLoadManager.ShowUI();
        }

        void _downLoadManager_Completed(object sender, RunWorkerCompletedEventArgs e)
        {
            // Unsubscribe from events
            _downLoadManager.Completed -= _downLoadManager_Completed;
            _downLoadManager.ProgressChanged -= _downLoadManager_ProgressChanged;

            //reset the progress bar
            progBarSearch2.Value = 0;

            if (e.Cancelled)
            {
                panelSearch.Visible = false;
                return;
            }

            var themeName = _downLoadManager.Information.StartDownloadArg.DataTheme == null
                                ? string.Empty
                                : _downLoadManager.Information.StartDownloadArg.DataTheme.Name;

            //Display theme in the main map
            AddThemeToMap(themeName);

            //Change the form's appearance
            lblSearching.Text = "Download Complete.";

            //refreshing the AddExisiting theme
            AddExistingThemes();
            txtThemeName.Text = "";

            //make the download progress-bar panel disappear
            panelSearch.Visible = false;
        }

        void _downLoadManager_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progBarSearch2.Value = e.ProgressPercentage;
            lblSearching.Text = e.UserState != null ? e.UserState.ToString() : string.Empty;
        }

        private void AddThemeToMap(string themeName)
        {
            try
            {
                //to refresh the series selector control
                //TODO: need other way to send this message
                IHydroAppManager mainApplication = _mapArgs.AppManager as IHydroAppManager;
                if (mainApplication != null)
                {
                    mainApplication.SeriesView.SeriesSelector.RefreshSelection();
                }

                var db = new DbOperations(Settings.Instance.DataRepositoryConnectionString, DatabaseTypes.SQLite);
                var manager = new HydroDesktop.Controls.Themes.ThemeManager(db);
                IFeatureSet fs = manager.GetFeatureSet(themeName, _mapArgs.Map.Projection);
                manager.AddThemeToMap(fs, themeName, _mapArgs.Map as DotSpatial.Controls.Map);
                //_mapArgs.Map.MapFrame.ResetBuffer();
            }
            catch { }
        }

        //If there is a 'Data Series' layer present in the map,
        //remove the 'Data Series' layer from the map
        private void RemoveDataSeriesLayer()
        {
            var layerToRemove = GetSearchResultLayer();
            if (layerToRemove != null)
            {
                _mapArgs.Map.Layers.Remove(layerToRemove);
            }
        }

        private IMapFeatureLayer GetSearchResultLayer()
        {
            //TODO: need another mechanizm to identify search layer (we can rename existing layer)

            IMapFeatureLayer layer = null;
            foreach (var lay in _mapArgs.Map.Layers)
            {
                if (lay.LegendText.ToLower() == Global.SEARCH_RESULT_LAYER_NAME.ToLower())
                {
                    layer = lay as IMapFeatureLayer;
                    break;
                }
            }
            return layer;
        }

        /// <summary>
        /// Displays search results (all data series and sites complying to the search criteria)
        /// </summary>
        private void ShowSearchResults(IFeatureSet fs)
        {
            //remove the 'Data Series' layer
            RemoveDataSeriesLayer();

            //try to save the search result layer and re-add it
            string hdProjectPath = Settings.Instance.CurrentProjectDirectory;

            string filename = Path.Combine(hdProjectPath,
                                           HydroDesktop.Search.Properties.Settings.Default.SearchResultName);
            fs.Filename = filename;
            fs.Save();
            fs = FeatureSet.OpenFile(filename);

            var searchLayerCreator = new SearchLayerCreator(_mapArgs.Map, fs);
            var laySearchResult = searchLayerCreator.Create();

            //assign the projection again
            fs.Reproject(_mapArgs.Map.Projection);

            _mapArgs.Map.Layers.Add(laySearchResult);
            searchDataGridView1.SetDataSource(laySearchResult);

            //to prevent the first row of data grid view from becoming selected
            searchDataGridView1.ClearSelection();

            //set the search result layer as selected
            SelectLayerInLegend(Global.SEARCH_RESULT_LAYER_NAME);
            
            // Starting information bubble engine
            if (searchInformer == null)
            {
                searchInformer = new SearchInformer();
            }
            searchInformer.Start(mapMain, laySearchResult);
        }

        #endregion

        private void btnReset_Click(object sender, EventArgs e)
        {
            DialogResult result3 = MessageBox.Show("Are you sure you want to discontinue the Search/download?",
                  "Reset",
                  MessageBoxButtons.YesNo,
                  MessageBoxIcon.Question,
                  MessageBoxDefaultButton.Button2);
            if (result3 == DialogResult.Yes)
            {
                if (backgroundWorker1.IsBusy || _downLoadManager.IsBusy)
                {
                    MessageBox.Show("Background search/download is active. Please wait.");
                    return;
                }
                //set the groupbox
                groupResults.Enabled = false;
                btnDownload.Enabled = false;
                btnReset.Enabled = false;
                searchDataGridView1.DataSource = null;
                tabControl2.SelectedIndex = 0;
                panelSearch.Visible = true;
                listBox4.Items.Clear();
                lbSelectedKeywords.Items.Clear();
                fillXml();
                fillAreaXml();
                lblDataSeries.Visible = false;
            }
            else
            {
                return;
            }
        }

        private void listBox8_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lboRestoreSearch.SelectedIndex != -1)
            {
                groupPreview.Visible = true;
                //pickup xml nodes
                string fname = Application.StartupPath + "\\q_save.xml";
                XmlDocument doc = new XmlDocument();
                doc.Load(fname);
                //textReader = new XmlTextReader(fname);
                //trial starts
                XmlNodeList xmlnode;
                int i = 0;
                xmlnode = doc.GetElementsByTagName("cNode_A");

                for (i = 0; i <= xmlnode.Count - 1; i++)
                {
                    //
                    XmlNodeList query_name = doc.SelectNodes("Gro/cNode_A");
                    XmlNode node = query_name[i];
                    XmlAttributeCollection attcol = node.Attributes;
                    if (attcol[0].Value == lboRestoreSearch.SelectedItem.ToString())
                    {
                        lblRestoreParameters.Text = xmlnode[i].ChildNodes.Item(0).InnerText.Trim();
                        lblRestoreWSValues.Text = xmlnode[i].ChildNodes.Item(1).InnerText.Trim();
                        label31.Text = xmlnode[i].ChildNodes.Item(2).InnerText.Trim();
                        lblRestoreDates.Text = xmlnode[i].ChildNodes.Item(3).InnerText.Trim();
                        lblRestoreSName.Text = xmlnode[i].ChildNodes.Item(4).InnerText.Trim();
                        lblRestoreALayer.Text = xmlnode[i].ChildNodes.Item(5).InnerText.Trim();
                        lblRestoreFName.Text = xmlnode[i].ChildNodes.Item(6).InnerText.Trim();
                    }
                }
                //pickup comes to an end
            }
            else
            {
                groupPreview.Visible = false;
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            //pickup xml nodes
            string file1 = "";
            string fname = Application.StartupPath + "\\q_save.xml";
            XmlDocument doc = new XmlDocument();
            doc.Load(fname);
            if (lboRestoreSearch.SelectedIndex == -1)
            {
                MessageBox.Show("Please select query name to delete");
                return;
            }
            //to take care of last xml node to delete...
            if (lboRestoreSearch.Items.Count < 1)
            {
                return;
            }
            else if (lboRestoreSearch.Items.Count == 1)
            {
                lblRestoreSName.Text = "name";
                lblRestoreALayer.Text = "name";
                lblRestoreFName.Text = "name";
                lblRestoreParameters.Text = "parameters";
                lblRestoreWSValues.Text = "";
                lblRestoreDates.Text = "dates";
                label31.Text = "";
                file1 = lboRestoreSearch.SelectedItem.ToString();
            }
            else
            {
                file1 = lboRestoreSearch.SelectedItem.ToString();
            }
            //XmlNodeList nodes = doc.SelectNodes("//cNode_A[@Search_Name=file1]");
            XmlNodeList nodes = doc.SelectNodes("//cNode_A[@Search_Name='" + file1 + "']");
            for (int i = nodes.Count - 1; i >= 0; i--)
            {
                nodes[i].ParentNode.RemoveChild(nodes[i]);
            }
            doc.Save(fname);
            //ComboBox1.Text = "";
            lboRestoreSearch.Items.Clear();
            groupPreview.Visible = false;

            Populate_xmlcombo();
            if (lboRestoreSearch.Items.Count == 1) lboRestoreSearch.SelectedIndex = 0;
            else if (lboRestoreSearch.SelectedIndex < 1) return;
            else if (lboRestoreSearch.SelectedIndex > 1) lboRestoreSearch.SelectedIndex = 0;
        }
        private void btnWebSerSelectAll_Click(object sender, EventArgs e)
        {
            if (treeViewWebServices.Nodes.Count > 0)
            {
                foreach (TreeNode tnode in treeViewWebServices.Nodes)
                {
                    tnode.Checked = true;
                }
                fillWebservicesXml();
            }
        }

        private void btnWebSerSelectNone_Click(object sender, EventArgs e)
        {
            if (treeViewWebServices.Nodes.Count > 0)
            {
                foreach (TreeNode tnode in treeViewWebServices.Nodes)
                {
                    tnode.Checked = false;
                }
            }
            fillWebservicesXml();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (cboShowWebServicesPanel.Checked == true)
            {
                groupboxWebservices.Visible = true;
            }
            else
            {
                groupboxWebservices.Visible = false;
            }
        }
        private void btnSearchCancel_Click(object sender, EventArgs e)
        {
            Cancel_worker();
        }

        private void checkSummary_CheckedChanged(object sender, EventArgs e)
        {
            if (checkSummary.Checked == true)
            {
                spcHor1.Panel2Collapsed = true;
                btnRunSearchMain.Visible = true;
            }
            else
            {
                spcHor1.Panel2Collapsed = false;
                btnRunSearchMain.Visible = false;
            }
        }

        private void btnRunSearchMain_Click(object sender, EventArgs e)
        {
            //to control the height of datagrid for results
            //panelSearch.Height = 41;

            RunSearchBackgroundWorker();
        }

        private void RunSearchBackgroundWorker()
        {
            // TODO: Why check backgroundWorker2? Isn't that one used only for time series download?
            if (_downLoadManager.IsBusy || backgroundWorker1.IsBusy)
            {
                MessageBox.Show("A previous search is still executing.  Please try again later.");
                return;
            }
            //check for rectangle select mode
            if (rectSelectMode == false) dataGridView1_Click(null, null);

            //setting the datagrid view to null before next search results come in
            searchDataGridView1.DataSource = null;
            lblDataSeries.Visible = false;
            groupResults.Enabled = false;
            btnDownload.Enabled = false;
            btnReset.Enabled = false;
            //show label searching = searching
            lblSearching.Text = "Searching..";
            //check for key query parameters provided?
            if (listBox4.Items.Count < 1)
            {
                MessageBox.Show("Please provide at least one Target Area for search.");
                tabControl2.SelectedIndex = 0;
                listBox4.Focus();
                return;
            }
            if (lbSelectedWebServices.Items.Count < 1)
            {
                MessageBox.Show("Please provide at least one Web Service for search.");
                lbSelectedWebServices.Focus();
                tabControl2.SelectedIndex = 1;
                return;
            }
            if (lbSelectedKeywords.Items.Count < 1)
            {
                MessageBox.Show("Please provide at least one Keyword for search.");
                lbSelectedKeywords.Focus();
                tabControl2.SelectedIndex = 2;
                return;
            }

            // Make sure the background worker isn't already doing some work
            if (backgroundWorker1.IsBusy == true)
            {
                MessageBox.Show("The background worker is busy now. Please try again later.");
                return;
            }

            // Get the search parameters
            SearchCriteria parameters = GetSearchParameters();

            //search is executed as a background thread - initialize the progress bar
            panelSearch.Visible = true;
            progBarSearch2.Value = 0;
            btnCancel.Enabled = true;
            tabControl2.SelectedIndex = 3;

            backgroundWorker1.RunWorkerAsync(parameters);
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            spcHor1.Panel2Collapsed = true;
            btnRunSearchMain.Visible = true;
            btnHideSearchSummary.Visible = false;
            btnShowSearchSummary.Visible = true;
        }

        private void button17_Click(object sender, EventArgs e)
        {
            spcHor1.Panel2Collapsed = false;
            btnRunSearchMain.Visible = false;
            btnShowSearchSummary.Visible = false;
            btnHideSearchSummary.Visible = true;
        }

        private void radioButton8_CheckedChanged(object sender, EventArgs e)
        {
            if (rbList.Checked == true)
            {
                spcKey.Panel2Collapsed = true;
                spcKey.Panel1Collapsed = false;
            }
        }

        private void radioButton9_CheckedChanged(object sender, EventArgs e)
        {
            if (rbTree.Checked == true)
            {
                spcKey.Panel2Collapsed = false;
                spcKey.Panel1Collapsed = true;
            }
        }

        private void radioButton10_CheckedChanged(object sender, EventArgs e)
        {
            if (rbBoth.Checked == true)
            {
                spcKey.Panel2Collapsed = false;
                spcKey.Panel1Collapsed = false;
            }
        }

        private void btnSelectArea_Click(object sender, EventArgs e)
        {
            ClearRectangle();
            rectSelectMode = false;
            //make 'area search' components invisible for reducing clutter
            label5.Visible = true;
            label14.Visible = true;
            label15.Visible = true;
            cboActiveLayer.Visible = true;
            lbFieldsActiveLayer.Visible = true;
            dgvSearch.Visible = true;
            //clear the listBox
            listBox4.Items.Clear();
            listBox4.Visible = false;
            label32.Visible = false;
            label47.Visible = false;
            //search summary labels
            label16.Visible = true;
            fillAreaXml();
            //activate the Area Selection Mode
            _mapArgs.Map.FunctionMode = FunctionMode.Select;

            //select the correct layer in the legend
            if (!String.IsNullOrEmpty(cboActiveLayer.SelectedItem.ToString()))
            {
                SelectLayerInLegend(cboActiveLayer.SelectedItem.ToString());
            }
        }

        private void SelectLayerInLegend(string legendText)
        {
            foreach (IMapLayer lay in _mapArgs.Map.MapFrame.GetAllLayers())
            {
                lay.IsSelected = lay.LegendText == legendText;
            }
        }

        //#region Ontology Utilities
        ///* for refatoring
        // * we need a set of test cases
        // * top level terms that should return empty string
        // * couple of leaf nodes, should be return the same
        // * terms that are under, and only the top level should be retained
        // * disjount terms, that should be returned
        // * disjoint terms with a term or two under
        // * 
        // * Note this is a good method to test (Tim good job). 
        // * We could make a small sample ontology xml file to represent a subset of 
        // * ontology and make a very small set of controlled use cases
        // * */
        ///// <summary>
        ///// Modifies the input keyword list by removing redundant or otherwise unnecessary items for efficient searching.
        ///// </summary>
        ///// <param name="KeywordList">List of input keywords to refine.</param>
        ///// <param name="OntologyXml">XML of the CUAHSI hydrologic ontology.</param>
        //private void RefineKeywordList ( List<string> KeywordList, XmlDocument OntologyXml )
        //{
        //    // Refactoring. This is the entry point
        //    // If searching 1st tier keywords, clear the list.
        //    List<string> tier1Keywords = GetKeywordsAtTier ( 1, OntologyXml );
        //    foreach ( string tier1keyword in tier1Keywords )
        //    {
        //        if ( KeywordList.Contains ( tier1keyword ) == true )
        //        {
        //            KeywordList.Clear ();
        //            return;
        //        }
        //    }

        //    // Remove repeated keywords.
        //    List<string> tmpList = KeywordList.Distinct ().ToList ();
        //    if ( tmpList.Count != KeywordList.Count )
        //    {
        //        KeywordList.Clear ();
        //        KeywordList.AddRange ( tmpList );
        //    }

        //    // Remove keywords that don't have a match in the ontology.
        //    RemoveUnmatchedKeywords ( KeywordList, OntologyXml );

        //    // Remove keywords if their ancestors are also in the list.
        //    RemoveRedundantChildKeywords ( KeywordList, OntologyXml );

        //    // Replace 2nd tier keywords with their 3rd tier child keywords.
        //    // 2nd tier keywords cannot be searched at HIS Central.
        //    List<string> tier2Keywords = GetKeywordsAtTier ( 2, OntologyXml );
        //    foreach ( string tier2keyword in tier2Keywords )
        //    {
        //        if ( KeywordList.Contains ( tier2keyword ) == true )
        //        {
        //            // Remove 2nd tier keyword
        //            RemoveAllFromList ( KeywordList, tier2keyword );

        //            // Add 3rd tier keywords that are children of the removed 2nd tier keyword.
        //            List<string> tier3Keywords = GetChildKeywords ( tier2keyword, OntologyXml );
        //            foreach ( string tier3keyword in tier3Keywords )
        //            {
        //                if ( KeywordList.Contains ( tier3keyword ) == false )
        //                {
        //                    KeywordList.Add ( tier3keyword );
        //                }
        //            }
        //        }
        //    }
        //}

        ///// <summary>
        ///// Gets all child keywords for the given keyword from the ontology XML.
        ///// </summary>
        ///// <param name="Keyword">The keyword for which child keywords are sought.</param>
        ///// <param name="OntologyXml">XML of the CUAHSI hydrologic ontology.</param>
        ///// <returns>List of child keywords for the given keyword from the ontology XML.</returns>
        //private List<string> GetChildKeywords ( string Keyword, XmlDocument OntologyXml )
        //{
        //    // Create a namespace manager to enable XPath searching.  Otherwise, no results are returned if a namespace is present.
        //    // This works even if no namespace is present.
        //    XmlNamespaceManager nsmgr = new XmlNamespaceManager ( OntologyXml.NameTable );
        //    nsmgr.AddNamespace ( "x", OntologyXml.DocumentElement.NamespaceURI );

        //    // Create an XPath expression to find all child keywords of the given keyword.
        //    string xpathExpression = "//x:OntologyNode[x:keyword='" + Keyword + "']/x:childNodes/x:OntologyNode/x:keyword";

        //    // Select all nodes that match the XPath expression.
        //    XmlNodeList keywordNodes = OntologyXml.SelectNodes ( xpathExpression, nsmgr );

        //    // Return a list of the parent keywords.
        //    return NodeListToStringList ( keywordNodes );
        //}

        ///// <summary>
        ///// Gets keywords at a given tier within the hierarchical CUAHSI hydrologic ontology.
        ///// </summary>
        ///// <param name="Tier">The tier for which keywords are sought. The highlest level is tier 1, the next level is tier 2, and so on.</param>
        ///// <param name="OntologyXml">XML of the CUAHSI hydrologic ontology.</param>
        ///// <returns>List of keywords at the given tier in the ontology XML.</returns>
        //private List<string> GetKeywordsAtTier ( int Tier, XmlDocument OntologyXml )
        //{
        //    // Validate inputs.
        //    if ( Tier < 1 )
        //    {
        //        throw new ArgumentOutOfRangeException ( "Tier", "Tier must be greater than or equal to 1" );
        //    }

        //    // Create a namespace manager to enable XPath searching.  Otherwise, no results are returned if a namespace is present.
        //    // This works even if no namespace is present.
        //    XmlNamespaceManager nsmgr = new XmlNamespaceManager ( OntologyXml.NameTable );
        //    nsmgr.AddNamespace ( "x", OntologyXml.DocumentElement.NamespaceURI );

        //    // Create an XPath expression to find all keywords at the given tier.
        //    StringBuilder expressionBuilder = new StringBuilder ( Tier * 25 );
        //    for ( int i = 2; i <= Tier; i++ )
        //    {
        //        expressionBuilder.Append ( "/x:OntologyNode/x:childNodes" );
        //    }
        //    expressionBuilder.Append ( "/x:OntologyNode/x:keyword" );
        //    string xpathExpression = expressionBuilder.ToString ();

        //    // Select all nodes that match the XPath expression.
        //    XmlNodeList keywordNodes = OntologyXml.SelectNodes ( xpathExpression, nsmgr );

        //    // Return a list of the keywords.
        //    return NodeListToStringList ( keywordNodes );
        //}

        ///// <summary>
        ///// Gets all ancestor keywords (parent, grandparent, etc.) for the given keyword from the ontology XML.
        ///// </summary>
        ///// <param name="Keyword">The keyword for which ancestor keywords are sought.</param>
        ///// <param name="OntologyXml">XML of the CUAHSI hydrologic ontology.</param>
        ///// <returns>List of ancestor keywords for the given keyword from the ontology XML.</returns>
        //private List<string> GetAncestorKeywords ( string Keyword, XmlDocument OntologyXml )
        //{
        //    // Create a namespace manager to enable XPath searching.  Otherwise, no results are returned if a namespace is present.
        //    // This works even if no namespace is present.
        //    XmlNamespaceManager nsmgr = new XmlNamespaceManager ( OntologyXml.NameTable );
        //    nsmgr.AddNamespace ( "x", OntologyXml.DocumentElement.NamespaceURI );

        //    // Create an XPath expression to find all parent keywords of the given keyword.
        //    string xpathExpression = "//x:OntologyNode[x:keyword='" + Keyword + "']/ancestor::x:OntologyNode/x:keyword";

        //    // Select all nodes that match the XPath expression.
        //    XmlNodeList keywordNodes = OntologyXml.SelectNodes ( xpathExpression, nsmgr );

        //    // Return a list of the keywords.
        //    return NodeListToStringList ( keywordNodes );
        //}

        ///// <summary>
        ///// Gets keyword nodes from the CUAHSI hydrologic ontology XML that match the given keyword.
        ///// </summary>
        ///// <param name="Keyword">The keyword for which keyword nodes are sought.</param>
        ///// <param name="OntologyXml">XML of the CUAHSI hydrologic ontology.</param>
        ///// <returns>Keyword nodes from the CUAHSI hydrologic ontology XML that match the given keyword.</returns>
        //private XmlNodeList GetKeywordNodes ( string Keyword, XmlDocument OntologyXml )
        //{
        //    // Create a namespace manager to enable XPath searching.  Otherwise, no results are returned if a namespace is present.
        //    // This works even if no namespace is present.
        //    XmlNamespaceManager nsmgr = new XmlNamespaceManager ( OntologyXml.NameTable );
        //    nsmgr.AddNamespace ( "x", OntologyXml.DocumentElement.NamespaceURI );

        //    // Create an XPath expression to find the given keyword.
        //    string xpathExpression = "//x:keyword[. = '" + Keyword + "']";

        //    // Select all nodes that match the XPath expression.
        //    return OntologyXml.SelectNodes ( xpathExpression, nsmgr );
        //}

        ///// <summary>
        ///// Modifies the input list by removing items whose ancestors from the Ontology XML also appear in the list.
        ///// </summary>
        ///// <param name="KeywordList">List of keywords for which redundant child keywords should be removed.</param>
        ///// <param name="OntologyXml">XML of the CUAHSI hydrologic ontology.</param>
        //private void RemoveRedundantChildKeywords ( List<string> KeywordList, XmlDocument OntologyXml )
        //{
        //    // Find parents for each keyword.  If parent also exists in the keyword list, mark the keyword for removal.
        //    List<string> keywordsToRemove = new List<string> ();
        //    foreach ( string keyword in KeywordList )
        //    {
        //        List<string> parentKeywords = GetAncestorKeywords ( keyword, OntologyXml );
        //        if ( parentKeywords.Intersect ( KeywordList ).Count () > 0 )
        //        {
        //            keywordsToRemove.Add ( keyword );
        //        }
        //    }

        //    // Remove unnecessary keywords.
        //    foreach ( string keywordToRemove in keywordsToRemove )
        //    {
        //        RemoveAllFromList ( KeywordList, keywordToRemove );
        //    }
        //}

        ///// <summary>
        ///// Modifies the input list by removing keywords that do not appear in the CUAHSI hydrologic Ontology.
        ///// </summary>
        ///// <param name="KeywordList">List of keywords for which redundant child keywords should be removed.</param>
        ///// <param name="OntologyXml">XML of the CUAHSI hydrologic ontology.</param>
        //private void RemoveUnmatchedKeywords ( List<string> KeywordList, XmlDocument OntologyXml )
        //{
        //    // Find keywords with no match in the ontology.
        //    List<string> keywordsToRemove = new List<string> ();
        //    foreach ( string keyword in KeywordList )
        //    {
        //        XmlNodeList matchingNodes = GetKeywordNodes ( keyword, OntologyXml );
        //        if ( matchingNodes.Count == 0 )
        //        {
        //            keywordsToRemove.Add ( keyword );
        //        }
        //    }

        //    // Remove unmatched keywords.
        //    foreach ( string keywordToRemove in keywordsToRemove )
        //    {
        //        RemoveAllFromList ( KeywordList, keywordToRemove );
        //    }
        //}

        ///// <summary>
        ///// Removes all occurrences of a specific string from the System.Collections.Generic.List.
        ///// </summary>
        ///// <param name="StringList">System.Collections.Generic.List of strings</param>
        ///// <param name="Item">The item to remove from the list</param>
        //private void RemoveAllFromList ( List<string> StringList, string Item )
        //{
        //    while ( StringList.Contains ( Item ) )
        //    {
        //        StringList.Remove ( Item );
        //    }
        //}

        ///// <summary>
        ///// Creates a list of InnerText values from the input XML node list.
        ///// </summary>
        ///// <param name="NodeList">XML node list whose InnerText values will be added to a string list.</param>
        ///// <returns>String list of InnerText values from the input XML list.</returns>
        //private List<string> NodeListToStringList ( XmlNodeList NodeList )
        //{
        //    List<string> stringList = new List<string> ();

        //    foreach ( XmlNode node in NodeList )
        //    {
        //        stringList.Add ( node.InnerText );
        //    }

        //    return stringList;
        //}

        ///// <summary>
        ///// Gets the full path to the XML file storing the CUAHSI hydrologic ontology.
        ///// </summary>
        ///// <returns>The full path to the XML file storing the CUAHSI hydrologic ontology.</returns>
        //private string GetOntologyFilePath ()
        //{
        //    // note for refactoring. load file on creation of object
        //    string hydroDesktopFolder = AppDomain.CurrentDomain.BaseDirectory;
        //    string ontologyFilePath = Path.Combine ( hydroDesktopFolder, _ontologyFilename );
        //    return ontologyFilePath;
        //}

        //#endregion

    }
}
