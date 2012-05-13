Imports System.Windows.Forms
Imports System.Drawing
Imports System.ComponentModel
Imports HydroDesktop.Common
Imports Controls
Imports DotSpatial.Controls
Imports System.Globalization
Imports HydroDesktop.Database
Imports HydroDesktop.Interfaces
Imports DotSpatial.Controls.Header
Imports System.ComponentModel.Composition
Imports DotSpatial.Controls.Docking


Namespace GraphView
    Public Class Main
        Inherits Extension

#Region "Variables"

        Private Const kGraph As String = "kHydroGraph_01"

        <Import("SeriesControl", GetType(ISeriesSelector))>
        Private appSeriesView As ISeriesSelector

        Private _mainControl As MainControl

        Private firstTimeLoaded As Boolean = True

        'reference to the main application and it's UI items
        Private Const _pluginName As String = "Graph"
        Private Const kSeriesViewPanelName As String = "Series View"

        Private tabGraph As RootItem

        Private Const rpPlots As String = "Plots"
        Private Const kTogglePlots As String = "kHydroPlotsGroup"

        Private rbTSA As SimpleActionItem 'Time Series
        Private rbProbability As SimpleActionItem 'Probability
        Private rbHistogram As SimpleActionItem 'Histogram
        Private rbBoxWhisker As SimpleActionItem 'Box/Whisker
        Private rbSummary As SimpleActionItem 'Summary

        Private Const rpPlotOption As String = "TSA & Probability Plot Options"
        Const PlotOptionsMenuKey = "kHydroPlotOptions"
        Private rbPlotType As MenuContainerItem 'Plot Type
        Private rbLine As SimpleActionItem 'Line
        Private rbPoint As SimpleActionItem 'Point
        Private rbBoth As SimpleActionItem 'Both
        Private rbColorSetting As SimpleActionItem 'Color Setting
        Private rbShowLegend As SimpleActionItem 'Close Legend

        Private Const rpHistogramOption As String = "Histogram Plot Options"
        Const kHistogramType = "kHistogramType"
        Private rbHistogramType As MenuContainerItem 'Histogram Type
        Private rbhtCount As SimpleActionItem 'Count
        Private rbhtProbability As SimpleActionItem 'Probability Density
        Private rbhtRelative As SimpleActionItem 'Relative Frequencies
        Const kHistogramAlgorithm = "kHistogramAlgorithm"
        Private rbAlgorithms As MenuContainerItem 'Binning Algorithms
        Private rbhaScott As SimpleActionItem 'Scott's
        Private rbhaSturges As SimpleActionItem 'Sturges'
        Private rbhaFreedman As SimpleActionItem 'Freedman-Diaconis’

        Private Const rpBoxWhiskerOption As String = "Box Whisker Plot Option"
        Const kBoxWhiskerType = "kBoxWhiskerType"
        Private rbBoxWhiskerType As MenuContainerItem 'Box Whisker Type
        Private rbbtMonthly As SimpleActionItem 'Monthly
        Private rbbtSeasonal As SimpleActionItem 'Seasonal
        Private rbbtYearly As SimpleActionItem 'Yearly
        Private rbbtOverall As SimpleActionItem 'Overall

        Private ReadOnly _datesFormat = CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern
        Private Const rpOtherOptions As String = "Date & Time"
        Private rbDateTimeSetting As SimpleActionItem 'Date Setting
        Private rbStartDate As TextEntryActionItem 'Date Setting
        Private rbEndDate As TextEntryActionItem 'Date Setting
        Private rbApplyDateSettings As SimpleActionItem
        Private rbDisplayFullDateRange As SimpleActionItem 'Display Full Date Range Toggle button
        Private boolFullDateRange As Boolean = True 'Display Full Date Range boolean indicator

        Private Const rpChart As String = "Chart"
        Private rbShowPointValues As SimpleActionItem 'Show Point Values Toggle button
        Private _showPointValues As Boolean
        Private rbZoomIn As SimpleActionItem 'Zoom In Toggle button
        Private rbZoomOut As SimpleActionItem 'Zoom Out Toggle button
        Private rbUndoZoom As SimpleActionItem 'Undo zoom Toggle button

#End Region


#Region "IExtension Members"

        'When the plugin is initialized
        Public Overrides Sub Activate()

            'watch for dock panel added event
            'If firstTimeLoaded Then
            '    AddHandler App.DockManager.PanelAdded, AddressOf DockPanelAdded
            'End If


            _mainControl = New MainControl(appSeriesView)
            _mainControl.Dock = DockStyle.Fill

            InitializeRibbonButtons()

            'If Not firstTimeLoaded Then
            Dim dp As New DockablePanel(kGraph, _pluginName, _mainControl, DockStyle.Fill)
            dp.DefaultSortOrder = 20
            App.DockManager.Add(dp)
            'End If


            AddHandler App.HeaderControl.RootItemSelected, AddressOf HeaderControl_RootItemSelected

            'when the graph dock panel is activated:
            'show graph ribbon tab and series view
            AddHandler App.DockManager.ActivePanelChanged, AddressOf DockManager_ActivePanelChanged

            Common.PluginEntryPoint = Me

            MyBase.Activate()
        End Sub

        'when the plug-in is deactivated
        Public Overrides Sub Deactivate()

            'auto-remove all ribbon items
            App.HeaderControl.RemoveAll()

            'remove the dock panel
            App.DockManager.Remove(kGraph)

            _mainControl = Nothing

            RemoveHandler App.DockManager.ActivePanelChanged, AddressOf DockManager_ActivePanelChanged
            RemoveHandler App.HeaderControl.RootItemSelected, AddressOf HeaderControl_RootItemSelected

            Common.PluginEntryPoint = Nothing

            'important line to deactivate the plugin
            MyBase.Deactivate()

        End Sub

        Sub HeaderControl_RootItemSelected(ByVal sender As Object, ByVal e As RootItemEventArgs)
            If e.SelectedRootKey = kGraph Then
                App.DockManager.SelectPanel(kGraph)
            End If
        End Sub

        Sub DockPanelAdded(ByVal sender As Object, ByVal args As Docking.DockablePanelEventArgs)

            If Not firstTimeLoaded Then Return

            If args.ActivePanelKey = "kMap" Then
                App.DockManager.Add(New DockablePanel(kGraph, _pluginName, _mainControl, DockStyle.Fill))
                firstTimeLoaded = False
            End If
        End Sub

        Private Sub InitializeRibbonButtons()

            Dim header As HeaderControl = App.HeaderControl

            'To Add Items to the ribbon menu
            tabGraph = New RootItem(kGraph, _pluginName)
            tabGraph.SortOrder = 30
            App.HeaderControl.Add(tabGraph)

            'Plot choosing Panel
            'Time Series Plot
            rbTSA = New SimpleActionItem("TimeSeries", AddressOf rbTSA_Click)
            rbTSA.RootKey = kGraph
            rbTSA.LargeImage = My.Resources.TSA
            rbTSA.GroupCaption = rpPlots
            rbTSA.ToggleGroupKey = kTogglePlots
            App.HeaderControl.Add(rbTSA)

            'Probability Plot
            rbProbability = New SimpleActionItem("Probability", AddressOf rbProbability_Click)
            rbProbability.RootKey = kGraph
            rbProbability.LargeImage = My.Resources.Probability
            rbProbability.GroupCaption = rpPlots
            rbProbability.ToggleGroupKey = kTogglePlots
            App.HeaderControl.Add(rbProbability)

            'Histogram Plot
            rbHistogram = New SimpleActionItem("Histogram", AddressOf rbHistogram_Click)
            rbHistogram.RootKey = kGraph
            rbHistogram.LargeImage = My.Resources.Histogram
            rbHistogram.GroupCaption = rpPlots
            rbHistogram.ToggleGroupKey = kTogglePlots
            App.HeaderControl.Add(rbHistogram)

            'Box/Whisker Plot
            rbBoxWhisker = New SimpleActionItem("Box/Whisker", AddressOf rbBoxWhisker_Click)
            rbBoxWhisker.RootKey = kGraph
            rbBoxWhisker.LargeImage = My.Resources.BoxWisker
            rbBoxWhisker.GroupCaption = rpPlots
            rbBoxWhisker.ToggleGroupKey = kTogglePlots
            App.HeaderControl.Add(rbBoxWhisker)

            'Summary Plot
            rbSummary = New SimpleActionItem("Summary", AddressOf rbSummary_Click)
            rbSummary.RootKey = kGraph
            rbSummary.LargeImage = My.Resources.Summary
            rbSummary.GroupCaption = rpPlots
            rbSummary.ToggleGroupKey = kTogglePlots
            App.HeaderControl.Add(rbSummary)

            'Option Panel for TSA and Probability
            rbPlotType = New MenuContainerItem(kGraph, PlotOptionsMenuKey, "Plot Type")
            rbPlotType.LargeImage = My.Resources.PlotType
            rbPlotType.GroupCaption = rpPlotOption
            App.HeaderControl.Add(rbPlotType)

            'Line
            rbLine = New SimpleActionItem(kGraph, PlotOptionsMenuKey, "Line", AddressOf rbLine_Click)
            rbLine.GroupCaption = rpPlotOption
            App.HeaderControl.Add(rbLine)
            'Point
            rbPoint = New SimpleActionItem(kGraph, PlotOptionsMenuKey, "Point", AddressOf rbPoint_Click)
            rbPoint.GroupCaption = rpPlotOption
            App.HeaderControl.Add(rbPoint)
            'Both
            rbBoth = New SimpleActionItem(kGraph, PlotOptionsMenuKey, "Both", AddressOf rbBoth_Click)
            rbBoth.GroupCaption = rpPlotOption
            App.HeaderControl.Add(rbBoth)

            'Color Setting
            rbColorSetting = New SimpleActionItem("Color Setting", AddressOf rbColorSetting_Click)
            rbColorSetting.RootKey = kGraph
            rbColorSetting.LargeImage = My.Resources.ColorSetting
            rbColorSetting.GroupCaption = rpPlotOption
            App.HeaderControl.Add(rbColorSetting)

            'Show Legend
            rbShowLegend = New SimpleActionItem("Show Legend", AddressOf rbShowLegend_Click)
            rbShowLegend.RootKey = kGraph
            rbShowLegend.LargeImage = My.Resources.Legend
            rbShowLegend.GroupCaption = rpPlotOption
            App.HeaderControl.Add(rbShowLegend)

            'Histogram Plot Option Panel
            'Histogram Type Menu
            rbHistogramType = New MenuContainerItem(kGraph, kHistogramType, "Histogram Type")
            rbHistogramType.LargeImage = My.Resources.HisType
            rbHistogramType.GroupCaption = rpHistogramOption
            App.HeaderControl.Add(rbHistogramType)

            'Count
            rbhtCount = New SimpleActionItem(kGraph, kHistogramType, "Count", AddressOf rbhtCount_Click)
            rbhtCount.GroupCaption = rpHistogramOption
            App.HeaderControl.Add(rbhtCount)
            'Probability Density
            rbhtProbability = New SimpleActionItem(kGraph, kHistogramType, "Probability Density", AddressOf rbhtProbability_Click)
            rbhtProbability.GroupCaption = rpHistogramOption
            App.HeaderControl.Add(rbhtProbability)
            'Relative Frequencies
            rbhtRelative = New SimpleActionItem(kGraph, kHistogramType, "Relative Frequencies", AddressOf rbhtRelative_Click)
            rbhtRelative.GroupCaption = rpHistogramOption
            App.HeaderControl.Add(rbhtRelative)
            rbHistogramType.Visible = False

            'Histogram Algorithm Menu
            rbAlgorithms = New MenuContainerItem(kGraph, kHistogramAlgorithm, "Binning Algorithms")
            rbAlgorithms.LargeImage = My.Resources.Binning
            rbAlgorithms.GroupCaption = rpHistogramOption
            App.HeaderControl.Add(rbAlgorithms)

            'Scott's
            rbhaScott = New SimpleActionItem(kGraph, kHistogramAlgorithm, "Scott's", AddressOf rbhaScott_Click)
            rbhaScott.GroupCaption = rpHistogramOption
            App.HeaderControl.Add(rbhaScott)
            'Sturges
            rbhaSturges = New SimpleActionItem(kGraph, kHistogramAlgorithm, "Sturges", AddressOf rbhaSturges_Click)
            rbhaSturges.GroupCaption = rpHistogramOption
            App.HeaderControl.Add(rbhaSturges)
            'Freedman-Diaconis
            rbhaFreedman = New SimpleActionItem(kGraph, kHistogramAlgorithm, "Freedman-Diaconis", AddressOf rbhaFreedman_Click)
            rbhaFreedman.GroupCaption = rpHistogramOption
            App.HeaderControl.Add(rbhaFreedman)
            rbAlgorithms.Visible = False

            'Box Whisker Plot Option Panel
            rbBoxWhiskerType = New MenuContainerItem(kGraph, kBoxWhiskerType, "Box Whisker Type")
            rbBoxWhiskerType.LargeImage = My.Resources.BoxWhiskerType
            rbBoxWhiskerType.GroupCaption = rpBoxWhiskerOption
            App.HeaderControl.Add(rbBoxWhiskerType)
            rbBoxWhiskerType.Visible = False
            'Monthly
            rbbtMonthly = New SimpleActionItem(kGraph, kBoxWhiskerType, "Monthly", AddressOf rbbtMonthly_Click)
            rbbtMonthly.GroupCaption = rpBoxWhiskerOption
            App.HeaderControl.Add(rbbtMonthly)
            rbbtMonthly.Visible = False
            'Seasonal
            rbbtSeasonal = New SimpleActionItem(kGraph, kBoxWhiskerType, "Seasonal", AddressOf rbbtSeasonal_Click)
            rbbtSeasonal.GroupCaption = rpBoxWhiskerOption
            App.HeaderControl.Add(rbbtSeasonal)
            rbbtSeasonal.Visible = False
            'Yearly
            rbbtYearly = New SimpleActionItem(kGraph, kBoxWhiskerType, "Yearly", AddressOf rbbtYearly_Click)
            rbbtYearly.GroupCaption = rpBoxWhiskerOption
            App.HeaderControl.Add(rbbtYearly)
            rbbtYearly.Visible = False
            'Overall
            rbbtOverall = New SimpleActionItem(kGraph, kBoxWhiskerType, "Overall", AddressOf rbbtOverall_Click)
            rbbtOverall.GroupCaption = rpBoxWhiskerOption
            App.HeaderControl.Add(rbbtOverall)
            rbbtOverall.Visible = False

            'Others
            'Date Setting
            rbStartDate = New TextEntryActionItem()
            rbStartDate.Caption = "Start"
            rbStartDate.GroupCaption = rpOtherOptions
            rbStartDate.RootKey = kGraph
            rbStartDate.Width = 60
            AddHandler rbStartDate.PropertyChanged, AddressOf dateSettings_PropertyChanged
            App.HeaderControl.Add(rbStartDate)

            rbEndDate = New TextEntryActionItem()
            rbEndDate.Caption = " End"
            rbEndDate.GroupCaption = rpOtherOptions
            rbEndDate.RootKey = kGraph
            rbEndDate.Width = 60
            AddHandler rbEndDate.PropertyChanged, AddressOf dateSettings_PropertyChanged
            App.HeaderControl.Add(rbEndDate)

            AddHandler _mainControl.DatesChanged, AddressOf mainControlDatesChanged

            rbApplyDateSettings = New SimpleActionItem("Refresh", AddressOf rbDateTimeRefresh_Click)
            rbApplyDateSettings.RootKey = kGraph
            rbApplyDateSettings.LargeImage = My.Resources.DateSetting
            rbApplyDateSettings.GroupCaption = rpOtherOptions
            App.HeaderControl.Add(rbApplyDateSettings)

            rbDisplayFullDateRange = New SimpleActionItem("Full Date Range", AddressOf rbDisplayFullDateRange_Click)
            rbDisplayFullDateRange.RootKey = kGraph
            rbDisplayFullDateRange.LargeImage = My.Resources.FullDateRange
            rbDisplayFullDateRange.SmallImage = My.Resources.FullDateRange_16
            rbDisplayFullDateRange.GroupCaption = rpOtherOptions
            rbDisplayFullDateRange.Enabled = False
            App.HeaderControl.Add(rbDisplayFullDateRange)

            rbDateTimeSetting = New SimpleActionItem("Date Setting", AddressOf rbDateTimeSetting_Click)
            rbDateTimeSetting.RootKey = kGraph
            rbDateTimeSetting.LargeImage = My.Resources.DateSetting
            rbDateTimeSetting.GroupCaption = rpOtherOptions
            App.HeaderControl.Add(rbDateTimeSetting)
            rbDateTimeSetting.Visible = False

            'Chart
            'Show Point Values
            rbShowPointValues = New SimpleActionItem("Show Point Values", AddressOf rbShowPointValues_Click)
            rbShowPointValues.RootKey = kGraph
            rbShowPointValues.GroupCaption = rpChart
            App.HeaderControl.Add(rbShowPointValues)
            rbShowPointValues.Visible = False
            rbShowPointValues_Click()

            'Zoom In
            rbZoomIn = New SimpleActionItem("Zoom In", AddressOf rbZoomIn_Click)
            rbZoomIn.RootKey = kGraph
            rbZoomIn.GroupCaption = rpChart
            rbZoomIn.Visible = False
            App.HeaderControl.Add(rbZoomIn)

            ' Zoom Out
            rbZoomOut = New SimpleActionItem("Zoom Out", AddressOf rbZoomOut_Click)
            rbZoomOut.RootKey = kGraph
            rbZoomOut.GroupCaption = rpChart
            rbZoomOut.Visible = False
            App.HeaderControl.Add(rbZoomOut)

            ' Undo Zoom
            rbUndoZoom = New SimpleActionItem("Undo Zoom", AddressOf rbUndoZoom_Click)
            rbUndoZoom.RootKey = kGraph
            rbUndoZoom.GroupCaption = rpChart
            rbUndoZoom.Visible = False
            App.HeaderControl.Add(rbUndoZoom)

            'The button should initially be checked
            rbTSA_Click()
            rbTSA.Toggle()

        End Sub

        Private Sub dateSettings_PropertyChanged(ByVal sender As Object, ByVal e As PropertyChangedEventArgs)
            If "Text".Equals(e.PropertyName) Then

                Dim startDate = ValidateDateEdit(rbStartDate, "Start Date", _datesFormat, False)
                If (startDate Is Nothing) Then Return
                Dim endDate = ValidateDateEdit(rbEndDate, "End Date", _datesFormat, False)
                If (endDate Is Nothing) Then Return

                If _mainControl.StartDateLimit.Date >= startDate And
                       _mainControl.EndDateLimit.Date <= endDate Then
                    rbDisplayFullDateRange.Enabled = False
                Else
                    rbDisplayFullDateRange.Enabled = True
                End If

            End If
        End Sub

        Private Sub rbDateTimeRefresh_Click(ByVal sender As Object, ByVal e As EventArgs)
            ' Validation of Start/End date
            Dim startDate = ValidateDateEdit(rbStartDate, "Start Date", _datesFormat, True)
            If (startDate Is Nothing) Then Return
            Dim endDate = ValidateDateEdit(rbEndDate, "End Date", _datesFormat, True)
            If (endDate Is Nothing) Then Return
            ' end of validation

            _mainControl.StartDateTime = startDate
            _mainControl.EndDateTime = endDate
            _mainControl.IsDisplayFullDate = False
            _mainControl.ApplyOptions()
        End Sub

        Private Function ValidateDate(str As String, dateFormat As String) As DateTime?
            Try
                Return DateTime.ParseExact(str, dateFormat, CultureInfo.CurrentCulture)
            Catch ex As Exception
                Return Nothing
            End Try
        End Function

        Private Function ValidateDateEdit(item As TextEntryActionItem, itemName As String, dateFormat As String, showMessage As Boolean) As DateTime?
            Dim result As DateTime?
            result = ValidateDate(item.Text, dateFormat)
            If (result Is Nothing And showMessage) Then
                MessageBox.Show(String.Format("{0} is in incorrect format. Please enter {1} in the format {2}", itemName, itemName.ToLower(), dateFormat),
                                String.Format("{0} validation", itemName), MessageBoxButtons.OK, MessageBoxIcon.Information)
            End If
            Return result
        End Function


        Private Sub mainControlDatesChanged(ByVal sender As Object, ByVal e As EventArgs)
            rbStartDate.Text = _mainControl.StartDateTime.ToString(_datesFormat)
            rbEndDate.Text = _mainControl.EndDateTime.ToString(_datesFormat)
        End Sub

#End Region

#Region "Event Handlers"
        'Click Time Series
        Sub rbTSA_Click()
            'UncheckOtherPlotButtons(rbTSA)
            _mainControl.TabControl2.SelectTab(0)
            rbPlotType.Visible = True
            rbColorSetting.Visible = True
            rbShowLegend.Visible = True

            rbHistogramType.Visible = False
            rbAlgorithms.Visible = False
            rbBoxWhiskerType.Visible = False

        End Sub

        Sub rbProbability_Click()
            'UncheckOtherPlotButtons(rbProbability)
            _mainControl.TabControl2.SelectTab(1)
            rbPlotType.Visible = True
            rbColorSetting.Visible = True
            rbShowLegend.Visible = True

            rbHistogramType.Visible = False
            rbAlgorithms.Visible = False
            rbBoxWhiskerType.Visible = False
        End Sub

        Sub rbHistogram_Click()
            'UncheckOtherPlotButtons(rbHistogram)
            _mainControl.TabControl2.SelectTab(2)
            rbPlotType.Visible = False
            rbColorSetting.Visible = False
            rbShowLegend.Visible = False

            rbHistogramType.Visible = True
            rbAlgorithms.Visible = True
            rbBoxWhiskerType.Visible = False
        End Sub

        Sub rbBoxWhisker_Click()
            'UncheckOtherPlotButtons(rbBoxWhisker)
            _mainControl.TabControl2.SelectTab(3)
            rbPlotType.Visible = False
            rbColorSetting.Visible = False
            rbShowLegend.Visible = False

            rbHistogramType.Visible = False
            rbAlgorithms.Visible = False
            rbBoxWhiskerType.Visible = True
            'rpPlotOption.Visible = False
            'rpHistogramOption.Visible = False
            'rpBoxWhiskerOption.Visible = True
        End Sub

        Sub rbSummary_Click()
            'UncheckOtherPlotButtons(rbSummary)
            _mainControl.TabControl2.SelectTab(4)
            rbPlotType.Visible = False
            rbColorSetting.Visible = False
            rbShowLegend.Visible = False

            rbHistogramType.Visible = False
            rbAlgorithms.Visible = False
            rbBoxWhiskerType.Visible = False
            'rpPlotOption.Visible = False
            'rpHistogramOption.Visible = False
            'rpBoxWhiskerOption.Visible = False
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
            Dim text = If(_mainControl.CPlotOptions1.IsShowLegend, "Show Legend", "Close Legend")

            _mainControl.CPlotOptions1.IsShowLegend = Not _mainControl.CPlotOptions1.IsShowLegend
            _mainControl.ApplyOptions()
            rbShowLegend.Caption = text
        End Sub

        Sub rbColorSetting_Click()
            Dim frmCC = New ColorSettingsDialog(_mainControl.linecolorlist, _mainControl.pointcolorlist)
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

            'rckbDisplayFullDateRange.Checked = False
            _mainControl.IsDisplayFullDate = False
            Dim frmDateTimeSetting = New DateTimeSettingsDialog(_mainControl)
            frmDateTimeSetting.ShowDialog()
        End Sub

        'Display full date range toggle button is clicked
        Private Sub rbDisplayFullDateRange_Click()
            If _mainControl.IsDisplayFullDate Then
                _mainControl.IsDisplayFullDate = False
            Else
                _mainControl.IsDisplayFullDate = True
                _mainControl.ApplyOptions()
            End If
        End Sub

        'Show Point Values 
        Private Sub rbShowPointValues_Click()
            _mainControl.ShowPointValues(_showPointValues)
            If _showPointValues Then
                rbShowPointValues.Caption = "Show Point Values - On"
            Else
                rbShowPointValues.Caption = "Show Point Values - Off"
            End If
            _showPointValues = Not _showPointValues
        End Sub

        Private Sub rbUndoZoom_Click()
            _mainControl.UndoZoom()
        End Sub

        Private Sub rbZoomIn_Click()
            _mainControl.ZoomIn()
        End Sub

        Private Sub rbZoomOut_Click()
            _mainControl.ZoomOut()
        End Sub


        'this is replaced by toggle buttons
        'Sub UncheckOtherPlotButtons(ByRef PlotButton As RibbonItem)
        '    For Each rbi As RibbonItem In rpPlots.Items
        '        If TypeOf rbi Is RibbonButton And Not rbi.Equals(PlotButton) Then
        '            rbi.Checked = False
        '        End If
        '    Next
        'End Sub

        Sub DockManager_ActivePanelChanged(ByVal sender As Object, ByVal e As Docking.DockablePanelEventArgs)

            'activate the graph ribbon tab and the series view panel
            If e.ActivePanelKey = kGraph Then
                App.HeaderControl.SelectRoot(kGraph)
                App.DockManager.SelectPanel( SharedConstants.SeriesViewKey)
            End If

        End Sub

#End Region

    End Class
End Namespace
