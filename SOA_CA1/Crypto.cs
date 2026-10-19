namespace SOA_CA1;

public class Crypto
{
    private string _id;
    private string _name;
    private string _symbol;
    private string _icon = string.Empty;
    private decimal _price;
    private decimal _volume;
    private decimal _marketCap;
    private decimal _totalSupply;
    private decimal _allTimeHigh;
    private decimal _allTimeLow;
    private decimal _priceChange1h;
    private List<float> _chartPoints = [];

    public string Id
    {
        get => _id;
        set => _id = value ?? throw new ArgumentNullException(nameof(value));
    }

    public string Name
    {
        get => _name;
        set => _name = value ?? throw new ArgumentNullException(nameof(value));
    }

    public string Symbol
    {
        get => _symbol;
        set => _symbol = value ?? throw new ArgumentNullException(nameof(value));
    }
    
    public string Icon
    {
        get => _icon;
        set => _icon = value ?? string.Empty;
    }

    public decimal Price
    {
        get => _price;
        set => _price = value < 0 ? 0 : value;
    }

    public decimal Volume
    {
        get => _volume;
        set => _volume = value < 0 ? 0 : value;
    }

    public decimal MarketCap
    {
        get => _marketCap;
        set => _marketCap = value < 0 ? 0 : value;
    }

    public decimal TotalSupply
    {
        get => _totalSupply;
        set => _totalSupply = value < 0 ? 0 : value;
    }

    public decimal AllTimeHigh
    {
        get => _allTimeHigh;
        set => _allTimeHigh = value < 0 ? 0 : value;
    }

    public decimal AllTimeLow
    {
        get => _allTimeLow;
        set => _allTimeLow = value < 0 ? 0 : value;
    }

    public decimal PriceChange1h
    {
        get => _priceChange1h;
        set => _priceChange1h = value > -100
            ? value
            : throw new ArgumentOutOfRangeException(nameof(value), value, "1h price change must be greater than -100%.");
    }

    public List<float> ChartPoints
    {
        get => _chartPoints;
        set => _chartPoints = value ?? throw new ArgumentNullException(nameof(value));
    }
    
    public decimal PriceChange1hAmount
    {
        get => Price - Price / (1 + PriceChange1h / 100);
    }

    public Crypto()
    {
        _id = "undefined";
        _name = "undefined";
        _symbol = "undefined";
        _price = 0m;
        _volume = 0m;
        _marketCap = 0m;
        _totalSupply = 0m;
        _priceChange1h = 0m;
    }

    public Crypto(string id, string name, string symbol, decimal price, decimal volume, decimal marketCap,
        decimal totalSupply, decimal priceChange1h)
    {
        _id = id ?? throw new ArgumentNullException(nameof(id));
        _name = name ?? throw new ArgumentNullException(nameof(name));
        _symbol = symbol ?? throw new ArgumentNullException(nameof(symbol));
        Price = price;
        Volume = volume;
        MarketCap = marketCap;
        TotalSupply = totalSupply;
        PriceChange1h = priceChange1h;
    }

    public Crypto(string id, string name, string symbol, decimal price, decimal volume, decimal marketCap,
        decimal totalSupply, decimal priceChange1h, List<float> chartPoints)
        : this(id, name, symbol, price, volume, marketCap, totalSupply, priceChange1h)
    {
        _chartPoints = chartPoints ?? throw new ArgumentNullException(nameof(chartPoints));
    }

    public override string ToString() => $"{Name} ({Symbol})";
}
