// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Form1.cs" company="MBSoft">
//   MBSoft
// </copyright>
// <summary>
//   
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FileSearch
{
    using System;
    using System.Data;
    using System.Data.SqlClient;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Windows.Forms;

    using GenerateConnectionString;

    /// <summary>
    /// Defines the Form1 type
    /// </summary>
    public partial class CleanYourDatabase : Form
    {
        #region Constants and Fields

        /// <summary>
        /// string connectionString
        /// </summary>
        private string connectionString;

        /// <summary>
        /// string path
        /// </summary>
        private string path;

        /// <summary>
        /// Thread thread
        /// </summary>
        private Thread thread;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CleanYourDatabase"/> class.
        /// </summary>
        /// <history>
        /// By: Wael Refaat
        /// Email: wael.r.roushdy@gmil.com
        /// On: 12:09 AM
        /// </history>
        public CleanYourDatabase()
        {
            InitializeComponent();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Handles the Click event of the AboutToolStripMenuItem control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <history>
        /// By: Wael Refaat
        /// Email: wael.r.roushdy@gmil.com
        /// On: 9:24 PM
        /// </history>
        private void AboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var about = new About();
            about.ShowDialog();
        }

        /// <summary>
        /// Applies the seach.
        /// </summary>
        /// <history>
        /// By: Wael Refaat
        /// Email: wael.r.roushdy@gmil.com
        /// On: 13/05/2010
        /// </history>
        private void ApplySeach()
        {
            var test = this.GetAllStoredProcedures();
            var files = Directory.GetFiles(this.path, "*.cs", SearchOption.AllDirectories).ToList();
            files.AddRange(Directory.GetFiles(this.path, "*.vb", SearchOption.AllDirectories).ToList());
            files.AddRange(Directory.GetFiles(this.path, "*.aspx", SearchOption.AllDirectories).ToList());
            files.AddRange(Directory.GetFiles(this.path, "*.dbml", SearchOption.AllDirectories).ToList());
            files.AddRange(Directory.GetFiles(this.path, "*.edmx", SearchOption.AllDirectories).ToList());

            progressBar1.Minimum = 0;
            progressBar1.Maximum = test.Rows.Count;

            progressBar1.Value = 0;

            for (int i = 0; i < test.Rows.Count; i++)
            {
                var storedProcedureName = Convert.ToString(test.Rows[i]["name"]);
                label1.Text = storedProcedureName;
                bool exist = false;
                foreach (string filePath in files)
                {
                    StreamReader streamReader = new StreamReader(filePath);
                    string content = streamReader.ReadToEnd();
                    if (content.IndexOf(storedProcedureName) > -1)
                    {
                        exist = true;
                        break;
                    }

                    streamReader.Dispose();
                }

                if (!exist)
                {
                    listBox1.Items.Add(storedProcedureName);
                    label3.Text = listBox1.Items.Count.ToString();
                }

                progressBar1.Value += 1;
            }

            progressBar1.Value = 0;
            label1.Text = string.Empty;
            MessageBox.Show("Completed Successfuly");
        }

        /// <summary>
        /// Handles the Click event of the BtnBrowseProject control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <history>
        /// By: Wael Refaat
        /// Email: wael.r.roushdy@gmil.com
        /// On: 9:24 PM
        /// </history>
        private void BtnBrowseProject_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.ShowDialog();
            this.path = this.folderBrowserDialog1.SelectedPath;
            this.lblPath.Text = this.path;
        }

        /// <summary>
        /// Handles the Click event of the BtnGenerateScript control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <history>
        /// By: Wael Refaat
        /// Email: wael.r.roushdy@gmil.com
        /// On: 12:08 AM
        /// </history>
        private void BtnGenerateScript_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(this.path) || string.IsNullOrWhiteSpace(this.connectionString))
            {
                MessageBox.Show(
                    "Apply the search operation first !!", "MBSoft", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (listBox1.Items.Count == 0)
            {
                MessageBox.Show(
                    "There are no unused stored procedures in you project(s)", 
                    "MBSoft", 
                    MessageBoxButtons.OK, 
                    MessageBoxIcon.Exclamation);
                return;
            }

            if (listBox1.Items.Count > 0)
            {
                var script = this.GenerateScript();
                saveFileDialog1.Filter = "SQL Script|*.sql";
                saveFileDialog1.Title = "Export Script";
                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    if (!string.IsNullOrWhiteSpace(saveFileDialog1.FileName))
                    {
                        if (!Directory.Exists(saveFileDialog1.FileName))
                        {
                            FileStream fileStream = new FileStream(saveFileDialog1.FileName, FileMode.CreateNew);
                            StreamWriter writer = new StreamWriter(fileStream);
                            writer.Write(script);
                            writer.Close();
                            fileStream.Dispose();
                        }
                        else
                        {
                            FileStream fileStream = new FileStream(saveFileDialog1.FileName, FileMode.Open);
                            StreamWriter writer = new StreamWriter(fileStream);
                            writer.Write(script);
                            writer.Close();
                            fileStream.Dispose();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Handles the Click event of the BtnGetConnectionString control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <history>
        /// By: Wael Refaat
        /// Email: wael.r.roushdy@gmil.com
        /// On: 9:24 PM
        /// </history>
        private void BtnGetConnectionString_Click(object sender, EventArgs e)
        {
            var conn = new GetConnectionString();
            conn.ShowDialog();

            this.connectionString = conn.ConnectionString;
            lblConnectionString.Text = this.connectionString;
        }

        /// <summary>
        /// Handles the Click event of the BtnRunScript control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <history>
        /// By: Wael Refaat
        /// Email: wael.r.roushdy@gmil.com
        /// On: 12:08 AM
        /// </history>
        private void BtnRunScript_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(this.path) || string.IsNullOrWhiteSpace(this.connectionString))
            {
                MessageBox.Show(
                    "Apply the search operation first !!", "MBSoft", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (listBox1.Items.Count == 0)
            {
                MessageBox.Show(
                    "There are no unused stored procedures in you project(s)", 
                    "MBSoft", 
                    MessageBoxButtons.OK, 
                    MessageBoxIcon.Exclamation);
                return;
            }

            var dialogResult =
                MessageBox.Show(
                    @"By this action you will remove these stored procedures from the database. Are you sure you want to perform this action?", 
                    "MBSoft", 
                    MessageBoxButtons.YesNo, 
                    MessageBoxIcon.Warning);
            if (dialogResult == DialogResult.No)
            {
                return;
            }

            if (!string.IsNullOrWhiteSpace(this.connectionString))
            {
                SqlConnection conn = new SqlConnection(this.connectionString);
                SqlCommand command = new SqlCommand();
                command.Connection = conn;
                command.CommandType = CommandType.Text;
                command.CommandText = this.GenerateScript();
                try
                {
                    conn.Open();
                    command.ExecuteNonQuery();
                    conn.Close();
                }
                catch (Exception exception)
                {
                    MessageBox.Show(exception.Message);
                }
            }
            else
            {
                MessageBox.Show("Specify ConnectionString first");
            }
        }

        /// <summary>
        /// Handles the Click event of the BtnStart control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <history>
        /// By: Wael Refaat
        /// Email: wael.r.roushdy@gmil.com
        /// On: 12:08 AM
        /// </history>
        private void BtnStart_Click(object sender, EventArgs e)
        {
            if (listBox1.Items.Count > 0)
            {
                listBox1.Items.Clear();
            }

            if (string.IsNullOrWhiteSpace(this.path) || string.IsNullOrWhiteSpace(this.connectionString))
            {
                MessageBox.Show("Path or connection srting are not correct");
                return;
            }

            this.thread = new Thread(new ThreadStart(this.ApplySeach));
            this.thread.IsBackground = true;
            this.thread.Start();
        }

        /// <summary>
        /// Handles the Click event of the ExitToolStripMenuItem control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <history>
        /// By: Wael Refaat
        /// Email: wael.r.roushdy@gmil.com
        /// On: 9:24 PM
        /// </history>
        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Handles the Load event of the Form1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <history>
        /// By: Wael Refaat
        /// Email: wael.r.roushdy@gmil.com
        /// On: 12:08 AM
        /// </history>
        private void Form1_Load(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Generates the script.
        /// </summary>
        /// <returns>script Generates</returns>
        /// <history>
        /// By: Wael Refaat
        /// Email: wael.r.roushdy@gmil.com
        /// On: 12:08 AM
        /// </history>
        private string GenerateScript()
        {
            var script = "-- This script generated by Wael Refaat's apps" + Environment.NewLine;
            script += @"Begin Try
begin transaction " + Environment.NewLine;

            if (listBox1.Items.Count > 0)
            {
                script = this.listBox1.Items.Cast<string>().Aggregate(
                    script, 
                    (current, item) => current + string.Format("drop PROCEDURE {0} {1}print 'procedure {0} deleted' {1}", item, Environment.NewLine));
            }

            script += "print 'Operation completed successfully'";
            script += @"commit transaction
End Try
Begin Catch
rollback
print 'An Error Occured, rolling back operation'
END Catch";
            return script;
        }

        /// <summary>
        /// Gets all stored procedures.
        /// </summary>
        /// <returns>DataTable of all stored procedures</returns>
        /// <history>
        /// By: Wael Refaat
        /// Email: wael.r.roushdy@gmil.com
        /// On: 13/05/2010
        /// </history>
        private DataTable GetAllStoredProcedures()
        {
            var query =
                string.Format(
                    "SELECT [name] FROM sysobjects WHERE xtype = 'P' AND [name] NOT LIKE 'aspnet_%' ORDER BY [name]");

            SqlConnection connection = new SqlConnection(this.connectionString);
            SqlCommand command = new SqlCommand();
            command.Connection = connection;
            command.CommandType = CommandType.Text;
            command.CommandText = query;
            DataSet filled = new DataSet();
            try
            {
                using (connection)
                {
                    connection.Open();

                    SqlDataAdapter dataAdapter = new SqlDataAdapter(command);
                    dataAdapter.Fill(filled);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                connection.Close();
            }

            if (filled.Tables.Count > 0)
            {
                return filled.Tables[0];
            }

            return new DataTable("ERROR");
        }

        /// <summary>
        /// Handles the Click event of the HowToUseToolStripMenuItem control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <history>
        /// By: Wael Refaat
        /// Email: wael.r.roushdy@gmil.com
        /// On: 9:24 PM
        /// </history>
        private void HowToUseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var how = new HowToUse();
            how.ShowDialog();
        }

        #endregion
    }
}