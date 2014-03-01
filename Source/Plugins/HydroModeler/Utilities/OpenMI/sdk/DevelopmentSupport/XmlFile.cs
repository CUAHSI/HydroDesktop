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
using System.Globalization;
using System.IO;
using System.Xml;
using System.Xml.Schema;

namespace Oatc.OpenMI.Sdk.DevelopmentSupport
{
	/// <summary>
	/// Support class for reading and writing objects to xml.
	/// Each object to be written or read is accessed in a generic way for the properties to write or read. This is done via an aggregate intermediate object.
	/// Objects are written once in an xml file. If they are encountered more than once, references are written. References may refer to other files.
	/// Writing and reading strings is always done and expected in neutral culture (English-US) in order to enable exchangability.
	/// Needed meta info is retrieved from MetaInfo.
	/// <seealso href="XmlFileMetaInfo.htm">MetaInfo used by XmlFile</seealso>
	/// <seealso cref="Oatc.OpenMI.Sdk.DevelopmentSupport.IAggregate"/>
	/// <seealso cref="Oatc.OpenMI.Sdk.DevelopmentSupport.MetaInfo"/>
	/// </summary>
	public class XmlFile
	{
		private static CultureInfo _culture = CultureInfo.CreateSpecificCulture ("");

		private static Hashtable _fileList          = new Hashtable();
		private static ArrayList _readObjectsList   = new ArrayList();
		private static ArrayList _unreadObjectsList = new ArrayList();
		private static Hashtable _rootList          = new Hashtable();
		private static Hashtable _rootKeyList       = new Hashtable();
		private static Hashtable _rootObjectList    = new Hashtable();
		private static Hashtable _aggregateTable    = new Hashtable();

		#region Public Read Methods

		/// <summary>
		/// Reads an object from file
		/// The file to read should have been registered with the object earlier by a reqad or write action
		/// </summary>
		/// <param name="target">The object to be read</param>
		/// <exception cref="System.Exception">Cannot find file to read</exception>
		/// <exception cref="System.Exception">Cannot resolve class type when an object for an xml element must be instantiated</exception>
		/// <exception cref="System.Exception">Cannot find class type</exception>
		/// <exception cref="System.Exception">Xml element holds a reference, but referenced object cannot be found</exception>
		/// <exception cref="System.Exception">Schema cannot be found although it has been specified in MetaInfo</exception>
		/// <exception cref="System.Exception">Validation error when xml file doesn't meet specified schema</exception>
		public static void Read (object target) 
		{
			string file = (string) _fileList[target];
			if (file != null) 
			{
				XmlFile.Read (target, new FileInfo(file));
			}
		}

		/// <summary>
		/// Reads an object from file
		/// </summary>
		/// <param name="target">The object to be read</param>
		/// <param name="file">The file containing the object's information</param>
		/// <exception cref="System.Exception">Cannot find file to read</exception>
		/// <exception cref="System.Exception">Cannot resolve class type when an object for an xml element must be instantiated</exception>
		/// <exception cref="System.Exception">Cannot find class type</exception>
		/// <exception cref="System.Exception">Xml element holds a reference, but referenced object cannot be found</exception>
		/// <exception cref="System.Exception">Schema cannot be found although it has been specified in MetaInfo</exception>
		/// <exception cref="System.Exception">Validation error when xml file doesn't meet specified schema</exception>
		public static void Read (object target, FileInfo file) 
		{
			if (file.Exists) 
			{
				XmlFile.CreateRegistration (target);

				XmlReader reader = new XmlTextReader(file.FullName);

				string xmlValidation	= (string) MetaInfo.GetAttribute (target.GetType(), "XmlSchema");
				string xmlNameSpace		= (string) MetaInfo.GetAttribute (target.GetType(), "XmlNameSpace");
				string xsdPackage		= (string) MetaInfo.GetAttribute (target.GetType(), "XsdPackage");

				if ((xmlValidation != null) && (xmlNameSpace != null))
				{
					Stream schemaStream = XmlFile.GetSchema (target.GetType(), xmlValidation, xsdPackage);
					if (schemaStream != null) 
					{
						XmlSchema schema = XmlSchema.Read(schemaStream, new ValidationEventHandler (ValidationCallBack));

						// VS2005 fix - the following code is deprecated and ws replaced by new code - see below
						//XmlSchemaCollection xsc = new XmlSchemaCollection();
						//xsc.ValidationEventHandler += new ValidationEventHandler( ValidationCallBack );
						//xsc.Add( schema );  //XSD schema

						//reader = new XmlValidatingReader( reader );
						//( (XmlValidatingReader) reader ).ValidationType = ValidationType.Schema;
						//( (XmlValidatingReader) reader ).ValidationEventHandler += new ValidationEventHandler( ValidationCallBack );
						//( (XmlValidatingReader) reader ).Schemas.Add( xsc );

						XmlSchemaSet xss = new XmlSchemaSet();
						xss.ValidationEventHandler += new ValidationEventHandler (ValidationCallBack);
						xss.Add(schema);  //XSD schema

						XmlReaderSettings settings = new XmlReaderSettings();
						settings.ValidationType = ValidationType.Schema;
						settings.ValidationEventHandler += new ValidationEventHandler (ValidationCallBack);
						settings.Schemas.Add( xss );

						reader = XmlReader.Create( reader, settings );
					} 
					else 
					{
						throw new Exception ("Required schema " + xmlValidation + " not found for parsing " + file.FullName);
					}
				} 
				else 
				{
				}

				_fileList[target] = file.FullName;
				_readObjectsList.Add (target);
				_unreadObjectsList.Remove (target);

				reader.MoveToContent();
				XmlFile.Read (reader, target, null, null, target);
				reader.Close();
			} 
			else 
			{
				_fileList[target] = file.FullName;
			}
		}

		/// <summary>
		/// Reads and creates an object from a given file
		/// </summary>
		/// <param name="file">The file containig the object to be read</param>
		/// <returns>Populated object</returns>
		/// <exception cref="System.Exception">Cannot find file to read</exception>
		/// <exception cref="System.Exception">Cannot resolve class type when an object for an xml element must be instantiated</exception>
		/// <exception cref="System.Exception">Cannot find class type</exception>
		/// <exception cref="System.Exception">Xml element holds a reference, but referenced object cannot be found</exception>
		/// <exception cref="System.Exception">Schema cannot be found although it has been specified in MetaInfo</exception>
		/// <exception cref="System.Exception">Validation error when xml file doesn't meet specified schema</exception>
		public static object GetRead (FileInfo file) 
		{
			return XmlFile.GetRead(file, null);
		}

		/// <summary>
		/// Creates and reads an object from a given file
		/// </summary>
		/// <param name="file">The file containig the object to be read</param>
		/// <param name="objectType">The expected class type of the new object</param>
		/// <returns>The new populated object</returns>
		/// <exception cref="System.Exception">Cannot find file to read</exception>
		/// <exception cref="System.Exception">Cannot resolve class type when an object for an xml element must be instantiated</exception>
		/// <exception cref="System.Exception">Cannot find class type</exception>
		/// <exception cref="System.Exception">Xml element holds a reference, but referenced object cannot be found</exception>
		/// <exception cref="System.Exception">Schema cannot be found although it has been specified in MetaInfo</exception>
		/// <exception cref="System.Exception">Validation error when xml file doesn't meet specified schema</exception>
		public static object GetRead (FileInfo file, Type objectType) 
		{
			if (file.Exists) 
			{
				object target = GetFileObject(file.FullName);

				if (target == null) 
				{
					XmlReader reader = new XmlTextReader(file.FullName);
					reader.MoveToContent();

					string classType  = (string) MetaInfo.GetAttributeDefault(reader.GetAttribute ("Type"), "XmlTypeAlias", reader.GetAttribute ("Type"));
					ObjectSupport.LoadAssembly (reader.GetAttribute ("Assembly"));

					reader.Close();

					if (classType != null) 
					{
//						classType = (string) MetaInfo.GetAttributeDefault (ObjectSupport.GetType(ClassType), "XmlTypeAlias", classType);
						target = ObjectSupport.GetInstance (classType);
					}

					if ((target == null) && (objectType != null))
					{
						target = ObjectSupport.GetInstance (objectType);
					}

					if (target == null) 
					{
						if (classType != null) 
						{
							throw new Exception("Could not find class type " + classType);
						}
						else if (objectType != null) 
						{
							throw new Exception("Could not find class type " + objectType.FullName);
						}
					}
				}


				XmlFile.Read (target, file);

				return target;
			} 
			else 
			{
				throw new Exception ("File doesn't exist: " + file.FullName);
			}
		}

		#endregion

		#region Xml Validation

		/// <summary>
		/// Callback method used for validation against an xsd file
		/// </summary>
		/// <param name="sender">Sender of the method</param>
		/// <param name="args">Error details</param>
		/// <exception cref="System.Exception">Validation error when xml file doesn't meet specified schema</exception>
		private static void ValidationCallBack (object sender, ValidationEventArgs args)
		{
			throw new Exception ("Validation error: " + args.Message);
		}  

		/// <summary>
		/// Gets a stream containing an xsd file, which resides within an assembly.
		/// </summary>
		/// <param name="type">If the xsdPackage isn't defined, type in the assembly in which the xsd file should reside</param>
		/// <param name="xmlSchema">Name of the schema</param>
		/// <param name="xsdPackage">The assembly in which the xsd file resides</param>
		/// <returns>The xsd stream, null if not found</returns>
		private static Stream GetSchema (Type type, string xmlSchema, string xsdPackage) 
		{
			Assembly assembly = Assembly.GetAssembly(type);
			if ((xsdPackage != null) && (!xsdPackage.Trim().Equals(""))) 
			{
				// VS2005 fix
				// this fix also needed to include full name to XsdPackage meta-info in XmlConfiguration.cs
				//assembly = Assembly.LoadWithPartialName(xsdPackage);
				assembly = Assembly.Load( xsdPackage ); 
			}

			Stream	 stream	  = assembly.GetManifestResourceStream(xmlSchema);
			if (stream == null) 
			{
				string assemblyName = assembly.GetName().Name;
				stream = assembly.GetManifestResourceStream(assemblyName + "." + xmlSchema);
			}

			if (stream == null) 
			{
				FileInfo assemblyFile = new FileInfo(assembly.Location);
				FileInfo schema = FileSupport.ExpandRelativePath (assemblyFile.Directory, xmlSchema);
				if (schema.Exists) 
				{
					stream = schema.OpenRead();
				}
			}

			return stream;
		}

		#endregion

		#region Internal Read Methods

		/// <summary>
		/// Reads the current xml element into an object.
		/// Then moves the xml stream forward to the next element.
		/// If the next element is one level deeper, that element is read by calling this method recursively.
		/// Finally the current element is positioned on the first unread element.
		/// By calling this method at the top level of an xml file, the whole xml file is read.
		/// </summary>
		/// <param name="reader">The xml stream</param>
		/// <param name="target">The object which will be populated with data within the current xml element</param>
		/// <param name="parent">The parent of the object, i.e. the object to which the target will be assigned</param>
		/// <param name="targetProperty">The property of the parent</param>
		/// <param name="root">The root object of the xml file</param>
		/// <exception cref="System.Exception">Read only property has null value, but a property of this object should be set according to the xml file</exception>
		private static void Read (XmlReader reader, object target, object parent, string targetProperty, object root) 
		{
			// If it is a reference, the object has been read already and assigned to its parent
			string Reference = XmlFile.GetKeyFromXml(target.GetType(), reader, true);
			if (Reference != null) 
			{
				XmlFile.MoveToNextElement(reader); // moves to next element
				return;
			}

			bool inCurrentXmlFile = (reader.GetAttribute ("File") == null);

			if ((reader.GetAttribute ("File") == null) && (Reference == null))
			{
				_rootList[target] = root;
			}

			XmlFile.RegisterObject (target, reader, root);

			IAggregate aggregate = XmlFile.GetAggregate(target);
			XmlFile.ReadAttributes (aggregate, reader, root);

			string defaultProperty = MetaInfo.GetAttribute (target.GetType(), "XmlDefaultProperty") as string;
			if (defaultProperty != null) 
			{
				parent = target;
				target = aggregate.GetValue(defaultProperty);
				targetProperty = defaultProperty;

				aggregate = XmlFile.GetAggregate(target);
			}

			int processLevel = reader.Depth;

			string[] properties = aggregate.Properties;

			// Special handling for lists
			if (target is IList) 
			{
				IList List = (IList) target;
				List.Clear();

				string listEntryClass = (string) MetaInfo.GetAttributeDefault (parent.GetType(), targetProperty, "XmlItemType", MetaInfo.GetAttribute (target.GetType(), "XmlItemType"));

				if (!reader.IsEmptyElement) 
				{
					XmlFile.MoveToNextElement(reader); // moves to first element
					while ((reader.Depth > processLevel) && (reader.NodeType != XmlNodeType.EndElement)) 
					{
						object targetValue = XmlFile.GetObject (reader, ObjectSupport.GetType(listEntryClass), root);
						if (targetValue != null) 
						{
							XmlFile.Read (reader, targetValue, target, null, root);
							List.Add (targetValue);
						}
						else 
						{
							XmlFile.MoveToNextElement (reader);
						}
					}
				}
				else 
				{
					XmlFile.MoveToNextElement(reader); // moves to next element
				}
			} 
			// Special handling for hashtables
			else if (target is IDictionary) 
			{
				IDictionary Dictionary = (IDictionary) target;
				Dictionary.Clear();

				if (!reader.IsEmptyElement) 
				{
					XmlFile.MoveToNextElement(reader); // moves to first dictionary element
					while ((reader.Depth > processLevel) && (reader.NodeType != XmlNodeType.EndElement)) 
					{
						XmlFile.MoveToNextElement(reader); // moves to key
						object keyValue = XmlFile.GetObject (reader, null, root);
						XmlFile.Read (reader, keyValue, target, null, root);

						object targetValue = XmlFile.GetObject (reader, null, root);
						if (targetValue != null) 
						{
							XmlFile.Read (reader, targetValue, target, null, root);
						}
						else 
						{
							XmlFile.MoveToNextElement (reader);
						}

						Dictionary.Add (keyValue, targetValue);
					}
				}
				else 
				{
					XmlFile.MoveToNextElement(reader); // moves to next element
				}
			} 
			else 
			{
				if (!reader.IsEmptyElement) 
				{
					XmlFile.MoveToNextElement(reader);

					// Read all sub elements
					while (!reader.EOF && (reader.Depth > processLevel))
					{
						string property = XmlFile.GetProperty(aggregate, reader.Name);
						if (property != null) 
						{
							if ((Boolean) MetaInfo.GetAttributeDefault (target.GetType(), property, "XmlSkipElement", false)) 
							{
								XmlFile.MoveToNextElement(reader);
								while (reader.Depth > processLevel + 1) 
								{
									XmlFile.MoveToNextElement(reader);
								}
							}
							else 
							{
								object targetValue;
								if (!aggregate.CanWrite(property)) 
								{
									targetValue = aggregate.GetValue(property);
									if (targetValue == null) 
									{
										throw new Exception ("Cannot access readonly element " + property + " at tag " + reader.Name);
									}
								}
								else
								{
									Type type = ObjectSupport.GetType((string) MetaInfo.GetAttributeDefault (target.GetType(), property, "XmlType", aggregate.GetType(property).FullName));
									targetValue = XmlFile.GetObject (reader, type, root);
								}

								if (targetValue != null) 
								{
									XmlFile.Read (reader, targetValue, target, property, root);
									if (aggregate.CanWrite(property)) 
									{
										aggregate.SetValue (property, targetValue);
									}
								}
								else 
								{
									XmlFile.MoveToNextElement (reader);
								}
							}
						}
						else
						{
							XmlFile.MoveToNextElement(reader); // moves to next
						}
					}
				}
				else 
				{
					XmlFile.MoveToNextElement(reader); // moves to next
				}
			}

			// Ensures that UpdateSource is only called once per aggregate
			if (inCurrentXmlFile) 
			{
				aggregate.UpdateSource();
			}
		}

		/// <summary>
		/// Gets the object associated with an xml element.
		/// Different procedures are followed for the following cases:
		/// 1) The file is mentioned. Then that file is read and the top object (a.k.a. the root) of that file is returned
		/// 2) The file is mentioned and the xml element is defined as a reference. Then that file is read (if not before) and the root of the file is asked for a property with the specified reference.
		/// 3) The file isn't mentioned and the xml element is defined as a reference. Then the object is retrieved from the registration.
		/// 4) Otherwise the object is instantiated
		/// </summary>
		/// <param name="reader">The xml stream</param>
		/// <param name="defaultType">The type to instantiate when no type is specified in the xml element</param>
		/// <param name="root">The top object of the xml stream</param>
		/// <returns>The object associated with the xml element</returns>
		/// <exception cref="System.Exception">Cannot resolve class type when an object for an xml element must be instantiated</exception>
		/// <exception cref="System.Exception">Cannot find class type</exception>
		/// <exception cref="System.Exception">Cannot instantiate object for known class type</exception>
		/// <exception cref="System.Exception">Xml element holds a reference, but referenced object cannot be found</exception>
		private static object GetObject (XmlReader reader, Type defaultType, object root) 
		{
			object targetValue = null;

			string file       = reader.GetAttribute ("File");
			string classType  = (string) MetaInfo.GetAttributeDefault(reader.GetAttribute ("Type"), "XmlTypeAlias", reader.GetAttribute ("Type"));

			ObjectSupport.LoadAssembly (reader.GetAttribute ("Assembly"));

			Type   targetType = null;

			if (classType != null) 
			{
//				classType = (string) MetaInfo.GetAttributeDefault (ObjectSupport.GetType(classType), "XmlTypeAlias", classType);
				targetType = ObjectSupport.GetType (classType);
			}

			if ((classType == null) && (defaultType != null))
			{
				targetType = defaultType;
			}

			string reference = XmlFile.GetKeyFromXml (targetType, reader, true);
			if ((targetType == null) && (reference == null))
			{
				if (classType != null) 
				{
					throw new Exception ("Cannot find type " + classType + " when reading element tag " + reader.Name);
				}
				else 
				{
					throw new Exception ("Cannot resolve element tag " + reader.Name);
				}
			}

			if (file != null)
			{
				string fullFile = FileSupport.ExpandRelativePath(XmlFile.GetRegisteredFile(root).Directory, file).FullName;
				object targetRoot = GetFileObject (fullFile);
				if ((reference != null) && (!reference.Equals("")))
				{
					if (File.Exists (fullFile)) 
					{
						if (targetRoot == null) 
						{
							targetRoot = XmlFile.GetRead(new FileInfo(fullFile), targetType);					
						}

						IAggregate aggregate = XmlFile.GetAggregate(targetRoot);
						targetValue = aggregate.GetReferencedValue (reference);

						if (targetValue == null) 
						{
							throw new Exception ("Cannot find referenced element " + reference + " at tag " + reader.Name);
						}
					} 
					else 
					{
						if (!((bool) MetaInfo.GetAttributeDefault (targetType, "XmlAllowFileMissing", false))) 
						{
							throw new Exception ("Referenced file does not exist: " + fullFile);
						}
					}
				}
				else // File != null && Reference == null
				{
					targetValue = targetRoot;
					if (File.Exists (fullFile)) 
					{
						if (targetValue == null) 
						{
							targetValue = ObjectSupport.GetInstance (targetType);
							_fileList[targetValue] = file;

							XmlFile.Read (targetValue, new FileInfo(fullFile));
						}

						if (targetValue == null) 
						{
							throw new Exception ("Cannot find referenced element in " + fullFile + " at tag " + reader.Name);
						}

						string identifier = reader.GetAttribute("Identifier");
						if (identifier != null)
						{
							XmlFile.RegisterKey(root, identifier, targetValue);
						}
					}
					else 
					{
						if (!((bool) MetaInfo.GetAttributeDefault (targetType, "XmlAllowFileMissing", false))) 
						{
							throw new Exception ("Referenced file does not exist: " + fullFile);
						}
					}
				}
			} 
			else // file == null
			{
				if (reference != null) 
				{
					targetValue = XmlFile.GetRegisteredTarget(root, reference);
					if (targetValue == null) 
					{
						throw new Exception ("Cannot find referenced element " + reference + " at tag " + reader.Name);
					}
				}
				else // File == null && Reference == null
				{
					string attributeValue = reader.GetAttribute ("Value");
					if (attributeValue == null) 
					{
						targetValue = ObjectSupport.GetInstance (targetType);
					}
					else if (targetType.Equals (typeof (FileInfo)))
					{
						targetValue = FileSupport.ExpandRelativePath (XmlFile.GetRegisteredFile(root).Directory, attributeValue);
					}
					else if (targetType.Equals (typeof (DirectoryInfo)))
					{
						targetValue = FileSupport.ExpandRelativeDirectory (XmlFile.GetRegisteredFile(root).Directory, attributeValue);
					}
					else 
					{
						targetValue = ObjectSupport.GetInstance (targetType, attributeValue, _culture);
					}

					if (targetValue == null) 
					{
						throw new Exception ("Cannot instantiate " + targetType.FullName + " at element tag " + reader.Name);
					}
				}
			}

//			if (targetValue == null) 
//			{
//				throw new Exception ("Cannot resolve element tag " + reader.Name);
//			}

			return targetValue;

		}

		/// <summary>
		/// Reads the attributes of the current xml element and uses them to populute the target with
		/// </summary>
		/// <param name="target">The object which is populated with the xml attributes</param>
		/// <param name="reader">The xml stream</param>
		/// <param name="root">Top object of the xml stream</param>
		/// <exception cref="System.Exception">Cannot find class type</exception>
		/// <exception cref="System.Exception">Cannot instantiate object for known class type</exception>
		private static void ReadAttributes (IAggregate target, XmlReader reader, object root) 
		{
			// Read all attributes
			string[] properties = target.Properties;
			for (int i = 0; i < properties.Length; i++) 
			{
				string name = XmlFile.GetElementName (target.Source.GetType(), properties[i], false);
				if (name != null) 
				{
					string attributeValue = reader.GetAttribute(name);
					if (attributeValue != null) 
					{
						string classType = (string) MetaInfo.GetAttributeDefault (target.Source.GetType(), properties[i], "XmlType", target.GetType(properties[i]).FullName);
						Type type = ObjectSupport.GetType(classType);
						if (type == null) 
						{
							throw new Exception("Could not find class type " + classType);
						}

						if (type.Equals(typeof(FileInfo))) 
						{
							attributeValue = FileSupport.ExpandRelativePath (XmlFile.GetRegisteredFile(root).Directory, attributeValue).FullName;
						}

						if (type.Equals(typeof(DirectoryInfo))) 
						{
							attributeValue = FileSupport.ExpandRelativeDirectory (XmlFile.GetRegisteredFile(root).Directory, attributeValue).FullName;
						}

						object targetValue = ObjectSupport.GetInstance (type, attributeValue, _culture);
						if (targetValue == null) 
						{
							throw new Exception("Could not instantiate class type " + type.FullName);
						}

						if (target.CanWrite(properties[i])) 
						{
							target.SetValue(properties[i], targetValue);
						}
					}
				}
			}
		}

		/// <summary>
		/// Moves the xml stream forward to the next readable element
		/// </summary>
		/// <param name="reader">The xml stream</param>
		private static void MoveToNextElement(XmlReader reader) 
		{
			reader.Read();
			while (!reader.EOF && (reader.NodeType != XmlNodeType.Element)) 
			{
				reader.Read();
			}
		}

		#endregion

		#region Administration

		/// <summary>
		/// Sets up registration tables for a root object
		/// </summary>
		/// <param name="root">The root object</param>
		private static void CreateRegistration (object root) 
		{
			if (_rootKeyList[root] == null) 
			{
				_rootKeyList[root] = new Hashtable();
			}

			if (_rootObjectList[root] == null) 
			{
				_rootObjectList[root] = new Hashtable();
			}
		}

		/// <summary>
		/// Registers that an object is written to or read from the same file as a root object.
		/// This method calls RegisterKey internally.
		/// </summary>
		/// <param name="target">The object to register</param>
		/// <param name="reader">The xml stream</param>
		/// <param name="root">The object at the top of an xml file</param>
		private static void RegisterObject (object target, XmlReader reader, object root) 
		{
			string key = XmlFile.GetKeyFromXml (target.GetType(), reader, false);
			
			// Register the target as being read from this xml file
			if (key != null)
			{
				XmlFile.RegisterKey (root, key, target);
			}
		}

		/// <summary>
		/// Registers an object with a unique key within the scope of the root.
		/// </summary>
		/// <param name="root">The root object</param>
		/// <param name="identifier">The unique key</param>
		/// <param name="target">The object to register</param>
		private static void RegisterKey (object root, object identifier, object target) 
		{
			Hashtable keys = (Hashtable) _rootKeyList[root];
			keys[identifier] = target;

			Hashtable objects = (Hashtable) _rootObjectList[root];
			objects[target] = identifier;
		}

		/// <summary>
		/// Gets a key from registration given the object.
		/// The key will only be found if RegisterKey or RegisterObject has been called before.
		/// </summary>
		/// <param name="root">The root object, the scope where is searched for the object</param>
		/// <param name="target">The object to search for</param>
		/// <returns>The key, null if not found</returns>
		private static object GetRegisteredKey (object root, object target) 
		{
			Hashtable objects = (Hashtable) _rootObjectList[root];
			if (objects != null) 
			{
				return objects[target];
			}

			return null;
		}

		/// <summary>
		/// Gets an object from registration given the key.
		/// The object will only be found if RegisterKey or RegisterObject has been called before.
		/// </summary>
		/// <param name="root">The root object, the scope where is searched for the key</param>
		/// <param name="identifier">The key to search for</param>
		/// <returns>The associated object, null if not found</returns>
		public static object GetRegisteredTarget (object root, object identifier) 
		{
			Hashtable keys = (Hashtable) _rootKeyList[root];
			if (keys != null) 
			{
				return keys[identifier];
			}

			return null;
		}

		/// <summary>
		/// Gets the file associated with an object.
		/// The object should be the root of the file, i.e. associated with the top xml element.
		/// </summary>
		/// <param name="anObject"></param>
		/// <returns>The file, null if the object isn't a root of a file</returns>
		public static FileInfo GetRegisteredFile (object anObject)
		{
			if (_fileList[anObject] != null) 
			{
				return new FileInfo((string) _fileList[anObject]);
			}

			return null;
		}

		/// <summary>
		/// Gets a list of all objects, whcih are known to be the root of a file.
		/// The root of a file is the object associated with the top xml element.
		/// </summary>
		/// <returns></returns>
		public static object[] GetRegisteredObjects() 
		{
			ArrayList list = new ArrayList(_fileList.Keys);
			return list.ToArray();
		}

		/// <summary>
		/// Gets the root object of a file
		/// </summary>
		/// <param name="File">Full name of the file</param>
		/// <returns>The root object, null if not found</returns>
		private static object GetFileObject(string File) 
		{
			IDictionaryEnumerator DictionaryEnumerator = _fileList.GetEnumerator();
			while (DictionaryEnumerator.MoveNext()) 
			{
				if (DictionaryEnumerator.Value.Equals (File)) 
				{
					object TargetValue = DictionaryEnumerator.Key;
					return TargetValue;
				}
			}

			return null;
		}

		/// <summary>
		/// Gets the file registered with an object and if not found, a new file is created
		/// </summary>
		/// <param name="target">The object an associated file is searched for</param>
		/// <param name="dir">Directory in which a new file will be created, if no file exists</param>
		/// <returns>The associated file</returns>
		private static FileInfo GetForcedRegisteredFile (object target, DirectoryInfo dir) 
		{
			if (XmlFile.GetRegisteredFile(target) != null) 
			{
				return XmlFile.GetRegisteredFile(target);
			}

			FileInfo newFile = new FileInfo(dir + "\\" + target.GetType().Name + "1.xml");
			for (int i = 1; newFile.Exists; i++) 
			{
				newFile = new FileInfo (dir + "\\" + target.GetType().Name + i.ToString() + ".xml");
			}

			_fileList[target] = newFile;

			return newFile;
		}

		/// <summary>
		/// Removes an object of all registrations.
		/// To be used for releasing memory
		/// </summary>
		/// <param name="disposeObject">The object to be removed from registrations</param>
		public static void DisposeObject (object disposeObject) 
		{
			_fileList[disposeObject] = null;
			_readObjectsList.Remove(disposeObject);
			_unreadObjectsList.Remove(disposeObject);

			_rootList[disposeObject] = null;

			foreach (object o in new ArrayList(_rootList.Keys)) 
			{
				if (_rootList[o] == disposeObject) 
				{
					_rootList[o] = null;
				}
			}

			_rootKeyList[disposeObject] = null;
			_rootObjectList[disposeObject] = null;
		}

		#endregion

		#region Aggregate

		/// <summary>
		/// Gets the aggregate of an object.
		/// The aggregate is an object, which can be used for querying the object in a generic way.
		/// The MetaInfo attribute "ObjectAggregate" is used to identify the class type of the aggregate, if not present DefaultAggregate is used.
		/// The same aggregate object is reused when the same object is passed.
		/// <seealso cref="Oatc.OpenMI.Sdk.DevelopmentSupport.IAggregate"/>
		/// <seealso cref="Oatc.OpenMI.Sdk.DevelopmentSupport.MetaInfo"/>
		/// </summary>
		/// <param name="target">The object an aggregate is asked for</param>
		/// <returns>The aggregate object</returns>
		/// <exception cref="System.Exception">Cannot find aggregate class type</exception>
		/// <exception cref="System.Exception">Cannot instantiate aggregate for known class type</exception>
		private static IAggregate GetAggregate (object target) 
		{
			IAggregate aggregate = (IAggregate) _aggregateTable[target];
			if (aggregate == null) 
			{
				string classType = (string)MetaInfo.GetAttributeDefault (target.GetType(), "ObjectAggregate", typeof (DefaultAggregate).FullName);
				aggregate = (IAggregate) ObjectSupport.GetInstance (classType, target);
				if (aggregate == null) 
				{
					throw new Exception("Could not find class type " + classType);
				}

				_aggregateTable[target] = aggregate;
			}

			aggregate.UpdateAggregate();

			return aggregate;
		}

		#endregion

		#region Keys of objects

		/// <summary>
		/// Gets a string from an xml file which uniquely identifies the object which is currently parsed
		/// This string reflects the identification of the object, e.g. the ID of the object
		/// </summary>
		/// <param name="TargetType"></param>
		/// <param name="reader">The xml file stream</param>
		/// <param name="referenced">Indication whether the current xml element refers to a prior definition of the object</param>
		/// <returns>The identification string</returns>
		private static string GetKeyFromXml(Type TargetType, XmlReader reader, bool referenced) 
		{
			string key = null;

			if (referenced) 
			{
				string Reference  = reader.GetAttribute ("Reference");
				if (Reference != null) 
				{
					//key = Reference + ";";
					return Reference;
				}

				if (TargetType != null) 
				{
					string[] property = MetaInfo.GetProperties(TargetType);

					// Try all properties which are registered as key
					for (int i = 0; i < property.Length; i++) 
					{
						if ((Boolean) MetaInfo.GetAttributeDefault (TargetType, property[i], "XmlKey", false)) 
						{
							string KeyValue  = reader.GetAttribute ((string) MetaInfo.GetAttributeDefault (TargetType, property[i], "XmlRefName", property[i]));
							if (KeyValue != null) 
							{
								key += KeyValue + ";"; 
							}
						}
					}
				}
			} 
			else 
			{
				string Identifier = reader.GetAttribute("Identifier");
				if (Identifier != null) 
				{
					//key = Identifier + ";";
					return Identifier;
				}

				string[] property = MetaInfo.GetProperties(TargetType);

				// Try all properties which are registered as key
				for (int i = 0; i < property.Length; i++) 
				{
					if ((Boolean) MetaInfo.GetAttributeDefault (TargetType, property[i], "XmlKey", false))
					{
						string KeyValue  = reader.GetAttribute ((string) MetaInfo.GetAttributeDefault (TargetType, property[i], "XmlName", property[i]));
						if (KeyValue != null) 
						{
							key += KeyValue + ";"; 
						}
					}
				}
			}

			if (key != null) 
			{
				key += TargetType.FullName;
			}

			return key;
		}

		/// <summary>
		/// Gets the key from an object. 
		/// If the key isn't unique and the property allows for key generation, a unique key is generated.
		/// </summary>
		/// <param name="target"></param>
		/// <param name="root"></param>
		/// <returns></returns>
		private static string GetKeyFromObject(IAggregate target, object root) 
		{
			string key = null;
			int lastkey = 0;

			string[] property = target.Properties;
			int generatedProperty = -1;

			bool generationPossible = true;
			while (generationPossible) 
			{
				key = null;
				lastkey++;
				generationPossible = false;

				for (int i = 0; i < property.Length; i++) 
				{
					if ((Boolean) MetaInfo.GetAttributeDefault (target.Source.GetType(), property[i], "XmlKey", false))
					{
						object KeyObject  = target.GetValue(property[i]);
						if ((KeyObject != null) && (!KeyObject.ToString().Trim().Equals("")))
						{
							key += KeyObject.ToString() + ";"; 
						}
						else if ((Boolean) MetaInfo.GetAttributeDefault (target.Source.GetType(), property[i], "XmlAllowGeneration", true))
						{
							generationPossible = true;
							generatedProperty = i;
							key += lastkey + ";";
						}
					}
				}

				if (key != null) 
				{
					key += target.Source.GetType().FullName;

					object registeredObject = XmlFile.GetRegisteredTarget(root, key);
					if ((registeredObject == null) || (registeredObject == target)) 
					{
						if (generatedProperty != -1) 
						{
							Type PropertyType = target.GetType(property[generatedProperty]);
							if (PropertyType.Equals(typeof(string)))
							{
								target.SetValue(property[generatedProperty], Convert.ToString(lastkey));
							}
							else if (PropertyType.Equals(typeof(Int16)) || PropertyType.Equals(typeof(Int32)) || PropertyType.Equals(typeof(Int64))) 
							{
								target.SetValue(property[generatedProperty], lastkey);
							}
						}
						return key;
					} 
				}
			}

			return key;
		}

		#endregion

		#region Public Write Methods

		/// <summary>
		/// Writes an object to an xml file
		/// The registered file of the object will be used as xml file. This is the file
		/// to which the object was written to or read from in an earlier stage
		/// </summary>
		/// <param name="target">The object to be written</param>
		/// <exception cref="System.Exception">Cannot derive key for object which must be written as a reference. Probably MetaInfo is missing for the class and subject XmlKey</exception>exception>
		public static void Write (object target) 
		{
			object fileobj = _fileList[target];
			FileInfo file = null;

			if (fileobj is FileInfo) 
			{
				file = (FileInfo) fileobj;
			}

			if (fileobj is string) 
			{
				file = new FileInfo((string)fileobj);
			}

			if (file != null) 
			{
				XmlFile.Write (target, file);
			}
		}

		/// <summary>
		/// Writes an object to an xml file
		/// </summary>
		/// <param name="target">The object to write</param>
		/// <param name="file">The xml file</param>
		/// <exception cref="System.Exception">Cannot derive key for object which must be written as a reference. Probably MetaInfo is missing for the class and subject XmlKey</exception>exception>
		public static void Write (object target, FileInfo file) 
		{
			XmlFile.CreateRegistration (target);

			FileInfo targetFile = XmlFile.GetTempFile(file.Directory);

			XmlTextWriter writer = new XmlTextWriter(targetFile.FullName, null);

			_fileList[target] = file.FullName;

			//Use automatic indentation for readability.
			writer.Formatting = Formatting.Indented;

			writer.WriteStartDocument();

			string xmlStartElement = (string) MetaInfo.GetAttributeDefault (target.GetType(), "XmlStartElement", target.GetType().Name);
			writer.WriteStartElement(xmlStartElement);

			string xmlNameSpace = (string) MetaInfo.GetAttribute (target.GetType(), "XmlNameSpace");
			if (xmlNameSpace != null) 
			{
				writer.WriteAttributeString("xmlns", xmlNameSpace);
			}

			// Write the actual values
			XmlFile.Write (writer, target, null, null, null, target, null, new ArrayList());

			// end the root element
			//Write the XML to file and close the writer
			writer.WriteEndElement();
			writer.Close();

			file.Delete();
			targetFile.MoveTo (file.FullName);
		}

		#endregion

		#region Internal Write Methods

		/// <summary>
		/// Writes an object to xml. This will write 
		/// 1) the type and assembly of the object to write
		/// 2) xml attributes for all properties of the object which can be written as xml attributes
		/// 3) xml elements for all other properties (this this method is called recursively)
		/// </summary>
		/// <param name="writer">The xml stream</param>
		/// <param name="target">The object to be written</param>
		/// <param name="parent">The object in the xml stream, which is associated with the xml parent element</param>
		/// <param name="targetProperty">Type of the object</param>
		/// <param name="expectedType">Type expected </param>
		/// <param name="root">Object associated with the top of the xml stream</param>
		/// <param name="parentName">Name of the xml parent tag</param>
		/// <param name="writtenObjects">List of all object written so far in this xml stream</param>
		/// <exception cref="System.Exception">Cannot derive key for object which must be written as a reference. Probably MetaInfo is missing for the class and subject XmlKey</exception>exception>
		private static void Write (XmlTextWriter writer, object target, object parent, string targetProperty, Type expectedType, object root, string parentName, ArrayList writtenObjects) 
		{
			if (target == null) 
			{
				return;
			}

			bool reference = false;

			if ((expectedType == null) || (!expectedType.Equals(target.GetType())))
			{
				string xmlTypeName = (string) MetaInfo.GetAttributeDefault (target.GetType().FullName, "XmlTypeName", target.GetType().FullName);
				writer.WriteAttributeString("Type", xmlTypeName);
				if (ObjectSupport.IsLoadedAssembly(target.GetType().Assembly) && !(bool) MetaInfo.GetAttributeDefault(target.GetType().Assembly, "XmlSkipElement", false)) 
				{
					if (target.GetType().Assembly.GlobalAssemblyCache) 
					{
						writer.WriteAttributeString("Assembly", target.GetType().Assembly.FullName);
					}
					else
					{
						writer.WriteAttributeString("Assembly", target.GetType().Assembly.Location);
					}
				}
			}

			// Check whether this object has been written in this xml file already
			// If so, just write the reference definition
			if (CollectionSupport.ContainsObject (writtenObjects, target))
			{
				object key = XmlFile.GetRegisteredKey(root, target);
				if (key == null) 
				{
					throw new Exception ("Could not derive key for referenced object " + target.ToString());
				}
				if ((Boolean) MetaInfo.GetAttributeDefault (target.GetType(), "ObjectIndexAsKey", false)) 
				{
					writer.WriteAttributeString("Reference", key.ToString());
				}
				reference = true;
			}

			IAggregate aggregate = XmlFile.GetAggregate(target);
			aggregate.UpdateAggregate();

			// Find the parent under which the target will be written
/*			string parentProperty = (string) MetaInfo.GetAttributeDefault (target.GetType(), "XmlParent", null);
			if (parentProperty != null) 
			{
				object parentValue = aggregate.GetValue (parentProperty);
				if (parentValue != null) 
				{
					object rootParent = _rootList[parentValue];
					_rootList[target] = rootParent;
				}
			}*/

			// Check whether the object will be written in another file
			// This should occur when
			//	1. The root of this object isn't the same as the root writing at this moment
			//	2. The object is declared that it should be written in it's own file
			// The root is the object at the top of the xml file
			bool anotherFile = XmlFile.DifferentFiles (target, root);

			if (!anotherFile) 
			{
				if (target != root) 
				{
					anotherFile = (Boolean) MetaInfo.GetAttributeDefault (target.GetType(), "XmlFile", false);
					if (anotherFile) 
					{
						_rootList[target] = target;
					}
				}
			}

			if (anotherFile)
			{
				if (_fileList[_rootList[target]] == null) 
				{
					XmlFile.GetForcedRegisteredFile(_rootList[target], XmlFile.GetRegisteredFile(root).Directory);
					XmlFile.Write (_rootList[target]);
				}

				writer.WriteAttributeString("File", FileSupport.GetRelativePath (XmlFile.GetRegisteredFile(root).Directory, new FileInfo((string) _fileList[_rootList[target]])));
				if ((Boolean) MetaInfo.GetAttributeDefault (target.GetType(), "ObjectIndexAsKey", false)) 
				{
					writer.WriteAttributeString("Reference", XmlFile.GetRegisteredKey(_rootList[target], target).ToString());
				}
				reference = true;
			}

			DirectoryInfo baseDirectory = XmlFile.GetRegisteredFile(root).Directory;

			if (XmlFile.IsAttribute(target.GetType())) 
			{
				writer.WriteAttributeString("Value", XmlFile.ToString(target, baseDirectory));
				return;
			}

			if (!reference) 
			{
				_rootList[target] = root;

				// Add the object to the written objects list
				int identifier = writtenObjects.Count + 1;
				writtenObjects.Add(target);

				string identificationString = Convert.ToString(identifier);
				if (!((Boolean) MetaInfo.GetAttributeDefault (target.GetType(), "ObjectIndexAsKey", false))) 
				{
					identificationString = XmlFile.GetKeyFromObject(aggregate, root);
				}

				if (identificationString != null) 
				{
					XmlFile.RegisterKey(root, identificationString, target);
				}

				if ((Boolean) MetaInfo.GetAttributeDefault (target.GetType(), "ObjectIndexAsKey", false)) 
				{
					writer.WriteAttributeString("Identifier", Convert.ToString(identifier));
				}
			}

			// Write all xml attributes
			if (!(target is IList) && !(target is IDictionary))
			{
				string[] property = XmlFile.GetSortedProperties(aggregate);

				// Pass 1: Write all properties which can be represented as an attribute
				for (int i = 0; i < property.Length; i++) 
				{
					if (IsAttribute (aggregate.GetType(property[i]))) 
					{
						if (XmlFile.PropertyWrite (target.GetType(), property[i], reference))
						{
							bool skipElement = (Boolean) MetaInfo.GetAttributeDefault (target.GetType(), property[i], "XmlSkipElement", false);
							if (!skipElement) 
							{
								object targetValue = aggregate.GetValue(property[i]);
								if (targetValue != null) 
								{
									string name = XmlFile.GetElementName(target.GetType(), property[i], reference);
									writer.WriteAttributeString(name, XmlFile.ToString(targetValue, baseDirectory));
								}
							}
						}
					} 
				}
			}

			// Move the target to the default property if any
			string defaultProperty = MetaInfo.GetAttribute (target.GetType(), "XmlDefaultProperty") as string;
			if (defaultProperty != null) 
			{
				parent = target;
				target = aggregate.GetValue (defaultProperty);
				targetProperty = defaultProperty;
				aggregate = XmlFile.GetAggregate(target);
				aggregate.UpdateAggregate();
				parentName = XmlFile.GetElementName (parent.GetType(), defaultProperty, false);
			}

			// Special handling for lists
			if (target is IList) 
			{
				if (parent == null) 
				{
					parent = target;
				}

				string listEntryClass = (string) MetaInfo.GetAttributeDefault (parent.GetType(), targetProperty, "XmlItemType", MetaInfo.GetAttribute (target.GetType(), "XmlItemType"));

				for (int j = 0; j < ((IList) target).Count; j++) 
				{
					string name = XmlFile.GetSingleName(parentName);
					if (CollectionSupport.ContainsObject(writtenObjects, ((IList) target)[j]))
					{
						name = (string) MetaInfo.GetAttributeDefault (parent.GetType(), targetProperty, "XmlRefItemName", MetaInfo.GetAttributeDefault (parent.GetType(), targetProperty, "XmlItemName", name));
					} 
					else 
					{
						name = (string) MetaInfo.GetAttributeDefault (parent.GetType(), targetProperty, "XmlItemName", name);
					}

					writer.WriteStartElement(name);
					XmlFile.Write (writer, ((IList) target)[j], null, null, ObjectSupport.GetType(listEntryClass), root, name, writtenObjects);
					writer.WriteEndElement();  // end Name
				}
			}
			// Special handling for hashtables
			else if (target is IDictionary) 
			{
				if (parent == null) 
				{
					parent = target;
				}

				IDictionaryEnumerator DictionaryEnumerator = ((IDictionary) target).GetEnumerator();
				while (DictionaryEnumerator.MoveNext()) 
				{
					if (DictionaryEnumerator.Value != null) 
					{
						string name = (string) MetaInfo.GetAttributeDefault (parent.GetType(), targetProperty, "XmlItemName", XmlFile.GetSingleName(parentName));
						writer.WriteStartElement(name);
						writer.WriteStartElement("Key");
						XmlFile.Write (writer, DictionaryEnumerator.Key, null, null, null, root, null, writtenObjects);
						writer.WriteEndElement();  // key
						writer.WriteStartElement("Value");
						XmlFile.Write (writer, DictionaryEnumerator.Value, null, null, null, root, null, writtenObjects);
						writer.WriteEndElement();  // value
						writer.WriteEndElement();  // property
					}
				}
			} 
			// Normal write procedure
			else 
			{
				// Get all properties which must be written
				string[] property = XmlFile.GetSortedProperties(aggregate);

				// Pass 2: Write all properties which must be represented as a sub element
				for (int i = 0; i < property.Length; i++) 
				{
					if (!IsAttribute (aggregate.GetType(property[i]))) 
					{
						if (XmlFile.PropertyWrite (target.GetType(), property[i], reference))
						{
							bool skipElement = (Boolean) MetaInfo.GetAttributeDefault (target.GetType(), property[i], "XmlSkipElement", false);
							if (!skipElement) 
							{
								object targetValue = aggregate.GetValue(property[i]);

								if (targetValue != null) 
								{
									string name = XmlFile.GetElementName(target.GetType(), property[i], reference || CollectionSupport.ContainsObject(writtenObjects, targetValue) || XmlFile.DifferentFiles(targetValue, root));
									//Get instance of the attribute.    
									writer.WriteStartElement(name);

									Type type = ObjectSupport.GetType((string) MetaInfo.GetAttributeDefault (target.GetType(), property[i], "XmlType", aggregate.GetType(property[i]).FullName));
									if (!aggregate.CanWrite(property[i]))
									{
										type = targetValue.GetType(); // this enforces that no type information is written; it cannot be used anyway, because the property is read only
									}
									XmlFile.Write (writer, targetValue, target, property[i], type, root, name, writtenObjects);

									writer.WriteEndElement();  // end Name
								}
							}
						}
					}
				}
			}
		}

		/// <summary>
		/// Gets the string representing an object.
		/// This string value is used for writing the object in an xml file.
		/// </summary>
		/// <param name="target">The object a string value is requested for</param>
		/// <param name="directory">Directory, which is used in case of files. Then the relative file path is returned</param>
		/// <returns>The string representation</returns>
		private static string ToString (object target, DirectoryInfo directory) 
		{
			if (target is FileInfo) 
			{
				return FileSupport.GetRelativePath(directory, (FileInfo) target);
			}
			else if (target is DirectoryInfo) 
			{
				return FileSupport.GetRelativePath(directory, (DirectoryInfo) target);
			}
			else if (target is IFormattable) 
			{
				return ((IFormattable) target).ToString (null, _culture);
			}
			else if (target is bool) 
			{
				return ((bool) target).ToString().ToLower();
			}
			else 
			{
				return target.ToString();
			}
		}

		/// <summary>
		/// Gets all properties of an aggregate in a sorted way.
		/// MetaInfo attribute "XmlIndex" is used for the sorting.
		/// </summary>
		/// <param name="aggregate">The aggregate</param>
		/// <returns>Sorted property names</returns>
		private static string[] GetSortedProperties (IAggregate aggregate) 
		{
			SortedList list = new SortedList();
			string[] properties = aggregate.Properties;

			for (int i = 0; i < properties.Length; i++) 
			{
				int index = (int)MetaInfo.GetAttributeDefault (aggregate.Source.GetType(), properties[i], "XmlIndex", 1000);
				string listIndex = index.ToString("D9") + " " + properties[i].ToLower();
				if (!list.Contains (listIndex))
				{
					list.Add (listIndex, properties[i]);
				}
			}

			/*			for (int i = 0; i < list.GetValueList()..Values.Count; i++) 
						{
							propertyList.Add (list.GetByIndex(i));
						}*/

			ArrayList propertyList = new ArrayList(list.Values);
			return (string[]) propertyList.ToArray(typeof(string));
		}

		/// <summary>
		/// Gets the xml element name for a specific property
		/// </summary>
		/// <param name="type">The class type containing the property</param>
		/// <param name="property">The property to write</param>
		/// <param name="reference">Identifies whether the property is to be written completely or just a reference, because the property has been written before</param>
		/// <returns>The xml element name</returns>
		private static string GetElementName(Type type, string property, bool reference) 
		{
			if (reference) 
			{
				return (string) MetaInfo.GetAttributeDefault (type, property, "XmlRefName", MetaInfo.GetAttributeDefault (type, property, "XmlName", property));
			} 
			else 
			{
				return (string) MetaInfo.GetAttributeDefault (type, property, "XmlName", property);
			}
		}

		/// <summary>
		/// Indicates whether two objects are written in the same file. 
		/// This should occur when 1) the root of this object isn't the same as the root writing at this moment
		/// or 2) the object is declared that it should be written in it's own file. 
		/// The root is the object at the top of the xml file
		/// </summary>
		/// <param name="target">First object</param>
		/// <param name="root">Second object</param>
		/// <returns>Indication same file</returns>
		private static bool DifferentFiles (object target, object root) 
		{
			// Find the parent under which the target will be written
			IAggregate aggregate = XmlFile.GetAggregate(target);

			if (_rootList[target] == null) 
			{
				string parentProperty = (string) MetaInfo.GetAttributeDefault (target.GetType(), "XmlParent", null);
				if (parentProperty != null) 
				{
					object parentValue = aggregate.GetValue (parentProperty);
					if (parentValue != null) 
					{
						object rootParent = _rootList[parentValue];
						_rootList[target] = rootParent;
					}
				}
			}

			return (_rootList[target] != null) && (_rootList[target] != root);
		}

		/// <summary>
		/// Indicates whether a property should be written
		/// </summary>
		/// <param name="type">The class type of the object containing the property</param>
		/// <param name="property">The property to write</param>
		/// <param name="reference">Identifies whether the property is to be written completely or just a reference, because the property has been written before</param>
		/// <returns>Boolean indicating whether the property is to be written</returns>
		private static bool PropertyWrite (Type type, string property, bool reference) 
		{
			bool allowed = !reference && ((Boolean) MetaInfo.GetAttributeDefault (type, property, "XmlElement", true));
			allowed = allowed || (reference && (((Boolean) MetaInfo.GetAttributeDefault (type, property, "XmlRefElement", false)) || ((Boolean) MetaInfo.GetAttributeDefault (type, property, "XmlKey", false))));

			return allowed;
		}

		/// <summary>
		/// Gets the single word given a plural word.
		/// For example, plural word "variables" would return "variable".
		/// </summary>
		/// <param name="parentName">The plural word</param>
		/// <returns>The single word, "Item" if nothing specific can be derived</returns>
		private static string GetSingleName(string parentName) 
		{
			if (parentName == null) 
			{
				return "Item";
			}

			string singleName = "";

			if (parentName.EndsWith("List") || parentName.EndsWith("list")) 
			{
				singleName = parentName.Substring (0, parentName.Length - 4);
			}

			if (parentName.EndsWith("Set") || parentName.EndsWith("set")) 
			{
				singleName = parentName.Substring (0, parentName.Length - 3);
			}

			if (parentName.EndsWith("Collection") || parentName.EndsWith("collection")) 
			{
				singleName = parentName.Substring (0, parentName.Length - 10);
			}

			if (parentName.EndsWith("s"))
			{
				singleName = parentName.Substring (0, parentName.Length - 1);
			}

			if (parentName.EndsWith("ies"))
			{
				singleName = parentName.Substring (0, parentName.Length - 3) + "y";
			}

			if (singleName.Trim().Equals(""))
			{
				singleName = "Item";
			}

			return singleName;
		}

		/// <summary>
		/// Gets the property given a specific xml element name
		/// </summary>
		/// <param name="target">The object which will contain the property</param>
		/// <param name="name">The xml element name</param>
		/// <returns>The property name, null if not found</returns>
		private static string GetProperty (IAggregate target, string name) 
		{
			string[] properties = target.Properties;
			for (int i = 0; i < properties.Length; i++) 
			{
				string elementName = XmlFile.GetElementName (target.Source.GetType(), properties[i], false);
				if ((elementName != null) && (elementName.Equals(name)))
				{
					return properties[i];
				}

				elementName = XmlFile.GetElementName (target.Source.GetType(), properties[i], true);
				if ((elementName != null) && (elementName.Equals(name)))
				{
					return properties[i];
				}
			}

			return null;
		}

		/// <summary>
		/// Indicates whether an object can be written as an xml attribute (instead of an xml element)
		/// </summary>
		/// <param name="targetType">The class type of the object which should be written</param>
		/// <returns>Indication of writable as xml attribute</returns>
		private static bool IsAttribute (Type targetType) 
		{
			return (targetType.IsPrimitive) || (targetType.IsEnum) || (targetType.Equals (typeof(string))) || (targetType.Equals (typeof(FileInfo))) || (targetType.Equals (typeof(DirectoryInfo))) || (targetType.Equals(typeof(DateTime)));
		}

		/// <summary>
		/// Gets a temporary file.
		/// When writing an xml file, first a temporary file is used and later it is moved to the actually intended file.
		/// In this way the original file is left untouched when an exception occurs.
		/// </summary>
		/// <param name="TempDirectory">The directory where the temp file should be located</param>
		/// <returns>The temporary file</returns>
		private static FileInfo GetTempFile(DirectoryInfo TempDirectory) 
		{
			int index = 0;
			while (true) 
			{
				FileInfo file = new FileInfo(TempDirectory.FullName + "\\temp" + index + ".xml");
				if (!file.Exists) 
				{
					return file;
				}

				index++;
			}
		}

		#endregion
	}
}
