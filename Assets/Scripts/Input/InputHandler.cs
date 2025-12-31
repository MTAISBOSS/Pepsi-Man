using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Script.Input
{
    public class InputHandler : MonoBehaviour
    {
        public static event Action<Vector2> OnMoveInput;

        private InputSystem_Actions _inputAction;

        public Vector2 MoveInput { get; private set; }

        #region Singleton

        public static InputHandler Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                InitializeInput();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        #endregion

        private void InitializeInput()
        {
            _inputAction = new InputSystem_Actions();

            _inputAction.Player.Move.performed += OnMovePerformed;
            _inputAction.Player.Move.canceled += OnMoveCanceled;
        }

        private void OnEnable()
        {
            _inputAction?.Enable();
        }

        private void OnDisable()
        {
            _inputAction?.Disable();
        }

        #region Input Callbacks

        private void OnMovePerformed(InputAction.CallbackContext context)
        {
            MoveInput = context.ReadValue<Vector2>();
            OnMoveInput?.Invoke(MoveInput);
        }

        private void OnMoveCanceled(InputAction.CallbackContext context)
        {
            MoveInput = Vector2.zero;
            OnMoveInput?.Invoke(MoveInput);
        }

        #endregion

        private void OnDestroy()
        {
            if (_inputAction != null)
            {
                _inputAction.Player.Move.performed -= OnMovePerformed;
                _inputAction.Player.Move.canceled -= OnMoveCanceled;
            }
        }

        public void EnablePlayerInput() => _inputAction.Player.Enable();
        public void DisablePlayerInput() => _inputAction.Player.Disable();

        public void EnableUIInput() => _inputAction.UI.Enable();
        public void DisableUIInput() => _inputAction.UI.Disable();
        public bool IsJumpButtonDown() => _inputAction.Player.Jump.WasPressedThisFrame();
        public bool IsSlideButtonDown() => _inputAction.Player.Crouch.WasPressedThisFrame();
        public bool IsSlideButtonUp() => _inputAction.Player.Crouch.WasReleasedThisFrame();
    }
}