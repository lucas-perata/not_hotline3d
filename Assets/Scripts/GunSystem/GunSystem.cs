using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GunSystem : MonoBehaviour
{
    public Rigidbody rb;

    // Gun stats
    public int damage;
    public float timeBetweenShooting,
        spread,
        range,
        reloadTime,
        timeBetweenShoots;
    public int magazineSize,
        bulletsPetTap;
    public bool allowButtonHold;
    int bulletsLeft,
        bulletsShot;
    AudioSource m_shootingSound;

    // general checks
    bool shooting,
        readyToShoot,
        reloading;

    // references
    public Camera playerCam;
    public Transform attackPoint;
    public RaycastHit rayHit;
    public LayerMask whatIsEnemy;

    // Shake
    public CameraShake camShake;
    public float camShakeMagnitude,
        camShakeDuration;

    // Graphics
    public GameObject muzzleFlash,
        bulletHoleGraphic;
    public TextMeshProUGUI text;

    RaycastHit hit;

    private void Awake()
    {
        bulletsLeft = magazineSize;
        readyToShoot = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        m_shootingSound = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        MyInput();

        // Set text
        text.SetText(bulletsLeft + " / " + magazineSize);
    }

    private void MyInput()
    {
        if (allowButtonHold)
            shooting = Input.GetKey(KeyCode.Mouse0);
        else
            shooting = Input.GetKeyDown(KeyCode.Mouse0);

        if (Input.GetKeyDown(KeyCode.R) && bulletsLeft < magazineSize && !reloading)
            Reload();

        if (readyToShoot && shooting && !reloading && bulletsLeft > 0)
        {
            bulletsShot = bulletsPetTap;
            Shoot();
        }
    }

    void ShootAudio()
    {
        m_shootingSound.Play();
    }

    private void Shoot()
    {
        readyToShoot = false;

        // Spread
        float x = Random.Range(-spread, spread);
        float y = Random.Range(-spread, spread);

        // Less accuracy while moving

        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        if (flatVel.magnitude > 0)
        {
            spread = spread * 1.2f;
        }
        else
        {
            spread = 1;
        }

        // Calculate direction with spread
        Vector3 direction = playerCam.transform.forward + new Vector3(x, y, 0);

        // RayCast
        if (
            Physics.Raycast(playerCam.transform.position, direction, out rayHit, range, whatIsEnemy)
        )
        {
            Debug.Log(rayHit.collider.name);

            /* if (rayHit.collider.CompareTag("Enemy")) */
            /*     // TODO: Add enemy with tag and function to take damage */
            /*     rayHit.collider.GetComponent<ShootingAi>().TakeDamage(damage); */
        }

        // Shake camera and play audio


        StartCoroutine(camShake.ShakeM(camShakeDuration, camShakeMagnitude));

        // Bullet hole and muzzleFlash

        Vector3 reticulePos = new Vector3(0.5f, 0.5f, 0f);
        Vector3 offSet = new Vector3(0f, 0f, -0.001f);
        Ray rayOrigin = Camera.main.ViewportPointToRay(reticulePos);

        RaycastHit hitInfo;

        if (Physics.Raycast(rayOrigin, out hitInfo))
        {
            GameObject hole = Instantiate(bulletHoleGraphic, hitInfo.point, Quaternion.identity);

            hole.transform.position += offSet;
        }
        ;
        /* Instantiate( */
        /*     bulletHoleGraphic, */
        /*     rayHit.point, */
        /*     Quaternion.FromToRotation(Vector3.forward, rayHit.normal) */
        /* ); */

        Instantiate(muzzleFlash, attackPoint.position, Quaternion.identity);

        ShootAudio();

        bulletsLeft--;
        bulletsShot--;

        Invoke("ResetShot", timeBetweenShooting);

        if (bulletsShot > 0 && bulletsLeft > 0)
            Invoke("Shoot", timeBetweenShoots);
    }

    private void ResetShot()
    {
        readyToShoot = true;
    }

    private void Reload()
    {
        reloading = true;
        Invoke("ReloadFinished", reloadTime);
    }

    private void ReloadFinished()
    {
        bulletsLeft = magazineSize;
        reloading = false;
    }
}
