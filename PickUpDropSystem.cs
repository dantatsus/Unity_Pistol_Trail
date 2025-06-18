using TMPro;
using UnityEngine;

public class PickupDropSystem : MonoBehaviour
{
    [Header("References")]
    public Transform playerCam;      // Oyuncu kameras�n�n Transform'u (FPS ise genelde PlayerCam objesi)
    public Transform weaponHolder;   // Silah�n elde tutulaca�� konum (bo� bir GameObject)

    [Header("Settings")]
    public float pickUpRange = 2f;   // Silah� alabilece�imiz maksimum mesafe
    public float dropForwardForce = 2f;  // Silah� b�rak�rken ileriye uygulanacak kuvvet
    public float dropUpwardForce = 1f;   // Silah� b�rak�rken yukar�ya uygulanacak kuvvet
    public KeyCode pickUpKey = KeyCode.E; // Silah� al�p b�rakmak i�in bas�lacak tu�

    // Elimizdeki silah referans�
    public GameObject currentWeapon = null;

    [SerializeField] TextMeshProUGUI ammoUI;

    void Update()
    {
        // Pickup/Drop tu�una bas�ld���nda
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
        // Kameran�n tam ortas�ndan ileriye do�ru bir raycast at�yoruz
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

        // Silah�n rigidbody�sini kapat�yoruz (havada durmas� ve fiziksel etkile�ime girmemesi i�in)
        Rigidbody rb = currentWeapon.GetComponent<Rigidbody>();
        if (rb)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        // Collider�� trigger�a �eviriyoruz ki karakterle �arp��mas�n
        Collider col = currentWeapon.GetComponent<Collider>();
        if (col)
        {
            col.isTrigger = true;
        }

        // Silah�, weaponHolder objesinin �ocu�u yap�yoruz
        currentWeapon.transform.SetParent(weaponHolder);

        // Pozisyonu ve rotasyonu s�f�rl�yoruz (weaponHolder��n tam konumunda dursun)
        currentWeapon.transform.localPosition = Vector3.zero;
        currentWeapon.transform.localRotation = Quaternion.Euler(0, 90, 0);

        ammoUI.gameObject.SetActive(true);
    }

    void DropWeapon()
    {
        // Silah�n parent'�n� kald�r (hi�bir objeye ba�l� olmas�n)
        currentWeapon.transform.SetParent(null);

        // RigidBody'sini tekrar aktif et (fizik sim�lasyonuna dahil olsun)
        Rigidbody rb = currentWeapon.GetComponent<Rigidbody>();
        if (rb)
        {
            rb.isKinematic = false;
            rb.useGravity = true;

            // Biraz ileriye ve yukar�ya do�ru kuvvet uygulayal�m ki silah yere d��erken hafif�e f�rlas�n
            rb.AddForce(playerCam.forward * dropForwardForce, ForceMode.Impulse);
            rb.AddForce(playerCam.up * dropUpwardForce, ForceMode.Impulse);
        }

        // Collider�� tekrar normal hale getir
        Collider col = currentWeapon.GetComponent<Collider>();
        if (col)
        {
            col.isTrigger = false;
        }

        // Art�k elimizde silah yok
        currentWeapon = null;
    }
}
