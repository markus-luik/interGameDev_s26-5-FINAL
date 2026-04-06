using UnityEngine;

public class PlayerRotation : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        RotatePlayerToMouse();
    }

    //rotation of the player
    private void RotatePlayerToMouse()
    {
       // Cast a ray from the mouse to a horizontal plane at player height.
       Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
       Plane plane = new Plane(Vector3.up, new Vector3(0f, transform.position.y, 0f));

       if (plane.Raycast(ray, out float distance))
       {
           Vector3 hitPoint = ray.GetPoint(distance);
           Vector3 direction = hitPoint - transform.position;
           direction.y = 0f; // rotate only around Y axis

           if (direction.sqrMagnitude > 0.0001f)
           {
               transform.rotation = Quaternion.LookRotation(direction);
           }
       }
    }
   
}
