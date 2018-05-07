using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class PointsText : MonoBehaviour
{
    [SerializeField]
    private PlayerController playerController;
    [SerializeField]
    private Text pointTextLabel;
    [SerializeField]
    private string format = "{0} <size=9>pts</size>";
    private Image backgroundImage;

    private void Start()
    {
        backgroundImage = GetComponent<Image>();
        playerController.PlayerPointsChanged += OnPlayerPointsChanged;
        OnPlayerPointsChanged(this, new PlayerPointsChangedEventArgs(playerController, playerController.Points, playerController.Points));
    }

    private void OnPlayerPointsChanged(object sender, PlayerPointsChangedEventArgs args)
    {
        StartCoroutine(TransitionText(args.OldPoints, args.NewPoints));
        float newWidth = GetLengthOfPointsText(string.Format(format, args.NewPoints)) + 5;
        if (newWidth < 10)
        {
            newWidth = backgroundImage.rectTransform.sizeDelta.x;
        }

        backgroundImage.rectTransform.sizeDelta = new Vector2(newWidth, backgroundImage.rectTransform.sizeDelta.y);
    }

    private IEnumerator TransitionText(int start, int destination)
    {
        const float transitionTime = 1;
        float elapsedTime = 0;

        int currentPoints = start;
        while (elapsedTime < transitionTime)
        {
            currentPoints = Mathf.FloorToInt(Mathf.Lerp(currentPoints, destination, elapsedTime / transitionTime));
            pointTextLabel.text = string.Format(format, currentPoints);

            elapsedTime += Time.deltaTime;


            yield return new WaitForEndOfFrame();
        }
    }

    private int GetLengthOfPointsText() => GetLengthOfPointsText(pointTextLabel.text);
    private int GetLengthOfPointsText(string message)
    {
        int totalLength = 0;
        Font font = pointTextLabel.font;
        char[] characters = message.ToCharArray();

        foreach (char character in characters)
        {
            CharacterInfo characterInfo;
            font.GetCharacterInfo(character, out characterInfo, pointTextLabel.fontSize);
            totalLength += characterInfo.advance;
        }

        return totalLength;
    }
}
