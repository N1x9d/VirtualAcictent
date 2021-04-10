using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Threading;
using Microsoft.Office.Interop.Excel;
using DataTable = System.Data.DataTable;


namespace VA
{
    public class DataWorker
    {
        private MainWindowVM vm;

        public DataWorker(MainWindowVM VM)
        {
            vm = VM;
        }
        
        public void createTableExel(DataTable db)
        {
            var excelApp = new Application();
            Workbook workBook;
            Worksheet workSheet;
            var table = db;
            workBook = excelApp.Workbooks.Add();
            workSheet = (Worksheet)workBook.Worksheets.get_Item(1);
            var rng2 = workSheet.Range[workSheet.Cells[1, 2],
                workSheet.Cells[table.Rows.Count + 1, table.Columns.Count]];
            var rng3 = workSheet.Range[workSheet.Cells[1, 1],
                workSheet.Cells[table.Rows.Count + 1, table.Columns.Count]];
            CreateExelTable(table, rng3, workSheet, excelApp);
           
        }
        public void CreateExelTable(DataTable table, Range range, Worksheet workSheet, Application excelApp)
        {
            for (var j = 0; j < table.Columns.Count; j++) workSheet.Cells[1, j + 1] = table.Columns[j].ColumnName;
            for (var j = 0; j < table.Rows.Count; j++)
            for (var i = 0; i < table.Columns.Count; i++)
                workSheet.Cells[j + 2, i + 1] = table.Rows[j][i];
            range.Borders.Color = Color.Black.ToArgb();
         
            excelApp.Visible = true;
            excelApp.UserControl = true;
        }

    }
}
