using UnityEngine;

public class PistolAnimationController : MonoBehaviour
{
    public WeaponShootController controller;   // Player�daki script�i s�r�kle

    public void OnMagOut() => controller?.OnMagOut();
    public void OnMagIn() => controller?.OnMagIn();
}