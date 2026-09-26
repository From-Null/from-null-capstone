using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class App : MonoBehaviour
{
    // [SerializeField] makes a private field show up in the Inspector.
    [SerializeField] VisualTreeAsset homeScreen;
    [SerializeField] VisualTreeAsset cellViewer;

    // Which cells the plant and animal cell buttons go to
    [SerializeField] Cell plantCell;
    [SerializeField] Cell animalCell;

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

        root.Q<Button>("plant-cells").clicked += () => ShowCell(plantCell);
        root.Q<Button>("animal-cells").clicked += () => ShowCell(animalCell);
    }

    void ShowCell(Cell cell)
    {
        Show(cellViewer);

        root.Q<Label>("species").text = cell.species.displayName;
        root.Q<Label>("cell-name").text = cell.displayName;
        root.Q<Label>("kingdom").text = "Kingdom " + cell.type.kingdom;

        root.Q<Label>("info-title").text = cell.type.displayName + " cell";
        root.Q<Label>("info-text").text = cell.type.description;

        root.Q<Button>("back").clicked += ShowHome;
    }
}
