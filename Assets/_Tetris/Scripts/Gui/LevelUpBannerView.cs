using System.Collections;
using UnityEngine;

public sealed class LevelUpBannerView : MonoBehaviour
{
    [SerializeField] private float m_VisibleFor = 1.25f;
    
    public void FlashAnimated()
    {
        gameObject.SetActive(true);
        StartCoroutine(FlashRoutine());
    }

    private IEnumerator FlashRoutine()
    {
        yield return new WaitForSeconds(m_VisibleFor);
        gameObject.SetActive(false);
    }
}
