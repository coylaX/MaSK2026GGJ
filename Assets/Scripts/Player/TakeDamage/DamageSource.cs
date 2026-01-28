using UnityEngine;

public class DamageSource : MonoBehaviour
{
    [Header("伤害数值")]
    public float damageAmount = 10f;

    // 可选：是否在碰撞后销毁（例如子弹需要，尖刺不需要）
    public bool destroyOnHit = false; 

    // 如果是怪物，可以动态从 MonsterBase 同步数值
    public void SetDamage(float val) => damageAmount = val;
}