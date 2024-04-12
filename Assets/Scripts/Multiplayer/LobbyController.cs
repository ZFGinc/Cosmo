using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Steamworks;
using UnityEngine.UI;
using System.Linq;

public class LobbyController : MonoBehaviour
{
    public ulong CurrentlobbyId {get; private set;}

    [SerializeField] private Text _lobbyNameText;
    [SerializeField] private Transform _playerListViewContent;
    [SerializeField] private GameObject _playerIconItemPrefab;

    private GameObject _localCharacter;
    private CharacterSteam _character;

    private bool _playerItemCreated = false;
    private List<PlayerIconItem> _playerIconItems = new();

    private CustomNetworkManager _networkManager;
    private CustomNetworkManager Manager
    {
        get
        {
            if (_networkManager != null) return _networkManager;
            return _networkManager = CustomNetworkManager.singleton as CustomNetworkManager;
        }
    }

    public static LobbyController Instance;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(this.gameObject);
    }

    public void UpdateLobbyName()
    {
        CurrentlobbyId = Manager.GetComponent<SteamLobby>().CurrentLobbyId;
        _lobbyNameText.text = SteamMatchmaking.GetLobbyData(
            new CSteamID(CurrentlobbyId), 
            "name"
        );
    }

    public void UpdatePlayerList()
    {
        if (!_playerItemCreated) CreateHostPlayerItem();
        if(_playerIconItems.Count < Manager.Characters.Count) CreateClientPlayerItem();
        if (_playerIconItems.Count > Manager.Characters.Count) RemovePlayerItem();
        if (_playerIconItems.Count == Manager.Characters.Count) UpdatePlayerItem();
    }

    public void FindLocalPlayer() 
    {
        _localCharacter = GameObject.Find("LocalCharacter");
        _character = _localCharacter.GetComponent<CharacterSteam>();
    }

    public void CreateHostPlayerItem()
    {
        foreach(CharacterSteam character in Manager.Characters)
        {
            GameObject NewPlayerIconItem = Instantiate(_playerIconItemPrefab) as GameObject;
            PlayerIconItem NewPlayerIconItemScript = NewPlayerIconItem.GetComponent<PlayerIconItem>();

            NewPlayerIconItemScript.PlayerName = character.PlayerName;
            NewPlayerIconItemScript.ConnectionID = character.ConnectionID;
            NewPlayerIconItemScript.PlayerSteamID = character.PlayerSteamID;

            NewPlayerIconItemScript.SetPlayerValues();

            NewPlayerIconItem.transform.SetParent(_playerListViewContent);
            NewPlayerIconItem.transform.localScale = Vector3.one;

            _playerIconItems.Add(NewPlayerIconItemScript);
        }
        _playerItemCreated = true;
    }

    public void CreateClientPlayerItem()
    {
        foreach (CharacterSteam character in Manager.Characters)
        {
            if(!_playerIconItems.Any(b => b.ConnectionID == character.ConnectionID)){
                GameObject NewPlayerIconItem = Instantiate(_playerIconItemPrefab) as GameObject;
                PlayerIconItem NewPlayerIconItemScript = NewPlayerIconItem.GetComponent<PlayerIconItem>();

                NewPlayerIconItemScript.PlayerName = character.PlayerName;
                NewPlayerIconItemScript.ConnectionID = character.ConnectionID;
                NewPlayerIconItemScript.PlayerSteamID = character.PlayerSteamID;

                NewPlayerIconItemScript.SetPlayerValues();

                NewPlayerIconItem.transform.SetParent(_playerListViewContent);
                NewPlayerIconItem.transform.localScale = Vector3.one;

                _playerIconItems.Add(NewPlayerIconItemScript);
            }
        }
    }

    public void UpdatePlayerItem() 
    {
        foreach (CharacterSteam character in Manager.Characters)
        {
            foreach(PlayerIconItem playerIcon in _playerIconItems)
            {
                if(playerIcon.ConnectionID == character.ConnectionID)
                {
                    playerIcon.PlayerName = character.PlayerName;
                    playerIcon.SetPlayerValues();
                }
            }
        }
    }

    public void RemovePlayerItem()
    {
        List<PlayerIconItem> listToRemove = new();

        foreach(PlayerIconItem item in _playerIconItems)
        {
            if(!Manager.Characters.Any(b=>b.ConnectionID == item.ConnectionID))
            {
                listToRemove.Add(item);
            }
        }
        if (listToRemove.Count == 0) return;
        
        foreach (PlayerIconItem item in listToRemove)
        {
            GameObject objectToRemove = item.gameObject;
            listToRemove.Remove(item);
            Destroy(objectToRemove);
            objectToRemove = null;
        }
    }
}
