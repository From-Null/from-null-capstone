using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class App : MonoBehaviour
{
    // [SerializeField] makes a private field show up in the Inspector.
    [SerializeField] VisualTreeAsset homeScreen;
    [SerializeField] VisualTreeAsset cellViewer;

    // Which types of cell the plant and animal cell buttons go to
    [SerializeField] CellType plantCellType;
    [SerializeField] CellType animalCellType;

    VisualElement root;

    void OnEnable()
    {
        root = GetComponent<UIDocument>().rootVisualElement;
        ShowHome();
    }

    void Show(VisualTreeAsset screen)
    {
        root.Clear();
        root.styleSheets.Clear();
        screen.CloneTree(root);
    }

    void ShowHome()
    {
        Show(homeScreen);

        root.Q<Button>("plant-cells").clicked += () => ShowCell(new Cell(plantCellType));
        root.Q<Button>("animal-cells").clicked += () => ShowCell(new Cell(animalCellType));
    }

    void ShowCell(Cell cell)
    {
        Show(cellViewer);

        var kingdom = cell.type.species.kingdom;

        root.Q<Label>("species").text = cell.type.species.displayName;
        root.Q<Label>("cell-name").text = cell.type.displayName;
        root.Q<Label>("kingdom").text = "Kingdom " + kingdom.displayName;

        root.Q<Label>("info-title").text = kingdom.commonName;
        root.Q<Label>("info-text").text = kingdom.description;

        root.Q<Button>("back").clicked += ShowHome;
    }
}
