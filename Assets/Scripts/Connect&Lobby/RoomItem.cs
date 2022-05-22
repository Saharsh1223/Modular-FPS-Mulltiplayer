using Photon.Realtime;
using TMPro;
using UnityEngine;

public class RoomItem : MonoBehaviour
{
    [SerializeField] private TMP_Text roomNameText;
    
    public string currentRoomName;
    private Lobby lobby;

    private void Start()
    {
        lobby = FindObjectOfType<Lobby>(); //Find the Script "Lobby"
    }

    //Sets the Room Name connecting with Lobby script!
    public void Setup(RoomInfo roomInfo)
    {
        string roomName = roomInfo.Name;
        
        //Set the RoomNameText's text variable with the Room Name and the Current Players with Max Players!
        roomNameText.text = roomName + " (" + roomInfo.PlayerCount + "/" + roomInfo.MaxPlayers + ")"; //Example: "EPIC ROOM (3/10)"
        currentRoomName = roomName; //Set the currentRoomName value to roomName value so that we could join the room using the currentRoomName value!
    }

    //Executed on the RoomItem Button is Clicked!
    public void JoinRoom()
    {
        lobby.JoinRoom(currentRoomName); //Join the Room!
    }
}
