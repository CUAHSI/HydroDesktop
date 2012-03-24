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
        private readonly List<ServiceInfoGroup> _serviceInfo = new List<ServiceInfoGroup>();

        const int CONTROLS_START_X = 13;
        const int CONSTROLS_START_Y = 11;
        const int VERTICAL_PADDING = 3;
        const int HORISONTAL_PADDING = 13;

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
        public bool IsInfoAlreadySetted(List<ServiceInfoGroup> info)
        {
            if (info == null) throw new ArgumentNullException("info");

            if (_serviceInfo.Count != info.Count) return false;
            return _serviceInfo.All(item => info.Exists(infoItem => infoItem.Equals(item)));
        }

        /// <summary>
        /// Set info to current tooltip
        /// </summary>
        /// <param name="infos">Info to set</param>
        /// <exception cref="ArgumentNullException"><paramref name="infos"/>must be not null</exception>
        public void SetInfo(IEnumerable<ServiceInfoGroup> infos)
        {
            if (infos == null) throw new ArgumentNullException("infos");
            _serviceInfo.Clear();
            _serviceInfo.AddRange(infos.Where(item => !item.IsEmpty));
            
            SuspendLayout();

            Controls.Clear();
          
            var controls = new List<UserControl>();
            foreach(var info in _serviceInfo)
            {
                var userControl = new UserControl {Tag = info};
                AddInfoIntoContainer(info, userControl);
                controls.Add(userControl);
            }

            Control controlToAdd;
            if (controls.Count == 1)
            {
                controlToAdd = controls[0];
            }else
            {
                var tabControl = new TabControl {Width = 0, Height = 0};
                tabControl.Padding = new Point();
                tabControl.DrawMode = TabDrawMode.OwnerDrawFixed;
                tabControl.DrawItem += tabControl_DrawItem;
                foreach (var userControl in controls)
                {
                    var itemForCommonParts = ((ServiceInfoGroup)(userControl.Tag)).GetItems().First();

                    var tabPage = new TabPage(itemForCommonParts.DataSource)
                                      {
                                          Padding = new Padding(0,0,0,0),
                                          Width = userControl.Width,
                                          Height = userControl.Height,
                                          BackColor = GetBackColor(),
                                      };
                    
                    tabPage.Controls.Add(userControl);
                    tabPage.Controls[0].Dock = DockStyle.Fill;

                    var needWidth = tabPage.Width + 8; 
                    if (tabControl.Width < needWidth)
                    {
                        tabControl.Width = needWidth;
                    }
                    var needHeight = tabPage.Height + 8 + tabControl.ItemSize.Height +
                                     tabControl.Padding.Y + 8;
                    if (tabControl.Height < needHeight)
                    {
                        tabControl.Height = needHeight;
                    }

                    tabControl.TabPages.Add(tabPage);
                }
                controlToAdd = tabControl;
            }

            controlToAdd.Paint += userControl_Paint;
            Controls.Add(controlToAdd);
            Popup.Width = controlToAdd.Width;
            Popup.Height = controlToAdd.Height;
            controlToAdd.Dock = DockStyle.Fill;

            ResumeLayout(true);
            CustomToolTipControl_SizeChanged(this, EventArgs.Empty);
        }

        private void AddInfoIntoContainer(ServiceInfoGroup info, Control container)
        {
            var thisWidth = 0;
            var startX = CONTROLS_START_X;
            var startY = CONSTROLS_START_Y;

            var itemForCommonParts = info.GetItems().First();

            // Data Source label
            var lbDataSource = new LinkLabel { AutoSize = true, Location = new Point(startX, startY) };
            lbDataSource.LinkClicked += lblServiceDesciptionUrl_LinkClicked;
            AddControl(container, lbDataSource);
            lbDataSource.Text = itemForCommonParts.DataSource;
            lbDataSource.Links[0].LinkData = itemForCommonParts.ServiceDesciptionUrl;
            lbDataSource.Links[0].Enabled = IsValidServiceDesciptionUrl(itemForCommonParts.ServiceDesciptionUrl);
            CalculateContainerSize(lbDataSource, ref thisWidth, ref startY);

            // Site Name label
            startY += 2;
            var lbSiteName = new Label { AutoSize = true, Location = new Point(startX, startY) };
            AddControl(container, lbSiteName);
            lbSiteName.Text = itemForCommonParts.SiteName;
            CalculateContainerSize(lbSiteName, ref thisWidth, ref startY);

            // Variable labels...
            var sameVarName = info.GetItems().All(item => itemForCommonParts.VarName == item.VarName);
            var sameType = info.GetItems().All(item => itemForCommonParts.DataType == item.DataType);
            var showDataType = info.ItemsCount > 1 &&
                               (!sameType || sameVarName);
            const int max_variables_count = 10; // If variables list is too long, limit display to max_variables_count
            var variablesList = info.GetItems().ToList();
            for (int i = 0; i < variablesList.Count && i < max_variables_count; i++)
            {
                var item = variablesList[i];
                var lbVariable = new Label { AutoSize = true, Location = new Point(startX, startY) };
                AddControl(container, lbVariable);
                lbVariable.Text = string.Format("{0}{1} - {2}{3}",
                                                item.VarName,
                                                !showDataType ? string.Empty : ", " + item.DataType,
                                                item.ValueCountAsString,
                                                item.IsDownloaded ? string.Empty : " (estimated)");
                CalculateContainerSize(lbVariable, ref thisWidth, ref startY);
            }
            if (variablesList.Count > max_variables_count)
            {
                var lbVariable = new Label { AutoSize = true, Location = new Point(startX, startY) };
                AddControl(container, lbVariable);
                lbVariable.Text = string.Format("{0} more available but not shown", variablesList.Count - max_variables_count);
                CalculateContainerSize(lbVariable, ref thisWidth, ref startY);
            }

            // Download data label
            startY += 2;
            var lbDowloadData = new LinkLabel
                                    {
                                        AutoSize = true,
                                        Location = new Point(startX, startY),
                                        Tag = info,
                                    };
            lbDowloadData.LinkClicked += lblDownloadData_LinkClicked;
            AddControl(container, lbDowloadData);
            lbDowloadData.Text = info.GetItems().Any(item => item.IsDownloaded) ? "Download updated data" : "Download data";
            CalculateContainerSize(lbDowloadData, ref thisWidth, ref startY);
            lbDowloadData.Location = new Point(thisWidth - lbDowloadData.Width, lbDowloadData.Location.Y);

            container.Width = thisWidth + CONTROLS_START_X;
            container.Height = startY + CONSTROLS_START_Y;
        }

        private void CalculateContainerSize(Control child, ref int width, ref int height)
        {
            var needWidth = child.Location.X + child.Size.Width + HORISONTAL_PADDING;
            if (width < needWidth)
            {
                width = needWidth;
            }

            var needHeight = child.Location.Y + child.Size.Height + VERTICAL_PADDING;
            if (height < needHeight)
            {
                height = needHeight;
            }
        }

        private void AddControl(Control container, Control cntrl)
        {
            container.Controls.Add(cntrl);
        }

        #endregion

        #region Private methods

        void lblDownloadData_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            var lbl = sender as LinkLabel;
            if (lbl == null) return;
            var infoGroup = lbl.Tag as ServiceInfoGroup;
            if (infoGroup == null) return;

            if (Popup != null)
            {
                Popup.Close();
            }

            var seriesList = new List<OneSeriesDownloadInfo>(infoGroup.ItemsCount);
            seriesList.AddRange(infoGroup.GetItems().Select(ClassConvertor.ServiceInfoToOneSeriesDownloadInfo));
            var layer = infoGroup.GetItems().First().Layer; // we have at least one element

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
                if (IsValidServiceDesciptionUrl(target))
                {
                    Process.Start(target);
                }
            }
        }

        private bool IsValidServiceDesciptionUrl(string target)
        {
            return null != target && (target.StartsWith("http") || target.StartsWith("www"));
        }

        void CustomToolTipControl_SizeChanged(object sender, EventArgs e)
        {
            Region = new Region(graphicsPath = CreateRoundRectangle(Width - 1, Height - 1, 6));
        }

        void CustomToolTipControl_Load(object sender, EventArgs e)
        {
            BackColor = GetBackColor();
        }

        private static Color GetBackColor()
        {
            return SystemColors.Info;
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

        void tabControl_DrawItem(object sender, DrawItemEventArgs e)
        {
            var tabControl = sender as TabControl;
            if (tabControl == null) return;

            var tabPage = tabControl.TabPages[e.Index];
            var servInfo = (ServiceInfoGroup)tabPage.Controls[0].Tag;
            var isDowloaded = !servInfo.IsEmpty && servInfo.GetItems().Any(item => item.IsDownloaded);

            var font = isDowloaded? new Font(e.Font, FontStyle.Bold) : e.Font;
            var brush = isDowloaded ? Brushes.Green : Brushes.Black;
            e.Graphics.DrawString(tabControl.TabPages[e.Index].Text, font, brush, e.Bounds.Left + 2, e.Bounds.Top + 2);
        }

        void userControl_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.TranslateTransform(-1, -1);
            using (var p = GetBorderPen(((UserControl)sender).Tag as ServiceInfoGroup))
            {
                e.Graphics.DrawPath(p, graphicsPath);
            }
            e.Graphics.ResetTransform();
        }

        private Pen GetBorderPen(ServiceInfoGroup servInfo)
        {
            if (servInfo != null)
            {
                var isDowloaded = !servInfo.IsEmpty && servInfo.GetItems().Any(item => item.IsDownloaded);
                return isDowloaded
                           ? new Pen(Color.Green, 5)
                           : new Pen(SystemColors.WindowFrame, 2);
            }

            return new Pen(SystemColors.WindowFrame, 2);
        }

        #endregion
    }
}
