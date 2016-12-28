using UnityEngine;
using System.Collections;

public class BombModel : MonoBehaviour {

    Bomb owner;
    Material mat;
    Renderer bRenderer;
	// Use this for initialization
	public void init (Bomb owner) {
        this.owner = owner;
        transform.parent = owner.transform;
        transform.localPosition = new Vector3(0, 0, 0);
        bRenderer = GetComponent<Renderer>();
        name = "Bomb Model";
        Material mat = bRenderer.material;                                // Get the material component of this quad object.
        mat.mainTexture = Resources.Load<Texture2D>("Textures/bomb");  // Set the texture.  Must be in Resources folder.
        mat.shader = Shader.Find("Sprites/Default");						// Tell the renderer that our textures have transparency. 
        bRenderer.sortingLayerName = "Fore";
        DestroyImmediate(GetComponent<MeshCollider>());
        BoxCollider2D coll = gameObject.AddComponent<BoxCollider2D>();
        coll.isTrigger = true;
        Rigidbody2D rigid = gameObject.AddComponent<Rigidbody2D>();
        rigid.isKinematic = true;
        rigid.gravityScale = 0;
        gameObject.tag = "marble";
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject collideObj = collision.gameObject;
        //print("Collided");

        if (collideObj.tag == "marble")
        {
            Marble m = collideObj.GetComponent<MarbleModel>().owner;
            Destroy(m.gameObject);
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update () {
	
	}
    public Bomb getOwner()
    {
        return owner;
    }
}
