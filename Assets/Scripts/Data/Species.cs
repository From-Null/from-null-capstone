using UnityEngine;
using System;
using System.Collections.Generic;

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
    public Guid speciesId;

    // e.g., "Homo sapiens (Human)"
    public string displayName => genusName + " " + speciesName + " (" + commonName + ")";
    
    //get all cells of this species, placeholder
    public List<Cell> getCells(Species species){
        return new List<Cell>();
    }
    
    //get all cells of this type, placeholder
    public List<Species> getSpecies(Species species){
        return new List<Species>();
    }
}
