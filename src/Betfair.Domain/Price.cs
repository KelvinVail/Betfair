namespace Betfair.Domain;

public class Price : ValueObject
{
    private static readonly Price[] _validPrices =
    {
        new (1.01, 0), new (1.02, 1), new (1.03, 2), new (1.04, 3), new (1.05, 4), new (1.06, 5), new (1.07, 6), new (1.08, 7), new (1.09, 8), new (1.1, 9),
        new (1.11, 10), new (1.12, 11), new (1.13, 12), new (1.14, 13), new (1.15, 14), new (1.16, 15), new (1.17, 16), new (1.18, 17), new (1.19, 18), new (1.2, 19),
        new (1.21, 20), new (1.22, 21), new (1.23, 22), new (1.24, 23), new (1.25, 24), new (1.26, 25), new (1.27, 26), new (1.28, 27), new (1.29, 28), new (1.3, 29),
        new (1.31, 30), new (1.32, 31), new (1.33, 32), new (1.34, 33), new (1.35, 34), new (1.36, 35), new (1.37, 36), new (1.38, 37), new (1.39, 38), new (1.4, 39),
        new (1.41, 40), new (1.42, 41), new (1.43, 42), new (1.44, 43), new (1.45, 44), new (1.46, 45), new (1.47, 46), new (1.48, 47), new (1.49, 48), new (1.5, 49),
        new (1.51, 50), new (1.52, 51), new (1.53, 52), new (1.54, 53), new (1.55, 54), new (1.56, 55), new (1.57, 56), new (1.58, 57), new (1.59, 58), new (1.6, 59),
        new (1.61, 60), new (1.62, 61), new (1.63, 62), new (1.64, 63), new (1.65, 64), new (1.66, 65), new (1.67, 66), new (1.68, 67), new (1.69, 68), new (1.7, 69),
        new (1.71, 70), new (1.72, 71), new (1.73, 72), new (1.74, 73), new (1.75, 74), new (1.76, 75), new (1.77, 76), new (1.78, 77), new (1.79, 78), new (1.8, 79),
        new (1.81, 80), new (1.82, 81), new (1.83, 82), new (1.84, 83), new (1.85, 84), new (1.86, 85), new (1.87, 86), new (1.88, 87), new (1.89, 88), new (1.9, 89),
        new (1.91, 90), new (1.92, 91), new (1.93, 92), new (1.94, 93), new (1.95, 94), new (1.96, 95), new (1.97, 96), new (1.98, 97), new (1.99, 98), new (2, 99),

        new (2.02, 100), new (2.04, 101), new (2.06, 102), new (2.08, 103), new (2.1, 104), new (2.12, 105), new (2.14, 106), new (2.16, 107), new (2.18, 108), new (2.2, 109),
        new (2.22, 110), new (2.24, 111), new (2.26, 112), new (2.28, 113), new (2.3, 114), new (2.32, 115), new (2.34, 116), new (2.36, 117), new (2.38, 118), new (2.4, 119),
        new (2.42, 120), new (2.44, 121), new (2.46, 122), new (2.48, 123), new (2.5, 124), new (2.52, 125), new (2.54, 126), new (2.56, 127), new (2.58, 128), new (2.6, 129),
        new (2.62, 130), new (2.64, 131), new (2.66, 132), new (2.68, 133), new (2.7, 134), new (2.72, 135), new (2.74, 136), new (2.76, 137), new (2.78, 138), new (2.8, 139),
        new (2.82, 140), new (2.84, 141), new (2.86, 142), new (2.88, 143), new (2.9, 144), new (2.92, 145), new (2.94, 146), new (2.96, 147), new (2.98, 148), new (3, 149),

        new (3.05, 150), new (3.1, 151), new (3.15, 152), new (3.2, 153), new (3.25, 154), new (3.3, 155), new (3.35, 156), new (3.4, 157), new (3.45, 158), new (3.5, 159),
        new (3.55, 160), new (3.6, 161), new (3.65, 162), new (3.7, 163), new (3.75, 164), new (3.8, 165), new (3.85, 166), new (3.9, 167), new (3.95, 168), new (4, 169),

        new (4.1, 170), new (4.2, 171), new (4.3, 172), new (4.4, 173), new (4.5, 174), new (4.6, 175), new (4.7, 176), new (4.8, 177), new (4.9, 178), new (5, 179),
        new (5.1, 180), new (5.2, 181), new (5.3, 182), new (5.4, 183), new (5.5, 184), new (5.6, 185), new (5.7, 186), new (5.8, 187), new (5.9, 188), new (6, 189),

        new (6.2, 190), new (6.4, 191), new (6.6, 192), new (6.8, 193), new (7, 194), new (7.2, 195), new (7.4, 196), new (7.6, 197), new (7.8, 198), new (8, 199),
        new (8.2, 200), new (8.4, 201), new (8.6, 202), new (8.8, 203), new (9, 204), new (9.2, 205), new (9.4, 206), new (9.6, 207), new (9.8, 208), new (10, 209),

        new (10.5, 210), new (11, 211), new (11.5, 212), new (12, 213), new (12.5, 214), new (13, 215), new (13.5, 216), new (14, 217), new (14.5, 218), new (15, 219),
        new (15.5, 220), new (16, 221), new (16.5, 222), new (17, 223), new (17.5, 224), new (18, 225), new (18.5, 226), new (19, 227), new (19.5, 228), new (20, 229),

        new (21, 230), new (22, 231), new (23, 232), new (24, 233), new (25, 234), new (26, 235), new (27, 236), new (28, 237), new (29, 238), new (30, 239), new (32, 240),
        new (34, 241), new (36, 242), new (38, 243), new (40, 244), new (42, 245), new (44, 246), new (46, 247), new (48, 248), new (50, 249),

        new (55, 250), new (60, 251), new (65, 252), new (70, 253), new (75, 254), new (80, 255), new (85, 256), new (90, 257), new (95, 258), new (100, 259),

        new (110, 260), new (120, 261), new (130, 262), new (140, 263), new (150, 264), new (160, 265), new (170, 266), new (180, 267), new (190, 268), new (200, 269),
        new (210, 270), new (220, 271), new (230, 272), new (240, 273), new (250, 274), new (260, 275), new (270, 276), new (280, 277), new (290, 278), new (300, 279),
        new (310, 280), new (320, 281), new (330, 282), new (340, 283), new (350, 284), new (360, 285), new (370, 286), new (380, 287), new (390, 288), new (400, 289),
        new (410, 290), new (420, 291), new (430, 292), new (440, 293), new (450, 294), new (460, 295), new (470, 296), new (480, 297), new (490, 298), new (500, 299),
        new (510, 300), new (520, 301), new (530, 302), new (540, 303), new (550, 304), new (560, 305), new (570, 306), new (580, 307), new (590, 308), new (600, 309),
        new (610, 310), new (620, 311), new (630, 312), new (640, 313), new (650, 314), new (660, 315), new (670, 316), new (680, 317), new (690, 318), new (700, 319),
        new (710, 320), new (720, 321), new (730, 322), new (740, 323), new (750, 324), new (760, 325), new (770, 326), new (780, 327), new (790, 328), new (800, 329),
        new (810, 330), new (820, 331), new (830, 332), new (840, 333), new (850, 334), new (860, 335), new (870, 336), new (880, 337), new (890, 338), new (900, 339),
        new (910, 340), new (920, 341), new (930, 342), new (940, 343), new (950, 344), new (960, 345), new (970, 346), new (980, 347), new (990, 348), new (1000, 349),
    };

    private static readonly Dictionary<int, Price> _prices = _validPrices.ToDictionary(x => (int)Math.Round(x.DecimalOdds * 100), y => y);

    private Price(double decimalOdds, int tick)
    {
        DecimalOdds = decimalOdds;
        Tick = tick;
    }

    public int Tick { get; }

    public double DecimalOdds { get; }

    public double Chance => 1 / DecimalOdds;

    public double MinimumStake => Math.Min(Math.Ceiling(10 / DecimalOdds * 100) / 100, 2);

    public static Price Of(double decimalOdds)
    {
        _prices.TryGetValue((int)Math.Round(decimalOdds * 100), out var price);
        if (price is not null) return price;

        throw new ArgumentException("Invalid Price");
    }

    public Price AddTicks(int ticks)
    {
        var newIndex = Tick + ticks;
        if (newIndex >= 350) return new Price(1000, 350);
        if (newIndex < 0) return new Price(1.01, 0);

        return _validPrices[newIndex];
    }

    public int TicksBetween(Price endPrice)
    {
        if (endPrice == null) return 0;

        return endPrice.Tick - Tick;
    }

    public override string ToString() => $"{DecimalOdds}";

    protected override IEnumerable<IComparable> GetEqualityComponents()
    {
        yield return Tick;
    }
}