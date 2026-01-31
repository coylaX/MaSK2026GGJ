using UnityEngine;

public class LineRangeVisualizer : MonoBehaviour
{
    public LineRenderer closeLine; 
    public LineRenderer farLine;   

    [Header("粗细设置")]
    public float visualWidth = 0.025f; // 已经减半
    public int segments = 72;

    void Update()
    {
        if (PlayerBuff.PlayerBuffInstance == null) return;

        float pScale = transform.lossyScale.x;
        if (pScale == 0) pScale = 1f;

        float finalLocalWidth = visualWidth / pScale;

        // --- 近距离红圈 (Inside) ---
        if (PlayerBuff.PlayerBuffInstance.attackCloseEnemy)
        {
            float targetR = 1.5f;
            // Inside: 路径向内偏移半个线宽，边缘刚好在3单位处
            float drawR = (targetR - (visualWidth / 2f)) / pScale;
            UpdateLine(closeLine, drawR, finalLocalWidth);
        }
        else if(closeLine.enabled) closeLine.enabled = false;

        // --- 远距离绿圈 (Outside) ---
        if (PlayerBuff.PlayerBuffInstance.attackFarEnemy)
        {
            float targetR = 3f;
            // Outside: 路径向外偏移半个线宽，内边缘刚好在5单位处
            float drawR = (targetR + (visualWidth / 2f)) / pScale;
            UpdateLine(farLine, drawR, finalLocalWidth);
        }
        else if(farLine.enabled) farLine.enabled = false;
    }

    void UpdateLine(LineRenderer line, float radius, float width)
    {
        if (!line.enabled) line.enabled = true;
        line.startWidth = width;
        line.endWidth = width;
        line.positionCount = segments;
        for (int i = 0; i < segments; i++)
        {
            float angle = i * (2 * Mathf.PI / segments);
            line.SetPosition(i, new Vector3(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius, 0));
        }
    }
}