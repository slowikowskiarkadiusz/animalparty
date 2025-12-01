using System;

public abstract record Card
{
    public string Name { get; }
    public string Description { get; }
    public CardType Type { get; }
    public CardQuality Quality { get; }

    public Card(string name, string description, CardType type, CardQuality quality)
    {
        Name = name;
        Description = description;
        Type = type;
        Quality = quality;
    }
}

public record BirdCard : Card
{
    public int? Turns;
    public bool RequriesTarget;

    protected BirdCard(Card original) : base(original) { }

    public BirdCard(string name, string description, CardType type, CardQuality quality, bool requriesTarget, int? turns = null) : base(name, description, type, quality)
    {
        RequriesTarget = requriesTarget;
        Turns = turns;
    }

    public static BirdCard Magpie => new("Magpie", "Get coins equal to the sum of other players' coins", CardType.Bird, CardQuality.Common, true);
}

public record TotemCard : Card
{
    protected TotemCard(Card original) : base(original) { }

    public TotemCard(string name, string description, CardType type, CardQuality quality) : base(name, description, type, quality) { }

    public static TotemCard PerpetualVelocity => new("Totem of Perpetual Velocity", "Doubles dice rolls for each player", CardType.Totem, CardQuality.Common);
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