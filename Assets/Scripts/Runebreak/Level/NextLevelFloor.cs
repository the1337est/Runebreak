using UnityEngine;

public class NextLevelFloor : Floor
{
    private void Update()
    {
        if(Target == null) return;
        
        float borderX = transform.position.x - (WorldSize.x / 2f);
        float fadeStartX = borderX - (WorldSize.x / 2f);
        float alpha = Mathf.InverseLerp(fadeStartX, borderX, Target.position.x);

        // 4. Apply Color
        Color color = FloorRenderer.color;
        color.a = alpha;
        SetColor(color);

        if (Target.position.x >= WorldSize.x / 2f + WorldSize.x / 4f)
        {
            EventBus.Publish(new LevelSwitchOverEvent());
        }
    }
}
