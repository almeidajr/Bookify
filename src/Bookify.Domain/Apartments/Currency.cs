namespace Bookify.Domain.Apartments;

public record Currency
{
    public static readonly Currency Usd = new("USD");
    public static readonly Currency Eur = new("EUR");
    public static readonly Currency Brl = new("BRL");

    internal static readonly Currency None = new(string.Empty);

    public static readonly IReadOnlyCollection<Currency> All = new[]
    {
        Usd,
        Eur,
        Brl
    };

    private Currency(string code)
    {
        Code = code;
    }

    public string Code { get; init; }

    public static Currency FromCode(string code)
    {
        return All.FirstOrDefault(c => c.Code == code) ??
               throw new ApplicationException("The currency code is invalid");
    }
}