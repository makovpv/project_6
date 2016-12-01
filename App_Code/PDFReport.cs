using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;
using System.Drawing;

/// <summary>
/// Summary description for PDFReport
/// </summary>
public class PDFReport
{
	public PDFReport()
	{
		//
		// TODO: Add constructor logic here
		//
	}

    //public static MemoryStream Generate(string p_path)
    public static void Generate(string p_path)
    {
        // http://habrahabr.ru/post/112707/
        var doc = new Document();

        //MemoryStream ms = new MemoryStream();
        PdfWriter.GetInstance(doc, new FileStream(p_path + @"\Document.pdf", FileMode.Create));
        //PdfWriter.GetInstance(doc, ms);
        doc.Open();

        PdfPTable table = new PdfPTable(6);
        PdfPCell cell = new PdfPCell(new Phrase("Simple table",
              new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.TIMES_ROMAN, 16,
              iTextSharp.text.Font.NORMAL, new BaseColor(Color.Orange))));
        cell.BackgroundColor = new BaseColor(Color.Wheat);
        cell.Padding = 5;
        cell.Colspan = 6;
        cell.HorizontalAlignment = Element.ALIGN_CENTER;
        table.AddCell(cell);
        table.AddCell("Sotrudnic");
        table.AddCell("Дата");
        table.AddCell("Балл");
        table.AddCell("Рейтинг");
        table.AddCell("Процент");
        table.AddCell("Зоны роста");

        doc.Add(table);
        doc.Close();

        //return ms;
    }
}