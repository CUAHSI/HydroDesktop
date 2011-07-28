using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;

namespace HydroDesktop.DataDownload.Download
{
    public partial class RedownloadControl : UserControl
    {
        public event EventHandler<RedownloadArgs> DoRedownload;

        public RedownloadControl()
        {
            InitializeComponent();
            InitCombo();
        }

        private RedownloadOption CurrentOption
        {
            get
            {
                return ((KeyValuePair<RedownloadOption, string>)cmbOptions.SelectedItem).Key;
            }
        }

        private void InitCombo()
        {
            cmbOptions.DropDownStyle = ComboBoxStyle.DropDownList;

            var enumValues = new List<KeyValuePair<RedownloadOption, string>>
                                 {
                                     new KeyValuePair<RedownloadOption, string>(RedownloadOption.AllWithErrors,
                                                                                GetDescription(RedownloadOption.AllWithErrors)),
                                     new KeyValuePair<RedownloadOption, string>(RedownloadOption.SelectedWithErrors,
                                                                                GetDescription(RedownloadOption.SelectedWithErrors)),
                                     new KeyValuePair<RedownloadOption, string>(RedownloadOption.AllSelected,
                                                                                GetDescription(RedownloadOption.AllSelected)),
                                     new KeyValuePair<RedownloadOption, string>(RedownloadOption.All,
                                                                                GetDescription(RedownloadOption.All)),
                                 };
            
            cmbOptions.DataSource = enumValues;
            cmbOptions.DisplayMember = "Value";
            cmbOptions.ValueMember = "Key";
        }

        private static string GetDescription(Enum value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            string description = value.ToString();
            var fieldInfo = value.GetType().GetField(description);
            var attributes =
                (DescriptionAttribute[])
                fieldInfo.GetCustomAttributes(typeof (DescriptionAttribute), false);

            if (attributes.Length > 0)
            {
                description = attributes[0].Description;
            }
            return description;
        }

        private void btnRedownload_Click(object sender, EventArgs e)
        {
            var handler = DoRedownload;
            if (handler != null)
                handler(this, new RedownloadArgs(CurrentOption));
        }
    }

    public enum RedownloadOption
    {
        [Description("All series with errors")]
        AllWithErrors,
        [Description("Selected with status = error")]
        SelectedWithErrors,
        [Description("All selected (any status)")]
        AllSelected,
        [Description("All")]
        All
    }

    public class RedownloadArgs : EventArgs
    {
        public RedownloadOption RedownloadOption { get; private set; }

        public RedownloadArgs(RedownloadOption redownloadOption)
        {
            RedownloadOption = redownloadOption;
        }
    }
}
