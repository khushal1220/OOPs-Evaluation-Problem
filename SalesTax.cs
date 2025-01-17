/* 

Input:
1 imported bottle of perfume at 27.99
3 bottle of perfume at 18.99
100 packet of headache pills at 9.75
9 box of imported chocolates at 11.25

Output:
1 imported bottle of perfume: 32.19
3 bottle of perfume: 20.89
100 packet of headache pills: 9.75
9 box of imported chocolates: 11.85
Sales Taxes: 15.30
Total: 1176.51

*/

using System;
using System.Collections.Generic;
using BillingSystem;
//App is a idle class which may use Bill class 
public class App
{
    public static void Main()
    {
        Bill bill = new Bill();
        string input;
        while (true)
        {
            input = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(input))
            {
                break;
            }
            try
            {
                bill.AddItem(input);

            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex}");
                break;
            }
        }
        //bill.RemoveItem("100 packet of headache pills at 9.75");
        bill.GenerateReceipt();
    }
}

//here is the implimentation of Bill class
namespace BillingSystem
{
    public class Bill
    {
        struct ShoppingItem
        {
            public int Quantity { get; }
            public string Name { get; }
            public float Price { get; }
            public bool IsExempted { get; }
            public bool IsImported { get; }

            public ShoppingItem(int quantity, string name, float price, bool isExempted, bool isImported)
            {
                Quantity = quantity;
                Name = name;
                Price = price;
                IsImported = isImported;
                IsExempted = isExempted;
            }
        }

        List<ShoppingItem> items = new List<ShoppingItem>();
        public void AddItem(string input)
        {
            try
            {
                ShoppingItem item;
                item = parseShopingItem(input);
                items.Add(item);
            }
            catch (Exception ex)
            {
                throw new FormatException($"{ex}");
            }
        }
        public void RemoveItem(string input)
        {
            try
            {
                ShoppingItem item;
                item = parseShopingItem(input);
                if (items.Contains(item)) items.Remove(item);
            }
            catch (Exception ex)
            {
                throw new FormatException($"{ex}");
            }

        }
        float CalculateSalesTax(ShoppingItem item)
        {
            float taxRate = 0.0f;

            if (!item.IsExempted) taxRate += 0.10f;

            if (item.IsImported) taxRate += 0.05f;

            float calculatedTax = item.Price * taxRate;
            return (float)Math.Ceiling(calculatedTax * 20) / 20;
        }

        public void GenerateReceipt()
        {
            float totalSalesTax = 0.0f;
            float totalAmount = 0.0f;
            foreach (var item in items)
            {
                float tax = CalculateSalesTax(item);
                float totalPrice = item.Price + tax;
                totalSalesTax += item.Quantity * tax;
                totalAmount += item.Quantity * totalPrice;
                Console.WriteLine($"{item.Quantity} {item.Name}: {string.Format("{0:0.00}", totalPrice)}");
            }

            Console.WriteLine($"Sales Taxes: {string.Format("{0:0.00}", totalSalesTax)}");
            Console.WriteLine($"Total: {string.Format("{0:0.00}", totalAmount)}");
        }

        ShoppingItem parseShopingItem(string details)
        {
            var nameAndPrice = details.Split(" at ");
            if (nameAndPrice.Length != 2) throw new FormatException($"Invalid purchase format ");

            string quantityAndNamePart = nameAndPrice[0];
            float price = float.Parse(nameAndPrice[1]);

            var quantityAndName = quantityAndNamePart.Split(" ");
            if (quantityAndName.Length < 2) throw new FormatException("Invalid purchase format");
            var quantity = int.Parse(quantityAndName[0]);
            var name = quantityAndNamePart.Substring(quantityAndNamePart.IndexOf(' ') + 1);

            bool isImported = name.Contains("imported", StringComparison.OrdinalIgnoreCase);
            bool isExempt = name.Contains("book", StringComparison.OrdinalIgnoreCase) ||
                             name.Contains("chocolate", StringComparison.OrdinalIgnoreCase) ||
                            name.Contains("pill", StringComparison.OrdinalIgnoreCase);

            return new ShoppingItem(quantity, name, price, isExempt, isImported);
        }
    }
}
