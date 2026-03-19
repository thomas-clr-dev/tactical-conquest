using UnityEngine;

[System.Serializable]
public class UnitSelectionData
{
    public UnitType UnitType;
    public int TotalAvailable;
    public int SelectedToMove;
    public int RemainingOnTile;

    public UnitSelectionData(UnitType type, int total)
    {
        UnitType = type;
        TotalAvailable = total;
        SelectedToMove = 0;
        RemainingOnTile = total;
    }

    public bool CanSelectMore()
    {
        return RemainingOnTile > 1;
    }

    public void IncrementSelection()
    {
        if (CanSelectMore())
        {
            SelectedToMove++;
            RemainingOnTile--;
        }
    }

    public void DecrementSelection()
    {
        if (CanSelectMore())
        {
            SelectedToMove--;
            RemainingOnTile++;
        }
    }
}
