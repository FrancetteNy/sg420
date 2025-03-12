using UnityEngine;
using System.Collections.Generic;
public class CannabisBranchData
{
    public List<Vector3> SplinePoints;
    public Vector3 BranchDirection;
    public float YDirection;

    public CannabisBranchData(CannabisBranch branch)
    {
        SplinePoints = branch.catmullRomSplinePoints;
        BranchDirection = branch.branchDirection;
        YDirection = branch.yDirection;
    }

}
