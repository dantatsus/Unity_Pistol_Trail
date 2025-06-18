using System.Collections;
using TMPro;
using UnityEngine;

public class WeaponShootController : MonoBehaviour
{
    /* ---------- REFERENCES ---------- */
    [Header("References")]
    [SerializeField] private PickupDropSystem pickupDropSystem;
    [SerializeField] private Camera playerCamera;

    /* ---------- PROJECTILE ---------- */
    [Header("Projectile Settings")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float fireRate = 0.55f;

    /* ---------- AUDIO ---------- */
    [Header("Audio Settings")]
    [SerializeField] private AudioClip shootSound;
    [SerializeField] private AudioSource weaponAudioSource;

    /* ---------- LASER ---------- */
    [Header("Laser Settings")]
    [SerializeField] private LineRenderer laserLine;
    [SerializeField] private float laserDistance = 100f;
    [SerializeField] private LayerMask laserHitMask;

    [Header("Laser Dot")]
    [SerializeField] private GameObject laserDotPrefab;
    [SerializeField] private float dotOffset = 0.01f;

    /* ---------- AMMO ---------- */
    [Header("Ammo Settings")]
    [SerializeField] private int maxMagazineSize = 17;
    [SerializeField] private float reloadDuration = 2f;
    [SerializeField] private GameObject physicsMagazinePrefab;
    private int currentAmmo;
    private bool isReloading;

    /* ---------- CASING ---------- */
    [Header("Casing Settings")]
    [SerializeField] private GameObject casingPrefab;          // kovan prefab
    [SerializeField] private float casingEjectForce = 2f;      // N (Impulse)
    [SerializeField] private float casingEjectTorque = 5f;     // rasgele tork
    [SerializeField] private float casingLifeTime = 5f;        // saniye sonra yok et

    /* ---------- UI ---------- */
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI ammoText;

    /* ---------- PRIVATE ---------- */
    private float nextFireTime = 0f;
    private GameObject laserDotInstance;

    /* ---------- MONO ---------- */
    void Awake()
    {
        currentAmmo = maxMagazineSize;
        UpdateAmmoUI();

        if (laserDotPrefab)
        {
            laserDotInstance = Instantiate(laserDotPrefab);
            laserDotInstance.SetActive(false);
        }
    }

    void Update()
    {
        if (pickupDropSystem.currentWeapon != null && Input.GetMouseButtonDown(0))
        {
            if (currentAmmo == 0 && !isReloading) 
            {
                StartReload();
            }
            else
            {
                if (Time.time >= nextFireTime)
                {
                    nextFireTime = Time.time + fireRate;
                    FireWeapon();
                }
            }
        }

        // --- MANUEL RELOAD ---
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (!isReloading && currentAmmo < maxMagazineSize)
                StartReload();
        }
    }

    void LateUpdate()
    {
        if (pickupDropSystem.currentWeapon != null)
            UpdateLaser();
        else
        {
            if (laserLine) laserLine.enabled = false;
            if (laserDotInstance) laserDotInstance.SetActive(false);
        }
    }

    /* ---------- FIRE ---------- */
    void FireWeapon()
    {
        GameObject currentGun = pickupDropSystem.currentWeapon;
        if (currentGun == null) return;

        currentAmmo--;
        UpdateAmmoUI();

        // animasyon
        Animator anim = currentGun.GetComponent<Animator>();
        if (anim) anim.SetTrigger("ShootTrigger");

        // ses
        if (weaponAudioSource && shootSound)
            weaponAudioSource.PlayOneShot(shootSound);

        // mermi
        Transform muzzle = currentGun.transform.Find("Muzzle");
        if (muzzle)
        {
            Instantiate(projectilePrefab,
                        muzzle.position,
                        Quaternion.LookRotation(muzzle.forward));
        }
        else
        {
            Debug.LogWarning("Muzzle transform not found!");
        }

        // kovan
        EjectCasing(currentGun);
    }

    /* ---------- RELOAD FLOW ---------- */
    void StartReload()
    {
        if (isReloading) return;
        isReloading = true;

        GameObject gun = pickupDropSystem.currentWeapon;
        if (gun && gun.TryGetComponent(out Animator anim))
            anim.SetTrigger("ReloadTrigger");

        StartCoroutine(ReloadRoutine());
    }

    IEnumerator ReloadRoutine()
    {
        yield return new WaitForSeconds(reloadDuration);
        currentAmmo = maxMagazineSize;
        UpdateAmmoUI();
        isReloading = false;
    }

    // Animation Event – þarjör çýkar
    public void OnMagOut()
    {
        GameObject gun = pickupDropSystem.currentWeapon;
        if (!gun) return;

        Transform magSlot = gun.transform.Find("Magazine");
        if (!magSlot) return;

        // Orijinal modeli gizle
        foreach (var r in magSlot.GetComponentsInChildren<Renderer>())
            r.enabled = false;

        // Fiziksel þarjör oluþtur
        if (physicsMagazinePrefab)
        {
            GameObject dropMag = Instantiate(physicsMagazinePrefab, magSlot.position, magSlot.rotation);
            Destroy(dropMag, 5f);
        }
    }

    // Animation Event – yeni þarjör tak
    public void OnMagIn()
    {
        GameObject gun = pickupDropSystem.currentWeapon;
        if (!gun) return;

        Transform magSlot = gun.transform.Find("Magazine");
        if (!magSlot) return;

        // Görünürlüðü geri aç
        foreach (var r in magSlot.GetComponentsInChildren<Renderer>())
            r.enabled = true;
    }

    /* ---------- UI HELPER ---------- */
    void UpdateAmmoUI()
    {
        if (ammoText)
            ammoText.text = $"{currentAmmo}/{maxMagazineSize}";
    }

    /* ---------- CASING ---------- */
    void EjectCasing(GameObject currentGun)
    {
        if (!casingPrefab) return;

        Transform eject = currentGun.transform.Find("CasingEject"); // ismini sen belirle
        if (!eject) { Debug.LogWarning("CasingEject transform not found!"); return; }

        GameObject casing = Instantiate(casingPrefab, eject.position, eject.rotation);

        if (casing.TryGetComponent(out Rigidbody rb))
        {
            // sað tarafa (eject.right) impuls + rastgele tork
            rb.AddForce(eject.right * casingEjectForce, ForceMode.Impulse);
            rb.AddTorque(Random.insideUnitSphere * casingEjectTorque, ForceMode.Impulse);
        }

        Destroy(casing, casingLifeTime);
    }

    /* ---------- LASER & DOT ---------- */
    void UpdateLaser()
    {
        GameObject currentGun = pickupDropSystem.currentWeapon;
        if (!currentGun || !laserLine) return;

        Transform laserOrigin = currentGun.transform.Find("Laser");
        if (!laserOrigin) return;

        Vector3 start = laserOrigin.position;
        Vector3 direction = laserOrigin.forward;

        laserLine.enabled = true;

        if (Physics.Raycast(start, direction, out RaycastHit hit, laserDistance, laserHitMask))
        {
            SetLaser(start, hit.point);
            UpdateLaserDot(hit.point, hit.normal, true);
        }
        else
        {
            SetLaser(start, start + direction * laserDistance);
            UpdateLaserDot(Vector3.zero, Vector3.up, false);
        }
    }

    void SetLaser(Vector3 start, Vector3 end)
    {
        laserLine.SetPosition(0, start);
        laserLine.SetPosition(1, end);
    }

    void UpdateLaserDot(Vector3 pos, Vector3 normal, bool visible)
    {
        if (!laserDotInstance) return;

        if (visible)
        {
            laserDotInstance.SetActive(true);
            laserDotInstance.transform.position = pos + normal * dotOffset;
            laserDotInstance.transform.rotation =
                Quaternion.LookRotation(normal) * Quaternion.Euler(0f, 180f, 0f); // 180° düzeltme
        }
        else
        {
            laserDotInstance.SetActive(false);
        }
    }
}
