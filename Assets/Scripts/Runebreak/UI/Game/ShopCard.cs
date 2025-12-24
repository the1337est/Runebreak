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


    [SerializeField] private Image _borderImage;
    [SerializeField] private Image _backgroundImage;
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
        if(!_buyButton.interactable) return;
        EventBus.Publish(new ShopBuyEvent(_item));
        EventBus.Publish(new ShopCardDisposeEvent(this));
    }

    public void Set(UpgradeSO item, ItemColorSet itemColorSet)
    {
        _item = item;
        _titleText.text = item.Name;
        _subtitleText.text = item.Rarity.ToString().ToUpper();
        _descriptionText.text = GetDescription();
        _costText.text = item.BaseCost.ToString();
        _buyButton.interactable = Player.Instance.Resources.Get(ResourceType.Coins) >= _item.BaseCost;
        _borderImage.color = itemColorSet.BorderColor;
        _backgroundImage.color = itemColorSet.BackgroundColor;
        _subtitleText.color = itemColorSet.RarityColor;
    }

    public void UpdateInteractability(float coins)
    {
        _buyButton.interactable = coins >= _item.BaseCost;
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
