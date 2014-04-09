Public Class PlotOptions
    Public Enum TimeSeriesType
        Both
        Line
        Point
        None
    End Enum
#Region " Member Variables "
    Private _TimeSeriesMethod As TimeSeriesType
    Private ReadOnly _IsPlotCensored As Boolean
    Private _GetLineColor As Drawing.Color
    Private _GetPointColor As Drawing.Color
    Private ReadOnly _ShowLegend As Boolean
    Private ReadOnly _UseCensoredData As Boolean
#End Region

    Public Sub New(ByVal TimeSeriesMethod As TimeSeriesType, ByVal GetLineColor As Drawing.Color, _
                   ByVal GetPointColor As Drawing.Color, ByVal ShowLegend As Boolean, _
                   ByVal UseCensoredData As Boolean, Optional ByVal IsPlotCensored As Boolean = True)
        _TimeSeriesMethod = TimeSeriesMethod
        _IsPlotCensored = IsPlotCensored
        _GetLineColor = GetLineColor
        _GetPointColor = GetPointColor
        _ShowLegend = ShowLegend
        _UseCensoredData = UseCensoredData

    End Sub

    Public Property TimeSeriesMethod() As TimeSeriesType
        Get
            Return _TimeSeriesMethod
        End Get
        Set(ByVal value As TimeSeriesType)
            _TimeSeriesMethod = value
        End Set
    End Property
    Public ReadOnly Property IsPlotCensored() As Boolean
        Get
            Return _IsPlotCensored
        End Get
    End Property
    Public Property GetLineColor() As Drawing.Color
        Get
            Return _GetLineColor
        End Get
        Set(ByVal value As Drawing.Color)
            _GetLineColor = value
        End Set
    End Property
    Public Property GetPointColor() As Drawing.Color
        Get
            Return _GetPointColor
        End Get
        Set(ByVal value As Drawing.Color)
            _GetPointColor = value
        End Set
    End Property
    Public ReadOnly Property ShowLegend() As Boolean
        Get
            Return _ShowLegend
        End Get
    End Property
    Public ReadOnly Property UseCensoredData() As Boolean
        Get
            Return _UseCensoredData
        End Get
    End Property
End Class
