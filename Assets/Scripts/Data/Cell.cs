// Cell holds temporary state data, such as a selected organelle
// or a response to an environmental stimulus.
// It's not saved.
public class Cell
{
    // The type of cell, e.g., a human cheek cell
    public CellType type;

    public Cell(CellType type)
    {
        this.type = type;
    }
}
