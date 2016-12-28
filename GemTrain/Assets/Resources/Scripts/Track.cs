using UnityEngine;
using System.Collections;

public class Track : MonoBehaviour
{
    public int out1;   //The two outward directions any track can have
    public int out2;
    private bool hasGem;
    public bool hasCurve;

    // Use this for initialization
    void Start()
    {
       
    }

    public void init(int i, int j)
    {
        this.out1 = i;
        this.out2 = j;
        hasGem = false;
    }

    // Update is called once per frame
    void Update() { 
    
    }

    public int getOutDirection(int curDirection)
    {
        //we are taking in the train's current direction and returning the corresponding outward direction for this track
        int opposite = (curDirection + 2) % 4;
        if(opposite == out1)
        {
            return out2;
        } else
        {
            return out1;
        }
    }

    public void setOuts(int a, int b)
    {
        out1 = a;
        out2 = b;
    }

    public bool doesHaveGem()
    {
        return hasGem;
    }

    public void setHasGem(bool b)
    {
        hasGem = b;
    }
}
