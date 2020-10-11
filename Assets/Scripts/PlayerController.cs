using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using Packages.Rider.Editor;
using UnityEditor;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    private Control[] last;
    private Control[] current;

    private Action lastAction { get; set; } = new Action(-1, -1, Action.ActionType.Reset);
    private Action action { get; set; } = new Action(-1, -1, Action.ActionType.Reset);
    private bool firstRun;
    private bool blockSwing;

    // Start is called before the first frame update
    void Start() {
        firstRun = true;
        blockSwing = true;
    }

    // Update is called once per frame
    void Update() {
        FindAction();
    }

    private void FindAction() {
        if (current != null) {
            last = current;
        }

        current = GetCurrentControls();
        if (!firstRun && current.SequenceEqual(last)) {
            return;
        }
        firstRun = false;
        Action.ActionType type;
        if (current.Contains(Control.Insert)) {
            type = Action.ActionType.Reset;
        } else {
            switch (current.Length) {
                case 1 when lastAction.actionType == Action.ActionType.Reset:
                    type = Action.ActionType.Jab;
                    break;
                case 2:
                    type = Action.ActionType.Block;
                    blockSwing = true;
                    break;
                case 1:
                    if (action.actionType == Action.ActionType.Block && blockSwing) {
                        blockSwing = false;
                        type = Action.ActionType.Nothing;
                        break;
                    }
                    type = Action.ActionType.Swing;
                    break;
                default:
                    type = Action.ActionType.Nothing;
                    break;
            }
        }

        int start;
        int end;

        switch (type) {
            case Action.ActionType.Jab: {
                start = end = (int) current[0];
                break;
            }
            case Action.ActionType.Swing: {
                start = lastAction.endPosition;
                end = (int) current[0];
                break;
            }
            case Action.ActionType.Block: {
                int first = (int) current[0];
                int second = (int) current[1];
                
                //first is necessarily lower since preserving insertion order in list of getcontrols()

                //I will point out for the upcoming section, these all could be combined into one if/else statement, but this is significantly more readable and
                //the compiler optimizes it out anyway (I compiled it standalone and looked at comparing .net bytecode)
                if (second - first == 6 && first <= 3) {
                    //legal block, since +6 means vertical assuming low is on bottom
                    start = first;
                    end = second;
                    break;
                }

                if (first % 3 == 1 && second % 3 == 0 && second - first == 2) {
                    //horizontal block
                    start = first;
                    end = second;
                    break;
                }

                if (first == 1 && second == 9 || first == 3 && second == 7) {
                    //diagonal block
                    start = first;
                    end = second;
                    break;
                }

                //illegal block
                start = -1;
                end = -1;
                type = Action.ActionType.Trip;
                break;
            }
            default: {
                //if you reset or tripped, irrelevant
                start = -1;
                end = -1;
                break;
            }
        }

        lastAction = action;
        action = new Action(start, end, type);
    }

    void OnGUI() {
        GUI.Label(new Rect(0,0,100,100), action.startPosition + " " + action.endPosition + " " + action.actionType);
    }

    private enum Control {
        Insert, LowLeft, LowMid, LowRight, MidLeft, MidCenter, MidRight, TopLeft, TopMid, TopRight //use enum ordinals to represent numpad key
        //TODO hack abilities
    }

    private static Control[] GetCurrentControls() {
        List<Control> controls = new List<Control>();
        if (Input.GetKey(KeyCode.Insert) || Input.GetKey(KeyCode.Keypad0)) {
           controls.Add(Control.Insert); 
        }

        if (Input.GetKey(KeyCode.Keypad1)) {
            controls.Add(Control.LowLeft);
        }

        if (Input.GetKey(KeyCode.Keypad2)) {
            controls.Add(Control.LowMid);
        }
        if (Input.GetKey(KeyCode.Keypad3)) {
            controls.Add(Control.LowRight);
        }
        if (Input.GetKey(KeyCode.Keypad4)) {
            controls.Add(Control.MidLeft);
        }
        if (Input.GetKey(KeyCode.Keypad5)) {
            controls.Add(Control.MidCenter);
        }
        if (Input.GetKey(KeyCode.Keypad6)) {
            controls.Add(Control.MidRight);
        }
        if (Input.GetKey(KeyCode.Keypad7)) {
            controls.Add(Control.TopLeft);
        }
        if (Input.GetKey(KeyCode.Keypad8)) {
            controls.Add(Control.TopMid);
        }
        if (Input.GetKey(KeyCode.Keypad9)) {
            controls.Add(Control.TopRight);
        }

        return controls.ToArray();
    }


    public class Action {
        internal int startPosition { get; }
        internal int endPosition { get; }
        internal ActionType actionType { get; }

        internal enum ActionType {
            Swing, Jab, Block, Reset, Trip, Nothing
        }

        internal Action(int startPosition, int endPosition, ActionType actionType) {
            this.startPosition = startPosition;
            this.endPosition = endPosition;
            this.actionType = actionType;
        }
    }
}
