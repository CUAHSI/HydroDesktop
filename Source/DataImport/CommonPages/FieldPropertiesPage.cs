using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using HydroDesktop.Interfaces.ObjectModel;
using Wizard.UI;

namespace DataImport.CommonPages
{
    public partial class FieldPropertiesPage : InternalWizardPage
    {
        private readonly DataImportContext _context;
        private readonly int _columnHeight;

        public FieldPropertiesPage(DataImportContext context)
        {
            _context = context;
            InitializeComponent();

            _columnHeight = dgvPreview.ColumnHeadersHeight;
        }

        private void FieldPropertiesPage_SetActive(object sender, CancelEventArgs e)
        {
            SetWizardButtons(WizardButtons.Next | WizardButtons.Back);

            dgvPreview.Controls.Clear();
            dgvPreview.DataSource = _context.Settings.Preview;
            
            dgvPreview.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
            dgvPreview.ColumnHeadersHeight = 3 * _columnHeight;
            dgvPreview.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            for (int i = 0; i < dgvPreview.Columns.Count; i++)
            {
                var column = dgvPreview.Columns[i];
                column.SortMode = DataGridViewColumnSortMode.NotSortable;
                
                var columnName = column.Name;
                var columnSize = column.HeaderCell.Size;
                var columnLocation = dgvPreview.GetCellDisplayRectangle(i, -1, true).Location;

                var columnData = new ColumnData {ColumnIndex = i};

                // Label with column name
                var label = new Label {Text = columnName, Visible = true};
                dgvPreview.Controls.Add(label);
                label.Location = new Point(columnLocation.X, columnLocation.Y);
                label.Size = new Size(columnSize.Width, _columnHeight);

                // "Import column" checkbox
                var checkBox = new CheckBox {Text = "Import", Checked = true, Visible = true};
                dgvPreview.Controls.Add(checkBox);
                checkBox.Location = new Point(columnLocation.X, columnLocation.Y + _columnHeight);
                checkBox.Size = new Size(columnSize.Width, _columnHeight);
                
                // Additional properties button
                var button = new Button {Text = "...", Visible = true};
                dgvPreview.Controls.Add(button);
                button.Location = new Point(columnLocation.X, columnLocation.Y + _columnHeight * 2);
                button.Size = new Size(columnSize.Width, _columnHeight);
                button.Click += delegate
                                    {
                                        var cData = (ColumnData) button.Tag;
                                        using (var form = new FieldPropertiesForm((ColumnData) cData.Clone()))
                                        {
                                            var res = form.ShowDialog();
                                            if (res == DialogResult.OK)
                                            {
                                                button.Tag = form.ColumnData;
                                            }
                                        }
                                    };
                button.Tag = columnData;

                checkBox.CheckedChanged += delegate
                                               {
                                                   columnData.ImportColumn = checkBox.Checked;
                                                   button.Enabled = checkBox.Checked;
                                               };
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var createSiteform = new CreateSiteForm();
            createSiteform.ShowDialog();
            var site = createSiteform.Entity;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var form = new DetailsForm();
            form.ShowDialog();
            var q = form.QualityControlLevel;
        }
    }

    public class ColumnData : ICloneable
    {
        public bool ImportColumn { get; set; }
        public int ColumnIndex { get; set; }

        public Site Site { get; set; }
        public Variable Variable { get; set; }

        public QualityControlLevel QualityControlLevel { get; set; }
        public Method Method { get; set; }
        public Source Source { get; set; }

        public object Clone()
        {
            var copy = (ColumnData) MemberwiseClone();

            if (copy.Site != null) copy.Site = (Site) copy.Site.Clone();
            if (copy.Variable != null) copy.Variable = (Variable) copy.Variable.Clone();
            if (copy.QualityControlLevel != null) copy.QualityControlLevel = (QualityControlLevel) copy.QualityControlLevel.Clone();
            if (copy.Method != null) copy.Method = (Method) copy.Method.Clone();
            if (copy.Source != null) copy.Source = (Source) copy.Source.Clone();

            return copy;
        }
    }
    
}
