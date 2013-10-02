using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Reflection;

using OpenMI.Standard2;
using Oatc.OpenMI.Gui.Controls;
using Oatc.OpenMI.Gui.Core;

namespace Oatc.OpenMI.Gui.ConfigurationEditor
{
    public partial class FactoriesDialog : Form
    {
        UIOutputItem _itemOut;
        UIInputItem _itemIn;
		List<UIOutputItem> _adapters = new List<UIOutputItem>();
		List<UIAdaptedFactory> _factories
			= new List<UIAdaptedFactory>();

        bool AddFactory(UIAdaptedFactory iFactory)
		{
			foreach (UIAdaptedFactory factory in _factories)
				if (factory.Factory.GetType() == iFactory.GetType())
					return false;

			_factories.Add(iFactory);

            return true;
		}

        public FactoriesDialog(List<UIModel> models, UIOutputItem itemOut, UIInputItem itemIn)
        {
            _itemOut = itemOut;
            _itemIn = itemIn;

            UIAdaptedFactory factory;
            string firstSourceFactory = null;

            foreach (UIModel model in models)
            {
                foreach (IAdaptedOutputFactory iFactory in model.LinkableComponent.AdaptedOutputFactories)
                {
                    factory = new UIAdaptedFactory();

                    if (_itemOut.Component == model.LinkableComponent)
                    {
                        factory.InitialiseAsNative(iFactory.Id, model.LinkableComponent);

                        if (firstSourceFactory == null)
                            firstSourceFactory = factory.ToString();
                    }
                    else
                        factory.InitialiseAs3rdParty(iFactory.GetType(), model.LinkableComponent);

                    AddFactory(factory);
                }
            }
                        
            InitializeComponent();

			groupBox2.Text = string.Format(
				"Factory sources that adapt \"{0}\" -> \"{1}\"",
				itemOut.Caption, itemIn.Caption);

			UpdateUIFactories();

            if (firstSourceFactory != null)
                cbFactories.SelectedIndex = cbFactories.FindStringExact(firstSourceFactory);

            UpdateControlEnabling();
        }

		public List<UIOutputItem> Adapters
        {
            get
            {
				List<UIOutputItem> adapters = new List<UIOutputItem>(clbAdapters.CheckedItems.Count);
                foreach (object o in new ArrayList(clbAdapters.CheckedItems))
					adapters.Add((UIOutputItem)o);
                return adapters;
            }
        }

		void UpdateUIFactories()
		{
			cbFactories.Items.Clear();

			foreach (IAdaptedOutputFactory factory in _factories)
				cbFactories.Items.Add(factory);

			cbFactories.SelectedIndex = cbFactories.Items.Count - 1;

			UpdateUIAdapters();
		}

        void UpdateUIAdapters()
        {
            clbAdapters.Items.Clear();

			if (cbFactories.SelectedItem == null)
				return;

			UIAdaptedFactory factory = (UIAdaptedFactory)cbFactories.SelectedItem;

            foreach (IIdentifiable id in factory.GetAvailableAdaptedOutputIds(_itemOut, _itemIn))
                clbAdapters.Items.Add(
					factory.NewAdaptedUIOutputItem(id.Id, _itemOut, _itemIn));

            clbAdapters.Invalidate();
        }

		private void OnAddFactory(object sender, EventArgs e)
		{
			OpenFileDialog dlg = new OpenFileDialog();
			dlg.Filter = "Assemblies (*.dll)|*.dll|Executables (*.exe)|*.exe|All files (*.*)|*.*";

			if (dlg.ShowDialog() != DialogResult.OK)
				return;

			try
			{
				AddFactories(new FileInfo(dlg.FileName));
			}
			catch (Exception ex)
			{
				MessageBox.Show("Add Factories ...", ex.Message,
					MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void AddFactories(FileInfo assemblyFileInfo)
        {
			if (!assemblyFileInfo.Exists)
				throw new Exception(
					"File not found: " + assemblyFileInfo.FullName);

			Assembly assembly = Assembly.LoadFile(assemblyFileInfo.FullName);
			
			List<Type> factoryTypes = new List<Type>();

			foreach (Type type in assembly.GetExportedTypes())
			{
                // need a parameterless constructor
                if (type.GetConstructor(Type.EmptyTypes) == null)
                    continue;

				foreach (Type inter in type.GetInterfaces())
				{
					if (inter == typeof(IAdaptedOutputFactory))
					{
						factoryTypes.Add(type);
						break;
					}
				}
			}

            if (factoryTypes.Count == 0)
            {
                MessageBox.Show("No types derived from IAdaptedOutputFactory with parameterless constructors found",
                    "Add Factories", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            UIAdaptedFactory factory;

            foreach (Type type in factoryTypes)
            {
                factory = new UIAdaptedFactory();
                factory.InitialiseAs3rdParty(assemblyFileInfo, type.ToString());
                AddFactory(factory);
            }

			UpdateUIFactories();
        }

        private void OnFactoryDetails(object sender, EventArgs e)
        {
            UIAdaptedFactory factory = _factories[cbFactories.SelectedIndex];

            string s = string.Format("Type: {0}\r\nAssembly: {1}",
                factory.Type.ToString(),
                factory.Assembly.FullName
                );

            MessageBox.Show(s, "Selected factory details", 
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void OnSelectionChanged(object sender, EventArgs e)
        {
            UpdateControlEnabling();
        }

        void UpdateControlEnabling()
        {
            btnAdd.Enabled = clbAdapters.CheckedIndices.Count > 0;
        }
    }
}
