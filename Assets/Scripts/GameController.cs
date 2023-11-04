using System;
using System.Collections;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] private Tetromino m_Tetromino;

    private void Start()
    {
        StartCoroutine(MoveDownRoutine());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
            m_Tetromino.TryMoveLeft();
        else if (Input.GetKeyDown(KeyCode.D))
            m_Tetromino.TryMoveRight();
        else if (Input.GetKeyDown(KeyCode.R))
            m_Tetromino.TryRotate();
    }
    
    private IEnumerator MoveDownRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.5f);
            m_Tetromino.TryMoveDown();
        }
    }
}
