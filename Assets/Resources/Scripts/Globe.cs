using UnityEngine; 
using System.Collections; 

public class Globe : MonoBehaviour 
{ 


    bool hasGrabbedPoint = false;
    Vector3 grabbedPoint;

   void Update () 
   { 
      if (Input.GetMouseButton(0)) 
      {         
        if(!hasGrabbedPoint)
        {
            hasGrabbedPoint = true;                                             
            grabbedPoint = getTouchedPoint();
        }
        else
        {
            Vector3 targetPoint = getTouchedPoint();
            Quaternion rot = Quaternion.FromToRotation (grabbedPoint, targetPoint);
            transform.localRotation *= rot;             
        }
      }
      else
        hasGrabbedPoint = false;      
   }

   Vector3 getTouchedPoint()
   {
        RaycastHit hit;
        Physics.Raycast (Camera.main.ScreenPointToRay(Input.mousePosition), out hit);

        return transform.InverseTransformPoint(hit.point);
   }
}