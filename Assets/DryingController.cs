using UnityEngine;

public class DryingController : MonoBehaviour
{
    DriedPlantData _driedPlantData;
    public DriedPlantData DriedPlantData { 
        get {
            return this._driedPlantData; 
        } 
        set {
            this._driedPlantData = value;
            this.PlantObject.SetActive(value != null && value.DryingAge != null && value.DryingAge.Stage != DryingStage.Empty);
        } 
    }
    public GameObject PlantObject;

}
