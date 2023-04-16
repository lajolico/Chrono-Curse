 using UnityEngine;
 using System.Collections;
 
 public class SmoothCamera2D : MonoBehaviour {
     
    //  public float dampTime = 0.15f;
    //  private Vector3 velocity = Vector3.zero;
    //  public Transform target;

    
    //  public float pixPerUnit = 16;
    //  void LateUpdate()
    //  {        
    //      transform.position = new Vector3(
    //          Mathf.Round(transform.parent.position.x * pixPerUnit) / pixPerUnit,
    //          Mathf.Round(transform.parent.position.y * pixPerUnit) / pixPerUnit, -50);        
    //  }
 
    //  public float pixPerUnit = 16;
    //  void LateUpdate()
    //  {        
    //      transform.position = new Vector3(
    //          Mathf.Round(target.position.x * pixPerUnit) / pixPerUnit,
    //          Mathf.Round(target.position.y * pixPerUnit) / pixPerUnit,
    //          target.position.z);        
    //  }
 
     // Update is called once per frame
    //  void Update () 
    //  {
    //      if (target)
    //      {
    //          Vector3 point = GetComponent<Camera>().WorldToViewportPoint(target.position);
    //          Vector3 delta = target.position - GetComponent<Camera>().ViewportToWorldPoint(new Vector3(0.5f, 0.5f, point.z)); //(new Vector3(0.5, 0.5, point.z));
    //          Vector3 destination = transform.position + delta;
    //          transform.position = Vector3.SmoothDamp(transform.position, destination, ref velocity, dampTime);
    //      }
     
    //  }
 }