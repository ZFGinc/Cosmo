using Mirror;
using Steamworks;
using TMPro;
using UnityEngine;

public class CharacterSteam : NetworkBehaviour
{
    [SerializeField] private TMP_Text _textPlayerName;
    [Space(10)]
    [SyncVar, HideInInspector] public int ConnectionID;
    [SyncVar, HideInInspector] public int PlayerIdNumber;
    [SyncVar, HideInInspector] public ulong PlayerSteamID;
    [SyncVar(hook = nameof(PlayerNameUpdate)), HideInInspector] public string PlayerName;

    private CustomNetworkManager _networkManager;

    private CustomNetworkManager Manager
    {
        get
        {
            if (_networkManager != null) return _networkManager;
            return _networkManager = CustomNetworkManager.singleton as CustomNetworkManager;
        }
    }

    [Command]
    private void CmdSetPlayerName(string playerName)
    {
        this.PlayerNameUpdate(this.PlayerName, playerName);
    }

    public override void OnStartAuthority()
    {
        CmdSetPlayerName(SteamFriends.GetPersonaName().ToString());
        gameObject.name = "LocalCharacter";
        LobbyController.Instance.FindLocalPlayer();
        LobbyController.Instance.UpdateLobbyName();
    }

    public override void OnStartClient()
    {
        Manager.Characters.Add(this);
        LobbyController.Instance.UpdateLobbyName();
        LobbyController.Instance.UpdatePlayerList();
    }

    public override void OnStopClient()
    {
        Manager.Characters.Remove(this);
        LobbyController.Instance.UpdateLobbyName();
    }

    public void PlayerNameUpdate(string oldValue, string newValue)
    {
        if (isServer)
        {
            this.PlayerName = newValue;
            _textPlayerName.gameObject.SetActive(false);
        }

        if (isClient)
        {
            LobbyController.Instance.UpdatePlayerList();
        }

        _textPlayerName.text = newValue;
    }
}
