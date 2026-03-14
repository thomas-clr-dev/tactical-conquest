using UnityEngine;

[CreateAssetMenu(fileName = "UnitData", menuName = "Scriptable Objects/UnitData")]
public class UnitData : ScriptableObject
{
    public string UnitName;
    public int AttackPower;
    public int MaxMovement;
    public int Price;
    public GameObject PrefabModel;
}
