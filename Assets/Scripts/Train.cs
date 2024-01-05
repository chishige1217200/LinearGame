using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Train : MonoBehaviour
{
    public int size = 0;
    [HideInInspector] public Rigidbody2D rb { get; private set; }
    [HideInInspector] public PolygonCollider2D pc { get; private set; }

    private void Awake()
    {
        this.rb = GetComponent<Rigidbody2D>();
        this.pc = GetComponent<PolygonCollider2D>();
    }

    private void Update()
    {
        if (this.rb != null && IsOutOfRange())
        {
            GameController gc = GameObject.Find("GameController").GetComponent<GameController>();
            gc.GameOver();
        }
    }

    private bool IsOutOfRange()
    {
        //if (this.transform.position.y > 5.8f) return true;
        if (Mathf.Abs(this.transform.position.x) > 4f) return true;

        return false;
    }

    public void SetPosition(Vector3 position)
    {
        this.transform.position = position;
    }

    public void DoDrop()
    {
        this.rb.simulated = true;
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (this.rb.simulated) // ������̂�
        {
            if (collision.gameObject.CompareTag("Train") && this.size == collision.gameObject.GetComponent<Train>().size) // ���T�C�Y�̎ԗ��Ƃ̏Փ�
            {
                GameController gc = GameObject.Find("GameController").GetComponent<GameController>();
                if (this.size < 10) // ���j�A�ȊO
                {
                    if (JudgeMerge(collision)) // ���̂��Ĉ�傫���T�C�Y��
                    {
                        // TODO:���̉���炷
                        gc.PlayMergeSound();
                        gc.AddScore(this.size);
                        Instantiate(gc.trains[size + 1],
                            new Vector3((this.transform.position.x + collision.transform.position.x) / 2, (this.transform.position.y + collision.transform.position.y) / 2, 0),
                            Quaternion.identity, gc.trainGroup).GetComponent<Train>().DoDrop();

                    }
                    Destroy(this.gameObject);
                }
                else // ���j�A�̏ꍇ
                {
                    if (JudgeMerge(collision))
                    {
                        gc.PlayMergeSound();
                        gc.AddScore(this.size);
                    }
                    Destroy(this.gameObject);
                }
            }
        }
    }

    //public void OnTriggerEnter2D(Collider2D collision)
    //{
    //    if (collision.gameObject.CompareTag("GameOver"))
    //    {
    //        GameController gc = GameObject.Find("GameController").GetComponent<GameController>();
    //        gc.GameOver();
    //    }
    //}

    private bool JudgeMerge(Collision2D collision) // ���̂��邩���ł��邩�𔻒肷��
    {
        if (this.transform.position.y < collision.transform.position.y)// y���W����������
        {
            return true;
        }
        if (this.transform.position.y == collision.transform.position.y) // y���W��������
        {
            Rigidbody2D opponent = collision.gameObject.GetComponent<Rigidbody2D>();
            if (this.rb.velocity.magnitude < opponent.velocity.magnitude) // ���x����������
            {
                return true;
            }
            if (this.rb.velocity.magnitude == opponent.velocity.magnitude) // ���x��������
            {
                if (Mathf.Abs(this.transform.position.x) > Mathf.Abs(collision.transform.position.x)) // x���W�̐�Βl���傫����
                {
                    return true;
                }
            }
        }
        return false;
    }
}
