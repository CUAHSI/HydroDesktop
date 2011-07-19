Imports System.Windows.Forms
Imports DotSpatial.Controls
Imports DotSpatial.Controls.RibbonControls
Imports HydroDesktop.Database
Imports HydroDesktop.Interfaces


<Plugin("Ribbon Sample Plugin VB.NET", Author:="ISU", UniqueName:="Ribbon_Sample_Plugin_VB", Version:="1.0")> _
    Public Class Main
    Inherits Extension
    Implements IMapPlugin

#Region "Variables"

    '//reference to the main application and it's UI items
    Private _mapArgs As IMapPluginArgs

    Private _seriesView As ISeriesView

    Private _ribbonButton1 As RibbonButton

    Private _mainControl As MyUserControl

    Private Const _pluginName As String = "VB Sample Plugin"

#End Region

#Region "IExtension Members"

    'Fires when the plugin should become inactive
    Protected Overrides Sub OnDeactivate()

    End Sub
#End Region

#Region "IMapPlugin Members"


    'When the plugin is initialized
    Public Sub Initialize(ByVal args As IMapPluginArgs) Implements IMapPlugin.Initialize

        'Assign the variable for accessing map and main application
        _mapArgs = args

        'To Add Items to the ribbon menu
        If Not _mapArgs.Ribbon Is Nothing Then

            'To add a ribbon button to the 'Home' RibbonTab
            _ribbonButton1 = New RibbonButton(_pluginName)
            'the button is added to the second panel of the first RibbonTab.
            _mapArgs.Ribbon.Tabs(0).Panels(0).Items.Add(_ribbonButton1)
            'Set the image of the ribbon button. The image is taken from the Resources.resx file
            _ribbonButton1.Image = My.Resources.vb_icon_32
            'Link the button click event handler
            AddHandler _ribbonButton1.Click, AddressOf ribbonButton1_Click

            'To add a ribbon panel with a dropDown and regular button
        End If

        'To add a new View to the main application window
        If Not _mapArgs.PanelManager Is Nothing Then

            Dim manager As IHydroAppManager = CType(_mapArgs.AppManager, IHydroAppManager)
            If Not manager Is Nothing Then
                _seriesView = manager.SeriesView
                _mainControl = New MyUserControl(_seriesView.SeriesSelector)
                _mainControl.Dock = DockStyle.Fill
                _seriesView.AddPanel(_pluginName, _mainControl)
            End If
        End If

    End Sub

    Public Overloads Sub Activate() Implements IMapPlugin.Activate
        'MyBase.Activate()
        MyBase.OnActivate()
    End Sub

    'when the plug-in is deactivated
    Public Overloads Sub Deactivate() Implements IMapPlugin.Deactivate

        _mapArgs.PanelManager.RemoveTab(_pluginName)
        _mapArgs.Ribbon.Tabs(0).Panels(0).Items.Remove(_ribbonButton1)

        If _seriesView.PanelNames.Contains(_pluginName) Then
            _seriesView.RemovePanel(_pluginName)
        End If


        'important line to deactivate the plugin
        MyBase.OnDeactivate()

    End Sub
    Public Overloads Property IsEnabled() As Boolean Implements IMapPlugin.IsEnabled
        Get
            Return MyBase.IsEnabled
        End Get
        Set(ByVal value As Boolean)
            MyBase.IsEnabled = value
        End Set
    End Property

#End Region

#Region "Event Handlers"

    'when the 'VB.NET sample' button is clicked, select the RibbonSamplePlugin view
    Sub ribbonButton1_Click()
        'Set main view to 'SeriesView'
        _mapArgs.PanelManager.SelectedTabName = "Series View"

        'Set the seriesPanel to 'VB.NET sample plugin'
        If Not _seriesView Is Nothing Then
            _seriesView.VisiblePanelName = _pluginName
        End If
    End Sub

#End Region

End Class

