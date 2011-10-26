<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class cHistogramPlot
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
		Me.zgHistogramPlot = New ZedGraph.ZedGraphControl
		Me.SuspendLayout()
		'
		'zgHistogramPlot
		'
		Me.zgHistogramPlot.AutoScroll = True
		Me.zgHistogramPlot.Dock = System.Windows.Forms.DockStyle.Fill
		Me.zgHistogramPlot.Location = New System.Drawing.Point(0, 0)
		Me.zgHistogramPlot.Name = "zgHistogramPlot"
		Me.zgHistogramPlot.ScrollGrace = 0
		Me.zgHistogramPlot.ScrollMaxX = 0
		Me.zgHistogramPlot.ScrollMaxY = 0
		Me.zgHistogramPlot.ScrollMaxY2 = 0
		Me.zgHistogramPlot.ScrollMinX = 0
		Me.zgHistogramPlot.ScrollMinY = 0
		Me.zgHistogramPlot.ScrollMinY2 = 0
		Me.zgHistogramPlot.Size = New System.Drawing.Size(200, 200)
		Me.zgHistogramPlot.TabIndex = 0
		'
		'cHistogramPlot
		'
		Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
		Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
		Me.Controls.Add(Me.zgHistogramPlot)
		Me.Name = "cHistogramPlot"
		Me.Size = New System.Drawing.Size(200, 200)
		Me.ResumeLayout(False)

	End Sub
    Friend WithEvents zgHistogramPlot As ZedGraph.ZedGraphControl

End Class
