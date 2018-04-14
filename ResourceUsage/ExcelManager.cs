
using Aspose.Cells;
using System.Collections.Generic;
using System.IO;
public class ExcelIns
{
    private string path;
    private Workbook workbook;
    public ExcelIns(string p)
    {
        path = p;
    }
    public Worksheet GetWorkSheet(int index = 0)
    {
        if (workbook == null)
        {
            workbook = File.Exists(path) ? new Workbook(path) : new Workbook();
        }
        return workbook.Worksheets[index];
    }
    public void WriteSimpleList(List<string> content)
    {
        Worksheet ws = GetWorkSheet();
        for (int i = 0; i < content.Count; i++)
        {
            Cell itemCell = ws.Cells[i, 0];
            itemCell.PutValue(content[i]);
        }
        workbook.Save(path);
    }
}
public class ExcelManager
{
    public static ExcelIns GetExcel(string path)
    {
        return new ExcelIns(path);
    }
}
