using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;

[SelectionBase]
public class Player_Movement : MonoBehaviour
{
    #region Editor Data
    [Header("Movement Settings")]
    [SerializeField] float _moveSpeed = 300f;
    [SerializeField] private float maxJumpForce;
    [SerializeField] private float minJumpForce;
    [SerializeField] private float maxJumpTime;
    [SerializeField] private float fallMultiplier = 2.5f; 
    [SerializeField] private float lowJumpMultiplier = 2f; 


    [Header("Dependencies")]
    [SerializeField] Rigidbody2D _rb;
    [SerializeField] Animator _anim;
    [SerializeField] private Collider2D groundCheck;
    [SerializeField] private Transform wallCheck;
    [SerializeField] private LayerMask wallLayer;
    #endregion

    #region  Private Data
    private Vector2 _moveDir = Vector2.zero;
    private bool _isGrounded;
    private bool _isJumping;
    private float _jumpTimeCounter;
    private bool _isWallSliding;
    private float _wallSlidingSpeed = 2f;
    #endregion

    #region  UpdateFýxedUpdate
    private void Update()
    {
        Get_Input();
        Get_Jump_Input();
        WallSlide();
        _isGrounded = CheckIfGrounded();

        if (transform.position.y < -9.0f)
        {
            EditorApplication.isPlaying = false;
        }
    }


    private void FixedUpdate()
    {
        Movement_Update();
        _anim.SetFloat("_xVelocity", math.abs(_moveDir.x));
        _anim.SetFloat("_yVelocity", Convert.ToInt16(_rb.velocity.y));
        slideControls();
        if (_moveDir.x > 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        else if (_moveDir.x < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
        HandleJumping();
    }
    #endregion

    #region INPUT
    private void Get_Input()
    {
        if (!_isWallSliding || Convert.ToInt16(_rb.velocity.y) == 0)
            _moveDir.x = Input.GetAxis("Horizontal");
    }

    private void Get_Jump_Input() 
    {
        _isGrounded = CheckIfGrounded();

        if (((_isGrounded) || _isWallSliding) && Input.GetButtonDown("Jump"))
        {
            _isJumping = true;
            _jumpTimeCounter = 0f;

            if (_isGrounded)
            {
                _rb.velocity = new Vector2(_rb.velocity.x, minJumpForce);
            }
            else if (_isWallSliding) 
            {
                if (transform.localScale.x > 0)
                {
                    _rb.velocity = new Vector2(-10f, minJumpForce);
                    transform.localScale = new Vector3(-1, 1, 1);
                }
                else
                {
                    _rb.velocity = new Vector2(10f, minJumpForce);
                    transform.localScale = new Vector3(1, 1, 1);
                }
                _moveDir.x = 0;
            }
        }

        if (Input.GetButtonUp("Jump"))
        {
            _isJumping = false;
        }
    }

    #endregion

    #region MOVEMENT
    private void Movement_Update()
    {
        _rb.velocity = new Vector2(_moveDir.x * _moveSpeed * Time.fixedDeltaTime, _rb.velocity.y);
    }

    private bool CheckIfGrounded()
    {
        if (groundCheck == null)
            return false;

        Bounds bounds = groundCheck.GetComponent<BoxCollider2D>().bounds;
        Collider2D[] colliders = Physics2D.OverlapBoxAll(bounds.center, bounds.size, 0f);

        foreach (Collider2D collider in colliders)
        {
            if (collider.gameObject != gameObject) 
            {
                _anim.SetBool("_isJumping", false);
                return true;
            }
        }
        return false;
    }
    #endregion

    #region Jump Handling
    private void HandleJumping()
    {
        if (_isJumping && Input.GetButton("Jump"))
        {
            if (_jumpTimeCounter < maxJumpTime)
            {
                _rb.velocity = new Vector2(_rb.velocity.x, maxJumpForce);
                _jumpTimeCounter += Time.fixedDeltaTime;
            }
            else
            {
                _isJumping = false;
            }
        }

        if (_rb.velocity.y < 0 && !_isWallSliding)
        {
            _rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime;
        }
        else if (_rb.velocity.y > 0 && !Input.GetButton("Jump") && !_isWallSliding)
        {
            _rb.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.fixedDeltaTime;
        }
    }
    #endregion


    #region WallJump

    private bool wallContact() 
    {
        return Physics2D.OverlapCircle(wallCheck.position, 0.2f, wallLayer);
    }

    private void WallSlide() 
    {
        if (wallContact() && !_isJumping)
        {
            _anim.SetBool("_isSliding", true);
            _anim.SetBool("_isJumping", false);
            Debug.Log(_anim.GetBool("_isJumping"));
            _isWallSliding = true;
            _rb.velocity = new Vector2(_rb.velocity.x, Mathf.Clamp(_rb.velocity.y, -_wallSlidingSpeed, float.MaxValue));
        }
        else 
        {
            _anim.SetBool("_isSliding", false);
            _isWallSliding = false;
        }
    }

    private void slideControls() 
    {
        if (_isWallSliding)
        {
            _anim.SetBool("_isJumping", false);
            _anim.SetBool("_isSliding", true);
            _moveSpeed = 225f;
        }
        else if (!_isGrounded)
        {
            _anim.SetFloat("_xVelocity", math.abs(_moveDir.x) - math.abs(_moveDir.x));
            _anim.SetBool("_isJumping", true);
            _anim.SetBool("_isSliding", false);
            _moveSpeed = 225f;
        }
        else
        {
            _anim.SetBool("_isJumping", false);
            _anim.SetBool("_isSliding", false);
            _moveSpeed = 300f;
        }
    }

    #endregion
}
