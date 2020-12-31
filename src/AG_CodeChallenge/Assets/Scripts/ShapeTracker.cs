using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class ShapePoints
{
    public string shapeName;
    public Vector3[] points;
}

public class ShapeTracker : MonoBehaviour
{
    public GameObject[] shapes;
    public int currentShape = 0;

    public ShapePoints[] drawShapes;
    public Material shapeMat;
    private Coroutine[] shapeChangeCos;

    public float growTime = 1.0f;
    public float growTimeBounce = 0.25f;
    public float shrinkTime = 0.75f;
    private int lastShape = 0;

    private void OnEnable()
    {
        CreateObjects();

        shapeChangeCos = new Coroutine[drawShapes.Length];
    }

    private void CreateObjects()
    {
        for (int i = 0; i < drawShapes.Length; i++)
        {
            if (drawShapes[i].shapeName == "Circle")
            {
                CreateCircleMesh(i);
                CreateCircleCollider(i);
            }
            else
            {
                CreateMesh(i);
                CreateCollider(i);
            }
        }
        SetShapeScale(Vector3.zero);
    }

    private void CreateMesh(int num)
    {
        MeshRenderer meshRenderer = shapes[num].AddComponent<MeshRenderer>();
        meshRenderer.sharedMaterial = shapeMat;
        MeshFilter meshFilter = shapes[num].AddComponent<MeshFilter>();

        // Convert Vector3 vertices to Vector2 for triangulation
        Vector2[] trianglesV2 = new Vector2[drawShapes[num].points.Length];
        for (int i = 0; i < trianglesV2.Length; i++)
        {
            trianglesV2[i] = drawShapes[num].points[i];
        }
        var triangulator = new Triangulator(trianglesV2);
        var trianglesT = triangulator.Triangulate();

        // Make sure same number of normals as vertices
        Vector3[] normals = new Vector3[drawShapes[num].points.Length];
        for (int i = 0; i < normals.Length; i++)
        {
            normals[i] = -Vector3.forward;
        }

        Mesh newMesh = new Mesh
        {
            vertices = drawShapes[num].points,
            triangles = trianglesT,
            normals = normals
        };

        meshFilter.mesh = newMesh;

        meshRenderer.sortingLayerName = "Shapes";
    }

    private void CreateCircleMesh(int num)
    {
        MeshRenderer meshRenderer = shapes[num].AddComponent<MeshRenderer>();
        meshRenderer.sharedMaterial = shapeMat;
        MeshFilter meshFilter = shapes[num].AddComponent<MeshFilter>();

        int numVertices = drawShapes[num].points.Length;
        float cirRadius = drawShapes[num].points[0].x;

        var circleVertices = Enumerable.Range(0, numVertices).Select(i => {
        var theta = 2 * Mathf.PI * i / numVertices;
        return new Vector2(Mathf.Cos(theta), Mathf.Sin(theta)) * cirRadius;
        }).ToArray();

        // Convert circleVertices Vector2 array to Vector3 array
        Vector3[] circleVertices3d = new Vector3[circleVertices.Length];
        for (int i = 0; i < circleVertices3d.Length; i++)
        {
            circleVertices3d[i] = circleVertices[i];
        }

        // Triangles
        var trianglesT = new Triangulator(circleVertices).Triangulate();

        // Make sure same number of normals as vertices
        Vector3[] normals = new Vector3[drawShapes[num].points.Length];
        for (int i = 0; i < normals.Length; i++)
        {
            normals[i] = -Vector3.forward;
        }

        Mesh newMesh = new Mesh
        {
            vertices = circleVertices3d,
            triangles = trianglesT,
            normals = normals
        };

        meshFilter.mesh = newMesh;

        meshRenderer.sortingLayerName = "Shapes";
    }

    private void CreateCollider(int num)
    {
        PolygonCollider2D polyColl = shapes[num].AddComponent<PolygonCollider2D>();

        Vector2[] pointArray = new Vector2[drawShapes[num].points.Length];
        for (int i = 0; i < drawShapes[num].points.Length; i++)
        {
            pointArray[i].x = drawShapes[num].points[i].x;
            pointArray[i].y = drawShapes[num].points[i].y;
        }

        polyColl.points = pointArray;
    }

    private void CreateCircleCollider(int num)
    {
        CircleCollider2D circColl = shapes[num].AddComponent<CircleCollider2D>();
        circColl.radius = drawShapes[num].points[0].x;
        circColl.offset = Vector2.zero;
    }

    private void SetShapeScale (Vector3 setScale)
    {
        for (int i = 0; i < shapes.Length; i++)
        {
            if (i != lastShape)
            {
                shapes[i].transform.localScale = setScale;
            }
        }
    }

    public void SwitchShape(int newShape)
    {
        shapes[newShape].SetActive(true);

        // Stop scale change of new and last shape
        if (shapeChangeCos[lastShape] != null) StopCoroutine(shapeChangeCos[lastShape]);
        if (shapeChangeCos[newShape] != null) StopCoroutine(shapeChangeCos[newShape]);
        // Shrink last shape
        shapeChangeCos[lastShape] = StartCoroutine(ChangeShapeSize(false, lastShape));
        // Grow new shape
        shapeChangeCos[newShape] = StartCoroutine(ChangeShapeSize(true, newShape));

        lastShape = newShape;
    }

    IEnumerator ChangeShapeSize(bool grow, int whichShape)
    {
        float changeTime = grow ? growTime : shrinkTime;
        Vector3 endScale = grow ? new Vector3(1.2f, 1.2f, 1.2f) : Vector3.zero;
        Vector3 startScale = shapes[whichShape].transform.localScale;

        float t = 0;
        // Grow or shrink
        while (t < 1)
        {
            t += Mathf.Clamp(Time.deltaTime / changeTime, 0, 1);
            float tEased = Mathf.SmoothStep(0, 1, t);
            shapes[whichShape].transform.localScale = Vector3.Lerp(startScale, endScale, tEased);

            yield return null;
        }

        if (!grow) shapes[whichShape].SetActive(false);

        // Bounce if growing
        if (grow)
        {
            t = 0;

            while (t < 1)
            {
                t += Mathf.Clamp(Time.deltaTime / growTimeBounce, 0, 1);
                float tEased = Mathf.SmoothStep(0, 1, t);
                shapes[whichShape].transform.localScale = Vector3.Lerp(endScale, Vector3.one, tEased);

                yield return null;
            }
        }

    }

}
