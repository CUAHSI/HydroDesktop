Imports HydroDesktop.Database
Imports HydroDesktop.Interfaces
Imports GraphView.My.Resources

Namespace Controls

    Public Class MainControl

#Region "Fields"

        Private ReadOnly _parent As GraphViewPlugin
        Private colorcount As Integer = 0
        Private ReadOnly selectedSeriesIdList As New List(Of Int32) 'the list of the series which is selected

#End Region

#Region "Constructors"
        Public Sub New(ByVal parent As GraphViewPlugin)
            ' This call is required by the Windows Form Designer.
            InitializeComponent()

            _parent = parent

            'assign the events
            AddHandler parent.SeriesSelector.SeriesCheck, AddressOf SeriesSelector_SeriesCheck
            AddHandler parent.SeriesSelector.Refreshed, AddressOf SeriesSelector_Refreshed
            AddHandler Disposed, AddressOf OnDisposing

            probabilityPlot.SeriesSelector = parent.SeriesSelector
            probabilityPlot.AppManager = parent.App
            timeSeriesPlot.SeriesSelector = parent.SeriesSelector
            timeSeriesPlot.AppManager = parent.App

            selectedSeriesIdList.Clear()
            _parent.PlotOptions.StartDateLimit = Today.AddYears(-150)
            _parent.PlotOptions.EndDateLimit = Today
        End Sub


        Private Sub OnDisposing(ByVal sender As Object, ByVal e As EventArgs)
            ' Unsubscribe from events
            RemoveHandler Disposed, AddressOf OnDisposing
            RemoveHandler _parent.SeriesSelector.SeriesCheck, AddressOf SeriesSelector_SeriesCheck
            RemoveHandler _parent.SeriesSelector.Refreshed, AddressOf SeriesSelector_Refreshed
        End Sub

#End Region

#Region "Private Methods"

        Public Sub ShowTimeSeriesPlot()
            tcPlots.SelectTab(tpTimeSeries)
        End Sub

        Public Sub ShowProbabilityPlot()
            tcPlots.SelectTab(tpTimeSeries)
        End Sub

        Public Sub ShowHistogramPlot()
            tcPlots.SelectTab(tpHistogram)
        End Sub

        Public Sub ShowBoxWhiskerPlot()
            tcPlots.SelectTab(tpBoxWhisker)
        End Sub

        Public Sub ShowSummaryPlot()
            tcPlots.SelectTab(tpDataSummary)
        End Sub

        Private Sub SeriesSelector_Refreshed(ByVal sender As Object, ByVal e As EventArgs)
            boxWhisker.Clear()
            probabilityPlot.Clear()
            selectedSeriesIdList.Clear()
        End Sub

        'when a series is checked in the series selector control
        Private Sub SeriesSelector_SeriesCheck(ByVal sender As Object, ByVal e As SeriesEventArgs)

            'Declaring all variables
            Dim curveIndex As Integer = 0
            Dim removedSeriesID As Integer = 0
            Dim CheckedSeriesState As Boolean = False

            If Not (selectedSeriesIdList.Contains(_parent.SeriesSelector.SelectedSeriesID)) Then
                selectedSeriesIdList.Add(_parent.SeriesSelector.SelectedSeriesID)
                CheckedSeriesState = True
            Else
                For i As Integer = 0 To selectedSeriesIdList.Count - 1
                    If Not _parent.SeriesSelector.CheckedIDList.Contains(selectedSeriesIdList(i)) Then
                        removedSeriesID = selectedSeriesIdList(i)
                        curveIndex = i
                    End If
                Next

                selectedSeriesIdList.Remove(selectedSeriesIdList(curveIndex))
                If (selectedSeriesIdList.Count = 0) Or (selectedSeriesIdList.Count = 1) Then
                    'Clear the graph and repolt the whole graph
                    probabilityPlot.Remove(0)
                    boxWhisker.Clear()
                    histogramPlot.Clear()
                    probabilityPlot.Clear()
                    colorcount = 0
                    _parent.PlotOptions.StartDateLimit = Today.AddYears(-150)
                    _parent.PlotOptions.EndDateLimit = Today
                Else
                    probabilityPlot.Remove(removedSeriesID)
                    boxWhisker.Remove(removedSeriesID)
                    histogramPlot.Remove(removedSeriesID)
                    If _parent.PlotOptions.DisplayFullDate Then
                        ResetDateRange()
                    End If
                End If
            End If


            If (CheckedSeriesState = True) Or (selectedSeriesIdList.Count = 1) Then

                'progress bar setting
                ProgressBar.Visible = True
                ProgressBar.Maximum = 11
                ProgressBar.Minimum = 0
                ProgressBar.Value = 0

                Dim seriesID = selectedSeriesIdList(selectedSeriesIdList.Count - 1)
                PlotGraps(seriesID)
                colorcount += 1

            End If

            Dim seriesPlotInfo = New SeriesPlotInfo(_parent.SeriesSelector.CheckedIDList,
                                                    _parent.SeriesSelector.SiteDisplayColumn,
                                                    _parent.PlotOptions)

            dataSummary.Plot(seriesPlotInfo)
            timeSeriesPlot.Plot(seriesPlotInfo)


            probabilityPlot.Refreshing()
            histogramPlot.Refreshing()
            boxWhisker.Refreshing()

            ProgressBar.Visible = False

        End Sub

        Private Function GetTimeSeriesPlotOptions(ByVal seriesID As Integer) As OneSeriesPlotInfo
            Return OneSeriesPlotInfo.Create(seriesID, _parent.SeriesSelector.SiteDisplayColumn, _parent.PlotOptions)
        End Function

        Private Sub PlotGraps(ByVal seriesID As Int32)

            'Date Range setting
            DateRangeSelection(seriesID)

            'get data
            Dim timeSeriesOptions = GetTimeSeriesPlotOptions(seriesID)

            'Set different color to each curve if the color option is not selected
            ColorChooser(timeSeriesOptions.PlotOptions)

            Dim statistics = SummaryStatistics.Create(timeSeriesOptions.DataTable, timeSeriesOptions.PlotOptions.UseCensoredData)
            If statistics.NumberOfObservations > statistics.NumberOfCensoredObservations Then
                boxWhisker.Plot(timeSeriesOptions, statistics.StandardDeviation)
                If ProgressBar.Value < ProgressBar.Maximum Then ProgressBar.Value += 1

                probabilityPlot.Plot(timeSeriesOptions, statistics.StandardDeviation)
                If ProgressBar.Value < ProgressBar.Maximum Then ProgressBar.Value += 1

                histogramPlot.Plot(timeSeriesOptions, statistics.StandardDeviation)
                If ProgressBar.Value < ProgressBar.Maximum Then ProgressBar.Value += 1

            ElseIf statistics.NumberOfObservations = statistics.NumberOfCensoredObservations Then
                If boxWhisker.CurveCount = 0 Then boxWhisker.SetGraphPaneTitle(MessageStrings.All_Data_Censored)
                If probabilityPlot.CurveCount = 0 Then probabilityPlot.SetGraphPaneTitle(MessageStrings.All_Data_Censored)
                If histogramPlot.CurveCount = 0 Then histogramPlot.SetGraphPaneTitle(MessageStrings.All_Data_Censored)

            End If

        End Sub

        Public Sub ApplyOptions()

            Dim seriesPlotInfo = New SeriesPlotInfo(_parent.SeriesSelector.CheckedIDList, _parent.SeriesSelector.SiteDisplayColumn, _parent.PlotOptions)

            'progress bar setting
            ProgressBar.Visible = True
            ProgressBar.Maximum = selectedSeriesIdList.Count * 10
            ProgressBar.Minimum = 0
            ProgressBar.Value = 0
            colorcount = 0

            'Clear the graph and plot it again
            boxWhisker.Clear()
            histogramPlot.Clear()
            probabilityPlot.Clear()

            'Ploting the Time Series graph and Probability graph
            For Each s As Integer In selectedSeriesIdList
                PlotGraps(s)
                colorcount += 1
            Next

            dataSummary.Plot(seriesPlotInfo)
            timeSeriesPlot.Plot(seriesPlotInfo)

            probabilityPlot.Refreshing()
            histogramPlot.Refreshing()
            boxWhisker.Refreshing()

            ProgressBar.Visible = False
        End Sub

        Public Sub ShowPointValues(ByVal showPointValues As Boolean)
            timeSeriesPlot.ShowPointValues = showPointValues
            probabilityPlot.ShowPointValues = showPointValues
            histogramPlot.ShowPointValues = showPointValues
            boxWhisker.ShowPointValues = showPointValues
        End Sub

        Public Sub UndoZoom()
            timeSeriesPlot.ZoomOutAll()
            probabilityPlot.ZoomOutAll()
            histogramPlot.ZoomOutAll()
            boxWhisker.ZoomOutAll()
        End Sub

        Public Sub ZoomIn()
            timeSeriesPlot.ZoomIn()
            probabilityPlot.ZoomIn()
            histogramPlot.ZoomIn()
            boxWhisker.ZoomIn()
        End Sub

        Public Sub ZoomOut()
            timeSeriesPlot.ZoomOut()
            probabilityPlot.ZoomOut()
            histogramPlot.ZoomOut()
            boxWhisker.ZoomOut()
        End Sub

        Private Sub ColorChooser(ByVal options As PlotOptions)
            options.GetPointColor = options.PointColorList(colorcount Mod 10)
            options.GetLineColor = options.LineColorList(colorcount Mod 10)
        End Sub

        Private Sub DateRangeSelection(ByVal serieID As Integer)
            Dim repository = RepositoryFactory.Instance.Get(Of IDataSeriesRepository)()
            Dim series = repository.GetByKey(serieID)
            If series = Nothing Then Return

            Dim bDateTime = series.BeginDateTime
            Dim eDateTime = series.EndDateTime
            Dim plotOptions = _parent.PlotOptions

            If plotOptions.StartDateLimit > bDateTime Or plotOptions.StartDateLimit = Today.AddYears(-150) Then
                plotOptions.StartDateLimit = bDateTime
            End If
            If plotOptions.EndDateLimit < eDateTime Or plotOptions.EndDateLimit = Today Then
                plotOptions.EndDateLimit = eDateTime
            End If

            If _parent.PlotOptions.DisplayFullDate Then
                plotOptions.StartDateTime = plotOptions.StartDateLimit
                plotOptions.EndDateTime = plotOptions.EndDateLimit
            End If

        End Sub

        Private Sub ResetDateRange()
            Dim plotOptions = _parent.PlotOptions

            plotOptions.StartDateLimit = Today.AddYears(-150)
            plotOptions.EndDateLimit = Today
            For i As Integer = 0 To selectedSeriesIdList.Count - 1
                DateRangeSelection(i)
            Next
        End Sub
#End Region

    End Class

End Namespace