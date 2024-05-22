using EIS.Runtime.Misc;
using EIS.Runtime.Restrictions;
using UnityEngine;

namespace EIS.Runtime.Player
{
    public class PlayerController : Singleton<PlayerController>
    {
        [Tooltip("Move speed of the character")] [SerializeField]
        private float MoveSpeed = 5.0f;

        [Tooltip("Sprint speed of the character in m/s")] [SerializeField]
        private float SprintSpeed = 8.0f;

        [Tooltip("Rotation speed of the character")] [SerializeField]
        private float RotationSpeed = 1.0f;

        [Tooltip("Look up/down sensitivity")] [SerializeField]
        private float LookSensitivity = 2.0f;

        [Tooltip("Gravity applied to the player")] [SerializeField]
        private float Gravity = -9.81f;

        [SerializeField] private LayerMask groundLayers;

        private CharacterController characterController;
        private Vector3 velocity; // Stores current player velocity
        private bool isGrounded;  // Flag to indicate if player is grounded

        private float yRotation = 0.0f; // Store current y rotation

        void Start()
        {
            characterController = GetComponent<CharacterController>();
            Cursor.lockState = CursorLockMode.Locked;
            velocity = Vector3.zero; // Initialize velocity to zero
        }

        void Update()
        {
            Move();
            Rotate();
        }

        private void Move()
        {
            if (!PlayerRestrictions.Conditions.CanMove())
                return;

            // Get movement input
            float horizontalInput = Input.GetAxisRaw("Horizontal");
            float verticalInput = Input.GetAxisRaw("Vertical");

            // Determine movement speed (normal or sprint)
            float speed = MoveSpeed;
            if (Input.GetKey(KeyCode.LeftShift))
            {
                speed = SprintSpeed;
            }

            // Calculate movement vector
            Vector3 movement = transform.right * horizontalInput + transform.forward * verticalInput;
            movement.Normalize();

            // Ground check
            isGrounded = Physics.Raycast(transform.position, Vector3.down, characterController.height / 2f, groundLayers);

            // Apply gravity
            if (!isGrounded)
            {
                velocity.y += Gravity * Time.deltaTime;
            }
            else
            {
                velocity.y = -0.1f; // Set a small downward force when grounded to avoid sticking
            }

            // Combine movement and gravity
            movement.y = velocity.y;

            // Move the character
            characterController.Move(movement * (speed * Time.deltaTime));
        }

        private void Rotate()
        {
            if (!PlayerRestrictions.Conditions.CanRotateCamera())
                return;

            // Rotate the character based on mouse input
            float mouseX = Input.GetAxis("Mouse X");
            transform.Rotate(Vector3.up * (mouseX * RotationSpeed));

            // Look up/down with mouse
            float mouseY = Input.GetAxis("Mouse Y");
            yRotation -= mouseY * LookSensitivity;

            // Clamp y rotation to avoid flipping the camera
            yRotation = Mathf.Clamp(yRotation, -90.0f, 90.0f);

            // Apply y rotation to the camera (assuming camera is a child)
            transform.GetChild(0).transform.localRotation = Quaternion.Euler(yRotation, 0.0f, 0.0f);
        }
    }
}
