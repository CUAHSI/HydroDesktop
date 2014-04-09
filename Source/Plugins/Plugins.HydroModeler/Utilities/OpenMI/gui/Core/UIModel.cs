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
using DotSpatial.Controls;
using DotSpatial.Modeling;
using DotSpatial.Modeling.Forms;
using System.Text.RegularExpressions;

namespace Oatc.OpenMI.Gui.Core
{
	/// <summary>
	/// Summary description for Model.
	/// </summary>
	public class UIModel
	{
		private string _omiFilename;
	
		//private Font _font;

		/// <summary>
		/// <c>true</c> if user is moving the model rectangle on the screen
		/// </summary>
		private bool _isMoving;

		private ILinkableComponent _linkableComponent;

		private string _modelID;

		private Pen _rectanglePen;

        private Color _color = Color.LightGreen;
        private double _highlight = 1;
        private int _width = 100;
        private int _height = 100;
        private Font _font = SystemFonts.MessageBoxFont;
        private ModelShape _shape = ModelShape.Rectangle;



        /// <summary>
        /// Gets or set the base color of the shapes gradient
        /// </summary>
        public Color Color
        {
            get { return _color; }
            set { _color = value; }
        }

        /// <summary>
        /// Returns 1 if the object is not highlighted less than 1 if it is highlighted
        /// </summary>
        public double Highlight
        {
            get { return _highlight; }
            set { _highlight = value; }
        }

        /// <summary>
        /// Gets or sets the width of the element
        /// </summary>
        public int Width
        {
            get { return _width; }
            set { _width = value; }
        }

        /// <summary>
        /// Gets or sets the shape of the element
        /// </summary>
        public int Height
        {
            get { return _height; }
            set { _height = value; }
        }
        /// <summary>
        /// Gets or sets the font used to draw the text on the element
        /// </summary>
        public Font Font
        {
            get { return _font; }
            set { _font = value; }
        }

        public ModelShape Shape
        {
            get { return _shape; }
            set { _shape = value; }
        }

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

			Rect = new Rectangle(100,100,50,3*_font.Height);	

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

        /// <summary>
        /// Draws model elements, using the DotSpatial approach
        /// </summary>
        /// <param name="dispacement">X,Y point</param>
        /// <param name="graph">graphics</param>
        /// <param name="Shape">Shape: can be rectangle, ellipse, or triangle</param>
        public void Draw(Point displacement, Graphics graph, DotSpatial.Modeling.Forms.ModelShape Shape)
        {

            Rectangle rectToDraw = Rect;

            //Get x,y
            Point topLeft = new Point();
            topLeft.X = Rect.X;
            topLeft.Y = Rect.Y;
            
            //Sets up the colors to use
            Pen outlinePen = new Pen(DotSpatial.Symbology.SymbologyGlobal.ColorFromHsl(Color.GetHue(), Color.GetSaturation(), Color.GetBrightness() * 0.6 * Highlight), 1.0F);
            Color gradientTop = DotSpatial.Symbology.SymbologyGlobal.ColorFromHsl(Color.GetHue(), Color.GetSaturation(), Color.GetBrightness() * 0.7 * Highlight);
            Color gradientBottom = DotSpatial.Symbology.SymbologyGlobal.ColorFromHsl(Color.GetHue(), Color.GetSaturation(), Color.GetBrightness() * 1.0 * Highlight);

            //The path used for drop shadows
            GraphicsPath shadowPath = new GraphicsPath();
            ColorBlend colorBlend = new ColorBlend(3);
            colorBlend.Colors = new Color[] { Color.Transparent, Color.FromArgb(180, Color.DarkGray), Color.FromArgb(180, Color.DimGray) };
            colorBlend.Positions = new float[] { 0f, 0.125f,1f};

            RectangleF textRect = new RectangleF(0,0,0,0);

            //stores temporary model name
            string modelID = _modelID;

            #region Draw Ellipse Object
            //Draws Ellipse Shapes
            if (Shape == ModelShape.Ellipse)
            {

                //Adjust size of ellipse to fit text
                SizeF textSize = graph.MeasureString(_modelID, _font);
                
                if (!_modelID.Contains(" "))
                {

                    string newID = null;
                    //try splitting model based on Capital Letters
                    if (Regex.IsMatch(_modelID.Substring(1,_modelID.Length-1), "[A-Z]"))
                    {
                        for (int i = 0; i <= _modelID.Length - 1; i++)
                        {
                            if (Char.IsUpper(_modelID[i]) && i > 0)
                            {
                                newID += " " + _modelID[i];
                            }
                            else
                                newID += _modelID[i];
                        }
                        modelID = newID;           //set the model id equal to the new id
                    }
                    else   //clip the modelID to fit within the ellipse
                    {
                        if (_modelID.Length < 10)
                            newID = _modelID;
                        else if(_modelID.Length<=20)
                            newID = _modelID.Substring(0, 10) + " " + _modelID.Substring(9, _modelID.Length - 9);
                        else
                            newID = _modelID.Substring(0, 10) + " " + _modelID.Substring(9, 10);
                        modelID = newID;//set the model id equal to the new id
                    }
                    

                    //_modelID = _modelID.Substring(0, 20);
                    //while ((textSize.Width > Rect.Width) || (textSize.Height > Rect.Height))
                    //{
                    //    Rect.Width++;
                    //    Rect.Height++;
                    //}
                }
                textRect = new RectangleF(topLeft.X+1, topLeft.Y + (Rect.Height - textSize.Height) / 2, Rect.Width, textSize.Height);


                //Draws the shadow
                shadowPath.AddEllipse(topLeft.X, topLeft.Y, Rect.Width + 7, Rect.Height + 7);
                PathGradientBrush shadowBrush = new PathGradientBrush(shadowPath);
                shadowBrush.WrapMode = WrapMode.Clamp;
                shadowBrush.InterpolationColors = colorBlend;
                graph.FillPath(shadowBrush, shadowPath);

                //Draws the Ellipse
                Rectangle fillArea = new Rectangle(topLeft.X, topLeft.Y, Rect.Width, Rect.Height);
                LinearGradientBrush myBrush = new LinearGradientBrush(fillArea, gradientBottom, gradientTop, LinearGradientMode.Vertical);
                graph.FillEllipse(myBrush, topLeft.X, topLeft.Y, Rect.Width, Rect.Height);
                graph.SmoothingMode = SmoothingMode.AntiAlias;
                //graph.PixelOffsetMode = PixelOffsetMode.HighQuality;
                graph.DrawEllipse(outlinePen, topLeft.X, topLeft.Y, Rect.Width, Rect.Height);

                //update model rectangle
                Rect.Height = Convert.ToInt32(Math.Max(textRect.Height, Rect.Height));
                Rect.Width = Convert.ToInt32(Math.Max(textRect.Width, Rect.Width));
                Rect.X = topLeft.X;
                Rect.Y = topLeft.Y;
                
                Rectangle TextRectangle = Rectangle.Ceiling(textRect);
                TextRectangle.Width += 2;

                StringFormat strFormat = new StringFormat();
                strFormat.Alignment = StringAlignment.Center;
                strFormat.LineAlignment = StringAlignment.Center;
                graph.DrawString(modelID, Font, new SolidBrush(Color.FromArgb(250, Color.Black)), Rect,strFormat);

                //Garbage collection
                myBrush.Dispose();
            }
            #endregion

            #region Draw Triangular Object
            //Draws Triangular Shapes
            if (Shape == ModelShape.Triangle)
            {
                //Draws the shadow
                Point[] ptShadow = new Point[4];
                ptShadow[0] = new Point(topLeft.X + (Rect.Width / 2), topLeft.Y - 5);
                ptShadow[1] = new Point(topLeft.X + Rect.Width + 7, topLeft.Y + (Rect.Height / 2));
                ptShadow[2] = new Point(topLeft.X + (Rect.Width / 2), topLeft.Y + Rect.Height + 5);
                ptShadow[3] = new Point(topLeft.X, topLeft.Y + (Rect.Height / 2));

                shadowPath.AddLines(ptShadow);
                PathGradientBrush shadowBrush = new PathGradientBrush(shadowPath);
                shadowBrush.WrapMode = WrapMode.Clamp;
                shadowBrush.InterpolationColors = colorBlend;
                graph.FillPath(shadowBrush, shadowPath);

                //Draws the shape
                Point[] pt = new Point[4];
                pt[0] = new Point(topLeft.X + (Rect.Width / 2), topLeft.Y);
                pt[1] = new Point(topLeft.X + Rect.Width, topLeft.Y + (Rect.Height / 2));
                pt[2] = new Point(topLeft.X + (Rect.Width / 2), topLeft.Y + Rect.Height);
                pt[3] = new Point(topLeft.X, topLeft.Y + (Rect.Height / 2));

                GraphicsPath myPath = new GraphicsPath();
                myPath.AddLines(pt);
                Rectangle fillArea = new Rectangle(topLeft.X - (Rect.Width/2), topLeft.Y, Rect.Width, Rect.Height);
                LinearGradientBrush myBrush = new LinearGradientBrush(fillArea, gradientBottom, gradientTop, LinearGradientMode.Vertical);
                graph.FillPath(myBrush, myPath);
                graph.DrawPath(outlinePen, myPath);

                //Draws the text
                SizeF textSize = graph.MeasureString("Trigger", Font, Rect.Width);

                //if ((textSize.Width > Rect.Width) || (textSize.Height > Rect.Height))
                //    textRect = new RectangleF(topLeft.X - (Rect.Width - textSize.Width) / 2, topLeft.Y + (Rect.Height - textSize.Height) / 2, textSize.Width, textSize.Height);
                //else
                textRect = new RectangleF(topLeft.X, topLeft.Y + (Rect.Height - textSize.Height) / 2, Rect.Width, textSize.Height);

                //Update model rectangle
                Rect.Height = Convert.ToInt32(Math.Max(textRect.Height, Rect.Height));
                Rect.Width = Convert.ToInt32(Math.Max(textRect.Width, Rect.Width));
                Rect.X = topLeft.X;
                Rect.Y = topLeft.Y;

                //draw text
                Rectangle TextRectangle = Rectangle.Ceiling(textRect);
                TextRectangle.Width += 2;
                System.Windows.Forms.TextFormatFlags flags = System.Windows.Forms.TextFormatFlags.HorizontalCenter |
                System.Windows.Forms.TextFormatFlags.VerticalCenter | System.Windows.Forms.TextFormatFlags.WordBreak;
                System.Windows.Forms.TextRenderer.DrawText(graph, "Trigger", Font, TextRectangle, Color.Black, flags);

                //Garbage collection
                myBrush.Dispose();
            }
            #endregion


            //Garbage collection
            shadowPath.Dispose();
            outlinePen.Dispose();
        }
        private static GraphicsPath GetRoundedRect(RectangleF baseRect, float radius)
        {
            if ((radius <= 0.0F) || radius >= ((Math.Min(baseRect.Width, baseRect.Height)) / 2.0))
            {
                GraphicsPath mPath = new GraphicsPath();
                mPath.AddRectangle(baseRect);
                mPath.CloseFigure();
                return mPath;
            }

            float diameter = radius * 2.0F;
            SizeF sizeF = new SizeF(diameter, diameter);
            RectangleF arc = new RectangleF(baseRect.Location, sizeF);
            GraphicsPath path = new GraphicsPath();

            // top left arc 
            path.AddArc(arc, 180, 90);

            // top right arc 
            arc.X = baseRect.Right - diameter;
            path.AddArc(arc, 270, 90);

            // bottom right arc 
            arc.Y = baseRect.Bottom - diameter;
            path.AddArc(arc, 0, 90);

            // bottom left arc
            arc.X = baseRect.Left;
            path.AddArc(arc, 90, 90);

            path.CloseFigure();
            return path;
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
