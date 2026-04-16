using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace ETSimpleKit
{
    public class ScoreMachine
    {
        //
        // Current field for case that only need to display current best score index
        //
        public int currentWeek;
        public int bestScoreWeek;
        public int bestScore;
        //
        //
        //
        public int GetbestScore(string playerPrefIndex) => PlayerPrefs.GetInt(playerPrefIndex);
        /// <summary>
        /// Try set score if larger, force will replace. Return true if best.
        /// </summary>
        /// <param name="force"></param>
        public bool CheckScore(int score, string playerPrefIndex = "bestScore", bool force = false)
        {
            if (force)
            {
                bestScore = score;
                PlayerPrefs.SetInt(playerPrefIndex, bestScore);
                return true;
            }
            if (PlayerPrefs.HasKey(playerPrefIndex))
            {
                bestScore = PlayerPrefs.GetInt(playerPrefIndex);
                if (bestScore < score)
                {
                    bestScore = score;
                    PlayerPrefs.SetInt(playerPrefIndex, bestScore);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                bestScore = score;
                PlayerPrefs.SetInt(playerPrefIndex, bestScore);
                return true;
            }
        }
        /// <summary>
         /// Try set score if larger, force will replace. Return if best.
         /// </summary>
         /// <param name="force"></param>
        public bool SetBestScoreWeek(int score, bool force = false)
        {
            if (force)
            {
                bestScoreWeek = score;
                PlayerPrefs.SetInt("bestWeek", bestScoreWeek);
            }
            if (PlayerPrefs.HasKey("bestWeek"))
            {
                bestScoreWeek = PlayerPrefs.GetInt("bestWeek");
                if (bestScoreWeek < score)
                {
                    bestScoreWeek = score;
                    PlayerPrefs.SetInt("bestWeek", bestScoreWeek);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                bestScoreWeek = score;
                PlayerPrefs.SetInt("bestWeek", bestScoreWeek);
                return true;
            }
        }
        protected int GetCurrentWeekOfYear()
        {
            DateTime currentDate = DateTime.Now;
            CultureInfo ci = CultureInfo.CurrentCulture;
            int weekOfYear = ci.Calendar.GetWeekOfYear(currentDate, ci.DateTimeFormat.CalendarWeekRule, ci.DateTimeFormat.FirstDayOfWeek);
            return weekOfYear;
        }
    }
}

