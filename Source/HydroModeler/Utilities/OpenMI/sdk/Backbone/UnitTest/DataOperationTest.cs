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
using NUnit.Framework;
using OpenMI.Standard;
using Oatc.OpenMI.Sdk.Backbone;

namespace Oatc.OpenMI.Sdk.Backbone.UnitTest
{
	[TestFixture]
	public class DataOperationTest
	{
		public DataOperationTest()
		{
		}

		[Test]
		public void Constructor() 
		{
			DataOperation operation = new DataOperation("DataOperationID");
			Argument param1 = new Argument("key1","value1",true,"argument1");
			Argument param2 = new Argument("key2","value2",true,"argument2");
			Argument param3 = new Argument("key3","value3",true,"argument3");
			operation.AddArgument(param1);
			operation.AddArgument(param2);
			operation.AddArgument(param3);
			Assert.AreEqual("DataOperationID",operation.ID);
			
			DataOperation operation2 = new DataOperation(operation);

			Assert.AreEqual(operation,operation2);

		}

		[Test]
		public void ID()
		{	DataOperation operation = new DataOperation();
			operation.ID = "OperationID";
			Assert.AreEqual("OperationID",operation.ID);
		}

		[Test]
		public void Arguments()
		{
			Argument param1 = new Argument("key1","value1",true,"argument1");
			Argument param2 = new Argument("key2","value2",true,"argument2");
			Argument param3 = new Argument("key3","value3",true,"argument3");

			DataOperation operation = new DataOperation();
			operation.AddArgument(param1);
			operation.AddArgument(param2);
			operation.AddArgument(param3);

			Assert.AreEqual(3,operation.ArgumentCount);
			Assert.AreEqual(param1,operation.GetArgument(0));
			Assert.AreEqual(param2,operation.GetArgument(1));
			Assert.AreEqual(param3,operation.GetArgument(2));
		}

		[Test]
		public void Equals()
		{
			Argument param1 = new Argument("key1","value1",true,"argument1");
			Argument param2 = new Argument("key2","value2",true,"argument2");
			Argument param3 = new Argument("key3","value3",true,"argument3");

			DataOperation operation1 = new DataOperation("ID");

			operation1.AddArgument(param1);
			operation1.AddArgument(param2);
			operation1.AddArgument(param3);

			param1 = new Argument("key1","value1",true,"argument1");
			param2 = new Argument("key2","value2",true,"argument2");
			param3 = new Argument("key3","value3",true,"argument3");

			DataOperation operation2 = new DataOperation("ID");
			operation2.AddArgument(param1);
			operation2.AddArgument(param2);

			Assert.IsFalse(operation1.Equals(operation2));

			operation2.AddArgument(param3);

			Assert.IsTrue(operation1.Equals(operation2));

			param1.Key="key";

			Assert.IsFalse(operation1.Equals(operation2));

			param1.Key="key1";

			operation1.ID = "ID1";

			Assert.IsFalse(operation1.Equals(operation2));

			operation1.ID = "ID";

			Assert.IsFalse(operation1.Equals(null));
			Assert.IsFalse(operation1.Equals("string"));
		}
	}
}
