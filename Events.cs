namespace App;

public class Events
{
    //List<Participants> participants = new List<Participants>();
    //Date.Time lägga till tid för eventets start och stopp
    //Participants, roller
    //Title på event
    //Beskrivning på event
    //Plats för eventet

    public string EventType;
    public string Title;
    public string Description;
    public string EventStart;
    public string EventEnd;
    List<Duration> duration = new();

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
        string eventStartDate = "";
        string eventStartTime = "";
        string eventEndDate = "";
        string eventEndTime = "";

        Console.Write("Do you want to enter date and time manually? y/n");
        string input = Console.ReadLine();

        if (input == "y")
        {
            Console.Write("Start Date: ");
            eventStartDate = Console.ReadLine();
            Console.Write("Start Time: ");
            eventStartTime = Console.ReadLine();
            Console.Write("End Date: ");
            eventEndDate = Console.ReadLine();
            Console.Write("End Time: ");
            eventEndTime = Console.ReadLine();

            Duration durationsStart = new(eventStartDate, eventStartTime);
            Duration durationsEnd = new(eventEndDate, eventEndTime);

            Events events = new(eventType, title, description, durationsStart, durationsEnd);
        }

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
