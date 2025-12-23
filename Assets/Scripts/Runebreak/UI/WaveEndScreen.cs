using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;

public class WaveEndScreen : MonoBehaviour
{
    [SerializeField] private CanvasGroup _containerGroup;
    [SerializeField] private CanvasGroup _rootGroup;
    
    public void Show()
    {
        _rootGroup.alpha = 0;
        _containerGroup.alpha = 0;
        Reveal();
    }

    private void Reveal()
    {
        gameObject.SetActive(true);
        _rootGroup.DOFade(1f, 0.2f).OnComplete(() =>
        {
            if (_containerGroup != null)
            {
                _containerGroup.DOFade(1f, 0.1f).OnComplete(() =>
                {
                    StartCoroutine(HideAfterDelay(3.5f));
                });
            }
        });
    }

    private IEnumerator HideAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (gameObject != null)
        {
            gameObject.SetActive(false);
        }
    }
}
