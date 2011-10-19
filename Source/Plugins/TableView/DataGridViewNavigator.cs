using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using HydroDesktop.Database;

namespace TableView
{
    public partial class DataGridViewNavigator : UserControl
    {
        #region Fields

        private DbOperations _dbOperations;
        private string _dataQuery;
        private string _countQuery;

        #endregion

        #region Events

        public event EventHandler<PageChangedEventArgs> PageChanged;

        #endregion

        #region Constructors

        public DataGridViewNavigator()
        {
            InitializeComponent();

            btnFirst.Enabled = btnPrev.Enabled = btnNext.Enabled = btnLast.Enabled = false;
            btnFirst.BackColor = btnPrev.BackColor = btnNext.BackColor = btnLast.BackColor = SystemColors.Control;
            lblInfo.Text = string.Empty;

            btnFirst.Click += delegate { CurrentPage = 0; };
            btnPrev.Click += delegate { CurrentPage--; };
            btnNext.Click += delegate { CurrentPage++; };
            btnLast.Click += delegate { CurrentPage = PagesCount - 1; };

            PageChanged += DataGridViewNavigator_PageChanged;
        }

        #endregion

        #region Private methods

        void DataGridViewNavigator_PageChanged(object sender, EventArgs e)
        {
            // Update naivagation buttons
            btnFirst.Enabled = CurrentPage != 0;
            btnPrev.Enabled = CurrentPage > 0;
            btnNext.Enabled = CurrentPage != PagesCount - 1 && PagesCount > 0;
            btnLast.Enabled = CurrentPage < PagesCount - 1;

            lblInfo.Text = string.Format("{0} of {1}", PagesCount > 0? CurrentPage + 1 : 0, PagesCount);
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
                if (_dbOperations != null && _dataQuery != null && _countQuery != null)
                    Initialize(_dbOperations, _dataQuery, _countQuery);
            }
        }

        private int _currentPage;
        /// <summary>
        /// Current page number
        /// </summary>
        public int CurrentPage
        {
            get { return _currentPage; }
            private set
            {
                if (value < 0 || value > PagesCount ||
                    (value == PagesCount && PagesCount != 0)) return;

                _currentPage = value;
               
                var table = _dbOperations.LoadTable(string.Format("{0} limit {1} offset {2}", _dataQuery, ValuesPerPage, CurrentPage * ValuesPerPage));

                var handler = PageChanged;
                if (handler != null)
                    handler(this, new PageChangedEventArgs(table));
            }
        }

        /// <summary>
        /// Total pages count
        /// </summary>
        public int PagesCount {get;private set; }

        #endregion

        #region Public methods

        /// <summary>
        /// Initialize navigator with queries to load data
        /// </summary>
        /// <param name="dbOperations">DbOperations</param>
        /// <param name="dataQuery">Query to select data</param>
        /// <param name="countQuery">Query to count data</param>
        public void Initialize(DbOperations dbOperations, string dataQuery, string countQuery)
        {
            if (dbOperations == null) throw new ArgumentNullException("dbOperations");
            if (dataQuery == null) throw new ArgumentNullException("dataQuery");
            if (countQuery == null) throw new ArgumentNullException("countQuery");
            
            _dbOperations = dbOperations;
            _dataQuery = dataQuery;
            _countQuery = countQuery;

            var count = Convert.ToInt32(dbOperations.ExecuteSingleOutput(countQuery));
            var needNavigation = count > ValuesPerPage;
            btnFirst.Enabled = btnPrev.Enabled = btnNext.Enabled = btnLast.Enabled = needNavigation;

            int remainder;
            var div = Math.DivRem(count, ValuesPerPage, out remainder);
            PagesCount = remainder == 0 ? div : div + 1;

            CurrentPage = 0;
        }

        #endregion
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
