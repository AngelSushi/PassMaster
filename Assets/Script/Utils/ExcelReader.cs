using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Excel = Microsoft.Office.Interop.Excel;

public class ExcelReader : MonoBehaviour {
    
    public static Excel.Application CreateExcelApp() {
        Excel.Application app = new Excel.Application();

        app.DisplayAlerts = false;
        app.Visible = true;

        return app;
    }

    
}
