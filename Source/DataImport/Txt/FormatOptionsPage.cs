using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using HydroDesktop.Common.Tools;
using Wizard.UI;

namespace DataImport.Txt
{
    public partial class FormatOptionsPage : InternalWizardPage
    {
        private readonly DataImportContext _context;
        private readonly TxtImportSettings _settings;

        public FormatOptionsPage(DataImportContext context)
        {
            _context = context;
            _settings = (TxtImportSettings) context.Settings;

            InitializeComponent();
            
            // FileType combo box
            cmbFileType.DataSource = Enum.GetValues(typeof(TxtFileType));
            cmbFileType.Format += delegate(object s, ListControlConvertEventArgs args)
            {
                args.Value = ((TxtFileType)args.ListItem).Description();
            };
            cmbFileType.SelectedValueChanged += CmbFileTypeOnSelectedValueChanged;
            cmbFileType.DataBindings.Clear();
            cmbFileType.DataBindings.Add("SelectedItem", _settings, "FileType", true, DataSourceUpdateMode.OnPropertyChanged);
            CmbFileTypeOnSelectedValueChanged(cmbFileType, EventArgs.Empty);
        }

        private void CmbFileTypeOnSelectedValueChanged(object sender, EventArgs eventArgs)
        {
            var txtFileType = (TxtFileType) cmbFileType.SelectedItem;
            switch (txtFileType)
            {
                case TxtFileType.FixedWidth:
                    gbxDelimiters.Visible = false;
                    dgvPreview.Location = new Point(gbxDelimiters.Location.X,
                                                    gbxDelimiters.Location.Y);
                    dgvPreview.Height = Height - dgvPreview.Location.Y - 20;
                    break;
                case TxtFileType.Delimited:
                    gbxDelimiters.Visible = true;
                    dgvPreview.Location = new Point(gbxDelimiters.Location.X,
                                                    gbxDelimiters.Location.Y + gbxDelimiters.Height + 5);
                    dgvPreview.Height = Height - dgvPreview.Location.Y - 20;
                    break;
            }
        }

        private void FormatOptionsPage_SetActive(object sender, CancelEventArgs e)
        {

        }
    }
}
