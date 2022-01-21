using Bolt;
using Ludiq;
using UnityEngine;

namespace AI
{
    [UnitCategory("Custom Units")]
    [UnitTitle("TransitionPredicateNode")]
    public class TransitionPredicateNode : Unit
    {
        [DoNotSerialize] // No need to serialize ports.
        // ReSharper disable once NotAccessedField.Local
        private ControlInput _inputTrigger; //Adding the ControlInput port variable

        [DoNotSerialize] // No need to serialize ports.
        private ControlOutput _outputTriggerTrue;//Adding the ControlOutput port variable.
        
        private ControlOutput _outputTriggerFalse;//Adding the ControlOutput port variable.

        [DoNotSerialize] // No need to serialize ports.
        private ValueInput _transitionPredicateInputValue;
        
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
                    Debug.LogError("Missing target GameObject");
                    return _outputTriggerFalse;
                }
                TransitionPredicate transitionPredicate = flow.GetValue<TransitionPredicate>(_transitionPredicateInputValue);
                if (transitionPredicate == null)
                {
                    Debug.LogError("Missing transition predicate", gameObject);
                    return _outputTriggerFalse;
                }
                return transitionPredicate.IsPredicateSatisfied(gameObject) ? _outputTriggerTrue : _outputTriggerFalse;
            });
            //Making the ControlOutput port visible and setting its key.
            _outputTriggerTrue = ControlOutput("True");
            _outputTriggerFalse = ControlOutput("False");
            
            // TriggerCustomEvent
            _transitionPredicateInputValue = ValueInput<TransitionPredicate>("Transition Predicate", null);

            _targetGameObject = ValueInput("Target (None is Self)", (GameObject)null).NullMeansSelf();
        }
    }
}
