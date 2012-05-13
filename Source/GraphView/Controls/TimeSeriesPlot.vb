Imports System.Windows.Forms
Imports System.Drawing
Imports DotSpatial.Controls
Imports HydroDesktop.Database
Imports HydroDesktop.Interfaces
Imports GraphView.My.Resources
Imports HydroDesktop.Interfaces.ObjectModel
Imports ZedGraph

Namespace Controls

    Public Class TimeSeriesPlot
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


        Public Property SeriesSelector() As ISeriesSelector
        Public Property AppManager() As AppManager

        Public Function CurveCount() As Int32
            Return zgTimeSeries.GraphPane.CurveList.Count
        End Function

        Public Sub Plot(ByRef options As OneSeriesPlotInfo)
            Graph(options)
        End Sub

        Public Sub SetGraphPaneTitle(ByVal title As String)
            zgTimeSeries.GraphPane.Title.Text = title
        End Sub


        Public Sub Clear()
            Try
                If zgTimeSeries Is Nothing Then Return
                If zgTimeSeries.GraphPane Is Nothing Then Return

                Dim gPane As GraphPane = zgTimeSeries.GraphPane

                If gPane.CurveList Is Nothing Then Return
                If gPane.GraphObjList Is Nothing Then Return

                gPane.CurveList.Clear()
                SetGraphPaneTitle(MessageStrings.No_Data_Plot)
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

        Private Sub Graph(ByRef options As OneSeriesPlotInfo)
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
                    SetGraphPaneTitle(MessageStrings.No_Data_Plot)
                Else
                    'Setting Legend
                    If m_Options.ShowLegend Then
                        gPane.Legend.IsVisible = True
                        gPane.Legend.Position = 12
                    Else
                        gPane.Legend.IsVisible = False
                    End If

                    'Setting Scroll and scale
                    zgTimeSeries.IsShowVScrollBar = True
                    zgTimeSeries.IsShowHScrollBar = True
                    zgTimeSeries.IsAutoScrollRange = True
                    'Setting X Axis
                    gPane.XAxis.IsVisible = True
                    gPane.XAxis.Title.Text = "Date and Time"
                    SetGraphPaneTitle(m_VariableWithUnits & vbCrLf & " at " & m_Site)

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
                            p.Tag = row.Item("LocalDateTime").ToString + ": " + row.Item("DataValue").ToString
                            pointList.Add(p)
                        Else
                            If DataValue.IsCensored(p.Tag.ToString) Then
                                pointList.Add(p)
                                p.Tag = row.Item("LocalDateTime").ToString + ": " + row.Item("DataValue").ToString
                            End If
                        End If
                    Next row

                    Dim curve As LineItem = gPane.AddCurve(m_Site, pointList, m_Options.GetLineColor, SymbolType.Circle)

                    curve.Tag = options
                    curve.Symbol.Fill = New Fill(m_Options.GetPointColor, Color.Black)
                    curve.Symbol.Fill.RangeMin = 0
                    curve.Symbol.Fill.RangeMax = 1
                    curve.Symbol.Size = 4
                    curve.Symbol.Fill.SecondaryValueGradientColor = Color.Empty
                    curve.Symbol.Fill.Type = FillType.GradientByColorValue
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

                    'Setting Legend Title
                    Dim needShowDataType = False
                    For Each c In zgTimeSeries.GraphPane.CurveList
                        Dim cOptions = DirectCast(c.Tag, OneSeriesPlotInfo)

                        For Each cc In zgTimeSeries.GraphPane.CurveList
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
                        curve.Label.Text = options.SiteName + ", " + options.VariableName + ", ID: " + options.SeriesID.ToString
                    Else
                        ' Update legend for all curves
                        For Each c In zgTimeSeries.GraphPane.CurveList
                            Dim cOptions = DirectCast(c.Tag, OneSeriesPlotInfo)
                            c.Label.Text = cOptions.SiteName + ", " + cOptions.VariableName + ", " + cOptions.DataType + ", ID: " + cOptions.SeriesID.ToString
                        Next
                    End If


                    'Setting Y Axis
                    curve.Link.Title = m_VariableWithUnits
                End If

                SettingYAsixs()
                SettingTitle()

            Catch ex As Exception
                Throw New Exception("Error Occured in ZGTimeSeries.Graph" & vbCrLf & ex.Message)
            End Try


        End Sub

        Private Sub zgTimeSeries_ContextMenuBuilder(ByVal sender As ZedGraphControl, ByVal menuStrip As ContextMenuStrip, ByVal mousePt As Point, ByVal objState As ZedGraphControl.ContextMenuObjectState) Handles zgTimeSeries.ContextMenuBuilder
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

            'Error checking
            Dim sSelector = SeriesSelector
            If sSelector Is Nothing Then Exit Sub

            'Check selected series
            Dim checkedSeries As Integer = sSelector.CheckedIDList.Count()

            'Check if there is any series to export
            If (checkedSeries <= 0) Then
                MessageBox.Show("No Data To Export")

            Else

                'Build a datatable to export
                Dim exportTable As DataTable = New DataTable
                Dim repo = RepositoryFactory.Instance.Get(Of IDataValuesRepository)()
                'Build datatable for each series and then add all series' datatable to the exportTable
                For count As Integer = 1 To checkedSeries
                    'Error checking
                    Dim checkedSeriesID As Integer = sSelector.CheckedIDList(count - 1)
                    Dim totalData = repo.GetTableForExportFromTimeSeriesPlot(checkedSeriesID)

                    If count = 1 Then
                        exportTable = totalData.Copy()
                    Else
                        exportTable.Merge(totalData, True)
                    End If

                Next count

                If (AppManager IsNot Nothing) Then
                    Dim exportPlugin = AppManager.Extensions.OfType(Of IDataExportPlugin).FirstOrDefault()
                    If exportPlugin IsNot Nothing Then
                        exportPlugin.Export(exportTable)
                    End If
                End If
            End If
        End Sub

        Protected Sub SetLineColor()
            If zgTimeSeries.GraphPane.CurveList.Count > 0 Then
                Dim newColor As Color = PromptForColor(zgTimeSeries.GraphPane.CurveList.Item(0).Color)
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

            Dim CurveListCopy As New CurveList
            For i = 0 To zgTimeSeries.GraphPane.CurveList.Count - 1
                CurveListCopy.Add(zgTimeSeries.GraphPane.CurveList(i))
            Next
            Try
                zgTimeSeries.GraphPane.CurveList.Clear()
            Catch
            End Try
            For i = 0 To CurveListCopy.Count - 1

                If Not (CurveID(CurveListCopy(i)) = ID) Then
                    zgTimeSeries.GraphPane.CurveList.Add(CurveListCopy(i))

                End If
            Next

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
                    .Title.Text = MessageStrings.No_Data_Plot
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

        Private Function CurveID(ByVal curve As CurveItem) As Integer
            Dim cOptions = DirectCast(curve.Tag, OneSeriesPlotInfo)
            If cOptions Is Nothing Then Return Nothing
            Return cOptions.SeriesID
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
End Namespace