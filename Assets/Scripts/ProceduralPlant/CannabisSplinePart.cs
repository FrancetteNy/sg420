using System;
using UnityEngine;
[Serializable]
public class CannabisSplinePart
{
    public Vector3 SplinePointA;
    public Vector3 SplinePointB;
    public GameObject parentObj;
    public GameObject leafPrefab;
    private GameObject leaf;
    public int numberOfLeafes;
    private PlantGenerator plantGenerator;


    public CannabisSplinePart(Vector3 a, Vector3 b, GameObject parentObj, GameObject leafObj, int numberOfLeafes) {
        SplinePointA = a;
        SplinePointB = b;
        this.numberOfLeafes = numberOfLeafes;
        this.parentObj = parentObj;
        this.leafPrefab = leafObj;

    }
}

