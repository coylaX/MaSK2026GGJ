using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class MonsterAI_AStar : MonoBehaviour {
    private MonsterBase stats;
    private Rigidbody2D rb;
    private Transform player;
    private RoomNavGraph navGraph;

    private Vector2 currentTargetWorldPos;
    private Vector3 lastReachedLocalWp = Vector3.one * -999f;
    private Vector2 lastMoveDir = Vector2.zero;
    private bool isDirectlyChasing = false;

    public float arrivalDist = 0.35f;
    public float steeringSmooth = 12f;
    public float backPenalty = 20f; 

    // --- 新增防卡死变量 ---
    private float stuckTimer = 0f;
    private const float STUCK_THRESHOLD = 3.0f; // 3秒阈值

    void Start() {
        stats = GetComponent<MonsterBase>();
        rb = GetComponent<Rigidbody2D>();
        GameObject p = GameObject.FindWithTag("Player");
        if (p != null) player = p.transform;
        navGraph = GetComponentInParent<RoomNavGraph>();
        SearchNextTarget();
    }

    void FixedUpdate() {
        if (!player || !navGraph || stats.IsInKnockback()) return;

        int mask = LayerMask.GetMask("block");
        // bool canSee = !Physics2D.Linecast(transform.position, player.position, mask);
        bool canSee = false; // 保持你关闭直线逻辑的状态

        if (canSee) {
            currentTargetWorldPos = player.position;
            isDirectlyChasing = true;
        } else if (isDirectlyChasing) {
            isDirectlyChasing = false;
            SearchNextTarget();
        }

        Vector2 moveDir = (currentTargetWorldPos - (Vector2)transform.position).normalized;
        Vector2 sep = GetSeparationForce();
        rb.velocity = Vector2.Lerp(rb.velocity, (moveDir + sep * 1.2f).normalized * stats.moveSpeed, Time.fixedDeltaTime * steeringSmooth);
        
        Debug.DrawLine(transform.position, currentTargetWorldPos, Color.red);

        // --- 核心逻辑修改：判定到达或超时卡死 ---
        if (!isDirectlyChasing) {
            stuckTimer += Time.fixedDeltaTime; // 开始计时

            // 逻辑 A: 正常到达目标格子
            if (Vector2.Distance(transform.position, currentTargetWorldPos) < arrivalDist) {
                lastReachedLocalWp = navGraph.transform.InverseTransformPoint(currentTargetWorldPos);
                SearchNextTarget();
            } 
            // 逻辑 B: 防卡死 - 3s未到达则强制重新寻路
            else if (stuckTimer >= STUCK_THRESHOLD) {
                SearchNextTarget();
            }
        }
    }

    void SearchNextTarget() {
        stuckTimer = 0f; // 只要开始新的寻路，计时器就清零

        int mask = LayerMask.GetMask("block");
        Vector3 lStart = navGraph.transform.InverseTransformPoint(transform.position);
        Vector3 lTarget = navGraph.transform.InverseTransformPoint(player.position);

        List<Vector3> path = FindLocalPath(lStart, lTarget, mask);
        if (path != null && path.Count > 0) {
            Vector3 nextL = (Vector3.Distance(path[0], lastReachedLocalWp) < 0.1f && path.Count > 1) ? path[1] : path[0];
            currentTargetWorldPos = navGraph.transform.TransformPoint(nextL);
            lastMoveDir = (currentTargetWorldPos - (Vector2)transform.position).normalized;
        }
    }

    // --- 以下为原封不动的功能函数 ---
    private List<Vector3> FindLocalPath(Vector3 s, Vector3 t, int mask) {
        var wps = navGraph.localWaypoints;
        int sIdx = -1, eIdx = -1;
        float ds = float.MaxValue, de = float.MaxValue;

        for (int i = 0; i < wps.Count; i++) {
            Vector3 wPos = navGraph.transform.TransformPoint(wps[i]);
            if (!Physics2D.Linecast(transform.position, wPos, mask)) {
                float d = Vector3.Distance(s, wps[i]);
                if (d < ds) { ds = d; sIdx = i; }
            }
            if (!Physics2D.Linecast(player.position, wPos, mask)) {
                float d = Vector3.Distance(t, wps[i]);
                if (d < de) { de = d; eIdx = i; }
            }
        }
        if (sIdx == -1 || eIdx == -1) return null;

        var open = new List<int> { sIdx };
        var cameFrom = new Dictionary<int, int>();
        var g = new Dictionary<int, float> { [sIdx] = 0 };
        var f = new Dictionary<int, float> { [sIdx] = Vector3.Distance(wps[sIdx], wps[eIdx]) };

        while (open.Count > 0) {
            int curr = open.OrderBy(n => f.ContainsKey(n) ? f[n] : float.MaxValue).First();
            if (curr == eIdx) {
                var res = new List<Vector3> { wps[curr] };
                while (cameFrom.ContainsKey(curr)) { curr = cameFrom[curr]; res.Add(wps[curr]); }
                res.Reverse(); return res;
            }
            open.Remove(curr);
            for (int i = 0; i < wps.Count; i++) {
                float d = Vector3.Distance(wps[curr], wps[i]);
                if (i == curr || d > 1.1f) continue;
                if (Physics2D.Linecast(navGraph.transform.TransformPoint(wps[curr]), navGraph.transform.TransformPoint(wps[i]), mask)) continue;

                float pen = (Vector2.Dot((wps[i] - wps[curr]).normalized, lastMoveDir) < -0.8f) ? backPenalty : 0;
                float tg = g[curr] + d + pen;
                if (!g.ContainsKey(i) || tg < g[i]) {
                    cameFrom[i] = curr; g[i] = tg;
                    f[i] = g[i] + Vector3.Distance(wps[i], wps[eIdx]);
                    if (!open.Contains(i)) open.Add(i);
                }
            }
        }
        return null;
    }

    private Vector2 GetSeparationForce() {
        Vector2 f = Vector2.zero;
        var cols = Physics2D.OverlapCircleAll(transform.position, 0.5f);
        foreach (var c in cols) if (c.gameObject != gameObject && c.CompareTag("Enemy"))
            f += ((Vector2)transform.position - (Vector2)c.transform.position).normalized;
        return f.normalized;
    }
}