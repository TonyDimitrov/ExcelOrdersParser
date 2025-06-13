using Werehouse.Models;
using OfficeOpenXml;

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
                        if (string.IsNullOrWhiteSpace(status1))
                            order.Statuses.Enqueue(status1);

                        string status2 = worksheet.Cells[row, 10].Value?.ToString();
                        if (string.IsNullOrWhiteSpace(status2))
                            order.Statuses.Enqueue(status2);

                        string status3 = worksheet.Cells[row, 11].Value?.ToString();
                        if (string.IsNullOrWhiteSpace(status3))
                            order.Statuses.Enqueue(status3);

                        string status4 = worksheet.Cells[row, 12].Value?.ToString();
                        if (string.IsNullOrWhiteSpace(status4))
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
