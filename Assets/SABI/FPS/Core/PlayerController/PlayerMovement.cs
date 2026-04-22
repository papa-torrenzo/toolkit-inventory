using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace SABI
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerMovement : MonoBehaviour
    {
        public float speed = 6.0f;
        public float jumpSpeed = 8.0f;
        public float gravity = 20.0f;

        private Vector3 moveDirection = Vector3.zero;
        private CharacterController controller;

        [SerializeField]
        private Image jetpackFuelSlider;

        public float jetpackForce = 5.0f; // The force applied when using the jetpack
        public float maxJetpackFuel = 2.0f; // Maximum time the jetpack can be used
        private float currentJetpackFuel;
        public float jetpackFuelConsumptionRate = 1.0f; // Rate at which fuel is consumed
        public float jetpackRechargeRate = 0.5f; // Rate at which fuel recharges when grounded
        public float airControlMultiplier = 0.5f;

        // Footstep variables
        public AudioClip[] footstepClips; // Array of footstep sounds
        public float footstepInterval = 0.5f; // Time between footsteps
        private float nextFootstepTime = 0f; // Time for the next footstep

        // Jetpack sound variables
        public AudioClip jetpackClip; // Jetpack sound

        [SerializeField]
        private float jetAudioVolume = 1,
            footstepAudioVolume = 1;

        [SerializeField]
        private AudioObject jetpackAudioObject;

        private void Awake()
        {
            controller = GetComponent<CharacterController>();
        }

        private void Start()
        {
            currentJetpackFuel = maxJetpackFuel; // Start with full fuel
        }

        void Update()
        {
            // Horizontal movement input
            float horizontalAxis = 0f;
            float verticalAxis = 0f;

            if (Keyboard.current.dKey.isPressed)
                horizontalAxis += 1f;
            if (Keyboard.current.aKey.isPressed)
                horizontalAxis -= 1f;
            if (Keyboard.current.wKey.isPressed)
                verticalAxis += 1f;
            if (Keyboard.current.sKey.isPressed)
                verticalAxis -= 1f;

            Vector3 horizontalInput = new Vector3(horizontalAxis, 0.0f, verticalAxis);
            horizontalInput = transform.TransformDirection(horizontalInput);

            if (controller.isGrounded)
            {
                // Move direction based on axes when grounded
                moveDirection = horizontalInput * speed;

                if (Keyboard.current.spaceKey.isPressed)
                {
                    moveDirection.y = jumpSpeed;
                }

                // Play footstep sounds
                if (horizontalInput.magnitude > 0 && Time.time >= nextFootstepTime)
                {
                    PlayFootstepSound();
                    nextFootstepTime = Time.time + footstepInterval;
                }

                // Stop jetpack sound if grounded
                if (jetpackAudioObject?.GetIsActive() ?? false)
                    AudioManager.Instence.StopAudio(jetpackAudioObject);

                // Recharge jetpack fuel when grounded
                currentJetpackFuel = Mathf.Min(
                    currentJetpackFuel + jetpackRechargeRate * Time.deltaTime,
                    maxJetpackFuel
                );
            }
            else
            {
                // Air control for jetpack
                moveDirection.x = horizontalInput.x * speed * airControlMultiplier;
                moveDirection.z = horizontalInput.z * speed * airControlMultiplier;

                // Jetpack logic when in the air
                if (Keyboard.current.spaceKey.isPressed && currentJetpackFuel > 0)
                {
                    moveDirection.y = jetpackForce; // Apply upward force

                    // Consume fuel
                    currentJetpackFuel -= jetpackFuelConsumptionRate * Time.deltaTime;

                    if (
                        !jetpackAudioObject
                        || (jetpackAudioObject && !jetpackAudioObject.GetIsActive())
                    )
                    {
                        jetpackAudioObject = AudioManager.Instence.PlayAndGetAudioObject(
                            jetpackClip,
                            isContinues: true,
                            volume: jetAudioVolume
                        );
                    }
                }
                else
                {
                    // Apply gravity when not using the jetpack
                    moveDirection.y -= gravity * Time.deltaTime;

                    // Stop jetpack sound if not using jetpack
                    if (jetpackAudioObject?.GetIsActive() ?? false)
                        AudioManager.Instence.StopAudio(jetpackAudioObject);
                }
            }

            if (jetpackFuelSlider)
                jetpackFuelSlider.fillAmount = currentJetpackFuel / maxJetpackFuel;

            // Move the controller
            controller.Move(moveDirection * Time.deltaTime);
        }

        private void PlayFootstepSound()
        {
            if (footstepClips.Length > 0)
            {
                AudioManager.Instence.Play(
                    footstepClips.GetRandomItem(),
                    volume: footstepAudioVolume
                );
            }
        }
    }
}
