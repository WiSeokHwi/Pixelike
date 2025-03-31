using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    public Transform target;
    public float smoothSpeed = 0.125f;
    public Vector2 offset;


    private void LateUpdate()
    {
        {
            if (target == null) return;

            // 목표 위치 계산
            Vector3 desiredPosition = new Vector3(target.position.x + offset.x, target.position.y + offset.y, transform.position.z);

            // 부드러운 이동 처리
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

            // 카메라 위치 적용
            transform.position = smoothedPosition;
        }
    }
}
