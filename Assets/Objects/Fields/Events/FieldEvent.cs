using System;
using System.Collections;

public abstract class FieldEvent
{
    protected int Index { get; }

    public FieldEvent(int index)
    {
        this.Index = index;
    }

    public abstract IEnumerator Execute(PieceController pieceController, PlayerUIController playerUiController, FieldEventDataSet[] fieldEventDataSets);

    protected T GetDataSet<T>(FieldEventDataSet[] fieldEventDataSets, FieldEventType type) where T : class
    {
        if (fieldEventDataSets[Index].type != type)
            throw new Exception("Incorrect FieldEventType");

        switch (type)
        {
            case FieldEventType.CoinGivingEvent:
                return fieldEventDataSets[Index].CoinGivingEventDataSet as T;
            case FieldEventType.VendorFieldEvent:
                return fieldEventDataSets[Index].VendorFieldEventDataSet as T;
            default:
                throw new Exception("Incorrect FieldEventType");
        }
    }
}
