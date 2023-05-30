using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance;

    public HighScoreData HighScores { get; private set; }
    
    // the rank (position) player achieved in the top 10 high scores
    public int RankIndexAchieved = -1;

    // Currently active theme (background)
    public int ActiveTheme = 0;

    private void Awake()
    {
        // singelton 
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        // persist on scene change
        DontDestroyOnLoad(gameObject);

        // load highscore data from json file
        LoadHighScores();
    }

    // is score amount enough to reach the hall of fame (top 10)
    public bool QualifiesForHighScore(int score)
    {        
        RankIndexAchieved = HighScores.GetRankingIndex(score);
        return RankIndexAchieved > -1;
    }

    public void InsertHighScore(int score)
    {
        if (RankIndexAchieved < 0)
        {
            return;
        }

        HighScores.Insert(RankIndexAchieved, "", score);
    }

    // load highscore data from json file (if exists)
    public void LoadHighScores()
    {
        string filename = GetHighScoresFilePath();
        
        if (File.Exists(filename))
        {
            string data = File.ReadAllText(filename);
            HighScores = JsonUtility.FromJson<HighScoreData>(data);
            HighScores.Sort();
            return;
        }

        // create an empty high score file
        SaveHighSores();
    }

    // save highscore data to json file
    public void SaveHighSores()
    {
        string data = HighScores.GetSaveData();
        File.WriteAllText(GetHighScoresFilePath(), data);
    }

    // get path to the highscore json file
    private string GetHighScoresFilePath()
    {
        return Path.Combine(Application.persistentDataPath, "highscores.json");
    }

    // returns name and score of the 1st place
    public string GetHighestScore()
    {
        if (HighScores.entries[0].score > 0)
        {
            return HighScores.entries[0].name + " : " + HighScores.entries[0].score;
        }

        return "0";
    }

    [Serializable]
    public class HighScoreData
    {
        public List<HighScoreEntry> entries;
        public static int MAX_ENTRIES = 10;

        // Sort the list by score, descending
        public void Sort()
        {
            entries.Sort((x, y) => y.score.CompareTo(x.score));
        }

        // returns the index of the first score that is less than this (param score)
        public int GetRankingIndex(int score)
        {
            for (int i = 0; i < entries.Count; i++)
            {
                if (entries[i].score == 0)
                {
                    return i;
                }

                if (entries[i].score < score)
                {
                    return i;
                }
            }

            // not enough score to qualify
            return -1;
        }

        // returns json string with sorted score
        // additional empty entries are added if there are less than 10 (MAX_ENTRIES)
        public string GetSaveData()
        {
            // create empty entries if needed
            int emptyEntriesToAdd = MAX_ENTRIES - entries.Count;
            if (emptyEntriesToAdd > 0)
            {
                for (int i = 0; i < emptyEntriesToAdd; i++)
                {
                    entries.Add(new HighScoreEntry("", 0));
                }
            }

            Sort();
            return JsonUtility.ToJson(this, true);
        }

        // inserts - replaces an item at provided index,
        // items from (including) the index are pushed (shifted) down, the lowest entry is removed
        public void Insert(int index, string name, int score)
        {
            if (index == MAX_ENTRIES - 1)
            {
                entries[index].Set(name, score);
                return;
            }

            // push the entries from (including) index down to the bottom
            for (int i = entries.Count - 1; i > index; i--)
            {
                int prevIndex = i - 1;
                entries[i].Set(entries[prevIndex].name, entries[prevIndex].score);
            }

            entries[index].Set(name, score);
        }
    }

    [Serializable]
    public class HighScoreEntry
    {
        public string name;
        public int score;

        public HighScoreEntry(string name, int score)
        {
            Set(name, score);
        }

        public void Set(string name, int score)
        {
            this.name = name;
            this.score = score;
        }
    }
}
