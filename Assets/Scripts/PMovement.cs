using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(CapsuleCollider))]
public class PMovement : MonoBehaviour {
    public Camera playerCamera;
    public GameObject SpellWheelCont;
    public Transform thirdPersonOffset;
    public float walkSpeed = 6f;
    public float runSpeed = 12f;
    public float jumpPower = 7f;
    public float gravity = 20f;
    public float lookSpeed = 2f;
    public float lookXLimit = 45f;
    public float maxHealth = 100f;
    public float health;
    public float defaultHeight = 2f;
    public float crouchHeight = 1f;
    public float crouchSpeed = 3f;
    public float groundCheckDistance = 0.2f;

    private Vector3 moveDirection = Vector3.zero;
    private float rotationX = 0;
    private float rotationY = 0;
    private Rigidbody rb;
    private CapsuleCollider capsuleCollider;
    private bool canMove = true;
    private bool spellWheelOpen = false;
    private bool isFirstPerson = true;
    private bool isGrounded = false;
    private bool wasGroundedLastFrame = false;
    private bool jumpRequested = false;
    private float verticalVelocity = 0f;
    private float originalWalkSpeed;
    private float originalRunSpeed;
    private bool takingDamage = false;
    private Vector3 knockbackVelocity = Vector3.zero;
    private int extraJumpsRemaining = 0; // extra jumps disponibles después del salto inicial
    public float knockbackMultiplier = 20f;
    public float knockbackDecay = 8f;

    public bool IsRunning { get; private set; }
    public bool IsCrouching { get; private set; }
    public float CurrentSpeed { get; private set; }
    public bool IsJumping { get; private set; }

    void Start() {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        rb.useGravity = false;

        capsuleCollider = GetComponent<CapsuleCollider>();
        if (capsuleCollider == null) {
            Debug.LogWarning("PMovement expects a CapsuleCollider for ground checking.");
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        originalWalkSpeed = walkSpeed;
        originalRunSpeed = runSpeed;

        // Initialize current health to max on start
        health = maxHealth;

        // Ensure spell wheel is hidden at start if assigned
        if (SpellWheelCont != null) SpellWheelCont.SetActive(false);
    }

    void Update() {
        // Toggle Spell Wheel with Q
        if (Input.GetKeyDown(KeyCode.Q)) {
            Debug.Log("Q pressed - toggling SpellWheel");
            spellWheelOpen = !spellWheelOpen;
            if (SpellWheelCont != null) {
                SpellWheelCont.SetActive(spellWheelOpen);
                Debug.Log("SpellWheelCont set active: " + spellWheelOpen);
            } else {
                Debug.LogWarning("SpellWheelCont is not assigned on PMovement.");
            }

            // When the spell wheel is open, disable movement and show cursor for UI interaction
            canMove = !spellWheelOpen;
            if (spellWheelOpen) {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            } else {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }

        if (Input.GetKeyDown(KeyCode.E)) {
            isFirstPerson = !isFirstPerson;
        }

        if (!isFirstPerson) {
            rotationY = transform.eulerAngles.y;
        }

        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        IsCrouching = Input.GetKey(KeyCode.LeftControl) && canMove;
        IsRunning = canMove && !IsCrouching && Input.GetKey(KeyCode.LeftShift);
        float curSpeedX = canMove ? (IsRunning ? runSpeed : walkSpeed) * Input.GetAxis("Vertical") : 0;
        float curSpeedY = canMove ? (IsRunning ? runSpeed : walkSpeed) * Input.GetAxis("Horizontal") : 0;

        moveDirection = (forward * curSpeedX) + (right * curSpeedY);
        CurrentSpeed = moveDirection.magnitude;

        if (Input.GetButtonDown("Jump") && canMove) {
            bool hasDoubleJump = SelectSpell.weaponID == 1;
            
            // Primer salto o salto adicional cuando se selecciona double
            if (isGrounded)
            {
                jumpRequested = true;
                extraJumpsRemaining = hasDoubleJump ? 1 : 0;
                IsJumping = true;
            }
            else if (hasDoubleJump && extraJumpsRemaining > 0)
            {
                verticalVelocity = jumpPower;
                extraJumpsRemaining--;
                IsJumping = true;
            }
        }

        if (IsCrouching) {
            if (capsuleCollider != null) {
                capsuleCollider.height = crouchHeight;
            }
            walkSpeed = crouchSpeed;
            runSpeed = crouchSpeed;
        } else {
            if (capsuleCollider != null) {
                capsuleCollider.height = defaultHeight;
            }
            walkSpeed = originalWalkSpeed;
            runSpeed = originalRunSpeed;
        }

        if (canMove) {
            rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);

            if (isFirstPerson) {
                playerCamera.transform.localPosition = Vector3.zero;
                playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
                transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
            } else {
                rotationY += Input.GetAxis("Mouse X") * lookSpeed;
                Vector3 offset = new Vector3(0, 0, -5f);
                Quaternion rotation = Quaternion.Euler(rotationX, rotationY, 0);
                playerCamera.transform.position = transform.position + rotation * offset + Vector3.up * 1.5f;
                playerCamera.transform.LookAt(transform.position + Vector3.up * 1f);
                transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
            }
        }
    }

    void FixedUpdate() {
        UpdateGroundedState();

        if (isGrounded && verticalVelocity < 0f) {
            verticalVelocity = -1f;
        }

        if (jumpRequested) {
            verticalVelocity = jumpPower;
            jumpRequested = false;
        }

        verticalVelocity -= gravity * Time.fixedDeltaTime;

        Vector3 targetVelocity = new Vector3(moveDirection.x, verticalVelocity, moveDirection.z);

        // Apply knockback velocity (will be decayed over time)
        targetVelocity += knockbackVelocity;

        rb.linearVelocity = targetVelocity;

        // Decay knockback over time so it tapers off smoothly
        knockbackVelocity = Vector3.Lerp(knockbackVelocity, Vector3.zero, knockbackDecay * Time.fixedDeltaTime);

        // Reset jump flag y contador cuando toca el suelo
        if (isGrounded && !wasGroundedLastFrame)
        {
            IsJumping = false;
            extraJumpsRemaining = SelectSpell.weaponID == 1 ? 1 : 0;
        }
        wasGroundedLastFrame = isGrounded;
    }

    private void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.CompareTag("MovingPlatform")) {
            transform.SetParent(collision.transform, true);
        }
    }


    private void OnCollisionExit(Collision collision) {
        if (collision.gameObject.CompareTag("MovingPlatform")) {
            transform.SetParent(null);
        }
    }

    public void TakeDamage(Vector2 direction, float damage) {
        if (takingDamage) return;
        Vector3 knockback = new Vector3(direction.x, 0, direction.y).normalized * knockbackMultiplier * 2f;
        // Set knockback velocity so it isn't immediately overwritten by movement code
        knockbackVelocity = knockback;
        StartCoroutine(FlashRed());

        health -= damage;
        health = Mathf.Clamp(health, 0, maxHealth);
        takingDamage = true;
        Invoke(nameof(ResetDamage), 0.5f);
    }

    private IEnumerator FlashRed()
    {
        Renderer rend = GetComponentInChildren<Renderer>();
        if (rend != null)
        {
            Color original = rend.material.color;
            rend.material.color = Color.red;
            yield return new WaitForSeconds(0.2f);
            rend.material.color = original;
        }
        else
        {
            yield return null;
        }
    }

    private void ResetDamage()
    {
        takingDamage = false;
    }

    private void UpdateGroundedState() {
        if (capsuleCollider != null) {
            Vector3 origin = transform.position + Vector3.up * 0.1f;
            float checkDistance = (capsuleCollider.height * 0.5f) + groundCheckDistance;
            isGrounded = Physics.Raycast(origin, Vector3.down, checkDistance);
        } else {
            isGrounded = Physics.Raycast(transform.position, Vector3.down, groundCheckDistance + 0.1f);
        }
    }
}
