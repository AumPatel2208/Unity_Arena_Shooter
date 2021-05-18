/*
 * Currently being Edited by Aum Patel (28th April 2021)
 *
 * Source : https://github.com/WiggleWizard/quake3-movement-unity3d/blob/master/CPMPlayer.cs
 * - Edited by PrzemyslawNowaczyk (11.10.17)
 *   -----------------------------
 *   Deleting unused variables
 *   Changing obsolete methods
 *   Changing used input methods for consistency
 *   -----------------------------
 *
 * - Edited by NovaSurfer (31.01.17).
 *   -----------------------------
 *   Rewriting from JS to C#
 *   Deleting "Spawn" and "Explode" methods, deleting unused varibles
 *   -----------------------------
 * Just some side notes here.
 *
 * - Should keep in mind that idTech's cartisian plane is different to Unity's:
 *    Z axis in idTech is "up/down" but in Unity Z is the local equivalent to
 *    "forward/backward" and Y in Unity is considered "up/down".
 *
 * - Code's mostly ported on a 1 to 1 basis, so some naming convensions are a
 *   bit fucked up right now.
 *
 * - UPS is measured in Unity units, the idTech units DO NOT scale right now.
 *
 * - Default values are accurate and emulates Quake 3's feel with CPM(A) physics.
 *
 */

using UnityEngine;

// Contains the command the user wishes upon the character
struct Cmd {
    public float forwardMove;
    public float rightMove;
    public float upMove;
}

public class CPMPlayer : MonoBehaviour {
    // Camera 
    public Transform playerView;

    public float playerViewYOffset = 0.6f; // The height at which the camera is bound to

    // mouse sensitivity
    public float xMouseSensitivity = 30.0f;
    public float yMouseSensitivity = 30.0f;


    /*Frame occuring factors*/
    public float gravity = 20.0f;
    public float friction = 6; //Ground friction

    /* Movement stuff */
    public float moveSpeed = 7.0f;  // Ground move speed
    public float runAcceleration = 14.0f; // Ground accel
    public float runDeceleration = 10.0f; // Deceleration that occurs when running on the ground
    public float airAcceleration = 2.0f;  // Air acceleration
    public float airDeceleration = 2.0f;  // Deceleration experienced when opposite strafing
    public float airControl = 0.3f;  // How precise air control is
    public float sideStrafeAcceleration = 50.0f; // How fast acceleration occurs to get up to sideStrafeSpeed when
    public float sideStrafeSpeed = 1.0f;  // What the max speed to generate when side strafing
    public float jumpSpeed = 8.0f;  // The speed at which the character's up axis gains when hitting jump
    public bool holdJumpToBhop = false; // When enabled allows player to just hold jump button to keep on bhopping perfectly. Beware: smells like casual.

    /*print() style */
    public GUIStyle style;

    /*FPS Stuff */
    public float fpsDisplayRate = 4.0f; // 4 updates per sec
    private int _frameCount = 0;
    private float _dt = 0.0f;
    private float _fps = 0.0f;

    // Character controller for the player
    private CharacterController _controller;

    // Camera rotations
    private float _cameraRotationX = 0.0f;
    private float _cameraRotationY = 0.0f;

    private Vector3 _moveDirectionNorm = Vector3.zero;
    private Vector3 _playerVelocity = Vector3.zero;
    private float _playerTopVelocity = 0.0f;

    // Q3: players can queue the next jump just before he hits the ground
    private bool _wishJump = false;

    // Used to display real time friction values
    private float _playerFriction = 0.0f;

    // Player commands, stores wish commands that the player asks for (Forward, back, jump, etc)
    private Cmd _cmd;

    private void Start() {
        // Hide the cursor
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        if (playerView == null) {
            // Maybe should not use Camera.main, as it does a GameObject.FindByTag("MainCamera")
            Camera mainCamera = Camera.main;
            if (mainCamera != null)
                playerView = mainCamera.gameObject.transform;
        }

        // Put the camera inside the capsule collider
        var position = transform.position;
        playerView.position = new Vector3(
            position.x,
            position.y + playerViewYOffset,
            position.z);

        _controller = GetComponent<CharacterController>();
    }

    private void Update() {
        // Do FPS calculation
        _frameCount++;
        _dt += Time.deltaTime;
        if (_dt > 1.0 / fpsDisplayRate) {
            _fps = Mathf.Round(_frameCount / _dt);
            _frameCount = 0;
            _dt -= 1.0f / fpsDisplayRate;
        }

        /* Ensure that the cursor is locked into the screen */
        if (Cursor.lockState != CursorLockMode.Locked) {
            if (Input.GetButtonDown("Fire1"))
                Cursor.lockState = CursorLockMode.Locked;
        }

        /* Camera rotation stuff, mouse controls this shit */
        _cameraRotationX -= Input.GetAxisRaw("Mouse Y") * xMouseSensitivity * 0.02f;
        _cameraRotationY += Input.GetAxisRaw("Mouse X") * yMouseSensitivity * 0.02f;

        // Clamp the X rotation
        if (_cameraRotationX < -90)
            _cameraRotationX = -90;
        else if (_cameraRotationX > 90)
            _cameraRotationX = 90;

        this.transform.rotation = Quaternion.Euler(0, _cameraRotationY, 0);            // Rotates the collider
        playerView.rotation = Quaternion.Euler(_cameraRotationX, _cameraRotationY, 0); // Rotates the camera


        /* Movement, here's the important part */
        QueueJump();
        if (_controller.isGrounded)
            GroundMove();
        else if (!_controller.isGrounded)
            AirMove();

        // Move the controller
        _controller.Move(_playerVelocity * Time.deltaTime);

        /* Calculate top velocity */
        Vector3 udp = _playerVelocity;
        udp.y = 0.0f;
        if (udp.magnitude > _playerTopVelocity)
            _playerTopVelocity = udp.magnitude;

        // Need to move the camera after the player has been moved because otherwise the camera will clip the player if going fast enough and will always be 1 frame behind.
        // Set the camera's position to the transform
        playerView.position = new Vector3(
            transform.position.x,
            transform.position.y + playerViewYOffset,
            transform.position.z);
    }

    /*******************************************************************************************************\
   |* MOVEMENT
   \*******************************************************************************************************/

    /**
     * Sets the movement direction based on player input
     */
    private void SetMovementDir() {
        _cmd.forwardMove = Input.GetAxisRaw("Vertical");
        _cmd.rightMove = Input.GetAxisRaw("Horizontal");
    }

    /**
     * Queues the next jump just like in Q3
     */
    private void QueueJump() {
        if (holdJumpToBhop) {
            _wishJump = Input.GetButton("Jump");
            return;
        }

        if (Input.GetButtonDown("Jump") && !_wishJump)
            _wishJump = true;
        if (Input.GetButtonUp("Jump"))
            _wishJump = false;
    }

    /**
     * Execs when the player is in the air
    */
    private void AirMove() {
        Vector3 wishdir;
        float wishvel = airAcceleration;
        float accel;

        SetMovementDir();

        wishdir = new Vector3(_cmd.rightMove, 0, _cmd.forwardMove);
        wishdir = transform.TransformDirection(wishdir);

        float wishspeed = wishdir.magnitude;
        wishspeed *= moveSpeed;

        wishdir.Normalize();
        _moveDirectionNorm = wishdir;

        // CPM: Aircontrol
        float wishspeed2 = wishspeed;
        if (Vector3.Dot(_playerVelocity, wishdir) < 0)
            accel = airDeceleration;
        else
            accel = airAcceleration;
        // If the player is ONLY strafing left or right
        if (_cmd.forwardMove == 0 && _cmd.rightMove != 0) {
            if (wishspeed > sideStrafeSpeed)
                wishspeed = sideStrafeSpeed;
            accel = sideStrafeAcceleration;
        }

        Accelerate(wishdir, wishspeed, accel);
        if (airControl > 0)
            AirControl(wishdir, wishspeed2);
        // !CPM: Aircontrol

        // Apply gravity
        _playerVelocity.y -= gravity * Time.deltaTime;
    }

    /**
     * Air control occurs when the player is in the air, it allows
     * players to move side to side much faster rather than being
     * 'sluggish' when it comes to cornering.
     */
    private void AirControl(Vector3 wishdir, float wishspeed) {
        float zspeed;
        float speed;
        float dot;
        float k;

        // Can't control movement if not moving forward or backward
        if (Mathf.Abs(_cmd.forwardMove) < 0.001 || Mathf.Abs(wishspeed) < 0.001)
            return;
        zspeed = _playerVelocity.y;
        _playerVelocity.y = 0;
        /* Next two lines are equivalent to idTech's VectorNormalize() */
        speed = _playerVelocity.magnitude;
        _playerVelocity.Normalize();

        dot = Vector3.Dot(_playerVelocity, wishdir);
        k = 32;
        k *= airControl * dot * dot * Time.deltaTime;

        // Change direction while slowing down
        if (dot > 0) {
            _playerVelocity.x = _playerVelocity.x * speed + wishdir.x * k;
            _playerVelocity.y = _playerVelocity.y * speed + wishdir.y * k;
            _playerVelocity.z = _playerVelocity.z * speed + wishdir.z * k;

            _playerVelocity.Normalize();
            _moveDirectionNorm = _playerVelocity;
        }

        _playerVelocity.x *= speed;
        _playerVelocity.y = zspeed; // Note this line
        _playerVelocity.z *= speed;
    }

    /**
     * Called every frame when the engine detects that the player is on the ground
     */
    private void GroundMove() {
        Vector3 wishdir;

        // Do not apply friction if the player is queueing up the next jump
        if (!_wishJump)
            ApplyFriction(1.0f);
        else
            ApplyFriction(0);

        SetMovementDir();

        wishdir = new Vector3(_cmd.rightMove, 0, _cmd.forwardMove);
        wishdir = transform.TransformDirection(wishdir);
        wishdir.Normalize();
        _moveDirectionNorm = wishdir;

        var wishspeed = wishdir.magnitude;
        wishspeed *= moveSpeed;

        Accelerate(wishdir, wishspeed, runAcceleration);

        // Reset the gravity velocity
        _playerVelocity.y = -gravity * Time.deltaTime;

        if (_wishJump) {
            _playerVelocity.y = jumpSpeed;
            _wishJump = false;
        }
    }

    /**
     * Applies friction to the player, called in both the air and on the ground
     */
    private void ApplyFriction(float t) {
        Vector3 vec = _playerVelocity; // Equivalent to: VectorCopy();
        float speed;
        float newspeed;
        float control;
        float drop;

        vec.y = 0.0f;
        speed = vec.magnitude;
        drop = 0.0f;

        /* Only if the player is on the ground then apply friction */
        if (_controller.isGrounded) {
            control = speed < runDeceleration ? runDeceleration : speed;
            drop = control * friction * Time.deltaTime * t;
        }

        newspeed = speed - drop;
        _playerFriction = newspeed;
        if (newspeed < 0)
            newspeed = 0;
        if (speed > 0)
            newspeed /= speed;

        _playerVelocity.x *= newspeed;
        _playerVelocity.z *= newspeed;
    }

    private void Accelerate(Vector3 wishdir, float wishspeed, float accel) {
        float addspeed;
        float accelspeed;
        float currentspeed;

        currentspeed = Vector3.Dot(_playerVelocity, wishdir);
        addspeed = wishspeed - currentspeed;
        if (addspeed <= 0)
            return;
        accelspeed = accel * Time.deltaTime * wishspeed;
        if (accelspeed > addspeed)
            accelspeed = addspeed;

        _playerVelocity.x += accelspeed * wishdir.x;
        _playerVelocity.z += accelspeed * wishdir.z;
    }

    private void OnGUI() {
        GUI.Label(new Rect(0, 0, 400, 100), "FPS: " + _fps, style);
        var ups = _controller.velocity;
        ups.y = 0;
        GUI.Label(new Rect(0, 15, 400, 100), "Speed: " + Mathf.Round(ups.magnitude * 100) / 100 + "ups", style);
        GUI.Label(new Rect(0, 30, 400, 100), "Top Speed: " + Mathf.Round(_playerTopVelocity * 100) / 100 + "ups", style);
    }
}
