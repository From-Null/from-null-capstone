using UnityEngine;

[CreateAssetMenu(menuName = "AR Science/Cell Type")]
public class CellType : ScriptableObject
{
    // e.g., "Plant" or "Animal"
    public string displayName;
    [TextArea] public string description;
    public string kingdom;
    public bool hasNucleus;
    public bool isMulticellular;
}
