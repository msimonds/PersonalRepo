using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class MarbleManager : MonoBehaviour {
   
    public int rows;
    public int cols;
    public int marbleHealth;
    GameObject marbsFolder;   // This will be an empty game object used for organizing objects in the Hierarchy pane.
    List<Marble> marbsList;         // This list will hold the gem objects that are created.
    List<Tile> Board;
    List<Gem> gemList;
    GameObject[,] boardArray;
    Text scoreText;
    int totalScore;
    int gemType;
 

    //TODO: reduce size of collissions, fix corner switch bug, sound to switches
    // Use this for initialization
    void Start () {
        this.gameObject.name = "MarbleManager";
        marbsFolder = new GameObject();
        marbsFolder.name = "MarbleFolder";
        marbsList = new List<Marble>();
        gemList = new List<Gem>();
        Board = new List<Tile>();
        boardArray = new GameObject[rows, cols];
        setupBoard();
        totalScore = 0;
        gemType = 1;  
        
    }	
	
    void setupBoard()
    {
        //types of tiles {0=empty, 1=turn tile}
        float posx = 0;
        float posy = 0;        

       System.Random rand = new System.Random();
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                int type = rand.Next(100);
                //pick a # btwn 1-100, if <15 choose turn, >=15 choose empty
                if (i == 0 || j == 0 || i==rows-1 || j==cols-1)
                {
                    type = 101;
                }
                makeTile(type, posx, posy, i, j);
                posx += 1f;
            }
            posx = 0;
            posy -= 1f;
        }

        buildNeighbors();
        Tile startTile=null;
        do
        {
            startTile = randomTileGen();
        } while (startTile.getType() == 2);

        //sets a marble on a random tile
        int dir;
        System.Random randy = new System.Random();
        dir = randy.Next(3);

        placeMarble(startTile.gameObject,dir, marbleHealth,1);
        placeMarble(boardArray[0, 1], 2, marbleHealth,1);
        placeMarble(boardArray[rows - 1, cols - 2], 0, marbleHealth,1);

        //Set up text score
        scoreText = gameObject.AddComponent<Text>();
        scoreText = GameObject.Find("Text").GetComponent<Text>();
        
         
    }

    GameObject findStart()
    {
        System.Random rand = new System.Random();
        int r = rand.Next(Board.Count);

        foreach (Tile t in Board)
        {
            if (r == 0)
            {
                return t.gameObject;
                
            }
            r--;
        }
        return null;

    }

    public void placeMarble(GameObject tile, int dir, int h, float speed)
    {
        //instantiate a marble object, place it in right position on tile, face it in any direction, add it to the marbles list
        
        GameObject marbObject = new GameObject();
        Marble marb = marbObject.AddComponent<Marble>();
        marbObject.transform.position = tile.transform.position;

        marb.init(dir,tile.GetComponent<Tile>(), this, h);
    }

    void makeBomb()
    {
        GameObject bombObject = new GameObject();
        Bomb bomb = bombObject.AddComponent<Bomb>();
        Tile startTile = null;
        do
        {
            startTile = randomTileGen();
        } while (startTile.getType() != 2);

        float x = startTile.getBoardPosition().x;
        float y = startTile.getBoardPosition().y;
        int dir;
        if(x == 0)
        {
            dir = 2; 
        } else if(x == rows - 1)
        {
            dir = 0;
        } else if (y == 0)
        {
            dir = 1;
        }
        else
        {
            dir = 3;
        }

        bomb.init(dir, startTile, this);
                    
    }

    void buildNeighbors()
    {
        for(int i=0; i< rows; i++)
        {
            for(int j=0; j < cols; j++)
            {
                setNeighbors(i, j);
                print("Type: "+ boardArray[i,j].GetComponent<Tile>().getType()+" Neighbors: " + boardArray[i, j].GetComponent<Tile>().north + " " +
                  boardArray[i, j].GetComponent<Tile>().east + " " +
                  boardArray[i, j].GetComponent<Tile>().south + " " +
                  boardArray[i, j].GetComponent<Tile>().west);
            }
        }
    }

    private void setNeighbors(int i, int j)
    {
        if (i == 0)
        { 
            boardArray[i, j].GetComponent<Tile>().setNorth(boardArray[rows-1, j]);
            boardArray[i, j].GetComponent<Tile>().setSouth(boardArray[i + 1, j]);
        }
        else           //we're in the middle rows 
        {
            boardArray[i, j].GetComponent<Tile>().setNorth(boardArray[i - 1, j]);
            boardArray[i, j].GetComponent<Tile>().setSouth(boardArray[(i + 1)%rows, j]);
        }

        if (j == 0)
        {//at first column, no W 
            boardArray[i, j].GetComponent<Tile>().setEast(boardArray[i, j + 1]);
            boardArray[i, j].GetComponent<Tile>().setWest(boardArray[i, cols-1]);
        }
       
        else
        {//we're in the middle, set E and W

            boardArray[i, j].GetComponent<Tile>().setEast(boardArray[i, (j + 1)%cols]);
            boardArray[i, j].GetComponent<Tile>().setWest(boardArray[i, j - 1]);
        }
    }

    void makeTile(int type, float posx, float posy, int boardx, int by)
    {
        GameObject tileObj = new GameObject();
        
        tileObj.name = "board_" + Board.Count.ToString();
        
        tileObj.transform.position = new Vector3(posx, posy, 0);        
        Tile tile = tileObj.AddComponent<Tile>();
        BoxCollider2D collider = tileObj.AddComponent<BoxCollider2D>();
        AudioSource au = tileObj.AddComponent<AudioSource>();
        au.playOnAwake = false;
        au.clip = Resources.Load<AudioClip>("Audio/switchTrack");


        if (type < 15)
        {           
            tile.init(1,boardx, by);
        }
        else if (type <= 100)
        {
            tile.init(0, boardx, by);
        }
        else
        {
            tile.init(2, boardx, by);
        }

        Board.Add(tile);
        boardArray[boardx, by] = tileObj;
    }

    public void gemCollected(Gem g)
    {
        totalScore += 20;
        gemList.Remove(g);
    }

    void Update()
    {
        scoreText.text = "Score: " + totalScore;

        if ((Time.frameCount % 500 == 0) && (Board.Count-(2*rows + 2*cols) > gemList.Count))
        {
            System.Random rand = new System.Random();
            int i;
            int j;
            do
            {
                i = rand.Next(1,rows-2);
                j = rand.Next(1, cols - 2);
            } while (boardArray[i,j].GetComponent<Tile>().doesHaveGem());

            addGem(boardArray[i, j].GetComponent<Tile>());
        }

        if(Time.frameCount % 750 == 0) {
            makeBomb();
        }
    }

    
    //takes in the tile to add the gem to
    private void addGem(Tile j)
    {
        GameObject gemObject = new GameObject();            // Create a new empty game object that will hold a gem.
        Gem gem = gemObject.AddComponent<Gem>();            // Add the Gem.cs script to the object.
        Tile track = j;                                            // Set the gem's parent object to be the gem folder.
        gemObject.transform.position = track.gameObject.transform.position;      // Position the gem at x,y.	
        //print(gem.transform.position + " track: " + track.transform.position);
        gem.name = "Gem";
        gem.init(gemType, this);                            // Initialize the gem script.        
       
        gemObject.tag = "gem";
       
        gemType = (gemType % 4) + 1;
        gemList.Add(gem);
        track.setGem(true);
    }

   

    public void removeGem(Gem g)
    {
        gemList.Remove(g);
    }

    public Tile randomTileGen()
    {
        System.Random rand = new System.Random();
        int i;
        int j;
        i = rand.Next(rows);
        j = rand.Next(cols);

        return boardArray[i,j].GetComponent<Tile>();
    }


}
