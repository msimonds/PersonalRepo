using UnityEngine;
using System.Collections;

public class Marble : MonoBehaviour {

    
    MarbleModel model;
    MarbleManager manager;
    public float speed;    
    int health;
    Tile curTile;
    Tile targetTile;
    Tile prevTile;
    bool offBoard;
    int turboTime;    //the time the marble has a boost after being clicked
    float rote;  
    int direction; //the direction the marble is facing (0,1,2,3 == NESW)

    //needs to call updateMarbleList when entering and leaving a tile


	// Use this for initialization
	void Start () {
       
    }

    public void init(int dir, Tile start, MarbleManager manager, int health)
    {
        GameObject modelObject;
        this.manager = manager;
        curTile = start;
        this.health = health;
        modelObject = GameObject.CreatePrimitive(PrimitiveType.Quad);
        model = modelObject.AddComponent<MarbleModel>();
        model.init(this);       
        direction = dir;
        rotateMarble(dir * -90);
        targetTile = start;
        speed = 1;
        offBoard = false;
        turboTime = 0;
    }

    void rotateMarble(float angle)
    {
        //rotates angle degrees in this direction: <-
        model.rotate(angle);
    }

    void updateTarget()
    {
        //get current tile, look at the neighbor corresponding to the inward direction, set the target correspondingly
        int outDir= curTile.getOutDir(direction);
        targetTile = findTarget(outDir, curTile);       
        direction = outDir;

        if(targetTile.getType()==2 && curTile.getType()!=2)
        {
            //set a new marble on the wall tile at the other edge of the board 
            Tile rebirth = findTarget(direction, targetTile);
            manager.placeMarble(rebirth.gameObject, direction, health, speed);
            offBoard = true;  
        }

        rote = 90f / ((Mathf.Abs(transform.position.x - targetTile.transform.position.x)
           + Mathf.Abs(transform.position.y - targetTile.transform.position.y)) / (Time.deltaTime * speed));

    }
    //Finds the target tile for the corresponding neighbor
    Tile findTarget(int outDir, Tile t)
    {
        Tile target = null;
        switch (outDir)
        {
            case 0:
                target = t.north.GetComponent<Tile>();
                break;
            case 1:
                target = t.east.GetComponent<Tile>();
                break;
            case 2:
                target = t.south.GetComponent<Tile>();
                break;
            case 3:
                target = t.west.GetComponent<Tile>();
                break;
        }
        return target;
    }   
    
    public void turboHit()
    {
        print("marble down");
        if(turboTime == 0)
        {            
            turboTime = 500;
        }
    } 
	
	// Update is called once per frame
	void FixedUpdate () {

        updateSpeed();
        
        if(targetTile.transform.position == this.transform.position)
        {
            if (offBoard)
            {
                Destroy(this.gameObject);
            }
            else
            {
                curTile.updateMarbleList(this);
                curTile = targetTile;
                updateTarget();//updates the targetTile
                targetTile.updateMarbleList(this);
            }            
        }
        else
        {
            //moveTowards
            transform.position = Vector3.MoveTowards(transform.position, targetTile.transform.position, Time.deltaTime * speed);
            //If there's a curve, rotate accordingly
            if (targetTile.getType() == 1 && targetTile.getOutDir(direction) != direction)
            {
                updateRotation();
            }
        }

    }   

    void updateRotation()
    {
        Vector3 axis = Vector3.back;
        int rotateint = ((direction + 2) % 4) - targetTile.GetComponent<Tile>().getOutDir(direction);


        if (rotateint == 3 || rotateint == -1)
        {

            axis = Vector3.forward;
        }

        //angles/time 90/(deltaTime *transform.position-target.position

        transform.Rotate(axis, rote);
    }

    //If the marble isn't in a refractory period 
    void updateSpeed()
    {
        if (turboTime > 0)
        {
            if (turboTime > 450)
            {
                speed = speed * 1.06f;
            }
            else
            {
                speed = 1;
            }

            turboTime--;
        }
        
    }

    public MarbleManager getOwner()
    {
        return manager;
    }

    public void hitMarble()
    {
        health--;
        if(health < 1)
        {
            Destroy(gameObject);
        }
    }
}
