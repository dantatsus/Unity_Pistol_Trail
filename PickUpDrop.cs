using UnityEngine;

public class PickUpDrop : MonoBehaviour
{
    [Header("References")]
    public Camera playerCam;               
    public Transform normalItemHoldPoint;  
    public Transform weaponHoldPoint;      

    [Header("Pickup Settings")]
    public float pickUpRange = 3f;         

    private GameObject heldNormalItem = null;
    private GameObject heldWeapon = null;

    private bool isHold = false;

    void Update()
    {
        // 1) Normal eþya: Sol týk basýlý tutma
        HandleNormalItemPickup();

        // 2) Silah: F ile al, G ile býrak
        HandleWeaponPickup();

        print($"isHold: {isHold}");
    }

    void HandleNormalItemPickup()
    {
        if (heldNormalItem == null && Input.GetMouseButtonDown(0))
        {
            Ray ray = playerCam.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, pickUpRange))
            {
                if (hit.transform.CompareTag("NormalItem"))
                {
                    PickUpNormalItem(hit.transform.gameObject);
                    isHold = true;
                }
            }
        }

        if (heldNormalItem != null && Input.GetMouseButtonUp(0))
        {
            DropNormalItem();
            isHold = false;
        }
    }

    void HandleWeaponPickup()
    {
        if (heldWeapon == null && Input.GetKeyDown(KeyCode.F))
        {
            Ray ray = playerCam.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, pickUpRange))
            {
                if (hit.transform.CompareTag("Weapon"))
                {
                    PickUpWeapon(hit.transform.gameObject);
                    isHold = true;
                }
            }
        }

        if (heldWeapon != null && Input.GetKeyDown(KeyCode.G))
        {
            DropWeapon();
            isHold = false;
        }
    }

    void PickUpNormalItem(GameObject item)
    {
        heldNormalItem = item;

        Rigidbody rb = item.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.useGravity = false;
        }

        item.layer = LayerMask.NameToLayer("HeldObject");

        item.transform.SetParent(normalItemHoldPoint);

        item.transform.localPosition = Vector3.zero;
        item.transform.localRotation = Quaternion.identity;
    }

    void DropNormalItem()
    {
        if (heldNormalItem == null) return;

        Rigidbody rb = heldNormalItem.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.useGravity = true;
        }

        heldNormalItem.layer = LayerMask.NameToLayer("Default");

        heldNormalItem.transform.SetParent(null);

        rb.AddForce(playerCam.transform.forward * 2f, ForceMode.Impulse);

        heldNormalItem = null;
    }

    void PickUpWeapon(GameObject weapon)
    {
        heldWeapon = weapon;

        Rigidbody rb = weapon.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true; // Elimizde olduðu sürece sabit
            rb.useGravity = false;
            rb.detectCollisions = true; // Çarpýþmalar aktif kalsýn
        }

        SetLayerRecursively(weapon, LayerMask.NameToLayer("HeldObject"));

        weapon.transform.SetParent(weaponHoldPoint);

        weapon.transform.localPosition = Vector3.zero;
        weapon.transform.localRotation = Quaternion.Euler(0, 90, 0); 

    }

    void DropWeapon()
    {
        if (heldWeapon == null) return;

        Rigidbody rb = heldWeapon.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false; // Elimizde olduðu sürece sabit
            rb.useGravity = true;
        }

        SetLayerRecursively(heldWeapon, LayerMask.NameToLayer("Default"));

        heldWeapon.transform.SetParent(null);

        rb.AddForce(playerCam.transform.forward * 2f, ForceMode.Impulse);

        heldWeapon = null;
    }

    void SetLayerRecursively(GameObject obj, int newLayer)
    {
        if (obj == null) return;

        obj.layer = newLayer;

        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, newLayer);
        }
    }


}
