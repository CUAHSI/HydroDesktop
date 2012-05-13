Imports System.Drawing

Public Class PlotOptions
    Public Event DatesChanged As EventHandler

    Public Property TimeSeriesMethod() As TimeSeriesType = TimeSeriesType.Both
    Public Property HistTypeMethod() As HistogramType = HistogramType.Count
    Public Property HistAlgorothmsMethod() As HistorgramAlgorithms = HistorgramAlgorithms.Sturges
    Public Property numBins() As Integer
    Public Property binWidth() As Double
    Public Property xMax() As Double
    Public Property xMin() As Double
    Public Property yMax() As Double
    Public Property yMin() As Double
    Public Property xMajor() As Double
    Public Property BoxWhiskerMethod() As BoxWhiskerType = BoxWhiskerType.Monthly
    Public Property IsPlotCensored() As Boolean = True
    Public Property GetLineColor() As Color = Color.Black
    Public Property GetPointColor() As Color = Color.Black
    Public Property LineColorList() As Integer
    Public Property PointColorList() As Integer
    Public Property ShowLegend() As Boolean = True
    Public Property UseCensoredData() As Boolean = False
    Public Property DisplayFullDate As Boolean = True


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

End Class

Public Enum TimeSeriesType
    Both
    Line
    Point
    None
End Enum

Public Enum BoxWhiskerType
    Monthly
    Seasonal
    Yearly
    Overall
End Enum

Public Enum HistogramType
    Count
    Probability
    Relative
End Enum

Public Enum HistorgramAlgorithms
    Sturges
    Scott
    Freedman
End Enum
