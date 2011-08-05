Imports System.Windows.Forms
Imports DotSpatial.Controls
Imports DotSpatial.Controls.RibbonControls
Imports HydroDesktop.Database
Imports HydroDesktop.Interfaces
Imports DotSpatial.Controls.Header



Public Class Main
    Inherits Extension

#Region "Variables"

    Private _seriesView As ISeriesView

    Private _mainControl As MyUserControl

    Private Const _pluginName As String = "VB Sample Plugin"

#End Region

#Region "IMapPlugin Members"


    'When the plugin is initialized
    Public Overrides Sub Activate()

        'To add a ribbon button to the 'Home' RibbonTab
        Dim button1 As New SimpleActionItem(_pluginName, AddressOf ribbonButton1_Click)
        button1.LargeImage = My.Resources.vb_icon_32
        App.HeaderControl.Add(button1)

        'To add a new View to the main application window
        If Not App.TabManager Is Nothing Then

            Dim manager As IHydroAppManager = TryCast(App, IHydroAppManager)
            If Not manager Is Nothing Then
                _seriesView = manager.SeriesView
                _mainControl = New MyUserControl(_seriesView.SeriesSelector)
                _mainControl.Dock = DockStyle.Fill
                _seriesView.AddPanel(_pluginName, _mainControl)
            End If
        End If

        MyBase.Activate()
    End Sub

    'when the plug-in is deactivated
    Public Overloads Sub Deactivate()

        App.TabManager.RemoveTab(_pluginName)
        App.HeaderControl.RemoveItems()

        If _seriesView.PanelNames.Contains(_pluginName) Then
            _seriesView.RemovePanel(_pluginName)
        End If


        'important line to deactivate the plugin
        MyBase.Deactivate()

    End Sub

#End Region

#Region "Event Handlers"

    'when the 'VB.NET sample' button is clicked, select the RibbonSamplePlugin view
    Sub ribbonButton1_Click()
        'Set main view to 'SeriesView'
        App.TabManager.SelectedTabName = "Series View"

        'Set the seriesPanel to 'VB.NET sample plugin'
        If Not _seriesView Is Nothing Then
            _seriesView.VisiblePanelName = _pluginName
        End If
    End Sub

#End Region

End Class

