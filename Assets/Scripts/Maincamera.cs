using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target; // 따라갈 캐릭터의 Transform
    public Vector3 offset;   // 카메라와 캐릭터 사이의 거리

    void LateUpdate()
    {
        if (target != null)
        {
            // 카메라 위치를 캐릭터 위치 + 오프셋으로 설정
            transform.position = target.position + offset;
        }
    }
}