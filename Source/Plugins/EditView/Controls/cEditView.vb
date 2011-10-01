Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Drawing
Imports System.Data
Imports System.Linq
Imports System.Text
Imports System.Windows.Forms
Imports HydroDesktop.Database
Imports System.Collections
Imports System.Data.Common
Imports System.Threading
Imports System.Globalization
Imports QualifierHandling
Imports ZedGraph
Imports HydroDesktop.Interfaces


'Namespace EditView
Public Class cEditView
    'Inherits UserControl

#Region "privateDeclaration"
    'Private sriesList As New ArrayList()
    'private int rbSequenceTime = new Int32();

    Private connString = HydroDesktop.Configuration.Settings.Instance.DataRepositoryConnectionString
    Private dbTools As New DbOperations(connString, DatabaseTypes.SQLite)
    Private CurveEditingColor As Drawing.Color = Color.Black

    'private SeriesSelector control for checking / unchecking the series
    Public _seriesSelector As ISeriesSelector

    Public Originaldt As Data.DataTable
    Public Editdt As DataTable
    Public newseriesID As Integer = 0
    Public Editing As Boolean = False
    Public selectedSeriesIdList As New ArrayList()
    Private ccList0 As New ArrayList()
    Public nodataseriescount As Integer = 0
    Public colorcount As Integer = 0
    Public ShowLegend As Boolean
    Public Canceled As Boolean = False

    Private Const ErrMsgForNotEditing As String = "Please select a series to edit first."
    Private Const ErrMsgForNotPointSelected As String = "Please select a point for editing."
#End Region

#Region "Constructor"

    Public Sub RefreshSelection()
        pTimeSeriesPlot.Clear()
        'SeriesSelector.RefreshSelection()
    End Sub

    Private Sub SettingColor()
        ccList0.Clear()
        ccList0.Add(System.Drawing.Color.FromArgb(106, 61, 154))
        ccList0.Add(System.Drawing.Color.FromArgb(202, 178, 214))
        ccList0.Add(System.Drawing.Color.FromArgb(255, 127, 0))
        ccList0.Add(System.Drawing.Color.FromArgb(253, 191, 111))
        ccList0.Add(System.Drawing.Color.FromArgb(227, 26, 28))
        ccList0.Add(System.Drawing.Color.FromArgb(251, 154, 153))
        ccList0.Add(System.Drawing.Color.FromArgb(51, 160, 44))
        ccList0.Add(System.Drawing.Color.FromArgb(178, 223, 138))
        ccList0.Add(System.Drawing.Color.FromArgb(31, 120, 180))
        ccList0.Add(System.Drawing.Color.FromArgb(166, 206, 227))
    End Sub

    Public Sub New(ByVal seriesSelector As ISeriesSelector)
        'InitializeComponent()

        ' This call is required by the Windows Form Designer.
        InitializeComponent()

        'set the 'seriesMenu' private variable
        _seriesSelector = seriesSelector

        'assign the events
        AddHandler _seriesSelector.SeriesCheck, AddressOf SeriesSelector_SeriesCheck
        'AddHandler _args.SeriesView.SeriesSelector.SeriesSelected, AddressOf SeriesSelector_SeriesSelected

        'this is used for the open existing project events
        AddHandler _seriesSelector.Refreshed, AddressOf SeriesSelector_Refreshed

        'SeriesSelector.MultipleCheck = True
        gboxDataFilter.Enabled = False
        ddlTimePeriod.SelectedItem = ddlTimePeriod.Items(0)
        lblstatus.Text = "Ready"
        SettingColor()
        pTimeSeriesPlot.Clear()

        'Add Event
        'SeriesSelector.MultipleCheck = false;

    End Sub

    Public Sub initialize()
        gboxDataFilter.Enabled = False
        'SeriesSelector.MultipleCheck = True
        ddlTimePeriod.SelectedItem = ddlTimePeriod.Items(0)
        lblstatus.Text = "Ready"
    End Sub

#End Region

#Region "Views"

#Region "Method"
    'To refresh the themes shown in the series selector
    Public Sub RefreshView()
        _seriesSelector.RefreshSelection()
    End Sub

    Private Sub ShowAllFieldsinSequence()
        Dim connString = HydroDesktop.Configuration.Settings.Instance.DataRepositoryConnectionString
        Dim dbTools As New DbOperations(connString, DatabaseTypes.SQLite)
        Originaldt = New DataTable
        Dim dt As New DataTable
        Dim SQLString As New StringBuilder

        SQLString.Append("SELECT ValueID, SeriesID, DataValue, ValueAccuracy, LocalDateTime, UTCOffset, ")
        SQLString.Append("DateTimeUTC, QualifierCode, OffsetValue, OffsetTypeID, CensorCode, SampleID, ")
        SQLString.Append("FileID FROM DataValues AS d LEFT JOIN Qualifiers AS q ON (d.QualifierID = q.QualifierID) ")
        SQLString.Append("WHERE SeriesID = ")
        For Each seriesID As Integer In selectedSeriesIdList
            SQLString.Append(seriesID)
            SQLString.Append(",")
        Next
        SQLString.Remove(SQLString.Length - 1, 1)
        SQLString.Append(")")


        dt = dbTools.LoadTable("DataValues", SQLString.ToString)
        dt.Columns.Add("Other")
        For i As Integer = 0 To dt.Rows.Count - 1
            dt.Rows(i)("Other") = 0

        Next
        dgvDataValues.DataSource = dt
        ResetGridViewStyle()
    End Sub

    Public Sub PlotGraph(ByVal SeriesID As Integer)
        Dim options As PlotOptions = New PlotOptions(PlotOptions.TimeSeriesType.Line, ccList0(colorcount Mod 10), CurveEditingColor, False, True)
        Dim connString = HydroDesktop.Configuration.Settings.Instance.DataRepositoryConnectionString
        Dim dbTools As New DbOperations(connString, DatabaseTypes.SQLite)
        Dim nodatavalue As Double
        Dim data As DataTable = New DataTable()
        Dim variableName As String = ""
        Dim unitsName As String = ""
        Dim siteName As String = ""

        nodatavalue = dbTools.ExecuteSingleOutput("SELECT NoDataValue FROM DataSeries LEFT JOIN Variables ON DataSeries.VariableID = Variables.VariableID WHERE (SeriesID = '" & SeriesID & "')")
        'data = dbTools.LoadTable("DataValues", "SELECT ValueID, DataValue, LocalDateTime, CensorCode FROM DataValues WHERE (SeriesID = '" & SeriesID & "') AND (DataValue <> '" & nodatavalue & "') ORDER BY LocalDateTime")
        'data = dbTools.LoadTable("DataValues", "SELECT * FROM DataValues WHERE (SeriesID = '" & SeriesID & "') AND (DataValue <> '" & nodatavalue & "') ORDER BY LocalDateTime")
        data = dbTools.LoadTable("DataValues", "SELECT * FROM DataValues WHERE (SeriesID = '" & SeriesID & "') ORDER BY LocalDateTime")
        variableName = dbTools.ExecuteSingleOutput("SELECT VariableName FROM DataSeries LEFT JOIN Variables ON Variables.VariableID = DataSeries.VariableID WHERE SeriesID = '" & SeriesID & "'")
        unitsName = dbTools.ExecuteSingleOutput("SELECT UnitsName FROM DataSeries LEFT JOIN Variables ON Variables.VariableID = DataSeries.VariableID LEFT JOIN Units ON Variables.VariableUnitsID = Units.UnitsID WHERE SeriesID = '" & SeriesID & "'")
        siteName = dbTools.ExecuteSingleOutput("SELECT SiteName FROM DataSeries LEFT JOIN Sites ON Sites.SiteID = DataSeries.SiteID WHERE SeriesID = '" & SeriesID & "'")

        If data.Rows.Count = 1 Then
            options.TimeSeriesMethod = PlotOptions.TimeSeriesType.Point
        End If

        If data.Rows.Count <= 0 Then
            MsgBox("The Selected Series has no curve")
        Else
            pTimeSeriesPlot.Plot(data, siteName, variableName, unitsName, options, SeriesID)
        End If

        pTimeSeriesPlot.Refreshing()

        data.Dispose()

        ckbShowLegend_CheckedChanged()

        colorcount += 1
    End Sub

    Private Sub RemoveSeriesFromDataGridView(ByVal SeriesID As Integer)
        Dim removedRows As Integer = 0
        pbProgressBar.Minimum = 0
        pbProgressBar.Maximum = dgvDataValues.Rows.Count - 1
        pbProgressBar.Visible = True
        pbProgressBar.Value = 0
        lblstatus.Text = "Removing Series"

        For i As Integer = 0 To dgvDataValues.Rows.Count - 1
            If dgvDataValues.Rows(i - removedRows).Cells("SeriesID").Value = SeriesID Then
                dgvDataValues.Rows.Remove(dgvDataValues.Rows(i - removedRows))
                removedRows += 1
            End If
            pbProgressBar.Value = i
        Next

        lblstatus.Text = "Ready"
    End Sub

    Public Sub AddSeriesToDataGridView(ByVal SeriesID As Integer)
        Dim connString = HydroDesktop.Configuration.Settings.Instance.DataRepositoryConnectionString
        Dim dbTools As New DbOperations(connString, DatabaseTypes.SQLite)
        Originaldt = New DataTable
        Dim dt As New DataTable
        Dim dtdgvDataSource As DataTable
        Dim SQLString As New StringBuilder

        SQLString.Append("SELECT ValueID, SeriesID, DataValue, ValueAccuracy, LocalDateTime, UTCOffset, ")
        SQLString.Append("DateTimeUTC, QualifierCode, OffsetValue, OffsetTypeID, CensorCode, SampleID, ")
        SQLString.Append("FileID FROM DataValues AS d LEFT JOIN Qualifiers AS q ON (d.QualifierID = q.QualifierID) ")
        SQLString.Append("WHERE SeriesID = " + SeriesID.ToString)



        dt = dbTools.LoadTable("DataValues", SQLString.ToString)


        dt.Columns.Add("Other")
        If dgvDataValues.Rows.Count = 0 Then
            For i As Integer = 0 To dt.Rows.Count - 1
                dt.Rows(i)("Other") = 0
            Next
            dgvDataValues.DataSource = dt
            dgvDataValues.AllowUserToAddRows = False
        Else
            dtdgvDataSource = dgvDataValues.DataSource
            For i As Integer = 0 To dt.Rows.Count - 1
                dt.Rows(i)("Other") = 0
            Next
            dtdgvDataSource.Merge(dt)
        End If

        Originaldt = dt.Copy

        ResetGridViewStyle()
    End Sub

#End Region

#Region "Event"

    'refreshes the view when the database changes
    Private Sub SeriesSelector_Refreshed(ByVal sender As Object, ByVal e As EventArgs)

        Me.connString = HydroDesktop.Configuration.Settings.Instance.DataRepositoryConnectionString
        Me.dbTools = New DbOperations(Me.connString, DatabaseTypes.SQLite)

        'to clear all values
        Originaldt = Nothing
        Editdt = Nothing
        newseriesID = 0
        Editing = False
        selectedSeriesIdList.Clear()
        nodataseriescount = 0
        colorcount = 0
        ShowLegend = False
        Canceled = False
        pTimeSeriesPlot.Clear()
        pTimeSeriesPlot.Refreshing()

    End Sub

    Private Sub cEditView_Load(ByVal sender As Object, ByVal e As EventArgs)
        'populate the series selector control
        'seriesSelector1.PopulateTreeView2();
        dgvDataValues.ColumnHeadersVisible = True
        'dataViewSeries.Columns.ToString
        ' Set the column header style.
        'DataGridViewCellStyle columnHeaderStyle =new DataGridViewCellStyle();
        'dataViewSeries.ColumnHeadersDefaultCellStyle.BackColor = Color.Aqua;
        'dataViewSeries.ColumnHeadersDefaultCellStyle.Font = new Font("Verdana", 10, FontStyle.Regular);
        'dataViewSeries.ColumnHeadersDefaultCellStyle =columnHeaderStyle;
        dgvDataValues.ColumnHeadersBorderStyle = ProperColumnHeadersBorderStyle
        'rbSequenceTime = 0;
    End Sub

    Private Shared ReadOnly Property ProperColumnHeadersBorderStyle() As DataGridViewHeaderBorderStyle
        Get
            Return If((SystemFonts.MessageBoxFont.Name = "Segoe UI"), DataGridViewHeaderBorderStyle.None, DataGridViewHeaderBorderStyle.Raised)
        End Get
    End Property

    Private Sub SeriesSelector_SeriesCheck()
        'Declaring all variables
        Dim connString = HydroDesktop.Configuration.Settings.Instance.DataRepositoryConnectionString
        Dim dbTools As New DbOperations(connString, DatabaseTypes.SQLite)
        Dim data As DataTable = New DataTable()
        Dim variableName As String = ""
        Dim unitsName As String = ""
        Dim siteName As String = ""
        Dim count As Integer = 0
        Dim curveIndex As Integer
        Dim SeriesSelector = _seriesSelector



        If Not SeriesSelector.CheckedIDList.Length > selectedSeriesIdList.Count Then

            If pTimeSeriesPlot.HasEditingCurve Then
                pTimeSeriesPlot.EditCurvePointList = pTimeSeriesPlot.CopyCurvePointList(pTimeSeriesPlot.EditingCurve)
                pTimeSeriesPlot.EditCurveLable = pTimeSeriesPlot.EditingCurve.Label.Text
                pTimeSeriesPlot.EditCurveTitle = pTimeSeriesPlot.EditingCurve.Link.Title
            End If
            curveIndex = selectedSeriesIdList.IndexOf(SeriesSelector.SelectedSeriesID)
            selectedSeriesIdList.Remove(SeriesSelector.SelectedSeriesID)
            If SeriesRowsCount(SeriesSelector.SelectedSeriesID) = 0 Then
                nodataseriescount -= 1
            End If
            If (selectedSeriesIdList.Count = 0) Then
                pTimeSeriesPlot.Clear()
            ElseIf (selectedSeriesIdList.Count = 1) Then
                Try
                    pTimeSeriesPlot.Remove(curveIndex - nodataseriescount)
                    curveIndex = pTimeSeriesPlot.CurveID(0)
                    pTimeSeriesPlot.Remove(0)
                    If SeriesRowsCount(SeriesSelector.SelectedSeriesID) = 0 Then
                        nodataseriescount += 1
                    ElseIf Not curveIndex = newseriesID Then
                        PlotGraph(curveIndex)
                    Else : curveIndex = newseriesID
                        Dim curve As LineItem = pTimeSeriesPlot.zgTimeSeries.GraphPane.AddCurve(pTimeSeriesPlot.EditCurveLable, pTimeSeriesPlot.EditCurvePointList, Color.Black, SymbolType.Circle)
                        pTimeSeriesPlot.SettingCurveStyle(curve)
                        curve.Link.Title = pTimeSeriesPlot.EditCurveTitle
                        pTimeSeriesPlot.SettingTitle()
                        pTimeSeriesPlot.AddYAxis(curve)
                        pTimeSeriesPlot.zgTimeSeries.GraphPane.XAxis.IsVisible = True
                        pTimeSeriesPlot.zgTimeSeries.GraphPane.XAxis.Title.Text = "Date and Time"
                    End If
                Catch
                    nodataseriescount -= 1
                End Try
            Else
                Try
                    pTimeSeriesPlot.Remove(curveIndex - nodataseriescount)
                Catch
                    nodataseriescount -= 1
                End Try

            End If

        Else
            If Not selectedSeriesIdList.Contains(SeriesSelector.SelectedSeriesID) Then
                selectedSeriesIdList.Add(SeriesSelector.SelectedSeriesID)
            Else
                Return 'added by jiri to correct error when SeriesCheck event occurs multiple times
            End If

            curveIndex = selectedSeriesIdList.IndexOf(SeriesSelector.SelectedSeriesID)

            If SeriesRowsCount(SeriesSelector.SelectedSeriesID) = 0 Then
                nodataseriescount += 1
            ElseIf Not SeriesSelector.SelectedSeriesID = newseriesID Then
                PlotGraph(SeriesSelector.SelectedSeriesID)
            Else : SeriesSelector.SelectedSeriesID = newseriesID
                Dim curve As LineItem = pTimeSeriesPlot.zgTimeSeries.GraphPane.AddCurve(pTimeSeriesPlot.EditCurveLable, pTimeSeriesPlot.EditCurvePointList, Color.Black, SymbolType.Circle)
                pTimeSeriesPlot.SettingCurveStyle(curve)
                curve.Link.Title = pTimeSeriesPlot.EditCurveTitle
                pTimeSeriesPlot.SettingTitle()
                pTimeSeriesPlot.AddYAxis(curve)
                pTimeSeriesPlot.zgTimeSeries.GraphPane.XAxis.IsVisible = True
                pTimeSeriesPlot.zgTimeSeries.GraphPane.XAxis.Title.Text = "Date and Time"
            End If

        End If

        pTimeSeriesPlot.Refreshing()
    End Sub

    Public Sub btnSelectSeries_Click()
        Dim SQLString As StringBuilder = New StringBuilder
        Dim SeriesSelector = _seriesSelector


        If Not SeriesSelector.SelectedSeriesID = 0 Then

            dgvDataValues.DataSource = Nothing

            initialize()

            newseriesID = SeriesSelector.SelectedSeriesID

            Editdt = Nothing

            SQLString.Append("SELECT ValueID, SeriesID, DataValue, ValueAccuracy, LocalDateTime, UTCOffset, ")
            SQLString.Append("DateTimeUTC, QualifierCode, OffsetValue, OffsetTypeID, CensorCode, SampleID, ")
            SQLString.Append("FileID FROM DataValues AS d LEFT JOIN Qualifiers AS q ON (d.QualifierID = q.QualifierID) ")
            SQLString.Append("WHERE SeriesID = " + newseriesID.ToString)

            Editdt = dbTools.LoadTable(SQLString.ToString)
            Editdt.Columns.Add("Other")
            Editdt.Columns.Add("Selected")
            For i As Integer = 0 To Editdt.Rows.Count - 1
                Editdt.Rows(i)("Other") = 0
                Editdt.Rows(i)("Selected") = 0
            Next

            Originaldt = Editdt.Copy

            dgvDataValues.DataSource = Editdt

            'get the begin and end datetime of the series
            Dim BeginDateTime As Date = dbTools.ExecuteSingleOutput("SELECT BeginDateTime FROM DataSeries WHERE (SeriesID = '" & newseriesID.ToString & "')")
            Dim EndDateTime As Date = dbTools.ExecuteSingleOutput("SELECT EndDateTime FROM DataSeries WHERE (SeriesID = '" & newseriesID.ToString & "')")
            'setting the datetime constrint to larger range

            dtpBefore.MinDate = Today.AddYears(-150)
            dtpBefore.MaxDate = Today.AddDays(1)
            dtpAfter.MinDate = Today.AddYears(-150)
            dtpAfter.MaxDate = Today

            If BeginDateTime <> Nothing Then
                'setting the default datetime values
                dtpAfter.Value = BeginDateTime
                'setting the datetime constrint by the begin and end datetime
                dtpBefore.MinDate = BeginDateTime
                dtpAfter.MinDate = BeginDateTime
            End If
            If EndDateTime <> Nothing Then
                'setting the default datetime values
                dtpBefore.Value = EndDateTime
                'setting the datetime constrint by the begin and end datetime
                dtpBefore.MaxDate = EndDateTime
                dtpAfter.MaxDate = EndDateTime
            End If

            If dbTools.ExecuteSingleOutput("SELECT QualityControlLevelCode FROM DataSeries AS d LEFT JOIN QualityControlLevels AS q ON (d.QualityControlLevelID = q.QualityControlLevelID) WHERE SeriesID = " + newseriesID.ToString).ToString = "Raw Data" Then
                gboxDataFilter.Enabled = False
            Else
                gboxDataFilter.Enabled = True
                rbtnValueThreshold.Select()
            End If
            ResetGridViewStyle()

            Try
                Dim curveIndex As Integer = selectedSeriesIdList.IndexOf(SeriesSelector.SelectedSeriesID)

                If SeriesSelector.CheckedIDList.Contains(SeriesSelector.SelectedSeriesID) Then
                    pTimeSeriesPlot.EnterEditMode(curveIndex - nodataseriescount)
                    pTimeSeriesPlot.RemoveSelectedPoints()
                Else
                    PlotGraph(SeriesSelector.SelectedSeriesID)
                    pTimeSeriesPlot.EnterEditMode(pTimeSeriesPlot.zgTimeSeries.GraphPane.CurveList.Count - 1)
                    pTimeSeriesPlot.Remove(pTimeSeriesPlot.zgTimeSeries.GraphPane.CurveList.Count - 1)
                    pTimeSeriesPlot.Refreshing()
                End If

            Catch
                MsgBox("The Selected Series has no curve")
            End Try

            If SeriesRowsCount(newseriesID) < 1 Then
                gboxDataFilter.Enabled = False
            Else
                gboxDataFilter.Enabled = True
            End If

            Editing = True
        Else
            MsgBox("Please select a series for editing.")
        End If
    End Sub

    'Private Sub SeriesSelector_CriterionChanged(ByVal sender As Object, ByVal e As EventArgs) Handles SeriesSelector.CriterionChanged
    '    'dgvDataValues.DataSource = Nothing
    '    'dgvDataValues.Columns.Clear()
    '    'SeriesSelector.CheckedIDList.Clear()
    '    'selectedSeriesIdList.Clear()
    'End Sub

    Public Sub ckbShowLegend_CheckedChanged()
        If ShowLegend Then
            pTimeSeriesPlot.zgTimeSeries.GraphPane.Legend.IsVisible = True
        Else
            pTimeSeriesPlot.zgTimeSeries.GraphPane.Legend.IsVisible = False
        End If
        If pTimeSeriesPlot.zgTimeSeries.GraphPane.CurveList.Count <= 1 Then
            pTimeSeriesPlot.zgTimeSeries.GraphPane.Legend.IsVisible = False
        End If
        pTimeSeriesPlot.Refreshing()
    End Sub

    Private Sub EditingReminder(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs)
        If Editing And Not Canceled Then
            Dim result As Integer

            result = MsgBox("You are editing a series. Do you want to save before leaving?", MsgBoxStyle.YesNo, "Save?")
            If result = 7 Then

            Else
                SaveGraphChangesToDatabase()
            End If
        End If
    End Sub

#End Region

#End Region

#Region "Editing"

    'Reset style of data grid view
    Public Sub ResetGridViewStyle() Handles dgvDataValues.Sorted
        'dgvDataValues.ReadOnly = True
        For i As Integer = 0 To dgvDataValues.Columns.Count - 1
            dgvDataValues.Columns(i).ReadOnly = True
        Next
        'dgvDataValues.Columns("DataValue").ReadOnly = False
        dgvDataValues.Columns("Other").Visible = False
        dgvDataValues.Columns("Selected").Visible = False

        dgvDataValues.ClearSelection()
        For i As Integer = 0 To dgvDataValues.Rows.Count - 1
            If dgvDataValues.Rows(i).Cells("Other").Value = -1 Then
                dgvDataValues.Rows(i).DefaultCellStyle.BackColor = Color.Red
            Else
                dgvDataValues.Rows(i).DefaultCellStyle.BackColor = Nothing
            End If

            If dgvDataValues.Rows(i).Cells("Selected").Value = 1 Then
                dgvDataValues.Rows(i).Selected = True
            Else
                dgvDataValues.Rows(i).Selected = False
            End If
        Next
    End Sub

    'Derive New Series
    Public Sub btnDeriveNewDataSeries_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        Dim SeriesSelector = _seriesSelector
        'check if the user selected any series, then open the Derive New Series form
        If Not SeriesSelector.SelectedSeriesID = 0 Then
            Dim frmDeriveNewDataSeries As fDeriveNewDataSeries
            frmDeriveNewDataSeries = New fDeriveNewDataSeries()
            frmDeriveNewDataSeries._cEditView = Me
            frmDeriveNewDataSeries._SelectedSeriesID = SeriesSelector.SelectedSeriesID
            frmDeriveNewDataSeries.SetDefault()
            frmDeriveNewDataSeries.ShowDialog()
        Else
            MsgBox("Please select a series to derive.")
        End If
    End Sub

    'Selection of point
    Private Sub dataViewSeries_SelectionChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles dgvDataValues.MouseClick, dgvDataValues.KeyDown, dgvDataValues.KeyUp, dgvDataValues.MouseDoubleClick   ', dgvDataValues.SelectionChanged
        Dim IDlist As New ArrayList

        'Clear all selected points (Z value)
        pTimeSeriesPlot.RemoveSelectedPoints()

        'Select points (Add Z value)
        For i As Integer = 0 To dgvDataValues.SelectedRows.Count - 1
            IDlist.Add(CType(dgvDataValues.SelectedRows(i).Cells("ValueID").Value, Integer))
        Next

        pTimeSeriesPlot.SelectingPoints(IDlist)

        pTimeSeriesPlot.Refreshing()


        For i As Integer = 0 To Editdt.Rows.Count - 1
            If IDlist.Contains(CType(Editdt.Rows(i)("ValueID"), Integer)) Then
                Editdt.Rows(i)("Selected") = 1
            Else
                Editdt.Rows(i)("Selected") = 0
            End If
        Next

    End Sub

    'Apply Changes to Database
    Public Sub btnApplyToDatabase_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)

        If MsgBox("Are You Sure You Want to Apply the Changes to the Database", MsgBoxStyle.YesNo Or vbDefaultButton2, "Question") = MsgBoxResult.Yes Then
            SaveGraphChangesToDatabase()
            MsgBox("Save finished!")
            If SeriesRowsCount(newseriesID) < 1 Then
                gboxDataFilter.Enabled = False
            Else
                gboxDataFilter.Enabled = True
            End If
        End If

    End Sub

    'Restore Data
    Public Sub btnRestoreData_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        'Dim dt As New DataTable

        ''Setting the Data Grid View back to original data
        'dt = Originaldt.Copy
        ''pTimeSeriesPlot.RestoreOriginalData()
        'dgvDataValues.DataSource = dt
        'ResetGridViewStyle()
        'SaveGraphChangesToDatabase()

        ''Setting the Graph
        'selectedSeriesIdList.Remove(newseriesID)
        'selectedSeriesIdList.Add(newseriesID)
        'With pTimeSeriesPlot.zgTimeSeries.GraphPane
        '    For j As Integer = 0 To .CurveList.Count - 1
        '        If .CurveList(j).Color = CurveEditingColor Then
        '            .CurveList.Remove(.CurveList(j))
        '        End If
        '    Next
        'End With
        'PlotGraph(newseriesID)
        'Dim curveIndex As Integer = selectedSeriesIdList.IndexOf(newseriesID)
        'pTimeSeriesPlot.EnterEditMode(curveIndex - nodataseriescount)

        'initialize()
        If MsgBox("Are You Sure You Want to Restore the Data to the Original?", MsgBoxStyle.YesNo Or vbDefaultButton2, "Question") = MsgBoxResult.Yes Then

            Editdt = Originaldt.Copy
            RefreshDataGridView()
            pTimeSeriesPlot.ReplotEditingCurve(Editdt)

            If SeriesRowsCount(newseriesID) < 1 Then
                gboxDataFilter.Enabled = False
            Else
                gboxDataFilter.Enabled = True
            End If

            MsgBox("Restore Complete!")
        End If

    End Sub

    'Clear Filter
    Private Sub btnClearFilter_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnClearFilter.Click
        'Make all text boxes blank
        ddlTimePeriod.SelectedItem = ddlTimePeriod.Items(0)
        txtDataGapValue.Text = ""
        txtEditDFVTChange.Text = ""
        txtValueLarger.Text = ""
        txtValueLess.Text = ""
        rbtnValueThreshold.Select()
        dgvDataValues.ClearSelection()
        pTimeSeriesPlot.RemoveSelectedPoints()
    End Sub

    'radio buttons change events
    Private Sub gboxradiobuttons_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs)
        'Enable different group box when different radio button is checked

        If rbtnValueThreshold.Checked Then
            gboxValueThreshold.Enabled = True
            gboxDataGap.Enabled = False
            gboxDate.Enabled = False
            txtEditDFVTChange.Enabled = False
        End If

        If rbtnDataGap.Checked Then
            gboxValueThreshold.Enabled = False
            gboxDataGap.Enabled = True
            gboxDate.Enabled = False
            txtEditDFVTChange.Enabled = False
        End If

        If rbtnDate.Checked Then
            gboxValueThreshold.Enabled = False
            gboxDataGap.Enabled = False
            gboxDate.Enabled = True
            txtEditDFVTChange.Enabled = False
        End If

        If rbtnEditDFVTChange.Checked Then
            gboxValueThreshold.Enabled = False
            gboxDataGap.Enabled = False
            gboxDate.Enabled = False
            txtEditDFVTChange.Enabled = True
        End If

    End Sub

    'Filter
    Private Sub btnApplyFilter_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnApplyFilter.Click
        'check which method the user wanted to choose the range of data
        'Value Threshold Filter
        If rbtnValueThreshold.Checked Then

            'Validation
            If Not IsNumeric(txtValueLarger.Text) And (Not txtValueLarger.Text = Nothing) Then
                MsgBox("Please enter numbers")
                Exit Sub
            End If
            If Not IsNumeric(txtValueLess.Text) And Not (txtValueLess.Text = Nothing) Then
                MsgBox("Please enter numbers")
                Exit Sub
            End If

            'Filter
            If Not (txtValueLarger.Text = Nothing) And Not (txtValueLess.Text = Nothing) Then
                If pTimeSeriesPlot.HasEditingCurve() Then
                    pTimeSeriesPlot.ChangeZvalueWithValueThreshold(Val(txtValueLarger.Text), Val(txtValueLess.Text))
                Else
                    ValueThresholdFilter(Val(txtValueLarger.Text), Val(txtValueLess.Text))
                End If
            ElseIf txtValueLarger.Text = Nothing And Not (txtValueLess.Text = Nothing) Then
                Dim largest As Decimal = Convert.ToDecimal(dbTools.ExecuteSingleOutput("SELECT MAX(DataValue) FROM DataValues WHERE SeriesID = " + newseriesID.ToString))
                If pTimeSeriesPlot.HasEditingCurve() Then
                    pTimeSeriesPlot.ChangeZvalueWithValueThreshold(largest, Val(txtValueLess.Text))
                Else
                    ValueThresholdFilter(largest, Val(txtValueLess.Text))
                End If
            ElseIf txtValueLess.Text = Nothing And Not (txtValueLarger.Text = Nothing) Then
                Dim smallest As Decimal = Convert.ToDecimal(dbTools.ExecuteSingleOutput("SELECT MIN(DataValue) FROM DataValues WHERE SeriesID = " + newseriesID.ToString))
                If pTimeSeriesPlot.HasEditingCurve() Then
                    pTimeSeriesPlot.ChangeZvalueWithValueThreshold(Val(txtValueLarger.Text), smallest)
                Else
                    ValueThresholdFilter(Val(txtValueLarger.Text), smallest)
                End If
            End If
        End If

        'Value Change Threshold Filter
        If rbtnEditDFVTChange.Checked Then
            'Validation
            If Not IsNumeric(txtEditDFVTChange.Text) And Not (txtEditDFVTChange.Text) = Nothing Then
                MsgBox("Please enter numbers")
                Exit Sub
            End If

            'Filter
            If pTimeSeriesPlot.HasEditingCurve() Then
                pTimeSeriesPlot.ChangeZvalueWithValueChangeThreshold(Val(txtEditDFVTChange.Text))
            Else
                ValueChangeThreshold(Val(txtEditDFVTChange.Text))
            End If
        End If

        'Date Filter
        If rbtnDate.Checked Then
            If pTimeSeriesPlot.HasEditingCurve() Then
                pTimeSeriesPlot.ChangeZvalueWithDate(dtpBefore.Value, dtpAfter.Value)
            Else
                DateFilter(dtpBefore.Value, dtpAfter.Value)
            End If
        End If

        'Data Gap Filter
        If rbtnDataGap.Checked Then
            'Validation
            If Not IsNumeric(txtDataGapValue.Text) And Not (txtDataGapValue.Text) = Nothing Then
                MsgBox("Please enter numbers")
                Exit Sub
            End If

            'Filter
            Dim datagap As Double
            Select Case ddlTimePeriod.SelectedIndex
                Case 0
                    datagap = Val(txtDataGapValue.Text)
                Case 1
                    datagap = Val(txtDataGapValue.Text) * 60
                Case 2
                    datagap = Val(txtDataGapValue.Text) * 60 * 60
                Case 3
                    datagap = Val(txtDataGapValue.Text) * 60 * 60 * 24
                Case Else
                    datagap = Val(txtDataGapValue.Text)
            End Select

            If pTimeSeriesPlot.HasEditingCurve() Then
                pTimeSeriesPlot.ChangeZvalueWithDataGap(datagap)
            Else
                DataGapFilter(datagap)
            End If

        End If

        If pTimeSeriesPlot.HasEditingCurve() Then
            ReflectZvalue()
        End If



        pTimeSeriesPlot.Refreshing()

    End Sub

    'Change Y value by add, minus or setting it directing
    Public Sub btnChangeYValue_Click()
        If Editing Then
            If dgvDataValues.SelectedRows.Count >= 1 Then
                Dim frmChangeYValue As fChangeYValue
                frmChangeYValue = New fChangeYValue()
                frmChangeYValue._cEditView = Me
                'frmChangeYValue.Show()

                frmChangeYValue.ShowDialog()
                frmChangeYValue = Nothing
            Else
                MsgBox(ErrMsgForNotPointSelected)
            End If
        Else
            MsgBox(ErrMsgForNotEditing)
        End If
    End Sub

    'Change Y value by Interpolating
    Public Sub btnInterpolate_Click()
        If Editing Then
            If MsgBox("Are You Sure You Want to Interpolate the Selected Values?", MsgBoxStyle.YesNo Or vbDefaultButton2, "Question") = MsgBoxResult.Yes Then
                If dgvDataValues.SelectedRows.Count >= 1 Then
                    Dim returned As Boolean = False
                    If pTimeSeriesPlot.HasEditingCurve Then
                        'pTimeSeriesPlot.ChangeValueByInterpolating(returned)
                        ChangeValueByInterpolating(returned)
                        pTimeSeriesPlot.ReplotEditingCurve(Editdt)
                    Else
                        ChangeValueByInterpolating(returned)
                    End If

                    'If Not returned Then

                    '    If pTimeSeriesPlot.HasEditingCurve Then
                    '        ReflectChanges()
                    '    End If
                    'End If

                    RefreshDataGridView()
                Else
                    MsgBox(ErrMsgForNotPointSelected)
                End If
            End If
        Else
            MsgBox(ErrMsgForNotEditing)
        End If

    End Sub


    'Public Sub btnAddNewPoint_Click()
    '    If Editing Then
    '        Dim frmAddNewPoint As fAddNewPoint = New fAddNewPoint()

    '        frmAddNewPoint._cEditView = Me
    '        frmAddNewPoint.ShowDialog()
    '    Else
    '        MsgBox(ErrMsgForNotEditing)
    '    End If
    'End Sub


    Public Sub btnFlag_Click()
        If Editing Then
            If dgvDataValues.SelectedRows.Count >= 1 Then
                Dim QualifiersTableManagement As New fQualifiersTableManagement()
                QualifiersTableManagement._cEditView = Me
                QualifiersTableManagement.ShowDialog()
                QualifiersTableManagement = Nothing
            Else
                MsgBox(ErrMsgForNotPointSelected)
            End If
        Else
            MsgBox(ErrMsgForNotEditing)
        End If
    End Sub




    'Associate changes to table from graph
    'Public Sub ReflectChanges()
    '    Dim j As Integer
    '    Dim eCurve As CurveItem = pTimeSeriesPlot.EditingCurve

    '    For i As Integer = 0 To eCurve.Points.Count - 1

    '        j = 0
    '        Do Until dgvDataValues.Rows(j).Cells("ValueID").Value = Val(pTimeSeriesPlot.PointValueID(i))
    '            j += 1
    '        Loop
    '        dgvDataValues.Rows(j).Cells("DataValue").Value = Val(eCurve.Points(i).Y)

    '    Next
    'End Sub

    'Associate changes to graph from table when "DataValue" changed
    'Private Sub dgvDataValues_CellValueChanged(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles dgvDataValues.CellValueChanged
    '    If e.ColumnIndex = 2 Then
    '        If pTimeSeriesPlot.HasEditingCurve Then
    '            Dim j As Integer = 0
    '            Dim eCurve As CurveItem = pTimeSeriesPlot.EditingCurve
    '            For i As Integer = 0 To pTimeSeriesPlot.zgTimeSeries.GraphPane.CurveList.Count - 1
    '                If pTimeSeriesPlot.zgTimeSeries.GraphPane.CurveList(i).Color = CurveEditingColor Then
    '                    eCurve = pTimeSeriesPlot.zgTimeSeries.GraphPane.CurveList(i)
    '                End If
    '            Next
    '            'if not adding a new point 
    '            If Not dgvDataValues.AllowUserToAddRows = True Then

    '                'If data value is changing
    '                If e.ColumnIndex = dgvDataValues.Columns("DataValue").Index Then
    '                    Do Until pTimeSeriesPlot.PointValueID(j) = dgvDataValues.Rows(e.RowIndex).Cells("ValueID").Value
    '                        j += 1
    '                    Loop
    '                    eCurve.Points(j).Y = dgvDataValues.Rows(e.RowIndex).Cells("DataValue").Value
    '                End If
    '            End If

    '            'Allow user to save the changes
    '            btnApplyToDatabase.Enabled = True
    '            btnRestoreData.Enabled = True

    '            pTimeSeriesPlot.Refreshing()
    '        End If
    '    End If
    'End Sub

    'Adding a point
    Public Sub btnAddNewPoint_Click()
        If Editing Then
            Dim frmAddNewPoint As fAddNewPoint = New fAddNewPoint()
            If dgvDataValues.SelectedRows.Count = 2 Then
                With dgvDataValues
                    If .SelectedRows(0).Cells("LocalDateTime").Value < .SelectedRows(1).Cells("LocalDateTime").Value Then
                        frmAddNewPoint._FirstDate = Convert.ToDateTime(.SelectedRows(0).Cells("LocalDateTime").Value)
                        frmAddNewPoint._SecondDate = Convert.ToDateTime(.SelectedRows(1).Cells("LocalDateTime").Value)
                    Else
                        frmAddNewPoint._FirstDate = Convert.ToDateTime(.SelectedRows(1).Cells("LocalDateTime").Value)
                        frmAddNewPoint._SecondDate = Convert.ToDateTime(.SelectedRows(0).Cells("LocalDateTime").Value)
                    End If
                End With
                frmAddNewPoint.AutoDateTime()
            End If
            frmAddNewPoint._cEditView = Me
            frmAddNewPoint.ShowDialog()
        Else
            MsgBox(ErrMsgForNotEditing)
        End If
    End Sub

    'Delete the selected point
    Public Sub btnDeletePoint_Click()
        If Editing Then
            If dgvDataValues.SelectedRows.Count >= 1 Then
                If MsgBox("Do you want to delete the point/points?", MsgBoxStyle.OkCancel, "Delete point") = MsgBoxResult.Ok Then
                    For i As Integer = 0 To Editdt.Rows.Count - 1
                        If Editdt.Rows(i)("Selected") = 1 Then
                            Editdt.Rows(i)("Other") = -1
                        End If
                    Next
                    'If pTimeSeriesPlot.HasEditingCurve Then
                    '    pTimeSeriesPlot.DeletingPoints()
                    'End If
                End If
                pTimeSeriesPlot.ReplotEditingCurve(Editdt)
                RefreshDataGridView()

            Else
                MsgBox(ErrMsgForNotPointSelected)
            End If
        Else
            MsgBox(ErrMsgForNotEditing)
        End If
    End Sub

    'Associate Table selection with Zvalue(selected points) in the graph Method
    Public Sub ReflectZvalue()
        Dim IDlist As New ArrayList 'List(Of Integer)
        Dim eCurve As CurveItem = pTimeSeriesPlot.zgTimeSeries.GraphPane.CurveList(0)
        For i As Integer = 0 To pTimeSeriesPlot.zgTimeSeries.GraphPane.CurveList.Count - 1
            If pTimeSeriesPlot.zgTimeSeries.GraphPane.CurveList(i).Color = CurveEditingColor Then
                eCurve = pTimeSeriesPlot.zgTimeSeries.GraphPane.CurveList(i)
            End If
        Next

        For i As Integer = 0 To eCurve.Points.Count - 1
            If eCurve.Points(i).Z = 1 Then
                IDlist.Add(Val(pTimeSeriesPlot.PointValueID(i)))
            End If
        Next

        For i As Integer = 0 To dgvDataValues.Rows.Count - 1
            If IDlist.Contains(Val(dgvDataValues.Rows(i).Cells("ValueID").Value)) Then
                dgvDataValues.Rows(i).Selected = True
            Else
                dgvDataValues.Rows(i).Selected = False
            End If
        Next

        dataViewSeries_SelectionChanged(New System.Object, New System.EventArgs)

        'pTimeSeriesPlot.SelectingPoints(IDlist)
        'pTimeSeriesPlot.Refreshing()
    End Sub

    'Saving changes Method
    Public Sub SaveGraphChangesToDatabase()
        Dim SQLstring As String
        Dim SQLstring2 As String
        Dim datavalue As Double
        'Dim QualifierCode As String
        Dim ValueID As Integer
        Dim RowIndexList As New List(Of Integer)
        Dim RestoreDeletedPoint As Boolean = False
        Dim dt As New DataTable
        Dim ValueIDList As New List(Of Integer)


        'Deleting added points after restore data
        For i As Integer = 0 To dgvDataValues.Rows.Count - 1
            ValueIDList.Add(dgvDataValues.Rows(i).Cells("ValueID").Value)
        Next
        dt = dbTools.LoadTable("SELECT ValueID FROM DataValues WHERE SeriesID = " + newseriesID.ToString)
        For i As Integer = 0 To dt.Rows.Count - 1
            If Not ValueIDList.Contains(dt.Rows(i)("ValueID")) Then
                SQLstring = "DELETE FROM DataValues WHERE ValueID = " + dt.Rows(i)("ValueID").ToString
                dbTools.ExecuteNonQuery(SQLstring)
            End If
        Next


        'Setting progress bar
        Dim frmloading As ProgressBar = pbProgressBar
        frmloading.Visible = True
        frmloading.Maximum = dgvDataValues.Rows.Count - 1
        frmloading.Minimum = 0
        frmloading.Value = 0

        lblstatus.Text = "Saving..."
        SQLstring2 = "BEGIN TRANSACTION; "
        'saving by table
        For i As Integer = 0 To Editdt.Rows.Count - 1

            ValueID = Editdt.Rows(i)("ValueID")

            'deleting point

            If Not Editdt.Rows(i)("Other") = 0 Then
                'Deleteing point
                If Editdt.Rows(i)("Other") = -1 Then
                    SQLstring = "DELETE FROM DataValues WHERE ValueID = " + ValueID.ToString + "; "
                    SQLstring2 += SQLstring


                    'Adding point
                ElseIf Editdt.Rows(i)("Other") = 1 Then
                    If dbTools.ExecuteSingleOutput("Select ValueID FROM DataValues WHERE ValueID = " + ValueID.ToString) = Nothing Then
                        SQLstring = "INSERT INTO DataValues (ValueID,SeriesID,DataValue,ValueAccuracy,LocalDateTime,UTCOffset,DateTimeUTC, "
                        SQLstring += "OffsetValue, OffsetTypeID, CensorCode, QualifierID, SampleID, FileID) VALUES ("
                        'ValueID,SeriesID,DataValue
                        For j As Integer = 0 To 2
                            SQLstring += Editdt.Rows(i)(j).ToString + ","
                        Next
                        'ValueAccuracy
                        If Editdt.Rows(i)(3) Is DBNull.Value Then
                            SQLstring += "NULL,"
                        Else
                            SQLstring += Editdt.Rows(i)(3).ToString + ","
                        End If
                        'LocalDateTime
                        SQLstring += "'" + Convert.ToDateTime(Editdt.Rows(i)(4)).ToString("yyyy-MM-dd HH:mm:ss") + "',"
                        'UTCOffset
                        SQLstring += Editdt.Rows(i)(5).ToString
                        'DateTimeUTC
                        SQLstring += ",'" + Convert.ToDateTime(Editdt.Rows(i)(6)).ToString("yyyy-MM-dd HH:mm:ss") + "',"
                        'OffsetValue
                        If Editdt.Rows(i)(8) Is DBNull.Value Then
                            SQLstring += "NULL,"
                        Else
                            SQLstring += Editdt.Rows(i)(8).ToString + ","
                        End If
                        'OffsetTypeID
                        If Editdt.Rows(i)(9) Is DBNull.Value Then
                            SQLstring += "NULL,"
                        Else
                            SQLstring += Editdt.Rows(i)(9).ToString + ","
                        End If
                        'CensorCode
                        If Editdt.Rows(i)(10) Is DBNull.Value Then
                            SQLstring += "NULL,"
                        Else
                            SQLstring += "'" + Editdt.Rows(i)(10).ToString + "',"
                        End If
                        'QualifierID
                        If Editdt.Rows(i)(7) Is DBNull.Value Then
                            SQLstring += "NULL,"
                        Else
                            SQLstring += GetQualifierID(Editdt.Rows(i)(7).ToString).ToString + ","
                        End If
                        'SampleID
                        If Editdt.Rows(i)(11) Is DBNull.Value Then
                            SQLstring += "NULL,"
                        Else
                            SQLstring += Editdt.Rows(i)(11).ToString() + ","
                        End If
                        'FileID
                        If Editdt.Rows(i)(12) Is DBNull.Value Then
                            SQLstring += "NULL)"
                        Else
                            SQLstring += Editdt.Rows(i)(12).ToString() + "); "
                        End If

                        SQLstring2 += SQLstring
                    End If


                    'updating point
                ElseIf Editdt.Rows(i)("Other") = 2 Then
                    'Update 
                    If Not datavalue = dgvDataValues.Rows(i).Cells("DataValue").Value Then
                        SQLstring = "UPDATE DataValues SET DataValue = "
                        SQLstring += Editdt.Rows(i)("DataValue").ToString + ", QualifierID = "
                        SQLstring += GetQualifierID(ReadQualifierCode(Editdt.Rows(i)("QualifierCode"))).ToString
                        SQLstring += " WHERE ValueID = "
                        SQLstring += ValueID.ToString + "; "

                        SQLstring2 += SQLstring

                    End If
                End If
            End If

            frmloading.Value = i
        Next

        SQLstring2 += "COMMIT;"

        dbTools.ExecuteNonQuery(SQLstring2)

        'Update DataSeries
        'UpdateDataSeries(newseriesID)

        ''Remove rows from dgvDataValues where is deleted
        'For i As Integer = 0 To dgvDataValues.Rows.Count - 1
        '    If dgvDataValues.Rows(i).Cells("Other").Value = -1 Then
        '        RowIndexList.Add(i)
        '    End If
        'Next

        'If RowIndexList.Count > 0 Then
        '    For i As Integer = RowIndexList.Count - 1 To 0
        '        dgvDataValues.Rows.Remove(dgvDataValues.Rows(RowIndexList(i)))
        '    Next
        'End If



        'Update Data Series
        DataSeriesHandling.UpdateDataSeriesFromDataValues(newseriesID)


        RefreshDataGridView()
        pTimeSeriesPlot.ReplotEditingCurve(Editdt)

        frmloading.Value = 0
        lblstatus.Text = "Ready"

    End Sub

    Private Function ReadQualifierCode(ByVal rowValue As Object)
        If IsDBNull(rowValue) Then
            Return String.Empty
        Else
            Return rowValue.ToString()
        End If
    End Function

    'Count the rows of a series
    Public Function SeriesRowsCount(ByVal SeriesID As Integer) As Integer
        'Dim dt As New DataTable
        'dt = dbTools.LoadTable("DataValues", "SELECT * FROM DataValues WHERE SeriesID = " + SeriesID.ToString)
        'Return dt.Rows.Count
        Dim rowCount As Object = dbTools.ExecuteSingleOutput("SELECT ValueCount FROM DataSeries WHERE SeriesID = '" + SeriesID.ToString() + "'")
        Return CInt(rowCount)
    End Function

    'Update DataSeries Table in the database
    Private Sub UpdateDataSeries(ByVal SeriesID As Integer)
        Dim SQLstring As String
        Dim BeginDateTime As DateTime
        Dim EndDateTime As DateTime
        Dim BeginDateTimeUTC As DateTime
        Dim EndDateTimeUTC As DateTime

        SQLstring = "SELECT LocalDateTime FROM DataValues WHERE SeriesID = " + SeriesID.ToString + " ORDER BY LocalDateTime"
        BeginDateTime = dbTools.ExecuteSingleOutput(SQLstring)
        SQLstring = "SELECT LocalDateTime FROM DataValues WHERE SeriesID = " + SeriesID.ToString + " ORDER BY LocalDateTime DESC"
        EndDateTime = dbTools.ExecuteSingleOutput(SQLstring)
        SQLstring = "SELECT DateTimeUTC FROM DataValues WHERE SeriesID = " + SeriesID.ToString + " ORDER BY DateTimeUTC"
        BeginDateTimeUTC = dbTools.ExecuteSingleOutput(SQLstring)
        SQLstring = "SELECT DateTimeUTC FROM DataValues WHERE SeriesID = " + SeriesID.ToString + " ORDER BY DateTimeUTC DESC"
        EndDateTimeUTC = dbTools.ExecuteSingleOutput(SQLstring)

        SQLstring = "UPDATE DataSeries SET ValueCount = " + SeriesRowsCount(SeriesID).ToString + ", "
        SQLstring += "BeginDateTime = '" + BeginDateTime.ToString("yyyy-MM-dd HH:mm:ss") + "', "
        SQLstring += "EndDateTime = '" + EndDateTime.ToString("yyyy-MM-dd HH:mm:ss") + "', "
        SQLstring += "BeginDateTimeUTC = '" + BeginDateTimeUTC.ToString("yyyy-MM-dd HH:mm:ss") + "', "
        SQLstring += "EndDateTimeUTC = '" + EndDateTimeUTC.ToString("yyyy-MM-dd HH:mm:ss") + "', "
        SQLstring += "UpdateDateTime = '" + Now.ToString("yyyy-MM-dd HH:mm:ss") + "' "
        SQLstring += "WHERE SeriesID = " + SeriesID.ToString

        dbTools.ExecuteNonQuery(SQLstring)
    End Sub

    'Reload the Data Grid View
    Public Sub RefreshDataGridView()
        Editdt.DefaultView.Sort = "[LocalDateTime] Asc"

        dgvDataValues.DataSource = Editdt.DefaultView
        ResetGridViewStyle()
    End Sub

#Region "Filters"
    'Value Threshold Filter
    Public Sub ValueThresholdFilter(ByVal LargerThanValue As Double, ByVal LessThanValue As Double)
        For i As Integer = 0 To dgvDataValues.Rows.Count - 1
            If LargerThanValue < LessThanValue Then
                If dgvDataValues.Rows(i).Cells("DataValue").Value > LargerThanValue And dgvDataValues.Rows(i).Cells("DataValue").Value < LessThanValue Then
                    dgvDataValues.Rows(i).Selected = True
                Else
                    dgvDataValues.Rows(i).Selected = False
                End If
            Else
                If dgvDataValues.Rows(i).Cells("DataValue").Value > LargerThanValue Or dgvDataValues.Rows(i).Cells("DataValue").Value < LessThanValue Then
                    dgvDataValues.Rows(i).Selected = True
                Else
                    dgvDataValues.Rows(i).Selected = False
                End If
            End If
        Next

        'Selecting the background data table
        For i As Integer = 0 To Editdt.Rows.Count - 1
            If LargerThanValue < LessThanValue Then
                If Editdt.Rows(i)("DataValue") > LargerThanValue And Editdt.Rows(i)("DataValue") < LessThanValue Then
                    Editdt.Rows(i)("Selected") = 1
                Else
                    Editdt.Rows(i)("Selected") = 0
                End If
            Else
                If Editdt.Rows(i)("DataValue") > LargerThanValue Or Editdt.Rows(i)("DataValue") < LessThanValue Then
                    Editdt.Rows(i)("Selected") = 1
                Else
                    Editdt.Rows(i)("Selected") = 0
                End If
            End If
        Next
    End Sub

    'Value Change Threshold Filter
    Public Sub ValueChangeThreshold(ByVal ValueChange As Double)
        dgvDataValues.ClearSelection()

        For i As Integer = 1 To dgvDataValues.Rows.Count - 1
            If (Math.Abs(dgvDataValues.Rows(i).Cells("DataValue").Value - dgvDataValues.Rows(i - 1).Cells("DataValue").Value) > ValueChange) Then
                dgvDataValues.Rows(i).Selected = True
                dgvDataValues.Rows(i - 1).Selected = True
            End If
        Next



        'Selecting the background data table
        Editdt.Rows(0)("Selected") = 0
        For i As Integer = 1 To Editdt.Rows.Count - 1
            If (Math.Abs(Editdt.Rows(i)("DataValue") - Editdt.Rows(i - 1)("DataValue")) > ValueChange) Then
                Editdt.Rows(i)("Selected") = 1
                Editdt.Rows(i - 1)("Selected") = 1
            Else
                Editdt.Rows(i)("Selected") = 0
            End If
        Next
    End Sub

    'Date Filter
    Public Sub DateFilter(ByVal DateBefore As DateTime, ByVal DateAfter As DateTime)
        For i As Integer = 0 To dgvDataValues.Rows.Count - 1
            If DateAfter > DateBefore Then
                If dgvDataValues.Rows(i).Cells("LocalDateTime").Value >= DateAfter.ToOADate Or dgvDataValues.Rows(i).Cells("LocalDateTime").Value <= DateBefore.ToOADate Then
                    dgvDataValues.Rows(i).Selected = True
                Else
                    dgvDataValues.Rows(i).Selected = False
                End If
            Else
                If dgvDataValues.Rows(i).Cells("LocalDateTime").Value >= DateAfter.ToOADate And dgvDataValues.Rows(i).Cells("LocalDateTime").Value <= DateBefore.ToOADate Then
                    dgvDataValues.Rows(i).Selected = True
                Else
                    dgvDataValues.Rows(i).Selected = False
                End If
            End If
        Next


        'Selecting the background data table
        For i As Integer = 0 To Editdt.Rows.Count - 1
            If DateAfter > DateBefore Then
                If Editdt.Rows(i)("LocalDateTime") >= DateAfter.ToOADate Or Editdt.Rows(i)("LocalDateTime") <= DateBefore.ToOADate Then
                    Editdt.Rows(i)("Selected") = 1
                Else
                    Editdt.Rows(i)("Selected") = 0
                End If
            Else
                If Editdt.Rows(i)("LocalDateTime") >= DateAfter.ToOADate And Editdt.Rows(i)("LocalDateTime") <= DateBefore.ToOADate Then
                    Editdt.Rows(i)("Selected") = 1
                Else
                    Editdt.Rows(i)("Selected") = 0
                End If
            End If
        Next
    End Sub

    'Data Gap Filter
    Public Sub DataGapFilter(ByVal GapInSecond As Integer)
        Dim different As Long
        Dim date1 As DateTime
        Dim date2 As DateTime

        dgvDataValues.Rows(0).Selected = False

        For i As Integer = 1 To dgvDataValues.Rows.Count - 1
            date1 = dgvDataValues.Rows(i).Cells("LocalDateTime").Value
            date2 = dgvDataValues.Rows(i - 1).Cells("LocalDateTime").Value
            different = DateDiff(DateInterval.Second, date1, date2, FirstDayOfWeek.Monday, FirstWeekOfYear.Jan1)

            If Math.Abs(different) > GapInSecond Then
                dgvDataValues.Rows(i).Selected = True
                dgvDataValues.Rows(i - 1).Selected = True
            Else
                dgvDataValues.Rows(i).Selected = False
            End If

        Next



        'Selecting the background data table
        Editdt.Rows(0)("Selected") = 0
        For i As Integer = 1 To Editdt.Rows.Count - 1
            date1 = Editdt.Rows(i)("LocalDateTime")
            date2 = Editdt.Rows(i - 1)("LocalDateTime")
            different = DateDiff(DateInterval.Second, date1, date2, FirstDayOfWeek.Monday, FirstWeekOfYear.Jan1)

            If Math.Abs(different) > GapInSecond Then
                Editdt.Rows(i)("Selected") = 1
                Editdt.Rows(i - 1)("Selected") = 1
            Else
                Editdt.Rows(i)("Selected") = 0
            End If
        Next
    End Sub

#End Region

#Region "Change Value Function"

    Public Sub ChangeValueByAddOrMinus(ByVal Adding As Boolean, ByVal values As Double)
        'For i As Integer = 0 To dgvDataValues.SelectedRows.Count - 1
        '    'changing value for the data grid view
        '    If Adding Then
        '        dgvDataValues.SelectedRows(i).Cells("DataValue").Value += values
        '    Else
        '        dgvDataValues.SelectedRows(i).Cells("DataValue").Value -= values
        '    End If

        '    'changing value for background curve
        '    For j As Integer = 0 To pTimeSeriesPlot.EditCurvePointList.Count - 1
        '        If pTimeSeriesPlot.EditCurvePointList(j).Tag.ToString.Substring(0, pTimeSeriesPlot.EditCurvePointList(j).Tag.ToString.IndexOf(",")) = dgvDataValues.SelectedRows(i).Cells("ValueID").Value Then
        '            pTimeSeriesPlot.EditCurvePointList(j).Y = dgvDataValues.SelectedRows(i).Cells("DataValue").Value

        '        End If
        '    Next
        'Next

        'changing value for background data table
        For i As Integer = 0 To Editdt.Rows.Count - 1
            If Editdt.Rows(i)("Selected") = 1 Then
                If Adding Then
                    Editdt.Rows(i)("DataValue") += values
                Else
                    Editdt.Rows(i)("DataValue") -= values
                End If

                If Not Editdt.Rows(i)("Other") = -1 And Not Editdt.Rows(i)("Other") = 1 Then
                    Editdt.Rows(i)("Other") = 2
                End If

            End If
        Next



    End Sub

    Public Sub ChangeValueByMultiply(ByVal Multiplier As Double)
        'For i As Integer = 0 To dgvDataValues.SelectedRows.Count - 1
        '    dgvDataValues.SelectedRows(i).Cells("DataValue").Value *= Multiplier

        '    For j As Integer = 0 To pTimeSeriesPlot.EditCurvePointList.Count - 1
        '        If pTimeSeriesPlot.EditCurvePointList(j).Tag.ToString.Substring(0, pTimeSeriesPlot.EditCurvePointList(j).Tag.ToString.IndexOf(",")) = dgvDataValues.SelectedRows(i).Cells("ValueID").Value Then
        '            pTimeSeriesPlot.EditCurvePointList(j).Y = dgvDataValues.SelectedRows(i).Cells("DataValue").Value

        '        End If
        '    Next
        'Next

        'changing value for background data table
        For i As Integer = 0 To Editdt.Rows.Count - 1
            If Editdt.Rows(i)("Selected") = 1 Then
                Editdt.Rows(i)("DataValue") *= Multiplier
                If Not Editdt.Rows(i)("Other") = -1 And Not Editdt.Rows(i)("Other") = 1 Then
                    Editdt.Rows(i)("Other") = 2
                End If
            End If
        Next
    End Sub

    Public Sub ChangeValueBySettingValue(ByVal value As Double)
        'For i As Integer = 0 To dgvDataValues.SelectedRows.Count - 1
        '    dgvDataValues.SelectedRows(i).Cells("DataValue").Value = value

        '    For j As Integer = 0 To pTimeSeriesPlot.EditCurvePointList.Count - 1
        '        If pTimeSeriesPlot.EditCurvePointList(j).Tag.ToString.Substring(0, pTimeSeriesPlot.EditCurvePointList(j).Tag.ToString.IndexOf(",")) = dgvDataValues.SelectedRows(i).Cells("ValueID").Value Then
        '            pTimeSeriesPlot.EditCurvePointList(j).Y = dgvDataValues.SelectedRows(i).Cells("DataValue").Value

        '        End If
        '    Next
        'Next


        'changing value for background data table
        For i As Integer = 0 To Editdt.Rows.Count - 1
            If Editdt.Rows(i)("Selected") = 1 Then
                Editdt.Rows(i)("DataValue") = value
                If Not Editdt.Rows(i)("Other") = -1 And Not Editdt.Rows(i)("Other") = 1 Then
                    Editdt.Rows(i)("Other") = 2
                End If
            End If
        Next
    End Sub

    Public Sub ChangeValueByInterpolating(ByRef returned As Boolean)
        Dim i As Integer = 1
        Dim count As Integer = 1
        Dim difference As Double

        'Check if the first point and last point is selected
        If dgvDataValues.Rows(0).Selected Or dgvDataValues.Rows(dgvDataValues.Rows.Count - 1).Selected Then
            MsgBox("You cannot interpolate with the first point or last point selected.")
            returned = True
            Return
        End If

        'Interpolating
        'Do Until (i >= dgvDataValues.Rows.Count - 2)
        '    If dgvDataValues.Rows(i).Selected Then
        '        If dgvDataValues.Rows(i + 1).Selected = False Then
        '            difference = (dgvDataValues.Rows(i - 1).Cells("DataValue").Value + dgvDataValues.Rows(i + 1).Cells("DataValue").Value)
        '            dgvDataValues.Rows(i).Cells("DataValue").Value = difference / 2
        '            i += 1
        '        Else
        '            count = 1
        '            Do Until (dgvDataValues.Rows(i + 1).Selected = False)
        '                count += 1
        '                i += 1
        '            Loop
        '            difference = (dgvDataValues.Rows(i + 1).Cells("DataValue").Value + dgvDataValues.Rows(i - count).Cells("DataValue").Value)
        '            For j As Integer = 1 To count
        '                dgvDataValues.Rows(i + 1 - j).Cells("DataValue").Value = dgvDataValues.Rows(i + 1).Cells("DataValue").Value - difference / (count + 1) * j
        '            Next
        '        End If
        '    Else
        '        i += 1
        '    End If
        'Loop

        'For k As Integer = 0 To dgvDataValues.SelectedRows.Count - 1
        '    For j As Integer = 0 To pTimeSeriesPlot.EditCurvePointList.Count - 1
        '        If pTimeSeriesPlot.EditCurvePointList(j).Tag.ToString.Substring(0, pTimeSeriesPlot.EditCurvePointList(j).Tag.ToString.IndexOf(",")) = dgvDataValues.SelectedRows(k).Cells("ValueID").Value Then
        '            pTimeSeriesPlot.EditCurvePointList(j).Y = dgvDataValues.SelectedRows(k).Cells("DataValue").Value

        '        End If
        '    Next
        'Next



        'changing value for background data table
        i = 1
        Do Until (i >= Editdt.Rows.Count - 2)
            If Editdt.Rows(i)("Selected") = 1 Then
                If Not Editdt.Rows(i + 1)("Selected") = 1 Then
                    difference = Editdt.Rows(i - 1)("DataValue") + Editdt.Rows(i + 1)("DataValue")
                    Editdt.Rows(i)("DataValue") = difference / 2
                    If Not Editdt.Rows(i)("Other") = -1 And Not Editdt.Rows(i)("Other") = 1 Then
                        Editdt.Rows(i)("Other") = 2
                    End If
                    i += 1
                Else
                    count = 1
                    Do Until Not (Editdt.Rows(i + 1)("Selected") = 1)
                        count += 1
                        i += 1
                    Loop
                    difference = (Editdt.Rows(i + 1)("DataValue") - Editdt.Rows(i - count)("DataValue"))
                    For j As Integer = 1 To count
                        Editdt.Rows(i + 1 - j)("DataValue") = Editdt.Rows(i + 1)("DataValue") - difference / (count + 1) * j
                        If Not Editdt.Rows(i + 1 - j)("Other") = -1 And Not Editdt.Rows(i + 1 - j)("Other") = 1 Then
                            Editdt.Rows(i + 1 - j)("Other") = 2
                        End If
                    Next
                End If
            Else
                i += 1
            End If
        Loop

    End Sub

#End Region

#End Region




   
End Class
'End Namespace