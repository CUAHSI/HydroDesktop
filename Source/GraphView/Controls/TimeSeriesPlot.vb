Imports System.Windows.Forms
Imports System.Drawing
Imports DotSpatial.Controls
Imports HydroDesktop.Database
Imports HydroDesktop.Interfaces
Imports GraphView.My.Resources
Imports HydroDesktop.Interfaces.ObjectModel
Imports HydroDesktop.Interfaces.PluginContracts
Imports HydroDesktop.Common.Tools
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
            gPane.XAxis.IsVisible = False
            gPane.YAxis.IsVisible = False
            gPane.Y2Axis.IsVisible = False
            SetGraphPaneTitle(MessageStrings.No_Data_Plot)
        End Sub

        Private Const XColumn As String = "LocalDateTime"
        Private Const YColumn As String = "DataValue"


        Public Property SeriesSelector() As ISeriesSelector
        Public Property AppManager() As AppManager

        Private Function CurveCount() As Int32
            Return zgTimeSeries.GraphPane.CurveList.Count
        End Function

        Private Sub SetGraphPaneTitle(ByVal title As String)
            zgTimeSeries.GraphPane.Title.Text = title
            zgTimeSeries.GraphPane.Title.FontSpec.Size = 14
        End Sub

        Public Sub Plot(ByVal seriesPlotInfo As SeriesPlotInfo) Implements IPlot.Plot
            ' Save curves before clear
            Dim curves = zgTimeSeries.GraphPane.CurveList.Select(Function(xItem) xItem).ToList()
            Clear()
            For Each oneSeriesInfo In seriesPlotInfo.GetSeriesInfo()
                If oneSeriesInfo.Statistics.NumberOfObservations > oneSeriesInfo.Statistics.NumberOfCensoredObservations Then
                    Graph(oneSeriesInfo)

                    ' Try to found previous curve with same SeriesID
                    Dim prevCurve = curves.FirstOrDefault(Function(xItem) DirectCast(xItem.Tag, OneSeriesPlotInfo).SeriesID = oneSeriesInfo.SeriesID)
                    If prevCurve IsNot Nothing Then
                        ' If found, restore color setting
                        Dim lastCurve = zgTimeSeries.GraphPane.CurveList(zgTimeSeries.GraphPane.CurveList.Count - 1)
                        If Not seriesPlotInfo.IsColorsChanged Then
                            lastCurve.Color = prevCurve.Color
                        End If
                    End If

                ElseIf oneSeriesInfo.Statistics.NumberOfObservations = oneSeriesInfo.Statistics.NumberOfCensoredObservations Then
                    If CurveCount() = 0 Then SetGraphPaneTitle(MessageStrings.All_Data_Censored)
                End If
            Next
            Refreshing()
        End Sub

        Private Sub Clear()
            Dim gPane As GraphPane = zgTimeSeries.GraphPane
            gPane.CurveList.Clear()
            SetGraphPaneTitle(MessageStrings.No_Data_Plot)
            gPane.Title.FontSpec.Size = 14
            gPane.XAxis.IsVisible = False
            gPane.YAxis.IsVisible = False
            gPane.Y2Axis.IsVisible = False
            gPane.GraphObjList.Clear()
            zgTimeSeries.IsShowVScrollBar = False
            zgTimeSeries.IsShowHScrollBar = False
            zgTimeSeries.Refresh()
        End Sub

        Private Sub Graph(ByVal options As OneSeriesPlotInfo)
            Try
                Dim gPane As GraphPane = zgTimeSeries.GraphPane

                Dim m_Data = options.DataTable
                Dim m_Site = options.SiteName
                Dim m_VariableWithUnits = options.GetVariableWithUnitsString()
                Dim m_Options = options.PlotOptions

                If (m_Data Is Nothing) Or (m_Data.Rows.Count <= 0) Then
                    gPane.XAxis.IsVisible = False
                    gPane.YAxis.IsVisible = False
                    zgTimeSeries.IsShowVScrollBar = False
                    zgTimeSeries.IsShowHScrollBar = False
                    SetGraphPaneTitle(MessageStrings.No_Data_Plot)
                    gPane.Title.FontSpec.Size = 14
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
                    gPane.XAxis.Title.FontSpec.Size = 12
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

                    Dim curve As LineItem = gPane.AddCurve(m_Site, pointList, options.LineColor, SymbolType.Circle)

                    curve.Tag = options
                    curve.Symbol.Fill = New Fill(options.PointColor, Color.Black)
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
                        If curve.Label.Text.Length > 70 Then
                            curve.Label.Text = curve.Label.Text.Substring(0, 70) + "..."
                        End If
                    Else
                        ' Update legend for all curves
                        For Each c In zgTimeSeries.GraphPane.CurveList
                            Dim cOptions = DirectCast(c.Tag, OneSeriesPlotInfo)
                            c.Label.Text = cOptions.SiteName + ", " + cOptions.VariableName + ", " + cOptions.DataType + ", ID: " + cOptions.SeriesID.ToString
                            If curve.Label.Text.Length > 70 Then
                                c.Label.Text = curve.Label.Text.Substring(0, 70) + "..."
                            End If
                        Next
                    End If


                    'Setting Y Axis
                    curve.Link.Title = m_VariableWithUnits
                End If

                SettingYAsixs()
                SettingTitle()

                'ZedGraph apparently doesn't play nice on Mac OS when a Title and Legend are visible. 
                'As a temporary measure, we will remove these two things.
                If DotSpatial.Mono.Mono.IsRunningOnMono() Then
                    gPane.Title.IsVisible = False
                    gPane.Legend.IsVisible = False
                End If

            Catch ex As Exception
                Throw New Exception("Error Occured in ZGTimeSeries.Graph" & vbCrLf & ex.Message)
            End Try


        End Sub

        Private Sub zgTimeSeries_ContextMenuBuilder(ByVal sender As ZedGraphControl, ByVal menuStrip As ContextMenuStrip,
                                                    ByVal mousePt As Point, ByVal objState As ZedGraphControl.ContextMenuObjectState) Handles zgTimeSeries.ContextMenuBuilder
            ' from http://zedgraph.org/wiki/index.php?title=Edit_the_Context_Menu

            ' Add item to export to text file
            Dim item As ToolStripMenuItem = New ToolStripMenuItem()
            item.Text = MessageStrings.Export_Time_Series
            item.Enabled = SeriesSelector IsNot Nothing AndAlso SeriesSelector.CheckedIDList.Any()
            AddHandler item.Click, AddressOf ExportToTextFile
            menuStrip.Items.Add(item)

            ' Add item to export to change line color
            item = New ToolStripMenuItem()
            item.Enabled = sender.GraphPane.CurveList.Count > 0
            If item.Enabled Then
                Dim curve As CurveItem = Nothing
                Dim iNearest As Integer
                Dim founded = sender.GraphPane.FindNearestPoint(mousePt, curve, iNearest)
                founded = founded OrElse Not IsNothing(curve)
                item.Text = If(founded,
                               MessageStrings.Set_Line_Color + ": " + curve.Label.Text,
                               MessageStrings.Set_Line_Color_No_Point)
                item.Enabled = founded
                item.Tag = curve
            Else
                item.Text = MessageStrings.Set_Line_Color_No_Point
            End If

            AddHandler item.Click, AddressOf SetLineColor
            menuStrip.Items.Add(item)

        End Sub


        Private Sub ExportToTextFile()
            Debug.Assert(SeriesSelector IsNot Nothing)
            Debug.Assert(SeriesSelector.CheckedIDList.Any())

            Dim sSelector = SeriesSelector

            'Build a datatable to export
            Dim exportTable As DataTable = New DataTable
            Dim repo = RepositoryFactory.Instance.Get(Of IDataValuesRepository)()
            'Build datatable for each series and then add all series' datatable to the exportTable
            For count As Integer = 1 To sSelector.CheckedIDList.Count()
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
        End Sub

        Private Sub SetLineColor(ByVal sender As ToolStripMenuItem, ByVal eventArgs As EventArgs)
            Dim curve = TryCast(sender.Tag, CurveItem)
            If curve Is Nothing Then Return
            Dim newColor = DrawingHelper.PromptForColor(curve.Color)
            If newColor.HasValue Then
                curve.Color = newColor.Value
                zgTimeSeries.Refresh()
            End If
        End Sub

        Private Sub Refreshing()
            zgTimeSeries.AxisChange()
            zgTimeSeries.Refresh()
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
                        .Title.Text = "Multiple TimeSeries"
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
                        NewAsix.Title.FontSpec.Size = 12
                        If NewAsix.Title.Text.Length > 40 Then
                            NewAsix.Title.Text = NewAsix.Title.Text.Substring(0, 40) + "..."
                        End If
                        NewAsix.MajorTic.IsInside = False
                        NewAsix.MinorTic.IsInside = False
                        NewAsix.MajorTic.IsOpposite = False
                        NewAsix.MinorTic.IsOpposite = False
                        .YAxisList.Add(NewAsix)
                    Else
                        Dim NewAsix As New Y2Axis()
                        NewAsix.Title.Text = AsixsList(i)
                        NewAsix.Title.FontSpec.Size = 12
                        If NewAsix.Title.Text.Length > 40 Then
                            NewAsix.Title.Text = NewAsix.Title.Text.Substring(0, 40) + "..."
                        End If
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