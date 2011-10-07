using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Search3.Settings.UI
{
    public partial class KeywordsUserControl : UserControl
    {
        #region Fields

        private IEnumerable<string> _keywords;

        #endregion

        #region Constructors

        public KeywordsUserControl()
        {
            InitializeComponent();

            // Keywords display
            rbList.Tag = DisplayMode.List;
            rbTree.Tag = DisplayMode.Tree;
            rbBoth.Tag = DisplayMode.Both;
            foreach (var rb in new[] {rbList, rbTree, rbBoth})
                rb.CheckedChanged += rb_CheckedChanged;
        }

        #endregion

        #region Public methods

        public void SetData(IEnumerable<string> keywords, OntologyTree ontologyTree)
        {
            if (keywords == null) throw new ArgumentNullException("keywords");
            if (ontologyTree == null) throw new ArgumentNullException("ontologyTree");

            _keywords = keywords;

            tboTypeKeyword.Clear();
            lblKeywordRelation.Text = "";
            lbKeywords.Items.Clear();
            treeviewOntology.Nodes.Clear();

            foreach (var keyword in keywords.Where(keyword => !lbKeywords.Items.Contains(keyword)))
            {
                lbKeywords.Items.Add(keyword);
            }
            FillTreeviewOntology(treeviewOntology.Nodes, ontologyTree.Nodes);
        }

        public void AddSelectedKeywords(IEnumerable<string> keywords)
        {
            if (keywords == null) throw new ArgumentNullException("keywords");

            foreach(var keyword in keywords)
            {
                if (!lbSelectedKeywords.Items.Contains(keyword))
                {
                    lbSelectedKeywords.Items.Add(keyword);
                }
            }
        }

        public void RemoveSelectedKeywords(IEnumerable<string> keywords = null)
        {
            if (keywords == null)
            {
                // remove all keywords
                lbSelectedKeywords.Items.Clear();
            }else
            {
                foreach(var keyword in keywords)
                {
                    lbSelectedKeywords.Items.Remove(keyword);
                }
            }
        }

        public IEnumerable<string> GetSelectedKeywords()
        {
            var res = new List<string>(lbSelectedKeywords.Items.Count);
            res.AddRange(from object item in lbSelectedKeywords.Items select item.ToString());
            return res;
        }

        #endregion

        #region Private methods

        private void FillTreeviewOntology(TreeNodeCollection treeNodeCollection, IEnumerable<OntologyNode> ontologyNodes)
        {
            foreach (var oNode in ontologyNodes)
            {
                var treeNode = CreateTreeNodeFromOntologyNode(oNode);
                treeNodeCollection.Add(treeNode);
                foreach (var oChild in oNode.Nodes)
                {
                    var childTreeNode = CreateTreeNodeFromOntologyNode(oChild);
                    treeNode.Nodes.Add(childTreeNode);
                    FillTreeviewOntology(childTreeNode.Nodes, oChild.Nodes);
                }
            }
        }

        private TreeNode CreateTreeNodeFromOntologyNode(OntologyNode oNode)
        {
            var res = new TreeNode(oNode.Text) { Tag = oNode };
            return res;
        }

        void rb_CheckedChanged(object sender, EventArgs e)
        {
            var displayMode = (DisplayMode)((RadioButton)sender).Tag;
            spcKey.Panel2Collapsed = displayMode == DisplayMode.List;
            spcKey.Panel1Collapsed = displayMode == DisplayMode.Tree;
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            //control display if user is in treeview mode
            if (rbTree.Checked)
            {
                rbList.Checked = true;
            }
            
            lbKeywords.ClearSelected();
            var searchString2 = tboTypeKeyword.Text;
            lbKeywords.SelectionMode = SelectionMode.MultiSimple;
            var x = -1;

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

        private void listBox1_MouseUp(object sender, MouseEventArgs e)
        {
            var keyIndex = lbKeywords.IndexFromPoint(e.Location);
            if (keyIndex < 0) return;

            tboTypeKeyword.Text = "";
            var strNode = lbKeywords.Items[keyIndex].ToString();

            foreach (var str in _keywords)
            {
                if (str.ToLower() == strNode.ToLower())
                {
                    tboTypeKeyword.Text = str;
                    FindInTreeView(treeviewOntology.Nodes, tboTypeKeyword.Text);
                }
            }

            //check ends
            if (tboTypeKeyword.Text == "")
            {
                tboTypeKeyword.Text = lbKeywords.Items[keyIndex].ToString();
                FindInTreeView(treeviewOntology.Nodes, tboTypeKeyword.Text);
            }
        }

        private void tvOntology_AfterSelect(object sender, TreeViewEventArgs e)
        {
            tboTypeKeyword.Text = e.Node.Text;
            FindInTreeView(treeviewOntology.Nodes, tboTypeKeyword.Text);
        }

        private void FindInTreeView(IEnumerable tncoll, string strNode)
        {
            foreach (TreeNode tnode in tncoll)
            {
                if (tnode.Text.ToLower() == strNode.ToLower())
                {
                    tnode.TreeView.SelectedNode = tnode;
                    lblKeywordRelation.Text = tnode.FullPath;
                }

                FindInTreeView(tnode.Nodes, strNode);
            }
        }

        private void btnAddKeyword_Click(object sender, EventArgs e)
        {
            if (lbKeywords.SelectedIndex == -1)
            {
                MessageBox.Show("Please select a valid Keyword.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var itemToAdd = lbKeywords.SelectedItem.ToString();
            if (GetSelectedKeywords().Any(item => item == itemToAdd))
            {
                MessageBox.Show("This Keyword is already selected, Please select another keyword.", "Information",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            AddSelectedKeywords(new[] {itemToAdd});
        }

        private void button15_Click(object sender, EventArgs e)
        {
            if (lbSelectedKeywords.SelectedItems.Count <= 0)
                return;

            var itemsToRemove = new List<string>(lbSelectedKeywords.SelectedItems.Count);
            itemsToRemove.AddRange(from object selected in lbSelectedKeywords.SelectedItems select selected.ToString());
            RemoveSelectedKeywords(itemsToRemove);
        }
       
        #endregion

        #region Helpers

        private enum DisplayMode
        {
            List,
            Tree,
            Both
        }

        #endregion
    }
}
