using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShopCard : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _titleText;
    [SerializeField] private TextMeshProUGUI _subtitleText;
    [SerializeField] private TextMeshProUGUI _descriptionText;
    [SerializeField] private TextMeshProUGUI _costText;

    [SerializeField] private Button _buyButton;
    
    private UpgradeSO _item;

    private void OnEnable()
    {
        _buyButton.onClick.AddListener(HandleBuyClick);
    }
    
    private void OnDisable()
    {
        _buyButton.onClick.RemoveListener(HandleBuyClick);
    }

    private void HandleBuyClick()
    {
        Debug.Log($"Buy clicked: {name} / {_item.Name}");
        if(!_buyButton.interactable) return;
        EventBus.Publish(new ShopBuyEvent(_item));
    }

    public void Set(UpgradeSO item)
    {
        _item = item;
        _titleText.text = item.Name;
        _subtitleText.text = item.Rarity.ToString();
        _descriptionText.text = GetDescription();
        _costText.text = item.BaseCost.ToString();
    }

    public void UpdateInteractability(float coins)
    {
        _buyButton.interactable = coins <= _item.BaseCost;
    }

    private string GetDescription()
    {
        string result = string.Empty;
        foreach (var change in _item.Changes)
        {
            result += change + "\n\n";
        }
        return result;
    }

    public void Select()
    {
        EventSystem.current.SetSelectedGameObject(_buyButton.gameObject);
    }
}
