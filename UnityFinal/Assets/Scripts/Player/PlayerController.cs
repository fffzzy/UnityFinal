﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Control the player movement
// Only can handle the input directly here
// All animations about movement are implemented here
// Notice that we need two player.
// Save the playerLocation as two index[x][z] (which block the player is currently land on)

public enum PlayerID {
    Nobody = -1,
    Player1 = 0,
    Player2 = 1,
    // Player3 = 2,
    // Player4 = 3,
}

public class PlayerController : MonoBehaviour
{
    float DefaultMoveCoolDownTime = 0.15f;    
    float MoveCoolDownTime;    
    
    float m_moveCoolDownTimer = 0;
    
    
    PlayerID m_playerID;

    Animator animator;

    Direction m_direction;
    BlockLogic nextBlock;
    private int xIndex;
    private int zIndex;
    public bool m_isAttacking;   
    private bool jumpState; 
 
    private bool m_isMoving;
    // Start is called before the first frame update
    void Start()
    {
        m_isAttacking = false;
        m_isMoving = false;
        jumpState = false;
        MoveCoolDownTime = DefaultMoveCoolDownTime;
        animator = GetComponent<Animator>();
    }

    public void InitInfo(PlayerID id, int x, int z) {
        m_playerID = id;
        xIndex = x;
        zIndex = z;
        GetComponent<PlayerLogic>().InitInfo(id);
    }

    void Update() {

        if (m_moveCoolDownTimer < MoveCoolDownTime) {
            m_moveCoolDownTimer += Time.deltaTime;
        }

        float m_horizontalFireInput = Input.GetAxisRaw(m_playerID.ToString() + "_HorizontalFire");                
        float m_verticalFireInput = Input.GetAxisRaw(m_playerID.ToString() + "_VerticalFire");
        bool isHorizontalFiring = Input.GetButton(m_playerID.ToString() + "_HorizontalFire");
        bool isVerticalFiring = Input.GetButton(m_playerID.ToString() + "_VerticalFire");

        float m_horizontalMoveInput = Input.GetAxisRaw(m_playerID.ToString() + "_HorizontalMove");                
        float m_verticalMoveInput = Input.GetAxisRaw(m_playerID.ToString() + "_VerticalMove");
        bool isHorizontalMoving = Input.GetButton(m_playerID.ToString() + "_HorizontalMove");
        bool isVerticalMoving = Input.GetButton(m_playerID.ToString() + "_VerticalMove");
        bool isMoved = false;     


        if (!(isHorizontalFiring && isVerticalFiring)) { // Prevent Vertical Fire            
            if (isHorizontalFiring) {
                if (m_horizontalFireInput > 0) {
                    TryAttack(Direction.Right);
                } else {
                    TryAttack(Direction.Left);
                }        
            }

            if (isVerticalFiring) {
                if (m_verticalFireInput > 0) {
                    TryAttack(Direction.Up);
                } else {
                    TryAttack(Direction.Down);
                }
            }
        }


        if (!m_isAttacking && !(isHorizontalMoving && isVerticalMoving) && (m_moveCoolDownTimer > MoveCoolDownTime)) { // Prevent Moving Diagonally
            if ( isHorizontalMoving && !m_isMoving) {
                
                if (m_horizontalMoveInput > 0) {
                    isMoved = TryMove(Direction.Right);
                } else {
                    isMoved = TryMove(Direction.Left);
                }

                if (isMoved) {
                    m_isMoving = true;
                    m_moveCoolDownTimer = 0.0f;
                }
            }

            if ( isVerticalMoving && !m_isMoving) {
                
                if (m_verticalMoveInput > 0) {
                    isMoved = TryMove(Direction.Up);
                } else {
                    isMoved = TryMove(Direction.Down);
                }

                if (isMoved) {
                    m_isMoving = true;
                    m_moveCoolDownTimer = 0.0f;
                }
            }
        }       

        

        if (Input.GetButtonDown(m_playerID.ToString() + "_CastPowerUp")) {
            GetComponent<PlayerLogic>().castCurrentTakingPowerUp();
        }

    }

    // Update is called once per frame
    void FixedUpdate()
    {

    }
    
    bool TryMove(Direction direction) {
        // return true if it successfully moved
        
        // Handle edge
        if (transform.parent.gameObject.GetComponent<BlockLogic>().isEdge[(int)direction]) {
            return false;
        }
        
        
        int new_x_index = xIndex;
        int new_z_index = zIndex;
        switch(direction) {
            case Direction.XPlus:
                transform.rotation = Quaternion.LookRotation(Vector3.right); // Update the player facing direction
                new_x_index += 1;
            break;
            case Direction.XMinus:
                transform.rotation = Quaternion.LookRotation(Vector3.left);       
                new_x_index -= 1;
            break;
            case Direction.ZPlus:
                transform.rotation = Quaternion.LookRotation(Vector3.forward);
                new_z_index += 1;
            break;
            case Direction.ZMinus:
                transform.rotation = Quaternion.LookRotation(Vector3.back);                
                new_z_index -= 1;
            break;
        }
        m_direction = direction;

        BlockLogic block = MapLogic.Instance.getBlock(new_x_index, new_z_index).GetComponent<BlockLogic>(); // The new block that player is trying to accessing
        if (block.isWalkable()) {
            BlockLogic cur_block = MapLogic.Instance.getBlock(xIndex, zIndex).GetComponent<BlockLogic>(); // current block

            if (jumpState) {
                animator.SetTrigger("Jump1");
            } else {
                animator.SetTrigger("Jump2");
            }
            jumpState = !jumpState;
            
            // transform.SetParent(block.transform, false); // set the parent to the certain block to move the player        
            // block.setPlayer(gameObject);
            return true;
        }        
        return false;
    }

    bool TryAttack(Direction direction) {
        WeaponLogic weapon = GetComponentInChildren<WeaponLogic>();
        if (!weapon) {
            return false;
        }
        switch(direction) {
            case Direction.XPlus:
                if (!m_isMoving) {
                    transform.rotation = Quaternion.LookRotation(Vector3.right); // update the player facing direction
                }
            break;
            case Direction.XMinus:
                if (!m_isMoving) {
                    transform.rotation = Quaternion.LookRotation(Vector3.left); // update the player facing direction
                }
            break;
            case Direction.ZPlus:
                if (!m_isMoving) {
                    transform.rotation = Quaternion.LookRotation(Vector3.forward); // update the player facing direction
                }
            break;
            case Direction.ZMinus:
                if (!m_isMoving) {
                    transform.rotation = Quaternion.LookRotation(Vector3.back); // update the player facing direction
                }
            break;
        }

        if (!m_isMoving) {
            m_direction = direction;    
        }
        if (GetComponentInChildren<WeaponLogic>()) {
            // Call the attack funcion in the weapon
            return GetComponentInChildren<WeaponLogic>().tryAttack();
        }
        return false;
    }

    void MoveToNextBlock() {        
        if (transform.parent.gameObject.GetComponent<BlockLogic>().isEdge[(int)m_direction]) {
            return;
        }

        int new_x_index = xIndex;
        int new_z_index = zIndex;
        switch(m_direction) {
            case Direction.XPlus:                
                new_x_index += 1;
            break;
            case Direction.XMinus:                
                new_x_index -= 1;
            break;
            case Direction.ZPlus:                
                new_z_index += 1;
            break;
            case Direction.ZMinus:            
                new_z_index -= 1;
            break;
        }        

        BlockLogic block = MapLogic.Instance.getBlock(new_x_index, new_z_index).GetComponent<BlockLogic>(); // The new block that player is trying to accessing
        
        if (block.isWalkable()) {
            BlockLogic cur_block = MapLogic.Instance.getBlock(xIndex, zIndex).GetComponent<BlockLogic>(); // current block     
            cur_block.resetPlayer();
            xIndex = new_x_index;
            zIndex = new_z_index;
            
            // set the parent to the certain block to move the player            
            transform.SetParent(block.transform, true);            
            block.setPlayer(gameObject);         
        }                
    }

    public void finishMoving() {
        m_isMoving = false;

    }

    public void applySpeedUp() {
        animator.SetFloat("SpeedMultiplier", 1.4f);
        MoveCoolDownTime = DefaultMoveCoolDownTime * 1 / 1.4f;
    }
    
    public void resetSpeed() {
        animator.SetFloat("SpeedMultiplier", 1);
        MoveCoolDownTime = DefaultMoveCoolDownTime;
    }
}
