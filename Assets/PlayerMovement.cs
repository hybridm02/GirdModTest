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
    public int moveCount;

    public TextMeshPro moveCountText;

    // Start is called before the first frame update
    void Start()
    {
        gridMaker = GameObject.Find("GridMaker").GetComponent<GridMaker>();
        tileScript = GameObject.Find("Tile").GetComponent<Tiles>();

        moveCount = 6;
        moveCountText.text = "" + moveCount;
    }

    // Update is called once per frame
    void Update()
    {
        moveCountText.text = "" + moveCount;
        Debug.Log("moveCount" + moveCount);


        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W) && !gridMaker.stopTile)
        {

        }

        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A) && !gridMaker.stopTile)
        {
            Swap(1, 0);
            if (gridMaker.HasMatched())
            {
                //moveCount = 6;
            }
            else
            {
                //moveCount--;
            }
        }

        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D) && !gridMaker.stopTile)
        {
            Swap(-1, 0);
            if (gridMaker.HasMatched())
            {
                //moveCount = 6;
            }
            else
            {
                //moveCount--;
            }
        }

        if (moveCount == 0){
            //lose, change scene
            //GameManager.Instance.GameOver();
            SceneManager.LoadScene(1);
        }
    }

    void Swap (int x, int y){

        Vector2 oldLocation = new Vector2(playerPosition.x, playerPosition.y);
        Vector2 newLocation = new Vector2(playerPosition.x + x, playerPosition.y + y); 

        if (newLocation.x < GridMaker.WIDTH && newLocation.x >= 0){
            //&& newLocation.y < GridMaker.HEIGHT && newLocation.y >= 0){

            GameObject swappedTile = gridMaker.tiles[(int)newLocation.x, (int)newLocation.y];
            Vector2 swapPosition = swappedTile.transform.localPosition;

            swappedTile.transform.position = transform.position;
            transform.localPosition = swapPosition;

            gridMaker.tiles[(int)oldLocation.x, (int)oldLocation.y] = swappedTile;
            gridMaker.tiles[(int)newLocation.x, (int)newLocation.y] = gameObject;

            playerPosition = newLocation;
        }
    }

    void MoveCountRestore(){
        moveCount = 6;
    }
}
