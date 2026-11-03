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

    // Radius of the sphere that models are scaled to fit in
    const float ModelRadius = 0.22f;

    // Zoom
    const float DefaultZoom = 1f;
    const float MinZoom = 0.25f;
    const float MaxZoom = 4f;
    const float ZoomStep = 0.25f;
    const float PinchZoomSpeed = 0.003f;

    // Degrees rotated per pixel moved in the gesture
    const float DragRotationSpeed = 0.25f;

    VisualElement root;
    RenderTexture cellTexture;
    float currentZoom = DefaultZoom;

    void OnEnable()
    {
        EnhancedTouchSupport.Enable();
#if UNITY_EDITOR
        // Enable simulating touch controls with mouse
        TouchSimulation.Enable();
 #endif
        root = GetComponent<UIDocument>().rootVisualElement;
        ShowHome();
    }

    void Update()
    {
        if (!cellCamera.enabled) return;

        var touches = Touch.activeTouches;
        if (touches.Count == 1) DragRotateGesture(touches[0].delta);
        else if (touches.Count == 2) PinchZoomGesture(touches[0], touches[1]);
    }

    // Moving one finger to rotate the model
    void DragRotateGesture(Vector2 delta)
    {
        var xform = cellCamera.transform;
        modelRoot.Rotate(xform.up, -1 * delta.x * DragRotationSpeed, Space.World);
        modelRoot.Rotate(xform.right, delta.y * DragRotationSpeed, Space.World);
    }

    // Two fingers pinched or spread to zoom in or out
    void PinchZoomGesture(Touch t0, Touch t1)
    {
        var prev = (t1.screenPosition - t1.delta) - (t0.screenPosition - t0.delta);
        var curr = t1.screenPosition - t0.screenPosition;

        SetZoom(currentZoom + (curr.magnitude - prev.magnitude) * PinchZoomSpeed);
    }

    // Reset to default zoom and positioning
    void ResetView()
    {
        SetZoom(DefaultZoom);
        modelRoot.localRotation = Quaternion.identity;
    }

    void Show(VisualTreeAsset screen)
    {
        HideModel();
        root.Clear();
        root.styleSheets.Clear();
        screen.CloneTree(root);
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
        root.Q<Button>("reset").clicked += ResetView;
        root.Q<Button>("back").clicked += ShowHome;

        // connects the labels to the cell model and "Labels" button
        var model = ShowModel(cell.type.model);
        if (model != null)
        {
            var cellLabels = model.AddComponent<CellLabels>();
            cellLabels.Initialize(cellCamera, root.Q<VisualElement>("cell-view"), root.Q<Button>("toggle-labels"));

            var cellInspect = model.AddComponent<CellInspect>();
            cellInspect.Initialize(cellCamera, root.Q<VisualElement>("cell-view"), root.Q<Label>("info-title"), root.Q<Label>("info-text"));
        }
    }

    void SetZoom(float zoom)
    {
        currentZoom = Mathf.Clamp(zoom, MinZoom, MaxZoom);
        modelRoot.localScale = Vector3.one * currentZoom;
    }

    GameObject ShowModel(GameObject model)
    {
        if (model == null) return null;

        var instance = Instantiate(model, modelRoot);
        FitModel(instance.transform);

        if (cellTexture == null)
        {
            cellTexture = new RenderTexture(1024, 1024, 24);
        }
        cellCamera.targetTexture = cellTexture;
        cellCamera.enabled = true;
        root.Q("cell-view").style.backgroundImage = Background.FromRenderTexture(cellTexture);

        ResetView();

        return instance;
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

        root.Q<Button>("back").clicked += ShowHome;
    }

}
