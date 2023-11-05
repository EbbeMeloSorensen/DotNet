using System.Collections.Generic;
using FluentAssertions;
using Xunit;

namespace Craft.DataStructures.UnitTest
{
    public class StateMachineV1Test
    {
        [Fact]
        public void FunWithStateMachineForTrafficLight()
        {
            // Arrange
            var red = new StateV1("Red");
            var redAndYellow = new StateV1("Red and Yellow");
            var green = new StateV1("Green");
            var yellow = new StateV1("Yellow");

            var states = new List<StateV1>
            {
                red,
                redAndYellow,
                green,
                yellow
            };

            var trafficLight = new StateMachineV1(states);

            trafficLight.AddTransition(red, redAndYellow);
            trafficLight.AddTransition(redAndYellow, green);
            trafficLight.AddTransition(green, yellow);
            trafficLight.AddTransition(yellow, red);

            // Act/Assert
            trafficLight.StateV1.Name.Should().Be("Red");
            trafficLight.ExitsFromCurrentState.Count.Should().Be(1);

            trafficLight.SwitchState();
            trafficLight.StateV1.Name.Should().Be("Red and Yellow");

            trafficLight.SwitchState();
            trafficLight.StateV1.Name.Should().Be("Green");

            trafficLight.SwitchState();
            trafficLight.StateV1.Name.Should().Be("Yellow");

            trafficLight.SwitchState();
            trafficLight.StateV1.Name.Should().Be("Red");
        }

        [Fact]
        public void FunWithStateMachineForLockableDoor()
        {
            // Arrange
            var closed = new StateV1("Closed");
            var open = new StateV1("Open");
            var locked = new StateV1("Locked");

            var states = new List<StateV1>
            {
                closed,
                open,
                locked
            };

            var trafficLight = new StateMachineV1(states);

            trafficLight.AddTransition(open, closed);
            trafficLight.AddTransition(closed, open, "open");
            trafficLight.AddTransition(closed, locked, "lock");
            trafficLight.AddTransition(locked, closed);

            // Act/Assert
            trafficLight.StateV1.Name.Should().Be("Closed");
            trafficLight.ExitsFromCurrentState.Count.Should().Be(2);

            trafficLight.SwitchState("open");
            trafficLight.StateV1.Name.Should().Be("Open");
            trafficLight.ExitsFromCurrentState.Count.Should().Be(1);

            trafficLight.SwitchState();
            trafficLight.StateV1.Name.Should().Be("Closed");
            trafficLight.ExitsFromCurrentState.Count.Should().Be(2);

            trafficLight.SwitchState("lock");
            trafficLight.StateV1.Name.Should().Be("Locked");
            trafficLight.ExitsFromCurrentState.Count.Should().Be(1);

            trafficLight.SwitchState();
            trafficLight.StateV1.Name.Should().Be("Closed");
        }
    }
}
