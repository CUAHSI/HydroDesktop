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
	public class ArgumentTest
	{
		public ArgumentTest()
		{
		}

		[Test]
		public void Constructor()
		{
			Argument param = new Argument("key","value",true,"argument1");
			Assert.AreEqual("key",param.Key);
			Assert.AreEqual("value",param.Value);

			Argument param2 = new Argument(param);
			Assert.AreEqual(param,param2);
		}

		[Test]
		public void Key()
		{
			Argument param = new Argument();
			param.Key = "OperationKey";
			Assert.AreEqual("OperationKey",param.Key);
		}

		[Test]
		public void Value()
		{
			Argument param = new Argument();
			param.Value = "OperationValue";
			Assert.AreEqual("OperationValue",param.Value);
		}

		[Test]
		public void ReadOnly()
		{
			Argument param = new Argument();
			param.ReadOnly = true;
			Assert.AreEqual(true,param.ReadOnly);
			param.ReadOnly = false;
			Assert.AreEqual(false,param.ReadOnly);
		}

		[Test]
		public void Description()
		{
			Argument param = new Argument();
			param.Description = "Description";
			Assert.AreEqual("Description",param.Description);
		}

		[Test]
		public void Equals()
		{
			Argument param1 = new Argument("key","value",true,"argument1");
			Argument param2 = new Argument("key","value",true,"argument2");

			Assert.IsTrue(param1.Equals(param2));
			param1.Key = "key1";
			Assert.IsFalse(param1.Equals(param2));
			param1.Key = "key";
			param1.Value ="value1";
			Assert.IsFalse(param1.Equals(param2));

			Assert.IsFalse(param1.Equals(null));
			Assert.IsFalse(param1.Equals("string"));
		}

	}
}
