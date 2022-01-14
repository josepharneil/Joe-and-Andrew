using System;
using Bolt;
using Ludiq;

namespace AI
{
    [UnitCategory("Custom Units")]
    [UnitTitle("Transition Predicate Node")]
    public class TransitionPredicateNode : Unit
    {
        [DoNotSerialize] // No need to serialize ports.
        // ReSharper disable once NotAccessedField.Local
        private ControlInput _inputTrigger; //Adding the ControlInput port variable

        [DoNotSerialize] // No need to serialize ports.
        private ControlOutput _outputTrigger;//Adding the ControlOutput port variable.

        [DoNotSerialize] // No need to serialize ports.
        private ValueInput _transitionPredicateInputValue;

        [DoNotSerialize] // No need to serialize ports.
        private ValueOutput _transitionPredicateOutputValue;
        
        protected override void Definition()
        {
            //Making the ControlInput port visible, setting its key and running the anonymous action method to pass the flow to the outputTrigger port.
            _inputTrigger = ControlInput("", flow =>
            {
                TransitionPredicate transitionPredicate = flow.GetValue<TransitionPredicate>(_transitionPredicateInputValue);
                if (transitionPredicate)
                {
                    bool result = transitionPredicate.IsPredicateSatisfied(flow.stack.gameObject);
                    flow.SetValue(_transitionPredicateOutputValue, result);
                }
                return _outputTrigger;
            });
            //Making the ControlOutput port visible and setting its key.
            _outputTrigger = ControlOutput("");
            
            _transitionPredicateInputValue = ValueInput<TransitionPredicate>("Transition Predicate", null);
            
            _transitionPredicateOutputValue = ValueOutput<bool>("Predicate");
        }
    }
}
