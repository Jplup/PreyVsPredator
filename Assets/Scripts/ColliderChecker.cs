using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderChecker : MonoBehaviour{
    public InfinityBorders master;
    public Vector2 borderPosition;
    public void OnTriggerEnter2D(Collider2D other){
        if (other.isTrigger) {return;}
        if (master.objectsToMirror.Contains(other.gameObject)){
            int indexOfItem=master.objectsToMirror.IndexOf(other.gameObject);
            Vector2 newInfo=new Vector2(
                master.bordersInContact[indexOfItem].x,
                master.bordersInContact[indexOfItem].y
            );
            if (borderPosition==new Vector2(-1,0)) {newInfo.x=-1;}
            if (borderPosition==new Vector2(1,0))  {newInfo.x=1;}
            if (borderPosition==new Vector2(0,-1)) {newInfo.y=-1;}
            if (borderPosition==new Vector2(0,1))  {newInfo.y=1;}
            master.bordersInContact[indexOfItem]=newInfo;
            List<GameObject> objectsToAdd=master.mirrorObjects[master.objectsToMirror.IndexOf(other.gameObject)];
            objectsToAdd.Add(Instantiate(master.mirrorTemplate,transform.position,Quaternion.identity));
            objectsToAdd.Add(Instantiate(master.mirrorTemplate,transform.position,Quaternion.identity));
            master.mirrorObjects[master.objectsToMirror.IndexOf(other.gameObject)]=objectsToAdd;
        }else{
            if (borderPosition==new Vector2(-1,0)) {master.bordersInContact.Add(new Vector2(-1,0));}
            if (borderPosition==new Vector2(1,0))  {master.bordersInContact.Add(new Vector2(1,0));}
            if (borderPosition==new Vector2(0,-1)) {master.bordersInContact.Add(new Vector2(0,-1));}
            if (borderPosition==new Vector2(0,1))  {master.bordersInContact.Add(new Vector2(0,1));}
            master.objectsToMirror.Add(other.gameObject);
            List<GameObject> objectsToAdd=new List<GameObject>();
            objectsToAdd.Add(Instantiate(master.mirrorTemplate,transform.position,Quaternion.identity));
            master.mirrorObjects.Add(objectsToAdd);
        }
    }
    public void OnTriggerExit2D(Collider2D other){
        if (other.isTrigger) {return;}
        if (master.objectsToMirror.Contains(other.gameObject)){
            try{
                int indexOfItem=master.objectsToMirror.IndexOf(other.gameObject);
                Vector2 newInfo=new Vector2(
                    master.bordersInContact[indexOfItem].x,
                    master.bordersInContact[indexOfItem].y
                );
                if (borderPosition==new Vector2(-1,0)) {newInfo.x=0;}
                if (borderPosition==new Vector2(1,0))  {newInfo.x=0;}
                if (borderPosition==new Vector2(0,-1)) {newInfo.y=0;}
                if (borderPosition==new Vector2(0,1))  {newInfo.y=0;}
                master.bordersInContact[indexOfItem]=newInfo;
                master.CheckObject(indexOfItem);


                /*
                List<Vector2> nv2=new List<Vector2>();
                List<List<GameObject>> nllg=new List<List<GameObject>>();
                List<GameObject> nlg=new List<GameObject>();
                List<GameObject> nsl;
                for (int i = 0; i < master.objectsToMirror.Count; i++){
                    nv2.Add(master.bordersInContact[i]);
                    nlg.Add(master.objectsToMirror[i]);
                    nsl=new List<GameObject>();
                    for (int a = 0; a < master.mirrorObjects[i].Count; a++) {nsl.Add(master.mirrorObjects[i][a]);}
                    nllg.Add(nsl);
                }
                master.oldBordersInContact=nv2;
                master.oldMirrorObjects=nllg;
                master.oldObjectsToMirror=nlg;
                */


            }
            catch (System.Exception){
                /*
                Debug.Log("Current:");
                Debug.Log("       obs:");
                for (int i = 0; i < master.objectsToMirror.Count; i++){
                    Debug.Log("             "+master.objectsToMirror[i].name);
                }
                Debug.Log("       bs:");
                for (int i = 0; i < master.objectsToMirror.Count; i++){
                    Debug.Log("             "+master.bordersInContact[i].ToString());
                }
                Debug.Log("       bs:");
                for (int i = 0; i < master.objectsToMirror.Count; i++){
                    Debug.Log("             "+i+":");
                    for (int a = 0; a < master.mirrorObjects[i].Count; a++){
                        Debug.Log("                        "+master.mirrorObjects[i][a].name);
                    }
                }
                Debug.Log("Old:");
                Debug.Log("       obs:");
                for (int i = 0; i < master.oldObjectsToMirror.Count; i++){
                    Debug.Log("             "+master.oldObjectsToMirror[i].name);
                }
                Debug.Log("       bs:");
                for (int i = 0; i < master.oldObjectsToMirror.Count; i++){
                    Debug.Log("             "+master.oldBordersInContact[i].ToString());
                }
                Debug.Log("       bs:");
                for (int i = 0; i < master.oldObjectsToMirror.Count; i++){
                    Debug.Log("             "+i+":");
                    for (int a = 0; a < master.oldMirrorObjects[i].Count; a++){
                        Debug.Log("                        "+master.oldMirrorObjects[i][a].name);
                    }
                }*/
                throw;
            }
            
        }else{return;}
    }
}
