Imports HydroDesktop.Database
Imports System.Globalization
Imports System.Threading
Imports System.Text
Imports System.Windows.Forms
Imports HydroDesktop.Interfaces


Public Class fDeriveNewDataSeries

    Private connString = HydroDesktop.Configuration.Settings.Instance.DataRepositoryConnectionString
    Private dbTools As New DbOperations(connString, DatabaseTypes.SQLite)
    Private newSeriesID As Integer
    Private todaystring As String = DateTime.Today.ToString("yyyy-MM-dd HH:mm:ss")
    Private Const DERIVED_METHOD_DESCRIPTION = "Derived using HydroDesktop Edit View"
    Private ReadOnly _SelectedSeriesID As Integer
    Private ReadOnly _cEditView As cEditView
    'Private _defaultDerivedVariableID As Integer

    Public Sub New(ByVal seriesId As Int32, ByRef cEditView As cEditView)

        _SelectedSeriesID = seriesId
        _cEditView = cEditView

        InitializeComponent()
        initialize()

        SetDefault()
    End Sub

    Private Sub initialize()
        'fill all lists of this form
        rbtnCopy.Checked = True
        FillQualityControlLevel()
        FillMethods()
        FillVariable()
    End Sub

    Public Sub FillQualityControlLevel()
        Dim dt As DataTable

        'Fill up Quality Control Level drop down list
        dt = dbTools.LoadTable("QualityControlLevels", "SELECT * FROM QualityControlLevels")
        dt.Rows.Add()
        dt.Rows(dt.Rows.Count - 1).Item(0) = dbTools.GetNextID("QualityControlLevels", "QualityControlLevelID").ToString
        dt.Rows(dt.Rows.Count - 1).Item(1) = "New Qulity Control Level..."
        ddlQualityControlLevel.DataSource = dt
        ddlQualityControlLevel.DisplayMember = "QualityControlLevelCode"
        ddlQualityControlLevel.ValueMember = "QualityControlLevelID"
    End Sub

    Public Sub FillMethods()
        Dim repo = RepositoryFactory.Instance.Get(Of IMethodsRepository)(dbTools)

        ' Check for Derived method 
        Dim derivedMethod = repo.GetMethodID(DERIVED_METHOD_DESCRIPTION)
        If Not derivedMethod.HasValue Then
            ' Insert Derived method
            repo.InsertMethod(DERIVED_METHOD_DESCRIPTION, "unknown")
        End If

        'Fill up Method drop down list
        Dim dt = repo.GetAllMethods()
        dt.Rows.Add()
        dt.Rows(dt.Rows.Count - 1).Item(0) = dbTools.GetNextID("Methods", "MethodID").ToString
        dt.Rows(dt.Rows.Count - 1).Item(1) = "New Method..."
        ddlMethods.DataSource = dt
        ddlMethods.DisplayMember = "MethodDescription"
        ddlMethods.ValueMember = "MethodID"
    End Sub

    Public Sub FillVariable()
        Dim variablesRepository = RepositoryFactory.Instance.Get(Of IVariablesRepository)(dbTools)
        Dim dataSeriesRepository = RepositoryFactory.Instance.Get(Of IDataSeriesRepository)(dbTools)

        'todo: Create default derived variable
        'Dim currentVariableID = dataSeriesRepository.GetVariableID(_SelectedSeriesID)
        'Dim var = variablesRepository.GetByID(currentVariableID)
        'Dim defaultDerivedVariableID = variablesRepository.CreateCopy(currentVariableID)

        'Fill up Variable drop down list
        Dim dt = variablesRepository.GetAll()
        dt.Rows.Add()
        dt.Rows(dt.Rows.Count - 1).Item(0) = dbTools.GetNextID("Variables", "VariableID").ToString
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

    Private Sub AlgebraicValidation(ByVal Validated As Boolean)

        If Not IsNumeric(txtA.Text) Or Not IsNumeric(txtB.Text) Or Not IsNumeric(txtC.Text) Or _
        Not IsNumeric(txtD.Text) Or Not IsNumeric(txtE.Text) Or Not IsNumeric(txtF.Text) Then
            Validated = False
        End If


    End Sub

    Private Sub InsertNewSeries()
        Dim SQLstring As StringBuilder = New StringBuilder()
        Dim tempstring As String
        Dim dt As DataTable


        Try
            newSeriesID = dbTools.GetNextID("DataSeries", "SeriesID")
            dt = dbTools.LoadTable("DataSeries", "SELECT * FROM DataSeries WHERE SeriesID = " + _SelectedSeriesID.ToString)

            'Making the INSERT SQL string for the new data series
            SQLstring.Append("INSERT INTO DataSeries(SeriesID, SiteID, VariableID, IsCategorical, MethodID, SourceID, ")
            SQLstring.Append("QualityControlLevelID, BeginDateTime, EndDateTime, BeginDateTimeUTC, EndDateTimeUTC, ")
            SQLstring.Append("ValueCount, CreationDateTime, Subscribed, UpdateDateTime, LastcheckedDateTime) Values (")
            'SeriesID value
            SQLstring.Append(newSeriesID.ToString + ", ")
            'SiteID value
            SQLstring.Append(dt.Rows(0).Item(1).ToString + ", ")
            'VariableID values
            SQLstring.Append(ddlVariable.SelectedValue.ToString + ", ")
            'IsCategorical value
            If dt.Rows(0).Item(3).ToString = "True" Then
                SQLstring.Append("1, ")
            Else
                SQLstring.Append("0, ")
            End If
            'MethodID value
            tempstring = ddlMethods.SelectedValue.ToString()
            SQLstring.Append(tempstring + ", ")
            'SourceID value
            SQLstring.Append(dt.Rows(0).Item(5).ToString + ", ")
            'QualityControlLevelID value
            tempstring = ddlQualityControlLevel.SelectedValue.ToString()
            SQLstring.Append(tempstring + ", ")
            'BeginDateTime, EndDateTime, BeginDateTimeUTC and EndDateTimeUTC values
            For i As Integer = 7 To 10
                tempstring = dt.Rows(0).Item(i).ToString
                tempstring = DateTime.ParseExact(dt.Rows(0).Item(i).ToString, "yyyy/MM/dd H:mm:ss", Nothing).ToString("yyyy-MM-dd HH:mm:ss")
                SQLstring.Append("'" + tempstring + "', ")
            Next
            'ValueCount, CreationDateTime, Subscribed, UpdateDateTime and LastcheckedDateTime values
            SQLstring.Append(dt.Rows(0).Item(11).ToString + ", '" + todaystring + "', 0, '" + todaystring + "','" + todaystring + "')")
            tempstring = SQLstring.ToString

            'Execute the SQL string

            dbTools.ExecuteNonQuery(tempstring)
            dt.Dispose()

        Catch ex As Exception
            Throw New Exception("Error Occured in InsertNewSeries." & vbCrLf & ex.Message)
        End Try

    End Sub

    Private Sub InsertSeriesProvenance()
        Dim SQLstring As StringBuilder = New StringBuilder()

        Try
            SQLstring.Append("INSERT INTO SeriesProvenance(ProvenanceID, ProvenanceDateTime, InputSeriesID, OutputSeriesID, MethodID, Comment) VALUES (")
            'ProvenanceID value
            SQLstring.Append(dbTools.GetNextID("SeriesProvenance", "ProvenanceID").ToString + ",")
            'ProvenanceDateTime value
            SQLstring.Append("'" + todaystring.ToString + "',")
            'InputSeriesID value
            SQLstring.Append(_SelectedSeriesID.ToString + ",")
            'OutputSeriesID value
            SQLstring.Append(newSeriesID.ToString + ",")
            'MethodID value
            SQLstring.Append(ddlMethods.SelectedValue.ToString + ",")
            'Comment value
            If txtComment.Text = Nothing Then
                SQLstring.Append("NULL)")
            Else
                SQLstring.Append("'" + txtComment.Text.ToString + "')")
            End If


            dbTools.ExecuteNonQuery(SQLstring.ToString)

        Catch ex As Exception
            Throw New Exception("Error Occured in InsertSeriesProvenance." & vbCrLf & ex.Message)
        End Try
    End Sub

    Private Sub InsertNewDataThemes()
        Try

            Dim SQLstring As String
            SQLstring = "SELECT ThemeID FROM DataThemes WHERE SeriesID = " + _SelectedSeriesID.ToString
            Dim ThemeID As Integer = dbTools.ExecuteSingleOutput(SQLstring)

            SQLstring = "INSERT INTO DataThemes(ThemeID, SeriesID) VALUES ("
            SQLstring += ThemeID.ToString + "," + newSeriesID.ToString + ")"

            dbTools.ExecuteNonQuery(SQLstring)

        Catch ex As Exception
            Throw New Exception("Error Occured in InsertNewDataThemes." & vbCrLf & ex.Message)
        End Try

    End Sub

    Private Sub InsertNewDataValues()
        Dim SQLstring As StringBuilder = New StringBuilder()
        Dim SQLstring2 As String
        Dim newValueID As Integer
        Dim dt As DataTable
        Dim currentvalue As Double
        Dim nodatavalue As Double
        Dim newvalue As Double
        Dim A As Double = txtA.Text
        Dim B As Double = txtB.Text
        Dim C As Double = txtC.Text
        Dim D As Double = txtD.Text
        Dim E As Double = txtE.Text
        Dim F As Double = txtF.Text

        Dim tempstring As String

        Try
            nodatavalue = dbTools.ExecuteSingleOutput("SELECT NoDataValue FROM DataSeries LEFT JOIN Variables ON DataSeries.VariableID = Variables.VariableID WHERE (SeriesID = '" & newSeriesID & "')")
            SQLstring.Append("SELECT * FROM DataValues WHERE SeriesID = " + _SelectedSeriesID.ToString)
            dt = dbTools.LoadTable("DataValues", SQLstring.ToString)

            If rbtnAlgebraic.Checked Then
                For i As Integer = 0 To dt.Rows.Count - 1
                    currentvalue = dt.Rows(i).Item("DataValue")

                    If currentvalue <> nodatavalue Then
                        'NOTE: Equation = Fx^5 + Ex^4 + Dx^3 + Cx^2 + Bx + A
                        newvalue = (F * (Math.Pow(currentvalue, 5))) + (E * (Math.Pow(currentvalue, 4))) + (D * (Math.Pow(currentvalue, 3))) + (C * (Math.Pow(currentvalue, 2))) + (B * currentvalue) + A
                        newvalue = Math.Round(newvalue, 5)
                    Else
                        newvalue = nodatavalue
                    End If

                    dt.Rows(i).Item("DataValue") = newvalue
                Next
            ElseIf rbtnAggregate.Checked Then

            End If


            'Setting progress bar
            Dim frmloading As ProgressBar = _cEditView.pbProgressBar
            frmloading.Visible = True
            frmloading.Maximum = dt.Rows.Count - 1
            frmloading.Minimum = 0
            frmloading.Value = 0

            _cEditView.lblstatus.Text = "Creating New Data Values Table"

            SQLstring = New StringBuilder()
            For i As Integer = 0 To dt.Columns.Count - 1
                SQLstring.Append(dt.Columns(i).ColumnName.ToString + ",")
            Next
            SQLstring.Remove(SQLstring.Length - 1, 1)
            tempstring = SQLstring.ToString

            newValueID = dbTools.GetNextID("DataValues", "ValueID")

            SQLstring2 = "BEGIN TRANSACTION; "




            For j As Integer = 0 To dt.Rows.Count - 1

                'Making the INSERT SQL string for the new data values

                SQLstring = New StringBuilder()
                SQLstring.Append("INSERT INTO DataValues(")
                SQLstring.Append(tempstring)
                SQLstring.Append(") VALUES (")
                SQLstring.Append((newValueID + j).ToString + "," + newSeriesID.ToString + ",")
                SQLstring.Append(dt.Rows(j).Item("DataValue").ToString + ",")
                If dt.Rows(j).Item("ValueAccuracy").ToString = "" Then
                    SQLstring.Append("NULL,")
                Else
                    SQLstring.Append(dt.Rows(j).Item("ValueAccuracy").ToString + ",")
                End If
                'SQLstring2 = dt.Rows(j).Item("LocalDateTime").ToString

                SQLstring.Append("'" + DateTime.ParseExact(dt.Rows(j).Item("LocalDateTime").ToString, "yyyy/MM/dd H:mm:ss", Nothing).ToString("yyyy-MM-dd HH:mm:ss") + "',")
                SQLstring.Append(dt.Rows(j).Item("UTCOffset").ToString + ",")
                'SQLstring2 = dt.Rows(j).Item("DateTimeUTC").ToString

                SQLstring.Append("'" + DateTime.ParseExact(dt.Rows(j).Item("DateTimeUTC").ToString, "yyyy/MM/dd H:mm:ss", Nothing).ToString("yyyy-MM-dd HH:mm:ss") + "',")
                For i As Integer = 7 To 12
                    If dt.Rows(j).Item(i).ToString = "" Then
                        SQLstring.Append("NULL,")
                    Else

                        If i = 9 Then
                            SQLstring.Append("'" + dt.Rows(j).Item(i).ToString + "',")
                        Else
                            SQLstring.Append(dt.Rows(j).Item(i).ToString + ",")
                        End If
                    End If
                Next
                SQLstring.Remove(SQLstring.Length - 1, 1)
                SQLstring.Append("); ")

                SQLstring2 += SQLstring.ToString


                'Execute the SQL string

                frmloading.Value = j
            Next

            SQLstring2 += "COMMIT;"

            dbTools.ExecuteNonQuery(SQLstring2)

            _cEditView.lblstatus.Text = "Ready"

            dt.Dispose()


            'For i As Integer = 0 To dt.Rows.Count - 1
            '    dt.Rows(i)("SeriesID") = newSeriesID
            'Next



            'Dim tstring() As String = Nothing
            ''tstring(0) = "ValueID"
            ''tstring(1) = "SeriesID"
            ''tstring(2) = "DataValue"
            ''tstring(3) = "ValueAccuracy"
            ''tstring(4) = "LocalDateTime"
            ''tstring(5) = "UTCOffset"
            ''tstring(6) = "DateTimeUTC"
            ''tstring(7) = "OffsetValue"
            ''tstring(8) = "OffsetTypeID"
            ''tstring(9) = "CensorCode"
            ''tstring(10) = "QualifierID"
            ''tstring(11) = "SampleID"
            ''tstring(12) = "FileID"





            'dbTools.SaveTable("DataValues", dt, "ValueID", tstring)

            'dt.Dispose()

        Catch ex As Exception
            'Throw New Exception("Error Occured in InsertNewValues." & vbCrLf & ex.Message)
        End Try
    End Sub

    Private Sub InsertAggregateDataValues()
        Dim SQLstring As String
        Dim SQLstring2 As String
        Dim ColumnNames As String
        Dim newValueID As Integer
        Dim dt As DataTable
        Dim nodatavalue As Double
        Dim newvalue As Double
        Dim currentdate As DateTime
        Dim UTC As Double
        Dim firstdate As DateTime
        Dim lastdate As DateTime
        Dim i As Integer = 0

        Try
            'Setting values to variables
            SQLstring = "SELECT NoDataValue FROM DataSeries LEFT JOIN Variables ON DataSeries.VariableID = Variables.VariableID WHERE SeriesID = " + newSeriesID.ToString
            nodatavalue = dbTools.ExecuteSingleOutput(SQLstring)
            SQLstring = "SELECT * FROM DataValues WHERE SeriesID = " + _SelectedSeriesID.ToString
            dt = dbTools.LoadTable("DataValues", SQLstring)
            SQLstring = "SELECT BeginDateTime FROM DataSeries WHERE SeriesID = " + newSeriesID.ToString
            firstdate = dbTools.ExecuteSingleOutput(SQLstring)
            SQLstring = "SELECT EndDateTime FROM DataSeries WHERE SeriesID = " + newSeriesID.ToString
            lastdate = dbTools.ExecuteSingleOutput(SQLstring)

            'Setting current date (first date) to the first day of the month/quarter
            If rbtnDaily.Checked Then
                currentdate = New DateTime(firstdate.Year, firstdate.Month, firstdate.Day)
            ElseIf rbtnMonthly.Checked Then
                currentdate = New DateTime(firstdate.Year, firstdate.Month, 1)
            ElseIf rbtnQuarterly.Checked Then
                Select Case firstdate.Month
                    Case 1 To 3
                        currentdate = New DateTime(firstdate.Year, 1, 1)
                    Case 4 To 6
                        currentdate = New DateTime(firstdate.Year, 4, 1)
                    Case 7 To 9
                        currentdate = New DateTime(firstdate.Year, 7, 1)
                    Case 10 To 12
                        currentdate = New DateTime(firstdate.Year, 10, 1)
                End Select
            End If


            'Setting progress bar
            Dim frmloading As ProgressBar = _cEditView.pbProgressBar
            frmloading.Visible = True
            If rbtnDaily.Checked Then
                frmloading.Maximum = (lastdate - firstdate).TotalDays
            ElseIf rbtnMonthly.Checked Then
                frmloading.Maximum = Math.Round((lastdate - firstdate).TotalDays / 30)
            ElseIf rbtnQuarterly.Checked Then
                frmloading.Maximum = Math.Round((lastdate - firstdate).TotalDays / 90)
            End If
            frmloading.Minimum = 0
            frmloading.Value = 0

            _cEditView.lblstatus.Text = "Creating New Data Values Table"

            ColumnNames = ""
            For j As Integer = 0 To dt.Columns.Count - 2
                ColumnNames += (dt.Columns(j).ColumnName.ToString + ",")
            Next
            ColumnNames += dt.Columns(dt.Columns.Count - 1).ColumnName.ToString()

            newValueID = dbTools.GetNextID("DataValues", "ValueID")

            SQLstring2 = "BEGIN TRANSACTION; "


            'Create the New Values
            While currentdate <= lastdate
                If rbtnDaily.Checked Then
                    SQLstring = "LocalDateTime >= '" + currentdate.ToString() + "' AND LocalDateTime <= '" + currentdate.AddDays(1).AddMilliseconds(-1).ToString() + "' AND DataValue <> " + nodatavalue.ToString
                ElseIf rbtnMonthly.Checked Then
                    SQLstring = "LocalDateTime >= '" + currentdate.ToString() + "' AND LocalDateTime <= '" + currentdate.AddMonths(1).AddMilliseconds(-1).ToString() + "' AND DataValue <> " + nodatavalue.ToString
                ElseIf rbtnQuarterly.Checked Then
                    SQLstring = "LocalDateTime >= '" + currentdate.ToString() + "' AND LocalDateTime <= '" + currentdate.AddMonths(3).AddMilliseconds(-1).ToString() + "' AND DataValue <> " + nodatavalue.ToString
                End If

                Try
                    If rbtnMaximum.Checked Then
                        newvalue = dt.Compute("Max(DataValue)", SQLstring)
                    ElseIf rbtnMinimum.Checked Then
                        newvalue = dt.Compute("MIN(DataValue)", SQLstring)
                    ElseIf rbtnAverage.Checked Then
                        newvalue = dt.Compute("AVG(DataValue)", SQLstring)
                    ElseIf rbtnSum.Checked Then
                        newvalue = dt.Compute("Sum(DataValue)", SQLstring)
                    End If
                    UTC = dt.Compute("AVG(UTCOffset)", SQLstring)
                Catch
                    newvalue = nodatavalue
                End Try

                'Making the INSERT SQL string for the new data values

                SQLstring = "INSERT INTO DataValues("
                SQLstring += ColumnNames + ") VALUES ("
                SQLstring += (newValueID + i).ToString + "," + newSeriesID.ToString + ","
                SQLstring += newvalue.ToString + ",0,"
                SQLstring += "'" + DateTime.ParseExact(currentdate.ToString, "yyyy/MM/dd H:mm:ss", Nothing).ToString("yyyy-MM-dd HH:mm:ss") + "',"
                SQLstring += UTC.ToString + ","
                SQLstring += "'" + DateTime.ParseExact(currentdate.AddHours(UTC).ToString, "yyyy/MM/dd H:mm:ss", Nothing).ToString("yyyy-MM-dd HH:mm:ss") + "',"
                For j As Integer = 7 To 11
                    If j = 9 Then
                        SQLstring += "'nc',"
                    Else
                        SQLstring += "NULL,"
                    End If
                Next
                SQLstring += "NULL); "


                SQLstring2 += SQLstring


                If rbtnDaily.Checked Then
                    currentdate = currentdate.AddDays(1)
                ElseIf rbtnMonthly.Checked Then
                    currentdate = currentdate.AddMonths(1)
                ElseIf rbtnQuarterly.Checked Then
                    currentdate = currentdate.AddMonths(3)
                End If

                i += 1
                frmloading.Value = i - 1
            End While

            SQLstring2 += "COMMIT;"

            'Execute the SQL string
            dbTools.ExecuteNonQuery(SQLstring2)


            _cEditView.lblstatus.Text = "Ready"
            dt.Dispose()

            'Dim tempstring() As String
            'tempstring = Nothing



            'dbTools.SaveTable("DataValues2", dt, "ValueID", tempstring)

        Catch ex As Exception
            'Throw New Exception("Error Occured in InsertAggregateDataValues." & vbCrLf & ex.Message)
        End Try
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

        currentQualityControlLevelID = dbTools.ExecuteSingleOutput("SELECT QualityControlLevelID FROM DataSeries WHERE SeriesID = " + _SelectedSeriesID.ToString)

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
        Dim repo = RepositoryFactory.Instance.Get(Of IMethodsRepository)(dbTools)
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
        Dim variablesTableManagement As fVariablesTableManagement = New fVariablesTableManagement(variableID, Me)
        variablesTableManagement.Show()
    End Sub

    Public Sub SetDefaultVariable()

        Dim repo = RepositoryFactory.Instance.Get(Of IDataSeriesRepository)(dbTools)
        Dim currentVariableID = repo.GetVariableID(_SelectedSeriesID)

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
    End Sub

    Private Sub btnCancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnCancel.Click
        Me.Close()
    End Sub

    Private Sub btnNewSeries_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnNewSeries.Click

        Dim ci As CultureInfo = Thread.CurrentThread.CurrentCulture

        Dim Validated As Boolean = True

        If rbtnAlgebraic.Checked Then

            AlgebraicValidation(Validated)

        End If

        If Validated Then

            Thread.CurrentThread.CurrentCulture = New CultureInfo("ja-jp")

            InsertNewSeries()

            InsertSeriesProvenance()

            InsertNewDataThemes()

            If rbtnAggregate.Checked Then

                InsertAggregateDataValues()

            Else

                InsertNewDataValues()

            End If

            _cEditView.pTimeSeriesPlot.Clear()

            _cEditView._seriesSelector.RefreshSelection()

            MsgBox("Derive Complete")

            gboxDeriveOption.Enabled = False

            gboxgeneral.Enabled = False

            btnNewSeries.Enabled = False

            btnCancel.Text = "Close"

            _cEditView.pbProgressBar.Value = 0

        Else

            MsgBox("Please enter numbers in the Algebraic Equation.")

        End If

    End Sub

#End Region


End Class