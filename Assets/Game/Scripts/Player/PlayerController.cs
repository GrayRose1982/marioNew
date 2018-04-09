using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Com.LuisPedroFonseca.ProCamera2D;
using DG.Tweening;
using Spine.Unity;
using UpdateType = DG.Tweening.UpdateType;

public class PlayerController : MonoBehaviour, IAttackable, IAttackedable, ICharacter
{
    #region Variable

    public static PlayerController Instance;


    /// <summary>
    /// Direction moving of player
    /// </summary>
    public static PlayerMoveDirection Direction;

    /// <summary>
    /// Direction of speed up. When dont speed up, value is none
    /// </summary>
    public static SpeedUp SpeedUp = SpeedUp.None;

    /// <summary>
    /// State jump of player
    /// </summary>
    public static JumpState JumpState;

    public static bool IsSpeedUp = false;

    private static Vector3 _currentCheckPoint;

    public static Action AttackAction;

    /// <summary>
    /// Turn on and off debug mode
    /// </summary>
    [SerializeField] private bool _isDebugMode;

    /// <summary>
    /// This break all rule of system about lose condition of player 
    /// </summary>
    [SerializeField] private bool _isTestMode = false;

    /// <summary>
    /// Undead after touch enemy or bullet
    /// </summary>
    [SerializeField] private bool _isUndead;

    /// <summary>
    /// Check player was block 
    /// </summary>
    [SerializeField] private bool _blockInFront;

    /// <summary>
    /// Collider bound of player
    /// </summary>
    [SerializeField] private BoxCollider2D _collider;

    /// <summary>
    /// Player rigidbody2d
    /// </summary>
    [SerializeField] private Rigidbody2D _rigid;

    /// <summary>
    /// Player animation control
    /// </summary>
    [SerializeField] private SkeletonAnimation _skelention;

    /// <summary>
    /// Transform of skeleton
    /// </summary>
    [SerializeField] private Transform _skelentionTrans;

    /// <summary>
    /// Transform of player
    /// </summary>
    [SerializeField] private Transform _trans;

    /// <summary>
    /// Normal speed jump and run of player
    /// </summary>
    [SerializeField] private Vector2 _normalSpeed = new Vector2(2f, 2f);

    /// <summary>
    /// Speed jump and run of player when boot
    /// </summary>
    [SerializeField] private Vector2 _maxVelocity = new Vector2(10f, 10f);

    /// <summary>
    /// Current action of player
    /// </summary>
    [SerializeField] private PlayerAction _playerAction;

    /// <summary>
    /// Current evelopment of player
    /// </summary>
    [SerializeField] private PlayerType _currentPlayerType;

    /// <summary>
    /// Layer block player
    /// </summary>
    [SerializeField] private LayerMask _blockLayer;

    /// <summary>
    /// Bullets can fire
    /// </summary>
    [SerializeField] private Bullet[] _bullets;

    /// <summary>
    /// Base gravity scale effect to player
    /// </summary>
    [SerializeField] private float gravityScale = 3f;
    [SerializeField] private GameObject _iceBlock;

    /// <summary>
    /// Character player carry to throw
    /// </summary>
    [SerializeField] private IThrowable _throwCharacter;

    /// <summary>
    /// Platform player stand on
    /// </summary>
    private Platform _platform;

    [SerializeField] private PlayerMoveDirection _directionDrag;
    /// <summary>
    /// Old position of platform, update each frame
    /// </summary>
    private Vector3 _oldPlatformPosition;
    /// <summary>
    /// Timer count time can jump
    /// </summary>
    private float _timeJump = 100f;
    /// <summary>
    /// Timer count time drag
    /// </summary>
    private float _timeDrag = 100f;

    private float _timeJumpWhenDragWall = 100f;

    /// <summary>
    /// Flag help know player landed
    /// </summary>
    [SerializeField] private bool _canJump;

    /// <summary>
    /// Is player iced or something...
    /// </summary>
    [SerializeField] private bool _canMove = true;

    /// <summary>
    /// Maximum time can hold jump button
    /// </summary>
    [SerializeField] private float maxTimeHoldJump = .25f;
    /// <summary>
    /// Maximum time player drag after speed up
    /// </summary>
    [SerializeField] private float maxTimeDrag = .5f;

    /// <summary>
    /// Maximum time player drag after speed up
    /// </summary>
    [SerializeField] private float maxTimeJumpAfterDragWall = .1f;

    [SerializeField] private Vector2 _sizeWhenPlayerSmall;
    [SerializeField] private Vector2 _offsetPlayerWhenSmall;
    [SerializeField] private Vector2 _sizeWhenPlayerBig;
    [SerializeField] private Vector2 _offsetWhenPlayerBig;
    [SerializeField] private bool _closePipe;
    /// <summary>
    /// Minimum alpha of material of player
    /// </summary>
    private float _minAlpha = 150f / 255f;

    // Add to test game in device
    private bool _isRespawn = false;

    private List<DirectionPipe> _directionCheckList = new List<DirectionPipe>();
    #endregion

    #region Properties

    public Rigidbody2D Rigid
    {
        get
        {
            if (!_rigid)
                _rigid = GetComponent<Rigidbody2D>();

            return _rigid;
        }
    }

    public Collider2D Col
    {
        get { return _collider; }
    }


    public event Action UnfreezeAction;

    public PlayerAction PlayerAction
    {
        get
        {
            return _playerAction;
        }

        set
        {
            if (_playerAction == value)
                return;
            _playerAction = value;

            if (IsAlive)
                SetAnimation();
        }
    }

    public bool IsAlive { get; private set; }

    public bool IsFreeze { get; private set; }

    public bool CanMove
    {
        get { return _canMove && !IsFreeze; }
        private set
        {
            _canMove = value;
        }
    }

    /// <summary>
    /// Property transform of player
    /// </summary>
    public Transform Trans { get { return _trans; } }

    #endregion

    #region Function

#if UNITY_EDITOR
    public virtual void OnDrawGizmos()
    {
        if (!_isDebugMode)
            return;

        Gizmos.color = Color.blue;
        #region Raycast in front of player

        Vector2 boxSize = _collider.bounds.size;
        boxSize.x = .1f;
        boxSize.y *= 1f;

        var position = new Vector2(_skelentionTrans.localScale.x > 0 ?
                _collider.bounds.max.x :
                _collider.bounds.min.x,
            _collider.bounds.max.y - boxSize.y / 2);

        position.x += _skelentionTrans.localScale.x > 0
            ? boxSize.x / 2
            : -(boxSize.x / 2);

        Gizmos.DrawWireCube(position, boxSize);

        #endregion

        #region Raycast on top of player
        boxSize = _collider.bounds.size;
        boxSize.y *= .1f;
        boxSize.x *= .95f;

        position = new Vector2(_collider.bounds.center.x, _collider.bounds.max.y + boxSize.y / 2);

        Gizmos.DrawWireCube(position, boxSize);

        #endregion

        #region Raycast on bottom of player
        if (Rigid.velocity.y < 0)
        {
            boxSize = _collider.bounds.size;
            boxSize.y = .1f;

            position = new Vector2(_collider.bounds.center.x, _collider.bounds.min.y - boxSize.y / 2);

            Gizmos.DrawWireCube(position, boxSize);
        }

        #endregion

        #region Raycast player-self
        position = _collider.bounds.center;
        boxSize = _collider.bounds.size;
        position.y += boxSize.y * .25f / 2;
        boxSize.y *= .75f;

        Gizmos.DrawWireCube(position, boxSize);
        #endregion
    }
#endif

    void Awake()
    {
        Instance = this;
    }

    void OnEnable()
    {
        AttackAction = Attack;
    }

    void OnDisable()
    {
        AttackAction = null;
    }

    void Start()
    {
        IsAlive = false;
    }

    void FixedUpdate()
    {
        if (!IsAlive)
            if (Input.GetKeyDown(KeyCode.T))
                if (PlayerData.SpawnPlayer())
                    InitLife();

        _closePipe = false;

#if !UNITY_EDITOR
        if (!IsAlive && !_isRespawn)
        {
            Invoke("InitLife", 2f);
            _isRespawn = true;
        }
#endif

        if (!IsAlive || IsFreeze || !CanMove)
            return;
        // Create raycast to check an collision around player
        //else
        if (Rigid.velocity.y <= 0)
            RaycastBottom();

        RaycastTop();
        RaycastPlayerself();
        RaycastFront();

#if UNITY_EDITOR
        // Control by keyboard 
        if (Input.GetKey(KeyCode.A))
            Direction = PlayerMoveDirection.Left;
        else if (Input.GetKey(KeyCode.D))
            Direction = PlayerMoveDirection.Right;
        else
            Direction = PlayerMoveDirection.None;

        if (Input.GetKeyDown(KeyCode.Space))
            JumpState = JumpState.Press;
        else if (Input.GetKeyUp(KeyCode.Space))
            JumpState = JumpState.Release;
        else if (Input.GetKey(KeyCode.Space))
            JumpState = JumpState.Hold;

        if (Input.GetKeyDown(KeyCode.F) && !IsActionHasFlag(PlayerAction.Jump))
            if (Input.GetKey(KeyCode.A))
                SpeedUp = SpeedUp.Left;
            else if (Input.GetKey(KeyCode.D))
                SpeedUp = SpeedUp.Right;
            else
                SpeedUp = SpeedUp.None;


        // Make player attack
        if (Input.GetKeyDown(KeyCode.R))
            Attack();
#endif
        if (IsSpeedUp && !IsActionHasFlag(PlayerAction.SpeedUp) && !IsActionHasFlag(PlayerAction.Jump))
        {
            if (Direction == PlayerMoveDirection.Left)
                SpeedUp = SpeedUp.Left;
            else if (Direction == PlayerMoveDirection.Right)
                SpeedUp = SpeedUp.Right;
        }

        // Moving player in horizontal
        switch (Direction)
        {
            case PlayerMoveDirection.None:
                MovePlayer(0);
                break;
            case PlayerMoveDirection.Left:
                MovePlayer(-_normalSpeed.x);
                break;
            case PlayerMoveDirection.Right:
                MovePlayer(_normalSpeed.x);
                break;
            default:
                Debug.LogError("Dont set up for " + Direction);
                break;
        }

        // Make player crouch
        if (Input.GetKeyDown(KeyCode.S))
            Crouching(true);
        else if (Input.GetKeyUp(KeyCode.S))
            Crouching(false);

        // Make player jump
        switch (JumpState)
        {
            case JumpState.None:
                break;
            case JumpState.Press:
                PressJumpButton();
                HoldJumpButton();
                break;
            case JumpState.Hold:
                HoldJumpButton();
                break;
            case JumpState.Release:
                ReleaseJumpButton();
                break;
            default:
                Debug.LogError("Dont set up for " + JumpState);
                break;
        }

        if (_closePipe)
        {
            if (Direction != PlayerMoveDirection.None)
                _directionCheckList.Add(Direction == PlayerMoveDirection.Left
                    ? DirectionPipe.Right
                    : DirectionPipe.Left);
            if (Input.GetKey(KeyCode.S))
                _directionCheckList.Add(DirectionPipe.Top);
            if (Input.GetKey(KeyCode.W))
                _directionCheckList.Add(DirectionPipe.Bottom);

            CreateRaycastCheckPipe();
        }

        if (_platform)
            CheckIsInPlatform();

        ControlAcion();
    }


    void LateUpdate()
    {
        //if (_platform)
        //    if (!IsActionHasFlag(PlayerAction.Jump))
        //        CheckIsInPlatform();
        //    else
        //        _oldPlatformPosition = _platform.Trans.position;
    }

    public static void CheckPoint(Vector3 position)
    {
        _currentCheckPoint = position;
    }

    /// <summary>
    /// Moving follow platform
    /// </summary>
    private void CheckIsInPlatform()
    {
        //Check is player is on platform
        bool inAxisX = _trans.position.x < _platform.Collider.bounds.max.x &&
                       _trans.position.x > _platform.Collider.bounds.min.x;
        bool inAxisY = _trans.position.y > _platform.Collider.bounds.center.y;

        if (!(inAxisX && inAxisY))
        {
            Trans.SetParent(null);
            _platform = null;
        }
    }

    /// <summary>
    /// Limit speed of player and control action follow speed
    /// </summary>
    private void ControlAcion()
    {
        Vector2 currentVelo = Rigid.velocity;
        //currentVelo.x = Mathf.Clamp(currentVelo.x, -_maxVelocity.x, _maxVelocity.x);
        //currentVelo.y = Mathf.Clamp(currentVelo.y, -_maxVelocity.y, _maxVelocity.y);

        if (!IsActionHasFlag(PlayerAction.Climb) && !IsActionHasFlag(PlayerAction.Jump))
            if (Mathf.Abs(currentVelo.x) <= float.Epsilon)
                if (currentVelo.y < -3f)
                    PlayerAction |= PlayerAction.Jump;

        //Rigid.velocity = currentVelo;
    }

    /// <summary>
    /// Set action for crouch
    /// </summary>
    /// <param name="isCrouch"></param>
    private void Crouching(bool isCrouch)
    {
        if (!IsActionHasFlag(PlayerAction.Climb))
            if (isCrouch)
                PlayerAction |= PlayerAction.Crouch;
            else PlayerAction &= ~PlayerAction.Crouch;
    }

    #region Jump function

    /// <summary>
    /// On press jump button, start cal time can jump
    /// </summary>
    public void PressJumpButton()
    {
        if (_canJump)
        {
            Trans.SetParent(null);

            _skelentionTrans.localPosition = Vector3.zero;

            _platform = null;

            _timeJump = 0;
            _canJump = false;
            JumpState = JumpState.Hold;
        }
        else
            JumpState = JumpState.None;
    }

    /// <summary>
    /// On hold jump button, count up time can jump. 
    /// </summary>
    public void HoldJumpButton()
    {
        if (_timeJump < maxTimeHoldJump)
            Jump();
    }

    /// <summary>
    /// On release jump button, increase time hold. Player cant jump anymore 
    /// </summary>
    public void ReleaseJumpButton()
    {
        _timeJump += maxTimeHoldJump;
        JumpState = JumpState.None;
    }

    /// <summary>
    /// Take action player jump
    /// </summary>
    public void Jump()
    {
        // Change velocity in axis Y
        Vector2 currentVelocity = Rigid.velocity;
        currentVelocity.y = _normalSpeed.y;

        // Jump out when drag in wall
        if (IsActionHasFlag(PlayerAction.DragWall))
        {
            _skelentionTrans.localScale = new Vector3(-_skelentionTrans.localScale.x,
                _skelentionTrans.localScale.y, _skelentionTrans.localScale.z);
            _timeJumpWhenDragWall = 0;
        }

        Rigid.velocity = currentVelocity;

        // count up follow delta time
        _timeJump += Time.deltaTime;

        // Remove action drag when jump
        if (IsActionHasFlag(PlayerAction.Drag))
        {
            PlayerAction &= ~PlayerAction.Drag;
            SpeedUp = SpeedUp.None;
        }

        // Add action jump
        PlayerAction |= PlayerAction.Jump;

        if (_timeJump >= maxTimeHoldJump)
        {
            JumpState = JumpState.None;
            _canJump = false;
        }
    }

    /// <summary>
    /// Take a small jump when jump on enemy
    /// </summary>
    private void SmallJump()
    {
        Vector2 velo = Rigid.velocity;
        velo.y = _normalSpeed.y;
        Rigid.velocity = velo;
    }

    #endregion

    /// <summary>
    /// Take action move player by <see cref="Rigidbody2D"/>
    /// </summary>
    /// <param name="speed">speed of player</param>
    private void MovePlayer(float speed)
    {
        if (!_canMove)
            return;

        if (_timeJumpWhenDragWall > maxTimeJumpAfterDragWall)
            if (!IsActionHasFlag(PlayerAction.Drag))
            {
                float currentScale = _skelentionTrans.localScale.x;
                bool isFlip = speed < Mathf.Epsilon ^ currentScale < 0;

                // Hanling speed for player
                if ((int)SpeedUp == (int)Direction ||
                    (IsActionHasFlag(PlayerAction.Jump) && SpeedUp != SpeedUp.None))
                {
                    speed *= 1.5f;

                    // Set action speed up
                    if ((int)SpeedUp == (int)Direction &&
                        SpeedUp != SpeedUp.None &&
                        !IsActionHasFlag(PlayerAction.SpeedUp))
                        PlayerAction |= PlayerAction.SpeedUp;

                    // Set drag direction 
                    if (Direction != PlayerMoveDirection.None)
                        _directionDrag = Direction;
                }
                else
                    // Direction of player is different with speed up
                    if (SpeedUp != SpeedUp.None)
                {
                    PlayerAction &= ~PlayerAction.SpeedUp;
                    PlayerAction |= PlayerAction.Drag;
                    _timeDrag = 0;
                }

                // Handling action state for player, flip if needed
                if (Mathf.Abs(speed) <= Mathf.Epsilon)
                    PlayerAction &= ~PlayerAction.Run;
                else
                {
                    // Flip player 
                    if (isFlip && !IsActionHasFlag(PlayerAction.Drag))
                        _skelentionTrans.localScale = new Vector3(-currentScale, _skelentionTrans.localScale.y,
                            _skelentionTrans.localScale.z);
                    PlayerAction |= PlayerAction.Run;

                    if (_blockInFront)
                        PlayerAction &= ~PlayerAction.Run;
                }

                // Handling velocity for character
                if (IsActionHasFlag(PlayerAction.DragWall))
                {
                    // Drag on the wall
                    Vector2 currentVelocity = Rigid.velocity;
                    currentVelocity.x = 0;
                    currentVelocity.y = -1f;
                    Rigid.velocity = currentVelocity;
                }
                // Change velocity in axis X for player
                else if (!_blockInFront || isFlip)
                {
                    Vector2 currentVelocity = Rigid.velocity;
                    currentVelocity.x = speed;
                    Rigid.velocity = currentVelocity;
                }

            }
            else
            {
                // is player drag after speed up
                if (_timeDrag <= maxTimeDrag)
                {
                    float multiple = 1 - _timeDrag / maxTimeDrag;
                    //Debug.Log(_timeDrag);
                    PlayerAction |= PlayerAction.Drag;
                    float dragSpeed = _directionDrag == PlayerMoveDirection.Left ? -_normalSpeed.x * multiple : _normalSpeed.x * multiple;
                    Vector2 currentVelocity = Rigid.velocity;
                    currentVelocity.x = dragSpeed;
                    Rigid.velocity = currentVelocity;
                }
                else
                {
                    SpeedUp = SpeedUp.None;
                    PlayerAction &= ~PlayerAction.Drag;
                }
            }
        else
        {
            bool isRight = _skelentionTrans.localScale.x > 0;

            Vector2 currentVelocity = Rigid.velocity;
            float multiple = 1 - _timeJumpWhenDragWall / maxTimeJumpAfterDragWall;
            currentVelocity.x = isRight ? _normalSpeed.x * multiple : -_normalSpeed.x * multiple;
            currentVelocity.y = _normalSpeed.y / 2;
            Rigid.velocity = currentVelocity;
        }

        if (_timeJumpWhenDragWall <= maxTimeJumpAfterDragWall)
            _timeJumpWhenDragWall += Time.deltaTime;
        _timeDrag += Time.deltaTime;
    }

    /// <summary>
    /// Player take action climb
    /// </summary>
    private void Climb(float speed)
    {
        Rigid.velocity = new Vector2(0, speed);

        if (!IsActionHasFlag(PlayerAction.Climb))
        {
            Rigid.gravityScale = 0f;
            PlayerAction = PlayerAction.Climb | (PlayerAction & PlayerAction.Jump) |
                           (PlayerAction & PlayerAction.Carry);
        }
        else if (Mathf.Abs(speed) <= float.Epsilon ^ !_skelention.loop)
            SetAnimation();

        if (!_canJump)
            _canJump = true;
    }

    /// <summary>
    /// Change animation for player after set action.
    /// </summary>
    public void SetAnimation()
    {
        if (!IsAlive)
            return;

        bool isLoop = true;
        string animationName = PlayerAnimationName.Idle;
        if (IsActionHasFlag(PlayerAction.Climb))
        {
            isLoop = Mathf.Abs(Rigid.velocity.y) >= float.Epsilon;
            animationName = PlayerAnimationName.Climb;
        }
        else if (IsActionHasFlag(PlayerAction.Jump))
        {
            isLoop = false;
            animationName = IsActionHasFlag(PlayerAction.Carry) ?
                PlayerAnimationName.CarryJump :
                IsActionHasFlag(PlayerAction.DragWall) ?
                    PlayerAnimationName.DragWall :
                    PlayerAnimationName.Jump;
        }
        else
        {
            if (IsActionHasFlag(PlayerAction.Carry))
                if (IsActionHasFlag(PlayerAction.Run))
                    animationName = IsActionHasFlag(PlayerAction.SpeedUp) ?
                        PlayerAnimationName.CarryFastRun :
                        PlayerAnimationName.CarryRun;
                else
                    animationName = PlayerAnimationName.IdleCarry;
            else if (IsActionHasFlag(PlayerAction.Run))
                if (IsActionHasFlag(PlayerAction.Drag))
                {
                    isLoop = false;
                    animationName = PlayerAnimationName.DragSpeedUp;
                }
                else
                    animationName = IsActionHasFlag(PlayerAction.SpeedUp) ?
                          PlayerAnimationName.FastRun :
                          PlayerAnimationName.Run;
            else if (IsActionHasFlag(PlayerAction.Crouch))
                animationName = IsActionHasFlag(PlayerAction.Run) ? PlayerAnimationName.CrouchRun : PlayerAnimationName.PrepareCrouch;
            else if (IsActionHasFlag(PlayerAction.Drag))
            {
                isLoop = false;
                animationName = PlayerAnimationName.DragSpeedUp;
            }
        }

        if (_skelention.loop != isLoop && _skelention.AnimationName == animationName)
            _skelention.AnimationName = PlayerAnimationName.Idle;
        _skelention.loop = isLoop;
        _skelention.AnimationName = animationName;
    }

    /// <summary>
    /// Compare two enum action of player
    /// </summary>
    /// <param name="action">Action need to find</param>
    private bool IsActionHasFlag(PlayerAction action)
    {
        return (_playerAction & action) == action;
    }

    /// <summary>
    /// Compare two enum compareType of player
    /// </summary>
    /// <param name="action">Type need to find</param>
    private bool IsTypeHasFlag(PlayerType compareType)
    {
        return (_currentPlayerType & compareType) == compareType;
    }

    #region Get item

    /// <summary>
    /// Change state when get a item
    /// </summary>
    /// <param name="typeGroupTo"></param>
    public void GrownTo(PlayerType typeGroupTo)
    {
        if (IsTypeHasFlag(typeGroupTo))
            return;

        if (_currentPlayerType == PlayerType.None)
            _currentPlayerType = PlayerType.Small;

        ChangeCollider(typeGroupTo);

        if (typeGroupTo == PlayerType.Super)
            _currentPlayerType |= PlayerType.Super;
        else if (_currentPlayerType <= PlayerType.Grown
                 || (int)typeGroupTo > (int)PlayerType.Grown)
            if ((_currentPlayerType & PlayerType.Super) == PlayerType.Super)
                _currentPlayerType = typeGroupTo | PlayerType.Super;
            else
                _currentPlayerType = typeGroupTo;

        ChangeSkin();
    }

    /// <summary>
    /// Change skelecton data asset
    /// </summary>
    private void ChangeSkin()
    {
        SkeletonDataAsset newSkin = GameData.Instance.Player.Small;

        if (IsTypeHasFlag(PlayerType.Small))
            newSkin = GameData.Instance.Player.Small;
        else if (IsTypeHasFlag(PlayerType.Grown))
            newSkin = GameData.Instance.Player.Grown;
        else if (IsTypeHasFlag(PlayerType.Fire))
            newSkin = GameData.Instance.Player.Fire;
        else if (IsTypeHasFlag(PlayerType.Ice))
            newSkin = GameData.Instance.Player.Ice;

        _skelention.skeletonDataAsset = newSkin;
        _skelention.Initialize(true);
    }

    /// <summary>
    /// Change collider when player become big or small
    /// </summary>
    private void ChangeCollider(PlayerType newType)
    {
        if (newType == PlayerType.Super)
            return;

        if (newType == PlayerType.Small)
        {
            _collider.size = _sizeWhenPlayerSmall;
            _collider.offset = _offsetPlayerWhenSmall;
        }
        else if (IsTypeHasFlag(PlayerType.Small))
        {
            _collider.size = _sizeWhenPlayerBig;
            _collider.offset = _offsetWhenPlayerBig;
        }
    }

    /// <summary>
    /// Fade animation 
    /// </summary>
    /// <param name="smooth">Delta time and delta fade</param>
    /// <returns>Second waint</returns>
    private IEnumerator AnimationPlayer(float smooth)
    {
        bool isUp = false;
        WaitForSeconds w = new WaitForSeconds(smooth);
        float coundDown = 0;
        float speedFade = smooth;
        while (true)
        {
            coundDown += smooth;
            Color c = Color.white;
            foreach (Material material in GameData.Instance.Player.SkeletonMaterials)
            {
                c = material.color;
                c.a += isUp ? speedFade : -speedFade;
                material.color = c;
            }

            if (c.a <= _minAlpha)
                isUp = true;
            else if (c.a >= 1f)
                isUp = false;

            if (coundDown >= 2f)
            {
                foreach (Material material in GameData.Instance.Player.SkeletonMaterials)
                    material.color = Color.white;

                _isUndead = false;
                break;
            }
            yield return w;
        }
    }

    #endregion

    #region Raycast

    /// <summary>
    /// Create raycast to check is stand near pipe 
    /// </summary>
    private void CreateRaycastCheckPipe()
    {
        foreach (var direction in _directionCheckList)
        {
            Vector2 startpoint = Col.bounds.center;
            Vector2 size = Col.bounds.size * .8f;

            // Find direction of player
            switch (direction)
            {
                case DirectionPipe.Left:
                    startpoint.x = Col.bounds.max.x;
                    break;
                case DirectionPipe.Right:
                    startpoint.x = Col.bounds.min.x;
                    break;
                case DirectionPipe.Top:
                    startpoint.y = Col.bounds.min.y;
                    break;
                case DirectionPipe.Bottom:
                    startpoint.y = Col.bounds.max.y;
                    break;
            }

            // is direction of player have pipe to move
            RaycastHit2D[] hits = Physics2D.BoxCastAll(startpoint, size, 0, Vector2.zero);
            foreach (var d in hits)
                if (d.transform.CompareTag(ObjectTag.Pipe))
                {
                    var p = d.transform.GetComponent<Pipe>();

                    // Pipe can go in and fix with diretion of player
                    if (p.Direction == direction && p.IsPipeIn)
                    {
                        var pipieOut = PipeController.FindCouple(p.Index);
                        if (pipieOut != null)
                            MoveByPipe(p, pipieOut);
                    }
                }

        }

        _directionCheckList.Clear();
    }

    /// <summary>
    /// Create raycast in top size
    /// </summary>
    private void RaycastTop()
    {
        Vector2 boxSize = _collider.bounds.size;
        boxSize.y *= .1f;
        boxSize.x *= .95f;
        Vector2 position = new Vector2(_collider.bounds.center.x, _collider.bounds.max.y + boxSize.y / 2);

        RaycastHit2D[] hit = Physics2D.BoxCastAll(position, boxSize, 0, Vector2.zero);

        foreach (var obj in hit)
            if (obj.transform.CompareTag(ObjectTag.TileBlock) ||
                obj.transform.CompareTag(ObjectTag.Pipe) ||
                obj.transform.CompareTag(ObjectTag.Ground))
            {
                var speed = Rigid.velocity;
                speed.y = -1f;
                Rigid.velocity = speed;

                var goreObject = obj.transform.GetComponent<IGoreable>();
                if (goreObject != null)
                    goreObject.Gore();

                ReleaseJumpButton();
                if (obj.transform.CompareTag(ObjectTag.Pipe))
                    _closePipe = true;
            }

    }

    /// <summary>
    /// Create raycast in bottom size
    /// </summary>
    private void RaycastBottom()
    {
        Vector2 boxSize = _collider.bounds.size;
        boxSize.y = .1f;
        Vector2 position = new Vector2(_collider.bounds.center.x, _collider.bounds.min.y - boxSize.y / 2);

        RaycastHit2D[] hit = Physics2D.BoxCastAll(position, boxSize, 0, Vector2.zero);

        bool isHaveGround = false;
        foreach (var obj in hit)
        {
            if (obj.transform.CompareTag(ObjectTag.Ground) ||
                obj.transform.CompareTag(ObjectTag.TileBlock) ||
                obj.transform.CompareTag(ObjectTag.Pipe))
            {
                var actionRemove = PlayerAction.Jump | PlayerAction.Climb;
                PlayerAction &= ~actionRemove;
                isHaveGround = true;
                Rigid.gravityScale = gravityScale;

                _skelentionTrans.localPosition = Vector3.zero;

                if (obj.transform.CompareTag(ObjectTag.Pipe))
                    _closePipe = true;
            }
            else if (obj.transform.CompareTag(ObjectTag.Boss))
            {
                Debug.Log("error here");
                IAttackedable enemy = obj.transform.GetComponent<IAttackedable>();
                if (enemy != null)
                {
                    SmallJump();
                    enemy.Attacked(AttackType.Jump, this);
                }
            }
            else if (obj.transform.CompareTag(ObjectTag.Enemy) && IsActionHasFlag(PlayerAction.Jump))
            {
                IAttackedable enemy = obj.transform.GetComponent<IAttackedable>();
                if (enemy != null && ((ICharacter)enemy).IsAlive && !((ICharacter)enemy).IsFreeze)
                {
                    SmallJump();
                    enemy.Attacked(AttackType.Jump, this);
                }
            }
            else if (obj.transform.CompareTag(ObjectTag.Platform))
            {
                isHaveGround = true;

                _skelentionTrans.localPosition = Vector3.zero;
                var actionRemove = PlayerAction.Jump | PlayerAction.Climb;
                PlayerAction &= ~actionRemove;

                if (_platform)
                    continue;

                Rigid.gravityScale = gravityScale;

                _platform = obj.transform.GetComponent<Platform>();
                Trans.SetParent(obj.transform);
            }
        }

        _canJump = isHaveGround;
    }

    /// <summary>
    /// Create raycast in player-self
    /// </summary>
    private void RaycastPlayerself()
    {
        Vector2 position = _collider.bounds.center;
        Vector2 sizeBoxCast = _collider.bounds.size;

        position.y += sizeBoxCast.y * .25f / 2;
        sizeBoxCast.y *= .75f;
        RaycastHit2D[] hit = Physics2D.BoxCastAll(position, sizeBoxCast, 0, Vector2.zero);
        if (hit.Length > 1)
            foreach (var obj in hit)
            {
                if (obj.transform.transform.CompareTag(ObjectTag.Player))
                    continue;

                if (obj.transform.CompareTag(ObjectTag.Item))
                {
                    IGetable itemGet = obj.transform.GetComponent<IGetable>();
                    itemGet.EffectGetItem(this);
                }
                else if (obj.transform.CompareTag(ObjectTag.Enemy) && !_isUndead)
                {
                    IShield shield = obj.transform.GetComponent<IShield>();
                    IAttackedable enemy = obj.transform.GetComponent<IAttackedable>();
                    if (shield != null && !shield.IsShield)
                    {
                        if (!shield.IsRole)
                        {
                            enemy.Attacked(AttackType.Jump, this);
                            SmallJump();
                        }
                    }
                    else
                        if (enemy != null && ((ICharacter)enemy).IsAlive && !((ICharacter)enemy).IsFreeze)
                        PlayerWasHurt(enemy);
                }
                else if (obj.transform.CompareTag(ObjectTag.Vine))
                {
                    if (Input.GetKey(KeyCode.W))
                        Climb(_normalSpeed.y);
                    else if (Input.GetKey(KeyCode.S))
                        Climb(-_normalSpeed.y);
                    else if (IsActionHasFlag(PlayerAction.Climb))
                        Climb(0);
                }
                else if (obj.transform.CompareTag(ObjectTag.Coin))
                {
                    obj.collider.enabled = false;
                    obj.transform.DOMoveY(2.5f, .3f).SetRelative();
                    DestroyObject(obj.transform.gameObject, .3f);
                    PlayerData.GetCoin();
                }
                else if (obj.transform.CompareTag(ObjectTag.EndPoint))
                {
                    Rigid.velocity = Vector2.zero;
                    Rigid.isKinematic = true;
                    CanMove = false;
                    GetEndFlag(obj.transform.position.y, Vector3.right * 10);
                }
                else if (obj.transform.CompareTag(ObjectTag.DeadEnd))
                    StartCoroutine(Dead());
                else if (obj.transform.CompareTag(ObjectTag.PigBullet))
                    PlayerWasHurt();
            }
        else if (IsActionHasFlag(PlayerAction.Climb))
        {
            // When raycast just hit player
            PlayerAction &= ~PlayerAction.Climb;
            PlayerAction |= PlayerAction.Jump;
            Rigid.gravityScale = gravityScale;
        }
    }


    /// <summary>
    /// Create raycast in front of player
    /// </summary>
    private void RaycastFront()
    {
        Vector2 boxSize = _collider.bounds.size;
        boxSize.x = .1f;
        boxSize.y *= 1f;
        Vector2 position = new Vector2(_skelentionTrans.localScale.x > 0 ?
                _collider.bounds.max.x :
                _collider.bounds.min.x,
            _collider.bounds.max.y - boxSize.y / 2);

        position.x += _skelentionTrans.localScale.x > 0 ? boxSize.x / 2 : -(boxSize.x / 2);

        RaycastHit2D[] hit = Physics2D.BoxCastAll(position, boxSize, 0f, Vector2.zero, .1f, _blockLayer);

        _blockInFront = hit.Length > 0;

        foreach (var d in hit)
            if (d.transform.CompareTag(ObjectTag.Pipe))
                _closePipe = true;

        if (_blockInFront && IsActionHasFlag(PlayerAction.Jump) && Rigid.velocity.y < 0)
        {
            _canJump = true;
            PlayerAction |= PlayerAction.DragWall;

            float currentDirection = _skelentionTrans.localScale.x;

            float diff = currentDirection <= 0 ?
                -Col.bounds.size.x / 2 / Trans.localScale.x - .05f :
                Col.bounds.size.x / 2 / Trans.localScale.x + .05f;

            _skelentionTrans.localPosition = new Vector3(diff, 0, 0);

            PlayerAction &= ~PlayerAction.Drag;
            PlayerAction &= ~PlayerAction.SpeedUp;
            SpeedUp = SpeedUp.None;
        }
        else
        {
            PlayerAction &= ~PlayerAction.DragWall;
        }
    }

    /// <summary>
    /// Player dead
    /// </summary>
    private IEnumerator Dead()
    {
        _skelention.loop = false;
        _skelention.AnimationName = PlayerAnimationName.Dead;
        _canMove = false;
        _isUndead = true;
        _currentPlayerType = PlayerType.None;
        IsAlive = false;
        ProCamera2D.Instance.RemoveAllCameraTargets();
        yield return new WaitForSeconds(1f);
    }

    /// <summary>
    /// Player was hurt by enemy or bullet
    /// </summary>
    private void PlayerWasHurt(IAttackedable enemy = null)
    {
        if (_isUndead)
            return;

        _isUndead = true;
        if (IsTypeHasFlag(PlayerType.Super))
        {
            if (enemy != null)
                enemy.Attacked(AttackType.Dead);
        }
        else if (IsTypeHasFlag(PlayerType.Grown))
        {
            _currentPlayerType = PlayerType.Small;
            StartCoroutine(AnimationPlayer(.1f));
            Vector2 velocityPushBack = new Vector2(_skelentionTrans.rotation.x < 0 ? 5f : -5f, 10);
            _rigid.velocity = velocityPushBack;
        }
        else if (IsTypeHasFlag(PlayerType.Ice) || IsTypeHasFlag(PlayerType.Fire) || IsTypeHasFlag(PlayerType.Winged))
        {
            _currentPlayerType = PlayerType.Grown;
            StartCoroutine(AnimationPlayer(.1f));
            Vector2 velocityPushBack = new Vector2(_skelentionTrans.rotation.x < 0 ? 5f : -5f, 10);
            _rigid.velocity = velocityPushBack;
        }
        else if (IsTypeHasFlag(PlayerType.Small))
        {
            StartCoroutine(Dead());
        }

        ChangeSkin();
        ChangeCollider(_currentPlayerType);
    }

    /// <summary>
    /// Hit the end flag
    /// </summary>
    /// <param name="flagstaffEnd">position end of flag</param>
    /// <param name="endPosition">position end map</param>
    private void GetEndFlag(float flagstaffEnd, Vector3 endPosition)
    {
        Sequence endPath = DOTween.Sequence();
        float positionSlide = flagstaffEnd - 1.28f / 2 - _trans.position.y;

        // Change anamtion to climb
        _skelention.loop = false;
        _skelention.AnimationName = PlayerAnimationName.Climb;

        // path when player hit the flag. It slided player down
        endPath.Append(_trans.DOMoveY(positionSlide, 1f).SetSpeedBased(true)
            .SetRelative().SetEase(Ease.Linear).SetUpdate(UpdateType.Fixed).OnComplete(() =>
            {
                _skelention.loop = true;
                _skelention.AnimationName = PlayerAnimationName.Walk;
            }));

        // Path player run to the catlse
        endPath.Append(_trans.DOMove(endPosition, 5f).SetSpeedBased(true)
            .SetRelative().SetEase(Ease.Linear).SetUpdate(UpdateType.Fixed).OnComplete(InitLife));
    }

    /// <summary>
    /// Move allow pipe to other world
    /// </summary>
    /// <param name="pipeIn"><see cref="PipeIn"/> </param>
    /// <param name="pipeOut"><see cref="PipeOut"/> </param>
    private void MoveByPipe(Pipe pipeIn, Pipe pipeOut)
    {
        CanMove = false;
        Rigid.velocity = Vector2.zero;
        Sequence pathMove = DOTween.Sequence();

        // Change animation to wall
        _skelention.loop = true;
        _skelention.AnimationName = PlayerAnimationName.Walk;
        Col.isTrigger = true;
        Rigid.isKinematic = true;
        var positionIn = Vector2.zero;
        var positionOut = Vector2.zero;
        var positionStart = pipeIn.transform.position;
        var positionPipeTo = pipeOut.transform.position;

        // Set up position player go in
        switch (pipeIn.Direction)
        {
            case DirectionPipe.Bottom:
                positionIn.y = 1.28f / 2 * 3;
                positionStart.x -= 1.28f / 2;
                positionStart.y += 1.28f / 2;
                break;
            case DirectionPipe.Top:
                positionIn.y -= 1.28f / 2 * 3;
                positionStart.x += 1.28f / 2;
                positionStart.y = _trans.position.y;
                break;
            case DirectionPipe.Left:
                positionIn.x = 1.28f / 2 * 3;
                positionStart.x -= 1.28f;
                positionStart.y -= 1.28f / 2;
                break;
            case DirectionPipe.Right:
                positionIn.x -= 1.28f / 2 * 3;
                positionStart.x += 1.28f;
                positionStart.y -= 1.28f / 2 * 3;
                break;
        }

        // Set up position player go out 
        switch (pipeOut.Direction)
        {
            case DirectionPipe.Bottom:
                positionOut.y = -1.28f / 2 * 3;
                positionPipeTo.x -= 1.28f / 2;
                break;
            case DirectionPipe.Top:
                positionOut.y = +1.28f / 2;
                positionPipeTo.x += 1.28f / 2;
                break;
            case DirectionPipe.Left:
                positionOut.x = -1.28f;
                positionPipeTo.y -= 1.28f / 2;
                break;
            case DirectionPipe.Right:
                positionOut.x = 1.28f;
                positionPipeTo.y -= 1.28f / 2 * 3;
                break;
        }

        _trans.position = positionStart;
        pathMove.Append(_trans.DOMove(positionIn, 1f).SetSpeedBased(true)
            .SetRelative().SetEase(Ease.Linear).SetUpdate(UpdateType.Fixed).OnComplete(() =>
            {
                _skelention.loop = true;
                _skelention.AnimationName = PlayerAnimationName.Walk;
                _trans.position = positionPipeTo;
                CanMove = false;

                // Flip player to correct with direction of out
                float scaleX = pipeOut.Direction == DirectionPipe.Left ? -1f : 1f;
                _skelentionTrans.localScale = new Vector3(scaleX, _skelentionTrans.localScale.y, _skelentionTrans.localScale.z);
            }));


        pathMove.Append(_trans.DOMove(positionOut, 1f).SetSpeedBased(true)
            .SetRelative().SetEase(Ease.Linear).SetUpdate(UpdateType.Fixed).OnComplete(() =>
            {
                // Back player to control by gamer
                Rigid.isKinematic = false;
                CanMove = true;
                _playerAction = PlayerAction.Idle;
                Col.isTrigger = false;

                _skelention.loop = true;
                _skelention.AnimationName = PlayerAnimationName.Idle;
            }));

    }

    #endregion

    #region attack and attacked

    public void Attack()
    {
        if (!IsActionHasFlag(PlayerAction.Carry))
            // block in front of player, cant fire normal
            if (!_blockInFront)
                switch (_currentPlayerType)
                {
                    case PlayerType.Ice:
                    case PlayerType.Fire:
                        foreach (var b in _bullets)
                            if (!b.IsActive)
                            {
                                AttackType at = _currentPlayerType == PlayerType.Fire ? AttackType.Fire :
                                    _currentPlayerType == PlayerType.Ice ? AttackType.Ice :
                                    AttackType.Hit;

                                bool isRight = _skelentionTrans.localScale.x > 0;
                                b.FireBullet(at, isRight, _collider.bounds.center);
                                break;
                            }
                        break;
                    case PlayerType.Winged:
                        Debug.Log("Dont know this case " + PlayerType.Winged);
                        break;
                }
            else
            {
                // Find freezed enemy to throw
                Vector2 boxSize = _collider.bounds.size;
                boxSize.x = .1f;
                boxSize.y *= .95f;
                Vector2 position = new Vector2(_skelentionTrans.localScale.x > 0 ? _collider.bounds.max.x : _collider.bounds.min.x, _collider.bounds.max.y - boxSize.y / 2);

                position.x += _skelentionTrans.localScale.x > 0 ? boxSize.x / 2 : -(boxSize.x / 2);

                RaycastHit2D[] hit = Physics2D.BoxCastAll(position, boxSize, 0f, Vector2.zero, .1f, _blockLayer);
                foreach (var obj in hit)
                {
                    ICharacter c = obj.transform.GetComponent<ICharacter>();
                    if (c != null && c.IsFreeze)
                        CarryObject(c);
                }
            }
        else
        {
            ThrowObject();
        }

    }

    #region Throw object

    private void ThrowObject()
    {
        _skelention.loop = false;
        _skelention.AnimationName = PlayerAnimationName.Throw;

        Vector2 direction = new Vector2(15f, 5f);

        direction.x = _skelentionTrans.localScale.x < 0 ? -direction.x : direction.x;

        _throwCharacter.Throw(direction);
        _playerAction &= ~PlayerAction.Carry;
        ((EnemyBehavior)_throwCharacter).Trans.SetParent(null);
    }

    private void CarryObject(ICharacter character)
    {
        // Change state of object
        _throwCharacter = character as IThrowable;
        character.Rigid.bodyType = RigidbodyType2D.Kinematic;
        character.Trans.SetParent(_trans);
        character.Col.enabled = false;
        Vector2 positionIce = new Vector2(0, _collider.bounds.size.y / 2);
        character.Trans.localPosition = positionIce;
        character.UnfreezeAction += CharacterCarryUnfreezeAction;

        //Change state of player
        _skelention.loop = false;
        _skelention.AnimationName = PlayerAnimationName.PrepareCarry;
        _playerAction |= PlayerAction.Carry;
    }

    private void CharacterCarryUnfreezeAction()
    {
        _throwCharacter = null;
        _playerAction &= ~PlayerAction.Carry;
        _skelention.loop = true;
        _skelention.AnimationName = PlayerAnimationName.Idle;
    }

    public void Attacked(AttackType type, IAttackedable attacker = null)
    {
        if (IsAlive && !_isUndead)
            switch (type)
            {
                case AttackType.Hit:
                case AttackType.Reflect:
                case AttackType.Fire:
                    PlayerWasHurt(attacker);
                    break;
                case AttackType.Ice:
                    Debug.Log("Player was freeze");
                    Freeze();
                    break;
                default:
                    Debug.LogWarning("dont have this case " + type);
                    break;
            }
        else Debug.Log("Player is undead or dead");
    }
    #endregion

    private void Freeze()
    {
        IsFreeze = true;
        _iceBlock.SetActive(true);
        Invoke("UnFreeze", 3f);
    }

    private void UnFreeze()
    {
        _iceBlock.SetActive(false);
        IsFreeze = false;
    }

    #endregion

    /// <summary>
    /// Init new Life for player
    /// </summary>
    public void InitLife()
    {
        // Add player become camera target
        // be sure just one target in camera
        ProCamera2D.Instance.RemoveAllCameraTargets();
        Camera.main.transform.position = new Vector3(
            _currentCheckPoint.x, _currentCheckPoint.y, Camera.main.transform.position.z);

        Trans.position = _currentCheckPoint;
        ProCamera2D.Instance.AddCameraTarget(_trans);

        // Add action and state for player
        _currentPlayerType = PlayerType.Small;
        _playerAction = PlayerAction.Idle;

        // Reset state for player
        _skelention.loop = true;
        Rigid.isKinematic = false;
        IsAlive = true;
        _isUndead = true;
        _canJump = true;
        _canMove = true;
        IsFreeze = false;

        // reskin and change collider
        ChangeSkin();
        ChangeCollider(_currentPlayerType);
        _isRespawn = false;

        if (_throwCharacter != null)
        {
            ((EnemyBehavior)_throwCharacter).Destroy();
            ((EnemyBehavior)_throwCharacter).Trans.SetParent(null);
        }
        gameObject.SetActive(true);
        StartCoroutine(AnimationPlayer(.1f));
    }

    #endregion
}