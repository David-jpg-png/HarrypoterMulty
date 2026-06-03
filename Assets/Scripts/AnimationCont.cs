using UnityEngine;

[RequireComponent(typeof(Animator))]
public class AnimationCont : MonoBehaviour
{
    private Animator anim;
    private PMovement pMovement;

    [SerializeField] private string speedParam = "Speed";
    [SerializeField] private string movingParam = "isMoving";
    [SerializeField] private string runningParam = "isRunning";
    [SerializeField] private string crouchingParam = "isCrouching";
    [SerializeField] private string jumpParam = "Jump";

    private void Awake()
    {
        anim = GetComponent<Animator>();
        pMovement = GetComponent<PMovement>();
        
        if (anim == null)
        {
            Debug.LogWarning("AnimationCont requiere un Animator en el mismo GameObject.");
        }
        if (pMovement == null)
        {
            Debug.LogWarning("AnimationCont requiere un PMovement en el mismo GameObject.");
        }
    }

    private void Update()
    {
        if (anim == null || pMovement == null) return;

        // Leer estados directamente de PMovement
        anim.SetFloat(speedParam, pMovement.CurrentSpeed);
        anim.SetBool(movingParam, pMovement.CurrentSpeed > 0.1f);
        anim.SetBool(runningParam, pMovement.IsRunning);
        anim.SetBool(crouchingParam, pMovement.IsCrouching);
        
        // Debug: Imprimir valores
        if (pMovement.CurrentSpeed > 0 || pMovement.IsRunning || pMovement.IsCrouching)
        {
            Debug.Log($"AnimationCont - Speed: {pMovement.CurrentSpeed:F2}, Running: {pMovement.IsRunning}, Crouching: {pMovement.IsCrouching}");
        }
        
        // Trigger jump cuando está saltando
        if (pMovement.IsJumping)
        {
            Debug.Log("AnimationCont - Jump triggered!");
            anim.SetTrigger(jumpParam);
        }
    }

    public void UpdateMovement(float speed, bool running, bool crouching)
    {
        if (anim == null) return;

        anim.SetFloat(speedParam, speed);
        anim.SetBool(movingParam, speed > 0.1f);
        anim.SetBool(runningParam, running);
        anim.SetBool(crouchingParam, crouching);
    }

    public void TriggerJump()
    {
        if (anim == null) return;
        anim.SetTrigger(jumpParam);
    }
}
