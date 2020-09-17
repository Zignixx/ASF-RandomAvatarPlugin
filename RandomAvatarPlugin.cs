using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ArchiSteamFarm.Json;
using ArchiSteamFarm.Plugins;
using SteamKit2;

namespace ArchiSteamFarm.Cobra.RenamePlugin {
	[Export(typeof(IPlugin))]
	internal sealed class RandomAvatarPlugin : IBotCommand {
		private static readonly Random Random = new Random();
		internal static int RandomNext(int min, int max) {
			lock (Random) {
				return Random.Next(min, max);
			}
		}
		public string Name => nameof(RandomAvatarPlugin);
		public Version Version => typeof(RandomAvatarPlugin).Assembly.GetName().Version!;

		public async Task<string?> ResponseRandomAvatar(Bot bot, ulong steamID) {
			if (steamID == 0) {
				bot.ArchiLogger.LogNullError(nameof(steamID));

				return null;
			}

			if (!bot.HasPermission(steamID, BotConfig.EPermission.Master)) {
				return null;
			}

			if (!bot.IsConnectedAndLoggedOn) {
				return bot.Commands.FormatBotResponse(ArchiSteamFarm.Localization.Strings.BotNotConnected);
			}

			string[] avatars = new string[56] { "218620,109", "230410,27", "302670,8", "231430,40", "8500,9", "252150,10", "742230,1", "967750,1", "955900,1", "703390,15", "1165120,1", "466740,1", "432940,2", "873090,1", "636700,1", "39170,14", "468170,1", "835860,40", "38740,6", "639840,1", "1138260,1", "1032840,1", "966450,2", "104100,3", "520530,1", "873980,1", "589850,1", "1089330,1", "596700,1", "1073570,1", "725510,2", "537140,1", "17570,16", "966070,1", "457710,1", "644400,12", "1192410,1", "480780,1", "599990,1", "503460,1", "720630,1", "263860,4", "1126970,1", "958640,13", "436870,1", "528260,1", "657140,1", "495480,26", "575430,1", "965960,1", "912300,1", "882790,1", "922450,48", "972200,1", "985680,1", "910320,1" };
			string random_avatar_game = avatars[RandomNext(0, avatars.Length)];
			string avatar_appid = random_avatar_game.Split(',')[0];
			int avatars_num = Convert.ToInt32(random_avatar_game.Split(',')[1]) - 1;
			string avatar_id = RandomNext(0, avatars_num).ToString();
			string avatar_request = "/games/" + avatar_appid + "/selectAvatar";
			Dictionary<string, string> avatar_data = new Dictionary<string, string>(2) {
						{ "selectedAvatar", avatar_id }
					};
			await bot.ArchiWebHandler.UrlPostToHtmlDocumentWithSession(ArchiWebHandler.SteamCommunityURL, avatar_request, avatar_data).ConfigureAwait(false);
			return bot.Commands.FormatBotResponse(ArchiSteamFarm.Localization.Strings.Done);

		}

		public async Task<string?> ResponseRandomAvatar(ulong steamID, string botNames) {
			if ((steamID == 0) || string.IsNullOrEmpty(botNames)) {
				ASF.ArchiLogger.LogNullError(nameof(steamID) + " || " + nameof(botNames));

				return null;
			}

			HashSet<Bot>? bots = Bot.GetBots(botNames);

			if ((bots == null) || (bots.Count == 0)) {
				return ASF.IsOwner(steamID) ? Commands.FormatStaticResponse(string.Format(ArchiSteamFarm.Localization.Strings.BotNotFound, botNames)) : null;
			}

			IList<string?> results = await Utilities.InParallel(bots.Select(curbot => ResponseRandomAvatar(curbot, steamID))).ConfigureAwait(false);

			List<string?> responses = new List<string?>(results.Where(result => !string.IsNullOrEmpty(result)));

			return responses.Count > 0 ? string.Join(Environment.NewLine, responses) : null;
		}

		public async Task<string?> OnBotCommand(Bot bot, ulong steamID, string message, string[] args) {
			switch (args[0].ToUpperInvariant()) {
				case "RANDOMAVATAR" when args.Length > 1:
					return await ResponseRandomAvatar(steamID, Utilities.GetArgsAsText(args, 1, ",")).ConfigureAwait(false);
				case "RANDOMAVATAR":
					return await ResponseRandomAvatar(bot, steamID).ConfigureAwait(false);
				default:
					return null;
			}
		}
		public void OnLoaded() {
			ASF.ArchiLogger.LogGenericInfo("RandomAvatarPlugin by Cobra");
		}
	}
}
