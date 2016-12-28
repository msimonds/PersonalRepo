using UnityEngine;
using System.Collections;

public class Trigger : MonoBehaviour {

	void OnTriggerExit2D(){
        GameObject.Find("GemTrainManager").GetComponent<GemTrainManager>().triggerHit();
    }
}
