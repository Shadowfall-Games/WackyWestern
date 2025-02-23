using UnityEngine;

public class HandContact : MonoBehaviour
    {
        public ActiveRagdoll _player;
    
        //Is left or right hand
        public bool Left;
    
        //Have joint/grabbed
        public bool hasJoint;

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
            if(_player._useControls)
            {
                //Left Hand
                //On input release destroy joint
                if(Left)
                {
                    if(hasJoint && !_inputSystem.Player.ReachLeft.IsPressed())
                    {
                        this.gameObject.GetComponent<FixedJoint>().breakForce = 0;
                        hasJoint = false;
                    }

                    if(hasJoint && this.gameObject.GetComponent<FixedJoint>() == null)
                    {
                        hasJoint = false;
                    }
                }

                //Right Hand
                //On input release destroy joint
                if(!Left)
                {
                    if(hasJoint && !_inputSystem.Player.ReachRight.IsPressed())
                    {
                        this.gameObject.GetComponent<FixedJoint>().breakForce = 0;
                        hasJoint = false;
                    }

                    if(hasJoint && this.gameObject.GetComponent<FixedJoint>() == null)
                    {
                        hasJoint = false;
                    }
                }
            }
        }

        //Grab on collision when input is used
        private void OnCollisionEnter(Collision col)
        {
            if(_player._useControls)
            {
                //Left Hand
                if(Left)
                {
                    if(col.gameObject.tag == "CanBeGrabbed" && col.gameObject.layer != LayerMask.NameToLayer(_player._thisPlayerLayer) && !hasJoint)
                    {
                        if(_inputSystem.Player.ReachLeft.IsPressed() && !hasJoint && !_player._punchingLeft)
                        {
                            hasJoint = true;
                            this.gameObject.AddComponent<FixedJoint>();
                            this.gameObject.GetComponent<FixedJoint>().breakForce = Mathf.Infinity;
                            this.gameObject.GetComponent<FixedJoint>().connectedBody = col.gameObject.GetComponent<Rigidbody>();
                        }
                    }
                
                }

                //Right Hand
                if(!Left)
                {
                    if(col.gameObject.tag == "CanBeGrabbed" && col.gameObject.layer != LayerMask.NameToLayer(_player._thisPlayerLayer) && !hasJoint)
                    {
                        if(_inputSystem.Player.ReachRight.IsPressed() && !hasJoint && !_player._punchingRight)
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