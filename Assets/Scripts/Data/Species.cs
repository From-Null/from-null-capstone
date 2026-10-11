using UnityEngine;

[CreateAssetMenu(menuName = "AR Science/Species")]
public class Species : ScriptableObject
{
    [Header("Names")]
    [Tooltip("The common name, e.g., Human")]
    public string commonName;
    [Tooltip("The genus name, e.g., Homo")]
    public string genusName;
    [Tooltip("The species name, e.g., sapiens")]
    public string speciesName;

    [Header("Properties")]
    [Tooltip("Extended description")]
    [TextArea] public string description;
    [Tooltip("Default link for more information, such as a Wikipedia article")]
    public string infoUri;

    [Tooltip("Is the species multicellular?")]
    public bool isMulticellular;
    [Tooltip("The kingdom the species belongs to")]
    public Kingdom kingdom;

    // e.g., "Homo sapiens"
    public string displayName => genusName + " " + speciesName + " (" + commonName + ")";
}
