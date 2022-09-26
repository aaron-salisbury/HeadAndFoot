using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml;

namespace HeadAndFoot.Core.Data
{
    internal static class CRUD
    {
        private const string SPRITES_XML_FILEPATH = "HeadAndFoot.Core.Data.TexturePackerSprites.xml";

        public static Dictionary<string, Image> ReadSprites()
        {
            // www.codeproject.com/tips/451129/csharp-spritesheet-reader-parser

            XmlDocument doc = new XmlDocument();
            doc.Load(new StringReader(GetEmbeddedResourceText(SPRITES_XML_FILEPATH)));

            XmlNode metadata = doc.SelectSingleNode("/plist/dict/key[.='metadata']");
            XmlNode realTextureFileName = metadata.NextSibling.SelectSingleNode("key[.='realTextureFileName']");
            string spritesheetName = realTextureFileName.NextSibling.InnerText;
            Image spriteSheetImage = Image.FromFile(spritesheetName);
            XmlNode frames = doc.SelectSingleNode("/plist/dict/key[.='frames']");
            XmlNodeList list = frames.NextSibling.SelectNodes("key");

            Dictionary<string, Image> sprites = new Dictionary<string, Image>(list.Count);

            foreach (XmlNode node in list)
            {
                XmlNode dict = node.NextSibling;
                string strRectangle = dict.SelectSingleNode("key[.='frame']").NextSibling.InnerText;
                string strOffset = dict.SelectSingleNode("key[.='offset']").NextSibling.InnerText;
                string strSourceRect = dict.SelectSingleNode("key[.='sourceColorRect']").NextSibling.InnerText;
                string strSourceSize = dict.SelectSingleNode("key[.='sourceSize']").NextSibling.InnerText;

                Rectangle frame = ParseRectangle(strRectangle);
                Point offset = ParsePoint(strOffset);
                Rectangle sourceRectangle = ParseRectangle(strSourceRect);
                Point size = ParsePoint(strSourceSize);

                Image sprite = new Bitmap(size.X, size.Y);
                Graphics drawer = Graphics.FromImage(sprite);
                drawer.DrawImage(spriteSheetImage, sourceRectangle, frame, GraphicsUnit.Pixel);
                drawer.Save();
                drawer.Dispose();

                sprites[node.InnerText] = sprite;
            }

            return sprites;
        }

        private static Rectangle ParseRectangle(string rectangle)
        {
            Regex expression = new Regex(@"\{\{(\d+),(\d+)\},\{(\d+),(\d+)\}\}");
            Match match = expression.Match(rectangle);

            if (match.Success)
            {
                int x = int.Parse(match.Groups[1].Value);
                int y = int.Parse(match.Groups[2].Value);
                int w = int.Parse(match.Groups[3].Value);
                int h = int.Parse(match.Groups[4].Value);

                return new Rectangle(x, y, w, h);
            }

            return Rectangle.Empty;
        }

        private static Point ParsePoint(string point)
        {
            Regex expression = new Regex(@"\{(\d+),(\d+)\}");
            Match match = expression.Match(point);

            if (match.Success)
            {
                int x = int.Parse(match.Groups[1].Value);
                int y = int.Parse(match.Groups[2].Value);

                return new Point(x, y);
            }

            return Point.Empty;
        }

        private static string GetEmbeddedResourceText(string filename)
        {
            string result = string.Empty;

            using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(filename))
            using (StreamReader streamReader = new StreamReader(stream))
            {
                result = streamReader.ReadToEnd();
            }

            return result;
        }
    }
}
