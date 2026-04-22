namespace SABI
{
    public abstract class MWM_Magazine : MWM
    {
        public abstract bool GetIsMagazineFull();
        public abstract bool GetIsMagazineEmpty();
        public abstract int GetBulletsLeft();
        public abstract void SetMagazineFull();
        public abstract void SetBulletsLeft(int bulletsLeft);
        public abstract void RemoveOneBullet();
    }
}
