﻿using System;

public interface IGameScore
{
    event Action<int> PointsChanged; 
    
    int TotalPoints { get; }
    int BestPoints { get; }

    void IncreasePoints(int points);
    void ResetPoints();
}