#region Copyright
/*
* Copyright (c) 2005,2006,2007, OpenMI Association
* All rights reserved.
*
* Redistribution and use in source and binary forms, with or without
* modification, are permitted provided that the following conditions are met:
*     * Redistributions of source code must retain the above copyright
*       notice, this list of conditions and the following disclaimer.
*     * Redistributions in binary form must reproduce the above copyright
*       notice, this list of conditions and the following disclaimer in the
*       documentation and/or other materials provided with the distribution.
*     * Neither the name of the OpenMI Association nor the
*       names of its contributors may be used to endorse or promote products
*       derived from this software without specific prior written permission.
*
* THIS SOFTWARE IS PROVIDED BY "OpenMI Association" ``AS IS'' AND ANY
* EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
* WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
* DISCLAIMED. IN NO EVENT SHALL "OpenMI Association" BE LIABLE FOR ANY
* DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
* (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
* LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
* ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
* (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
* SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/
#endregion

using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Xml;
using Oatc.OpenMI.Sdk.Backbone;
using OpenMI.Standard;

namespace Oatc.OpenMI.Gui.Core
{
	/// <summary>
	/// Summary description for Model.
	/// </summary>
	public class UIModel
	{
		private string _omiFilename;
	
		private Font _font;

		/// <summary>
		/// <c>true</c> if user is moving the model rectangle on the screen
		/// </summary>
		private bool _isMoving;

		private ILinkableComponent _linkableComponent;

		private string _modelID;

		private Pen _rectanglePen;
		
		/// <summary>
		/// Creates a new instance of <see cref="UIModel">UIModel</see> class.
		/// </summary>
		public UIModel()
		{
			_isMoving = false;

			//  Workarround to handle a bug from Microsoft
			//  A bug, see http://dturini.blogspot.com/2004_08_01_dturini_archive.html
			//  or  http://support.microsoft.com/default.aspx?scid=KB;EN-US;q326219#appliesto
			try
			{
				_font = new Font("Arial", 11);
			}
			catch (ArithmeticException)
			{
				Utils.ResetFPU(); 
				_font = new Font("Arial", 11);
			} 

			Rect = new Rectangle(30,30,100,3*_font.Height);	

			_rectanglePen = new Pen( Color.Blue, 1 );
		}


		/// <summary>
		/// Creates a new instance of trigger model.
		/// </summary>
		/// <returns>Returns trigger model.</returns>
		/// <remarks>See <see cref="Trigger">Trigger</see> for more detail.</remarks>
		public static UIModel NewTrigger()
		{
			UIModel trigger = new UIModel();

			trigger.LinkableComponent = new Trigger();
			trigger.OmiFilename = CompositionManager.TriggerModelID;
			trigger._modelID = CompositionManager.TriggerModelID;

			return( trigger );
		}


		/// <summary>
		/// Gets or sets path to OMI file representing this model.
		/// </summary>
		/// <remarks>Setting of this property has only sense in case this model is trigger, see
		/// <see cref="NewTrigger">NewTrigger</see> method.</remarks>
		public string OmiFilename
		{
			get { return(_omiFilename); }
			set { _omiFilename = value; }
		}

		/// <summary>
		/// Gets ID of this model.
		/// </summary>
		/// <remarks>ID is equivalent to <see cref="ILinkableComponent.ModelID">ILinkableComponent.ModelID</see>.
		/// It must be unique in the composition.
		/// </remarks>
		public string ModelID
		{
			get { return(_modelID); }
		}

		private void MoveModel( Point offset )
		{
			Rect.Offset( offset );
		}

		/// <summary>
		/// Draws this model's rectangle into specified <see cref="Graphics">Graphics</see> object.
		/// </summary>
		/// <param name="displacement">Displacement of composition box in whole composition area.</param>
		/// <param name="g"><see cref="Graphics">Graphics</see> where rectangle should be drawn.</param>
		public void Draw(Point displacement, Graphics g)
		{
			Rectangle rectToDraw = Rect;
			rectToDraw.X -= displacement.X;
			rectToDraw.Y -= displacement.Y;

			Region fillRegion = new Region(rectToDraw);	
		
			g.FillRegion( GetFillBrush(), fillRegion );
			g.DrawRectangle( _rectanglePen, rectToDraw);
			g.DrawString( _modelID, _font, Brushes.Black,rectToDraw);
		}

		private Brush GetFillBrush()
		{			
			if( _modelID == CompositionManager.TriggerModelID )
			{
				// trigger has different color
				if( _isMoving )
					return( new SolidBrush(Color.SteelBlue) );
				else
					return( new SolidBrush(Color.SkyBlue) );
			}			

			if( _isMoving )
				return( new SolidBrush(Color.Goldenrod) );
			else
				return( new SolidBrush(Color.Yellow) );
		}

		/// <summary>
		/// Gets middle point of model's rectangle.
		/// </summary>
		/// <returns>Returns middle point of model's rectangle.</returns>
		public Point GetMidPoint()
		{
			return new Point( Rect.X + Rect.Width/2, Rect.Y + Rect.Height/2 );
		}

		/// <summary>
		/// Determines whether point is in model's rectangle.
		/// </summary>
		/// <param name="point">Point</param>
		/// <returns>Returns <c>true</c> if the point is in model's rectangle, otherwise returns <c>false</c>.</returns>
		public bool IsPointInside( Point point )
		{
			return( Rect.X < point.X
				&& (Rect.X + Rect.Width) > point.X
				&& Rect.Y < point.Y
				&& (Rect.Y + Rect.Height) > point.Y );
		}


		/// <summary>
		/// Gets or sets whether model's rectangle is currently moving.
		/// </summary>
		/// <remarks>
		/// It's useful for example to draw moving rectangles with different color.
		/// </remarks>
		public bool IsMoving
		{
			get	{ return _isMoving; }
			set { _isMoving = value; }
		}

		/// <summary>
		/// Model's rectangle.
		/// </summary>
		public Rectangle Rect;	

		/// <summary>
		/// Linkable component corresponding to this model.
		/// </summary>
		public ILinkableComponent LinkableComponent
		{
			get
			{
				return _linkableComponent;
			}
			set
			{
				_linkableComponent = value;
			}
		}


		/// <summary>
		/// Sets this model according to OMI file.
		/// </summary>
		/// <param name="filename">Relative or absolute path to OMI file describing the model.</param>
		/// <param name="relativeDirectory">Directory <c>filename</c> is relative to, or <c>null</c> if <c>filename</c> is absolute or relative to current directory.</param>
		/// <remarks>See <see cref="Utils.GetFileInfo">Utils.GetFileInfo</see> for more info about how
		/// specified file is searched.</remarks>	
		public void ReadOMIFile( string relativeDirectory, string filename )
		{
			// Open OMI file as xmlDocument
			FileInfo omiFileInfo = Utils.GetFileInfo( relativeDirectory, filename );
			if( !omiFileInfo.Exists )
				throw( new Exception("Omi file not found (CurrentDirectory='"+Directory.GetCurrentDirectory()+"', File='"+filename+"')") );

			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.Load( omiFileInfo.FullName );

			// get 1st LinkableComponent element
			XmlElement xmlLinkableComponent = null;
			foreach( XmlNode node in xmlDocument.ChildNodes )
				if( node.Name=="LinkableComponent" )
				{
					xmlLinkableComponent = (XmlElement)node;
					break;
				}

			// load assembly if present (from relative location of the OMI file)
			if (xmlLinkableComponent == null)
			{
				throw new Exception("No linkable components found in composition file " + omiFileInfo);
			}
			else
			{
				string assemblyFilename = xmlLinkableComponent.GetAttribute("Assembly");
				if (assemblyFilename != null)
					AssemblySupport.LoadAssembly(omiFileInfo.DirectoryName, assemblyFilename);
			}

			// read arguments
			ArrayList linkableComponentArguments = new ArrayList();

			foreach( XmlElement xmlArguments in xmlLinkableComponent.ChildNodes )
				if( xmlArguments.Name == "Arguments" )
					foreach( XmlElement xmlArgument in xmlArguments.ChildNodes )
						linkableComponentArguments.Add( new Argument(xmlArgument.GetAttribute("Key"), xmlArgument.GetAttribute("Value"), true, "No description"));

			// get new instance of ILinkableComponent type
			// for this moment set current directory to omi file's directory
			string oldDirectory = Directory.GetCurrentDirectory(); 
			try 
			{
				Directory.SetCurrentDirectory( omiFileInfo.DirectoryName );

				string classTypeName = xmlLinkableComponent.GetAttribute("Type");
				object obj = AssemblySupport.GetNewInstance( classTypeName );
				if ( ! ( obj is ILinkableComponent ) )
				{
					throw new Exception("\n\nThe class type " + classTypeName + " in\n" +
						filename +
						"\nis not an OpenMI.Standard.ILinkableComponent (OpenMI.Standard.dll version 1.4.0.0)." +
						"\nYou may have specified a wrong class name, " +
						"\nor the class implements the ILinkableComponent interface of a previous version of the OpenMI Standard.\n");
				}
				_linkableComponent = (ILinkableComponent)obj;
				_linkableComponent.Initialize( (IArgument[])linkableComponentArguments.ToArray(typeof(IArgument)) );
			}
			finally
			{
				Directory.SetCurrentDirectory( oldDirectory );
			}

			_omiFilename = omiFileInfo.FullName;
			
			_modelID = _linkableComponent.ModelID;

			// remote components have rectangle style
			string componentDescription = _linkableComponent.ComponentDescription;
			if( componentDescription != null )
				if( componentDescription.IndexOf( "OpenMI.Distributed" ) >= 0 )				
					_rectanglePen.DashStyle = DashStyle.Dash;
		}
		 

		

	}
}
