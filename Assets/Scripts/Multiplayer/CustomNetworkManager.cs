using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Steamworks;
using NaughtyAttributes;

public class CustomNetworkManager : NetworkManager
{
    public List<CharacterSteam> Characters { get; } = new List<CharacterSteam>();

    [SerializeField, Required] private SteamLobby _steamLobby;
    [Space(10)]
    [SerializeField] private CharacterSteam _characterSteamPrefab;

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        CharacterSteam character = Instantiate(_characterSteamPrefab);
        character.ConnectionID = conn.connectionId;
        character.PlayerIdNumber = Characters.Count + 1;
        character.PlayerSteamID = (ulong)SteamMatchmaking.GetLobbyMemberByIndex(
            (CSteamID)_steamLobby.CurrentLobbyId,
            Characters.Count
        );

        NetworkServer.AddPlayerForConnection(conn, character.gameObject);
    }
}
