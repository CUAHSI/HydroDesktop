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
using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Xml;
using System.IO;
using System.Collections;
using System.Threading;
using OpenMI.Standard;
using Oatc.OpenMI.Sdk.Backbone;
using Oatc.OpenMI.Sdk.DevelopmentSupport;


namespace Oatc.OpenMI.Gui.Core
{

    /// <summary>
    /// Summary description for CompositionManager.
    /// </summary>
    public class CompositionManager
    {
        #region Static members

        private static Event _simulationFinishedEvent;
        private static Event _simulationFailedEvent;

        static CompositionManager()
        {
            _simulationFinishedEvent = new Event(EventType.GlobalProgress);
            _simulationFinishedEvent.Description = "Simulation finished successfuly...";

            _simulationFailedEvent = new Event(EventType.GlobalProgress);
            _simulationFailedEvent.Description = "Simulation FAILED...";
        }


        /// <summary>
        /// Special event saying that simulation has finished.
        /// </summary>
        public static IEvent SimulationFinishedEvent
        {
            get { return (_simulationFinishedEvent); }
        }


        /// <summary>
        /// Special event saying that simulation has failed.
        /// </summary>
        public static IEvent SimulationFailedEvent
        {
            get { return (_simulationFailedEvent); }
        }


        /// <summary>
        /// Unique ID of trigger "model".
        /// </summary>
        /// <remarks>Standard models cannot have this ID.</remarks>
        public const string TriggerModelID = "Oatc.OpenMI.Gui.Trigger";

        #endregion

        #region Internal members

        bool _shouldBeSaved;

        Thread _runThread;
        bool _running;
        bool _runPrepareForComputationStarted;
        IListener _runListener;
        bool _runInSameThread;

        ArrayList _models;

        //public ArrayList ModelList
        //{
        //    get { return _models; }
        //}

        ArrayList _connections;
        bool[] _listenedEventTypes;
        DateTime _triggerInvokeTime;
        string _logFileName;
        bool _showEventsInListbox;

        public string _filePath;

        public string FilePath
        {
            get { return _filePath; }
        }

        #endregion

        /// <summary>
        /// Creates a new empty instance of <c>CompositionManager</c> class.
        /// </summary>
        /// <remarks>See <see cref="Initialize">Initialize</see> for more detail.</remarks>
        public CompositionManager()
        {
            Initialize();
        }


        #region Public properties

        /// <summary>
        /// Gets list of all models (ie. instances of <see cref="UIModel">UIModel</see> class) in composition.
        /// </summary>
        public ArrayList Models
        {
            get { return (_models); }
        }


        /// <summary>
        /// Gets list of all connections (ie. instances of <see cref="UIConnection">UIConnection</see> class) in composition.
        /// </summary>
        public ArrayList Connections
        {
            get { return (_connections); }
        }


        /// <summary>
        /// Gets array of <c>bool</c> describing which events should be listened during simulation run.
        /// </summary>
        /// <remarks>Array has <see cref="EventType.NUM_OF_EVENT_TYPES">EventType.NUM_OF_EVENT_TYPES</see>
        /// elements. See <see cref="EventType">EventType</see>, <see cref="Run">Run</see> for more detail.
        /// </remarks>
        public bool[] ListenedEventTypes
        {
            get { return (_listenedEventTypes); }
            /*	set
                {
                    Debug.Assert( value.Length == (int)EventType.NUM_OF_EVENT_TYPES );
                    _listenedEventTypes = value;
                    _shouldBeSaved = true;
                }*/
        }


        /// <summary>
        /// Time when trigger should be invoked.
        /// </summary>
        /// <remarks>See <see cref="EventType">EventType</see> and <see cref="Run">Run</see> for more detail.</remarks>
        public DateTime TriggerInvokeTime
        {
            get { return (_triggerInvokeTime); }
            set
            {
                if (_triggerInvokeTime != value)
                {
                    _triggerInvokeTime = value;
                    _shouldBeSaved = true;
                }
            }
        }


        /// <summary>
        /// Relative or absolute path to text file for logging simulation run.
        /// </summary>
        public string LogToFile
        {
            get { return (_logFileName); }
            set
            {
                if (_logFileName != value)
                {
                    _logFileName = value;
                    _shouldBeSaved = true;
                }
            }
        }


        /// <summary>
        /// Gets or sets whether simulation should be run in same thread. By default it's <c>false</c>.
        /// </summary>
        /// <remarks>
        /// This is only recommendation of composition author, you can override
        /// this setting while calling <see cref="Run">Run</see> method. For example
        /// if running from console, simulation is always executed in same thread.
        /// </remarks>
        public bool RunInSameThread
        {
            get { return (_runInSameThread); }
            set
            {
                if (_runInSameThread != value)
                {
                    _shouldBeSaved = true;
                    _runInSameThread = value;
                }
            }
        }


        /// <summary>
        /// Gets or sets whether events should be showed in list-box during simulation in UI.
        /// </summary>
        public bool ShowEventsInListbox
        {
            get { return (_showEventsInListbox); }
            set
            {
                if (_showEventsInListbox != value)
                {
                    _showEventsInListbox = value;
                    _shouldBeSaved = true;
                }
            }
        }


        /// <summary>
        /// Gets or sets wheather composition was changed and should be saved to OPR file.
        /// </summary>
        /// <remarks>See <see cref="SaveToFile">SaveToFile</see>.</remarks>
        public bool ShouldBeSaved
        {
            get { return (_shouldBeSaved); }
            set { _shouldBeSaved = value; }
        }


        #endregion

        #region Public methods

        /// <summary>
        /// Initializes this composition.
        /// </summary>
        public void Initialize()
        {
            _models = new ArrayList();
            _connections = new ArrayList();
            _listenedEventTypes = new bool[(int)EventType.NUM_OF_EVENT_TYPES];
            for (int i = 0; i < (int)EventType.NUM_OF_EVENT_TYPES; i++)
                _listenedEventTypes[i] = false;

            _triggerInvokeTime = new DateTime(1900, 1, 1);

            _showEventsInListbox = true;

            _logFileName = "CompositionRun.log";

            _shouldBeSaved = false;

            _runPrepareForComputationStarted = false;
            _runThread = null;
            _running = false;
            _runListener = null;
            _runInSameThread = false;

        }


        /// <summary>
        /// Releases all models and intializes this composition.
        /// </summary>
        public void Release()
        {
            RemoveAllModels();
            Initialize();
        }


        /// <summary>
        /// Adds new model to this composition.
        /// </summary>
        /// <param name="omiFilename">Relative or absolute path to OMI file describing the model.</param>
        /// <param name="directory">Directory <c>omiFilename</c> is relative to, or <c>null</c> if <c>omiFilename</c> is absolute or relative to current directory.</param>
        /// <returns>Returns newly added model.</returns>
        /// <remarks>See <see cref="Utils.GetFileInfo">Utils.GetFileInfo</see> for more info about how
        /// specified file is searched.</remarks>
        public UIModel AddModel(string directory, string omiFilename)
        {
            UIModel newUiModel;

            //UIModel temp = new UIModel();
            //temp.ReadOMIFile(directory, omiFilename);
            //check to see if model component implements ITrigger
            //if (temp.LinkableComponent is ILinkableComponent)
            //{

            //}
            //else
            if (omiFilename == TriggerModelID)
            {
                newUiModel = UIModel.NewTrigger();
                newUiModel.Color = System.Drawing.Color.Salmon;
                newUiModel.Rect = new System.Drawing.Rectangle(100, 100, 100, 100);
                newUiModel.Shape = DotSpatial.Modeling.Forms.ModelShape.Triangle;
            }
            else
            {
                newUiModel = new UIModel();
                newUiModel.Color = System.Drawing.Color.LightGreen;
                newUiModel.Shape = DotSpatial.Modeling.Forms.ModelShape.Ellipse;
                
                newUiModel.Rect = new System.Drawing.Rectangle(100, 100, 115, 75);

                newUiModel.ReadOMIFile(directory, omiFilename);
            }

            // check whether ModelID is unique and also calculate newUiModel's positon
            foreach (UIModel uiModel in _models)
            {
                if (newUiModel.ModelID == uiModel.ModelID)
                    throw (new Exception("Composition already contains model with ModelID \"" + newUiModel.ModelID + "\" "));

                if (newUiModel.Rect.X == uiModel.Rect.X && newUiModel.Rect.Y == uiModel.Rect.Y)
                {
                    newUiModel.Rect.X = newUiModel.Rect.X + newUiModel.Rect.Width / 2;
                    newUiModel.Rect.Y = newUiModel.Rect.Y + newUiModel.Rect.Height / 2;
                }
            }

            _models.Add(newUiModel);

            _shouldBeSaved = true;

            return (newUiModel);
        }


        /// <summary>
        /// Removes specified model from composition.
        /// </summary>
        /// <param name="model">Model to be removed.</param>
        /// <remarks>The <c>Dispose</c> method is called on the model.</remarks>
        public void RemoveModel(UIModel model)
        {
            // first remove all links from/to this model
            UIConnection[] copyOfLinks = (UIConnection[])_connections.ToArray(typeof(UIConnection));
            foreach (UIConnection uiLink in copyOfLinks)
                if (uiLink.AcceptingModel == model || uiLink.ProvidingModel == model)
                    RemoveConnection(uiLink);

            try
            {
                // We call Finish() after computation finished,
                // Dispose() when removing models
                model.LinkableComponent.Dispose();
            }
            catch
            {
                // we don't care about just disposed model, so do nothing...
            }

            _shouldBeSaved = true;
            _models.Remove(model); // remove model itself
        }


        /// <summary>
        /// Removes all model from composition.
        /// </summary>
        /// <remarks>See <see cref="RemoveModel">RemoveModel</see> for more detail.</remarks>
        public void RemoveAllModels()
        {
            UIModel[] copyOfModels = (UIModel[])_models.ToArray(typeof(UIModel));
            foreach (UIModel model in copyOfModels)
                RemoveModel(model);
        }


        /// <summary>
        /// Creates new connection between two models in composition.
        /// </summary>
        /// <param name="providingModel">Source model</param>
        /// <param name="acceptingModel">Target model</param>
        /// <remarks>Connection between two models is just abstraction which can hold links between models.
        /// The direction of connection and its links is same. There can be only one connection between two models.</remarks>
        public void AddConnection(UIModel providingModel, UIModel acceptingModel)
        {
            if (providingModel == acceptingModel)
                throw (new Exception("Cannot connect model with itself."));

            // Check whether both models exist
            bool providingFound = false, acceptingFound = false;
            foreach (UIModel model in _models)
            {
                if (model == providingModel)
                    providingFound = true;
                if (model == acceptingModel)
                    acceptingFound = true;
            }
            if (!providingFound || !acceptingFound)
                throw (new Exception("Cannot find providing or accepting."));

            // check whether this link isn't already here (if yes, do nothing)
            foreach (UIConnection link in _connections)
                if (link.ProvidingModel == providingModel && link.AcceptingModel == acceptingModel)
                    return;

            // if providing model is trigger, do nothing
            if (providingModel.ModelID == TriggerModelID)
                return;

            // if accepting model is trigger, remove all other trigger connections
            if (acceptingModel.ModelID == TriggerModelID)
            {
                ArrayList connectionsToRemove = new ArrayList();
                foreach (UIConnection uiLink in _connections)
                {
                    if (uiLink.AcceptingModel.ModelID == TriggerModelID
                        || uiLink.ProvidingModel.ModelID == TriggerModelID)
                        connectionsToRemove.Add(uiLink);
                }
                foreach (UIConnection uiLink in connectionsToRemove)
                    RemoveConnection(uiLink);
            }

            _connections.Add(new UIConnection(providingModel, acceptingModel));

            _shouldBeSaved = true;
        }


        /// <summary>
        /// Removes connection between two models.
        /// </summary>
        /// <param name="connection">Connection to be removed.</param>
        public void RemoveConnection(UIConnection connection)
        {
            // remove ILinks from both connected components
            if (!_runPrepareForComputationStarted)
                foreach (ILink link in connection.Links)
                {
                    string linkID = link.ID;
                    ILinkableComponent
                        sourceComponent = link.SourceComponent,
                        targetComponent = link.TargetComponent;


                    sourceComponent.RemoveLink(linkID);
                    targetComponent.RemoveLink(linkID);
                }

            _connections.Remove(connection);

            _shouldBeSaved = true;
        }


        /// <summary>
        /// Determines whether composition has trigger.
        /// </summary>
        /// <returns>Returns <c>true</c> if composition has trigger, otherwise returns <c>false</c>.</returns>
        /// <remarks>See <see cref="GetTrigger">GetTrigger</see> for more detail.</remarks>
        public bool HasTrigger()
        {
            return (GetTrigger() != null);
        }


        /// <summary>
        /// Gets trigger "model".
        /// </summary>
        /// <returns>Returns trigger, or <c>null</c> if composition has no trigger.</returns>
        public Trigger GetTrigger()
        {
            foreach (UIModel uiModel in _models)
                if (uiModel.ModelID == TriggerModelID)
                    return ((Trigger)uiModel.LinkableComponent);
            return (null);
        }


        /// <summary>
        /// Saves composition to OmiEd Project XML file (OPR).
        /// </summary>
        /// <param name="filePath">Path to OPR file.</param>
        public void SaveToFile(string filePath)
        {
            _filePath = filePath;
            
            XmlDocument xmlDocument = new XmlDocument();
            SaveToXmlDocument(xmlDocument);
            
            xmlDocument.Save(filePath);

            _shouldBeSaved = false;
        }


        /// <summary>
        /// Loads composition from OmiEd Project XML file (OPR).
        /// </summary>
        /// <param name="filePath">Path to OPR file.</param>
        public void LoadFromFile(string filePath)
        {
            _filePath = filePath;
            
            XmlDocument xmlDocument = new XmlDocument();
            FileInfo fileInfo = new FileInfo(filePath);

            xmlDocument.Load(fileInfo.FullName);

            // omi files will be searched relatively from filePath's path
            LoadFromXmlDocument(fileInfo.DirectoryName, xmlDocument);

            _shouldBeSaved = false;
        }


        /// <summary>
        /// Reloads the composition.
        /// </summary>
        /// <remarks>Reloading is useful if you want to run simulation multiple times in one execution time.
        /// Some models aren't able to run simulation after it was already run, and may crash in such case.
        /// That's because they need to create new instance of them, on which the <c>Initialize</c> method
        /// is called. 
        /// Reloading is done same way like when you save the composition to OPR file, restarts the application,
        /// and open this OPR file again. Of course, it is done only internally in the memory.</remarks>
        public void Reload()
        {
            XmlDocument xmlDocument = new XmlDocument();

            SaveToXmlDocument(xmlDocument);

            // preserve members that aren't saved to XML
            bool oldShouldBeSaved = _shouldBeSaved;

            Release();
            AssemblySupport.ReleaseAll();

            LoadFromXmlDocument(Path.GetDirectoryName(_filePath), xmlDocument);

            _shouldBeSaved = oldShouldBeSaved;
        }


        /// <summary>
        /// Calculates time horizon of the simulation,
        /// ie. time between earliest model start and latest model end.
        /// </summary>
        /// <returns>Returns simulation time horizon.</returns>
        public ITimeSpan GetSimulationTimehorizon()
        {
            TimeStamp start = new TimeStamp(double.MaxValue),
                end = new TimeStamp(double.MinValue);

            foreach (UIModel model in _models)
            {
                if (model.ModelID == CompositionManager.TriggerModelID)
                    continue;
                start.ModifiedJulianDay = Math.Min(start.ModifiedJulianDay, model.LinkableComponent.TimeHorizon.Start.ModifiedJulianDay);
                end.ModifiedJulianDay = Math.Max(end.ModifiedJulianDay, model.LinkableComponent.TimeHorizon.End.ModifiedJulianDay);
            }

            Debug.Assert(start.ModifiedJulianDay < end.ModifiedJulianDay);

            return (new Oatc.OpenMI.Sdk.Backbone.TimeSpan(start, end));
        }


        /// <summary>
        /// Runs simulation.
        /// </summary>
        /// <param name="runListener">Simulation listener.</param>
        /// <param name="runInSameThread">If <c>true</c>, simulation is run in same thread like caller,
        /// ie. method blocks until simulation don't finish. If <c>false</c>, simulation is
        /// run in separate thread and method returns immediately.</param>
        /// <remarks>
        /// Simulation is run the way that trigger invokes <see cref="ILinkableComponent.GetValues">ILinkableComponent.GetValues</see>
        /// method of the model it's connected to
        /// at the time specified by <see cref="TriggerInvokeTime">TriggerInvokeTime</see> property.
        /// If you need to use more than one listener you can use <see cref="ProxyListener">ProxyListener</see>
        /// class or <see cref="ProxyMultiThreadListener">ProxyMultiThreadListener</see> if <c>runInSameThread</c> is <c>false</c>.
        /// </remarks>
        public void Run(IListener runListener, bool runInSameThread)
        {
            if (!HasTrigger())
                throw (new Exception("Composition has no trigger."));
            if (_running)
                throw (new Exception("Simulation is already running."));

            _running = true;
            _runListener = runListener;

            try
            {
                TimeStamp runToTime = new TimeStamp(CalendarConverter.Gregorian2ModifiedJulian(_triggerInvokeTime));

                // Create informative message
                if (_runListener != null)
                {
                    StringBuilder description = new StringBuilder();
                    description.Append("Starting simulation at ");
                    description.Append(DateTime.Now.ToString());
                    description.Append(",");

                    description.Append(" composition consists from following models:\n");
                    foreach (UIModel model in _models)
                    {
                        description.Append(model.ModelID);
                        description.Append(", ");
                    }

                    // todo: add more info?

                    Event theEvent = new Event(EventType.Informative);
                    theEvent.Description = description.ToString();
                    _runListener.OnEvent(theEvent);
                }

                _runPrepareForComputationStarted = true;

                // prepare for computation
                if (_runListener != null)
                {
                    Event theEvent = new Event(EventType.Informative);
                    theEvent.Description = "Preparing for computation....";
                    _runListener.OnEvent(theEvent);
                }
                foreach (UIModel uimodel in _models)
                {
                    if (_runListener != null)
                    {
                        Event theEvent = new Event(EventType.Informative);
                        theEvent.Description = "Calling Prepare() method of model " + uimodel.ModelID;
                        _runListener.OnEvent(theEvent);
                    }
                    uimodel.LinkableComponent.Prepare();
                }

                // subscribing event listener to all models
                if (_runListener != null)
                {
                    Event theEvent = new Event(EventType.Informative);
                    theEvent.Description = "Subscribing proxy event listener....";
                    _runListener.OnEvent(theEvent);

                    for (int i = 0; i < _runListener.GetAcceptedEventTypeCount(); i++)
                        foreach (UIModel uimodel in _models)
                        {
                            theEvent = new Event(EventType.Informative);
                            theEvent.Description = "Calling Subscribe() method with EventType." + ((EventType)i).ToString() + " of model " + uimodel.ModelID;
                            _runListener.OnEvent(theEvent);

                            for (int j = 0; j < uimodel.LinkableComponent.GetPublishedEventTypeCount(); j++)
                                if (uimodel.LinkableComponent.GetPublishedEventType(j) == _runListener.GetAcceptedEventType(i))
                                {
                                    uimodel.LinkableComponent.Subscribe(_runListener, _runListener.GetAcceptedEventType(i));
                                    break;
                                }
                        }
                }



                if (!runInSameThread)
                {
                    // creating run thread
                    if (_runListener != null)
                    {
                        Event theEvent = new Event(EventType.Informative);
                        theEvent.Description = "Creating run thread....";
                        _runListener.OnEvent(theEvent);
                    }

                    _runThread = new Thread(new ThreadStart(RunThreadFunction));

                    // starting thread...
                    if (_runListener != null)
                    {
                        Event theEvent = new Event(EventType.GlobalProgress);
                        theEvent.Description = "Starting run thread....";
                        _runListener.OnEvent(theEvent);
                    }

                    _runThread.Start();
                }
                else
                {
                    // run simulation in same thread (for example when running from console)
                    if (_runListener != null)
                    {
                        Event theEvent = new Event(EventType.Informative);
                        theEvent.Description = "Running simulation in same thread....";
                        _runListener.OnEvent(theEvent);
                    }
                    RunThreadFunction();
                }
            }
            catch (System.Exception e)
            {
                if (_runListener != null)
                {
                    Event theEvent = new Event(EventType.Informative);
                    theEvent.Description = "Exception occured while initiating simulation run: " + e.ToString();
                    _runListener.OnEvent(theEvent);
                    _runListener.OnEvent(SimulationFailedEvent); // todo: add info about time to this event
                }
            }

        }


        /// <summary>
        /// Stops the simulation.
        /// </summary>
        /// <remarks>This method has effect only if simulation is run in separate thread
        /// (see <see cref="Run">Run</see> method).
        /// This method calls <see cref="Thread.Abort()">Abort</see> method on the simulation thread.</remarks>
        public void Stop()
        {
            if (_running && _runThread != null)
                _runThread.Abort();
            _runThread = null;
        }


        #endregion

        #region Private methods

        /// <summary>
        /// Saves composition to XML document.
        /// </summary>
        /// <param name="xmlDocument">XML document</param>
        private void SaveToXmlDocument(XmlDocument xmlDocument)
        {
            XmlElement xmlRoot = xmlDocument.CreateElement("guiComposition");

            xmlRoot.SetAttribute("version", "1.0");

            // save UIModels
            XmlElement models = xmlDocument.CreateElement("models");
            foreach (UIModel model in _models)
            {
                XmlElement xmlUiModel = xmlDocument.CreateElement("model");

                string oprDirectory = Path.GetDirectoryName(_filePath);

                xmlUiModel.SetAttribute("omi", FileSupport.GetRelativePath(oprDirectory, model.OmiFilename));
                xmlUiModel.SetAttribute("rect_x", model.Rect.X.ToString());
                xmlUiModel.SetAttribute("rect_y", model.Rect.Y.ToString());
                xmlUiModel.SetAttribute("rect_width", model.Rect.Width.ToString());
                xmlUiModel.SetAttribute("rect_height", model.Rect.Height.ToString());

                models.AppendChild(xmlUiModel);
            }
            xmlRoot.AppendChild(models);

            // save UILinks
            XmlElement links = xmlDocument.CreateElement("links");
            foreach (UIConnection uiLink in _connections)
            {
                XmlElement xmlUiLink = xmlDocument.CreateElement("uilink");

                xmlUiLink.SetAttribute("model_providing", uiLink.ProvidingModel.ModelID);
                xmlUiLink.SetAttribute("model_accepting", uiLink.AcceptingModel.ModelID);

                // save OpenMI Links
                foreach (Link link in uiLink.Links)
                {
                    XmlElement xmlLink = xmlDocument.CreateElement("link");
                    xmlLink.SetAttribute("id", link.ID);
                    xmlLink.SetAttribute("source_elementset", link.SourceElementSet.ID);
                    xmlLink.SetAttribute("source_quantity", link.SourceQuantity.ID);
                    xmlLink.SetAttribute("target_elementset", link.TargetElementSet.ID);
                    xmlLink.SetAttribute("target_quantity", link.TargetQuantity.ID);

                    // save selected DataOperations
                    for (int i = 0; i < link.DataOperationsCount; i++)
                    {
                        XmlElement xmlDataOperation = xmlDocument.CreateElement("dataoperation");
                        xmlDataOperation.SetAttribute("id", link.GetDataOperation(i).ID);

                        // save DataOperation's writeable arguments
                        for (int j = 0; j < link.GetDataOperation(i).ArgumentCount; j++)
                            if (!link.GetDataOperation(i).GetArgument(j).ReadOnly)
                            {
                                XmlElement xmlArgument = xmlDocument.CreateElement("argument");
                                xmlArgument.SetAttribute("key", link.GetDataOperation(i).GetArgument(j).Key);
                                xmlArgument.SetAttribute("value", link.GetDataOperation(i).GetArgument(j).Value);
                                xmlDataOperation.AppendChild(xmlArgument);
                            }
                        xmlLink.AppendChild(xmlDataOperation);
                    }
                    xmlUiLink.AppendChild(xmlLink);
                }
                links.AppendChild(xmlUiLink);
            }
            xmlRoot.AppendChild(links);

            // save run properties
            XmlElement runProperties = xmlDocument.CreateElement("runproperties");

            CultureInfo currentCulture = Thread.CurrentThread.CurrentCulture;
            Thread.CurrentThread.CurrentCulture = new CultureInfo("");
            
            StringBuilder str = new StringBuilder((int)EventType.NUM_OF_EVENT_TYPES);
            for (int i = 0; i < _listenedEventTypes.Length; i++)
                str.Append(_listenedEventTypes[i] ? "1" : "0");

            runProperties.SetAttribute("listenedeventtypes", str.ToString());
            runProperties.SetAttribute("triggerinvoke", _triggerInvokeTime.ToString());

            runProperties.SetAttribute("runinsamethread", _runInSameThread ? "1" : "0");

            runProperties.SetAttribute("showeventsinlistbox", _showEventsInListbox ? "1" : "0");

            runProperties.SetAttribute("logfilename", _logFileName == null ? "" : _logFileName);

            xmlRoot.AppendChild(runProperties);

            xmlDocument.AppendChild(xmlRoot);

            Thread.CurrentThread.CurrentCulture = currentCulture;
        }


        /// <summary>
        /// Loads composition from XML document.
        /// </summary>
        /// <param name="omiRelativeDirectory">Directory the OMI files are relative to.</param>
        /// <param name="xmlDocument">XML document</param>
        private void LoadFromXmlDocument(string omiRelativeDirectory, XmlDocument xmlDocument)
        {
            // once you choose to load new file, all previously opened models are closed
            Release();

            CultureInfo currentCulture = Thread.CurrentThread.CurrentCulture;
            Thread.CurrentThread.CurrentCulture = new CultureInfo("");
            

            XmlElement xmlRoot = (XmlElement)xmlDocument.ChildNodes[0];
            XmlElement xmlModels = (XmlElement)xmlRoot.ChildNodes[0];
            XmlElement xmlLinks = (XmlElement)xmlRoot.ChildNodes[1];

            // run properties aren't mandatory
            XmlElement xmlRunProperties = null;
            if (xmlRoot.ChildNodes.Count > 2)
                xmlRunProperties = (XmlElement)xmlRoot.ChildNodes[2];

            // check
            if (xmlRoot.GetAttribute("version") != "1.0")
                throw (new FormatException("Version of file not supported. Currently supported only version '1.0'"));
            if (xmlModels.Name != "models"
                || xmlLinks.Name != "links")
                throw (new FormatException("Unknown file format ('models' or 'links' tag not present where expected)."));
            if (xmlRunProperties != null)
                if (xmlRunProperties.Name != "runproperties")
                    throw (new FormatException("Unknown file format ('runproperties' tag not present where expected)."));

            // read UIModels
            foreach (XmlElement xmlUiModel in xmlModels.ChildNodes)
            {
                try
                {
                    UIModel uiModel = AddModel(omiRelativeDirectory, xmlUiModel.GetAttribute("omi"));

                    uiModel.Rect.X = Int32.Parse(xmlUiModel.GetAttribute("rect_x"));
                    uiModel.Rect.Y = Int32.Parse(xmlUiModel.GetAttribute("rect_y"));
                    uiModel.Rect.Width = Int32.Parse(xmlUiModel.GetAttribute("rect_width"));
                    uiModel.Rect.Height = Int32.Parse(xmlUiModel.GetAttribute("rect_height"));
                }
                catch (Exception e)
                {
                    throw (new Exception(
                        "Model cannot be added to composition due to exception.\n" +
                        "OMI filename: " + xmlUiModel.GetAttribute("omi") + "\n" +
                        "Exception: " + e.ToString()));
                }
            }

            // read UILinks
            foreach (XmlElement xmlUiLink in xmlLinks.ChildNodes)
            {
                // find models corresponding to this UIConnection
                UIModel providingModel = null, acceptingModel = null;
                foreach (UIModel uiModel in _models)
                    if (uiModel.ModelID == xmlUiLink.GetAttribute("model_providing"))
                    {
                        providingModel = uiModel;
                        break;
                    }
                foreach (UIModel uiModel in _models)
                    if (uiModel.ModelID == xmlUiLink.GetAttribute("model_accepting"))
                    {
                        acceptingModel = uiModel;
                        break;
                    }

                if (providingModel == null || acceptingModel == null)
                {
                    throw (new Exception(
                        "One model (or both) corresponding to this link cannot be found...\n" +
                        "Providing model: " + xmlUiLink.GetAttribute("model_providing") + "\n" +
                        "Accepting model: " + xmlUiLink.GetAttribute("model_accepting")));
                }

                // construct UIConnection
                UIConnection uiLink = new UIConnection(providingModel, acceptingModel);

                // read OpenMI Links
                foreach (XmlElement xmlLink in xmlUiLink.ChildNodes)
                {
                    // find corresponding exchange items
                    IOutputExchangeItem outputExchangeItem = null;
                    IInputExchangeItem inputExchangeItem = null;

                    int count = providingModel.LinkableComponent.OutputExchangeItemCount;
                    string sourceElementSetID = xmlLink.GetAttribute("source_elementset");
                    string sourceQuantityID = xmlLink.GetAttribute("source_quantity");
                    for (int i = 0; i < count; i++)
                        if (sourceElementSetID == providingModel.LinkableComponent.GetOutputExchangeItem(i).ElementSet.ID
                            && sourceQuantityID == providingModel.LinkableComponent.GetOutputExchangeItem(i).Quantity.ID)
                        {
                            outputExchangeItem = providingModel.LinkableComponent.GetOutputExchangeItem(i);
                            break;
                        }

                    for (int i = 0; i < acceptingModel.LinkableComponent.InputExchangeItemCount; i++)
                        if (xmlLink.GetAttribute("target_elementset") == acceptingModel.LinkableComponent.GetInputExchangeItem(i).ElementSet.ID
                            && xmlLink.GetAttribute("target_quantity") == acceptingModel.LinkableComponent.GetInputExchangeItem(i).Quantity.ID)
                        {
                            inputExchangeItem = acceptingModel.LinkableComponent.GetInputExchangeItem(i);
                            break;
                        }

                    if (outputExchangeItem == null || inputExchangeItem == null)
                        throw (new Exception(
                            "Cannot find exchange item\n" +
                            "Providing model: " + providingModel.ModelID + "\n" +
                            "Accepting model: " + acceptingModel.ModelID + "\n" +
                            "Source ElementSet: " + xmlLink.GetAttribute("source_elementset") + "\n" +
                            "Source Quantity: " + xmlLink.GetAttribute("source_quantity") + "\n" +
                            "Target ElementSet: " + xmlLink.GetAttribute("target_elementset") + "\n" +
                            "Target Quantity: " + xmlLink.GetAttribute("target_quantity")));


                    // read selected DataOperation's IDs, find their equivalents
                    // in outputExchangeItem, and add these to link
                    ArrayList dataOperationsToAdd = new ArrayList();

                    foreach (XmlElement xmlDataOperation in xmlLink.ChildNodes)
                        for (int i = 0; i < outputExchangeItem.DataOperationCount; i++)
                        {
                            IDataOperation dataOperation = outputExchangeItem.GetDataOperation(i);
                            if (dataOperation.ID == xmlDataOperation.GetAttribute("id"))
                            {
                                // set data operation's arguments if any
                                foreach (XmlElement xmlArgument in xmlDataOperation.ChildNodes)
                                {
                                    string argumentKey = xmlArgument.GetAttribute("key");
                                    for (int j = 0; j < dataOperation.ArgumentCount; j++)
                                    {
                                        IArgument argument = dataOperation.GetArgument(j);
                                        if (argument.Key == argumentKey && !argument.ReadOnly)
                                            argument.Value = xmlArgument.GetAttribute("value");
                                    }
                                }

                                dataOperationsToAdd.Add(dataOperation);
                                break;
                            }
                        }

                    // now construct the Link...
                    Link link = new Link(
                        providingModel.LinkableComponent,
                        outputExchangeItem.ElementSet,
                        outputExchangeItem.Quantity,
                        acceptingModel.LinkableComponent,
                        inputExchangeItem.ElementSet,
                        inputExchangeItem.Quantity,
                        "No description available.",
                        xmlLink.GetAttribute("id"),
                        dataOperationsToAdd);


                    // ...add the link to the list
                    uiLink.Links.Add(link);

                    // and add it to both LinkableComponents
                    uiLink.AcceptingModel.LinkableComponent.AddLink(link);
                    uiLink.ProvidingModel.LinkableComponent.AddLink(link);
                }

                // add new UIConnection to list of connections
                _connections.Add(uiLink);
            }

            // read run properties (if present)
            if (xmlRunProperties != null)
            {
                string str = xmlRunProperties.GetAttribute("listenedeventtypes");
                if (str.Length != (int)EventType.NUM_OF_EVENT_TYPES)
                    throw (new FormatException("Invalid number of event types in 'runproperties' tag, expected " + EventType.NUM_OF_EVENT_TYPES + ", but only " + str.Length + " found."));
                for (int i = 0; i < (int)EventType.NUM_OF_EVENT_TYPES; i++)
                    switch (str[i])
                    {
                        case '1': _listenedEventTypes[i] = true; break;
                        case '0': _listenedEventTypes[i] = false; break;
                        default: throw (new FormatException("Unknown format of 'listenedeventtypes' attribute in 'runproperties' tag."));
                    }
                _triggerInvokeTime = DateTime.Parse(xmlRunProperties.GetAttribute("triggerinvoke"));

                _logFileName = xmlRunProperties.GetAttribute("logfilename");
                if (_logFileName != null)
                {
                    _logFileName = _logFileName.Trim();
                    if (_logFileName == "")
                        _logFileName = null; // if not set, logfile isn't used
                }


                str = xmlRunProperties.GetAttribute("showeventsinlistbox");
                if (str == null || str == "" || str == "1")
                    _showEventsInListbox = true; // if not set, value is true
                else
                    _showEventsInListbox = false;

                str = xmlRunProperties.GetAttribute("runinsamethread");
                if (str == "1")
                    _runInSameThread = true;
            }


            Thread.CurrentThread.CurrentCulture = currentCulture;
        }


        /// <summary>
        /// This method is called in <see cref="Run">Run</see> method.
        /// </summary>
        private void RunThreadFunction()
        {
            Trigger trigger = GetTrigger();

            Debug.Assert(trigger != null);

            Thread.Sleep(0);

            try
            {
                // run it !!!
                trigger.Run(new TimeStamp(CalendarConverter.Gregorian2ModifiedJulian(TriggerInvokeTime)));

                // close models down
                if (_runListener != null)
                {
                    Event theEvent = new Event(EventType.Informative);
                    theEvent.Description = "Closing models down...";
                    _runListener.OnEvent(theEvent);
                }

                foreach (UIModel uimodel in _models)
                {
                    if (_runListener != null)
                    {
                        string ModelID = uimodel.ModelID;
                        Event theEvent = new Event(EventType.Informative);
                        theEvent.Description = "Calling Finish() on model " + ModelID;
                        _runListener.OnEvent(theEvent);
                    }
                    uimodel.LinkableComponent.Finish();
                }

                // thread finishes - send well known event
                if (_runListener != null)
                {
                    _simulationFinishedEvent.Description = "Simulation finished successfuly at " + DateTime.Now.ToString() + "...";
                    _runListener.OnEvent(SimulationFinishedEvent);
                }
            }
            catch (Exception e)
            {
                if (_runListener != null)
                {
                    Event theEvent = new Event(EventType.Informative);
                    theEvent.Description = "Exception occured during simulation: " + e.ToString();
                    _runListener.OnEvent(theEvent);

                    _simulationFailedEvent.Description = "Simulation FAILED at " + DateTime.Now.ToString() + "...";
                    _runListener.OnEvent(SimulationFailedEvent);
                }
            }
            finally
            {
                _running = false;
                _runListener = null; // release listener
            }
        }


        #endregion
    }
}
