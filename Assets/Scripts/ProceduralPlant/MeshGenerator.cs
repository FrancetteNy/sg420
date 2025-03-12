using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MeshGenerator : MonoBehaviour
{
    [SerializeField] private AnimationCurve _radiusSize;
    

    public Mesh GenerateMesh(List<Vector3> sp, float radius, int circleSegments) {
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Vector2> uvs = new List<Vector2>();
        float relativeSizeOfSpline = 1f / sp.Count; //Size of a segment of the curve to gradually lower the radius

        for (int j = 0; j <= sp.Count - 2; j++) {
            float relativeBottomPointOnTheCurve = relativeSizeOfSpline * (j + 1); //Point on the radius-curve
            float relativeTopPointOnTheCurve = relativeSizeOfSpline * (j + 2);
            //Radius for the segments
            float currentBottomRadius = radius * _radiusSize.Evaluate(relativeBottomPointOnTheCurve);
            float currentTopRadius =radius * _radiusSize.Evaluate(relativeTopPointOnTheCurve);
            
            
            int lowerStartIndex = vertices.Count;
            
            //Bottom vertices construction
            vertices.Add(sp[j]);
            uvs.Add(new Vector2(0.5f, 0.5f));
            
            for (int i = 0; i <= circleSegments; i++) {
                float angle = i * Mathf.PI * 2 / circleSegments;
                float x = Mathf.Cos(angle) * currentBottomRadius + sp[j].x;
                float z = Mathf.Sin(angle) * currentBottomRadius + sp[j].z;
                vertices.Add(new Vector3(x, sp[j].y, z));
                uvs.Add(new Vector2((x / currentBottomRadius + 1) * 0.5f, (z / currentBottomRadius + 1) * 0.5f));
            }
            //Bottom triangles construction
            if (j == 0) {
                for (int i = 1; i <= circleSegments; i++) {
                    triangles.Add(0); // Mittelpunkt
                    triangles.Add(i); // aktueller Punkt
                    triangles.Add(i + 1); // nächster Punkt
                }
            }

            //Upper vertices construction
            int upperStartIndex = vertices.Count;
            vertices.Add(sp[j+1]);
            uvs.Add(new Vector2(0.5f, 0.5f));
            
            for (int i = 0; i <= circleSegments; i++) {
                float angle = i * Mathf.PI * 2 / circleSegments;
                float x = Mathf.Cos(angle) * currentTopRadius + sp[j+1].x;
                float z = Mathf.Sin(angle) * currentTopRadius + sp[j+1].z;
                vertices.Add(new Vector3(x, sp[j + 1].y, z));
                uvs.Add(new Vector2((x / currentTopRadius + 1) * 0.5f, (z / currentTopRadius + 1) * 0.5f));
            }
            
            //Upper triangles construction
            if (j == sp.Count() - 2) {
                for (int i = 0; i <= circleSegments; i++) {
                    triangles.Add(upperStartIndex);
                    triangles.Add(upperStartIndex + i + 1);
                    triangles.Add(upperStartIndex + i);
                }
            }
            
            //Side triangles construction, based on the lower and upper vertices
            for (int i = 1; i <= circleSegments; i++)
            {
                int lowerCurrent = i + lowerStartIndex;
                int lowerNext = i + 1 + lowerStartIndex;
                int upperCurrent = upperStartIndex + i;
                int upperNext = upperStartIndex + i + 1;

                //first triangle
                triangles.Add(lowerCurrent);
                triangles.Add(upperCurrent);
                triangles.Add(upperNext);

                //second triangle
                triangles.Add(lowerCurrent);
                triangles.Add(upperNext);
                triangles.Add(lowerNext);
            }
        }
        
        //Generating mesh
        Mesh mesh = new Mesh();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.RecalculateNormals();
        return mesh;
        
    }
}
