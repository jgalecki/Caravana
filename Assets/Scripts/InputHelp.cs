using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.IO
{
    public static class InputHelp
    {
        public enum Buttons { Left, Right, Up, Down, Jump, Attack, Start }

        internal static bool AnyButtonDown()
        {
            return GetButtonDown(Buttons.Attack) ||
                GetButtonDown(Buttons.Down) ||
                GetButtonDown(Buttons.Jump) ||
                GetButtonDown(Buttons.Left) ||
                GetButtonDown(Buttons.Right) ||
                GetButtonDown(Buttons.Start) ||
                GetButtonDown(Buttons.Up);
        }

        //public const float JOYSTICK_DEADSPACE = 0.2f;

        //public static bool KeyboardAndMouseEnabled;

        public static bool GetButtonDown(Buttons button)
        {
            switch (button)
            {
                case Buttons.Left:
                    return Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.Q);
                case Buttons.Right:
                    return Input.GetKeyDown(KeyCode.D);
                case Buttons.Up:
                    return Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.Z);
                case Buttons.Down:
                    return Input.GetKeyDown(KeyCode.S);
                case Buttons.Jump:
                    return Input.GetKeyDown(KeyCode.Space);
                case Buttons.Attack:
                    return Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Return);
                case Buttons.Start:
                    return Input.GetKeyDown(KeyCode.F);
                default:
                    throw new ArgumentException("Not prepared for buttondown button  " + button.ToString());
            }
        }

        public static bool GetButtonUp(Buttons button)
        {
            switch (button)
            {
                case Buttons.Left:
                    return Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.Q);
                case Buttons.Right:
                    return Input.GetKeyUp(KeyCode.D);
                case Buttons.Up:
                    return Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.Z);
                case Buttons.Down:
                    return Input.GetKeyUp(KeyCode.S);
                case Buttons.Jump:
                    return Input.GetKeyUp(KeyCode.Space);
                case Buttons.Attack:
                    return Input.GetMouseButtonUp(0);
                case Buttons.Start:
                    return Input.GetKeyUp(KeyCode.F);
                default:
                    throw new ArgumentException("Not prepared for buttonup button " + button.ToString());
            }
        }

        public static bool GetButton(Buttons button)
        {
            switch (button)
            {
                case Buttons.Left:
                    return Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.Q);
                case Buttons.Right:
                    return Input.GetKey(KeyCode.D);
                case Buttons.Up:
                    return Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.Z);
                case Buttons.Down:
                    return Input.GetKey(KeyCode.S);
                case Buttons.Jump:
                    return Input.GetKey(KeyCode.Space);
                case Buttons.Attack:
                    return Input.GetMouseButton(0);
                case Buttons.Start:
                    return Input.GetKey(KeyCode.F);
                default:
                    throw new ArgumentException("Not prepared for buttonup button " + button.ToString());
            }
        }

        //public static float GetVertJoystick(bool deadzone = true)
        //{
        //    if (Input.GetKey(KeyCode.W))
        //    {
        //        KeyboardAndMouseEnabled = true;
        //        return 1;
        //    }
        //    else if (Input.GetKey(KeyCode.S))
        //    {
        //        KeyboardAndMouseEnabled = true;
        //        return -1;
        //    }

        //    if (deadzone && Math.Abs(Input.GetAxis("Vertical")) < JOYSTICK_DEADSPACE)
        //    {
        //        return 0;
        //    }

        //    return Input.GetAxis("Vertical");
        //}

        //internal static float GetVertJoystickRaw()
        //{
        //    return GetVertJoystick(false);
        //}

        //public static float GetHorJoystick(bool deadZone = true)
        //{
        //    if (Input.GetKey(KeyCode.D))
        //    {
        //        KeyboardAndMouseEnabled = true;
        //        return 1;
        //    }
        //    else if (Input.GetKey(KeyCode.A))
        //    {
        //        KeyboardAndMouseEnabled = true;
        //        return -1;
        //    }

        //    if (deadZone && Math.Abs(Input.GetAxis("Horizontal")) < JOYSTICK_DEADSPACE)
        //    {
        //        return 0;
        //    }

        //    return Input.GetAxis("Horizontal");
        //}

        //internal static float GetHorJoystickRaw()
        //{
        //    return GetHorJoystick(false);
        //}

        //internal static bool InDeadSpace()
        //{
        //    if (KeyboardAndMouseEnabled) return false;
        //    return Math.Abs(Input.GetAxis("Horizontal")) < JOYSTICK_DEADSPACE && Math.Abs(Input.GetAxis("Vertical")) < JOYSTICK_DEADSPACE;
        //}

        // Keep all our classes only interacting with this class. Shadow ResetInputAxes().
        internal static void ResetInputAxes()
        {
            Input.ResetInputAxes();
        }

        internal static Vector2 MousePosition()
        {
            return new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        }
    }
}
