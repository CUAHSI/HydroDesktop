using System;
using System.Windows.Forms;

namespace Search3.Settings.UI
{
    public partial class DateSettingsDialog : Form
    {
        private readonly DateSettings _dateSettings;

        private DateSettingsDialog(DateSettings dateSettings)
        {
            InitializeComponent();
            

            _dateSettings = dateSettings;

            dtpStartDate.MinDate = DateTime.MinValue;
            dtpStartDate.MaxDate = DateTime.MaxValue;
            dtpEndDate.MinDate = DateTime.MinValue;
            dtpEndDate.MaxDate = DateTime.MaxValue;

            dtpStartDate.DataBindings.Add(new Binding("Value", _dateSettings, "StartDate")
                                              {DataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged});
            dtpEndDate.DataBindings.Add(new Binding("Value", _dateSettings, "EndDate")
                                            {DataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged});

            // Quick radio Buttons
            rbLastDay.Tag = RoundMode.LastDay;
            rbLastWeek.Tag = RoundMode.LastWeek;
            rbLastMonth.Tag = RoundMode.LastMonth;
            rbLastYear.Tag = RoundMode.LastYear;
            foreach (var rb in new[] { rbLastDay, rbLastWeek, rbLastMonth, rbLastYear })
                rb.CheckedChanged += rbQuickMode_CheckedChanged;
        }

        void rbQuickMode_CheckedChanged(object sender, EventArgs e)
        {
            var radioButton = sender as RadioButton;
            if (radioButton == null ||
                !radioButton.Checked) return;

            var roundMode = (RoundMode)radioButton.Tag;
            switch(roundMode)
            {
                case RoundMode.LastDay:
                    dtpStartDate.Value = dtpEndDate.Value;
                    break;
                case RoundMode.LastWeek:
                    dtpStartDate.Value = dtpEndDate.Value.AddDays(-7);
                    break;
                case RoundMode.LastMonth:
                    dtpStartDate.Value = dtpEndDate.Value.AddDays(-30);
                    break;
                case RoundMode.LastYear:
                    dtpStartDate.Value = dtpEndDate.Value.AddDays(-365);
                    break;
            }
        }

        public static DialogResult ShowDialog(DateSettings dateSettings)
        {
            if (dateSettings == null) throw new ArgumentNullException("dateSettings");

            using(var form = new DateSettingsDialog(dateSettings.Copy()))
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    dateSettings.Copy(form._dateSettings);
                }

                return form.DialogResult;
            }
        }

        private enum RoundMode
        {
            LastDay,
            LastWeek,
            LastMonth,
            LastYear
        }
    }
}
