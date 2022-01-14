using Bolt;
using Ludiq;

namespace AI
{
    [UnitCategory("Custom Units")]
    [UnitTitle("State Action Node")]
    public class StateActionNode : Unit
    {
        [DoNotSerialize] // No need to serialize ports.
        // ReSharper disable once NotAccessedField.Local
        private ControlInput _inputTrigger; //Adding the ControlInput port variable

        [DoNotSerialize] // No need to serialize ports.
        private ControlOutput _outputTrigger;//Adding the ControlOutput port variable.

        [DoNotSerialize] // No need to serialize ports.
        private ValueInput _stateActionInputValue;
        
        protected override void Definition()
        {
            //Making the ControlInput port visible, setting its key and running the anonymous action method to pass the flow to the outputTrigger port.
            _inputTrigger = ControlInput("", flow =>
            {
                StateAction stateAction = flow.GetValue<StateAction>(_stateActionInputValue);
                if (stateAction)
                {
                    stateAction.PerformAction(flow.stack.gameObject);
                }
                // var array = flow.GetValue<List<PatrolStateAction>>(_stateActionInputValue);
                // foreach (StateAction stateAction in array)
                // {
                //     if (stateAction == null) continue;
                //     stateAction.PerformAction();
                // }
                
                
                return _outputTrigger;
            });
            //Making the ControlOutput port visible and setting its key.
            _outputTrigger = ControlOutput("");
            
            _stateActionInputValue = ValueInput<StateAction>("StateAction", null);
            
        }
    }
}
