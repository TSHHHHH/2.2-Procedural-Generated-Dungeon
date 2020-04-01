using System.Collections.Generic;
using UnityEngine;

public class DungeonPiece : MonoBehaviour
{
    public string[] CusTag;

    public int iteration;

    private void Start()
    {
        //Give rooms random color
        if (CusTag[0] == "Room" || CusTag[0] == "Room_Small" || CusTag[0] == "Room_Hexagon" || CusTag[0] == "Room_Medium")
        {
            GetComponent<SpriteRenderer>().color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
        }
    }

    private void Update()
    {
    }

    private void FixedUpdate()
    {
    }

    public Connector[] getExits()
    {
        return GetComponentsInChildren<Connector>();
    }

    //Give different type of room different size
    public void randomSize()
    {
        //using tag to determine different type of room
        if (CusTag[0] == "Room" || CusTag[0] == "Room_Medium")
        {
            transform.localScale = new Vector2(Random.Range(1, 6), Random.Range(1, 6));
        }
        else if (CusTag[0] == "Room_Small" || CusTag[0] == "Room_Hexagon")
        {
            float squareSize = Random.Range(1, 3);
            transform.localScale = new Vector2(squareSize, squareSize);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //the collision will be count only when there is a younger iteration collide with otlder iteration, so they will not keep triggering each other's functions that inside OnCollisionEnter2D
        if (iteration > collision.transform.GetComponent<DungeonPiece>().iteration)
        {
            //get exits from both dungeon piece
            var exits = getExits();
            var colliExit = collision.transform.GetComponent<DungeonPiece>().getExits();

            //add them into 2 different list (I think only use one list should be easier and have better performance)
            List<string> exitsV2 = new List<string>();
            for (int i = 0; i < exits.Length; i++)
            {
                exitsV2.Add(exits[i].transform.position.ToString("f1"));
            }

            List<string> colliExitV2 = new List<string>();
            for (int a = 0; a < colliExit.Length; a++)
            {
                colliExitV2.Add(colliExit[a].transform.position.ToString("f1"));
            }

            //compare items in each list,they are connect with eachother corretly if there is any same value detected
            foreach (var item in exitsV2)
            {
                if (colliExitV2.Contains(item))
                {
                    //make the function will not continuo to run
                    return;
                }
            }

            //destroye the younger iteration if there is no same value
            Destroy(transform.gameObject);
        }// there is chance when 2 dungeon piece with same iteration colldie with each other, I take the easist way which is destroy them both
        else if (iteration == collision.transform.GetComponent<DungeonPiece>().iteration)
        {
            Destroy(transform.gameObject);
        }
    }
}