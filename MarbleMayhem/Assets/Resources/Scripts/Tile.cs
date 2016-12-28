using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Tile : MonoBehaviour {

    public GameObject north;
    public GameObject east;
    public GameObject south;
    public GameObject west;
    public AudioClip switchAudio;

    int type; //0=empty, 1=turn;
    int out1;
    int out2;  
    Vector2 boardPosition;
    Dictionary<int, int> getOutward;
    MarbleManager parent;
    TileModel model;
    bool hasGem;
    bool isWall;
    AudioSource source;
    float volLowRange = .5f;
    float volHighRange = 1.0f;
    


    List<Marble> currentMarbles;

    public void init(int type, int boardx, int boardy)
    {
        //determine the type, make the correct quad, add quad to currentMarbles, build the dictionary map whatever thing for outward direction
        this.type = type;
        source = GetComponent<AudioSource>();
        getOutward = new Dictionary<int, int>(); 
        boardPosition = new Vector2(boardx, boardy);
        GameObject modelObject;
        parent = GameObject.Find("MarbleManager").GetComponent<MarbleManager>(); ;
        currentMarbles = new List<Marble>();
        hasGem = false;
        

        if (type == 0)
        {            
            modelObject = GameObject.CreatePrimitive(PrimitiveType.Quad); 
            // Create a quad object for holding the gem texture.
            out1 = 7;
            out2 = out1;                      
        }
        else if (type == 2)
        {
            modelObject = GameObject.CreatePrimitive(PrimitiveType.Quad);
            
            out1 = -1;
            out2 = out1;

        }
        else
        {
            modelObject = new GameObject();
            out1 = 1;
            out2 = 2;   
            //make a new child with two quads and a script for controlling the quads
        }
        //tileModel.init(type, position, owner)
        populateOutward();
        model = modelObject.AddComponent<TileModel>();
        model.init(type, this); 
    }

    // Update is called once per frame
    void Update() {

    }

    public void updateMarble()
    {
        //figure out how to make maps so that I can map the entering direction as a key and the proper outward direction as value
        //if a number matches either of the opposite of the outward directions from the track (ie a northeast track has 
        //outward directions to the north and the east) so we get the opposites, IF these oppossites match the inward direction of the marble then
        //we must apply the turn and direct them in the correct outward direction, ELSE we do not apply a turn and they must continue in the 
        //direction in which they entered. 

    }


    public int getOutDir(int inwardDir)
    {
        //just returns the dictionary value for the inwardDir as a key
       int val;

        getOutward.TryGetValue(inwardDir,out val);
        return val;
    }

    

    void OnMouseDown()
    {       
        //if no marbles on the tile and if the tile is a turn tile then rotateTile should be called
        if(currentMarbles.Count==0 && type == 1)
        {
            source.PlayOneShot(source.clip, 0.7f);
            rotateTile();           
        }
    }

    void rotateTile()
    {
        //update the rotation of the quadmodel, update out1 and out2, update the map that holds the key value for each inward direction 
        model.rotate();
        out1 = (out1 +3) % 4;
        out2 = (out2 +3) % 4;
        populateOutward();
        
    }
    void populateOutward()
    {
        getOutward.Clear();
        //populates dictionary mapping inward direction for the tile to the proper outward direction
        //first assume that we have straight edges, if type==0 then you should modify based on the out directions
        getOutward.Add(0, 0);
        getOutward.Add(1, 1);
        getOutward.Add(2, 2);
        getOutward.Add(3, 3);
        if (type == 1)
        {
            //if the out1=0 out2=1 then .Add(3, 0) .Add(2,1) == .Add(opp(out2),out1) .Add(opp(out1), out2)
            int opp1 = getOppositeDir(out1);
            int opp2 = getOppositeDir(out2);

            getOutward.Remove(opp1);
            getOutward.Remove(opp2);
            getOutward.Add(opp2, out1);
            getOutward.Add(opp1, out2);
        }
    }

    int getOppositeDir(int curDir)
    {
        return (curDir + 2) % 4;
    }

    public Vector2 getBoardPosition()
    {
        return boardPosition;
    }

    //called by a marble whenever it enters a Tile and when leaving
    public void updateMarbleList(Marble marble) 
    {
        if (currentMarbles.Contains(marble))
        {
            currentMarbles.Remove(marble);
        }
        else
        {
            currentMarbles.Add(marble);
        }

    }

    public void setNorth(GameObject t)
    {
        north = t;
    }

    public void setEast(GameObject t)
    {
        east= t;
    }
    public void setSouth(GameObject t)
    {
        south = t;
    }

    public void setWest(GameObject t)
    {
        west = t;
    }

    public int getType()
    {
        return type;
    }

    public bool doesHaveGem()
    {
        return hasGem;
    }

    public void setGem(bool b)
    {
        hasGem = b;
    }
    

}
