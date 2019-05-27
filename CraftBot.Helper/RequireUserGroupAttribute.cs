using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System.Threading.Tasks;

namespace CraftBot.Helper
{
    public class RequireUserGroupAttribute : CheckBaseAttribute
    {
        public override async Task<bool> ExecuteCheckAsync(CommandContext ctx, bool help) => ctx.User.Id == 194891941509332992;
    }
}
