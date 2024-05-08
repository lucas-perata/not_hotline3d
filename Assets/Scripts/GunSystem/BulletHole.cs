using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletHole : MonoBehaviour
{
    public GameObject bulletHole;
    public float distance = 10f;
    Camera cam;

    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            RaycastHit hit;
            if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, distance))
            {
                GameObject bh = Instantiate(
                    bulletHole,
                    hit.point
                        + new Vector3(
                            hit.normal.x * 0.01f,
                            hit.normal.y * 0.01f,
                            hit.normal.z * 0.01f
                        ),
                    Quaternion.LookRotation(-hit.normal)
                );
            }
        }
    }
}
