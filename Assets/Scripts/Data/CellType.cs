using UnityEngine;

[CreateAssetMenu(menuName = "AR Science/Cell Type")]
public class CellType : ScriptableObject
{
    [Tooltip("E.g., \"Waterweed leaf cell\" or \"Human cheek cell\"")]
    public string displayName;
    [Tooltip("Extended description")]
    [TextArea] public string description;
    [Tooltip("Whether the cell has a nucleus or not")]
    public bool hasNucleus;
    [Tooltip("The species the cell belongs to")]
    public Species species;
}
