using Content.Server.Chemistry.Components.SolutionManager;
using Content.Server.Chemistry.EntitySystems;
using Content.Shared.Administration;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.FixedPoint;
using Robust.Shared.Console;
using Robust.Shared.Prototypes;

namespace Content.Server.Administration.Commands
{
    /// <summary>
    ///     Command that allows you to edit an existing solution by adding (or removing) reagents.
    /// </summary>
    [AdminCommand(AdminFlags.Fun)]
    public sealed class AddReagent : IConsoleCommand
    {
        public string Command => "addreagent";
        public string Description => "Add (or remove) some amount of reagent from some solution.";
        public string Help => $"Usage: {Command} <target> <solution> <reagent> <quantity>";

        public void Execute(IConsoleShell shell, string argStr, string[] args)
        {
            if (args.Length < 4)
            {
                shell.WriteLine($"Not enough arguments.\n{Help}");
                return;
            }

            if (!EntityUid.TryParse(args[0], out var uid))
            {
                shell.WriteLine($"Invalid entity id.");
                return;
            }

            if (!IoCManager.Resolve<IEntityManager>().TryGetComponent(uid, out SolutionContainerManagerComponent man))
            {
                shell.WriteLine($"Entity does not have any solutions.");
                return;
            }

            if (!man.Solutions.ContainsKey(args[1]))
            {
                var validSolutions = string.Join(", ", man.Solutions.Keys);
                shell.WriteLine($"Entity does not have a \"{args[1]}\" solution. Valid solutions are:\n{validSolutions}");
                return;
            }
            var solution = man.Solutions[args[1]];

            if (!IoCManager.Resolve<IPrototypeManager>().HasIndex<ReagentPrototype>(args[2]))
            {
                shell.WriteLine($"Unknown reagent prototype");
                return;
            }

            if (!float.TryParse(args[3], out var quantityFloat))
            {
                shell.WriteLine($"Failed to parse quantity");
                return;
            }
            var quantity = FixedPoint2.New(MathF.Abs(quantityFloat));

            if (quantityFloat > 0)
                EntitySystem.Get<SolutionContainerSystem>().TryAddReagent(uid, solution, args[2], quantity, out var _);
            else
                EntitySystem.Get<SolutionContainerSystem>().TryRemoveReagent(uid, solution, args[2], quantity);
        }
    }
}
