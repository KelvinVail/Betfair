namespace Betfair.Core;

public sealed class Venue
{
    private Venue(string id) => Id = id;

    public static Venue Albany => new ("Albany");

    public static Venue AliceSprings => new ("Alice Springs");

    public static Venue Aqueduct => new ("Aqueduct");

    public static Venue CentralPark => new ("Central Park");

    public static Venue CharlesTown => new ("Charles Town");

    public static Venue ChelmsfordCity => new ("Chelmsford City");

    public static Venue CoffsHarbour => new ("Coffs Harbour");

    public static Venue Crayford => new ("Crayford");

    public static Venue DeltaDowns => new ("Delta Downs");

    public static Venue Doncaster => new ("Doncaster");

    public static Venue Ellerslie => new ("Ellerslie");

    public static Venue FairGrounds => new ("Fair Grounds");

    public static Venue GoldenGateFields => new ("Golden Gate Fields");

    public static Venue GulfstreamPark => new ("Gulfstream Park");

    public static Venue Hereford => new ("Hereford");

    public static Venue Hove => new ("Hove");

    public static Venue Kelso => new ("Kelso");

    public static Venue LaurelPark => new ("Laurel Park");

    public static Venue Lingfield => new ("Lingfield");

    public static Venue Monmore => new ("Monmore");

    public static Venue Newcastle => new ("Newcastle");

    public static Venue Nowra => new ("Nowra");

    public static Venue OaklawnPark => new ("Oaklawn Park");

    public static Venue Oxford => new ("Oxford");

    public static Venue Pau => new ("Pau");

    public static Venue PerryBarr => new ("Perry Barr");

    public static Venue Punchestown => new ("Punchestown");

    public static Venue Romford => new ("Romford");

    public static Venue Sale => new ("Sale");

    public static Venue SantaAnitaPark => new ("Santa Anita Park");

    public static Venue Scottsville => new ("Scottsville");

    public static Venue Strathalbyn => new ("Strathalbyn");

    public static Venue SunshineCoast => new ("Sunshine Coast");

    public static Venue Swindon => new ("Swindon");

    public static Venue TampaBayDowns => new ("Tampa Bay Downs");

    public static Venue Turffontein => new ("Turffontein");

    public static Venue TurfwayPark => new ("Turfway Park");

    public static Venue Warwick => new ("Warwick");

    public static Venue Wetherby => new ("Wetherby");

    public static Venue Wodonga => new ("Wodonga");

    public static Venue Wolverhampton => new ("Wolverhampton");

    public string Id { get; init; }

    public static Venue Of(string id) =>
        new (id);
}
