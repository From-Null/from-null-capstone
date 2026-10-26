using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CellLabels : MonoBehaviour
{
    // organelle names students will see
    readonly Dictionary<string, string> partNames = new()
    {
        { "cell_wall", "Cell Wall" },
        { "lipid_bilayer", "Cell Membrane" },
        { "cytoskeleton", "Cytoskeleton" },
        { "central_vacuole_", "Central Vacuole" },
        { "floating_ribosomes", "Ribosomes" },
        { "chloroplast", "Chloroplast" },
        { "chloroplast_2", "Chloroplast" },
        { "rough_smooth_er", "Endoplasmic Reticulum" },
        { "Nucleolus", "Nucleolus" },
        { "Mitochondria", "Mitochondria" },
        { "Nucleus", "Nucleus" },
        { "golgi", "Golgi Apparatus" }
    };

    readonly List<(Transform part, Vector3 center, Label label)> labels = new();

    Camera cellCamera;
    VisualElement cellView;
    Button labelsButton;
    bool labelsVisible;

    public void Initialize(Camera camera, VisualElement view, Button button)
    {
        // gets the camera, viewer, and button from App.cs
        cellCamera = camera;
        cellView = view;
        labelsButton = button;

        foreach (var part in GetComponentsInChildren<Transform>())
        {
            if (!partNames.TryGetValue(part.name, out var displayName))
                continue;

            // finds the 3D shapes that make up each organelle
            var renderers = part.GetComponentsInChildren<Renderer>();
            if (renderers.Length == 0)
                continue;

            var bounds = renderers[0].bounds;
            foreach (var renderer in renderers)
                bounds.Encapsulate(renderer.bounds);

            var center = part.InverseTransformPoint(bounds.center);

            // creates the organelle's label
            var label = new Label(displayName);
            label.AddToClassList("organelle-label");

            label.pickingMode = PickingMode.Ignore;

            // keeps labels hidden until the student clicks button
            label.style.display = DisplayStyle.None;

            cellView.Add(label);
            labels.Add((part, center, label));
        }
        labelsButton.SetEnabled(labels.Count > 0);
        labelsButton.clicked += ToggleLabels;
    }

    void ToggleLabels()
    {
        labelsVisible = !labelsVisible;
        UpdateLabels();
    }

    void LateUpdate()
    {
        UpdateLabels();
    }

    void UpdateLabels()
    {
        if (cellView == null || cellCamera == null)
            return;

        float width = cellView.contentRect.width;
        float height = cellView.contentRect.height;

        if (width <= 0 || height <= 0)
            return;

        var texture = cellCamera.targetTexture;
        if (texture == null)
            return;

        float aspect = (float)texture.width / texture.height;

        float imageWidth = Mathf.Min(width, height * aspect);
        float imageHeight = imageWidth / aspect;
        float offsetX = (width - imageWidth) / 2f;
        float offsetY = (height - imageHeight) / 2f;

        foreach (var entry in labels)
        {
            var worldPosition = entry.part.TransformPoint(entry.center);
            var point = cellCamera.WorldToViewportPoint(worldPosition);

            bool visible = labelsVisible
                && point.z > 0
                && point.x >= 0 && point.x <= 1
                && point.y >= 0 && point.y <= 1;

            entry.label.style.display = visible
                ? DisplayStyle.Flex
                : DisplayStyle.None;

            if (!visible)
                continue;

            entry.label.style.left = offsetX + point.x * imageWidth;
            entry.label.style.top = offsetY + (1f - point.y) * imageHeight;
        }
    }

    void OnDestroy()
    {
        if (labelsButton != null)
            labelsButton.clicked -= ToggleLabels;

        foreach (var entry in labels)
            entry.label.RemoveFromHierarchy();
    }
}
