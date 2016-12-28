using UnityEngine;
using System.Collections;

public class Train : MonoBehaviour {

    //fields: curTrack, curDirection(the direction it's facing, to know where to go to next track), velocity(where it is, where it is going, at what speed)
    private GameObject curTrack;
    private GameObject targetTrack;
    private GameObject gtManage;
    int curDirection; //0123 == NESW
    int speed;
    float rote;



    public void init(GameObject start, int dir)
    {       
        transform.position = start.transform.position;        
        this.curTrack = start;
        this.curDirection = dir;
        updateTarget();
        gtManage = GameObject.Find("GemTrainManager");
        speed = 1;    
    }
	
	// Update is called once per frame
	void FixedUpdate () {  
        if (targetTrack.transform.position == transform.position)
        {           
            this.curTrack = targetTrack;
            //print("My pos: " + transform.position + "curEast: " + curTrack.GetComponent<Tile>().getNeighbor(1));
            updateTarget();
            int oppDir = (curDirection + 2) % 4;
            if(targetTrack.GetComponent<Track>().out1 != oppDir && targetTrack.GetComponent<Track>().out2 != oppDir)
            {
                Destroy(this.gameObject);
                //Destroy this train if it goes off rails
            }            
        }
        else
        {
            //update the position
            //print("updating pos: " + transform.position +" targTrackEast " +targetTrack.GetComponent<Tile>().east);
            
            
                transform.position = Vector2.MoveTowards(transform.position, targetTrack.transform.position, Time.deltaTime * speed);
            if (targetTrack.GetComponent<Track>().hasCurve)
            {
                updateRotation();
            }

        }               
	}

    void updateTarget()
    {
        int trackOutDir = curTrack.GetComponent<Track>().getOutDirection(curDirection);        
       // print("curdir: " + curDirection + " trackout: " + trackOutDir +" curtrack: "+ curTrack);
        
        this.curDirection = trackOutDir;
        this.targetTrack = curTrack.GetComponent<Tile>().getNeighbor(trackOutDir);
        rote = 90f / ((Mathf.Abs(transform.position.x - targetTrack.transform.position.x)
           + Mathf.Abs(transform.position.y - targetTrack.transform.position.y)) / Time.deltaTime);
        
        Vector3 axis = Vector3.forward;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        GameObject collideObj = collision.gameObject;

        if(collideObj.tag== "train")
        {
            //destroy both trains
            Destroy(collideObj);
            Destroy(this.gameObject);
        } else if(collideObj.tag == "gem")
        {
            gtManage.GetComponent<GemTrainManager>().hitGem();
            Destroy(collideObj);
        }
    }

    void updateRotation()
    {
        Vector3 axis = Vector3.back;
        int rotateint = ((curDirection+2)%4) - targetTrack.GetComponent<Track>().getOutDirection(curDirection);


        if (rotateint ==3 || rotateint==-1)
        {
            
            axis = Vector3.forward;
        }
        
        //angles/time 90/(deltaTime *transform.position-target.position

        transform.Rotate(axis, rote);
    }

    
    
   
}
