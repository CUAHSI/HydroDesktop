using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using HydroDesktop.Search.LayerInformation.PopupControl;

namespace HydroDesktop.Search.LayerInformation
{
    public partial class CustomToolTipControl : UserControl
    {
        #region Fields

        private GraphicsPath graphicsPath;
        private readonly Label[] labels;

        #endregion

        #region Constructors

        public CustomToolTipControl()
        {
            InitializeComponent();

            SizeChanged += CustomToolTipControl_SizeChanged;

            PointInfo = new PointInfo();

            PointInfo.PropertyChanged += PointInfo_PropertyChanged;

            lblServiceDesciptionUrl.TextChanged += lblServiceDesciptionUrl_TextChanged;
            lblServiceDesciptionUrl.LinkClicked += lblServiceDesciptionUrl_LinkClicked;

            labels = new[] { lblDataSource, lblSiteName, lblValueCount, lblServiceDesciptionUrl };

            foreach (var lbl in labels)
                lbl.SizeChanged += lbl_SizeChanged;

            Load += CustomToolTipControl_Load;
        }

        void PointInfo_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch(e.PropertyName)
            {
                case "DataSource":
                    lblDataSource.Text = PointInfo.DataSource;
                    break;
                case "SiteName":
                    lblSiteName.Text = PointInfo.SiteName;
                    break;
                case "ValueCountAsString":
                    lblValueCount.Text = PointInfo.ValueCountAsString;
                    break;
                case "ServiceDesciptionUrl":
                    lblServiceDesciptionUrl.Text = PointInfo.ServiceDesciptionUrl;
                    break;
            }
        }
       
        #endregion

        #region Properties

        public PointInfo PointInfo { get; private set; }
        public Popup Popup { get; set; }

        #endregion

        #region Private methods

        void lblServiceDesciptionUrl_TextChanged(object sender, EventArgs e)
        {
            lblServiceDesciptionUrl.Links[0].LinkData = lblServiceDesciptionUrl.Text;
        }

        void lblServiceDesciptionUrl_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                var target = e.Link.LinkData as string;

                // If the value looks like a URL, navigate to it.
                if (null != target && (target.StartsWith("http") || target.StartsWith("www")))
                {
                    System.Diagnostics.Process.Start(target);
                }

            }
        }

        void CustomToolTipControl_SizeChanged(object sender, EventArgs e)
        {
            Region = new Region(graphicsPath = CreateRoundRectangle(Width - 1, Height - 1, 6));
        }

        void lbl_SizeChanged(object sender, EventArgs e)
        {
            if (Popup == null) return;

            var width = 0;
            foreach (var lbl in labels)
                if (lbl.Size.Width > width)
                    width = lbl.Size.Width;

            Popup.Size = new Size(width + 20, Height);
        }

        void CustomToolTipControl_Load(object sender, EventArgs e)
        {
            BackColor = SystemColors.Info;
        }

        private static GraphicsPath CreateRoundRectangle(int w, int h, int r)
        {
            int d = r << 1;
            var path = new GraphicsPath();
            path.StartFigure();
            path.AddArc(new Rectangle(0, 0, d, d), 180, 90);
            path.AddLine(r, 0, w - r, 0);
            path.AddArc(new Rectangle(w - d, 0, d, d), 270, 90);
            path.AddLine(w + 1, r, w + 1, h - r);
            path.AddArc(new Rectangle(w - d, h - d, d, d), 0, 90);
            path.AddLine(w - r, h + 1, r, h + 1);
            path.AddArc(new Rectangle(0, h - d, d, d), 90, 90);
            path.AddLine(0, h - r, 0, r);
            path.CloseFigure();
            return path;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.TranslateTransform(-1, -1);
            using (var p = new Pen(SystemColors.WindowFrame, 2))
            {
                e.Graphics.DrawPath(p, graphicsPath);
            }
            e.Graphics.ResetTransform();
        }

        #endregion
    }

    public class PointInfo : INotifyPropertyChanged
    {
        const string unknown = "Unknown";

        private static readonly PointInfo Unknown = new PointInfo();

        public PointInfo()
        {
            DataSource = unknown;
            SiteName = unknown;
            ValueCount = null;
            ServiceDesciptionUrl = unknown;
        }

        private string _dataSource;
        public string DataSource
        {
            get { return _dataSource; }
            set
            {
                _dataSource = value;
                NotifyPropertyChanged("DataSource");
            }
        }

        private string _siteName;
        public string SiteName
        {
            get { return _siteName; }
            set
            {
                _siteName = value;
                NotifyPropertyChanged("SiteName");
            }
        }

        private int? _valueCount;
        public int? ValueCount
        {
            get { return _valueCount; }
            set
            {
                _valueCount = value;
                NotifyPropertyChanged("ValueCount");
                NotifyPropertyChanged("ValueCountAsString");
            }
        }

        private string _serviceDesciptionUrl;
        public string ServiceDesciptionUrl
        {
            get { return _serviceDesciptionUrl; }
            set
            {
                _serviceDesciptionUrl = value;
                NotifyPropertyChanged("ServiceDesciptionUrl");
            }
        }

        public string ValueCountAsString
        {
            get { return ValueCount.HasValue ? string.Format("{0} values", ValueCount) : unknown; }
        }

        public bool IsEmpty { get { return Equals(Unknown); } }

        public override bool Equals(object obj)
        {
            var pi = obj as PointInfo;
            if (pi == null) return false;

            return pi.DataSource == DataSource &&
                   pi.SiteName == SiteName &&
                   pi.ValueCount == ValueCount &&
                   pi.ServiceDesciptionUrl == ServiceDesciptionUrl;
        }
        public override int GetHashCode()
        {
            return DataSource.GetHashCode() ^ SiteName.GetHashCode() ^
                   ValueCount.GetHashCode() ^ ServiceDesciptionUrl.GetHashCode();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }

    }
}
