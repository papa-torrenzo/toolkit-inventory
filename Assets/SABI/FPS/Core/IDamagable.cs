namespace SABI
{
    public interface IDamagable
    {
        public void TakeDamage(float damage, IDamagableSource source);
    }
}
