using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    [SerializeField] private GameObject graphics;

    private void Start()
    {
        graphics.SetActive(false);
        
        Destroy(GetComponent<MeshFilter>());
        Destroy(GetComponent<MeshRenderer>());
    }
}
