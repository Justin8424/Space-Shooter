using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mine : MonoBehaviour
{
    [SerializeField]
    private float _maxHeight = 1f;
    [SerializeField]
    private float _minHeight= .5f;
    [SerializeField]
    private float _hoverSpeed = 3;

    [SerializeField]
    private AudioSource _audioSource;
    [SerializeField]
    private AudioClip _explosionSound;

    private float _hoverHeight;
    private float _hoverRange;
    private Vector3 _posOffSet = new Vector3();
    private Vector3 tempPos = new Vector3();
 
    private Animator _anim;

    // Start is called before the first frame update
    void Start()
    {
        _anim = GetComponent<Animator>();
        _audioSource = GetComponent<AudioSource>();
        _hoverHeight = (_maxHeight + _minHeight) / 2.0f;
        _hoverRange = _maxHeight - _minHeight;
        _posOffSet = transform.position;
        if (_audioSource == null)
        {
            Debug.LogError("The Enemy Audio Source is NULL");
        }
        else
        {
            _audioSource.clip = _explosionSound;
        }
    }

    // Update is called once per frame
    void Update()
    {
        MineHover();
      
    }

    void MineHover()
    {
        tempPos = _posOffSet;
        tempPos.y += Mathf.Cos(Time.time * _hoverSpeed) * _hoverRange;
        transform.position = tempPos;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            Player player = other.transform.GetComponent<Player>();
            player.Damage();
            _anim.SetTrigger("OnMineDeath");
            _audioSource.Play();

            Destroy(GetComponent<Collider2D>());
            Destroy(this.gameObject, 2.8f);
        }
        if (other.tag == "Laser")
        {
            Destroy(other.gameObject);
            _anim.SetTrigger("OnMineDeath");
            _audioSource.Play();

            Destroy(GetComponent<Collider2D>());
            Destroy(this.gameObject, 2.8f);
        }
    }
}
