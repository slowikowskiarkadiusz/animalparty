public struct PlayerRegistration
{
    public string Name { get; private set; }
    public PieceType PieceType { get; private set; }

    public PlayerRegistration(string name, PieceType pieceType)
    {
        Name = name;
        PieceType = pieceType;
    }
}

public enum PieceType
{
    Default,
}