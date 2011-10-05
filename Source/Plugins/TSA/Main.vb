Imports System.Windows.Forms
Imports System.Drawing
Imports DotSpatial.Controls
Imports DotSpatial.Controls.RibbonControls
Imports HydroDesktop.Database
Imports HydroDesktop.Interfaces

Namespace TSA
    <Plugin("Graph", Author:="Utah State University", UniqueName:="ODM_Tools_Visualization", Version:="1.0")> _
    Public Class Main
        Inherits Extension
        Implements IMapPlugin

#Region "Variables"

        Private _mapArgs As IMapPluginArgs
        Private appSeriesView As ISeriesView

        '//the tab page which will be added to the tab control by the plugin
        Private _tabPage As TabPage = Nothing
        Private _mainControl As cTSA


        '//reference to the main application and it's UI items

        'Private _t As ITabManager
        Private Const _pluginName As String = "Graph"

        'Private _ribbonButton1 As RibbonButton
        Private tabGraph As RibbonTab


        Private rpPlots As New RibbonPanel("Plots")
        Private rbTSA As New RibbonButton("Time Series")
        Private rbProbability As New RibbonButton("Probability")
        Private rbHistogram As New RibbonButton("Histogram")
        Private rbBoxWhisker As New RibbonButton("Box/Whisker")
        Private rbSummary As New RibbonButton("Summary")

        Private rpTSAOption As New RibbonPanel("TSA & Probability Plot Options")
        Private rbTSPType As New RibbonButton("Plot Type")
        Private rbLine As New RibbonButton("Line")
        Private rbPoint As New RibbonButton("Point")
        Private rbBoth As New RibbonButton("Both")
        Private rbColorSetting As New RibbonButton("Color Setting")
        Private rbShowLegend As New RibbonButton("Close Legend")

        Private rpHistogramOption As New RibbonPanel("Histogram Plot Options")
        Private rbHistogramType As New RibbonButton("Histogram Type")
        Private rbhtCount As New RibbonButton("Count")
        Private rbhtProbability As New RibbonButton("Probability Density")
        Private rbhtRelative As New RibbonButton("Relative Frequencies")
        Private rbAlgorithms As New RibbonButton("Binning Algorithms")
        Private rbhaScott As New RibbonButton("Scott's")
        Private rbhaSturges As New RibbonButton("Sturges'")
        Private rbhaFreedman As New RibbonButton("Freedman-Diaconis’")

        Private rpBoxWhiskerOption As New RibbonPanel("Box Whisker Plot Option")
        Private rbBoxWhiskerType As New RibbonButton("Box Whisker Type")
        Private rbbtMonthly As New RibbonButton("Monthly")
        Private rbbtSeasonal As New RibbonButton("Seasonal")
        Private rbbtYearly As New RibbonButton("Yearly")
        Private rbbtOverall As New RibbonButton("Overall")

        Private rpOtherOptions As New RibbonPanel("Date & Time")
        Private rbDateTimeSetting As New RibbonButton("Date Setting")
        Public rlblStratDate As New RibbonLabel
        Public rlblEndDate As New RibbonLabel
        Private rckbDisplayFullDateRange As New RibbonCheckBox
        Private rtxttest As New RibbonComboBox


#End Region

#Region "IExtension Members"

        'Fires when the plugin should become inactive
        Protected Overrides Sub OnDeactivate()

        End Sub
#End Region

#Region "IMapPlugin Members"


        'When the plugin is initialized
        Public Sub Initialize(ByVal args As IMapPluginArgs) Implements IMapPlugin.Initialize

            'Assign the variable for accessing map and main application
            _mapArgs = args

            'To Add Items to the ribbon menu
            If Not _mapArgs.Ribbon Is Nothing Then
                'The main application has by default theree ribbon tabs: Home, Map View and Data.
                'Home is always the first ribbon Tab. 'Views' is always the first ribbon panel in
                'the Home ribbon tab.

                tabGraph = New RibbonTab(args.Ribbon, _pluginName)
                _mapArgs.Ribbon.Tabs.Add(tabGraph)
                AddHandler tabGraph.ActiveChanged, AddressOf ribbonButton1_Click

                'To add a ribbon panel with a dropDown and regular button
                'To add a new View to the main application window
                If Not _mapArgs.PanelManager Is Nothing Then

                    Dim manager As IHydroAppManager = TryCast(_mapArgs.AppManager, IHydroAppManager)
                    If Not manager Is Nothing Then
                        appSeriesView = manager.SeriesView
                        _mainControl = New cTSA(appSeriesView.SeriesSelector)
                        appSeriesView.AddPanel(_pluginName, _mainControl)
                    End If

                    'add the event when the main view is changed
                    AddHandler _mapArgs.PanelManager.SelectedIndexChanged, AddressOf panelManager_SelectedIndexChanged

                    InitializeRibbonButtons()

                End If
            Else

                '    //add some items to the newly created tab control
                If Not (_tabPage Is Nothing) Then
                    Dim plots As New cTSA(_mapArgs)
                    _tabPage.Controls.Add(plots)
                    plots.Dock = DockStyle.Fill
                End If
            End If




        End Sub

        Public Overloads Sub Activate() Implements IMapPlugin.Activate
            'MyBase.Activate()
            MyBase.OnActivate()
        End Sub

        'when the plug-in is deactivated
        Public Overloads Sub Deactivate() Implements IMapPlugin.Deactivate

            'If Not _mapArgs.Ribbon Is Nothing Then
            '_mapArgs.Ribbon.Tabs(0).Panels(0).Items.Remove(_ribbonButton1)
            '_mapArgs.Ribbon.Tabs.Remove(tabGraph)
            'If _t.Contains(_pluginName) Then
            '    _t.RemoveTab(_pluginName)
            'End If

            '_mapArgs.SeriesView.RemovePanel(_pluginName)

            ''important line to deactivate the plugin
            'MyBase.OnDeactivate()

            'Else
            'MyBase.Deactivate()
            'If Not _tabPage Is Nothing Then
            '    '_mainTabControl.TabPages.Remove(_tabPage)
            '    'RemoveHandler _mainTabControl.TabIndexChanged, AddressOf TabChanged
            'End If
            'End If

            If Not _mapArgs.Ribbon Is Nothing Then
                _mapArgs.PanelManager.RemoveTab(_pluginName)
                _mapArgs.Ribbon.Tabs.Remove(tabGraph)
                If Not appSeriesView Is Nothing Then
                    appSeriesView.RemovePanel(_pluginName)
                End If
                rpPlots.Items.Clear()
                rpTSAOption.Items.Clear()
                rpHistogramOption.Items.Clear()
                rpBoxWhiskerOption.Items.Clear()
                rpOtherOptions.Items.Clear()

                '_mapArgs.Ribbon.Tabs(0).Panels(0).Items.Remove(_ribbonButton1)

                'important line to deactivate the plugin
                MyBase.OnDeactivate()

            Else
                'MyBase.Deactivate()
                'If Not _tabPage Is Nothing Then
                '    _mainTabControl.TabPages.Remove(_tabPage)
                '    RemoveHandler _mainTabControl.TabIndexChanged, AddressOf TabChanged
                'End If
            End If

        End Sub

        Public Overloads Property IsEnabled() As Boolean Implements IMapPlugin.IsEnabled
            Get
                Return MyBase.IsEnabled
            End Get
            Set(ByVal value As Boolean)
                MyBase.IsEnabled = value
            End Set
        End Property

        Private Sub InitializeRibbonButtons()

            'Plot choosing Panel
            rpPlots.ButtonMoreVisible = False
            'rpPlots.ButtonMoreEnabled = False

            rbTSA.Image = My.Resources.TSA
            'rbTSA.CheckOnClick = True
            'rbTSA.Checked = True
            AddHandler rbTSA.Click, AddressOf rbTSA_Click
            rpPlots.Items.Add(rbTSA)

            rbProbability.Image = My.Resources.Probability
            'rbProbability.CheckOnClick = True
            AddHandler rbProbability.Click, AddressOf rbProbability_Click
            rpPlots.Items.Add(rbProbability)

            rbHistogram.Image = My.Resources.Histogram
            'rbHistogram.CheckOnClick = True
            AddHandler rbHistogram.Click, AddressOf rbHistogram_Click
            rpPlots.Items.Add(rbHistogram)

            rbBoxWhisker.Image = My.Resources.BoxWisker
            'rbBoxWhisker.CheckOnClick = True
            AddHandler rbBoxWhisker.Click, AddressOf rbBoxWhisker_Click
            rpPlots.Items.Add(rbBoxWhisker)

            rbSummary.Image = My.Resources.Summary
            'rbSummary.CheckOnClick = True
            AddHandler rbSummary.Click, AddressOf rbSummary_Click
            rpPlots.Items.Add(rbSummary)

            'Option Panel for TSA and Probability
            rpTSAOption.ButtonMoreVisible = False
            'rpTSAOption.ButtonMoreEnabled = False


            rbTSPType.Image = My.Resources.PlotType
            rbTSPType.Style = RibbonButtonStyle.DropDown
            rbTSPType.DropDownItems.Add(rbLine)
            rbTSPType.DropDownItems.Add(rbPoint)
            rbTSPType.DropDownItems.Add(rbBoth)
            AddHandler rbLine.Click, AddressOf rbLine_Click
            AddHandler rbPoint.Click, AddressOf rbPoint_Click
            AddHandler rbBoth.Click, AddressOf rbBoth_Click
            rpTSAOption.Items.Add(rbTSPType)

            rbColorSetting.Image = My.Resources.ColorSetting
            AddHandler rbColorSetting.Click, AddressOf rbColorSetting_Click
            rpTSAOption.Items.Add(rbColorSetting)

            rbShowLegend.Image = My.Resources.Legend
            AddHandler rbShowLegend.Click, AddressOf rbShowLegend_Click
            rpTSAOption.Items.Add(rbShowLegend)

            'Histogram Plot Option Panel
            rpHistogramOption.ButtonMoreVisible = False
            'rpHistogramOption.ButtonMoreEnabled = False
            'rpHistogramOption.Visible = False

            rbHistogramType.Image = My.Resources.HisType
            rbHistogramType.Style = RibbonButtonStyle.DropDown
            rbHistogramType.DropDownItems.Add(rbhtCount)
            rbHistogramType.DropDownItems.Add(rbhtProbability)
            rbHistogramType.DropDownItems.Add(rbhtRelative)
            AddHandler rbhtCount.Click, AddressOf rbhtCount_Click
            AddHandler rbhtProbability.Click, AddressOf rbhtProbability_Click
            AddHandler rbhtRelative.Click, AddressOf rbhtRelative_Click
            rpHistogramOption.Items.Add(rbHistogramType)

            rbAlgorithms.Image = My.Resources.Binning
            rbAlgorithms.Style = RibbonButtonStyle.DropDown
            rbAlgorithms.DropDownItems.Add(rbhaSturges)
            rbAlgorithms.DropDownItems.Add(rbhaScott)
            rbAlgorithms.DropDownItems.Add(rbhaFreedman)
            AddHandler rbhaSturges.Click, AddressOf rbhaSturges_Click
            AddHandler rbhaScott.Click, AddressOf rbhaScott_Click
            AddHandler rbhaFreedman.Click, AddressOf rbhaFreedman_Click
            rpHistogramOption.Items.Add(rbAlgorithms)

            'Box Whisker Plot Option Panel
            rpBoxWhiskerOption.ButtonMoreVisible = False
            'rpBoxWhiskerOption.ButtonMoreEnabled = False
            'rpBoxWhiskerOption.Visible = False

            rbBoxWhiskerType.Image = My.Resources.BoxWhiskerType
            rbBoxWhiskerType.Style = RibbonButtonStyle.DropDown
            rbBoxWhiskerType.DropDownItems.Add(rbbtMonthly)
            rbBoxWhiskerType.DropDownItems.Add(rbbtSeasonal)
            rbBoxWhiskerType.DropDownItems.Add(rbbtYearly)
            rbBoxWhiskerType.DropDownItems.Add(rbbtOverall)
            AddHandler rbbtMonthly.Click, AddressOf rbbtMonthly_Click
            AddHandler rbbtSeasonal.Click, AddressOf rbbtSeasonal_Click
            AddHandler rbbtYearly.Click, AddressOf rbbtYearly_Click
            AddHandler rbbtOverall.Click, AddressOf rbbtOverall_Click
            rpBoxWhiskerOption.Items.Add(rbBoxWhiskerType)

            'Others
            rpOtherOptions.ButtonMoreVisible = False
            'rpOtherOptions.ButtonMoreEnabled = False

            rbDateTimeSetting.Image = My.Resources.DateSetting
            AddHandler rbDateTimeSetting.Click, AddressOf rbDateTimeSetting_Click
            rpOtherOptions.Items.Add(rbDateTimeSetting)

            rlblStratDate.Text = "Start Date:"
            rlblEndDate.Text = "End Date:"
            rpOtherOptions.Items.Add(rlblStratDate)
            rpOtherOptions.Items.Add(rlblEndDate)
            _mainControl.rlblStratDate = rlblStratDate
            _mainControl.rlblEndDate = rlblEndDate

            rckbDisplayFullDateRange.Text = "Display Full Date Range"
            rckbDisplayFullDateRange.Checked = True
            AddHandler rckbDisplayFullDateRange.CheckBoxCheckChanged, AddressOf rckbDisplayFullDateRange_CheckBoxCheckChanged
            rpOtherOptions.Items.Add(rckbDisplayFullDateRange)


            tabGraph.Panels.Add(rpPlots)
            tabGraph.Panels.Add(rpTSAOption)
            tabGraph.Panels.Add(rpHistogramOption)
            tabGraph.Panels.Add(rpBoxWhiskerOption)
            tabGraph.Panels.Add(rpOtherOptions)


            rbTSA_Click()

        End Sub

#End Region

#Region "Event Handlers"

        Sub rbTSA_Click()
            UncheckOtherPlotButtons(rbTSA)
            _mainControl.TabControl2.SelectTab(0)
            rpTSAOption.Visible = True
            rpHistogramOption.Visible = False
            rpBoxWhiskerOption.Visible = False
        End Sub

        Sub rbProbability_Click()
            UncheckOtherPlotButtons(rbProbability)
            _mainControl.TabControl2.SelectTab(1)
            rpTSAOption.Visible = True
            rpHistogramOption.Visible = False
            rpBoxWhiskerOption.Visible = False
        End Sub

        Sub rbHistogram_Click()
            UncheckOtherPlotButtons(rbHistogram)
            _mainControl.TabControl2.SelectTab(2)
            rpTSAOption.Visible = False
            rpHistogramOption.Visible = True
            rpBoxWhiskerOption.Visible = False
        End Sub

        Sub rbBoxWhisker_Click()
            UncheckOtherPlotButtons(rbBoxWhisker)
            _mainControl.TabControl2.SelectTab(3)
            rpTSAOption.Visible = False
            rpHistogramOption.Visible = False
            rpBoxWhiskerOption.Visible = True
        End Sub

        Sub rbSummary_Click()
            UncheckOtherPlotButtons(rbSummary)
            _mainControl.TabControl2.SelectTab(4)
            rpTSAOption.Visible = False
            rpHistogramOption.Visible = False
            rpBoxWhiskerOption.Visible = False
        End Sub

        Sub rbLine_Click()
            _mainControl.CPlotOptions1.tsType = PlotOptions.TimeSeriesType.Line
            _mainControl.ApplyOptions()
        End Sub
        Sub rbPoint_Click()
            _mainControl.CPlotOptions1.tsType = PlotOptions.TimeSeriesType.Point
            _mainControl.ApplyOptions()
        End Sub
        Sub rbBoth_Click()
            _mainControl.CPlotOptions1.tsType = PlotOptions.TimeSeriesType.Both
            _mainControl.ApplyOptions()
        End Sub

        Sub rbShowLegend_Click()
            If rbShowLegend.Text = "Close Legend" Then
                _mainControl.CPlotOptions1.IsShowLegend = False
                _mainControl.ApplyOptions()
                rbShowLegend.Text = "Show Legend"
            Else
                _mainControl.CPlotOptions1.IsShowLegend = True
                _mainControl.ApplyOptions()
                rbShowLegend.Text = "Close Legend"
            End If
        End Sub

        Sub rbColorSetting_Click()
            Dim frmCC = New frmColorCollection(_mainControl.linecolorlist, _mainControl.pointcolorlist)
            frmCC._CTSA = _mainControl
            frmCC.ShowDialog()
        End Sub

        Sub rbhtCount_Click()
            _mainControl.CPlotOptions1.hpType = PlotOptions.HistogramType.Count
            _mainControl.ApplyOptions()
        End Sub
        Sub rbhtProbability_Click()
            _mainControl.CPlotOptions1.hpType = PlotOptions.HistogramType.Probability
            _mainControl.ApplyOptions()
        End Sub
        Sub rbhtRelative_Click()
            _mainControl.CPlotOptions1.hpType = PlotOptions.HistogramType.Relative
            _mainControl.ApplyOptions()
        End Sub

        Sub rbhaSturges_Click()
            _mainControl.CPlotOptions1.hpAlgo = PlotOptions.HistorgramAlgorithms.Sturges
            _mainControl.ApplyOptions()
        End Sub
        Sub rbhaScott_Click()
            _mainControl.CPlotOptions1.hpAlgo = PlotOptions.HistorgramAlgorithms.Scott
            _mainControl.ApplyOptions()
        End Sub
        Sub rbhaFreedman_Click()
            _mainControl.CPlotOptions1.hpAlgo = PlotOptions.HistorgramAlgorithms.Freedman
            _mainControl.ApplyOptions()
        End Sub

        Sub rbbtMonthly_Click()
            _mainControl.CPlotOptions1.bwType = PlotOptions.BoxWhiskerType.Monthly
            _mainControl.ApplyOptions()
        End Sub
        Sub rbbtSeasonal_Click()
            _mainControl.CPlotOptions1.bwType = PlotOptions.BoxWhiskerType.Seasonal
            _mainControl.ApplyOptions()
        End Sub
        Sub rbbtYearly_Click()
            _mainControl.CPlotOptions1.bwType = PlotOptions.BoxWhiskerType.Yearly
            _mainControl.ApplyOptions()
        End Sub
        Sub rbbtOverall_Click()
            _mainControl.CPlotOptions1.bwType = PlotOptions.BoxWhiskerType.Overall
            _mainControl.ApplyOptions()
        End Sub

        Sub rbDateTimeSetting_Click()

            'First make sure that the dates are within limits.  This will also ensure that a time series has been selected.
            If _mainControl.StartDateTime.CompareTo(_mainControl.StartDateLimit) < 0 Or _
                _mainControl.EndDateTime.CompareTo(_mainControl.EndDateLimit) > 0 Then

                MessageBox.Show("Please select a series first.")
                Return 'Leave without doing anything else.
            End If

            rckbDisplayFullDateRange.Checked = False
            _mainControl.IsDisplayFullDate = False
            Dim frmDateTimeSetting = New fDateTimeSetting
            frmDateTimeSetting._CTSA = _mainControl
            frmDateTimeSetting.initialize()

            'also changing dates in the ribbon
            If frmDateTimeSetting.ShowDialog() = DialogResult.OK Then
                rlblStratDate.Text = "Start Date: " & _mainControl.StartDateTime.ToString()
                rlblEndDate.Text = "End Date: " & _mainControl.EndDateTime.ToString()
            End If
        End Sub

        Sub rckbDisplayFullDateRange_CheckBoxCheckChanged()
            If rckbDisplayFullDateRange.Checked Then
                _mainControl.IsDisplayFullDate = True
                _mainControl.ApplyOptions()
            Else
                _mainControl.IsDisplayFullDate = False
            End If
        End Sub


        'when the 'VB.NET sample' button is clicked, select the RibbonSamplePlugin view
        Sub ribbonButton1_Click()

            'Set main view to 'SeriesView'
            _mapArgs.PanelManager.SelectedTabName = "Series View"

            'Set the seriesPanel to 'VB.NET sample plugin'
            If Not appSeriesView Is Nothing Then
                appSeriesView.VisiblePanelName = _pluginName
            End If
        End Sub

        'when the selected view is changed, refresh the series selector control by
        'calling the Initialize method of the main user control
        Sub panelManager_SelectedIndexChanged()
            '_mapArgs.PanelManager.SelectedTabName = "Series View"
            'If _t.SelectedTabName = _pluginName Then
            '    _mainControl.initialize()
            '    _mainControl.RefreshView()

            'End If
            '_mapArgs.SeriesView.VisiblePanelName = _pluginName
        End Sub

        Public Sub TabChanged(ByVal sender As Object, ByVal e As EventArgs)
            'If _mainTabControl Is Nothing Then Exit Sub
            If _tabPage Is Nothing Then Exit Sub
        End Sub

        Sub UncheckOtherPlotButtons(ByRef PlotButton As RibbonItem)
            For Each rbi As RibbonItem In rpPlots.Items
                If TypeOf rbi Is RibbonButton And Not rbi.Equals(PlotButton) Then
                    rbi.Checked = False
                End If
            Next
        End Sub

#End Region




    End Class
End Namespace
