using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace WebParserTools
{
    public class ExcelParseExport : ParseExport
    {

        public ExcelParseExport(string outFileName)
        {
            _outFileName = outFileName;
        }

        private readonly string _outFileName;

        public override void SaveToSource(List<PostInformation> postInformations)
        {
            using (ExcelPackage excelPackage = new ExcelPackage())
            {
                excelPackage.Workbook.Properties.Author = "noname";
                excelPackage.Workbook.Properties.Title = "экспорт";
                excelPackage.Workbook.Properties.Subject = "результаты экспорта";
                excelPackage.Workbook.Properties.Created = DateTime.Now;

                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("Страница 1");

                SaveHeaders(worksheet);
                SaveContent(worksheet, postInformations);

                FileInfo fi = new FileInfo(_outFileName);
                excelPackage.SaveAs(fi);
            }
        }


        private void SaveHeaders(ExcelWorksheet worksheet)
        {
            worksheet.Cells[1,1].Value = "Имя";
            worksheet.Cells[1,2].Value = "Телефонный номер";
            worksheet.Cells[1,3].Value = "Дата публикации";
            worksheet.Cells[1,4].Value = "Тема";
            worksheet.Cells[1,5].Value = "Сообщение";
        }

        private void SaveContent(ExcelWorksheet worksheet, List<PostInformation> postInformations)
        {
            for (int i=0; i < postInformations.Count; i++)
            {
                worksheet.Cells[i + 2, 1].Value = postInformations[i].Author.Name;
                worksheet.Cells[i + 2, 2].Value = postInformations[i].Author.PhoneNumber;
                worksheet.Cells[i + 2, 3].Value = postInformations[i].Date;
                worksheet.Cells[i + 2, 4].Value = postInformations[i].Theme;
                worksheet.Cells[i + 2, 5].Value = postInformations[i].Message;
            }          
        }
    }
}
