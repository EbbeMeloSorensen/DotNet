using System.Collections.Generic;
using FluentAssertions;
using Xunit;

namespace Craft.DataStructures.UnitTest
{
    public class StateMachineTest
    {
        [Fact]
        public void FunWithStateMachineForTrafficLight()
        {
            // Arrange
            var red = new State("Red");
            var redAndYellow = new State("Red and Yellow");
            var green = new State("Green");
            var yellow = new State("Yellow");

            var states = new List<State>
            {
                red,
                redAndYellow,
                green,
                yellow
            };

            var trafficLight = new StateMachine(states);

            trafficLight.AddTransition(red, redAndYellow);
            trafficLight.AddTransition(redAndYellow, green);
            trafficLight.AddTransition(green, yellow);
            trafficLight.AddTransition(yellow, red);

            // Act/Assert
            trafficLight.State.Name.Should().Be("Red");
            trafficLight.ExitsFromCurrentState.Count.Should().Be(1);

            trafficLight.SwitchState();
            trafficLight.State.Name.Should().Be("Red and Yellow");

            trafficLight.SwitchState();
            trafficLight.State.Name.Should().Be("Green");

            trafficLight.SwitchState();
            trafficLight.State.Name.Should().Be("Yellow");

            trafficLight.SwitchState();
            trafficLight.State.Name.Should().Be("Red");
        }

        [Fact]
        public void FunWithStateMachineForLockableDoor()
        {
            // Arrange
            var closed = new State("Closed");
            var open = new State("Open");
            var locked = new State("Locked");

            var states = new List<State>
            {
                closed,
                open,
                locked
            };

            var trafficLight = new StateMachine(states);

            trafficLight.AddTransition(open, closed);
            trafficLight.AddTransition(closed, open, "open");
            trafficLight.AddTransition(closed, locked, "lock");
            trafficLight.AddTransition(locked, closed);

            // Act/Assert
            trafficLight.State.Name.Should().Be("Closed");
            trafficLight.ExitsFromCurrentState.Count.Should().Be(2);

            trafficLight.SwitchState("open");
            trafficLight.State.Name.Should().Be("Open");
            trafficLight.ExitsFromCurrentState.Count.Should().Be(1);

            trafficLight.SwitchState();
            trafficLight.State.Name.Should().Be("Closed");
            trafficLight.ExitsFromCurrentState.Count.Should().Be(2);

            trafficLight.SwitchState("lock");
            trafficLight.State.Name.Should().Be("Locked");
            trafficLight.ExitsFromCurrentState.Count.Should().Be(1);

            trafficLight.SwitchState();
            trafficLight.State.Name.Should().Be("Closed");
        }
    }
}
