using OfficeOpenXml.Style;
using System.Collections.Generic;
using Werehouse.Data;
using Werehouse.Models;

namespace Werehouse.Services
{
    public class OrdersAdaptorService
    {
        StatusProvider _statusProvider;
        public OrdersAdaptorService(StatusProvider statusProvider)
        {
            _statusProvider = statusProvider;
        }
        public (List<CalculatedOrder>groupedData, List<CalculatedOrder> invalidData) CalculateOrdersQuantity(List<Order> orders)
        {
            var calculatedOrders = new List<CalculatedOrder>();
            foreach (var order in orders)
            {
                //изпълнена
                //изпълнено
                //частично 0
                //отожена
                ResolveOrderByStatus(order, calculatedOrders);
            }

            List<CalculatedOrder> invalidData = calculatedOrders.Where(co => !co.IsValid)
                .ToList();

            List<CalculatedOrder> groupedData = GroupOrders(calculatedOrders);

            return (groupedData, invalidData);
        }

        private List<CalculatedOrder> GroupOrders(List<CalculatedOrder> calculatedOrders)
        {
            return calculatedOrders
                  .Where(o => o.IsValid) // Exclude invalid orders
                  .GroupBy(o => new { o.PharmacyName, o.Product })
                  .Select(g => new CalculatedOrder
                  {
                      PharmacyName = g.Key.PharmacyName,
                      Product = g.Key.Product,
                      RequestedQuantity = g.Sum(x => x.RequestedQuantity),
                      RabatQuantity = g.Sum(x => x.RabatQuantity),
                      PartialQuantity = g.Sum(x => x.PartialQuantity),
                      FlagCompleted = g.Any(x => x.FlagCompleted),
                      FlagDalay = g.Any(x => x.FlagDalay),
                      FlagPartiallyDone = g.Any(x => x.FlagPartiallyDone),
                  })
                  .ToList();
        }

        private void ResolveOrderByStatus(Order order, List<CalculatedOrder> calculatedOrders)
        {
            CalculatedOrder calculatedOrder = new();
            while (true)
            {
                if (IsOrderCompleted(order, calculatedOrder))
                {
                    calculatedOrders.Add(calculatedOrder);

                    return;
                }
                if (IsOrderDelayed(order, calculatedOrder))
                {
                    calculatedOrders.Add(calculatedOrder);

                    continue;
                }
                if (IsOrderPartuallyCompleted(order, calculatedOrder))
                {
                    calculatedOrders.Add(calculatedOrder);

                    continue;
                }

                // Exceptional case
                ExceptionalCase(order, calculatedOrder);
                calculatedOrders.Add(calculatedOrder);

                return;
            }
        }

        private bool IsOrderCompleted(Order order, CalculatedOrder calculatedOrder)
        {
            if (order.Statuses.Count == 0)
                return true;

            string status = order.Statuses.Peek();

            if (_statusProvider.Completed.Any(cs => status.Contains(cs)))
            {
                calculatedOrder.RequestDate = order.RequestDate;
                calculatedOrder.RequestNumber = order.RequestNumber;
                calculatedOrder.PharmacyName = order.PharmacyName;
                calculatedOrder.Product = order.Product;
                calculatedOrder.RequestedQuantity = order.RequestedQuantity;
                calculatedOrder.RabatQuantity = order.RabatQuantity;
                calculatedOrder.FlagCompleted = true;

                order.Statuses.Dequeue();

                return true;
            }

            return false;
        }

        private bool IsOrderDelayed(Order order, CalculatedOrder calculatedOrder)
        {
            if (order.Statuses.Count == 0)
                return true;

            string status = order.Statuses.Peek();

            if (_statusProvider.Delay.Any(cs => status.Contains(cs)))
            {
                calculatedOrder.RequestDate = order.RequestDate;
                calculatedOrder.RequestNumber = order.RequestNumber;
                calculatedOrder.PharmacyName = order.PharmacyName;
                calculatedOrder.Product = order.Product;
                calculatedOrder.RequestedQuantity = order.RequestedQuantity;
                calculatedOrder.RabatQuantity = order.RabatQuantity;
                calculatedOrder.FlagDalay = true;

                order.Statuses.Dequeue();

                return true;
            }

            return false;
        }

        private bool IsOrderPartuallyCompleted(Order order, CalculatedOrder calculatedOrder)
        {
            if (order.Statuses.Count == 0)
                return true;

            string status = order.Statuses.Peek();

            if (_statusProvider.Partially.Any(cs => status.Contains(cs)))
            {
                calculatedOrder.RequestDate = order.RequestDate;
                calculatedOrder.RequestNumber = order.RequestNumber;
                calculatedOrder.PharmacyName = order.PharmacyName;
                calculatedOrder.Product = order.Product;
                calculatedOrder.RequestedQuantity = order.RequestedQuantity;
                calculatedOrder.RabatQuantity = order.RabatQuantity;
                calculatedOrder.FlagDalay = true;

                string[] statusParts = status.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
                int partialQuantity = 0;

                if (statusParts.Length > 1)
                {
                    int.TryParse(statusParts[1], out partialQuantity);
                }

                calculatedOrder.PartialQuantity = partialQuantity;
                order.Statuses.Dequeue();

                return true;
            }

            return false;
        }

        private void ExceptionalCase(Order order, CalculatedOrder calculatedOrder)
        {
            calculatedOrder.RequestDate = order.RequestDate;
            calculatedOrder.RequestNumber = order.RequestNumber;
            calculatedOrder.PharmacyName = order.PharmacyName;
            calculatedOrder.Product = order.Product;
            calculatedOrder.RequestedQuantity = order.RequestedQuantity;
            calculatedOrder.RabatQuantity = order.RabatQuantity;
            calculatedOrder.IsValid = false;
        }
    }
}
