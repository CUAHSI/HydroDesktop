Imports System.Windows.Forms
Imports DotSpatial.Controls
Imports HydroDesktop.Database
Imports HydroDesktop.Interfaces
Imports DotSpatial.Controls.Header
Imports System.ComponentModel.Composition
Imports DotSpatial.Controls.Docking


Namespace HydroDesktop.SamplePluginVB

    Public Class Main
        Inherits Extension

#Region "Variables"

        <Import("SeriesControl", GetType(ISeriesSelector))>
        Private SeriesControl As ISeriesSelector

        Private _mainControl As MyUserControl

        Private Const _pluginName As String = "VB_Sample"
        Private Const kHydroSampleVB As String = "kHydroSampleVB" 'the root item key

#End Region

#Region "IMapPlugin Members"

        'When the plugin is initialized
        Public Overrides Sub Activate()

            Dim root1 As New RootItem(kHydroSampleVB, _pluginName)
            root1.SortOrder = 70
            App.HeaderControl.Add(root1)

            'To add a ribbon button to the 'Home' RibbonTab
            Dim button1 As New SimpleActionItem(_pluginName, AddressOf button1_Click)
            button1.LargeImage = My.Resources.vb_icon_32
            button1.RootKey = kHydroSampleVB
            App.HeaderControl.Add(button1)

            'testing event handlers
            AddHandler App.DockManager.PanelAdded, AddressOf PanelAdded
            AddHandler App.DockManager.PanelRemoved, AddressOf PanelRemoved
            AddHandler App.DockManager.PanelClosed, AddressOf PanelClosed

            If Not SeriesControl Is Nothing Then
                _mainControl = New MyUserControl(SeriesControl)
                _mainControl.Dock = DockStyle.Fill


                Dim myPanel As New DockablePanel(kHydroSampleVB, _pluginName, _mainControl, DockStyle.Fill)
                myPanel.DefaultSortOrder = 200
                App.DockManager.Add(myPanel)
            End If

            MyBase.Activate()
        End Sub

        Sub PanelAdded(ByVal sender As Object, ByVal e As Docking.DockablePanelEventArgs)
            Debug.WriteLine("Panel Added:" & e.ActivePanelKey)
        End Sub

        Sub PanelRemoved(ByVal sender As Object, ByVal e As Docking.DockablePanelEventArgs)
            Debug.WriteLine("Panel Removed:" & e.ActivePanelKey)
        End Sub

        Sub PanelClosed(ByVal sender As Object, ByVal e As Docking.DockablePanelEventArgs)
            Debug.WriteLine("Panel Closed:" & e.ActivePanelKey)
        End Sub


        'when the plug-in is deactivated
        Public Overrides Sub Deactivate()

            App.HeaderControl.RemoveAll()

            App.DockManager.Remove(kHydroSampleVB)

            MyBase.Deactivate()

        End Sub

#End Region

#Region "Event Handlers"

        Sub button1_Click()

        End Sub

#End Region

    End Class
End Namespace

