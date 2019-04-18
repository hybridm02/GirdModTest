using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GridMaker : MonoBehaviour
{
    public int score = 0;
    public Text scoreText;

    public const int WIDTH = 7; //overall grid
    public const int HEIGHT = 9;

    public static float slideLerp = -1;
    public float lerpSpeed = 0.25f;

    public float xOffset = WIDTH / 2f - 0.5f; //space between 
    public float yOffset = HEIGHT / 2f - 0.5f;

    GameObject gridHolder; 

    public GameObject[,] tiles; //2d array 
    public GameObject tilePrefab;
    public GameObject playerPrefab;

    public bool isMatch = false;
    public bool stopTile;

    public GameObject destroyEffect; //particle prefab

    public bool inDrop = false;
    //public Vector3 startDropPos;
    //public Vector3 destDropPos;

    public GameObject emptyPrefab;

    //GameObject nextTile;
    GameObject droppedTile;
    Vector2 newDropTilePos;

    Vector2 oldPlayerPos;

    public AudioSource AudioSource;
    [Header("AUDIO CLIPS")]
    public AudioClip arrowDown;
    public AudioClip arrowUp;
    public AudioClip dropTile;
    public AudioClip destroyTile;
    public AudioClip buttonClick;
    public AudioClip alarm;



    // Start is called before the first frame update
    void Start()
    {
        score = 0;
        scoreText.text = "" + score;

        tiles = new GameObject[WIDTH, HEIGHT];

        gridHolder = new GameObject();
        gridHolder.transform.position = new Vector3(-1f, -1f, 0);
       
        for (int x = 0; x < WIDTH; x++)
        {
            for (int y = 0; y < HEIGHT; y++)
            {
                if (x == 3 && y == 0)
                {
                    GameObject player = Instantiate(playerPrefab);
                    player.transform.parent = gridHolder.transform;
                    player.name = "PLAYER ( " + x + ", " + y + " )";
                    player.transform.localPosition = new Vector2(WIDTH - x - xOffset, HEIGHT - y - yOffset);
                    //= new Vector2(WIDTH - x - xOffset, 0);

                    tiles[x, y] = player;

                    PlayerMovement playerScript = playerPrefab.GetComponent<PlayerMovement>();
                    playerScript.SetPlayerType(Random.Range(0, playerScript.playerSprite.Length));

                    Vector2 playerLocation = new Vector2(WIDTH / 2, 0);
                    //Vector2 playerLocation = new Vector2(-1, 0); // goes down to the correct pos
                    //GameObject removedTile = tiles[(int)playerLocation.x, (int)playerLocation.y];

                    player.GetComponent<PlayerMovement>().playerPosition = playerLocation;

                    //Destroy(removedTile);
                }
                else if (x >= 0 && x <= WIDTH && y >= 5 && y <= HEIGHT)
                {
                    GameObject newTile = Instantiate(tilePrefab);

                    //make the new tile's parent the blank grid holder
                    newTile.transform.parent = gridHolder.transform;
                    newTile.name = "( " + x + ", " + y + " )";
                    newTile.transform.localPosition
                           = new Vector2(WIDTH - x - xOffset, HEIGHT - y - yOffset);

                    tiles[x, y] = newTile;

                    Tiles tileScript = newTile.GetComponent<Tiles>();
                    //tileScript.SetSprite(Random.Range(0, tileScript.tileSprites.Length));
                    tileScript.SetType(Random.Range(0, tileScript.tokenSprites.Length));
                }
                else if (x >= 0 && x <= WIDTH && y < 5 && y <= HEIGHT)
                {
                    //Instantiate(gameObject);
                    GameObject emptySpot = Instantiate(emptyPrefab);
                    emptySpot.transform.parent = gridHolder.transform;
                    emptySpot.name = "EMPTY ( " + x + ", " + y + " )";
                    emptySpot.transform.localPosition
                           = new Vector2(WIDTH - x - xOffset, HEIGHT - y - yOffset);

                    tiles[x, y] = emptySpot;
                }
            }
        }

        //check if match at the beginning
        while (HasMatched())
        {
            for (int x = 0; x < WIDTH; x++)
            {
                for (int y = 0; y < HEIGHT; y++)
                {
                    Tiles tileScript = tiles[x, y].GetComponent<Tiles>();

                    if (tileScript != null)
                    {
                        if (x < WIDTH - 2 && tileScript.isMatch(tiles[x + 1, y], tiles[x + 2, y]))
                        {
                            tileScript.SetType(Random.Range(0, tileScript.tokenSprites.Length));
                        }
                        if (y < HEIGHT - 2 && tileScript.isMatch(tiles[x, y + 1], tiles[x, y + 2]))
                        {
                            tileScript.SetType(Random.Range(0, tileScript.tokenSprites.Length));
                        }
                    }
                }
            }
        }

        //Vector2 nextTilePosition = new Vector2(6.0f, 3.0f);
        //nextTile = Instantiate(tilePrefab, nextTilePosition, Quaternion.identity);
    }

    // Update is called once per frame
    void Update()
    {
        PlayerMovement playerScript = GameObject.FindWithTag("Player").GetComponent<PlayerMovement>();
        scoreText.text = "" + score;


        //check if has GameObject emptySpot
        //if null, generate one
        for (int x = 0; x < WIDTH; x++)
        {
            for (int y = 0; y < HEIGHT; y++)
            {
                if (tiles[x, y] == null)
                {
                    GameObject emptySpot = Instantiate(emptyPrefab);
                    emptySpot.transform.parent = gridHolder.transform;
                    emptySpot.name = "EMPTY ( " + x + ", " + y + " )";
                    emptySpot.transform.localPosition 
                             = new Vector2(WIDTH - x - xOffset, HEIGHT - y - yOffset); 

                    tiles[x, y] = emptySpot;

                    Debug.Log("empty tile Instantiated at ( " + x + ", " + y + " )");
                }
                else if (tiles[x, y].CompareTag("Tile") == true && tiles[x, y].CompareTag("Empty") == true)
                {
                    Destroy(GameObject.FindWithTag("Empty"));

                    Debug.Log("empty destroyed at (" + x + ", " + y + " )");
                }
            }
        }


        if (Input.GetKeyUp(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
        {

            PlayerDropDropTile();
            AudioSource.PlayOneShot(dropTile);
            oldPlayerPos = playerScript.playerPosition;

            playerScript.SetPlayerType(Random.Range(0, playerScript.playerSprite.Length));
        }
            

        if (slideLerp < 0 && !Repopulate() && HasMatched()){
            //Debug.Log("MATCH!");
            RemoveMatches();
        } else if (slideLerp >= 0){
            stopTile = true;
            slideLerp += Time.deltaTime / lerpSpeed;

            if (slideLerp >= 1)
            {
                slideLerp = -1;
            }
        } 

        if (slideLerp < 0 && !HasMatched())
        {
            stopTile = false;
        }



        // LERPING !!!! LOL DOESN'T WORK YET
        /*
        startDropPos = droppedTile.transform.localPosition;
        destDropPos = newDropTilePos;

        droppedTile.transform.localPosition = Vector2.Lerp(startDropPos, destDropPos, Time.deltaTime / lerpSpeed);
        */
    }

    //public void CheckEmptySpot()
    //{

    //}

    public Tiles HasMatched(){
        for (int x = 0; x < WIDTH; x++)
        {
            for (int y = 0; y < HEIGHT; y++)
            {
                Tiles tileScript = tiles[x, y].GetComponent<Tiles>();

                if(tileScript != null && !tileScript.isEmptyTile){
                    if(x < WIDTH - 2 && tileScript.isMatch(tiles[x + 1, y], tiles[x + 2, y])){
                        return tileScript;
                    }
                    if(y < HEIGHT - 2 && tileScript.isMatch(tiles[x, y + 1], tiles[x, y + 2])){
                        return tileScript;
                    }
                }
            }
        }
        return null;
    }


    public void RemoveMatches(){
        for (int x = 0; x < WIDTH; x++)
        {
            for (int y= 0; y < HEIGHT; y++)
            {
                Tiles tileScript = tiles[x, y].GetComponent<Tiles>();

                if (tileScript != null && !tileScript.isEmptyTile){
                    if (x < WIDTH - 2 && tileScript.isMatch(tiles[x + 1, y], tiles[x + 2, y]))
                    {
                        //particles 
                        Instantiate(destroyEffect, tiles[x, y].transform.position, Quaternion.identity);
                        Instantiate(destroyEffect, tiles[x + 1, y].transform.position, Quaternion.identity);
                        Instantiate(destroyEffect, tiles[x + 2, y].transform.position, Quaternion.identity);
                       
                        Destroy(tiles[x, y]);
                        Destroy(tiles[x + 1, y]);
                        Destroy(tiles[x + 2, y]);
                        AudioSource.PlayOneShot(destroyTile);
                        score += 3;


                    }
                    if (y < HEIGHT - 2 && tileScript.isMatch(tiles[x, y + 1], tiles[x, y + 2]))
                    {
                        //particles 
                        Instantiate(destroyEffect, tiles[x, y].transform.position, Quaternion.identity);
                        Instantiate(destroyEffect, tiles[x, y + 1].transform.position, Quaternion.identity);
                        Instantiate(destroyEffect, tiles[x, y + 2].transform.position, Quaternion.identity);
                       
                        Destroy(tiles[x, y]);
                        Destroy(tiles[x, y + 1]);
                        Destroy(tiles[x, y + 2]);
                        AudioSource.PlayOneShot(destroyTile);
                        score += 3;

                    }
                }
            }
        }
    }


    //public void PlayerCreateDropTile(){
        //GameObject droppedTile = Instantiate(tilePrefab);

        //for (int x = 0; x < WIDTH; x++)
        //{
            //for (int y = 0; y < HEIGHT; y++)
            //{
                //PlayerMovement playerScript 
                //    = GameObject.FindWithTag("Player").GetComponent<PlayerMovement>();

                //if (playerScript != null)
                //{
                    ////create droppedTile

                    //droppedTile.transform.parent = gridHolder.transform;
                    //droppedTile.name = "DroppedTile ( " + x + ", " + y + " )";

                    //droppedTile.transform.localPosition
                    //           = GameObject.FindWithTag("Player").transform.localPosition;


                    //tiles[x, y] = droppedTile;

                    ////set dropped tile sprite
                    //Tiles tileScript = droppedTile.GetComponent<Tiles>();
                    //SpriteRenderer droppedTileSR = droppedTile.GetComponent<SpriteRenderer>();
                    //SpriteRenderer playerSR = GameObject.FindWithTag("Player").GetComponent<SpriteRenderer>();
                    //droppedTileSR.sprite = playerSR.sprite;

                    //playerScript.playerPosition



                    // GameObject lastTileInY = tiles[(int)droppedTile.transform.localPosition.x, ]

                    //Debug.Log();
                    //lerp
                    //startDropPos = droppedTile.transform.localPosition;
                    //destDropPos = new Vector2(startDropPos.x, HEIGHT - y - yOffset);

                    //droppedTile.transform.localPosition 
                               //= Vector2.Lerp(startDropPos, destDropPos, Time.deltaTime / lerpSpeed);

    //            }



    //        }
    //    }

    //    // set drop tile to new location 

    //}

    public void PlayerDropDropTile()
    {

        bool onlyOnce = true;

        for (int y = 0; y < HEIGHT; y++)
        {
            PlayerMovement playerScript
                    = GameObject.FindWithTag("Player").GetComponent<PlayerMovement>();
            if( onlyOnce == true && tiles[(int)playerScript.playerPosition.x, y].CompareTag("Tile") == true) {
                int newY = y - 1;
                //Debug.Log(tiles[(int)playerScript.playerPosition.x, newY]);

                newDropTilePos = new Vector2(tiles[(int)playerScript.playerPosition.x, newY].transform.localPosition.x,
                                                     tiles[(int)playerScript.playerPosition.x, newY].transform.localPosition.y);

                onlyOnce = false;

                // GameObject droppedTile = Instantiate(tilePrefab);
                droppedTile = Instantiate(tilePrefab);

                droppedTile.transform.parent = gridHolder.transform;
                droppedTile.name = "DroppedTile ( " + playerScript.playerPosition.x + ", " + newY + " )";

                // we set the tile's new position here!!!
                droppedTile.transform.localPosition = newDropTilePos;

                Tiles tileScript = droppedTile.GetComponent<Tiles>();
                SpriteRenderer droppedTileSR = droppedTile.GetComponent<SpriteRenderer>();
                SpriteRenderer playerSR = GameObject.FindWithTag("Player").GetComponent<SpriteRenderer>();
                droppedTileSR.sprite = playerSR.sprite;

                tiles[(int)playerScript.playerPosition.x, newY] = droppedTile;

            }
        }

    }


    public bool Repopulate(){
        //bool repop = false;

        //for (int x = 0; x < WIDTH; x++)
        //{
        //    for (int y = 0; y < HEIGHT; y++)
        //    {
        //        //Tiles tileScript = tiles[x, y].GetComponent<Tiles>();
        //        //PlayerMovement playerScript = playerPrefab.GetComponent<PlayerMovement>();
        //        if (tiles[x, y] == null)
        //        {
        //            repop = true;

        //            if (y == 0) //if its on top
        //            {
        //                tiles[x, 0] = Instantiate(tilePrefab);
        //                Tiles tileScript = tiles[x, 0].GetComponent<Tiles>();

        //                tileScript.SetType(Random.Range(0, tileScript.tokenSprites.Length));
        //                tiles[x, 0].transform.parent = gridHolder.transform;
        //                tiles[x, 0].name = "repopTile ( " + x + ", " + y + " )";
        //                tiles[x, 0].transform.localPosition
        //                           = new Vector2(WIDTH - x - xOffset, HEIGHT - y - yOffset);

        //                Debug.Log("Repopulate");
        //            }
        //            else
        //            {
        //                slideLerp = 0;
        //                tiles[x, y] = tiles[x, y - 1];
        //                Tiles tileScript = tiles[x, y].GetComponent<Tiles>();
        //                if (tileScript != null)
        //                {
        //                    tileScript.SetupSlide(new Vector2(WIDTH - x - xOffset, HEIGHT - y - yOffset));
        //                }
        //                PlayerMovement playerScript = tiles[x, y].GetComponent<PlayerMovement>();
        //                if(playerScript != null)
        //                {
        //                    playerScript.playerPosition.Set(x, y);
        //                }

        //                tiles[x, y - 1] = null;
        //            }
        //        }
        //    }
        //}
        //return repop;
        return false;
    }


    public void GameRestart(){
        SceneManager.LoadScene(0);
    }

    public void GameOver(){
        SceneManager.LoadScene(1);
    }

    public void Quit()
    {
        Application.Quit();
    }



}
