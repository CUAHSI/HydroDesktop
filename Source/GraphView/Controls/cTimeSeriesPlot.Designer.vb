<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class cTimeSeriesPlot
    Inherits System.Windows.Forms.UserControl

    'UserControl overrides dispose to clean up the component list.
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
		Me.components = New System.ComponentModel.Container
        Me.zgTimeSeries = New Controls.ZedGraphControlEx
		Me.SuspendLayout()
		'
		'zgTimeSeries
		'
		Me.zgTimeSeries.AutoScroll = True
		Me.zgTimeSeries.Dock = System.Windows.Forms.DockStyle.Fill
		Me.zgTimeSeries.Location = New System.Drawing.Point(0, 0)
		Me.zgTimeSeries.Name = "zgTimeSeries"
		Me.zgTimeSeries.ScrollGrace = 0
		Me.zgTimeSeries.ScrollMaxX = 0
		Me.zgTimeSeries.ScrollMaxY = 0
		Me.zgTimeSeries.ScrollMaxY2 = 0
		Me.zgTimeSeries.ScrollMinX = 0
		Me.zgTimeSeries.ScrollMinY = 0
		Me.zgTimeSeries.ScrollMinY2 = 0
		Me.zgTimeSeries.Size = New System.Drawing.Size(200, 200)
		Me.zgTimeSeries.TabIndex = 0
		'
		'cTimeSeriesPlot
		'
		Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
		Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
		Me.Controls.Add(Me.zgTimeSeries)
		Me.Name = "cTimeSeriesPlot"
		Me.Size = New System.Drawing.Size(200, 200)
		Me.ResumeLayout(False)

	End Sub
    Friend WithEvents zgTimeSeries As Controls.ZedGraphControlEx

End Class
