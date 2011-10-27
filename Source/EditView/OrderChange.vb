Imports System.Windows.Forms
Public Class OrderChange
    Const DDL_WIDTH As Integer = 200
    Dim dropDowns() As ComboBox
    Dim m_order() As String
    Public Sub New(ByVal order() As String)

        ' This call is required by the Windows Form Designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        m_order = order

        ReDim dropDowns(order.Length - 1)
        For i As Integer = 0 To (order.Length - 1)
            Dim ddl As New ComboBox
            AddHandler ddl.SelectedIndexChanged, AddressOf dropDowns_SelectedIndexChanged
            'DO WORK HERE
            If (i = 0) Then
                ddl.Enabled = True
                ddl.Items.Clear()
                ddl.Items.AddRange(order)
            Else
                ddl.Enabled = False
            End If
            ddl.DropDownStyle = ComboBoxStyle.DropDownList
            ddl.Width = DDL_WIDTH
            ddl.Top = (i * 30) + 5
            ddl.Left = 5
            dropDowns(i) = ddl
            pnlList.Controls.Add(ddl)
        Next i
    End Sub

    Private Sub OrderChange_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

    End Sub

    Private Sub dropDowns_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs)
        Dim ddl As ComboBox = sender
        Dim index As Integer = Array.IndexOf(dropDowns, ddl)

        If ddl.SelectedItem Is Nothing Then
            If ((index + 1) < dropDowns.Length) Then
                dropDowns(index + 1).Enabled = False
                dropDowns(index + 1).Items.Clear()
            End If
        Else
            Dim available As New List(Of String)(m_order)
            For x As Integer = 0 To index
                available.Remove(dropDowns(x).SelectedItem)
            Next x
            If ((index + 1) < dropDowns.Length) Then
                dropDowns(index + 1).Enabled = True
                dropDowns(index + 1).Items.Clear()
                dropDowns(index + 1).Items.AddRange(available.ToArray)
                For y As Integer = (index + 2) To (dropDowns.Length - 1)
                    dropDowns(y).Enabled = False
                    dropDowns(y).Items.Clear()
                Next y
            End If
        End If
    End Sub

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        Dim valid As Boolean = True
        For Each ddl As ComboBox In dropDowns
            If ddl.SelectedItem Is Nothing Then
                valid = False
                Exit For
            End If
        Next ddl
        If valid Then
            Me.DialogResult = Windows.Forms.DialogResult.OK
        Else
            MsgBox("All values must be selected", , "Error")
        End If
    End Sub

    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click
        Me.DialogResult = Windows.Forms.DialogResult.Cancel
    End Sub

    Public ReadOnly Property Order() As String()
        Get
            Dim temp(dropDowns.Length - 1) As String

            For i As Integer = 0 To (dropDowns.Length - 1)
                temp(i) = dropDowns(i).SelectedItem
            Next i

            Return temp
        End Get
    End Property
End Class