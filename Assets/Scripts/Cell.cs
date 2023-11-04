﻿using System;
using System.Collections;
using UnityEngine;

public sealed class Cell : MonoBehaviour, ICell, IAnimate
{
    private int m_MoveDownCount;
    
    public void MoveDown()
    {
        m_MoveDownCount++;
    }

    private void Update()
    {
        if (m_MoveDownCount > 0)
        {
            var startPosition = transform.position;
            var targetPosition = transform.position + Vector3.down * m_MoveDownCount;
            this.Animate(0.25f, t => t, t =>
            {
                transform.position = Vector3.LerpUnclamped(startPosition, targetPosition, t);
            });
            m_MoveDownCount = 0;
        }
    }

    public void Destroy()
    {
        var startScale = transform.localScale;
        this.Animate(0.15f, t => t, t =>
        {
            transform.localScale = Vector3.LerpUnclamped(startScale, Vector3.zero, t);
        }, () =>
        {
            var go = gameObject;
            go.SetActive(false);
            Destroy(go);
        });
    }

    public Coroutine AnimationRoutine { get; set; }
}