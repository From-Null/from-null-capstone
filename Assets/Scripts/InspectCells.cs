using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CellInspect : MonoBehaviour{

    OrganelleLookup organelles;

    Camera cellCamera;
    VisualElement cellView;
    Label infoTitle;
    Label infoText;

    public void Initialize(Camera camera, VisualElement view, Label title, Label text, OrganelleLookup lookup){

        cellCamera = camera;
        cellView = view;
        infoTitle = title;
        infoText = text;
        organelles = lookup;

        // makes each organelle selectable
        foreach (var mesh in GetComponentsInChildren<MeshFilter>()){

            Transform part = mesh.transform;
            bool found = false;

            while (part != null && part != transform){

                if (organelles.TryGet(part.name, out _)){
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

                if (organelles.TryGet(part.name, out var organelle)){
                    infoTitle.text = organelle.displayName;
                    infoText.text = organelle.description;
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
