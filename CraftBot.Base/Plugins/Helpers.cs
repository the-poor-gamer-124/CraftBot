using CraftBot.Helper;

using DSharpPlus;
using DSharpPlus.CommandsNext.Attributes;

namespace CraftBot.Base.Plugins
{
    public static class Helpers
    {
        /// <summary>
        /// Converts a <see cref="CheckBaseAttribute"/> to a human-readable format.
        /// </summary>
        /// <param name="attribute">The attribute to describe.</param>
        /// <returns>A description of <paramref name="attribute"/></returns>
        public static string ToDescription(this CheckBaseAttribute attribute)
        {
            //TODO: Add localization support.
#pragma warning disable IDE0011 // Add braces
            switch (attribute)
            {
                case RequireBotPermissionsAttribute botPermissionsAttribute:
                    return "Requires the bot to have the following permissions" + botPermissionsAttribute.Permissions.ToPermissionString();
                case RequireDirectMessageAttribute _:
                    return "Requires to be run inside direct messages";
                case RequireGuildAttribute _:
                    return "Requires to be run inside a server";
                case RequireNsfwAttribute _:
                    return "Requires to be run inside a NSFW channel";
                case RequireOwnerAttribute _:
                case RequireUserGroupAttribute _:
                    return "Requires to be run by the owner of this bot";
                case RequirePermissionsAttribute permissionsAttribute:
                    return "Requires the following permissions: " + permissionsAttribute.Permissions.ToPermissionString();
                case RequirePrefixesAttribute prefixesAttribute:
                    return "Requires to be run with one of following prefixes: " + string.Join(", ", prefixesAttribute.Prefixes);
                case RequireRolesAttribute rolesAttribute:
                    return "Requires the user to have following roles: " + string.Join(", ", rolesAttribute.RoleNames);
                case RequireUserPermissionsAttribute userPermissionsAttribute:
                    return "Requires the user to have following permissions: " + userPermissionsAttribute.Permissions.ToPermissionString();
            }
#pragma warning restore IDE0011 // Add braces

            return "Description not available";
        }
    }
}