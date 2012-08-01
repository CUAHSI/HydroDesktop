Imports System.Globalization
Imports GraphView.My.Resources
Imports ZedGraph

Namespace Controls

    Public Class HistogramPlot
        Implements IChart

        Private m_StdDev As Double = 0
        Private yValue() As Double
        Private xCenterList() As Double
        Private lbin() As Double
        Private rbin() As Double


        Public Sub Plot(ByVal seriesPlotInfo As SeriesPlotInfo) Implements IPlot.Plot
            Clear()
            For Each oneSeriesInfo In seriesPlotInfo.GetSeriesInfo()
                If oneSeriesInfo.Statistics.NumberOfObservations > oneSeriesInfo.Statistics.NumberOfCensoredObservations Then
                    Plot(oneSeriesInfo, oneSeriesInfo.Statistics.StandardDeviation)
                ElseIf oneSeriesInfo.Statistics.NumberOfObservations = oneSeriesInfo.Statistics.NumberOfCensoredObservations Then
                    If CurveCount() = 0 Then SetGraphPaneTitle(MessageStrings.All_Data_Censored)
                End If
            Next
            Refreshing()
        End Sub

        Private Function CurveCount() As Int32
            Return zgHistogramPlot.GraphPane.CurveList.Count
        End Function

        Private Sub SetGraphPaneTitle(ByVal title As String)
            zgHistogramPlot.GraphPane.Title.Text = title
        End Sub

        Private Sub Plot(ByVal options As OneSeriesPlotInfo, ByVal e_StdDev As Double)
            If zgHistogramPlot.MasterPane.PaneList.Count <> 0 Then
                Dim cOptions = DirectCast(zgHistogramPlot.MasterPane.PaneList(0).Tag, OneSeriesPlotInfo)
                If cOptions Is Nothing Then
                    zgHistogramPlot.MasterPane.PaneList.Clear()
                End If
            End If

            m_StdDev = e_StdDev
            zgHistogramPlot.MasterPane.Title.IsVisible = False

            Dim gPane As GraphPane = New GraphPane
            zgHistogramPlot.MasterPane.PaneList.Add(gPane)
            Graph(gPane, options)

            If zgHistogramPlot.MasterPane.PaneList.Count > 1 Then
                zgHistogramPlot.IsShowHScrollBar = False
                zgHistogramPlot.IsShowVScrollBar = False
            End If
        End Sub

        Private Sub Clear()
            zgHistogramPlot.MasterPane.PaneList.Clear()
            zgHistogramPlot.MasterPane.PaneList.Add(New GraphPane)
            zgHistogramPlot.MasterPane.PaneList(0).Title.IsVisible = True
            zgHistogramPlot.MasterPane.PaneList(0).Title.Text = MessageStrings.No_Data_Plot
            zgHistogramPlot.MasterPane.PaneList(0).XAxis.IsVisible = False
            zgHistogramPlot.MasterPane.PaneList(0).YAxis.IsVisible = False
            zgHistogramPlot.MasterPane.PaneList(0).Border.IsVisible = False
            zgHistogramPlot.MasterPane.PaneList(0).AxisChange()
            zgHistogramPlot.MasterPane.Border.IsVisible = False
            zgHistogramPlot.IsShowHScrollBar = False
            zgHistogramPlot.IsShowVScrollBar = False
            zgHistogramPlot.Refresh()
            zgHistogramPlot.AxisChange()
        End Sub

        Private Sub Graph(ByVal gPane As GraphPane, ByRef options As OneSeriesPlotInfo)
            Try
                Dim m_VariableWithUnits = options.VariableName & " - " & options.VariableUnits
                Dim m_Options = options.PlotOptions
                ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                'New code
                ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                Histogram_Calc(options.DataTable, m_Options)


                'Dim i As Integer
                'Dim gPane As ZedGraph.GraphPane = zgHistogramPlot.GraphPane 'GraphPane of the zg5Histogram plot object -> used to set data and characteristics
                Dim ptList As New PointPairList 'collection of points for the Histogram chart
                Dim histBars As BarItem 'Bar Item curve -> Histogram bars on the plot           

                zgHistogramPlot.IsShowVScrollBar = True
                zgHistogramPlot.IsShowHScrollBar = True
                gPane.XAxis.IsVisible = True
                gPane.YAxis.IsVisible = True
                gPane.Legend.IsVisible = False
                With m_Options


                    If (.HistTypeMethod = HistogramType.Probability) Then
                        gPane.YAxis.Title.Text = "Probability Density"
                    ElseIf (.HistTypeMethod = HistogramType.Count) Then
                        gPane.YAxis.Title.Text = MessageStrings.Number_Observations
                    ElseIf (.HistTypeMethod = HistogramType.Relative) Then
                        gPane.YAxis.Title.Text = "Relative Number of Observations"
                    End If

                    'set bar settings

                    gPane.BarSettings.Type = BarType.Cluster
                    gPane.BarSettings.MinBarGap = 0
                    gPane.BarSettings.MinClusterGap = 0
                    gPane.XAxis.Scale.IsLabelsInside = False
                    gPane.Border.IsVisible = False

                    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                    ' Scaling the X axis better
                    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

                    gPane.XAxis.Title.Text = m_VariableWithUnits
                    gPane.XAxis.MinorTic.IsAllTics = False
                    gPane.XAxis.Title.Gap = 0.2
                    gPane.XAxis.Scale.Mag = 0
                    gPane.XAxis.MinorGrid.IsVisible = False
                    gPane.XAxis.MinorTic.Color = Drawing.Color.Transparent
                    gPane.XAxis.MajorGrid.IsVisible = True
                    gPane.XAxis.Scale.Min = 0
                    gPane.XAxis.IsVisible = True
                    gPane.XAxis.Scale.IsVisible = True

                    gPane.XAxis.MajorTic.IsBetweenLabels = True

                    gPane.XAxis.Scale.Min = .xMin
                    gPane.XAxis.Scale.Max = .xMax
                    gPane.XAxis.Scale.MajorStep = .xMajor
                    gPane.XAxis.Scale.MagAuto = False

                    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                    ' Scaling the Y axis better
                    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''


                    gPane.YAxis.MajorGrid.IsVisible = True
                    gPane.YAxis.MinorGrid.IsVisible = False
                    gPane.YAxis.Scale.MagAuto = False
                    gPane.Tag = options

                    Dim needShowDataType = False
                    For Each c In zgHistogramPlot.MasterPane.PaneList
                        Dim cOptions = DirectCast(c.Tag, OneSeriesPlotInfo)

                        For Each cc In zgHistogramPlot.MasterPane.PaneList
                            If Not ReferenceEquals(c, cc) Then
                                Dim ccOptions = DirectCast(cc.Tag, OneSeriesPlotInfo)

                                If ccOptions.SiteName = cOptions.SiteName And
                                   ccOptions.VariableName = cOptions.VariableName Then
                                    needShowDataType = True
                                    Exit For
                                End If
                            End If
                        Next
                    Next
                    If Not needShowDataType Then
                        ' Set legend only for current curve
                        gPane.Title.Text = options.SiteName + ", " + options.VariableName + ", ID: " + options.SeriesID.ToString
                    Else
                        ' Update legend for all curves
                        For Each c In zgHistogramPlot.MasterPane.PaneList()
                            Dim cOptions = DirectCast(c.Tag, OneSeriesPlotInfo)
                            c.Title.Text = cOptions.SiteName + ", " + cOptions.VariableName + ", " + cOptions.DataType + ", ID: " + cOptions.SeriesID.ToString
                        Next
                    End If


                    gPane.XAxis.Scale.FormatAuto = False

                    Dim min As Double = Double.MinValue
                    Dim list1 As List(Of Double)
                    Dim k As Double
                    For Each k In yValue
                        If (k < min) Then
                            min = k
                        End If
                    Next
                    Dim max As Double = Double.MinValue
                    For Each k In yValue
                        If (k > max) Then
                            max = k
                        End If
                    Next
                    list1 = Pretty.PrettyP(0, max)

                    gPane.YAxis.Scale.Min = 0
                    gPane.YAxis.Scale.MajorStep = list1.Item(2)
                    gPane.YAxis.Scale.MinorStep = gPane.YAxis.Scale.MajorStep / 5
                    gPane.XAxis.MajorTic.IsAllTics = True


                    'gPane.XAxis.Cross = gPane.YAxis.Scale.Min
                    gPane.YAxis.Scale.IsLabelsInside = False
                    gPane.YAxis.MajorTic.IsAllTics = False
                    gPane.YAxis.MajorTic.IsInside = True
                    gPane.YAxis.MinorTic.IsAllTics = False
                    gPane.YAxis.MinorTic.IsInside = True

                    gPane.XAxis.Scale.Min = .xMin
                    gPane.XAxis.Scale.Max = .xMax
                    'gPane.YAxis.Scale.Min = .yMin
                    gPane.YAxis.Scale.Max = list1.Item(1)


                    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                    'Adding the values to the pointlist
                    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                    ptList.Add(xCenterList, yValue)

                    histBars = gPane.AddBar("Histogram", ptList, Drawing.Color.Black)

                End With

            Catch ex As Exception
                Throw New Exception("Error Occured in ZGHistogram.RenderGraph" & vbCrLf & ex.Message)
            End Try
        End Sub

#Region "Calculation for Main Histogram Algorithm"

        Private Sub Histogram_Calc(ByRef HistTable As DataTable, ByRef pOptions As PlotOptions) ', ByVal SS As Statistics)

            Dim m_Data = HistTable
            Dim i, j As Integer
            Dim dX As Double
            Dim bMax As Integer
            Dim jMode As Integer
            Static yMode As Double
            Dim xLimits As List(Of Double) = New List(Of Double)
            Dim yLimits As List(Of Double)

            'do the binning according to the pci_plotoptions HistAlg item
            If pOptions.HistAlgorothmsMethod = HistorgramAlgorithms.Sturges Then
                'Sturges gives us the number of bins
                pOptions.numBins = Math.Ceiling(Math.Log(Convert.ToDouble(Statistics.Count(m_Data)), 2) + 1)
                xLimits = Pretty.PrettyP(Statistics.Minimum(m_Data), Statistics.Maximum(m_Data), pOptions.numBins)

                dX = xLimits(2)
                pOptions.numBins = (xLimits(1) - xLimits(0)) / xLimits(2)
                pOptions.binWidth = dX

            ElseIf pOptions.HistAlgorothmsMethod = HistorgramAlgorithms.Scott Then
                ' Scotts gives the binwidth


                pOptions.binWidth = (3.5 * m_StdDev) / (Statistics.Count(m_Data) ^ (1 / 3))

                pOptions.numBins = Math.Round((Statistics.Maximum(m_Data) - Statistics.Minimum(m_Data)) / pOptions.binWidth)
                xLimits = Pretty.PrettyP(Statistics.Minimum(m_Data), Statistics.Maximum(m_Data), pOptions.numBins)

                dX = xLimits(2)
                pOptions.numBins = (xLimits(1) - xLimits(0)) / xLimits(2)
                pOptions.binWidth = dX


            ElseIf pOptions.HistAlgorothmsMethod = HistorgramAlgorithms.Freedman Then
                'FD gives us the bin width, dX

                pOptions.binWidth = 2 * (Statistics.UpperQuartile(m_Data) - Statistics.LowerQuartile(m_Data)) / (Statistics.Count(m_Data) ^ (1 / 3))

                pOptions.numBins = Math.Round((Statistics.Maximum(m_Data) - Statistics.Minimum(m_Data)) / pOptions.binWidth)
                xLimits = Pretty.PrettyP(Statistics.Minimum(m_Data), Statistics.Maximum(m_Data), pOptions.numBins)

                dX = xLimits(2)
                pOptions.numBins = (xLimits(1) - xLimits(0)) / xLimits(2)
                pOptions.binWidth = dX
            End If

            With pOptions
                .xMax = xLimits(1)
                .xMin = xLimits(0)
                .xMajor = dX

                'make it so we don't have too many tick marks on the plot
                Do Until (.xMax - .xMin) / .xMajor < 16
                    .xMajor = .xMajor * 2
                Loop

                ReDim yValue(.numBins)
                ReDim xCenterList(.numBins)
                ReDim lbin(.numBins)
                ReDim rbin(.numBins)

                'This loop finds the xCenter list with the left and right bound values
                For j = 0 To .numBins
                    yValue(j) = 0
                    xCenterList(j) = (.xMin + (j + 1) * dX + .xMin + j * dX) / 2.0#
                    lbin(j) = .xMin + j * dX
                    rbin(j) = .xMin + (j + 1) * dX

                Next j


                'Now that we know the xCenterList (e.g., we know the bins)
                ' we need to put each datapoint into its respective bin
                For i = 0 To .numBins
                    'query the data tables for each bin
                    Dim str As String = "DataValue >= " & lbin(i).ToString(CultureInfo.InvariantCulture) & " and DataValue < " & rbin(i).ToString(CultureInfo.InvariantCulture)
                    yValue(i) = HistTable.Compute("count(DataValue)", str)
                Next

                'loop through bins and find the max bin count and the mode
                jMode = 0
                bMax = 0
                For j = 0 To .numBins
                    If (yValue(j) > bMax) Then
                        jMode = j
                        bMax = yValue(j)
                    End If
                    '.normalizer = .normalizer + .yValue(j) * dX
                Next j
                yMode = xCenterList(jMode)

                yLimits = Pretty.PrettyP(0, bMax)
                .yMax = yLimits(1)
                .yMin = yLimits(0)


                'transform the bin counts (if needed)
                If .HistTypeMethod = HistogramType.Count Then
                    '.yValue is good
                    'do nothing
                ElseIf .HistTypeMethod = HistogramType.Probability Then
                    'use normalizing constant to make the histogram integrate to 1
                    Dim pdf As Double = 0
                    Dim k As Integer

                    For k = 0 To .numBins
                        pdf += yValue(k) * dX
                    Next

                    For j = 0 To .numBins
                        yValue(j) = yValue(j) / pdf
                    Next

                ElseIf .HistTypeMethod = HistogramType.Relative Then
                    'use normalizing constant to make the histogram relative
                    For j = 0 To .numBins
                        yValue(j) = yValue(j) / Statistics.Count(m_Data)
                    Next
                End If

            End With

        End Sub
#End Region


        Private Function CalculateHistogramNumBins(ByVal numValues As Double) As Integer
            'this function calculates the number of Bins -> Bars for the Histogram Chart on the Visualize Tab
            'Inputs:  numValues -> the total number of valid values
            'Outputs: Integer -> the number of bins needed
            Try
                Dim numBins As Integer = 0
                Dim top As Double   'top half of the equation
                Dim bottom As Double 'bottom half of the equation
                '#bins = ((2.303*squareRoot(n))/(natural log(n)))*(2)

                top = 2.303 * Math.Sqrt(numValues)
                bottom = Math.Log(numValues)
                numBins = Math.Floor((top / bottom) * 2)

                If numBins < 5 Then
                    numBins = 5
                End If

                Return numBins
            Catch ex As Exception
                Throw New Exception("Error Occured in ZGHistogram.CalculateHistogramNumBins" & vbCrLf & ex.Message)
            End Try
        End Function


        Public Sub New()

            ' This call is required by the Windows Form Designer.
            InitializeComponent()

            ' Add any initialization after the InitializeComponent() call.
            Dim gPane As GraphPane = zgHistogramPlot.MasterPane.PaneList(0)
            gPane.Border.IsVisible = False
            gPane.Legend.IsVisible = False
            gPane.BarSettings.Type = BarType.Stack
            zgHistogramPlot.MasterPane.Border.IsVisible = False
        End Sub

        Private Sub Refreshing()
            zgHistogramPlot.AxisChange()
            zgHistogramPlot.Refresh()
        End Sub

        Private Sub Remove(ByVal ID As Integer)
            Dim PaneListCopy As New PaneList
            For i = 0 To zgHistogramPlot.MasterPane.PaneList.Count - 1
                PaneListCopy.Add(zgHistogramPlot.MasterPane.PaneList(i))
            Next

            zgHistogramPlot.MasterPane.PaneList.Clear()

            For i = 0 To PaneListCopy.Count - 1
                If Not (PaneID(PaneListCopy(i)) = ID) Then
                    zgHistogramPlot.MasterPane.PaneList.Add(PaneListCopy(i))
                End If
            Next
        End Sub

        Public Function PaneID(ByVal pane As GraphPane) As Integer
            Dim cOptions = DirectCast(pane.Tag, OneSeriesPlotInfo)
            If cOptions Is Nothing Then Return Nothing
            Return cOptions.SeriesID
        End Function

        Public Property ShowPointValues() As Boolean Implements IChart.ShowPointValues
            Get
                Return zgHistogramPlot.IsShowPointValues
            End Get
            Set(ByVal value As Boolean)
                zgHistogramPlot.IsShowPointValues = value
            End Set
        End Property

        Public Sub ZoomIn() Implements IChart.ZoomIn
            zgHistogramPlot.ZoomIn()
        End Sub

        Public Sub ZoomOut() Implements IChart.ZoomOut
            zgHistogramPlot.ZoomOut(zgHistogramPlot.GraphPane)
        End Sub

        Public Sub ZoomOutAll() Implements IChart.ZoomOutAll
            zgHistogramPlot.ZoomOutAll(zgHistogramPlot.GraphPane)
        End Sub
    End Class
End Namespace