Imports Controls
Imports ZedGraph
Imports System.Drawing
Imports HydroDesktop.Database
Imports HydroDesktop.Interfaces


Public Class cProbabilityPlot
    Implements IChart
    
    Private Shared m_VarList As New List(Of String)
    Private m_SeriesSelector As ISeriesSelector
    Dim m_StdDev

    'the main series selector control
    Public Property SeriesSelector() As ISeriesSelector
        Get
            Return m_SeriesSelector
        End Get
        Set(ByVal value As ISeriesSelector)
            m_SeriesSelector = value
        End Set
    End Property

    Public Sub Plot(ByRef options As TimeSeriesPlotOptions, Optional ByVal e_StdDev As Double = 0)
        Try
            Dim m_Data = options.DataTable
            Dim m_VariableWithUnits = options.VariableName & " - " & options.VariableUnits
            m_VarList.Add(m_VariableWithUnits)

            If (e_StdDev = 0) And (Not (m_Data Is Nothing)) And (m_Data.Rows.Count > 0) Then
                m_StdDev = Statistics.StandardDeviation(m_Data)
            Else
                m_StdDev = e_StdDev
            End If
            PlotProbability(options)
        Catch ex As Exception
            Throw New Exception("Error Occured in ZGProbability.Plot" & vbCrLf & ex.Message)
        End Try
    End Sub

    Public Sub Clear()
        Try
            Dim gPane As GraphPane = zgProbabilityPlot.GraphPane
            gPane.CurveList.Clear()
            gPane.Title.Text = "No Data To Plot"
            gPane.XAxis.IsVisible = False
            gPane.YAxis.IsVisible = False
            gPane.GraphObjList.Clear()
            zgProbabilityPlot.IsShowVScrollBar = False
            zgProbabilityPlot.IsShowHScrollBar = False
            zgProbabilityPlot.Refresh()
        Catch ex As Exception
            Throw New Exception("Error Occured in ZGProbability.Clear" & vbCrLf & ex.Message)
        End Try
    End Sub


#Region " Probability "

    Private Sub PlotProbability(ByRef options As TimeSeriesPlotOptions)

        Dim m_Data = options.DataTable.Copy
        Dim m_Site = options.SiteName
        Dim m_VariableWithUnits = options.VariableName & " - " & options.VariableUnits
        Dim m_Options = options.PlotOptions

        Dim i As Integer 'counter
        Dim gPane As ZedGraph.GraphPane 'GraphPane of the zgProbability plot object -> used to set data and characteristics
        'Dim g As Drawing.Graphics 'graphics object of the zgProbability plot object -> used to redraw/update the plot
        Dim ptList As ZedGraph.PointPairList 'collection of points for the Probability plot
        Dim bflPtList As New ZedGraph.PointPairList
        Dim probLine As ZedGraph.LineItem
        Dim bflLine As New ZedGraph.LineItem("")
        Dim validRows() As Data.DataRow
        Dim numRows As Integer
        Dim curValue As Double
        Dim curX As Double
        Dim curFreq As Double
        Dim IsInYAxis As Boolean = False
        Try

            '1. Set the Graph Pane, graphics object
            gPane = zgProbabilityPlot.GraphPane
            'gPane.CurveList.Clear()
            'g = zg5Probability.CreateGraphics

            '2. Validate data
            'If m_Data Is Nothing Then
            'reset Title = No Data
            'gPane.Title.Text = "No Data To Plot"
            'zgProbabilityPlot.Refresh()
            'release resources
            'If Not (g Is Nothing) Then
            '	g.Dispose()
            '	g = Nothing
            'End If

            'exit
            'Exit Try
            'End If

            '3. let user know something is being plotted


            '4.get the data
            ''get all the rows from the table that are not censored, order by Value
            'validRows = m_VisPlotData.Select(db_fld_ValCensorCode & " <> " & db_val_valCensorCode_lt, db_fld_ValValue & " ASC") 'selected rows from m_VisPlotData that have censorcode <> "lt"

            'get all data(even censored ones), order by Value
            validRows = m_Data.Select("", "DataValue ASC")
            numRows = validRows.GetLength(0)
            'make sure data was selected
            'If (numRows = 0) Or (m_Data Is Nothing) Then
            '    'reset Title = No Data
            '    gPane.Title.Text = "No Data To Plot"
            '    gPane.XAxis.IsVisible = False
            '    gPane.YAxis.IsVisible = False
            '    gPane.GraphObjList.Clear()
            '    zgProbabilityPlot.IsShowVScrollBar = False
            '    zgProbabilityPlot.IsShowHScrollBar = False
            '    zgProbabilityPlot.Refresh()
            '    'release resources
            '    'If Not (g Is Nothing) Then
            '    '	g.Dispose()
            '    '	g = Nothing
            '    'End If
            '    'exit



            '    '5. Set Graph Properties
            'Else
            ''turn on the legend
            If m_Options.ShowLegend Then
                gPane.Legend.IsVisible = True
                gPane.Legend.Position = 12
            Else
                gPane.Legend.IsVisible = False
            End If
            ''turn on scroll bar
            zgProbabilityPlot.IsShowVScrollBar = True
            zgProbabilityPlot.IsShowHScrollBar = True
            zgProbabilityPlot.IsAutoScrollRange = True
            If gPane.IsZoomed() = True Then
                zgProbabilityPlot.ZoomOutAll(gPane)
            End If
            'x-axis
            gPane.XAxis.IsVisible = True
            gPane.XAxis.MajorTic.Size = 0
            gPane.XAxis.MinorTic.Size = 0
            gPane.XAxis.Title.Text = vbCrLf & vbCrLf & "Cumulative Frequency < Stated Value %"
            gPane.XAxis.Title.Gap = 0.2
            gPane.XAxis.Type = ZedGraph.AxisType.Linear
            gPane.XAxis.Scale.IsVisible = False
            gPane.XAxis.Scale.Min = -4.0
            gPane.XAxis.Scale.Max = 4.0
            gPane.XAxis.MajorTic.IsAllTics = False
            gPane.XAxis.Scale.MinGrace = 0
            gPane.XAxis.Scale.MaxGrace = 0
            ''y-axis
            'gPane.YAxis.IsVisible = True
            'gPane.YAxis.MajorGrid.IsVisible = True
            'gPane.YAxis.MajorGrid.Color = Drawing.Color.Gray
            'gPane.YAxis.Title.Text = variable & "  " & varUnits
            'gPane.YAxis.Type = ZedGraph.AxisType.Linear
            'gPane.YAxis.Scale.MinGrace = 0.025
            'gPane.YAxis.Scale.MaxGrace = 0.025
            'gPane.YAxis.Scale.MagAuto = False
            'Title
            While (GetStringLen(m_Site, gPane.Title.FontSpec.GetFont(gPane.CalcScaleFactor)) > zgProbabilityPlot.Width)
                m_Site = GraphTitleBreaks(m_Site)
            End While

            'Setting title
            'If Not (gPane.Title.Text = "Alarm! It is not good comparison (Different variables)") Then
            '    If (gPane.Title.Text Like (m_Var + "*")) Then
            '        gPane.Title.Text = m_Var
            '    ElseIf (gPane.Title.Text = "Title") Or _
            '    (gPane.Title.Text = "No Data To Plot") Then
            gPane.Title.Text = m_VariableWithUnits & vbCrLf & " at " & m_Site
            '        gPane.Legend.IsVisible = False
            '    Else
            '        gPane.Title.Text = "Alarm! It is not good comparison (Different variables)"
            '    End If
            'End If

            '6. Create the Pts for the Line
            ptList = New ZedGraph.PointPairList

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
                        If (p.Tag.ToString.ToLower = Statistics.NotCensored) Or (p.Tag.ToString.ToLower = Statistics.Unknown) Then
                            p.ColorValue = 0
                        Else
                            p.ColorValue = 1
                        End If
                        ptList.Add(p)
                    Else
                        If Not ((p.Tag.ToString.ToLower = Statistics.NotCensored) Or (p.Tag.ToString.ToLower = Statistics.Unknown)) Then
                            ptList.Add(p)
                        End If
                    End If
                End If
            Next i

            '7. Plot the Data
            'create the points
            'probLine = New ZedGraph.LineItem("ProbCurve")
            probLine = gPane.AddCurve(m_Site, ptList, m_Options.GetLineColor, ZedGraph.SymbolType.Circle)
            probLine.Tag = options
            probLine.Symbol.Fill = New Fill(m_Options.GetPointColor, m_Options.GetPointColor)
            probLine.Symbol.Fill.RangeMin = 0
            probLine.Symbol.Fill.RangeMax = 1
            probLine.Symbol.Size = 4
            probLine.Symbol.Fill.SecondaryValueGradientColor = Color.Empty
            probLine.Symbol.Fill.Type = FillType.GradientByColorValue
            probLine.Symbol.Border.IsVisible = False
            probLine.Line.IsVisible = False

            Select Case m_Options.TimeSeriesMethod
                Case PlotOptions.TimeSeriesType.Line
                    probLine.Line.IsVisible = True
                    probLine.Symbol.IsVisible = False
                Case PlotOptions.TimeSeriesType.Point
                    probLine.Line.IsVisible = False
                    probLine.Symbol.IsVisible = True
                Case PlotOptions.TimeSeriesType.None
                    probLine.Line.IsVisible = False
                    probLine.Symbol.IsVisible = False
                Case Else
                    probLine.Line.IsVisible = True
                    probLine.Symbol.IsVisible = True
            End Select

            'Setting Legend Title
            Dim needShowDataType = False
            For Each c In zgProbabilityPlot.GraphPane.CurveList
                Dim cOptions = DirectCast(c.Tag, TimeSeriesPlotOptions)

                For Each cc In zgProbabilityPlot.GraphPane.CurveList
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
                probLine.Label.Text = options.SiteName + ", " + options.VariableName + ", ID: " + options.SeriesID.ToString
            Else
                ' Update legend for all curves
                For Each c In zgProbabilityPlot.GraphPane.CurveList
                    Dim cOptions = DirectCast(c.Tag, TimeSeriesPlotOptions)
                    c.Label.Text = cOptions.SiteName + ", " + cOptions.VariableName + ", " + cOptions.DataType + ", ID: " + cOptions.SeriesID.ToString
                Next
            End If

            'Setting Y Axis
            probLine.Link.Title = m_VariableWithUnits
            'If gPane.CurveList.Count = 1 Then
            '    With gPane.YAxis
            '        .Scale.MagAuto = False
            '        .IsVisible = True
            '        .Title.Text = m_Var
            '        '.Scale.FontSpec.FontColor = probLine.Color
            '        '.Title.FontSpec.FontColor = probLine.Color
            '        '.Color = probLine.Color
            '        probLine.IsY2Axis = False
            '        probLine.YAxisIndex = 0
            '    End With
            'End If
            'i = 0
            'While Not i >= gPane.YAxisList.Count
            '    If gPane.YAxisList(i).Title.Text = probLine.Link.Title Then
            '        probLine.IsY2Axis = False
            '        probLine.YAxisIndex = i
            '        IsInYAxis = True
            '    End If
            '    i += 1
            'End While
            'i = 0
            'While Not i >= gPane.Y2AxisList.Count
            '    If gPane.Y2AxisList(i).Title.Text = probLine.Link.Title Then
            '        probLine.IsY2Axis = True
            '        probLine.YAxisIndex = i
            '        IsInYAxis = True
            '    End If
            '    i += 1
            'End While
            'If IsInYAxis = False Then
            '    If gPane.Y2AxisList(0).Title.Text = "" Then

            '        With gPane.Y2AxisList(0)


            '            '.Scale.FontSpec.FontColor = probLine.Color
            '            '.Title.FontSpec.FontColor = probLine.Color
            '            '.Color = probLine.Color
            '            .IsVisible = True
            '            .Scale.MagAuto = False

            '            .MajorTic.IsInside = False
            '            .MinorTic.IsInside = False
            '            .MajorTic.IsOpposite = False
            '            .MinorTic.IsOpposite = False

            '            .Scale.Align = AlignP.Inside

            '            .Title.Text = probLine.Link.Title

            '            probLine.IsY2Axis = True
            '            probLine.YAxisIndex = 0
            '        End With
            '    ElseIf gPane.YAxisList.Count > gPane.Y2AxisList.Count Then
            '        Dim newYAxis As New Y2Axis(probLine.Link.Title)
            '        gPane.Y2AxisList.Add(newYAxis)
            '        'newYAxis.Scale.FontSpec.FontColor = probLine.Color
            '        'newYAxis.Title.FontSpec.FontColor = probLine.Color
            '        'newYAxis.Color = probLine.Color
            '        newYAxis.IsVisible = True
            '        newYAxis.Scale.MagAuto = False

            '        newYAxis.MajorTic.IsInside = False
            '        newYAxis.MinorTic.IsInside = False
            '        newYAxis.MajorTic.IsOpposite = False
            '        newYAxis.MinorTic.IsOpposite = False

            '        newYAxis.Scale.Align = AlignP.Inside

            '        newYAxis.Title.Text = probLine.Link.Title

            '        probLine.IsY2Axis = True
            '        probLine.YAxisIndex = gPane.Y2AxisList.Count - 1
            '    Else
            '        Dim newYAxis As New YAxis(probLine.Link.Title)
            '        gPane.YAxisList.Add(newYAxis)
            '        'newYAxis.Scale.FontSpec.FontColor = probLine.Color
            '        'newYAxis.Title.FontSpec.FontColor = probLine.Color
            '        'newYAxis.Color = probLine.Color
            '        newYAxis.IsVisible = True
            '        newYAxis.Scale.MagAuto = False

            '        newYAxis.MajorTic.IsInside = False
            '        newYAxis.MinorTic.IsInside = False
            '        newYAxis.MajorTic.IsOpposite = False
            '        newYAxis.MinorTic.IsOpposite = False

            '        newYAxis.Scale.Align = AlignP.Inside

            '        newYAxis.Title.Text = probLine.Link.Title

            '        probLine.IsY2Axis = False
            '        probLine.YAxisIndex = gPane.YAxisList.Count - 1
            '    End If
            'End If




            '8. set up Tic Marks

            For i = 0 To 20
                AddLabelToPlot(gPane, GetProbabilityLabel(i), GetProbabilityValue(i))
            Next i

            'set up scrolling 
            zgProbabilityPlot.IsAutoScrollRange = False
            zgProbabilityPlot.ScrollMinX = -4.0
            zgProbabilityPlot.ScrollMaxX = 4.0
            zgProbabilityPlot.ScrollMinY = 0
            zgProbabilityPlot.ScrollMaxY = 1.025 * m_Data.Compute("MAX(DataValue)", "")
            'draw the plot
            'zgProbabilityPlot.AxisChange()
            'zgProbabilityPlot.Refresh()
            SettingYAsixs()
            SettingTitle()

            '9. Release resources
            If Not (ptList Is Nothing) Then
                ptList = Nothing
            End If
            If Not (bflPtList Is Nothing) Then
                bflPtList = Nothing
            End If
            If Not (probLine Is Nothing) Then
                probLine = Nothing
            End If
            If Not (bflLine Is Nothing) Then
                bflLine = Nothing
            End If
            If Not (validRows Is Nothing) Then
                validRows = Nothing
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
        Catch ex As System.Exception
            Throw New Exception("An Error occurred while calculating the X-Position for a point in the Probability Plot." & vbCrLf & "Message= " & ex.Message, ex)
        End Try
    End Function

    Private Function CalculateProbabilityFreq(ByVal rank As Integer, ByVal numRows As Integer) As Double
        Try
            Return Math.Round((rank - 0.375) / (numRows + 1 - 2 * (0.375)), 3)
        Catch ex As Exception

        End Try
    End Function

    Private Function CreateProbabilityXAxisLabels(ByVal pane As ZedGraph.GraphPane, ByVal axis As ZedGraph.Axis, ByVal val As Double, ByVal index As Integer) As String
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

#End Region

    Private Sub zgProbabilityPlot_ContextMenuBuilder(ByVal sender As ZedGraph.ZedGraphControl, ByVal menuStrip As System.Windows.Forms.ContextMenuStrip, ByVal mousePt As System.Drawing.Point, ByVal objState As ZedGraph.ZedGraphControl.ContextMenuObjectState) Handles zgProbabilityPlot.ContextMenuBuilder
        ' from http://zedgraph.org/wiki/index.php?title=Edit_the_Context_Menu
        ' Create a new menu item
        Dim item As System.Windows.Forms.ToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        ' This is the user-defined Tag so you can find this menu item later if necessary
        item.Name = "export_to_text_file"
        item.Tag = "export_to_text_file"
        ' This is the text that will show up in the menu
        item.Text = "Export Frequency Data"
        ' Add a handler that will respond when that menu item is selected
        AddHandler item.Click, AddressOf ExportToTextFile
        ' Add the menu item to the menu
        menuStrip.Items.Add(item)
    End Sub
    Protected Sub ExportToTextFile()
        'Check selected series
        Dim checkedSeries As Integer = SeriesSelector.CheckedIDList.Count()

        'Check if there is any series to export
        If (checkedSeries <= 0) Then
            System.Windows.Forms.MessageBox.Show("No Data To Export")

        Else

            'Build a datatable to export
            Dim exportTable As DataTable = New DataTable

            'Build datatable for each series and then add all series' datatable to the exportTable
            For count As Integer = 1 To checkedSeries

                'Build a datatable as "totalData" for each series
                Dim conn = HydroDesktop.Configuration.Settings.Instance.DataRepositoryConnectionString
                Dim dbOperation As New DbOperations(conn, DatabaseTypes.SQLite)
                Dim totalData As DataTable = New DataTable

                'Add columns in the table
                Dim column1 As DataColumn = New DataColumn("Number")
                Dim column2 As DataColumn = New DataColumn("Value")
                Dim column3 As DataColumn = New DataColumn("Frequency")

                totalData.Columns.Add(column1)
                totalData.Columns.Add(column2)
                totalData.Columns.Add(column3)

                'Add sites information for the series
                Dim row As DataRow
                row = totalData.NewRow()
                'TODO: use the series name
                row(0) = SeriesSelector.CheckedIDList(count - 1)
                totalData.Rows.Add(row)

                row = totalData.NewRow()
                row(0) = m_VarList(count - 1)
                totalData.Rows.Add(row)

                Dim head(2) As Object
                head(0) = "Number"
                head(1) = "Value"
                head(2) = "Frequency (%)"
                row = totalData.NewRow()
                row.ItemArray = head
                totalData.Rows.Add(row)

                'Select datavalue from database
                Dim dataValue As DataTable = New DataTable
                Dim sql As String
                sql = "SELECT DataValue FROM DataValues WHERE SeriesID = " & SeriesSelector.CheckedIDList(count - 1)
                dataValue = dbOperation.LoadTable("value", sql)

                Dim validRow() As DataRow
                Dim numRow As Integer
                'Order all values in ascending sequence
                validRow = dataValue.Select("", "DataValue ASC")
                numRow = validRow.GetLength(0)

                'Add non-repeated frequency data into "totalData" datatable
                For r As Integer = 0 To numRow - 1
                    Dim row_count As Integer = totalData.Rows.Count()
                    row = totalData.NewRow()
                    row(0) = row_count - 2
                    row(1) = validRow(r).Item("DataValue")
                    row(2) = CalculateProbabilityFreq(r + 1, numRow) * 100

                    If r = 0 Then
                        totalData.Rows.Add(row)
                    ElseIf row(2) = totalData.Rows(row_count - 1).Item(2) Or row(1) = totalData.Rows(row_count - 1).Item(1) Then

                    Else
                        totalData.Rows.Add(row)
                    End If
                Next r

                'Add or append totalData into exportTable
                If count = 1 Then
                    exportTable = totalData.Copy()
                Else
                    exportTable.Merge(totalData, True)
                End If

            Next count

            'Export Data
            Dim exportForm As HydroDesktop.ImportExport.ExportDataTableToTextFileDialog = New HydroDesktop.ImportExport.ExportDataTableToTextFileDialog(exportTable)
            exportForm.ShowDialog()

        End If
    End Sub

    Private Sub AddLabelToPlot(ByRef gpane As ZedGraph.GraphPane, ByVal label As String, ByVal xLoc As Double)
        Dim myText As ZedGraph.TextObj
        Dim myTic As ZedGraph.TextObj
        Try
            myText = New ZedGraph.TextObj(label, xLoc, 1.05, ZedGraph.CoordType.XScaleYChartFraction)
            myText.FontSpec.Size = 13
            myText.FontSpec.Border.IsVisible = False
            myText.FontSpec.Fill = New ZedGraph.Fill(Drawing.Color.FromArgb(25, Drawing.Color.White))
            gpane.GraphObjList.Add(myText)
            myTic = New ZedGraph.TextObj("|", xLoc, 0.997, ZedGraph.CoordType.XScaleYChartFraction)
            myTic.FontSpec.Size = 12.0
            myTic.FontSpec.Border.IsVisible = False
            myTic.FontSpec.Fill = New ZedGraph.Fill(Drawing.Color.FromArgb(25, Drawing.Color.White))
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

    Public Sub New()

        ' This call is required by the Windows Form Designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        zgProbabilityPlot.GraphPane.Legend.IsVisible = False
        zgProbabilityPlot.GraphPane.Border.IsVisible = False
    End Sub

    Public Sub Refreshing()
        zgProbabilityPlot.AxisChange()
        zgProbabilityPlot.Refresh()
    End Sub

    Public Sub Remove(ByVal curveIndex As Integer)
        'added by jiri to prevent unhandled exception
        If zgProbabilityPlot.GraphPane.CurveList.Count = 0 Then
            Return
        End If
        Dim IsExist As Boolean = False

        Dim CurveListCopy As New CurveList
        For i = 0 To zgProbabilityPlot.GraphPane.CurveList.Count - 1
            CurveListCopy.Add(zgProbabilityPlot.GraphPane.CurveList(i))
        Next
        'zgProbabilityPlot.GraphPane.CurveList(curveIndex).Clear()
        zgProbabilityPlot.GraphPane.CurveList.Clear()

        For i = 0 To CurveListCopy.Count - 1
            If Not (i = curveIndex) Then
                zgProbabilityPlot.GraphPane.CurveList.Add(CurveListCopy(i))
            End If
        Next

        'Remove Y Axis
        'With zgProbabilityPlot.GraphPane
        '    For i = 0 To .YAxisList.Count - 1
        '        IsExist = False
        '        For j = 0 To .CurveList.Count - 1
        '            If .CurveList(j).Link.Title = .YAxisList(i).Title.Text Then
        '                IsExist = True
        '            End If
        '        Next
        '        If Not IsExist Then
        '            If .YAxisList.Count = 1 Then
        '                .YAxisList.Remove(.YAxisList(i))
        '                .YAxisList.Add("")
        '                Exit For
        '            Else
        '                .YAxisList.Remove(.YAxisList(i))
        '                Exit For
        '            End If
        '        End If
        '    Next
        '    For i = 0 To .Y2AxisList.Count - 1
        '        IsExist = False
        '        For j = 0 To .CurveList.Count - 1
        '            If .CurveList(j).Link.Title = .Y2AxisList(i).Title.Text Then
        '                IsExist = True
        '            End If
        '        Next
        '        If Not IsExist Then
        '            If .Y2AxisList.Count = 1 Then
        '                .Y2AxisList.Remove(.Y2AxisList(i))
        '                .Y2AxisList.Add("")
        '                Exit For
        '            Else
        '                .Y2AxisList.Remove(.Y2AxisList(i))
        '                Exit For
        '            End If
        '        End If
        '    Next
        'End With

        SettingYAsixs()
        SettingTitle()
    End Sub

    Private Sub SettingTitle()
        Dim IsSame As Boolean = True

        With zgProbabilityPlot.GraphPane
            .Title.IsVisible = True
            If .CurveList.Count > 1 Then
                For i As Integer = 1 To .CurveList.Count - 1
                    If Not .CurveList(i).Link.Title = .CurveList(i - 1).Link.Title Then
                        IsSame = False
                    End If
                Next
                If IsSame Then
                    .Title.Text = .CurveList(0).Link.Title
                Else
                    .Title.IsVisible = False
                End If
            ElseIf .CurveList.Count = 1 Then
                '.Title.Text = .CurveList(0).Link.Title
                .Legend.IsVisible = False
            ElseIf .CurveList.Count = 0 Then
                .Title.Text = "No Data To Plot"
            End If

        End With
    End Sub

    Private Sub SettingYAsixs()
        Dim AsixsList As New ArrayList()

        With zgProbabilityPlot.GraphPane

            For i As Integer = 0 To .CurveList.Count - 1
                If Not AsixsList.Contains(.CurveList(i).Link.Title) Then
                    AsixsList.Add(.CurveList(i).Link.Title)
                End If
            Next

            .YAxisList.Clear()
            .Y2AxisList.Clear()
            For i As Integer = 0 To AsixsList.Count - 1
                If i Mod 2 = 0 Then
                    Dim NewAsix As New YAxis()
                    NewAsix.Title.Text = AsixsList(i)
                    NewAsix.MajorTic.IsInside = False
                    NewAsix.MinorTic.IsInside = False
                    NewAsix.MajorTic.IsOpposite = False
                    NewAsix.MinorTic.IsOpposite = False
                    .YAxisList.Add(NewAsix)
                Else
                    Dim NewAsix As New Y2Axis()
                    NewAsix.Title.Text = AsixsList(i)
                    NewAsix.MajorTic.IsInside = False
                    NewAsix.MinorTic.IsInside = False
                    NewAsix.MajorTic.IsOpposite = False
                    NewAsix.MinorTic.IsOpposite = False
                    .Y2AxisList.Add(NewAsix)
                End If
            Next

            If .YAxisList.Count = 0 Then
                .YAxisList.Add("")
            End If
            If .Y2AxisList.Count = 0 Then
                .Y2AxisList.Add("")
            End If

            For i As Integer = 0 To .CurveList.Count - 1

                For j As Integer = 0 To .YAxisList.Count - 1
                    If .CurveList(i).Link.Title = .YAxisList(j).Title.Text Then
                        .CurveList(i).IsY2Axis = False
                        .CurveList(i).YAxisIndex = j
                    End If
                Next

                For j As Integer = 0 To .Y2AxisList.Count - 1
                    If .CurveList(i).Link.Title = .Y2AxisList(j).Title.Text Then
                        .CurveList(i).IsY2Axis = True
                        .CurveList(i).YAxisIndex = j
                    End If
                Next

            Next
            .Y2Axis.IsVisible = True
        End With

    End Sub

    Public Property ShowPointValues() As Boolean Implements IChart.ShowPointValues
        Get
            Return zgProbabilityPlot.IsShowPointValues
        End Get
        Set(ByVal value As Boolean)
            zgProbabilityPlot.IsShowPointValues = value
        End Set
    End Property

    Public Sub ZoomIn() Implements IChart.ZoomIn
        zgProbabilityPlot.ZoomIn()
    End Sub

    Public Sub ZoomOut() Implements IChart.ZoomOut
        zgProbabilityPlot.ZoomOut(zgProbabilityPlot.GraphPane)
    End Sub

    Public Sub ZoomOutAll() Implements IChart.ZoomOutAll
        zgProbabilityPlot.ZoomOutAll(zgProbabilityPlot.GraphPane)
    End Sub

End Class
