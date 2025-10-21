namespace App;

public class Events
{
    //List<Participants> participants = new List<Participants>();
    //Date.Time lägga till tid för eventets start och stopp
    //Participants, roller
    //Title på event
    //Beskrivning på event
    //Plats för eventet

    public string Title;
    public string Description;
    public string EventStart;
    public string EventEnd;

    public Events(string title, string description, string eventStart, string eventEnd)
    {
        Title = title;
        Description = description;
        EventStart = eventStart;
        EventEnd = eventEnd;
    }

    public static Events NewEntry(string title, string description, string eventStart)
    {
        // Console.Write("Do you want to enter date and time manually? y/n");
        // string input = Console.ReadLine();

        string eventEnd = DateTime.Now.ToString();

        Events newEvent = new(title, description, eventStart, eventEnd);

        string[] eventToAdd =
        {
            "---Title---\n"
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
