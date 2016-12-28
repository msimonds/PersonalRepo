using UnityEngine;
using System.Collections;

public class Switch : MonoBehaviour {

    int alt1;
    int alt2;
    int curAlt;
    int neighbor; //which direction is the switch track at from the switch
    string trackSprite;
    string leverSprite;

    GameObject switchTrack;

    public void init(int n, int a1, int a2)
    {
        alt1 = a1;
        alt2 = a2;
        curAlt = a1;
        neighbor = n;
    }

    public void finishSetup()
    {
        switchTrack = this.gameObject.GetComponent<Tile>().getNeighbor(neighbor);
        getOuts(curAlt);//sets the trackSprite string
        updateCurve(switchTrack.GetComponent<Track>());
    }

    public GameObject getSwitchTrack()
    {
        return switchTrack;
    }
    
    //Returns array of out directions of the switched track, also updates the sprite
    int[] getOuts(int type)
    {
        int[] outs;

        switch (type)
        {            
            case 0:
                outs =new int[2] {0, 2};
                trackSprite = "straightTracks";
                break;
            case 1:
                outs = new int[2] { 1, 3 };
                trackSprite = "sideTrack";
                break;
            case 2:
                outs = new int[2] { 0, 1 };
                trackSprite = "northeastTrack";
                break;
            case 3:
                outs = new int[2] { 0, 3};
                trackSprite = "northwestTrack";
                break;
            case 4:
                outs = new int[2] { 1, 2 };
                trackSprite = "southeastTrack";
                break;
            case 5:
                outs = new int[2] { 2, 3};
                trackSprite = "southwestTrack";
                break;
            default:
                return null;
        }
        return outs;
    }    

    void OnMouseDown()
    {
        curAlt = (curAlt == alt1) ? alt2 : alt1;
        leverSprite = (leverSprite == "leverleft") ? "leverright" : "leverleft";

        int[] outs = getOuts(curAlt);
        switchTrack.GetComponent<Track>().setOuts(outs[0], outs[1]);

        //for optimization should probs load dis shit somewhere else, prob v expensive operations
        Texture2D txtr = Resources.Load("Textures/" + trackSprite) as Texture2D;
        switchTrack.GetComponent<SpriteRenderer>().sprite = Sprite.Create(txtr, new Rect(0, 0, txtr.width, txtr.height), new Vector2(0.5f, 0.5f), 100);
        txtr = Resources.Load("Textures/" + leverSprite) as Texture2D;
        this.gameObject.GetComponent<SpriteRenderer>().sprite = Sprite.Create(txtr, new Rect(0, 0, txtr.width, txtr.height), new Vector2(0.5f, 0.5f), 100);
        updateCurve(switchTrack.GetComponent<Track>());
        print("Clicked mouse on switch");

    }
    void updateCurve(Track curTrack)
    {
        if (curAlt > 1)
        {
            curTrack.hasCurve = true;
        }
        else
        {
            curTrack.hasCurve = false;
        }
    }
    
}
