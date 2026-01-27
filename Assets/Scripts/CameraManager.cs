using UnityEngine;

public class CameraManager : MonoBehaviour {
    public static CameraManager Instance;
    public float lerpSpeed = 8f;
    private Vector3 targetPos;

    void Awake() { Instance = this; targetPos = transform.position; }

    void Update() {
        transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * lerpSpeed);
    }

    public void SwitchToRoom(Vector3 center) {
        targetPos = new Vector3(center.x, center.y, transform.position.z);
    }
}