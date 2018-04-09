using UnityEngine;

public class Startpoint : MonoBehaviour
{
    private bool firstTimeSpawn = true;
    void OnDrawGizmos()
    {
        Gizmos.DrawIcon(transform.position, "CheckPoint", true);
    }

    void OnEnable()
    {

        PlayerController.CheckPoint(transform.position);
    }

    void OnDisable()
    {
        firstTimeSpawn = true;
    }
}
