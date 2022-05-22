//Kinda advanced Photon Lobby System

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;
using Photon.Pun;
using TMPro;

public class Lobby : MonoBehaviourPunCallbacks
{
    [Header("Main Panels")]
    [SerializeField] private GameObject usernamePanel;
    [SerializeField] private GameObject lobbyPanel;
    [SerializeField] private GameObject waitingPanel;
    
    [Header("Username Panel Stuff")]
    [SerializeField] private TMP_InputField usernameInput;

    [Header("Lobby Panel Stuff")]
    [SerializeField] private CreateRoom creatingRooms;
    [SerializeField] private JoinRoom joiningRooms;
    [SerializeField] private WaitingInRooms waitingInRooms;

    #region Unity Methods

    private void Start()
    {
        //Check if has the previous saved data of the Username and if so, then load it.
        if (PlayerPrefs.HasKey("usernameKey")) usernameInput.text = PlayerPrefs.GetString("usernameKey");

        //Make sure the Game timer Input Fields are Zero!
        creatingRooms.gameTimeMinutes.text = "00";
        creatingRooms.gameTimeSeconds.text = "00";
    }

    private void Update()
    {
        creatingRooms.maxPlayers = (int) creatingRooms.valueSlider.value; //Set the Max Players value to the Slider Max Players value!
        creatingRooms.sliderValueText.text = creatingRooms.valueSlider.value.ToString(); //Set the Slider value Text value to the Slider Max Players value!
    }

    #endregion

    #region Public Methods
    
    //Executed on Username Input Field Changes its text value in the Username panel.
    public void UpdateUsername()
    {
        PhotonNetwork.NickName = usernameInput.text; //Set the default Nickname of the Player and get the value whenever we need it!
        PlayerPrefs.SetString("usernameKey", usernameInput.text); //Save the Username of the Player to load it next time!
    }

    //Executed on clicking the "Play!" button in the Username panel!
    public void Play()
    {
        //Load the Lobby Panel since we got the Username input.
        usernamePanel.SetActive(false);
        lobbyPanel.SetActive(true);
    }
    
    //Executed on clicking the "Create Room!" button in the Lobby panel!
    public void CreateRoom()
    {
        if (string.IsNullOrWhiteSpace(creatingRooms.createInput.text)) return; //Make sure not null or has spaces
        
        RoomOptions roomOptions = new RoomOptions(); //Create Room Options variable.
        roomOptions.MaxPlayers = (byte) creatingRooms.maxPlayers; //Set the Room Options Max Players value to the Max Players value.

        
        PhotonNetwork.CreateRoom(creatingRooms.createInput.text, roomOptions, TypedLobby.Default); //Finally get the Room Option values and create the Room!
        Debug.Log("Creating Room " + creatingRooms.createInput.text + "..."); //Debugging
    }

    //Executed in the "JoinRoom" method in the RoomItem script
    public void JoinRoom(string roomName)
    {
        PhotonNetwork.JoinRoom(roomName); //Join the Room
    }

    
    //Executed when "Start Game!" Button is Clicked in the Player waiting List!
    public void StartGame()
    {
        PhotonNetwork.LoadLevel("Game"); //Load the Game Scene!
    }
    
    //Executed when "Leave Room" Button is Clicked in the Player waiting List!
    public void LeaveRoom()
    {
        //Leave the Room!
        PhotonNetwork.LeaveRoom();
        
        //Disable all the Panels except the Username Panel
        waitingPanel.SetActive(false);
        lobbyPanel.SetActive(false);
        usernamePanel.SetActive(true);

        PhotonNetwork.JoinLobby(); //Make sure to Join the Lobby to get the Room List Updates!
    }
    
    //Executed to Filter the Game Time Minutes InputField 
    public void FilterGameTimeMinutes()
    {
        //Make sure we are only allowing the user to type in numbers
        creatingRooms.gameTimeMinutes.characterValidation = TMP_InputField.CharacterValidation.Digit;
        
        //Make sure the user is only allowed to use the numbers from 0 to 60 (you can change it)
        if (int.Parse(creatingRooms.gameTimeMinutes.text) > 60f)
        {
            creatingRooms.gameTimeMinutes.text = "60";
        }
        
        //Set the default text to "00" if there is no input
        if (creatingRooms.gameTimeMinutes.text == null)
        {
            creatingRooms.gameTimeMinutes.text = "00";
        }
    }
    
    //Executed to Filter the Game Time Seconds InputField 
    public void FilterGameTimeSeconds()
    {
        //Make sure we are only allowing the user to type in numbers
        creatingRooms.gameTimeSeconds.characterValidation = TMP_InputField.CharacterValidation.Digit;
        
        //Make sure the user is only allowed to use the numbers from 0 to 60 (you can change it)
        if (int.Parse(creatingRooms.gameTimeSeconds.text) > 60f)
        {
            creatingRooms.gameTimeSeconds.text = "60";
        }
        
        //Set the default text to "00" if there is no input
        if (creatingRooms.gameTimeSeconds.text == null)
        {
            creatingRooms.gameTimeSeconds.text = "00";
        }
    }

    //Basic Time Conversion
    public void GetSecondsFromMinutesAndSeconds()
    {
        float minutes = float.Parse(creatingRooms.gameTimeMinutes.text); //Get the minutes
        float seconds = float.Parse(creatingRooms.gameTimeSeconds.text); //Get the seconds

        //Calculate!
        float totalMinutes = (0f * 60f) + minutes + (seconds / 60f); 
        float totalSeconds = totalMinutes * 60f;
    }

    #endregion

    #region Private Methods

    private void UpdateRoomList(List<RoomInfo> roomList)
    {
        //Delete all the Rooms Displayed so we can load them back!
        foreach (RoomItem item in joiningRooms.roomItemsList)
        {
            Destroy(item.gameObject); //Destroy
        }
        joiningRooms.roomItemsList.Clear(); //Clear the Room List

        //Add all the Rooms Deleted with the newly available Rooms!
        foreach (RoomInfo room in roomList)
        {
            //Move on if the Room was Removed from the List
            if(room.RemovedFromList)
                continue;
            
            RoomItem newRoom = Instantiate(joiningRooms.roomItemPrefab, joiningRooms.roomContentObject); //Spawn the New Room
            newRoom.Setup(room); //Set the Room Name of the Room!
            joiningRooms.roomItemsList.Add(newRoom); //Add the New Room into the Room List!
            
            if (room.PlayerCount == 0) //If Room count is 0
            {
                PhotonNetwork.Destroy(newRoom.gameObject); //Destroy the Room
                joiningRooms.roomItemsList.Remove(newRoom); //Remove the Room from the Room List
            }
        }
        
        Debug.Log("Updated Room List!"); //Debugging
    }
    
    #endregion

    #region Photon Methods

    //Executed on Successfully Joined Room
    public override void OnJoinedRoom()
    {
        Debug.Log("Successfully Created/Joined the Room " + creatingRooms.createInput.text); //Debugging

        //Load the Waiting Panel and Disable the Lobby Panel
        lobbyPanel.SetActive(false);
        waitingPanel.SetActive(true);

        waitingInRooms.playerRoomNameText.text = PhotonNetwork.CurrentRoom.Name; //Set the Room Name

        Player[] players = PhotonNetwork.PlayerList; //Get the Player List using an Array of Players
        
        //Delete all the Player Objects Displayed so we can load them back!
        foreach (Transform item in waitingInRooms.playerContentObject)
        {
            Destroy(item.gameObject); //Destroy
        }

        for (int i = 0; i < players.Length; i++)
        {
            Instantiate(waitingInRooms.playerItemPrefab, waitingInRooms.playerContentObject).GetComponent<PlayerListItem>().Setup(players[i]); //Spawn the player in the Player List!
        }
        
        waitingInRooms.startGameButton.SetActive(PhotonNetwork.IsMasterClient); //Make sure only the Host can start the Game xD
    }

    //Executed when the Host switches
    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        waitingInRooms.startGameButton.SetActive(PhotonNetwork.IsMasterClient); //Make sure only the Host can start the Game xD
    }

    //Executed on Creating Room Failed
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("Failed Creating Room. Error: " + message + ". Return Code: " + returnCode); //Debugging
    }

    //Executed on the Room List Update
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {

        if (Time.time >= joiningRooms.nextUpdateTime)
        {
            UpdateRoomList(roomList); //Update the Room List in a separate method!
            joiningRooms.nextUpdateTime = Time.time + joiningRooms.timeBetweenUpdates;
        }
    }
    
    //Executed on Player Enters the Room
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Instantiate(waitingInRooms.playerItemPrefab, waitingInRooms.playerContentObject).GetComponent<PlayerListItem>().Setup(newPlayer); //Spawn the player in the Player List!
    }

    #endregion
}

#region Serialized Classes

[System.Serializable]
public class CreateRoom
{
    [Header("Room Settings")]
    public int maxPlayers;
    
    [Header("References")]
    public TMP_InputField createInput;
    public TMP_Text sliderValueText;
    public Slider valueSlider;
    [Space]
    public TMP_InputField gameTimeMinutes;
    public TMP_InputField gameTimeSeconds;
}

[System.Serializable]
public class JoinRoom
{
    [Header("Script References")]
    public RoomItem roomItemPrefab;
    
    [Header("References")]
    public List<RoomItem> roomItemsList;
    public Transform roomContentObject;

    [Header("Settings")]
    public float timeBetweenUpdates = 1.5f;
    [HideInInspector] public float nextUpdateTime;
}

[System.Serializable]
public class WaitingInRooms
{
    [Header("References")]
    public GameObject playerItemPrefab;
    public Transform playerContentObject;
    public TMP_Text playerRoomNameText;
    [Space]
    public GameObject startGameButton;

    [HideInInspector] public int totalGameTimeInSeconds;
}

#endregion