using System;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Config;

namespace DiceRollAddon
{
    public class RollCommand : ModSystem
    {
        private ICoreClientAPI capi;

        public override void StartClientSide(ICoreClientAPI api)
        {
            base.StartClientSide(api);
            capi = api;

            capi.ChatCommands
                .Create("roll")
                .WithDescription("Make a dice roll")
                .WithArgs(api.ChatCommands.Parsers.Word("diceInput"))
                .HandleWith(OnRollCommand);
        }
        private TextCommandResult OnRollCommand(TextCommandCallingArgs args)
        {
            string diceInput = (string)args[0];

            if (string.IsNullOrEmpty(diceInput))
            {
                return TextCommandResult.Error(Lang.Get("diceRoll.usage"));
            }

            int diceSides, totalRoll;
            string individualRolls;

            if (!RollDice(diceInput, out diceSides, out totalRoll, out individualRolls))
            {
                return TextCommandResult.Error(Lang.Get("diceRoll.invalidFormat"));
            }

            IClientPlayer player = capi.World.Player;
            string playerName = player.PlayerName;

            string localizedMessage = Lang.Get(
                "diceRoll.resultMessage",
                playerName,
                diceInput,
                totalRoll,
                individualRolls
            );

            capi.SendChatMessage($"/it {localizedMessage}");

            return TextCommandResult.Success();
        }
        private bool RollDice(string input, out int diceSides, out int totalRoll, out string individualRolls)
        {
            totalRoll = 0;
            individualRolls = "";
            diceSides = 0;

            string[] parts = input.Split('d');
            if (parts.Length != 2) return false;

            int numberOfRolls;
            if (int.TryParse(parts[0], out numberOfRolls) && int.TryParse(parts[1], out diceSides) && numberOfRolls > 0 && diceSides > 0)
            {
                Random random = new Random();
                for (int i = 0; i < numberOfRolls; i++)
                {
                    int roll = random.Next(1, diceSides + 1);
                    totalRoll += roll;
                    individualRolls += roll.ToString() + (i < numberOfRolls - 1 ? ", " : "");
                }
                return true;
            }

            return false;
        }
    }
}
