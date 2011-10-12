Imports System.Windows.Forms
Imports System.Drawing
Imports DotSpatial.Controls
Imports DotSpatial.Controls.RibbonControls
Imports HydroDesktop.Database
Imports HydroDesktop.Interfaces
Imports DotSpatial.Controls.Header
Imports System.ComponentModel.Composition
Imports DotSpatial.Controls.Docking


Namespace GraphView
    Public Class Main
        Inherits Extension

#Region "Variables"

        Private Const kGraph As String = "kHydroGraph"

        <Import("SeriesControl", GetType(ISeriesSelector))>
        Private appSeriesView As ISeriesSelector

        Private _mainControl As cTSA

        'reference to the main application and it's UI items
        Private Const _pluginName As String = "Graph"
        Private Const kSeriesViewPanelName As String = "Series View"

        Private tabGraph As RootItem

        Private rpPlots As String = "Plots"
        Private kTogglePlots As String = "kHydroPlotsGroup"

        Private rbTSA As SimpleActionItem 'Time Series
        Private rbProbability As SimpleActionItem 'Probability
        Private rbHistogram As SimpleActionItem 'Histogram
        Private rbBoxWhisker As SimpleActionItem 'Box/Whisker
        Private rbSummary As SimpleActionItem 'Summary

        Private rpPlotOption As String = "TSA & Probability Plot Options"
        Const PlotOptionsMenuKey = "kHydroPlotOptions"
        Private rbPlotType As MenuContainerItem 'Plot Type
        Private rbLine As SimpleActionItem 'Line
        Private rbPoint As SimpleActionItem 'Point
        Private rbBoth As SimpleActionItem 'Both
        Private rbColorSetting As SimpleActionItem 'Color Setting
        Private rbShowLegend As SimpleActionItem 'Close Legend

        Private rpHistogramOption As String = "Histogram Plot Options"
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

        Private rpBoxWhiskerOption As String = "Box Whisker Plot Option"
        Const kBoxWhiskerType = "kBoxWhiskerType"
        Private rbBoxWhiskerType As MenuContainerItem 'Box Whisker Type
        Private rbbtMonthly As SimpleActionItem 'Monthly
        Private rbbtSeasonal As SimpleActionItem 'Seasonal
        Private rbbtYearly As SimpleActionItem 'Yearly
        Private rbbtOverall As SimpleActionItem 'Overall

        Private rpOtherOptions As String = "Date & Time"
        Private rbDateTimeSetting As SimpleActionItem 'Date Setting
        Private rbDisplayFullDateRange As SimpleActionItem 'Display Full Date Range Toggle button
        Private boolFullDateRange As Boolean = True 'Display Full Date Range boolean indicator
#End Region


#Region "IExtension Members"

        'When the plugin is initialized
        Public Overrides Sub Activate()

            'To Add Items to the ribbon menu
            tabGraph = New RootItem(kGraph, _pluginName)
            tabGraph.SortOrder = 30
            App.HeaderControl.Add(tabGraph)

            If Not appSeriesView Is Nothing Then
                _mainControl = New cTSA(appSeriesView)
            Else
                _mainControl = New cTSA()
            End If
            _mainControl.Dock = DockStyle.Fill
            App.DockManager.Add(New DockablePanel(kGraph, _pluginName, _mainControl, DockStyle.Fill))

            InitializeRibbonButtons()

            'when the graph dock panel is activated:
            'show graph ribbon tab and series view
            AddHandler App.DockManager.ActivePanelChanged, AddressOf DockManager_ActivePanelChanged

            MyBase.Activate()
        End Sub

        'when the plug-in is deactivated
        Public Overrides Sub Deactivate()

            'auto-remove all ribbon items
            App.HeaderControl.RemoveAll()

            'remove the dock panel
            App.DockManager.Remove(kGraph)

            'important line to deactivate the plugin
            MyBase.Deactivate()

        End Sub

        Private Sub InitializeRibbonButtons()

            Dim header As HeaderControl = App.HeaderControl

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
            rbLine = New SimpleActionItem(kGraph, PlotOptionsMenuKey, "Line", AddressOf rbLine_Click)
            rbLine.GroupCaption = rpPlotOption
            header.Add(rbLine)
            'Point
            rbPoint = New SimpleActionItem(kGraph, PlotOptionsMenuKey, "Point", AddressOf rbPoint_Click)
            rbPoint.GroupCaption = rpPlotOption
            header.Add(rbPoint)
            'Both
            rbBoth = New SimpleActionItem(kGraph, PlotOptionsMenuKey, "Both", AddressOf rbBoth_Click)
            rbBoth.GroupCaption = rpPlotOption
            header.Add(rbBoth)

            'Color Setting
            rbColorSetting = New SimpleActionItem("Color Setting", AddressOf rbColorSetting_Click)
            rbColorSetting.RootKey = kGraph
            rbColorSetting.LargeImage = My.Resources.ColorSetting
            rbColorSetting.GroupCaption = rpPlotOption
            header.Add(rbColorSetting)

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

            'Box Whisker Plot Option Panel
            rbBoxWhiskerType = New MenuContainerItem(kGraph, kBoxWhiskerType, "Box Whisker Type")
            rbBoxWhiskerType.LargeImage = My.Resources.BoxWhiskerType
            rbBoxWhiskerType.GroupCaption = rpBoxWhiskerOption
            header.Add(rbBoxWhiskerType)
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

            'Others
            'Date Setting
            rbDateTimeSetting = New SimpleActionItem("Date Setting", AddressOf rbDateTimeSetting_Click)
            rbDateTimeSetting.RootKey = kGraph
            rbDateTimeSetting.LargeImage = My.Resources.DateSetting
            rbDateTimeSetting.GroupCaption = rpOtherOptions
            header.Add(rbDateTimeSetting)

            'TODO: Start and end date labels: Add to a different place!
            'rlblStratDate.Text = "Start Date:"
            'rlblEndDate.Text = "End Date:"
            'rpOtherOptions.Items.Add(rlblStratDate)
            'rpOtherOptions.Items.Add(rlblEndDate)
            '_mainControl.rlblStratDate = rlblStratDate
            '_mainControl.rlblEndDate = rlblEndDate

            'Display Full Date Range toggle button
            rbDisplayFullDateRange = New SimpleActionItem("Display Full Date Range", AddressOf rbDisplayFullDateRange_Click)
            rbDisplayFullDateRange.RootKey = kGraph
            rbDisplayFullDateRange.ToggleGroupKey = "tkFullDateRange"
            rbDisplayFullDateRange.GroupCaption = rpOtherOptions
            header.Add(rbDisplayFullDateRange)

            'The button should initially be checked
            rbTSA_Click()

        End Sub

#End Region

#Region "Event Handlers"
        'Click Time Series
        Sub rbTSA_Click()
            'UncheckOtherPlotButtons(rbTSA)
            _mainControl.TabControl2.SelectTab(0)
            rbPlotType.Visible = True
            rbHistogramType.Visible = False
            rbAlgorithms.Visible = False
            rbBoxWhiskerType.Visible = False

        End Sub

        Sub rbProbability_Click()
            'UncheckOtherPlotButtons(rbProbability)
            _mainControl.TabControl2.SelectTab(1)
            rbPlotType.Visible = True
            rbHistogramType.Visible = False
            rbAlgorithms.Visible = False
            rbBoxWhiskerType.Visible = False
            'rpPlotType.Visible = True
            'rpHistogramOption.Visible = False
            'rpBoxWhiskerOption.Visible = False
        End Sub

        Sub rbHistogram_Click()
            'UncheckOtherPlotButtons(rbHistogram)
            _mainControl.TabControl2.SelectTab(2)
            rbPlotType.Visible = False
            rbHistogramType.Visible = True
            rbAlgorithms.Visible = True
            rbBoxWhiskerType.Visible = False
            'rpPlotOption.Visible = False
            'rpHistogramOption.Visible = True
            'rpBoxWhiskerOption.Visible = False
        End Sub

        Sub rbBoxWhisker_Click()
            'UncheckOtherPlotButtons(rbBoxWhisker)
            _mainControl.TabControl2.SelectTab(3)
            rbPlotType.Visible = False
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
            If rbShowLegend.Caption = "Close Legend" Then
                _mainControl.CPlotOptions1.IsShowLegend = False
                _mainControl.ApplyOptions()
                rbShowLegend.Caption = "Show Legend"
            Else
                _mainControl.CPlotOptions1.IsShowLegend = True
                _mainControl.ApplyOptions()
                rbShowLegend.Caption = "Close Legend"
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

            'rckbDisplayFullDateRange.Checked = False
            _mainControl.IsDisplayFullDate = False
            Dim frmDateTimeSetting = New fDateTimeSetting
            frmDateTimeSetting._CTSA = _mainControl
            frmDateTimeSetting.initialize()
            frmDateTimeSetting.ShowDialog()
        End Sub

        'Display full date range toggle button is clicked
        Sub rbDisplayFullDateRange_Click()
            If _mainControl.IsDisplayFullDate Then
                _mainControl.IsDisplayFullDate = False
            Else
                _mainControl.IsDisplayFullDate = True
                _mainControl.ApplyOptions()
            End If
            'If rckbDisplayFullDateRange.Checked Then
            '    _mainControl.IsDisplayFullDate = True
            '    _mainControl.ApplyOptions()
            'Else
            '    _mainControl.IsDisplayFullDate = False
            'End If
        End Sub


        'this is replaced by toggle buttons
        'Sub UncheckOtherPlotButtons(ByRef PlotButton As RibbonItem)
        '    For Each rbi As RibbonItem In rpPlots.Items
        '        If TypeOf rbi Is RibbonButton And Not rbi.Equals(PlotButton) Then
        '            rbi.Checked = False
        '        End If
        '    Next
        'End Sub

        Sub DockManager_ActivePanelChanged(ByVal sender As Object, ByVal e As Docking.ActivePanelChangedEventArgs)

            'activate the graph ribbon tab and the series view panel
            If e.ActivePanelKey = kGraph Then
                App.HeaderControl.SelectRoot(kGraph)
                App.DockManager.SelectPanel("kHydroSeriesView")
            End If

        End Sub

#End Region

    End Class
End Namespace
