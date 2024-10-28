using UnityEngine;

public class ClickableObject : MonoBehaviour
{
    private FirstController firstController = (FirstController)SSDirector.getInstance().currentSceneController;

    void Update()
    {
        // Detect left-mouse clicks
        if (Input.GetMouseButtonDown(0))  // 0 for left-click
        {
            // Creates a ray that emits from the camera to the location of the mouse pointer
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // Detect the collision of the ray with an object in the scene
            if (Physics.Raycast(ray, out hit))
            {
                // Check if the colliding object is the current object
                if (hit.collider != null && hit.collider.gameObject == gameObject)
                {
                    if (firstController.roleDict.ContainsKey(gameObject.GetInstanceID()))
                    {
                        Debug.Log("Priset or Devil");
                        firstController.MoveRole(gameObject);
                    }
                    else
                    {
                        Debug.Log("Boat");
                        firstController.MoveBoat();
                    }
                }
            }
        }
    }
}
