using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using HydroDesktop.DataDownload.Downloading;
using HydroDesktop.DataDownload.LayerInformation.PopupControl;

namespace HydroDesktop.DataDownload.LayerInformation
{
    public partial class CustomToolTipControl : UserControl
    {
        #region Fields

        private GraphicsPath graphicsPath;
        private ServiceInfoGroup _serviceInfo = new ServiceInfoGroup();

        const int CONTROLS_START_X = 13;
        const int CONSTROLS_START_Y = 11;
        const int VERTICAL_PADDING = 3;

        #endregion

        #region Constructors

        public CustomToolTipControl()
        {
            InitializeComponent();

            SizeChanged += CustomToolTipControl_SizeChanged;
            Load += CustomToolTipControl_Load;
        }
       
        #endregion

        #region Properties

        public Popup Popup { get; set; }

        #endregion

        #region Public methods

        /// <summary>
        /// Is info already setted on current tooltip
        /// </summary>
        /// <param name="info">Info to check</param>
        /// <returns>True - setted, false - otherwise</returns>
        public bool IsInfoAlreadySetted(ServiceInfoGroup info)
        {
            return _serviceInfo.Equals(info);
        }

        /// <summary>
        /// Set info to current tooltip
        /// </summary>
        /// <param name="info">Info to set</param>
        /// <exception cref="ArgumentNullException"><paramref name="info"/>must be not null</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="info"/> must be non-empty (IsEmpty = false)</exception>
        public void SetInfo(ServiceInfoGroup info)
        {
            if (info == null) throw new ArgumentNullException("info");
            if (info.IsEmpty) throw new ArgumentOutOfRangeException("info", "info must be non-empty");

            _serviceInfo = info;
            
            SuspendLayout();

            Controls.Clear();
            
            var thisWidth = 0;
            var startX = CONTROLS_START_X;
            var startY = CONSTROLS_START_Y;

            var itemForCommonParts = info.GetItems().First();

            // Data Source label
            var lbDataSource = new LinkLabel {AutoSize = true, Location = new Point(startX, startY)};
            lbDataSource.LinkClicked += lblServiceDesciptionUrl_LinkClicked;
            AddControl(lbDataSource, ref startY);
            lbDataSource.Text = itemForCommonParts.DataSource;
            lbDataSource.Links[0].LinkData = itemForCommonParts.ServiceDesciptionUrl;
            if (lbDataSource.Width > thisWidth) thisWidth = lbDataSource.Width;

            // Site Name label
            startY += 2;
            var lbSiteName = new Label {AutoSize = true, Location = new Point(startX, startY)};
            AddControl(lbSiteName, ref startY);
            lbSiteName.Text = itemForCommonParts.SiteName;
            if (lbSiteName.Width > thisWidth) thisWidth = lbSiteName.Width;

            foreach(var item in info.GetItems())
            {
                // Variable label
                var lbVariable = new Label {AutoSize = true, Location = new Point(startX, startY)};
                AddControl(lbVariable, ref startY);
                lbVariable.Text = string.Format("{0}{1} - {2}{3}",
                                                item.VarName,
                                                info.ItemsCount <= 1? string.Empty : ", " + item.DataType,
                                                item.ValueCountAsString,
                                                item.IsDownloaded ? string.Empty : " (estimated)");
                if (lbVariable.Width > thisWidth) thisWidth = lbVariable.Width;
            }
            
            // Download data label
            startY += 2;
            var lbDowloadData = new LinkLabel {AutoSize = true, Location = new Point(startX, startY)};
            lbDowloadData.LinkClicked += lblDownloadData_LinkClicked;
            AddControl(lbDowloadData, ref startY);
            lbDowloadData.Text = "Download data";
            if (lbDowloadData.Width > thisWidth) thisWidth = lbDowloadData.Width;
            lbDowloadData.Location = new Point(thisWidth - lbDowloadData.Width + 10, lbDowloadData.Location.Y);

            Popup.Width = thisWidth + 20;
            Popup.Height = startY + CONSTROLS_START_Y;

            ResumeLayout(true);
            CustomToolTipControl_SizeChanged(this, EventArgs.Empty);
        }

        private void AddControl(Control cntrl, ref int startY)
        {
            Controls.Add(cntrl);
            startY += cntrl.Size.Height + VERTICAL_PADDING;
        }

        #endregion

        #region Private methods

        void lblDownloadData_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Debug.Assert(!_serviceInfo.IsEmpty);

            if (Popup != null)
            {
                Popup.Close();
            }

            var seriesList = new List<OneSeriesDownloadInfo>(_serviceInfo.ItemsCount);
            seriesList.AddRange(_serviceInfo.GetItems().Select(ClassConvertor.ServiceInfoToOneSeriesDownloadInfo));
            var layer = _serviceInfo.GetItems().First().Layer; // we have at least one element

            var dataThemeName = layer.LegendText;
            var startArgs = new StartDownloadArg(seriesList, dataThemeName);
            Global.PluginEntryPoint.StartDownloading(startArgs, layer);
        }

        void lblServiceDesciptionUrl_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                var target = e.Link.LinkData as string;

                // If the value looks like a URL, navigate to it.
                if (null != target && (target.StartsWith("http") || target.StartsWith("www")))
                {
                    Process.Start(target);
                }

            }
        }

        void CustomToolTipControl_SizeChanged(object sender, EventArgs e)
        {
            Region = new Region(graphicsPath = CreateRoundRectangle(Width - 1, Height - 1, 6));
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
            var isDowloaded = !_serviceInfo.IsEmpty && _serviceInfo.GetItems().All(item => item.IsDownloaded);
            return    isDowloaded
                       ? new Pen(Color.Green, 5)
                       : new Pen(SystemColors.WindowFrame, 2);
        }

        #endregion
    }
}
