using UnityEngine;

public enum UnitType
{
    Scout = 0,
    Soldier = 1,
    Tank = 2
}

[CreateAssetMenu(fileName = "UnitData", menuName = "Scriptable Objects/UnitData")]
public class UnitData : ScriptableObject
{
    [Header("Unit Info")]
    public int UnitOwner;
    public string UnitName;
    public UnitType UnitType;

    [Header("Unit Stats")]
    public int AttackPower;
    public int MaxMovement;
    public int Price;

    [Header("Unit Visual")]
    public GameObject PrefabModel;
}
