namespace SOA_CA1;

// One price on the Crypto Market chart. A record (reference type) because ApexCharts requires TItem to be a class.
public record ChartPoint(DateTime Time, decimal Price);
