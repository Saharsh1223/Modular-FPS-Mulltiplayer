//Basic player look

using UnityEngine;
using Photon.Pun;

public class Look : MonoBehaviour {

    [Header("References")]
    [SerializeField] private Transform cameraHolder;
    [SerializeField] private Transform player;
    [SerializeField] private Transform orientation;
    [SerializeField] private PhotonView pv;

    [Header("Look Settings")]
    [SerializeField] private float sensX = 10f;
    [SerializeField] private float sensY = 10f;
    [Space]
    public bool cursorLocked;

    private float y;
    private float x;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        if (pv.IsMine)
        {
            float mouseX = Input.GetAxisRaw("Mouse X") * 0.1f;
            float mouseY = Input.GetAxisRaw("Mouse Y") * 0.1f;

            y += mouseX * sensX;
            x -= mouseY * sensY;

            x = Mathf.Clamp(x, -90f, 90f);

            //player.rotation = Quaternion.Euler(0f, y, 0f);
            orientation.rotation = Quaternion.Euler(0f, y, 0f);
            cameraHolder.localRotation = Quaternion.Euler(x, y, 0f);

            if (cursorLocked)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            else
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }
    }

    /*public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(orientation.rotation);
            stream.SendNext(cameraHolder.localRotation);
        }
        else
        {
            orientation.rotation = (Quaternion) stream.ReceiveNext();
            cameraHolder.localRotation = (Quaternion) stream.ReceiveNext();
        }
    }*/
}