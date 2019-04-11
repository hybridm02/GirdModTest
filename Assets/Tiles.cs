using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tiles : MonoBehaviour
{
    public int type;
    //public Color[] tokenColors;
    public Sprite[] tokenSprites; //SET IT TO 66666666666

    public Vector3 startPosition;
    public Vector3 destPosition;

    public bool inSlide = false;
    public bool match = false;

    //public GridMaker gridMaker;
    void Start()
    {
        if (GetComponent<PlayerMovement>())
        {
            type = -1;
        }
        //gridMaker = GameObject.Find("GameManager").GetComponent<GridMaker>();
    }

    void Update()
    {
        if (inSlide)
        {
            if (GridMaker.slideLerp < 0)
            {
                transform.localPosition = destPosition;
                inSlide = false;
            } else {
                transform.localPosition = Vector3.Lerp(startPosition, destPosition, GridMaker.slideLerp);

            }
        }
    }

    public void SetType(int i)
    {
        type = i;
        //GetComponent<SpriteRenderer>().color = tokenColors[type];
        GetComponent<SpriteRenderer>().sprite = tokenSprites[type];
    }

    public bool isMatch(GameObject gameObject1, GameObject gameObject2)
    {
        Tiles ts1 = gameObject1.GetComponent<Tiles>();
        Tiles ts2 = gameObject2.GetComponent<Tiles>();
        return ts1 != null && ts2 != null && type == ts1.type && type == ts2.type;


    }

    public void SetupSlide(Vector2 newDestPos)
    {
        inSlide = true;
        startPosition = transform.localPosition;
        destPosition = newDestPos;
    }




}
