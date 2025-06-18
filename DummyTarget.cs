using UnityEngine;
using TMPro;

public class DummyTarget : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private Animator animator;          // FallTrigger burada

    private bool isDown = false;

    public void OnHit(Vector3 hitPoint)
    {
        if (isDown) return;
        isDown = true;

        animator.ResetTrigger("FallTrigger");  // Her ihtimale kar�� s�f�rla
        animator.SetTrigger("FallTrigger");
        animator.Update(0f);  // Bu sat�r animasyonu ayn� frame i�inde g�nceller!
    }

}