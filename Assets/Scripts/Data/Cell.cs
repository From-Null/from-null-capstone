using System;
using System.Collections.Generic;
// Cell holds temporary state data, such as a selected organelle
// or a response to an environmental stimulus.
// It's not saved.
public class Cell
{
    // The type of cell, e.g., a human cheek cell
    public CellType type;
    public Guid cellId;
    public Guid modelId;
    public string name;
    //public Organelle[] organelles;
    public Species species;

    public Cell(CellType type)
    {
        this.type = type;
        this.cellId = cellId;
        this.modelId = modelId;
        this.name = name;
        this.species = species;
    }
    
    //get all cells, placeholder
    public List<Cell> getCells(){
        return new List<Cell>();
    }
}
