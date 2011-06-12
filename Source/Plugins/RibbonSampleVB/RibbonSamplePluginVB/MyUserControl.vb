Imports HydroDesktop.Database
Imports HydroDesktop.Interfaces
Imports HydroDesktop.Configuration

'This is the main user control added by the plug-in to the main view.
Public Class MyUserControl

#Region "Private Variables"
    Private _seriesSelector As ISeriesSelector
#End Region

    Public Sub New()

        ' This call is required by the Windows Form Designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.

        btnSaveEdits.Enabled = False

    End Sub

    Public Sub New(ByVal seriesSelector As ISeriesSelector)

        ' This call is required by the Windows Form Designer.
        InitializeComponent()

        'set the 'args' private variable
        _seriesSelector = seriesSelector

        'assign the events
        AddHandler _seriesSelector.SeriesCheck, AddressOf SeriesSelector_SeriesCheck
        AddHandler _seriesSelector.SeriesSelected, AddressOf SeriesSelector_SeriesSelected

        'This call is required for the SeriesSelector control synchronization
        'SeriesSelector41.Database = args.Database
        'AddHandler SeriesSelector41.SeriesChecked, AddressOf SeriesSelector_SeriesChecked
        'AddHandler SeriesSelector41.SeriesUnchecked, AddressOf SeriesSelector_SeriesUnchecked
        'AddHandler SeriesSelector41.SeriesSelected, AddressOf SeriesSelector_SeriesSelected
        btnSaveEdits.Enabled = False

    End Sub

    Sub SeriesSelector_SeriesSelected(ByVal sender As Object, ByVal e As SeriesEventArgs)
        txtSelectedID.Text = _seriesSelector.SelectedSeriesID

        'Show content in the data table
        Dim dbTools As DbOperations = New DbOperations(Settings.Instance.DataRepositoryConnectionString, DatabaseTypes.SQLite)
        Dim table As DataTable = dbTools.LoadTable("SELECT * FROM DataValues WHERE SeriesID = " & txtSelectedID.Text)
        DataGridView1.DataSource = table

        If lstCheckedSeries.Items.Count > 0 Then
            btnSaveEdits.Enabled = True
        Else
            btnSaveEdits.Enabled = False
        End If
    End Sub

    Sub SeriesSelector_SeriesCheck(ByVal sender As Object, ByVal e As SeriesEventArgs)
        txtSelectedID.Text = _seriesSelector.SelectedSeriesID
        'Show the list of checked series IDs
        lstCheckedSeries.Items.Clear()
        Dim checkedIDs() As Integer = _seriesSelector.CheckedIDList
        For Each seriesID As Integer In checkedIDs
            lstCheckedSeries.Items.Add(seriesID)
        Next
    End Sub

    

    Private Sub btnSaveEdits_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSaveEdits.Click

        Dim table As DataTable = DataGridView1.DataSource
        Dim dbTools As DbOperations = New DbOperations(Settings.Instance.DataRepositoryConnectionString, DatabaseTypes.SQLite)
        Dim uniqueFields(1) As String
        uniqueFields(0) = "SeriesID"
        uniqueFields(1) = "LocalDateTime"
        dbTools.SaveTable("DataValues", table, "ValueID", uniqueFields)
        MsgBox("Edits have been saved.")
    End Sub
End Class
