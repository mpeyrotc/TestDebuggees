namespace InvoiceCalcApp;

internal class Program
{
    static void Main()
    {
        Console.WriteLine("=== Invoice Calculator ===");
        Console.WriteLine("Enter item details to generate an invoice.");
        Console.WriteLine();

        Console.Write("Item name: ");
        string itemName = Console.ReadLine() ?? "Item";

        Console.Write("Unit price ($): ");
        if (!decimal.TryParse(Console.ReadLine(), out decimal unitPrice))
        {
            Console.WriteLine("Invalid price.");
            return;
        }

        Console.Write("Quantity: ");
        if (!int.TryParse(Console.ReadLine(), out int quantity))
        {
            Console.WriteLine("Invalid quantity.");
            return;
        }

        var invoice = InvoiceGenerator.Generate(itemName, unitPrice, quantity);
        PrintInvoice(invoice);
    }

    static void PrintInvoice(Invoice invoice)
    {
        Console.WriteLine();
        Console.WriteLine("--- INVOICE ---");
        Console.WriteLine($"Item:     {invoice.ItemName}");
        Console.WriteLine($"Price:    ${invoice.UnitPrice:F2} x {invoice.Quantity}");
        Console.WriteLine($"Subtotal: ${invoice.Subtotal:F2}");
        Console.WriteLine($"Tax (8%): ${invoice.TaxAmount:F2}");
        Console.WriteLine($"Total:    ${invoice.Total:F2}");
        Console.WriteLine("---------------");
    }
}
