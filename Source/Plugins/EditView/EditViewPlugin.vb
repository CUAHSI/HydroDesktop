Imports System.Windows.Forms
Imports DotSpatial.Controls
Imports DotSpatial.Controls.RibbonControls
Imports HydroDesktop.Database
Imports HydroDesktop.Interfaces
Imports DotSpatial.Controls.Header
Imports System.ComponentModel.Composition

Namespace EditView

    Public Class EditViewPlugin
        Inherits Extension

#Region "Variables"

        <Import("SeriesViewControl")>
        Private _seriesView As SeriesView.SeriesViewControl

        Private _mainControl As cEditView

        Private Const _pluginName As String = "Edit"
        Private Const _editTabKey As String = "kEdit"

        Private _EditView As New RootItem(_editTabKey, _pluginName)

        Private btnSelectSeries As SimpleActionItem

        Private btnDeriveNewDataSeries As SimpleActionItem

        Private btnRestoreData As SimpleActionItem

        Private btnApplyToDatabase As SimpleActionItem

        Private ckbShowLegend As SimpleActionItem

        Private btnChangeYValue As SimpleActionItem

        Private btnInterpolate As SimpleActionItem

        Private btnFlag As SimpleActionItem
        Private btnAddNewPoint As SimpleActionItem

        Private btnDeletePoint As SimpleActionItem

#End Region

#Region "IExtension Members"

        ''' <summary>
        ''' Initialize the  plugin
        ''' </summary>
        Public Overrides Sub Activate()

            '**************************************************************************************
            'Adding the tab
            App.HeaderControl.Add(_EditView)

            'TODO handle this using DockManager
            'AddHandler _EditView.ActiveChanged, AddressOf EditViewTab_ActiveChanged
            'AddHandler App.Ribbon.ActiveTabChanged, AddressOf Ribbon_ActiveChanged

            '**************************************************************************************

            'To add a new main panel View to the main application window
            If Not _seriesView Is Nothing Then
                _mainControl = New cEditView(_seriesView.SeriesSelector)
            Else
                _mainControl = New cEditView()
            End If
            _mainControl.Dock = DockStyle.Fill
            App.DockManager.Add("kEditViewPanel", "edit", _mainControl, DockStyle.Fill)

            'Dim manager As IHydroAppManager = TryCast(App, IHydroAppManager)
            'If Not manager Is Nothing Then

            '    _seriesView = manager.SeriesView
            '    _mainControl = New cEditView(_seriesView.SeriesSelector)
            '    _seriesView.AddPanel(_pluginName, _mainControl)
            'Else
            '    _mainControl = New cEditView()
            '    _mainControl.Dock = DockStyle.Fill
            '    App.DockManager.Add("kEditView", _mainControl, DockStyle.Fill)
            'End If

            InitializeRibbonButtons()

            'opening project event
            AddHandler App.SerializationManager.Deserializing, AddressOf SerializationManager_Deserializing

        End Sub


        Private Sub InitializeRibbonButtons()

            'Main Function Panel
            Dim mainFunctionGroup As String = "Main Functions"

            btnSelectSeries = New SimpleActionItem("Edit Series", AddressOf btnSelectSeries_Click)
            btnSelectSeries.RootKey = _editTabKey
            btnSelectSeries.LargeImage = My.Resources.Edit
            btnSelectSeries.GroupCaption = mainFunctionGroup
            App.HeaderControl.Add(btnSelectSeries)

            btnDeriveNewDataSeries = New SimpleActionItem("Derive Series", AddressOf _mainControl.btnDeriveNewDataSeries_Click)
            btnDeriveNewDataSeries.RootKey = _editTabKey
            btnDeriveNewDataSeries.LargeImage = My.Resources.DeriveNewSeries
            btnDeriveNewDataSeries.GroupCaption = mainFunctionGroup
            App.HeaderControl.Add(btnDeriveNewDataSeries)

            btnRestoreData = New SimpleActionItem("Restore Data", AddressOf _mainControl.btnRestoreData_Click)
            btnRestoreData.RootKey = _editTabKey
            btnRestoreData.LargeImage = My.Resources.Restore
            btnRestoreData.GroupCaption = mainFunctionGroup
            App.HeaderControl.Add(btnRestoreData)

            btnApplyToDatabase = New SimpleActionItem("Save To Database", AddressOf _mainControl.btnApplyToDatabase_Click)
            btnApplyToDatabase.RootKey = _editTabKey
            btnApplyToDatabase.LargeImage = My.Resources.Save
            btnApplyToDatabase.GroupCaption = mainFunctionGroup
            App.HeaderControl.Add(btnApplyToDatabase)

            'Plot Function Panel
            Dim editFunctionGroup As String = "Edit Functions"

            btnChangeYValue = New SimpleActionItem("Change Value", AddressOf _mainControl.btnChangeYValue_Click)
            btnChangeYValue.RootKey = _editTabKey
            btnChangeYValue.LargeImage = My.Resources.ChangeValue
            btnChangeYValue.GroupCaption = editFunctionGroup
            App.HeaderControl.Add(btnChangeYValue)

            btnInterpolate = New SimpleActionItem("Interpolate", AddressOf _mainControl.btnInterpolate_Click)
            btnInterpolate.RootKey = _editTabKey
            btnInterpolate.LargeImage = My.Resources.Interpolate
            btnInterpolate.GroupCaption = editFunctionGroup
            App.HeaderControl.Add(btnInterpolate)

            btnFlag = New SimpleActionItem("Flag", AddressOf _mainControl.btnFlag_Click)
            btnFlag.RootKey = _editTabKey
            btnFlag.LargeImage = My.Resources.Flag
            btnFlag.GroupCaption = editFunctionGroup
            App.HeaderControl.Add(btnFlag)

            btnAddNewPoint = New SimpleActionItem("Add Point", AddressOf _mainControl.btnAddNewPoint_Click)
            btnAddNewPoint.RootKey = _editTabKey
            btnAddNewPoint.LargeImage = My.Resources.Add
            btnAddNewPoint.GroupCaption = editFunctionGroup
            App.HeaderControl.Add(btnAddNewPoint)

            btnDeletePoint = New SimpleActionItem("Delete Point", AddressOf _mainControl.btnDeletePoint_Click)
            btnDeletePoint.RootKey = _editTabKey
            btnDeletePoint.LargeImage = My.Resources.Delete
            btnDeletePoint.GroupCaption = editFunctionGroup
            App.HeaderControl.Add(btnDeletePoint)

            'Main Function Panel
            Dim plotFunctionGroup As String = "Plot Function"

            ckbShowLegend = New SimpleActionItem("Show Legend", AddressOf _mainControl.ckbShowLegend_Click)
            ckbShowLegend.RootKey = _editTabKey
            ckbShowLegend.LargeImage = My.Resources.Legend
            ckbShowLegend.GroupCaption = plotFunctionGroup
            ckbShowLegend.ToggleGroupKey = "kEditLegend"
            App.HeaderControl.Add(ckbShowLegend)

            _mainControl.ShowLegend = False

            'disable buttons by default
            btnAddNewPoint.Enabled = False
            btnApplyToDatabase.Enabled = False
            btnChangeYValue.Enabled = False
            btnDeletePoint.Enabled = False
            btnFlag.Enabled = False
            btnInterpolate.Enabled = False
            btnRestoreData.Enabled = False

            MyBase.Activate()

        End Sub

        Public Overrides Sub Deactivate()

            App.HeaderControl.RemoveItems()

            MyBase.Deactivate()

        End Sub

        Private Sub LeavingEditView()
            _mainControl.pTimeSeriesPlot.Clear()
            _mainControl.dgvDataValues.DataSource = Nothing
            _mainControl.Editing = False
            _mainControl.Canceled = False
            _mainControl.newseriesID = 0
            _mainControl.gboxDataFilter.Enabled = False
            btnAddNewPoint.Enabled = False
            btnApplyToDatabase.Enabled = False
            btnChangeYValue.Enabled = False
            btnDeletePoint.Enabled = False
            btnFlag.Enabled = False
            btnInterpolate.Enabled = False
            btnRestoreData.Enabled = False


            'TODO allow changing of button caption
            btnSelectSeries.Caption = "Edit Series"
        End Sub

#End Region

#Region "Event Handlers"

        Sub Ribbon_ActiveChanged()

            Dim myTab As RibbonTab = App.Ribbon.Tabs.Find(Function(t) t.Text = _pluginName)
            Dim homeTab As RibbonTab = App.Ribbon.Tabs.Find(Function(t) t.Text = "Home")

            If myTab.Active Then
                'TabManager.SelectedTabName = "Series View"
                _seriesView.VisiblePanelName = _pluginName
            ElseIf homeTab.Active Then
                'TabManager.SelectedTabName = "Map View"
            Else
                LeavingEditView()
            End If



            'If Not _mainControl.Editing Then
            '    If Not _seriesView Is Nothing Then
            '        _seriesView.VisiblePanelName = _pluginName
            '    End If
            'Else
            '    _mapArgs.PanelManager.SelectedTabName = "MapView"
            '    If _mainControl.Canceled Then
            '        _mainControl.Canceled = False
            '        _mapArgs.PanelManager.SelectedTabName = "Series View"
            '    Else
            '        LeavingEditView()
            '    End If
            'End If

        End Sub

        'old code from Ribbon_ActiveChanged..
        'If Not _mainControl.Editing Then
        '    If _EditView.Active Then
        '        _mapArgs.PanelManager.SelectedTabName = _pluginName
        '        _mainControl.RefreshSelection()
        '    Else
        '        _mapArgs.PanelManager.SelectedTabName = "MapView"
        '        LeavingEditView()
        '    End If
        'Else
        '    If Not _EditView.Active Then
        '        _mapArgs.PanelManager.SelectedTabName = "MapView"
        '        If _mainControl.Canceled Then
        '            _mainControl.Canceled = False
        '            _mapArgs.Ribbon.ActiveTab = _EditView

        '            '_mainControl.pTimeSeriesPlot.Clear()
        '            '_mapArgs.Ribbon.ActiveTab = _EditView
        '            'With _mainControl.pTimeSeriesPlot.zgTimeSeries.GraphPane
        '            '    For i As Integer = 0 To .CurveList.Count - 1
        '            '        If _mainControl.newseriesID = _mainControl.pTimeSeriesPlot.CurveID(i) Then
        '            '            _mainControl.pTimeSeriesPlot.EnterEditMode(i)
        '            '        End If
        '            '    Next
        '            'End With
        '        Else
        '            LeavingEditView()
        '        End If
        '    End If
        'End If

        Sub btnSelectSeries_Click()
            If Not _mainControl.Editing Then
                If Not _seriesView.SeriesSelector.SelectedSeriesID = 0 Then
                    _mainControl.btnSelectSeries_Click()

                    'TODO : allow to change button caption
                    btnSelectSeries.Caption = "Stop Editing"
                    _mainControl.gboxDataFilter.Enabled = True

                    btnAddNewPoint.Enabled = True
                    btnApplyToDatabase.Enabled = True
                    btnChangeYValue.Enabled = True
                    btnDeletePoint.Enabled = True
                    btnFlag.Enabled = True
                    btnInterpolate.Enabled = True
                    btnRestoreData.Enabled = True
                End If
            Else
                If _mainControl.Editing Then
                    Dim result As Integer

                    result = MsgBox("You are editing a series. Do you want to save your edits?", MsgBoxStyle.YesNoCancel, "Save?")
                    If result = MsgBoxResult.Yes Then
                        _mainControl.SaveGraphChangesToDatabase()
                    End If
                End If

                btnAddNewPoint.Enabled = False
                btnApplyToDatabase.Enabled = False
                btnChangeYValue.Enabled = False
                btnDeletePoint.Enabled = False
                btnFlag.Enabled = False
                btnInterpolate.Enabled = False
                btnRestoreData.Enabled = False
                _mainControl.gboxDataFilter.Enabled = False

                'Change caption back to Edit Series (TODO support changing of caption)
                btnSelectSeries.Caption = "Edit Series"

                _mainControl.Editing = False
                _mainControl.newseriesID = 0
                _mainControl.pTimeSeriesPlot.ClearEditMode()
                _mainControl.dgvDataValues.DataSource = Nothing
            End If
        End Sub

        'Sub ckbShowLegend_CheckBoxCheckChanged()
        '    'TODO support check box
        '    'If ckbShowLegend.Checked Then
        '    '    _mainControl.ShowLegend = True
        '    'Else
        '    '    _mainControl.ShowLegend = False
        '    'End If
        '    _mainControl.ckbShowLegend_CheckedChanged()
        'End Sub

        Private Sub SerializationManager_Deserializing(ByVal sender As Object, ByVal e As SerializingEventArgs)
            _mainControl.RefreshSelection()
        End Sub

#End Region

    End Class
End Namespace
