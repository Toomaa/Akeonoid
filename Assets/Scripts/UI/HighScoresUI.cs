using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HighScoresUI : MonoBehaviour
{
    [SerializeField]
    private List<HighScoreEntryUI> _entries;

    private void Start()
    {
        // get ui items for displaying high scores
        _entries = gameObject.GetComponentsInChildren<HighScoreEntryUI>().ToList();

        for (int i = 0; i < _entries.Count; i++)
        {
            DataManager.HighScoreEntry entry = DataManager.Instance.HighScores.entries[i];
            _entries[i].Setup(i, entry.name, entry.score);

            if (i == DataManager.Instance.RankIndexAchieved)
            {
                _entries[i].EnableInput();
            }
        }
    }

    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene(0);
    }
}
