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
    public class ScoreMachine_Best : ScoreMachine 
    {
        UnityEvent<int,int> _onLoadScore;
        public void Init(UnityEvent<int, int> onLoadScore)
        {
            _onLoadScore = onLoadScore;
        }
        public void LoadScore(int score)
        {
            CheckScore(score);
            _onLoadScore.Invoke(score,bestScore);
        } 
    }

}


