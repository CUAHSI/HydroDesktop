Imports ZedGraph
Imports System.Drawing
Imports HydroDesktop.Database
Imports HydroDesktop.Interfaces

Public Class cTimeSeriesPlot

    Public Sub New()
        ' This call is required by the Windows Form Designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        Dim gPane As GraphPane = zgTimeSeries.GraphPane
        gPane.XAxis.Type = AxisType.Date
        gPane.YAxis.Type = AxisType.Linear
        gPane.Border.IsVisible = False
        gPane.Legend.IsVisible = False
        SettingColor()
    End Sub


    Private Const XColumn As String = "LocalDateTime"
    Private Const YColumn As String = "DataValue"
    Private connString = HydroDesktop.Configuration.Settings.Instance.DataRepositoryConnectionString

    Private Shared m_Data As DataTable
    Private Shared m_Site As String
    Private Shared m_VariableWithUnits As String
    Private Shared m_Variable As String
    Private Shared m_Options As PlotOptions
    Private Shared m_SeriesID As Integer

    Public EditCurvePointList As PointPairList
    Public EditCurveLable As String
    Public EditCurveTitle As String

    Private ccList0 As New ArrayList()

    Private Sub SettingColor()
        ccList0.Clear()
        ccList0.Add(Color.FromArgb(106, 61, 154))
        ccList0.Add(Color.FromArgb(202, 178, 214))
        ccList0.Add(Color.FromArgb(255, 127, 0))
        ccList0.Add(Color.FromArgb(253, 191, 111))
        ccList0.Add(Color.FromArgb(227, 26, 28))
        ccList0.Add(Color.FromArgb(251, 154, 153))
        ccList0.Add(Color.FromArgb(51, 160, 44))
        ccList0.Add(Color.FromArgb(178, 223, 138))
        ccList0.Add(Color.FromArgb(31, 120, 180))
        ccList0.Add(Color.FromArgb(166, 206, 227))
    End Sub

    'the main series selector control
    'Public Property SeriesSelector() As SeriesSelector3
    '    Get
    '        Return m_SeriesSelector
    '    End Get
    '    Set(ByVal value As SeriesSelector3)
    '        m_SeriesSelector = value
    '    End Set
    'End Property


    Public Sub Plot(ByRef objDataTable As Data.DataTable, ByVal strSiteName As String, ByVal strVariableName As String, ByVal strVariableUnits As String, ByRef objOptions As PlotOptions, ByRef intSeriesID As Integer)
        Try
            m_Data = objDataTable.Copy
            m_Site = strSiteName
            m_Variable = strVariableName
            m_VariableWithUnits = strVariableName & " - " & strVariableUnits
            m_Options = objOptions
            m_SeriesID = intSeriesID

            Graph()

        Catch ex As Exception
            Throw New Exception("Error Occured in ZGTimeSeries.Plot" & vbCrLf & ex.Message)
        End Try
    End Sub

    Public Sub Replot(ByVal options As PlotOptions)
        Try
            m_Options = options
            Graph()
        Catch ex As Exception
            Throw New Exception("Error Occured in ZGTimeSeries.Replot" & vbCrLf & ex.Message)
        End Try
    End Sub

    Public Sub Clear()
        Try
            Dim gPane As GraphPane = zgTimeSeries.GraphPane
            If gPane Is Nothing Then Return
            If gPane.CurveList Is Nothing Then Return
            'm_Data.Clear()
            gPane.CurveList.Clear()
            gPane.XAxis.IsVisible = False
            gPane.YAxis.IsVisible = False
            zgTimeSeries.IsShowVScrollBar = False
            zgTimeSeries.IsShowHScrollBar = False
            gPane.Title.Text = "No Data To Plot"
            'Graph()

        Catch ex As Exception
            Throw New Exception("Error Occured in ZGTimeSeries.Clear" & vbCrLf & ex.Message)
        End Try
    End Sub

    Protected Sub Graph()
        Try
            Dim gPane As GraphPane = zgTimeSeries.GraphPane
            Dim i As Integer = 0
            Dim IsInYAxis As Boolean = False
            'gPane.CurveList.Clear()


            If (m_Data Is Nothing) Or (m_Data.Rows.Count <= 0) Then
                gPane.XAxis.IsVisible = False
                gPane.YAxis.IsVisible = False
                zgTimeSeries.IsShowVScrollBar = False
                zgTimeSeries.IsShowHScrollBar = False
                gPane.Title.Text = "No Data To Plot"
            Else
                'Setting Legend
                gPane.Legend.IsVisible = True
                gPane.Legend.Position = 12


                'Setting Scroll and scale
                zgTimeSeries.ZoomOutAll(gPane)
                zgTimeSeries.IsShowVScrollBar = True
                zgTimeSeries.IsShowHScrollBar = True
                zgTimeSeries.IsAutoScrollRange = True
                'gPane.YAxis.Scale.MagAuto = False
                'Setting Axises
                gPane.XAxis.IsVisible = True
                gPane.XAxis.Title.Text = "Date and Time"
                'gPane.YAxis.IsVisible = True
                'gPane.YAxis.Title.Text = m_Var
                'Setting Title
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
                        If (p.Tag.ToString.ToLower = Statistics.NotCensored) Or (p.Tag.ToString.ToLower = Statistics.Unknown) Then
                            p.ColorValue = 0
                        Else
                            p.ColorValue = 1
                        End If
                        p.Tag = row.Item("ValueID").ToString + ", " + row.Item("LocalDateTime").ToString + ": " + row.Item("DataValue").ToString
                        pointList.Add(p)
                    Else
                        If Not ((p.Tag.ToString.ToLower = Statistics.NotCensored) Or (p.Tag.ToString.ToLower = Statistics.Unknown)) Then
                            pointList.Add(p)
                            p.Tag = row.Item("ValueID").ToString + ", " + row.Item("LocalDateTime").ToString + ": " + row.Item("DataValue").ToString
                        End If
                    End If
                Next row



                Dim curve As LineItem = gPane.AddCurve(m_Site, pointList, m_Options.GetLineColor, SymbolType.Circle)
                SettingCurveStyle(curve)
                Select Case m_Options.TimeSeriesMethod
                    Case PlotOptions.TimeSeriesType.Line
                        curve.Line.IsVisible = True
                        curve.Symbol.IsVisible = False
                    Case PlotOptions.TimeSeriesType.Point
                        curve.Line.IsVisible = False
                        curve.Symbol.IsVisible = True
                    Case PlotOptions.TimeSeriesType.None
                        curve.Line.IsVisible = False
                        curve.Symbol.IsVisible = False
                    Case Else
                        curve.Line.IsVisible = True
                        curve.Symbol.IsVisible = True
                End Select

                curve.Label.Text += ", " + m_Variable + ", ID: " + m_SeriesID.ToString

                'Setting Y Axis
                curve.Link.Title = m_VariableWithUnits
                If gPane.CurveList.Count = 1 Then
                    With gPane.YAxis
                        .Scale.MagAuto = False
                        .IsVisible = True
                        .Title.Text = m_VariableWithUnits
                        '.Scale.FontSpec.FontColor = curve.Color
                        '.Title.FontSpec.FontColor = curve.Color
                        '.Color = curve.Color
                        curve.IsY2Axis = False
                        curve.YAxisIndex = 0
                    End With
                End If
                While Not i >= gPane.YAxisList.Count
                    If gPane.YAxisList(i).Title.Text = curve.Link.Title Then
                        curve.IsY2Axis = False
                        curve.YAxisIndex = i
                        IsInYAxis = True
                    End If
                    i += 1
                End While
                i = 0
                While Not i >= gPane.Y2AxisList.Count
                    If gPane.Y2AxisList(i).Title.Text = curve.Link.Title Then
                        curve.IsY2Axis = True
                        curve.YAxisIndex = i
                        IsInYAxis = True
                    End If
                    i += 1
                End While
                If IsInYAxis = False Then
                    If gPane.Y2AxisList(0).Title.Text = "" Then

                        With gPane.Y2AxisList(0)


                            '.Scale.FontSpec.FontColor = curve.Color
                            '.Title.FontSpec.FontColor = curve.Color
                            '.Color = curve.Color
                            .IsVisible = True
                            .Scale.MagAuto = False

                            .MajorTic.IsInside = False
                            .MinorTic.IsInside = False
                            .MajorTic.IsOpposite = False
                            .MinorTic.IsOpposite = False

                            .Scale.Align = AlignP.Inside

                            .Title.Text = curve.Link.Title

                            curve.IsY2Axis = True
                            curve.YAxisIndex = 0
                        End With
                    ElseIf gPane.YAxisList.Count > gPane.Y2AxisList.Count Then
                        Dim newYAxis As New Y2Axis(curve.Link.Title)
                        gPane.Y2AxisList.Add(newYAxis)
                        'newYAxis.Scale.FontSpec.FontColor = curve.Color
                        'newYAxis.Title.FontSpec.FontColor = curve.Color
                        'newYAxis.Color = curve.Color
                        newYAxis.IsVisible = True
                        newYAxis.Scale.MagAuto = False

                        newYAxis.MajorTic.IsInside = False
                        newYAxis.MinorTic.IsInside = False
                        newYAxis.MajorTic.IsOpposite = False
                        newYAxis.MinorTic.IsOpposite = False

                        newYAxis.Scale.Align = AlignP.Inside

                        newYAxis.Title.Text = curve.Link.Title

                        curve.IsY2Axis = True
                        curve.YAxisIndex = gPane.Y2AxisList.Count - 1
                    Else
                        Dim newYAxis As New YAxis(curve.Link.Title)
                        gPane.YAxisList.Add(newYAxis)
                        'newYAxis.Scale.FontSpec.FontColor = curve.Color
                        'newYAxis.Title.FontSpec.FontColor = curve.Color
                        'newYAxis.Color = curve.Color
                        newYAxis.IsVisible = True
                        newYAxis.Scale.MagAuto = False

                        newYAxis.MajorTic.IsInside = False
                        newYAxis.MinorTic.IsInside = False
                        newYAxis.MajorTic.IsOpposite = False
                        newYAxis.MinorTic.IsOpposite = False

                        newYAxis.Scale.Align = AlignP.Inside

                        newYAxis.Title.Text = curve.Link.Title

                        curve.IsY2Axis = False
                        curve.YAxisIndex = gPane.YAxisList.Count - 1
                    End If

                End If
                'gPane.Legend.Fill = New Fill(m_Options.GetLineColor, Brushes.AliceBlue, FillType.None)
                'gPane.Legend.FontSpec.
            End If
            SettingTitle()
        Catch ex As Exception
            Throw New Exception("Error Occured in ZGTimeSeries.Graph" & vbCrLf & ex.Message)
        End Try


    End Sub

    Private Sub zgTimeSeries_ContextMenuBuilder(ByVal sender As ZedGraph.ZedGraphControl, ByVal menuStrip As System.Windows.Forms.ContextMenuStrip, ByVal mousePt As System.Drawing.Point, ByVal objState As ZedGraph.ZedGraphControl.ContextMenuObjectState) Handles zgTimeSeries.ContextMenuBuilder
        ' from http://zedgraph.org/wiki/index.php?title=Edit_the_Context_Menu

        ' Add item to export to text file
        ' Create a new menu item
        Dim item As System.Windows.Forms.ToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        ' This is the user-defined Tag so you can find this menu item later if necessary
        item.Name = "export_to_text_file"
        item.Tag = "export_to_text_file"
        ' This is the text that will show up in the menu
        item.Text = "Export to Text File"
        ' Add a handler that will respond when that menu item is selected
        'AddHandler item.Click, AddressOf ExportToTextFile
        ' Add the menu item to the menu
        menuStrip.Items.Add(item)

        ' Add item to export to change line color
        item = New System.Windows.Forms.ToolStripMenuItem()
        item.Name = "set_line_color"
        item.Tag = "set_line_color"
        item.Text = "Set Line Color"
        AddHandler item.Click, AddressOf SetLineColor
        menuStrip.Items.Add(item)
    End Sub

    Private Function PromptForColor(ByVal defaultColor As System.Drawing.Color) As System.Drawing.Color
        Dim dlgColor As System.Windows.Forms.ColorDialog = New System.Windows.Forms.ColorDialog()

        If Not IsDBNull(defaultColor) Then
            dlgColor.Color = defaultColor
        End If

        If (dlgColor.ShowDialog() = Windows.Forms.DialogResult.OK) Then
            Return dlgColor.Color
        Else
            Return Nothing
        End If
    End Function

    'Protected Sub ExportToTextFile()


    '    'Error checking
    '    If SeriesSelector Is Nothing Then Exit Sub

    '    'Check selected series
    '    Dim checkedSeries As Integer = SeriesSelector.checkedIDList.Count()

    '    'Check if there is any series to export
    '    If (checkedSeries <= 0) Then
    '        System.Windows.Forms.MessageBox.Show("No Data To Export")

    '    Else

    '        'Build a datatable to export
    '        Dim exportTable As DataTable = New DataTable

    '        Dim conn = Config.DataRepositoryConnectionString
    '        Dim dbOperation As New DbOperations(conn, DatabaseTypes.SQLite)
    '        Dim totalData As New Data.DataTable

    '        'Build datatable for each series and then add all series' datatable to the exportTable
    '        For count As Integer = 1 To checkedSeries

    '            Dim sql As String

    '            'Error checking
    '            If SeriesSelector Is Nothing Then Exit Sub

    '            Dim checkedSeriesID As Integer = SeriesSelector.checkedIDList(count - 1)

    '            sql = "SELECT ds.SeriesID, s.SiteName, v.VariableName, dv.DataValue, dv.LocalDateTime, U.UnitsName FROM DataSeries ds, Sites s, Variables v, DataValues dv, Units U WHERE v.VariableID = ds.VariableID AND s.SiteID = ds.SiteID AND dv.SeriesID = ds.SeriesID AND U.UnitsID = v.VariableUnitsID AND ds.SeriesID = " & checkedSeriesID
    '            totalData = dbOperation.LoadTable("DataValues", sql)

    '            If count = 1 Then
    '                exportTable = totalData.Copy()
    '            Else
    '                exportTable.Merge(totalData, True)
    '            End If

    '        Next count

    '        Dim exportForm As HydroDesktop.ImportExport.ExportDataTableToTextFileDialog = New HydroDesktop.ImportExport.ExportDataTableToTextFileDialog(exportTable)
    '        exportForm.ShowDialog()
    '    End If
    'End Sub

    Protected Sub SetLineColor()
        If zgTimeSeries.GraphPane.CurveList.Count > 0 Then
            Dim newColor As System.Drawing.Color = PromptForColor(zgTimeSeries.GraphPane.CurveList.Item(0).Color)
            If Not IsDBNull(newColor) Then
                zgTimeSeries.GraphPane.CurveList.Item(0).Color = newColor
                zgTimeSeries.Refresh()
            End If
        End If
    End Sub

    Public Sub Refreshing()
        zgTimeSeries.AxisChange()
        zgTimeSeries.Refresh()
    End Sub

    Public Sub Remove(ByVal curveIndex As Integer)
        'added by jiri to prevent unhandled exception
        If zgTimeSeries.GraphPane.CurveList.Count = 0 Then
            Return
        End If

        Dim IsExist As Boolean = False
        Dim CurveListCopy As New CurveList
        For i = 0 To zgTimeSeries.GraphPane.CurveList.Count - 1
            CurveListCopy.Add(zgTimeSeries.GraphPane.CurveList(i))
        Next
        Try
            zgTimeSeries.GraphPane.CurveList(curveIndex).Clear()
            zgTimeSeries.GraphPane.CurveList.Clear()
        Catch
        End Try
        For i = 0 To CurveListCopy.Count - 1
            If Not (i = curveIndex) Then
                zgTimeSeries.GraphPane.CurveList.Add(CurveListCopy(i))
            End If
        Next

        'Remove Y Axis
        With zgTimeSeries.GraphPane
            For i = 0 To .YAxisList.Count - 1
                IsExist = False
                For j = 0 To .CurveList.Count - 1
                    If .CurveList(j).Link.Title = .YAxisList(i).Title.Text Then
                        IsExist = True
                    End If
                Next
                If Not IsExist Then
                    If .YAxisList.Count = 1 Then
                        .YAxisList.Remove(.YAxisList(i))
                        .YAxisList.Add("")
                    Else
                        .YAxisList.Remove(.YAxisList(i))
                    End If
                End If
            Next
            For i = 0 To .Y2AxisList.Count - 1
                IsExist = False
                For j = 0 To .CurveList.Count - 1
                    If .CurveList(j).Link.Title = .Y2AxisList(i).Title.Text Then
                        IsExist = True
                    End If
                Next
                If Not IsExist Then
                    If .Y2AxisList.Count = 1 Then
                        .Y2AxisList.Remove(.Y2AxisList(i))
                        .Y2AxisList.Add("")
                    Else
                        .Y2AxisList.Remove(.Y2AxisList(i))
                    End If
                End If
            Next
        End With

        SettingTitle()

    End Sub

    Public Sub SettingTitle()
        Dim IsSame As Boolean = True

        With zgTimeSeries.GraphPane
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
                .Title.Text = .CurveList(0).Link.Title
                .Legend.IsVisible = False
            ElseIf .CurveList.Count = 0 Then
                .Title.Text = "No Data To Plot"
            End If

        End With
    End Sub

    Public Sub AddYAxis(ByRef curve As LineItem)
        Dim gPane As GraphPane = zgTimeSeries.GraphPane
        Dim i As Integer = 0
        Dim IsInYAxis As Boolean = False


        If gPane.CurveList.Count = 1 Then
            With gPane.YAxis
                .Scale.MagAuto = False
                .IsVisible = True
                .Title.Text = m_VariableWithUnits
                '.Scale.FontSpec.FontColor = curve.Color
                '.Title.FontSpec.FontColor = curve.Color
                '.Color = curve.Color
                curve.IsY2Axis = False
                curve.YAxisIndex = 0
            End With
        End If
        While Not i >= gPane.YAxisList.Count
            If gPane.YAxisList(i).Title.Text = curve.Link.Title Then
                curve.IsY2Axis = False
                curve.YAxisIndex = i
                IsInYAxis = True
            End If
            i += 1
        End While
        i = 0
        While Not i >= gPane.Y2AxisList.Count
            If gPane.Y2AxisList(i).Title.Text = curve.Link.Title Then
                curve.IsY2Axis = True
                curve.YAxisIndex = i
                IsInYAxis = True
            End If
            i += 1
        End While
        If IsInYAxis = False Then
            If gPane.Y2AxisList(0).Title.Text = "" Then

                With gPane.Y2AxisList(0)


                    '.Scale.FontSpec.FontColor = curve.Color
                    '.Title.FontSpec.FontColor = curve.Color
                    '.Color = curve.Color
                    .IsVisible = True
                    .Scale.MagAuto = False

                    .MajorTic.IsInside = False
                    .MinorTic.IsInside = False
                    .MajorTic.IsOpposite = False
                    .MinorTic.IsOpposite = False

                    .Scale.Align = AlignP.Inside

                    .Title.Text = curve.Link.Title

                    curve.IsY2Axis = True
                    curve.YAxisIndex = 0
                End With
            ElseIf gPane.YAxisList.Count > gPane.Y2AxisList.Count Then
                Dim newYAxis As New Y2Axis(curve.Link.Title)
                gPane.Y2AxisList.Add(newYAxis)
                'newYAxis.Scale.FontSpec.FontColor = curve.Color
                'newYAxis.Title.FontSpec.FontColor = curve.Color
                'newYAxis.Color = curve.Color
                newYAxis.IsVisible = True
                newYAxis.Scale.MagAuto = False

                newYAxis.MajorTic.IsInside = False
                newYAxis.MinorTic.IsInside = False
                newYAxis.MajorTic.IsOpposite = False
                newYAxis.MinorTic.IsOpposite = False

                newYAxis.Scale.Align = AlignP.Inside

                newYAxis.Title.Text = curve.Link.Title

                curve.IsY2Axis = True
                curve.YAxisIndex = gPane.Y2AxisList.Count - 1
            Else
                Dim newYAxis As New YAxis(curve.Link.Title)
                gPane.YAxisList.Add(newYAxis)
                'newYAxis.Scale.FontSpec.FontColor = curve.Color
                'newYAxis.Title.FontSpec.FontColor = curve.Color
                'newYAxis.Color = curve.Color
                newYAxis.IsVisible = True
                newYAxis.Scale.MagAuto = False

                newYAxis.MajorTic.IsInside = False
                newYAxis.MinorTic.IsInside = False
                newYAxis.MajorTic.IsOpposite = False
                newYAxis.MinorTic.IsOpposite = False

                newYAxis.Scale.Align = AlignP.Inside

                newYAxis.Title.Text = curve.Link.Title

                curve.IsY2Axis = False
                curve.YAxisIndex = gPane.YAxisList.Count - 1
            End If

        End If
    End Sub

    Public Sub SettingCurveStyle(ByRef curve As LineItem)
        curve.Symbol.Fill = New Fill(m_Options.GetPointColor, Color.Red)
        curve.Symbol.Fill.RangeMin = 0
        curve.Symbol.Fill.RangeMax = 1
        curve.Symbol.Size = 4
        curve.Symbol.Fill.SecondaryValueGradientColor = Color.Empty
        curve.Symbol.Fill.Type = FillType.GradientByColorValue
        curve.Symbol.Border.IsVisible = False
    End Sub

    'Developing

    'Public Sub YAxisesReordering()
    '    Dim newYAxis As New Y2Axis()
    '    Dim IsBalance As Boolean = False

    '    With zgTimeSeries.GraphPane
    '        If .YAxisList.Count = 1 And .Y2AxisList.Count = 1 Then
    '            If .YAxisList(0).Title.Text = "" And Not .Y2AxisList(0).Title.Text = "" Then
    '                .YAxisList(0).Title.Text = .Y2AxisList(0).Title.Text
    '                .YAxisList(0).Scale.MagAuto = False

    '            End If
    '        End If
    '        If .YAxisList.Count = .Y2AxisList.Count Or .YAxisList.Count - .Y2AxisList.Count = 1 Then

    '        End If
    '    End With


    'End Sub

#Region "Filters"

    Public Sub SelectingPoint(ByVal ID As Integer)
        If HasEditingCurve() Then
            For i As Integer = 0 To EditingCurve.Points.Count - 1
                If PointValueID(i) = ID Then
                    EditingCurve.Points(i).Z = 1
                End If
            Next
            Refreshing()
        End If
    End Sub

    Public Sub SelectingPoints(ByVal IDlist As ArrayList)
        If HasEditingCurve() Then
            For i As Integer = 0 To EditingCurve.Points.Count - 1
                If IDlist.Contains(PointValueID(i)) Then
                    EditingCurve.Points(i).Z = 1
                End If
            Next
        End If
    End Sub

    Public Sub RemoveSelectedPoints()
        If HasEditingCurve() Then
            For i As Integer = 0 To EditingCurve.Points.Count - 1
                EditingCurve.Points(i).Z = 0
            Next
            Refreshing()
        End If
    End Sub

    Public Sub ChangeZvalueWithValueThreshold(ByVal LargerThanValue As Double, ByVal LessThanValue As Double)

        Dim curve As CurveItem = EditingCurve()


        For i As Integer = 0 To curve.Points.Count - 1
            If LargerThanValue < LessThanValue Then
                If curve.Points(i).Y > LargerThanValue And curve.Points(i).Y < LessThanValue Then
                    curve.Points(i).Z = 1
                Else
                    curve.Points(i).Z = 0
                End If
            Else
                If curve.Points(i).Y > LargerThanValue Or curve.Points(i).Y < LessThanValue Then
                    curve.Points(i).Z = 1
                Else
                    curve.Points(i).Z = 0
                End If
            End If
        Next
        Refreshing()
    End Sub

    Public Sub ChangeZvalueWithValueChangeThreshold(ByVal ValueChange As Double)
        Dim curve As CurveItem = EditingCurve()

        RemoveSelectedPoints()

        If (Math.Abs(curve.Points(0).Y - curve.Points(1).Y) > ValueChange) Then
            curve.Points(0).Z = 1
            curve.Points(1).Z = 1
        End If

        For i As Integer = 1 To curve.Points.Count - 1
            If (Math.Abs(curve.Points(i).Y - curve.Points(i - 1).Y) > ValueChange) Then
                curve.Points(i).Z = 1
                curve.Points(i - 1).Z = 1
            End If
        Next
        Refreshing()
    End Sub

    Public Sub ChangeZvalueWithDate(ByVal DateBefore As DateTime, ByVal DateAfter As DateTime)
        Dim curve As CurveItem = EditingCurve()
        For i As Integer = 0 To curve.Points.Count - 1
            If DateAfter > DateBefore Then
                If curve.Points(i).X >= DateAfter.ToOADate Or curve.Points(i).X <= DateBefore.ToOADate Then
                    curve.Points(i).Z = 1
                Else
                    curve.Points(i).Z = 0
                End If
            Else
                If curve.Points(i).X >= DateAfter.ToOADate And curve.Points(i).X <= DateBefore.ToOADate Then
                    curve.Points(i).Z = 1
                Else
                    curve.Points(i).Z = 0
                End If
            End If
        Next
        Refreshing()
    End Sub

    Public Sub ChangeZvalueWithDataGap(ByVal GapInSecond As Integer)
        Dim curve As CurveItem = EditingCurve()
        Dim different As Long
        Dim date1 As DateTime
        Dim date2 As DateTime

        curve.Points(0).Z = 0

        For i As Integer = 1 To curve.Points.Count - 1
            date1 = DateTime.FromOADate(curve.Points(i).X)
            date2 = DateTime.FromOADate(curve.Points(i - 1).X)
            different = DateDiff(DateInterval.Second, date1, date2, FirstDayOfWeek.Monday, FirstWeekOfYear.Jan1)

            If Math.Abs(different) > GapInSecond Then
                curve.Points(i).Z = 1
                curve.Points(i - 1).Z = 1
            Else
                curve.Points(i).Z = 0
            End If
        Next
        Refreshing()
    End Sub

#End Region

#Region "Edit Functions"

    Public Sub ClearEditMode()
        Dim curve As LineItem
        For i As Integer = 0 To zgTimeSeries.GraphPane.CurveList.Count - 1
            curve = zgTimeSeries.GraphPane.CurveList(i)
            curve.Color = ccList0(i Mod 10)
            curve.Symbol.IsVisible = False
        Next
        Refreshing()
    End Sub

    Public Function CopyCurvePointList(ByVal curve As CurveItem)
        Dim pointList As New PointPairList
        For i As Integer = 0 To curve.Points.Count - 1
            pointList.Add(curve.Points(i))
        Next
        Return pointList
    End Function

    Public Sub EnterEditMode(ByVal curveindex As Integer)
        Dim curve As LineItem = zgTimeSeries.GraphPane.CurveList(curveindex)
        curve.Color = Color.Black
        curve.Symbol.IsVisible = True
        Refreshing()

        EditCurvePointList = New PointPairList
        EditCurvePointList = CopyCurvePointList(zgTimeSeries.GraphPane.CurveList(curveindex))
        EditCurveLable = zgTimeSeries.GraphPane.CurveList(curveindex).Label.Text
        EditCurveTitle = zgTimeSeries.GraphPane.CurveList(curveindex).Link.Title
    End Sub

    Public Sub ChangeValueByAddOrMinus(ByVal Adding As Boolean, ByVal values As Double)
        Dim curve As CurveItem = EditingCurve()
        For i As Integer = 0 To curve.Points.Count - 1

            If curve.Points(i).Z = 1 Then
                If Adding Then
                    curve.Points(i).Y += values
                Else
                    curve.Points(i).Y -= values
                End If

            End If
        Next
        Refreshing()
    End Sub

    Public Sub ChangeValueByMultiply(ByVal Multiplier As Double)
        Dim curve As CurveItem = EditingCurve()
        For i As Integer = 0 To curve.Points.Count - 1

            If curve.Points(i).Z = 1 Then
                curve.Points(i).Y *= Multiplier

            End If
        Next
        Refreshing()
    End Sub

    Public Sub ChangeValueByInterpolating(ByRef returned As Boolean)
        Dim curve As CurveItem = EditingCurve()
        Dim i As Integer = 1
        Dim count As Integer = 1
        Dim difference As Double

        'Check if the first point and last point is selected
        If curve.Points(0).Z = 1 Or curve.Points(curve.Points.Count - 1).Z = 1 Then
            MsgBox("You cannot flat with the first point or last point selected")
            returned = True
            Return
        End If

        'Interpolating
        Do Until (i >= curve.Points.Count - 2)
            If curve.Points(i).Z = 1 Then
                If curve.Points(i + 1).Z = 0 Then
                    curve.Points(i).Y = (curve.Points(i - 1).Y + curve.Points(i + 1).Y) / 2
                    i += 1
                Else
                    count = 1
                    Do Until (curve.Points(i + 1).Z = 0)
                        count += 1
                        i += 1
                    Loop
                    difference = curve.Points(i + 1).Y - curve.Points(i - count).Y
                    For j As Integer = 1 To count
                        curve.Points(i + 1 - j).Y = curve.Points(i + 1).Y - difference / (count + 1) * j
                    Next
                End If
            Else
                i += 1
            End If
        Loop
        Refreshing()

    End Sub

    Public Sub ChangeValueBySettingValue(ByVal value As Double)
        For i As Integer = 0 To EditingCurve.Points.Count - 1

            If EditingCurve.Points(i).Z = 1 Then
                EditingCurve.Points(i).Y = value
            End If
        Next
        Refreshing()
    End Sub

    Public Sub DeletingPoints()
        Dim PointIndexList As New List(Of Integer)
        Dim removedPoints As Integer = 0

        For i As Integer = 0 To EditingCurve.Points.Count - 1
            If EditingCurve.Points(i).Z = 1 Then
                PointIndexList.Add(i)
            End If
        Next

        For i As Integer = 0 To PointIndexList.Count - 1
            EditingCurve.RemovePoint(PointIndexList(i) - removedPoints)
            removedPoints += 1
        Next

        Refreshing()
    End Sub

    Public Function HasEditingCurve() As Boolean
        Dim Has As Boolean = False

        If zgTimeSeries.GraphPane.CurveList.Count < 1 Then
            Return Has
        Else
            For i As Integer = 0 To zgTimeSeries.GraphPane.CurveList.Count - 1
                If zgTimeSeries.GraphPane.CurveList(i).Color = Color.Black Then
                    Has = True
                End If
            Next
            Return Has
        End If
    End Function

    Public Function EditingCurve() As CurveItem
        For i As Integer = 0 To zgTimeSeries.GraphPane.CurveList.Count - 1
            If zgTimeSeries.GraphPane.CurveList(i).Color = Color.Black Then
                Return zgTimeSeries.GraphPane.CurveList(i)
            End If
        Next
        Return zgTimeSeries.GraphPane.CurveList(0)
    End Function

    Public Function PointValueID(ByVal pointindex As Integer) As Integer
        Dim ID As Integer
        ID = (EditingCurve.Points(pointindex).Tag.ToString.Substring(0, EditingCurve.Points(pointindex).Tag.ToString.IndexOf(",")))
        Return ID
    End Function

    Public Function CurveID(ByVal curveindex As Integer) As Integer
        Dim ID As Integer
        Dim StartIndex As Integer
        Dim IDLength As Integer
        StartIndex = zgTimeSeries.GraphPane.CurveList(0).Label.Text.ToString.IndexOf("ID:") + 4
        IDLength = zgTimeSeries.GraphPane.CurveList(0).Label.Text.ToString.Length - StartIndex
        ID = (zgTimeSeries.GraphPane.CurveList(0).Label.Text.ToString.Substring(StartIndex, IDLength))
        Return ID
    End Function

    Public Sub ReplotEditingCurve(ByVal Editdt As DataTable)
        Dim CurveListCopy As New CurveList
        Dim curveIndex As Integer
        Dim EditCurveIsY2Axis As Boolean
        Dim EditCurveYAxisIndex As Integer
        Dim label As String = ""
        Dim pointList As New PointPairList

        If HasEditingCurve() Then
            For i As Integer = 0 To zgTimeSeries.GraphPane.CurveList.Count - 1
                If zgTimeSeries.GraphPane.CurveList(i).Label.Text = EditingCurve.Label.Text Then
                    curveIndex = i
                    label = EditingCurve.Label.Text
                    EditCurveIsY2Axis = EditingCurve.IsY2Axis
                    EditCurveYAxisIndex = EditingCurve.YAxisIndex
                End If
                CurveListCopy.Add(zgTimeSeries.GraphPane.CurveList(i))
            Next

            Try
                zgTimeSeries.GraphPane.CurveList(curveIndex).Clear()
                zgTimeSeries.GraphPane.CurveList.Clear()
            Catch
            End Try
            For i = 0 To CurveListCopy.Count - 1
                If Not (i = curveIndex) Then
                    zgTimeSeries.GraphPane.CurveList.Add(CurveListCopy(i))
                End If
            Next
        End If

        For Each row As DataRow In Editdt.Rows
            If Not row.Item("Other").ToString = "-1" Then

                Dim p As New PointPair(New XDate(CDate(row.Item(XColumn))), row.Item(YColumn))
                p.Tag = row.Item("CensorCode").ToString
                If m_Options.IsPlotCensored Then
                    If (p.Tag.ToString.ToLower = Statistics.NotCensored) Or (p.Tag.ToString.ToLower = Statistics.Unknown) Then
                        p.ColorValue = 0
                    Else
                        p.ColorValue = 1
                    End If
                    p.Tag = row.Item("ValueID").ToString + ", " + row.Item("LocalDateTime").ToString + ": " + row.Item("DataValue").ToString
                    pointList.Add(p)
                Else
                    If Not ((p.Tag.ToString.ToLower = Statistics.NotCensored) Or (p.Tag.ToString.ToLower = Statistics.Unknown)) Then
                        pointList.Add(p)
                        p.Tag = row.Item("ValueID").ToString + ", " + row.Item("LocalDateTime").ToString + ": " + row.Item("DataValue").ToString
                    End If
                End If

            End If
        Next row


        'Re-arrange the other of the point by date
        Dim pointlistcopy As New PointPairList
        Dim point As PointPair
        Dim pointindex As Integer
        For i As Integer = 0 To pointList.Count - 1
            pointlistcopy.Add(pointList(i))
        Next
        pointList.Clear()
        pointList = New PointPairList
        For i As Integer = 0 To pointlistcopy.Count - 1
            point = pointlistcopy(0)
            pointindex = 0
            For j As Integer = 0 To pointlistcopy.Count - 1
                If point.X > pointlistcopy(j).X Then
                    point = pointlistcopy(j)
                    pointindex = j
                End If
            Next
            pointlistcopy.Remove(pointlistcopy(pointindex))
            pointList.Add(point)
        Next


        Dim curve As LineItem = zgTimeSeries.GraphPane.AddCurve(label, pointList, Color.Black, SymbolType.Circle)
        SettingCurveStyle(curve)
        If Not EditCurveYAxisIndex = Nothing Then
            curve.IsY2Axis = EditCurveIsY2Axis
            curve.YAxisIndex = EditCurveYAxisIndex
        End If

        Dim IDlist As New List(Of Integer)

        For i As Integer = 0 To Editdt.Rows.Count - 1
            If Editdt.Rows(i)("Selected").ToString = "1" Then
                IDlist.Add(Editdt.Rows(i)("ValueID"))
            End If
        Next

        For j As Integer = 0 To curve.Points.Count - 1
            If IDlist.Contains(PointValueID(j)) Then
                curve.Points(j).Z = 1
            Else
                curve.Points(j).Z = 0
            End If
        Next

        Refreshing()

    End Sub


#End Region

    'Public Sub RestoreOriginalData()
    '    Clear()
    '    Graph()
    '    Refreshing()
    'End Sub




End Class
