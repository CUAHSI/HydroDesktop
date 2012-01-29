Imports Controls
Imports ZedGraph
Imports System.Drawing
Imports HydroDesktop.Database
Imports HydroDesktop.Interfaces

Public Class cTimeSeriesPlot
    Implements IChart

    Public Sub New()
        ' This call is required by the Windows Form Designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        Dim gPane As GraphPane = zgTimeSeries.GraphPane
        gPane.XAxis.Type = AxisType.Date
        gPane.YAxis.Type = AxisType.Linear
        gPane.Border.IsVisible = False
        gPane.Legend.IsVisible = False
    End Sub

    Private Const XColumn As String = "LocalDateTime"
    Private Const YColumn As String = "DataValue"

    Private m_SeriesSelector As ISeriesSelector
    Public Property SeriesSelector() As ISeriesSelector
        Get
            Return m_SeriesSelector
        End Get
        Set(ByVal value As ISeriesSelector)
            m_SeriesSelector = value
        End Set
    End Property


    Public Sub Plot(ByRef options As TimeSeriesPlotOptions)
        Try
            Graph(options)
        Catch ex As Exception
            Throw New Exception("Error Occurred in ZGTimeSeries.Plot" & vbCrLf & ex.Message)
        End Try
    End Sub


    Public Sub Clear()
        Try
            If zgTimeSeries Is Nothing Then Return
            If zgTimeSeries.GraphPane Is Nothing Then Return

            Dim gPane As GraphPane = zgTimeSeries.GraphPane

            If gPane.CurveList Is Nothing Then Return
            If gPane.GraphObjList Is Nothing Then Return

            gPane.CurveList.Clear()
            gPane.Title.Text = "No Data To Plot"
            gPane.XAxis.IsVisible = False
            gPane.YAxis.IsVisible = False
            gPane.GraphObjList.Clear()
            zgTimeSeries.IsShowVScrollBar = False
            zgTimeSeries.IsShowHScrollBar = False
            'Graph()

        Catch ex As Exception
            Throw New Exception("Error Occured in ZGTimeSeries.Clear" & vbCrLf & ex.Message)
        End Try
    End Sub

    Private Sub Graph(ByRef options As TimeSeriesPlotOptions)
        Try
            Dim gPane As GraphPane = zgTimeSeries.GraphPane

            Dim m_Data = options.DataTable.Copy
            Dim m_Site = options.SiteName
            Dim m_VariableWithUnits = options.VariableName & " - " & options.VariableUnits
            Dim m_Options = options.PlotOptions

            If (m_Data Is Nothing) Or (m_Data.Rows.Count <= 0) Then
                gPane.XAxis.IsVisible = False
                gPane.YAxis.IsVisible = False
                zgTimeSeries.IsShowVScrollBar = False
                zgTimeSeries.IsShowHScrollBar = False
                gPane.Title.Text = "No Data To Plot"
            Else
                'Setting Legend
                If m_Options.ShowLegend Then
                    gPane.Legend.IsVisible = True
                    gPane.Legend.Position = 12
                Else
                    gPane.Legend.IsVisible = False
                End If

                'Setting Scroll and scale
                'zgTimeSeries.ZoomOutAll(gPane)
                zgTimeSeries.IsShowVScrollBar = True
                zgTimeSeries.IsShowHScrollBar = True
                zgTimeSeries.IsAutoScrollRange = True
                'Setting X Axis
                gPane.XAxis.IsVisible = True
                gPane.XAxis.Title.Text = "Date and Time"

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
                        p.Tag = row.Item("LocalDateTime").ToString + ": " + row.Item("DataValue").ToString
                        pointList.Add(p)
                    Else
                        If Not ((p.Tag.ToString.ToLower = Statistics.NotCensored) Or (p.Tag.ToString.ToLower = Statistics.Unknown)) Then
                            pointList.Add(p)
                            p.Tag = row.Item("LocalDateTime").ToString + ": " + row.Item("DataValue").ToString
                        End If
                    End If
                Next row

                Dim curve As LineItem = gPane.AddCurve(m_Site, pointList, m_Options.GetLineColor, SymbolType.Circle)
                curve.Tag = options
                curve.Symbol.Fill = New Fill(m_Options.GetPointColor, m_Options.GetPointColor)
                curve.Symbol.Fill.RangeMin = 0
                curve.Symbol.Fill.RangeMax = 1
                curve.Symbol.Size = 4
                curve.Symbol.Fill.SecondaryValueGradientColor = Color.Empty
                curve.Symbol.Fill.Type = FillType.GradientByColorValue
                curve.Symbol.Border.IsVisible = False
                curve.Symbol.Border.IsVisible = False
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

                'Setting Legend Title
                Dim needShowDataType = False
                For Each c In zgTimeSeries.GraphPane.CurveList
                    Dim cOptions = DirectCast(c.Tag, TimeSeriesPlotOptions)

                    For Each cc In zgTimeSeries.GraphPane.CurveList
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
                    curve.Label.Text = options.SiteName + ", " + options.VariableName + ", ID: " + options.SeriesID.ToString
                Else
                    ' Update legend for all curves
                    For Each c In zgTimeSeries.GraphPane.CurveList
                        Dim cOptions = DirectCast(c.Tag, TimeSeriesPlotOptions)
                        c.Label.Text = cOptions.SiteName + ", " + cOptions.VariableName + ", " + cOptions.DataType + ", ID: " + cOptions.SeriesID.ToString
                    Next
                End If


                'Setting Y Axis
                curve.Link.Title = m_VariableWithUnits
                '    If gPane.CurveList.Count = 1 Then
                '        With gPane.YAxis
                '            .Scale.MagAuto = False
                '            .IsVisible = True
                '            .Title.Text = m_Var
                '            '.Scale.FontSpec.FontColor = curve.Color
                '            '.Title.FontSpec.FontColor = curve.Color
                '            '.Color = curve.Color
                '            curve.IsY2Axis = False
                '            curve.YAxisIndex = 0
                '        End With
                '    End If
                '    While Not i >= gPane.YAxisList.Count
                '        If gPane.YAxisList(i).Title.Text = curve.Link.Title Then
                '            curve.IsY2Axis = False
                '            curve.YAxisIndex = i
                '            IsInYAxis = True
                '        End If
                '        i += 1
                '    End While
                '    i = 0
                '    While Not i >= gPane.Y2AxisList.Count
                '        If gPane.Y2AxisList(i).Title.Text = curve.Link.Title Then
                '            curve.IsY2Axis = True
                '            curve.YAxisIndex = i
                '            IsInYAxis = True
                '        End If
                '        i += 1
                '    End While
                '    If IsInYAxis = False Then
                '        If gPane.Y2AxisList(0).Title.Text = "" Then

                '            With gPane.Y2AxisList(0)


                '                '.Scale.FontSpec.FontColor = curve.Color
                '                '.Title.FontSpec.FontColor = curve.Color
                '                '.Color = curve.Color
                '                .IsVisible = True
                '                .Scale.MagAuto = False

                '                .MajorTic.IsInside = False
                '                .MinorTic.IsInside = False
                '                .MajorTic.IsOpposite = False
                '                .MinorTic.IsOpposite = False

                '                .Scale.Align = AlignP.Inside

                '                .Title.Text = curve.Link.Title

                '                curve.IsY2Axis = True
                '                curve.YAxisIndex = 0
                '            End With
                '        ElseIf gPane.YAxisList.Count > gPane.Y2AxisList.Count Then
                '            Dim newYAxis As New Y2Axis(curve.Link.Title)
                '            gPane.Y2AxisList.Add(newYAxis)
                '            'newYAxis.Scale.FontSpec.FontColor = curve.Color
                '            'newYAxis.Title.FontSpec.FontColor = curve.Color
                '            'newYAxis.Color = curve.Color
                '            newYAxis.IsVisible = True
                '            newYAxis.Scale.MagAuto = False

                '            newYAxis.MajorTic.IsInside = False
                '            newYAxis.MinorTic.IsInside = False
                '            newYAxis.MajorTic.IsOpposite = False
                '            newYAxis.MinorTic.IsOpposite = False

                '            newYAxis.Scale.Align = AlignP.Inside

                '            newYAxis.Title.Text = curve.Link.Title

                '            curve.IsY2Axis = True
                '            curve.YAxisIndex = gPane.Y2AxisList.Count - 1
                '        Else
                '            Dim newYAxis As New YAxis(curve.Link.Title)
                '            gPane.YAxisList.Add(newYAxis)
                '            'newYAxis.Scale.FontSpec.FontColor = curve.Color
                '            'newYAxis.Title.FontSpec.FontColor = curve.Color
                '            'newYAxis.Color = curve.Color
                '            newYAxis.IsVisible = True
                '            newYAxis.Scale.MagAuto = False

                '            newYAxis.MajorTic.IsInside = False
                '            newYAxis.MinorTic.IsInside = False
                '            newYAxis.MajorTic.IsOpposite = False
                '            newYAxis.MinorTic.IsOpposite = False

                '            newYAxis.Scale.Align = AlignP.Inside

                '            newYAxis.Title.Text = curve.Link.Title

                '            curve.IsY2Axis = False
                '            curve.YAxisIndex = gPane.YAxisList.Count - 1
                '        End If

                '    End If
                '        'gPane.Legend.Fill = New Fill(m_Options.GetLineColor, Brushes.AliceBlue, FillType.None)
                '        'gPane.Legend.FontSpec.
            End If

            SettingYAsixs()
            SettingTitle()

            'zgTimeSeries.AxisChange()
            'zgTimeSeries.Refresh()

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
        AddHandler item.Click, AddressOf ExportToTextFile
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

    Protected Sub ExportToTextFile()

        'Error checking
        Dim sSelector = SeriesSelector
        If sSelector Is Nothing Then Exit Sub

        'Check selected series
        Dim checkedSeries As Integer = sSelector.CheckedIDList.Count()

        'Check if there is any series to export
        If (checkedSeries <= 0) Then
            System.Windows.Forms.MessageBox.Show("No Data To Export")

        Else

            'Build a datatable to export
            Dim exportTable As DataTable = New DataTable

            Dim conn = HydroDesktop.Configuration.Settings.Instance.DataRepositoryConnectionString
            Dim dbOperation As New DbOperations(conn, DatabaseTypes.SQLite)
            Dim totalData As New Data.DataTable

            'Build datatable for each series and then add all series' datatable to the exportTable
            For count As Integer = 1 To checkedSeries

                Dim sql As String

                'Error checking
                Dim checkedSeriesID As Integer = sSelector.CheckedIDList(count - 1)

                sql = "SELECT ds.SeriesID, s.SiteName, v.VariableName, dv.DataValue, dv.LocalDateTime, U.UnitsName FROM DataSeries ds, Sites s, Variables v, DataValues dv, Units U WHERE v.VariableID = ds.VariableID AND s.SiteID = ds.SiteID AND dv.SeriesID = ds.SeriesID AND U.UnitsID = v.VariableUnitsID AND ds.SeriesID = " & checkedSeriesID
                totalData = dbOperation.LoadTable("DataValues", sql)

                If count = 1 Then
                    exportTable = totalData.Copy()
                Else
                    exportTable.Merge(totalData, True)
                End If

            Next count

            Dim exportForm As HydroDesktop.ImportExport.ExportDataTableToTextFileDialog = New HydroDesktop.ImportExport.ExportDataTableToTextFileDialog(exportTable)
            exportForm.ShowDialog()
        End If
    End Sub

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

    Public Sub Remove(ByVal ID As Integer)
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
            'zgTimeSeries.GraphPane.CurveList(curveIndex).Clear()
            zgTimeSeries.GraphPane.CurveList.Clear()
        Catch
        End Try
        For i = 0 To CurveListCopy.Count - 1

            If Not (CurveID(CurveListCopy(i)) = ID) Then
                zgTimeSeries.GraphPane.CurveList.Add(CurveListCopy(i))

            End If

            'If Not (i = curveIndex) Then
            '    zgTimeSeries.GraphPane.CurveList.Add(CurveListCopy(i))

            'End If

        Next

        'Remove Y Axis
        'With zgTimeSeries.GraphPane
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
        '            Else
        '                .YAxisList.Remove(.YAxisList(i))
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
        '            Else
        '                .Y2AxisList.Remove(.Y2AxisList(i))
        '            End If
        '        End If
        '    Next
        'End With


        SettingYAsixs()

        SettingTitle()

    End Sub

    Private Sub SettingTitle()
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
                '.Title.Text = .CurveList(0).Link.Title
                .Legend.IsVisible = False
            ElseIf .CurveList.Count = 0 Then
                .Title.Text = "No Data To Plot"
            End If

        End With
    End Sub

    Private Sub SettingYAsixs()
        Dim AsixsList As New ArrayList()

        With zgTimeSeries.GraphPane

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

    Public Function CurveID(ByVal curve As CurveItem) As Integer
        Dim ID As Integer
        Dim StartIndex As Integer
        Dim IDLength As Integer
        StartIndex = curve.Label.Text.ToString.IndexOf("ID:") + 4
        IDLength = curve.Label.Text.ToString.Length - StartIndex
        ID = (curve.Label.Text.ToString.Substring(StartIndex, IDLength))
        Return ID
    End Function

    Public Property ShowPointValues() As Boolean Implements IChart.ShowPointValues
        Get
            Return zgTimeSeries.IsShowPointValues
        End Get
        Set(ByVal value As Boolean)
            zgTimeSeries.IsShowPointValues = value
        End Set
    End Property

    Public Sub ZoomIn() Implements IChart.ZoomIn
        zgTimeSeries.ZoomIn()
    End Sub

    Public Sub ZoomOut() Implements IChart.ZoomOut
        zgTimeSeries.ZoomOut(zgTimeSeries.GraphPane)
    End Sub

    Public Sub ZoomOutAll() Implements IChart.ZoomOutAll
        zgTimeSeries.ZoomOutAll(zgTimeSeries.GraphPane)
    End Sub

End Class