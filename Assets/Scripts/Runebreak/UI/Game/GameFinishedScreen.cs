using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class GameFinishedScreen : MonoBehaviour
{
    [SerializeField] private CanvasGroup _containerGroup;
    [SerializeField] private CanvasGroup _rootGroup;
    [SerializeField] private Button _returnToMenuButton;

    
    public void Show()
    {
        _rootGroup.alpha = 0;
        _containerGroup.alpha = 0;
        Reveal();
        
    }

    private void OnEnable()
    {
        _returnToMenuButton.onClick.AddListener(HandleReturnToMenu);
    }
    
    private void OnDisable()
    {
        _returnToMenuButton.onClick.RemoveListener(HandleReturnToMenu);
    }

    private void HandleReturnToMenu()
    {
        EventBus.Publish(new ReturnToMenuEvent());
    }

    private void Reveal()
    {
        gameObject.SetActive(true);
        _rootGroup.DOFade(1f, 0.5f).OnComplete(() =>
        {
            if (_containerGroup != null)
            {
                _containerGroup.DOFade(1f, 0.3f);
            }
        });
    }
}
