using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Money
{
    public decimal Amount { get; private set; }
    public string CurrencyCode { get; private set; }

    private static readonly Dictionary<string, string> SymbolToCode = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
    {
        { "$", "USD" },
        { "PHP", "PHP" },
        { "₱", "PHP" },
        { "€", "EUR" },
        { "GBP", "GBP" }
    };

    public Money(string input)
    {
        Parse(input);
    }

    private void Parse(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            throw new FormatException("Empty or null input.");

        input = input.Trim();

        string currencyCode = null;
        foreach (var kvp in SymbolToCode)
        {
            if (input.StartsWith(kvp.Key, StringComparison.OrdinalIgnoreCase) ||
                input.EndsWith(kvp.Key, StringComparison.OrdinalIgnoreCase))
            {
                currencyCode = kvp.Value;
                break;
            }
        }

        if (currencyCode == null)
            currencyCode = "USD"; 

        bool hasComma = input.Contains(",");
        bool hasDot = input.Contains(".");

        decimal value;

        if (hasComma && hasDot)
        {
            var cleaned = input.Replace(",", "");
            if (!decimal.TryParse(cleaned, NumberStyles.Number, CultureInfo.InvariantCulture, out value))
                throw new FormatException($"Invalid price format: {input}");
        }
        else if (hasComma && !hasDot)
        {
            if (input.Length - input.LastIndexOf(",") <= 3)
            {
                var cleaned = input.Replace(".", "").Replace(",", ".");
                if (!decimal.TryParse(cleaned, NumberStyles.Number, CultureInfo.InvariantCulture, out value))
                    throw new FormatException($"Invalid price format: {input}");
            }
            else
            {
                throw new FormatException($"Ambiguous format: {input}");
            }
        }
        else
        {
            if (!decimal.TryParse(input, NumberStyles.Number, CultureInfo.InvariantCulture, out value))
                throw new FormatException($"Invalid price format: {input}");
        }

        Amount = value;
        CurrencyCode = currencyCode;
    }

    public override string ToString()
    {
        return $"{CurrencyCode} {Amount:F2}";
    }
}

namespace _30__Currency_Formatter___Parser
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var inputs = new List<string>
        {
            "$1,299.50",
            "1299,50",
            "PHP 1299.5",
            "€1.299,75",
            "GBP 500"
        };

            var valid = new List<Money>();
            var invalid = new List<string>();

            foreach (var inp in inputs)
            {
                try
                {
                    var money = new Money(inp);
                    valid.Add(money);
                }
                catch (FormatException ex)
                {
                    invalid.Add($"{inp} → {ex.Message}");
                }
            }

            var sorted = valid.OrderBy(m => m.Amount).ToList();

            Console.WriteLine("Valid prices (sorted):");
            foreach (var m in sorted)
                Console.WriteLine(m);

            Console.WriteLine("\nInvalid prices:");
            foreach (var err in invalid)
                Console.WriteLine(err);
        }
    }
}
