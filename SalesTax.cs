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
//App is a idle class which may use Bill System
public class App
{
    public static void Main()
    {
        Bill bill = new Bill();
        string[] ExemptedItems = { "pills", "chocolate","book"};
        TaxUtility taxUtility= new TaxUtility(.1f,.05f,ExemptedItems);
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

        Receipt receipt = bill.GenerateReceipt(taxUtility);
        //bill.RemoveItem("100 packet of headache pills at 9.75");
        //receipt = new Receipt(bill,taxUtility);

        receipt.Print();

    }
}

//here is the implimentation of Bill System
namespace BillingSystem
{
    public struct ShoppingItem
    {
        public int Quantity { get; }
        public string Name { get; }
        public float Price { get; }
        public bool IsImported { get; }

        public ShoppingItem(int quantity, string name, float price,bool isImported)
        {
            Quantity = quantity;
            Name = name;
            Price = price;
            IsImported = isImported;
        }
    }
    public static class BillingSystemUtilities
    {
        public static ShoppingItem parseShoppingItem(string details)
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

            return new ShoppingItem(quantity, name, price, isImported);
        }
    }

    public class Bill
    {
        List<ShoppingItem> items = new List<ShoppingItem>();
        public IReadOnlyList<ShoppingItem> Items => items.AsReadOnly();
        public void AddItem(string input)
        {
            try
            {
                ShoppingItem item = BillingSystemUtilities.parseShoppingItem(input);
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
                ShoppingItem item = BillingSystemUtilities.parseShoppingItem(input);
                if (items.Contains(item)) items.Remove(item);
            }
            catch (Exception ex)
            {
                throw new FormatException($"{ex}");
            }
        }

        public Receipt GenerateReceipt(TaxUtility taxUtility)
        {
            return new Receipt(this,taxUtility);
        }
    }
    public class TaxUtility
    {
        public float SalesTax {get;} = .1f;
        public float ImportTax {get;} = .05f;
        public string[] Exempted {get;}

        public TaxUtility(float SalesTax, float ImportTax, string[] ExemptedItems){
            this.SalesTax = SalesTax;
            this.ImportTax = ImportTax;
            this.Exempted = ExemptedItems;
        }

        public bool IsExempt(string itemName){
            foreach(var item in Exempted ) {
                if(itemName.Contains(item, StringComparison.OrdinalIgnoreCase)) return true;
            }
            return false;
        }
        public float CalculateSalesTax(float Price , string Name,bool IsImported)
        {
            float taxRate = 0.0f;

            if (!IsExempt(Name)) taxRate += SalesTax;

            if (IsImported) taxRate += ImportTax;

            float calculatedTax = Price * taxRate;
            return (float)Math.Ceiling(calculatedTax * 20) / 20;
        }
    }
    public class Receipt
    {
        public Bill bill {get;}
        public TaxUtility taxUtility{get;}
        public Receipt(Bill bill,TaxUtility taxUtility)
        {
            this.bill = bill;
            this.taxUtility = taxUtility;
        }

        public void Print()
        {
            if(bill == null || taxUtility == null){
                throw new FormatException("Bill or TaxUtility is not correctly configured");
                return;
            }
            float totalSalesTax = 0.0f;
            float totalAmount = 0.0f;
            foreach (var item in bill.Items)
            {
                float tax = taxUtility.CalculateSalesTax(item.Price,item.Name,item.IsImported);
                float totalPrice = item.Price + tax;
                totalSalesTax += item.Quantity * tax;
                totalAmount += item.Quantity * totalPrice;
                Console.WriteLine($"{item.Quantity} {item.Name}: {string.Format("{0:0.00}", totalPrice)}");
            }

            Console.WriteLine($"Sales Taxes: {string.Format("{0:0.00}", totalSalesTax)}");
            Console.WriteLine($"Total: {string.Format("{0:0.00}", totalAmount)}");
        }
    }
}
