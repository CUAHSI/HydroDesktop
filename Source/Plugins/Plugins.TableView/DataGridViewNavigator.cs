using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

namespace HydroDesktop.Plugins.TableView
{
    public partial class DataGridViewNavigator : UserControl
    {
        #region Fields

        private const string LOADING_DATA = "Loading data...";
        private IPagedTableGetter _tableGetter;
        private BackgroundWorker _worker;

        #endregion

        #region Events

        public event EventHandler<PageChangedEventArgs> PageChanged;

        #endregion

        #region Constructors

        public DataGridViewNavigator()
        {
            InitializeComponent();

            DisableNavButtons();
            btnFirst.BackColor = btnPrev.BackColor = btnNext.BackColor = btnLast.BackColor = SystemColors.Control;

            btnFirst.Click += delegate { CurrentPage = 0; };
            btnPrev.Click += delegate { CurrentPage--; };
            btnNext.Click += delegate { CurrentPage++; };
            btnLast.Click += delegate { CurrentPage = PagesCount - 1; };

            PageChanged += DataGridViewNavigator_PageChanged;

            // TextBox setting
            tbInfo.Click += (sender, args) => tbInfo.SelectAll(); // AutoSelect all text on clicking
            tbInfo.CausesValidation = false;
            tbInfo.KeyPress += (sender, args) =>
                {
                    // Validate only if Enter was pressed
                    if (args.KeyChar == (char)Keys.Enter)
                    {
                        var pageToJump = ValidatePageJumperTextBox();
                        if (pageToJump.HasValue)
                        {
                            CurrentPage = pageToJump.Value - 1;
                        }
                    }
                };
        }

        #endregion

        #region Private methods

        private int? ValidatePageJumperTextBox()
        {
            var text = tbInfo.Text;

            var valid = false;
            int res;

            // Try parse text as Integer
            if (Int32.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out res))
            {
                valid = true;
            }
            else
            {
                // Try parse text as "n of m"
                if (!String.IsNullOrWhiteSpace(text))
                {
                    var split = text.Split(new[] { "of" }, StringSplitOptions.RemoveEmptyEntries);
                    int first, last;
                    if (split.Length == 2 && Int32.TryParse(split[0].Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out first) &&
                                             Int32.TryParse(split[1].Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out last) &&
                                             last == PagesCount)
                    {
                        res = first;
                        valid = true;
                    }
                }
            }
            // Check for interval
            string message;
            if (!valid)
            {
                message = "Please enter a valid page number.";
            }
            else if (res > PagesCount || res <= 0)
            {
                message = string.Format("Page number should be in interval [1; {0}]", PagesCount);
            }
            else
            {
                message = null;
            }
            errorProvider1.SetError(tbInfo, message);
            return message == null? res : (int?) null;
        }

        private void DisableNavButtons()
        {
            btnFirst.Enabled = btnPrev.Enabled = btnNext.Enabled = btnLast.Enabled = tbInfo.Enabled = false;
        }

        void DataGridViewNavigator_PageChanged(object sender, EventArgs e)
        {
            // Update navigation buttons
            btnFirst.Enabled = CurrentPage != 0;
            btnPrev.Enabled = CurrentPage > 0;
            btnNext.Enabled = CurrentPage != PagesCount - 1 && PagesCount > 0;
            btnLast.Enabled = CurrentPage < PagesCount - 1;

            tbInfo.Enabled = PagesCount > 0;
            tbInfo.Text = string.Format("{0} of {1}", PagesCount > 0 ? CurrentPage + 1 : 0, PagesCount);
            errorProvider1.SetError(tbInfo, null);
        }

        private void HideStatus()
        {
            lblStatus.Visible = false;
        }

        private void ShowStatus()
        {
            lblStatus.Text = LOADING_DATA;
            lblStatus.Visible = true;
        }

        #endregion

        #region Properties

        private int _valuesPerPage = 1000;
        /// <summary>
        /// Maximal count of values on page
        /// </summary>
        public int ValuesPerPage
        {
            get { return _valuesPerPage; }
            set
            {
                _valuesPerPage = value;
                if (_tableGetter != null)
                {
                    Initialize(_tableGetter);
                }
            }
        }

        private int _currentPage;

        /// <summary>
        /// Current page number (zero-based)
        /// </summary>
        public int CurrentPage
        {
            get { return _currentPage; }
            private set
            {
                if (value == _currentPage) return;
                if (value < 0 || value > PagesCount ||
                    (value == PagesCount && PagesCount != 0)) return;

                CheckNavigatorState();
                DisableNavButtons();
                ShowStatus();

                _currentPage = value;
                _worker = new BackgroundWorker();
                _worker.DoWork += delegate(object sender, DoWorkEventArgs args)
                                      {
                                          var table = _tableGetter.GetTable(ValuesPerPage, CurrentPage);
                                          args.Result = table;
                                      };
                _worker.RunWorkerCompleted += delegate(object sender, RunWorkerCompletedEventArgs args)
                                             {
                                                 var table = (DataTable) args.Result;
                                                 var handler = PageChanged;
                                                 if (handler != null)
                                                     handler(this, new PageChangedEventArgs(table));

                                                 HideStatus();
                                             };
                _worker.RunWorkerAsync();
            }
        }

        /// <summary>
        /// Total pages count
        /// </summary>
        public int PagesCount { get; private set; }

        #endregion

        #region Public methods

        private void CheckNavigatorState()
        {
            if (_worker != null && _worker.IsBusy)
            {
                throw new InvalidOperationException("Previous navigator's call is not finished.");
            }
        }

        /// <summary>
        /// Initialize navigator with queries to load data
        /// </summary>
        /// <param name="tableGetter">Class that returns data table for given ValuesPerPage and  CurrentPage.</param>
        public void Initialize(IPagedTableGetter tableGetter)
        {
            CheckNavigatorState();
            DisableNavButtons();
            ShowStatus();

            _tableGetter = tableGetter;

            _worker = new BackgroundWorker();
            _worker.DoWork += delegate(object sender, DoWorkEventArgs args)
                                  {
                                      var rowsCount = tableGetter.GetTotalCount();
                                      args.Result = rowsCount;
                                  };
            _worker.RunWorkerCompleted += delegate(object sender, RunWorkerCompletedEventArgs args)
                                              {
                                                  var rowsCount = (long) args.Result;
                                                  long remainder;
                                                  var div = (int) Math.DivRem(rowsCount, ValuesPerPage, out remainder);
                                                  PagesCount = remainder == 0 ? div : div + 1;
                                                  _currentPage = -1; // This need because CurrentPage checked for non-equality before setting it's value
                                                  CurrentPage = 0;
                                              };
            _worker.RunWorkerAsync();
        }

        #endregion
    }

    public interface IPagedTableGetter
    {
        DataTable GetTable(int valuesPerPage, int currentPage);
        long GetTotalCount();
    }

    public class PageChangedEventArgs : EventArgs
    {
        public DataTable DataTable { get; private set; }

        public PageChangedEventArgs(DataTable dataTable)
        {
            DataTable = dataTable;
        }
    }
}
