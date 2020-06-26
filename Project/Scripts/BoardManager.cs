using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
public class BoardManager : MonoBehaviour
{//lays out the randomly generated levels based on current level number

    [Serializable]
    public class Count
    {
        public int minimum;
        public int maximum;

        public Count(int min, int max)
        {
            minimum = min;
            maximum = max;
        }
    }
    
    //8X8 game board
    public int columns = 8;
    public int rows = 8;
    public Count wallCount = new Count(5, 9); //specify random range of walls to spawn, min of 5, max of 9 per level
    public Count foodCount = new Count(1, 5);//as above but with food
    public GameObject exit;
    public GameObject[] floorTiles;
    public GameObject[] wallTiles;
    public GameObject[] foodTiles;
    public GameObject[] enemyTiles;
    public GameObject[] outerWallTiles;


    private Transform boardHolder;//keeps heiratchy clean 
    private List <Vector3> gridPositions = new List <Vector3> (); //keeps track of the positions of items, determines if the area has item spawned on it 

    void InitialiseList()
    {
        gridPositions.Clear();

        //-1 used to allow the player to appear in a safe spot
        //helps to ensure a passable level
        for (int x = 1; x < columns - 1; x++)
        {
            for (int y = 1; y < rows - 1; y++)
            {
                gridPositions.Add(new Vector3(x, y, 0f));//determines the position of the play grid and items 
            }
        }
    }

    void BoardSetup()//lays out outer wall tiles and floor tile backgrounds 
    {
        boardHolder = new GameObject("Board").transform;

        //builds an edge around the active game board
        for (int x = -1; x < columns + 1; x++)
        {
            for (int y = -1; y < rows + 1; y++)
            {
                GameObject toInstantiate = floorTiles[Random.Range(0, floorTiles.Length)];//random between 0 and length of floor tiles
                if (x == -1 || x == columns || y == -1 || y == rows) //creates the outer wall boarder
                {
                    toInstantiate = outerWallTiles[Random.Range(0, outerWallTiles.Length)];
                }
                //quaternion instaniates with no rotation
                GameObject instance = Instantiate(toInstantiate, new Vector3(x, y, 0f), Quaternion.identity) as GameObject;

                instance.transform.SetParent(boardHolder);
            }
        }
    }

    Vector3 RandomPosition()
    {
        int randomIndex = Random.Range(0, gridPositions.Count);
        Vector3 randomPosition = gridPositions[randomIndex];
        gridPositions.RemoveAt(randomIndex);//ensure that two items dont spawn in the same tile

        return randomPosition;
    }

    void LayoutObjectAtRandom(GameObject[] tileArray, int minimum, int maximum)
    {
        int objectCount = Random.Range(minimum, maximum + 1);

        for( int i = 0; i < objectCount; i++)
        {
            Vector3 randomPosition = RandomPosition();
            GameObject tileChoice = tileArray[Random.Range(0, tileArray.Length)];
            Instantiate(tileChoice, randomPosition, Quaternion.identity);
        }
    }
    //called by game manager to set up the board
    public void SetUpScene(int level)
    {
        BoardSetup();// creates outer walls and floors
        InitialiseList();//reset list of gridpositions

        //Instantiate a random number of wall tiles based on minimum and maximum, at randomized positions.
        LayoutObjectAtRandom(wallTiles, wallCount.minimum, wallCount.maximum);

        //Instantiate a random number of food tiles based on minimum and maximum, at randomized positions.
        LayoutObjectAtRandom(foodTiles, foodCount.minimum, foodCount.maximum);

        //Determine number of enemies based on current level number, based on a logarithmic progression
        int enemyCount = (int)Mathf.Log(level, 2f);

        //Instantiate a random number of enemies based on minimum and maximum, at randomized positions.
        LayoutObjectAtRandom(enemyTiles, enemyCount, enemyCount);

        Instantiate(exit, new Vector3(columns - 1, rows - 1, 0f), Quaternion.identity);//always in upper right corner of the level 
        //keeps the exit in the same place no matter the size of the game board
    }

   
}//end main
