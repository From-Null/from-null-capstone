using UnityEngine;
using System;
using System.Collections.Generic;

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
    public Guid cellTypeId;
    public bool isMulticellular;
    public Kingdom kingdom;
    
    //get all cells of this type, placeholder
    public List<Cell> getCells(CellType type){
        return new List<Cell>();
    }
    
    //get all cell types, placeholder
    public List<CellType> getCellTypes(CellType type){
        return new List<CellType>();
    }
}
