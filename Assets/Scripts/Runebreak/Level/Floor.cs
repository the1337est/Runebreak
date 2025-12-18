using UnityEngine;

public class Floor : MonoBehaviour
{
    protected Transform Target;
    protected Vector2 WorldSize;

    [SerializeField] protected SpriteRenderer FloorRenderer;
    [SerializeField] protected SpriteRenderer WallRenderer;

    protected WaveData Data;
    
    public void Initialize(WaveData data)
    {
        if (data == null) return;
        Data = data;
        FloorRenderer.sprite = data.FloorSprite;
        WallRenderer.sprite = data.WallSprite;
    }

    private void Awake()
    {
        Target = Player.Instance.transform;
        WorldSize = LevelManager.Instance.WorldSize;
        WallRenderer.transform.localPosition = Vector2.up * WorldSize.y / 2;
    }

    protected void SetColor(Color color)
    {
        FloorRenderer.color = color;
        WallRenderer.color = color;
    }

}
