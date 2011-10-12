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

            If Not SeriesControl Is Nothing Then
                _mainControl = New MyUserControl(SeriesControl)
                _mainControl.Dock = DockStyle.Fill
                App.DockManager.Add(New DockablePanel(kHydroSampleVB, _pluginName, _mainControl, DockStyle.Fill))
            End If

            MyBase.Activate()
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

