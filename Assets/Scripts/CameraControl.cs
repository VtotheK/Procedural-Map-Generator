using UnityEngine;

public class CameraControl : MonoBehaviour {

    Camera camera;
    PlayerMovement player;
    private void Start()
    {
        player = FindObjectOfType<PlayerMovement>();
    }
    private void Update()
    {
        transform.position = new Vector3(player.transform.position.x, player.transform.position.y, -10);
    }
    //private void Start()
    //{
    //    camera = GetComponent<Camera>();
    //}

    //public void SetAspectRatio(int width, int height)
    //{
    //    camera.aspect = width / height;
    //    camera.orthographicSize = height / 2;
    //}
    //public void MoveCamera(int width, int height)
    //{
    //    camera.transform.position = new Vector3((int)width/2, (int)height/2, -10);
    //}
}
