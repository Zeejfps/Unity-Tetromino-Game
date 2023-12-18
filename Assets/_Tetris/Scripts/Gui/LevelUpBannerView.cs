using System.Collections;
using UnityEngine;

public sealed class LevelUpBannerView : MonoBehaviour
{
    public void FlashAnimated()
    {
        gameObject.SetActive(true);
        StartCoroutine(FlashRoutine());
    }

    private IEnumerator FlashRoutine()
    {
        yield return new WaitForSeconds(0.5f);
        gameObject.SetActive(false);
    }
}
