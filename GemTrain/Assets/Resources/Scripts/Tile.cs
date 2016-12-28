using UnityEngine;
using System.Collections;

public class Tile : MonoBehaviour {

    

    public GameObject north; //0
    public GameObject east; //1
    public GameObject south; //2
    public GameObject west;  //3

	// Use this for initialization
	void Awake () {
        north = null;
        east = null;
        south = null;
        west = null;

       
	
	}	

    public GameObject getNeighbor(int direction)
    {//returns the requested Tile neighbor
        
        switch (direction)
        {
            case 0:
                return north;
            case 1:
                return east;
            case 2:
                return south;
            case 3:
                return west;
            default:
                //print("getNeighbor must recieve an int");
                return null;
        }

    }
    public void setNorth(GameObject neighbor)
    {

        north = neighbor;
    }
    public void setEast(GameObject neighbor) {
        east = neighbor;
    }

    public void setSouth(GameObject neighbor) {
        south = neighbor;
    }

    public void setWest(GameObject neighbor)
    {
        west = neighbor;
    }

    public Tile getTile()
    {
        return this;
    }    
        
}
