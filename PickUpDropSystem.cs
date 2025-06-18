using TMPro;
using UnityEngine;

public class PickupDropSystem : MonoBehaviour
{
    [Header("References")]
    public Transform playerCam;      // Oyuncu kamerasýnýn Transform'u (FPS ise genelde PlayerCam objesi)
    public Transform weaponHolder;   // Silahýn elde tutulacaðý konum (boþ bir GameObject)

    [Header("Settings")]
    public float pickUpRange = 2f;   // Silahý alabileceðimiz maksimum mesafe
    public float dropForwardForce = 2f;  // Silahý býrakýrken ileriye uygulanacak kuvvet
    public float dropUpwardForce = 1f;   // Silahý býrakýrken yukarýya uygulanacak kuvvet
    public KeyCode pickUpKey = KeyCode.E; // Silahý alýp býrakmak için basýlacak tuþ

    // Elimizdeki silah referansý
    public GameObject currentWeapon = null;

    [SerializeField] TextMeshProUGUI ammoUI;

    void Update()
    {
        // Pickup/Drop tuþuna basýldýðýnda
        if (Input.GetKeyDown(pickUpKey))
        {
            // Elimizde silah yoksa -> Pickup
            if (currentWeapon == null)
            {
                TryPickupWeapon();
            }
            // Elimizde silah varsa -> Drop
            else
            {
                DropWeapon();
            }
        }
    }

    void TryPickupWeapon()
    {
        RaycastHit hit;
        // Kameranýn tam ortasýndan ileriye doðru bir raycast atýyoruz
        if (Physics.Raycast(playerCam.position, playerCam.forward, out hit, pickUpRange))
        {
            if (hit.transform.CompareTag("Weapon"))
            {
                PickupWeapon(hit.transform.gameObject);
            }
        }
    }

    void PickupWeapon(GameObject weapon)
    {
        currentWeapon = weapon;

        // Silahýn rigidbody’sini kapatýyoruz (havada durmasý ve fiziksel etkileþime girmemesi için)
        Rigidbody rb = currentWeapon.GetComponent<Rigidbody>();
        if (rb)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        // Collider’ý trigger’a çeviriyoruz ki karakterle çarpýþmasýn
        Collider col = currentWeapon.GetComponent<Collider>();
        if (col)
        {
            col.isTrigger = true;
        }

        // Silahý, weaponHolder objesinin çocuðu yapýyoruz
        currentWeapon.transform.SetParent(weaponHolder);

        // Pozisyonu ve rotasyonu sýfýrlýyoruz (weaponHolder’ýn tam konumunda dursun)
        currentWeapon.transform.localPosition = Vector3.zero;
        currentWeapon.transform.localRotation = Quaternion.Euler(0, 90, 0);

        ammoUI.gameObject.SetActive(true);
    }

    void DropWeapon()
    {
        // Silahýn parent'ýný kaldýr (hiçbir objeye baðlý olmasýn)
        currentWeapon.transform.SetParent(null);

        // RigidBody'sini tekrar aktif et (fizik simülasyonuna dahil olsun)
        Rigidbody rb = currentWeapon.GetComponent<Rigidbody>();
        if (rb)
        {
            rb.isKinematic = false;
            rb.useGravity = true;

            // Biraz ileriye ve yukarýya doðru kuvvet uygulayalým ki silah yere düþerken hafifçe fýrlasýn
            rb.AddForce(playerCam.forward * dropForwardForce, ForceMode.Impulse);
            rb.AddForce(playerCam.up * dropUpwardForce, ForceMode.Impulse);
        }

        // Collider’ý tekrar normal hale getir
        Collider col = currentWeapon.GetComponent<Collider>();
        if (col)
        {
            col.isTrigger = false;
        }

        // Artýk elimizde silah yok
        currentWeapon = null;
    }
}
