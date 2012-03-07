
Imports HydroDesktop.Common
Imports HydroDesktop.Database
Imports System.Windows.Forms
Imports HydroDesktop.Interfaces
Imports HydroDesktop.Interfaces.ObjectModel


Public Class fDeriveNewDataSeries
    Implements IProgressHandler

    Private newSeriesID As Integer

    Private Const DERIVED_METHOD_DESCRIPTION = "Derived using HydroDesktop Edit View"
    Private ReadOnly _SelectedSeriesID As Integer
    Private ReadOnly _cEditView As cEditView
    Private _derivedVariable As Variable
    Private _selectedSeriesVariable As Variable

    ReadOnly variablesRepository As IVariablesRepository = RepositoryFactory.Instance.Get(Of IVariablesRepository)()
    ReadOnly dataSeriesRepository As IDataSeriesRepository = RepositoryFactory.Instance.Get(Of IDataSeriesRepository)()
    ReadOnly dataThemesRepository As IDataThemesRepository = RepositoryFactory.Instance.Get(Of IDataThemesRepository)()
    ReadOnly qualityControlLevelsRepository As IQualityControlLevelsRepository = RepositoryFactory.Instance.Get(Of IQualityControlLevelsRepository)()
    Dim dataValuesRepository As IDataValuesRepository = RepositoryFactory.Instance.Get(Of IDataValuesRepository)()

    Public Sub New(ByVal seriesId As Int32, ByRef cEditView As cEditView)

        _SelectedSeriesID = seriesId
        _cEditView = cEditView

        InitializeComponent()
        initialize()

        SetDefault()
    End Sub

    Private Sub initialize()
        'fill all lists of this form
        FillQualityControlLevel()
        FillMethods()

        'Create derived variable
        Dim currentVariableID = dataSeriesRepository.GetVariableID(_SelectedSeriesID)
        _selectedSeriesVariable = variablesRepository.GetByKey(currentVariableID)
        _derivedVariable = DirectCast(_selectedSeriesVariable.Clone(), Variable)
        variablesRepository.AddVariable(_derivedVariable)
        _derivedVariable.ValueType = "Derived Value"
        FillVariable()

        rbtnCopy.Checked = True
    End Sub

    Public Sub FillQualityControlLevel()
        Dim dt As DataTable

        'Fill up Quality Control Level drop down list
        dt = qualityControlLevelsRepository.AsDataTable()
        dt.Rows.Add()
        dt.Rows(dt.Rows.Count - 1).Item(0) = qualityControlLevelsRepository.GetNextID()
        dt.Rows(dt.Rows.Count - 1).Item(1) = "New Qulity Control Level..."
        ddlQualityControlLevel.DataSource = dt
        ddlQualityControlLevel.DisplayMember = "QualityControlLevelCode"
        ddlQualityControlLevel.ValueMember = "QualityControlLevelID"
    End Sub

    Public Sub FillMethods()
        Dim repo = RepositoryFactory.Instance.Get(Of IMethodsRepository)()

        ' Check for Derived method 
        Dim derivedMethod = repo.GetMethodID(DERIVED_METHOD_DESCRIPTION)
        If Not derivedMethod.HasValue Then
            ' Insert Derived method
            repo.InsertMethod(DERIVED_METHOD_DESCRIPTION, "unknown")
        End If

        'Fill up Method drop down list
        Dim dt = repo.AsDataTable()
        dt.Rows.Add()
        dt.Rows(dt.Rows.Count - 1).Item(0) = repo.GetNextID().ToString
        dt.Rows(dt.Rows.Count - 1).Item(1) = "New Method..."
        ddlMethods.DataSource = dt
        ddlMethods.DisplayMember = "MethodDescription"
        ddlMethods.ValueMember = "MethodID"
    End Sub

    Private Sub FillVariable()
        'Fill up Variable drop down list
        Dim dt = variablesRepository.AsDataTable()
        dt.Rows.Add()
        dt.Rows(dt.Rows.Count - 1).Item(0) = variablesRepository.GetNextID()
        dt.Rows(dt.Rows.Count - 1).Item(1) = "New Variable..."
        ddlVariable.DataSource = dt
        ddlVariable.DisplayMember = "VariableCode"
        ddlVariable.ValueMember = "VariableID"
    End Sub

    Private Sub SetDefault()
        'setting text boxes to blank
        txtA.Text = "0"
        txtB.Text = "0"
        txtC.Text = "0"
        txtD.Text = "0"
        txtE.Text = "0"
        txtF.Text = "0"
        txtComment.Text = ""

        SetDefaultMethods()
        SetDefaultQualityControlLevel()
        SetDefaultVariable()
    End Sub

    Private Sub InsertNewSeries()
        newSeriesID = dataSeriesRepository.InsertNewSeries(_SelectedSeriesID, ddlVariable.SelectedValue, ddlMethods.SelectedValue, ddlQualityControlLevel.SelectedValue)
    End Sub

    Private Sub InsertSeriesProvenance()
        Dim entity = New SeriesProvenance()
        entity.ProvenanceDateTime = DateTime.Today
        entity.InputSeries = New Series()
        entity.InputSeries.Id = _SelectedSeriesID
        entity.OutputSeries = New Series()
        entity.OutputSeries.Id = newSeriesID
        entity.Method = New Method()
        entity.Method.Id = ddlMethods.SelectedValue
        entity.Comment = txtComment.Text

        RepositoryFactory.Instance.Get(Of ISeriesProvenanceRepository).AddNew(entity)
    End Sub

    Private Sub InsertNewDataThemes()
        dataThemesRepository.InsertNewTheme(_SelectedSeriesID, newSeriesID)
    End Sub

    Private Sub InsertNewDataValues()
        Dim A As Double = txtA.Text
        Dim B As Double = txtB.Text
        Dim C As Double = txtC.Text
        Dim D As Double = txtD.Text
        Dim E As Double = txtE.Text
        Dim F As Double = txtF.Text

        Dim dt = dataValuesRepository.GetAll(_SelectedSeriesID)

        Dim frmloading As ProgressBar = _cEditView.pbProgressBar
        frmloading.Visible = True
        frmloading.Maximum = dt.Rows.Count - 1
        frmloading.Minimum = 0
        frmloading.Value = 0
        _cEditView.lblstatus.Text = "Creating New Data Values"

        dataSeriesRepository.DeriveInsertDataValues(A, B, C, D, E, F, dt, newSeriesID, _SelectedSeriesID, rbtnAlgebraic.Checked, Me)

        _cEditView.lblstatus.Text = "Ready"
    End Sub

    Private Sub InsertAggregateDataValues()

        'Setting values to variables
        Dim series = dataSeriesRepository.GetSeriesByID(newSeriesID)

        Dim nodatavalue = series.Variable.NoDataValue
        Dim firstDate = series.BeginDateTime
        Dim lastdate = series.EndDateTime

        Dim dt = dataValuesRepository.GetAll(_SelectedSeriesID)

        'Setting current date (first date) to the first day of the month/quarter
        Dim currentdate As DateTime
        If rbtnDaily.Checked Then
            currentdate = New DateTime(firstDate.Year, firstDate.Month, firstDate.Day)
        ElseIf rbtnMonthly.Checked Then
            currentdate = New DateTime(firstDate.Year, firstDate.Month, 1)
        ElseIf rbtnQuarterly.Checked Then
            Select Case firstDate.Month
                Case 1 To 3
                    currentdate = New DateTime(firstDate.Year, 1, 1)
                Case 4 To 6
                    currentdate = New DateTime(firstDate.Year, 4, 1)
                Case 7 To 9
                    currentdate = New DateTime(firstDate.Year, 7, 1)
                Case 10 To 12
                    currentdate = New DateTime(firstDate.Year, 10, 1)
            End Select
        End If


        'Setting progress bar
        Dim frmloading As ProgressBar = _cEditView.pbProgressBar
        frmloading.Visible = True
        If rbtnDaily.Checked Then
            frmloading.Maximum = (lastdate - firstDate).TotalDays
        ElseIf rbtnMonthly.Checked Then
            frmloading.Maximum = Math.Round((lastdate - firstDate).TotalDays / 30)
        ElseIf rbtnQuarterly.Checked Then
            frmloading.Maximum = Math.Round((lastdate - firstDate).TotalDays / 90)
        End If
        frmloading.Minimum = 0
        frmloading.Value = 0

        _cEditView.lblstatus.Text = "Creating New Data Values"

        Dim dMode As DeriveAggregateMode
        If rbtnDaily.Checked Then
            dMode = DeriveAggregateMode.Daily
        ElseIf rbtnMonthly.Checked Then
            dMode = DeriveAggregateMode.Monthly
        ElseIf rbtnQuarterly.Checked Then
            dMode = DeriveAggregateMode.Quarterly
        End If

        Dim computeMode As DeriveComputeMode
        If rbtnMaximum.Checked Then
            computeMode = DeriveComputeMode.Maximum
        ElseIf rbtnMinimum.Checked Then
            computeMode = DeriveComputeMode.Minimum
        ElseIf rbtnAverage.Checked Then
            computeMode = DeriveComputeMode.Average
        ElseIf rbtnSum.Checked Then
            computeMode = DeriveComputeMode.Sum
        End If

        dataSeriesRepository.DeriveInsertAggregateDataValues(dt, newSeriesID, currentdate, lastdate, dMode, computeMode, nodatavalue, Me)
        _cEditView.lblstatus.Text = "Ready"
    End Sub

#Region "Events"

#Region "QualityControlLevel data accesses"

    Private Sub btnQualityControlLevel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnQualityControlLevel.Click
        Dim QualityControlLevelTableManagement As fQualityControlLevelTableManagement = New fQualityControlLevelTableManagement()
        QualityControlLevelTableManagement.Show()
        QualityControlLevelTableManagement._fDeriveNewDataSeries = Me
        QualityControlLevelTableManagement._QualityControlLevelID = ddlQualityControlLevel.SelectedValue
        QualityControlLevelTableManagement.initialize()
    End Sub

    Private Sub ddlQualityControlLevel_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ddlQualityControlLevel.SelectedIndexChanged
        If ddlQualityControlLevel.SelectedIndex = ddlQualityControlLevel.Items.Count - 1 Then
            Dim QualityControlLevelTableManagement As fQualityControlLevelTableManagement = New fQualityControlLevelTableManagement()
            QualityControlLevelTableManagement.initialize()
            QualityControlLevelTableManagement.Show()
            QualityControlLevelTableManagement._fDeriveNewDataSeries = Me
        End If
    End Sub

    Public Sub SetDefaultQualityControlLevel()
        Dim currentQualityControlLevelID As Integer
        Dim count As Integer = 0

        currentQualityControlLevelID = dataSeriesRepository.GetQualityControlLevelID(_SelectedSeriesID.ToString)

        While Not (ddlQualityControlLevel.SelectedValue = currentQualityControlLevelID)
            ddlQualityControlLevel.SelectedItem = ddlQualityControlLevel.Items.Item(count)
            count += 1
        End While
    End Sub

#End Region

#Region "Methods data accesses"

    Private Sub btnMethods_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnMethods.Click
        Dim MethodTableManagement As fMethodTableManagement = New fMethodTableManagement()
        MethodTableManagement.Show()
        MethodTableManagement._fDeriveNewDataSeries = Me
        MethodTableManagement._MethodID = ddlMethods.SelectedValue
        MethodTableManagement.initialize()
    End Sub

    Private Sub ddlMethods_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ddlMethods.SelectedIndexChanged
        If ddlMethods.SelectedIndex = ddlMethods.Items.Count - 1 Then
            Dim MethodTableManagement As fMethodTableManagement = New fMethodTableManagement()
            MethodTableManagement.initialize()
            MethodTableManagement.Show()
            MethodTableManagement._fDeriveNewDataSeries = Me
        End If
    End Sub

    Public Sub SetDefaultMethods()
        Dim repo = RepositoryFactory.Instance.Get(Of IMethodsRepository)()
        Dim derivedMethod = repo.GetMethodID(DERIVED_METHOD_DESCRIPTION)
        ddlMethods.SelectedValue = derivedMethod
    End Sub

#End Region

#Region "Variables data accesses"

    Private Sub ddlVariable_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ddlVariable.SelectedIndexChanged
        If ddlVariable.SelectedIndex = ddlVariable.Items.Count - 1 Then
            ShowVariablesTableManagment(Nothing)
        End If
    End Sub

    Private Sub btnVariable_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnVariable.Click
        ShowVariablesTableManagment(ddlVariable.SelectedValue)
    End Sub

    Private Sub ShowVariablesTableManagment(ByVal variableID As Integer)
        Dim variablesTableManagement As fVariablesTableManagement = New fVariablesTableManagement(variableID)
        If variablesTableManagement.ShowDialog() = DialogResult.OK Then
            FillVariable()
            Dim count As Integer = 0
            While Not (ddlVariable.SelectedValue = variablesTableManagement.VariableID)
                ddlVariable.SelectedItem = ddlVariable.Items.Item(count)
                count += 1
            End While
        End If
    End Sub

    Public Sub SetDefaultVariable()
        Dim currentVariableID = _derivedVariable.Id

        Dim count As Integer = 0
        While Not (ddlVariable.SelectedValue = currentVariableID)
            ddlVariable.SelectedItem = ddlVariable.Items.Item(count)
            count += 1
        End While
    End Sub

#End Region

    Private Sub AlgebraicTextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtA.TextChanged, txtB.TextChanged, txtC.TextChanged, txtD.TextChanged, txtE.TextChanged, txtF.TextChanged
        If txtA.Text = Nothing Then
            txtA.Text = "0"
            txtA.Select(0, 1)
        End If

        If txtB.Text = Nothing Then
            txtB.Text = "0"
            txtB.Select(0, 1)
        End If

        If txtC.Text = Nothing Then
            txtC.Text = "0"
            txtC.Select(0, 1)
        End If

        If txtD.Text = Nothing Then
            txtD.Text = "0"
            txtD.Select(0, 1)
        End If

        If txtE.Text = Nothing Then
            txtE.Text = "0"
            txtE.Select(0, 1)
        End If

        If txtF.Text = Nothing Then
            txtF.Text = "0"
            txtF.Select(0, 1)
        End If

    End Sub

    Private Sub btnBackToOriginal_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnBackToOriginal.Click
        SetDefault()
    End Sub

    Private Sub rbtnCheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles rbtnCopy.CheckedChanged, rbtnAlgebraic.CheckedChanged, rbtnAggregate.CheckedChanged
        If rbtnAlgebraic.Checked Then
            gboxAggregate.Enabled = False
            gboxAlgebraic.Enabled = True
        ElseIf rbtnAggregate.Checked Then
            rbtnDaily.Checked = True
            rbtnMaximum.Checked = True
            gboxAggregate.Enabled = True
            gboxAlgebraic.Enabled = False
        Else
            gboxAggregate.Enabled = False
            gboxAlgebraic.Enabled = False
        End If

        UpdateDerivedVarible()
    End Sub

    Private Sub UpdateDerivedVarible()
        If Not rbtnAggregate.Checked Then
            _derivedVariable.DataType = _selectedSeriesVariable.DataType
            _derivedVariable.TimeSupport = _selectedSeriesVariable.TimeSupport
        Else
            'Update TimeSupport
            If rbtnDaily.Checked Then
                _derivedVariable.TimeSupport = 1.0
            ElseIf rbtnMonthly.Checked Then
                _derivedVariable.TimeSupport = 30.0
            ElseIf rbtnQuarterly.Checked Then
                _derivedVariable.TimeSupport = 120.0
            End If

            'Update DataType
            If rbtnMaximum.Checked Then
                _derivedVariable.DataType = "Maximum"
            ElseIf rbtnMinimum.Checked Then
                _derivedVariable.DataType = "Minimum"
            ElseIf _rbtnAverage.Checked Then
                _derivedVariable.DataType = "Average"
            ElseIf _rbtnSum.Checked Then
                _derivedVariable.DataType = "Sum"
            End If
        End If

        'Save changes
        Dim repo = RepositoryFactory.Instance.Get(Of IVariablesRepository)()
        repo.Update(_derivedVariable)
    End Sub

    Private Sub rbtnDaily_CheckedChanged(sender As System.Object, e As System.EventArgs) Handles rbtnSum.CheckedChanged, rbtnQuarterly.CheckedChanged, rbtnMonthly.CheckedChanged, rbtnMinimum.CheckedChanged, rbtnMaximum.CheckedChanged, rbtnDaily.CheckedChanged, rbtnAverage.CheckedChanged
        UpdateDerivedVarible()
    End Sub

    Private Sub btnCancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnCancel.Click
        Me.Close()
    End Sub

    Private Sub btnNewSeries_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnNewSeries.Click
        gboxDeriveOption.Enabled = False
        gboxgeneral.Enabled = False
        btnNewSeries.Enabled = False

        InsertNewSeries()
        InsertSeriesProvenance()
        InsertNewDataThemes()

        If rbtnAggregate.Checked Then
            InsertAggregateDataValues()
        Else
            InsertNewDataValues()
        End If

        _cEditView._seriesSelector.RefreshSelection()

        MsgBox("Derive Complete", MsgBoxStyle.OkOnly Or MsgBoxStyle.Information, "Edit View")
        btnCancel.Text = "Close"
        _cEditView.pbProgressBar.Value = 0
    End Sub

#End Region

    Public Sub ReportProgress(ByVal percentage As Integer, ByVal state As Object) Implements IProgressHandler.ReportProgress
        _cEditView.pbProgressBar.Value = percentage
    End Sub

    Public Sub CheckForCancel() Implements IProgressHandler.CheckForCancel
        Throw New NotImplementedException()
    End Sub

    Public Sub ReportMessage(ByVal message As String) Implements IProgressHandler.ReportMessage
        Throw New NotImplementedException()
    End Sub
End Class