namespace App;

public class Duration
{
    public string EventStartDate;
    public string EventStartTime;
    public string EventEndDate;
    public string EventEndTime;

    public Duration(
        string eventStartDate,
        string eventStartTime,
        string eventEndDate,
        string eventEndTime
    )
    {
        EventStartDate = eventStartDate;
        EventStartTime = eventStartTime;
        EventEndDate = eventStartDate;
        EventEndTime = eventEndTime;
    }
}
