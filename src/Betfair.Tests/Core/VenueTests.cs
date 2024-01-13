using Betfair.Core;

namespace Betfair.Tests.Core;

public class VenueTests
{
    [Theory]
    [InlineData("New Venue")]
    [InlineData("Other")]
    public void VenueCanBeCreateFromStringValue(string venueId) =>
        Venue.Of(venueId).Id.Should().Be(venueId);

    [Fact]
    public void CanCreateVenueAlbany() =>
        Venue.Albany.Id.Should().Be("Albany");

    [Fact]
    public void CanCreateVenueAliceSprings() =>
        Venue.AliceSprings.Id.Should().Be("Alice Springs");

    [Fact]
    public void CanCreateVenueAqueduct() =>
        Venue.Aqueduct.Id.Should().Be("Aqueduct");

    [Fact]
    public void CanCreateVenueCentralPark() =>
        Venue.CentralPark.Id.Should().Be("Central Park");

    [Fact]
    public void CanCreateVenueCharlesTown() =>
        Venue.CharlesTown.Id.Should().Be("Charles Town");

    [Fact]
    public void CanCreateVenueChelmsfordCity() =>
        Venue.ChelmsfordCity.Id.Should().Be("Chelmsford City");

    [Fact]
    public void CanCreateVenueCoffsHarbour() =>
        Venue.CoffsHarbour.Id.Should().Be("Coffs Harbour");

    [Fact]
    public void CanCreateVenueCrayford() =>
        Venue.Crayford.Id.Should().Be("Crayford");

    [Fact]
    public void CanCreateVenueDeltaDowns() =>
        Venue.DeltaDowns.Id.Should().Be("Delta Downs");

    [Fact]
    public void CanCreateVenueDoncaster() =>
        Venue.Doncaster.Id.Should().Be("Doncaster");

    [Fact]
    public void CanCreateVenueEllerslie() =>
        Venue.Ellerslie.Id.Should().Be("Ellerslie");

    [Fact]
    public void CanCreateVenueFairGrounds() =>
        Venue.FairGrounds.Id.Should().Be("Fair Grounds");

    [Fact]
    public void CanCreateVenueGoldenGateFields() =>
        Venue.GoldenGateFields.Id.Should().Be("Golden Gate Fields");

    [Fact]
    public void CanCreateVenueGulfstreamPark() =>
        Venue.GulfstreamPark.Id.Should().Be("Gulfstream Park");

    [Fact]
    public void CanCreateVenueHereford() =>
        Venue.Hereford.Id.Should().Be("Hereford");

    [Fact]
    public void CanCreateVenueHove() =>
        Venue.Hove.Id.Should().Be("Hove");

    [Fact]
    public void CanCreateVenueKelso() =>
        Venue.Kelso.Id.Should().Be("Kelso");

    [Fact]
    public void CanCreateVenueLaurelPark() =>
        Venue.LaurelPark.Id.Should().Be("Laurel Park");

    [Fact]
    public void CanCreateVenueLingfield() =>
        Venue.Lingfield.Id.Should().Be("Lingfield");

    [Fact]
    public void CanCreateVenueMonmore() =>
        Venue.Monmore.Id.Should().Be("Monmore");

    [Fact]
    public void CanCreateVenueNewcastle() =>
        Venue.Newcastle.Id.Should().Be("Newcastle");

    [Fact]
    public void CanCreateVenueNowra() =>
        Venue.Nowra.Id.Should().Be("Nowra");

    [Fact]
    public void CanCreateVenueOaklawnPark() =>
        Venue.OaklawnPark.Id.Should().Be("Oaklawn Park");

    [Fact]
    public void CanCreateVenueOxford() =>
        Venue.Oxford.Id.Should().Be("Oxford");

    [Fact]
    public void CanCreateVenuePau() =>
        Venue.Pau.Id.Should().Be("Pau");

    [Fact]
    public void CanCreateVenuePerryBarr() =>
        Venue.PerryBarr.Id.Should().Be("Perry Barr");

    [Fact]
    public void CanCreateVenuePunchestown() =>
        Venue.Punchestown.Id.Should().Be("Punchestown");

    [Fact]
    public void CanCreateVenueRomford() =>
        Venue.Romford.Id.Should().Be("Romford");

    [Fact]
    public void CanCreateVenueSale() =>
        Venue.Sale.Id.Should().Be("Sale");

    [Fact]
    public void CanCreateVenueSantaAnitaPark() =>
        Venue.SantaAnitaPark.Id.Should().Be("Santa Anita Park");

    [Fact]
    public void CanCreateVenueScottsville() =>
        Venue.Scottsville.Id.Should().Be("Scottsville");

    [Fact]
    public void CanCreateVenueStrathalbyn() =>
        Venue.Strathalbyn.Id.Should().Be("Strathalbyn");

    [Fact]
    public void CanCreateVenueSunshineCoast() =>
        Venue.SunshineCoast.Id.Should().Be("Sunshine Coast");

    [Fact]
    public void CanCreateVenueSwindon() =>
        Venue.Swindon.Id.Should().Be("Swindon");

    [Fact]
    public void CanCreateVenueTampaBayDowns() =>
        Venue.TampaBayDowns.Id.Should().Be("Tampa Bay Downs");

    [Fact]
    public void CanCreateVenueTurffontein() =>
        Venue.Turffontein.Id.Should().Be("Turffontein");

    [Fact]
    public void CanCreateVenueTurfwayPark() =>
        Venue.TurfwayPark.Id.Should().Be("Turfway Park");

    [Fact]
    public void CanCreateVenueWarwick() =>
        Venue.Warwick.Id.Should().Be("Warwick");

    [Fact]
    public void CanCreateVenueWetherby() =>
        Venue.Wetherby.Id.Should().Be("Wetherby");

    [Fact]
    public void CanCreateVenueWodonga() =>
        Venue.Wodonga.Id.Should().Be("Wodonga");

    [Fact]
    public void CanCreateVenueWolverhampton() =>
        Venue.Wolverhampton.Id.Should().Be("Wolverhampton");
}
