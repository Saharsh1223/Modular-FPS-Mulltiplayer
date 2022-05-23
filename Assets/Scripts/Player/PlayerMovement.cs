//Script which handles the Player and its Movement

using System;
using System.Collections;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviourPunCallbacks, IDamageable/*, IPunObservable*/
{

    [Header("Panels")]
    public GameObject mainPanel;
    public GameObject diedPanel;
    public GameObject gameOverPanel;
    public GameObject pauseMenuPanel;
    
    [Header("References")]
    public Rigidbody rb;
    public Transform orientation;
    [Space]
    [SerializeField] private Camera _camera;
    [SerializeField] private WeaponSway sway;
    [SerializeField] private GameObject canvasObject;
    [Space]
    public GameObject capsuleObject;
    [Space]
    [SerializeField] private UsernameDisplay usernameDisplay;
    [SerializeField] private Look playerLook;
    [Space]
    [SerializeField] private TMP_Text killedByText;

    [Header("Movement")]
    [SerializeField] private float speed = 40f;
    [SerializeField] private float airSpeed = 20f;
    [Space]
    [SerializeField] private float jumpForce = 12.5f;
    [SerializeField] private float jumpRate = 15f;
    private float nextTimeToJump;
    [SerializeField] private PhysicMaterial playerMat;

    [Header("Ground Detection")]
    [SerializeField] private bool isGrounded;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundDistance = 0.3f;

    [Header("Drag")]
    [SerializeField] private float drag = 6f;
    [SerializeField] private float airDrag = 2f;

    [Header("Weapon Equipment")] 
    [SerializeField] private Item[] items;
    [Space]
    private int itemIndex;
    private int previousIndex = -1;
    [Space]
    public bool canSwitchGuns;

    [Header("Health Management")]
    [SerializeField] private Image healthbarImage;
    private const float maxHealth = 100f;
    private float currentHealth = maxHealth;

    [HideInInspector] public PlayerManager playerManager;

    [Space(height: 20)]
    public GameObject weaponHolder;

    private Vector3 moveDirection;
    public PhotonView pv;
    [HideInInspector] public string playerName;

    private float h;
    private float v;

    private float sendGameOverOnce;

    private void Awake()
    {
        //rb.freezeRotation = true;
        playerManager = PhotonView.Find((int)pv.InstantiationData[0]).GetComponent<PlayerManager>();
    }

    private void Start()
    {
        if (pv.IsMine)
        {
            EquipItem(0);
        }
        else
        {
            //Destroy(rb);
            _camera.gameObject.GetComponent<AudioListener>().enabled = false;
            Destroy(_camera.gameObject);
        }
    }

    private void Update()
    {
        if(pv == null) return;

        if (!pv.IsMine)
        {
            sway.enabled = false;
            canvasObject.SetActive(false);
            
            gameObject.tag = "OtherPlayer";
            return;
        }

        if (!RoomManager.Instance.timer.timerOn && RoomManager.Instance.timer.startedGame)
        {
            SetGameOver();
        }
        
        playerName = usernameDisplay.usernameText.text;

        gameObject.tag = "Player";
        
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);


        if (Input.GetKeyDown(KeyCode.Escape) && !pauseMenuPanel.activeInHierarchy && !gameOverPanel.activeInHierarchy)
        {
            pauseMenuPanel.SetActive(true);
        }
        else if (Input.GetKeyDown(KeyCode.Escape) && pauseMenuPanel.activeInHierarchy)
        {
            pauseMenuPanel.SetActive(false);
        }

        if (gameOverPanel.activeInHierarchy)
        {
            playerLook.cursorLocked = false;
        }
        else
        {
            playerLook.cursorLocked = true;
        }

        if (!pauseMenuPanel.activeInHierarchy)
        {
            Movement();
            HandleDrag();
        
            HandleInventory();
            UseGun();

            playerLook.cursorLocked = true;
        }
        else
        {
            playerLook.cursorLocked = false;
        }
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.JoinLobby();

        SceneManager.LoadScene("Lobby");
    }

    public void Resume()
    {
        pauseMenuPanel.SetActive(false);
    }

    public void SetGameOver()
    {
        pv.RPC("GameOverRPC", RpcTarget.All);
    }
    
    private void Movement()
    {
        h = Input.GetAxisRaw("Horizontal");
        v = Input.GetAxisRaw("Vertical");
            
        moveDirection = (orientation.forward * v + orientation.right * h).normalized;

        if (pv.IsMine)
        {
            if (isGrounded && Input.GetKey(KeyCode.Space) && Time.time >= nextTimeToJump)
            {
                nextTimeToJump = Time.time + 1f / jumpRate;
                Jump();
            }
        }
    }
    
    private void HandleDrag()
    {
        if (isGrounded)
            rb.drag = drag;
        else
            rb.drag = airDrag;
    }

    private void Jump()
    {
        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    private void FixedUpdate()
    {
        if (!pv.IsMine) return;
        
        if(isGrounded)
            rb.AddForce(moveDirection * speed, ForceMode.Acceleration);
        else
            rb.AddForce(moveDirection * airSpeed, ForceMode.Acceleration);
    }

    private void HandleInventory()
    {
        if (canSwitchGuns)
        {
            for (int i = 0; i < items.Length; i++)
            {
                if (Input.GetKeyDown((i + 1).ToString()))
                {
                    EquipItem(i);
                    break;
                }
            }

            if (Input.GetAxisRaw("Mouse ScrollWheel") > 0f)
            {
                if (itemIndex >= items.Length - 1)
                    EquipItem(0);
                else
                    EquipItem(itemIndex + 1);
            }
            else if (Input.GetAxisRaw("Mouse ScrollWheel") < 0f)
            {
                if (itemIndex <= 0)
                    EquipItem(items.Length - 1);
                else
                    EquipItem(itemIndex - 1);
            }   
        }
    }

    private void UseGun()
    {
        Gun currentGun = items[itemIndex].gameObject.GetComponent<Gun>();

        if (currentGun.automatic)
        {
            if (Input.GetMouseButton(0) && !currentGun.isReloading && Time.time >= currentGun.nextTimeToFire)
            {
                items[itemIndex].Use();
                currentGun.nextTimeToFire = Time.time + 1f / currentGun.fireRate;
            }
            else
            {
                currentGun.StopShootingAnimationAuto();
            }
        }
        else
        {
            if (Input.GetMouseButtonDown(0))
            {
                items[itemIndex].Use();
            }
            else
            {
                currentGun.StopShootingAnimationNonAuto();
            }
        }
    }

    private void EquipItem(int _index)
    {
        if(pv == null) return;
        
        if (_index == previousIndex) return;

        itemIndex = _index;
        items[itemIndex].itemGameObject.SetActive(true);

        if (previousIndex != -1)
        {
            items[previousIndex].itemGameObject.SetActive(false);
        }

        previousIndex = itemIndex;

        if (pv.IsMine)
        {
            Hashtable hash = new Hashtable();
            hash.Add("itemIndex", itemIndex);
            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
        }
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (!pv.IsMine && targetPlayer == pv.Owner)
        {
            EquipItem((int)changedProps["itemIndex"]);
        }
    }

    public void TakeDamage(float damage)
    {
        pv.RPC("RPC_TakeDamage", RpcTarget.All, damage);
    }

    private void Die(Player player)
    {
        playerManager.Die();
        pv.RPC("DieRPC", RpcTarget.All, player);
    }

    private void OnCollisionStay(Collision collisionInfo)
    {
        if (pv.IsMine)
        {
            if (collisionInfo.gameObject.CompareTag("Stairs"))
            {
                gameObject.GetComponent<CapsuleCollider>().material = null;
                rb.AddForce(moveDirection * speed * 3f, ForceMode.Acceleration);
            }
            else
            {
                gameObject.GetComponent<CapsuleCollider>().material = playerMat;
            }
        }
    }
    
    [PunRPC]
    private void RPC_TakeDamage(float damage, PhotonMessageInfo info)
    {
        if(!pv.IsMine)
            return;

        Player sender = info.Sender;
        
        Debug.Log("Took damage:" + damage);
        currentHealth -= damage;
        healthbarImage.fillAmount = currentHealth / maxHealth;

        if (currentHealth <= 0 && RoomManager.Instance.timer.timerOn)
        {
            Die(sender);
        }
    }

    [PunRPC]
    private void DieRPC(Player sender)
    {
        GetComponent<CapsuleCollider>().enabled = false;
        
        capsuleObject.SetActive(false);
        weaponHolder.SetActive(false);
        
        mainPanel.SetActive(false);
        diedPanel.SetActive(true);
        
        usernameDisplay.gameObject.SetActive(false);
        
        rb.useGravity = false;

        killedByText.text = "KILLED BY: " + sender.NickName;
    }
    
    [PunRPC]
    private void GameOverRPC()
    {
        GetComponent<CapsuleCollider>().enabled = false;
        
        capsuleObject.SetActive(false);
        weaponHolder.SetActive(false);
        
        mainPanel.SetActive(false);
        gameOverPanel.SetActive(true);
        
        usernameDisplay.gameObject.SetActive(false);
        
        rb.useGravity = false;
        
        Debug.Log("Game OVER!");
    }
}