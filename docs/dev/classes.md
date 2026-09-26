# Classes

```mermaid
classDiagram
    class Cell {
        +CellType type
        +Cell(CellType)
    }
    class CellType {
        <<ScriptableObject>>
        +string displayName
        +string description
        +bool hasNucleus
        +Species species
    }
    class Species {
        <<ScriptableObject>>
        +string genusName
        +string speciesName
        +string description
        +string infoUri
        +bool isMulticellular
        +Kingdom kingdom
        +string displayName
    }
    class Kingdom {
        <<ScriptableObject>>
        +string scientificName
        +string commonName
        +string description
        +string infoUri
        +string displayName
    }
    Cell --> CellType : type
    CellType --> Species : species
    Species --> Kingdom : kingdom
```
