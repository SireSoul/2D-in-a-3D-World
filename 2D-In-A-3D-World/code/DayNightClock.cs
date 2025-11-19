using Raylib_cs;

public class DayNightClock
{
    public int Hour24 { get; private set; }   // 0..23
    public int Minute { get; private set; }   // 0..59

    // Derived properties
    public bool IsAM => Hour24 < 12;

    public int Hour12
        => Hour24 == 0 ? 12        // 00:xx -> 12:xx AM
         : Hour24 > 12 ? Hour24 - 12
         : Hour24;                 // 01..12

    public string AmPm => IsAM ? "AM" : "PM";

    // How much “fractional minutes” we’ve accumulated from dt
    private float minuteAccumulator = 0f;

    public DayNightClock(int startHour24 = 6, int startMinute = 0)
    {
        SetTime(startHour24, startMinute);
    }

    public void SetTime(int hour24, int minute)
    {
        // Normalize into valid ranges
        hour24 = ((hour24 % 24) + 24) % 24;   // handle negatives just in case
        minute = ((minute % 60) + 60) % 60;

        Hour24 = hour24;
        Minute = minute;
    }

    /// <summary>
    /// Advance the clock based on real time.
    /// minutesPerRealSecond = how many in-game minutes pass per 1 real second.
    /// e.g. 1f = 1 minute/sec, 0.5f = 1 minute every 2 seconds, 10f = very fast days.
    /// </summary>
    public void Update(float dt, float minutesPerRealSecond)
    {
        minuteAccumulator += dt * minutesPerRealSecond;

        while (minuteAccumulator >= 1f)
        {
            minuteAccumulator -= 1f;
            AdvanceOneMinute();
        }

        if (Raylib.IsKeyDown(KeyboardKey.LeftShift) && Raylib.IsKeyDown(KeyboardKey.Equal))
        {
            AdvanceTenMinutes();
        }
    }

    private void AdvanceOneMinute()
    {
        Minute++;
        if (Minute >= 60)
        {
            Minute = 0;
            Hour24 = (Hour24 + 1) % 24;
        }
    }

    public override string ToString()
    {
        return $"{Hour12:00}:{Minute:00} {AmPm}";
    }

    private void AdvanceTenMinutes()
    {
        Minute += 10;
        if (Minute >= 60)
        {
            Minute = 0;
            Hour24 = (Hour24 + 1) % 24;
        }
    }

    /// <summary>
    /// Returns a 0..1 fraction of the current day:
    /// 0.0 = midnight, 0.5 = noon, 1.0 ~= next midnight.
    /// Perfect to drive your day/night brightness.
    /// </summary>
    public float NormalizedTimeOfDay()
    {
        float totalMinutes = Hour24 * 60f + Minute;
        return totalMinutes / (24f * 60f);
    }

    public void Draw()
    {
        Raylib.DrawRectangle(0, 0, 110, 35, new Color(0, 0, 0, 180));
        string timeText = ToString();
        Raylib.DrawText(timeText, 10, 10, 20, Color.White);
    }
}