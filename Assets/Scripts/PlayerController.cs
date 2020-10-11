using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Packages.Rider.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerController : MonoBehaviour {
    private Control[] last;
    private Control[] current;
    private Transform sword;
    [FormerlySerializedAs("points")] public Transform[] attackPoints;
    public Transform[] blockPoints;

    private Action lastAction { get; set; } = new Action(0, 0, Action.ActionType.Reset);
    public static Action action { get; set; } = new Action(0, 0, Action.ActionType.Reset);
    private Action copyBuffer;
    private bool firstRun;
    private bool blockSwing;
    private float trip;
    
    // Start is called before the first frame update
    void Start() {
        firstRun = true;
        blockSwing = true;
        sword = transform.GetChild(0);
        trip = 0;
    }

    // Update is called once per frame
    void Update() {
        FindAction();
        SwingSword();
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
        if (current.Contains(Control.Copy)) {
            copyBuffer = action;
            return;
        }

        if (current.Contains(Control.Paste)) {
            action = copyBuffer;
            trip = 0;
            return;
        }
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
                //legal swing combinations
                if (Math.Abs(end - start) == 1 || Math.Abs(end - start) == 3) {
                    break;
                }

                type = Action.ActionType.Trip;
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
            case Action.ActionType.Reset:
                start = 0;
                end = 0;
                break;
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

    private void SwingSword() {
        if (trip > 0) {
            trip = Math.Max(0, trip - Time.deltaTime);
            return;
        }
        switch (action.actionType) {
            case Action.ActionType.Reset:
            case Action.ActionType.Swing:
            case Action.ActionType.Jab:
                sword.position = attackPoints[action.endPosition].position;
                sword.rotation = attackPoints[action.endPosition].rotation;
                //todo animate
                break;
            case Action.ActionType.Block:
                switch (action.startPosition) {
                    case 1:
                        switch (action.endPosition) {
                            case 3:
                                sword.position = blockPoints[0].position;
                                sword.rotation = blockPoints[0].rotation;
                                break;
                            case 7:
                                sword.position = blockPoints[1].position;
                                sword.rotation = blockPoints[1].rotation;
                                break;
                            case 9:
                                sword.position = blockPoints[2].position;
                                sword.rotation = blockPoints[2].rotation;
                                break;
                        }

                        break;
                    case 2:
                        sword.position = blockPoints[3].position;
                        sword.rotation = blockPoints[3].rotation;
                        break;
                    case 3:
                        switch (action.endPosition) {
                            case 7:
                                sword.position = blockPoints[4].position;
                                sword.rotation = blockPoints[4].rotation;
                                break;
                            case 9:
                                sword.position = blockPoints[5].position;
                                sword.rotation = blockPoints[5].rotation;
                                break;
                        }

                        break;
                    case 4:
                        sword.position = blockPoints[6].position;
                        sword.rotation = blockPoints[6].rotation;
                        break;
                    case 7:
                        sword.position = blockPoints[7].position;
                        sword.rotation = blockPoints[7].rotation;
                        break;
                }
                break;
            case Action.ActionType.Trip:
                //TODO add a stumble animation
                trip = 0.5f;
                sword.position = attackPoints[0].position;
                sword.rotation = attackPoints[0].rotation;
                break;
        }
    }
    
    void OnGUI() {
        GUI.Label(new Rect(0,0,100,100), action.startPosition + " " + action.endPosition + " " + action.actionType);
    }

    private enum Control {
        Insert, LowLeft, LowMid, LowRight, MidLeft, MidCenter, MidRight, TopLeft, TopMid, TopRight, Copy, Paste //use enum ordinals to represent numpad key
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

        if (Input.GetKey(KeyCode.C)) {
            controls.Add(Control.Copy);
        }

        if (Input.GetKeyDown(KeyCode.V)) {
            controls.Add(Control.Paste);
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
