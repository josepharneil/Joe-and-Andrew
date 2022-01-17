using System.Collections.Generic;
using Bolt;
using Ludiq;
using UnityEngine;

namespace AI
{
    [UnitCategory("Custom Units")]
    [UnitTitle("List<StateAction> Node")]
    public class ListStateActionNode : Unit
    {
        [DoNotSerialize] // No need to serialize ports.
        // ReSharper disable once NotAccessedField.Local
        private ControlInput _inputTrigger; //Adding the ControlInput port variable

        [DoNotSerialize] // No need to serialize ports.
        private ControlOutput _outputTrigger;//Adding the ControlOutput port variable.

        [DoNotSerialize] // No need to serialize ports.
        private ValueInput _stateActionListInputValue;
        
        [DoNotSerialize] // No need to serialize ports.
        private ValueInput _targetGameObject;

        protected override void Definition()
        {
            //Making the ControlInput port visible, setting its key and running the anonymous action method to pass the flow to the outputTrigger port.
            _inputTrigger = ControlInput("", flow =>
            {                
                GameObject gameObject = flow.GetValue<GameObject>(_targetGameObject);
                if (gameObject == null)
                {
                    Debug.LogError("Missing target Game Object");
                    return _outputTrigger;
                }
                List<StateAction> stateActions = flow.GetValue<List<StateAction>>(_stateActionListInputValue);
                foreach (StateAction stateAction in stateActions)
                {
                    if (stateAction)
                    {
                        stateAction.PerformAction(flow.stack.self);
                    }
                    else
                    {
                        Debug.LogError("Missing state action", gameObject);
                    }
                }
                return _outputTrigger;
            });
            //Making the ControlOutput port visible and setting its key.
            _outputTrigger = ControlOutput("");
            
            _stateActionListInputValue = ValueInput<List<StateAction>>("State Actions", new List<StateAction>());
            
            _targetGameObject = ValueInput("Target", (GameObject)null).NullMeansSelf();
        }
    }
}