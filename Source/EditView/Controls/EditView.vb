Imports System.Collections.Generic
Imports System.Drawing
Imports System.Data
Imports System.Linq
Imports System.Windows.Forms
Imports HydroDesktop.Database
Imports ZedGraph
Imports HydroDesktop.Interfaces

Namespace Controls

    Public Class EditView

#Region "privateDeclaration"

        Private ReadOnly CurveEditingColor As Color = Color.Black
        Private _seriesSelector As ISeriesSelector
        Private OriginalDt As DataTable
        Public Editdt As DataTable
        Public newseriesID As Integer = 0
        Public Editing As Boolean = False
        Private ReadOnly selectedSeriesIdList As New List(Of Int32)
        Private ReadOnly ccList0 As New List(Of Color)
        Private nodataseriescount As Integer = 0
        Private colorcount As Integer = 0
        Public ShowLegend As Boolean

        Dim _dataValuesRepo As IDataValuesRepository
        Dim _dataSeriesRepo As IDataSeriesRepository

        Private Const ErrMsgForNotEditing As String = "Please select a series to edit first."
        Private Const ErrMsgForNotPointSelected As String = "Please select a point for editing."

        Private _needToRefresh As Boolean
#End Region

#Region "Constructor"

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

        Public Sub New(ByVal seriesSelector As ISeriesSelector)
            'InitializeComponent()

            ' This call is required by the Windows Form Designer.
            InitializeComponent()

            'set the 'seriesMenu' private variable
            _seriesSelector = seriesSelector

            'assign the events
            AddHandler Disposed, AddressOf OnDisposing
            AddHandler _seriesSelector.SeriesCheck, AddressOf SeriesSelector_SeriesCheck
            AddHandler _seriesSelector.Refreshed, AddressOf SeriesSelector_Refreshed
            AddHandler VisibleChanged, AddressOf OnMeVisibleChanged

            initialize()
            SettingColor()
            pTimeSeriesPlot.Clear()
        End Sub

        Private Sub OnDisposing(ByVal sender As Object, ByVal e As EventArgs)
            ' Unsubscribe from events
            RemoveHandler Disposed, AddressOf OnDisposing
            RemoveHandler _seriesSelector.SeriesCheck, AddressOf SeriesSelector_SeriesCheck
            RemoveHandler _seriesSelector.Refreshed, AddressOf SeriesSelector_Refreshed
            _seriesSelector = Nothing
        End Sub


        Private Sub initialize()
            gboxDataFilter.Enabled = False
            ddlTimePeriod.SelectedItem = ddlTimePeriod.Items(0)
            lblstatus.Text = "Ready"
        End Sub

#End Region

#Region "Views"

#Region "Method"

        Private Sub PlotGraph(ByVal SeriesID As Integer)
            Dim options As PlotOptions = New PlotOptions(PlotOptions.TimeSeriesType.Line, ccList0(colorcount Mod 10), CurveEditingColor, False, True)
            Dim data As DataTable

            Dim series = _dataSeriesRepo.GetByKey(SeriesID)

            Dim variableName = series.Variable.Name
            Dim unitsName = series.Variable.VariableUnit.Name
            Dim siteName = If(_seriesSelector.SiteDisplayColumn = "SiteName", series.Site.Name, series.Site.Code)

            data = _dataValuesRepo.GetAllOrderByLocalDateTime(SeriesID)
            If data.Rows.Count = 1 Then
                options.TimeSeriesMethod = PlotOptions.TimeSeriesType.Point
            End If

            If data.Rows.Count <= 0 Then
                MsgBox("The Selected Series has no curve")
            Else
                pTimeSeriesPlot.Plot(data, siteName, variableName, unitsName, options, SeriesID)
            End If

            pTimeSeriesPlot.Refreshing()

            colorcount += 1
        End Sub

#End Region

#Region "Event"

        Private Sub SeriesSelector_Refreshed(ByVal sender As Object, ByVal e As EventArgs)
            RefreshDbTools()
        End Sub

        Private Sub RefreshDbTools()
            _dataValuesRepo = RepositoryFactory.Instance.Get(Of IDataValuesRepository)()
            _dataSeriesRepo = RepositoryFactory.Instance.Get(Of IDataSeriesRepository)()
        End Sub

        Private Sub DoSeriesRefresh()
            Dim idsToRemove = New List(Of Integer)
            Dim idsToAdd = New List(Of Integer)()

            For Each Id As Integer In selectedSeriesIdList
                If Not _seriesSelector.CheckedIDList.Contains(Id) Then
                    idsToRemove.Add(Id)
                End If
            Next

            For Each Id As Integer In _seriesSelector.CheckedIDList
                If Not selectedSeriesIdList.Contains(Id) Then
                    idsToAdd.Add(Id)
                End If
            Next

            If idsToRemove.Count > 0 And pTimeSeriesPlot.HasEditingCurve Then
                pTimeSeriesPlot.EditCurvePointList = pTimeSeriesPlot.CopyCurvePointList(pTimeSeriesPlot.EditingCurve)
                pTimeSeriesPlot.EditCurveLable = pTimeSeriesPlot.EditingCurve.Label.Text
                pTimeSeriesPlot.EditCurveTitle = pTimeSeriesPlot.EditingCurve.Link.Title
            End If

            For Each Id As Integer In idsToRemove
                removeSeries(Id)
            Next

            For Each Id As Integer In idsToAdd
                addSeries(Id)
            Next

            pTimeSeriesPlot.Refreshing()

        End Sub

        Private Sub DoSeriesCheck()
            If (Not Visible) Then
                _needToRefresh = True
                Return
            End If

            If Not _seriesSelector.CheckedIDList.Length > selectedSeriesIdList.Count Then

                If pTimeSeriesPlot.HasEditingCurve Then
                    pTimeSeriesPlot.EditCurvePointList = pTimeSeriesPlot.CopyCurvePointList(pTimeSeriesPlot.EditingCurve)
                    pTimeSeriesPlot.EditCurveLable = pTimeSeriesPlot.EditingCurve.Label.Text
                    pTimeSeriesPlot.EditCurveTitle = pTimeSeriesPlot.EditingCurve.Link.Title
                End If
                removeSeries(_seriesSelector.SelectedSeriesID)

            Else
                addSeries(_seriesSelector.SelectedSeriesID)
            End If

            pTimeSeriesPlot.Refreshing()
        End Sub

        Private Sub addSeries(seriesId As Integer)
            If Not selectedSeriesIdList.Contains(seriesId) Then
                selectedSeriesIdList.Add(seriesId)
            Else
                Return 'added by jiri to correct error when SeriesCheck event occurs multiple times
            End If

            If SeriesRowsCount(seriesId) = 0 Then
                nodataseriescount += 1
            ElseIf Not seriesId = newseriesID Then
                PlotGraph(seriesId)
            Else : seriesId = newseriesID
                Dim curve As LineItem = pTimeSeriesPlot.zgTimeSeries.GraphPane.AddCurve(pTimeSeriesPlot.EditCurveLable, pTimeSeriesPlot.EditCurvePointList, Color.Black, SymbolType.Circle)
                pTimeSeriesPlot.SettingCurveStyle(curve)
                curve.Link.Title = pTimeSeriesPlot.EditCurveTitle
                pTimeSeriesPlot.SettingTitle()
                pTimeSeriesPlot.AddYAxis(curve)
                pTimeSeriesPlot.zgTimeSeries.GraphPane.XAxis.IsVisible = True
                pTimeSeriesPlot.zgTimeSeries.GraphPane.XAxis.Title.Text = "Date and Time"
            End If
        End Sub

        Private Sub removeSeries(Id As Integer)
            Dim curveIndex = selectedSeriesIdList.IndexOf(Id)
            selectedSeriesIdList.Remove(Id)

            If SeriesRowsCount(Id) = 0 Then
                nodataseriescount -= 1
            End If

            If (selectedSeriesIdList.Count = 0) Then
                pTimeSeriesPlot.Clear()
            ElseIf (selectedSeriesIdList.Count = 1) Then
                Try
                    pTimeSeriesPlot.Remove(curveIndex - nodataseriescount)
                    curveIndex = pTimeSeriesPlot.CurveID(0)
                    pTimeSeriesPlot.Remove(0)

                    If SeriesRowsCount(Id) = 0 Then
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
                    If Not nodataseriescount = 0 Then
                        nodataseriescount -= 1
                    End If

                End Try
            Else
                Try
                    pTimeSeriesPlot.Remove(curveIndex - nodataseriescount)
                Catch
                    If Not nodataseriescount = 0 Then
                        nodataseriescount -= 1
                    End If
                End Try
            End If
        End Sub

        Private Sub OnMeVisibleChanged(ByVal sender As Object, ByVal e As EventArgs)
            If (Not Visible) Then Return

            If (_needToRefresh) Then
                _needToRefresh = False
                DoSeriesRefresh()
            End If
        End Sub

        Private Sub SeriesSelector_SeriesCheck(ByVal sender As Object, ByVal e As SeriesEventArgs)
            DoSeriesCheck()
        End Sub

        Public Sub btnSelectSeries_Click()
            If Not _seriesSelector.SelectedSeriesID = 0 Then
                initialize()

                newseriesID = _seriesSelector.SelectedSeriesID
                Editdt = _dataValuesRepo.GetTableForEditView(newseriesID)
                dgvDataValues.DataSource = Editdt

                OriginalDt = Editdt.Copy()

                'get the begin and end datetime of the series
                Dim series = _dataSeriesRepo.GetByKey(newseriesID)
                Dim beginDateTime As Date = series.BeginDateTime
                Dim endDateTime As Date = series.EndDateTime

                'setting the datetime constrint to larger range

                dtpBefore.MinDate = Today.AddYears(-150)
                dtpBefore.MaxDate = Today.AddDays(1)
                dtpAfter.MinDate = Today.AddYears(-150)
                dtpAfter.MaxDate = Today

                If beginDateTime <> Nothing Then
                    'setting the default datetime values
                    dtpAfter.Value = beginDateTime
                    'setting the datetime constrint by the begin and end datetime
                    dtpBefore.MinDate = beginDateTime
                    dtpAfter.MinDate = beginDateTime
                End If
                If endDateTime <> Nothing Then
                    'setting the default datetime values
                    dtpBefore.Value = endDateTime
                    'setting the datetime constrint by the begin and end datetime
                    dtpBefore.MaxDate = endDateTime
                    dtpAfter.MaxDate = endDateTime
                End If

                If _dataSeriesRepo.GetQualityControlLevelCode(newseriesID) = "Raw Data" Then
                    gboxDataFilter.Enabled = False
                Else
                    gboxDataFilter.Enabled = True
                    rbtnValueThreshold.Select()
                End If
                ResetGridViewStyle()

                Try
                    Dim curveIndex As Integer = selectedSeriesIdList.IndexOf(_seriesSelector.SelectedSeriesID)

                    If _seriesSelector.CheckedIDList.Contains(_seriesSelector.SelectedSeriesID) Then
                        pTimeSeriesPlot.EnterEditMode(curveIndex - nodataseriescount)
                        pTimeSeriesPlot.RemoveSelectedPoints()
                    Else
                        PlotGraph(_seriesSelector.SelectedSeriesID)
                        pTimeSeriesPlot.EnterEditMode(pTimeSeriesPlot.zgTimeSeries.GraphPane.CurveList.Count - 1)
                        pTimeSeriesPlot.Remove(pTimeSeriesPlot.zgTimeSeries.GraphPane.CurveList.Count - 1)
                        pTimeSeriesPlot.Refreshing()
                    End If

                Catch
                    MsgBox("The Selected Series has no curve")
                End Try

                gboxDataFilter.Enabled = SeriesRowsCount(newseriesID) >= 1
                Editing = True
            Else
                MsgBox("Please select a series for editing.")
            End If
        End Sub

        Public Sub ckbShowLegend_Click()
            ShowLegend = Not ShowLegend
            pTimeSeriesPlot.zgTimeSeries.GraphPane.Legend.IsVisible = ShowLegend
            pTimeSeriesPlot.Refreshing()
        End Sub

#End Region

#End Region

#Region "Editing"

        'Reset style of data grid view
        Private Sub ResetGridViewStyle() Handles dgvDataValues.Sorted
            For i As Integer = 0 To dgvDataValues.Columns.Count - 1
                dgvDataValues.Columns(i).ReadOnly = True
            Next
            dgvDataValues.Columns("Other").Visible = False

            For i As Integer = 0 To dgvDataValues.Rows.Count - 1
                If dgvDataValues.Rows(i).Cells("Other").Value = -1 Then
                    dgvDataValues.Rows(i).DefaultCellStyle.BackColor = Color.Red
                Else
                    dgvDataValues.Rows(i).DefaultCellStyle.BackColor = Nothing
                End If
            Next
        End Sub

        'Derive New Series
        Public Sub btnDeriveNewDataSeries_Click(ByVal sender As System.Object, ByVal e As EventArgs)
            'check if the user selected any series, then open the Derive New Series form
            If Not _seriesSelector.SelectedSeriesID = 0 Then
                Dim frmDeriveNewDataSeries As DeriveNewDataSeries
                frmDeriveNewDataSeries = New DeriveNewDataSeries(_seriesSelector.SelectedSeriesID, Me, _seriesSelector)
                frmDeriveNewDataSeries.ShowDialog()
            Else
                MsgBox("Please select a series to derive.")
            End If
        End Sub

        'Apply Changes to Database
        Public Sub btnApplyToDatabase_Click(ByVal sender As System.Object, ByVal e As EventArgs)
            If MsgBox("Are You Sure You Want to Apply the Changes to the Database", MsgBoxStyle.YesNo Or vbDefaultButton2 Or MsgBoxStyle.Question, "Question") = MsgBoxResult.Yes Then
                SaveGraphChangesToDatabase()
                gboxDataFilter.Enabled = SeriesRowsCount(newseriesID) >= 1
                MsgBox("Save finished!")
            End If
        End Sub

        'Restore Data
        Public Sub btnRestoreData_Click(ByVal sender As System.Object, ByVal e As EventArgs)
            If MsgBox("Are You Sure You Want to Restore the Data to the Original?", MsgBoxStyle.YesNo Or vbDefaultButton2 Or MsgBoxStyle.Question, "Question") = MsgBoxResult.Yes Then
                Editdt = OriginalDt
                RefreshDataGridView()
                pTimeSeriesPlot.ReplotEditingCurve(Me)
                gboxDataFilter.Enabled = SeriesRowsCount(newseriesID) >= 1
                MsgBox("Restore Complete!")
            End If

        End Sub

        'Clear Filter
        Private Sub btnClearFilter_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles btnClearFilter.Click
            'Make all text boxes blank
            ddlTimePeriod.SelectedItem = ddlTimePeriod.Items(0)
            txtDataGapValue.Text = ""
            txtEditDFVTChange.Text = ""
            txtValueLarger.Text = ""
            txtValueLess.Text = ""
            rbtnValueThreshold.Select()
            dgvDataValues.ClearSelection()
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
        Private Sub btnApplyFilter_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles btnApplyFilter.Click
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
                    Dim largest As Decimal = Convert.ToDecimal(_dataValuesRepo.GetMaxValue(newseriesID))
                    If pTimeSeriesPlot.HasEditingCurve() Then
                        pTimeSeriesPlot.ChangeZvalueWithValueThreshold(largest, Val(txtValueLess.Text))
                    Else
                        ValueThresholdFilter(largest, Val(txtValueLess.Text))
                    End If
                ElseIf txtValueLess.Text = Nothing And Not (txtValueLarger.Text = Nothing) Then
                    Dim smallest As Decimal = Convert.ToDecimal(_dataValuesRepo.GetMinValue(newseriesID))
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
                    Dim frmChangeYValue As ChangeYValue
                    frmChangeYValue = New ChangeYValue()
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
                If MsgBox("         The Selected Value Range will be Interpolated." + Environment.NewLine +
                          "            This creates new intermediary data points." + Environment.NewLine + Environment.NewLine +
                        "Are You Sure You Want to Interpolate the Selected Values?", MsgBoxStyle.YesNo Or vbDefaultButton2, "Question") = MsgBoxResult.Yes Then
                    If dgvDataValues.SelectedRows.Count >= 1 Then
                        Dim returned As Boolean = False
                        If pTimeSeriesPlot.HasEditingCurve Then
                            'pTimeSeriesPlot.ChangeValueByInterpolating(returned)
                            ChangeValueByInterpolating(returned)
                            pTimeSeriesPlot.ReplotEditingCurve(Me)
                        Else
                            ChangeValueByInterpolating(returned)
                        End If
                        RefreshDataGridView()
                    Else
                        MsgBox(ErrMsgForNotPointSelected)
                    End If
                End If
            Else
                MsgBox(ErrMsgForNotEditing)
            End If

        End Sub

        Public Sub btnFlag_Click()
            If Editing Then
                If dgvDataValues.SelectedRows.Count >= 1 Then
                    Dim QualifiersTableManagement As New QualifiersTableManagement()
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

        'Adding a point
        Public Sub btnAddNewPoint_Click()
            If Editing Then
                Dim frmAddNewPoint As AddNewPoint = New AddNewPoint()
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

        Public Function GetSelectedRows() As DataGridViewSelectedRowCollection
            Return dgvDataValues.SelectedRows
        End Function

        'Delete the selected point
        Public Sub btnDeletePoint_Click()
            If Editing Then
                Dim selectedRows = GetSelectedRows()
                If selectedRows.Count >= 1 Then
                    If MsgBox("Do you want to delete the point/points?", MsgBoxStyle.OkCancel, "Delete point") = MsgBoxResult.Ok Then
                        For Each row As DataGridViewRow In selectedRows
                            row.Cells("Other").Value = -1
                        Next
                    End If
                    pTimeSeriesPlot.ReplotEditingCurve(Me)
                    RefreshDataGridView()
                Else
                    MsgBox(ErrMsgForNotPointSelected)
                End If
            Else
                MsgBox(ErrMsgForNotEditing)
            End If
        End Sub

        'Associate Table selection with Zvalue(selected points) in the graph Method
        Private Sub ReflectZvalue()
            Dim IDlist As New List(Of Integer)
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

            'Unsubscribe from SelectionChanged event to avoid multiple raising of event
            RemoveHandler dgvDataValues.SelectionChanged, AddressOf dgvDataValues_SelectionChanged

            For i As Integer = 0 To dgvDataValues.Rows.Count - 1
                If IDlist.Contains(Val(dgvDataValues.Rows(i).Cells("ValueID").Value)) Then
                    dgvDataValues.Rows(i).Selected = True
                Else
                    dgvDataValues.Rows(i).Selected = False
                End If
            Next

            'Subscribe to SelectionChanged event and fire it
            AddHandler dgvDataValues.SelectionChanged, AddressOf dgvDataValues_SelectionChanged
            dgvDataValues_SelectionChanged(Me, EventArgs.Empty)
        End Sub

        'Saving changes Method
        Public Sub SaveGraphChangesToDatabase()

            Dim ValueIDList As New List(Of Integer)
            'Deleting added points after restore data
            For i As Integer = 0 To dgvDataValues.Rows.Count - 1
                ValueIDList.Add(dgvDataValues.Rows(i).Cells("ValueID").Value)
            Next

            Dim dv = _dataSeriesRepo.GetDataValuesIDs(newseriesID)
            For i As Integer = 0 To dv.Count - 1
                If Not ValueIDList.Contains(dv(i)) Then
                    _dataValuesRepo.DeleteById(dv(i))
                End If
            Next

            'Setting progress bar
            Dim frmloading As ProgressBar = pbProgressBar
            frmloading.Visible = True
            frmloading.Maximum = dgvDataValues.Rows.Count - 1
            frmloading.Minimum = 0
            frmloading.Value = 0

            lblstatus.Text = "Saving..."
            _dataValuesRepo.UpdateValuesForEditView(Editdt)

            'Update Data Series
            _dataSeriesRepo.UpdateDataSeriesFromDataValues(newseriesID)

            RefreshDataGridView()
            pTimeSeriesPlot.ReplotEditingCurve(Me)

            frmloading.Value = 0
            lblstatus.Text = "Ready"

        End Sub

        'Count the rows of a series
        Private Function SeriesRowsCount(ByVal seriesID As Integer) As Integer
            Dim series = _dataSeriesRepo.GetByKey(seriesID)
            Return If(series Is Nothing, 0, series.ValueCount)
        End Function

        'Reload the Data Grid View
        Public Sub RefreshDataGridView()
            Editdt.DefaultView.Sort = "[LocalDateTime] Asc"

            dgvDataValues.DataSource = Editdt.DefaultView
            ResetGridViewStyle()
        End Sub

#Region "Filters"
        'Value Threshold Filter
        Private Sub ValueThresholdFilter(ByVal LargerThanValue As Double, ByVal LessThanValue As Double)
            For i As Integer = 0 To dgvDataValues.Rows.Count - 1
                Dim dv = Convert.ToDouble(dgvDataValues.Rows(i).Cells("DataValue").Value)
                If LargerThanValue < LessThanValue Then
                    dgvDataValues.Rows(i).Selected = dv > LargerThanValue And dv < LessThanValue
                Else
                    dgvDataValues.Rows(i).Selected = dv > LargerThanValue Or dv < LessThanValue
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
        End Sub

        'Date Filter
        Private Sub DateFilter(ByVal DateBefore As DateTime, ByVal DateAfter As DateTime)
            For i As Integer = 0 To dgvDataValues.Rows.Count - 1
                If DateAfter > DateBefore Then
                    dgvDataValues.Rows(i).Selected = dgvDataValues.Rows(i).Cells("LocalDateTime").Value >= DateAfter.ToOADate Or dgvDataValues.Rows(i).Cells("LocalDateTime").Value <= DateBefore.ToOADate
                Else
                    dgvDataValues.Rows(i).Selected = dgvDataValues.Rows(i).Cells("LocalDateTime").Value >= DateAfter.ToOADate And dgvDataValues.Rows(i).Cells("LocalDateTime").Value <= DateBefore.ToOADate
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
        End Sub

#End Region

#Region "Change Value Function"

        Public Sub ChangeValueByAddOrMinus(ByVal Adding As Boolean, ByVal values As Double)
            For Each row As DataGridViewRow In GetSelectedRows()
                If Adding Then
                    row.Cells("DataValue").Value += values
                Else
                    row.Cells("DataValue").Value -= values
                End If

                If Not row.Cells("Other").Value = -1 And Not row.Cells("Other").Value = 1 Then
                    row.Cells("Other").Value = 2
                End If
            Next
        End Sub

        Public Sub ChangeValueByMultiply(ByVal Multiplier As Double)
            For Each row As DataGridViewRow In GetSelectedRows()
                row.Cells("DataValue").Value *= Multiplier
                If Not row.Cells("Other").Value = -1 And Not row.Cells("Other").Value = 1 Then
                    row.Cells("Other").Value = 2
                End If
            Next
        End Sub

        Public Sub ChangeValueBySettingValue(ByVal value As Double)
            For Each row As DataGridViewRow In GetSelectedRows()
                row.Cells("DataValue").Value = value
                If Not row.Cells("Other").Value = -1 And Not row.Cells("Other").Value = 1 Then
                    row.Cells("Other").Value = 2
                End If
            Next
        End Sub

        Private Sub ChangeValueByInterpolating(ByRef returned As Boolean)
            Dim i As Integer = 1
            Dim count As Integer = 1
            Dim difference As Double

            'Check if the first point and last point is selected
            If dgvDataValues.Rows(0).Selected Or dgvDataValues.Rows(dgvDataValues.Rows.Count - 1).Selected Then
                MsgBox("You cannot interpolate with the first point or last point selected.")
                returned = True
                Return
            End If

            i = 1
            Do Until (i >= dgvDataValues.Rows.Count - 2)
                If dgvDataValues.Rows(i).Selected Then
                    If Not dgvDataValues.Rows(i + 1).Selected = 1 Then
                        difference = Editdt.Rows(i - 1)("DataValue") + Editdt.Rows(i + 1)("DataValue")
                        Editdt.Rows(i)("DataValue") = difference / 2
                        If Not Editdt.Rows(i)("Other") = -1 And Not Editdt.Rows(i)("Other") = 1 Then
                            Editdt.Rows(i)("Other") = 2
                        End If
                        i += 1
                    Else
                        count = 1
                        Do Until Not (dgvDataValues.Rows(i + 1).Selected = 1)
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

        Private Sub dgvDataValues_SelectionChanged(sender As System.Object, e As EventArgs) Handles dgvDataValues.SelectionChanged
            Dim selectedRows = GetSelectedRows()
            Dim IDlist As New List(Of Int32)(selectedRows.Count)
            IDlist.AddRange(From row As DataGridViewRow In selectedRows Select CType(row.Cells("ValueID").Value, Integer))

            pTimeSeriesPlot.SelectingPoints(IDlist)
            pTimeSeriesPlot.Refreshing()

            ' Show info about selection
            lblstatus.Text = String.Format("{0} out of {1} values selected", selectedRows.Count, Editdt.Rows.Count)
        End Sub

    End Class
End Namespace