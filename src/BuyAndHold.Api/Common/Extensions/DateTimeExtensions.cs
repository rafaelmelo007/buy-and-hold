namespace BuyAndHold.Api.Common.Extensions;
public static class DateTimeExtensions
{
    public static DateOnly AddWorkingDays(this DateOnly date, int days, HashSet<DateOnly> holidays = null)
    {
        var currentDate = date;
        int direction = days < 0 ? -1 : 1;

        while (days != 0)
        {
            currentDate = currentDate.AddDays(direction);

            if (IsWorkingDay(currentDate, holidays))
            {
                days -= direction;
            }
        }

        return currentDate;
    }

    private static bool IsWorkingDay(DateOnly date, HashSet<DateOnly> holidays)
    {
        // Check if the day is a weekend (Saturday or Sunday)
        if (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday)
        {
            return false;
        }

        // Check if the day is a holiday
        if (holidays != null && holidays.Contains(date))
        {
            return false;
        }

        return true;
    }
}
