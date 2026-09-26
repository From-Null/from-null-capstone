using UnityEngine;

[CreateAssetMenu(menuName = "AR Science/Species")]
public class Species : ScriptableObject
{
    public string genusName;
    public string speciesName;
    [TextArea] public string description;
    public string infoUri;

    public string displayName => genusName + " " + speciesName;
}
