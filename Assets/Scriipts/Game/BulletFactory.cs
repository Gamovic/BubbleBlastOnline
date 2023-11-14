
using UnityEngine;

public class BulletFactory : Factory
{
    private Bullet bullet;
    private SpriteRenderer bulletSR;
    private static BulletFactory instance;

    public static BulletFactory Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new BulletFactory();
            }
            return instance;
        }
    }

    private void CreatePrototype(ref SpriteRenderer sr, ref Bullet bullet, string sprite, float speed)
    {
        bullet = new Bullet(speed);
        sr = new SpriteRenderer(Resources.Load(sprite));
    }

    public override GameObject Create(Vector2 position, string type)
    {
        GameObject bulletGO= new GameObject();
        bulletGO.transform.position = position;

        Bullet bulletClone = bullet.Clone();
        bullet.sprite = projectiles[currentIndex];
        projectileSR.GetComponent<SpriteRenderer>().color = Color.white; // Optional color adjustment

        projectileGO.AddComponent<Collider2D>();
        projectileGO.AddComponent<Projectile>();
        projectileGO.AddComponent<SpriteRenderer>().sprite = projectileSR.sprite;

        return projectileGO;
    }
}
