using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.UIElements;

public static class MeshGenerator
{
    public static IEnumerator GenerateTerrainMesh(float[,] heightMap, MeshSettings meshSettings, int levelOfDetail, Action<MeshData> callback)
    {
        yield return new WaitForSeconds(0.1f); // delay

        // Calculate the increment step based on the level of detail.
        int skipIncrement = (levelOfDetail == 0) ? 1 : levelOfDetail * 2;
        int numVertsPerLine = meshSettings.numVertsPerLine;

        // Define the top left starting point for mesh generation.
        Vector2 topLeft = new Vector2(-1, 1) * meshSettings.meshWorldSize / 2f;

        // Create a new MeshData object for the given number of vertices per line.
        MeshData meshData = new MeshData(numVertsPerLine, skipIncrement, meshSettings.useFlatShading);

        // Initialize the map for tracking vertex indices.
        int[,] vertexIndicesMap = new int[numVertsPerLine, numVertsPerLine];
        int meshVertexIndex = 0;
        int outOfMeshVertexIndex = -1;

        // Iterate through each vertex position in the grid.
        for (int y = 0; y < numVertsPerLine; y++)
        {
            for (int x = 0; x < numVertsPerLine; x++)
            {
                // Determine if the vertex is on the edge or should be skipped.
                bool isOutOfMeshVertex = y == 0 || y == numVertsPerLine - 1 || x == 0 || x == numVertsPerLine - 1;
                bool isSkippedVertex = x > 2 && x < numVertsPerLine - 3 && y > 2 && y < numVertsPerLine - 3 && ((x - 2) % skipIncrement != 0 || (y - 2) % skipIncrement != 0);

                // Map vertex indices for mesh and out-of-mesh vertices.
                if (isOutOfMeshVertex)
                {
                    vertexIndicesMap[x, y] = outOfMeshVertexIndex;
                    outOfMeshVertexIndex--;
                }
                else if (!isSkippedVertex)
                {
                    vertexIndicesMap[x, y] = meshVertexIndex;
                    meshVertexIndex++;
                }
            }
        }

        // Process each vertex for mesh and edge connections.
        for (int y = 0; y < numVertsPerLine; y++)
        {
            for (int x = 0; x < numVertsPerLine; x++)
            {
                // Check if vertex should be skipped based on LOD.
                bool isSkippedVertex = x > 2 && x < numVertsPerLine - 3 && y > 2 && y < numVertsPerLine - 3 && ((x - 2) % skipIncrement != 0 || (y - 2) % skipIncrement != 0);

                if (!isSkippedVertex)
                {
                    // Additional checks for different types of vertices.
                    bool isOutOfMeshVertex = y == 0 || y == numVertsPerLine - 1 || x == 0 || x == numVertsPerLine - 1;
                    bool isMeshEdgeVertex = (y == 1 || y == numVertsPerLine - 2 || x == 1 || x == numVertsPerLine - 2) && !isOutOfMeshVertex;
                    bool isMainVertex = (x - 2) % skipIncrement == 0 && (y - 2) % skipIncrement == 0 && !isOutOfMeshVertex && !isMeshEdgeVertex;
                    bool isEdgeConnectionVertex = (y == 2 || y == numVertsPerLine - 3 || x == 2 || x == numVertsPerLine - 3) && !isOutOfMeshVertex && !isMeshEdgeVertex && !isMainVertex;

                    // Get the vertex index and calculate its position.
                    int vertexIndex = vertexIndicesMap[x, y];
                    Vector2 percent = new Vector2(x - 1, y - 1) / (numVertsPerLine - 3);
                    Vector2 vertexPosition2D = topLeft + new Vector2(percent.x, -percent.y) * meshSettings.meshWorldSize;
                    float height = heightMap[x, y];

                    // Adjust vertex position for edge connection vertices.
                    if (isEdgeConnectionVertex)
                    {
                        bool isVertical = x == 2 || x == numVertsPerLine - 3;
                        int dstToMainVertexA = ((isVertical) ? y - 2 : x - 2) % skipIncrement;
                        int dstToMainVertexB = skipIncrement - dstToMainVertexA;
                        float dstPercentFromAToB = dstToMainVertexA / (float)skipIncrement;

                        Coord coordA = new Coord((isVertical) ? x : x - dstToMainVertexA, (isVertical) ? y - dstToMainVertexA : y);
                        Coord coordB = new Coord((isVertical) ? x : x + dstToMainVertexB, (isVertical) ? y + dstToMainVertexB : y);

                        float heightMainVertexA = heightMap[coordA.x, coordA.y];
                        float heightMainVertexB = heightMap[coordB.x, coordB.y];
                        height = heightMainVertexA * (1 - dstPercentFromAToB) + heightMainVertexB * dstPercentFromAToB;
                    }

                    // Add vertex to the mesh data.
                    meshData.AddVertex(new Vector3(vertexPosition2D.x, height, vertexPosition2D.y), percent, vertexIndex);

                    // Create triangles for the mesh.
                    if (x < numVertsPerLine - 1 && y < numVertsPerLine - 1 && (!isEdgeConnectionVertex || (x != 2 && y != 2)))
                    {
                        int currentIncrement = (isMainVertex && x != numVertsPerLine - 3 && y != numVertsPerLine - 3) ? skipIncrement : 1;

                        int a = vertexIndicesMap[x, y];
                        int b = vertexIndicesMap[x + currentIncrement, y];
                        int c = vertexIndicesMap[x, y + currentIncrement];
                        int d = vertexIndicesMap[x + currentIncrement, y + currentIncrement];
                        meshData.AddTriangle(a, d, c);
                        meshData.AddTriangle(d, a, b);
                    }
                }
            }
        }

        // Finalize the mesh data.
        meshData.ProcessMesh();

        // Return the generated mesh data through the callback
        callback(meshData);
    }

    public struct Coord
    {
        public readonly int x;
        public readonly int y;

        public Coord(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
    }
}

    public class EdgeConnectionVertexData
{
    public int vertexIndex;
    public int mainVertexAIndex;
    public int mainVertexBIndex;
    public float dstPercentFromAToB;

    public EdgeConnectionVertexData(int vertexIndex, int mainVertexAIndex, int mainVertexBIndex, float dstPercentFromAToB)
    {
        this.vertexIndex = vertexIndex;
        this.mainVertexAIndex = mainVertexAIndex;
        this.mainVertexBIndex = mainVertexBIndex;
        this.dstPercentFromAToB = dstPercentFromAToB;
    }


}
public class MeshData
{
    Vector3[] vertices;
    int[] triangles;
    Vector2[] uvs;
    Vector3[] bakedNormals;

    Vector3[] outOfMeshVertices;
    int[] outOfMeshTriangles;

    int triangleIndex;
    int outOfMeshTriangleIndex;

    List<EdgeConnectionVertexData> edgeConnectionVertices = new List<EdgeConnectionVertexData>();

    bool useFlatShading;

    public MeshData(int numVertsPerLine, int skipIncrement, bool useFlatShading)
    {
        this.useFlatShading = useFlatShading;

        // Calculate the number of edge, connection, and main vertices.
        int numMeshEdgeVertices = (numVertsPerLine - 2) * 4 - 4;
        int numEdgeConnectionVertices = (skipIncrement - 1) * (numVertsPerLine - 5) / skipIncrement * 4;
        int numMainVerticesPerLine = (numVertsPerLine - 5) / skipIncrement + 1;
        int numMainVertices = numMainVerticesPerLine * numMainVerticesPerLine;

        // Initialize the vertices and UVs arrays.
        vertices = new Vector3[numMeshEdgeVertices + numEdgeConnectionVertices + numMainVertices];
        uvs = new Vector2[vertices.Length];

        // Calculate the number of triangles for mesh edges and main areas.
        int numMeshEdgeTriangles = 8 * (numVertsPerLine - 4);
        int numMainTriangles = (numMainVerticesPerLine - 1) * (numMainVerticesPerLine - 1) * 2;
        triangles = new int[(numMeshEdgeTriangles + numMainTriangles) * 3];

        // Initialize arrays for out-of-mesh vertices and triangles.
        outOfMeshVertices = new Vector3[numVertsPerLine * 4 - 4];
        outOfMeshTriangles = new int[24 * (numVertsPerLine - 2)];
    }

    // Add a vertex to the mesh data.
    public void AddVertex(Vector3 vertexPosition, Vector2 uv, int vertexIndex)
    {
        if (vertexIndex < 0)
        {
            outOfMeshVertices[-vertexIndex - 1] = vertexPosition;
        }
        else
        {
            vertices[vertexIndex] = vertexPosition;
            uvs[vertexIndex] = uv;
        }
    }

    // Add a triangle to the mesh data.
    public void AddTriangle(int a, int b, int c)
    {
        if (a < 0 || b < 0 || c < 0)
        {
            outOfMeshTriangles[outOfMeshTriangleIndex] = a;
            outOfMeshTriangles[outOfMeshTriangleIndex + 1] = b;
            outOfMeshTriangles[outOfMeshTriangleIndex + 2] = c;
            outOfMeshTriangleIndex += 3;
        }
        else
        {
            triangles[triangleIndex] = a;
            triangles[triangleIndex + 1] = b;
            triangles[triangleIndex + 2] = c;
            triangleIndex += 3;
        }
    }

    // Declare an edge connection vertex in the mesh data.
    public void DeclareEdgeConnectionVertex(EdgeConnectionVertexData edgeConnectionVertexData)
    {
        if (edgeConnectionVertexData == null)
        {
            Debug.LogError("Null EdgeConnectionVertexData is being declared.");
            return;
        }

        edgeConnectionVertices.Add(edgeConnectionVertexData);
    }

    // Calculate normals for the vertices.
    Vector3[] CalculateNormals()
    {

        Vector3[] vertexNormals = new Vector3[vertices.Length];
        int triangleCount = triangles.Length / 3;
        for (int i = 0; i < triangleCount; i++)
        {
            int normalTriangleIndex = i * 3;
            int vertexIndexA = triangles[normalTriangleIndex];
            int vertexIndexB = triangles[normalTriangleIndex + 1];
            int vertexIndexC = triangles[normalTriangleIndex + 2];

            Vector3 triangleNormal = SurfaceNormalFromIndices(vertexIndexA, vertexIndexB, vertexIndexC);
            vertexNormals[vertexIndexA] += triangleNormal;
            vertexNormals[vertexIndexB] += triangleNormal;
            vertexNormals[vertexIndexC] += triangleNormal;
        }

        int borderTriangleCount = outOfMeshTriangles.Length / 3;
        for (int i = 0; i < borderTriangleCount; i++)
        {
            int normalTriangleIndex = i * 3;
            int vertexIndexA = outOfMeshTriangles[normalTriangleIndex];
            int vertexIndexB = outOfMeshTriangles[normalTriangleIndex + 1];
            int vertexIndexC = outOfMeshTriangles[normalTriangleIndex + 2];

            Vector3 triangleNormal = SurfaceNormalFromIndices(vertexIndexA, vertexIndexB, vertexIndexC);
            if (vertexIndexA >= 0)
            {
                vertexNormals[vertexIndexA] += triangleNormal;
            }
            if (vertexIndexB >= 0)
            {
                vertexNormals[vertexIndexB] += triangleNormal;
            }
            if (vertexIndexC >= 0)
            {
                vertexNormals[vertexIndexC] += triangleNormal;
            }
        }


        for (int i = 0; i < vertexNormals.Length; i++)
        {
            vertexNormals[i].Normalize();
        }

        return vertexNormals;

    }

    // Process edge connection vertices to smooth normals across the mesh.
    void ProcessEdgeConnectionVertices()
    {
        if (bakedNormals == null)
        {
            Debug.LogError("bakedNormals is null in ProcessEdgeConnectionVertices.");
            return;
        }

        foreach (EdgeConnectionVertexData e in edgeConnectionVertices)
        {
            if (e == null)
            {
                Debug.LogError("An element in edgeConnectionVertices is null.");
                continue;
            }

            if (e.mainVertexAIndex < 0 || e.mainVertexAIndex >= bakedNormals.Length ||
                e.mainVertexBIndex < 0 || e.mainVertexBIndex >= bakedNormals.Length ||
                e.vertexIndex < 0 || e.vertexIndex >= bakedNormals.Length)
            {
                Debug.LogError("Invalid index in edgeConnectionVertices.");
                continue;
            }

            bakedNormals[e.vertexIndex] = bakedNormals[e.mainVertexAIndex] * (1 - e.dstPercentFromAToB) + bakedNormals[e.mainVertexBIndex] * e.dstPercentFromAToB;
        }
    }

    Vector3 SurfaceNormalFromIndices(int indexA, int indexB, int indexC)
    {
        Vector3 pointA = (indexA < 0) ? outOfMeshVertices[-indexA - 1] : vertices[indexA];
        Vector3 pointB = (indexB < 0) ? outOfMeshVertices[-indexB - 1] : vertices[indexB];
        Vector3 pointC = (indexC < 0) ? outOfMeshVertices[-indexC - 1] : vertices[indexC];

        Vector3 sideAB = pointB - pointA;
        Vector3 sideAC = pointC - pointA;
        return Vector3.Cross(sideAB, sideAC).normalized;
    }

    public void ProcessMesh()
    {
        if (useFlatShading)
        {
            FlatShading();
        }
        else
        {
            BakeNormals();

            if (bakedNormals != null)
            {
                ProcessEdgeConnectionVertices();
            }
        }
    }

    void BakeNormals()
    {
        bakedNormals = CalculateNormals();
        Debug.Assert(bakedNormals != null, "bakedNormals is null after BakeNormals.");
    }



    void FlatShading()
    {
        Vector3[] flatShadedVertices = new Vector3[triangles.Length];
        Vector2[] flatShadedUvs = new Vector2[triangles.Length];

        for (int i = 0; i < triangles.Length; i++)
        {
            flatShadedVertices[i] = vertices[triangles[i]];
            flatShadedUvs[i] = uvs[triangles[i]];
            triangles[i] = i;
        }

        vertices = flatShadedVertices;
        uvs = flatShadedUvs;
    }

    public Mesh CreateMesh()
    {
        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        if (useFlatShading)
        {
            mesh.RecalculateNormals();
        }
        else
        {
            mesh.normals = bakedNormals;
        }
        return mesh;
    }

}