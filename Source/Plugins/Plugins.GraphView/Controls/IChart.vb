Namespace Controls
    Public Interface IChart
        Inherits IPlot
        Property ShowPointValues() As Boolean
        Sub ZoomIn()
        Sub ZoomOut()
        Sub ZoomOutAll()
    End Interface

    Public Interface IPlot
        Sub Plot(ByVal seriesPlotInfo As SeriesPlotInfo)
    End Interface
End Namespace