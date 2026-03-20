using UnityEngine;

[CreateAssetMenu(fileName = "UnitData", menuName = "Scriptable Objects/UnitData")]
public class UnitData : ScriptableObject
{
    [Header("Unit Info")]
    //public int UnitOwner;
    public string UnitName;

    [Header("Unit Stats")]
    public int AttackPower;
    public int MaxMovement;
    public int Price;

    [Header("Unit Visual")]
    public GameObject PrefabModel;
}
