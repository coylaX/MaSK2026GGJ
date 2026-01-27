using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class MonsterAI_AStar : MonoBehaviour {
    private MonsterBase stats;
    private Rigidbody2D rb;
    private Transform player;
    private RoomNavGraph navGraph;

    private Vector2 currentTargetPos;
    private Vector2 lastReachedWpPos = Vector2.one * -999f;
    private Vector2 lastMoveDirection = Vector2.zero;
    private bool isHeadingToPlayerDirectly = false;

    [Header("寻路参数")]
    public float arrivalThreshold = 0.35f;
    public float steeringSpeed = 12f;
    public float backPenalty = 10f; // 回头路的代价惩罚

    void Start() {
        stats = GetComponent<MonsterBase>();
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindWithTag("Player").transform;
        navGraph = GetComponentInParent<RoomNavGraph>();
        SearchNextTarget();
    }

    void FixedUpdate() {
        if (!player || !navGraph || stats.IsInKnockback()) return;

        int mask = LayerMask.GetMask("block");
        bool canSeePlayer = !Physics2D.Linecast(transform.position, player.position, mask);

        // 1. 动态切换：直视玩家或走格点
        if (canSeePlayer) {
            currentTargetPos = player.position;
            isHeadingToPlayerDirectly = true;
        } else if (isHeadingToPlayerDirectly) {
            isHeadingToPlayerDirectly = false;
            SearchNextTarget();
        }

        // 2. 物理位移
        Move();

        // 3. 到达判定：触发下一次“步进式”寻路
        if (!isHeadingToPlayerDirectly && Vector2.Distance(transform.position, currentTargetPos) < arrivalThreshold) {
            lastReachedWpPos = currentTargetPos;
            SearchNextTarget();
        }
    }

    void SearchNextTarget() {
        int mask = LayerMask.GetMask("block");
        List<Vector2> path = FindAStarPath(transform.position, player.position, mask);

        if (path != null && path.Count > 0) {
            Vector2 next = path[0];
            // 如果搜到刚走过的点，强制选下一个点
            if (Vector2.Distance(next, lastReachedWpPos) < 0.1f && path.Count > 1) {
                next = path[1];
            }
            
            lastMoveDirection = (next - (Vector2)transform.position).normalized;
            currentTargetPos = next;
        }
    }

    private List<Vector2> FindAStarPath(Vector2 start, Vector2 target, int mask) {
        var wps = navGraph.waypoints;
        int startIdx = -1, endIdx = -1;
        float minDistS = float.MaxValue, minDistE = float.MaxValue;

        for (int i = 0; i < wps.Count; i++) {
            if (!Physics2D.Linecast(start, wps[i], mask)) {
                float d = Vector2.Distance(start, wps[i]);
                if (d < minDistS) { minDistS = d; startIdx = i; }
            }
            if (!Physics2D.Linecast(target, wps[i], mask)) {
                float d = Vector2.Distance(target, wps[i]);
                if (d < minDistE) { minDistE = d; endIdx = i; }
            }
        }
        return (startIdx != -1 && endIdx != -1) ? CalculateAStar(startIdx, endIdx, mask) : null;
    }

    private List<Vector2> CalculateAStar(int start, int end, int mask) {
        var wps = navGraph.waypoints;
        var openSet = new List<int> { start };
        var cameFrom = new Dictionary<int, int>();
        var gScore = new Dictionary<int, float> { [start] = 0 };
        var fScore = new Dictionary<int, float> { [start] = Vector2.Distance(wps[start], wps[end]) };

        while (openSet.Count > 0) {
            int curr = openSet.OrderBy(n => fScore.ContainsKey(n) ? fScore[n] : float.MaxValue).First();
            if (curr == end) return ReconstructPath(cameFrom, curr);
            openSet.Remove(curr);

            for (int i = 0; i < wps.Count; i++) {
                float d = Vector2.Distance(wps[curr], wps[i]);
                if (i == curr || d > 1.1f || Physics2D.Linecast(wps[curr], wps[i], mask)) continue;

                // 【核心：回头惩罚】计算方向代价
                Vector2 dirToNext = (wps[i] - wps[curr]).normalized;
                float penalty = (Vector2.Dot(dirToNext, lastMoveDirection) < -0.8f) ? backPenalty : 0;

                float tentativeG = gScore[curr] + d + penalty;
                if (!gScore.ContainsKey(i) || tentativeG < gScore[i]) {
                    cameFrom[i] = curr;
                    gScore[i] = tentativeG;
                    fScore[i] = gScore[i] + Vector2.Distance(wps[i], wps[end]);
                    if (!openSet.Contains(i)) openSet.Add(i);
                }
            }
        }
        return null;
    }

    private List<Vector2> ReconstructPath(Dictionary<int, int> cf, int curr) {
        var res = new List<Vector2> { navGraph.waypoints[curr] };
        while (cf.ContainsKey(curr)) { curr = cf[curr]; res.Add(navGraph.waypoints[curr]); }
        res.Reverse();
        return res;
    }

    void Move() {
        Vector2 dir = (currentTargetPos - (Vector2)transform.position).normalized;
        Vector2 sep = GetSeparation();
        rb.velocity = Vector2.Lerp(rb.velocity, (dir + sep * 1.2f).normalized * stats.moveSpeed, Time.fixedDeltaTime * steeringSpeed);
        Debug.DrawLine(transform.position, currentTargetPos, Color.red);
    }

    Vector2 GetSeparation() {
        Vector2 f = Vector2.zero;
        var cols = Physics2D.OverlapCircleAll(transform.position, 0.5f);
        foreach (var c in cols) {
            if (c.gameObject != gameObject && c.CompareTag("Enemy"))
                f += ((Vector2)transform.position - (Vector2)c.transform.position).normalized;
        }
        return f.normalized;
    }
}