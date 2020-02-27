using System;
using System.IO;
using System.Linq;
using Task.Data;
using System.Collections.Generic;
namespace Task
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            var dataSource = new DataSource();


            var customers = dataSource.Customers;
            var suppliers = dataSource.Suppliers;
            var product = dataSource.Products;

            //foreach(var k in customers.Where(c => c.Phone.Contains("(")))
            //	Console.WriteLine(k.Phone);


            var namewidth = 1 + customers.Max(c => c.CompanyName.Length);
            ////заказы первого клиента
            //var ordersFirstCustomer = customers.First().Orders;

            ////сумма первого заказа у первого клиента
            //var summFirstOrderOfOFirstCustomer = customers.First().Orders.First().Total;

            //Customer[] sumTotalGreaterThan(decimal x) => customers.Where(c => c.Orders.Sum(o => o.Total) > x).ToArray();
           
            // задание 1
            dynamic sumTotalGreaterThan(decimal x) => customers.Select(c => new { Customer = c, OrdersTotal = c.Orders.Sum(o => o.Total) }).Where(c => c.OrdersTotal > x);
            // задание 2
            IEnumerable<Customer> sumOrderGreaterThan(decimal x) => customers.Where(c => c.Orders.Any(o => o.Total > x));
            // задание 3
            var sameCityAsCustomer = customers.Select(c => new { Customer = c, Suppliers = suppliers.Where(s => s.Country.Equals(c.Country) && s.City.Equals(c.City)) }).Where(v => v.Suppliers.Any());
            // задание 4
            var customersWithNoPostalRegionOpCode = customers.Where(c => c.Phone[0] != '(' || c.PostalCode.Any(p => p < '0' || p > '9') || string.IsNullOrEmpty(c.Region));
            
            var prices = new[] { 50, 120 };
            var naming = new[] { "Дешевая цена", "Средняя цена", "Дорогая цена" };
            // задание 5
            var productsGrouped = product.GroupBy(p => prices.IndexOf(w => w > p.UnitPrice)).Select(g => new { Index = naming[g.Key], Products = g.ToList() });

            using (StreamWriter Console = new StreamWriter("output.txt"))
            {
                decimal gt = 10000;
                Console.WriteLine($"Task 1 (Total sum is greater than {gt})");

                foreach (var cust in sumTotalGreaterThan(gt))
                    Console.WriteLine($"Customer: {{0,-{namewidth}}}\tTotal:{{1}}", cust.Customer.CompanyName, cust.OrdersTotal);

                gt = 5000;
                Console.WriteLine();
                Console.WriteLine($"Task 2 (Order total is greater than {gt})");
                foreach (var cust in sumOrderGreaterThan(gt))
                {
                    Console.WriteLine("Customer: {0}", cust.CompanyName);
                    foreach (var order in cust.Orders.Where(o => o.Total > gt))
                        Console.WriteLine("\tOrderID: {0}, order total: {1}", order.OrderID, order.Total);
                }
                Console.WriteLine();
                Console.WriteLine("Task 3 (Suppliers with the same country and city as customer)");
                foreach (var custSuppl in sameCityAsCustomer)
                {
                    Console.WriteLine("Customer {0}, country: {1}, city: {2}. Suppliers from the same city and country: ", custSuppl.Customer.CompanyName, custSuppl.Customer.Country, custSuppl.Customer.City);
                    //if((custSuppl.Suppliers?.Count() ?? 0) == 0)
                    //	Console.WriteLine("None");
                    foreach (var supplier in custSuppl.Suppliers)
                        Console.WriteLine("{0}", supplier.SupplierName);
                }

                Console.WriteLine();
                Console.WriteLine("Task 4 (Customers with no postal code nor region nor operator code)");
                foreach (var cust in customersWithNoPostalRegionOpCode)
                    Console.WriteLine($"{{0,-{namewidth}}}, Postal: {{1}}, Region: {{2}}, OP code: {{3}}",cust.CompanyName, cust.PostalCode, cust.Region, cust.Phone);


                Console.WriteLine();
                Console.WriteLine("Task 5 (Grouping by price (p<50, 50<=p<120, 120<=p)");
                foreach (var p in productsGrouped)
                {
                    Console.WriteLine();
                    Console.WriteLine(p.Index);
                    foreach (var prod in p.Products)
                        Console.WriteLine("Продукт {0}, цена за штуку: {1}", prod.ProductName, prod.UnitPrice);
                }

            }


        }
        /// <summary>
        /// Возвращает первый индекс элемента, который удовлетворяет условию. Если такого нет, возвращает длину перечисления
        /// </summary>
        /// <typeparam name="T">Тип перечисления</typeparam>
        /// <param name="v">Последовательность</param>
        /// <param name="pred">Условие</param>
        /// <returns>Индекс элемента</returns>
        public static int IndexOf<T>(this IEnumerable<T> v, Predicate<T> pred)
        {
            int index = 0;
            foreach (var val in v)
            {
                if (pred(val))
                    return index;
                index++;
            }
            return index;

        }
    }
}

/*

1. Выдайте список всех клиентов, чей суммарный оборот (сумма всех заказов) превосходит некоторую величину X.

2. Найдите всех клиентов, у которых были заказы, превосходящие по сумме величину X

3. Для каждого клиента составьте список поставщиков, находящихся в той же стране и том же городе. 

4. Укажите всех клиентов, у которых указан нецифровой почтовый код или не заполнен регион
или в телефоне не указан код оператора (для простоты считаем, что это равнозначно «нет круглых скобочек в начале»).

5. Сгруппируйте товары по группам «дешевые», «средняя цена», «дорогие». Границы каждой группы задайте сами	 
	 
	 */
