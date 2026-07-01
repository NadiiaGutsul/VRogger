
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TileGenerator : MonoBehaviour {

    private GameObject collectiblesTest;

    private List<GameObject> tileList = new List<GameObject>();
    private List<GameObject> tileList2 = new List<GameObject>();
    private List<GameObject> tileList3 = new List<GameObject>();

    private GameObject tree;
    private GameObject gras;
    private GameObject car;
    private GameObject water;
    private GameObject goal;

    private GameObject tilesParent;

    private GameObject player;
    private PlayerMovement playerController;
    private GameObject lastInCurrentList;
    private List<GameObject> currentList;

    private int tileSetsSpawned = 0;

    private bool moveBackEnabled = false;

    private List<GameObject> instantiatedList = new List<GameObject>();
    private List<GameObject> preloadedList = new List<GameObject>();
    
    private List<Transform> gridPositions = new List<Transform>();
    
    private CollectibleSpawner collectibleSpawner;

    private float totalOffset = 0f;
    private float moveBackTarget = 0f;
    
    private void Awake() {
        //lilypad = Resources.Load <GameObject>("Prefabs/Tiles/WaterTile_Lilypad");   // 0
        tree = Resources.Load<GameObject>("Prefabs/Tiles/WaterTile_Tree");          // 1
        car = Resources.Load<GameObject>("Prefabs/Tiles/CarTile");                  // 2
        water = Resources.Load<GameObject>("Prefabs/Tiles/WaterTile");              // 3
        gras = Resources.Load<GameObject>("Prefabs/Tiles/GrasTile");                // 4
        goal = Resources.Load<GameObject>("Prefabs/Tiles/GrasTile_Goal");

        collectibleSpawner = GetComponent<CollectibleSpawner>();
        
        tilesParent = GameObject.FindGameObjectWithTag("TilesParent");
        player = GameObject.FindGameObjectWithTag("PCPlayer");
        playerController = player.GetComponent<PlayerMovement>();


        playerController.goalReached.AddListener((goal) => {         // Need lambda expression becaue of parameter
            if (moveBackEnabled) return; // Prevent multiple triggers
            moveBackTarget = -totalOffset;
            moveBackEnabled = true;
            GenerateTilesInLine(totalOffset);
        });
    }
    
    void Start() {
    
        GenerateTileList();
        currentList = tileList;
        GenerateTilesInLine(0f);
    }

    private void FixedUpdate() {
        MoveTilesBack();
    }


    private void GenerateTilesInLine(float offset) {
       

        ClearLists();

        GenerateTiles(offset);
        
        collectibleSpawner.SpawnCollectibles(gridPositions);

        // Every generation adds (count - 1) * 0.25f to the offset
        // because the first tile is shared with the previous goal
        totalOffset += (currentList.Count - 1) * 0.25f;

        tileSetsSpawned++;
        GameManager.instance.score++;

        ChooseNextTileset();
        PreloadNextTileset(totalOffset);
    }

    private void ChooseNextTileset()
    {
        int tileSetAmount = 3;

        int nextTileSet = Random.Range(0, tileSetAmount); // Decide randomly which tileset gets spawned next

        switch (nextTileSet) {
            case 1:
                currentList = tileList2;
                break;
            case 2:
                currentList = tileList3;
                break;

            default:
                currentList = tileList2;
                break;
        }
    }

    private void PreloadNextTileset(float offset)
    {
        // Skip the first tile (index 0) because it's the goal of the previous set
        for (int i = 1; i < currentList.Count; i++)
        {
            GameObject tile = Instantiate(currentList[i], tilesParent.transform);
            
            // Disable all renderers
            foreach (var renderer in tile.GetComponentsInChildren<Renderer>())
            {
                renderer.enabled = false;
            }

            tile.transform.localPosition = new Vector3(0f, 0f, offset + (i * 0.25f));
            preloadedList.Add(tile);
        }
    }

    private void GenerateTiles(float offset)
    {
        if (preloadedList.Count > 0)
        {
            for (int i = 0; i < preloadedList.Count; i++)
            {
                GameObject tile = preloadedList[i];
                
                // Collect grid positions
                foreach (var childTransform in tile.GetComponentsInChildren<Transform>())
                {
                    if (childTransform.gameObject.name.Equals("SnapCube"))
                    {
                        gridPositions.Add(childTransform);
                    }
                }
                
                instantiatedList.Add(tile);
            }
            preloadedList.Clear();
            return;
        }

        // Fallback for the first time or if no preloaded tiles
        // Skip the first tile (index 0) if it's not the first generation
        int startIndex = (tileSetsSpawned == 0) ? 0 : 1;

        for (int i = startIndex; i < currentList.Count; i++) {

            GameObject tile = Instantiate(currentList[i], tilesParent.transform);

            // Check all instantiated children for SnapCubes and store them in a list to use for collectible spawning
            foreach (var childTransform in tile.GetComponentsInChildren<Transform>())
            {
                if (childTransform.gameObject.name.Equals("SnapCube"))
                {
                    gridPositions.Add(childTransform);
                }
            }
            
            tile.transform.localPosition = new Vector3(0f, 0f, offset + (i * 0.25f));

            instantiatedList.Add(tile);
        }
    }

    private void ClearLists()
    {
        instantiatedList.Clear();
        gridPositions.Clear();
        collectibleSpawner.DestroyCollectibles();
    }
    private void GenerateTileList() {
        tileList.Add(gras);     
        tileList.Add(gras);     
        tileList.Add(water);      
        tileList.Add(car);
        tileList.Add(gras);
        tileList.Add(tree);
        tileList.Add(car);
        tileList.Add(gras);
        tileList.Add(tree);
        tileList.Add(goal);

        tileList2.Add(tree);
        tileList2.Add(water);
        tileList2.Add(gras);
        tileList2.Add(car);
        tileList2.Add(water);
        tileList2.Add(car);
        tileList2.Add(gras);
        tileList2.Add(tree);
        tileList2.Add(goal);

        tileList3.Add(car);
        tileList3.Add(car);
        tileList3.Add(gras);
        tileList3.Add(water);
        tileList3.Add(car);
        tileList3.Add(gras);
        tileList3.Add(tree);
        tileList3.Add(water);
        tileList3.Add(goal);
    }

    private void MoveTilesBack() {
        
        if (moveBackEnabled) {
            
            GameManager.instance.isPaused = true;
            // Determines speed of movement
            tilesParent.transform.localPosition += Vector3.back * (1.5f * Time.deltaTime);

            // Clamp target so it doesn't overshoot
            float targetZ = moveBackTarget;

            if (tilesParent.transform.localPosition.z <= targetZ) {

                Vector3 pos = tilesParent.transform.localPosition;
                pos.z = targetZ;
                tilesParent.transform.localPosition = pos;

                moveBackEnabled = false;
                GameManager.instance.isPaused = false;
            }
        }
    }
}
