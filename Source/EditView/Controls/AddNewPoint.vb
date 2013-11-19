Imports HydroDesktop.Database
Imports HydroDesktop.Interfaces

Namespace Controls
    Public Class AddNewPoint
        Public Sub New()

            InitializeComponent()
            initialize()

        End Sub

        Private Sub initialize()

            'OffsetType Column
            Dim dtOffsetTypes = RepositoryFactory.Instance.Get(Of IOffsetTypesRepository)().AsDataTable()
            dtOffsetTypes.Rows.Add()
            dtOffsetTypes.Rows(dtOffsetTypes.Rows.Count - 1)("OffsetDescription") = "No Offset Type"
            dtOffsetTypes.Rows(dtOffsetTypes.Rows.Count - 1)("OffsetTypeID") = 0
            Dim OffsetType As New Windows.Forms.DataGridViewComboBoxColumn
            OffsetType.Name = "OffsetType"
            OffsetType.HeaderText = "OffsetType"
            OffsetType.DataPropertyName = "OffsetType"
            OffsetType.DataSource = dtOffsetTypes
            OffsetType.DisplayMember = "OffsetDescription"
            OffsetType.ValueMember = "OffsetTypeID"
            dgvNewPoints.Columns.Add(OffsetType)

            'Qualifier Column
            Dim dtQualifiers = RepositoryFactory.Instance.Get(Of IQualifiersRepository).AsDataTable()
            dtQualifiers.Rows.Add()
            dtQualifiers.Rows(dtQualifiers.Rows.Count - 1)("QualifierCode") = "No Qualifier"
            dtQualifiers.Rows(dtQualifiers.Rows.Count - 1)("QualifierID") = 0
            Dim Qualifier As New Windows.Forms.DataGridViewComboBoxColumn
            Qualifier.Name = "Qualifier"
            Qualifier.HeaderText = "Qualifier"
            Qualifier.DataPropertyName = "Qualifier"
            Qualifier.DataSource = dtQualifiers
            Qualifier.DisplayMember = "QualifierCode"
            Qualifier.ValueMember = "QualifierID"
            dgvNewPoints.Columns.Add(Qualifier)

            'Sample Column
            Dim dtSamples = RepositoryFactory.Instance.Get(Of ISamplesRepository).AsDataTable()
            dtSamples.Rows.Add()
            dtSamples.Rows(dtSamples.Rows.Count - 1)("SampleType") = "No Sample"
            dtSamples.Rows(dtSamples.Rows.Count - 1)("SampleID") = 0
            Dim Sample As New Windows.Forms.DataGridViewComboBoxColumn
            Sample.Name = "Sample"
            Sample.HeaderText = "Sample"
            Sample.DataPropertyName = "Sample"
            Sample.DataSource = dtSamples
            Sample.DisplayMember = "SampleType"
            Sample.ValueMember = "SampleID"
            dgvNewPoints.Columns.Add(Sample)

            'File Column
            Dim dtFiles = RepositoryFactory.Instance.Get(Of IDataFilesRepository).AsDataTable()
            dtFiles.Rows.Add()
            dtFiles.Rows(dtFiles.Rows.Count - 1)("FileName") = "No File"
            dtFiles.Rows(dtFiles.Rows.Count - 1)("FileID") = 0
            Dim File As New Windows.Forms.DataGridViewComboBoxColumn
            File.Name = "File"
            File.HeaderText = "File"
            File.DataPropertyName = "File"
            File.DataSource = dtFiles
            File.DisplayMember = "FileName"
            File.ValueMember = "FileID"
            dgvNewPoints.Columns.Add(File)



            'First row to show user the format of each cell
            dgvNewPoints.Rows.Add()
            dgvNewPoints.Rows(0).Cells("DataValue").Value = "Decimal"
            dgvNewPoints.Rows(0).Cells("ValueAccuracy").Value = "Decimal"
            dgvNewPoints.Rows(0).Cells("LocalDateTime").Value = "M/d/yyyy h:mm:ss tt"
            dgvNewPoints.Rows(0).Cells("UTCOffset").Value = "Decimal"
            dgvNewPoints.Rows(0).Cells("DateTimeUTC").Value = "M/d/yyyy h:mm:ss tt"
            dgvNewPoints.Rows(0).Cells("OffsetValue").Value = "Decimal"
            dgvNewPoints.Rows(0).Cells("CensorCode").Value = "String"


            'Second row to show user a sample of format
            dgvNewPoints.Rows.Add()
            dgvNewPoints.Rows(1).Cells("DataValue").Value = "68.55"
            dgvNewPoints.Rows(1).Cells("ValueAccuracy").Value = "0"
            dgvNewPoints.Rows(1).Cells("LocalDateTime").Value = "7/30/1988 5:30:30 PM"
            dgvNewPoints.Rows(1).Cells("UTCOffset").Value = "5"
            dgvNewPoints.Rows(1).Cells("DateTimeUTC").Value = "7/30/1988 10:30:30 PM"
            dgvNewPoints.Rows(1).Cells("OffsetValue").Value = "5"
            dgvNewPoints.Rows(1).Cells("CensorCode").Value = "nc"


            'Setting the format of first two rows
            dgvNewPoints.Rows(0).ReadOnly = True
            dgvNewPoints.Rows(1).ReadOnly = True
            dgvNewPoints.Rows(0).DefaultCellStyle.BackColor = Drawing.Color.Wheat
            dgvNewPoints.Rows(1).DefaultCellStyle.BackColor = Drawing.Color.Wheat

            'Setting the format of "not null" columns
            dgvNewPoints.Columns("DataValue").DefaultCellStyle.BackColor = Drawing.Color.Yellow
            dgvNewPoints.Columns("LocalDateTime").DefaultCellStyle.BackColor = Drawing.Color.Yellow
            dgvNewPoints.Columns("UTCOffset").DefaultCellStyle.BackColor = Drawing.Color.Yellow
            dgvNewPoints.Columns("DateTimeUTC").DefaultCellStyle.BackColor = Drawing.Color.Yellow

            'Make the Grid View to add new row for user to add new points
            dgvNewPoints.AllowUserToAddRows = True

        End Sub

        Public Sub AutoDateTime()
            'set default datetime if two points  are selected
            If Not _FirstDate = Nothing And Not _SecondDate = Nothing Then
                Dim ldt As DateTime

                ldt = DateTime.FromBinary(Math.Abs(_SecondDate.ToBinary - _FirstDate.ToBinary) / 2 + _FirstDate.ToBinary)
                dgvNewPoints.Rows.Add()
                dgvNewPoints.Rows(2).Cells("LocalDateTime").Value = ldt

            End If
        End Sub

        Private Sub btnAdd_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles btnAdd.Click
            Dim BeginningRow As Integer = 0
            Dim validate As Boolean = True
            Dim dt As DataTable
            Dim AddingRowsCount As Integer = 0
            'Dim p As PointPair
            'Dim tempp As PointPair
            'Dim k As Integer


            'Count if the first two description rows exist. User can delete these two rows.
            For i As Integer = 0 To 1
                If dgvNewPoints.Rows(i).DefaultCellStyle.BackColor = Drawing.Color.Wheat Then
                    BeginningRow += 1
                End If
            Next

            'Set all boxes' style to default
            For i As Integer = BeginningRow To dgvNewPoints.Rows.Count - 2
                For j As Integer = 0 To dgvNewPoints.Columns.Count - 1
                    dgvNewPoints.Rows(i).Cells(j).Style.ApplyStyle(dgvNewPoints.Columns(j).DefaultCellStyle)
                Next
            Next


            'Main function
            If dgvNewPoints.Rows.Count - BeginningRow > 1 Then
                'Validation
                For i As Integer = BeginningRow To dgvNewPoints.Rows.Count - 2
                    If Not IsNumeric(dgvNewPoints.Rows(i).Cells("DataValue").Value) Then
                        dgvNewPoints.Rows(i).Cells("DataValue").Style.BackColor = Drawing.Color.Red
                        validate = False
                    End If
                    If Not IsNumeric(dgvNewPoints.Rows(i).Cells("ValueAccuracy").Value) And Not dgvNewPoints.Rows(i).Cells("ValueAccuracy").Value = Nothing Then
                        dgvNewPoints.Rows(i).Cells("ValueAccuracy").Style.BackColor = Drawing.Color.Red
                        validate = False
                    End If
                    If Not IsDateTime(dgvNewPoints.Rows(i).Cells("LocalDateTime").Value) Then
                        dgvNewPoints.Rows(i).Cells("LocalDateTime").Style.BackColor = Drawing.Color.Red
                        validate = False
                    End If
                    If Not IsNumeric(dgvNewPoints.Rows(i).Cells("UTCOffset").Value) Then
                        dgvNewPoints.Rows(i).Cells("UTCOffset").Style.BackColor = Drawing.Color.Red
                        validate = False
                    End If
                    If Not IsDateTime(dgvNewPoints.Rows(i).Cells("DateTimeUTC").Value) Then
                        dgvNewPoints.Rows(i).Cells("DateTimeUTC").Style.BackColor = Drawing.Color.Red
                        validate = False
                    End If
                    If Not IsNumeric(dgvNewPoints.Rows(i).Cells("OffsetValue").Value) And Not dgvNewPoints.Rows(i).Cells("OffsetValue").Value = Nothing Then
                        dgvNewPoints.Rows(i).Cells("OffsetValue").Style.BackColor = Drawing.Color.Red
                        validate = False
                    End If
                Next

                If validate Then

                    dt = _cEditView.Editdt
                    'Count the Adding rows which already exist before this time of adding
                    For i As Integer = 0 To dt.Rows.Count - 1
                        If Not dt.Rows(i).RowState = DataRowState.Deleted Then
                            If dt.Rows(i)("Other") = 1 Then
                                AddingRowsCount += 1
                            End If
                        End If

                    Next

                    Dim dataValuesRepo = RepositoryFactory.Instance.Get(Of IDataValuesRepository)()
                    Dim qualifiersRepo = RepositoryFactory.Instance.Get(Of IQualifiersRepository)()
                    For i As Integer = BeginningRow To dgvNewPoints.Rows.Count - 2

                        dt.Rows.Add()
                        dt.Rows(dt.Rows.Count - 1)("ValueID") = dataValuesRepo.GetNextID() + AddingRowsCount
                        dt.Rows(dt.Rows.Count - 1)("SeriesID") = _cEditView.newseriesID
                        dt.Rows(dt.Rows.Count - 1)("DataValue") = dgvNewPoints.Rows(i).Cells("DataValue").Value
                        If Not (dgvNewPoints.Rows(i).Cells("ValueAccuracy").Value = Nothing) Then
                            dt.Rows(dt.Rows.Count - 1)("ValueAccuracy") = dgvNewPoints.Rows(i).Cells("ValueAccuracy").Value
                        Else
                            dt.Rows(dt.Rows.Count - 1)("ValueAccuracy") = 0
                        End If
                        dt.Rows(dt.Rows.Count - 1)("LocalDateTime") = Convert.ToDateTime(dgvNewPoints.Rows(i).Cells("LocalDateTime").Value) 'DateTime.ParseExact(dgvNewPoints.Rows(i).Cells("LocalDateTime").Value.ToString, "M/d/yyyy h:mm:ss tt", Nothing)
                        dt.Rows(dt.Rows.Count - 1)("UTCOffset") = dgvNewPoints.Rows(i).Cells("UTCOffset").Value
                        dt.Rows(dt.Rows.Count - 1)("DateTimeUTC") = Convert.ToDateTime(dgvNewPoints.Rows(i).Cells("DateTimeUTC").Value) 'DateTime.ParseExact(dgvNewPoints.Rows(i).Cells("DateTimeUTC").Value.ToString, "M/d/yyyy h:mm:ss tt", Nothing)
                        If Not dgvNewPoints.Rows(i).Cells("OffsetValue").Value = Nothing Then
                            dt.Rows(dt.Rows.Count - 1)("OffsetValue") = dgvNewPoints.Rows(i).Cells("OffsetValue").Value
                        End If
                        If Not dgvNewPoints.Rows(i).Cells("CensorCode").Value = Nothing Then
                            dt.Rows(dt.Rows.Count - 1)("CensorCode") = dgvNewPoints.Rows(i).Cells("CensorCode").Value
                        End If
                        If Not dgvNewPoints.Rows(i).Cells("OffsetType").Value = Nothing And Not dgvNewPoints.Rows(i).Cells("OffsetType").Value = 0 Then
                            dt.Rows(dt.Rows.Count - 1)("OffsetType") = dgvNewPoints.Rows(i).Cells("OffsetType").Value
                        End If
                        If Not dgvNewPoints.Rows(i).Cells("Qualifier").Value = Nothing And Not dgvNewPoints.Rows(i).Cells("Qualifier").Value = 0 Then
                            dt.Rows(dt.Rows.Count - 1)("QualifierCode") = qualifiersRepo.GetByKey(dgvNewPoints.Rows(i).Cells("Qualifier").Value).Code
                        Else
                            dt.Rows(dt.Rows.Count - 1)("QualifierCode") = "Added point"
                        End If
                        If Not dgvNewPoints.Rows(i).Cells("Sample").Value = Nothing And Not dgvNewPoints.Rows(i).Cells("Sample").Value = 0 Then
                            dt.Rows(dt.Rows.Count - 1)("SampleID") = dgvNewPoints.Rows(i).Cells("Sample").Value
                        End If
                        If Not dgvNewPoints.Rows(i).Cells("File").Value = Nothing And Not dgvNewPoints.Rows(i).Cells("File").Value = 0 Then
                            dt.Rows(dt.Rows.Count - 1)("FileID") = dgvNewPoints.Rows(i).Cells("File").Value
                        End If
                        dt.Rows(dt.Rows.Count - 1)("Other") = 1
                        AddingRowsCount += 1
                    Next

                    _cEditView.RefreshDataGridView()
                    _cEditView.pTimeSeriesPlot.ReplotEditingCurve(_cEditView)

                    Close()
                Else
                    lblError.Text = "The red boxes have problems. Please fix them and then try again."
                    lblError.BackColor = Drawing.Color.Red
                End If

            End If


        End Sub

        'setting default value of some columns
        Private Sub dgvNewPoints_RowsAdded(ByVal sender As System.Object, ByVal e As Windows.Forms.DataGridViewRowsAddedEventArgs) Handles dgvNewPoints.RowsAdded
            With dgvNewPoints.Rows(e.RowIndex)
                '.Cells("UTCOffset").Value = 0
                .Cells("CensorCode").Value = "nc"
            End With
        End Sub

        Private Sub btnCancel_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles btnCancel.Click
            Close()
        End Sub

        Private Function IsDateTime(ByVal value As String) As Boolean
            Try
                DateTime.ParseExact(value, "M/d/yyyy h:mm:ss tt", Nothing)
                Return True
            Catch ex As Exception
                Return False
            End Try
        End Function

        Private Sub dgvNewPoints_CellValueChanged(ByVal sender As System.Object, ByVal e As Windows.Forms.DataGridViewCellEventArgs) Handles dgvNewPoints.CellValueChanged
            If e.ColumnIndex <= 6 And e.ColumnIndex >= 4 And e.RowIndex >= 0 Then
                Dim LocalDateTime As DateTime
                Dim UTCOffset As Double
                Dim DateTimeUTC As DateTime

                If IsDateTime(dgvNewPoints.Rows(e.RowIndex).Cells("LocalDateTime").Value) Then
                    LocalDateTime = dgvNewPoints.Rows(e.RowIndex).Cells("LocalDateTime").Value
                End If
                If IsNumeric(dgvNewPoints.Rows(e.RowIndex).Cells("UTCOffset").Value) Then
                    UTCOffset = dgvNewPoints.Rows(e.RowIndex).Cells("UTCOffset").Value
                End If
                If IsDateTime(dgvNewPoints.Rows(e.RowIndex).Cells("DateTimeUTC").Value) Then
                    DateTimeUTC = dgvNewPoints.Rows(e.RowIndex).Cells("DateTimeUTC").Value
                End If


                If Not LocalDateTime = Nothing And (Not UTCOffset = Nothing Or UTCOffset = 0) Then
                    dgvNewPoints.Rows(e.RowIndex).Cells("DateTimeUTC").Value = LocalDateTime.AddHours(-UTCOffset)
                ElseIf (Not UTCOffset = Nothing Or UTCOffset = 0) And Not DateTimeUTC = Nothing Then
                    dgvNewPoints.Rows(e.RowIndex).Cells("LocalDateTime").Value = DateTimeUTC.AddHours(UTCOffset)
                ElseIf Not LocalDateTime = Nothing And Not DateTimeUTC = Nothing Then
                    dgvNewPoints.Rows(e.RowIndex).Cells("UTCOffset").Value = -((DateTimeUTC - LocalDateTime).TotalHours)
                End If
            End If
        End Sub

    End Class
End Namespace