using UnityEngine;

[CreateAssetMenu(menuName = "AR Science/Cell")]
public class Cell : ScriptableObject
{
    // e.g., "Leaf cell" or "Cheek cell"
    public string displayName;
    public CellType type;
    public Species species;
}
