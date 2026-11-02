using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CellInspect : MonoBehaviour{

    // organelle descriptions from references document
    readonly Dictionary<string, (string name, string description)> partInfo = new(){

        { "cell_wall", ("Cell Wall", "Strong outer layer that protects and supports the cell.") },
        { "lipid_bilayer", ("Cell Membrane", "Thin boundary that separates the cell from its surroundings. It also controls what enters and leaves the cell.") },
        { "cytoskeleton", ("Cytoskeleton", "Network of fibers and tubes that help shape and support the cell.") },
        { "central_vacuole_", ("Vacuole", "Stores water and other materials.") },
        { "floating_ribosomes", ("Ribosomes", "Make proteins which cells need to grow.") },
        { "chloroplast", ("Chloroplast", "Uses sunlight to help make food for the plant through photosynthesis.") },
        { "chloroplast_2", ("Chloroplast", "Uses sunlight to help make food for the plant through photosynthesis.") },
        { "rough_smooth_er", ("Endoplasmic Reticulum",
            "Rough Endoplasmic Reticulum (ER): Network of membranes that helps make and move proteins.\n" +
            "Smooth Endoplasmic Reticulum (ER): Network of membranes that helps make the fats that the cell needs.") },
        { "Nucleolus", ("Nucleolus", "A small area inside the nucleus that helps make ribosomes.") },
        { "Mitochondria", ("Mitochondria", "Turn energy from food into a form the cell can use. They are referred to as the powerhouses of the cell.") },
        { "Nucleus", ("Nucleus", "Holds DNA, which contains the instructions that help the cell grow and function. It is referred to as the cell’s control center.") },
        { "golgi", ("Golgi Apparatus", "Sorts and packages proteins from the Endoplasmic Reticulum to send them where they need to go.") }
    };

    Camera cellCamera;
    VisualElement cellView;
    Label infoTitle;
    Label infoText;

    public void Initialize(Camera camera, VisualElement view, Label title, Label text){

        cellCamera = camera;
        cellView = view;
        infoTitle = title;
        infoText = text;

        // makes each organelle selectable
        foreach (var mesh in GetComponentsInChildren<MeshFilter>()){

            Transform part = mesh.transform;
            bool found = false;

            while (part != null && part != transform){

                if (partInfo.ContainsKey(part.name)){
                    found = true;
                    break;
                }
                part = part.parent;
            }
            if (!found)
                continue;
            if (mesh.GetComponent<Collider>() == null){
                var collider = mesh.gameObject.AddComponent<MeshCollider>();
                collider.sharedMesh = mesh.sharedMesh;
            }
        }

        cellView.RegisterCallback<PointerDownEvent>(Inspect);
    }

    void Inspect(PointerDownEvent evt){

        if (cellCamera == null || !cellCamera.enabled)
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

        float x = (evt.localPosition.x - offsetX) / imageWidth;
        float y = 1f - (evt.localPosition.y - offsetY) / imageHeight;

        if (x < 0 || x > 1 || y < 0 || y > 1)
            return;

        var ray = cellCamera.ViewportPointToRay(new Vector3(x, y, 0));
        var hits = Physics.RaycastAll(ray);

        System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

        foreach (var hit in hits){
            var part = hit.transform;
            while (part != null && part != transform){

                if (partInfo.TryGetValue(part.name, out var info)){
                    infoTitle.text = info.name;
                    infoText.text = info.description;
                    return;
                }
                part = part.parent;
            }
        }
    }

    void OnDestroy(){
        if (cellView != null)
            cellView.UnregisterCallback<PointerDownEvent>(Inspect);
    }
}
