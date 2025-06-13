using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Werehouse.Models
{
    public class Order
    {
        public DateTime RequestDate { get; set; }        // Column A: Дата заявка
        public string RequestNumber { get; set; }        // Column C: номер заявка
        public string PharmacyName { get; set; }         // Column D: Име аптека
        public string Product { get; set; }              // Column E: Продукт
        public int RequestedQuantity { get; set; }       // Column F: заявено кол.
        public int RabatQuantity { get; set; }           // Column G: кол. Рабат
        public string Status1 { get; set; }              // Column I: статус 1
        public string Status2 { get; set; }              // Column J: статус 2
        public string Status3 { get; set; }              // Column K: статус 3
        public string Status4 { get; set; }              // Column K: статус 4
        public Queue<string> Statuses { get; set; } = new();

        //public override string ToString()
        //{
        //    return $"{RequestDate:yyyy-MM-dd} | {RequestNumber} | {PharmacyName} | {Product} | {RequestedQuantity} | {RabatQuantity}";
        //}
    }
}
