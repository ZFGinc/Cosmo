using Steamworks;
using UnityEngine.UI;
using UnityEngine;

public class PlayerIconItem : MonoBehaviour
{
    public string PlayerName;
    public int ConnectionID;
    public ulong PlayerSteamID;
    public bool AvatarReceived;

    [SerializeField] private RawImage _playerIcon;

    protected Callback<AvatarImageLoaded_t> ImageLoaded;

    private void Start()
    {
        ImageLoaded = Callback<AvatarImageLoaded_t>.Create(OnImageLoaded);
    }

    private void OnImageLoaded(AvatarImageLoaded_t callback)
    {
        if (callback.m_steamID.m_SteamID != PlayerSteamID) return;

        _playerIcon.texture = GetSteamImageAsTexture(callback.m_iImage);
    }

    private Texture2D GetSteamImageAsTexture(int iImage)
    {
        Texture2D texture = null;

        bool isValide = SteamUtils.GetImageSize(iImage, out uint width, out uint height);

        if(isValide)
        {
            byte[] image = new byte[width * height * 4];

            isValide = SteamUtils.GetImageRGBA(iImage, image, (int)(width * height * 4));

            if (isValide)
            {
                texture = new Texture2D((int)width, (int)height, TextureFormat.ARGB32, false, true);
                texture.LoadRawTextureData(image);
                texture.Apply();
            }
        }

        AvatarReceived = true;
        return texture;
    }

    private void GetPlayerIcon()
    {
        int imageID = SteamFriends.GetLargeFriendAvatar((CSteamID)PlayerSteamID);
        if(imageID == -1) return;

        Debug.Log(imageID);

        _playerIcon.texture = GetSteamImageAsTexture(imageID);
    }

    public void SetPlayerValues()
    {
        if(AvatarReceived) return;

        GetPlayerIcon();
    }
}
