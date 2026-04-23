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
        PlayerDamaged playerDamaged = GetComponent<PlayerDamaged>();
        if (playerDamaged != null && playerDamaged.IsDead)
            return;
        RotatePlayerToMouse();
    }

    //rotation of the player
    private void RotatePlayerToMouse()
    {
       if (Camera.main == null) return;

       Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
       mouseWorld.z = transform.position.z;

       Vector2 direction = mouseWorld - transform.position;
       if (direction.sqrMagnitude <= 0.0001f) return;

       float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
       transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }
   
}
