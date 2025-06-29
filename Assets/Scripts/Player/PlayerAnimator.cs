using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    //References
    Animator am;
    PlayerMovement pm;
    SpriteRenderer sr;


    void Start()
    {
        am = GetComponent<Animator>();
        pm = GetComponent<PlayerMovement>();
        sr = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (pm.moveDir != Vector2.zero)
        {
            am.SetBool("Move", true);

            SpriteDirectionChecker();
        }
        else
        {
            am.SetBool("Move", false);
        }
    }

    void SpriteDirectionChecker()
    {
        if (pm.moveDir.x > 0)
        {
            sr.flipX = false;
        }
        else if (pm.moveDir.x < 0)
        {
            sr.flipX = true;
        }
    }

    public void SetAnimatorController(RuntimeAnimatorController c)
    {
        if (!am) am = GetComponent<Animator>();
        am.runtimeAnimatorController = c;
    }
}
