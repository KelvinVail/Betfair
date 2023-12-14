namespace Betfair.Core;

public class Country
{
    private Country(string id) => Id = id;

    public static Country Algeria => new ("DZ");

    public static Country Argentina => new ("AR");

    public static Country Australia => new ("AU");

    public static Country Azerbaijan => new ("AZ");

    public static Country Bahrain => new ("BH");

    public static Country Bangladesh => new ("BD");

    public static Country Belgium => new ("BE");

    public static Country Bolivia => new ("BO");

    public static Country BosniaAndHerzegovina => new ("BA");

    public static Country BruneiDarussalam => new ("BN");

    public static Country Bulgaria => new ("BG");

    public static Country CostaRica => new ("CR");

    public static Country Croatia => new ("HR");

    public static Country CzechRepublic => new ("CZ");

    public static Country Denmark => new ("DK");

    public static Country Egypt => new ("EG");

    public static Country Ethiopia => new ("ET");

    public static Country Finland => new ("FI");

    public static Country France => new ("FR");

    public static Country Gambia => new ("GM");

    public static Country Germany => new ("DE");

    public static Country Ghana => new ("GH");

    public static Country Gibraltar => new ("GI");

    public static Country Greece => new ("GR");

    public static Country Hungary => new ("HU");

    public static Country India => new ("IN");

    public static Country Indonesia => new ("ID");

    public static Country Ireland => new ("IE");

    public static Country Israel => new ("IL");

    public static Country Italy => new ("IT");

    public static Country Jordan => new ("JO");

    public static Country Kenya => new ("KE");

    public static Country Kuwait => new ("KW");

    public static Country Malta => new ("MT");

    public static Country Mexico => new ("MX");

    public static Country Myanmar => new ("MM");

    public static Country Netherlands => new ("NL");

    public static Country NewZealand => new ("NZ");

    public static Country Poland => new ("PL");

    public static Country Portugal => new ("PT");

    public static Country Qatar => new ("QA");

    public static Country Romania => new ("RO");

    public static Country SaudiArabia => new ("SA");

    public static Country Slovakia => new ("SK");

    public static Country Slovenia => new ("SI");

    public static Country SouthAfrica => new ("ZA");

    public static Country Spain => new ("ES");

    public static Country Switzerland => new ("CH");

    public static Country Tanzania => new ("TZ");

    public static Country Thailand => new ("TH");

    public static Country Tunisia => new ("TN");

    public static Country Turkey => new ("TR");

    public static Country UnitedArabEmirates => new ("AE");

    public static Country UnitedKingdom => new ("GB");

    public static Country UnitedStates => new ("US");

    public static Country Uruguay => new ("UY");

    public static Country VietNam => new ("VN");

    public string Id { get; private set; }

    public static Country Of(string isoCode) => new (isoCode);
}
