Imports System.Windows.Forms
Imports DotSpatial.Controls
Imports HydroDesktop.Interfaces
Imports DotSpatial.Controls.Header
Imports System.ComponentModel.Composition
Imports DotSpatial.Controls.Docking


Namespace EditView

    Public Class EditViewPlugin
        Inherits Extension

#Region "Variables"

        <Import("SeriesControl", GetType(ISeriesSelector))>
        Private _seriesSelector As ISeriesSelector

        Private ignoreRootSelected As Boolean = False

        Private _mainControl As cEditView

        Private Const _pluginName As String = "Edit"
        'Private Const kEditView As String = "kHydroEditView"
        Private Const kEditView As String = "kHydroEditView"

        Private _EditView As RootItem

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
            'Adding the ribbon tab
            _EditView = New RootItem(kEditView, _pluginName)
            _EditView.SortOrder = 40
            App.HeaderControl.Add(_EditView)

            '**************************************************************************************

            'To add a new edit view dockable panel to the main application window
            'watch for dock panel added event
            'If firstTimeLoading Then
            '    AddHandler App.DockManager.PanelAdded, AddressOf DockPanelAdded
            'Else

            'End If


            _mainControl = New cEditView(_seriesSelector)

            _mainControl.Dock = DockStyle.Fill

            Dim dp As New DockablePanel(kEditView, _pluginName, _mainControl, DockStyle.Fill)
            dp.DefaultSortOrder = 30
            App.DockManager.Add(dp)

            'adding the Edit Tab
            AddHandler App.HeaderControl.RootItemSelected, AddressOf HeaderControl_RootItemSelected

            'when the edit view panel is activated, select the Edit ribbon tab
            AddHandler App.DockManager.ActivePanelChanged, AddressOf DockManager_ActivePanelChanged

            InitializeRibbonButtons()
        End Sub

        Sub HeaderControl_RootItemSelected(ByVal sender As Object, ByVal e As RootItemEventArgs)

            If ignoreRootSelected Then Return
            If e.SelectedRootKey = kEditView Then
                App.DockManager.SelectPanel(kEditView)
            End If

        End Sub


        'Sub DockPanelAdded(ByVal sender As Object, ByVal args As Docking.DockablePanelEventArgs)
        '    If Not firstTimeLoading Then Exit Sub

        '    If args.ActivePanelKey = "kMap" Then
        '        App.DockManager.Add(New DockablePanel(kEditView, _pluginName, _mainControl, DockStyle.Fill))
        '    End If
        '    App.DockManager.SelectPanel("kMap")
        'End Sub


        Private Sub InitializeRibbonButtons()

            'Main Function Panel
            Dim mainFunctionGroup As String = "Main Functions"

            btnSelectSeries = New SimpleActionItem("Edit Series", AddressOf btnSelectSeries_Click)
            btnSelectSeries.RootKey = kEditView
            btnSelectSeries.LargeImage = My.Resources.Edit
            btnSelectSeries.GroupCaption = mainFunctionGroup
            App.HeaderControl.Add(btnSelectSeries)

            btnDeriveNewDataSeries = New SimpleActionItem("Derive Series", AddressOf _mainControl.btnDeriveNewDataSeries_Click)
            btnDeriveNewDataSeries.RootKey = kEditView
            btnDeriveNewDataSeries.LargeImage = My.Resources.DeriveNewSeries
            btnDeriveNewDataSeries.GroupCaption = mainFunctionGroup
            App.HeaderControl.Add(btnDeriveNewDataSeries)

            btnRestoreData = New SimpleActionItem("Restore Data", AddressOf _mainControl.btnRestoreData_Click)
            btnRestoreData.RootKey = kEditView
            btnRestoreData.LargeImage = My.Resources.Restore
            btnRestoreData.GroupCaption = mainFunctionGroup
            App.HeaderControl.Add(btnRestoreData)

            btnApplyToDatabase = New SimpleActionItem("Save To Database", AddressOf _mainControl.btnApplyToDatabase_Click)
            btnApplyToDatabase.RootKey = kEditView
            btnApplyToDatabase.LargeImage = My.Resources.Save
            btnApplyToDatabase.GroupCaption = mainFunctionGroup
            App.HeaderControl.Add(btnApplyToDatabase)

            'Plot Function Panel
            Dim editFunctionGroup As String = "Edit Functions"

            btnChangeYValue = New SimpleActionItem("Change Value", AddressOf _mainControl.btnChangeYValue_Click)
            btnChangeYValue.RootKey = kEditView
            btnChangeYValue.LargeImage = My.Resources.ChangeValue
            btnChangeYValue.GroupCaption = editFunctionGroup
            App.HeaderControl.Add(btnChangeYValue)

            btnInterpolate = New SimpleActionItem("Interpolate", AddressOf _mainControl.btnInterpolate_Click)
            btnInterpolate.RootKey = kEditView
            btnInterpolate.LargeImage = My.Resources.Interpolate
            btnInterpolate.GroupCaption = editFunctionGroup
            App.HeaderControl.Add(btnInterpolate)

            btnFlag = New SimpleActionItem("Flag", AddressOf _mainControl.btnFlag_Click)
            btnFlag.RootKey = kEditView
            btnFlag.LargeImage = My.Resources.Flag
            btnFlag.GroupCaption = editFunctionGroup
            App.HeaderControl.Add(btnFlag)

            btnAddNewPoint = New SimpleActionItem("Add Point", AddressOf _mainControl.btnAddNewPoint_Click)
            btnAddNewPoint.RootKey = kEditView
            btnAddNewPoint.LargeImage = My.Resources.Add
            btnAddNewPoint.GroupCaption = editFunctionGroup
            App.HeaderControl.Add(btnAddNewPoint)

            btnDeletePoint = New SimpleActionItem("Delete Point", AddressOf _mainControl.btnDeletePoint_Click)
            btnDeletePoint.RootKey = kEditView
            btnDeletePoint.LargeImage = My.Resources.Delete
            btnDeletePoint.GroupCaption = editFunctionGroup
            App.HeaderControl.Add(btnDeletePoint)

            'Main Function Panel
            Dim plotFunctionGroup As String = "Plot Function"

            ckbShowLegend = New SimpleActionItem("Show Legend", AddressOf _mainControl.ckbShowLegend_Click)
            ckbShowLegend.RootKey = kEditView
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

            App.HeaderControl.RemoveAll()
            App.DockManager.Remove(kEditView)
            _mainControl = Nothing

            'Remove event handlers
            RemoveHandler App.HeaderControl.RootItemSelected, AddressOf HeaderControl_RootItemSelected
            RemoveHandler App.DockManager.ActivePanelChanged, AddressOf DockManager_ActivePanelChanged

            MyBase.Deactivate()

        End Sub

#End Region

#Region "Event Handlers"

        Sub DockManager_ActivePanelChanged(ByVal sender As Object, ByVal e As Docking.DockablePanelEventArgs)

            'activate the Edit ribbon tab and the series view panel
            If e.ActivePanelKey = kEditView Then
                ignoreRootSelected = True
                App.HeaderControl.SelectRoot(kEditView)
                ignoreRootSelected = False
                App.DockManager.SelectPanel("kHydroSeriesView")
            End If
            '_EditView.Visible = True
            'ElseIf e.ActivePanelKey <> "kHydroSeriesView" Then
            '    _EditView.Visible = False
            'End If

        End Sub

        Sub btnSelectSeries_Click()
            If Not _mainControl.Editing Then
                If Not _seriesSelector.SelectedSeriesID = 0 Then
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
                _mainControl.lblstatus.Text = String.Empty
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

#End Region

    End Class
End Namespace
