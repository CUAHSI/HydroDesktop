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
using System.Reflection;
using System.Collections;

namespace Oatc.OpenMI.Sdk.DevelopmentSupport
{
	/// <summary>
	/// Implementation of IAggregate which is based on reflection.
	/// This class is used by XmlFile as default
	/// <seealso cref="Oatc.OpenMI.Sdk.DevelopmentSupport.IAggregate"/>
	/// </summary>
	public class DefaultAggregate : IAggregate
	{
		private object _source = null;

		/// <summary>
		/// Constructor which gets the underlying source object
		/// </summary>
		/// <param name="source">The underlying source object</param>
		public DefaultAggregate(object source)
		{
			_source = source;
		}

		/// <summary>
		/// Gets the underlying source object
		/// </summary>
		public virtual object Source 
		{
			get {return _source;}
		}

		/// <summary>
		/// Gets a list of all properties defined in the class type of the source.
		/// Reflection is used to get this list.
		/// </summary>
		public virtual string[] Properties 
		{
			get 
			{
				PropertyInfo[] properties = _source.GetType().GetProperties();
				string[] propertyStrings = new string[properties.Length];
				for (int i = 0; i < propertyStrings.Length; i++) 
				{
					propertyStrings[i] = properties[i].Name;
				}
				return propertyStrings;
			}
		}

		/// <summary>
		/// Gets the class type of a property by reflection
		/// </summary>
		/// <param name="property">The property name</param>
		/// <returns>The type of the property</returns>
		public virtual Type GetType (string property) 
		{
			PropertyInfo prop = _source.GetType().GetProperty(property);
			if (prop != null) 
			{
				return prop.PropertyType;
			}

			return null;
		}

		/// <summary>
		/// Indicates whether a property can be written to. Reflection is used.
		/// </summary>
		/// <param name="property">The property name</param>
		/// <returns>Property is writable, false if property doesn't exist</returns>
		public virtual bool CanWrite (string property) 
		{
			PropertyInfo prop = _source.GetType().GetProperty(property);
			if (prop != null) 
			{
				return prop.CanWrite;
			}

			return false;
		}

		/// <summary>
		/// Indicates whether a property can be read. Reflection is used.
		/// </summary>
		/// <param name="property">The property name</param>
		/// <returns>Property is readable, false if property doesn't exist</returns>
		public virtual bool CanRead (string property) 
		{
			PropertyInfo prop = _source.GetType().GetProperty(property);
			if (prop != null) 
			{
				return prop.CanRead;
			}

			return false;
		}

		/// <summary>
		/// Gets a value for a certain property. Reflection is used.
		/// </summary>
		/// <param name="property">The property name</param>
		/// <returns>The property value, null if the property doesn't exist</returns>
		/// <exception cref="Exception">Internal exception raised by the source object when getting the value</exception>
		public virtual object GetValue (string property) 
		{
			PropertyInfo prop = _source.GetType().GetProperty(property);
			if (prop != null) 
			{
				try 
				{
					return prop.GetValue (_source, null);
				}
				catch (Exception e) 
				{
					throw new Exception ("Error when getting value of property " + property + " in " + _source.ToString() + "\n" + e.Message, e);
				}
			}

			return null;
		}

		/// <summary>
		/// Sets a value for a certain property. Reflection is used.
		/// </summary>
		/// <param name="property">The property name</param>
		/// <param name="target">The new property value</param>
		/// <exception cref="Exception">Internal exception raised by the source object when setting the value</exception>
		public virtual void SetValue (string property, object target) 
		{
			PropertyInfo prop = _source.GetType().GetProperty(property);
			if (prop != null) 
			{
				try 
				{
					prop.SetValue (_source, target, null);
				}
				catch (Exception e) 
				{
					throw new Exception ("Error when setting value " + target.ToString() + " to property " + property + " in " + _source.ToString() + "\n" + e.Message, e);
				}
			}
		}

		/// <summary>
		/// Gets a referenced value, i.e. a value corresponding with a reference string within the scope of the source.
		/// Implementation is delegated to XmlFile.GetRegisteredTarget.
		/// </summary>
		/// <param name="reference">Reference</param>
		/// <returns>The referenced object</returns>
		public virtual object GetReferencedValue (string reference) 
		{
			return XmlFile.GetRegisteredTarget(this.Source, reference);
		}

		/// <summary>
		/// Intended for updating the source after various SetValue calls.
		/// Takes no action, because all SetValue calls are delegated directly to the source object with reflection.
		/// </summary>
		public virtual void UpdateSource() 
		{
		}

		/// <summary>
		/// Intended for updating the aggregate before various GetValue calls.
		/// Takes no action, because all GetValue calls are delegated directly to the source object with reflection.
		/// </summary>
		public virtual void UpdateAggregate()
		{
		}
	}
}
