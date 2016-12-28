using UnityEngine;
using System.Collections;

public class MarbleModel : MonoBehaviour {

    public Marble owner;
    Renderer marbRenderer;
    

	// Use this for initialization
	void Start () {
	
	}

    public void init(Marble owner)
    {        
        this.owner = owner;
        transform.parent = owner.transform;
        transform.localPosition = new Vector3(0, 0, 0);
        marbRenderer = GetComponent<Renderer>();
        name = "MarbleModel";
        Material mat= marbRenderer.material;                                // Get the material component of this quad object.
        mat.mainTexture = Resources.Load<Texture2D>("Textures/marble");  // Set the texture.  Must be in Resources folder.
        mat.shader = Shader.Find("Sprites/Default");						// Tell the renderer that our textures have transparency. 
        marbRenderer.sortingLayerName = "Fore";
        DestroyImmediate(GetComponent<MeshCollider>());
        BoxCollider2D coll = gameObject.AddComponent<BoxCollider2D>();
        coll.isTrigger = true;
        Rigidbody2D rigid = gameObject.AddComponent<Rigidbody2D>();
        rigid.isKinematic = true;
        rigid.gravityScale = 0;
        gameObject.tag = "marble";
    }

    public void rotate(float angle)
    {

        Vector3 axis = Vector3.forward;
        
        owner.transform.Rotate(axis, angle);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject collideObj = collision.gameObject;
        //print("Collided");
        if (collideObj.tag != null)
        {

            if (collideObj.tag == "gemModel")
            {
                Gem deadGem = collideObj.GetComponent<GemModel>().owner;
                owner.getOwner().gemCollected(deadGem);
                Destroy(deadGem.gameObject);
            }
            else if (collideObj.tag == "marble")
            {
                Marble m = collideObj.GetComponent<MarbleModel>().owner;
                m.hitMarble();
            }
        }
    }


    // Update is called once per frame
    void Update () {
	
	}

    void OnMouseDown()
    {
        owner.turboHit();
    }
}
