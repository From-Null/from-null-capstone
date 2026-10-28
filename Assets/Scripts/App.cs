using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.UIElements;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

[RequireComponent(typeof(UIDocument))]
public class App : MonoBehaviour
{
    // [SerializeField] makes a private field show up in the Inspector.
    [SerializeField] VisualTreeAsset homeScreen;
    [SerializeField] VisualTreeAsset cellViewer;
    [SerializeField] VisualTreeAsset settingsScreen;
    [SerializeField] VisualTreeAsset tutorialScreen;

    // Which types of cell the plant and animal cell buttons go to
    [SerializeField] CellType plantCellType;
    [SerializeField] CellType animalCellType;

    // Camera for the cell models
    [SerializeField] Camera cellCamera;
    [SerializeField] Transform modelRoot;

    //loaded on top of theme.uss when dark mode is activated
    [SerializeField] StyleSheet darkThemeSheet;

    // Radius of the sphere that models are scaled to fit in
    const float ModelRadius = 0.22f;

    // Zoom
    const float DefaultZoom = 1f;
    const float MinZoom = 0.25f;
    const float MaxZoom = 4f;
    const float ZoomStep = 0.25f;
    const float PinchZoomSpeed = 0.003f;

    VisualElement root;
    RenderTexture cellTexture;
    float currentZoom = DefaultZoom;
    bool isDarkMode = false;

    void OnEnable()
    {
        EnhancedTouchSupport.Enable();
        root = GetComponent<UIDocument>().rootVisualElement;
        ShowHome();
    }

    void Update()
    {
        if (!cellCamera.enabled || Touch.activeTouches.Count != 2) return;

        var t0 = Touch.activeTouches[0];
        var t1 = Touch.activeTouches[1];

        float prevDist = Vector2.Distance(t0.screenPosition - t0.delta, t1.screenPosition - t1.delta);
        float currDist = Vector2.Distance(t0.screenPosition, t1.screenPosition);

        SetZoom(currentZoom + (currDist - prevDist) * PinchZoomSpeed);
    }

    void Show(VisualTreeAsset screen)
    {
        HideModel();
        root.Clear();
        root.styleSheets.Clear();
        screen.CloneTree(root);
        //re apply dark mode so it stays
        ApplyTheme();
    }

    //applies theme
    void ApplyTheme(){
        if(isDarkMode)
            root.styleSheets.Add(darkThemeSheet);
    }

    //toggles between light and dark mode
    void ToggleDarkMode(){
        isDarkMode = !isDarkMode;
        if(isDarkMode)
            root.styleSheets.Add(darkThemeSheet);
        else
            root.styleSheets.Remove(darkThemeSheet);
    }

    void ShowHome()
    {
        Show(homeScreen);

        root.Q<Button>("plant-cells").clicked += () => ShowCell(new Cell(plantCellType));
        root.Q<Button>("animal-cells").clicked += () => ShowCell(new Cell(animalCellType));
        root.Q<Button>("settings").clicked += () => ShowSettings();
        root.Q<Button>("tutorial").clicked += ShowTutorial;
    }

    void ShowTutorial()
    {
        Show(tutorialScreen);
        root.Q<Button>("home").clicked += ShowHome;
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

        root.Q<Button>("zoom-in").clicked += () => SetZoom(currentZoom + ZoomStep);
        root.Q<Button>("zoom-out").clicked += () => SetZoom(currentZoom - ZoomStep);
        root.Q<Button>("reset").clicked += () => SetZoom(DefaultZoom);
        root.Q<Button>("back").clicked += ShowHome;

        ShowModel(cell.type.model);
    }

    void SetZoom(float zoom)
    {
        currentZoom = Mathf.Clamp(zoom, MinZoom, MaxZoom);
        modelRoot.localScale = Vector3.one * currentZoom;
    }

    void ShowModel(GameObject model)
    {
        if (model == null) return;

        FitModel(Instantiate(model, modelRoot).transform);

        if (cellTexture == null)
        {
            cellTexture = new RenderTexture(1024, 1024, 24);
        }
        cellCamera.targetTexture = cellTexture;
        cellCamera.enabled = true;
        root.Q("cell-view").style.backgroundImage = Background.FromRenderTexture(cellTexture);
    }

    // Scale the model to fit in a sphere
    void FitModel(Transform model)
    {
        var renderers = model.GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0) return;

        var bounds = renderers[0].bounds;
        foreach (var r in renderers) bounds.Encapsulate(r.bounds);

        // Fit the model's largest extent
        var e = bounds.extents;
        var scale = ModelRadius / Mathf.Max(e.x, e.y, e.z);
        model.localScale *= scale;
        var center = model.position + (bounds.center - model.position) * scale;
        model.position += modelRoot.position - center;
    }

    void HideModel()
    {
        cellCamera.enabled = false;
        foreach (Transform child in modelRoot)
        {
            Destroy(child.gameObject);
        }
    }

    void OnDestroy()
    {
        Destroy(cellTexture);
    }

    void ShowSettings()
    {
        Show(settingsScreen);

        //set toggle label to current theme
        var themeToggle = root.Q<Button>("theme-toggle");
        themeToggle.text = isDarkMode ? "Light Mode" : "Dark Mode";

        root.Q<Button>("back").clicked += ShowHome;
        root.Q<Button>("theme-toggle").clicked += () =>{
            ToggleDarkMode();
            //update label
            themeToggle.text = isDarkMode ? "Light Mode" : "Dark Mode";
        };
    }
}
