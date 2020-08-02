using System.Collections.Generic;


namespace Generics.Robots {
    public interface IRobotAi<out TCommand> {
        TCommand GetCommand();
    }

    public class ShooterAi : IRobotAi<ShooterCommand> {
        int _counter = 1;

        public ShooterCommand GetCommand() {
            return ShooterCommand.ForCounter(_counter++);
        }
    }

    public class BuilderAi : IRobotAi<BuilderCommand> {
        int _counter = 1;

        public BuilderCommand GetCommand() {
            return BuilderCommand.ForCounter(_counter++);
        }
    }

    public interface IDevice<in TCommand> {
        string ExecuteCommand(TCommand command);
    }

    public class Mover : IDevice<IMoveCommand> {
        public string ExecuteCommand(IMoveCommand command) {
            return $"MOV {command.Destination.X}, {command.Destination.Y}";
        }
    }


    public class Robot {
        IRobotAi<IMoveCommand> ai;
        IDevice<IMoveCommand> device;

        public Robot(IRobotAi<IMoveCommand> ai, IDevice<IMoveCommand> executor) {
            this.ai = ai;
            this.device = executor;
        }

        public IEnumerable<string> Start(int steps) {
            for (int i = 0; i < steps; i++) {
                var command = ai.GetCommand();
                if (command == null)
                    break;
                yield return device.ExecuteCommand(command);
            }
        }

        public static Robot Create(IRobotAi<IMoveCommand> ai, IDevice<IMoveCommand> executor) {
            return new Robot(ai, executor);
        }
    }
}