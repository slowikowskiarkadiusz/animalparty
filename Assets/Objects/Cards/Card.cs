public struct Card
{
    public string Name { get; }
    public string Description { get; }
    public CardType Type { get; }
    public CardQuality Quality { get; }

    public Card(string name, string description, CardType type, CardQuality quality) : this()
    {
        Name = name;
        Description = description;
        Type = type;
        Quality = quality;
    }

    public static Card TotemPerpetualVelocity = new("Totem of Perpetual Velocity", "Doubles dice rolls for each player", CardType.Totem, CardQuality.Common);
    public static Card BirdMagpie = new("Magpie", "Get coins equal to the sum of other players' coins", CardType.Bird, CardQuality.Common);
}

public enum CardType
{
    Totem,
    Bird,
}

public enum CardQuality
{
    Common,
    Rare,
    Legendary,
}