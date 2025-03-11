using UnityEngine;

namespace Player.ActiveRagdoll
{
    public class HandContact : MonoBehaviour
    {
        [SerializeField] private ActiveRagdoll _player;

        [SerializeField] private bool Left;
        [SerializeField] private bool hasJoint;

        private InputSystem _inputSystem;

        private void OnEnable()
        {
            _inputSystem = new InputSystem();
            _inputSystem.Player.Enable();
        }

        private void OnDisable()
        {
            _inputSystem.Player.Disable();
        }
        private void Update()
        {
            if (_player.UseConrols())
            {
                //On input release destroy joint
                if (Left)
                {
                    if (hasJoint && !_inputSystem.Player.ReachLeft.IsPressed())
                    {
                        this.gameObject.GetComponent<FixedJoint>().breakForce = 0;
                        hasJoint = false;
                    }

                    if (hasJoint && this.gameObject.GetComponent<FixedJoint>() == null)
                    {
                        hasJoint = false;
                    }
                }

                //On input release destroy joint
                if (!Left)
                {
                    if (hasJoint && !_inputSystem.Player.ReachRight.IsPressed())
                    {
                        this.gameObject.GetComponent<FixedJoint>().breakForce = 0;
                        hasJoint = false;
                    }

                    if (hasJoint && this.gameObject.GetComponent<FixedJoint>() == null)
                    {
                        hasJoint = false;
                    }
                }
            }
        }

        private void OnCollisionEnter(Collision col)
        {
            if (_player.UseConrols())
            {
                if (Left)
                {
                    if (col.gameObject.tag == "CanBeGrabbed" && col.gameObject.layer != _player.ThisPlayerLayer() && !hasJoint)
                    {
                        if (_inputSystem.Player.ReachLeft.IsPressed() && !hasJoint && !_player.PunchingLeft())
                        {
                            hasJoint = true;
                            this.gameObject.AddComponent<FixedJoint>();
                            this.gameObject.GetComponent<FixedJoint>().breakForce = Mathf.Infinity;
                            this.gameObject.GetComponent<FixedJoint>().connectedBody = col.gameObject.GetComponent<Rigidbody>();
                        }
                    }

                }

                if (!Left)
                {
                    if (col.gameObject.tag == "CanBeGrabbed" && col.gameObject.layer != _player.ThisPlayerLayer() && !hasJoint)
                    {
                        if (_inputSystem.Player.ReachRight.IsPressed() && !hasJoint && !_player.PunchingRight())
                        {
                            hasJoint = true;
                            this.gameObject.AddComponent<FixedJoint>();
                            this.gameObject.GetComponent<FixedJoint>().breakForce = Mathf.Infinity;
                            this.gameObject.GetComponent<FixedJoint>().connectedBody = col.gameObject.GetComponent<Rigidbody>();
                        }
                    }
                }
            }
        }
    }
}