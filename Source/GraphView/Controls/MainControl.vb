Imports HydroDesktop.Database
Imports HydroDesktop.Interfaces

Namespace Controls

    Public Class MainControl

#Region "Fields"

        Private ReadOnly _parent As GraphViewPlugin
        Private ReadOnly _charts As ICollection(Of IChart)

#End Region

#Region "Constructors"

        Public Sub New(ByVal parent As GraphViewPlugin)
            ' This call is required by the Windows Form Designer.
            InitializeComponent()

            _parent = parent
            _charts = {timeSeriesPlot, probabilityPlot, histogramPlot, boxWhisker}

            'assign the events
            AddHandler parent.SeriesSelector.SeriesCheck, AddressOf SeriesSelector_SeriesCheck
            AddHandler Disposed, AddressOf OnDisposing

            probabilityPlot.SeriesSelector = parent.SeriesSelector
            probabilityPlot.AppManager = parent.App
            timeSeriesPlot.SeriesSelector = parent.SeriesSelector
            timeSeriesPlot.AppManager = parent.App

            _parent.PlotOptions.StartDateLimit = Today.AddYears(-150)
            _parent.PlotOptions.EndDateLimit = Today
        End Sub


        Private Sub OnDisposing(ByVal sender As Object, ByVal e As EventArgs)
            ' Unsubscribe from events
            RemoveHandler _parent.SeriesSelector.SeriesCheck, AddressOf SeriesSelector_SeriesCheck
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

        Public Sub ApplyOptions()
            ' todo: ProgressBar
            ' todo: checking for already loaded data tables
            ' todo: reload data table only when dates changed 
            Dim seriesPlotInfo = New SeriesPlotInfo(_parent.SeriesSelector.CheckedIDList,
                                                    _parent.SeriesSelector.SiteDisplayColumn,
                                                    _parent.PlotOptions)

            For Each id In _parent.SeriesSelector.CheckedIDList
                DateRangeSelection(id)
            Next

            dataSummary.Plot(seriesPlotInfo)
            timeSeriesPlot.Plot(seriesPlotInfo)
            probabilityPlot.Plot(seriesPlotInfo)
            histogramPlot.Plot(seriesPlotInfo)
            boxWhisker.Plot(seriesPlotInfo)
        End Sub

        Public Sub ShowPointValues(ByVal showPointValues As Boolean)
            For Each chart As IChart In _charts
                chart.ShowPointValues = showPointValues
            Next
        End Sub

        Public Sub UndoZoom()
            For Each chart As IChart In _charts
                chart.ZoomOutAll()
            Next
        End Sub

        Public Sub ZoomIn()
            For Each chart As IChart In _charts
                chart.ZoomIn()
            Next
        End Sub

        Public Sub ZoomOut()
            For Each chart As IChart In _charts
                chart.ZoomOut()
            Next
        End Sub

#End Region

#Region "Private methods"

        Private Sub SeriesSelector_SeriesCheck(ByVal sender As Object, ByVal e As SeriesEventArgs)
            ApplyOptions()
        End Sub

        Private Sub DateRangeSelection(ByVal serieID As Integer)
            Dim repository = RepositoryFactory.Instance.Get(Of IDataSeriesRepository)()
            Dim range = repository.GetDatesRange(serieID)
            If range Is Nothing Then Return

            Dim bDateTime = range.Item1
            Dim eDateTime = range.Item2
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

#End Region

    End Class

End Namespace