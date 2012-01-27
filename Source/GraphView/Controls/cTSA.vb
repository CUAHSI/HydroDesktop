Imports HydroDesktop.Database
Imports HydroDesktop.Interfaces
Imports HydroDesktop.Configuration
Imports Controls

Public Class cTSA

    'Public rlblStratDate As New RibbonLabel
    'Public rlblEndDate As New RibbonLabel

    'Inherits Windows.Forms.UserControl
    Private Const db_outFld_ValDTMonth As String = "DateMonth"
    Private Const db_outFld_ValDTYear As String = "DateYear"
    Private Const db_outFld_ValDTDay As String = "DateDay"

    Private Summary As New cSummaryStatistics

    'Private colorlist As New ArrayList()
    Public linecolorlist As New ArrayList()
    Public pointcolorlist As New ArrayList()
    Private colorcount As Integer = 0
    'the list of the series which is selected
    'Private selectedSeriesId As Integer = 0
    Private ReadOnly selectedSeriesIdList As New ArrayList()
    Private ccList0 As New ArrayList()
    Private _seriesMenu As ISeriesSelector


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

    Private Sub RaiseDatesChanged()
        RaiseEvent DatesChanged(Me, EventArgs.Empty)
    End Sub

    Public Event DatesChanged As EventHandler

    Public IsDisplayFullDate As Boolean = True

    Public Sub RefreshView()
        'SeriesSelector.RefreshSelection()
        'SeriesSelector.JudgeSelectOption()
    End Sub

    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.

        SetColorCollections()
        selectedSeriesIdList.Clear()
        pTimeSeries.Clear()

        StartDateLimit = Today.AddYears(-150)
        EndDateLimit = Today

    End Sub


    Public Sub New(ByVal seriesSelector As ISeriesSelector)

        ' This call is required by the Windows Form Designer.
        InitializeComponent()

        'set the 'args' private variable
        _seriesMenu = seriesSelector

        'assign the events
        AddHandler _seriesMenu.SeriesCheck, AddressOf SeriesSelector_SeriesCheck
        AddHandler _seriesMenu.Refreshed, AddressOf SeriesSelector_Refreshed
        'AddHandler _seriesMenu.SeriesView.SeriesSelector.SeriesSelected, AddressOf SeriesSelector_SeriesSelected

        SetColorCollections()
        selectedSeriesIdList.Clear()
        pTimeSeries.Clear()

        StartDateLimit = Today.AddYears(-150)
        EndDateLimit = Today

    End Sub

    'Public Sub initialize()

    '    'selectedSeriesIdList.Clear()
    '    ''Clear the graph and repolt the whole graph
    '    'Summary.ClearStatistics()
    '    'pDataSummary.ClearStatTables()
    '    ''pSummaryPlot.Clear()
    '    'pTimeSeries.Clear()
    '    'pBoxWhisker.Clear()
    '    'pHistogram.Clear()
    '    'pProbability.Clear()
    '    'colorcount = 0

    '    'Setting if multiplecheck is allowed                                                                                                                                    
    '    'SeriesSelector.MultipleCheck = True
    '    'SeriesSelector.CheckOnClick = True

    '    'Setting of the color collections
    '    SetColorCollections()
    '    selectedSeriesIdList.Clear()
    '    pTimeSeries.Clear()

    '    'CPlotOptions1.gboxTSPlotOptions.Visible = True
    '    'CPlotOptions1.gboxBoxPlotOptions.Visible = False
    '    'CPlotOptions1.gboxHistPlotOptions.Visible = False

    '    StartDateLimit = Today.AddYears(-150)
    '    EndDateLimit = Today

    '    'SeriesSelector.RefreshSelection()

    'End Sub

    Private Sub SetColorCollections()
        'Setting of color collections
        ccList0.Clear()
        ccList0.Add(System.Drawing.Color.FromArgb(106, 61, 154))
        ccList0.Add(System.Drawing.Color.FromArgb(202, 178, 214))
        ccList0.Add(System.Drawing.Color.FromArgb(255, 127, 0))
        ccList0.Add(System.Drawing.Color.FromArgb(253, 191, 111))
        ccList0.Add(System.Drawing.Color.FromArgb(227, 26, 28))
        ccList0.Add(System.Drawing.Color.FromArgb(251, 154, 153))
        ccList0.Add(System.Drawing.Color.FromArgb(51, 160, 44))
        ccList0.Add(System.Drawing.Color.FromArgb(178, 223, 138))
        ccList0.Add(System.Drawing.Color.FromArgb(31, 120, 180))
        ccList0.Add(System.Drawing.Color.FromArgb(166, 206, 227))

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
      
        Dim count As Integer = 0
        Dim SeriesSelector = _seriesMenu
        Dim curveIndex As Integer = 0
        Dim removedSeriesID As Integer = 0
        Dim CheckedSeriesState As Boolean = False

        If Not (selectedSeriesIdList.Contains(SeriesSelector.SelectedSeriesID)) Then
            selectedSeriesIdList.Add(SeriesSelector.SelectedSeriesID)
            CheckedSeriesState = True
        Else
            For i As Integer = 0 To selectedSeriesIdList.Count - 1
                If Not SeriesSelector.CheckedIDList.Contains(selectedSeriesIdList(i)) Then
                    removedSeriesID = selectedSeriesIdList(i)
                    curveIndex = i
                End If
            Next

            selectedSeriesIdList.Remove(selectedSeriesIdList(curveIndex))
            If (selectedSeriesIdList.Count = 0) Or (selectedSeriesIdList.Count = 1) Then
                'Clear the graph and repolt the whole graph
                Summary.ClearStatistics()
                pDataSummary.ClearStatTables()
                'pSummaryPlot.Clear()
                pTimeSeries.Remove(0)
                pProbability.Remove(0)
                pTimeSeries.Clear()
                pBoxWhisker.Clear()
                pHistogram.Clear()
                pProbability.Clear()
                colorcount = 0
                count = 0
                'rlblStratDate.Text = "Start Date: "
                'rlblEndDate.Text = "End Date:   "
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


        If (CheckedSeriesState = True) Or (selectedSeriesIdList.Count = 1) Then 'And (CPlotOptions1.dtpStartDatePicker.Value = lastStartDate) And (CPlotOptions1.dtpEndDatePicker.Value = lastEndDate) Then

            count += selectedSeriesIdList.Cast(Of Integer)().Count()

            'progress bar setting
            ProgressBar.Visible = True
            ProgressBar.Maximum = 11
            ProgressBar.Minimum = 0
            ProgressBar.Value = 0

            Dim seriesID = selectedSeriesIdList(count - 1)

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
                ProgressBar.Value += 1
                'pTimeSeries.zgTimeSeries.GraphPane.Title.Text = strStartDate
                pBoxWhisker.Plot(timeSeriesOptions, Summary.Statistic_StandardDeviation)
                ProgressBar.Value += 1
                pProbability.Plot(timeSeriesOptions, Summary.Statistic_StandardDeviation)
                ProgressBar.Value += 1
                pHistogram.Plot(timeSeriesOptions, Summary.Statistic_StandardDeviation)
                ProgressBar.Value += 1
                'pSummaryPlot.Plot(data, siteName, variableName, unitsName, options, Summary.Statistic_StandardDeviation)
            End If
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


    Public Sub ApplyOptions()

        Dim count As Integer = selectedSeriesIdList.Count

        'progress bar setting
        ProgressBar.Visible = True
        ProgressBar.Maximum = count * 10
        ProgressBar.Minimum = 0
        ProgressBar.Value = 0

        count = 0
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
            count += 1

            'setting Date Range
            DateRangeSelection(selectedSeriesIdList(count - 1))

            ' Get data
            Dim timeSeriesOptions = GetTimeSeriesPlotOptions(s)


            'Set different color to each curve if the color option is not selected
            ColorChooser(timeSeriesOptions.PlotOptions)


            If timeSeriesOptions.DataTable.Rows.Count > 0 Then
                Summary.GetStatistics(timeSeriesOptions.DataTable, timeSeriesOptions.PlotOptions)
                pDataSummary.CreateStatTable(timeSeriesOptions.SiteName, timeSeriesOptions.VariableName, s, timeSeriesOptions.DataTable, timeSeriesOptions.PlotOptions)
                If Summary.Statistic_NumberOfObservations > Summary.Statistic_NumberOfCensoredObservations Then
                    'pSummaryPlot.Plot(data, siteName, variableName, unitsName, options, Summary.Statistic_StandardDeviation)
                    pTimeSeries.Plot(timeSeriesOptions)
                    If ProgressBar.Value < 100 Then ProgressBar.Value += 1
                    pProbability.Plot(timeSeriesOptions, Summary.Statistic_StandardDeviation)
                    If ProgressBar.Value < 100 Then ProgressBar.Value += 1
                    pBoxWhisker.Plot(timeSeriesOptions, Summary.Statistic_StandardDeviation)
                    If ProgressBar.Value < 100 Then ProgressBar.Value += 1
                    pHistogram.Plot(timeSeriesOptions, Summary.Statistic_StandardDeviation)
                    If ProgressBar.Value < 100 Then ProgressBar.Value += 1
                End If
            End If
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
        Dim _BeginDateTime As DateTime
        Dim _EndDateTime As DateTime
        Dim connString = Settings.Instance.DataRepositoryConnectionString
        Dim dbTools As New DbOperations(connString, DatabaseTypes.SQLite)

        'check seriesID, if it exists
        Dim serieIDExists As Object = dbTools.ExecuteSingleOutput("SELECT SeriesID FROM DataSeries WHERe SeriesID = " & serieID)
        If serieIDExists = Nothing Then Return

        _BeginDateTime = dbTools.ExecuteSingleOutput("SELECT BeginDateTime FROM DataSeries WHERE (SeriesID = '" & serieID & "')")
        _EndDateTime = dbTools.ExecuteSingleOutput("SELECT EndDateTime FROM DataSeries WHERE (SeriesID = '" & serieID & "')")

        If StartDateLimit > _BeginDateTime Or StartDateLimit = Today.AddYears(-150) Then
            StartDateLimit = _BeginDateTime
        End If
        If EndDateLimit < _EndDateTime Or EndDateLimit = Today Then
            EndDateLimit = _EndDateTime
        End If

        If IsDisplayFullDate Then
            StartDateTime = StartDateLimit
            Me.EndDateTime = EndDateLimit
            'rlblStratDate.Text = "Start Date: " + StartDateTime.ToString
            'rlblEndDate.Text = "End Date:   " + EndDateTime.ToString
        End If

    End Sub

    Private Sub ResetDateRange()
        StartDateLimit = Today.AddYears(-150)
        EndDateLimit = Today
        Dim serieIDExists As Object
        Dim _BeginDateTime As DateTime
        Dim _EndDateTime As DateTime
        Dim connString = Settings.Instance.DataRepositoryConnectionString
        Dim dbTools As New DbOperations(connString, DatabaseTypes.SQLite)

        For i As Integer = 0 To selectedSeriesIdList.Count - 1
            serieIDExists = dbTools.ExecuteSingleOutput("SELECT SeriesID FROM DataSeries WHERe SeriesID = " & selectedSeriesIdList(i))
            If serieIDExists = Nothing Then Return

            _BeginDateTime = dbTools.ExecuteSingleOutput("SELECT BeginDateTime FROM DataSeries WHERE (SeriesID = '" & selectedSeriesIdList(i) & "')")
            _EndDateTime = dbTools.ExecuteSingleOutput("SELECT EndDateTime FROM DataSeries WHERE (SeriesID = '" & selectedSeriesIdList(i) & "')")

            If StartDateLimit > _BeginDateTime Or StartDateLimit = Today.AddYears(-150) Then
                StartDateLimit = _BeginDateTime
            End If
            If EndDateLimit < _EndDateTime Or EndDateLimit = Today Then
                EndDateLimit = _EndDateTime
            End If

            If IsDisplayFullDate Then
                StartDateTime = StartDateLimit
                Me.EndDateTime = EndDateLimit
                'rlblStratDate.Text = "Start Date: " + StartDateTime.ToString
                'rlblEndDate.Text = "End Date:   " + EndDateTime.ToString
            End If
        Next
    End Sub


End Class
