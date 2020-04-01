using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
    public DungeonPiece[] DungeonPieces;
    public DungeonPiece StartPiece;

    public int Iteration;

    //Variable to track the current iteration
    private int currentIteration = 0;

    private DungeonPiece startPiece;
    private List<Connector> pendingExits;

    private void Start()
    {
        startPiece = (DungeonPiece)Instantiate(StartPiece, transform.position, transform.rotation);

        pendingExits = new List<Connector>(startPiece.getExits());
    }

    private void Update()
    {
        //As I moved the genration code from start to update, for loop will make the dugeon keep generating, so I create another variable to track current iteration and stop it when it suppose to be over
        if (currentIteration < Iteration)
        {
            var newExits = new List<Connector>();

            foreach (var item in pendingExits)
            {
                //make sure the exit is available as it may be destroyed overlapping detection
                if (item != null)
                {
                    var newTag = GetRandom(item.CusTag);
                    var newDungeonPiecePrefab = GetRandomWithTag(DungeonPieces, newTag);
                    var newDungeonPiece = (DungeonPiece)Instantiate(newDungeonPiecePrefab);

                    markExits(item, newTag, newDungeonPiece);
                    var newDungeonPieceExits = newDungeonPiece.getExits();
                    var exitToMatch = newDungeonPieceExits.FirstOrDefault(x => x.isDefault) ?? GetRandom(newDungeonPieceExits);

                    //to transform the new dungeon piece to correct position
                    MatchExits(item, exitToMatch);

                    //give the new dungeon piece prefab its iteration index for helping overlapping detection
                    newDungeonPiece.iteration = currentIteration + 1;

                    //make sure the new dungeon piece is avilable as it may be destroyed by overlapping detection duting match exits function
                    if (newDungeonPiece != null)
                    {
                        newExits.AddRange(newDungeonPieceExits.Where(e => e != exitToMatch));
                    }
                }
            }

            pendingExits = newExits;
            currentIteration++;
        }
    }

    private static TItem GetRandom<TItem>(TItem[] array)
    {
        return array[Random.Range(0, array.Length)];
    }

    private static DungeonPiece GetRandomWithTag(IEnumerable<DungeonPiece> dungeonPieces, string tagToMatch)
    {
        var matchingPieces = dungeonPieces.Where(m => m.CusTag.Contains(tagToMatch)).ToArray();
        return GetRandom(matchingPieces);
    }

    private void MatchExits(Connector oldExit, Connector newExit)
    {
        var newDungeonPiece = newExit.transform.parent;
        var forwardVectorToMatch = -oldExit.transform.up;
        var correctiveRotation = Azimuth(forwardVectorToMatch) - Azimuth(newExit.transform.up);
        newDungeonPiece.RotateAround(newExit.transform.position, Vector3.forward, correctiveRotation);
        var correctiveTranslation = oldExit.transform.position - newExit.transform.position;
        newDungeonPiece.transform.position += correctiveTranslation;
    }

    private static float Azimuth(Vector3 vector)
    {
        return Vector3.Angle(Vector3.right, vector) * Mathf.Sign(vector.y);
    }

    //make the first corridor from each room's exit red and also handle the different room size
    private void markExits(Connector item, string newTag, DungeonPiece newDungeonPiece)
    {
        string exitParentTag = item.transform.parent.GetComponent<DungeonPiece>().CusTag[0];

        //if current dungeon piece is a corridor and next one is a room, mark the current dungeon piece red
        if (exitParentTag == "Corridor" || exitParentTag == "Corridor_Small")
        {
            if (newTag == "Room" || newTag == "Room_Small" || newTag == "Room_Hexagon" || newTag == "Room_Medium")
            {
                item.transform.parent.GetComponent<SpriteRenderer>().color = Color.red;
                newDungeonPiece.randomSize();
            }
        }//if current dungeon piece is a room and next one is a corridor, mark the corridor red
        else if (exitParentTag == "Room" || exitParentTag == "Room_Small" || exitParentTag == "Room_Hexagon" || exitParentTag == "Room_Medium")
        {
            if (newTag == "Corridor" || newTag == "Corridor_Small")
            {
                newDungeonPiece.transform.gameObject.GetComponent<SpriteRenderer>().color = Color.red;
            }
        }
    }
}