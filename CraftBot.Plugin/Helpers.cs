using DSharpPlus;
using DSharpPlus.CommandsNext.Attributes;

namespace CraftBot.Plugin
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
            #pragma warning disable IDE0011 // Add braces
            if (attribute is RequireBotPermissionsAttribute botPermissionsAttribute) return "Requires the bot to have the following permissions" + botPermissionsAttribute.Permissions.ToPermissionString();
            else if (attribute is RequireDirectMessageAttribute) return "Requires to be run inside direct messages";
            else if (attribute is RequireGuildAttribute) return "Requires to be run inside a server";
            else if (attribute is RequireNsfwAttribute) return "Requires to be run inside a NSFW channel";
            else if (attribute is RequireOwnerAttribute) return "Requires to be run by the owner of this bot";
            else if (attribute is RequirePermissionsAttribute permissionsAttribute) return "Requires the following permissions: " + permissionsAttribute.Permissions.ToPermissionString();
            else if (attribute is RequirePrefixesAttribute prefixesAttribute) return "Requires to be run with one of following prefixes: " + string.Join(", ", prefixesAttribute.Prefixes);
            else if (attribute is RequireRolesAttribute rolesAttribute) return "Requires the user to have following roles: " + string.Join(", ", rolesAttribute.RoleNames);
            else if (attribute is RequireUserPermissionsAttribute userPermissionsAttribute) return "Requires the user to have following permissions: " + userPermissionsAttribute.Permissions.ToPermissionString();
            #pragma warning restore IDE0011 // Add braces

            return "Description not available";
        }
    }
}
