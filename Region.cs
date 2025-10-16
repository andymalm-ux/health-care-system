namespace App;

public enum Regions
{
    Halland,
}

public class Location
{
    public string LocationName;
    public string LocationAdress;
    public string LocationDesc;

    public Regions region;

    public Location(string locationname, string locationadress, string locationdesc, Regions reg)
    {
        LocationName = locationname;
        LocationAdress = locationadress;
        LocationDesc = locationdesc;
        region = reg;
    }

    public string locationInfo()
    {
        return $"{LocationName}\n---\n{LocationAdress}\n---\n{LocationDesc}\n---\n{region}";
    }
}
