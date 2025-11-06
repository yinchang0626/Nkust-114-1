namespace C111181149.Models;

public sealed class MarriageCount
{
    public int Year { get; init; }
    public int Male { get; init; }
    public int Female { get; init; }

    public int Total => Male + Female;

    public override string ToString() => $"Year={Year}, Male={Male}, Female={Female}, Total={Total}";
}
