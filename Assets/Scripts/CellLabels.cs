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

    readonly List<(Transform part, Vector3 center, Label label, VisualElement line)> labels = new();

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

            var line = new VisualElement();
            line.AddToClassList("organelle-line");
            line.pickingMode = PickingMode.Ignore;
            line.style.display = DisplayStyle.None;
            
            // creates the organelle's label
            var label = new Label(displayName);
            label.AddToClassList("organelle-label");
            label.pickingMode = PickingMode.Ignore;

            // keeps labels hidden until the student clicks button
            label.style.display = DisplayStyle.None;
            cellView.Add(line);
            cellView.Add(label);
            labels.Add((part, center, label, line));

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

        bool zoomedIn = transform.parent != null &&
            transform.parent.localScale.x > 1.01f;

        var leftLabels = new List<(Label label, VisualElement line, Vector2 point)>();
        var rightLabels = new List<(Label label, VisualElement line, Vector2 point)>();

        // keeps zoomed-in labels from overlapping
        var usedPositions = new List<Rect>();

        foreach (var entry in labels)
        {
            entry.label.style.display = DisplayStyle.None;
            entry.line.style.display = DisplayStyle.None;

            if (!labelsVisible)
                continue;

            var worldPosition = entry.part.TransformPoint(entry.center);
            var point = cellCamera.WorldToViewportPoint(worldPosition);

            if (point.z <= 0 || point.x < 0 || point.x > 1 ||
                point.y < 0 || point.y > 1)
                continue;

            float x = offsetX + point.x * imageWidth;
            float y = offsetY + (1f - point.y) * imageHeight;

            if (zoomedIn)
            {
                entry.label.style.display = DisplayStyle.Flex;

                float labelWidth = entry.label.resolvedStyle.width;
                float labelHeight = entry.label.resolvedStyle.height;

                if (!(labelWidth > 0))
                    labelWidth = entry.label.text.Length * 20f + 24f;

                if (!(labelHeight > 0))
                    labelHeight = 52f;

                // finds an open spot near the organelle
                Vector2[] positions =
                {
                    new Vector2(0, 0),
                    new Vector2(-1, 0),
                    new Vector2(1, 0),
                    new Vector2(0, -1),
                    new Vector2(0, 1),
                    new Vector2(-1, -1),
                    new Vector2(1, -1),
                    new Vector2(-1, 1),
                    new Vector2(1, 1)
                };

                bool placed = false;

                for (int distance = 0; distance <= 5 && !placed; distance++)
                {
                    foreach (var position in positions)
                    {
                        float newX = x + position.x * distance * (labelWidth / 2f + 8f);
                        float newY = y + position.y * distance * (labelHeight / 2f + 8f);

                        var area = new Rect(
                            newX - labelWidth / 2f,
                            newY - labelHeight / 2f,
                            labelWidth,
                            labelHeight
                        );

                        if (area.xMin < 0 || area.xMax > width ||
                            area.yMin < 0 || area.yMax > height)
                            continue;

                        bool overlaps = false;

                        foreach (var used in usedPositions)
                        {
                            if (area.Overlaps(used))
                            {
                                overlaps = true;
                                break;
                            }
                        }

                        if (overlaps)
                            continue;

                        entry.label.style.left = newX;
                        entry.label.style.top = newY;

                        usedPositions.Add(area);
                        placed = true;
                        break;
                    }
                }

                if (!placed)
                    entry.label.style.display = DisplayStyle.None;

                continue;
            }

            var position2D = new Vector2(x, y);

            if (x < width / 2f)
                leftLabels.Add((entry.label, entry.line, position2D));
            else
                rightLabels.Add((entry.label, entry.line, position2D));
        }

        if (!zoomedIn)
        {
            PlaceLabels(leftLabels, width, height, true);
            PlaceLabels(rightLabels, width, height, false);
        }
    }

    void PlaceLabels(List<(Label label, VisualElement line, Vector2 point)> entries, float width, float height, bool left)
    {
        if (entries.Count == 0)
            return;

        // keeps labels in order from top to bottom
        entries.Sort((a, b) => a.point.y.CompareTo(b.point.y));

        float columnWidth = width * 0.32f;
        float labelX = left ? columnWidth / 2f : width - columnWidth / 2f;

        float rowHeight = 60f;
        float totalHeight = entries.Count * rowHeight;

        float middle = 0f;

        foreach (var entry in entries)
            middle += entry.point.y;

        middle /= entries.Count;

        float startY = Mathf.Clamp(
            middle - totalHeight / 2f,
            rowHeight / 2f,
            Mathf.Max(rowHeight / 2f, height - totalHeight)
        );

        for (int i = 0; i < entries.Count; i++)
        {
            var entry = entries[i];

            float labelY = startY + i * rowHeight + rowHeight / 2f;

            entry.label.style.display = DisplayStyle.Flex;
            entry.label.style.left = labelX;
            entry.label.style.top = labelY;

            float labelWidth = entry.label.resolvedStyle.width;

            if (!(labelWidth > 0))
                labelWidth = entry.label.text.Length * 16f + 24f;

            // starts the line at the edge of the label
            float startX = left
                ? labelX + labelWidth / 2f
                : labelX - labelWidth / 2f;

            UpdateLine(entry.line, startX, labelY, entry.point.x, entry.point.y);
        }
    }

    // draws a line from the label to its organelle
    void UpdateLine(VisualElement line, float startX, float startY, float endX, float endY)
    {
        float dx = endX - startX;
        float dy = endY - startY;
        float length = Mathf.Sqrt(dx * dx + dy * dy);

        if (length < 5f)
        {
            line.style.display = DisplayStyle.None;
            return;
        }

        float angle = Mathf.Atan2(dy, dx) * Mathf.Rad2Deg;

        line.style.display = DisplayStyle.Flex;
        line.style.left = startX;
        line.style.top = startY - 1.5f;
        line.style.width = length;
        line.style.height = 3f;
        line.style.rotate = new Rotate(new Angle(angle, AngleUnit.Degree));

        // rotates the line around its starting point
        line.style.transformOrigin = new TransformOrigin(
            Length.Percent(0),
            Length.Percent(50),
            0
        );
    }

    void OnDestroy()
    {
        if (labelsButton != null)
            labelsButton.clicked -= ToggleLabels;

        foreach (var entry in labels){
            entry.label.RemoveFromHierarchy();
            entry.line.RemoveFromHierarchy();
        }
    }
}
