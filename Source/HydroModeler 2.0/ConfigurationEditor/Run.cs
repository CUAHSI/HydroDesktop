using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading; // TODO remove with Sleep?
using System.IO;

using OpenMI.Standard2;
using Oatc.OpenMI.Gui.Core;

namespace Oatc.OpenMI.Gui.ConfigurationEditor
{
	public partial class Run : Form
	{
		enum RunStatus { Runable, Running, Done, Aborted, Failed, }

		RunStatus _runStatus = RunStatus.Runable;
		FileInfo _oprFile;
		Table _table;
		CompositionRun _runManager;
		List<CompositionRun.State> _logCache;
        int _nOprIndexTrigger = -1;
        DateTime _startTime;
        TimeSpan _timeSpan;

		public Run()
		{
			InitializeComponent();
		}

		public class Table : DataTable
		{
			public enum EColumns { Component = 0, Status, Progress, Changed, Events, }

			Type[] _columnTypes = new Type[] { 
				typeof(String),
				typeof(String),
				typeof(Int32),
				typeof(String),
				typeof(Int32),
				};

			String[] _columnNames = new String[] { 
				EColumns.Component.ToString(),
				EColumns.Status.ToString(),
				EColumns.Progress.ToString(),
				"Last event updated",
				EColumns.Events.ToString(),
				};

			public void Initialise(List<string> captions)
			{
				for (int n = 0; n < Enum.GetNames(typeof(EColumns)).Length; ++n)
					base.Columns.Add(new DataColumn(_columnNames[n], _columnTypes[n]));

				DataRow row;

				foreach (string caption in captions)
				{
					row = NewRow();
					row[(int)EColumns.Component] = caption;
					row[(int)EColumns.Status] = "";
                    row[(int)EColumns.Progress] = 0;
                    row[(int)EColumns.Changed] = "";
					row[(int)EColumns.Events] = 0;

					Rows.Add(row);
				}
			}

			public DataGridViewCell Cell(EColumns column, int oprIndex, DataGridView grid)
			{
				return grid.Rows[oprIndex].Cells[(int)column];
			}
		}

		public class Log : DataTable
		{
			public enum EColumns { DateTime = 0, Component, Details}

			Type[] _columnTypes = new Type[] { 
				typeof(DateTime),
				typeof(String),
				typeof(String),
				};

			String[] _columnNames = new String[] { 
				EColumns.DateTime.ToString(),
				EColumns.Component.ToString(),
				EColumns.Details.ToString(),
				};

			List<DataRow> _rows = new List<DataRow>();

			public void Initialise(List<CompositionRun.State> states)
			{
				for (int n = 0; n < Enum.GetNames(typeof(EColumns)).Length; ++n)
					base.Columns.Add(new DataColumn(_columnNames[n], _columnTypes[n]));

				DataRow row;
				LinkableComponentStatusChangeEventArgs status;
				ExchangeItemChangeEventArgs exchange;
				StringBuilder sb;
                int? progress;

				foreach (CompositionRun.State state in states)
				{
					row = NewRow();
					row[(int)EColumns.DateTime] = state.LastEventUpdate;

					if (state.StatusArgs != null)
					{
						status = state.StatusArgs;

                        progress = state.Progress;

						row[(int)EColumns.Component] = string.Format("{0}: {1}",
							state.OprIndex.ToString(),
							status.LinkableComponent.Caption);
						row[(int)EColumns.Details] = progress != null 
                            ? string.Format(
							    "[{0}%], Component Status: {1} => {2}",
                                progress.ToString(),
							    status.OldStatus.ToString(),
							    status.NewStatus.ToString())                                            
                            : string.Format(
							    "Component Status: {0} => {1}",
							    status.OldStatus.ToString(),
							    status.NewStatus.ToString());
					}
					else
					{
						exchange = state.ExchangeArgs;

						row[(int)EColumns.Component] = string.Format("{0}: {1}",
							state.OprIndex.ToString(),
							exchange.ExchangeItem.Component.Caption);

						sb = new StringBuilder(string.Format(
								"{0}: {1}, ",
								exchange.ExchangeItem is IInput ? "Target" : "Source",
								exchange.ExchangeItem.Caption));

						if (exchange.Message != string.Empty)
							sb.Append(exchange.Message);

						row[(int)EColumns.Details] = sb.ToString();
					}

					Rows.Add(row);
				}
			}

			public DataGridViewCell Cell(EColumns column, int oprIndex, DataGridView grid)
			{
				return grid.Rows[oprIndex].Cells[(int)column];
			}
		}

        void UpdateTitleText(int? progress)
        {
            string text = progress == null
                ? string.Format("Run: {0}", _oprFile.Name)
                : string.Format("Run [{0}%]: {1}", progress.Value.ToString(),  _oprFile.Name);

            if (text != Text)
                Text = text;
        }

		public void Initialise(string oprFile)
		{
			if (oprFile == "")
				Open();

			Status = RunStatus.Runable;
			_oprFile = new FileInfo(oprFile);
			_table = new Table();
			_runManager = new CompositionRun();
			_logCache = new List<CompositionRun.State>();

			List<string> captions = new List<string>();

            List<UIModel> models;
            List<UIConnection> connections;

            Opr.Load(_oprFile, out models, out connections);

			if (models.Count == 0)
				throw new InvalidDataException("No models found in " + _oprFile.FullName);

			// LinkableComponents

            int nOprIndex = -1;

            foreach (UIModel model in models)
			{
                ++nOprIndex;

                captions.Add(model.LinkableComponent.Caption);

                if (model.IsTrigger)
                    _nOprIndexTrigger = nOprIndex;
			}

			_table.Initialise(captions);

			dataGridView1.DataSource = _table;

			btnOk.Text = "Run";

            UpdateTitleText(null);
			Refresh();
		}

		private void btnLog_Click(object sender, EventArgs e)
		{
		}

		private void btnOk_Click(object sender, EventArgs e)
		{
			if (Status == RunStatus.Runable)
				RunComposition();
			else if (Status == RunStatus.Running)
				Abort();
			else
				Reload();
		}

		private void Abort()
		{
			_runManager.Cancel();
		}

		private void Reload()
		{
			FileInfo oprFile = _oprFile;

            AssemblySupport.ReleaseAll();

            progressBar1.Value = 0;
            progressBar1.Invalidate();
            UpdateTitleText(null);

			Initialise(oprFile.FullName);
		}

		private void RunComposition()
		{
			Status = RunStatus.Running;

            progressBar1.Minimum = 0;
            progressBar1.Maximum = 100;
            progressBar1.Value = 0;
            UpdateTitleText(0);

            _startTime = DateTime.Now;

			_runManager.AllowCancel = true; // as in UI
			_runManager.RunAsync(_oprFile, RunProgress, RunCompleted);
		}

		RunStatus Status
		{
			get { return _runStatus; }
			set
			{
				_runStatus = value;

                string elapsed = string.Format("{0},{1},{2}.{3}",
                    _timeSpan.Hours, _timeSpan.Minutes,
                    _timeSpan.Seconds, _timeSpan.Milliseconds);

				switch (_runStatus)
				{
					case RunStatus.Aborted:
						btnOk.Text = "Reload";
                        labelMessage.Text = string.Format("Aborted [{0}%]\r\nElapsed {1}",
                            progressBar1.Value, elapsed);
                        break;
					case RunStatus.Failed:
						btnOk.Text = "Reload";
                        labelMessage.Text = string.Format("Failed [{0}%]\r\nElapsed {1}",
                            progressBar1.Value, elapsed);
                        break;
					case RunStatus.Done:
						btnOk.Text = "Reload";
                        labelMessage.Text = string.Format("Run completed\r\nElapsed {0}",
                            elapsed);
                        break;
					case RunStatus.Runable:
						btnOk.Text = "Run";
                        labelMessage.Text = "Loaded";
                        break;
					case RunStatus.Running:
						btnOk.Text = "Abort";
                        // labelMessage.Text updated elsewhere
                        break;
					default:
						btnOk.Text = "Reload";
                        labelMessage.Text = "Requires reload";
                        break;
				}

				Refresh();
			}
		}
		

		void RunCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			if (_runManager.Canceled || e.Cancelled)
			{
				// Note that due to a race condition in 
				// the DoWork event handler, the Cancelled
				// flag may not have been set, even though
				// CancelAsync was called.
				Status = RunStatus.Aborted;
			}
			else if (e.Error != null)
			{
				Status = RunStatus.Failed;
				MessageBox.Show(Utils.ToString(e.Error),
					"Run failed ...", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			else
				Status = RunStatus.Done;
		}

		void RunProgress(object sender, ProgressChangedEventArgs e)
		{
			lock (this)
			{
				if (e.UserState != null
					&& (e.UserState is CompositionRun.State))
				{
					CompositionRun.State state
						= (CompositionRun.State)e.UserState;

					DataGridViewCell cellComponent = _table.Cell(Table.EColumns.Component, state.OprIndex, dataGridView1);

					// Status

					DataGridViewCell cellStatus = _table.Cell(Table.EColumns.Status, state.OprIndex, dataGridView1);

					ILinkableComponent iLC = state.StatusArgs != null
						? state.StatusArgs.LinkableComponent
						: state.ExchangeArgs.ExchangeItem.Component;

					cellStatus.Value = iLC != null ? iLC.Status.ToString() : "";
					dataGridView1.InvalidateCell(cellStatus);

					// Changes Count

					DataGridViewCell cellChanges = _table.Cell(Table.EColumns.Events, state.OprIndex, dataGridView1);

					cellChanges.Value = (int)cellChanges.Value + 1;
					dataGridView1.InvalidateCell(cellChanges);

					// Changed Time

					DataGridViewCell cellChanged = _table.Cell(Table.EColumns.Changed, state.OprIndex, dataGridView1);
					cellChanged.Value = state.LastEventUpdate;
					dataGridView1.InvalidateCell(cellChanged);

                    // Changed Progress

                    int? progress = state.Progress;

                    if (progress != null)
                    {
                        DataGridViewCell cellProgress = _table.Cell(Table.EColumns.Progress, state.OprIndex, dataGridView1);
                        cellProgress.Value = progress.Value;
                        dataGridView1.InvalidateCell(cellProgress);

                        if (state.OprIndex == _nOprIndexTrigger
                            && progressBar1.Value != progress.Value)
                        {
                            progressBar1.Value = progress.Value;
                            progressBar1.Invalidate();

                            UpdateTitleText(progress);

                            _timeSpan = DateTime.Now - _startTime;

                            string elapsed = string.Format("{0},{1},{2}.{3}",
                                _timeSpan.Hours, _timeSpan.Minutes, 
                                _timeSpan.Seconds, _timeSpan.Milliseconds);

                            labelMessage.Text = string.Format("Running [{0}%]\r\nElapsed {1}", 
                                progress.Value, elapsed);
                        }
                    }

					dataGridView1.Refresh();

					_logCache.Add(state);
				}
			}
		}

		public static void RunComposition(string oprFile)
		{
			try
			{
				Run run = new Run();
				run.Initialise(oprFile);
				run.ShowDialog();
			}
			catch (Exception e)
			{
				MessageBox.Show(Utils.ToString(e),
					"Run UI failed ...", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void openToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Open();
		}

		private void Open()
		{
			using (OpenFileDialog dlg = new OpenFileDialog())
			{
				dlg.Filter = "Projects (*.opr)|*.opr|All files|*.*";
				dlg.Multiselect = false;
				dlg.CheckFileExists = true;
				dlg.CheckPathExists = true;
				dlg.Title = "Open project...";

				if (dlg.ShowDialog(this) != DialogResult.OK)
					return;

				_oprFile = new FileInfo(dlg.FileName);

				Reload();
			}
		}

		private void logToolStripMenuItem_Click(object sender, EventArgs e)
		{
			RunLog logView = new RunLog(_logCache);
			logView.ShowDialog(this);
		}
	}
}
