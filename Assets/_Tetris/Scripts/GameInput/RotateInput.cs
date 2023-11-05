﻿using UnityEngine;

sealed class RotateInput : PlayingStateGameInput
{
    private readonly ITouchGestureDetector m_TouchGestureDetector;

    public RotateInput(IGameStateMachine gameStateMachine, ITouchGestureDetector touchGestureDetector)
        : base(gameStateMachine)
    {
        m_TouchGestureDetector = touchGestureDetector;
    }

    protected override void OnUpdate()
    {
        if (Input.GetKeyDown(KeyCode.R) ||
            Input.GetKeyDown(KeyCode.UpArrow) ||
            Input.GetKeyDown(KeyCode.W) ||
            m_TouchGestureDetector.TouchDetected())
        {
            OnPerformed();
        }
    }
}