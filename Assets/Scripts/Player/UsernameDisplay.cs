using System;
using Photon.Pun;
using TMPro;
using UnityEngine;


public class UsernameDisplay : MonoBehaviour
{
    [SerializeField] private PhotonView view;
    public TMP_Text usernameText;

    private void Awake()
    {
        usernameText.text = view.Owner.NickName;
        
        if(view.IsMine)
            usernameText.gameObject.SetActive(false);
    }
}
