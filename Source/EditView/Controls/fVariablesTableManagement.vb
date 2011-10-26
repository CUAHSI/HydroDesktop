'If _VariableID = Nothing, it means the user selected create new in fDeriveNewDataSeries

Imports HydroDesktop.Database
Imports System.Globalization
Imports System.Threading
Imports System.Text
Imports HydroDesktop.Interfaces


Public Class fVariablesTableManagement

    Private connString = HydroDesktop.Configuration.Settings.Instance.DataRepositoryConnectionString
    Private dbTools As New DbOperations(connString, DatabaseTypes.SQLite)

    Public Sub New()
        InitializeComponent()
    End Sub

    Private Sub CloseMe() Handles Me.Leave, btnCancel.Click
        Me.Close()
        If _fDeriveNewDataSeries.ddlVariable.SelectedIndex = _fDeriveNewDataSeries.ddlVariable.Items.Count - 1 Then
            _fDeriveNewDataSeries.SetDefaultVariable()
        End If
    End Sub


    Public Sub initialize()
        Dim dt As DataTable
        Dim dt2 As DataTable

        dt = dbTools.LoadTable("Units", "SELECT * FROM Units")
        ddlUnitsName.DataSource = dt
        ddlUnitsName.DisplayMember = "UnitsName"
        ddlUnitsName.ValueMember = "UnitsID"
        lblVUnitsType.Text = "Units Type: " + dt.Rows(0)("UnitsType")
        lblVUnitsAbbreviation.Text = "Units Abbreviation: " + dt.Rows(0)("UnitsAbbreviation")

        dt2 = dbTools.LoadTable("Units", "SELECT * FROM Units")
        ddlTUnitsName.DataSource = dt2
        ddlTUnitsName.DisplayMember = "UnitsName"
        ddlTUnitsName.ValueMember = "UnitsID"
        lblTUnitsType.Text = "Units Type: " + dt.Rows(0)("UnitsType")
        lblTUnitsAbbreviation.Text = "Units Abbreviation: " + dt.Rows(0)("UnitsAbbreviation")

        ddlValueType.SelectedItem = ddlValueType.Items(0)
        ddlDataType.SelectedItem = ddlDataType.Items(1)
        rbtnRegularYes.Checked = True

        If _VariableID = Nothing Then
            txtID.Text = dbTools.GetNextID("Methods", "MethodID").ToString
            btnSubmit.Text = "Add"
        Else
            txtID.Text = _VariableID.ToString
            txtVariableCode.Text = dbTools.ExecuteSingleOutput("SELECT VariableCode FROM Variables WHERE VariableID = " + _VariableID.ToString).ToString
            txtVariableName.Text = dbTools.ExecuteSingleOutput("SELECT VariableName FROM Variables WHERE VariableID = " + _VariableID.ToString).ToString
            txtSpeciation.Text = dbTools.ExecuteSingleOutput("SELECT Speciation FROM Variables WHERE VariableID = " + _VariableID.ToString).ToString
            txtSampleMedium.Text = dbTools.ExecuteSingleOutput("SELECT SampleMedium FROM Variables WHERE VariableID = " + _VariableID.ToString).ToString
            txtNoDataValue.Text = dbTools.ExecuteSingleOutput("SELECT NoDataValue FROM Variables WHERE VariableID = " + _VariableID.ToString).ToString
            txtTValue.Text = dbTools.ExecuteSingleOutput("SELECT TimeSupport FROM Variables WHERE VariableID = " + _VariableID.ToString).ToString
            txtGeneralCategory.Text = dbTools.ExecuteSingleOutput("SELECT GeneralCategory FROM Variables WHERE VariableID = " + _VariableID.ToString).ToString
            ddlUnitsName.SelectedItem = ddlUnitsName.Items(dbTools.ExecuteSingleOutput("SELECT VariableUnitsID FROM Variables WHERE VariableID = " + _VariableID.ToString) - 1)
            ddlTUnitsName.SelectedItem = ddlTUnitsName.Items(dbTools.ExecuteSingleOutput("SELECT TimeUnitsID FROM Variables WHERE VariableID = " + _VariableID.ToString) - 1)
            ddlDataType.Text = dbTools.ExecuteSingleOutput("SELECT DataType FROM Variables WHERE VariableID = " + _VariableID.ToString).ToString
            ddlValueType.Text = dbTools.ExecuteSingleOutput("SELECT ValueType FROM Variables WHERE VariableID = " + _VariableID.ToString).ToString
            btnSubmit.Text = "Edit"
        End If



    End Sub

    Private Sub InsertNewMethod()
        Dim SQLstring As String
        Try
            SQLstring = "INSERT INTO Variables(VariableID, VariableCode, VariableName, Speciation, VariableUnitsID, "
            SQLstring += "SampleMedium, ValueType, IsRegular, IsCategorical, TimeSupport, TimeUnitsID, DataType, "
            SQLstring += "GeneralCategory, NoDataValue) VALUES ("
            SQLstring += txtID.Text.ToString + ",'" + txtVariableCode.Text.ToString + "',"
            SQLstring += "'" + txtVariableName.Text.ToString + "','" + txtSpeciation.Text.ToString + "',"
            SQLstring += ddlUnitsName.SelectedValue.ToString + ",'" + txtSampleMedium.Text.ToString + "',"
            SQLstring += "'" + ddlValueType.Text.ToString + "'," + Val(rbtnRegularYes.Checked).ToString + ","
            SQLstring += "0," + txtTValue.Text.ToString + "," + ddlTUnitsName.SelectedValue.ToString + ","
            SQLstring += "'" + ddlDataType.Text.ToString + "','" + txtGeneralCategory.Text.ToString + "',"
            SQLstring += txtNoDataValue.Text.ToString + ")"

            dbTools.ExecuteNonQuery(SQLstring)

        Catch ex As Exception
            Throw New Exception("Error Occured when Inserting new methods." & vbCrLf & ex.Message)
        End Try
    End Sub

    Private Sub UpdateMethod()
        Dim SQLstring As String
        Try
            SQLstring = "UPDATE Variables SET VariableCode='" + txtVariableCode.Text.ToString + "',"
            SQLstring += "VariableName='" + txtVariableName.Text.ToString + "',"
            SQLstring += "Speciation='" + txtSpeciation.Text.ToString + "',"
            SQLstring += "VariableUnitsID=" + ddlUnitsName.SelectedValue.ToString + ","
            SQLstring += "SampleMedium='" + txtSampleMedium.Text.ToString + "',"
            SQLstring += "ValueType='" + ddlValueType.Text.ToString + "',"
            SQLstring += "IsRegular=" + Val(rbtnRegularYes.Checked).ToString + ",IsCategorical=0,"
            SQLstring += "TimeSupport=" + txtTValue.Text.ToString + ","
            SQLstring += "TimeUnitsID=" + ddlTUnitsName.SelectedValue.ToString + ","
            SQLstring += "DataType='" + ddlDataType.Text.ToString + "',"
            SQLstring += "GeneralCategory='" + txtGeneralCategory.Text.ToString + "',"
            SQLstring += "NoDataValue=" + txtNoDataValue.Text.ToString
            SQLstring += " WHERE VariableID = " + txtID.Text.ToString

            dbTools.ExecuteNonQuery(SQLstring)

        Catch ex As Exception
            Throw New Exception("Error Occured when Updating methods." & vbCrLf & ex.Message)
        End Try
    End Sub

    Private Sub btnSubmit_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSubmit.Click
        Dim Validated As Boolean = True
        Dim count As Integer = 0

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
            Validated = False
        End If

        If Not IsNumeric(txtTValue.Text) Then
            MsgBox("Please enter numbers in Time Support Value field")
            Validated = False
        End If



        If _VariableID = Nothing Then
            InsertNewMethod()
        Else
            UpdateMethod()
        End If

        _fDeriveNewDataSeries.FillVariable()

        While Not (_fDeriveNewDataSeries.ddlVariable.SelectedValue = Val(txtID.Text))
            _fDeriveNewDataSeries.ddlVariable.SelectedItem = _fDeriveNewDataSeries.ddlVariable.Items.Item(count)
            count += 1
        End While

        Me.Close()

    End Sub

    Private Sub ddlUnitsName_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ddlUnitsName.SelectionChangeCommitted

        Dim ID As Integer = ddlUnitsName.SelectedValue.ToString
        lblVUnitsType.Text = dbTools.ExecuteSingleOutput("SELECT UnitsType FROM Units WHERE UnitsID = " + ID.ToString).ToString
        lblVUnitsType.Text = "Units Type: " + lblVUnitsType.Text
        lblVUnitsAbbreviation.Text = dbTools.ExecuteSingleOutput("SELECT UnitsAbbreviation FROM Units WHERE UnitsID = " + ID.ToString).ToString
        lblVUnitsAbbreviation.Text = "Units Abbreviation: " + lblVUnitsAbbreviation.Text

    End Sub

    Private Sub ddlTUnitsName_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ddlTUnitsName.SelectionChangeCommitted

        Dim ID As Integer = ddlTUnitsName.SelectedValue.ToString
        lblTUnitsType.Text = dbTools.ExecuteSingleOutput("SELECT UnitsType FROM Units WHERE UnitsID = " + ID.ToString).ToString
        lblTUnitsType.Text = "Units Type: " + lblTUnitsType.Text
        lblTUnitsAbbreviation.Text = dbTools.ExecuteSingleOutput("SELECT UnitsAbbreviation FROM Units WHERE UnitsID = " + ID.ToString).ToString
        lblTUnitsAbbreviation.Text = "Units Abbreviation: " + lblTUnitsAbbreviation.Text

    End Sub

    Private Sub ddlValueType_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ddlValueType.SelectionChangeCommitted
        If ddlValueType.SelectedIndex = ddlValueType.Items.Count - 1 Then
            ddlValueType.Text = ""
        End If
    End Sub

    Private Sub ddlDataType_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ddlDataType.SelectionChangeCommitted
        If ddlDataType.SelectedIndex = ddlDataType.Items.Count - 1 Then
            ddlDataType.Text = ""
        End If
    End Sub


End Class