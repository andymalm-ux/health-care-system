namespace App;

public class Events
{
    public string EventType;
    public string Title;
    public string Description;
    public string EventStart;
    public string EventEnd;

    public Events(
        string eventType,
        string title,
        string description,
        string eventStart,
        string eventEnd
    )
    {
        EventType = eventType;
        Title = title;
        Description = description;
        EventStart = eventStart;
        EventEnd = eventEnd;
    }

    public static Events NewEntry(
        string eventType,
        string title,
        string description,
        string eventStart
    )
    {
        string eventEnd = DateTime.Now.ToString();

        Events newEvent = new(eventType, title, description, eventStart, eventEnd);

        string[] eventToAdd =
        {
            "---Type of Event---\n"
                + eventType
                + "\n---Title---\n"
                + title
                + "\n---Description---\n"
                + description
                + "\n---When---\n"
                + eventStart
                + " - "
                + eventEnd
                + "\n",
        };

        File.AppendAllLines("Event.txt", eventToAdd);

        return newEvent;
    }
}
