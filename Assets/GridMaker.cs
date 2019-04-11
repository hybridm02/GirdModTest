using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridMaker : MonoBehaviour
{
    public int score = 0;
    public Text scoreText;

    public const int WIDTH = 7; //overall grid
    public const int HEIGHT = 9;

    public static float slideLerp = -1;
    public float lerpSpeed = 0.25f;

    float xOffset = WIDTH / 2f - 0.5f; //space between 
    float yOffset = HEIGHT / 2f - 0.5f;

    public GameObject[,] tiles; //2d array 

    public GameObject tilePrefab;

    //public Tiles tileScript;

    GameObject gridHolder; //like a folder

    public GameObject playerPrefab;

    public bool isMatch = false;
    public bool stopTile;

    public GameObject destroyEffect; //particle prefab
    //public SpriteRenderer tileSprites;

    //[HideInInspector]public bool isMatched = false;

    // Start is called before the first frame update
    void Start()
    {
        score = 0;
        scoreText.text = "Score:" + score;

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
                    player.transform.localPosition
                          //= new Vector2(WIDTH - x - xOffset, 0);
                    = new Vector2(WIDTH - x - xOffset, HEIGHT - y - yOffset);

                    tiles[x, y] = player;

                    Vector2 playerLocation = new Vector2(WIDTH / 2, 0);

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
            }
        }

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
    }

    // Update is called once per frame
    void Update()
    {
        scoreText.text = "Score:" + score;

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

    }

    public Tiles HasMatched(){
        for (int x = 0; x < WIDTH; x++)
        {
            for (int y = 0; y < HEIGHT; y++)
            {
                Tiles tileScript = tiles[x, y].GetComponent<Tiles>();

                if(tileScript != null){
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

                if (tileScript != null){
                    if (x < WIDTH - 2 && tileScript.isMatch(tiles[x + 1, y], tiles[x + 2, y]))
                    {
                        //particles 
                        Instantiate(destroyEffect, tiles[x, y].transform.position, Quaternion.identity);
                        Instantiate(destroyEffect, tiles[x + 1, y].transform.position, Quaternion.identity);
                        Instantiate(destroyEffect, tiles[x + 2, y].transform.position, Quaternion.identity);
                        Destroy(tiles[x, y]);

                        Destroy(tiles[x + 1, y]);

                        Destroy(tiles[x + 2, y]);
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
                        score += 3;

                    }
                }
            }
        }
    }

    public bool Repopulate(){
        bool repop = false;

        for (int x = 0; x < WIDTH; x++)
        {
            for (int y = 0; y < HEIGHT; y++)
            {
                //Tiles tileScript = tiles[x, y].GetComponent<Tiles>();
                //PlayerMovement playerScript = playerPrefab.GetComponent<PlayerMovement>();
                if (tiles[x, y] == null)
                {
                    repop = true;

                    if (y == 0) //if its on top
                    {
                        tiles[x, 0] = Instantiate(tilePrefab);
                        Tiles tileScript = tiles[x, 0].GetComponent<Tiles>();

                        tileScript.SetType(Random.Range(0, tileScript.tokenSprites.Length));
                        tiles[x, 0].transform.parent = gridHolder.transform;
                        tiles[x, 0].name = "repopTile ( " + x + ", " + y + " )";
                        tiles[x, 0].transform.localPosition
                                   = new Vector2(WIDTH - x - xOffset, HEIGHT - y - yOffset);

                        Debug.Log("Repopulate");
                    }
                    else
                    {
                        slideLerp = 0;
                        tiles[x, y] = tiles[x, y - 1];
                        Tiles tileScript = tiles[x, y].GetComponent<Tiles>();
                        if (tileScript != null)
                        {
                            tileScript.SetupSlide(new Vector2(WIDTH - x - xOffset, HEIGHT - y - yOffset));
                        }
                        PlayerMovement playerScript = tiles[x, y].GetComponent<PlayerMovement>();
                        if(playerScript != null)
                        {
                            playerScript.playerPosition.Set(x, y);
                        }

                        tiles[x, y - 1] = null;
                    }
                }
            }
        }
        return repop;
    }






        /*
        public void MatchCheck()
        {
            isMatched = false;

            for (int x = 0; x < WIDTH - 2; x++)
            {
                for (int y = 0; y < HEIGHT; y++)
                {
                    if (tiles[x, y].GetComponent<SpriteRenderer>().sprite
                            == tiles[x + 1, y].GetComponent<SpriteRenderer>().sprite
                       && tiles[x, y].GetComponent<SpriteRenderer>().sprite
                            == tiles[x + 2, y].GetComponent<SpriteRenderer>().sprite)
                    {
                        Debug.Log("Horizontal Match 3");
                        isMatched = true;

                        Destroy(tiles[x, y]);
                        Destroy(tiles[x + 1, y]);
                        Destroy(tiles[x + 2, y]);
                        GameManager.Instance.score ++;
                    }
                }
            }

            for (int x = 0; x < WIDTH - 3; x++)
            {
                for (int y = 0; y < HEIGHT; y++)
                {
                    if (tiles[x, y].GetComponent<SpriteRenderer>().sprite
                            == tiles[x + 1, y].GetComponent<SpriteRenderer>().sprite
                       && tiles[x, y].GetComponent<SpriteRenderer>().sprite
                            == tiles[x + 2, y].GetComponent<SpriteRenderer>().sprite
                       && tiles[x, y].GetComponent<SpriteRenderer>().sprite
                            == tiles[x + 3, y].GetComponent<SpriteRenderer>().sprite)
                    {
                        Debug.Log("Horizontal Match 4");
                        isMatched = true;

                        Destroy(tiles[x, y]);
                        Destroy(tiles[x + 1, y]);
                        Destroy(tiles[x + 2, y]);
                        Destroy(tiles[x + 3, y]);
                        GameManager.Instance.score++;
                    }
                }
            }

            for (int x = 0; x < WIDTH - 4; x++)
            {
                for (int y = 0; y < HEIGHT; y++)
                {
                    if (tiles[x, y].GetComponent<SpriteRenderer>().sprite
                            == tiles[x + 1, y].GetComponent<SpriteRenderer>().sprite
                       && tiles[x, y].GetComponent<SpriteRenderer>().sprite
                            == tiles[x + 2, y].GetComponent<SpriteRenderer>().sprite
                       && tiles[x, y].GetComponent<SpriteRenderer>().sprite
                            == tiles[x + 3, y].GetComponent<SpriteRenderer>().sprite
                       && tiles[x, y].GetComponent<SpriteRenderer>().sprite
                            == tiles[x + 4, y].GetComponent<SpriteRenderer>().sprite)
                    {
                        Debug.Log("Horizontal Match 5 WOW");
                        isMatched = true;

                        Destroy(tiles[x, y]);
                        Destroy(tiles[x + 1, y]);
                        Destroy(tiles[x + 2, y]);
                        Destroy(tiles[x + 3, y]);
                        Destroy(tiles[x + 4, y]);
                        GameManager.Instance.score++;
                    }
                }
            }

            for (int x = 0; x < WIDTH; x++)
            {
                for (int y = 0; y < HEIGHT - 2; y++)
                {
                    if (tiles[x, y].GetComponent<SpriteRenderer>().sprite
                            == tiles[x, y + 1].GetComponent<SpriteRenderer>().sprite
                       && tiles[x, y].GetComponent<SpriteRenderer>().sprite
                            == tiles[x, y + 2].GetComponent<SpriteRenderer>().sprite)
                    {
                        Debug.Log("Vertical Match 3");
                        isMatched = true;

                        Destroy(tiles[x, y]);
                        Destroy(tiles[x, y + 1]);
                        Destroy(tiles[x, y + 2]);
                        GameManager.Instance.score++;
                    }
                }
            }
            for (int x = 0; x < WIDTH; x++)
            {
                for (int y = 0; y < HEIGHT - 3; y++)
                {
                    if (tiles[x, y].GetComponent<SpriteRenderer>().sprite
                            == tiles[x, y + 1].GetComponent<SpriteRenderer>().sprite
                       && tiles[x, y].GetComponent<SpriteRenderer>().sprite
                            == tiles[x, y + 2].GetComponent<SpriteRenderer>().sprite
                       && tiles[x, y].GetComponent<SpriteRenderer>().sprite
                            == tiles[x, y + 3].GetComponent<SpriteRenderer>().sprite)
                    {
                        Debug.Log("Vertical Match 4");
                        isMatched = true;

                        Destroy(tiles[x, y]);
                        Destroy(tiles[x, y + 1]);
                        Destroy(tiles[x, y + 2]);
                        Destroy(tiles[x, y + 3]);
                        GameManager.Instance.score++;
                    }
                }
            }
            for (int x = 0; x < WIDTH; x++)
            {
                for (int y = 0; y < HEIGHT - 4; y++)
                {
                    if (tiles[x, y].GetComponent<SpriteRenderer>().sprite
                            == tiles[x, y + 1].GetComponent<SpriteRenderer>().sprite
                       && tiles[x, y].GetComponent<SpriteRenderer>().sprite
                            == tiles[x, y + 2].GetComponent<SpriteRenderer>().sprite
                       && tiles[x, y].GetComponent<SpriteRenderer>().sprite
                            == tiles[x, y + 3].GetComponent<SpriteRenderer>().sprite
                       && tiles[x, y].GetComponent<SpriteRenderer>().sprite
                            == tiles[x, y + 4].GetComponent<SpriteRenderer>().sprite)
                    {
                        Debug.Log("Vertical Match 5");
                        isMatched = true;

                        Destroy(tiles[x, y]);
                        Destroy(tiles[x, y + 1]);
                        Destroy(tiles[x, y + 2]);
                        Destroy(tiles[x, y + 3]);
                        Destroy(tiles[x, y + 4]);
                        GameManager.Instance.score++;
                    }
                }
            }
        }
        */
    }
