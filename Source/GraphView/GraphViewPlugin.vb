Option Strict On

Imports System.Windows.Forms
Imports System.ComponentModel
Imports GraphView.Controls
Imports HydroDesktop.Common
Imports DotSpatial.Controls
Imports System.Globalization
Imports HydroDesktop.Interfaces
Imports DotSpatial.Controls.Header
Imports System.ComponentModel.Composition
Imports DotSpatial.Controls.Docking

Public Class GraphViewPlugin
    Inherits Extension

#Region "Fields"

    Private Const kGraph As String = "kHydroGraph_01"

    <Import("SeriesControl", GetType(ISeriesSelector))>
    Private appSeriesView As ISeriesSelector

    Private _mainControl As MainControl

    Private firstTimeLoaded As Boolean = True

    'reference to the main application and it's UI items
    Private Const _pluginName As String = "Graph"
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

    Private ReadOnly _datesFormat As String = CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern
    Private Const rpOtherOptions As String = "Date & Time"
    Private rbStartDate As TextEntryActionItem 'Date Setting
    Private rbEndDate As TextEntryActionItem 'Date Setting
    Private rbApplyDateSettings As SimpleActionItem
    Private rbDisplayFullDateRange As SimpleActionItem 'Display Full Date Range Toggle button

    Private Const rpChart As String = "Chart"
    Private rbShowPointValues As SimpleActionItem 'Show Point Values Toggle button
    Private _showPointValues As Boolean
    Private rbZoomIn As SimpleActionItem 'Zoom In Toggle button
    Private rbZoomOut As SimpleActionItem 'Zoom Out Toggle button
    Private rbUndoZoom As SimpleActionItem

    Private ReadOnly _plotOptions As PlotOptions = New PlotOptions()

    Public ReadOnly Property PlotOptions() As PlotOptions
        Get
            Return _plotOptions
        End Get
    End Property

    Public ReadOnly Property SeriesSelector() As ISeriesSelector
        Get
            Return appSeriesView
        End Get
    End Property

    'Undo zoom Toggle button

#End Region

#Region "IExtension Members"

    'When the plugin is initialized
    Public Overrides Sub Activate()
        _mainControl = New MainControl(Me)
        _mainControl.Dock = DockStyle.Fill

        InitializeRibbonButtons()

        Dim dp As New DockablePanel(kGraph, _pluginName, _mainControl, DockStyle.Fill)
        dp.DefaultSortOrder = 20
        App.DockManager.Add(dp)

        AddHandler App.HeaderControl.RootItemSelected, AddressOf HeaderControl_RootItemSelected
        AddHandler App.DockManager.ActivePanelChanged, AddressOf DockManager_ActivePanelChanged

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

        'important line to deactivate the plugin
        MyBase.Deactivate()

    End Sub

    Sub HeaderControl_RootItemSelected(ByVal sender As Object, ByVal e As RootItemEventArgs)
        If e.SelectedRootKey = kGraph Then
            App.DockManager.SelectPanel(kGraph)
        End If
    End Sub

    Sub DockPanelAdded(ByVal sender As Object, ByVal args As DockablePanelEventArgs)

        If Not firstTimeLoaded Then Return

        If args.ActivePanelKey = "kMap" Then
            App.DockManager.Add(New DockablePanel(kGraph, _pluginName, _mainControl, DockStyle.Fill))
            firstTimeLoaded = False
        End If
    End Sub

    Private Sub InitializeRibbonButtons()

        Dim header = App.HeaderControl

        'To Add Items to the ribbon menu
        tabGraph = New RootItem(kGraph, _pluginName)
        tabGraph.SortOrder = 30
        header.Add(tabGraph)

        'Plot choosing Panel
        'Time Series Plot
        rbTSA = New SimpleActionItem("TimeSeries", AddressOf rbTSA_Click)
        rbTSA.RootKey = kGraph
        rbTSA.LargeImage = My.Resources.TSA
        rbTSA.GroupCaption = rpPlots
        rbTSA.ToggleGroupKey = kTogglePlots
        header.Add(rbTSA)

        'Probability Plot
        rbProbability = New SimpleActionItem("Probability", AddressOf rbProbability_Click)
        rbProbability.RootKey = kGraph
        rbProbability.LargeImage = My.Resources.Probability
        rbProbability.GroupCaption = rpPlots
        rbProbability.ToggleGroupKey = kTogglePlots
        header.Add(rbProbability)

        'Histogram Plot
        rbHistogram = New SimpleActionItem("Histogram", AddressOf rbHistogram_Click)
        rbHistogram.RootKey = kGraph
        rbHistogram.LargeImage = My.Resources.Histogram
        rbHistogram.GroupCaption = rpPlots
        rbHistogram.ToggleGroupKey = kTogglePlots
        header.Add(rbHistogram)

        'Box/Whisker Plot
        rbBoxWhisker = New SimpleActionItem("Box/Whisker", AddressOf rbBoxWhisker_Click)
        rbBoxWhisker.RootKey = kGraph
        rbBoxWhisker.LargeImage = My.Resources.BoxWisker
        rbBoxWhisker.GroupCaption = rpPlots
        rbBoxWhisker.ToggleGroupKey = kTogglePlots
        header.Add(rbBoxWhisker)

        'Summary Plot
        rbSummary = New SimpleActionItem("Summary", AddressOf rbSummary_Click)
        rbSummary.RootKey = kGraph
        rbSummary.LargeImage = My.Resources.Summary
        rbSummary.GroupCaption = rpPlots
        rbSummary.ToggleGroupKey = kTogglePlots
        header.Add(rbSummary)

        'Option Panel for TSA and Probability
        rbPlotType = New MenuContainerItem(kGraph, PlotOptionsMenuKey, "Plot Type")
        rbPlotType.LargeImage = My.Resources.PlotType
        rbPlotType.GroupCaption = rpPlotOption
        header.Add(rbPlotType)

        'Line
        rbLine = New SimpleActionItem(kGraph, PlotOptionsMenuKey, "Line", Sub()
                                                                              _plotOptions.TimeSeriesMethod = TimeSeriesType.Line
                                                                              _mainControl.ApplyOptions()
                                                                          End Sub)
        rbLine.GroupCaption = rpPlotOption
        header.Add(rbLine)
        'Point
        rbPoint = New SimpleActionItem(kGraph, PlotOptionsMenuKey, "Point", Sub()
                                                                                _plotOptions.TimeSeriesMethod = TimeSeriesType.Point
                                                                                _mainControl.ApplyOptions()
                                                                            End Sub)
        rbPoint.GroupCaption = rpPlotOption
        header.Add(rbPoint)
        'Both
        rbBoth = New SimpleActionItem(kGraph, PlotOptionsMenuKey, "Both", Sub()
                                                                              _plotOptions.TimeSeriesMethod = TimeSeriesType.Both
                                                                              _mainControl.ApplyOptions()
                                                                          End Sub)
        rbBoth.GroupCaption = rpPlotOption
        header.Add(rbBoth)

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
        header.Add(rbShowLegend)

        'Histogram Plot Option Panel
        'Histogram Type Menu
        rbHistogramType = New MenuContainerItem(kGraph, kHistogramType, "Histogram Type")
        rbHistogramType.LargeImage = My.Resources.HisType
        rbHistogramType.GroupCaption = rpHistogramOption
        header.Add(rbHistogramType)

        'Count
        rbhtCount = New SimpleActionItem(kGraph, kHistogramType, "Count", AddressOf rbhtCount_Click)
        rbhtCount.GroupCaption = rpHistogramOption
        header.Add(rbhtCount)
        'Probability Density
        rbhtProbability = New SimpleActionItem(kGraph, kHistogramType, "Probability Density", AddressOf rbhtProbability_Click)
        rbhtProbability.GroupCaption = rpHistogramOption
        header.Add(rbhtProbability)
        'Relative Frequencies
        rbhtRelative = New SimpleActionItem(kGraph, kHistogramType, "Relative Frequencies", AddressOf rbhtRelative_Click)
        rbhtRelative.GroupCaption = rpHistogramOption
        header.Add(rbhtRelative)
        rbHistogramType.Visible = False

        'Histogram Algorithm Menu
        rbAlgorithms = New MenuContainerItem(kGraph, kHistogramAlgorithm, "Binning Algorithms")
        rbAlgorithms.LargeImage = My.Resources.Binning
        rbAlgorithms.GroupCaption = rpHistogramOption
        header.Add(rbAlgorithms)

        'Scott's
        rbhaScott = New SimpleActionItem(kGraph, kHistogramAlgorithm, "Scott's", AddressOf rbhaScott_Click)
        rbhaScott.GroupCaption = rpHistogramOption
        header.Add(rbhaScott)
        'Sturges
        rbhaSturges = New SimpleActionItem(kGraph, kHistogramAlgorithm, "Sturges", AddressOf rbhaSturges_Click)
        rbhaSturges.GroupCaption = rpHistogramOption
        header.Add(rbhaSturges)
        'Freedman-Diaconis
        rbhaFreedman = New SimpleActionItem(kGraph, kHistogramAlgorithm, "Freedman-Diaconis", AddressOf rbhaFreedman_Click)
        rbhaFreedman.GroupCaption = rpHistogramOption
        header.Add(rbhaFreedman)
        rbAlgorithms.Visible = False

        'Box Whisker Plot Option Panel
        rbBoxWhiskerType = New MenuContainerItem(kGraph, kBoxWhiskerType, "Box Whisker Type")
        rbBoxWhiskerType.LargeImage = My.Resources.BoxWhiskerType
        rbBoxWhiskerType.GroupCaption = rpBoxWhiskerOption
        header.Add(rbBoxWhiskerType)
        rbBoxWhiskerType.Visible = False
        'Monthly
        rbbtMonthly = New SimpleActionItem(kGraph, kBoxWhiskerType, "Monthly", AddressOf rbbtMonthly_Click)
        rbbtMonthly.GroupCaption = rpBoxWhiskerOption
        header.Add(rbbtMonthly)
        'Seasonal
        rbbtSeasonal = New SimpleActionItem(kGraph, kBoxWhiskerType, "Seasonal", AddressOf rbbtSeasonal_Click)
        rbbtSeasonal.GroupCaption = rpBoxWhiskerOption
        header.Add(rbbtSeasonal)
        'Yearly
        rbbtYearly = New SimpleActionItem(kGraph, kBoxWhiskerType, "Yearly", AddressOf rbbtYearly_Click)
        rbbtYearly.GroupCaption = rpBoxWhiskerOption
        header.Add(rbbtYearly)
        'Overall
        rbbtOverall = New SimpleActionItem(kGraph, kBoxWhiskerType, "Overall", AddressOf rbbtOverall_Click)
        rbbtOverall.GroupCaption = rpBoxWhiskerOption
        header.Add(rbbtOverall)
        'rbbtOverall.Visible = False

        'Others
        'Date Setting
        rbStartDate = New TextEntryActionItem()
        rbStartDate.Caption = "Start"
        rbStartDate.GroupCaption = rpOtherOptions
        rbStartDate.RootKey = kGraph
        rbStartDate.Width = 60
        AddHandler rbStartDate.PropertyChanged, AddressOf dateSettings_PropertyChanged
        header.Add(rbStartDate)

        rbEndDate = New TextEntryActionItem()
        rbEndDate.Caption = " End"
        rbEndDate.GroupCaption = rpOtherOptions
        rbEndDate.RootKey = kGraph
        rbEndDate.Width = 60
        AddHandler rbEndDate.PropertyChanged, AddressOf dateSettings_PropertyChanged
        header.Add(rbEndDate)

        AddHandler _plotOptions.DatesChanged, AddressOf mainControlDatesChanged

        rbApplyDateSettings = New SimpleActionItem("Refresh", AddressOf rbDateTimeRefresh_Click)
        rbApplyDateSettings.RootKey = kGraph
        rbApplyDateSettings.LargeImage = My.Resources.DateSetting
        rbApplyDateSettings.GroupCaption = rpOtherOptions
        header.Add(rbApplyDateSettings)

        rbDisplayFullDateRange = New SimpleActionItem("Full Date Range", AddressOf rbDisplayFullDateRange_Click)
        rbDisplayFullDateRange.RootKey = kGraph
        rbDisplayFullDateRange.LargeImage = My.Resources.FullDateRange
        rbDisplayFullDateRange.SmallImage = My.Resources.FullDateRange_16
        rbDisplayFullDateRange.GroupCaption = rpOtherOptions
        rbDisplayFullDateRange.Enabled = False
        header.Add(rbDisplayFullDateRange)

        'Chart
        'Show Point Values
        rbShowPointValues = New SimpleActionItem("Show Point Values", AddressOf rbShowPointValues_Click)
        rbShowPointValues.RootKey = kGraph
        rbShowPointValues.GroupCaption = rpChart
        header.Add(rbShowPointValues)
        rbShowPointValues.Visible = False
        rbShowPointValues_Click(Me, EventArgs.Empty)

        'Zoom In
        rbZoomIn = New SimpleActionItem("Zoom In", AddressOf rbZoomIn_Click)
        rbZoomIn.RootKey = kGraph
        rbZoomIn.GroupCaption = rpChart
        rbZoomIn.Visible = False
        header.Add(rbZoomIn)

        ' Zoom Out
        rbZoomOut = New SimpleActionItem("Zoom Out", AddressOf rbZoomOut_Click)
        rbZoomOut.RootKey = kGraph
        rbZoomOut.GroupCaption = rpChart
        rbZoomOut.Visible = False
        header.Add(rbZoomOut)

        ' Undo Zoom
        rbUndoZoom = New SimpleActionItem("Undo Zoom", AddressOf rbUndoZoom_Click)
        rbUndoZoom.RootKey = kGraph
        rbUndoZoom.GroupCaption = rpChart
        rbUndoZoom.Visible = False
        header.Add(rbUndoZoom)

        'The button should initially be checked
        rbTSA_Click(Me, EventArgs.Empty)
        rbTSA.Toggle()

    End Sub

    Private Sub dateSettings_PropertyChanged(ByVal sender As Object, ByVal e As PropertyChangedEventArgs)
        If "Text".Equals(e.PropertyName) Then

            Dim startDate = ValidateDateEdit(rbStartDate, "Start Date", _datesFormat, False)
            If (startDate Is Nothing) Then Return
            Dim endDate = ValidateDateEdit(rbEndDate, "End Date", _datesFormat, False)
            If (endDate Is Nothing) Then Return

            If _plotOptions.StartDateLimit.Date >= startDate And
                   _plotOptions.EndDateLimit.Date <= endDate Then
                rbDisplayFullDateRange.Enabled = False
            Else
                rbDisplayFullDateRange.Enabled = True
            End If

        End If
    End Sub

    Private Sub rbDateTimeRefresh_Click(ByVal sender As Object, ByVal e As EventArgs)
        ' Validation of Start/End date
        Dim startDate = ValidateDateEdit(rbStartDate, "Start Date", _datesFormat, True)
        If (Not startDate.HasValue) Then Return
        Dim endDate = ValidateDateEdit(rbEndDate, "End Date", _datesFormat, True)
        If (Not endDate.HasValue) Then Return
        ' end of validation

        _plotOptions.StartDateTime = startDate.Value
        _plotOptions.EndDateTime = endDate.Value
        _plotOptions.DisplayFullDate = False
        _mainControl.ApplyOptions(True)
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
        rbStartDate.Text = _plotOptions.StartDateTime.ToString(_datesFormat)
        rbEndDate.Text = _plotOptions.EndDateTime.ToString(_datesFormat)
    End Sub

#End Region

#Region "Event Handlers"
    'Click Time Series
    Sub rbTSA_Click(ByVal sender As Object, ByVal e As EventArgs)
        _mainControl.ShowTimeSeriesPlot()
        rbPlotType.Visible = True
        rbColorSetting.Visible = True
        rbShowLegend.Visible = True

        rbHistogramType.Visible = False
        rbAlgorithms.Visible = False
        rbBoxWhiskerType.Visible = False

    End Sub

    Sub rbProbability_Click(ByVal sender As Object, ByVal e As EventArgs)
        _mainControl.ShowProbabilityPlot()
        rbPlotType.Visible = True
        rbColorSetting.Visible = True
        rbShowLegend.Visible = True

        rbHistogramType.Visible = False
        rbAlgorithms.Visible = False
        rbBoxWhiskerType.Visible = False
    End Sub

    Sub rbHistogram_Click(ByVal sender As Object, ByVal e As EventArgs)
        _mainControl.ShowHistogramPlot()
        rbPlotType.Visible = False
        rbColorSetting.Visible = False
        rbShowLegend.Visible = False

        rbHistogramType.Visible = True
        rbAlgorithms.Visible = True
        rbBoxWhiskerType.Visible = False
    End Sub

    Sub rbBoxWhisker_Click(ByVal sender As Object, ByVal e As EventArgs)
        _mainControl.ShowBoxWhiskerPlot()
        rbPlotType.Visible = False
        rbColorSetting.Visible = False
        rbShowLegend.Visible = False

        rbHistogramType.Visible = False
        rbAlgorithms.Visible = False
        rbBoxWhiskerType.Visible = True
    End Sub

    Sub rbSummary_Click(ByVal sender As Object, ByVal e As EventArgs)
        _mainControl.ShowSummaryPlot()
        rbPlotType.Visible = False
        rbColorSetting.Visible = False
        rbShowLegend.Visible = False

        rbHistogramType.Visible = False
        rbAlgorithms.Visible = False
        rbBoxWhiskerType.Visible = False
    End Sub

    Sub rbShowLegend_Click(ByVal sender As Object, ByVal e As EventArgs)
        Dim text = If(_plotOptions.ShowLegend, "Show Legend", "Close Legend")

        _plotOptions.ShowLegend = Not _plotOptions.ShowLegend
        _mainControl.ApplyOptions()
        rbShowLegend.Caption = text
    End Sub

    Sub rbColorSetting_Click(ByVal sender As Object, ByVal e As EventArgs)
        Dim frmCC = New ColorSettingsDialog(_plotOptions.LineColorList, _plotOptions.PointColorList)
        AddHandler frmCC.ColorsApplied, AddressOf OnColorsApplied
        frmCC.ShowDialog()
    End Sub

    Private Sub OnColorsApplied(ByVal sender As Object, ByVal e As EventArgs)
        Dim form = DirectCast(sender, ColorSettingsDialog)

        _plotOptions.PointColorList.Clear()
        For i As Integer = 0 To form.pointcolorlist.Count
            _plotOptions.PointColorList.Add(form.pointcolorlist(i))
        Next
        _plotOptions.LineColorList.Clear()
        For i As Integer = 0 To form.linecolorlist.Count
            _plotOptions.LineColorList.Add(form.linecolorlist(i))
        Next

        _mainControl.ApplyOptions()
    End Sub

    Sub rbhtCount_Click(ByVal sender As Object, ByVal e As EventArgs)
        _plotOptions.HistTypeMethod = HistogramType.Count
        _mainControl.ApplyOptions()
    End Sub
    Sub rbhtProbability_Click(ByVal sender As Object, ByVal e As EventArgs)
        _plotOptions.HistTypeMethod = HistogramType.Probability
        _mainControl.ApplyOptions()
    End Sub
    Sub rbhtRelative_Click(ByVal sender As Object, ByVal e As EventArgs)
        _plotOptions.HistTypeMethod = HistogramType.Relative
        _mainControl.ApplyOptions()
    End Sub

    Sub rbhaSturges_Click(ByVal sender As Object, ByVal e As EventArgs)
        _plotOptions.HistAlgorothmsMethod = HistorgramAlgorithms.Sturges
        _mainControl.ApplyOptions()
    End Sub
    Sub rbhaScott_Click(ByVal sender As Object, ByVal e As EventArgs)
        _plotOptions.HistAlgorothmsMethod = HistorgramAlgorithms.Scott
        _mainControl.ApplyOptions()
    End Sub
    Sub rbhaFreedman_Click(ByVal sender As Object, ByVal e As EventArgs)
        _plotOptions.HistAlgorothmsMethod = HistorgramAlgorithms.Freedman
        _mainControl.ApplyOptions()
    End Sub

    Sub rbbtMonthly_Click(ByVal sender As Object, ByVal e As EventArgs)
        _plotOptions.BoxWhiskerMethod = BoxWhiskerType.Monthly
        _mainControl.ApplyOptions()
    End Sub
    Sub rbbtSeasonal_Click(ByVal sender As Object, ByVal e As EventArgs)
        _plotOptions.BoxWhiskerMethod = BoxWhiskerType.Seasonal
        _mainControl.ApplyOptions()
    End Sub
    Sub rbbtYearly_Click(ByVal sender As Object, ByVal e As EventArgs)
        _plotOptions.BoxWhiskerMethod = BoxWhiskerType.Yearly
        _mainControl.ApplyOptions()
    End Sub
    Sub rbbtOverall_Click(ByVal sender As Object, ByVal e As EventArgs)
        _plotOptions.BoxWhiskerMethod = BoxWhiskerType.Overall
        _mainControl.ApplyOptions()
    End Sub

    'Display full date range toggle button is clicked
    Private Sub rbDisplayFullDateRange_Click(ByVal sender As Object, ByVal e As EventArgs)
        _plotOptions.DisplayFullDate = Not _plotOptions.DisplayFullDate
        _mainControl.ApplyOptions(True)
    End Sub

    'Show Point Values 
    Private Sub rbShowPointValues_Click(ByVal sender As Object, ByVal e As EventArgs)
        _mainControl.ShowPointValues(_showPointValues)
        If _showPointValues Then
            rbShowPointValues.Caption = "Show Point Values - On"
        Else
            rbShowPointValues.Caption = "Show Point Values - Off"
        End If
        _showPointValues = Not _showPointValues
    End Sub

    Private Sub rbUndoZoom_Click(ByVal sender As Object, ByVal e As EventArgs)
        _mainControl.UndoZoom()
    End Sub

    Private Sub rbZoomIn_Click(ByVal sender As Object, ByVal e As EventArgs)
        _mainControl.ZoomIn()
    End Sub

    Private Sub rbZoomOut_Click(ByVal sender As Object, ByVal e As EventArgs)
        _mainControl.ZoomOut()
    End Sub

    Sub DockManager_ActivePanelChanged(ByVal sender As Object, ByVal e As DockablePanelEventArgs)

        'activate the graph ribbon tab and the series view panel
        If e.ActivePanelKey = kGraph Then
            App.HeaderControl.SelectRoot(kGraph)
            App.DockManager.SelectPanel(SharedConstants.SeriesViewKey)
            IsPanelActive = True
        Else
            IsPanelActive = False
        End If

    End Sub

    Private _isPanelActive As Boolean
    Public Property IsPanelActive As Boolean
        Get
            Return _isPanelActive
        End Get
        Set(ByVal value As Boolean)
            If (value = _isPanelActive) Then Return
            _isPanelActive = value

            RaiseEvent IsPanelActiveChanged()
        End Set
    End Property

    Public Event IsPanelActiveChanged()

#End Region

End Class
