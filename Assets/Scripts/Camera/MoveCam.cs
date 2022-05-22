//Script to move the camera

using UnityEngine;

public class MoveCam : MonoBehaviour
{
    [SerializeField] private Transform head;

    private void Update()
    {
        transform.position = head.position;
    }
}
