using System.Globalization;
using System;
using UnityEngine;
using UnityEngine.Events;

namespace ETSimpleKit
{
    /// <summary>
    /// curent score 
    /// weekly highest score
    /// highest score
    /// </summary>
    public class ScoreMachine_WeekBest: ScoreMachine
    {
        UnityEvent<int,int,int> _onLoadScore;
        public void Init(UnityEvent<int, int, int> onLoadScore)
        {
            _onLoadScore = onLoadScore;
        }
        public void LoadScore(int score)
        {
            if (PlayerPrefs.HasKey("currentWeek"))
            {
                currentWeek = PlayerPrefs.GetInt("currentWeek");
                if (currentWeek != GetCurrentWeekOfYear())
                {
                    currentWeek = GetCurrentWeekOfYear();
                    PlayerPrefs.SetInt("currentWeek", currentWeek);
                    SetBestScoreWeek(score,true);
                }
                else
                {
                    SetBestScoreWeek(score);
                }
            }
            else
            {
                currentWeek = GetCurrentWeekOfYear();
                PlayerPrefs.SetInt("currentWeek", currentWeek);
                SetBestScoreWeek(score, true);
            }
            CheckScore(score);
            _onLoadScore.Invoke(score, bestScoreWeek,bestScore);
        }
        
    }

}


