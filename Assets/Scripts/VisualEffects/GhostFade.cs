using UnityEngine;
public class GhostFade : MonoBehaviour {
    private SpriteRenderer sr;
    public float fadeSpeed = 2f;
    void Start() { sr = GetComponent<SpriteRenderer>(); }
    void Update() {
        Color c = sr.color;
        c.a -= Time.deltaTime * fadeSpeed;
        sr.color = c;
    }
}