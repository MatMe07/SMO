namespace Vichislenia
{
    public enum TimeUnit
    {
        Minute,
        Hour,
        Day
    }

    public static class TimeUnitExtensions
    {
        public static string Ru(this TimeUnit unit) => unit switch
        {
            TimeUnit.Minute => "мин",
            TimeUnit.Hour => "ч",
            TimeUnit.Day => "сут",
            _ => "?"
        };
    }
}
