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
using System.IO;
using System.Collections;
using System.ComponentModel;
using System.Reflection;

namespace Oatc.OpenMI.Sdk.DevelopmentSupport
{
	/// <summary>
	/// This class registers extra information about classes and (optionally) properties in your application.
	/// For example, information about how to write a property of a class into an xml file can be stored here.
	/// This mechanism has great similarities with the attribute mechanism in the .Net framework, but differences are that
	/// 1) entries can be set dynamically
	/// 2) no code modification is necessary in the target classes (the classes for which information is stored)
	/// </summary>
	/// <example>
	///	<c> MetaInfo.SetAttribute (typeof(ILinkableComponent), "XmlFile", true); </c>
	///	Tells that the interface ILinkableComponent has subject XmlFile, which has corresponding value true.
	///	There must be another class which knows that subject XmlFile exists and uses this information in some way.
	/// <c>	MetaInfo.SetAttribute (typeof(IElementSet), "ID", "XmlRefName", "RefID"); </c>
	///	Tells that the property ID in the interface IElementSet has subject XmlRefName, which has corresponding value "RefID".
	/// </example>
	public class MetaInfo
	{
		private static bool		 _initialized  = false;
		private static EntryList _targets	   = new EntryList();
		private static Hashtable _subjectTypes = new Hashtable();

		/// <summary>
		/// Writes all metainfo to a file
		/// </summary>
		/// <param name="file">The file</param>
		public static void Write (FileInfo file)
		{
			MetaInfo.Initialize();
			XmlFile.Write (_targets, file);
		}

		/// <summary>
		/// Reads all metainfo from file
		/// </summary>
		/// <param name="file">The file</param>
		public static void Read (FileInfo file)
		{
			MetaInfo.Initialize();
			XmlFile.Read (_targets, file);
		}

		/// <summary>
		/// Sets metainfo about the xml file where meta info is saved in
		/// </summary>
		private static void Initialize() 
		{
			if (!_initialized) 
			{
				MetaInfo.SetAttribute (typeof(EntryList), "XmlItemType", "Oatc.OpenMI.Sdk.DevelopmentSupport.MetaInfoEntry");
				MetaInfo.SetAttribute (typeof(MetaInfoEntry), "Properties", "XmlItemType", "Oatc.OpenMI.Sdk.DevelopmentSupport.MetaInfoClass");
				MetaInfo.SetAttribute (typeof(MetaInfoClass), "ObjectAggregate", "Oatc.OpenMI.Sdk.DevelopmentSupport.MetaInfoClassAggregate");
				_initialized = true;
			}
		}

		/// <summary>
		/// Gets all metainfo concerning a given class
		/// </summary>
		/// <param name="target">The class name</param>
		/// <returns>All metainfo about a class, null if not found</returns>
		private static MetaInfoEntry GetEntry (string target) 
		{
			foreach (MetaInfoEntry entry in _targets) 
			{
				if (entry.ClassName.Equals (target)) 
				{
					return entry;
				}
			}

			return null;
		}

		/// <summary>
		/// Gets all metainfo concerning a given class and creates an empty block if not found
		/// </summary>
		/// <param name="target">The class name</param>
		/// <returns>All metainfo about a class</returns>
		private static MetaInfoEntry GetEntryForced (string target) 
		{
			MetaInfoEntry entry = MetaInfo.GetEntry (target);

			if (entry == null) 
			{
				entry = new MetaInfoEntry (target);
				_targets.Add (entry);
			}

			return entry;
		}
		/// <summary>
		/// Stores information about a class
		/// </summary>
		/// <param name="target">The class about which information is stored (usually as class type or string)</param>
		/// <param name="subject">The type of information (e.g. how the class is named in an xml file)</param>
		/// <param name="targetValue">The actual value</param>
		public static void SetAttribute (object target, string subject, object targetValue) 
		{
			MetaInfo.SetAttribute (target, null, subject, targetValue);
		}

		/// <summary>
		/// Stores information about a class and property
		/// </summary>
		/// <param name="target">The class about which information is stored (usually as class type or string)</param>
		/// <param name="property">The property in the class</param>
		/// <param name="subject">The type of information (e.g. how the property is named in an xml file)</param>
		/// <param name="targetValue">The actual value</param>
		public static void SetAttribute (object target, string property, string subject, object targetValue) 
		{
			if (target is Type) 
			{
				target = ((Type)target).FullName;
			}
			else if (target is Assembly) 
			{
				target = ((Assembly)target).GetName().Name;
			}

			MetaInfoEntry table = MetaInfo.GetEntryForced (target.ToString());
			table.SetValue (property, subject, targetValue);
		}

		/// <summary>
		/// Gets the stored information for a class.
		/// Not only the class is examined, but also all superclasses and implemented interfaces.
		/// </summary>
		/// <param name="target">The class about which information will be retrieved</param>
		/// <param name="subject">The type of information required</param>
		/// <returns>The information stored for this object and type of information, null if not found</returns>
		public static object GetAttribute (Type target, string subject) 
		{
			return MetaInfo.GetAttributeDefault (target, null, subject, null);
		}

		/// <summary>
		/// Gets the stored information for a class and property.
		/// Not only the class is examined, but also all superclasses and implemented interfaces.
		/// </summary>
		/// <param name="target">The class type about which information will be retrieved</param>
		/// <param name="property">The property for which information is to be required</param>
		/// <param name="subject">The type of information required</param>
		/// <returns>The information stored for the class and property, null if not found</returns>
		public static object GetAttribute (Type target, string property, string subject) 
		{
			return MetaInfo.GetAttributeDefault (target, property, subject, null);
		}

		/// <summary>
		/// Gets the stored information for a class.
		/// Not only the class is examined, but also all superclasses and implemented interfaces.
		/// </summary>
		/// <param name="targetClass">The object about which information will be retrieved</param>
		/// <param name="subject">The type of information required</param>
		/// <param name="defaultValue">Default value if the information is not found</param>
		/// <returns>The information stored for this object and type, the default value if not found</returns>
		public static object GetAttributeDefault (Type targetClass, string subject, object defaultValue) 
		{
			return MetaInfo.GetAttributeDefault (targetClass, null, subject, defaultValue);
		}

		/// <summary>
		/// Gets the stored information for a class and property.
		/// Not only the class is examined, but also all superclasses and implemented interfaces.
		/// </summary>
		/// <param name="targetClass">The object about which information will be retrieved</param>
		/// <param name="property">The property in the class</param>
		/// <param name="subject">The type of information required</param>
		/// <param name="defaultValue">Default value if the information is not found</param>
		/// <returns>The information stored for this class, property and type, the default value if not found</returns>
		public static object GetAttributeDefault (Type targetClass, string property, string subject, object defaultValue) 
		{
			// try to find appropriate value in super class
			Type targetType = targetClass;
			while (targetType != null) 
			{
				string target = targetType.FullName;
				MetaInfoEntry table = MetaInfo.GetEntry (target);

				if (table != null)
				{
					if (table.Contains(property, subject))
					{
						return table.GetValue (property, subject);
					}
				}

				targetType = targetType.BaseType;
			}

			// try to find an interface implemented by the target type
			foreach (Type implementedInterface in targetClass.GetInterfaces()) 
			{
				object attribute = MetaInfo.GetAttributeDefault (implementedInterface, property, subject, null);
				if (attribute != null) 
				{
					return attribute;
				}
			}

			return defaultValue;
		}

		/// <summary>
		/// Gets the stored information for an assembly.
		/// </summary>
		/// <param name="targetAssembly">The assembly about which information will be retrieved</param>
		/// <param name="subject">The type of information required</param>
		/// <param name="defaultValue">Default value if the information is not found</param>
		/// <returns>The information stored for this class, property and type, the default value if not found</returns>
		public static object GetAttributeDefault (Assembly targetAssembly, string subject, object defaultValue) 
		{
			return MetaInfo.GetAttributeDefault (targetAssembly.GetName().Name, subject, defaultValue);
		}

		/// <summary>
		/// Gets the stored information for a string
		/// </summary>
		/// <param name="target">The string about which information will be retrieved</param>
		/// <param name="subject">The type of information required</param>
		/// <param name="defaultValue">Default value if the information is not found</param>
		/// <returns>The information stored for this class, property and type, the default value if not found</returns>
		public static object GetAttributeDefault (string target, string subject, object defaultValue) 
		{
			MetaInfoEntry table = MetaInfo.GetEntry (target);

			if (table != null)
			{
				if (table.Contains(null, subject))
				{
					return table.GetValue (null, subject);
				}
			}

			return defaultValue;
		}

		/// <summary>
		/// Gets a list of all properties in a class, for which a value has been stored.
		/// All superclasses and implemented interfaces of the class are examined too.
		/// </summary>
		/// <param name="targetClass">The class</param>
		/// <returns>List of properties</returns>
		public static string[] GetProperties (Type targetClass) 
		{
			// try to find appropriate value in super class
			ArrayList subjects = new ArrayList();
			Type targetType = targetClass;
			while (targetType != null) 
			{
				string fullName = targetType.FullName;
				MetaInfoEntry entry = MetaInfo.GetEntry (fullName);
				if (entry != null)
				{
					foreach (MetaInfoClass property in entry.Properties) 
					{
						if (!subjects.Contains(property.Name)) 
						{
							subjects.Add (property.Name);
						}
					}
				}

				targetType = targetType.BaseType;
			}

			// try to find interfaces implemented by the target type
			foreach (Type implementedInterface in targetClass.GetInterfaces()) 
			{
				string[] interfaceProperties = MetaInfo.GetProperties (implementedInterface);
				foreach (string interfaceProperty in interfaceProperties) 
				{
					if (!subjects.Contains(interfaceProperty)) 
					{
						subjects.Add (interfaceProperty);
					}
				}
			}

			return (string[]) subjects.ToArray(typeof(string));
		}
	}

	/// <summary>
	/// Special list for all entered information in MetaInfo
	/// </summary>
	internal class EntryList : ArrayList 
	{
	}

	/// <summary>
	/// Class which groups all information stored for a class in MetaInfo
	/// </summary>
	internal class MetaInfoEntry 
	{
		private string        _name;
		private ArrayList     _properties = new ArrayList();
		private MetaInfoClass _classProperties = new MetaInfoClass(null);

		/// <summary>
		/// Default constructor
		/// </summary>
		/// <param name="className">The class</param>
		public MetaInfoEntry (string className) 
		{
			_name = className;
		}

		/// <summary>
		/// Class for which information is stored
		/// </summary>
		public string ClassName 
		{
			get {return _name;}
			set {_name = value;}
		}

		/// <summary>
		/// Group of subjects and stored values directly under the class (so without properties)
		/// </summary>
		public MetaInfoClass Class
		{
			get {return _classProperties;}
		}

		/// <summary>
		/// List of all properties for which a value is stored
		/// </summary>
		public ArrayList Properties
		{
			get {return _properties;}
		}

		/// <summary>
		/// Gets a group of subjects and associated values for a property in the class
		/// </summary>
		/// <param name="property">The property</param>
		/// <returns>Group of subjects and associated values</returns>
		private MetaInfoClass GetMetaInfo (string property) 
		{
			foreach (MetaInfoClass metaInfo in _properties) 
			{
				if (metaInfo.Name.Equals (property)) 
				{
					return metaInfo;
				}
			}

			return null;
		}

		/// <summary>
		/// Gets a list of all properties for which a value is stored
		/// </summary>
		/// <returns>The list of properties</returns>
		public string[] GetProperties() 
		{
			ArrayList list = new ArrayList();
			foreach (MetaInfoClass metaInfo in _properties) 
			{
				list.Add (metaInfo.Name);
			}

			return (string[]) list.ToArray(typeof(string));
		}

		/// <summary>
		/// Gets the stored value directly under a class for a certain subject
		/// </summary>
		/// <param name="subject">The subject</param>
		/// <returns>The stored value, null if not found</returns>
		public object GetValue (string subject) 
		{
			return _classProperties[subject];
		}

		/// <summary>
		/// Gets the stored value directly under a class for a certain subject
		/// </summary>
		/// <param name="subject">The subject</param>
		/// <param name="targetValue">The new value</param>
		public void SetValue (string subject, object targetValue) 
		{
			_classProperties[subject] = targetValue;
		}

		/// <summary>
		/// Gets the stored value for a subject and property in the class
		/// </summary>
		/// <param name="property">The property</param>
		/// <param name="subject">The subject</param>
		/// <returns>The stored value, null if not found</returns>
		public object GetValue (string property, string subject) 
		{
			if (property == null) 
			{
				return this.GetValue (subject);
			}

			MetaInfoClass metaInfo = this.GetMetaInfo (property);
			if (metaInfo != null) 
			{
				return metaInfo[subject];
			}

			return null;
		}

		/// <summary>
		/// Stores a new value for a subject and property in the class
		/// </summary>
		/// <param name="property">The property</param>
		/// <param name="subject">The subject</param>
		/// <param name="targetValue">The new value</param>
		public void SetValue (string property, string subject, object targetValue) 
		{
			if (property == null) 
			{
				this.SetValue (subject, targetValue);
				return;
			}

			MetaInfoClass metaInfo = this.GetMetaInfo (property);
			if (metaInfo == null) 
			{
				metaInfo = new MetaInfoClass (property);
				_properties.Add (metaInfo);
			}

			metaInfo[subject] = targetValue;
		}

		/// <summary>
		/// Tells whether a value is stored for a property and subject in the class
		/// </summary>
		/// <param name="property">The property name</param>
		/// <param name="subject">The subject</param>
		/// <returns>Indication of presence of the property and subject</returns>
		public bool Contains (string property, string subject) 
		{
			if (property == null) 
			{
				return _classProperties.Contains (subject);
			}

			MetaInfoClass metaInfo = this.GetMetaInfo (property);
			if (metaInfo != null) 
			{
				return metaInfo.Contains (subject);
			}

			return false;
		}
	}

	/// <summary>
	/// Class which groups all information about a property of a class type in MetaInfo
	/// </summary>
	internal class MetaInfoClass 
	{
		private string _name;
		private Hashtable _entries = new Hashtable();

		/// <summary>
		/// Default constructor
		/// </summary>
		/// <param name="property">Property name</param>
		public MetaInfoClass (string property) 
		{
			_name = property;
		}

		/// <summary>
		/// The property name for which information is stored
		/// </summary>
		public string Name
		{
			get {return _name;}
			set {_name = value;}
		}

		/// <summary>
		/// Dictionary of all subjects and stored values.
		/// The index specified the subject.
		/// </summary>
		public object this [object index] 
		{
			get {return _entries[index];}
			set {_entries[index] = value;}
		} 

		/// <summary>
		/// Indication whether a subject is stored
		/// </summary>
		/// <param name="subject">The subject</param>
		/// <returns>Indication whether the subject is stored</returns>
		public bool Contains (object subject) 
		{
			return CollectionSupport.ContainsObject (_entries.Keys, subject);
		}

		/// <summary>
		/// Gets a list of all subjects stored for this property.
		/// </summary>
		/// <returns>List of all subjects</returns>
		public string[] GetProperties() 
		{
			ArrayList properties = new ArrayList();

			IDictionaryEnumerator DictionaryEnumerator = _entries.GetEnumerator();
			while (DictionaryEnumerator.MoveNext()) 
			{
				if (DictionaryEnumerator.Key is string) 
				{
					String subject = (string) DictionaryEnumerator.Key;
					properties.Add (subject);
				}
			}

			return (string[]) properties.ToArray(typeof (string));
		}
	}

	/// <summary>
	/// Aggregate class for MetaInfoClass.
	/// Used when writing and reading with XmlFile.
	/// </summary>
	public class MetaInfoClassAggregate : IAggregate
	{
		#region IAggregate Members

		private MetaInfoClass _class;

		/// <summary>
		/// Aggregate for the meta info class
		/// An aggregate is an "in between" object between the element set and XmlFile.
		/// <seealso cref="Oatc.OpenMI.Sdk.DevelopmentSupport.IAggregate"></seealso>/>
		/// <seealso cref="Oatc.OpenMI.Sdk.DevelopmentSupport.XmlFile"></seealso>/>
		/// </summary>
		public MetaInfoClassAggregate (object source)
		{
			_class = (MetaInfoClass) source;
			UpdateAggregate();
		}
	
		/// <summary>
		/// Gets the underlying object
		/// </summary>
		public object Source
		{
			get	{return _class;}
		}

		/// <summary>
		/// Gets a list of properties which are accessed in a generic way.
		/// </summary>
		public string[] Properties
		{
			get	
			{
				ArrayList props = new ArrayList (_class.GetProperties());
				props.Add ("Name");
				return (string[]) props.ToArray (typeof(string));
			}
		}

		/// <summary>
		/// Class type of a property
		/// </summary>
		/// <param name="property">The property name</param>
		/// <returns>The property type</returns>
		public Type GetType(string property)
		{
			if (property.Equals ("Name"))
			{
				return typeof(string);
			}

			return _class[property].GetType();
		}

		/// <summary>
		/// Tells whether a property can be written
		/// </summary>
		/// <param name="property">The property name</param>
		/// <returns>Indication of writable</returns>
		public bool CanWrite(string property)
		{
			return true;
		}

		/// <summary>
		/// Tells whether a property can be read
		/// </summary>
		/// <param name="property">The property name</param>
		/// <returns>Indication of readable</returns>
		public bool CanRead(string property)
		{
			return true;
		}

		/// <summary>
		/// Gets the value of a property
		/// </summary>
		/// <param name="property">The property name</param>
		/// <returns>The property value</returns>
		public object GetValue(string property)
		{
			if (property.Equals ("Name"))
			{
				return _class.Name;
			}

			return _class[property];
		}

		/// <summary>
		/// Sets a value for a certain property
		/// </summary>
		/// <param name="property">The property name</param>
		/// <param name="target">The new property value</param>
		public void SetValue(string property, object target)
		{
			if (property.Equals ("Name"))
			{
				_class.Name = (string) target;
				return;
			}

			_class[property] = target;
		}

		/// <summary>
		/// Updates the underlying source. Takes no action.
		/// </summary>
		public void UpdateSource()
		{
		}

		/// <summary>
		/// Prepares the aggregate for subsequent GetValue calls. Takes no action.
		/// </summary>
		public void UpdateAggregate()
		{
		}

		/// <summary>
		/// Gets a referenced value, i.e. a value corresponding with a reference string within the scope of the source.
		/// Implementation is delegated to XmlFile.GetRegisteredTarget.
		/// </summary>
		/// <param name="reference">Reference</param>
		/// <returns>The referenced object</returns>
		public object GetReferencedValue (string reference) 
		{
			return XmlFile.GetRegisteredTarget(this.Source, reference);
		}

		#endregion
	}
}
