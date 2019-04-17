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


    // Start is called before the first frame update
    void Start()
    {
        //tileScript = GameObject.Find("Tile").GetComponent<Tiles>();


    }

    // Update is called once per frame
    void Update()
    {
        gridMaker = GameObject.Find("GridMaker").GetComponent<GridMaker>();


        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A) && !gridMaker.stopTile)
        {
            gridMaker.AudioSource.PlayOneShot(gridMaker.arrowDown);
        }
        if (Input.GetKeyUp(KeyCode.LeftArrow) || Input.GetKeyUp(KeyCode.A) && !gridMaker.stopTile)
        {
            Swap(1, 0);
            gridMaker.AudioSource.PlayOneShot(gridMaker.arrowUp);
        }


        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D) && !gridMaker.stopTile)
        {
            gridMaker.AudioSource.PlayOneShot(gridMaker.arrowDown);
        }
        if (Input.GetKeyUp(KeyCode.RightArrow) || Input.GetKeyUp(KeyCode.D) && !gridMaker.stopTile)
        {
            Swap(-1, 0);
            gridMaker.AudioSource.PlayOneShot(gridMaker.arrowUp);
        }


    }

    public void SetPlayerType(int i)
    {
        playerType = i;
        GetComponent<SpriteRenderer>().sprite = playerSprite[playerType];
    }


    void Swap (int x, int y){

        Vector2 oldLocation = new Vector2(playerPosition.x, playerPosition.y);
        Vector2 newLocation = new Vector2(playerPosition.x + x, playerPosition.y + y); 

        if (newLocation.x < GridMaker.WIDTH && newLocation.x >= 0
            && newLocation.y < GridMaker.HEIGHT && newLocation.y >= 0){

            GameObject swappedTile = gridMaker.tiles[(int)newLocation.x, (int)newLocation.y];
            Vector2 swapPosition = swappedTile.transform.localPosition;

            swappedTile.transform.localPosition = transform.localPosition;
            transform.localPosition = swapPosition;

            gridMaker.tiles[(int)oldLocation.x, (int)oldLocation.y] = swappedTile;
            gridMaker.tiles[(int)newLocation.x, (int)newLocation.y] = gameObject;


            playerPosition = newLocation;


            //Debug.Log("PLAYER IN 2D ARRAY POSITION X = " + (int)newLocation.x + " Y = "+ (int)newLocation.y);
            //Debug.Log(playerPosition);
        }
    }


}
