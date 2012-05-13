Imports System.Windows.Forms
Imports HydroDesktop.Database
Imports HydroDesktop.Interfaces
Imports HydroDesktop.Interfaces.ObjectModel


Public Class fVariablesTableManagement
    
    Private _VariableID As Integer
    Private ReadOnly _unitsRepo As IUnitsRepository = RepositoryFactory.Instance.Get(Of IUnitsRepository)()
    Private ReadOnly _variablesRepo As IVariablesRepository = RepositoryFactory.Instance.Get(Of IVariablesRepository)()

    'If _VariableID = Nothing, it means the user selected create new in fDeriveNewDataSeries
    Public Sub New(ByVal variableID As Int32)
        _VariableID = variableID

        InitializeComponent()
        initialize()
    End Sub

    Private Sub CloseMe() Handles Me.Leave, btnCancel.Click
        DialogResult = DialogResult.Cancel
        Close()
    End Sub

    Public ReadOnly Property VariableID() As Integer
        Get
            Return _VariableID
        End Get
    End Property

    Private Sub initialize()
        Dim dt = _unitsRepo.AsDataTable()
        ddlUnitsName.DataSource = dt
        ddlUnitsName.DisplayMember = "UnitsName"
        ddlUnitsName.ValueMember = "UnitsID"
        lblVUnitsType.Text = "Units Type: " + dt.Rows(0)("UnitsType")
        lblVUnitsAbbreviation.Text = "Units Abbreviation: " + dt.Rows(0)("UnitsAbbreviation")

        Dim dt2 = _unitsRepo.AsDataTable()
        ddlTUnitsName.DataSource = dt2
        ddlTUnitsName.DisplayMember = "UnitsName"
        ddlTUnitsName.ValueMember = "UnitsID"
        lblTUnitsType.Text = "Units Type: " + dt.Rows(0)("UnitsType")
        lblTUnitsAbbreviation.Text = "Units Abbreviation: " + dt.Rows(0)("UnitsAbbreviation")

        ddlValueType.SelectedItem = ddlValueType.Items(0)
        ddlDataType.SelectedItem = ddlDataType.Items(1)

        If _VariableID = Nothing Then
            btnSubmit.Text = "Add"
            rbtnRegularYes.Checked = True
        Else
            Dim variable = _variablesRepo.GetByKey(_VariableID)

            txtVariableCode.Text = variable.Code
            txtVariableName.Text = variable.Name
            txtSpeciation.Text = variable.Speciation
            txtSampleMedium.Text = variable.SampleMedium
            txtNoDataValue.Text = variable.NoDataValue
            txtTValue.Text = variable.TimeSupport
            txtGeneralCategory.Text = variable.GeneralCategory

            ddlUnitsName.SelectedItem = ddlUnitsName.Items(variable.VariableUnit.Id- 1)
            ddlTUnitsName.SelectedItem = ddlTUnitsName.Items(variable.TimeUnit.Id - 1)

            Dim dataType = variable.DataType
            If Not ddlDataType.Items.Contains(dataType) Then
                ddlDataType.Items.Add(dataType)
            End If
            ddlDataType.Text = dataType
            ddlValueType.Text = variable.ValueType
            If variable.IsRegular Then
                rbtnRegularYes.Checked = True
            Else
                rbtnRegularNo.Checked = True
            End If

            btnSubmit.Text = "Edit"
        End If
    End Sub

    Private Sub InsertOrUpdateVariable()
        Dim variable = GetCurrentVariable()
        If _VariableID = Nothing Then
            _variablesRepo.AddVariable(variable)
            _VariableID = variable.Id
        Else
            _variablesRepo.Update(variable)
        End If
    End Sub

    Private Function GetCurrentVariable() As Variable
        Dim variable = New Variable()
        If Not (_VariableID = Nothing) Then
            variable.Id = _VariableID
        End If
        variable.Code = txtVariableCode.Text
        variable.Name = txtVariableName.Text
        variable.Speciation = txtSpeciation.Text
        variable.VariableUnit = New Unit()
        variable.VariableUnit.Id = ddlUnitsName.SelectedValue
        variable.SampleMedium = txtSampleMedium.Text
        variable.ValueType = ddlValueType.Text
        variable.IsRegular = Val(rbtnRegularYes.Checked)
        variable.TimeSupport = txtTValue.Text
        variable.TimeUnit = New Unit()
        variable.TimeUnit.Id = ddlTUnitsName.SelectedValue
        variable.DataType = ddlDataType.Text
        variable.GeneralCategory = txtGeneralCategory.Text
        variable.NoDataValue = txtNoDataValue.Text
        Return variable
    End Function

    Private Sub btnSubmit_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles btnSubmit.Click
        Dim Validated As Boolean = True

        'Validation
        If txtVariableCode.Text = Nothing Then
            Validated = False
        End If
        If txtVariableName.Text = Nothing Then
            Validated = False
        End If
        If txtSampleMedium.Text = Nothing Then
            Validated = False
        End If
        If txtSpeciation.Text = Nothing Then
            Validated = False
        End If
        If txtTValue.Text = Nothing Then
            Validated = False
        End If
        If txtNoDataValue.Text = Nothing Then
            Validated = False
        End If
        If txtGeneralCategory.Text = Nothing Then
            Validated = False
        End If

        If Not Validated Then
            MsgBox("Please fill in all fields.")
            Exit Sub
        End If

        If Not IsNumeric(txtNoDataValue.Text) Then
            MsgBox("Please enter numbers in No Data Value field")
            Exit Sub
        End If

        If Not IsNumeric(txtTValue.Text) Then
            MsgBox("Please enter numbers in Time Support Value field")
            Exit Sub
        End If

        InsertOrUpdateVariable()
        DialogResult = DialogResult.OK
        Close()
    End Sub

    Private Sub ddlUnitsName_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As EventArgs) Handles ddlUnitsName.SelectionChangeCommitted
        Dim ID As Integer = ddlUnitsName.SelectedValue.ToString
        Dim unit = _unitsRepo.GetByKey(ID)

        lblVUnitsType.Text = unit.UnitsType
        lblVUnitsType.Text = "Units Type: " + lblVUnitsType.Text
        lblVUnitsAbbreviation.Text = unit.Abbreviation
        lblVUnitsAbbreviation.Text = "Units Abbreviation: " + lblVUnitsAbbreviation.Text

    End Sub

    Private Sub ddlTUnitsName_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As EventArgs) Handles ddlTUnitsName.SelectionChangeCommitted
        Dim ID As Integer = ddlTUnitsName.SelectedValue.ToString
        Dim unit = _unitsRepo.GetByKey(ID)

        lblTUnitsType.Text = unit.UnitsType
        lblTUnitsType.Text = "Units Type: " + lblTUnitsType.Text
        lblTUnitsAbbreviation.Text = unit.Abbreviation
        lblTUnitsAbbreviation.Text = "Units Abbreviation: " + lblTUnitsAbbreviation.Text

    End Sub

    Private Sub ddlValueType_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As EventArgs) Handles ddlValueType.SelectionChangeCommitted
        If ddlValueType.SelectedIndex = ddlValueType.Items.Count - 1 Then
            ddlValueType.Text = ""
        End If
    End Sub

    Private Sub ddlDataType_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As EventArgs) Handles ddlDataType.SelectionChangeCommitted
        If ddlDataType.SelectedIndex = ddlDataType.Items.Count - 1 Then
            ddlDataType.Text = ""
        End If
    End Sub


End Class