namespace SOA_CA1;

public class Currency : IComparable<Currency>
{
    private string _code;
    private string _name;

    public string Code
    {
        get => _code;
        set => _code = value ?? throw new ArgumentNullException(nameof(value));
    }

    public string Name
    {
        get => _name;
        set => _name = value ?? throw new ArgumentNullException(nameof(value));
    }

    public Currency()
    {
        _code = "undefined";
        _name = "undefined";
    }

    public Currency(string code, string name)
    {
        _code = code ?? throw new ArgumentNullException(nameof(code));
        _name = name ?? throw new ArgumentNullException(nameof(name));
    }

    public int CompareTo(Currency? other)
    {
        if (other is null)
        {
            return 1;
        }

        return string.Compare(Code, other.Code, StringComparison.Ordinal);
    }

    public override string ToString() => $"{Code} — {Name}";
}
