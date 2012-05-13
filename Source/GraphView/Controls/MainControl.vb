Imports HydroDesktop.Database
Imports HydroDesktop.Interfaces
Imports System.Drawing

Namespace Controls

    Public Class MainControl

#Region "Fields"

        Private ReadOnly Summary As New SummaryStatistics
        Public linecolorlist As New List(Of Color)
        Public pointcolorlist As New List(Of Color)
        Private ReadOnly ccList0 As New List(Of Color)
        Private colorcount As Integer = 0
        Private ReadOnly selectedSeriesIdList As New List(Of Int32) 'the list of the series which is selected
        Private _seriesMenu As ISeriesSelector
        Public Event DatesChanged As EventHandler
        Public IsDisplayFullDate As Boolean = True

#End Region

#Region "Constructors"
        Public Sub New(ByVal seriesSelector As ISeriesSelector)
            ' This call is required by the Windows Form Designer.
            InitializeComponent()

            _seriesMenu = seriesSelector

            'assign the events
            AddHandler _seriesMenu.SeriesCheck, AddressOf SeriesSelector_SeriesCheck
            AddHandler _seriesMenu.Refreshed, AddressOf SeriesSelector_Refreshed
            AddHandler Disposed, AddressOf OnDisposing

            probabilityPlot.SeriesSelector = seriesSelector
            timeSeriesPlot.SeriesSelector = seriesSelector

            SetColorCollections()
            selectedSeriesIdList.Clear()
            timeSeriesPlot.Clear()

            StartDateLimit = Today.AddYears(-150)
            EndDateLimit = Today
        End Sub


        Private Sub OnDisposing(ByVal sender As Object, ByVal e As EventArgs)
            ' Unsubscribe from events
            RemoveHandler Disposed, AddressOf OnDisposing
            RemoveHandler _seriesMenu.SeriesCheck, AddressOf SeriesSelector_SeriesCheck
            RemoveHandler _seriesMenu.Refreshed, AddressOf SeriesSelector_Refreshed
            _seriesMenu = Nothing
        End Sub

#End Region

#Region "Properties"

        Private _startDateTime As DateTime
        Public Property StartDateTime() As DateTime
            Get
                Return _startDateTime
            End Get
            Set(ByVal value As DateTime)
                _startDateTime = value

                RaiseDatesChanged()
            End Set
        End Property

        Private _endDateTime As DateTime
        Public Property EndDateTime() As Date
            Get
                Return _endDateTime
            End Get
            Set(value As Date)
                _endDateTime = value

                RaiseDatesChanged()
            End Set
        End Property

        Private _startDateLimit As DateTime
        Public Property StartDateLimit() As Date
            Get
                Return _startDateLimit
            End Get
            Set(value As Date)
                _startDateLimit = value

                RaiseDatesChanged()
            End Set
        End Property

        Private _endDateLimit As DateTime
        Public Property EndDateLimit() As Date
            Get
                Return _endDateLimit
            End Get
            Set(value As Date)
                _endDateLimit = value

                RaiseDatesChanged()
            End Set
        End Property

#End Region

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

#Region "Private Methods"

        Private Sub RaiseDatesChanged()
            RaiseEvent DatesChanged(Me, EventArgs.Empty)
        End Sub

        Private Sub SetColorCollections()
            'Setting of color collections
            ccList0.Clear()
            ccList0.Add(Color.FromArgb(106, 61, 154))
            ccList0.Add(Color.FromArgb(202, 178, 214))
            ccList0.Add(Color.FromArgb(255, 127, 0))
            ccList0.Add(Color.FromArgb(253, 191, 111))
            ccList0.Add(Color.FromArgb(227, 26, 28))
            ccList0.Add(Color.FromArgb(251, 154, 153))
            ccList0.Add(Color.FromArgb(51, 160, 44))
            ccList0.Add(Color.FromArgb(178, 223, 138))
            ccList0.Add(Color.FromArgb(31, 120, 180))
            ccList0.Add(Color.FromArgb(166, 206, 227))

            For i = 0 To 9
                linecolorlist.Add(ccList0(i))
            Next
            For i = 0 To 9
                pointcolorlist.Add(ccList0(i))
            Next
        End Sub

        Private Sub SeriesSelector_Refreshed(ByVal sender As Object, ByVal e As EventArgs)

            timeSeriesPlot.Clear()
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

            If Not (selectedSeriesIdList.Contains(_seriesMenu.SelectedSeriesID)) Then
                selectedSeriesIdList.Add(_seriesMenu.SelectedSeriesID)
                CheckedSeriesState = True
            Else
                For i As Integer = 0 To selectedSeriesIdList.Count - 1
                    If Not _seriesMenu.CheckedIDList.Contains(selectedSeriesIdList(i)) Then
                        removedSeriesID = selectedSeriesIdList(i)
                        curveIndex = i
                    End If
                Next

                selectedSeriesIdList.Remove(selectedSeriesIdList(curveIndex))
                If (selectedSeriesIdList.Count = 0) Or (selectedSeriesIdList.Count = 1) Then
                    'Clear the graph and repolt the whole graph
                    Summary.ClearStatistics()
                    timeSeriesPlot.Remove(0)
                    probabilityPlot.Remove(0)
                    timeSeriesPlot.Clear()
                    boxWhisker.Clear()
                    histogramPlot.Clear()
                    probabilityPlot.Clear()
                    colorcount = 0
                    StartDateLimit = Today.AddYears(-150)
                    EndDateLimit = Today
                Else
                    timeSeriesPlot.Remove(removedSeriesID)
                    probabilityPlot.Remove(removedSeriesID)
                    boxWhisker.Remove(removedSeriesID)
                    histogramPlot.Remove(removedSeriesID)
                    If IsDisplayFullDate Then
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

            Dim seriesPlotInfo = New SeriesPlotInfo(_seriesMenu.CheckedIDList, _seriesMenu.SiteDisplayColumn, plotOptionsControl.Options, StartDateTime, EndDateTime)

            dataSummary.Plot(seriesPlotInfo)

            timeSeriesPlot.Refreshing()
            probabilityPlot.Refreshing()
            histogramPlot.Refreshing()
            boxWhisker.Refreshing()

            ProgressBar.Visible = False

        End Sub

        Private Function GetTimeSeriesPlotOptions(ByVal seriesID As Integer) As OneSeriesPlotInfo
            Return OneSeriesPlotInfo.Create(seriesID, StartDateTime, EndDateTime, _seriesMenu.SiteDisplayColumn, plotOptionsControl.Options)
        End Function

        Private Sub PlotGraps(ByVal seriesID As Int32)

            'Date Range setting
            DateRangeSelection(seriesID)

            'get data
            Dim timeSeriesOptions = GetTimeSeriesPlotOptions(seriesID)

            'Set different color to each curve if the color option is not selected
            ColorChooser(timeSeriesOptions.PlotOptions)

            Summary.GetStatistics(timeSeriesOptions.DataTable, timeSeriesOptions.PlotOptions)
            If Summary.Statistic_NumberOfObservations > Summary.Statistic_NumberOfCensoredObservations Then

                timeSeriesPlot.Plot(timeSeriesOptions)
                If ProgressBar.Value < ProgressBar.Maximum Then ProgressBar.Value += 1

                boxWhisker.Plot(timeSeriesOptions, Summary.Statistic_StandardDeviation)
                If ProgressBar.Value < ProgressBar.Maximum Then ProgressBar.Value += 1

                probabilityPlot.Plot(timeSeriesOptions, Summary.Statistic_StandardDeviation)
                If ProgressBar.Value < ProgressBar.Maximum Then ProgressBar.Value += 1

                histogramPlot.Plot(timeSeriesOptions, Summary.Statistic_StandardDeviation)
                If ProgressBar.Value < ProgressBar.Maximum Then ProgressBar.Value += 1

            ElseIf Summary.Statistic_NumberOfObservations = Summary.Statistic_NumberOfCensoredObservations Then

                Const ALL_CENSORED As String = "All data is censored, so there is no data do display"

                If timeSeriesPlot.CurveCount = 0 Then timeSeriesPlot.SetGraphPaneTitle(ALL_CENSORED)
                If boxWhisker.CurveCount = 0 Then boxWhisker.SetGraphPaneTitle(ALL_CENSORED)
                If probabilityPlot.CurveCount = 0 Then probabilityPlot.SetGraphPaneTitle(ALL_CENSORED)
                If histogramPlot.CurveCount = 0 Then histogramPlot.SetGraphPaneTitle(ALL_CENSORED)

            End If

        End Sub

        Public Sub ApplyOptions()

            Dim seriesPlotInfo = New SeriesPlotInfo(_seriesMenu.CheckedIDList, _seriesMenu.SiteDisplayColumn, plotOptionsControl.Options, StartDateTime, EndDateTime)

            'progress bar setting
            ProgressBar.Visible = True
            ProgressBar.Maximum = selectedSeriesIdList.Count * 10
            ProgressBar.Minimum = 0
            ProgressBar.Value = 0
            colorcount = 0

            'Clear the graph and plot it again
            Summary.ClearStatistics()
            timeSeriesPlot.Clear()
            boxWhisker.Clear()
            histogramPlot.Clear()
            probabilityPlot.Clear()

            'Ploting the Time Series graph and Probability graph
            For Each s As Integer In selectedSeriesIdList
                PlotGraps(s)
                colorcount += 1
            Next

            dataSummary.Plot(seriesPlotInfo)
            timeSeriesPlot.Refreshing()
            probabilityPlot.Refreshing()
            histogramPlot.Refreshing()
            boxWhisker.Refreshing()

            ProgressBar.Visible = False
        End Sub

        Public Sub ShowPointValues(ByVal showPointValues As Boolean)
            DirectCast(timeSeriesPlot, IChart).ShowPointValues = showPointValues
            DirectCast(probabilityPlot, IChart).ShowPointValues = showPointValues
            DirectCast(histogramPlot, IChart).ShowPointValues = showPointValues
            DirectCast(boxWhisker, IChart).ShowPointValues = showPointValues
        End Sub

        Public Sub UndoZoom()
            DirectCast(timeSeriesPlot, IChart).ZoomOutAll()
            DirectCast(probabilityPlot, IChart).ZoomOutAll()
            DirectCast(histogramPlot, IChart).ZoomOutAll()
            DirectCast(boxWhisker, IChart).ZoomOutAll()
        End Sub

        Public Sub ZoomIn()
            DirectCast(timeSeriesPlot, IChart).ZoomIn()
            DirectCast(probabilityPlot, IChart).ZoomIn()
            DirectCast(histogramPlot, IChart).ZoomIn()
            DirectCast(boxWhisker, IChart).ZoomIn()
        End Sub

        Public Sub ZoomOut()
            DirectCast(timeSeriesPlot, IChart).ZoomOut()
            DirectCast(probabilityPlot, IChart).ZoomOut()
            DirectCast(histogramPlot, IChart).ZoomOut()
            DirectCast(boxWhisker, IChart).ZoomOut()
        End Sub

        Private Sub ColorChooser(ByVal options As PlotOptions)
            options.GetPointColor = pointcolorlist(colorcount Mod 10)
            options.GetLineColor = linecolorlist(colorcount Mod 10)
        End Sub

        Private Sub DateRangeSelection(ByVal serieID As Integer)
            Dim repository = RepositoryFactory.Instance.Get(Of IDataSeriesRepository)()
            Dim series = repository.GetByKey(serieID)
            If series = Nothing Then Return

            Dim bDateTime = series.BeginDateTime
            Dim eDateTime = series.EndDateTime

            If StartDateLimit > bDateTime Or StartDateLimit = Today.AddYears(-150) Then
                StartDateLimit = bDateTime
            End If
            If EndDateLimit < eDateTime Or EndDateLimit = Today Then
                EndDateLimit = eDateTime
            End If

            If IsDisplayFullDate Then
                StartDateTime = StartDateLimit
                EndDateTime = EndDateLimit
            End If

        End Sub

        Private Sub ResetDateRange()
            StartDateLimit = Today.AddYears(-150)
            EndDateLimit = Today
            For i As Integer = 0 To selectedSeriesIdList.Count - 1
                DateRangeSelection(i)
            Next
        End Sub
#End Region

    End Class

End Namespace