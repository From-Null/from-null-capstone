using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneNavigation : MonoBehaviour
{
    public void OpenARViewer()
    {
        SceneManager.LoadScene("ARViewer");
    }

    public void OpenMainMenu()
    {
        SceneManager.LoadScene("Main");
    }
}
