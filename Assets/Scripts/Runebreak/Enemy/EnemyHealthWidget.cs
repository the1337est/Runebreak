using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class EnemyHealthWidget : MonoBehaviour
{

    [SerializeField] private Slider _slider;

    public void SetHealthNormalized(float val)
    {
        var clamped = Mathf.Clamp01(val);
        _slider.value = clamped;
        gameObject.SetActive(clamped < 1f && clamped > 0f);
    }
}
