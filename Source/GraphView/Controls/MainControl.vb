Imports HydroDesktop.Database
Imports HydroDesktop.Interfaces
Imports HydroDesktop.Configuration
Imports System.Drawing

Namespace Controls

    Public Class MainControl

#Region "Fields"

        Private ReadOnly Summary As New cSummaryStatistics
        Public linecolorlist As New List(Of Color)
        Public pointcolorlist As New List(Of Color)
        Private ReadOnly ccList0 As New List(Of Color)
        Private colorcount As Integer = 0
        Private ReadOnly selectedSeriesIdList As New List(Of Int32) 'the list of the series which is selected
        Private ReadOnly _seriesMenu As ISeriesSelector
        Public Event DatesChanged As EventHandler
        Public IsDisplayFullDate As Boolean = True

#End Region

#Region "Constructors"
        Public Sub New(Optional ByVal seriesSelector As ISeriesSelector = Nothing)
            ' This call is required by the Windows Form Designer.
            InitializeComponent()

            _seriesMenu = seriesSelector

            'assign the events
            If Not _seriesMenu Is Nothing Then
                AddHandler _seriesMenu.SeriesCheck, AddressOf SeriesSelector_SeriesCheck
                AddHandler _seriesMenu.Refreshed, AddressOf SeriesSelector_Refreshed
            End If

            pProbability.SeriesSelector = seriesSelector
            pTimeSeries.SeriesSelector = seriesSelector

            SetColorCollections()
            selectedSeriesIdList.Clear()
            pTimeSeries.Clear()

            StartDateLimit = Today.AddYears(-150)
            EndDateLimit = Today
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

        Private Sub SeriesSelector_Refreshed()
            pTimeSeries.Clear()
            pBoxWhisker.Clear()
            pProbability.Clear()
            selectedSeriesIdList.Clear()

        End Sub

        'when a series is checked in the series selector control
        Private Sub SeriesSelector_SeriesCheck() 'ByVal sender As Object, ByVal e As System.Windows.Forms.ItemCheckEventArgs)

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
                    pDataSummary.ClearStatTables()
                    pTimeSeries.Remove(0)
                    pProbability.Remove(0)
                    pTimeSeries.Clear()
                    pBoxWhisker.Clear()
                    pHistogram.Clear()
                    pProbability.Clear()
                    colorcount = 0
                    StartDateLimit = Today.AddYears(-150)
                    EndDateLimit = Today
                Else
                    pTimeSeries.Remove(removedSeriesID)
                    pProbability.Remove(removedSeriesID)
                    pBoxWhisker.Remove(removedSeriesID)
                    pHistogram.Remove(removedSeriesID)
                    pDataSummary.RemoveStatTable(removedSeriesID)
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

            pDataSummary.StatTableStyling()
            pTimeSeries.Refreshing()
            pProbability.Refreshing()
            pHistogram.Refreshing()
            pBoxWhisker.Refreshing()

            ProgressBar.Visible = False

        End Sub

        Private Function GetTimeSeriesPlotOptions(ByVal seriesID As Integer) As TimeSeriesPlotOptions
            Dim connString = Settings.Instance.DataRepositoryConnectionString
            Dim dbTools As New DbOperations(connString, DatabaseTypes.SQLite)

            Dim strStartDate = StartDateTime.ToString("yyyy-MM-dd HH:mm:ss")
            Dim strEndDate = EndDateTime.AddDays(1).AddMilliseconds(-1).ToString("yyyy-MM-dd HH:mm:ss")
            ProgressBar.Value += 1

            Dim nodatavalue = dbTools.ExecuteSingleOutput("SELECT NoDataValue FROM DataSeries LEFT JOIN Variables ON DataSeries.VariableID = Variables.VariableID WHERE (SeriesID = '" & seriesID & "')")
            ProgressBar.Value += 1
            Dim data = dbTools.LoadTable("DataValues", "SELECT DataValue, LocalDateTime, CensorCode, strftime('%m', LocalDateTime) as DateMonth, strftime('%Y', LocalDateTime) as DateYear FROM DataValues WHERE (SeriesID = '" & seriesID & "') AND (DataValue <> '" & nodatavalue & "') AND (LocalDateTime between '" & strStartDate & "' AND '" & strEndDate & "')  ORDER BY LocalDateTime")
            ProgressBar.Value += 1
            Dim variableName = dbTools.ExecuteSingleOutput("SELECT VariableName FROM DataSeries LEFT JOIN Variables ON Variables.VariableID = DataSeries.VariableID WHERE SeriesID = '" & seriesID & "'")
            ProgressBar.Value += 1
            Dim unitsName = dbTools.ExecuteSingleOutput("SELECT UnitsName FROM DataSeries LEFT JOIN Variables ON Variables.VariableID = DataSeries.VariableID LEFT JOIN Units ON Variables.VariableUnitsID = Units.UnitsID WHERE SeriesID = '" & seriesID & "'")
            ProgressBar.Value += 1
            Dim siteName = dbTools.ExecuteSingleOutput("SELECT " & _seriesMenu.SiteDisplayColumn & " FROM DataSeries LEFT JOIN Sites ON Sites.SiteID = DataSeries.SiteID WHERE SeriesID = '" & seriesID & "'")
            ProgressBar.Value += 1
            Dim dataType = dbTools.ExecuteSingleOutput("SELECT DataType FROM DataSeries LEFT JOIN Variables ON Variables.VariableID = DataSeries.VariableID WHERE SeriesID = '" & seriesID & "'")
            ProgressBar.Value += 1

            Dim options = CPlotOptions1.Options

            Dim timeSeriesOptions = New TimeSeriesPlotOptions
            timeSeriesOptions.DataTable = data
            timeSeriesOptions.DataType = dataType
            timeSeriesOptions.PlotOptions = options
            timeSeriesOptions.SeriesID = seriesID
            timeSeriesOptions.SiteName = siteName
            timeSeriesOptions.VariableName = variableName
            timeSeriesOptions.VariableUnits = unitsName

            Return timeSeriesOptions
        End Function
        
        Private Sub PlotGraps(ByVal seriesID As Int32)

            'Date Range setting
            DateRangeSelection(seriesID)

            'get data
            Dim timeSeriesOptions = GetTimeSeriesPlotOptions(seriesID)

            'Set different color to each curve if the color option is not selected
            ColorChooser(timeSeriesOptions.PlotOptions)

            Summary.GetStatistics(timeSeriesOptions.DataTable, timeSeriesOptions.PlotOptions)
            pDataSummary.CreateStatTable(timeSeriesOptions.SiteName, timeSeriesOptions.VariableName, seriesID, timeSeriesOptions.DataTable, timeSeriesOptions.PlotOptions)
            pDataSummary.StatTableStyling()

            If Summary.Statistic_NumberOfObservations > Summary.Statistic_NumberOfCensoredObservations Then

                pTimeSeries.Plot(timeSeriesOptions)
                If ProgressBar.Value < ProgressBar.Maximum Then ProgressBar.Value += 1

                pBoxWhisker.Plot(timeSeriesOptions, Summary.Statistic_StandardDeviation)
                If ProgressBar.Value < ProgressBar.Maximum Then ProgressBar.Value += 1

                pProbability.Plot(timeSeriesOptions, Summary.Statistic_StandardDeviation)
                If ProgressBar.Value < ProgressBar.Maximum Then ProgressBar.Value += 1

                pHistogram.Plot(timeSeriesOptions, Summary.Statistic_StandardDeviation)
                If ProgressBar.Value < ProgressBar.Maximum Then ProgressBar.Value += 1

            ElseIf Summary.Statistic_NumberOfObservations = Summary.Statistic_NumberOfCensoredObservations Then

                Const ALL_CENSORED As String = "All data is censored, so there is no data do display"

                If pTimeSeries.CurveCount = 0 Then pTimeSeries.SetGraphPaneTitle(ALL_CENSORED)
                If pBoxWhisker.CurveCount = 0 Then pBoxWhisker.SetGraphPaneTitle(ALL_CENSORED)
                If pProbability.CurveCount = 0 Then pProbability.SetGraphPaneTitle(ALL_CENSORED)
                If pHistogram.CurveCount = 0 Then pHistogram.SetGraphPaneTitle(ALL_CENSORED)

            End If

        End Sub

        Public Sub ApplyOptions()

            'progress bar setting
            ProgressBar.Visible = True
            ProgressBar.Maximum = selectedSeriesIdList.Count * 10
            ProgressBar.Minimum = 0
            ProgressBar.Value = 0
            colorcount = 0

            'Clear the graph and plot it again
            Summary.ClearStatistics()
            pDataSummary.ClearStatTables()
            pTimeSeries.Clear()
            pBoxWhisker.Clear()
            pHistogram.Clear()
            pProbability.Clear()

            'Ploting the Time Series graph and Probability graph
            For Each s As Integer In selectedSeriesIdList
                PlotGraps(s)
                colorcount += 1
            Next

            pDataSummary.StatTableStyling()
            pTimeSeries.Refreshing()
            pProbability.Refreshing()
            pHistogram.Refreshing()
            pBoxWhisker.Refreshing()

            ProgressBar.Visible = False
        End Sub

        Public Sub ShowPointValues(ByVal showPointValues As Boolean)
            DirectCast(pTimeSeries, IChart).ShowPointValues = showPointValues
            DirectCast(pProbability, IChart).ShowPointValues = showPointValues
            DirectCast(pHistogram, IChart).ShowPointValues = showPointValues
            DirectCast(pBoxWhisker, IChart).ShowPointValues = showPointValues
        End Sub

        Public Sub UndoZoom()
            DirectCast(pTimeSeries, IChart).ZoomOutAll()
            DirectCast(pProbability, IChart).ZoomOutAll()
            DirectCast(pHistogram, IChart).ZoomOutAll()
            DirectCast(pBoxWhisker, IChart).ZoomOutAll()
        End Sub

        Public Sub ZoomIn()
            DirectCast(pTimeSeries, IChart).ZoomIn()
            DirectCast(pProbability, IChart).ZoomIn()
            DirectCast(pHistogram, IChart).ZoomIn()
            DirectCast(pBoxWhisker, IChart).ZoomIn()
        End Sub

        Public Sub ZoomOut()
            DirectCast(pTimeSeries, IChart).ZoomOut()
            DirectCast(pProbability, IChart).ZoomOut()
            DirectCast(pHistogram, IChart).ZoomOut()
            DirectCast(pBoxWhisker, IChart).ZoomOut()
        End Sub

        Private Sub ColorChooser(ByVal options As PlotOptions)
            options.GetPointColor = pointcolorlist(colorcount Mod 10)
            options.GetLineColor = linecolorlist(colorcount Mod 10)
        End Sub

        Private Sub DateRangeSelection(ByVal serieID As Integer)
            Dim repository = RepositoryFactory.Instance.Get(Of IDataSeriesRepository)()
            Dim series = repository.GetSeriesByID(serieID)
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