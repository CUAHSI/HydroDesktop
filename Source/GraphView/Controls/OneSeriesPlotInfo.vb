Option Strict On

Imports System.Drawing
Imports HydroDesktop.Database
Imports HydroDesktop.Interfaces

Namespace Controls
    Public Class SeriesPlotInfo
        Private ReadOnly _seriesIDs As IList(Of Integer)
        Private ReadOnly _siteDisplayColumn As String
        Private ReadOnly _plotOptions As PlotOptions
        Private _seriesInfos As ICollection(Of OneSeriesPlotInfo)

        Sub New(ByRef seriesIDs As IList(Of Int32), ByRef siteDisplayColumn As String, ByRef plotOptions As PlotOptions)

            _seriesIDs = seriesIDs
            _siteDisplayColumn = siteDisplayColumn
            _plotOptions = plotOptions
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

            Dim dataSeriesRepo = RepositoryFactory.Instance.Get(Of IDataSeriesRepository)()
            Dim dataValuesRepo = RepositoryFactory.Instance.Get(Of IDataValuesRepository)()
            For i As Integer = 0 To _seriesIDs.Count - 1
                Dim seriesId = _seriesIDs(i)
                Dim series = dataSeriesRepo.GetByKey(seriesId)
                If (series Is Nothing) Then Continue For

                Dim strStartDate = PlotOptions.StartDateTime
                Dim strEndDate = PlotOptions.EndDateTime.AddDays(1).AddMilliseconds(-1)

                Dim nodatavalue = series.Variable.NoDataValue
                Dim data = dataValuesRepo.GetTableForGraphView(seriesId, nodatavalue, strStartDate, strEndDate)
                Dim variableName = series.Variable.Name
                Dim unitsName = series.Variable.VariableUnit.Name
                Dim siteName = If(_siteDisplayColumn = "SiteName", series.Site.Name, series.Site.Code)
                Dim dataType = series.Variable.DataType

                Dim result = New OneSeriesPlotInfo
                result.DataTable = data
                result.DataType = dataType
                result.PlotOptions = PlotOptions
                result.SeriesID = seriesId
                result.SiteName = siteName
                result.VariableName = variableName
                result.VariableUnits = unitsName
                result.Statistics = SummaryStatistics.Create(data, PlotOptions.UseCensoredData)
                result.LineColor = _plotOptions.LineColorList(i Mod _plotOptions.LineColorList.Count)
                result.PointColor = _plotOptions.PointColorList(i Mod _plotOptions.PointColorList.Count)

                _seriesInfos.Add(result)
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
        Public Property LineColor As Color = Color.Black
        Public Property PointColor As Color = Color.Black
        Public Property Statistics() As SummaryStatistics
    End Class
End Namespace