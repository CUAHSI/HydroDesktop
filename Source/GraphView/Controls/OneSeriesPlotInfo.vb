Option Strict On

Imports HydroDesktop.Database
Imports HydroDesktop.Interfaces

Namespace Controls
    Public Class SeriesPlotInfo
        Private ReadOnly _seriesIDs As IList(Of Integer)
        Private ReadOnly _siteDisplayColumn As String
        Private ReadOnly _plotOptions As PlotOptions
        Private ReadOnly _startDateTime As Date
        Private ReadOnly _endDateTime As Date
        Private _seriesInfos As ICollection(Of OneSeriesPlotInfo)

        Sub New(ByRef seriesIDs As IList(Of Int32), ByRef siteDisplayColumn As String, ByRef plotOptions As PlotOptions, ByVal startDateTime As DateTime, ByVal endDateTime As DateTime)

            _seriesIDs = seriesIDs
            _siteDisplayColumn = siteDisplayColumn
            _plotOptions = plotOptions
            _startDateTime = startDateTime
            _endDateTime = endDateTime
        End Sub

        Public ReadOnly Property PlotOptions() As PlotOptions
            Get
                Return _plotOptions
            End Get
        End Property

        Public Function GetSeriesInfo() As ICollection(Of OneSeriesPlotInfo)
            If (Not (_seriesInfos Is Nothing)) Then
                Return _seriesInfos
            End If

            _seriesInfos = New List(Of OneSeriesPlotInfo)(_seriesIDs.Count)
            For Each seriesId In _seriesIDs
                _seriesInfos.Add(OneSeriesPlotInfo.Create(seriesId, _startDateTime, _endDateTime, _siteDisplayColumn, _plotOptions))
            Next
            Return _seriesInfos
        End Function
    End Class

    Public Class OneSeriesPlotInfo
        Public Property DataTable As DataTable
        Public Property SiteName As String
        Public Property VariableName As String
        Public Property DataType As String
        Public Property VariableUnits As String
        Public Property PlotOptions As PlotOptions
        Public Property SeriesID As Integer

        Public Shared Function Create(ByVal seriesID As Integer, ByVal startDateTime As DateTime, ByVal endDateTime As DateTime, ByVal siteDisplayColumn As String, ByVal plotOptions As PlotOptions) As OneSeriesPlotInfo
            Dim dataSeriesRepo = RepositoryFactory.Instance.Get(Of IDataSeriesRepository)()
            Dim dataValuesRepo = RepositoryFactory.Instance.Get(Of IDataValuesRepository)()
            Dim series = dataSeriesRepo.GetByKey(seriesID)

            Dim strStartDate = startDateTime
            Dim strEndDate = endDateTime.AddDays(1).AddMilliseconds(-1)

            Dim nodatavalue = series.Variable.NoDataValue
            Dim data = dataValuesRepo.GetTableForGraphView(seriesID, nodatavalue, strStartDate, strEndDate)
            Dim variableName = series.Variable.Name
            Dim unitsName = series.Variable.VariableUnit.Name
            Dim siteName = If(siteDisplayColumn = "SiteName", series.Site.Name, series.Site.Code)
            Dim dataType = series.Variable.DataType

            Dim timeSeriesOptions = New OneSeriesPlotInfo
            timeSeriesOptions.DataTable = data
            timeSeriesOptions.DataType = dataType
            timeSeriesOptions.PlotOptions = plotOptions
            timeSeriesOptions.SeriesID = seriesID
            timeSeriesOptions.SiteName = siteName
            timeSeriesOptions.VariableName = variableName
            timeSeriesOptions.VariableUnits = unitsName

            Return timeSeriesOptions
        End Function
    End Class
End Namespace