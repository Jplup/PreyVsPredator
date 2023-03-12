using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfinityBorders : MonoBehaviour{
    public BoxCollider2D lBorder;
    public BoxCollider2D rBorder;
    public BoxCollider2D uBorder;
    public BoxCollider2D dBorder;
    public GameObject mirrorTemplate;
    public List<GameObject> objectsToMirror;
    public List<Vector2> bordersInContact;
    public List<List<GameObject>> mirrorObjects;
    public List<GameObject> oldObjectsToMirror;
    public List<Vector2> oldBordersInContact;
    public List<List<GameObject>> oldMirrorObjects;
    public float multiplicator;
    // Start is called before the first frame update
    void Start(){
        objectsToMirror=new List<GameObject>();
        mirrorObjects=new List<List<GameObject>>();
        bordersInContact=new List<Vector2>();
        lBorder.GetComponent<ColliderChecker>().master=this;
        rBorder.GetComponent<ColliderChecker>().master=this;
        uBorder.GetComponent<ColliderChecker>().master=this;
        dBorder.GetComponent<ColliderChecker>().master=this;
        multiplicator=transform.localScale.x*5;
    }

    public void CheckObject(int indexOfObject){
        if (bordersInContact[indexOfObject].magnitude==0) {
            objectsToMirror.RemoveAt(indexOfObject);
            List<GameObject> mirroredObject=mirrorObjects[indexOfObject];
            mirrorObjects.RemoveAt(indexOfObject);
            bordersInContact.RemoveAt(indexOfObject);
            foreach (GameObject toDestroy in mirroredObject) {Destroy(toDestroy);}
        }else{
            List<GameObject> mirroredObjects=new List<GameObject>();
            mirroredObjects.Add(mirrorObjects[indexOfObject][0]);
            Destroy(mirrorObjects[indexOfObject][1]);
            Destroy(mirrorObjects[indexOfObject][2]);
            mirrorObjects[indexOfObject]=mirroredObjects;
        }
    }

    // Update is called once per frame
    void Update(){
        for (int i = 0; i < objectsToMirror.Count; i++){
            if (objectsToMirror[i]==null){
                objectsToMirror.RemoveAt(i);
                bordersInContact.RemoveAt(i);
                mirrorObjects.RemoveAt(i);
            }
        }
        for (int i = 0; i < objectsToMirror.Count; i++){
            if (mirrorObjects[i].Count==1) {
                mirrorObjects[i][0].transform.position=new Vector3(
                objectsToMirror[i].transform.position.x-(multiplicator*bordersInContact[i].x),
                objectsToMirror[i].transform.position.y-(multiplicator*bordersInContact[i].y),
                0
                );
            }else{
                mirrorObjects[i][0].transform.position=new Vector3(
                objectsToMirror[i].transform.position.x-(multiplicator*bordersInContact[i].x),
                objectsToMirror[i].transform.position.y-(multiplicator*bordersInContact[i].y),
                0
                );
                mirrorObjects[i][1].transform.position=new Vector3(
                objectsToMirror[i].transform.position.x,
                objectsToMirror[i].transform.position.y-(multiplicator*bordersInContact[i].y),
                0
                );
                mirrorObjects[i][2].transform.position=new Vector3(
                objectsToMirror[i].transform.position.x-(multiplicator*bordersInContact[i].x),
                objectsToMirror[i].transform.position.y,
                0
                );
            }
        }
    }
}
