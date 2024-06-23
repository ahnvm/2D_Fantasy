using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    private Vector3 offset = new Vector3(0f, 0f, -10f);
    private float smoothTime = 0.25f;
    private Vector3 velocity = Vector3.zero;

    [SerializeField] private Transform target;

    private void Update()
    {

    }

    private void FixedUpdate()
    {
        Vector3 targetPosition = target.position + offset;
        Debug.Log(targetPosition.y);

        Vector3 currentPosition = transform.position;
        Vector3 newPosition = currentPosition;

        if (targetPosition.x > -12.25f && targetPosition.x < 3.2f)
        {
            newPosition.x = Mathf.SmoothDamp(currentPosition.x, targetPosition.x + 0.5f, ref velocity.x, smoothTime);
        }
        if (targetPosition.y > -4.1f)
        {
            newPosition.y = Mathf.SmoothDamp(currentPosition.y, targetPosition.y, ref velocity.y, smoothTime);
        }

        transform.position = newPosition;
    }

}