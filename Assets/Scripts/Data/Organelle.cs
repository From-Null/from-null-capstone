using System;
using System.Collections.Generic;

public class Organelle{
    public string description;
    public Guid modelId;
    public Guid organelleId;
    public string name;
    
    public Organelle(string name){
        this.description = description;
        this.modelId = modelId;
        this.organelleId = organelleId;
        this.name = name;
    }
    
    //get all organelles, placeholder
    public List<Organelle> getOrganelles(){
        return new List<Organelle>();
    }
}
