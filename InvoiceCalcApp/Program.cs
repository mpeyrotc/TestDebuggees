namespace InvoiceCalcApp;

internal class Program
{
    /// <summary>
    /// Entry point. Supports two modes:
    /// - Interactive: prompts for input (used when debugging).
    /// - Command-line: pass item_name unit_price quantity as arguments.
    /// </summary>
    static void Main(string[] args)
    {
        string itemName;
        decimal unitPrice;
        int quantity;

        if (args.Length >= 3)
        {
            itemName = args[0];
            if (!decimal.TryParse(args[1], out unitPrice) || !int.TryParse(args[2], out quantity))
            {
                Console.WriteLine("Usage: InvoiceCalcApp <item_name> <unit_price> <quantity>");
                Environment.Exit(1);
                return;
            }
        }
        else
        {
            Console.WriteLine("=== Invoice Calculator ===");
            Console.WriteLine("Enter item details to generate an invoice.");
            Console.WriteLine();

            Console.Write("Item name: ");
            itemName = Console.ReadLine() ?? "Item";

            Console.Write("Unit price ($): ");
            if (!decimal.TryParse(Console.ReadLine(), out unitPrice))
            {
                Console.WriteLine("Invalid price.");
                return;
            }

            Console.Write("Quantity: ");
            if (!int.TryParse(Console.ReadLine(), out quantity))
            {
                Console.WriteLine("Invalid quantity.");
                return;
            }
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

        // Sanity check: tax on an order should never exceed the subtotal itself
        if (invoice.TaxAmount > invoice.Subtotal)
        {
            throw new InvalidOperationException(
                $"Sanity check failed: tax ${invoice.TaxAmount:F2} exceeds subtotal ${invoice.Subtotal:F2}. " +
                "The tax calculation is producing the after-tax total instead of just the tax portion.");
        }
    }
}
