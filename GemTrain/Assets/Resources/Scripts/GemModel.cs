﻿using UnityEngine;
using System.Collections;

public class GemModel : MonoBehaviour
{
	private int gemType;		
	private float clock;		// Keep track of time since creation for animation.
	private Gem owner;			// Pointer to the parent object.
	private SpriteRenderer mat;		// Material for setting/changing texture and color.

	public void init(int gemType, Gem owner) {
		this.owner = owner;
		this.gemType = gemType;

		transform.parent = owner.transform;					// Set the model's parent to the gem.
		transform.localPosition = new Vector3(0,0,0);		// Center the model on the parent.
		name = "Gem Model";									// Name the object.

		mat = GetComponent<SpriteRenderer>();								// Get the material component of this quad object.
		Texture2D tx = Resources.Load<Texture2D>("Textures/gem"+gemType);   // Set the texture.  Must be in Resources folder.
        mat.sprite = Sprite.Create(tx, new Rect(0, 0, tx.width, tx.height), new Vector2(0.5f, 0.5f), 100);
        mat.sortingLayerName = "Foreground";
    }
		
	void Start () {
		clock = 0f;
	}

	void Update () {
		
		// Incrememnt the clock based on how much time has elapsed since the previous update.
		// Using deltaTime is critical for animation and movement, since the time between each call
		// to Update is unpredictable.
		clock = clock + Time.deltaTime;

		// A more serious project would probably use subclasses for gem types, or at least enums instead of 
		// this silly gemType int plus a bunch of if statements.  But it'll do for here.  

		if (gemType == 1) { // set rotation around z for spin effect
			transform.eulerAngles = new Vector3(0,0,360*clock);
		}
		if (gemType == 2) { // set position y for bounce effect
			transform.localPosition = new Vector3(0,Mathf.Sin(5*clock)/5,0);	
		}	
		if (gemType == 3) { // set scale in x and y for pulse effect
			transform.localScale = new Vector3(1+Mathf.Sin(3*clock)/2,1+Mathf.Sin(3*clock)/2,1);
		}
		
	}
}

