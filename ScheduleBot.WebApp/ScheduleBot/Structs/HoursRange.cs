namespace ScheduleBot.Structs;

public readonly struct HoursRange
{
    private readonly int _start;
    private readonly int _end;
    
    public HoursRange(int start, int end)
    {
        if (start is < 0 or > 23)
            throw new ArgumentException("Параметр start не может быть большее 23 или меньше 0");
        
        if (end is < 0 or > 23)
            throw new ArgumentException("Параметр end не может быть большее 23 или меньше 0");
        
        _start = start;
        _end = end;
    }

    private bool Equals(HoursRange other) => (_start == other._start) && (_end == other._end);

    public override bool Equals(object? obj) => obj is HoursRange other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(_start, _end);

    public static bool operator ==(HoursRange range, int hour)
    {
        return range._start <= range._end
            ? (hour >= range._start) && (hour <= range._end)
            : (hour >= range._start) || (hour <= range._end);  
    }

    public static bool operator !=(HoursRange range, int hour)
    {
        return range._start <= range._end
            ? !((hour >= range._start) && (hour <= range._end))
            : !((hour >= range._start) || (hour <= range._end));  
    }

    public static bool operator ==(int hour, HoursRange range)
    {
        return range._start <= range._end
            ? (hour >= range._start) && (hour <= range._end)
            : (hour >= range._start) || (hour <= range._end);
    }

    public static bool operator !=(int hour, HoursRange range)
    {
        return range._start <= range._end
            ? !((hour >= range._start) && (hour <= range._end))
            : !((hour >= range._start) || (hour <= range._end));
    }
}