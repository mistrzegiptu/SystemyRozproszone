using System;
using System.Collections.Generic;
using System.Text;

namespace MessageDTO
{
    public class AsciiUtils
    {
        public static string Create(string asciiArt)
        {
            return asciiArt.ToLower() switch
            {
                "sus" =>
                        "  ____ \n" +
                        " /  _ \\\n" +
                        "|  |_| |\n" +
                        " \\____/\n" +
                        "  || ||",
                "lenny" => "( ͡° ͜ʖ ͡°)",
                "flip" => "(╯°□°）╯︵ ┻━┻",
                "nyan" => "- - - - ~=p,,_,,q:3",
                _ => string.Empty
            };
        }
    }
}
