Imports System.Windows.Forms
Imports System.Drawing
Imports HydroDesktop.Database
Imports HydroDesktop.Interfaces
Imports GraphView.My.Resources
Imports HydroDesktop.Interfaces.ObjectModel
Imports ZedGraph

Namespace Controls

    Public Class ProbabilityPlot
        Implements IChart

        Private Shared ReadOnly m_VarList As New List(Of String)
        Private m_SeriesSelector As ISeriesSelector
        'the main series selector control
        Public Property SeriesSelector() As ISeriesSelector
            Get
                Return m_SeriesSelector
            End Get
            Set(ByVal value As ISeriesSelector)
                m_SeriesSelector = value
            End Set
        End Property

        Public Function CurveCount() As Int32
            Return zgProbabilityPlot.GraphPane.CurveList.Count
        End Function

        Public Sub SetGraphPaneTitle(ByVal title As String)
            zgProbabilityPlot.GraphPane.Title.Text = title
        End Sub

        Public Sub Plot(ByRef options As OneSeriesPlotInfo, Optional ByVal e_StdDev As Double = 0)
            Try
                Dim m_VariableWithUnits = options.VariableName & " - " & options.VariableUnits
                m_VarList.Add(m_VariableWithUnits)
                PlotProbability(options)
            Catch ex As Exception
                Throw New Exception("Error Occured in ZGProbability.Plot" & vbCrLf & ex.Message)
            End Try
        End Sub

        Public Sub Clear()
            Try
                Dim gPane As GraphPane = zgProbabilityPlot.GraphPane
                gPane.CurveList.Clear()
                gPane.Title.Text = MessageStrings.No_Data_Plot
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

        Private Sub PlotProbability(ByRef options As OneSeriesPlotInfo)

            Dim m_Data = options.DataTable.Copy
            Dim m_Site = options.SiteName
            Dim m_VariableWithUnits = options.VariableName & " - " & options.VariableUnits
            Dim m_Options = options.PlotOptions

            Dim i As Integer 'counter
            Dim gPane As GraphPane 'GraphPane of the zgProbability plot object -> used to set data and characteristics
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

                '1. Set the Graph Pane, graphics object
                gPane = zgProbabilityPlot.GraphPane

                'get all data(even censored ones), order by Value
                validRows = m_Data.Select("", "DataValue ASC")
                numRows = validRows.GetLength(0)

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
                gPane.XAxis.Type = AxisType.Linear
                gPane.XAxis.Scale.IsVisible = False
                gPane.XAxis.Scale.Min = -4.0
                gPane.XAxis.Scale.Max = 4.0
                gPane.XAxis.MajorTic.IsAllTics = False
                gPane.XAxis.Scale.MinGrace = 0
                gPane.XAxis.Scale.MaxGrace = 0

                'Title
                While (GetStringLen(m_Site, gPane.Title.FontSpec.GetFont(gPane.CalcScaleFactor)) > zgProbabilityPlot.Width)
                    m_Site = GraphTitleBreaks(m_Site)
                End While

                'Setting title
                gPane.Title.Text = m_VariableWithUnits & vbCrLf & " at " & m_Site

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
                probLine = gPane.AddCurve(m_Site, ptList, m_Options.GetLineColor, SymbolType.Circle)
                probLine.Tag = options
                probLine.Symbol.Fill = New Fill(m_Options.GetPointColor, Color.Black)
                probLine.Symbol.Fill.RangeMin = 0
                probLine.Symbol.Fill.RangeMax = 1
                probLine.Symbol.Size = 4
                probLine.Symbol.Fill.SecondaryValueGradientColor = Color.Empty
                probLine.Symbol.Fill.Type = FillType.GradientByColorValue
                probLine.Symbol.Border.IsVisible = False
                probLine.Line.IsVisible = False

                Select Case m_Options.TimeSeriesMethod
                    Case TimeSeriesType.Line
                        probLine.Line.IsVisible = True
                        probLine.Symbol.IsVisible = False
                    Case TimeSeriesType.Point
                        probLine.Line.IsVisible = False
                        probLine.Symbol.IsVisible = True
                    Case TimeSeriesType.None
                        probLine.Line.IsVisible = False
                        probLine.Symbol.IsVisible = False
                    Case Else
                        probLine.Line.IsVisible = True
                        probLine.Symbol.IsVisible = True
                End Select

                'Setting Legend Title
                Dim needShowDataType = False
                For Each c In zgProbabilityPlot.GraphPane.CurveList
                    Dim cOptions = DirectCast(c.Tag, OneSeriesPlotInfo)

                    For Each cc In zgProbabilityPlot.GraphPane.CurveList
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
                    probLine.Label.Text = options.SiteName + ", " + options.VariableName + ", ID: " + options.SeriesID.ToString
                Else
                    ' Update legend for all curves
                    For Each c In zgProbabilityPlot.GraphPane.CurveList
                        Dim cOptions = DirectCast(c.Tag, OneSeriesPlotInfo)
                        c.Label.Text = cOptions.SiteName + ", " + cOptions.VariableName + ", " + cOptions.DataType + ", ID: " + cOptions.SeriesID.ToString
                    Next
                End If

                'Setting Y Axis
                probLine.Link.Title = m_VariableWithUnits

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

        Private Sub zgProbabilityPlot_ContextMenuBuilder(ByVal sender As ZedGraphControl, ByVal menuStrip As ContextMenuStrip, ByVal mousePt As Point, ByVal objState As ZedGraphControl.ContextMenuObjectState) Handles zgProbabilityPlot.ContextMenuBuilder
            ' from http://zedgraph.org/wiki/index.php?title=Edit_the_Context_Menu
            ' Create a new menu item
            Dim item As ToolStripMenuItem = New ToolStripMenuItem()
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
                MessageBox.Show("No Data To Export")

            Else

                'Build a datatable to export
                Dim exportTable As DataTable = New DataTable

                'Build datatable for each series and then add all series' datatable to the exportTable

                Dim repo = RepositoryFactory.Instance.Get(Of IDataValuesRepository)()
                For count As Integer = 1 To checkedSeries

                    'Build a datatable as "totalData" for each series

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

                    Dim values = repo.GetValues(SeriesSelector.CheckedIDList(count - 1)).OrderBy(Function(x) x)
                    Dim numRow = values.Count()

                    'Add non-repeated frequency data into "totalData" datatable
                    For r As Integer = 0 To numRow - 1
                        Dim row_count As Integer = totalData.Rows.Count()
                        row = totalData.NewRow()
                        row(0) = row_count - 2
                        row(1) = values(r)
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
                Dim exportPlugin = Common.PluginEntryPoint.App.Extensions.OfType(Of IDataExportPlugin).FirstOrDefault()
                If Not exportPlugin Is Nothing Then
                    exportPlugin.Export(exportTable)
                End If

            End If
        End Sub

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
                    .Title.Text = MessageStrings.No_Data_Plot
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
End Namespace