/*
Tokenization string 
Author: Thanh Ngoc Dao - Thanh.dao@gmx.net
Copyright (c) 2005 by Thanh Ngoc Dao.
*/

using System;
using System.Collections;
using System.Text.RegularExpressions;

namespace WordsMatching
{
    /// <summary>
    /// Summary description for Tokeniser.
    /// Partition string off into subwords
    /// </summary>
    internal class Tokeniser
    {
        private void Normalize_Casing(ref string input)
        {
            //if it is formed by Pascal/Carmel casing
            for (int i = 0; i < input.Length; i++)
            {
                if (char.IsSeparator(input[i]))
                {
                    input = input.Replace(input[i].ToString(), " ");
                }
            }
            int idx = 1;
            while (idx < input.Length - 2)
            {
                ++idx;
                if (
                    char.IsUpper(input[idx])
                    && char.IsLower(input[idx + 1])
                    &&
                    !char.IsWhiteSpace(input[idx - 1]) && !char.IsSeparator(input[idx - 1])
                    )
                {
                    input = input.Insert(idx, " ");
                    ++idx;
                }
            }
        }

        public string[] Partition(string input)
        {
            var r = new Regex("([ \\t{}():;])");

            this.Normalize_Casing(ref input);
            //normalization to the lower case
            input = input.ToLower();

            string[] tokens = r.Split(input);

            var filter = new ArrayList();

            for (int i = 0; i < tokens.Length; i++)
            {
                MatchCollection mc = r.Matches(tokens[i]);
                if (mc.Count <= 0 && tokens[i].Trim().Length > 0)
                {
                    _ = filter.Add(tokens[i]);
                }
            }

            tokens = new string[filter.Count];
            for (int i = 0; i < filter.Count; i++)
            {
                tokens[i] = (string)filter[i];
            }

            return tokens;
        }


        public Tokeniser()
        {
            //
            // TODO: Add constructor logic here
            //
        }
    }
}
