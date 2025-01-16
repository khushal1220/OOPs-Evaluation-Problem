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

namespace SalesTaxPragram{
    public class ShoppingItem{
        public int Quantity;
        public string Name;
        public float Price;
        public bool IsExempted = false;
        public bool IsImported = false;
        
        public ShoppingItem(int quantity,string name,float price,bool isExempted,bool isImported){
            
            Quantity = quantity;
            Name = name;
            Price = price;
            IsImported = isImported;
            IsExempted = isExempted;
        }
    }
    
    public class Receipt
    {
        private readonly List<ShoppingItem> items = new List<ShoppingItem>();
        private float totalSalesTax = 0.0f;
        private float totalAmount = 0.0f;

        public void AddItem(ShoppingItem item)
        {
            float tax = CalculateSalesTax(item);
            float totalPrice = item.Price + tax;

            items.Add(item);
            totalSalesTax += tax * item.Quantity;
            totalAmount += totalPrice * item.Quantity;
        }
        
        public static float CalculateSalesTax(ShoppingItem item)
        {
            float taxRate = 0.0f;

            // Non-exempt items get a basic 10% tax
            if (!item.IsExempted) taxRate += 0.10f;

            // Imported items get an extra 5% tax
            if (item.IsImported) taxRate += 0.05f;

            // Tax is rounded up to the nearest 0.05
            float calculatedTax = item.Price * taxRate;
            return (float)Math.Ceiling(calculatedTax * 20) / 20;
        }
        
        public void Print()
        {
            foreach (var item in items)
            {
                float tax = CalculateSalesTax(item);
                float totalPrice = item.Price + tax;
                Console.WriteLine($"{item.Quantity} {item.Name}: {string.Format("{0:0.00}", totalPrice)}");
            }

            Console.WriteLine($"Sales Taxes: {string.Format("{0:0.00}", totalSalesTax)}");
            Console.WriteLine($"Total: {string.Format("{0:0.00}", totalAmount)}");
        }
    }
    public class Run
    {
    	public static void Main()
    	{
    	    Receipt receipt = new Receipt();
    		string input = Console.ReadLine();
            while (!string.IsNullOrWhiteSpace(input)){
                
                try{
                    ShoppingItem item;
                    item = parseShopingItem(input);
                    receipt.AddItem(item);
                    input = Console.ReadLine();
                }catch(Exception ex){
                    Console.WriteLine($"{ex}");
                    return;
                    break;
                }

            }
            receipt.Print();
    	}
    	
    	public  static ShoppingItem parseShopingItem(string details){
    	    var nameAndPrice = details.Split(" at ");
    	    if (nameAndPrice.Length != 2)throw new FormatException($"Invalid purchase format ");
    	    
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
                            
            return new ShoppingItem(quantity,name,price,isExempt,isImported);               
    	}
    }
}
