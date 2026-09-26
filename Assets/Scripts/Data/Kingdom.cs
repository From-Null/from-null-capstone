using UnityEngine;

[CreateAssetMenu(menuName = "AR Science/Kingdom")]
public class Kingdom : ScriptableObject
{
    [Header("Names")]
    [Tooltip("The scientific name, e.g., Plantae")]
    public string scientificName;
    [Tooltip("The common name, e.g., Plants")]
    public string commonName;

    [Header("Properties")]
    [Tooltip("Extended description")]
    [TextArea] public string description;
    [Tooltip("Default link for more information, such as a Wikipedia article")]
    public string infoUri;

    // e.g., "Plantae (Plants)"
    public string displayName => scientificName + " (" + commonName + ")";
}
