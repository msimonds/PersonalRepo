using UnityEngine;
using System.Collections;

public class Bomb : MonoBehaviour {

    BombModel model;
    MarbleManager manager;
    public float speed;
    Tile curTile;
    Tile targetTile;
    Tile prevTile;
    bool offBoard;
    float rote;
    int direction;

    // Use this for initialization
    public void init (int dir, Tile start, MarbleManager m) {
        manager = m;
        speed = 1;
        curTile = start;
     
        GameObject modelObject;
        modelObject = GameObject.CreatePrimitive(PrimitiveType.Quad);
        model = modelObject.AddComponent<BombModel>();
        model.init(this);
        direction = dir;        
        targetTile = start;
    }

    void updateTarget()
    {
        //get current tile, look at the neighbor corresponding to the inward direction, set the target correspondingly
        int outDir = curTile.getOutDir(direction);
        targetTile = findTarget(outDir, curTile);
        direction = outDir;

        if (targetTile.getType() == 2 && curTile.getType() != 2)
        {
            offBoard = true;
        }
         
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
    // Update is called once per frame
    void FixedUpdate()
    {

        if (targetTile.transform.position == this.transform.position)
        {
            if (offBoard)
            {
                Destroy(this.gameObject);
            }
            else
            {
                
                curTile = targetTile;
                updateTarget();//updates the targetTile
               
            }
        }
        else
        {
            //moveTowards
            transform.position = Vector3.MoveTowards(transform.position, targetTile.transform.position, Time.deltaTime * speed);
           
        }

    }


    public MarbleManager getOwner()
    {
        return manager;
    }
}
