namespace SABI
{
    using SABI;
    using UnityEngine;

    [RequireComponent(typeof(Rigidbody), typeof(SphereCollider))]
    public class BaseProjectile : MonoBehaviour
    {
        public enum CollisionEnterBehaviour
        {
            None,
            BasicProjectile,
        }

        [SerializeField]
        private CollisionEnterBehaviour collisionEnterBehaviour =
            CollisionEnterBehaviour.BasicProjectile;

        protected new Rigidbody rigidbody;

        [SerializeField]
        protected bool shootOnStart = true;

        [SerializeField]
        private GameObject MainProjectileObject;

        [SerializeField]
        private ParticleSystem vfx_spawn,
            vfx_hit;

        [SerializeField]
        private bool instantiateVfxInsteadOfPlayingIt;

        [SerializeField]
        private float force = 10;

        [SerializeField]
        private AudioClip audio_spawn,
            audio_hit;

        void Awake() => rigidbody = GetComponent<Rigidbody>();

        void Start()
        {
            if (shootOnStart)
                Shoot(transform.forward * force);
        }

        [ContextMenu("Shoot")]
        protected void Shoot(Vector3 direction)
        {
            if (rigidbody == null)
                return;
            rigidbody.linearVelocity = direction.normalized * force;
            PlayAudioClip(audio_spawn);
            HandleVfx(vfx_spawn);
        }

        void OnCollisionEnter(Collision other)
        {
            switch (collisionEnterBehaviour)
            {
                case CollisionEnterBehaviour.None:
                    break;
                case CollisionEnterBehaviour.BasicProjectile:
                    if (other.collider == GetComponent<Collider>())
                        return;
                    if (MainProjectileObject)
                        MainProjectileObject.SetActive(false);
                    PlayAudioClip(audio_hit);
                    HandleVfx(vfx_hit);
                    // this.DelayedExecution(vfx_hit.main.duration, () => this.DestroyGameObject());
                    Collider collider = GetComponent<Collider>();
                    break;
            }
        }

        private void PlayAudioClip(AudioClip clip)
        {
            if (clip != null)
                AudioManager.Instence.Play(clip);
        }

        void HandleVfx(ParticleSystem particleSystem)
        {
            if (particleSystem == null)
                return;

            if (instantiateVfxInsteadOfPlayingIt)
                Instantiate(particleSystem);
            else
                particleSystem.Play();
        }
    }
}
