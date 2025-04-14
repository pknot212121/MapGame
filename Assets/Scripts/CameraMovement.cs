using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public float speed;
    public float scrollSpeed;
    public float minZoom;
    public float maxZoom;
    public Camera cam;
    void FixedUpdate()
    {
        Vector3 pos = transform.position;
        if(Input.GetKey(KeyCode.LeftArrow)) pos.x -= speed;
        if(Input.GetKey(KeyCode.UpArrow)) pos.y += speed;
        if(Input.GetKey(KeyCode.RightArrow)) pos.x += speed;
        if(Input.GetKey(KeyCode.DownArrow)) pos.y -= speed;
        transform.position = pos;
    }

    void Update()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0f)
        {
            cam.orthographicSize -= scroll * scrollSpeed * 10f;
            cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, minZoom, maxZoom);
        }
    }
}
