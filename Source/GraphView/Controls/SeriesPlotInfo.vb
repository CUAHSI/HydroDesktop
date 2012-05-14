Option Strict On

Imports HydroDesktop.Database
Imports HydroDesktop.Interfaces

Namespace Controls
    Public Class SeriesPlotInfo

#Region "Fields"

        Private ReadOnly _siteDisplayColumn As String
        Private ReadOnly _plotOptions As PlotOptions
        Private ReadOnly _seriesInfos As IDictionary(Of Integer, OneSeriesPlotInfo)

#End Region

        Sub New(ByVal siteDisplayColumn As String, ByVal plotOptions As PlotOptions)
            _seriesInfos = New Dictionary(Of Integer, OneSeriesPlotInfo)()
            _siteDisplayColumn = siteDisplayColumn
            _plotOptions = plotOptions
        End Sub

        Public Sub Update(ByVal e As SeriesEventArgs)
            If Not e.IsChecked Then
                _seriesInfos.Remove(e.SeriesID)
            Else
                _seriesInfos(e.SeriesID) = Nothing
            End If
        End Sub

        Public Sub Update()
            For Each key As Integer In _seriesInfos.Keys.ToList()
                _seriesInfos(key) = Nothing
            Next
        End Sub

        Public ReadOnly Property PlotOptions() As PlotOptions
            Get
                Return _plotOptions
            End Get
        End Property

        Public Function GetSeriesIDs() As ICollection(Of Integer)
            Return _seriesInfos.Keys
        End Function

        Public Function GetSeriesInfo() As ICollection(Of OneSeriesPlotInfo)
            
            Dim list = New List(Of OneSeriesPlotInfo)(_seriesInfos.Count)

            Dim dataSeriesRepo = RepositoryFactory.Instance.Get(Of IDataSeriesRepository)()
            Dim dataValuesRepo = RepositoryFactory.Instance.Get(Of IDataValuesRepository)()
            For Each key As Integer In _seriesInfos.Keys.ToList()

                Dim seriesInfo = _seriesInfos(key)
                If seriesInfo Is Nothing Then
                    seriesInfo = New OneSeriesPlotInfo(Me)
                    _seriesInfos(key) = seriesInfo

                    Dim seriesId = key
                    Dim series = dataSeriesRepo.GetByKey(seriesId)

                    Dim strStartDate = PlotOptions.StartDateTime
                    Dim strEndDate = PlotOptions.EndDateTime.AddDays(1).AddMilliseconds(-1)

                    Dim nodatavalue = series.Variable.NoDataValue
                    Dim data = dataValuesRepo.GetTableForGraphView(seriesId, nodatavalue, strStartDate, strEndDate)
                    Dim variableName = series.Variable.Name
                    Dim unitsName = series.Variable.VariableUnit.Name
                    Dim siteName = If(_siteDisplayColumn = "SiteName", series.Site.Name, series.Site.Code)
                    Dim dataType = series.Variable.DataType

                    seriesInfo.DataTable = data
                    seriesInfo.DataType = dataType
                    seriesInfo.SeriesID = seriesId
                    seriesInfo.SiteName = siteName
                    seriesInfo.VariableName = variableName
                    seriesInfo.VariableUnits = unitsName
                    seriesInfo.Statistics = SummaryStatistics.Create(data, PlotOptions.UseCensoredData)
                End If

                Dim i = list.Count
                seriesInfo.LineColor = _plotOptions.LineColorList(i Mod _plotOptions.LineColorList.Count)
                seriesInfo.PointColor = _plotOptions.PointColorList(i Mod _plotOptions.PointColorList.Count)
                list.Add(seriesInfo)
            Next
            Return list
        End Function

    End Class
End NameSpace