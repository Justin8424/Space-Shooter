using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powerup : MonoBehaviour
{

    [SerializeField]
    private float _speed = 3.0f;
    [SerializeField] // 0= Triple Shot, 1= Speed, 2 = Shield
    private int powerupID;
   
    [SerializeField]
    private AudioClip _powerupClip;

    // Update is called once per frame

    void Update()
    {
        transform.Translate(Vector3.down * _speed * Time.deltaTime);
        if (transform.position.y < -4.5f)
        {
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            Player player = other.transform.GetComponent<Player>();

            AudioSource.PlayClipAtPoint(_powerupClip, new Vector3(0, 1,-10), 1f);
            if (player != null)
            {
             
                switch (powerupID)
                {
                    case 0:
                        player.TripleShotActive();
                        break;
                    case 1:
                        player.SpeedupActive();
                        break;
                    case 2:
                        player.ShieldsActive();
                        break;
                    case 3:
                        player.AmmoPowerUp();
                        break;
                    case 4:
                        player.ExtraLifePickup();
                        break;
                    case 5:
                        player.ThreeWayShotActive();
                        break;
                    default:
                        Debug.Log("Default Value");
                        break;

                }
                Destroy(this.gameObject);
            }
        }
    }
}
