using UnityEngine;
using System.Collections;

public class TileModel : MonoBehaviour {

    //Can either be attached to a quad object, or an game object with two child quad objects
    //This script will control these quad objects 
    int tileType;
    Tile owner;
    Material mat;
    GameObject modelObject;
    GameObject lightsObject;
    Renderer tileRenderer;


    

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void init(int type, Tile owner)
    {
        tileType = type;
        this.owner = owner;
        transform.parent = owner.transform;
        transform.localPosition = new Vector3(0, 0, 0);        

        if (type == 1)
        //add a quad for the tile and turning lights
        {
            modelObject = GameObject.CreatePrimitive(PrimitiveType.Quad);
            modelObject.transform.parent = this.transform;
            modelObject.transform.localPosition = new Vector3(0, 0, 0);
            tileRenderer = modelObject.GetComponent<Renderer>(); //Quad for the empty tile            

           lightsObject = GameObject.CreatePrimitive(PrimitiveType.Quad); //Quad for the turninig lights
            lightsObject.transform.parent = this.transform;
           lightsObject.transform.localPosition = new Vector3(0, 0, 0);
            Renderer turnRend = lightsObject.GetComponent<Renderer>();
            turnRend.sortingLayerName  = "Mid";
            turnRend.material.mainTexture = Resources.Load<Texture2D>("Textures/turnLights");
            turnRend.material.shader = Shader.Find("Sprites/Default");
        }
        else if (type == 2)
        {//if there's a wall
            tileRenderer = GetComponent<Renderer>();
            name = "TileModel";
            mat = tileRenderer.material;                                // Get the material component of this quad object.
            mat.mainTexture = Resources.Load<Texture2D>("Textures/tileWall");  // Set the texture.  Must be in Resources folder.
            mat.shader = Shader.Find("Sprites/Default");                        // Tell the renderer that our textures have transparency. 
            tileRenderer.sortingLayerName = "Fore";
            return;
        }
        else
        {
            tileRenderer = GetComponent<Renderer>();
        }
        name = "TileModel";
        mat = tileRenderer.material;                                // Get the material component of this quad object.
        mat.mainTexture = Resources.Load<Texture2D>("Textures/tileBlank");  // Set the texture.  Must be in Resources folder.
        mat.shader = Shader.Find("Sprites/Default");						// Tell the renderer that our textures have transparency. 
        tileRenderer.sortingLayerName = "Background";

    }

    public void rotate()
    {
        Vector3 axis = Vector3.forward;
        float angle = 90;
        lightsObject.transform.Rotate(axis, angle);
    }

    void OnMouseDown()
    {
        //print("tilemodel");
    }
}
