using System;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using thebasics.ModSystems.ProximityChat.Models;

namespace DiceRollAddon
{
    public class RollCommand : ModSystem
    {
        private ICoreClientAPI capi;

        // Méthode appelée côté client
        public override void StartClientSide(ICoreClientAPI api)
        {
            base.StartClientSide(api);
            capi = api;

            // Enregistrement de la commande /roll côté client
            capi.ChatCommands
                .Create("roll")
                .WithDescription("Effectue un jet de dés")
                .WithArgs(api.ChatCommands.Parsers.Word("diceInput")) // Utilisation correcte du parseur d'argument
                .HandleWith(OnRollCommand);
        }

        // Gestionnaire de la commande /roll
        private TextCommandResult OnRollCommand(TextCommandCallingArgs args)
        {
            // Récupération de l'argument du jet de dés
            string diceInput = (string)args[0];

            // Vérification de l'argument
            if (string.IsNullOrEmpty(diceInput))
            {
                return TextCommandResult.Error("Usage: /roll 1d6 ou /roll 1d20");
            }

            // Fonction pour effectuer un jet de dés
            int diceSides;
            int diceRoll = RollDice(diceInput, out diceSides);

            if (diceRoll == -1)
            {
                return TextCommandResult.Error("Mauvais format de dés. Utilise 1d6 ou /roll 1d20.");
            }

            // Récupère le joueur qui a exécuté la commande
            IClientPlayer player = capi.World.Player;
            string playerName = player.PlayerName;

            // Message du résultat
            string resultMessage = $"{playerName} a lancé un {diceInput} et a obtenu : {diceRoll}";

            // Envoi du résultat via le chat de proximité (say/normal/s)
            capi.SendChatMessage($"/say {resultMessage}"); // Utilise la commande /say pour envoyer le message dans le chat de proximité

            return TextCommandResult.Success();
        }

        // Fonction pour effectuer un lancer de dés
        private int RollDice(string input, out int sides)
        {
            sides = 0;
            string[] parts = input.Split('d');
            if (parts.Length != 2) return -1;

            if (int.TryParse(parts[1], out sides) && sides > 0)
            {
                Random random = new Random();
                return random.Next(1, sides + 1);
            }

            return -1;
        }
    }
}
