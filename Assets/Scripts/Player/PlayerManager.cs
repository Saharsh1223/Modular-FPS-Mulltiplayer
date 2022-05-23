//As the name Suggests, this Script manages the Player stuff like Dying, Respawning

using UnityEngine;
using Photon.Pun;
using System.IO;
using TMPro;

public class PlayerManager : MonoBehaviour
{
    private PhotonView pv;
    public GameObject controller;

    private void Awake()
    {
        pv = GetComponent<PhotonView>();

        if (pv.IsMine)
        {
            CreateController();
        }
    }

    private void CreateController()
    {
        Transform spawnPoint = SpawnManager.Instance.GetSpawnPoint();
        controller = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Player"), spawnPoint.position, spawnPoint.rotation, 0, new object[] { pv.ViewID }); //Instantiate our Player Controller
        
        Debug.Log("Instantiated Player");

        controller.GetComponent<PlayerMovement>().transform.rotation = Quaternion.identity;
        controller.GetComponent<PlayerMovement>().orientation.rotation = spawnPoint.rotation;

        foreach (PlayerMovement player in FindObjectsOfType<PlayerMovement>())
        {
            player.gameObject.name = player.playerName;
        }
    }

    public void Die()
    {
        Debug.Log("You Died!");
        Invoke("DestroyPlayerAndRespawn", 3.99f);
    }

    private void DestroyPlayerAndRespawn()
    {
        PhotonNetwork.Destroy(controller);
        CreateController();
    }
}
