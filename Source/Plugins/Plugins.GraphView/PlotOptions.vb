Imports System.Drawing

Public Class PlotOptions
    Public Event DatesChanged As EventHandler

    Public Sub New()
        SetColorCollections()
    End Sub

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
    Public Property LineColorList() As New List(Of Color)
    Public Property PointColorList() As New List(Of Color)
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


    Private Sub SetColorCollections()
        Dim colors() As Color = {Color.FromArgb(106, 61, 154),
                                 Color.FromArgb(202, 178, 214),
                                 Color.FromArgb(255, 127, 0),
                                 Color.FromArgb(253, 191, 111),
                                 Color.FromArgb(227, 26, 28),
                                 Color.FromArgb(251, 154, 153),
                                 Color.FromArgb(51, 160, 44),
                                 Color.FromArgb(178, 223, 138),
                                 Color.FromArgb(31, 120, 180),
                                 Color.FromArgb(166, 206, 227)}

        For Each color In colors
            LineColorList.Add(color)
            PointColorList.Add(color)
        Next
    End Sub

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
