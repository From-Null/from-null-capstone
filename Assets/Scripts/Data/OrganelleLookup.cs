using System;
using System.Collections.Generic;
using UnityEngine;

// Finds the organelle for an object in a cell type's 3D model
// We match by a case-insensitive comparison with the organelle
// name. If the object name has a period, we drop it and everything
// after it. Blender names duplicate objects with names like
// "lysosome.002", so we treat that the same as just "lysosome".
public class OrganelleLookup
{
    // Model object name -> Organelle
    readonly Dictionary<string, Organelle> organelles = new(StringComparer.OrdinalIgnoreCase);

    public OrganelleLookup(CellType cellType)
    {
        // Organelles match by exact name like "Nucleus"
        foreach (var organelle in Resources.LoadAll<Organelle>("Organelles"))
        {
            organelles[organelle.name] = organelle;
            organelles[organelle.displayName] = organelle;
        }

        // Model-specific organelle mappings
        // For example, "golgi" to "Golgi apparatus"
        foreach (var part in cellType.parts)
        {
            if (part.organelle != null)
                organelles[part.objectName] = part.organelle;
        }
    }

    // Match object names ignoring case and dropping suffixes
    // like ".002". For example, "lysosome.002" matches "Lysosome".
    public bool TryGet(string objectName, out Organelle organelle)
    {
        return organelles.TryGetValue(objectName.Split('.')[0], out organelle);
    }
}
