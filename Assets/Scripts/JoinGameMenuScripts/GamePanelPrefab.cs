using UnityEngine;
using Fusion;
using UnityEngine.UI;
using TMPro;

public class GamePanelPrefab : MonoBehaviour
{
    [SerializeField] private TMP_Text gameNameText;
    [SerializeField] private TMP_Text playersCountText;
    public SessionInfo session;

    public void Initialise(SessionInfo session)
    {
        this.session = session;
        gameNameText.text = session.Name;
        playersCountText.text = $"{session.PlayerCount}/{session.MaxPlayers}";
    }

    public void JoinClick()
    {
        NetworkController.me.ConnectToSession(session);
    }
}
