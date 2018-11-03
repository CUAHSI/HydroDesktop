using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Timers;
using System.Runtime.InteropServices;

namespace FacetedSearch3
{
    /// <summary>
    /// Mechanism for dealing with a single level of specificity of faceted search.
    /// </summary>
    public partial class SearchFacetSpecifier : UserControl
    {
        private FacetedSearchControl FacetedSearchHost;
        public IEnumerable<FacetedSearch3.CUAHSIFacetedSearch.OntologyElement> PreviousSelectedOntologyElements;      // serialized ontology elements that have been specified by previous SearchFacetSpecifiers
        public IEnumerable<FacetedSearch3.CUAHSIFacetedSearch.OntologyElement> MyFullFacetSet;                        // The entire catalog of ontological terms, unfiltered
        public IEnumerable<FacetedSearch3.CUAHSIFacetedSearch.OntologyElement> RemainingFacets;                       // response from XML webservice. These are the facets that are remaining given the user's selections.        
        public IEnumerable<FacetedSearch3.CUAHSIFacetedSearch.OntologyElement> Orphans;
        public List<FacetedSearch3.CUAHSIFacetedSearch.OntologyElement> MySelectedFacets;
        // private Boolean MyRootIsSpecified;                                              // Specifies whether the root element of one of the web-service layer properties has been selected
        public int MyFacetIndex;                                                        // position in the search stack that is the FacetFlowPanel on the CUAHSISearchForm (parent FlowLayoutPanel).        
        public System.Timers.Timer ac_Timer;                                            // manages delay of autocomplete 
        public double AutoDelay_ms = 800;                                               // autocomplete delay in milliseconds
        public int AutoComplete_charLim = 3;                                            // minimum length of text to be considered for autocomplete        
        private int ExpandedTextMaxLen = 25;                                            // the number of chars allowed to be shown in a synonym-appended Treenode Text

        #region Win32 API
        /// <summary>
        /// Win32 API commands for programmatically scrolling WPF forms controls. Not all necessarily used, but here to provide options.
        /// </summary>
        private const int WM_SCROLL = 276;   // Horizontal scroll
        private const int WM_VSCROLL = 277;  // Vertical scroll
        private const int SB_LINEUP = 0;     // Scrolls one line up
        private const int SB_LINELEFT = 0;   // Scrolls one cell left
        private const int SB_LINEDOWN = 1;   // Scrolls one line down
        private const int SB_LINERIGHT = 1;  // Scrolls one cell right
        private const int SB_PAGEUP = 2;     // Scrolls one page up
        private const int SB_PAGELEFT = 2;   // Scrolls one page left
        private const int SB_PAGEDOWN = 3;   // Scrolls one page down
        private const int SB_PAGERIGTH = 3;  // Scrolls one page right
        private const int SB_PAGETOP = 6;    // Scrolls to the upper left
        private const int SB_LEFT = 6;       // Scrolls to the left
        private const int SB_PAGEBOTTOM = 7; // Scrolls to the upper right
        private const int SB_RIGHT = 7;      // Scrolls to the right
        private const int SB_ENDSCROLL = 8;  // Ends scroll

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int SendMessage(IntPtr hWnd, int wMsg, IntPtr wParam, IntPtr lParam);

        #endregion

        /// <summary>
        /// Used for thread-safe autocomplete behavior
        /// </summary>
        /// <param name="Index">Index of the TreeNode to be exposed.</param>
        /// <param name="NameMatch">Name that was text-matched to trigger selection.</param>
        delegate void ExposeNodesCallback(Dictionary<int, string> ExposeSet);

        private void ExposeNodes(Dictionary<int, string> ExposeSet)
        {
            // reset TreeView
            FacetTree.CollapseAll();
            foreach (TreeNode n in FacetTree.Nodes)
            {
                RecursiveFontSetter(n, FontStyle.Regular, true);
            }

            // Expose nodes
            foreach (KeyValuePair<int, string> k in ExposeSet)
            {
                foreach (TreeNode n in FacetTree.Nodes)
                {
                    ExposeNode(n, k.Key, k.Value);
                }
            }            

            // scroll TreeView to the left
            SendMessage(FacetTree.Handle, WM_SCROLL, (IntPtr)SB_LEFT, IntPtr.Zero);
        }

        private void ExposeNode(TreeNode p, int KeyAsInt, string NameMatch)
        {
            if (p.Name == KeyAsInt.ToString())
            {
                if ((p.Tag as List<FacetedSearch3.CUAHSIFacetedSearch.OntologyElement>).First().cConceptName != NameMatch)
                {
                    p.Text = string.Format("{0} ({1})", (p.Tag as List<FacetedSearch3.CUAHSIFacetedSearch.OntologyElement>).First().cConceptName, NameMatch);                    
                }
                
                // do node formatting
                if (p.Text.Length > ExpandedTextMaxLen)
                {
                    p.ToolTipText = p.Text;
                    p.Text = p.Text.Substring(0, ExpandedTextMaxLen) + "...";
                }                
                
                p.NodeFont = new System.Drawing.Font(FacetTree.Font, FontStyle.Bold);
                p.Text += string.Empty;    // workaround for TreeView node truncation issue: http://support.microsoft.com/kb/937215/en-us?sp&s
                p.EnsureVisible();
            }
            else if (p.Checked)
            {
                p.EnsureVisible();
            }
                
            if (p.Nodes.Count > 0)
            {
                foreach (TreeNode n in p.Nodes)
                {
                    ExposeNode(n, KeyAsInt, NameMatch);
                }
            }            
        }

        /// <summary>
        /// Uses the existing FacetTree font to apply a new font to a node and all its children
        /// </summary>
        /// <param name="p"></param>
        /// <param name="f"></param>
        private void RecursiveFontSetter(TreeNode p, FontStyle f, Boolean ResetText)
        {
            p.NodeFont = new Font(FacetTree.Font, f);
            if (ResetText)
            {
                if (p.Text.Length > ExpandedTextMaxLen) // optimization exploiting "..." appending
                {
                    p.Text = (p.Tag as List<FacetedSearch3.CUAHSIFacetedSearch.OntologyElement>).First().cConceptName;
                }
                
                foreach (TreeNode n in p.Nodes)
                {
                    RecursiveFontSetter(n, f, true);
                }
            }
            else
            {
                
                foreach (TreeNode n in p.Nodes)
                {
                    RecursiveFontSetter(n, f, false);
                }
            }            
        }

        /// <summary>
        /// Constructor for use with serialized lists of OntologyElements.
        /// </summary>
        /// <param name="ParentForm">The parent control of this, currently set to a Form object</param>
        /// <param name="FullFacetSet">The entire catalog of ontological terms, unfiltered.</param>
        /// <param name="SelectedThusFar">The list of ontological terms that have already been selected (selected upstream of this).</param>
        /// <param name="RemainingFacets">The list of ontological terms that are remaining given the selections that have been made up to this.</param>
        /// <param name="MyIndex">Zero-based index of SearchFacetSpecifiers on the parent control. This is my position.</param>
        public SearchFacetSpecifier(FacetedSearchControl ParentForm, IEnumerable<FacetedSearch3.CUAHSIFacetedSearch.OntologyElement> FullFacetSet, IEnumerable<FacetedSearch3.CUAHSIFacetedSearch.OntologyElement> SelectedThusFar, IEnumerable<FacetedSearch3.CUAHSIFacetedSearch.OntologyElement> RemainingFacetsIn, int MyIndex)
        {
            #region Initialize Control
            InitializeComponent();
            FacetedSearchHost = ParentForm;
            RemainingFacets = RemainingFacetsIn;
            MyFacetIndex = MyIndex;
            PreviousSelectedOntologyElements = SelectedThusFar;
            MyFullFacetSet = FullFacetSet;
            Orphans = MyFullFacetSet.Where(r => r.IsRoot == false).ToList();
            // MyRootIsSpecified = false;
            MySelectedFacets = new List<FacetedSearch3.CUAHSIFacetedSearch.OntologyElement>();
            ac_Timer = new System.Timers.Timer(AutoDelay_ms);
            ac_Timer.AutoReset = false;
            ac_Timer.Elapsed += new ElapsedEventHandler(OnAutoCompleteStart);
            #endregion

            #region Render Display of Options
            RenderMyFacetTree();
            #endregion
        }

        /// <summary>
        /// Constructor for scaffolding tests
        /// </summary>
        /// <param name="RemainingFacets"></param>
        /// <param name="MyIndex"></param>
        public SearchFacetSpecifier(IEnumerable<FacetedSearch3.CUAHSIFacetedSearch.OntologyElement> RemainingFacetsIn, int MyIndex)
        {
            InitializeComponent();
            RemainingFacets = RemainingFacetsIn;
            MyFacetIndex = MyIndex;
            // MyRootIsSpecified = false;
            ac_Timer = new System.Timers.Timer(AutoDelay_ms);
            ac_Timer.AutoReset = false;
            ac_Timer.Elapsed += new ElapsedEventHandler(OnAutoCompleteStart);
        }        

        /// <summary>
        /// Makes the SearchFacetSpecifier display its full tree, and checks all of the facets that were specified in this SearchFacetSpecifier (as opposed to a higher-level instance)
        /// </summary>
        public void ReAnimate()
        {
            throw new NotImplementedException();
        }        
        
        /// <summary>
        /// Uses MyFullFacetSet and RemainingFacets to populate the FacetTree control. Displays the complete hierarchy minus the branches without any remaining facets.
        /// </summary>
        private void RenderMyFacetTree()
        {                        
            FacetTree.Nodes.Clear();            

            List<FacetedSearch3.CUAHSIFacetedSearch.OntologyElement> FailedListings = new List<FacetedSearch3.CUAHSIFacetedSearch.OntologyElement>();
            List<FacetedSearch3.CUAHSIFacetedSearch.OntologyElement> AlreadyListed = new List<FacetedSearch3.CUAHSIFacetedSearch.OntologyElement>();
            
            // insert roots if they are recorded as having data attached to them
            foreach (List<FacetedSearch3.CUAHSIFacetedSearch.OntologyElement> os in RemainingFacets.Where(r => r.IsRoot == true).GroupBy(r => r.cConceptID).ToList())
            {
                FacetTree.Nodes.Add(CreateCUAHSITreeNode(os));
                AlreadyListed.AddRange(os);
            }

            // place non-root elements, inserting roots as necessary
            Stack<List<FacetedSearch3.CUAHSIFacetedSearch.OntologyElement>> AddNodes = new Stack<List<FacetedSearch3.CUAHSIFacetedSearch.OntologyElement>>();
            List<FacetedSearch3.CUAHSIFacetedSearch.OntologyElement> NowElements = new List<FacetedSearch3.CUAHSIFacetedSearch.OntologyElement>();
            Boolean FoundRoot;
            foreach (IGrouping<int, FacetedSearch3.CUAHSIFacetedSearch.OntologyElement> g in RemainingFacets.GroupBy(r => r.cConceptID))
            {
                AddNodes.Clear();
                NowElements.Clear();                              

                NowElements.AddRange(g);

                FoundRoot = false;
                while (FoundRoot == false 
                    && !AlreadyListed.Contains(NowElements.First(), new OntologyElementForDisplayComparer())
                    && !PreviousSelectedOntologyElements.Contains(NowElements.First(), new OntologyElementForDisplayComparer()))
                {
                    AddNodes.Push(NowElements);
                    AlreadyListed.AddRange(NowElements);
                    if (NowElements.First().IsRoot)
                    {
                        FoundRoot = true;
                    }
                    else
                    {
                        NowElements = MyFullFacetSet.Where(r => r.cConceptID == (NowElements.First().cParentID)).ToList();  // a given conceptID can only have one parent
                        if (NowElements == null || NowElements.Count < 1)
                        {
                            MessageBox.Show(string.Format("Null element detected in tree leading concept {0}", NowElements.First().cConceptName));
                        }
                    }                    
                }

                // put stack of ontology
                while (AddNodes.Count > 0)
                {
                    List<FacetedSearch3.CUAHSIFacetedSearch.OntologyElement> ThisNode = AddNodes.Pop();
                    try
                    {
                        if (ThisNode.First().IsRoot)
                        {
                            FacetTree.Nodes.Add(CreateCUAHSITreeNode(ThisNode));
                        }
                        else
                        {
                            FacetTree.Nodes.Find(ThisNode.First().cParentID.ToString(), true).First().Nodes.Add(CreateCUAHSITreeNode(ThisNode));
                        }
                    }
                    catch
                    {
                        FailedListings.AddRange(ThisNode);                        
                    }                                            
                }
            }

            #region DebugDiagnostics
#if DEBUG
            if (FailedListings.Count > 0)
            {
                string FailedAdds = "";            
                foreach (FacetedSearch3.CUAHSIFacetedSearch.OntologyElement o in FailedListings)
                {
                    FailedAdds += string.Format("cID: {0}, cName: {1}, synonym: {2}\n ", o.cConceptID, o.cConceptName, o.Synonym);
                }
                MessageBox.Show(string.Format("There were {0} Failed Adds: {1}", FailedListings.Count, FailedAdds));
            }

            int LeafNodes = 0;
            foreach (TreeNode n in FacetTree.Nodes)
            {
                if (n.Nodes.Count < 1)
                {
                    LeafNodes++;
                }
            }

            // MessageBox.Show(string.Format("There are {0} nodes in the FacetTree, {1} objects in RemainingFacets, and {2} LeafNodes in FacetTree", FacetTree.Nodes.Count, RemainingFacets.Count, LeafNodes));
#endif            
            #endregion
        }

        /// <summary>
        /// Recursively makes children of the root from Options 
        /// </summary>
        /// <param name="root"></param>
        /// <param name="Options"></param>
        private void AddChildrenToFacetSearchRoot(TreeNode root)
        {
            // Loads up entire tree - could then prune from here            
            foreach (List<FacetedSearch3.CUAHSIFacetedSearch.OntologyElement> os in Orphans
                .Where(r => r.cConceptID == (root.Tag as List<FacetedSearch3.CUAHSIFacetedSearch.OntologyElement>).First().cConceptID)
                .GroupBy(r => r.cConceptID).ToList())
            {
                TreeNode n = CreateCUAHSITreeNode(os);
                AddChildrenToFacetSearchRoot(n);
                root.Nodes.Add(n);
            }                        
        }

        /// <summary>
        /// Decorates a TreeNode object with Tag, Name, and Text properties to facilitate lookups and querying
        /// </summary>
        /// <param name="os">List of OntologyElements with the same ConceptID</param>
        /// <returns></returns>
        private static TreeNode CreateCUAHSITreeNode(List<FacetedSearch3.CUAHSIFacetedSearch.OntologyElement> os)
        {            
            TreeNode n = new TreeNode();
            n.Name = os.First().cConceptID.ToString();
            n.Text = os.First().cConceptName;
            n.Tag = os;
            return n;
        }            

        private void NextBtn_Click(object sender, EventArgs e)
        {
            FacetedSearchHost.InvokeNextButton(CollectTotalSelectedFacets(), MyFacetIndex);            
        }

        /// <summary>
        /// Retrieves a list of OntologyElements from the checked facets of the faceted search
        /// </summary>
        /// <returns></returns>
        private List<FacetedSearch3.CUAHSIFacetedSearch.OntologyElement> CollectTotalSelectedFacets()
        {
            MySelectedFacets.Clear();
            foreach (TreeNode n in FacetTree.Nodes)
            {
                List<TreeNode> SelectedNodes = GetCheckedLeafChildren(n, new List<TreeNode>());
                foreach (TreeNode d in SelectedNodes)
                {
                    MySelectedFacets.Add((d.Tag as List<FacetedSearch3.CUAHSIFacetedSearch.OntologyElement>).First());
                }
            }

            List<FacetedSearch3.CUAHSIFacetedSearch.OntologyElement> TotalSelectedFacets = new List<FacetedSearch3.CUAHSIFacetedSearch.OntologyElement>();
            TotalSelectedFacets.AddRange(PreviousSelectedOntologyElements);
            TotalSelectedFacets.AddRange(MySelectedFacets);
            return TotalSelectedFacets;
        }

        private void SearchBtn_Click(object sender, EventArgs e)
        {
            FacetedSearchHost.InvokeSearchButton(CollectTotalSelectedFacets());
        }      

        /// <summary>
        /// Manages UI interactivity due to selections within the facet tree
        /// Selects
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FacetTree_AfterCheck(object sender, TreeViewEventArgs e)
        {
            if (e.Node.Checked)
            {                                
                // expand child nodes if there are just one of them
                if (e.Node.Nodes.Count == 1)
                {
                    e.Node.Expand();
                }

                // propagate selections to children
                foreach (TreeNode n in e.Node.Nodes)
                {
                    GovernChildren(n, true);
                }      
            }
            else
            {
                // propagate selections to childrens
                foreach (TreeNode n in e.Node.Nodes)
                {
                    GovernChildren(n, false);
                }
            }            
        }

        /// <summary>
        /// Moves up the FacetTree until it finds a node with an OntologyElement with isRoot == True, signifying that it is a root node of the faceted search
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        private TreeNode GetRootParentOfTreeNode(TreeNode n)
        {
            TreeNode NowNode = n;
            while ((NowNode.Tag as List<FacetedSearch3.CUAHSIFacetedSearch.OntologyElement>).First().IsRoot == false)
            {
                NowNode = n.Parent;
            }

            return NowNode;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FacetTree_BeforeCheck(object sender, TreeViewCancelEventArgs e)
        {            
            // CONSIDER IMPLEMENTING FILTER FOR NON-FIRST-SELECTION CHECKING HERE... SHOULD ALSO HAVE VISUAL CUE THOUGH
            // e.Cancel = true;
        }

        /// <summary>
        /// Recursively check or uncheck TreeNode children. Handles single-child expansion too. Called AfterCheck.
        /// </summary>
        /// <param name="p"></param>
        /// <param name="SetChecked"></param>
        public void GovernChildren(TreeNode p, Boolean SetChecked)
        {            
            p.Checked = SetChecked;
            if (p.Nodes.Count > 1)
            {
                foreach (TreeNode n in p.Nodes)
                {
                    GovernChildren(n, SetChecked);
                }
            }
            else if (p.Nodes.Count > 0)
            {
                if (SetChecked)
                {
                    p.Expand();                    
                }                

                foreach (TreeNode n in p.Nodes)
                {
                    GovernChildren(n, SetChecked);
                }
            }                       
        }

        /// <summary>
        /// Propagates a color down a tree
        /// </summary>
        /// <param name="p"></param>
        /// <param name="c"></param>
        public void ApplyColoringToNodeAndChildren(TreeNode p, Color c)
        {                        
            foreach (TreeNode n in p.Nodes)
            {
                ApplyColoringToNodeAndChildren(n, c);
            }
        }

        /// <summary>
        /// Recursively finds the number of nodes that are checked in the tree, including the node that is passed
        /// </summary>
        /// <param name="ns"></param>
        /// <returns></returns>
        public int CountCheckedChildren(TreeNode p, int NumSelected)
        {                         
            if (p.Checked)
            {
                NumSelected++;
            }

            foreach (TreeNode n in p.Nodes)
            {
                NumSelected = CountCheckedChildren(n, NumSelected);
            }

            return NumSelected;
        }

        /// <summary>
        /// Returns a list of all of the children of p that are checked and have no child-nodes
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public List<TreeNode> GetCheckedLeafChildren(TreeNode p, List<TreeNode> AlreadySelected)
        {
            // TO-DO: evolve behavior for selection to reflect canonical data binding to non-leaf/non-root items
            if (p.Checked && p.Nodes.Count < 1)
            {
                AlreadySelected.Add(p); // only count those that are checked
            }

            foreach (TreeNode n in p.Nodes)
            {                                                            
                AlreadySelected = GetCheckedLeafChildren(n, AlreadySelected);
            }

            return AlreadySelected;
        }

        /// <summary>
        /// Removes me and all of my children from the search stack
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteFacetBtn_Click(object sender, EventArgs e)
        {
            ac_Timer.Elapsed -= new ElapsedEventHandler(OnAutoCompleteStart);
            FacetedSearchHost.InvokeDeleteFacetButton(MyFacetIndex);
        }

        private void NextSQLBtn_Click(object sender, EventArgs e)
        {
            FacetedSearchHost.InvokeNextSQLButton(CollectTotalSelectedFacets(), MyFacetIndex);
        }

        private void SearchSQLBtn_Click(object sender, EventArgs e)
        {
            FacetedSearchHost.InvokeSearchSQLButton(CollectTotalSelectedFacets());
        }

        /// <summary>
        /// Event for autocomplete. Makes nodes visible.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FacetTextBox_TextChanged(object sender, EventArgs e)
        {
            if (FacetTextBox.TextLength >= AutoComplete_charLim)
            {
                // reset timer before starting count again
                if (ac_Timer.Enabled)
                {
                    ac_Timer.Stop();
                }                
                
                ac_Timer.Start();                
            }
            else
            {
                if (ac_Timer.Enabled)
                {
                    ac_Timer.Stop();
                }
            }
        }

        /// <summary>
        /// Exposes nodes of FacetTree that have canonical names or synonyms similar to the user's search terms
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        private void OnAutoCompleteStart(object source, ElapsedEventArgs e)
        {            
            // MessageBox.Show(string.Format("Autocompleting now for '{0}'...", FacetTextBox.Text.Trim()));

            String[] sTerms = FacetTextBox.Text.Trim().Split(new char[' ']);

            Dictionary<int, string> ExposeIndexes = new Dictionary<int,string>();
            foreach (string s in sTerms)
            {
                foreach (TreeNode n in FacetTree.Nodes)
                {
                    ExposeIndexes = GetExposeChildren(n, s.Trim(new char[',']), ExposeIndexes);  // do not include commas in the autocomplete search
                }
            }
                        
            ExposeNodesCallback en = new ExposeNodesCallback(ExposeNodes);
            this.Invoke(en, new object[] { ExposeIndexes });

            ExposeIndexes.Clear();            
            // MessageBox.Show("Autocompleting done.");
        }

        /// <summary>
        /// Recursive function for searching for children to expose in a UI tree by their canonical name and synonyms
        /// </summary>
        /// <param name="p"></param>
        private Dictionary<int, string> GetExposeChildren(TreeNode p, String src, Dictionary<int, string> exp)
        {
            foreach (TreeNode n in p.Nodes)
            {
                /*
                if (n.Checked) // make sure all previously-checked selections are still visible
                {
                    exp.Add((n.Tag as List<FacetedSearch3.CUAHSIFacetedSearch.OntologyElement>).First().cConceptID, null); // null signifies that 
                }                
                else           // select to expose as visible if this node matches the user's search
                {*/
                    // create single list of names for string matching to this node
                    List<String> TagNames = (n.Tag as List<FacetedSearch3.CUAHSIFacetedSearch.OntologyElement>).Select(r => r.cConceptName).ToList();
                    TagNames.AddRange((n.Tag as List<FacetedSearch3.CUAHSIFacetedSearch.OntologyElement>).Select(r => r.Synonym));

                    // tag this node for exposure if it matches the current search term
                    if (TagNames.Distinct().Any(r => r.IndexOf(src, StringComparison.OrdinalIgnoreCase) >= 0))
                    {
                        exp.Add((n.Tag as List<FacetedSearch3.CUAHSIFacetedSearch.OntologyElement>).First().cConceptID, TagNames.Where(r => r.IndexOf(src, StringComparison.OrdinalIgnoreCase) >= 0).First());
                    }

                    if (n.Nodes.Count > 0)
                    {
                        Dictionary<int, string> cExp = GetExposeChildren(n, src, exp);
                        foreach (KeyValuePair<int, string> k in cExp)
                        {
                            try
                            {
                                if (!exp.ContainsKey(k.Key)) // examine... in theory, for just 1 search word, this should not be needed (but it is...)
                                {
                                    exp.Add(k.Key, k.Value);
                                }
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine("Error adding key {0} value {1} with message {2}", k.Key, k.Value, e.Message);
                            }
                        }
                    }
                // }
            }
            return exp;
        }    
    }
}