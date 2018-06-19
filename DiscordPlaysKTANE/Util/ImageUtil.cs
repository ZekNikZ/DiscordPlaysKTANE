using System;
using System.IO;
using Svg;
using System.Drawing;
namespace DiscordPlaysKTANE {
    public static class ImageUtil {
        public static readonly SvgColourServer COLOR_WHITE = new SvgColourServer(Color.White);
        public static readonly SvgColourServer COLOR_BLACK = new SvgColourServer(Color.Black);
        public static readonly SvgColourServer COLOR_GRAY = new SvgColourServer(Color.LightGray);
        public static readonly SvgColourServer COLOR_SL_GREEN = new SvgColourServer(Color.Green);
        public static readonly SvgColourServer COLOR_SL_RED = new SvgColourServer(Color.Red);
        public static readonly SvgColourServer COLOR_SL_BLACK = new SvgColourServer(Color.Black);

        public static SvgDocument LoadModuleTemplate(string moduleName) {
            string path = String.Format(Constants.ImagePath, moduleName);
            SvgDocument template = SvgDocument.Open(path);
            if (template != null) {
                return template;
            } else {
                throw new FileNotFoundException();
            }
        }
    }
}
