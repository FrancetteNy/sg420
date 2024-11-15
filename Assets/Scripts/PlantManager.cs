using UnityEngine;

public class PlantManager : MonoBehaviour
{
    public GameObject[] Plants;
    private HighlightController highlightController;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        highlightController = FindAnyObjectByType<HighlightController>();
        foreach (GameObject p in Plants) { 
            highlightController.AddHighlightObject(p);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
