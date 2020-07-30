using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerlinMap : MonoBehaviour
{

    public static PerlinMap instance;

    public int perlinUnit;
    public int xInitEdge;
    public int yInitEdge;

    public int seed;
    public int randCount;

    public Texture2D colorGradient;

    public Transform[] treePrefab;
    public Transform[] stonePrefab;
    public Transform[] yTreePrefab;
	public Transform[] slimePrefab;
    public Transform resourcesParent;


    private Camera mainCamera;
    private TilesManager tilesManager;

    private List<float> randList;
    private Dictionary<Vector2Int, Vector2> perlinUnitMap;
    private Dictionary<Vector2Int, bool> perlinBoolMap;
    private Dictionary<Vector2Int, float> perlinMap;
    private Dictionary<Vector2Int, Transform> resourcesMap;
    private bool isUpdating;

    private float sqrt2 = Mathf.Sqrt(2);
    private List<terrainProp> terrainPropList;

    public Transform parentTrans;

    public struct terrainProp
    {
        public float min, max;
        public int tileIndex;
        public float treeProb, stoneProb, yTreeProb, slimeProb;
        public float smooth;
        public terrainProp(float f1, float f2, int i1, float f3, float f4, float f5, float f6,  float f7)
        {
            min = f1;
            max = f2;
            tileIndex = i1;
            smooth = f3;
            treeProb = f4;
            stoneProb = f5;
            yTreeProb = f6;
            slimeProb = f7;
        }
    }

    private void Awake()
    {
        //singleton pattern
        if (!instance)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(this);
        }

        InitPerlinMap();
        InitTerrainProp();
    }

    private void Start()
    {
        mainCamera = Camera.main;
        tilesManager = GameObject.Find("Tilemap").GetComponent<TilesManager>();
    }

    //initialize perlin map
    private void InitPerlinMap()
    {
        randList = GenerateRandList(seed, randCount);


        perlinUnitMap = new Dictionary<Vector2Int, Vector2>();
        //for (int i = 0; i <= xInitEdge / perlinUnit + 1; i++)
        //{
        //    for (int j = 0; j <= yInitEdge / perlinUnit + 1; j++)
        //    {
        //        Vector2Int index = new Vector2Int(i, j);
        //        perlinUnitMap.Add(index, CalcRandGrad(i, j));
        //    }
        //}

        ////test code
        //foreach (KeyValuePair<Vector2Int, Vector3> item in perlinUnitMap)
        //{
        //    Debug.Log(item.Key + ":" + item.Value.x + "/" + item.Value.y + "/" + item.Value.z);
        //}

        perlinMap = new Dictionary<Vector2Int, float>();
        //for (int i = 0; i < xInitEdge; i++)
        //{
        //    for (int j = 0; j < yInitEdge; j++)
        //    {
        //        perlinMap.Add(new Vector2Int(i, j), CalcPerlinMapValue(i, j));
        //    }
        //}

        ////test code
        //foreach (KeyValuePair<Vector2Int, float> item in perlinMap)
        //{
        //    Debug.Log(item.Key + ":" + item.Value);
        //}
        perlinBoolMap = new Dictionary<Vector2Int, bool>();
        resourcesMap = new Dictionary<Vector2Int, Transform>();
    }

    //initialize terrain property
    private void InitTerrainProp()
    {
        terrainPropList = new List<terrainProp>();
        terrainPropList.Add(new terrainProp(0f, .2f, 0, 0f, 0f, 0f, 0f, 0f));
        terrainPropList.Add(new terrainProp(0f, .3f, 1, 0f, 0f, 0f, 0f, 0f));
        terrainPropList.Add(new terrainProp(.3f, .4f, 2, 1f, .002f, 0f, 0f, 0f));
        terrainPropList.Add(new terrainProp(.4f, .5f, 3, 1f, .004f, 0f, 0f, 0f));
        terrainPropList.Add(new terrainProp(.5f, .6f, 4, 1f, .006f, .001f, .000f, 0f));
        terrainPropList.Add(new terrainProp(.6f, .7f, 5, 1f, .001f, .002f, .004f, .004f));
        terrainPropList.Add(new terrainProp(.7f, .9f, 6, 1f, .000f, .005f, .001f, .001f));
        terrainPropList.Add(new terrainProp(.9f, 1f, 7, .6f, 0f, .004f, 0f, 0f));
    }

    //get pseudo-random -1~1 list
    private List<float> GenerateRandList(int seed, int n)
    {
        List<float> result = new List<float>(n);
        int s = seed % 11113;
        for (int i = 0; i < 100 + n; i++)
        {
            s = s * s;
            s = s % 11113;
            if (i > 99) 
            {
                result.Add(Mathf.Clamp(s / 11113f * 2f - 1, -1, 1));
            }
        }
        return result;
    }

    //ease curse function
    private float Ease(float t)
    {
        return ((6 * t - 15) * t + 10) * t * t * t;
    }

    //calculate a random gradient vector by x and y
    private Vector2 CalcRandGrad(int x, int y)
    {

        int index = ((Mathf.Abs(x) * 97 + Mathf.Abs(y)) * 89 + 83) % randCount;

        float randX, randY; 
        if (x > 0 && y > 0)
        {
            randX = randList[index];
            randY = randList[(index + 1) % randCount];
        }
        else if (x < 0 && y > 0)
        {
            randX = randList[(index + 2) % randCount];
            randY = randList[(index + 3) % randCount];
        }
        else if (x > 0 && y < 0)
        {
            randX = randList[(index + 4) % randCount];
            randY = randList[(index + 5) % randCount];
        }
        else
        {
            randX = randList[(index + 6) % randCount];
            randY = randList[(index + 7) % randCount];
        }
        Vector2 randGrad = new Vector2(randX, randY).normalized;
        return randGrad;

    }

    //calculate perlin map value from perlin unit map
    private float CalcPerlinMapValue(int x, int y)
    {

        int xi = x / perlinUnit;
        int yi = y / perlinUnit;

        float xf = x % perlinUnit;
        float yf = y % perlinUnit;

        if (xf < 0)
        {
            xf = xf + perlinUnit;
            xi--;
        }
        if (yf < 0)
        {
            yf = yf + perlinUnit;
            yi--;
        }

        //Debug.Log(new Vector2(xf, yf));

        xf = xf / perlinUnit;
        yf = yf / perlinUnit;



        //Debug.Log(new Vector2Int(xi, yi) + " " + new Vector2(xf, yf));

        Vector2 LB = perlinUnitMap[new Vector2Int(xi, yi)];
        Vector2 LT = perlinUnitMap[new Vector2Int(xi, yi + 1)];
        Vector2 RB = perlinUnitMap[new Vector2Int(xi + 1, yi)];
        Vector2 RT = perlinUnitMap[new Vector2Int(xi + 1, yi + 1)];

        float dotLB = Vector2.Dot(new Vector2(xf, yf), LB);
        float dotLT = Vector2.Dot(new Vector2(xf, yf - 1), LT);
        float dotRB = Vector2.Dot(new Vector2(xf - 1, yf), RB);
        float dotRT = Vector2.Dot(new Vector2(xf - 1, yf - 1), RT);

        //Debug.Log("(" + x + "," + y + "):(" + dotLB + "," + dotLT + "," + dotRB + "," + dotRT + ")");

        float lerpB = Mathf.Lerp(dotLB, dotRB, Ease(xf));
        float lerpT = Mathf.Lerp(dotLT, dotRT, Ease(xf));
        float bilerp = Mathf.Lerp(lerpB, lerpT, Ease(yf));
        //Debug.Log(bilerp);

        return Mathf.Clamp((bilerp * sqrt2 + 1) / 2f, 0, 1);
        //return 0.5f;
    }

    public bool IsUpdating()
    {
        return isUpdating;
    }

    //coroutine
    public IEnumerator UpdatePerlinMap()
    {

        isUpdating = true;

        float x = mainCamera.transform.position.x;
        float y = mainCamera.transform.position.y;
        float unit = tilesManager.cellSize * perlinUnit;

        int xi = Mathf.FloorToInt(x / unit);
        int yi = Mathf.FloorToInt(y / unit);

        for(int i = xi - 1; i <= xi + 1; i++)
        {
            for (int j = yi - 1; j <= yi + 1; j++)
            {
                Vector2Int index = new Vector2Int(i, j);
                if (!perlinBoolMap.ContainsKey(index) || !perlinBoolMap[index])
                {
                    if (!perlinUnitMap.ContainsKey(index))
                    {
                        perlinUnitMap.Add(index, CalcRandGrad(i, j));
                    }
                    if (!perlinUnitMap.ContainsKey(index + Vector2Int.up))
                    {
                        perlinUnitMap.Add(index + Vector2Int.up, CalcRandGrad(i, j + 1));
                    }
                    if (!perlinUnitMap.ContainsKey(index + Vector2Int.right))
                    {
                        perlinUnitMap.Add(index + Vector2Int.right, CalcRandGrad(i + 1, j));
                    }
                    if (!perlinUnitMap.ContainsKey(index + Vector2Int.one))
                    {
                        perlinUnitMap.Add(index + Vector2Int.one, CalcRandGrad(i + 1, j + 1));
                    }

                    int count = 0;

                    for (int m = i * perlinUnit; m < (i + 1) * perlinUnit; m++)
                    {
                        //use n-- to ensure that resources instantiate from top to bottom
                        for (int n = (j + 1) * perlinUnit - 1; n > j * perlinUnit - 1; n--)
                        {
                            perlinMap.Add(new Vector2Int(m, n), CalcPerlinMapValue(m, n));
                            //tilesManager.SetTerrainTile(m, n);
                            //InitResource(m, n);

                            //run 500 times per frame
                            if (count % 500 == 0)
                            {
                                yield return null;
                            }
                            count++;
                        }
                    }

                    perlinBoolMap[index] = true;
                    
                }
            }
        }
        isUpdating = false;
        //Debug.Log("Coroutine End");
    }

    //instantiate resource prefabs
    public void InitResource(int x, int y)
    {

        float perlinNoise = perlinMap[new Vector2Int(x, y)];
        //float perlinNoiseL = perlinMap[new Vector2Int(x - 1, y)];
        //float perlinNoiseR = perlinMap[new Vector2Int(x + 1, y)];
        //float perlinNoiseB = perlinMap[new Vector2Int(x, y - 1)];
        //float perlinNoiseT = perlinMap[new Vector2Int(x, y + 1)];

        foreach (terrainProp item in terrainPropList)
        {
            if (perlinNoise > item.min && perlinNoise < item.max)
            {
                float treeRand = Random.Range(0f, 1f);
                float stoneRand = Random.Range(0f, 1f);
                float yTreeRand = Random.Range(0f, 1f);
                float slimeRand = Random.Range(0f, 1f);

                if (item.smooth == 0)
                {
                    tilesManager.SetTileCollider(x, y);
                }

                if (treeRand < item.treeProb)
                {
                    Transform tree = Instantiate(treePrefab[0], new Vector3(x * tilesManager.cellSize, y * tilesManager.cellSize, 0), Quaternion.identity);
                    tree.SetParent(resourcesParent);
                    tree.gameObject.GetComponent<SpriteRenderer>().sortingOrder = -y;
                    resourcesMap.Add(new Vector2Int(x, y), tree);
                }
                else if (stoneRand < item.stoneProb)
                {
                    int stoneIndex = Mathf.FloorToInt(Random.Range(0, stonePrefab.Length));
                    Transform stone = Instantiate(stonePrefab[stoneIndex], new Vector3(x * tilesManager.cellSize, y * tilesManager.cellSize, 0), Quaternion.identity);
                    stone.SetParent(resourcesParent);
                    stone.gameObject.GetComponent<SpriteRenderer>().sortingOrder = -y;
                    resourcesMap.Add(new Vector2Int(x, y), stone);
                }
                else if (yTreeRand < item.yTreeProb)
                {
                    Transform yTree = Instantiate(yTreePrefab[0], new Vector3(x * tilesManager.cellSize, y * tilesManager.cellSize, 0), Quaternion.identity);
                    yTree.SetParent(resourcesParent);
                    yTree.gameObject.GetComponent<SpriteRenderer>().sortingOrder = -y;
                    resourcesMap.Add(new Vector2Int(x, y), yTree);
                }
                else if (slimeRand < item.slimeProb)
                {
                    Transform slime = Instantiate(slimePrefab[0], new Vector3(x * tilesManager.cellSize, y * tilesManager.cellSize, 0), Quaternion.identity);
                    slime.SetParent(resourcesParent);
                    slime.gameObject.GetComponent<SpriteRenderer>().sortingOrder = -y;
                    resourcesMap.Add(new Vector2Int(x, y), slime);
                }
            }
        }

    }

    public Color GetColorByCoord(int x, int y)
    {
        float perlinNoise = 0.7f * perlinMap[new Vector2Int(x, y)] + 0.2f * perlinMap[new Vector2Int(((x + 17) * 4) % xInitEdge, ((y + 17) * 4) % yInitEdge)] + 0.1f * perlinMap[new Vector2Int(((x + 19) * 8) % xInitEdge, ((y + 19) * 8) % yInitEdge)];
        //float perlinNoise = 0.8f * perlinMap[new Vector2Int(x, y)] + 0.2f * perlinMap[new Vector2Int(((x + 17) * 4) % xInitEdge, ((y + 17) * 4) % yInitEdge)];
        //float perlinNoise = perlinMap[new Vector2Int(x, y)];

        //return new Color(perlinNoise, perlinNoise, perlinNoise);
        return colorGradient.GetPixelBilinear(perlinNoise, 0.5f);
    }

    public int GetTileIndexByCoord(int x, int y)
    {

        if (!perlinMap.ContainsKey(new Vector2Int(x, y)))
        {
            return -1;
        }
        //float perlinNoise = 0.75f * perlinMap[new Vector2Int(x, y)] + 0.2f * perlinMap[new Vector2Int(((x + 17) * 4) % xInitEdge, ((y + 17) * 4) % yInitEdge)] + 0.05f * perlinMap[new Vector2Int(((x + 19) * 8) % xInitEdge, ((y + 19) * 8) % yInitEdge)];
        //float perlinNoise = 0.8f * perlinMap[new Vector2Int(x, y)] + 0.2f * perlinMap[new Vector2Int(((x + 17) * 4) % xInitEdge, ((y + 17) * 4) % yInitEdge)];
        float perlinNoise = perlinMap[new Vector2Int(x, y)];
        int tileIndex = 0;

        foreach (terrainProp item in terrainPropList)
        {
            if (perlinNoise >= item.min && perlinNoise < item.max)
            {
                tileIndex = item.tileIndex;
            }
        }
        return tileIndex;
    }

    public Transform GetResourceByCoord(int x, int y)
    {
        if (!resourcesMap.ContainsKey(new Vector2Int(x, y)))
        {
            return null;
        }
        return resourcesMap[new Vector2Int(x, y)];
    }

}
