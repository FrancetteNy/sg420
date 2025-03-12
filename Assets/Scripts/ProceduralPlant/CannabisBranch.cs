using System;
using UnityEngine;
using System.Collections.Generic;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

[Serializable]
public class CannabisBranch
{
    public List<CannabisSplinePart> splineParts = new List<CannabisSplinePart>();
    public List<Vector3> catmullRomSplinePoints = new List<Vector3>();
    public int id;
    public GameObject leaf;
    public Vector3 branchDirection = Vector3.up;
    public float yDirection = 0f;
    public int numberOfLeafes;
    public float scale;
    public bool isOnTop;
    public Material branchMaterial;
    public Material leafMaterial;

    
    

    public CannabisBranch(List<CannabisSplinePart> splineParts, List<Vector3> catmullRomSplinePoints, int id) {
        this.splineParts = splineParts;
        this.catmullRomSplinePoints = catmullRomSplinePoints;
        this.id = id;
    }

    public CannabisBranch(int id) {
        this.id = id;
    }

    public CannabisBranch(int id, Vector3 branchDirection) {
        this.id = id;
        this.branchDirection = branchDirection;
    }

    public CannabisBranch(CannabisBranchData data)
    {
        catmullRomSplinePoints = data.SplinePoints;
        branchDirection = data.BranchDirection;
        yDirection = data.YDirection;
    }
    
    public void InstantiateLeaf(GameObject[] leafPrefabs, int numberOfLeafes, float scale, bool isOnTop, Transform transform)
    {
        this.numberOfLeafes = numberOfLeafes;
        this.scale = scale;
        this.isOnTop = isOnTop;
        
        int counterLeftRotation = 1;
        int counterRightRotation = 1;
        int scaleCounter = 1;
        
        Quaternion leafRotation = Quaternion.LookRotation(branchDirection);
        //leaf = Object.Instantiate(leafPrefab, splineParts[splineParts.Count - 1].SplinePointB, leafRotation);
        

        leaf = new GameObject("Leaf");
        leaf.transform.parent = transform;
        leaf.transform.position = splineParts[splineParts.Count - 1].SplinePointB;
        leaf.transform.rotation = leafRotation;
        if (isOnTop == true) {
            leaf.transform.rotation *= Quaternion.Euler(90f, 90f, 90f);
        }
        
        //leaf.name = "Leaf";
        leaf.transform.localScale = new Vector3(0.2f * scale, 0.2f * scale, 0.2f * scale);

        for (int i = 0; i < numberOfLeafes; i++) {
            
            GameObject leaflet = Object.Instantiate(leafPrefabs[0]);
            leaflet.transform.SetParent(leaf.transform);
            leaflet.transform.localPosition = Vector3.zero;
            leaflet.transform.localRotation = Quaternion.identity;
            leaflet.transform.localScale = Vector3.one * 10;
            //leaflet.transform.localRotation = Quaternion.Euler(0f,45f,0f);
           
           //Leaflet 0: Mitte
           
           //Leaflet 1,3,5 Links
           if ((i+1) % 2 == 0) {
               leaflet.transform.localRotation = Quaternion.Euler(10f,-(25f * counterLeftRotation),0f);
               leaflet.transform.localScale = Vector3.one * (10 - scaleCounter);
               counterLeftRotation++;
           }
           //Leaflet 2,4,6 Rechts
           if ((i+1) % 2 != 0) {
               leaflet.transform.localRotation = Quaternion.Euler(10f,25f * counterRightRotation,0f);
               leaflet.transform.localScale = Vector3.one * (10 - scaleCounter);
               counterRightRotation++;
               scaleCounter++;
           }

           if (i == 0) {
               leaflet.transform.localRotation = Quaternion.Euler(10f,0f,0f);
               leaflet.transform.localScale = Vector3.one * 10;
               counterRightRotation = 1; 
               counterLeftRotation = 1;
               scaleCounter = 1;
           }
        }
        
        




    }
    
    private Quaternion GetRotation(float xLowerBoundary, float xUpperBoundary, float yLowerBoundary, float yUpperBoundary, float zLowerBoundary, float zUpperBoundary) {
        float yRotation = Random.Range(yLowerBoundary, yUpperBoundary);
        float xRotation = Random.Range(xLowerBoundary, xUpperBoundary);
        float zRotation = Random.Range(zLowerBoundary, zUpperBoundary);
        Quaternion rotation = Quaternion.Euler(xRotation, yRotation, zRotation);
        return rotation;
    }


    public void AddSplinePartsToBranch(CannabisSplinePart splinePart) {
        splineParts.Add(splinePart);
        
    }

    public void RemoveSplinePartsToBranch() {
        
    }

    public void AddCatmullRomSplinePoints(List<Vector3> catmullRomSplinePoints) {
        this.catmullRomSplinePoints = catmullRomSplinePoints;
    }

    public void RemoveCatmullRomSplinePoints() {
        
    }

    public void DeleteAllCatmullRomSplinePoints() {
        catmullRomSplinePoints.Clear();
    }
}
