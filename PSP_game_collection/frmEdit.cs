using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.Odbc;
using System.Data.OleDb; //for data base connections

namespace PSP_game_collection
{
    public partial class frmEdit : Form
    {
        public frmEdit()
        {
            InitializeComponent();
            
        }

        private void frmEdit_Load(object sender, EventArgs e)
        {
            AddYears();
            int intID = Convert.ToInt32(lblID.Text);
            string sql = "Select * FROM PSP_GameCollection WHERE [ID]=" + intID;
            LoadTitle(sql);
        }

        private void AddYears()
        {
            int intYear = Convert.ToInt32(DateTime.Now.Year.ToString());
            int index = 0;
            
            //for intYear to 1900
            for (; intYear >= 1990; index++)
            {
                cboYear.Items.Add(intYear);
                //index = index + 1;

                intYear = intYear - 1;
            }

        }

        private string Directory()
        {
            string Dir = System.IO.Directory.GetCurrentDirectory(); //Gets the Current Directory of the programs executable
            // example path stored in "Dir"// C:\Users\Lonnie\Documents\AnimeCollection\AnimeCollection\bin\Debug


            return Dir;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            CloseForm();
        }

        private void UpdateTitle()
        {
            int intID = Convert.ToInt32(lblID.Text);
            string strName = txtName.Text;
            string strGenre = txtGenre.Text;
            int intYear = Convert.ToInt32(cboYear.Text);

            string SQLsearch = "UPDATE PSP_GameCollection SET [Title Name] = '" + strName + "', [Genre] = '" + strGenre +
                "', [Year Released] = " + intYear + " WHERE [ID] =" + intID;
            ExecuteQuery(SQLsearch);
        }

        private void ExecuteQuery(string sql)
        {

            char chrSlash = (char)92;
            string folder = chrSlash + "Database" + chrSlash;

            string fileName = folder + "PSP_collection.accdb"; //database filename

            string strDbPath = Directory() + fileName; //Gets the Current Directory of the programs executable 

            string strConnect = @"Driver={Microsoft Access Driver (*.mdb, *.accdb)};Dbq=" + strDbPath;

            string strMessage = "";

            OdbcConnection conn = null;
            //OdbcDataAdapter dadapter = null;
            if (!txtName.Text.Equals(""))
            {
                strMessage = "ID: " + lblID.Text + " Title: " + txtName.Text + " has been Sucessfully Updated.";

                conn = new OdbcConnection(strConnect); //creates a new database connection
                conn.Open(); //opens connection

                OdbcCommand cmd = new OdbcCommand(sql, conn);

                cmd.ExecuteNonQuery();
                //MsgBox(strMessage, MsgBoxStyle.OkOnly, "Record Sucessfully Updated!!!")
                MessageBox.Show(strMessage, "Update Successful", MessageBoxButtons.OK);

                conn.Close();


                CloseForm();

            }
            else
            {
                strMessage = "TextBoxes for Title Name, Format, and Suggested Retail Price cannot be blank.";
                //MsgBo(strMessage, MsgBoxStyle.OkOnly, "Invalid Input.")
                MessageBox.Show(strMessage, "Error", MessageBoxButtons.OK);
            }
            
        }
        private void ClearCombox()
        {
            //clears items in combo box
            for (int index = 0;  index < cboYear.Items.Count - 1; index++)
            {
                cboYear.Items.RemoveAt(0);
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            ClearAll();
        }
        private void ClearAll()
        {
            string strControl = "";
            //Control txtBox = controls;
            foreach (Control c in this.Controls)
            {
                //txtBox.Controls.ToString

                strControl = c.GetType().ToString();
                if (c.GetType().ToString() == "System.Windows.Forms.TextBox")//txtBox is TextBox)
                {
                    c.Text = "";

                }
                
            }

        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            UpdateTitle();
        }


        private void CloseForm() 
        {
            ClearCombox();
            ClearAll();
            this.Close();

            Application.OpenForms["frmMain"].Show();
            //CallDB()
        }
        private void LoadTitle(string sql)
        {

            char chrSlash = (char)92;
            string folder = chrSlash + "Database" + chrSlash;

            string fileName = folder + "PSP_collection.accdb"; //database filename

            string strDbPath = Directory() + fileName; //Gets the Current Directory of the programs executable 

            string strConnect = @"Driver={Microsoft Access Driver (*.mdb, *.accdb)};Dbq=" + strDbPath;

            string strMessage = "";

            OdbcConnection conn = null;
            //OdbcDataAdapter dadapter = null;
     
            strMessage = "ID: " + lblID.Text + " Title: " + txtName.Text + " has been Sucessfully Updated.";

            conn = new OdbcConnection(strConnect); //creates a new database connection
            conn.Open(); //opens connection

            OdbcCommand cmd = new OdbcCommand(sql, conn);

            OdbcDataReader myReader = cmd.ExecuteReader(CommandBehavior.SingleResult);// .ExecuteNonQuery();

            myReader.Read();

            txtName.Text = myReader[1].ToString();
            txtGenre.Text = myReader[2].ToString();
            cboYear.Text = myReader[3].ToString();

            myReader.Close();
            conn.Close();
                

            
        }
    }
}
