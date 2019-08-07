//'Author: Lonnie Coates, Jr.
//'Project Name: Anime Database 
//'Date: July 1, 2014

//'Objective: Connect to Anime_DB.accdb file and let the user query the database by title name in the textbox along with saving search...
//'...results to a Excel file that use the user can name


//Tip: In your Project, expand the "References", find the Microsoft Office Interop reference....
//...Right click it and select properties, and change "Embed Interop Types" to false

//'uses Reference: Microsoft Excel 12 Object Library on "COM" tab

using Excel = Microsoft.Office.Interop.Excel;
using System;
using System.Data;
using System.Windows.Forms;
using System.Data.Odbc;

namespace PSP_game_collection
{
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();

            DisplayAll();

            DisplayOutput(true);
        }

        private string Directory()
        {
            string Dir = System.IO.Directory.GetCurrentDirectory(); //Gets the Current Directory of the programs executable
            
            return Dir;
        }
        
        private void SearchDB() //Search by what the user types in the txtSearch textbox
        {
            string title = txtInput.Text;
            string columnName = dgvPSP.Columns[1].Name;
            string condition = "WHERE ([" + columnName + "] LIKE '%" + title + "%')";


            string SQLsearch = "Select * FROM PSP_GameCollection " + condition;
            ExecuteNonQuery(SQLsearch);

            txtResults.Text = "Search Results for: " + title;

        }
        private void CallDB()
        {
            DisplayAll();
        }
        private void DisplayAll() //Displays All Records for database
        {
            string SQLsearch = "Select * FROM PSP_GameCollection";
            ExecuteNonQuery(SQLsearch);

            lblTotal.Text = Convert.ToString(dgvPSP.RowCount - 1);

            lblCurrentNo.Text = Convert.ToString(dgvPSP.RowCount - 1);
        }

        private string DBLocation()
        {
            char chrSlash = (char)92;
            string folder = chrSlash + "Database" + chrSlash;
            string fileName = folder + "PSP_collection.accdb"; //database filename

            string strDbPath = Directory() + fileName; //Gets the Current Directory of the programs executable 

            return strDbPath;
        }


        private void ExecuteNonQuery(string sql)
        {
            string strDbPath = DBLocation();

            string strConnect = @"Driver={Microsoft Access Driver (*.mdb, *.accdb)};Dbq=" + strDbPath;

            OdbcConnection conn = null;
            OdbcDataAdapter dadapter = null;
            try
            {
                conn = new OdbcConnection(strConnect); //creates a new database connection
                conn.Open(); //opens connection

                dadapter = new OdbcDataAdapter(sql, conn); //creates data adpater to be used for the program's datagrid

                DataTable table = new DataTable();
                dadapter.Fill(table);

                this.dgvPSP.DataSource = table; // links the datagrid to a local copy of the database table
                
            }

            finally
            {
                if (conn != null) conn.Close(); //closes database connection
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {

           string strMessage = "Are you sure you want to close this application?";
           var varAnswer = MessageBox.Show(strMessage, "Exit Application", MessageBoxButtons.YesNo, 
               MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2);

           if (varAnswer == DialogResult.Yes)
           {
               Application.Exit();
           }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            SearchDB();

            DisplayOutput(false);
        }

        private void DisplayOutput(bool boolFirstDisplay)
        {

            string strOutput;
            int i = 0;

            strOutput = "";
            try
                {
                //If this is not the first the first time DisplayOutput is being called...
                if (boolFirstDisplay == false)
                {
                    //find currently selected row in datagrid view
                    i = Convert.ToInt32(dgvPSP.CurrentCell.RowIndex.ToString());

                    //Store Selected title name in strOutput 
                    strOutput = "Title Name: " + dgvPSP.Rows[i].Cells[1].Value.ToString();

                    lblCurrentNo.Text = Convert.ToString(dgvPSP.RowCount - 1);
                }
                else
                {
                    //Store first title name in strOutput
                    strOutput = "Title Name: " + dgvPSP.Rows[0].Cells[1].Value.ToString();
                }

                }
            catch(System.Exception e)
                {
                //Display nothing if datagrid view is blank
                strOutput = "";
                lblCurrentNo.Text = "";
                }

            //Display Selected title name
            txtOutput.Text = strOutput;
        }


        private void btnEdit_Click(object sender, EventArgs e)
        {
            string strOutput = txtOutput.Text;

            int i = 0;
            //find currently selected row in datagrid view
            i = Convert.ToInt32(dgvPSP.CurrentCell.RowIndex.ToString());

            if (!strOutput.Equals("") && strOutput.Equals("Title Name: " + dgvPSP.Rows[i].Cells[1].Value.ToString()))
            {
                OpenEditForm();
            }

        }
        private void btnAdd_Click(object sender, EventArgs e)
        {
            OpenAddForm();

        }
        private void OpenAddForm()
        {
            frmAdd AddForm = new frmAdd();
            AddForm.Show(); 
            this.Hide();

        }

        private void OpenEditForm()
        {
            int i = 0;

            frmEdit EditForm = new frmEdit();

            //find currently selected row in datagrid view
            i = Convert.ToInt32(dgvPSP.CurrentCell.RowIndex.ToString());

            //Store Selected title name in frmEdit's txtID textbox
            EditForm.lblID.Text = dgvPSP.Rows[i].Cells[0].Value.ToString();

            EditForm.Show();
            this.Hide();

        }


        private void btnDelete_Click(object sender, EventArgs e)
        {
            string strOutput = txtOutput.Text;
            //string strName = "";
            int i = 0;
            int intID = 0;
            //find currently selected row in datagrid view
            i = Convert.ToInt32(dgvPSP.CurrentCell.RowIndex.ToString());

            intID = Convert.ToInt32(dgvPSP.Rows[i].Cells[0].Value);

            string strMessage = "Are you sure you want to DELETE ID No. " + intID + " " + strOutput + "?";
            



            if (!strOutput.Equals("") && strOutput.Equals("Title Name: " + dgvPSP.Rows[i].Cells[1].Value.ToString()))
            {
                var varAnswer = MessageBox.Show(strMessage, "Delete " + strOutput, MessageBoxButtons.YesNo,
                MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2);

                if (varAnswer == DialogResult.Yes)
                {
                    DeleteTitle();

                    MessageBox.Show(strOutput + " was successfully Deleted.","Deletion Successful", MessageBoxButtons.OK);

                    DisplayAll();
                }
            }


        }

        private void DeleteTitle()
        {
            int intID = 0;
            int i = 0;


            //find currently selected row in datagrid view
            i = Convert.ToInt32(dgvPSP.CurrentCell.RowIndex.ToString());

            //Store Selected title name's ID in intID variable
            intID = Convert.ToInt32(dgvPSP.Rows[i].Cells[0].Value);


            string SQLsearch = "DELETE * FROM PSP_GameCollection WHERE [ID] =" + intID;
            ExecuteNonQuery(SQLsearch);
        }

        private void dgvPSP_Click(object sender, EventArgs e)
        {
            DisplayOutput(false);
        }
        private void dgvPSP_Sorted(object sender, EventArgs e)
        {
            DisplayOutput(false);
        }

        private void frmMain_VisibleChanged(object sender, EventArgs e)
        {
            // Will Requery the database
            if (this.Visible == true)
            CallDB();
        }

        private void btnSaveFile_Click(object sender, EventArgs e)
        {
            SaveFile();
        }

        private void SaveFile()
        {
            string strFile = "";

            strFile = SaveFileWindow();

            if (strFile.Equals("") || strFile.Equals(null))
            {}
            else
            {

                //Create New Excel File
                Excel.Application oExcel = new Excel.Application();
                
                oExcel.Visible = true;

                object objMissing = System.Reflection.Missing.Value;

                //Create New Workbook
                Excel.Workbook oBook = oExcel.Workbooks.Add(objMissing);

                Excel.Worksheet oSheet = (Excel.Worksheet)oBook.ActiveSheet;
                

                int intTotalColumn = 0;
                int intTotalRow = 0;
                int intColIndex = 0; 
                int intRowIndex = 0;
                string strResults = "";

                intTotalColumn = dgvPSP.ColumnCount;

                intTotalRow = dgvPSP.RowCount;

                strResults = txtResults.Text;

                if (strResults.Equals(""))
                {
                    strResults = "<--All Items-->";
                }
                else
                {
                    strResults = strResults.Replace("Search Results for: ", "");
                }

                
                oSheet.Cells[intRowIndex + 1, intColIndex + 1] = "Search Results for: "
                    + (char)34 + strResults + (char)34;


                //'dgvPSP.Item(1, i).Value
                for (intRowIndex = 0; intRowIndex < intTotalRow; intRowIndex++)
                {
                    for (intColIndex = 0; intColIndex < intTotalColumn; intColIndex++)
                    {
                        oSheet.Cells[intRowIndex + 3, intColIndex + 1] =
                            dgvPSP.Rows[intRowIndex].Cells[intColIndex].Value;

                        if (intRowIndex == 0)
                        {
                            oSheet.Cells[intRowIndex + 2, intColIndex + 1] =
                              dgvPSP.Columns[intColIndex].Name;
                        }

                    }
                

                    if (intRowIndex == 0)
                    { 
                        //'Formats Title of Table
                        Excel.Range oRange = oSheet.Range[oSheet.Cells[intRowIndex + 1, 1], oSheet.Cells[intRowIndex + 1, intColIndex]];
                        oRange.Font.Bold = true;
                        oRange.MergeCells = true;
                        oRange.HorizontalAlignment = Excel.XlVAlign.xlVAlignCenter;


                        //'Formats Table Fields
                        oRange = oSheet.Range[oSheet.Cells[intRowIndex + 2, 1], oSheet.Cells[intRowIndex + 2, intColIndex]];
                        oRange.Font.Bold = true;


                    }

                }
                oExcel.DisplayAlerts = false;

                //Auto Sizes all columns
                oSheet.Columns.AutoFit();


                oBook.SaveAs(strFile, -4143);//-4143 = Normal Workbook

                oBook.Close();
                oExcel.Quit();
        }
        }

        private string SaveFileWindow()
        {
            char chrSlash = (char)92;
            string saveFile = "";

            //Sets File Name
            SaveFileDialog sFile = new SaveFileDialog();
            sFile.Filter = "Excel 2003 (*.xls)|*.xls";
            sFile.FilterIndex = 1 ;
            
            sFile.InitialDirectory = Directory() + chrSlash + "Excel Reports" + chrSlash;


            if (sFile.ShowDialog() == DialogResult.OK)
            {
                saveFile = sFile.FileName.ToString();
            }

            return saveFile;
        }

        private void frmMain_Load(object sender, EventArgs e)
        {

        }
    }


}
