using DG.Tweening;
using UnityEngine;
/**
 * Title:左右移动，冲刺，跳跃
 * Description:
 */
public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    [SerializeField] private float jumpForce = 5;
    [SerializeField] private float moveSpeed = 5;
    [SerializeField] private float sprintDistance = 5;
    [SerializeField] private float sprintDuration = 0.2f;
    private SpriteRenderer sprite;

    [Header("运动状态")]
    public bool _isSprinting = false;
    public bool _isFalling = false;
    public bool isFacingRight = true;

    Vector2 moveInput;

    #region 生命周期
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
    }
    private void Update()
    {
        moveInput.x = Input.GetAxis("Horizontal");
        moveInput.y = Input.GetAxis("Vertical");
        if (Input.GetButtonDown("Jump"))
        {
            rb.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
        }
        if (Input.GetButtonDown("Dash") && !_isSprinting)
        {
            Dash();
        }
        _isFalling = rb.velocity.y < -0.1f;
    }
    protected void FixedUpdate()
    {
        rb.velocity = new Vector2(moveSpeed * moveInput.x, rb.velocity.y);
        if (moveInput.x < 0 && isFacingRight)
        {
            isFacingRight = false;
            sprite.flipX = true;
        }
        else if (moveInput.x > 0 && !isFacingRight)
        {
            isFacingRight = true;
            sprite.flipX = false;
        }
    }
    private void OnDisable()
    {
        DOTween.Kill(gameObject);
    }
    #endregion

    void Dash()
    {
        _isSprinting = true;
        float endX = transform.position.x + sprintDistance * (isFacingRight ? 1 : -1);
        //TODO:冲刺功能.这个穿墙
        DOTween.To(() => transform.position.x, (x) => transform.position = new Vector3(x, transform.position.y, transform.position.z), endX, sprintDuration).onComplete += () =>
        {
            _isSprinting = false;
        };
    }
}
