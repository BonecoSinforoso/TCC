using UnityEngine;

public class TerrainTextureDetector : MonoBehaviour
{
    [SerializeField] private Terrain _terrain;

    private void Update()
    {
        Vector3 playerPosition = transform.position;

        Vector3 terrainPosition = playerPosition - _terrain.transform.position;

        TerrainData terrainData = _terrain.terrainData;
        int mapX = Mathf.RoundToInt((terrainPosition.x / terrainData.size.x) * terrainData.alphamapWidth);
        int mapZ = Mathf.RoundToInt((terrainPosition.z / terrainData.size.z) * terrainData.alphamapHeight);

        float[,,] alphaMap = terrainData.GetAlphamaps(mapX, mapZ, 1, 1);

        float maxMix = 0f;
        int maxIndex = 0;

        for (int i = 0; i < alphaMap.GetLength(2); i++)
        {
            if (alphaMap[0, 0, i] > maxMix)
            {
                maxMix = alphaMap[0, 0, i];
                maxIndex = i;
            }
        }

        Debug.Log("indice do material: " + maxIndex);
    }
}
