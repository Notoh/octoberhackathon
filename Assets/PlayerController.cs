using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Packages.Rider.Editor;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    private Control[] last;
    private Control[] current;

    private Action lastAction { get; set; }
    private Action action { get; set; }
    
    // Start is called before the first frame update
    void Start() {
        lastAction = new Action(-1, -1, Action.ActionType.Reset);
        StartCoroutine(OnButtonPress());
    }

    // Update is called once per frame
    void Update() {
        last = current;
        current = GetCurrentControls();
    }

    IEnumerator OnButtonPress() {
        yield return new WaitUntil(() => !last.SequenceEqual(current));

        Action.ActionType type;
        if (current.Contains(Control.Insert)) {
            type = Action.ActionType.Reset;
        } else {
            if (current.Length == 1 && lastAction.actionType == Action.ActionType.Reset) {
                type = Action.ActionType.Jab;
            } else if (current.Length == 2) {
                type = Action.ActionType.Block;
            } else if(current.Length == 1) {
                type = Action.ActionType.Swing;
            } else {
                type = Action.ActionType.Trip;
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
                if (first > second) {
                    //reorder so first is lower, makes life easier
                    int temp = first;
                    first = second;
                    second = temp;
                }

                //I will point out for the upcoming section, these all could be combined into one if/else statement, but this is significantly more readable and
                //the compiler optimizes it out anyway (I compiled it standalone and looked at comparing .net bytecode)
                if (second - first == 6 && first <= 3) {
                    //legal block, since +6 means vertical assuming low is on bottom
                    start = first;
                    end = second;
                    break;
                }

                if (first % 3 == 1 && second % 3 == 0) { //horizontal block
                    start = first;
                    end = second;
                    break;
                }

                if (first == 1 && second == 9 || first == 3 && second == 7) { //diagonal block
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

    private enum Control {
        Insert, LowLeft, LowMid, LowRight, MidLeft, MidCenter, MidRight, TopLeft, TopMid, TopRight //use enum ordinals to represent numpad key
        //TODO hack abilities
    }

    private static bool IsControlPressed(Control control) {
        switch (control) {
            case Control.Insert:
                return Input.GetKeyDown(KeyCode.Insert) || Input.GetKeyDown(KeyCode.Keypad0);
            case Control.LowLeft:
                return Input.GetKeyDown(KeyCode.Keypad1);
            case Control.LowMid:
                return Input.GetKeyDown(KeyCode.Keypad2);
            case Control.LowRight:
                return Input.GetKeyDown(KeyCode.Keypad3);
            case Control.MidLeft:
                return Input.GetKeyDown(KeyCode.Keypad4);
            case Control.MidCenter:
                return Input.GetKeyDown(KeyCode.Keypad5);
            case Control.MidRight:
                return Input.GetKeyDown(KeyCode.Keypad6);
            case Control.TopLeft:
                return Input.GetKeyDown(KeyCode.Keypad7);
            case Control.TopMid:
                return Input.GetKeyDown(KeyCode.Keypad8);
            case Control.TopRight:
                return Input.GetKeyDown(KeyCode.Keypad9);
            default:
                return false;
        }
    }

    private static Control[] GetCurrentControls() {
        return Enum.GetValues(typeof(Control)).Cast<Control>().Where(IsControlPressed).ToArray();
    }


    public class Action {
        internal int startPosition { get; }
        internal int endPosition { get; }
        internal ActionType actionType { get; }

        internal enum ActionType {
            Swing, Jab, Block, Reset, Trip
        }

        internal Action(int startPosition, int endPosition, ActionType actionType) {
            this.startPosition = startPosition;
            this.endPosition = endPosition;
            this.actionType = actionType;
        }
    }
}
