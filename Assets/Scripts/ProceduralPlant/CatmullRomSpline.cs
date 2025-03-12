using UnityEngine;
using System.Collections.Generic;

public class CatmullRomSpline
{
    public Vector3 GetCatmullRomPosition(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3) {
        float t2 = t * t;
        float t3 = t2 * t;

        return 0.5f * (
            (2f * p1) +
            (-p0 + p2) * t +
            (2f * p0 - 5f * p1 + 4f * p2 - p3) * t2 +
            (-p0 + 3f * p1 - 3f * p2 + p3) * t3
        );
    }

    public List<Vector3> CalculateCatmullRomSplinePoints(List<CannabisSplinePart> splineParts, int segmentsPerCurve) {

        List<Vector3> catmullRomSplinePoints = new List<Vector3>();
        
        for (int i = 0; i < splineParts.Count - 1; i++) {
            Vector3 p0 = (i == 0) ? splineParts[i].SplinePointA : splineParts[i - 1].SplinePointA;
            Vector3 p1 = splineParts[i].SplinePointA;
            Vector3 p2 = splineParts[i + 1].SplinePointA;
            Vector3 p3 = (i == splineParts.Count - 2) ? splineParts[i + 1].SplinePointB : splineParts[i + 2].SplinePointA;
            Vector3 previousPoint = p1;
            for (int j = 1; j <= segmentsPerCurve; j++) {
                float t = j / (float)segmentsPerCurve;
                Vector3 currentPoint = GetCatmullRomPosition(t, p0, p1, p2, p3);
                    
                if (j == segmentsPerCurve && i == splineParts.Count - 2) {
                    currentPoint = p3;
                }
                    
                catmullRomSplinePoints.Add(currentPoint);
                previousPoint = currentPoint;
            }
        }
        return catmullRomSplinePoints;
    }
}
