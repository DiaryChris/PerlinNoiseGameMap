using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilesManager : MonoBehaviour
{


    public float cellSize;
    public Texture2D whiteTexture;
    public int viewRadius;
    public int hideRadius;


    public Tile[] pixelTerrainTiles;


    private Tilemap tilemap;
    private Grid layoutGrid;
    private Camera mainCamera;

    private Dictionary<Vector2Int, bool> tileBoolMap;

    private float orthoWidth;
    private float orthoHeight;

    // Use this for initialization
    private void Start ()
    {
        tilemap = gameObject.GetComponent<Tilemap>();
        layoutGrid = tilemap.layoutGrid;
        mainCamera = Camera.main;
        tilemap.ClearAllTiles();
        InitMap();

    }

    // Update is called once per frame
    private void Update ()
    {

        if (!PerlinMap.instance.IsUpdating())
        {
            //Debug.Log("Coroutine Start");
            StartCoroutine(PerlinMap.instance.UpdatePerlinMap());
        }

        List<Vector3Int> viewList = GetViewRange(viewRadius);
        List<Vector3Int> hideList = GetHideRange(viewRadius, hideRadius);
        SetTilesRange(viewList);
        HideTilesRange(hideList);
    }

    //get view range by main camera position
    private List<Vector3Int> GetViewRange(int radius)
    {
        List<Vector3Int> viewList = new List<Vector3Int>();

        float x = mainCamera.transform.position.x;
        float y = mainCamera.transform.position.y;

        int xi = Mathf.FloorToInt(x / cellSize);
        int yi = Mathf.FloorToInt(y / cellSize);

        for (int i = xi - radius; i <= xi + radius; i++)
        {
            for (int j = yi - radius; j <= yi + radius; j++)
            {
                if ((i - xi) * (i - xi) + (j - yi) * (j - yi) < radius * radius)
                {
                    viewList.Add(new Vector3Int(i, j, 0));
                }
            }
        }
        
        return viewList;
    }

    //get hide ring range by main camera position
    private List<Vector3Int> GetHideRange(int radius, int hideRadius)
    {
        List<Vector3Int> hideList = new List<Vector3Int>();

        float x = mainCamera.transform.position.x;
        float y = mainCamera.transform.position.y;

        int xi = Mathf.FloorToInt(x / cellSize);
        int yi = Mathf.FloorToInt(y / cellSize);

        for (int i = xi - hideRadius; i <= xi + hideRadius; i++)
        {
            for (int j = yi - hideRadius; j <= yi + hideRadius; j++)
            {
                if ((i - xi) * (i - xi) + (j - yi) * (j - yi) > radius * radius && (i - xi) * (i - xi) + (j - yi) * (j - yi) < hideRadius * hideRadius)
                {
                    hideList.Add(new Vector3Int(i, j, 0));
                }
            }
        }

        return hideList;
    }

    public void InitMap()
    {
        //set camera bgcolor
        mainCamera.backgroundColor = new Color(0f, 0f, 0f, 1f);

        //get camera width & height
        orthoWidth = mainCamera.orthographicSize * 2 * mainCamera.aspect;
        orthoHeight = mainCamera.orthographicSize * 2;

        //set cell size
        layoutGrid.cellSize = new Vector3(cellSize, cellSize, 0f);

        //setColorTiles();
        //setTerrainTiles();
    }

    public void SetColorTiles()
    {
        //new a white tile
        Tile tile = ScriptableObject.CreateInstance<Tile>();
        float spriteSize = 100 * cellSize;
        Rect spriteRect = new Rect(0, 0, spriteSize, spriteSize);
        tile.sprite = Sprite.Create(whiteTexture, spriteRect, new Vector2(0.5f, 0.5f));


        for (int i = 0; i < (int)(orthoWidth / cellSize) + 2; i++)
        {
            for (int j = 0; j < (int)(orthoHeight / cellSize) + 2; j++)
            {
                Vector3Int index = new Vector3Int(i - (int)(orthoWidth / cellSize * 0.5f) - 1, j - (int)(orthoHeight / cellSize * 0.5f) - 1, 0);
                tilemap.SetTile(index, tile);
                tilemap.RemoveTileFlags(index, TileFlags.LockColor);
                //tilemap.SetColor(index, new Color(0f, 0.5f, 0.5f));
                tilemap.SetColor(index, PerlinMap.instance.GetColorByCoord(i, j));
            }
        }

        //tilemap.SetColor(new Vector3Int(0, 0, 0), new Color(1, 1, 1));
        //tilemap.SetColor(new Vector3Int(1, 1, 0), new Color(0, 0, 0));
    }

    public void SetTerrainTile(int x, int y)
    {

        Vector3Int index = new Vector3Int(x, y, 0);
        int tileIndex = PerlinMap.instance.GetTileIndexByCoord(x, y);
        tilemap.SetTile(index, pixelTerrainTiles[tileIndex]);
    }

    public void SetTilesRange(List<Vector3Int> range)
    {
        foreach (Vector3Int index in range)
        {
            if (!tilemap.HasTile(index))
            {
                int tileIndex = PerlinMap.instance.GetTileIndexByCoord(index.x, index.y);
                if (tileIndex != -1)
                {
                    tilemap.SetTile(index, pixelTerrainTiles[tileIndex]);
                    PerlinMap.instance.InitResource(index.x, index.y);
                }
            }
            else
            {
                tilemap.RemoveTileFlags(index, TileFlags.LockColor);
                tilemap.SetColor(index, new Color(1, 1, 1));
                Transform resource = PerlinMap.instance.GetResourceByCoord(index.x, index.y);
                if (resource)
                {
                    resource.gameObject.SetActive(true);
                }
            }
        }
    }

    public void HideTilesRange(List<Vector3Int> range)
    {
        foreach (Vector3Int index in range)
        {
            if (tilemap.HasTile(index))
            {
                tilemap.RemoveTileFlags(index, TileFlags.LockColor);
                tilemap.SetColor(index, new Color(.5f, .5f, .5f));
                Transform resource = PerlinMap.instance.GetResourceByCoord(index.x, index.y);
                if (resource)
                {
                    PerlinMap.instance.GetResourceByCoord(index.x, index.y).gameObject.SetActive(false);
                }

            }
        }
    }

    //set the "ColliderType" of specific tile to "Grid"
    public void SetTileCollider(int x, int y)
    {
        Vector3Int index = new Vector3Int(x, y, 0);
        tilemap.SetColliderType(index, Tile.ColliderType.Grid);
    }
}
