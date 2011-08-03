using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using HydroDesktop.DataDownload.Downloading;
using HydroDesktop.DataDownload.LayerInformation.PopupControl;

namespace HydroDesktop.DataDownload.LayerInformation
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

            PointInfo = new ServiceInfo();
            PointInfo.PropertyChanged += PointInfo_PropertyChanged;

            lblServiceDesciptionUrl.TextChanged += lblServiceDesciptionUrl_TextChanged;
            lblServiceDesciptionUrl.LinkClicked += lblServiceDesciptionUrl_LinkClicked;
            lblDownloadData.LinkClicked += lblDownloadData_LinkClicked;

            labels = new[] { lblDataSource, lblSiteName, lblValueCount, lblServiceDesciptionUrl, lblDownloadData };

            foreach (var lbl in labels)
                lbl.SizeChanged += lbl_SizeChanged;

            Load += CustomToolTipControl_Load;
        }
       
        #endregion

        #region Properties

        public ServiceInfo PointInfo { get; private set; }
        public Popup Popup { get; set; }

        #endregion

        #region Private methods

        void PointInfo_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "DataSource":
                    lblDataSource.Text = PointInfo.DataSource;
                    break;
                case "SiteName":
                    lblSiteName.Text = PointInfo.SiteName;
                    break;
                case "ValueCountAsString":
                    lblValueCount.Text = string.Format("{0}{1}",
                        PointInfo.ValueCountAsString,
                        PointInfo.IsDownloaded ? string.Empty : " (estimated)");
                    break;
                case "ServiceDesciptionUrl":
                    lblServiceDesciptionUrl.Text = PointInfo.ServiceDesciptionUrl;
                    break;
                case "IsDownloaded":
                    lblDownloadData.Text = PointInfo.IsDownloaded ? "Download Updated Data" : "Download Data";
                    break;
            }
        }

        void lblDownloadData_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (Popup != null)
            {
                Popup.Close();
            }
            var oneSeries = ClassConvertor.ServiceInfoToOneSeriesDownloadInfo(PointInfo);
            var dataThemeName = PointInfo.Layer.LegendText;
            var startArgs = new StartDownloadArg(new List<OneSeriesDownloadInfo> {oneSeries}, dataThemeName);

            Global.PluginEntryPoint.StartDownloading(startArgs, PointInfo.Layer);
        }

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
            using (var p = GetBorderPen())
            {
                e.Graphics.DrawPath(p, graphicsPath);
            }
            e.Graphics.ResetTransform();
        }

        private Pen GetBorderPen()
        {
            return PointInfo.IsDownloaded
                       ? new Pen(Color.Green, 5)
                       : new Pen(SystemColors.WindowFrame, 2);
        }

        #endregion
    }
}
