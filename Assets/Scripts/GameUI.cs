using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameUI : MonoBehaviour
{
    [SerializeField]
    private List<Image> playersAvatar;
    [SerializeField]
    private List<TextMeshProUGUI> playersPoint;
    [SerializeField]
    private List<TextMeshProUGUI> passTexts;
    [SerializeField]
    private GameObject resultUI;
    [SerializeField]
    private List<Image> rankAvatar;
    [SerializeField]
    private List<TextMeshProUGUI> rankCardRemains;
    [SerializeField]
    private List<TextMeshProUGUI> rankPoint;
    [SerializeField]
    private TextMeshProUGUI winnerText;
    [SerializeField]
    private GameObject playerUI;
    [SerializeField]
    private TextMeshProUGUI whoseTurntext;
    [SerializeField]
    private TextMeshProUGUI comboText;
    private TurnController turnController;
    private IGameController gameController;

    private void Start()
    {
        InitializeAvatar();
        gameController = GetComponent<IGameController>();
        turnController = GetComponent<TurnController>();

        turnController.OnTurnChanged += UpdateTurnDisplay;
        turnController.OnComboSet += UpdateComboDisplay;
        turnController.OnPlayerPass += UpdatePassDisplay;
        turnController.GameEndEvent += GameOver;

    }
    private void InitializeAvatar()
    {
        for (int i = 0; i < playersAvatar.Count; i++)
            playersAvatar[i].sprite = AvatarController.Instance.GetAvatarData(i, AvatarMode.Normal);
    }    
    private void UpdateTurnDisplay(object sender, TurnController.OnTurnChangedEventArgs e)
    {
        whoseTurntext.text = "Player "+e.playerIndex+"'s turn";
    }
    private void UpdateComboDisplay(object sender, TurnController.OnComboSetEventArgs e)
    {
        comboText.text = e.comboName;
    }
    private void UpdatePassDisplay(object sender, TurnController.OnPlayerPassEventArgs e)
    {
        passTexts[e.playerIndex].gameObject.SetActive(e.isPass);
    }
    private void GameOver()
    {
        resultUI.SetActive(true);
        int index = 0;
        int points = 0;
        for(int i = 0; i < playersPoint.Count;i++) 
        {
            PlayerRecord record = gameController.GetPlayerRecords(i);
            playersPoint[i].text = record.totalPoints.ToString();
            if (record.thisRoundRemains >= 6)
            {
                rankAvatar[i].sprite = AvatarController.Instance.GetAvatarData(i, AvatarMode.Losing);
            }
            else if (record.thisRoundRemains > 2)
            {
                rankAvatar[i].sprite = AvatarController.Instance.GetAvatarData(i, AvatarMode.Normal);
            }
            else
                rankAvatar[i].sprite = AvatarController.Instance.GetAvatarData(i, AvatarMode.Winning);
            rankCardRemains[i].text = record.thisRoundRemains.ToString(); 
            rankPoint[i].text = record.thisRoundPoints.ToString();

            if (record.thisRoundPoints > points)
            {
                points = record.thisRoundPoints;
                index = i;
            }
        }

        winnerText.text = "PLAYER " + index + " WIN!!!";
    }
}
