using UnityEngine;

[CreateAssetMenu(menuName = "AR Science/Species")]
public class Species : ScriptableObject
{
    public string genusName;
    public string speciesName;
    [TextArea] public string description;
    public string infoUri;
    public bool isMulticellular;
    public Kingdom kingdom;

    public string displayName => genusName + " " + speciesName;
}
