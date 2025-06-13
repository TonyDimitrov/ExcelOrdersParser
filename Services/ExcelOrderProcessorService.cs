using ClosedXML.Excel;
using OfficeOpenXml;
using System.Drawing;
using Werehouse.Models;

namespace Werehouse.Services
{
    public class ExcelOrderProcessorService
    {
        public List<Order> ReadOrdersFromExcel(string filePath)
        {
            var orders = new List<Order>();

            FileInfo fileInfo = new FileInfo(filePath);
            ExcelPackage.License.SetNonCommercialOrganization("Toni-makaroni");
            using (var package = new ExcelPackage(fileInfo))
            {
                // First sheet
                ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                int rowCount = worksheet.Dimension.Rows;

                // Start from row 2 to skip header
                for (int row = 2; row <= rowCount; row++)
                {
                    try
                    {
                        var order = new Order
                        {
                            RequestDate = GetDateTimeFromExcel(worksheet.Cells[row, 1].Value),
                            RequestNumber = worksheet.Cells[row, 3].Value?.ToString(),
                            PharmacyName = worksheet.Cells[row, 4].Value?.ToString(),
                            Product = worksheet.Cells[row, 5].Value?.ToString(),
                            RequestedQuantity = GetIntFromExcel(worksheet.Cells[row, 6].Value),
                            RabatQuantity = GetIntFromExcel(worksheet.Cells[row, 7].Value),  
                        };

                        string status1 = worksheet.Cells[row, 9].Value?.ToString();
                        if (!string.IsNullOrWhiteSpace(status1))
                            order.Statuses.Enqueue(status1);

                        string status2 = worksheet.Cells[row, 10].Value?.ToString();
                        if (!string.IsNullOrWhiteSpace(status2))
                            order.Statuses.Enqueue(status2);

                        string status3 = worksheet.Cells[row, 11].Value?.ToString();
                        if (!string.IsNullOrWhiteSpace(status3))
                            order.Statuses.Enqueue(status3);

                        string status4 = worksheet.Cells[row, 12].Value?.ToString();
                        if (!string.IsNullOrWhiteSpace(status4))
                            order.Statuses.Enqueue(status4);

                        orders.Add(order);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error reading row {row}: {ex.Message}");
                    }
                }
            }

            return orders;
        }

        public void GenerateReport(List<CalculatedOrder> orders, string filePath)
        {
            // Group by PharmacyName and Product, sum quantities
            var groupedData = orders
                .Where(o => o.IsValid)
                .GroupBy(o => new { o.PharmacyName, o.Product })
                .Select(g => new
                {
                    PharmacyName = g.Key.PharmacyName,
                    Product = g.Key.Product,
                    RequestedQuantity = g.Sum(x => x.RequestedQuantity),
                    TotalQuantity = g.Sum(x => x.RequestedQuantity + x.RabatQuantity),
                    HasCompleted = g.Any(x => x.FlagCompleted),
                    HasPartial = g.Any(x => x.FlagPartiallyDone),
                    HasDelay = g.Any(x => x.FlagDalay)
                })
                .OrderBy(x => x.PharmacyName)
                .ThenBy(x => x.Product)
                .ToList();

            // Create Excel workbook
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Лист1");

                // Headers
                worksheet.Cell(1, 1).Value = "Артикул";
                worksheet.Cell(1, 2).Value = "Клиент";
                worksheet.Cell(1, 3).Value = "Продадено количество";
                worksheet.Cell(1, 4).Value = "пакет";
                worksheet.Cell(1, 5).Value = "Изпълнена";
                worksheet.Cell(1, 6).Value = "Частично изпълнена";
                worksheet.Cell(1, 7).Value = "Отложена";

                // Style headers
                var headerRange = worksheet.Range(1, 1, 1, 6);
                headerRange.Style.Font.Bold = true;
                headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;

                // Data rows
                int row = 2;
                foreach (var item in groupedData)
                {
                    worksheet.Cell(row, 1).Value = item.Product;
                    worksheet.Cell(row, 2).Value = item.PharmacyName;
                    worksheet.Cell(row, 3).Value = item.RequestedQuantity;
                    worksheet.Cell(row, 4).Value = item.TotalQuantity > item.RequestedQuantity
                        ? $"{item.RequestedQuantity}+{item.TotalQuantity - item.RequestedQuantity}"
                        : string.Empty;

                    if (item.HasCompleted)
                    {
                        worksheet.Cell(row, 5).Value = "да";
                        worksheet.Cell(row, 5).Style.Fill.BackgroundColor = XLColor.Green;
                    }

                    if (item.HasPartial)
                    {
                        worksheet.Cell(row, 6).Value = "да";
                        worksheet.Cell(row, 6).Style.Fill.BackgroundColor = XLColor.Orange;
                    }

                    if (item.HasDelay)
                    {
                        worksheet.Cell(row, 7).Value = "да";
                        worksheet.Cell(row, 7).Style.Fill.BackgroundColor = XLColor.Red;
                    }

                    row++;
                }

                // Auto-fit columns
                worksheet.Columns().AdjustToContents();

                // Save the file
                workbook.SaveAs(filePath);
            }
        }

        private DateTime GetDateTimeFromExcel(object excelValue)
        {
            if (excelValue == null) return DateTime.MinValue;

            if (excelValue is DateTime)
                return (DateTime)excelValue;

            if (DateTime.TryParse(excelValue.ToString(), out DateTime result))
                return result;

            return DateTime.MinValue;
        }

        private int GetIntFromExcel(object excelValue)
        {
            if (excelValue == null) return 0;

            if (excelValue is int)
                return (int)excelValue;

            if (int.TryParse(excelValue.ToString(), out int result))
                return result;

            return 0;
        }
    }
}
