using System;
using System.Collections.Generic;
using System.Composition;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ArchiSteamFarm.Json;
using ArchiSteamFarm.Plugins;
using Newtonsoft.Json.Linq;
using SteamKit2;

namespace ArchiSteamFarm.Cobra.RenamePlugin {
	[Export(typeof(IPlugin))]
	internal sealed class RandomAvatarPlugin : IBotCommand {
		private static readonly Random Random = new Random();
		internal static int RandomNext(int min, int max) {
			lock (Random) {
				return Random.Next(min,max);
			}
		}
		public string Name => nameof(RandomAvatarPlugin);
		public Version Version => typeof(RandomAvatarPlugin).Assembly.GetName().Version;
		public async Task<string> OnBotCommand(Bot bot, ulong steamID, string message, string[] args) {
			switch (args[0].ToUpperInvariant()) {
				case "RANDOMAVATAR" when bot.HasPermission(steamID, BotConfig.EPermission.Master):
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
				default:
					return null;
			}
		}
		public void OnLoaded() {
			ASF.ArchiLogger.LogGenericInfo("RandomAvatarPlugin by Cobra");
		}
	}
}
