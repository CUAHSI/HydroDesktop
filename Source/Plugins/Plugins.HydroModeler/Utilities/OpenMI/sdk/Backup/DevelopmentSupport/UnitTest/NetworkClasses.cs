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
using System.ComponentModel;
using System.Collections;

namespace Oatc.OpenMI.Sdk.DevelopmentSupport.UnitTest
{
	/// <summary>
	/// Summary description for Element.
	/// </summary>
	public class Element
	{
		private string _name;
		private string _id;

		public Element()
		{
		}

		public string ID 
		{
			get {return _id;}
			set {_id = value;}
		}

		public string Name 
		{
			get {return _name;}
			set {_name = value;}
		}
	}

	public class Location
	{
		private double m_x = 0;
		private double m_y = 0;

		public Location()
		{
		}

		public override bool Equals(object obj)
		{
			if (obj is Location) 
			{
				Location loc = (Location) obj;
				return ((loc.X == X) && (loc.Y == Y));
			}

			return base.Equals (obj);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode ();
		}

		public Location(double NewX, double NewY)
		{
			X = NewX;
			Y = NewY;
		}

		[Category("Location"),
		Description("Meters East-West")]
		public double X
		{
			get { return m_x; }
			set	{ m_x = value; }
		}

		[Category("Location"),
		Description("Meters North-South")]
		public double Y
		{
			get	{ return m_y; }
			set	{ m_y = value; }
		}

		public override String ToString() 
		{
			return "(" + X + "," + Y + ")";
		}
	}

	public class Node : Element
	{
		private Location m_location = new Location();
		private double m_height;
		private double m_volume;

		public Node()
		{
		}

		public Node(String NewName, Location NewLocation, double NewHeight, double NewVolume)
		{
			Name = NewName;
			Location = NewLocation;
			Height = NewHeight;
			Volume = NewVolume;
		}

		[Category("Location"),
		Description("Height above sea level")]
		public double Height
		{
			get	{ return m_height; }
			set	{ m_height = value; }
		}

		[Category("Structure"),
		Description("Containing water volume")]
			//		EditorAttribute (typeof(System.Windows.Forms.NumericUpDown), typeof(System.Drawing.Design.UITypeEditor))]
		public double Volume 
		{
			get	{ return m_volume; }
			set	{ m_volume = value; }
		}

		[Category("Location"),
		Description("Meters from center of network")]
		public Location Location
		{
			get	{ return m_location; }
			set	{ m_location = value; }
		}
	}

	public class Branch : Element
	{
		private Node m_begin;
		private Node m_end;
		private double m_length;
		private double m_width;

		public Branch()
		{
		}

		public Branch(String NewName, Node NewBegin, Node NewEnd, double NewLength, double NewWidth)
		{
			Name = NewName;
			BeginNode = NewBegin;
			EndNode = NewEnd;
			Length = NewLength;
			Width = NewWidth;
		}

		[Category("Location")]
		public Node BeginNode
		{
			get	{ return m_begin; }
			set	{ m_begin = value; }
		}

		[Category("Location")]
		public Node EndNode
		{
			get	{ return m_end; }
			set	{ m_end = value; }
		}

		[Category("Structure")]
		public double Length
		{
			get	{ return m_length; }
			set	{ m_length = value; }
		}

		[Category("Structure")]
		public double Width
		{
			get	{ return m_width; }
			set	{ m_width = value; }
		}
	}

	public class Network : Element
	{
		ArrayList mNodes = new ArrayList();
		ArrayList mBranches = new ArrayList();

		DateTime _lastModificationTime;

		public Network()
		{
		}

		public ArrayList Nodes
		{
			get
			{
				return mNodes;
			}
		}
		
		public ArrayList Branches
		{
			get
			{
				return mBranches;
			}
		}

		public DateTime LastModificationTime 
		{
			get {return _lastModificationTime;}
			set {_lastModificationTime = value;}
		}

		public static Network GetSampleNetwork() 
		{
			Network network = new Network();
			network.Name = "Rhine";
			network.LastModificationTime = DateTime.Now;

			Node node1 = new Node("Node1", new Location (35, 53), 30.4, 18000);
			Node node2 = new Node("Node2", new Location (23, 18), 26.4, 14000);
			Node node3 = new Node("Node3", new Location (67, 63), 12.6, 12000);
			Node node4 = new Node("Node4", new Location (12, 23), 0.3, 1500);
			Node node5 = new Node("Node5", new Location (14, 34), 9.6, 2000);

			Branch branch1 = new Branch ("Nederrijn", node1, node2, 10, 80);
			Branch branch2 = new Branch ("IJssel", node2, node3, 60, 30);
			Branch branch3 = new Branch ("Rijn", node2, node4, 100, 40);
			Branch branch4 = new Branch ("Nederrijn", node1, node5, 110, 50);

			network.Nodes.Add (node1);
			network.Nodes.Add (node2);
			network.Nodes.Add (node3);
			network.Nodes.Add (node4);
			network.Nodes.Add (node5);

			network.Branches.Add (branch1);
			network.Branches.Add (branch2);
			network.Branches.Add (branch3);
			network.Branches.Add (branch4);

			return network;
		}
	}

}
