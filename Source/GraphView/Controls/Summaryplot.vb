Imports System.Windows.Forms
Imports System.Drawing
Imports GraphView.My.Resources
Imports HydroDesktop.Interfaces.ObjectModel
Imports ZedGraph

Namespace Controls

    Public Class SummaryPlot

#Region "setup"

        ReadOnly gPane1 As GraphPane = New GraphPane
        Dim gPane2 As GraphPane = New GraphPane
        ReadOnly gPane3 As GraphPane = New GraphPane
        Dim gPane4 As GraphPane = New GraphPane

        Public Sub New()
            ' This call is required by the Windows Form Designer.
            InitializeComponent()

            ' Add any initialization after the InitializeComponent() call.
            Dim master As MasterPane = zgSummaryPlot.MasterPane
            master.PaneList.Clear()
            master.Title.IsVisible = True
            master.Title.Text = "Title"
            master.Add(gPane1)
            master.Add(gPane2)
            master.Add(gPane3)
            master.Add(gPane4)
            master.AxisChange()
            master.Border.IsVisible = False
            zgSummaryPlot.IsShowVScrollBar = False
            zgSummaryPlot.IsShowHScrollBar = False
            'gPane1.XAxis.Type = AxisType.Date
            'gPane1.YAxis.Type = AxisType.Linear

        End Sub

        Private Const XColumn As String = "LocalDateTime"
        Private Const YColumn As String = "DataValue"

        Public Shared m_Data As DataTable
        ' Public Shared m_DataSet As Data.DataSet
        Public Shared m_Site As String
        Public Shared m_Var As String
        Private Shared m_Options As PlotOptions
        Public Shared m_Units As String
        Private m_StdDev As Double

        'Histogram variables
        Private yValue() As Double
        Private xCenterList() As Double
        Private xCenterListLog() As Double
        Private lbin() As Double
        Private rbin() As Double
        Private LogScale As Boolean = False

        'Box Whisker variables
        Private Const db_outFld_ValDTMonth As String = "DateMonth"
        Private Const db_outFld_ValDTYear As String = "DateYear"
        Private Const db_outFld_ValDTDay As String = "DateDay"
#End Region

        Public Sub Plot(ByRef objDataTable As DataTable, ByVal strSiteName As String, ByVal strVariableName As String, ByVal strVariableUnits As String, ByRef objOptions As PlotOptions, Optional ByVal e_StdDev As Double = 0)
            Try
                Dim master As MasterPane = zgSummaryPlot.MasterPane
                If master.PaneList.Count = 0 Then
                    master.Add(gPane1)
                    master.Add(gPane2)
                    master.Add(gPane3)
                    master.Add(gPane4)
                End If
                Clear()

                m_Data = objDataTable.Copy
                m_Site = strSiteName
                m_Var = strVariableName & " - " & strVariableUnits
                m_Options = objOptions
                m_Units = strVariableUnits

                If (e_StdDev = 0) And (Not (m_Data Is Nothing)) And (m_Data.Rows.Count > 0) Then
                    m_StdDev = Statistics.StandardDeviation(m_Data)
                Else
                    m_StdDev = e_StdDev
                End If

                TimeSeriesPlot()
                PlotProbability(m_Site, m_Var, m_Units)
                HistogramPlot()
                BoxWhiskerPlot()

                gPane1.Title.IsVisible = False
                gPane2.Title.IsVisible = False
                gPane3.Title.IsVisible = False
                gPane4.Title.IsVisible = False

                master.Title.IsVisible = True
                master.Title.Text = m_Site

                gPane1.AxisChange()
                gPane2.AxisChange()
                gPane3.AxisChange()
                gPane4.AxisChange()

                zgSummaryPlot.Refresh()
                zgSummaryPlot.AxisChange()
            Catch ex As Exception
                Throw New Exception("Error Occured in zgSummaryPlot.Plot" & vbCrLf & ex.Message)
            End Try
        End Sub

        Public Sub Clear()
            Try
                'm_Data.Clear()

                gPane1.CurveList.Clear()
                gPane1.GraphObjList.Clear()
                gPane1.XAxis.IsVisible = False
                gPane1.YAxis.IsVisible = False

                gPane2.CurveList.Clear()
                gPane2.GraphObjList.Clear()
                gPane2.XAxis.IsVisible = False
                gPane2.YAxis.IsVisible = False

                gPane3.CurveList.Clear()
                gPane3.GraphObjList.Clear()
                gPane3.XAxis.IsVisible = False
                gPane3.YAxis.IsVisible = False

                gPane4.CurveList.Clear()
                gPane4.GraphObjList.Clear()
                gPane4.XAxis.IsVisible = False
                gPane4.YAxis.IsVisible = False

                zgSummaryPlot.MasterPane.Title.Text = MessageStrings.No_Data_Plot
                zgSummaryPlot.RestoreScale(gPane1)
                zgSummaryPlot.RestoreScale(gPane2)
                zgSummaryPlot.RestoreScale(gPane3)
                zgSummaryPlot.RestoreScale(gPane4)
                'Graph()

            Catch ex As Exception
                Throw New Exception("Error Occured in zgSummaryPlot.Clear" & vbCrLf & ex.Message)
            End Try
        End Sub

#Region "Time Series Plot"
        Public Sub Replot(ByVal options As PlotOptions)
            m_Options = options
            TimeSeriesPlot()
        End Sub


        Protected Sub TimeSeriesPlot()
            Try
                Dim gPane1 As GraphPane = zgSummaryPlot.GraphPane
                'gPane1.CurveList.Clear()


                If (m_Data Is Nothing) Or (m_Data.Rows.Count <= 0) Then
                    gPane1.XAxis.IsVisible = False
                    gPane1.YAxis.IsVisible = False
                    gPane1.Title.Text = MessageStrings.No_Data_Plot
                Else

                    'Setting Scroll and scale
                    zgSummaryPlot.ZoomOutAll(gPane1)
                    'Setting Axises
                    gPane1.XAxis.IsVisible = True
                    gPane1.XAxis.Title.Text = "Date and Time"
                    gPane1.YAxis.IsVisible = True
                    gPane1.YAxis.Title.Text = m_Var

                    If gPane1.CurveList.Count <= 1 Then
                        gPane1.Legend.IsVisible = False
                    End If

                    'Dim pointList As New ZedGraph.DataSourcePointList
                    'pointList.XDataMember = XColumn
                    'pointList.YDataMember = YColumn
                    'pointList.DataSource = m_Data
                    'pointList.TagDataMember = "CensorCode"

                    Dim pointList As New PointPairList


                    For Each row As DataRow In m_Data.Rows
                        Dim p As New PointPair(New XDate(CDate(row.Item(XColumn))), row.Item(YColumn))
                        p.Tag = row.Item("CensorCode").ToString
                        If m_Options.IsPlotCensored Then
                            If Not DataValue.IsCensored(p.Tag.ToString) Then
                                p.ColorValue = 0
                            Else
                                p.ColorValue = 1
                            End If
                            pointList.Add(p)
                        Else
                            If DataValue.IsCensored(p.Tag.ToString) Then
                                pointList.Add(p)
                            End If
                        End If
                    Next row


                    Dim curve As LineItem = gPane1.AddCurve(m_Site, pointList, Color.Black, SymbolType.Circle)
                    curve.Symbol.Fill = New Fill(Color.Black, Color.Black)
                    curve.Symbol.Fill.RangeMin = 0
                    curve.Symbol.Fill.RangeMax = 1
                    curve.Symbol.Size = 4
                    curve.Symbol.Fill.SecondaryValueGradientColor = Color.Empty
                    curve.Symbol.Fill.Type = FillType.GradientByColorValue
                    curve.Symbol.Border.IsVisible = False
                    curve.Symbol.Border.IsVisible = False
                    Select Case m_Options.TimeSeriesMethod
                        Case TimeSeriesType.Line
                            curve.Line.IsVisible = True
                            curve.Symbol.IsVisible = False
                        Case TimeSeriesType.Point
                            curve.Line.IsVisible = False
                            curve.Symbol.IsVisible = True
                        Case TimeSeriesType.None
                            curve.Line.IsVisible = False
                            curve.Symbol.IsVisible = False
                        Case Else
                            curve.Line.IsVisible = True
                            curve.Symbol.IsVisible = True
                    End Select


                    'Setting Legend
                    If (m_Options.ShowLegend) And (gPane1.CurveList.Count > 1) Then
                        gPane1.Legend.IsVisible = True
                        gPane1.Legend.Position = 12
                    Else
                        gPane1.Legend.IsVisible = False
                    End If


                    'gPane1.Legend.Fill = New Fill(m_Options.GetLineColor, Brushes.AliceBlue, FillType.None)
                    'gPane1.Legend.FontSpec.
                End If
                'zgSummaryPlot.AxisChange()
                'zgSummaryPlot.Refresh()
            Catch ex As Exception
                Throw New Exception("Error Occured in zgSummaryPlot.Graph" & vbCrLf & ex.Message)
            End Try


        End Sub

        Private Sub zgSummaryPlot_ContextMenuBuilder(ByVal sender As ZedGraphControl, ByVal menuStrip As ContextMenuStrip, ByVal mousePt As Point, ByVal objState As ZedGraphControl.ContextMenuObjectState) Handles zgSummaryPlot.ContextMenuBuilder
            ' from http://zedgraph.org/wiki/index.php?title=Edit_the_Context_Menu

            ' Add item to export to text file
            ' Create a new menu item
            Dim item As ToolStripMenuItem = New ToolStripMenuItem()
            ' This is the user-defined Tag so you can find this menu item later if necessary
            item.Name = "export_to_text_file"
            item.Tag = "export_to_text_file"
            ' This is the text that will show up in the menu
            item.Text = "Export to Text File"
            ' Add a handler that will respond when that menu item is selected
            AddHandler item.Click, AddressOf ExportToTextFile
            ' Add the menu item to the menu
            menuStrip.Items.Add(item)

            ' Add item to export to change line color
            item = New ToolStripMenuItem()
            item.Name = "set_line_color"
            item.Tag = "set_line_color"
            item.Text = "Set Line Color"
            AddHandler item.Click, AddressOf SetLineColor
            menuStrip.Items.Add(item)
        End Sub

        Private Function PromptForColor(ByVal defaultColor As Color) As Color
            Dim dlgColor As ColorDialog = New ColorDialog()

            If Not IsDBNull(defaultColor) Then
                dlgColor.Color = defaultColor
            End If

            If (dlgColor.ShowDialog() = DialogResult.OK) Then
                Return dlgColor.Color
            Else
                Return Nothing
            End If
        End Function

        Protected Sub ExportToTextFile()
            MessageBox.Show("Not Yet Implemented")
        End Sub

        Protected Sub SetLineColor()
            If zgSummaryPlot.GraphPane.CurveList.Count > 0 Then
                Dim newColor As Color = PromptForColor(zgSummaryPlot.GraphPane.CurveList.Item(0).Color)
                If Not IsDBNull(newColor) Then
                    zgSummaryPlot.GraphPane.CurveList.Item(0).Color = newColor
                    zgSummaryPlot.Refresh()
                End If
            End If
        End Sub
#End Region

#Region "Probability"

        Private Sub PlotProbability(ByVal site As String, ByVal variable As String, ByVal varUnits As String)
            Dim i As Integer 'counter
            'Dim gPane2 As ZedGraph.GraphPane 'GraphPane of the zgProbability plot object -> used to set data and characteristics
            'Dim g As Drawing.Graphics 'graphics object of the zgProbability plot object -> used to redraw/update the plot
            Dim ptList As PointPairList 'collection of points for the Probability plot
            Dim bflPtList As New PointPairList
            Dim probLine As LineItem
            Dim bflLine As New LineItem("")
            Dim validRows() As DataRow
            Dim numRows As Integer
            Dim curValue As Double
            Dim curX As Double
            Dim curFreq As Double
            Try
                'get all data(even censored ones), order by Value
                validRows = m_Data.Select("", "DataValue ASC")
                numRows = validRows.GetLength(0)

                If gPane2.IsZoomed() = True Then
                    zgSummaryPlot.ZoomOutAll(gPane2)
                End If
                'x-axis
                gPane2.XAxis.IsVisible = True
                gPane2.XAxis.MajorTic.Size = 0
                gPane2.XAxis.MinorTic.Size = 0
                gPane2.XAxis.Title.Text = vbCrLf & vbCrLf & "Cumulative Frequency < Stated Value %"
                gPane2.XAxis.Title.Gap = 0.2
                gPane2.XAxis.Type = AxisType.Linear
                gPane2.XAxis.Scale.IsVisible = False
                gPane2.XAxis.Scale.Min = -4.0
                gPane2.XAxis.Scale.Max = 4.0
                gPane2.XAxis.MajorTic.IsAllTics = False
                gPane2.XAxis.Scale.MinGrace = 0
                gPane2.XAxis.Scale.MaxGrace = 0
                'y-axis
                gPane2.YAxis.IsVisible = True
                gPane2.YAxis.MajorGrid.IsVisible = True
                gPane2.YAxis.MajorGrid.Color = Color.Gray
                gPane2.YAxis.Title.Text = variable & "  " & varUnits
                gPane2.YAxis.Type = AxisType.Linear
                gPane2.YAxis.Scale.MinGrace = 0.025
                gPane2.YAxis.Scale.MaxGrace = 0.025
                'Title
                While (GetStringLen(site, gPane2.Title.FontSpec.GetFont(gPane2.CalcScaleFactor)) > zgSummaryPlot.Width)
                    site = GraphTitleBreaks(site)
                End While


                '6. Create the Pts for the Line
                ptList = New PointPairList

                'Dim ptListF As New ZedGraph.PointPairList
                For i = 0 To numRows - 1
                    'get the y component
                    curValue = validRows(i).Item("DataValue")
                    'curX = CalculateProbabilityXPosition(i / numRows)
                    curFreq = CalculateProbabilityFreq(i + 1, numRows)
                    curX = CalculateProbabilityXPosition(curFreq)
                    'NOTE: use i+1 so rank = 1 -> N

                    'plot the point
                    If curValue >= 0 Then
                        Dim p As New PointPair(curX, curValue, "(" & curFreq * 100 & ", " & curValue & ")")
                        p.Tag = validRows(i).Item("CensorCode").ToString
                        If m_Options.IsPlotCensored Then
                            If Not DataValue.IsCensored(p.Tag.ToString) Then
                                p.ColorValue = 0
                            Else
                                p.ColorValue = 1
                            End If
                            ptList.Add(p)
                        Else
                            If DataValue.IsCensored(p.Tag.ToString) Then
                                ptList.Add(p)
                            End If
                        End If
                    End If
                Next i

                '7. Plot the Data
                'create the points
                'probLine = New ZedGraph.LineItem("ProbCurve")
                probLine = gPane2.AddCurve(site, ptList, Color.Black, SymbolType.Circle)
                probLine.Line.IsVisible = False
                probLine.Symbol.Fill = New Fill(Color.Black, Color.Black)
                probLine.Symbol.Fill.RangeMin = 0
                probLine.Symbol.Fill.RangeMax = 1
                probLine.Symbol.Size = 4
                probLine.Symbol.Fill.SecondaryValueGradientColor = Color.Empty
                probLine.Symbol.Fill.Type = FillType.GradientByColorValue
                probLine.Symbol.Border.IsVisible = False

                '8. set up Tic Marks

                For i = 0 To 20
                    AddLabelToPlot(gPane2, GetProbabilityLabel(i), GetProbabilityValue(i))
                Next i

                '9. turn on the legend
                If m_Options.ShowLegend And gPane4.CurveList.Count > 1 Then
                    gPane2.Legend.IsVisible = True
                    gPane2.Legend.Position = 12
                Else
                    gPane2.Legend.IsVisible = False
                End If

                'End If
            Catch ex As Exception
                'show an error message
                Throw New Exception("An Error occurred while graphing the Probability Plot on the Visualize Tab." & vbCrLf & "Message = " & ex.Message, ex)
            End Try
        End Sub

        Private Function CalculateProbabilityXPosition(ByVal freq As Double) As Double
            'Calculates the position along the x-axis to place the dot -> only used on the Probability Plot
            'Based on a normal curve distribution, Code is from Dr. Stevens
            'Inputs:  freq -> used to calculate the position so has a normal distribution look -> (i/numrows)
            'Outputs: Double -> the x-position to plot the point at
            Try
                Return Math.Round(4.91 * (freq ^ 0.14 - (1.0# - freq) ^ 0.14), 3)
            Catch ex As Exception
                Throw New Exception("An Error occurred while calculating the X-Position for a point in the Probability Plot." & vbCrLf & "Message= " & ex.Message, ex)
            End Try
        End Function

        Private Function CalculateProbabilityFreq(ByVal rank As Integer, ByVal numRows As Integer) As Double
            Try
                Return Math.Round((rank - 0.375) / (numRows + 1 - 2 * (0.375)), 3)
            Catch ex As Exception

            End Try
        End Function

        Private Function CreateProbabilityXAxisLabels(ByVal pane As GraphPane, ByVal axis As Axis, ByVal val As Double, ByVal index As Integer) As String
            Try
                'pane.XAxis.IsVisible = True
                'Select Case val
                '	Case -3.892
                '		Return "0.01"
                '	Case -3.5
                '		Return "0.02"
                '	Case -3.095
                '		Return "0.1"
                '	Case -2.323
                '		Return "1"
                '	Case -2.055
                '		Return "2"
                '	Case -1.645
                '		Return "5"
                '	Case -1.282
                '		Return "10"
                '	Case -0.842
                '		Return "20"
                '	Case -0.524
                '		Return "30"
                '	Case -0.254
                '		Return "40"
                '	Case 0
                '		Return "50"
                '	Case 0.254
                '		Return "60"
                '	Case 0.524
                '		Return "70"
                '	Case 0.842
                '		Return "80"
                '	Case 1.282
                '		Return "90"
                '	Case 1.645
                '		Return "95"
                '	Case 2.055
                '		Return "98"
                '	Case 2.323
                '		Return "99"
                '	Case 3.095
                '		Return "99.9"
                '	Case 3.5
                '		Return "99.98"
                '	Case 3.892
                '		Return "99.99"
                '	Case Else
                '		Return ""
                'End Select
                Return ""
            Catch ex As Exception
                Return ""
            End Try
        End Function

        Private Function GetProbabilityLabel(ByVal index As Integer) As String
            Select Case index
                Case 0
                    Return "0.01"
                Case 1
                    Return "0.02"
                Case 2
                    Return "0.1"
                Case 3
                    Return "1"
                Case 4
                    Return "2"
                Case 5
                    Return "5"
                Case 6
                    Return "10"
                Case 7
                    Return "20"
                Case 8
                    Return "30"
                Case 9
                    Return "40"
                Case 10
                    Return "50"
                Case 11
                    Return "60"
                Case 12
                    Return "70"
                Case 13
                    Return "80"
                Case 14
                    Return "90"
                Case 15
                    Return "95"
                Case 16
                    Return "98"
                Case 17
                    Return "99"
                Case 18
                    Return "99.9"
                Case 19
                    Return "99.98"
                Case 20
                    Return "99.99"
                Case Else
                    Return ""
            End Select
        End Function

        Private Function GetProbabilityValue(ByVal index As Integer) As Double
            Select Case index
                Case 0
                    Return -3.892
                Case 1
                    Return -3.5
                Case 2
                    Return -3.095
                Case 3
                    Return -2.323
                Case 4
                    Return -2.055
                Case 5
                    Return -1.645
                Case 6
                    Return -1.282
                Case 7
                    Return -0.842
                Case 8
                    Return -0.542
                Case 9
                    Return -0.254
                Case 10
                    Return 0
                Case 11
                    Return 0.254
                Case 12
                    Return 0.524
                Case 13
                    Return 0.842
                Case 14
                    Return 1.282
                Case 15
                    Return 1.645
                Case 16
                    Return 2.055
                Case 17
                    Return 2.323
                Case 18
                    Return 3.095
                Case 19
                    Return 3.5
                Case 20
                    Return 3.892
                Case Else
                    Return ""
            End Select
        End Function

        Private Sub AddLabelToPlot(ByRef gpane As GraphPane, ByVal label As String, ByVal xLoc As Double)
            Dim myText As TextObj
            Dim myTic As TextObj
            Try
                myText = New TextObj(label, xLoc, 1.05, CoordType.XScaleYChartFraction)
                myText.FontSpec.Size = 13
                myText.FontSpec.Border.IsVisible = False
                myText.FontSpec.Fill = New Fill(Color.FromArgb(25, Color.White))
                gpane.GraphObjList.Add(myText)
                myTic = New TextObj("|", xLoc, 0.997, CoordType.XScaleYChartFraction)
                myTic.FontSpec.Size = 12.0
                myTic.FontSpec.Border.IsVisible = False
                myTic.FontSpec.Fill = New Fill(Color.FromArgb(25, Color.White))
                gpane.GraphObjList.Add(myTic)
            Catch ex As Exception
                Throw New Exception("An Error occurred while creating an X-Axis Label for a plot on the Visualize Tab." & vbCrLf & "Message = " & ex.Message, ex)
            End Try
        End Sub

        Public Function GraphTitleBreaks(ByVal s As String) As String
            Dim offset As Integer
            For offset = 0 To ((s.Length - 1) \ 2)
                If (((s.Length - 1) \ 2) - offset) > 0 AndAlso (s(((s.Length - 1) \ 2) - offset) = " ") Then
                    Return s.Substring(0, (((s.Length - 1) \ 2) - offset)) & vbCrLf & s.Substring(((s.Length - 1) \ 2) - offset)
                ElseIf (((s.Length) \ 2) + offset) > 0 AndAlso (s(((s.Length) \ 2) + offset) = " ") Then
                    Return s.Substring(0, ((s.Length \ 2) + offset)) & vbCrLf & s.Substring((s.Length \ 2) + offset)
                End If
            Next offset
            Return (s.Substring(0, (((s.Length - 1) \ 2))) & vbCrLf & s.Substring(((s.Length - 1) \ 2)))
        End Function

        Public Function GetStringLen(ByVal s As String) As Integer
            'calculates the the length of the string s in pixels
            'Inputs:  s -> the string to find the length of
            'Outputs: Integer -> the length of the string s in pixels
            Dim l As New System.Windows.Forms.Label   'used to find the length of the string in pixels
            l.Text = s
            l.AutoSize = True
            Return l.Width
        End Function

        Public Function GetStringLen(ByVal s As String, ByVal stringFormat As Font) As Integer
            'calculates the the length of the string s in pixels
            'Inputs:  s -> the string to find the length of
            'Outputs: Integer -> the length of the string s in pixels
            Dim l As New System.Windows.Forms.Label   'used to find the length of the string in pixels
            'l.Height = 13
            'l.Width = 100
            l.Text = s
            l.Font = stringFormat
            l.AutoSize = True
            Return l.PreferredWidth
        End Function

#End Region

#Region "Historgram"

        Protected Sub HistogramPlot()
            Try

                ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                'New code
                ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                Histogram_Calc(m_Data, m_Options)


                'Dim i As Integer

                Dim ptList As New PointPairList 'collection of points for the Histogram chart
                Dim histBars As BarItem 'Bar Item curve -> Histogram bars on the plot           


                gPane3.XAxis.IsVisible = True
                gPane3.YAxis.IsVisible = True
                With m_Options


                    If (.HistTypeMethod = HistogramType.Probability) Then
                        gPane3.YAxis.Title.Text = "Probability Density"
                    ElseIf (.HistTypeMethod = HistogramType.Count) Then
                        gPane3.YAxis.Title.Text = MessageStrings.Number_Observations
                    ElseIf (.HistTypeMethod = HistogramType.Relative) Then
                        gPane3.YAxis.Title.Text = "Relative Number of Observations"
                    End If

                    'set bar settings

                    gPane3.BarSettings.Type = BarType.Cluster
                    gPane3.BarSettings.MinBarGap = 0
                    gPane3.BarSettings.MinClusterGap = 0
                    gPane3.XAxis.Scale.IsLabelsInside = False
                    gPane3.Border.IsVisible = True
                    gPane3.Legend.IsVisible = False

                    Dim Vector(1) As Double

                    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                    ' Scaling the X axis better
                    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

                    gPane3.XAxis.Title.Text = m_Var
                    gPane3.XAxis.MinorTic.IsAllTics = False
                    gPane3.XAxis.Title.Gap = 0.2
                    gPane3.XAxis.Scale.Mag = 0
                    gPane3.XAxis.MinorGrid.IsVisible = False
                    gPane3.XAxis.MinorTic.Color = Color.Transparent
                    gPane3.XAxis.MajorGrid.IsVisible = True
                    gPane3.XAxis.Scale.Min = 0
                    gPane3.XAxis.IsVisible = True
                    gPane3.XAxis.Scale.IsVisible = True

                    gPane3.XAxis.MajorTic.IsBetweenLabels = True

                    gPane3.XAxis.Scale.Min = .xMin
                    gPane3.XAxis.Scale.Max = .xMax
                    gPane3.XAxis.Scale.MajorStep = .xMajor

                    'If (.xMax - .xMin) / .xMajor > 15 Then
                    '    gPane3.XAxis.Scale.MajorStep = (.xMax - .xMin) / 15
                    'End If

                    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                    ' Scaling the Y axis better
                    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''


                    gPane3.YAxis.MajorGrid.IsVisible = True
                    gPane3.YAxis.MinorGrid.IsVisible = False
                    gPane3.Title.Text = m_Site
                    gPane3.XAxis.Scale.FormatAuto = False



                    Dim max As Double = Double.MinValue
                    Dim list1 As List(Of Double) = New List(Of Double)
                    Dim k As Double
                    For Each k In yValue
                        If (k > max) Then
                            max = k
                        End If
                    Next
                    list1 = Pretty.PrettyP(0, max)




                    gPane3.YAxis.Scale.Min = 0
                    gPane3.YAxis.Scale.MajorStep = list1.Item(2)
                    gPane3.YAxis.Scale.MinorStep = gPane3.YAxis.Scale.MajorStep / 5
                    gPane3.XAxis.MajorTic.IsAllTics = True


                    'gPane3.XAxis.Cross = gPane3.YAxis.Scale.Min
                    gPane3.YAxis.Scale.IsLabelsInside = False
                    gPane3.YAxis.MajorTic.IsAllTics = False
                    gPane3.YAxis.MajorTic.IsInside = True
                    gPane3.YAxis.MinorTic.IsAllTics = False
                    gPane3.YAxis.MinorTic.IsInside = True

                    gPane3.XAxis.Scale.Min = .xMin
                    gPane3.XAxis.Scale.Max = .xMax
                    gPane3.YAxis.Scale.Min = .yMin
                    gPane3.YAxis.Scale.Max = list1.Item(1)


                    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                    'Adding the values to the pointlist
                    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                    ptList.Add(xCenterList, yValue)

                    histBars = gPane3.AddBar("Histogram", ptList, Color.Black)

                End With

            Catch ex As Exception
                Throw New Exception("Error Occured in ZGHistogram.RenderGraph" & vbCrLf & ex.Message)
            End Try
        End Sub

#Region "Calculation for Main Histogram Algorithm"
        Public Sub Histogram_Calc(ByRef HistTable As DataTable, ByRef pOptions As PlotOptions) ', ByVal SS As Statistics)

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
                    Dim str As String = "DataValue >= " & lbin(i).ToString & " and DataValue < " & rbin(i).ToString
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
#End Region

#Region "Box Whisker"
        Protected Sub BoxWhiskerPlot()
            Try
                If ((m_Data Is Nothing) OrElse (m_Data.Rows.Count < 1)) Then 'OrElse ((m_DataSet Is Nothing) OrElse (m_DataSet.Tables.Count < 1)) Then
                    gPane4.CurveList.Clear()
                    gPane4.GraphObjList.Clear()
                    gPane4.Title.Text = MessageStrings.No_Data_Plot
                    gPane4.XAxis.IsVisible = False
                    gPane4.YAxis.IsVisible = False
                    gPane4.Legend.IsVisible = False

                Else
                    Dim i As Integer

                    Dim numPts As Integer 'number of points in ptList
                    Dim xTitle As String 'the title of the XAxis
                    Dim medianList As PointPairList 'collection of Median points for the Box/Whisker plot
                    Dim meanList As PointPairList 'collection of Mean points for the Box/Whisker plot
                    Dim outlierList As PointPairList = Nothing 'collection of Outlier points for the Box/Whisker plot
                    Dim boxes As BoxPlot() = Nothing 'collection of boxes to draw
                    Dim xAxisLabels As String() = Nothing 'collection of labels for the x-Axis
                    Dim medianLine As LineItem 'zedgraph curve item -> line that contains all of the Medain points
                    Dim meanLine As LineItem 'zedgraph curve item -> line that contains all of the Mean points
                    Dim outlierLine As LineItem 'zedgraph curve item -> line that contains all of the outliers
                    Dim min, max As Double 'the max,Min value
                    Dim showXTics As Boolean = True 'tracks if showing major tic marks along the x-axis

                    gPane4.Legend.IsVisible = False
                    '1. set the Graph Pane, graphics object
                    gPane4.CurveList.Clear()
                    gPane4.GraphObjList.Clear()

                    '4. Calculate Data for the correct type of BoxPlot
                    'get all the rows from the table with positive data, order by Date
                    medianList = New PointPairList()
                    meanList = New PointPairList()
                    min = 100
                    max = 0
                    Select Case m_Options.BoxWhiskerMethod
                        Case BoxWhiskerType.Monthly
                            'Calculate Boxplot for Monthly data
                            numPts = CalcBoxPlot_Monthly(medianList, meanList, boxes, xAxisLabels, min, max)
                            xTitle = "Month"
                            showXTics = True
                        Case BoxWhiskerType.Seasonal
                            'Calculate Boxplot for seasonal data
                            numPts = CalcBoxPlot_Seasonal(medianList, meanList, boxes, xAxisLabels, min, max)
                            xTitle = "Season"
                            showXTics = True
                        Case BoxWhiskerType.Yearly
                            'Calculate Boxplot for Yearly data
                            numPts = CalcBoxPlot_Yearly(medianList, meanList, boxes, xAxisLabels, min, max)
                            xTitle = "Year"
                            showXTics = True
                        Case BoxWhiskerType.Overall
                            'Calculate Boxplot for All data
                            numPts = CalcBoxPlot_Overall(medianList, meanList, boxes, xAxisLabels, min, max)
                            xTitle = "Overall"
                            showXTics = False
                        Case Else
                            'Calculate Boxplot for Monthly data
                            numPts = CalcBoxPlot_Monthly(medianList, meanList, boxes, xAxisLabels, min, max)
                            xTitle = "Month"
                            showXTics = True
                    End Select

                    '5. Set Graph Properties

                    'x-axis
                    gPane4.XAxis.IsVisible = True
                    gPane4.XAxis.Type = AxisType.Text
                    gPane4.XAxis.Scale.TextLabels = xAxisLabels
                    gPane4.XAxis.Title.Text = xTitle
                    gPane4.XAxis.MajorTic.IsAllTics = showXTics
                    'y-axis
                    gPane4.YAxis.IsVisible = True
                    gPane4.YAxis.MajorGrid.IsVisible = True
                    gPane4.YAxis.MajorGrid.Color = Color.Gray
                    'gPane4.YAxis.Type = ZedGraph.AxisType.Linear
                    gPane4.YAxis.Title.Text = m_Var
                    gPane4.YAxis.Scale.Min = min - (0.2 * m_StdDev)
                    gPane4.YAxis.Scale.Max = max + (0.2 * m_StdDev)
                    gPane4.YAxis.Scale.MaxGrace = 0 '0.025 '2.5% 
                    gPane4.YAxis.Scale.MinGrace = 0 '0.025 '2.5%

                    gPane4.Title.Text = m_Site

                    '6. Plot the Data
                    If numPts > 0 Then
                        'Add Median line to plot
                        medianLine = gPane4.AddCurve("MedianPts", medianList, Color.Black, SymbolType.Circle)
                        medianLine.Line.IsVisible = False
                        medianLine.Symbol.Fill = New Fill(Color.Black)
                        medianLine.Symbol.Size = 5
                        'Add Mean line to plot
                        meanLine = gPane4.AddCurve("MeanPts", meanList, Color.Red, SymbolType.Triangle)
                        meanLine.Line.IsVisible = False
                        meanLine.Symbol.Fill = New Fill(Color.Red)
                        meanLine.Symbol.Size = 5
                        'Draw BoxPlot,Outliers around each point
                        If Not (outlierList Is Nothing) Then
                            outlierList.Clear()
                        Else
                            outlierList = New PointPairList
                        End If
                        For i = 0 To numPts - 1
                            If Not (boxes(i) Is Nothing) Then
                                'Draw Box/Whisker
                                DrawBoxPlot(gPane4, boxes(i))
                                'set Outliers Points
                                DrawOutliers(outlierList, boxes(i))
                            End If
                        Next i
                        'draw the Outliers on plot
                        outlierLine = gPane4.AddCurve("Outliers", outlierList, Color.DarkGreen, SymbolType.Circle)
                        outlierLine.Line.IsVisible = False
                        outlierLine.Symbol.Fill = New Fill(Color.DarkGreen)
                        outlierLine.Symbol.Size = 4
                        outlierLine.IsOverrideOrdinal = True

                    Else
                        gPane4.XAxis.Scale.Min = 0
                        gPane4.XAxis.Scale.Max = 10
                        gPane4.YAxis.Scale.Min = 0
                        gPane4.YAxis.Scale.Max = 10
                    End If



                    'draw the plot
                    gPane4.XAxis.IsVisible = True
                    gPane4.YAxis.IsVisible = True
                    'zgSummaryPlot.RestoreScale(zgSummaryPlot.GraphPane)
                    'zgBoxWhiskerPlot.AxisChange()
                    'zgBoxWhiskerPlot.Refresh()
                End If
            Catch ex As Exception
                Throw New Exception("Error Occured in ZGBoxWhisker.RenderGraph" & vbCrLf & ex.Message)
            End Try

        End Sub

        Private Function CalcBoxPlot_Monthly(ByRef medianPtList As PointPairList, ByRef meanPtList As PointPairList, ByRef boxes As BoxPlot(), ByRef xAxisLabels As String(), ByRef min As Double, ByRef max As Double) As Integer
            'This function calculates the Mean and Median point lists, boxes, and x-axis lables for the Monthly Box Plot, it returns the number of points created
            'Inputs:  medianPtList (ByRef) -> the zedgraph PointPairList to Plot the Median values -> values are calculated in this function : input values are junk
            '         meanPtList (ByRef) -> the zedgraph PointPairList to Plot the Median values -> values are calculated in this function : input values are junk
            '         boxes (ByRef) -> the collection of data for drawing each box -> values are calculated in this function : input values are junk
            '         xAxisLabels (ByRef) -> the collection of labels for the x-Axis -> values are calcaulted in this funciton : input values are junk
            '         min (byRef) -> the minimum value -> value is calculated in this function : input value is initialized
            '         max (byRef) -> the maximum value -> value is calculated in this function : input value is initialized  
            'Outputs: Integer -> the number of points (boxes) in the point list being returned
            '         medianPtList (ByRef) -> the Median Point values for each month
            '         meanPtList (ByRef) -> the Mean Point values for each month
            '         boxes (ByRef) -> the calculated values for drawing a box around each point
            '         xAxisLabels (ByRef) -> the set of labels for the x-Axis -> 1 for each month
            '         min (byRef) -> the minimum value of the whisker/outliers
            '         max (byRef) -> the maximum value of the whisker/outliers
            Const numMonths As Integer = 12 'number of months in the year -> Monthly will always have this many points
            Dim i, j As Integer 'counter
            Dim monthData As DataTable 'clone of m_VisPlotData -> used to pull the data for each individual month 
            Dim validRows As DataRow() = Nothing 'collection of valid data for the current month
            Dim numValid As Integer 'number of valid rows retrieved
            Try
                '1. Create the Mean, Median point list
                If Not (medianPtList) Is Nothing Then
                    If medianPtList.Count > 0 Then
                        medianPtList.Clear()
                    End If
                Else
                    medianPtList = New PointPairList()
                End If
                If Not (meanPtList) Is Nothing Then
                    If meanPtList.Count > 0 Then
                        meanPtList.Clear()
                    End If
                Else
                    meanPtList = New PointPairList()
                End If

                '2. Create Axis labels
                xAxisLabels = CreateMonthLabels()

                '3. Get data for each month, calculate stats
                ReDim boxes(numMonths - 1)
                monthData = m_Data.Clone()
                For i = 0 To numMonths - 1
                    '4. get the data for the current month
                    'validRows = m_VisPlotData.Select(db_fld_ValDTMonth & " = " & (i + 1) & " AND " & db_fld_ValCensorCode & " <> " & db_val_valCensorCode_lt, db_fld_ValValue & " ASC")
                    'NOTE: INCLUDE the censored data
                    validRows = m_Data.Select(db_outFld_ValDTMonth & " = " & (i + 1), "DataValue ASC")
                    numValid = validRows.Length()
                    'see if have any points for this month
                    If numValid > 0 Then
                        'add the data for this month to monthData
                        If monthData.Rows.Count > 0 Then
                            monthData.Clear()
                        End If
                        For j = 0 To numValid - 1
                            monthData.ImportRow(validRows(j))
                        Next j
                        '5. calculate stats on data
                        If (boxes(i) Is Nothing) Then
                            boxes(i) = New BoxPlot
                        End If
                        CalcBoxPlotStats(numValid, monthData, boxes(i))

                        '6. calculate,add the point to ptList
                        'set x,y values for this box
                        boxes(i).xValue = i + 1
                        boxes(i).yValue = boxes(i).median
                        'add the point
                        medianPtList.Add(i, boxes(i).yValue, xAxisLabels(i) & ", " & "Median = " & boxes(i).yValue)
                        meanPtList.Add(i, boxes(i).mean, xAxisLabels(i) & ", " & "Mean = " & boxes(i).mean)

                        '7. Calc Outliers
                        'set the min,max to Lower,Upper Adjacent Values
                        'min
                        If boxes(i).adjacentLevel_Lower < min Then
                            min = boxes(i).adjacentLevel_Lower
                        End If
                        'max
                        If boxes(i).adjacentLevel_Upper > max Then
                            max = boxes(i).adjacentLevel_Upper
                        End If
                        'Calculate the Outliers for this set of data, set min,max values to min,Max Outlier values
                        CalcBoxPlotOutliers(numValid, monthData, boxes(i), min, max)
                    Else
                        medianPtList.Add(i, PointPair.Missing)
                        meanPtList.Add(i, PointPair.Missing)
                    End If
                Next i

                '8. Release resources
                If Not (monthData Is Nothing) Then
                    monthData.Dispose()
                    'monthData = Nothing
                End If
                If Not (validRows Is Nothing) Then
                    ReDim validRows(0)
                    'validRows = Nothing
                End If

                '9. return the number of points created
                Return numMonths
            Catch ex As Exception
                Return -1
                'ShowError("An Error occurred while calculating the Monthly Box Plot values. " & vbCrLf & "Message = " & ex.Message)
            End Try
            'return that none were created
            Return 0
        End Function

        Private Function CalcBoxPlot_Seasonal(ByRef medianPtList As PointPairList, ByRef meanPtList As PointPairList, ByRef boxes As BoxPlot(), ByRef xAxisLabels As String(), ByRef min As Double, ByRef max As Double) As Integer
            'This function calculates the point list, boxes, and x-axis lables for the Seasonal Box Plot, it returns the number of points created
            'Inputs:  medianPtList (ByRef) -> the zedgraph PointPairList to Plot the Median values -> values are calculated in this function : input values are junk
            '         meanPtList (ByRef) -> the zedgraph PointPairList to Plot the Median values -> values are calculated in this function : input values are junk
            '         boxes (ByRef) -> the collection of data for drawing each box -> values are calculated in this function : input values are junk
            '         xAxisLabels (ByRef) -> the collection of labels for the x-Axis -> values are calcaulted in this funciton : input values are junk
            '         min (byRef) -> the minimum value -> value is calculated in this function : input value is initialized
            '         max (byRef) -> the maximum value -> value is calculated in this function : input value is initialized  
            'Outputs: Integer -> the number of points (boxes) in the point list being returned
            '         medianPtList (ByRef) -> the Median Point values for each Season
            '         meanPtList (ByRef) -> the Mean Point values for each Season
            '         boxes (ByRef) -> the calculated values for drawing a box around each point
            '         xAxisLabels (ByRef) -> the set of labels for the x-Axis -> 1 for each month
            '         min (byRef) -> the minimum value of the whisker/outliers
            '         max (byRef) -> the maximum value of the whisker/outliers
            Const numSeasons As Integer = 4 'number of Seasons in the year -> Seasonal will always have this many points
            Dim i, j As Integer 'counter
            Dim seasonData As DataTable 'clone of m_VisPlotData -> used to pull the data for each individual month 
            Dim validRows As DataRow() = Nothing 'collection of valid data for the current month
            Dim numValid As Integer 'number of valid rows retrieved
            Try
                '1. Create the Mean, Median point list
                If Not (medianPtList) Is Nothing Then
                    If medianPtList.Count > 0 Then
                        medianPtList.Clear()
                    End If
                Else
                    medianPtList = New PointPairList()
                End If
                If Not (meanPtList) Is Nothing Then
                    If meanPtList.Count > 0 Then
                        meanPtList.Clear()
                    End If
                Else
                    meanPtList = New PointPairList()
                End If

                '2. Create Axis labels
                xAxisLabels = CreateSeasonLabels()

                '3. Get data for each month, calculate stats
                ReDim boxes(numSeasons - 1)
                seasonData = m_Data.Clone()
                For i = 0 To numSeasons - 1
                    '4. get the data for the current season
                    'validRows = m_VisPlotData.Select("(" & db_fld_ValDTMonth & " = " & ((3 * i) + 1) & " OR " & db_fld_ValDTMonth & " = " & ((3 * i) + 2) & " OR " & db_fld_ValDTMonth & " = " & ((3 * i) + 3) & ") AND (" & db_fld_ValCensorCode & " <> " & db_val_valCensorCode_lt & ")", db_fld_ValValue & " ASC")
                    'NOTE: INCLUDE the censored data
                    validRows = m_Data.Select("(" & db_outFld_ValDTMonth & " = " & ((3 * i) + 1) & " OR " & db_outFld_ValDTMonth & " = " & ((3 * i) + 2) & " OR " & db_outFld_ValDTMonth & " = " & ((3 * i) + 3) & ")", "DataValue ASC")
                    numValid = validRows.Length()
                    'see if have any points for this month
                    If numValid > 0 Then
                        'add the data for this month to monthData
                        If seasonData.Rows.Count > 0 Then
                            seasonData.Clear()
                        End If
                        For j = 0 To numValid - 1
                            seasonData.ImportRow(validRows(j))
                        Next j
                        '5. calculate stats on data
                        If (boxes(i) Is Nothing) Then
                            boxes(i) = New BoxPlot
                        End If
                        CalcBoxPlotStats(numValid, seasonData, boxes(i))

                        '6. calculate,add the point to ptList
                        'set x,y values for this box
                        boxes(i).xValue = i + 1
                        boxes(i).yValue = boxes(i).median
                        'add the point
                        medianPtList.Add(i, boxes(i).yValue, xAxisLabels(i) & ", " & "Median = " & boxes(i).yValue)
                        meanPtList.Add(i, boxes(i).mean, xAxisLabels(i) & ", " & "Mean = " & boxes(i).mean)

                        '7. Calc Outliers
                        'set the min,max to Lower,Upper Adjacent Values
                        'min
                        If boxes(i).adjacentLevel_Lower < min Then
                            min = boxes(i).adjacentLevel_Lower
                        End If
                        'max
                        If boxes(i).adjacentLevel_Upper > max Then
                            max = boxes(i).adjacentLevel_Upper
                        End If
                        'Calculate the Outliers for this set of data, set min,max values to min,Max Outlier values
                        CalcBoxPlotOutliers(numValid, seasonData, boxes(i), min, max)
                    Else
                        medianPtList.Add(i, PointPair.Missing)
                        meanPtList.Add(i, PointPair.Missing)
                    End If
                Next i

                '8. Release resources
                If Not (seasonData Is Nothing) Then
                    seasonData.Dispose()
                    'seasonData = Nothing
                End If
                If Not (validRows Is Nothing) Then
                    ReDim validRows(0)
                    'validRows = Nothing
                End If

                '9. return the number of points created
                Return numSeasons
            Catch ex As Exception
                Return -1
                'ShowError("An Error occurred while calculating the Seasonal Box Plot values. " & vbCrLf & "Message = " & ex.Message)
            End Try
            'return that none were created
            Return 0
        End Function

        Private Function CalcBoxPlot_Yearly(ByRef medianPtList As PointPairList, ByRef meanPtList As PointPairList, ByRef boxes As BoxPlot(), ByRef xAxisLabels As String(), ByRef min As Double, ByRef max As Double) As Integer
            'This function calculates the point list, boxes, and x-axis lables for the Yearly Box Plot, it returns the number of points created
            'Inputs:  medianPtList (ByRef) -> the zedgraph PointPairList to Plot the Median values -> values are calculated in this function : input values are junk
            '         meanPtList (ByRef) -> the zedgraph PointPairList to Plot the Median values -> values are calculated in this function : input values are junk
            '         boxes (ByRef) -> the collection of data for drawing each box -> values are calculated in this function : input values are junk
            '         xAxisLabels (ByRef) -> the collection of labels for the x-Axis -> values are calcaulted in this funciton : input values are junk
            '         min (byRef) -> the minimum value -> value is calculated in this function : input value is initialized
            '         max (byRef) -> the maximum value -> value is calculated in this function : input value is initialized  
            'Outputs: Integer -> the number of points (boxes) in the point list being returned
            '         medianPtList (ByRef) -> the Median Point values for each year
            '         meanPtList (ByRef) -> the Mean Point values for each year
            '         boxes (ByRef) -> the calculated values for drawing a box around each point
            '         xAxisLabels (ByRef) -> the set of labels for the x-Axis -> 1 for each Year
            '         min (byRef) -> the minimum value of the whisker/outliers
            '         max (byRef) -> the maximum value of the whisker/outliers
            Dim numYears As Integer = 0 'number of years in selected data
            Dim i, j As Integer 'counter
            Dim yearData As DataTable 'clone of m_VisPlotData -> used to pull the data for each individual month 
            Dim validRows As DataRow() = Nothing 'collection of valid data for the current month
            Dim numValid As Integer = Nothing 'number of valid rows retrieved
            Dim startYear As Integer 'the beginning year of the data
            Dim endYear As Integer 'the ending year of the data
            Dim curYear As Integer 'the current year evaluating data for
            Try
                '1. Create Axis labels
                numYears = CreateYearLabels(xAxisLabels, startYear, endYear)
                'make sure there is at least 1 year
                If numYears <= 0 Then
                    'return 0
                    Exit Try
                End If

                '2. Create the Mean, Median point list
                If Not (medianPtList) Is Nothing Then
                    If medianPtList.Count > 0 Then
                        medianPtList.Clear()
                    End If
                Else
                    medianPtList = New PointPairList()
                End If
                If Not (meanPtList) Is Nothing Then
                    If meanPtList.Count > 0 Then
                        meanPtList.Clear()
                    End If
                Else
                    meanPtList = New PointPairList()
                End If

                '3. Get data for each month, calculate stats
                ReDim boxes(numYears - 1)
                yearData = m_Data.Clone()
                For i = 0 To numYears - 1
                    '4. get the data for the current year
                    'TODO: Michelle: adjust this for Year Data
                    curYear = CInt(xAxisLabels(i))
                    'validRows = m_VisPlotData.Select(db_fld_ValDTYear & " = " & (curYear) & " AND " & db_fld_ValCensorCode & " <> " & db_val_valCensorCode_lt, db_fld_ValValue & " ASC")
                    'NOTE: INCLUDE censored data
                    validRows = m_Data.Select(db_outFld_ValDTYear & " = " & (curYear), "DataValue ASC")
                    numValid = validRows.Length()
                    'see if have any points for this month
                    If numValid > 0 Then
                        'add the data for this month to monthData
                        If yearData.Rows.Count > 0 Then
                            yearData.Clear()
                        End If
                        For j = 0 To numValid - 1
                            yearData.ImportRow(validRows(j))
                        Next j
                        '5. calculate stats on data
                        If (boxes(i) Is Nothing) Then
                            boxes(i) = New BoxPlot
                        End If
                        CalcBoxPlotStats(numValid, yearData, boxes(i))

                        '6. calculate,add the point to ptList
                        'set x,y values for this box
                        boxes(i).xValue = i + 1
                        boxes(i).yValue = boxes(i).median
                        'add the point
                        medianPtList.Add(i, boxes(i).yValue, xAxisLabels(i) & ", " & "Median = " & boxes(i).yValue)
                        meanPtList.Add(i, boxes(i).mean, xAxisLabels(i) & ", " & "Mean = " & boxes(i).mean)

                        '7. Calc Outliers
                        'set the min,max to Lower,Upper Adjacent Values
                        'min
                        If boxes(i).adjacentLevel_Lower < min Then
                            min = boxes(i).adjacentLevel_Lower
                        End If
                        'max
                        If boxes(i).adjacentLevel_Upper > max Then
                            max = boxes(i).adjacentLevel_Upper
                        End If
                        'Calculate the Outliers for this set of data, set min,max values to min,Max Outlier values
                        CalcBoxPlotOutliers(numValid, yearData, boxes(i), min, max)
                    Else
                        medianPtList.Add(i, PointPair.Missing)
                        meanPtList.Add(i, PointPair.Missing)
                    End If
                Next i

                '8. Release resources
                If Not (yearData Is Nothing) Then
                    yearData.Dispose()
                    'yearData = Nothing
                End If
                If Not (validRows Is Nothing) Then
                    ReDim validRows(0)
                    'validRows = Nothing
                End If

                '9. return the number of points created
                Return numYears
            Catch ex As Exception
                Return -1
                'ShowError("An Error occurred while calculating the Yearly Box Plot values. " & vbCrLf & "Message = " & ex.Message)
            End Try
            'return that none were created
            Return 0
        End Function

        Private Function CalcBoxPlot_Overall(ByRef medianPtList As PointPairList, ByRef meanPtList As PointPairList, ByRef boxes As BoxPlot(), ByRef xAxisLabels As String(), ByRef min As Double, ByRef max As Double) As Integer
            'This function calculates the point list, boxes, and x-axis labels for the Overall Box Plot, it returns the number of points created
            'Inputs:  medianPtList (ByRef) -> the zedgraph PointPairList to Plot the Median value -> value is calculated in this function : input values are junk
            '         meanPtList (ByRef) -> the zedgraph PointPairList to Plot the Median value -> value is calculated in this function : input values are junk
            '         boxes (ByRef) -> the collection of data for drawing each box -> values are calculated in this function : input values are junk
            '         xAxisLabels (ByRef) -> the collection of labels for the x-Axis -> values are calcaulted in this funciton : input values are junk
            '         min (byRef) -> the minimum value -> value is calculated in this function : input value is initialized
            '         max (byRef) -> the maximum value -> value is calculated in this function : input value is initialized  
            'Outputs: Integer -> the number of points (boxes) in the point list being returned
            '         medianPtList (ByRef) -> the overall Median Point value
            '         meanPtList (ByRef) -> the overall Mean Point value
            '         boxes (ByRef) -> the calculated values for drawing a box around each point
            '         xAxisLabels (ByRef) -> the set of labels for the x-Axis -> 1 total
            '         min (byRef) -> the minimum value of the whisker/outliers
            '         max (byRef) -> the maximum value of the whisker/outliers
            Const numPts As Integer = 5 'number of months in the year -> Overall will always have this many points
            Dim validRows() As DataRow
            Dim numValid As Integer 'number of valid rows retrieved
            Dim overallData As DataTable
            Dim i As Integer
            Try
                '1. Create the Mean, Median point list
                If Not (medianPtList) Is Nothing Then
                    If medianPtList.Count > 0 Then
                        medianPtList.Clear()
                    End If
                Else
                    medianPtList = New PointPairList()
                End If
                If Not (meanPtList) Is Nothing Then
                    If meanPtList.Count > 0 Then
                        meanPtList.Clear()
                    End If
                Else
                    meanPtList = New PointPairList()
                End If

                '2. Create Axis labels
                ReDim xAxisLabels(4)
                'xAxisLabels(2) = "Overall"

                '3. Get data for each month, calculate stats
                ReDim boxes(numPts - 1)
                overallData = m_Data.Clone()
                '4. get the valid data 
                'TODO: Michelle: !!Find out if ONLY retrieving Valid values -> censorCode <> "lt", or all values!!
                validRows = m_Data.Select("", "DataValue ASC")
                numValid = validRows.Length()
                'see if have any points 
                If numValid > 0 Then
                    'add the data to overallData
                    For i = 0 To numValid - 1
                        overallData.ImportRow(validRows(i))
                    Next i
                    '5. calculate stats on data
                    If (boxes(2) Is Nothing) Then
                        boxes(2) = New BoxPlot
                    End If
                    CalcBoxPlotStats(numValid, overallData, boxes(2))

                    '6. calculate,add the point to ptList
                    'set x,y values for this box
                    boxes(2).xValue = 3
                    boxes(2).yValue = boxes(2).median
                    'add the points
                    medianPtList.Add(1, PointPair.Missing)
                    meanPtList.Add(1, PointPair.Missing)
                    medianPtList.Add(2, PointPair.Missing)
                    meanPtList.Add(2, PointPair.Missing)
                    medianPtList.Add(3, boxes(2).yValue, xAxisLabels(2) & ", " & "Median = " & boxes(2).yValue)
                    meanPtList.Add(3, boxes(2).mean, xAxisLabels(2) & ", " & "Mean = " & boxes(2).mean)
                    medianPtList.Add(4, PointPair.Missing)
                    meanPtList.Add(4, PointPair.Missing)
                    medianPtList.Add(5, PointPair.Missing)
                    meanPtList.Add(5, PointPair.Missing)

                    '7. Calc Outliers
                    'set the min,max to Lower,Upper Adjacent Values
                    'min
                    If boxes(2).adjacentLevel_Lower < min Then
                        min = boxes(2).adjacentLevel_Lower
                    End If
                    'max
                    If boxes(2).adjacentLevel_Upper > max Then
                        max = boxes(2).adjacentLevel_Upper
                    End If
                    'Calculate the Outliers for this set of data, set min,max values to min,Max Outlier values
                    CalcBoxPlotOutliers(numValid, m_Data, boxes(2), min, max)
                End If

                '8. return the number of points created
                Return numPts
            Catch ex As Exception
                Return -1
                'ShowError("An Error occurred while calculating the Overall Box Plot values. " & vbCrLf & "Message = " & ex.Message)
            End Try
            'return that none were created
            Return 0
        End Function

        Private Sub CalcBoxPlotOutliers(ByVal numRows As Integer, ByRef monthData As DataTable, ByRef boxData As BoxPlot, ByRef min As Double, ByRef max As Double)
            '
            'Inputs:  
            'Outputs: 
            Dim i As Integer 'counter
            Dim curValue As Double 'current value checking
            Try
                '1. move through values 
                For i = 0 To numRows - 1
                    'get the value
                    curValue = monthData.Rows(i).Item("DataValue")
                    '2. find those that are below the Lower Adjacent Level -> add them to boxData.Outliers_Lower
                    If (curValue < boxData.adjacentLevel_Lower) Then
                        boxData.AddOutlier_Lower(curValue)
                    End If
                    '3. find those that are above the Upper Adjacent Level -> add them to boxData.Outliers_Upper
                    If (curValue > boxData.adjacentLevel_Upper) Then
                        boxData.AddOutlier_Upper(curValue)
                    End If
                    '4. compare to min,max
                    'min
                    If curValue < min Then
                        min = curValue
                    End If
                    'max
                    If curValue > max Then
                        max = curValue
                    End If
                Next i
            Catch ex As Exception
                Clear()
                'ShowError("An Error occurred while calculating the the Outliers for the Box Plot on the Visualize Tab." & vbCrLf & "Message = " & ex.Message)
            End Try
        End Sub

        Private Sub DrawBoxPlot(ByRef gPane As GraphPane, ByVal boxData As BoxPlot)
            'Dim x1, y1, x2, y2 As Double 'x,y values for bounds of rectangles
            Dim upperBoxShaded As BoxObj 'shaded box between 75% quantile and Upper 95% Confidence Limit on the Median
            Dim lowerBoxShaded As BoxObj 'shaded box between 25% quantile and Lower 95% Confidence Limit on the Median
            Dim hourglassPts As PointD() 'points for the Hourglass outline to make up the box outline
            Dim hourglassOutline As PolyObj 'Outline for the Hourglass
            Dim confIntervalLine As LineObj 'Line in center -> 95% Confidence Interval on the Mean
            Dim whisker_Upper As LineObj 'Upper Whisker -> Upper Adjacent Level
            Dim lineToWhisker_Upper As LineObj 'Line from Upper Whisker to 75% quantile (top of box)
            Dim whisker_Lower As LineObj 'Lower Whisker -> Lower Adjacent Level
            Dim lineToWhisker_Lower As LineObj 'Line from Lower Whisker to 25% quantil (bottom of box)
            Try
                '1. Draw Confidence Interval -> red line
                confIntervalLine = New LineObj(Color.Red, boxData.xValue, boxData.confidenceInterval95_Upper, boxData.xValue, boxData.confidenceInterval95_Lower)
                confIntervalLine.IsClippedToChartRect = True
                confIntervalLine.ZOrder = ZOrder.E_BehindCurves
                gPane.GraphObjList.Add(confIntervalLine)

                '2. Draw Upper Whisker, line
                'whisker
                whisker_Upper = New LineObj(Color.Black, boxData.xValue - 0.15, boxData.adjacentLevel_Upper, boxData.xValue + 0.15, boxData.adjacentLevel_Upper)
                whisker_Upper.IsClippedToChartRect = True
                whisker_Upper.ZOrder = ZOrder.E_BehindCurves
                whisker_Upper.Line.Width = 2
                gPane.GraphObjList.Add(whisker_Upper)
                'line between whisker, top of hourglass
                lineToWhisker_Upper = New LineObj(Color.Black, boxData.xValue, boxData.adjacentLevel_Upper, boxData.xValue, boxData.quantile_75th)
                lineToWhisker_Upper.IsClippedToChartRect = True
                lineToWhisker_Upper.ZOrder = ZOrder.E_BehindCurves
                lineToWhisker_Upper.Line.Width = 2
                gPane.GraphObjList.Add(lineToWhisker_Upper)

                '3. Draw Lower Whisker, line
                'whisker
                whisker_Lower = New LineObj(Color.Black, boxData.xValue - 0.15, boxData.adjacentLevel_Lower, boxData.xValue + 0.15, boxData.adjacentLevel_Lower)
                whisker_Lower.IsClippedToChartRect = True
                whisker_Lower.ZOrder = ZOrder.E_BehindCurves
                whisker_Lower.Line.Width = 2
                gPane.GraphObjList.Add(whisker_Lower)
                'line between whisker, top of hourglass
                lineToWhisker_Lower = New LineObj(Color.Black, boxData.xValue, boxData.quantile_25th, boxData.xValue, boxData.adjacentLevel_Lower)
                lineToWhisker_Lower.IsClippedToChartRect = True
                lineToWhisker_Lower.ZOrder = ZOrder.E_BehindCurves
                lineToWhisker_Lower.Line.Width = 2
                gPane.GraphObjList.Add(lineToWhisker_Lower)

                '4. Draw Hourglass outline
                'create points
                ReDim hourglassPts(10)
                'top
                hourglassPts(0) = New PointD(boxData.xValue - 0.3, boxData.quantile_75th)
                hourglassPts(1) = New PointD(boxData.xValue + 0.3, boxData.quantile_75th)
                'right side
                hourglassPts(2) = New PointD(boxData.xValue + 0.3, boxData.confidenceLimit95_Upper)
                hourglassPts(3) = New PointD(boxData.xValue + 0.15, boxData.median)
                hourglassPts(4) = New PointD(boxData.xValue + 0.3, boxData.confidenceLimit95_Lower)
                'bottom
                hourglassPts(5) = New PointD(boxData.xValue + 0.3, boxData.quantile_25th)
                hourglassPts(6) = New PointD(boxData.xValue - 0.3, boxData.quantile_25th)
                'left side
                hourglassPts(7) = New PointD(boxData.xValue - 0.3, boxData.confidenceLimit95_Lower)
                hourglassPts(8) = New PointD(boxData.xValue - 0.15, boxData.median)
                hourglassPts(9) = New PointD(boxData.xValue - 0.3, boxData.confidenceLimit95_Upper)
                'repeat 1st point -> end of poly
                hourglassPts(10) = New PointD(boxData.xValue - 0.3, boxData.quantile_75th)
                'create outline
                hourglassOutline = New PolyObj(hourglassPts)
                hourglassOutline.Border.Color = Color.SlateGray
                hourglassOutline.Border.IsVisible = True
                hourglassOutline.Fill.IsVisible = False
                hourglassOutline.IsClippedToChartRect = True
                hourglassOutline.ZOrder = ZOrder.E_BehindCurves
                gPane.GraphObjList.Add(hourglassOutline)

                '5. Draw Upper shaded box ->Upper 95% Confidence Limit to 75% quantile value
                If boxData.quantile_75th > boxData.confidenceLimit95_Upper Then
                    upperBoxShaded = New BoxObj(1, 1, 1, 1)
                    'If upperBoxShaded.Location Is Nothing Then
                    '    upperBoxShaded.Location = New ZedGraph.Location()
                    'End If
                    upperBoxShaded.Location.X = boxData.xValue - 0.3
                    upperBoxShaded.Location.Width = 0.6
                    upperBoxShaded.Location.Y = boxData.quantile_75th
                    upperBoxShaded.Location.Height = -Math.Round(boxData.quantile_75th - boxData.confidenceLimit95_Upper, 3)
                    'Else
                    'upperBoxShaded.Location.Y = boxData.confidenceLimit95_Upper
                    'upperBoxShaded.Location.Height = Math.Round(boxData.quantile_75th - boxData.confidenceLimit95_Upper, 3)

                    upperBoxShaded.Border.IsVisible = False
                    upperBoxShaded.Fill = New Fill(Color.LightGray)
                    upperBoxShaded.IsClippedToChartRect = True
                    upperBoxShaded.ZOrder = ZOrder.E_BehindCurves
                    gPane.GraphObjList.Add(upperBoxShaded)
                End If
                '6. Draw Lower shaded box ->Lower 95% Confidence Limit to 25% quantile value
                If boxData.confidenceLimit95_Lower > boxData.quantile_25th Then
                    lowerBoxShaded = New BoxObj(1, 1, 1, 1)
                    'If upperBoxShaded.Location Is Nothing Then
                    '    upperBoxShaded.Location = New ZedGraph.Location()
                    'End If
                    lowerBoxShaded.Location.X = boxData.xValue - 0.3
                    lowerBoxShaded.Location.Width = 0.6

                    lowerBoxShaded.Location.Y = boxData.confidenceLimit95_Lower
                    lowerBoxShaded.Location.Height = -Math.Round(boxData.confidenceLimit95_Lower - boxData.quantile_25th, 3)
                    'Else
                    'lowerBoxShaded.Location.Y = boxData.quantile_25th
                    'lowerBoxShaded.Location.Height = Math.Round(boxData.confidenceLimit95_Lower - boxData.quantile_25th, 3)
                    lowerBoxShaded.Border.IsVisible = False
                    lowerBoxShaded.Fill = New Fill(Color.LightGray)
                    lowerBoxShaded.IsClippedToChartRect = True
                    lowerBoxShaded.ZOrder = ZOrder.E_BehindCurves
                    gPane.GraphObjList.Add(lowerBoxShaded)
                End If

                '7. Release resources
                If Not (hourglassPts Is Nothing) Then
                    ReDim hourglassPts(0)
                    'hourglassPts = Nothing
                End If
                'If Not (hourglassOutline Is Nothing) Then
                'hourglassOutline = Nothing
                'End If
                'If Not (upperBoxShaded Is Nothing) Then
                'upperBoxShaded = Nothing
                'End If
                'If Not (lowerBoxShaded Is Nothing) Then
                'lowerBoxShaded = Nothing
                'End If
                'If Not (confIntervalLine Is Nothing) Then
                'IntervalLine = Nothing
                'End If
                'If Not (whisker_Upper Is Nothing) Then
                'whisker_Upper = Nothing
                'End If
                'If Not (lineToWhisker_Upper Is Nothing) Then
                'lineToWhisker_Upper = Nothing
                'End If
                'If Not (whisker_Lower Is Nothing) Then
                'whisker_Lower = Nothing
                'End If
                'If Not (lineToWhisker_Lower Is Nothing) Then
                'lineToWhisker_Lower = Nothing
                'End If
            Catch ex As Exception
                Clear()
                'ShowError("An Error occurred while drawing a Box/Whisker for the Box Plot. " & vbCrLf & "Message = " & ex.Message)
            End Try
        End Sub

        Private Sub DrawOutliers(ByRef outlierPtList As PointPairList, ByVal curBoxData As BoxPlot) 'ByRef gPane As ZedGraph.GraphPane, ByVal boxData As clsBoxPlot)
            Dim i As Integer 'counter
            Dim curValue As Double

            Try
                '1. validate data
                If curBoxData Is Nothing Then
                    Exit Try
                End If
                If outlierPtList Is Nothing Then
                    outlierPtList = New PointPairList
                End If

                '2. Add Lower Outliers
                If curBoxData.numOutliers_Lower > 0 Then
                    For i = 0 To curBoxData.numOutliers_Lower - 1
                        curValue = curBoxData.outlierValue_Lower(i)
                        If curValue <> -1 Then
                            outlierPtList.Add(curBoxData.xValue, curValue)
                        End If
                    Next i
                End If

                '3. Draw Upper Outliers
                If curBoxData.numOutliers_Upper > 0 Then
                    For i = 0 To curBoxData.numOutliers_Upper - 1
                        curValue = curBoxData.outlierValue_Upper(i)
                        If curValue <> -1 Then
                            'add the point
                            outlierPtList.Add(curBoxData.xValue, curValue)
                        End If
                    Next i
                End If
            Catch ex As Exception
                Clear()
                'ShowError("An Error occurred while drawing the Outliers for the Box Plot." & vbCrLf & "Message = " & ex.Message)
            End Try
        End Sub

        Private Function CreateMonthLabels() As String()
            Dim labels(11) As String
            'set the labels for each month
            labels(0) = "Jan"
            labels(1) = "Feb"
            labels(2) = "Mar"
            labels(3) = "Apr"
            labels(4) = "May"
            labels(5) = "Jun"
            labels(6) = "Jul"
            labels(7) = "Aug"
            labels(8) = "Sep"
            labels(9) = "Oct"
            labels(10) = "Nov"
            labels(11) = "Dec"

            Return labels
        End Function

        Private Function CreateSeasonLabels() As String()
            Dim labels(3) As String
            'set the labels for each month
            labels(0) = "Winter"
            labels(1) = "Spring"
            labels(2) = "Summer"
            labels(3) = "Fall"

            Return labels
        End Function

        Private Function CreateYearLabels(ByRef labels As String(), ByRef startYear As Integer, ByRef endYear As Integer) As Integer
            Dim i As Integer 'counter
            Dim numYears As Integer 'count of years in selected data
            Dim curYear As Integer 'current year creating a label for
            Try
                '1. calculate the start,end year
                startYear = m_Data.Compute("Min(" & db_outFld_ValDTYear & ")", "")
                endYear = m_Data.Compute("Max(" & db_outFld_ValDTYear & ")", "")

                '2. calculate the number of years
                numYears = (endYear - startYear) + 1

                '3. create the labels -> one for each year
                ReDim labels(numYears - 1)
                For i = 0 To numYears - 1
                    curYear = startYear + i
                    If (curYear >= startYear) AndAlso (curYear <= endYear) Then
                        labels(i) = curYear
                    End If
                Next i

                Return numYears
            Catch ex As Exception
                Return -1
                'ShowError("An Error occurred while creating the Year labels for the Box/Whisker Plot." & vbCrLf & "Message = " & ex.Message)
            End Try
            'return 0
            Return 0
        End Function

        Private Sub CalcBoxPlotStats(ByVal numRows As Integer, ByRef data As DataTable, ByRef boxData As BoxPlot)
            'Calculates and stores the stats for the given set of data for a BoxPlot
            'Inputs:  data (ByRef) -> the set of data to calculate the stats on
            '         boxData (ByRef) -> the clsBoxPlot object to store the calculated data into -> NOTE: the xValue should have already been set
            'Outputs: data (ByRef) -> 
            '         boxData (ByRef) -> the calculated stats so can draw the boxPlot at this point
            Dim variance As Double 'the variance of the values in data
            Dim stdDev As Double 'the standard deviation of the values in data
            Dim max As Double 'the maximum value in data
            Dim min As Double 'the minimum value in data
            Dim v As Double 'used to calculate the lower,upper whiskers (adjacent levels)
            Try
                'make sure have data
                If numRows = 0 OrElse data.Rows.Count <= 0 Then
                    Exit Try
                End If

                '1. Calculate the 25% and 75% quantile values
                boxData.quantile_25th = data.Rows(Math.Floor(numRows / 4)).Item("DataValue")
                boxData.quantile_75th = data.Rows(Math.Floor(numRows / 4) * 3).Item("DataValue")

                '2. calculate the Mean, Median
                'Mean
                boxData.mean = Math.Round(data.Compute("Avg(DataValue)", ""), 3)
                'Median
                If (numRows Mod 2 = 0 And numRows > 1) Then
                    'even number of values -> take the middle two and average them
                    Dim val1 As Double 'first of the middle values -> (numRows/2) - 1
                    Dim val2 As Double 'second of the middle value -> (numrows/2)
                    val1 = data.Rows((numRows / 2) - 1).Item("DataValue")
                    val2 = data.Rows(numRows / 2).Item("DataValue")
                    boxData.median = (val1 + val2) / 2
                Else
                    boxData.median = data.Rows(Math.Ceiling((numRows / 2) - 1)).Item("DataValue")
                End If

                '3. Calculate the upper and lower whiskers
                max = data.Compute("Max(DataValue)", "")
                min = data.Compute("Min(DataValue)", "")
                v = (boxData.quantile_75th - boxData.quantile_25th) * 1.5
                'lower whisker -> Lower Adjacent Level
                If (boxData.quantile_25th - v) < min Then
                    boxData.adjacentLevel_Lower = min
                Else
                    boxData.adjacentLevel_Lower = boxData.quantile_25th - v
                End If
                'upper whisker -> Upper Adjacent Level
                If (boxData.quantile_75th + v) > max Then
                    boxData.adjacentLevel_Upper = max
                Else
                    boxData.adjacentLevel_Upper = boxData.quantile_75th + v
                End If

                '4. Calculate the 95% Confidence Interval on the Mean,Calculate the 95% Confidence Limit on the Median
                If Not (data.Compute("Var(DataValue)", "") Is DBNull.Value) Then
                    variance = data.Compute("Var(DataValue)", "")
                    stdDev = Math.Sqrt(variance)
                Else
                    stdDev = 0
                End If
                'Confidence Interval on Mean
                boxData.confidenceInterval95_Lower = boxData.mean - 1.96 * (stdDev / (Math.Sqrt(numRows)))
                boxData.confidenceInterval95_Upper = boxData.mean + 1.96 * (stdDev / (Math.Sqrt(numRows)))
                'Confidence Limit on Median
                boxData.confidenceLimit95_Lower = boxData.median - 1.96 * (stdDev / (Math.Sqrt(numRows)))
                boxData.confidenceLimit95_Upper = boxData.median + 1.96 * (stdDev / (Math.Sqrt(numRows)))
            Catch ex As Exception
                Clear()
                'ShowError("An Error occurred while calculating the Box Plot Statistics for the Visualize Tab. " & vbCrLf & "Message = " & ex.Message)
            End Try
        End Sub
#End Region

    End Class
End Namespace