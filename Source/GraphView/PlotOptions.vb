Public Class PlotOptions
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
#Region " Member Variables "
    Private ReadOnly _TimeSeriesMethod As TimeSeriesType
    'Private _IsBarsManual As Boolean
    'Private _NumberOfBars As Integer
    'Private _IsDiscreteBars As Boolean
    Private ReadOnly _BoxWhiskerMethod As BoxWhiskerType
    Private ReadOnly _IsPlotCensored As Boolean
    Private _GetLineColor As System.Drawing.Color
    Private _GetPointColor As System.Drawing.Color
    Private _LineColorList As Integer
    Private _PointColorList As Integer
    Private ReadOnly _StartDate As Date
    Private _EndDate As Date
    Private ReadOnly _ChangeDateRange As Boolean
    Private _ShowLegend As Boolean
    Private _UseCensoredData As Boolean


    Private _HistType As HistogramType
    Private _HistAlgorothms As HistorgramAlgorithms
    Private _NumBins As Integer
    Private _BinWidth As Double
    Private _xMax As Double
    Private _xMin As Double
    Private _yMax As Double
    Private _yMin As Double
    Private _xMajor As Double
    Private _yValue As List(Of Double)
    Private _xCenterList As List(Of Double)
    Private _xCenterListLog As List(Of Double)
    Private _lbin As List(Of Double)
    Private _rbin As List(Of Double)
#End Region

    Public Sub New(ByVal TimeSeriesMethod As TimeSeriesType, ByVal HistType As HistogramType, _
                   ByVal HistAlgorothms As HistorgramAlgorithms, ByVal NumBins As Integer, ByVal BinWidth As Double, _
                   ByVal xMax As Double, ByVal xMin As Double, ByVal yMax As Double, ByVal yMin As Double, ByVal xMajor As Double, _
                   ByVal BoxWhiskerMethod As BoxWhiskerType, ByVal GetLineColor As System.Drawing.Color, _
                   ByVal GetPointColor As System.Drawing.Color, _
                   ByVal ShowLegend As Boolean, ByVal StartDate As Date, ByVal EndDate As Date, ByVal ChangeDateRange As Boolean, _
                   ByVal UseCensoredData As Boolean, Optional ByVal IsPlotCensored As Boolean = True)
        _TimeSeriesMethod = TimeSeriesMethod
        '_IsBarsManual = IsBarsManual
        '_NumberOfBars = NumberOfBars
        '_IsDiscreteBars = IsDiscreteBars
        _BoxWhiskerMethod = BoxWhiskerMethod
        _IsPlotCensored = IsPlotCensored
        _GetLineColor = GetLineColor
        _GetPointColor = GetPointColor
        _LineColorList = LineColorList
        _PointColorList = PointColorList
        _StartDate = StartDate
        _EndDate = EndDate
        _ChangeDateRange = ChangeDateRange
        _ShowLegend = ShowLegend
        _UseCensoredData = UseCensoredData



        _HistType = HistType
        _HistAlgorothms = HistAlgorothms
        _NumBins = NumBins
        _BinWidth = BinWidth
        _xMax = xMax
        _xMin = xMin
        _yMax = yMax
        _yMin = yMin
        _xMajor = xMajor
        '_yValue = yValue
        '_xCenterList = xCenterList
        '_xCenterListLog = xCenterListLog
        '_lbin = lbin
        '_rbin = rbin
    End Sub

    Public ReadOnly Property TimeSeriesMethod() As TimeSeriesType
        Get
            Return _TimeSeriesMethod
        End Get
    End Property
    'Public ReadOnly Property IsBarsManual() As Boolean
    '    Get
    '        Return _IsBarsManual
    '    End Get
    'End Property
    'Public ReadOnly Property NumberOfBars() As Integer
    '    Get
    '        Return _NumberOfBars
    '    End Get
    'End Property
    'Public ReadOnly Property IsDiscreteBars() As Boolean
    '    Get
    '        Return _IsDiscreteBars
    '    End Get
    'End Property
    Public ReadOnly Property HistTypeMethod() As HistogramType
        Get
            Return _HistType
        End Get
    End Property
    Public ReadOnly Property HistAlgorothmsMethod() As HistorgramAlgorithms
        Get
            Return _HistAlgorothms
        End Get
    End Property
    Public Property numBins() As Integer
        Get
            Return _NumBins
        End Get
        Set(ByVal value As Integer)
            _NumBins = value
        End Set
    End Property
    Public Property binWidth() As Double
        Get
            Return _BinWidth
        End Get
        Set(ByVal value As Double)
            _BinWidth = value
        End Set
    End Property
    Public Property xMax() As Double
        Get
            Return _xMax
        End Get
        Set(ByVal value As Double)
            _xMax = value
        End Set
    End Property
    Public Property xMin() As Double
        Get
            Return _xMin
        End Get
        Set(ByVal value As Double)
            _xMin = value
        End Set
    End Property
    Public Property yMax() As Double
        Get
            Return _yMax
        End Get
        Set(ByVal value As Double)
            _yMax = value
        End Set
    End Property
    Public Property yMin() As Double
        Get
            Return _yMin
        End Get
        Set(ByVal value As Double)
            _yMin = value
        End Set
    End Property
    Public Property xMajor() As Double
        Get
            Return _xMajor
        End Get
        Set(ByVal value As Double)
            _xMajor = value
        End Set
    End Property
    'Public Property yValue() As List(Of Double)
    '    Get
    '        Return _yValue
    '    End Get
    '    Set(ByVal value As List(Of Double))
    '        _yValue = value
    '    End Set
    'End Property
    'Public Property xCenterList() As List(Of Double)
    '    Get
    '        Return _xCenterList
    '    End Get
    '    Set(ByVal value As List(Of Double))
    '        _xCenterList = value
    '    End Set
    'End Property
    'Public Property xCenterListLog() As List(Of Double)
    '    Get
    '        Return _xCenterListLog
    '    End Get
    '    Set(ByVal value As List(Of Double))
    '        _xCenterListLog = value
    '    End Set
    'End Property
    'Public Property lbin() As List(Of Double)
    '    Get
    '        Return _lbin
    '    End Get
    '    Set(ByVal value As List(Of Double))
    '        _lbin = value
    '    End Set
    'End Property
    'Public Property rbin() As List(Of Double)
    '    Get
    '        Return _rbin
    '    End Get
    '    Set(ByVal value As List(Of Double))
    '        _rbin = value
    '    End Set
    'End Property
    Public ReadOnly Property BoxWhiskerMethod() As BoxWhiskerType
        Get
            Return _BoxWhiskerMethod
        End Get
    End Property
    Public ReadOnly Property IsPlotCensored() As Boolean
        Get
            Return _IsPlotCensored
        End Get
    End Property
    Public Property GetLineColor() As System.Drawing.Color
        Get
            Return _GetLineColor
        End Get
        Set(ByVal value As System.Drawing.Color)
            _GetLineColor = value
        End Set
    End Property
    Public Property GetPointColor() As System.Drawing.Color
        Get
            Return _GetPointColor
        End Get
        Set(ByVal value As System.Drawing.Color)
            _GetPointColor = value
        End Set
    End Property
    Public Property LineColorList() As Integer
        Get
            Return _LineColorList
        End Get
        Set(ByVal value As Integer)
            _LineColorList = value
        End Set
    End Property
    Public Property PointColorList() As Integer
        Get
            Return _PointColorList
        End Get
        Set(ByVal value As Integer)
            _PointColorList = value
        End Set
    End Property
    Public ReadOnly Property StartDate() As Date
        Get
            Return _StartDate
        End Get
    End Property
    Public ReadOnly Property EndDate() As Date
        Get
            Return _EndDate
        End Get
    End Property
    Public ReadOnly Property ChangeDateRange() As Boolean
        Get
            Return _ChangeDateRange
        End Get
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
