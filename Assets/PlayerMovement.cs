using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    public Vector2 playerPosition;
    public GridMaker gridMaker;
    public Tiles tileScript;

    public int playerType;
    public Sprite[] playerSprite;

    public int x;
    public int y;


    // Start is called before the first frame update
    void Start()
    {
        gridMaker = GameObject.Find("GridMaker").GetComponent<GridMaker>();
        //tileScript = GameObject.Find("Tile").GetComponent<Tiles>();


    }

    // Update is called once per frame
    void Update()
    {
        for (int x = 0; x < GridMaker.WIDTH; x++)
        {
            for (int y = 0; y < GridMaker.HEIGHT; y++)
            {
                if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D)
                   && !gridMaker.stopTile)
                {
                    transform.position = new Vector2(playerPosition.x + 1, playerPosition.y);
                    gridMaker.AudioSource.PlayOneShot(gridMaker.click);

                }
                if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A)
                   && !gridMaker.stopTile)
                {
                    transform.position = new Vector2(playerPosition.x - 1, playerPosition.y);
                    gridMaker.AudioSource.PlayOneShot(gridMaker.click);
                }

            }
        }




        //if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A) && !gridMaker.stopTile)
        //{
        //    Swap(1, 0);
        //    if (gridMaker.HasMatched())
        //    {
        //        //moveCount = 6;
        //    }
        //    else
        //    {
        //        //moveCount--;
        //    }
        //}

        //if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D) && !gridMaker.stopTile)
        //{
        //    Swap(-1, 0);
        //    if (gridMaker.HasMatched())
        //    {
        //        //moveCount = 6;
        //    }
        //    else
        //    {
        //        //moveCount--;
        //    }
        //}


    }

    public void SetPlayerType(int i)
    {
        playerType = i;
        GetComponent<SpriteRenderer>().sprite = playerSprite[playerType];
    }


    //void Swap (int x, int y){

    //    Vector2 oldLocation = new Vector2(playerPosition.x, playerPosition.y);
    //    Vector2 newLocation = new Vector2(playerPosition.x + x, playerPosition.y); 

    //    if (newLocation.x < GridMaker.WIDTH && newLocation.x >= 0){
    //        //&& newLocation.y < GridMaker.HEIGHT && newLocation.y >= 0){

    //        GameObject swappedTile = gridMaker.tiles[(int)newLocation.x, (int)newLocation.y];
    //        Vector2 swapPosition = swappedTile.transform.localPosition;

    //        swappedTile.transform.position = transform.position;
    //        transform.localPosition = swapPosition;

    //        gridMaker.tiles[(int)oldLocation.x, (int)oldLocation.y] = swappedTile;
    //        gridMaker.tiles[(int)newLocation.x, (int)newLocation.y] = gameObject;

    //        playerPosition = newLocation;
    //    }
    //}


}
