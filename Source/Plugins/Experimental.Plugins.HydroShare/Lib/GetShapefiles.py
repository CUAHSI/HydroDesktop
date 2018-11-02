# -*- coding: utf-8 -*- 

###########################################################################
## Python code generated with wxFormBuilder (version Aug 25 2009)
## http://www.wxformbuilder.org/
##
## PLEASE DO "NOT" EDIT THIS FILE!
###########################################################################

import wx

###########################################################################
## Class MyFrame1
###########################################################################

class MyFrame1 ( wx.Frame ):
	
	def __init__( self, parent ):
		wx.Frame.__init__  ( self, parent, id = wx.ID_ANY, title = u"Find Shapefile on HydroShare", pos = wx.DefaultPosition, size = wx.Size( 289,381 ), style = wx.DEFAULT_FRAME_STYLE|wx.TAB_TRAVERSAL )
		
		self.SetSizeHintsSz( wx.DefaultSize, wx.DefaultSize )
		
		bSizer1 = wx.BoxSizer( wx.VERTICAL )
		
		cmb_FilterSearchChoices = []
		self.cmb_FilterSearch = wx.ComboBox( self, wx.ID_ANY, u"Filter Search ...", wx.DefaultPosition, wx.DefaultSize, cmb_FilterSearchChoices, 0 )
		bSizer1.Add( self.cmb_FilterSearch, 0, wx.ALL|wx.EXPAND, 5 )
		
		lst_AvailableItemsChoices = []
		self.lst_AvailableItems = wx.ListBox( self, wx.ID_ANY, wx.DefaultPosition, wx.DefaultSize, lst_AvailableItemsChoices, wx.LB_MULTIPLE|wx.LB_NEEDED_SB )
		bSizer1.Add( self.lst_AvailableItems, 1, wx.ALL|wx.EXPAND, 5 )
		
		self.gag_ProgressBar = wx.Gauge( self, wx.ID_ANY, 100, wx.DefaultPosition, wx.DefaultSize, wx.GA_HORIZONTAL )
		bSizer1.Add( self.gag_ProgressBar, 0, wx.ALL|wx.EXPAND, 5 )
		
		bSizer3 = wx.BoxSizer( wx.HORIZONTAL )
		
		self.btn_cancel = wx.Button( self, wx.ID_ANY, u"Cancel", wx.DefaultPosition, wx.DefaultSize, 0 )
		bSizer3.Add( self.btn_cancel, 1, wx.ALL, 5 )
		
		self.btn_GetData = wx.Button( self, wx.ID_ANY, u"Get Data", wx.DefaultPosition, wx.DefaultSize, 0 )
		bSizer3.Add( self.btn_GetData, 1, wx.ALL, 5 )
		
		bSizer1.Add( bSizer3, 0, wx.EXPAND, 5 )
		
		self.SetSizer( bSizer1 )
		self.Layout()
		
		# Connect Events
		self.cmb_FilterSearch.Bind( wx.EVT_COMBOBOX, self.clk_FilterSearch )
		self.btn_cancel.Bind( wx.EVT_BUTTON, self.clk_Cancel )
		self.btn_GetData.Bind( wx.EVT_BUTTON, self.clk_GetData )
	
	def __del__( self ):
		pass
	
	
	# Virtual event handlers, overide them in your derived class

	def clk_FilterSearch( self, event ):
		event.Skip()
	
	def clk_Cancel( self, event ):
		event.Skip()
	
	def clk_GetData( self, event ):
		event.Skip()
	

