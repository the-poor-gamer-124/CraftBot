using DSharpPlus;
using System;
using System.Collections.Generic;
using System.Text;

namespace CraftBot.Helper
{
    public static class ProgressEmoji
    {
        public static string MakeProgressBar(this double progress, DiscordClient client, int length)
        {
            double characterProgress = progress * length;
            string output = "";

            int wholeCharacters = (int)(characterProgress / 1);
            for (int i = 0; i < wholeCharacters; i++)
            {
                output += "progress-100".GetEmojiString(client);
            }

            double unfinishedProgress = characterProgress - wholeCharacters;

            if (unfinishedProgress != 0)
            {
                if (unfinishedProgress >= 0.75)
                {
                    output += "progress-75".GetEmojiString(client);
                }
                else if (unfinishedProgress >= 0.50)
                {
                    output += "progress-50".GetEmojiString(client);
                }
                else if (unfinishedProgress >= 0.25)
                {
                    output += "progress-25".GetEmojiString(client);
                }
            }

            for (int i = 0; i < length - (characterProgress + unfinishedProgress); i++)
            {
                output += "progress-0".GetEmojiString(client);
            }

            return output;
        }
    }
}
