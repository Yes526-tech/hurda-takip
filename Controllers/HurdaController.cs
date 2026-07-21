using FireHurdaTakip.Data;
using FireHurdaTakip.Models;
using Microsoft.AspNetCore.Mvc;
using ClosedXML.Excel;
using System.IO;
using System.Linq;
using System;

namespace FireHurdaTakip.Controllers
{
    public class HurdaController : Controller
    {
        private readonly AppDbContext _context;

        public HurdaController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var kayitlar = _context.HurdaKayitlar.OrderByDescending(x => x.Tarih).ToList();
            return View(kayitlar);
        }

        [HttpGet]
        public IActionResult Ekle()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Ekle(HurdaKayit model)
        {
            if (ModelState.IsValid)
            {
                model.Tarih = DateTime.SpecifyKind(model.Tarih, DateTimeKind.Utc);
                model.ToplamPlastikHurdasiKg = model.IzabeyeGonderilenHurdaKg + model.PEnjeksiyonaGonderilenHurdaKg;
                _context.HurdaKayitlar.Add(model);
                _context.SaveChanges();



                TempData["Message"] = "Kayıt başarıyla eklendi.";
                return RedirectToAction("Index");
            }
            return View(model);
        }

        public IActionResult ExcelExport()
        {
            var kayitlar = _context.HurdaKayitlar.OrderBy(x => x.Tarih).ToList();

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Hurda Raporu");

            // Row 1: Title
            worksheet.Range("A1:G1").Merge().Value = "AKO AKÜ FABRİKASI AKÜMÜLATÖR VE PLASTİK FİRELERİ";
            var titleRange = worksheet.Range("A1:G1");
            titleRange.Style.Font.Bold = true;
            titleRange.Style.Font.FontSize = 14;
            titleRange.Style.Fill.BackgroundColor = XLColor.Yellow;

            // Row 2 & 3: Headers Merging and Values
            worksheet.Range("A2:A3").Merge().Value = "TARİH";
            worksheet.Range("B2:B3").Merge().Value = "TOPLAM\nPLASTİK HURDASI\n(KG)";
            worksheet.Range("C2:D2").Merge().Value = "AKÜMÜLATÖR HURDALARI";
            worksheet.Range("E2:F2").Merge().Value = "PLASTİK HURDALAR";
            worksheet.Range("G2:G3").Merge().Value = "AY";

            worksheet.Cell("C3").Value = "ALINAN SİPARİŞ NO";
            worksheet.Cell("D3").Value = "AKÜMÜLATÖR HURDASI\n(KG)";
            worksheet.Cell("E3").Value = "İZABEYE GÖNDERİLEN\nHURDA (KG)";
            worksheet.Cell("F3").Value = "P.ENJEKSİYONA GÖNERİLEN\nHURDA (KG)";

            // Header Colors
            var orangeColor = XLColor.FromHtml("#FFC000");
            var lightBlueColor = XLColor.FromHtml("#B4C6E7");
            
            worksheet.Range("A2:B3").Style.Fill.BackgroundColor = orangeColor;
            worksheet.Range("C2:F2").Style.Fill.BackgroundColor = orangeColor;
            worksheet.Range("C3:F3").Style.Fill.BackgroundColor = lightBlueColor;
            worksheet.Range("G2:G3").Style.Fill.BackgroundColor = lightBlueColor;

            // Common Header Styles
            var headerRange = worksheet.Range("A1:G3");
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            headerRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            headerRange.Style.Alignment.WrapText = true;
            headerRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            headerRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

            // Data Rows
            int row = 4;
            var alternateRowColor = XLColor.FromHtml("#D9E1F2");

            foreach (var item in kayitlar)
            {
                worksheet.Cell(row, 1).Value = item.Tarih.ToString("dd.MM.yyyy");
                worksheet.Cell(row, 2).Value = item.ToplamPlastikHurdasiKg;
                worksheet.Cell(row, 3).Value = item.AlinanSiparisNo;
                worksheet.Cell(row, 4).Value = item.AkumulatorHurdasiKg;
                worksheet.Cell(row, 5).Value = item.IzabeyeGonderilenHurdaKg;
                worksheet.Cell(row, 6).Value = item.PEnjeksiyonaGonderilenHurdaKg;
                worksheet.Cell(row, 7).Value = item.Ay;

                var dataRange = worksheet.Range(row, 1, row, 7);
                dataRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                dataRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                dataRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                dataRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

                if (row % 2 != 0) // alternating color starting at row 5
                {
                    dataRange.Style.Fill.BackgroundColor = alternateRowColor;
                }

                row++;
            }

            // Adjust Layout
            worksheet.Column(1).Width = 14;
            worksheet.Column(2).Width = 18;
            worksheet.Column(3).Width = 20;
            worksheet.Column(4).Width = 22;
            worksheet.Column(5).Width = 22;
            worksheet.Column(6).Width = 26;
            worksheet.Column(7).Width = 6;
            
            worksheet.Row(1).Height = 25;
            worksheet.Row(2).Height = 20;
            worksheet.Row(3).Height = 45;

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            var content = stream.ToArray();

            return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"HurdaRaporu_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx");
        }

        [HttpPost]
        public IActionResult UploadAndUpdateExcel(Microsoft.AspNetCore.Http.IFormFile excelFile)
        {
            if (excelFile == null || excelFile.Length == 0)
            {
                TempData["Message"] = "Lütfen bir Excel dosyası seçin.";
                return RedirectToAction("Index");
            }

            try
            {
                using var stream = new MemoryStream();
                excelFile.CopyTo(stream);
                stream.Position = 0;

                using var workbook = new XLWorkbook(stream);
                var worksheet = workbook.Worksheet("Hurda Raporu");
                
                var kayitlar = _context.HurdaKayitlar.OrderBy(x => x.Tarih).ToList();
                int row = 4;

                var alternateRowColor = XLColor.FromHtml("#D9E1F2");

                foreach (var item in kayitlar)
                {
                    worksheet.Cell(row, 1).Value = item.Tarih.ToString("dd.MM.yyyy");
                    worksheet.Cell(row, 2).Value = item.ToplamPlastikHurdasiKg;
                    worksheet.Cell(row, 3).Value = item.AlinanSiparisNo;
                    worksheet.Cell(row, 4).Value = item.AkumulatorHurdasiKg;
                    worksheet.Cell(row, 5).Value = item.IzabeyeGonderilenHurdaKg;
                    worksheet.Cell(row, 6).Value = item.PEnjeksiyonaGonderilenHurdaKg;
                    worksheet.Cell(row, 7).Value = item.Ay;

                    var dataRange = worksheet.Range(row, 1, row, 7);
                    dataRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    dataRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                    dataRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    dataRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

                    if (row % 2 != 0) // alternating color starting at row 5
                    {
                        dataRange.Style.Fill.BackgroundColor = alternateRowColor;
                    }
                    else
                    {
                        dataRange.Style.Fill.BackgroundColor = XLColor.NoColor;
                    }

                    row++;
                }

                using var outStream = new MemoryStream();
                workbook.SaveAs(outStream);
                var content = outStream.ToArray();

                return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"Guncel_HurdaRaporu_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx");
            }
            catch (Exception ex)
            {
                TempData["Message"] = "Dosya işlenirken hata oluştu: " + ex.Message;
                return RedirectToAction("Index");
            }
        }
    }
}
