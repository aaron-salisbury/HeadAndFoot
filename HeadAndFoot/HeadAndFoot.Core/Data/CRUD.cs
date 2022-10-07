using HeadAndFoot.Core.Base.Logging;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Xml;

namespace HeadAndFoot.Core.Data
{
    internal static class CRUD
    {
        private const string SPRITE_ELEMENT_NAME = "sprite";
        private const string CARD_BACKS_FILEPATH = "HeadAndFoot.Core.Data.Textures.cards-backs-data";

        public static Dictionary<string, System.Drawing.Image> ReadCardBackSprites()
        {
            // Inspired by www.codeproject.com/tips/451129/csharp-spritesheet-reader-parser

            try
            {
                Dictionary<string, System.Drawing.Image> sprites = new Dictionary<string, System.Drawing.Image>();

                string imagePath = string.Format("{0}.png", CARD_BACKS_FILEPATH);
                string xmlPath = string.Format("{0}.xml", CARD_BACKS_FILEPATH);

                System.Drawing.Image spriteSheetImage = System.Drawing.Image.FromStream(GetResourceStream(imagePath));

                XmlTextReader reader = new XmlTextReader(new StringReader(GetEmbeddedResourceText(xmlPath)));
                reader.ReadToFollowing(SPRITE_ELEMENT_NAME);

                do
                {
                    string name = reader.GetAttribute("name");
                    string xCoord = reader.GetAttribute("x");
                    string yCoord = reader.GetAttribute("y");
                    string w = reader.GetAttribute("w");
                    string h = reader.GetAttribute("h");

                    int x;
                    int y;
                    int width;
                    int height;
                    if (int.TryParse(xCoord, out x) && int.TryParse(yCoord, out y) && int.TryParse(w, out width) && int.TryParse(h, out height))
                    {
                        System.Drawing.Image sprite = new Bitmap(width, height);
                        Graphics drawer = Graphics.FromImage(sprite);
                        Rectangle destRect = new Rectangle(0, 0, width, height);
                        Rectangle srcRect = new Rectangle(x, y, width, height);
                        drawer.DrawImage(spriteSheetImage, destRect, srcRect, GraphicsUnit.Pixel);
                        drawer.Save();
                        drawer.Dispose();
                        sprites[name] = sprite;
                    }
                } while (reader.ReadToFollowing(SPRITE_ELEMENT_NAME));

                return sprites;
            }
            catch (Exception e)
            {
                AppLogger.Write(string.Format("Failed to retrieve card back sprites: {0}", e.Message), AppLogger.LogCategories.Error);
                return null;
            }
        }

        private static Stream GetResourceStream(string filename)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            Stream resourceStream = assembly.GetManifestResourceStream(filename);

            return resourceStream;
        }

        private static string GetEmbeddedResourceText(string filename)
        {
            string result = string.Empty;

            using (Stream stream = GetResourceStream(filename))
            using (StreamReader streamReader = new StreamReader(stream))
            {
                result = streamReader.ReadToEnd();
            }

            return result;
        }
    }
}
