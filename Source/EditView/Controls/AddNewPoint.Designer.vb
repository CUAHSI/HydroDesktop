Imports EditView

Namespace Controls

    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class AddNewPoint
        Inherits System.Windows.Forms.Form
        Public _cEditView As Controls.EditView
        Public _FirstDate As DateTime
        Public _SecondDate As DateTime
        'Form overrides dispose to clean up the component list.
        <System.Diagnostics.DebuggerNonUserCode()> _
        Protected Overrides Sub Dispose(ByVal disposing As Boolean)
            Try
                If disposing AndAlso components IsNot Nothing Then
                    components.Dispose()
                End If
            Finally
                MyBase.Dispose(disposing)
            End Try
        End Sub

        'Required by the Windows Form Designer
        Private components As System.ComponentModel.IContainer

        'NOTE: The following procedure is required by the Windows Form Designer
        'It can be modified using the Windows Form Designer.  
        'Do not modify it using the code editor.
        <System.Diagnostics.DebuggerStepThrough()> _
        Private Sub InitializeComponent()
            Me.components = New System.ComponentModel.Container()
            Dim DataGridViewCellStyle9 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
            Dim DataGridViewCellStyle10 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
            Dim DataGridViewCellStyle11 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
            Dim DataGridViewCellStyle12 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
            Dim DataGridViewCellStyle13 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
            Dim DataGridViewCellStyle14 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
            Dim DataGridViewCellStyle15 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
            Dim DataGridViewCellStyle16 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
            Me.ConfigBindingSource = New System.Windows.Forms.BindingSource(Me.components)
            Me.btnAdd = New System.Windows.Forms.Button()
            Me.dgvNewPoints = New System.Windows.Forms.DataGridView()
            Me.ValueID = New System.Windows.Forms.DataGridViewTextBoxColumn()
            Me.SeriesID = New System.Windows.Forms.DataGridViewTextBoxColumn()
            Me.DataValue = New System.Windows.Forms.DataGridViewTextBoxColumn()
            Me.ValueAccuracy = New System.Windows.Forms.DataGridViewTextBoxColumn()
            Me.LocalDateTime = New System.Windows.Forms.DataGridViewTextBoxColumn()
            Me.UTCOffset = New System.Windows.Forms.DataGridViewTextBoxColumn()
            Me.DateTimeUTC = New System.Windows.Forms.DataGridViewTextBoxColumn()
            Me.OffsetValue = New System.Windows.Forms.DataGridViewTextBoxColumn()
            Me.CensorCode = New System.Windows.Forms.DataGridViewTextBoxColumn()
            Me.lblHelp = New System.Windows.Forms.Label()
            Me.lblError = New System.Windows.Forms.Label()
            Me.btnCancel = New System.Windows.Forms.Button()
            CType(Me.ConfigBindingSource, System.ComponentModel.ISupportInitialize).BeginInit()
            CType(Me.dgvNewPoints, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.SuspendLayout()
            '
            'ConfigBindingSource
            '
            Me.ConfigBindingSource.DataSource = GetType(HydroDesktop.Configuration.Settings)
            '
            'btnAdd
            '
            Me.btnAdd.Location = New System.Drawing.Point(224, 198)
            Me.btnAdd.Name = "btnAdd"
            Me.btnAdd.Size = New System.Drawing.Size(274, 23)
            Me.btnAdd.TabIndex = 4
            Me.btnAdd.Text = "Add"
            Me.btnAdd.UseVisualStyleBackColor = True
            '
            'dgvNewPoints
            '
            Me.dgvNewPoints.AllowUserToAddRows = False
            Me.dgvNewPoints.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
            Me.dgvNewPoints.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.ValueID, Me.SeriesID, Me.DataValue, Me.ValueAccuracy, Me.LocalDateTime, Me.UTCOffset, Me.DateTimeUTC, Me.OffsetValue, Me.CensorCode})
            Me.dgvNewPoints.Dock = System.Windows.Forms.DockStyle.Top
            Me.dgvNewPoints.Location = New System.Drawing.Point(0, 0)
            Me.dgvNewPoints.Name = "dgvNewPoints"
            Me.dgvNewPoints.Size = New System.Drawing.Size(823, 167)
            Me.dgvNewPoints.TabIndex = 5
            '
            'ValueID
            '
            DataGridViewCellStyle9.Format = "N0"
            DataGridViewCellStyle9.NullValue = Nothing
            Me.ValueID.DefaultCellStyle = DataGridViewCellStyle9
            Me.ValueID.HeaderText = "ValueID"
            Me.ValueID.Name = "ValueID"
            Me.ValueID.Visible = False
            '
            'SeriesID
            '
            DataGridViewCellStyle10.Format = "N0"
            DataGridViewCellStyle10.NullValue = Nothing
            Me.SeriesID.DefaultCellStyle = DataGridViewCellStyle10
            Me.SeriesID.HeaderText = "SeriesID"
            Me.SeriesID.Name = "SeriesID"
            Me.SeriesID.Visible = False
            '
            'DataValue
            '
            DataGridViewCellStyle11.Format = "N6"
            DataGridViewCellStyle11.NullValue = Nothing
            Me.DataValue.DefaultCellStyle = DataGridViewCellStyle11
            Me.DataValue.HeaderText = "DataValue"
            Me.DataValue.Name = "DataValue"
            '
            'ValueAccuracy
            '
            DataGridViewCellStyle12.Format = "N6"
            Me.ValueAccuracy.DefaultCellStyle = DataGridViewCellStyle12
            Me.ValueAccuracy.HeaderText = "ValueAccuracy"
            Me.ValueAccuracy.Name = "ValueAccuracy"
            '
            'LocalDateTime
            '
            DataGridViewCellStyle13.Format = "G"
            DataGridViewCellStyle13.NullValue = Nothing
            Me.LocalDateTime.DefaultCellStyle = DataGridViewCellStyle13
            Me.LocalDateTime.HeaderText = "LocalDateTime"
            Me.LocalDateTime.Name = "LocalDateTime"
            '
            'UTCOffset
            '
            DataGridViewCellStyle14.Format = "N6"
            DataGridViewCellStyle14.NullValue = Nothing
            Me.UTCOffset.DefaultCellStyle = DataGridViewCellStyle14
            Me.UTCOffset.HeaderText = "UTCOffset"
            Me.UTCOffset.Name = "UTCOffset"
            '
            'DateTimeUTC
            '
            DataGridViewCellStyle15.Format = "G"
            DataGridViewCellStyle15.NullValue = Nothing
            Me.DateTimeUTC.DefaultCellStyle = DataGridViewCellStyle15
            Me.DateTimeUTC.HeaderText = "DateTimeUTC"
            Me.DateTimeUTC.Name = "DateTimeUTC"
            '
            'OffsetValue
            '
            DataGridViewCellStyle16.Format = "N6"
            Me.OffsetValue.DefaultCellStyle = DataGridViewCellStyle16
            Me.OffsetValue.HeaderText = "OffsetValue"
            Me.OffsetValue.Name = "OffsetValue"
            '
            'CensorCode
            '
            Me.CensorCode.HeaderText = "CensorCode"
            Me.CensorCode.Name = "CensorCode"
            '
            'lblHelp
            '
            Me.lblHelp.Location = New System.Drawing.Point(12, 171)
            Me.lblHelp.Name = "lblHelp"
            Me.lblHelp.Size = New System.Drawing.Size(206, 71)
            Me.lblHelp.TabIndex = 6
            Me.lblHelp.Text = "Please enter all cells in correct format. You can see the formats in the first ro" & _
        "w. The second row shows you a sample. The yellow cells should have data."
            '
            'lblError
            '
            Me.lblError.AutoSize = True
            Me.lblError.Location = New System.Drawing.Point(318, 174)
            Me.lblError.Name = "lblError"
            Me.lblError.Size = New System.Drawing.Size(0, 13)
            Me.lblError.TabIndex = 7
            '
            'btnCancel
            '
            Me.btnCancel.Location = New System.Drawing.Point(504, 198)
            Me.btnCancel.Name = "btnCancel"
            Me.btnCancel.Size = New System.Drawing.Size(274, 23)
            Me.btnCancel.TabIndex = 8
            Me.btnCancel.Text = "Cancel"
            Me.btnCancel.UseVisualStyleBackColor = True
            '
            'AddNewPoint
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.ClientSize = New System.Drawing.Size(823, 229)
            Me.Controls.Add(Me.btnCancel)
            Me.Controls.Add(Me.lblError)
            Me.Controls.Add(Me.lblHelp)
            Me.Controls.Add(Me.dgvNewPoints)
            Me.Controls.Add(Me.btnAdd)
            Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
            Me.MaximizeBox = False
            Me.MinimizeBox = False
            Me.Name = "AddNewPoint"
            Me.ShowInTaskbar = False
            Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
            Me.Text = "Add New Point"
            CType(Me.ConfigBindingSource, System.ComponentModel.ISupportInitialize).EndInit()
            CType(Me.dgvNewPoints, System.ComponentModel.ISupportInitialize).EndInit()
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub
        Friend WithEvents ConfigBindingSource As System.Windows.Forms.BindingSource
        Friend WithEvents btnAdd As System.Windows.Forms.Button
        Friend WithEvents dgvNewPoints As System.Windows.Forms.DataGridView
        Friend WithEvents lblHelp As System.Windows.Forms.Label
        Friend WithEvents lblError As System.Windows.Forms.Label
        Friend WithEvents ValueID As System.Windows.Forms.DataGridViewTextBoxColumn
        Friend WithEvents SeriesID As System.Windows.Forms.DataGridViewTextBoxColumn
        Friend WithEvents DataValue As System.Windows.Forms.DataGridViewTextBoxColumn
        Friend WithEvents ValueAccuracy As System.Windows.Forms.DataGridViewTextBoxColumn
        Friend WithEvents LocalDateTime As System.Windows.Forms.DataGridViewTextBoxColumn
        Friend WithEvents UTCOffset As System.Windows.Forms.DataGridViewTextBoxColumn
        Friend WithEvents DateTimeUTC As System.Windows.Forms.DataGridViewTextBoxColumn
        Friend WithEvents OffsetValue As System.Windows.Forms.DataGridViewTextBoxColumn
        Friend WithEvents CensorCode As System.Windows.Forms.DataGridViewTextBoxColumn
        Friend WithEvents btnCancel As System.Windows.Forms.Button
    End Class
End Namespace