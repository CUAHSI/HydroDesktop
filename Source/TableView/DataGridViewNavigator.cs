using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace TableView
{
    public partial class DataGridViewNavigator : UserControl
    {
        #region Fields

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
                if (_tableGetter != null)
                {
                    Initialize(_tableGetter);
                }
            }
        }

        private int _currentPage;
        private IPagedTableGetter _tableGetter;
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

                var table = _tableGetter.GetTable(ValuesPerPage, CurrentPage);
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
        /// <param name="tableGetter">Class that returns data table for given ValuesPerPage and  CurrentPage.</param>
        public void Initialize(IPagedTableGetter tableGetter)
        {
            _tableGetter = tableGetter;
            var rowsCount = tableGetter.GetTotalCount();
            
            var needNavigation = rowsCount > ValuesPerPage;
            btnFirst.Enabled = btnPrev.Enabled = btnNext.Enabled = btnLast.Enabled = needNavigation;

            long remainder;
            var div = (int)Math.DivRem(rowsCount, ValuesPerPage, out remainder);
            PagesCount = remainder == 0 ? div : div + 1;

            CurrentPage = 0;
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
