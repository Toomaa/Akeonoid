using TMPro;
using UnityEngine;

public class HighScoreEntryUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _numberText;
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private TMP_InputField _nameInput;
    [SerializeField] private TextMeshProUGUI _scoreText;

    private int _index = -1;
    private int _score = 0;

    // setup the item
    public void Setup(int number, string name, int score)
    {
        _index = number;
        _score = score;

        _numberText.text = (number + 1) + ".";
        _nameText.text = name;
        _scoreText.text = score.ToString();

        _nameInput.gameObject.SetActive(false);
    }

    // enable inputfield to set/chnage the name
    public void EnableInput()
    {
        _nameText.gameObject.SetActive(false);
        _nameInput.gameObject.SetActive(true);
        _nameInput.Select();
    }

    // finished writing the name (pressing enter)
    // update the high score entry and save high scores
    public void InputNameOnEndEdit(string name)
    {
        _nameInput.gameObject.SetActive(false);
        _nameInput.gameObject.SetActive(true);

        if (string.IsNullOrEmpty(name))
        {
            name = "Unnamed";
        }

        DataManager.Instance.HighScores.entries[_index].Set(name, _score);
        DataManager.Instance.SaveHighSores();
    }
}
