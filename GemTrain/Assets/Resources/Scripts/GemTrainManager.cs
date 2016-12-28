using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

using System;

public class GemTrainManager : MonoBehaviour {

   
        
    
    GameObject boardFolder;
    GameObject[,] board;
    List<GameObject> switchList;
    List<GameObject> trainList;
    GameObject[] trackArray;
    int Score;
    Text scoreText;
    int gemType;
    int totalGems;
    
   
    int NORTH = 0;
    int EAST = 1;
    int SOUTH = 2;
    int WEST = 3;
    int rows;
    int cols;

    // Use this for initialization
    void Start()
    {
        //ahaha this code is so bad
        float x = 0; //xy coord variables to lay out the track
        float y = 0;
        Score = 0;
        gemType = 1;

       
       
        boardFolder = new GameObject();
        boardFolder.name = "gameBoard";
        switchList = new List<GameObject>();
        List<GameObject> trackList=new List<GameObject>();

        GameObject tileObj;
        GameObject startTile;        
        
        System.IO.StreamReader file = new System.IO.StreamReader(@"C:\Users\Michael\Documents\CS_stuff\GameDesign\GemDemo\Assets\Resources\boardSetup.txt");
        String line = file.ReadLine();
        String[] temp = line.Split(' ');
        rows = Int32.Parse(temp[0]);
        cols = Int32.Parse(temp[1]);
        board = new GameObject[rows, cols];

        for (int i = 0; i <rows ; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                line = file.ReadLine();
                if (line.Length == 1) {//need to add in a check for the length of the string
                    //get the # and return array of tiles
                    tileObj = setupTile(Int32.Parse(line));
                    if (Int32.Parse(line) < 6)
                    {
                        trackList.Add(tileObj);
                    }
                }
                else {
                    //read in line formatted as [<neighbor> <alt1> <alt2>]
                    String[]arr= line.Split(' ');
                    tileObj = setupTile(7);
                    Switch s = tileObj.GetComponent<Switch>();
                    s.init(Int32.Parse(arr[0]), Int32.Parse(arr[1]), Int32.Parse(arr[2]));
                    switchList.Add(tileObj);
                   
                }
                tileObj.GetComponent<Transform>().position = new Vector3(x, y, 0);
                tileObj.transform.parent = boardFolder.transform;
                board[i, j] = tileObj;
                x += (float) 2.56;
            }
            x = 0;
            y -=(float) 2.56;
        }

        //CAN and SHOULD delete these loops and just do the stuff above by passing in a reference to the board array slots

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                setNeighbors(i, j);
                
                print("Neighbors: " + board[i,j].GetComponent<Tile>().north + " "+
                    board[i,j].GetComponent<Tile>().east + " "+
                    board[i,j].GetComponent<Tile>().south + " "+
                    board[i,j].GetComponent<Tile>().west);                 
            }
        }

        /*
        GameObject test = board[0, 0].GetComponent<Tile>().east
            .GetComponent<Tile>().east
            .GetComponent<Tile>().east;
        print("IT'S FUCKING THERE WHY ISNT IT SHOWING UP? "+test); */

        setSwitches(switchList);
        trackArray = trackList.ToArray();

        startTile = board[1,0];
        GameObject train = Instantiate(Resources.Load("Prefabs/trainFab") as GameObject);
        train.GetComponent<Train>().init(startTile, EAST);
       GameObject train2 = Instantiate(Resources.Load("Prefabs/trainFab") as GameObject);
        GameObject train3 = Instantiate(Resources.Load("Prefabs/trainFab") as GameObject);
        train2.GetComponent<Train>().init(board[10, 0], 1);
        train3.GetComponent<Train>().init(board[5, 9], 3);
        trainList.Add(train);
        trainList.Add(train2);
        trainList.Add(train3);

        
        //Ahaha so spaghetti

        scoreText = gameObject.AddComponent<Text>();

        //set the tile that will act as a trigger to change the start tiles to not start tiles
        BoxCollider2D trigger = board[9,2].AddComponent<BoxCollider2D>();
        board[9, 2].AddComponent<Trigger>();
        trigger.isTrigger = true;

        scoreText = GameObject.Find("Text").GetComponent<Text>();

        
    }           

    private GameObject setupTile(int type)
    {        
        //if you have time delete these switch statements and load a dictionary with the (type,prefab gameobject) as key-val
        switch (type)
        {
            case 0:
                return Instantiate(Resources.Load("Prefabs/straightTrack") as GameObject);
            case 1:
                return Instantiate(Resources.Load("Prefabs/sideTrack") as GameObject);
            case 2:
                return Instantiate(Resources.Load("Prefabs/northeastTrack") as GameObject);
            case 3:
                return Instantiate(Resources.Load("Prefabs/northwestTrack") as GameObject);
            case 4:
                return Instantiate(Resources.Load("Prefabs/southeastTrack") as GameObject);
            case 5:
                return Instantiate(Resources.Load("Prefabs/southwestTrack") as GameObject);
            case 6:
                return Instantiate(Resources.Load("Prefabs/grassTile") as GameObject);
            case 7:
                return Instantiate(Resources.Load("Prefabs/lever") as GameObject);
            default:
                return null;
        }
        
    }

     

    private void setSwitches(List<GameObject> tl)
    {
        foreach(GameObject s in tl){
            s.GetComponent<Switch>().finishSetup();
        }
        
    }

	
	void Update () {

        //update the score

        scoreText.text = "Score: " + Score.ToString();
        //if it's time pick a random track and make a gem pop up on it
        
        if ((Time.frameCount % 500 == 0) && (trackArray.Length != totalGems))
        {
            System.Random rand = new System.Random();
            int ind;
            do
            {
                ind = rand.Next(trackArray.Length);
            } while (trackArray[ind].GetComponent<Track>().doesHaveGem());

            addGem(ind);
        }     
	}

    public void hitGem()
    {
        Score += 20;
    }

    //takes in the index of the trackArray
    private void addGem(int ind)
    {
        GameObject gemObject = new GameObject();            // Create a new empty game object that will hold a gem.
        Gem gem = gemObject.AddComponent<Gem>();            // Add the Gem.cs script to the object.
        GameObject track = trackArray[ind];                                              // Set the gem's parent object to be the gem folder.
        gem.transform.position = track.transform.position;      // Position the gem at x,y.	
        gem.name = "Gem";
        gem.init(gemType, this);                            // Initialize the gem script.        
        gemObject.AddComponent<BoxCollider2D>();
        gemObject.tag = "gem";
        Rigidbody2D gemBody = gemObject.AddComponent<Rigidbody2D>();
        gemBody.gravityScale = 0;
        gemType = (gemType % 4) + 1;
        totalGems += 1;
        track.GetComponent<Track>().setHasGem(true);
    }  
    
    public void triggerHit()
    {
        //set start tiles to something different
        remake(board[1, 0], 6, 1,0);
        remake(board[1, 1], 4, 1,1);
        remake(board[5, 8], 0,5,8);
        print("trigger hit");
        //remake(board[5, 9], 6);
        Vector2 x = board[5, 9].transform.position;
        board[5, 9] = setupTile(7);
        board[5, 9].transform.position = x;
        setNeighbors(5, 9);
        Switch s = board[5, 9].GetComponent<Switch>();
        s.init(3, 0, 5);
        s.finishSetup();
        remake(board[10, 2], 2, 10,2);
        remake(board[10, 1], 6,10,1);
        remake(board[10, 0], 6,10,0);
    }
    
    private void remake(GameObject obj, int type, int i, int j)
    {
        Vector2 pos = obj.transform.position;
        if(type == 6)
        {
            obj =setupTile(6);
        } else if(type== 2)
        {
            //reset sprite, fix track outs
            Texture2D txtr = Resources.Load("Textures/northeastTrack") as Texture2D;
            obj.GetComponent<SpriteRenderer>().sprite = Sprite.Create(txtr, new Rect(0, 0, txtr.width, txtr.height), new Vector2(0.5f, 0.5f), 100);
            obj.GetComponent<Track>().init(0, 1);

        }
        else if (type == 4){
            Texture2D txtr = Resources.Load("Textures/southeastTrack") as Texture2D;
            obj.GetComponent<SpriteRenderer>().sprite = Sprite.Create(txtr, new Rect(0, 0, txtr.width, txtr.height), new Vector2(0.5f, 0.5f), 100);
            obj.GetComponent<Track>().init(2,1);
        } else if(type == 0)
        {
            Texture2D txtr = Resources.Load("Textures/straightTracks") as Texture2D;
            obj.GetComponent<SpriteRenderer>().sprite = Sprite.Create(txtr, new Rect(0, 0, txtr.width, txtr.height), new Vector2(0.5f, 0.5f), 100);
            obj.GetComponent<Track>().init(0, 2);
        }
        obj.transform.position = pos;
        setNeighbors(i, j);
        
    }

    private void setNeighbors(int i, int j)
    {
        if (i == 0)
        { //we're on the first row, no North neighbors, set S

            board[i, j].GetComponent<Tile>().setSouth(board[i + 1, j]);

        }
        else if (i == rows - 1)//we're at the last row, no South neighbors
        {
            board[i, j].GetComponent<Tile>().setNorth(board[i - 1, j]);
        }
        else           //we're in the middle rows 
        {
            board[i, j].GetComponent<Tile>().setNorth(board[i - 1, j]);
            board[i, j].GetComponent<Tile>().setSouth(board[i + 1, j]);
        }

        if (j == 0)
        {//at first column, no W 
            board[i, j].GetComponent<Tile>().setEast(board[i, j + 1]);

        }
        else if (j == cols - 1)   //at last column, no E
        {
            board[i, j].GetComponent<Tile>().setWest(board[i, j - 1]);
        }
        else
        {//we're in the middle, set E and W

            board[i, j].GetComponent<Tile>().setEast(board[i, j + 1]);
            board[i, j].GetComponent<Tile>().setWest(board[i, j - 1]);

        }
    }
    
}
