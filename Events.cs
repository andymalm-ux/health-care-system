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

    public Events(string title, string description, string eventStart)
    {
        Title = title;
        Description = description;
        EventStart = eventStart;
    }

    public static Events NewEntry(string title, string description)
    {
        //List<Events> events = new();
        string eventStart = DateTime.Now.ToString();

        Events newEvent = new(title, description, eventStart);

        // events.Add(newEvent);

        return newEvent;

        /*foreach (Events e in events)
        {
            if (e != null)
            {
                Console.WriteLine(e.Title + "\n" + e.Description + "\n" + e.EventStart);
            }
            else
            {
                Console.WriteLine("Error");
            }
        }*/
    }
}
