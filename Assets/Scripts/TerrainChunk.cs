using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class TerrainChunk
{
    // Coordinates of the chunk in the terrain grid.
    public Vector2 coord;

    // GameObjects and components for rendering and collision detection.
    private GameObject meshObject;
    private Vector2 sampleCentre;
    private Bounds bounds;

    private MeshRenderer meshRenderer;
    private MeshFilter meshFilter;
    private MeshCollider meshCollider;

    // Level of Detail (LOD) information.
    private LODInfo[] detailLevels;
    private LODMesh[] lodMeshes;
    private int colliderLODIndex;

    // Height map and its status.
    private HeightMap heightMap;
    private bool heightMapReceived;
    private int previousLODIndex = -1;
    private bool hasSetCollider;
    private float maxViewDst;

    // Settings for height map and mesh.
    private HeightMapSettings heightMapSettings;
    private MeshSettings meshSettings;

    // The viewer's position, used for LOD calculations.
    private Transform viewer;

    // Event triggered when the visibility of the chunk changes.
    public event System.Action<TerrainChunk, bool> onVisibilityChanged;

    public TerrainChunk(Vector2 coord, HeightMapSettings heightMapSettings, MeshSettings meshSettings, LODInfo[] detailLevels, int colliderLODIndex, Transform parent, Transform viewer, Material material)
    {
        this.coord = coord;
        this.detailLevels = detailLevels;
        this.colliderLODIndex = colliderLODIndex;
        this.heightMapSettings = heightMapSettings;
        this.meshSettings = meshSettings;
        this.viewer = viewer;

        // Calculate the sample center based on the chunk coordinate and mesh world size.
        sampleCentre = coord * meshSettings.meshWorldSize / meshSettings.meshScale;
        Vector2 position = coord * meshSettings.meshWorldSize;
        bounds = new Bounds(position, Vector2.one * meshSettings.meshWorldSize);

        // Initialize the mesh object and components.
        meshObject = new GameObject("Terrain Chunk");
        meshRenderer = meshObject.AddComponent<MeshRenderer>();
        meshFilter = meshObject.AddComponent<MeshFilter>();
        meshCollider = meshObject.AddComponent<MeshCollider>();
        meshRenderer.material = material;

        meshObject.transform.position = new Vector3(position.x, 0, position.y);
        meshObject.transform.parent = parent;
        SetVisible(false);

        // Initialize LOD meshes.
        lodMeshes = new LODMesh[detailLevels.Length];
        for (int i = 0; i < detailLevels.Length; i++)
        {
            lodMeshes[i] = new LODMesh(detailLevels[i].lod, UpdateTerrainChunk);
            if (i == colliderLODIndex)
            {
                lodMeshes[i].updateCallback += UpdateCollisionMesh;
            }
        }

        maxViewDst = detailLevels[detailLevels.Length - 1].visibleDstThreshold;
        Load();
    }

    // Loads the height map for the chunk.
    public void Load()
    {
        MonoBehaviourHelper.Instance.StartHelperCoroutine(
            HeightMapGenerator.GenerateHeightMap(
                meshSettings.numVertsPerLine, meshSettings.numVertsPerLine,
                heightMapSettings, sampleCentre, OnHeightMapReceived));
    }

    // Callback for when the height map is received.
    private void OnHeightMapReceived(HeightMap map)
    {
        this.heightMap = map;
        heightMapReceived = true;
        UpdateTerrainChunk();
    }

    // Updates the terrain chunk based on the viewer's position.
    public void UpdateTerrainChunk()
    {
        if (heightMapReceived)
        {
            float viewerDstFromNearestEdge = Mathf.Sqrt(bounds.SqrDistance(viewerPosition));
            bool visible = viewerDstFromNearestEdge <= maxViewDst;

            Debug.Log($"Chunk at {coord}, Distance: {viewerDstFromNearestEdge}, Visible: {visible}");

            if (visible)
            {
                int lodIndex = 0;
                for (int i = 0; i < detailLevels.Length - 1; i++)
                {
                    if (viewerDstFromNearestEdge > detailLevels[i].visibleDstThreshold)
                    {
                        lodIndex = i + 1;
                    }
                    else
                    {
                        break;
                    }
                }

                if (lodIndex != previousLODIndex)
                {
                    LODMesh lodMesh = lodMeshes[lodIndex];
                    if (lodMesh.hasMesh)
                    {
                        previousLODIndex = lodIndex;
                        meshFilter.mesh = lodMesh.mesh;
                    }
                    else if (!lodMesh.hasRequestedMesh)
                    {
                        lodMesh.RequestMesh(heightMap, meshSettings);
                    }
                }
            }

            // Change visibility of the chunk based on the distance to the viewer.
            if (IsVisible() != visible)
            {
                SetVisible(visible);
                onVisibilityChanged?.Invoke(this, visible);
            }
        }
    }

    // Update the collision mesh for the chunk.
    public void UpdateCollisionMesh()
    {
        if (!hasSetCollider)
        {
            float sqrDstFromViewerToEdge = bounds.SqrDistance(viewerPosition);

            if (sqrDstFromViewerToEdge < detailLevels[colliderLODIndex].sqrVisibleDstThreshold)
            {
                if (!lodMeshes[colliderLODIndex].hasRequestedMesh)
                {
                    lodMeshes[colliderLODIndex].RequestMesh(heightMap, meshSettings);
                }
            }

            if (sqrDstFromViewerToEdge < detailLevels[colliderLODIndex].sqrVisibleDstThreshold)
            {
                if (lodMeshes[colliderLODIndex].hasMesh)
                {
                    meshCollider.sharedMesh = lodMeshes[colliderLODIndex].mesh;
                    hasSetCollider = true;
                }
            }
        }
    }

    public void SetVisible(bool visible)
    {
        Debug.Log($"Setting visibility of chunk at {coord} to {visible}");
        meshObject.SetActive(visible);
    }



    public bool IsVisible()
    {
        return meshObject.activeSelf;
    }

    Vector2 viewerPosition
    {
        get
        {
            return new Vector2(viewer.position.x, viewer.position.z);
        }
    }
}

public class LODMesh
{
    public Mesh mesh;
    public bool hasRequestedMesh;
    public bool hasMesh;
    private int lod;
    public event System.Action updateCallback;

    public LODMesh(int lod, System.Action updateCallback)
    {
        this.lod = lod;
        this.updateCallback = updateCallback;
    }

    // Request mesh with specific LOD settings.
    public void RequestMesh(HeightMap heightMap, MeshSettings meshSettings)
    {
        hasRequestedMesh = true;
        MonoBehaviourHelper.Instance.StartHelperCoroutine(GenerateMesh(heightMap, meshSettings));
    }

    // Coroutine to generate the terrain mesh.
    private IEnumerator GenerateMesh(HeightMap heightMap, MeshSettings meshSettings)
    {
        yield return MeshGenerator.GenerateTerrainMesh(heightMap.values, meshSettings, lod, OnMeshDataReceived);
    }

    // Callback for when mesh data is received.
    private void OnMeshDataReceived(MeshData meshData)
    {
        mesh = meshData.CreateMesh();
        hasMesh = true;
        updateCallback?.Invoke();
    }
}


public class MonoBehaviourHelper : MonoBehaviour
{
    private static MonoBehaviourHelper _instance;

    public static MonoBehaviourHelper Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject obj = new GameObject("MonoBehaviourHelper");
                _instance = obj.AddComponent<MonoBehaviourHelper>();
            }
            return _instance;
        }
    }

    public void StartHelperCoroutine(IEnumerator coroutine)
    {
        StartCoroutine(coroutine);
    }
}
