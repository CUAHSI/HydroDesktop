using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using Oatc.OpenMI.Gui.Core;

namespace Oatc.OpenMI.Gui.ConfigurationEditor
{
	public partial class RunLog : Form
	{
		Run.Log _table = new Run.Log();

		public RunLog(List<CompositionRun.State> cache)
		{
			InitializeComponent();

			_table.Initialise(cache);

			dataGridView1.DataSource = _table;
		}
	}
}
