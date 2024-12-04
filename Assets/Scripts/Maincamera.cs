using UnityEngine;

public class CameraBoundary : MonoBehaviour
{
    public Transform target; // 캐릭터의 Transform
    public float minX, maxX, minY, maxY; // 이동 가능한 영역의 경계

    void LateUpdate()
    {
        if (target != null)
        {
            // 타겟 위치를 제한
            float clampedX = Mathf.Clamp(target.position.x, minX, maxX);
            float clampedY = Mathf.Clamp(target.position.y, minY, maxY);

            // 카메라 위치 설정
            transform.position = new Vector3(clampedX, clampedY, transform.position.z);
        }
    }
}
