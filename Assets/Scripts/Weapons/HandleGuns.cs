//Handles the Gun sizes for you and the other Players!

using Photon.Pun;
using UnityEngine;

public class HandleGuns : MonoBehaviour
{
    [SerializeField] private PhotonView pv;
    [Space(50)]
    [SerializeField] private GameObject AK_74;
    [SerializeField] private GameObject M1991;
    [SerializeField] private GameObject M4_8;
    [SerializeField] private GameObject Bennelli_M4;

    private void Start()
    {
        if (!pv.IsMine)
        {
            AK_74.transform.localScale += new Vector3(0.9f, 0.9f, 0.9f);
            M1991.transform.localScale += new Vector3(0.9f, 0.9f, 0.9f);
            M4_8.transform.localScale += new Vector3(0.9f, 0.9f, 0.9f);
            Bennelli_M4.transform.localScale += new Vector3(0.9f, 0.9f, 0.9f);
        }
        else
        {
            AK_74.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
            M1991.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
            M4_8.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
            Bennelli_M4.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
        }
    }
}
