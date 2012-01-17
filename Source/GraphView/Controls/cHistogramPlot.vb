Imports Controls
Imports System.Globalization
Imports ZedGraph

Public Class cHistogramPlot
    Implements IChart

    Private Shared m_Data As Data.DataTable
    
    Private Const m_MaxHistBins As Integer = 20 'holds the maximum number of Bins for a Histogram plot, 20 = selected due to spacing of values on the plot
    Private m_StdDev As Double = 0

    Private yValue() As Double
    Private xCenterList() As Double
    Private xCenterListLog() As Double
    Private lbin() As Double
    Private rbin() As Double
    Private LogScale As Boolean = False


    Public Sub Plot(ByRef options As TimeSeriesPlotOptions, ByVal e_StdDev As Double)
        Try
            m_Data = options.DataTable.Copy
          

            Dim i As Integer

            If zgHistogramPlot.MasterPane.PaneList.Count <> 0 Then
                If zgHistogramPlot.MasterPane.PaneList(0).Title.Text = "Title" Or zgHistogramPlot.MasterPane.PaneList(0).Title.Text = "No Data To Plot" Then
                    zgHistogramPlot.MasterPane.PaneList.Clear()
                End If
            End If

            If (e_StdDev = 0) And (Not (m_Data Is Nothing)) And (m_Data.Rows.Count > 0) Then
                m_StdDev = Statistics.StandardDeviation(m_Data)
            Else
                m_StdDev = e_StdDev
            End If

            zgHistogramPlot.MasterPane.Title.IsVisible = False

            Dim gPane As GraphPane = New GraphPane
            zgHistogramPlot.MasterPane.PaneList.Add(gPane)
            i = zgHistogramPlot.MasterPane.PaneList.Count - 1
            Graph(zgHistogramPlot.MasterPane.PaneList(i), options)

            If zgHistogramPlot.MasterPane.PaneList.Count > 1 Then
                zgHistogramPlot.IsShowHScrollBar = False
                zgHistogramPlot.IsShowVScrollBar = False
            End If

        Catch ex As Exception
            Throw New Exception("Error Occured in ZGHistogram.Plot" & vbCrLf & ex.Message)
        End Try
    End Sub

    Public Sub Clear()
        Try
            'If Not (m_Data Is Nothing) Then
            'm_Data.Dispose()
            'm_Data = Nothing
            'End If
            'm_Data.Clear()
            'Graph()

            'Dim gPane As ZedGraph.GraphPane = zgHistogramPlot.GraphPane
            'gPane.CurveList.Clear()
            'gPane.Title.Text = "No Data To Plot"
            zgHistogramPlot.MasterPane.PaneList.Clear()
            zgHistogramPlot.MasterPane.PaneList.Add(New GraphPane)
            zgHistogramPlot.MasterPane.PaneList(0).Title.IsVisible = True
            zgHistogramPlot.MasterPane.PaneList(0).Title.Text = "No Data To Plot"
            zgHistogramPlot.MasterPane.PaneList(0).XAxis.IsVisible = False
            zgHistogramPlot.MasterPane.PaneList(0).YAxis.IsVisible = False
            zgHistogramPlot.MasterPane.PaneList(0).Border.IsVisible = False
            zgHistogramPlot.MasterPane.PaneList(0).AxisChange()
            zgHistogramPlot.MasterPane.Border.IsVisible = False
            zgHistogramPlot.IsShowHScrollBar = False
            zgHistogramPlot.IsShowVScrollBar = False
            zgHistogramPlot.Refresh()
            zgHistogramPlot.AxisChange()

            'gPane.XAxis.IsVisible = False
            'gPane.YAxis.IsVisible = False
        Catch ex As Exception
            Throw New Exception("Error Occured in ZGHistogram.Clear" & vbCrLf & ex.Message)
        End Try
    End Sub

    Protected Sub Graph(ByVal gPane As GraphPane, ByRef options As TimeSeriesPlotOptions)
        Try
            Dim m_Site = options.SiteName
            Dim m_Variable = options.VariableName
            Dim m_VariableWithUnits = options.VariableName & " - " & options.VariableUnits
            Dim m_Options = options.PlotOptions
            Dim m_ID = options.SeriesID

            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            'New code
            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            Histogram_Calc(m_Data, m_Options)


            'Dim i As Integer
            'Dim gPane As ZedGraph.GraphPane = zgHistogramPlot.GraphPane 'GraphPane of the zg5Histogram plot object -> used to set data and characteristics
            Dim ptList As New ZedGraph.PointPairList 'collection of points for the Histogram chart
            Dim histBars As ZedGraph.BarItem 'Bar Item curve -> Histogram bars on the plot           

            zgHistogramPlot.IsShowVScrollBar = True
            zgHistogramPlot.IsShowHScrollBar = True
            gPane.XAxis.IsVisible = True
            gPane.YAxis.IsVisible = True
            gPane.Legend.IsVisible = False
            With m_Options


                If (.HistTypeMethod = PlotOptions.HistogramType.Probability) Then
                    gPane.YAxis.Title.Text = "Probability Density"
                ElseIf (.HistTypeMethod = PlotOptions.HistogramType.Count) Then
                    gPane.YAxis.Title.Text = "Number of Observations"
                ElseIf (.HistTypeMethod = PlotOptions.HistogramType.Relative) Then
                    gPane.YAxis.Title.Text = "Relative Number of Observations"
                End If

                'set bar settings

                gPane.BarSettings.Type = ZedGraph.BarType.Cluster
                gPane.BarSettings.MinBarGap = 0
                gPane.BarSettings.MinClusterGap = 0
                gPane.XAxis.Scale.IsLabelsInside = False
                gPane.Border.IsVisible = False

                Dim Vector(1) As Double

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

                'If (.xMax - .xMin) / .xMajor > 15 Then
                '    gPane.XAxis.Scale.MajorStep = (.xMax - .xMin) / 15
                'End If

                ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                ' Scaling the Y axis better
                ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''


                gPane.YAxis.MajorGrid.IsVisible = True
                gPane.YAxis.MinorGrid.IsVisible = False
                gPane.YAxis.Scale.MagAuto = False
                gPane.Tag = options

                Dim needShowDataType = False
                For Each c In zgHistogramPlot.MasterPane.PaneList
                    Dim cOptions = DirectCast(c.Tag, TimeSeriesPlotOptions)

                    For Each cc In zgHistogramPlot.MasterPane.PaneList
                        If Not ReferenceEquals(c, cc) Then
                            Dim ccOptions = DirectCast(cc.Tag, TimeSeriesPlotOptions)

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
                        Dim cOptions = DirectCast(c.Tag, TimeSeriesPlotOptions)
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



                'If min > 0 Then
                '    gPane.YAxis.Scale.Min = 0
                'Else
                '    gPane.YAxis.Scale.Min = min - m_StdDev * 0.2
                'End If
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




            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            ''Old(code)
            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            'If ((m_Data Is Nothing) OrElse (m_Data.Rows.Count < 1)) Then 'OrElse ((m_DataSet Is Nothing) OrElse (m_DataSet.Tables.Count < 1)) Then
            '    Dim gPane As ZedGraph.GraphPane = zgHistogramPlot.GraphPane
            '    gPane.CurveList.Clear()
            '    gPane.Title.Text = "No Data To Plot"
            '    zgHistogramPlot.IsShowVScrollBar = False
            '    zgHistogramPlot.IsShowHScrollBar = False
            '    gPane.XAxis.IsVisible = False
            '    gPane.YAxis.IsVisible = False
            'Else
            '    Dim i As Integer 'counter
            '    Dim gPane As GraphPane  'GraphPane of the zg5Histogram plot object -> used to set data and characteristics
            '    Dim ptList As ZedGraph.PointPairList 'collection of points fro the Histogram chart
            '    Dim histBars As ZedGraph.BarItem 'Bar Item curve -> Histogram bars on the plot
            '    Dim numValid As Integer 'number of valid rows returned
            '    Dim numBins As Integer 'number of bars in the bar chart
            '    Dim minValue As Double 'lowest valid value
            '    Dim maxValue As Double 'highest valid value
            '    Dim xRange As Double 'range of Values with padding
            '    Dim dX As Double 'distance between bins
            '    Dim totalCount As Integer
            '    Dim maxTotal As Integer
            '    Dim lastValue As Double
            '    Dim xValue As Double
            '    Dim nextXValue As Double

            '    '1. set the Graph Pane, graphics object
            '    gPane = zgHistogramPlot.GraphPane
            '    gPane.CurveList.Clear()
            '    ''ture on scroll bar
            '    zgHistogramPlot.IsShowVScrollBar = True
            '    zgHistogramPlot.IsShowHScrollBar = True
            '    zgHistogramPlot.IsAutoScrollRange = False ' IsAutoScrollRange = true does not work in Histogram

            '    'g = zg5Histogram.CreateGraphics

            '    '' '' '' '' '' '' ''2. Set Graph Properties
            '    '' '' '' '' '' '' ''x-axis
            '    gPane.XAxis.IsVisible = True
            '    gPane.XAxis.MajorGrid.IsVisible = True
            '    gPane.XAxis.MajorGrid.Color = Color.Gray
            '    ' '' '' '' '' '' ''gPane.XAxis.MinorTic.Size = 0
            '    ' '' '' '' '' '' ''gPane.XAxis.Type = ZedGraph.AxisType.Linear
            '    gPane.XAxis.Title.Text = m_Var
            '    ' '' '' '' '' '' ''gPane.XAxis.Title.Gap = 0.2
            '    'gPane.XAxis.Scale.IsVisible = False
            '    ' '' '' '' '' '' ''gPane.XAxis.Scale.MinGrace = 0.025 '2.5% padding on front
            '    ' '' '' '' '' '' ''gPane.XAxis.Scale.MaxGrace = 0.025 '2.5% padding on back
            '    '' '' '' '' '' '' ''gPane.XAxis.Scale.Mag = 0
            '    '' '' '' '' '' '' ''y-axis
            '    ' '' '' '' '' '' ''gPane.YAxis.IsVisible = True
            '    gPane.YAxis.MajorGrid.IsVisible = True
            '    gPane.YAxis.MajorGrid.Color = Color.Gray
            '    gPane.YAxis.Title.Text = "Number of Observations"
            '    ' '' '' '' '' '' ''gPane.YAxis.Scale.MinGrace = 0.025 '2.5% padding on front
            '    ' '' '' '' '' '' ''gPane.YAxis.Scale.MaxGrace = 0.025 '2.5% padding on back
            '    '' '' '' '' '' '' ''Title
            '    gPane.Title.Text = m_Site

            '    '6. Create the Pts for the Bars
            '    'TODO: Michelle: Track the number of bins created -> set the tboxHPNumBins.Text value

            '    'set min,max,range values
            '    minValue = Math.Floor(CDbl(m_Data.Compute("Min( DataValue )", "")) - (0.2 * m_StdDev))
            '    maxValue = Math.Ceiling(CDbl(m_Data.Compute("Max( DataValue )", "")) + (0.2 * m_StdDev))
            '    numValid = m_Data.Rows.Count

            '    'gPane.XAxis.Scale.Min = Int(minValue)
            '    'gPane.XAxis.Scale.Max = Int(maxValue + 1)

            '    xRange = (maxValue - minValue) 'Math.Round(maxValue - minValue, 3)
            '    'get the number of bins -> bars, tic marks
            '    'figure out for self
            '    numBins = CalculateHistogramNumBins(numValid)
            '    If numBins > m_MaxHistBins Then
            '        numBins = m_MaxHistBins
            '    End If


            '    'dx = range/(#bins - 1)
            '    dX = (xRange / (numBins)).ToString("0.###e0")   'Math.Round(xRange / (numBins), 3)
            '    'modify dX so is a discreet value (a whole number) value -> modified dX will always be smaller than calculated to ensure the correct number of bins!
            '    'If dX > Math.Floor(dX) + 0.5 Then
            '    '    dX = Math.Ceiling(dX)
            '    'Else
            '    '    dX = Math.Floor(dX)
            '    'End If
            '    'Do a check if dx = 0, make it minimum of 1
            '    If dX <= 0 Then
            '        dX = 1
            '    End If

            '    'get the count of values for each value, add it to ptList
            '    ptList = New ZedGraph.PointPairList

            '    lastValue = 0
            '    xValue = minValue
            '    nextXValue = Math.Round(xValue + dX, 3)
            '    totalCount = 0
            '    maxTotal = 0

            '    Dim xLabels(numBins) As String
            '    For i = 0 To numBins - 1
            '        If xValue <= maxValue Then
            '            '1. get the count of values in range
            '            totalCount = m_Data.Compute("Count( DataValue )", "(( DataValue >= " & xValue & ") AND (DataValue < " & nextXValue & "))")
            '            '2. add the point to the list
            '            'ptList.Add(xValue, totalCount, xValue & " - " & nextXValue)
            '            ptList.Add(CDbl((xValue + (dX / 2)).ToString("0.###e0")), totalCount, xValue & " - " & nextXValue)
            '            '3. create the tic mark,lable
            '            'xLabels(i) = xValue
            '            '4. see if totalCount > maxTotal
            '            If totalCount > maxTotal Then
            '                maxTotal = totalCount
            '            End If
            '            '5. calculate next xValue,nextXValue
            '            xValue = nextXValue
            '            nextXValue = Math.Round(xValue + dX, 3)
            '        End If
            '        xLabels(i) = ""
            '    Next i

            '    'Set scroll bar range manually 
            '    zgHistogramPlot.ScrollMaxX = maxValue + 0.5 * m_StdDev
            '    zgHistogramPlot.ScrollMinX = minValue - 0.5 * m_StdDev
            '    zgHistogramPlot.ScrollMaxY = maxTotal * 1.05
            '    zgHistogramPlot.ScrollMinY = 0

            '    'Set the scale
            '    gPane.XAxis.Scale.Min = minValue
            '    gPane.XAxis.Scale.Max = maxValue
            '    'gPane.XAxis.Scale.MinGrace = 0.001
            '    'gPane.XAxis.Scale.MaxGrace = 0.001
            '    gPane.XAxis.Scale.MajorStep = dX
            '    gPane.XAxis.MinorTic.Color = Color.Transparent
            '    gPane.XAxis.Type = AxisType.Linear
            '    gPane.BarSettings.MinBarGap = 0
            '    gPane.BarSettings.MinClusterGap = 0
            '    gPane.BarSettings.Type = BarType.Overlay
            '    gPane.YAxis.Scale.Min = 0


            '    'gPane.XAxis.Scale.TextLabels = xLabels


            '    '7. Plot the Data
            '    'create the bars
            '    histBars = gPane.AddBar("histogram", ptList, Color.Black)

            '    'set bar settings
            '    gPane.XAxis.MajorTic.IsBetweenLabels = True
            '    ' '' '' '' '' ''gPane.XAxis.Scale.IsLabelsInside = False

            '    'draw the plot
            '    gPane.XAxis.IsVisible = True
            '    gPane.YAxis.IsVisible = True
            'End If
            'gPane.AxisChange()

            'zgHistogramPlot.RestoreScale(zgHistogramPlot.GraphPane)
            'zgHistogramPlot.AxisChange()
            'zgHistogramPlot.Refresh()

        Catch ex As Exception
            Throw New Exception("Error Occured in ZGHistogram.RenderGraph" & vbCrLf & ex.Message)
        End Try
    End Sub

#Region "Calculation for Main Histogram Algorithm"
    Public Sub Histogram_Calc(ByRef HistTable As DataTable, ByRef pOptions As PlotOptions) ', ByVal SS As Statistics)

        Dim i, j As Integer

        'Dim d_rows() As System.Data.DataRow

        ''Data rows populated from the Data table.
        'd_rows = HistTable.Select("", "Value ASC")

        'Dim y(d_rows.Length - 1) As Double

        'For i = 0 To d_rows.Length - 1
        '    y(i) = CDbl(d_rows(i).Item("value"))
        'Next

        'If (d_rows.Length = 0) Then
        '    Exit Sub
        'End If

        Dim dX As Double
        Dim bMax As Integer
        Dim jMode As Integer
        Static yMode As Double
        Dim xLimits As List(Of Double) = New List(Of Double)
        Dim yLimits As List(Of Double) = New List(Of Double)


        'do the binning according to the pci_plotoptions HistAlg item
        If pOptions.HistAlgorothmsMethod = PlotOptions.HistorgramAlgorithms.Sturges Then
            'Sturges gives us the number of bins
            pOptions.numBins = Math.Ceiling(Math.Log(System.Convert.ToDouble(Statistics.Count(m_Data)), 2) + 1)
            xLimits = Pretty.PrettyP(Statistics.Minimum(m_Data), Statistics.Maximum(m_Data), pOptions.numBins)

            dX = xLimits(2)
            pOptions.numBins = (xLimits(1) - xLimits(0)) / xLimits(2)
            pOptions.binWidth = dX

        ElseIf pOptions.HistAlgorothmsMethod = PlotOptions.HistorgramAlgorithms.Scott Then
            ' Scotts gives the binwidth


            pOptions.binWidth = (3.5 * m_StdDev) / (Statistics.Count(m_Data) ^ (1 / 3))

            pOptions.numBins = Math.Round((Statistics.Maximum(m_Data) - Statistics.Minimum(m_Data)) / pOptions.binWidth)
            xLimits = Pretty.PrettyP(Statistics.Minimum(m_Data), Statistics.Maximum(m_Data), pOptions.numBins)

            dX = xLimits(2)
            pOptions.numBins = (xLimits(1) - xLimits(0)) / xLimits(2)
            pOptions.binWidth = dX


        ElseIf pOptions.HistAlgorothmsMethod = PlotOptions.HistorgramAlgorithms.Freedman Then
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
            ReDim xCenterListLog(.numBins)
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
                Dim validRows As DataRow()

                validRows = HistTable.Select(str, "")

                yValue(i) = validRows.Length
                validRows = Nothing
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
            If .HistTypeMethod = PlotOptions.HistogramType.Count Then
                '.yValue is good
                'do nothing
            ElseIf .HistTypeMethod = PlotOptions.HistogramType.Probability Then
                'use normalizing constant to make the histogram integrate to 1
                Dim pdf As Double = 0
                Dim k As Integer

                For k = 0 To .numBins
                    pdf += yValue(k) * dX
                Next

                For j = 0 To .numBins
                    yValue(j) = yValue(j) / pdf
                Next

            ElseIf .HistTypeMethod = PlotOptions.HistogramType.Relative Then
                'use normalizing constant to make the histogram relative
                For j = 0 To .numBins
                    yValue(j) = yValue(j) / Statistics.Count(m_Data)
                Next
            End If

        End With

    End Sub
#End Region
    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    ''Old(code)
    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    'Private Function CalculateHistogramNumBins(ByVal numValues As Double) As Integer
    '    'this function calculates the number of Bins -> Bars for the Histogram Chart on the Visualize Tab
    '    'Inputs:  numValues -> the total number of valid values
    '    'Outputs: Integer -> the number of bins needed
    '    Try
    '        Dim numBins As Integer = 0
    '        Dim top As Double   'top half of the equation
    '        Dim bottom As Double 'bottom half of the equation
    '        '#bins = ((2.303*squareRoot(n))/(natural log(n)))*(2)

    '        top = 2.303 * Math.Sqrt(numValues)
    '        bottom = Math.Log(numValues)
    '        numBins = Math.Floor((top / bottom) * 2)

    '        If numBins < 5 Then
    '            numBins = 5
    '        End If

    '        Return numBins
    '    Catch ex As Exception
    '        Throw New Exception("Error Occured in ZGHistogram.CalculateHistogramNumBins" & vbCrLf & ex.Message)
    '    End Try
    'End Function


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
        'zgHistogramPlot.MasterPane.PaneList.Clear()
        zgHistogramPlot.MasterPane.Border.IsVisible = False


    End Sub

    Private Sub zgHistogramPlot_ContextMenuBuilder(ByVal sender As ZedGraph.ZedGraphControl, ByVal menuStrip As System.Windows.Forms.ContextMenuStrip, ByVal mousePt As System.Drawing.Point, ByVal objState As ZedGraph.ZedGraphControl.ContextMenuObjectState) Handles zgHistogramPlot.ContextMenuBuilder
        ' from http://zedgraph.org/wiki/index.php?title=Edit_the_Context_Menu
        ' Create a new menu item
        Dim item As System.Windows.Forms.ToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        ' This is the user-defined Tag so you can find this menu item later if necessary
        item.Name = "export_to_text_file"
        item.Tag = "export_to_text_file"
        ' This is the text that will show up in the menu
        item.Text = "Export to Text File"
        ' Add a handler that will respond when that menu item is selected
        AddHandler item.Click, AddressOf ExportToTextFile
        ' Add the menu item to the menu
        menuStrip.Items.Add(item)
    End Sub

    Protected Sub ExportToTextFile()
        System.Windows.Forms.MessageBox.Show("Not Yet Implemented")
    End Sub
    Public Sub Refreshing()
        zgHistogramPlot.AxisChange()
        zgHistogramPlot.Refresh()
    End Sub

    Public Sub Remove(ByVal ID As Integer)
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



        'For i = 0 To PaneListCopy.Count - 1
        '    If Not (i = ID) Then
        '        'zgTimeSeries.GraphPane.AddCurve(CurveListCopy(i).Label.ToString, CurveListCopy(i).Points, CurveListCopy(i).Color)
        '        'Dim curve As LineItem = zgTimeSeries.GraphPane.AddCurve(CurveListCopy(i).Label.ToString, CurveListCopy(i).Points, CurveListCopy(i).Color, SymbolType.Circle)
        '        'curve.Symbol.Fill = New Fill(m_Options.GetPointColor)
        '        'curve.Symbol.Fill.RangeMin = 0
        '        'curve.Symbol.Fill.RangeMax = 1
        '        'curve.Symbol.Size = 4
        '        'curve.Symbol.Fill.SecondaryValueGradientColor = Color.Empty
        '        'curve.Symbol.Fill.Type = FillType.GradientByColorValue
        '        'curve.Symbol.Border.IsVisible = False
        '        'curve.Symbol.Border.IsVisible = False
        '        zgHistogramPlot.MasterPane.PaneList.Add(PaneListCopy(i))
        '    End If
        'Next
    End Sub

    Public Function PaneID(ByVal pane As GraphPane) As Integer
        Dim ID As Integer
        Dim StartIndex As Integer
        Dim IDLength As Integer
        StartIndex = pane.Title.Text.ToString.IndexOf("ID:") + 4
        IDLength = pane.Title.Text.ToString.Length - StartIndex
        ID = (pane.Title.Text.ToString.Substring(StartIndex, IDLength))
        Return ID
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
