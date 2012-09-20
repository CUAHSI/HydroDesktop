Imports System.Drawing
Imports System.Windows.Forms
Imports System.ComponentModel
Imports HydroDesktop.Common.Tools

Public Class ColorPalette

    Const COLOR_CNT As Int32 = 10
    Private ReadOnly _colors As IList(Of Color)
    Private ReadOnly _colorControls As IList(Of Label)
    Private _showButtons As Boolean

    Public Event OnLineClicked As EventHandler(Of ColorPaletteButtonEventArgs)
    Public Event OnPointClicked As EventHandler(Of ColorPaletteButtonEventArgs)

    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.

        _colorControls = New List(Of Label)(COLOR_CNT)
        _colors = Enumerable.Repeat(Color.White, COLOR_CNT).ToList()
        InitColorControl(lblc10)
        InitColorControl(lblc11)
        InitColorControl(lblc12)
        InitColorControl(lblc13)
        InitColorControl(lblc14)
        InitColorControl(lblc15)
        InitColorControl(lblc16)
        InitColorControl(lblc17)
        InitColorControl(lblc18)
        InitColorControl(lblc19)

        ShowButtons = True
    End Sub

#Region "Properties"

    <Browsable(False)>
    Public Property DefaultColorFunc() As Func(Of Color)

    Public Property CanChangeColors() As Boolean

    Public Property ShowButtons() As Boolean
        Get
            Return _showButtons
        End Get
        Set(value As Boolean)
            _showButtons = value

            btnbothc1.Visible = value
            btnlinec1.Visible = value
            btnpointc1.Visible = value
        End Set
    End Property

    <Browsable(False)>
    Public Property Colors() As IList(Of Color)
        Get
            Return _colors
        End Get
        Set(ByVal value As IList(Of Color))
            If value Is Nothing Then Return
            If value.Count <> COLOR_CNT Then Return

            For i As Integer = 0 To COLOR_CNT - 1
                _colors(i) = value(i)
                _colorControls(i).BackColor = value(i)
            Next
        End Set
    End Property

#End Region

#Region "Private methods"

    Private Sub InitColorControl(ByVal label As Label)
        _colorControls.Add(label)
        label.BackColor = _colors(_colorControls.IndexOf(label))
        AddHandler label.Click, AddressOf ColorButtonClick
    End Sub

    Private Sub ColorButtonClick(ByVal sender As Object, ByVal e As EventArgs)
        If Not CanChangeColors Then Return

        Dim label = TryCast(sender, Label)
        If label Is Nothing Then Return

        Dim defaltColor = If(DefaultColorFunc Is Nothing, Nothing, DefaultColorFunc.Invoke())
        Dim newColor = DrawingHelper.PromptForColor(defaltColor)

        If newColor.HasValue Then
            label.BackColor = newColor.Value
            _colors(_colorControls.IndexOf(label)) = newColor.Value
        End If
    End Sub

    Private Sub btnbothc1_Click(sender As Object, e As EventArgs) Handles btnbothc1.Click
        btnlinec1_Click(sender, e)
        btnpointc1_Click(sender, e)
    End Sub

    Private Sub btnlinec1_Click(sender As Object, e As EventArgs) Handles btnlinec1.Click
        RaiseEvent OnLineClicked(Me, New ColorPaletteButtonEventArgs(_colors))
    End Sub

    Private Sub btnpointc1_Click(sender As Object, e As EventArgs) Handles btnpointc1.Click
        RaiseEvent OnPointClicked(Me, New ColorPaletteButtonEventArgs(_colors))
    End Sub

#End Region

End Class

Public Class ColorPaletteButtonEventArgs
    Inherits EventArgs

    Private ReadOnly _colors As IList(Of Color)
    Public ReadOnly Property Colors() As IList(Of Color)
        Get
            Return _colors
        End Get
    End Property

    Sub New(ByVal colors As IList(Of Color))
        _colors = colors
    End Sub

End Class