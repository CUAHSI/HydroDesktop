using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Search3.Settings.UI
{
    public partial class KeywordsUserControl : UserControl
    {
        #region Constructors

        public KeywordsUserControl()
        {
            InitializeComponent();
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Set data into control.
        /// </summary>
        /// <param name="keywordsSettings">Keyword settings.</param>
        /// <exception cref="ArgumentNullException">Throws if <paramref name="keywordsSettings"/> is null.</exception>
        public void BindKeywordsSettings(KeywordsSettings keywordsSettings)
        {
            if (keywordsSettings == null) throw new ArgumentNullException("keywordsSettings");

            tboTypeKeyword.Clear();
            lblKeywordRelation.Text = "";
            tboTypeKeyword.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            var autoCompleteSource = new AutoCompleteStringCollection();
            autoCompleteSource.AddRange(keywordsSettings.Keywords.ToArray());
            tboTypeKeyword.AutoCompleteCustomSource = autoCompleteSource;
            tboTypeKeyword.AutoCompleteSource = AutoCompleteSource.CustomSource;

            tboTypeKeyword.KeyDown += delegate(object sender, KeyEventArgs args)
                                          {
                                              if (args.KeyCode == Keys.Enter)
                                              {
                                                  treeviewOntology.SelectedNode = null;

                                                  // Replace keyword by synonym
                                                  var keyword = tboTypeKeyword.Text.Trim();
                                                  keyword = keywordsSettings.FindSynonym(keyword);
                                                  
                                                  UpdateKeywordTextBox(keyword);
                                                  AddKeyword();
                                              }
                                          };
            
            // Ontology tree
            treeviewOntology.AfterSelect += tvOntology_AfterSelect;
            treeviewOntology.BeginUpdate();
            treeviewOntology.Nodes.Clear();
            FillTreeviewOntology(treeviewOntology.Nodes, keywordsSettings.OntologyTree.Nodes);
            treeviewOntology.EndUpdate();

            // Selected keywords
            AddSelectedKeywords(keywordsSettings.SelectedKeywords);
            if (keywordsSettings.SelectedKeywords.Any())
            {
                // Select first keyword in textbox
                UpdateKeywordTextBox(keywordsSettings.SelectedKeywords.First());
            }
        }

        /// <summary>
        /// Add selected keywords.
        /// </summary>
        /// <param name="keywords">Keywords to add.</param>
        /// <exception cref="ArgumentNullException">Throws if <paramref name="keywords"/> is null.</exception>
        private void AddSelectedKeywords(IEnumerable<string> keywords)
        {
            if (keywords == null) throw new ArgumentNullException("keywords");

            foreach (var keyword in keywords.Where(keyword => !lbSelectedKeywords.Items.Contains(keyword)))
            {
                lbSelectedKeywords.Items.Add(keyword);
            }
        }

        /// <summary>
        /// Remove selected keywords.
        /// </summary>
        /// <param name="keywords">Keywords to remove. If parameter is null - removes all selected keywords.</param>
        private void RemoveSelectedKeywords(IEnumerable<string> keywords = null)
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

        /// <summary>
        /// Get selected keywords.
        /// </summary>
        /// <returns>Selected keywords.</returns>
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

        private void UpdateKeywordTextBox(string text)
        { 
            treeviewOntology.AfterSelect -= tvOntology_AfterSelect;
            tboTypeKeyword.Text = text;
            FindInTreeView(treeviewOntology.Nodes, tboTypeKeyword.Text);
            treeviewOntology.AfterSelect += tvOntology_AfterSelect;
        }

        private void tvOntology_AfterSelect(object sender, TreeViewEventArgs e)
        {
            UpdateKeywordTextBox(e.Node.Text);
        }

        private bool FindInTreeView(IEnumerable tncoll, string strNode)
        {
            foreach (TreeNode tnode in tncoll)
            {
                if (string.Equals(tnode.Text, strNode, StringComparison.OrdinalIgnoreCase))
                {
                    tnode.TreeView.SelectedNode = tnode;
                    lblKeywordRelation.Text = tnode.FullPath;
                    return true;
                }

                var res = FindInTreeView(tnode.Nodes, strNode);
                if (res)
                {
                    return true;
                }
            }
            return false;
        }

        private void btnAddKeyword_Click(object sender, EventArgs e)
        {
            AddKeyword();
        }

        private void AddKeyword()
        {
            var node = treeviewOntology.SelectedNode;
            if (node == null)
            {
                MessageBox.Show("Please select a valid Keyword.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var itemToAdd = node.Text;
            if (GetSelectedKeywords().Any(item => item == itemToAdd))
            {
                MessageBox.Show("This Keyword is already selected, Please select another keyword.", "Information",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            AddSelectedKeywords(new[] { itemToAdd });
        }

        private void btnRemoveKeyword_Click(object sender, EventArgs e)
        {
            if (lbSelectedKeywords.SelectedItems.Count <= 0)
                return;

            var itemsToRemove = new List<string>(lbSelectedKeywords.SelectedItems.Count);
            itemsToRemove.AddRange(from object selected in lbSelectedKeywords.SelectedItems select selected.ToString());
            RemoveSelectedKeywords(itemsToRemove);
        }
       
        #endregion
    }
}
