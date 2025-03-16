using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class PlantGenerator : MonoBehaviour
{
    private CatmullRomSpline _catmullRomSplineReference = new CatmullRomSpline();
    private List<CannabisBranch> _cannabisBranches = new List<CannabisBranch>();
    [SerializeField] private MeshGenerator meshGenerator;
    
    
    
    [SerializeField] private AnimationCurve _radiusSize;
    [SerializeField] private Material[] _materials;
    [SerializeField] private float RADIUS = 0.1f;
    [SerializeField] private int CIRCLE_SEGMENTS = 20;
    [SerializeField] private int SPLINE_SEGMENTS = 20;
    [SerializeField] private GameObject[] leafPrefabs;
    

    
    [SerializeField] private Age.GrowthStage phase;
    [SerializeField] private int ageNumber;
    
    private PlantController _plantController;
    private int _plantLayer;
    
        
        
    //testing variables
    private Vector3 _initialPosition;
    private float _length = 0.009f ;
    private float _radius = 0.001f;

    private void Awake()
    {
        _plantLayer = LayerMask.NameToLayer("ProceduralPlant");

    }
    private void Start()
    {
        _plantController = GetComponentInParent<PlantController>();
        _initialPosition = transform.position;
        RebuildCannabisPlant();
    }
    
    
    
    public void GenerateCannabisPlant() {
        phase = _plantController.PlantData.Age.Stage;
        ageNumber = _plantController.PlantData.Age.AgeNumber;
        switch (phase) {
            case Age.GrowthStage.GERMINATION:
                if (ageNumber == 1) {
                    CannabisBranch stem1 = new CannabisBranch(0);
                    for (int i = 0; i <= 5; i++) {
                        Vector3 newSplinePointB = (_initialPosition + Vector3.up * _length) +
                                                  Random.insideUnitSphere * _length / 8;
                        CannabisSplinePart splinePart =
                            new CannabisSplinePart(_initialPosition, newSplinePointB, null, null, 0);
                        _initialPosition = splinePart.SplinePointB;
                        stem1.AddSplinePartsToBranch(splinePart);
                    }
                    stem1.AddCatmullRomSplinePoints(
                        _catmullRomSplineReference.CalculateCatmullRomSplinePoints(stem1.splineParts, 20));
                    _cannabisBranches.Add(stem1);
                    SpawnLeafAtEndOfBranch(leafPrefabs, 1, _cannabisBranches[0], 1, true);
                    DeleteAllLeafes();
                    ExtendBranch(_cannabisBranches[0], Vector3.up, 4);
                    SpawnLeafAtEndOfBranch(leafPrefabs, 1, _cannabisBranches[0], 1, true);
                    
                }



                break;
            case Age.GrowthStage.SEEDLING:
                _radius = 0.0015f;
                float xDirection = 0f;
                float yDirection = 0f;
                float randomOposite = 0f;
                
                if (ageNumber == 0) {
                    DeleteAllLeafes();
                    
                    ExtendBranch(_cannabisBranches[0], Vector3.up, 4);
                    SpawnLeafAtEndOfBranch(leafPrefabs, 3, _cannabisBranches[0], 1, true);
                    
                    xDirection = Random.Range(28f, 40f);
                    yDirection = Random.Range(0f, 360f);
                    GenerateSubBranch(_cannabisBranches[0], 0.1f, xDirection, yDirection, 6);
                    SpawnLeafAtEndOfBranch(leafPrefabs, 1, _cannabisBranches[1], 1.2f, false);
                    
                    xDirection = Random.Range(28f, 40f);
                    randomOposite = Random.Range(yDirection + 160f, yDirection + 200f);
                    GenerateSubBranch(_cannabisBranches[0], 0.1f, xDirection, randomOposite, 4);
                    SpawnLeafAtEndOfBranch(leafPrefabs, 3, _cannabisBranches[2], 1.2f, false);
                    
                    xDirection = Random.Range(28f, 40f);
                    yDirection = Random.Range(0f, 360f);
                    GenerateSubBranch(_cannabisBranches[0], 0.8f, xDirection, yDirection, 4);
                    _cannabisBranches[3].yDirection = yDirection;
                    SpawnLeafAtEndOfBranch(leafPrefabs, 1, _cannabisBranches[3], 1.2f, false);
                }

                if (ageNumber == 1) {

                    DeleteAllLeafes();
                    
                    ExtendBranch(_cannabisBranches[0], Vector3.up, 4);
                    SpawnLeafAtEndOfBranch(leafPrefabs, 5, _cannabisBranches[0], 1f, true);
                    
                    ExtendBranch(_cannabisBranches[1], _cannabisBranches[1].branchDirection, 4);
                    SpawnLeafAtEndOfBranch(leafPrefabs, 3, _cannabisBranches[1], 1.4f, false);
                    
                    ExtendBranch(_cannabisBranches[2], _cannabisBranches[2].branchDirection, 6);
                    SpawnLeafAtEndOfBranch(leafPrefabs, 5, _cannabisBranches[2], 1.4f, false);
                    
                    ExtendBranch(_cannabisBranches[3], _cannabisBranches[3].branchDirection, 4);
                    SpawnLeafAtEndOfBranch(leafPrefabs, 3, _cannabisBranches[3], 1.4f, false);
                    
                    xDirection = Random.Range(28f, 40f);
                    yDirection = _cannabisBranches[3].yDirection;
                    randomOposite = Random.Range(yDirection + 160f, yDirection + 200f);
                    GenerateSubBranch(_cannabisBranches[0], 0.6f, xDirection, randomOposite, 6);
                    SpawnLeafAtEndOfBranch(leafPrefabs, 1, _cannabisBranches[4], 1.2f, false);
                }
                
                if (ageNumber == 2) {
                    DeleteAllLeafes();
                    
                    ExtendBranch(_cannabisBranches[0], Vector3.up, 2);
                    SpawnLeafAtEndOfBranch(leafPrefabs, 7, _cannabisBranches[0], 1f, true);

                    ExtendBranch(_cannabisBranches[1], _cannabisBranches[1].branchDirection, 2);
                    SpawnLeafAtEndOfBranch(leafPrefabs, 5, _cannabisBranches[1], 1.5f, false);

                    ExtendBranch(_cannabisBranches[2], _cannabisBranches[2].branchDirection, 2);
                    SpawnLeafAtEndOfBranch(leafPrefabs, 7, _cannabisBranches[2], 1.4f, false);

                    ExtendBranch(_cannabisBranches[3], _cannabisBranches[3].branchDirection, 2);
                    SpawnLeafAtEndOfBranch(leafPrefabs, 5, _cannabisBranches[3], 1.4f, false);

                    ExtendBranch(_cannabisBranches[4], _cannabisBranches[4].branchDirection, 3);
                    SpawnLeafAtEndOfBranch(leafPrefabs, 3, _cannabisBranches[4], 1.4f, false);
                    
                    xDirection = Random.Range(28f, 40f);
                    GenerateSubBranch(_cannabisBranches[0], 0.8f, xDirection, randomOposite, 2);
                    SpawnLeafAtEndOfBranch(leafPrefabs, 1, _cannabisBranches[5], 1.1f, false);
                }

                if (ageNumber == 3) {
                    DeleteAllLeafes();
                    
                    ExtendBranch(_cannabisBranches[0], Vector3.up, 2);
                    SpawnLeafAtEndOfBranch(leafPrefabs, 7, _cannabisBranches[0], 1f, true);

                    ExtendBranch(_cannabisBranches[1], _cannabisBranches[1].branchDirection, 2);
                    SpawnLeafAtEndOfBranch(leafPrefabs, 5, _cannabisBranches[1], 1.7f, false);

                    ExtendBranch(_cannabisBranches[2], _cannabisBranches[2].branchDirection, 2);
                    SpawnLeafAtEndOfBranch(leafPrefabs, 7, _cannabisBranches[2], 1.7f, false);

                    ExtendBranch(_cannabisBranches[3], _cannabisBranches[3].branchDirection, 2);
                    SpawnLeafAtEndOfBranch(leafPrefabs, 5, _cannabisBranches[3], 1.5f, false);

                    ExtendBranch(_cannabisBranches[4], _cannabisBranches[4].branchDirection, 3);
                    SpawnLeafAtEndOfBranch(leafPrefabs, 3, _cannabisBranches[4], 1.5f, false);

                    ExtendBranch(_cannabisBranches[5], _cannabisBranches[5].branchDirection, 2);
                    SpawnLeafAtEndOfBranch(leafPrefabs, 3, _cannabisBranches[5], 1.5f, false);
                }


                break;
            case Age.GrowthStage.VEGETATIVEGROWTH:
                _radius = 0.002f;
                
                if (ageNumber == 0) {
                    DeleteAllLeafes();
                        
                    ExtendBranch(_cannabisBranches[0], Vector3.up, 8);
                    SpawnLeafAtEndOfBranch(leafPrefabs, 7, _cannabisBranches[0], 1f, true);
                    
                    ExtendBranch(_cannabisBranches[1], _cannabisBranches[1].branchDirection, 4);
                    SpawnLeafAtEndOfBranch(leafPrefabs, 7, _cannabisBranches[1], 2.3f, false);
                
                    ExtendBranch(_cannabisBranches[2], _cannabisBranches[2].branchDirection, 4);
                    SpawnLeafAtEndOfBranch(leafPrefabs, 7, _cannabisBranches[2], 2.3f, false);
                    
                    ExtendBranch(_cannabisBranches[3], _cannabisBranches[3].branchDirection, 4);
                    SpawnLeafAtEndOfBranch(leafPrefabs, 7, _cannabisBranches[3], 2.3f, false);
                
                    ExtendBranch(_cannabisBranches[4], _cannabisBranches[4].branchDirection, 4);
                    SpawnLeafAtEndOfBranch(leafPrefabs, 5, _cannabisBranches[4], 2.3f, false);
                
                    ExtendBranch(_cannabisBranches[5], _cannabisBranches[5].branchDirection, 4);
                    SpawnLeafAtEndOfBranch(leafPrefabs, 3, _cannabisBranches[5], 2.3f, false);
                    
                    xDirection = Random.Range(28f, 40f);
                    yDirection = _cannabisBranches[5].yDirection;
                    randomOposite = Random.Range(yDirection + 160f, yDirection + 200f);
                    GenerateSubBranch(_cannabisBranches[0], 0.57f, xDirection, randomOposite, 4);
                    SpawnLeafAtEndOfBranch(leafPrefabs, 1, _cannabisBranches[6], 1.7f, false);
                    
                    xDirection = Random.Range(28f, 40f);
                    yDirection = Random.Range(0f, 360f);
                    GenerateSubBranch(_cannabisBranches[0], 0.7f, xDirection, yDirection, 4);
                    SpawnLeafAtEndOfBranch(leafPrefabs, 3, _cannabisBranches[7], 1.7f, false);
                    
                    xDirection = Random.Range(28f, 40f);
                    randomOposite = Random.Range(yDirection + 160f, yDirection + 200f);
                    GenerateSubBranch(_cannabisBranches[0], 0.7f, xDirection, randomOposite, 2);
                    SpawnLeafAtEndOfBranch(leafPrefabs, 1, _cannabisBranches[8], 1.7f, false);
                    
                    xDirection = Random.Range(28f, 40f);
                    yDirection = Random.Range(0f, 360f);
                    GenerateSubBranch(_cannabisBranches[0], 0.9f, xDirection, yDirection, 2);
                    SpawnLeafAtEndOfBranch(leafPrefabs, 3, _cannabisBranches[9], 1.7f, false);
                }

                if (ageNumber == 1) {
                    DeleteAllLeafes();
                        
                    ExtendBranch(_cannabisBranches[0], Vector3.up, 2);
                    SpawnLeafAtEndOfBranch(leafPrefabs, 7, _cannabisBranches[0], 1f, true);
                    
                    ExtendBranch(_cannabisBranches[1], _cannabisBranches[1].branchDirection, 2);
                    SpawnLeafAtEndOfBranch(leafPrefabs, 7, _cannabisBranches[1], 2.3f, false);
                
                    ExtendBranch(_cannabisBranches[2], _cannabisBranches[2].branchDirection, 2);
                    SpawnLeafAtEndOfBranch(leafPrefabs, 7, _cannabisBranches[2], 2.3f, false);
                    
                    ExtendBranch(_cannabisBranches[3], _cannabisBranches[3].branchDirection, 2);
                    SpawnLeafAtEndOfBranch(leafPrefabs, 7, _cannabisBranches[3], 2.3f, false);
                
                    ExtendBranch(_cannabisBranches[4], _cannabisBranches[4].branchDirection, 2);
                    SpawnLeafAtEndOfBranch(leafPrefabs, 7, _cannabisBranches[4], 2.3f, false);
                
                    ExtendBranch(_cannabisBranches[5], _cannabisBranches[5].branchDirection, 2);
                    SpawnLeafAtEndOfBranch(leafPrefabs, 5, _cannabisBranches[5], 2.3f, false);
                    
                    ExtendBranch(_cannabisBranches[6], _cannabisBranches[6].branchDirection, 2);
                    SpawnLeafAtEndOfBranch(leafPrefabs, 3, _cannabisBranches[6], 2.3f, false);
                    
                    ExtendBranch(_cannabisBranches[7], _cannabisBranches[7].branchDirection, 2);
                    SpawnLeafAtEndOfBranch(leafPrefabs, 5, _cannabisBranches[7], 2.3f, false);
                    
                    ExtendBranch(_cannabisBranches[8], _cannabisBranches[8].branchDirection, 2);
                    SpawnLeafAtEndOfBranch(leafPrefabs, 3, _cannabisBranches[8], 2.3f, false);
                    
                    ExtendBranch(_cannabisBranches[9], _cannabisBranches[9].branchDirection, 2);
                    SpawnLeafAtEndOfBranch(leafPrefabs, 5, _cannabisBranches[9], 2.3f, false);
                    
                    xDirection = Random.Range(28f, 40f);
                    yDirection = _cannabisBranches[9].yDirection;
                    randomOposite = Random.Range(yDirection + 160f, yDirection + 200f);
                    GenerateSubBranch(_cannabisBranches[0], 0.7f, xDirection, randomOposite, 2);
                    SpawnLeafAtEndOfBranch(leafPrefabs, 1, _cannabisBranches[10], 1.7f, false);
                }
                    
                if (ageNumber == 2) {
                    DeleteAllLeafes();
                        
                    ExtendBranch(_cannabisBranches[0], Vector3.up, 3);
                    SpawnLeafAtEndOfBranch(leafPrefabs, 7, _cannabisBranches[0], 1f, true);
                    
                    ExtendBranch(_cannabisBranches[1], _cannabisBranches[1].branchDirection, 1);
                    SpawnLeafAtEndOfBranch(leafPrefabs, 7, _cannabisBranches[1], 2.3f, false);
                
                    ExtendBranch(_cannabisBranches[2], _cannabisBranches[2].branchDirection, 1);
                    SpawnLeafAtEndOfBranch(leafPrefabs, 7, _cannabisBranches[2], 2.3f, false);
                    
                    ExtendBranch(_cannabisBranches[3], _cannabisBranches[3].branchDirection, 1);
                    SpawnLeafAtEndOfBranch(leafPrefabs, 7, _cannabisBranches[3], 2.3f, false);
                
                    ExtendBranch(_cannabisBranches[4], _cannabisBranches[4].branchDirection, 1);
                    SpawnLeafAtEndOfBranch(leafPrefabs, 7, _cannabisBranches[4], 2.3f, false);
                
                    ExtendBranch(_cannabisBranches[5], _cannabisBranches[5].branchDirection, 1);
                    SpawnLeafAtEndOfBranch(leafPrefabs, 5, _cannabisBranches[5], 2.3f, false);
                    
                    ExtendBranch(_cannabisBranches[6], _cannabisBranches[6].branchDirection, 1);
                    SpawnLeafAtEndOfBranch(leafPrefabs, 3, _cannabisBranches[6], 2.3f, false);
                    
                    ExtendBranch(_cannabisBranches[7], _cannabisBranches[7].branchDirection, 1);
                    SpawnLeafAtEndOfBranch(leafPrefabs, 5, _cannabisBranches[7], 2.3f, false);
                    
                    ExtendBranch(_cannabisBranches[8], _cannabisBranches[8].branchDirection, 1);
                    SpawnLeafAtEndOfBranch(leafPrefabs, 3, _cannabisBranches[8], 2.3f, false);
                    
                    ExtendBranch(_cannabisBranches[9], _cannabisBranches[9].branchDirection, 1);
                    SpawnLeafAtEndOfBranch(leafPrefabs, 5, _cannabisBranches[9], 2.3f, false);
                    
                    ExtendBranch(_cannabisBranches[10], _cannabisBranches[10].branchDirection, 2);
                    SpawnLeafAtEndOfBranch(leafPrefabs, 3, _cannabisBranches[10], 2.3f, false);
                    
                    xDirection = Random.Range(28f, 40f);
                    yDirection = Random.Range(0f, 360f);
                    GenerateSubBranch(_cannabisBranches[0], 0.85f, xDirection, yDirection, 2);
                    SpawnLeafAtEndOfBranch(leafPrefabs, 3, _cannabisBranches[11], 1.7f, false);
                    
                    xDirection = Random.Range(28f, 40f);
                    randomOposite = Random.Range(yDirection + 160f, yDirection + 200f);
                    GenerateSubBranch(_cannabisBranches[0], 0.85f, xDirection, randomOposite, 2);
                    SpawnLeafAtEndOfBranch(leafPrefabs, 3, _cannabisBranches[12], 1.7f, false);
                }

                if (ageNumber == 3) {
                    DeleteAllLeafes();
                    
                    ExtendBranch(_cannabisBranches[0], Vector3.up, 3);
                    SpawnLeafAtEndOfBranch(leafPrefabs, 7, _cannabisBranches[0], 1f, true);
                    
                    ExtendBranch(_cannabisBranches[1], _cannabisBranches[1].branchDirection, 1);
                    SpawnLeafAtEndOfBranch(leafPrefabs, 7, _cannabisBranches[1], 2.3f, false);
                
                    ExtendBranch(_cannabisBranches[2], _cannabisBranches[2].branchDirection, 1);
                    SpawnLeafAtEndOfBranch(leafPrefabs, 7, _cannabisBranches[2], 2.3f, false);
                    
                    ExtendBranch(_cannabisBranches[3], _cannabisBranches[3].branchDirection, 1);
                    SpawnLeafAtEndOfBranch(leafPrefabs, 7, _cannabisBranches[3], 2.3f, false);
                
                    ExtendBranch(_cannabisBranches[4], _cannabisBranches[4].branchDirection, 1);
                    SpawnLeafAtEndOfBranch(leafPrefabs, 7, _cannabisBranches[4], 2.3f, false);
                
                    ExtendBranch(_cannabisBranches[5], _cannabisBranches[5].branchDirection, 1);
                    SpawnLeafAtEndOfBranch(leafPrefabs, 5, _cannabisBranches[5], 2.3f, false);
                    
                    ExtendBranch(_cannabisBranches[6], _cannabisBranches[6].branchDirection, 1);
                    SpawnLeafAtEndOfBranch(leafPrefabs, 3, _cannabisBranches[6], 2.3f, false);
                    
                    ExtendBranch(_cannabisBranches[7], _cannabisBranches[7].branchDirection, 1);
                    SpawnLeafAtEndOfBranch(leafPrefabs, 5, _cannabisBranches[7], 2.3f, false);
                    
                    ExtendBranch(_cannabisBranches[8], _cannabisBranches[8].branchDirection, 1);
                    SpawnLeafAtEndOfBranch(leafPrefabs, 3, _cannabisBranches[8], 2.3f, false);
                    
                    ExtendBranch(_cannabisBranches[9], _cannabisBranches[9].branchDirection, 1);
                    SpawnLeafAtEndOfBranch(leafPrefabs, 5, _cannabisBranches[9], 2.3f, false);
                    
                    ExtendBranch(_cannabisBranches[10], _cannabisBranches[10].branchDirection, 2);
                    SpawnLeafAtEndOfBranch(leafPrefabs, 3, _cannabisBranches[10], 2.3f, false);
                    
                    ExtendBranch(_cannabisBranches[11], _cannabisBranches[11].branchDirection, 2);
                    SpawnLeafAtEndOfBranch(leafPrefabs, 3, _cannabisBranches[11], 2.3f, false);
                    
                    ExtendBranch(_cannabisBranches[12], _cannabisBranches[12].branchDirection, 2);
                    SpawnLeafAtEndOfBranch(leafPrefabs, 3, _cannabisBranches[12], 2.3f, false);
                }
                
                if (ageNumber == 4) {
                    DeleteAllLeafes();
                    
                    ExtendBranch(_cannabisBranches[0], Vector3.up, 1);
                    SpawnLeafAtEndOfBranch(leafPrefabs, 7, _cannabisBranches[0], 1f, true);
                    
                    ExtendBranch(_cannabisBranches[1], _cannabisBranches[1].branchDirection, 1);
                    SpawnLeafAtEndOfBranch(leafPrefabs, 7, _cannabisBranches[1], 3f, false);
                
                    ExtendBranch(_cannabisBranches[2], _cannabisBranches[2].branchDirection, 1);
                    SpawnLeafAtEndOfBranch(leafPrefabs, 7, _cannabisBranches[2], 3f, false);
                    
                    SpawnLeafAtEndOfBranch(leafPrefabs, 7, _cannabisBranches[3], 3f, false);
                    
                    SpawnLeafAtEndOfBranch(leafPrefabs, 7, _cannabisBranches[4], 3f, false);
                    
                    SpawnLeafAtEndOfBranch(leafPrefabs, 7, _cannabisBranches[5], 2.3f, false);
                    
                    
                    ExtendBranch(_cannabisBranches[6], _cannabisBranches[6].branchDirection, 1);
                    SpawnLeafAtEndOfBranch(leafPrefabs, 5, _cannabisBranches[6], 2.3f, false);
                    
                    ExtendBranch(_cannabisBranches[7], _cannabisBranches[7].branchDirection, 1);
                    SpawnLeafAtEndOfBranch(leafPrefabs, 7, _cannabisBranches[7], 2.3f, false);
                    
                    ExtendBranch(_cannabisBranches[8], _cannabisBranches[8].branchDirection, 1);
                    SpawnLeafAtEndOfBranch(leafPrefabs, 5, _cannabisBranches[8], 2.3f, false);
                    
                    ExtendBranch(_cannabisBranches[9], _cannabisBranches[9].branchDirection, 1);
                    SpawnLeafAtEndOfBranch(leafPrefabs, 7, _cannabisBranches[9], 2.3f, false);
                    
                    ExtendBranch(_cannabisBranches[10], _cannabisBranches[10].branchDirection, 1);
                    SpawnLeafAtEndOfBranch(leafPrefabs, 5, _cannabisBranches[10], 2.3f, false);
                    
                    ExtendBranch(_cannabisBranches[11], _cannabisBranches[11].branchDirection, 1);
                    SpawnLeafAtEndOfBranch(leafPrefabs, 7, _cannabisBranches[11], 2.3f, false);
                    
                    ExtendBranch(_cannabisBranches[12], _cannabisBranches[12].branchDirection, 1);
                    SpawnLeafAtEndOfBranch(leafPrefabs, 5, _cannabisBranches[12], 2.3f, false);
                }
                
                if (ageNumber == 5) {
                    DeleteAllLeafes();
                    
                    ExtendBranch(_cannabisBranches[0], Vector3.up, 1);
                    SpawnLeafAtEndOfBranch(leafPrefabs, 7, _cannabisBranches[0], 1f, true);
                    
                    ExtendBranch(_cannabisBranches[1], _cannabisBranches[1].branchDirection, 1);
                    SpawnLeafAtEndOfBranch(leafPrefabs, 7, _cannabisBranches[1], 3f, false);
                    
                    ExtendBranch(_cannabisBranches[2], _cannabisBranches[2].branchDirection, 1);
                    SpawnLeafAtEndOfBranch(leafPrefabs, 7, _cannabisBranches[2], 3f, false);
                    
                    SpawnLeafAtEndOfBranch(leafPrefabs, 7, _cannabisBranches[3], 3f, false);
                    
                    SpawnLeafAtEndOfBranch(leafPrefabs, 7, _cannabisBranches[4], 3f, false);
                    
                    SpawnLeafAtEndOfBranch(leafPrefabs, 7, _cannabisBranches[5], 2.3f, false);
                    
                    ExtendBranch(_cannabisBranches[6], _cannabisBranches[6].branchDirection, 1);
                    SpawnLeafAtEndOfBranch(leafPrefabs, 5, _cannabisBranches[6], 2.3f, false);
                    
                    ExtendBranch(_cannabisBranches[7], _cannabisBranches[7].branchDirection, 1);
                    SpawnLeafAtEndOfBranch(leafPrefabs, 7, _cannabisBranches[7], 2.3f, false);
                    
                    ExtendBranch(_cannabisBranches[8], _cannabisBranches[8].branchDirection, 1);
                    SpawnLeafAtEndOfBranch(leafPrefabs, 5, _cannabisBranches[8], 2.3f, false);
                    
                    ExtendBranch(_cannabisBranches[9], _cannabisBranches[9].branchDirection, 1);
                    SpawnLeafAtEndOfBranch(leafPrefabs, 7, _cannabisBranches[9], 2.3f, false);
                    
                    ExtendBranch(_cannabisBranches[10], _cannabisBranches[10].branchDirection, 1);
                    SpawnLeafAtEndOfBranch(leafPrefabs, 5, _cannabisBranches[10], 2.3f, false);
                    
                    ExtendBranch(_cannabisBranches[11], _cannabisBranches[11].branchDirection, 1);
                    SpawnLeafAtEndOfBranch(leafPrefabs, 7, _cannabisBranches[11], 2.3f, false);
                    
                    ExtendBranch(_cannabisBranches[12], _cannabisBranches[12].branchDirection, 1);
                    SpawnLeafAtEndOfBranch(leafPrefabs, 5, _cannabisBranches[12], 2.3f, false);
                    
                    xDirection = Random.Range(28f, 40f);
                    yDirection = Random.Range(0f, 360f);
                    GenerateSubBranch(_cannabisBranches[0], 0.9f, xDirection, yDirection, 2);
                    SpawnLeafAtEndOfBranch(leafPrefabs, 1, _cannabisBranches[13], 1.7f, false);
                }
                
                if (ageNumber == 6) {
                    DeleteAllLeafes();
                    
                    ExtendBranch(_cannabisBranches[0], Vector3.up, 2);
                    SpawnLeafAtEndOfBranch(leafPrefabs, 7, _cannabisBranches[0], 1f, true);
                    
                    ExtendBranch(_cannabisBranches[1], _cannabisBranches[1].branchDirection, 1);
                    SpawnLeafAtEndOfBranch(leafPrefabs, 7, _cannabisBranches[1], 3f, false);
                    
                    ExtendBranch(_cannabisBranches[2], _cannabisBranches[2].branchDirection, 1);
                    SpawnLeafAtEndOfBranch(leafPrefabs, 7, _cannabisBranches[2], 3f, false);
                    
                    ExtendBranch(_cannabisBranches[3], _cannabisBranches[3].branchDirection, 1);
                    SpawnLeafAtEndOfBranch(leafPrefabs, 7, _cannabisBranches[3], 3f, false);
                
                    ExtendBranch(_cannabisBranches[4], _cannabisBranches[4].branchDirection, 1);
                    SpawnLeafAtEndOfBranch(leafPrefabs, 7, _cannabisBranches[4], 3f, false);
                
                    ExtendBranch(_cannabisBranches[5], _cannabisBranches[5].branchDirection, 1);
                    SpawnLeafAtEndOfBranch(leafPrefabs, 7, _cannabisBranches[5], 2.3f, false);
                    
                    ExtendBranch(_cannabisBranches[6], _cannabisBranches[6].branchDirection, 1);
                    SpawnLeafAtEndOfBranch(leafPrefabs, 5, _cannabisBranches[6], 2.3f, false);
                    
                    ExtendBranch(_cannabisBranches[7], _cannabisBranches[7].branchDirection, 1);
                    SpawnLeafAtEndOfBranch(leafPrefabs, 7, _cannabisBranches[7], 2.3f, false);
                    
                    ExtendBranch(_cannabisBranches[8], _cannabisBranches[8].branchDirection, 2);
                    SpawnLeafAtEndOfBranch(leafPrefabs, 5, _cannabisBranches[8], 2.3f, false);
                    
                    ExtendBranch(_cannabisBranches[9], _cannabisBranches[9].branchDirection, 2);
                    SpawnLeafAtEndOfBranch(leafPrefabs, 7, _cannabisBranches[9], 2.3f, false);
                    
                    ExtendBranch(_cannabisBranches[10], _cannabisBranches[10].branchDirection, 2);
                    SpawnLeafAtEndOfBranch(leafPrefabs, 5, _cannabisBranches[10], 2.3f, false);
                    
                    ExtendBranch(_cannabisBranches[11], _cannabisBranches[11].branchDirection, 2);
                    SpawnLeafAtEndOfBranch(leafPrefabs, 7, _cannabisBranches[11], 2.3f, false);
                    
                    ExtendBranch(_cannabisBranches[12], _cannabisBranches[12].branchDirection, 2);
                    SpawnLeafAtEndOfBranch(leafPrefabs, 5, _cannabisBranches[12], 2.3f, false);
                    
                    ExtendBranch(_cannabisBranches[13], _cannabisBranches[13].branchDirection, 2);
                    SpawnLeafAtEndOfBranch(leafPrefabs, 3, _cannabisBranches[13], 2.3f, false);
                }
                
                if (ageNumber == 7) {
                    DeleteAllLeafes();
                    
                    ExtendBranch(_cannabisBranches[0], Vector3.up, 2);
                    SpawnLeafAtEndOfBranch(leafPrefabs, 7, _cannabisBranches[0], 1f, true);
                    
                    ExtendBranch(_cannabisBranches[1], _cannabisBranches[1].branchDirection, 1);
                    SpawnLeafAtEndOfBranch(leafPrefabs, 7, _cannabisBranches[1], 3f, false);
                
                    ExtendBranch(_cannabisBranches[2], _cannabisBranches[2].branchDirection, 1);
                    SpawnLeafAtEndOfBranch(leafPrefabs, 7, _cannabisBranches[2], 3f, false);
                    
                    ExtendBranch(_cannabisBranches[3], _cannabisBranches[3].branchDirection, 1);
                    SpawnLeafAtEndOfBranch(leafPrefabs, 7, _cannabisBranches[3], 3f, false);
                
                    ExtendBranch(_cannabisBranches[4], _cannabisBranches[4].branchDirection, 1);
                    SpawnLeafAtEndOfBranch(leafPrefabs, 7, _cannabisBranches[4], 3f, false);
                
                    ExtendBranch(_cannabisBranches[5], _cannabisBranches[5].branchDirection, 1);
                    SpawnLeafAtEndOfBranch(leafPrefabs, 7, _cannabisBranches[5], 2.3f, false);
                    
                    ExtendBranch(_cannabisBranches[6], _cannabisBranches[6].branchDirection, 1);
                    SpawnLeafAtEndOfBranch(leafPrefabs, 5, _cannabisBranches[6], 2.3f, false);
                    
                    ExtendBranch(_cannabisBranches[7], _cannabisBranches[7].branchDirection, 1);
                    SpawnLeafAtEndOfBranch(leafPrefabs, 7, _cannabisBranches[7], 2.3f, false);
                    
                    ExtendBranch(_cannabisBranches[8], _cannabisBranches[8].branchDirection, 2);
                    SpawnLeafAtEndOfBranch(leafPrefabs, 5, _cannabisBranches[8], 2.3f, false);
                    
                    ExtendBranch(_cannabisBranches[9], _cannabisBranches[9].branchDirection, 2);
                    SpawnLeafAtEndOfBranch(leafPrefabs, 7, _cannabisBranches[9], 2.3f, false);
                    
                    ExtendBranch(_cannabisBranches[10], _cannabisBranches[10].branchDirection, 2);
                    SpawnLeafAtEndOfBranch(leafPrefabs, 5, _cannabisBranches[10], 2.3f, false);
                    
                    ExtendBranch(_cannabisBranches[11], _cannabisBranches[11].branchDirection, 2);
                    SpawnLeafAtEndOfBranch(leafPrefabs, 7, _cannabisBranches[11], 2.3f, false);
                    
                    ExtendBranch(_cannabisBranches[12], _cannabisBranches[12].branchDirection, 2);
                    SpawnLeafAtEndOfBranch(leafPrefabs, 5, _cannabisBranches[12], 2.3f, false);
                    
                    ExtendBranch(_cannabisBranches[13], _cannabisBranches[13].branchDirection, 2);
                    SpawnLeafAtEndOfBranch(leafPrefabs, 3, _cannabisBranches[13], 2.3f, false);
                }
                

               
                
                
                break;
            case Age.GrowthStage.FLOWERING:
                
                
                break;
            case Age.GrowthStage.EMPTY:
                RebuildCannabisPlant();
                break;
            default:
                break;
        }
        GenerateMesh(_materials[0]);
    }

    public void RebuildCannabisPlant()
    {
        if (_plantController.PlantData.CannabisBranches != null)
        {
            foreach (Transform child in transform)
            {
                if (child.name == "Branch")
                {
                    Destroy(child.gameObject);
                }
            }
            _cannabisBranches.Clear();
            DeleteAllLeafes();

            _cannabisBranches = _plantController.PlantData.CannabisBranches;

            foreach (var branch in _cannabisBranches) {
                SpawnLeafAtEndOfBranch(leafPrefabs, branch.numberOfLeafes, branch, branch.scale, branch.isOnTop);
            }

            
            
            GenerateMesh(_materials[0]);
        }
    }
    
    private void SetLeafColour(Material material)
    {
        DeleteAllLeafes();
        foreach (CannabisBranch branch in _cannabisBranches)
        {
            branch.InstantiateLeaf(leafPrefabs, branch.numberOfLeafes, branch.scale, branch.isOnTop, this.transform);
            List<GameObject> leaflets = branch.leaf.GetComponentsInChildren<Transform>()
                .Select(t => t.gameObject)
                .ToList();
            foreach (var gameObject in leaflets)
            {
                gameObject.layer = _plantLayer;
                if (gameObject.GetComponentInChildren<MeshRenderer>() != null)
                gameObject.GetComponentInChildren<MeshRenderer>().material.color = Color.blue;
            }
        }
    }

    private void ChangeLeafs(Material material)
    {
        DeleteAllLeafes();
        foreach (CannabisBranch branch in _cannabisBranches)
        {
            branch.InstantiateLeaf(leafPrefabs, branch.numberOfLeafes, branch.scale, branch.isOnTop, this.transform);
            branch.leaf.transform.rotation *= Quaternion.Euler(80f, 0f, 0f);
            
            List<GameObject> leaflets = branch.leaf.GetComponentsInChildren<Transform>()
                .Select(t => t.gameObject)
                .ToList();
            foreach (var gameObject in leaflets)
            {
                gameObject.layer = _plantLayer;

                if (gameObject.GetComponentInChildren<MeshRenderer>() != null)
                    gameObject.GetComponentInChildren<MeshRenderer>().material = material;
            }
        }
    }

    private void SpawnLeafAtEndOfBranch(GameObject[] leafPrefabs, int numberLeafes ,CannabisBranch branch, float scale, bool isOnTop) {
            branch.InstantiateLeaf(leafPrefabs, numberLeafes, scale, isOnTop, this.transform);
            List<GameObject> leaflets = branch.leaf.GetComponentsInChildren<Transform>()
                .Select(t => t.gameObject)
                .ToList();
            foreach (var gameObject in leaflets)
            {
                gameObject.layer = _plantLayer;
            }
    }

    


    public void SetLayerMask()
    {
        foreach (Transform child in transform)
        {
            child.gameObject.layer = _plantLayer;
            List<GameObject> gameObjects =
                child.GetComponentsInChildren<Transform>().Select(t => t.gameObject).ToList();
            foreach (GameObject gameObject in gameObjects)
            {
                gameObject.layer = _plantLayer;
            }
        }
    }

    public void RemodelCannabisPlant(bool correctAmountOfNutrients, bool correctAmountOfWater) {
        if (!correctAmountOfNutrients)
        {
            if (_plantController.PlantData.Soil.StoredNutrients < PlantManagerConstants.MinNutrients)
            {
                //not enough nutrients -> colour of the cannabisbranches turn brighter and yellowish
                GenerateMesh(_materials[1]);
            }

            if (_plantController.PlantData.Soil.StoredNutrients > PlantManagerConstants.MaxNutrients)
            {
                //too many nutrients -> colour of the cannabisbranches turn to a brighter green
                GenerateMesh(_materials[2]);
            }
        }

        if (!correctAmountOfWater)
        {
            if (_plantController.PlantData.Soil.StoredWater < PlantManagerConstants.MinWater)
            {
                //not enough water -> colour of leafs turn darker and leafs hang down
                ChangeLeafs(_materials[3]);
            }

            if (_plantController.PlantData.Soil.StoredWater > PlantManagerConstants.MaxWater)
            {
                //too much water -> colour of leafs turn brighter and leafs hang down
                ChangeLeafs(_materials[4]);

            }
        }
        
        
    }
    


    
    private void ShowSplines(List<CannabisBranch> cannabisBranches) {
        for (int i = 0; i <= cannabisBranches.Count - 1; i++) {
            for (int j = 0; j < cannabisBranches[i].catmullRomSplinePoints.Count - 1; j++) {
                Debug.DrawLine(cannabisBranches[i].catmullRomSplinePoints[j], cannabisBranches[i].catmullRomSplinePoints[j + 1], Color.white);
            }
        }
    }
    

    



    
    
    
    private Vector3 GetTangent(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3) {
        float t2 = t * t;

        return 0.5f * (
            -p0 + p2 +
            2f * t * (2f * p0 - 5f * p1 + 4f * p2 - p3) +
            3f * t2 * (-p0 + 3f * p1 - 3f * p2 + p3)
        ).normalized;
    }

    

    private void GenerateSubBranch(CannabisBranch branch, float t, float xDirection, float yDirection, int numberOfSplineParts) {
        

        int id = _cannabisBranches.Count;
        List<CannabisSplinePart> splineParts = branch.splineParts;
                
                
                
        int partIndex = Mathf.FloorToInt(t * (splineParts.Count - 1));
        float localT = (t * (splineParts.Count - 1)) - partIndex;
                
        CannabisSplinePart selectedPart = splineParts[partIndex];
        Vector3 branchPosition = _catmullRomSplineReference.GetCatmullRomPosition(localT, 
            selectedPart.SplinePointA, 
            splineParts[partIndex + 1].SplinePointA, 
            splineParts[partIndex + 2].SplinePointA, 
            splineParts[partIndex + 2].SplinePointB);
                
        Vector3 tangent = GetTangent(localT, 
            selectedPart.SplinePointA, 
            splineParts[partIndex + 1].SplinePointA, 
            splineParts[partIndex + 2].SplinePointA, 
            splineParts[partIndex + 2].SplinePointB);
        Vector3 branchDirection = Quaternion.Euler(xDirection, yDirection, 0) * tangent;
        Vector3 currentPosition = branchPosition;
                
        CannabisBranch branch1 = new CannabisBranch(id, branchDirection);
        for (int i = 0; i <= numberOfSplineParts; i++) {
            Vector3 newSplinePointB = (currentPosition + branchDirection * _length) + Random.insideUnitSphere * _length / 8;
            CannabisSplinePart splinePart = new CannabisSplinePart(currentPosition, newSplinePointB, null, null, 0);
            currentPosition = splinePart.SplinePointB;
            branch1.AddSplinePartsToBranch(splinePart);
        }
                
        branch1.AddCatmullRomSplinePoints(_catmullRomSplineReference.CalculateCatmullRomSplinePoints(branch1.splineParts, 20));
        _cannabisBranches.Add(branch1);
    }
    
    public void GenerateMesh(Material material) {
        GameObject[] allObjects = GameObject.FindObjectsByType<GameObject>(FindObjectsSortMode.None);
        
        foreach (Transform child in transform)
        {
            if (child.name == "Branch") {
                Destroy(child.gameObject); 
            }
        }
        
        for (int i = 0; i <= _cannabisBranches.Count - 1; i++) {
            Mesh tubeMesh = meshGenerator.GenerateMesh(_cannabisBranches[i].catmullRomSplinePoints, _radius, CIRCLE_SEGMENTS);
            GameObject tubeObject = new GameObject("Branch");
            tubeObject.transform.parent = this.transform;
            MeshFilter meshFilter = tubeObject.AddComponent<MeshFilter>();
            MeshRenderer meshRenderer = tubeObject.AddComponent<MeshRenderer>();
            meshFilter.mesh = tubeMesh;
            meshRenderer.material = material;
        }
        SetLayerMask();
    }

    private void DeleteAllLeafes() {
        GameObject[] allObjects = FindObjectsByType<GameObject>(FindObjectsSortMode.None);
        foreach (Transform child in transform)
        {
            if (child.name == "Leaf") {
                Destroy(child.gameObject); 
            }
        }
    }
    
    private void ExtendBranch(CannabisBranch branch, Vector3 dir, int segments) {
        _initialPosition = branch.splineParts[branch.splineParts.Count - 1].SplinePointB;
        for (int i = 0; i < segments; i++) {

            Vector3 newSplinePointB = (_initialPosition + dir * _length) +
                                      Random.insideUnitSphere * _length / 8;
            CannabisSplinePart splinePart =
                new CannabisSplinePart(_initialPosition, newSplinePointB, null, null, 0);
            _initialPosition = splinePart.SplinePointB;
            branch.AddSplinePartsToBranch(splinePart);
        }
        branch.AddCatmullRomSplinePoints(
            _catmullRomSplineReference.CalculateCatmullRomSplinePoints(branch.splineParts, 20));
    }
}

