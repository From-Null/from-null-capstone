using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class ARViewerUI : MonoBehaviour
{
    [SerializeField] private SceneNavigation sceneNavigation;

    private void OnEnable()
    {
        VisualElement root =
            GetComponent<UIDocument>().rootVisualElement;

        Button returnButton = root.Q<Button>("return-to-menu");

        returnButton.clicked += sceneNavigation.OpenMainMenu;
    }

    private void OnDisable()
    {
        VisualElement root =
            GetComponent<UIDocument>().rootVisualElement;

        Button returnButton = root.Q<Button>("return-to-menu");

        returnButton.clicked -= sceneNavigation.OpenMainMenu;
    }
}
