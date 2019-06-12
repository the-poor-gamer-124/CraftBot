using CraftBot.Profiles;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace CraftBot.Commands.Features
{
    public partial class MainCommands
    {
        #region Public Methods

        [Command("ehre")]
        [Aliases("honor")]
        public async Task EhreGenommen(CommandContext context, DiscordUser user)
        {
            using (var client = new HttpClient())
            using (var stream = new MemoryStream(await client.GetByteArrayAsync(user.AvatarUrl)))
            using (var avatar = (Bitmap)Image.FromStream(stream))
            using (var outputStream = new MemoryStream())
            {
                using (var bitmap = (Bitmap)Image.FromFile("ehre.png"))
                {
                    using (var graphics = Graphics.FromImage(bitmap))
                    {
                        graphics.DrawImage(avatar, new Rectangle(85, 85, 65, 65));
                    }

                    bitmap.Save(outputStream, ImageFormat.Png);
                }
                await context.RespondWithFileAsync("ehre.png", outputStream, string.Format(await context.GetStringAsync("Ehre_Message"), user.Mention));
            }
        }

        #endregion Public Methods
    }
}