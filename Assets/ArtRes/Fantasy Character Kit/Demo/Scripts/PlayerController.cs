using UnityEngine;

namespace Cute2dDemo
{
    public class PlayerController : MonoBehaviour
    {
        /*
         This script needs components listed below.
            1. <Rigidbody2D>
            2. <Collider2D> : for collision
            3. <Collider2D>(isTrigger:True) : for ground detection
         */
        public float movePower = 10f;
        public float jumpPower = 15f;

        private Rigidbody2D rb;
        private Animator anim;
        private bool isJumping = false;
        private bool alive = true;


        // Start is called before the first frame update
        void Start()
        {
            anim = GetComponent<Animator>();
            rb = GetComponent<Rigidbody2D>();
            rb.gravityScale = 5;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                Restart();
            }

            if (alive)
            {
                if (Input.GetAxisRaw("Horizontal") < 0)
                {
                    RunLeft();
                }
                else if (Input.GetAxisRaw("Horizontal") > 0)
                {
                    RunRight();
                }
                else
                {
                    RunStop();
                }

                if (Input.GetButtonDown("Jump") || Input.GetAxisRaw("Vertical") > 0)
                {
                    Jump();
                }

                if (Input.GetKeyDown(KeyCode.Alpha1))
                {
                    Hurt();
                }

                if (Input.GetKeyDown(KeyCode.Alpha2))
                {
                    Die();
                }
                if (Input.GetMouseButtonDown(0))
                {
                    Attack();
                }
                if (Input.GetMouseButtonDown(1))
                {
                    Skill();
                }
                if (Input.GetKeyDown(KeyCode.Alpha4))
                {
                    Attack2();
                }
                if (Input.GetKeyDown(KeyCode.Alpha5))
                {
                    Skill2();
                }
            }
        }


        void Restart()
        {
                anim.SetTrigger("idle");
                alive = true;
        }

        void RunLeft()
        {
            if (transform.localScale.x > 0)
                transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
            if (!anim.GetBool("isJump"))
                anim.SetBool("isRun", true);
            rb.velocity = new Vector2(-movePower, rb.velocity.y);
        }
        void RunRight()
        {
            if (transform.localScale.x < 0)
                transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
            if (!anim.GetBool("isJump"))
                anim.SetBool("isRun", true);
            rb.velocity = new Vector2(movePower, rb.velocity.y);
        }
        void RunStop()
        {
            anim.SetBool("isRun", false);
            rb.velocity = new Vector2(0, rb.velocity.y);
        }
        void Jump()
        {
            if(!isJumping && rb.velocity.y <= 0)
            {
                isJumping = true;
                anim.SetBool("isJump", true);
                rb.velocity = new Vector2(rb.velocity.x, jumpPower);
            }
        }

        //groundCheck
        private void OnTriggerStay2D(Collider2D collision)
        {
            if(collision.gameObject != gameObject)
            {
                if(rb.velocity.y <= 0)
                {
                isJumping = false;
                anim.SetBool("isJump", false);
                }
            }
        }

        void Hurt()
        {
            anim.SetTrigger("hurt");
            if (transform.localScale.x > 0)
            {
                rb.AddForce(new Vector2(-5f, 5f), ForceMode2D.Impulse);
                transform.position += new Vector3(-0.5f, 0.03f, 0f);
            }
            else
            {
                rb.AddForce(new Vector2(5f, 5f), ForceMode2D.Impulse);
                transform.position += new Vector3(0.5f, 0.03f, 0f);
            }
        }
        void Die()
        {
            anim.SetTrigger("die");
            alive = false;
        }
        void Attack()
        {
                anim.SetTrigger("attack");
        }
        void Attack2()
        {
            anim.SetTrigger("attack2");
        }
        void Skill()
        {
            anim.SetTrigger("skill");
        }
        void Skill2()
        {
            anim.SetTrigger("skill2");
        }
    }
}
