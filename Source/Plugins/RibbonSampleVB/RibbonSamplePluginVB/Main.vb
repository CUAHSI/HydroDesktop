Imports System.Windows.Forms
Imports DotSpatial.Controls
Imports DotSpatial.Controls.RibbonControls
Imports HydroDesktop.Database
Imports HydroDesktop.Interfaces
Imports DotSpatial.Controls.Header
Imports System.ComponentModel.Composition


Namespace HydroDesktop.SamplePluginVB

    Public Class Main
        Inherits Extension

#Region "Variables"

        <Import("SeriesControl", GetType(ISeriesSelector))>
        Private SeriesControl As ISeriesSelector

        Private _mainControl As MyUserControl

        Private Const _pluginName As String = "VB_Sample"
        Private Const kHydroSampleVBDock As String = "kHydroSampleVBDock"

#End Region

#Region "IMapPlugin Members"

        'When the plugin is initialized
        Public Overrides Sub Activate()

            'To add a ribbon button to the 'Home' RibbonTab
            Dim button1 As New SimpleActionItem(_pluginName, AddressOf button1_Click)
            button1.LargeImage = My.Resources.vb_icon_32
            App.HeaderControl.Add(button1)

            If Not SeriesControl Is Nothing Then
                _mainControl = New MyUserControl(SeriesControl)
                _mainControl.Dock = DockStyle.Fill
                App.DockManager.Add(kHydroSampleVBDock, _pluginName, _mainControl, DockStyle.Fill)
            End If

            MyBase.Activate()
        End Sub

        'when the plug-in is deactivated
        Public Overrides Sub Deactivate()

            App.HeaderControl.RemoveItems()

            App.DockManager.Remove(kHydroSampleVBDock)

            MyBase.Deactivate()

        End Sub

#End Region

#Region "Event Handlers"

        Sub button1_Click()

        End Sub

#End Region

    End Class
End Namespace

