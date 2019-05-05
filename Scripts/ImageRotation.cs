using UnityEngine;

public class ImageRotation : MonoBehaviour
{
    [SerializeField]
    private float speed = 200f;

    void FixedUpdate()
    {
        transform.Rotate(0f, 0f, speed * Time.fixedDeltaTime);
    }
}
