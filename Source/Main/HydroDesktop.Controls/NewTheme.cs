using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using HydroDesktop.Database;
using System.Data.Common;
using System.Collections;
using HydroDesktop.Interfaces;

namespace HydroDesktop.Controls
{
    public partial class NewTheme : Form
    {
        #region Constructor
        public NewTheme(ArrayList checkedIDList)
        {
            InitializeComponent();
            _checkedIDList = checkedIDList;
        }
        #endregion

        #region variables

        private ArrayList _checkedIDList;

        #endregion

        #region Close Form
        /// <summary>
        /// Close the form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bntCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        #endregion

        #region Save Theme
        /// <summary>
        /// Save the text value to 
        /// "DataThemeDescriptions" & "DataThemes" table
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bntOK_Click(object sender, EventArgs e)
        {
            try
            {
                //Set up the datatable path
                DbOperations db = new DbOperations
                    (HydroDesktop.Configuration.Settings.Instance.DataRepositoryConnectionString, DatabaseTypes.SQLite);

                //Update Table DataThemeDescriptions
                string myTime = DateTime.Now.ToString();
                StringBuilder sqlTheme = new StringBuilder();
                sqlTheme.Append("INSERT INTO DataThemeDescriptions(ThemeName,ThemeDescription,DateCreated)");
                sqlTheme.Append(" VALUES('" + textBox1.Text.Trim().ToString() + "','" + textBox2.Text.Trim().ToString() + "','");
                sqlTheme.Append("" + myTime + "')");

                DbCommand cmd = db.CreateCommand(sqlTheme.ToString());
                cmd.Connection.Open();
                cmd.CommandText = sqlTheme.ToString();
                cmd.ExecuteNonQuery();
                cmd.Connection.Close();

                //Update Table DataThemes
                string serSql = "SELECT ThemeID FROM DataThemeDescriptions WHERE ThemeName='" +
                    textBox1.Text.Trim().ToString() + "' and ThemeDescription='" + textBox2.Text.Trim().ToString() + "'";
                DataTable serDat = db.LoadTable("serDat", serSql);
                
                int newThemeID = Convert.ToInt32(serDat.Rows[0][0].ToString());
                //insert data
                string sql ="";
                DbCommand cmd2 = db.CreateCommand(sql);
                cmd2.Connection.Open();

                foreach (int mySeries in _checkedIDList)
                {
                    sql = "INSERT INTO DataThemes (ThemeID,SeriesID) VALUES('"+
                        newThemeID+"','"+mySeries+"')";
                    cmd2.CommandText = sql;
                    cmd2.ExecuteNonQuery();
                }
                cmd2.Connection.Close();
                this.Close();
            }
            catch(Exception ey)
            {
                MessageBox.Show(ey.Message);
            }
        }
        #endregion
    }
}
