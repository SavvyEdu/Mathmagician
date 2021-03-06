﻿using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
public class Player : PhysicsObject, IHealthObject, IPlayer
{
    #region private fields
    [Header("Movement")]
    [SerializeField] private float moveSpeed; //how fast the object can move
    [SerializeField] private float jumpSpeed; //initial jump speed
    [Range(-gravityMag, gravityMag)] [SerializeField] private float addedSpeed; //gravity added on the way up
    private Vector2 movementInput; //user input that will move the player
    private bool jumping = false; //is the player jumping

    private Animator anim; //reference to attached animator component
    private SpriteRenderer render; //attached sprite renderer

    [Header("Health")]
    [SerializeField] private int maxHealth; //maximum health of the player
    private static float health = -1; //health of the object
    [SerializeField] private float invulnerabilityTime; //how long the invulnerability timer lasts
    private float invulnerabilityTimer;
    private bool invulnerable; //true when player is immune to damage

    private bool touchingLadder; //true when player is touching the ladder
    private bool climbing;
    private bool canSwim = false;
    private bool swimming;
    
    [Header ("Reseting")]
    private Vector2 returnPosition; //position to return to when the player falls off the map
    private bool returning = false; //true when player is returning to returnPosition
    private float returnTime; //Time it takes for player to return to position
    private Vector2 returnVelocity;
    [SerializeField] private int returnVelocityDivider;
    #endregion

    #region Properties 
    public float Health {
        get { return health; }
        set { health = value; }
    }
    public int MaxHealth { get { return maxHealth; } }
    public bool Invulnerable {
        get { return invulnerable; }
        set { invulnerable = value; }
    }
    public bool InFallZone {
        get { return inFallZone; }
        set { inFallZone = value; }
    }
    public bool TouchingWater { get { return touchingWater; } }
    public Vector2 ReturnPosition {
        get { return returnPosition; }
        set { returnPosition = value; }
    }
    public Animator Animator { get { return anim; } }

    public override Vector2 MoveVelocity { get { return moveVelocity * moveSpeed; } }
    public bool CanSwim { set { canSwim = value; } }
    public bool IsReturning { get { return returning; } }

    public float MoveSpeed {
        get { return moveSpeed; }
        set { moveSpeed = value; }
    }
    public float JumpSpeed {
        get { return jumpSpeed; }
        set { jumpSpeed = value; }
    }
    #endregion

    //Start is already being called in Base PhysicsObject Class
    private void Awake()
    {
        anim = GetComponent<Animator>();
        render = GetComponent<SpriteRenderer>();

        if (maxHealth < 1) { maxHealth = 1; } //must have at leath one health point
        if (health < 0) { health = maxHealth; }

        SetReturnPosition(transform.position); //set the return position to 1 unit above where the player initially spawned

        if (Global.startPosition != Vector2.one)
        {
            transform.position = Global.startPosition;
        }
    }

    protected override void Update()
    {
        if (!Global.paused)
        {
            //don't accept input when frozen
            if (frozen)
            {
                movementInput = inputVelocity;
                jumping = false;
                return;
            }
            //get and then modify cardinal input 
            movementInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
            if((climbing) && Input.GetButton("Jump")) { movementInput += Vector2.up; }
            movementInput = movementInput.normalized * Mathf.Clamp(movementInput.magnitude, 0, 1.0f) * moveSpeed; //make sure length of input vector is less than 1; 

            //can input a jump before you hit the ground
            if (Input.GetButtonDown("Jump"))
            {
                jumping = true;
            }
            //releasing jump always cancels jumping
            else if (Input.GetButtonUp("Jump"))
            {
                jumping = false;
            }
        }
    }

    protected override void FixedUpdate()
    {
        if (!Global.paused)
        {
            if (returning)
            {
                //move back to returnPosition
                moveVelocity = Vector2.SmoothDamp(transform.position, returnPosition, ref returnVelocity, returnTime);
                gravityVelocity = Vector2.up * moveVelocity.y; //set moveVelocities for camera
                transform.position = moveVelocity;
                if (((Vector2)transform.position - returnPosition).magnitude < 1)
                {
                    gravityVelocity = Vector2.zero;
                    returning = false;
                }
                //manual mapipulation of animator to falling animation
                anim.SetBool("grounded", false);
                anim.SetFloat("verticalVel", -1);
                return; //don't more the player conventually 
            }

            //determine moveVeclocity and if the object is moving
            moveVelocity = movementInput * Time.deltaTime;

            if (inFallZone)
            {
                //manual mapipulation of animator to falling animation
                anim.SetBool("grounded", false);
                anim.SetFloat("verticalVel", -1);

                gravityVelocity = Vector2.zero;

                base.FixedUpdate();

                gravityVelocity = gravity.normalized * maxGravity;
                return; //don't let the player move 
            }

            #region Movement
            //jumping when on ground
            if (jumping && (grounded || climbing || swimming))
            {
                gravityVelocity /*+*/= (inheritGravity ? groundNormal : Vector2.up) * jumpSpeed * Time.deltaTime;
                jumping = false; //insures that you can only jump once after pressing jump button
                //climbing = false; //jump out of climbing
            }

            //add velocity while moving upwards 
            if (Vector2.Dot(gravity, gravityVelocity) < 0) //check if moving upwards
            {
                CollideOneway(false);
                //add to velocity in direction of velocity proportional to velocity magnitude
                if (Input.GetButton("Jump"))
                {
                    gravityVelocity += addedSpeed * gravityVelocity.normalized * Time.deltaTime;
                }
            }
            //moving downwards
            else
            {
                CollideOneway(true);
            }

            //fall throught one way platforms when input is down
            if (grounded && Vector2.Dot(gravity, moveVelocity) > 0 &&
                Input.GetAxis("Vertical") < 0)
            {
                CollideOneway(false);
            }

            //start climbing if move velocity is up or down (not just side to side)
            if (touchingLadder && moveVelocity.y != 0)
            {
                climbing = true;
            }
            if(touchingWater)
            {
                swimming = canSwim;
            }
            touchingLadder = false; //reset every time 
            if (climbing)
            {
                //while climbing player can move in all 4 directions
                gravityVelocity = Proj(moveVelocity, groundNormal) - (gravity * Time.deltaTime);
                moveVelocity = Proj(moveVelocity, groundTangent);
            }
            else
            {
                //update moveVelocity to be only in direction of ground normal
                if (grounded) { moveVelocity = Proj(moveVelocity, groundTangent); }        
            }
            base.FixedUpdate();
            if (!touchingLadder)
            {
                climbing = false;
            }
            if (!touchingWater)
            {
                swimming = false;
            }
            #endregion

            #region Animation
            //send values to animator
            anim.SetBool("grounded", grounded || climbing);
            if (climbing)
            {
                anim.SetFloat("verticalVel", 0); //cancel fall animation
                //use greatest movement direction for movement animation
                anim.SetFloat("horizontalMove", gravityVelocity.magnitude > moveVelocity.magnitude 
                    ? gravityVelocity.magnitude : moveVelocity.magnitude);
            }
            else
            {
                anim.SetFloat("verticalVel", gravityVelocity.magnitude * (Vector2.Angle(gravity, gravityVelocity) > 90 ? 1 : -1));
                anim.SetFloat("horizontalMove", moveVelocity.magnitude * (Vector2.Dot(transform.right, moveVelocity) < 0 ? 1 : -1));
            }

            //invulnerabilty animation
            render.color = Color.white;
            if (invulnerable)
            {
                invulnerabilityTimer -= Time.deltaTime; //increase timer 
                if (invulnerabilityTimer <= invulnerabilityTime - 0.2f)
                {
                    //damage animation and color change
                    anim.SetBool("damage", false);
                    render.color = Color.gray; //fade Slightly
                }
                //stop the timer and end invulnerability
                if (invulnerabilityTimer <= 0)
                {
                    invulnerable = false;
                }
            }
            #endregion
        }
    }

    /// <summary>
    /// choose whether the object should collide with oneway platforms
    /// </summary>
    /// <param name="collide">true if object collides</param>
    private void CollideOneway(bool collide)
    {
        Physics2D.IgnoreLayerCollision(gameObject.layer, LayerMask.NameToLayer("Oneway"), !collide);
        filter.SetLayerMask(Physics2D.GetLayerCollisionMask(gameObject.layer));
    }

    protected override void HitSpikes()
    {
        Damage(2);
    }

    protected override void TouchLadder()
    {
        touchingLadder = true;
        //start climbing if move velocity is up or down (not just side to side)
        if (moveVelocity.y != 0)
        {
            climbing = true;
        }
    }

    protected override void TouchWater()
    {
        base.TouchWater();
        //start climbing if move velocity is up or down (not just side to side)
        swimming = canSwim;
    }

    #region Health
    public void Damage(float amount)
    {
        if (!invulnerable)
        {
            health -= amount;
            invulnerable = true;
            invulnerabilityTimer = invulnerabilityTime; //start the timer
            anim.SetBool("damage", true);
            //death code
            if (health <= 0)
            {
                health = 0;
                frozen = true;
                anim.SetTrigger("death");
                jumping = false;
                //reset game is called by the death animation
            }
        }
    }

    public void Heal(float amount)
    {
        //add health up to max health
        health = Mathf.Clamp(health + amount, 0, MaxHealth);
    }

    public void FullHeal()
    {
        health = maxHealth;
    }
    #endregion

    private void SetReturnPosition(Vector2 set)
    {
        returnPosition = set + Vector2.up;
    }

    public void OnPlayerFall()
    {
        returnVelocity = Vector2.zero; //reset the return Velocity
        returnTime = ((Vector2)transform.position - returnPosition).magnitude / returnVelocityDivider;
        returning = true; //start returning to return position
    }
}