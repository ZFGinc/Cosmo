using UnityEngine;
using Mirror;
using Steamworks;

[RequireComponent(typeof(CustomNetworkManager))]
public class SteamLobby : MonoBehaviour
{
    public ulong CurrentLobbyId { get; private set; } = 0;

    private CustomNetworkManager _networkManager;

    protected Callback<LobbyCreated_t> _onLobbyCreated;
    protected Callback<GameLobbyJoinRequested_t> _onGameLobbyJoinRequested;
    protected Callback<LobbyEnter_t> _onLobbyEntered;

    private const string HostAddressKey = "HostAddress";

    private void Start()
    {
        _networkManager = GetComponent<CustomNetworkManager>();

        if (!SteamManager.Initialized) return;

        _onLobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
        _onGameLobbyJoinRequested = Callback<GameLobbyJoinRequested_t>.Create(OnGameLobbyJoinRequested);
        _onLobbyEntered = Callback<LobbyEnter_t>.Create(OnLobbyEntered);

        HostLobby();
    }

    private void OnLobbyCreated(LobbyCreated_t callback)
    {
        if (callback.m_eResult != EResult.k_EResultOK) return;

        Debug.Log("Lobby created succesfully");

        _networkManager.StartHost();

        SteamMatchmaking.SetLobbyData(
            new CSteamID(callback.m_ulSteamIDLobby),
            HostAddressKey,
            SteamUser.GetSteamID().ToString()
        );

        SteamMatchmaking.SetLobbyData(
            new CSteamID(callback.m_ulSteamIDLobby),
            "name",
            SteamFriends.GetPersonaName().ToString() + "`s LOBBY"
        );
    }

    private void OnGameLobbyJoinRequested(GameLobbyJoinRequested_t callback)
    {
        Debug.Log("Request to Join Lobby");

        LeaveLobby();
        SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);
    }

    private void OnLobbyEntered(LobbyEnter_t callback)
    {
        //Everyone
        CurrentLobbyId = callback.m_ulSteamIDLobby;

        //Clients
        if (NetworkServer.active) return;

        string hostAddress = SteamMatchmaking.GetLobbyData(
            new CSteamID(callback.m_ulSteamIDLobby),
            HostAddressKey
        );

        _networkManager.networkAddress = hostAddress;
        _networkManager.StartClient();

        Debug.Log($"Join Lobby: {hostAddress}");
    }

    public void HostLobby()
    {
        LeaveLobby();
        SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypeFriendsOnly, _networkManager.maxConnections);
    }

    public void LeaveLobby()
    {
        if (NetworkServer.active) _networkManager.StopHost();
        else _networkManager.StopClient();
    }

    public void ConnectToFriend()
    {
        SteamFriends.ActivateGameOverlay("friends");
    }
}
