using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.IO;

namespace BattleShip.WPF.Models
{
    public class PDFCreater
    {
        static string ttf = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "ARIAL.TTF");
        static BaseFont baseFont = BaseFont.CreateFont(ttf, BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
        static Font font = new Font(baseFont, 14, Font.NORMAL);
        static string path = "report/";

        public static string ReportData(List<ReportData> reports)
        {
            int colspan_user = 4;
            int colspan_steps = 5;
            int it = 0;

            var doc = new Document();
            Directory.CreateDirectory(path);
            var returned_path = Directory.GetCurrentDirectory() + "\\" + $"{path}report_{DateTime.Now.ToString("MMddyyy-Hmmss")}_{reports[0].Pair.FirstPlayer.Name}.pdf";
            PdfWriter.GetInstance(doc, new FileStream(returned_path, FileMode.Create));
            doc.Open();

            foreach (var report in reports)
            {
                it++;
                PdfPTable table_user = new PdfPTable(colspan_user);
                table_user.TotalWidth = 500f;
                var cell = new PdfPCell(new Phrase($"Матч №{it}", font));
                cell.Padding = 5;
                cell.Colspan = colspan_user;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                table_user.AddCell(cell);

                cell = new PdfPCell(new Phrase("Имя игрока", font));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                table_user.AddCell(cell);
                cell = new PdfPCell(new Phrase("Количество игр", font));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                table_user.AddCell(cell);
                cell = new PdfPCell(new Phrase("Процент побед", font));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                table_user.AddCell(cell);
                cell = new PdfPCell(new Phrase("Точность выстрелов", font));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                table_user.AddCell(cell);

                PdfPCell add_cell = new PdfPCell(new Phrase(report.Pair.FirstPlayer.Name, font));
                add_cell.HorizontalAlignment = Element.ALIGN_CENTER;
                table_user.AddCell(add_cell);

                add_cell = new PdfPCell(new Phrase(report.Pair.FirstPlayer.TotalGame.ToString(), font));
                add_cell.HorizontalAlignment = Element.ALIGN_CENTER;
                table_user.AddCell(add_cell);

                add_cell = new PdfPCell(new Phrase(report.Pair.FirstPlayer.Winrate.ToString(), font));
                add_cell.HorizontalAlignment = Element.ALIGN_CENTER;
                table_user.AddCell(add_cell);

                add_cell = new PdfPCell(new Phrase(report.Pair.FirstPlayer.Accuracy.ToString(), font));
                add_cell.HorizontalAlignment = Element.ALIGN_CENTER;
                table_user.AddCell(add_cell);

                add_cell = new PdfPCell(new Phrase(report.Pair.SecondPlayer.Name, font));
                add_cell.HorizontalAlignment = Element.ALIGN_CENTER;
                table_user.AddCell(add_cell);

                add_cell = new PdfPCell(new Phrase(report.Pair.SecondPlayer.TotalGame.ToString(), font));
                add_cell.HorizontalAlignment = Element.ALIGN_CENTER;
                table_user.AddCell(add_cell);

                add_cell = new PdfPCell(new Phrase(report.Pair.SecondPlayer.Winrate.ToString(), font));
                add_cell.HorizontalAlignment = Element.ALIGN_CENTER;
                table_user.AddCell(add_cell);

                add_cell = new PdfPCell(new Phrase(report.Pair.SecondPlayer.Accuracy.ToString(), font));
                add_cell.HorizontalAlignment = Element.ALIGN_CENTER;
                table_user.AddCell(add_cell);

                doc.Add(table_user);
                doc.NewPage();

                PdfPTable steps_table = new PdfPTable(colspan_steps);
                steps_table.TotalWidth = 500f;
                cell = new PdfPCell(new Phrase($"Шаги матча №{it}", font));
                cell.Padding = 5;
                cell.Colspan = colspan_steps;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                steps_table.AddCell(cell);

                cell = new PdfPCell(new Phrase("Кто ходит", font));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                steps_table.AddCell(cell);
                cell = new PdfPCell(new Phrase("Место выстрела", font));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                steps_table.AddCell(cell);
                cell = new PdfPCell(new Phrase("Результат выстрела", font));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                steps_table.AddCell(cell);
                cell = new PdfPCell(new Phrase("Корабли первого игрока", font));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                steps_table.AddCell(cell);
                cell = new PdfPCell(new Phrase("Корабли второго игрока", font));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                steps_table.AddCell(cell);

                foreach (var step in report.Steps)
                {
                    add_cell = new PdfPCell(new Phrase(step.NamePlayer, font));
                    add_cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    steps_table.AddCell(add_cell);

                    add_cell = new PdfPCell(new Phrase(step.PlaceShoot, font));
                    add_cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    steps_table.AddCell(add_cell);

                    add_cell = new PdfPCell(new Phrase(step.Result, font));
                    add_cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    steps_table.AddCell(add_cell);

                    add_cell = new PdfPCell(new Phrase(step.ShipsLeftFirstPlayer.ToString(), font));
                    add_cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    steps_table.AddCell(add_cell);

                    add_cell = new PdfPCell(new Phrase(step.ShipsLeftSecondPlayer.ToString(), font));
                    add_cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    steps_table.AddCell(add_cell);
                }
                doc.Add(steps_table);
                doc.NewPage();
            }
            doc.Close();
            return returned_path;
        }
    }
}
