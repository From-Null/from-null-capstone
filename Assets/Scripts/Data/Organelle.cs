using UnityEngine;
using System;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "AR Science/Organelle")]
public class Organelle : ScriptableObject
{
    [Tooltip("E.g., \"Nucleus\" or \"Cytoplasm\"")]
    public string displayName;
    [Tooltip("Extended description")]
    [TextArea] public string description;
}
