using UnityEngine;

public class PistolAnimationController : MonoBehaviour
{
    public WeaponShootController controller;   // Player’daki script’i sürükle

    public void OnMagOut() => controller?.OnMagOut();
    public void OnMagIn() => controller?.OnMagIn();
}