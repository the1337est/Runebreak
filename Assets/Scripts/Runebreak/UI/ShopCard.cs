using TMPro;
using UnityEngine;

public class ShopCard : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _titleText;
    [SerializeField] private TextMeshProUGUI _subtitleText;
    [SerializeField] private TextMeshProUGUI _descriptionText;
    [SerializeField] private TextMeshProUGUI _costText;

    private UpgradeSO _item;
    
    public void Set(UpgradeSO item)
    {
        _item = item;
        _titleText.text = item.Name;
        _subtitleText.text = item.Rarity.ToString();
        _descriptionText.text = GetDescription();
        _costText.text = item.BaseCost.ToString();
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
}
