<#
.SYNOPSIS
    Generates the original Head and Foot card sprite sheet.

.DESCRIPTION
    Renders every card face, both jokers, and the card back with GDI+ and saves them
    as one PNG sprite sheet. The art is drawn entirely from code in this script; it is
    styled after the small, bold, 16-colour look of late-1990s desktop card games
    without copying any existing card artwork.

    Sheet layout (each cell is 71 x 96 pixels, the classic desktop card size):
      Rows 0-3: Clubs, Diamonds, Hearts, Spades.
      Columns 0-12: Ace, 2, 3, 4, 5, 6, 7, 8, 9, 10, Jack, Queen, King.
      Row 4: column 0 red joker, column 1 black joker, column 2 card back.

.EXAMPLE
    powershell -ExecutionPolicy Bypass -File tools\CardArt\New-CardArt.ps1
#>
param(
    [string]$OutputPath = (Join-Path $PSScriptRoot '..\..\src\HeadAndFoot.DesktopApp\Assets\Cards\cards.png')
)

$ErrorActionPreference = 'Stop'

Add-Type -ReferencedAssemblies System.Drawing -TypeDefinition @'
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;

public static class CardArtGenerator
{
    public const int CARD_WIDTH = 71;
    public const int CARD_HEIGHT = 96;

    private static readonly Color RED = Color.FromArgb(255, 0, 0);
    private static readonly Color BLACK = Color.FromArgb(0, 0, 0);
    private static readonly Color NAVY = Color.FromArgb(0, 0, 128);
    private static readonly Color BLUE = Color.FromArgb(0, 0, 255);
    private static readonly Color YELLOW = Color.FromArgb(255, 255, 0);
    private static readonly Color GOLD = Color.FromArgb(255, 204, 0);
    private static readonly Color SKIN = Color.FromArgb(255, 220, 168);
    private static readonly Color FRAME_FILL = Color.FromArgb(255, 255, 230);

    private static readonly string[] RANK_TEXT = new string[] { "A", "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K" };

    // Suit order for rows: 0 clubs, 1 diamonds, 2 hearts, 3 spades.
    public static void Generate(string path)
    {
        using (Bitmap sheet = new Bitmap(CARD_WIDTH * 13, CARD_HEIGHT * 5, PixelFormat.Format32bppArgb))
        {
            using (Graphics g = Graphics.FromImage(sheet))
            {
                g.Clear(Color.Transparent);

                for (int suit = 0; suit < 4; suit++)
                {
                    for (int rank = 0; rank < 13; rank++)
                    {
                        using (Bitmap card = DrawFace(suit, rank))
                        {
                            g.DrawImageUnscaled(card, rank * CARD_WIDTH, suit * CARD_HEIGHT);
                        }
                    }
                }

                using (Bitmap card = DrawJoker(true))
                {
                    g.DrawImageUnscaled(card, 0, 4 * CARD_HEIGHT);
                }

                using (Bitmap card = DrawJoker(false))
                {
                    g.DrawImageUnscaled(card, CARD_WIDTH, 4 * CARD_HEIGHT);
                }

                using (Bitmap card = DrawBack())
                {
                    g.DrawImageUnscaled(card, 2 * CARD_WIDTH, 4 * CARD_HEIGHT);
                }
            }

            sheet.Save(path, ImageFormat.Png);
        }
    }

    private static bool IsRed(int suit)
    {
        return suit == 1 || suit == 2;
    }

    private static Bitmap NewCard(Color fill)
    {
        Bitmap bitmap = new Bitmap(CARD_WIDTH, CARD_HEIGHT, PixelFormat.Format32bppArgb);
        using (Graphics g = Graphics.FromImage(bitmap))
        {
            g.Clear(Color.Transparent);
            g.SmoothingMode = SmoothingMode.None;
            using (GraphicsPath outline = RoundedRectangle(new Rectangle(0, 0, CARD_WIDTH - 1, CARD_HEIGHT - 1), 3))
            {
                using (SolidBrush brush = new SolidBrush(fill))
                {
                    g.FillPath(brush, outline);
                }

                g.DrawPath(Pens.Black, outline);
            }
        }

        return bitmap;
    }

    private static GraphicsPath RoundedRectangle(Rectangle bounds, int radius)
    {
        int diameter = radius * 2;
        GraphicsPath path = new GraphicsPath();
        path.AddArc(bounds.Left, bounds.Top, diameter, diameter, 180, 90);
        path.AddArc(bounds.Right - diameter, bounds.Top, diameter, diameter, 270, 90);
        path.AddArc(bounds.Right - diameter, bounds.Bottom - diameter, diameter, diameter, 0, 90);
        path.AddArc(bounds.Left, bounds.Bottom - diameter, diameter, diameter, 90, 90);
        path.CloseFigure();
        return path;
    }

    private static Bitmap DrawFace(int suit, int rank)
    {
        Bitmap bitmap = NewCard(Color.White);
        Color ink = IsRed(suit) ? RED : BLACK;

        using (Graphics g = Graphics.FromImage(bitmap))
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = TextRenderingHint.SingleBitPerPixelGridFit;

            DrawIndex(g, RANK_TEXT[rank], suit, ink, false);
            DrawRotated(g, delegate(Graphics rotated) { DrawIndex(rotated, RANK_TEXT[rank], suit, ink, false); });

            if (rank == 0)
            {
                DrawAce(g, suit);
            }
            else if (rank >= 10)
            {
                DrawCourt(g, suit, rank);
            }
            else
            {
                DrawPips(g, suit, rank + 1);
            }
        }

        return bitmap;
    }

    private delegate void Painter(Graphics g);

    private static void DrawRotated(Graphics g, Painter painter)
    {
        GraphicsState state = g.Save();
        g.TranslateTransform(CARD_WIDTH, CARD_HEIGHT);
        g.RotateTransform(180);
        painter(g);
        g.Restore(state);
    }

    private static void DrawIndex(Graphics g, string text, int suit, Color ink, bool joker)
    {
        using (Font font = new Font("Arial", 9f, FontStyle.Bold, GraphicsUnit.Pixel))
        {
            using (SolidBrush brush = new SolidBrush(ink))
            {
                if (text == "10")
                {
                    // Squeeze the two digits together so "10" fits the narrow index column.
                    g.DrawString("1", font, brush, 0f, 2f);
                    g.DrawString("0", font, brush, 5f, 2f);
                }
                else
                {
                    using (StringFormat format = new StringFormat())
                    {
                        format.Alignment = StringAlignment.Center;
                        g.DrawString(text, font, brush, new RectangleF(1f, 2f, 12f, 12f), format);
                    }
                }
            }
        }

        if (!joker)
        {
            DrawPip(g, suit, new PointF(7f, 18.5f), 7f);
        }
    }

    private static void DrawPip(Graphics g, int suit, PointF center, float size)
    {
        using (GraphicsPath path = PipPath(suit, center, size))
        {
            using (SolidBrush brush = new SolidBrush(IsRed(suit) ? RED : BLACK))
            {
                g.FillPath(brush, path);
            }
        }
    }

    private static GraphicsPath PipPath(int suit, PointF c, float size)
    {
        float h = size;
        float w = size * 0.92f;
        float left = c.X - w / 2f;
        float top = c.Y - h / 2f;
        GraphicsPath path = new GraphicsPath();

        switch (suit)
        {
            case 1: // Diamond
                path.AddPolygon(new PointF[]
                {
                    new PointF(c.X, top),
                    new PointF(c.X + w * 0.40f, c.Y),
                    new PointF(c.X, top + h),
                    new PointF(c.X - w * 0.40f, c.Y)
                });
                break;
            case 2: // Heart
                path.AddBezier(c.X, top + h, c.X - w * 0.15f, top + h * 0.80f, left, top + h * 0.55f, left, top + h * 0.30f);
                path.AddBezier(left, top + h * 0.30f, left, top - h * 0.02f, c.X - w * 0.05f, top - h * 0.02f, c.X, top + h * 0.22f);
                path.AddBezier(c.X, top + h * 0.22f, c.X + w * 0.05f, top - h * 0.02f, left + w, top - h * 0.02f, left + w, top + h * 0.30f);
                path.AddBezier(left + w, top + h * 0.30f, left + w, top + h * 0.55f, c.X + w * 0.15f, top + h * 0.80f, c.X, top + h);
                path.CloseFigure();
                break;
            case 3: // Spade
                float bodyBottom = top + h * 0.78f;
                path.AddBezier(c.X, top, c.X - w * 0.15f, top + h * 0.22f, left, top + h * 0.40f, left, top + h * 0.58f);
                path.AddBezier(left, top + h * 0.58f, left, bodyBottom, c.X - w * 0.08f, bodyBottom, c.X, top + h * 0.62f);
                path.AddBezier(c.X, top + h * 0.62f, c.X + w * 0.08f, bodyBottom, left + w, bodyBottom, left + w, top + h * 0.58f);
                path.AddBezier(left + w, top + h * 0.58f, left + w, top + h * 0.40f, c.X + w * 0.15f, top + h * 0.22f, c.X, top);
                path.CloseFigure();
                path.AddPolygon(new PointF[]
                {
                    new PointF(c.X, top + h * 0.62f),
                    new PointF(c.X + w * 0.20f, top + h),
                    new PointF(c.X - w * 0.20f, top + h)
                });
                break;
            default: // Club
                float r = w * 0.25f;
                path.AddEllipse(c.X - r, top, r * 2f, r * 2f);
                path.AddEllipse(left + w * 0.02f, top + h * 0.33f, r * 2f, r * 2f);
                path.AddEllipse(left + w * 0.98f - r * 2f, top + h * 0.33f, r * 2f, r * 2f);
                path.AddEllipse(c.X - r * 0.6f, top + h * 0.30f, r * 1.2f, r * 1.2f);
                path.AddPolygon(new PointF[]
                {
                    new PointF(c.X, top + h * 0.45f),
                    new PointF(c.X + w * 0.20f, top + h),
                    new PointF(c.X - w * 0.20f, top + h)
                });
                break;
        }

        path.FillMode = FillMode.Winding;
        return path;
    }

    private static void DrawAce(Graphics g, int suit)
    {
        PointF center = new PointF(CARD_WIDTH / 2f, CARD_HEIGHT / 2f);

        if (suit == 3)
        {
            // The ace of spades gets a framed, larger pip, a nod to the traditional ornate ace.
            using (Pen pen = new Pen(NAVY, 1f))
            {
                g.DrawEllipse(pen, center.X - 19f, center.Y - 19f, 38f, 38f);
            }

            DrawPip(g, suit, center, 28f);
            return;
        }

        DrawPip(g, suit, center, 20f);
    }

    private static void DrawPips(Graphics g, int suit, int count)
    {
        const float L = 21f;
        const float M = 35.5f;
        const float R = 50f;
        const float TOP = 17f;
        const float BOTTOM = 79f;
        const float MID = 48f;
        float upperThird = TOP + (BOTTOM - TOP) / 3f;
        float lowerThird = TOP + (BOTTOM - TOP) * 2f / 3f;

        PointF[] points;
        switch (count)
        {
            case 2:
                points = new PointF[] { P(M, TOP), P(M, BOTTOM) };
                break;
            case 3:
                points = new PointF[] { P(M, TOP), P(M, MID), P(M, BOTTOM) };
                break;
            case 4:
                points = new PointF[] { P(L, TOP), P(R, TOP), P(L, BOTTOM), P(R, BOTTOM) };
                break;
            case 5:
                points = new PointF[] { P(L, TOP), P(R, TOP), P(M, MID), P(L, BOTTOM), P(R, BOTTOM) };
                break;
            case 6:
                points = new PointF[] { P(L, TOP), P(R, TOP), P(L, MID), P(R, MID), P(L, BOTTOM), P(R, BOTTOM) };
                break;
            case 7:
                points = new PointF[] { P(L, TOP), P(R, TOP), P(M, (TOP + MID) / 2f), P(L, MID), P(R, MID), P(L, BOTTOM), P(R, BOTTOM) };
                break;
            case 8:
                points = new PointF[] { P(L, TOP), P(R, TOP), P(M, (TOP + MID) / 2f), P(L, MID), P(R, MID), P(M, (MID + BOTTOM) / 2f), P(L, BOTTOM), P(R, BOTTOM) };
                break;
            case 9:
                points = new PointF[] { P(L, TOP), P(R, TOP), P(L, upperThird), P(R, upperThird), P(M, MID), P(L, lowerThird), P(R, lowerThird), P(L, BOTTOM), P(R, BOTTOM) };
                break;
            default:
                points = new PointF[] { P(L, TOP), P(R, TOP), P(M, (TOP + upperThird) / 2f), P(L, upperThird), P(R, upperThird), P(L, lowerThird), P(R, lowerThird), P(M, (lowerThird + BOTTOM) / 2f), P(L, BOTTOM), P(R, BOTTOM) };
                break;
        }

        foreach (PointF point in points)
        {
            if (point.Y > MID + 0.5f)
            {
                // Pips below the middle face the other way, as on a printed card.
                PointF mirrored = new PointF(CARD_WIDTH - point.X, CARD_HEIGHT - point.Y);
                DrawRotated(g, delegate(Graphics rotated) { DrawPip(rotated, suit, mirrored, 13f); });
            }
            else
            {
                DrawPip(g, suit, point, 13f);
            }
        }
    }

    private static PointF P(float x, float y)
    {
        return new PointF(x, y);
    }

    private static void DrawCourt(Graphics g, int suit, int rank)
    {
        Rectangle frame = new Rectangle(14, 10, 43, 76);
        using (SolidBrush fill = new SolidBrush(FRAME_FILL))
        {
            g.FillRectangle(fill, frame);
        }

        Region previousClip = g.Clip;
        g.SetClip(new Rectangle(frame.X, frame.Y, frame.Width, frame.Height / 2));
        DrawCourtHalf(g, suit, rank, frame);
        g.Clip = previousClip;

        GraphicsState state = g.Save();
        g.TranslateTransform(CARD_WIDTH, CARD_HEIGHT);
        g.RotateTransform(180);
        Rectangle mirroredFrame = new Rectangle(CARD_WIDTH - frame.Right, CARD_HEIGHT - frame.Bottom, frame.Width, frame.Height);
        g.SetClip(new Rectangle(mirroredFrame.X, mirroredFrame.Y, mirroredFrame.Width, mirroredFrame.Height / 2));
        DrawCourtHalf(g, suit, rank, mirroredFrame);
        g.Restore(state);

        using (Pen divider = new Pen(IsRed(suit) ? RED : NAVY, 1f))
        {
            g.DrawLine(divider, frame.Left, frame.Top + frame.Height / 2f, frame.Right, frame.Top + frame.Height / 2f);
        }

        using (Pen border = new Pen(IsRed(suit) ? RED : NAVY, 1f))
        {
            g.SmoothingMode = SmoothingMode.None;
            g.DrawRectangle(border, frame);
            g.SmoothingMode = SmoothingMode.AntiAlias;
        }
    }

    private static void DrawCourtHalf(Graphics g, int suit, int rank, Rectangle frame)
    {
        bool red = IsRed(suit);
        Color robe = red ? RED : NAVY;
        Color trim = red ? NAVY : RED;
        float cx = frame.Left + frame.Width / 2f + 2f;
        float top = frame.Top;

        // Robe and shoulders.
        using (SolidBrush brush = new SolidBrush(robe))
        {
            g.FillPolygon(brush, new PointF[]
            {
                new PointF(cx - 18f, top + 38f),
                new PointF(cx - 13f, top + 27f),
                new PointF(cx + 13f, top + 27f),
                new PointF(cx + 18f, top + 38f)
            });
        }

        using (SolidBrush brush = new SolidBrush(GOLD))
        {
            g.FillPolygon(brush, new PointF[]
            {
                new PointF(cx - 3f, top + 27f),
                new PointF(cx + 3f, top + 27f),
                new PointF(cx + 5f, top + 38f),
                new PointF(cx - 5f, top + 38f)
            });
        }

        using (Pen pen = new Pen(trim, 1.5f))
        {
            g.DrawLine(pen, cx - 13f, top + 27f, cx + 13f, top + 27f);
            g.DrawLine(pen, cx - 1f, top + 29f, cx - 1f, top + 38f);
            g.DrawLine(pen, cx + 1f, top + 29f, cx + 1f, top + 38f);
        }

        // Hair behind the face.
        Color hair = rank == 11 ? GOLD : Color.FromArgb(128, 64, 0);
        using (SolidBrush brush = new SolidBrush(hair))
        {
            if (rank == 11)
            {
                g.FillEllipse(brush, cx - 10f, top + 9f, 20f, 26f);
            }
            else
            {
                g.FillEllipse(brush, cx - 8f, top + 11f, 16f, 12f);
            }
        }

        // Face.
        using (SolidBrush brush = new SolidBrush(SKIN))
        {
            g.FillEllipse(brush, cx - 6f, top + 12f, 12f, 14f);
        }

        g.DrawEllipse(Pens.Black, cx - 6f, top + 12f, 12f, 14f);
        g.FillRectangle(Brushes.Black, cx - 3f, top + 17f, 1.5f, 1.5f);
        g.FillRectangle(Brushes.Black, cx + 2f, top + 17f, 1.5f, 1.5f);
        using (Pen mouth = new Pen(RED, 1f))
        {
            g.DrawLine(mouth, cx - 2f, top + 22f, cx + 2f, top + 22f);
        }

        if (rank == 12)
        {
            // King's beard.
            using (SolidBrush brush = new SolidBrush(Color.FromArgb(128, 128, 128)))
            {
                g.FillPolygon(brush, new PointF[]
                {
                    new PointF(cx - 6f, top + 21f),
                    new PointF(cx + 6f, top + 21f),
                    new PointF(cx, top + 30f)
                });
            }
        }

        // Headwear: a pointed crown for the king, a rounded crown for the queen, a feathered cap for the jack.
        using (SolidBrush gold = new SolidBrush(YELLOW))
        {
            if (rank == 12)
            {
                PointF[] crown = new PointF[]
                {
                    new PointF(cx - 8f, top + 13f), new PointF(cx - 8f, top + 4f), new PointF(cx - 4f, top + 9f),
                    new PointF(cx, top + 2f), new PointF(cx + 4f, top + 9f), new PointF(cx + 8f, top + 4f), new PointF(cx + 8f, top + 13f)
                };
                g.FillPolygon(gold, crown);
                g.DrawPolygon(Pens.Black, crown);
                using (SolidBrush jewel = new SolidBrush(trim))
                {
                    g.FillEllipse(jewel, cx - 1.5f, top + 9f, 3f, 3f);
                }
            }
            else if (rank == 11)
            {
                g.FillPie(gold, cx - 8f, top + 4f, 16f, 16f, 180f, 180f);
                g.DrawArc(Pens.Black, cx - 8f, top + 4f, 16f, 16f, 180f, 180f);
                using (SolidBrush jewel = new SolidBrush(trim))
                {
                    g.FillEllipse(jewel, cx - 1.5f, top + 6f, 3f, 3f);
                }
            }
            else
            {
                using (SolidBrush cap = new SolidBrush(trim))
                {
                    g.FillEllipse(cap, cx - 10f, top + 6f, 20f, 8f);
                }

                g.DrawEllipse(Pens.Black, cx - 10f, top + 6f, 20f, 8f);
                using (Pen feather = new Pen(GOLD, 2f))
                {
                    g.DrawBezier(feather, cx + 4f, top + 8f, cx + 10f, top + 2f, cx + 14f, top + 3f, cx + 16f, top + 7f);
                }
            }
        }

        // A small suit pip beside the figure ties the court card to its suit.
        DrawPip(g, suit, new PointF(frame.Left + 5f, frame.Top + 6f), 7f);
    }

    private static Bitmap DrawJoker(bool red)
    {
        Bitmap bitmap = NewCard(Color.White);
        Color ink = red ? RED : BLACK;

        using (Graphics g = Graphics.FromImage(bitmap))
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = TextRenderingHint.SingleBitPerPixelGridFit;

            DrawJokerIndex(g, ink);
            DrawRotated(g, delegate(Graphics rotated) { DrawJokerIndex(rotated, ink); });

            float cx = CARD_WIDTH / 2f;
            float cy = CARD_HEIGHT / 2f;
            Color left = red ? RED : NAVY;
            Color right = red ? NAVY : RED;

            // Three-pointed jester hat.
            using (SolidBrush brush = new SolidBrush(left))
            {
                g.FillPolygon(brush, new PointF[] { new PointF(cx - 13f, cy - 6f), new PointF(cx - 20f, cy - 22f), new PointF(cx - 2f, cy - 9f) });
            }

            using (SolidBrush brush = new SolidBrush(GOLD))
            {
                g.FillPolygon(brush, new PointF[] { new PointF(cx - 5f, cy - 8f), new PointF(cx, cy - 28f), new PointF(cx + 5f, cy - 8f) });
            }

            using (SolidBrush brush = new SolidBrush(right))
            {
                g.FillPolygon(brush, new PointF[] { new PointF(cx + 2f, cy - 9f), new PointF(cx + 20f, cy - 22f), new PointF(cx + 13f, cy - 6f) });
            }

            using (SolidBrush bell = new SolidBrush(YELLOW))
            {
                g.FillEllipse(bell, cx - 23f, cy - 25f, 6f, 6f);
                g.FillEllipse(bell, cx - 3f, cy - 32f, 6f, 6f);
                g.FillEllipse(bell, cx + 17f, cy - 25f, 6f, 6f);
            }

            g.DrawEllipse(Pens.Black, cx - 23f, cy - 25f, 6f, 6f);
            g.DrawEllipse(Pens.Black, cx - 3f, cy - 32f, 6f, 6f);
            g.DrawEllipse(Pens.Black, cx + 17f, cy - 25f, 6f, 6f);

            // Face.
            using (SolidBrush brush = new SolidBrush(SKIN))
            {
                g.FillEllipse(brush, cx - 10f, cy - 10f, 20f, 22f);
            }

            g.DrawEllipse(Pens.Black, cx - 10f, cy - 10f, 20f, 22f);
            g.FillEllipse(Brushes.Black, cx - 5f, cy - 3f, 3f, 3f);
            g.FillEllipse(Brushes.Black, cx + 2f, cy - 3f, 3f, 3f);
            using (Pen smile = new Pen(RED, 1.5f))
            {
                g.DrawArc(smile, cx - 6f, cy - 2f, 12f, 9f, 20f, 140f);
            }

            // Ruffled collar.
            for (int i = 0; i < 5; i++)
            {
                Color point = i % 2 == 0 ? left : right;
                using (SolidBrush brush = new SolidBrush(point))
                {
                    float x = cx - 15f + i * 6f;
                    g.FillPolygon(brush, new PointF[] { new PointF(x, cy + 11f), new PointF(x + 6f, cy + 11f), new PointF(x + 3f, cy + 20f) });
                }
            }
        }

        return bitmap;
    }

    private static void DrawJokerIndex(Graphics g, Color ink)
    {
        using (Font font = new Font("Arial", 8f, FontStyle.Bold, GraphicsUnit.Pixel))
        {
            using (SolidBrush brush = new SolidBrush(ink))
            {
                using (StringFormat format = new StringFormat())
                {
                    format.Alignment = StringAlignment.Center;
                    string letters = "JOKER";
                    for (int i = 0; i < letters.Length; i++)
                    {
                        g.DrawString(letters[i].ToString(), font, brush, new RectangleF(1f, 2f + i * 8f, 11f, 9f), format);
                    }
                }
            }
        }
    }

    private static Bitmap DrawBack()
    {
        Bitmap bitmap = NewCard(Color.White);

        using (Graphics g = Graphics.FromImage(bitmap))
        {
            g.SmoothingMode = SmoothingMode.None;
            Rectangle field = new Rectangle(4, 4, CARD_WIDTH - 8, CARD_HEIGHT - 8);

            using (SolidBrush brush = new SolidBrush(NAVY))
            {
                g.FillRectangle(brush, field);
            }

            g.SetClip(field);

            // A diamond lattice with a dot in each cell.
            using (Pen lattice = new Pen(Color.FromArgb(32, 80, 255), 1f))
            {
                for (int offset = -CARD_HEIGHT; offset < CARD_WIDTH + CARD_HEIGHT; offset += 8)
                {
                    g.DrawLine(lattice, offset, 0, offset + CARD_HEIGHT, CARD_HEIGHT);
                    g.DrawLine(lattice, offset, CARD_HEIGHT, offset + CARD_HEIGHT, 0);
                }
            }

            using (SolidBrush dot = new SolidBrush(Color.FromArgb(0, 255, 255)))
            {
                // Lattice cells are centred where x and y differ by 4 (mod 8) along both diagonals.
                for (int y = 0; y < CARD_HEIGHT; y += 4)
                {
                    int startX = (y / 4) % 2 == 0 ? 4 : 0;
                    for (int x = startX; x < CARD_WIDTH; x += 8)
                    {
                        g.FillRectangle(dot, x, y, 1, 1);
                    }
                }
            }

            g.ResetClip();

            // Inner keyline and a centred yellow medallion.
            g.DrawRectangle(Pens.White, field.X + 2, field.Y + 2, field.Width - 5, field.Height - 5);

            PointF c = new PointF(CARD_WIDTH / 2f, CARD_HEIGHT / 2f);
            PointF[] medallion = new PointF[]
            {
                new PointF(c.X, c.Y - 13f), new PointF(c.X + 10f, c.Y), new PointF(c.X, c.Y + 13f), new PointF(c.X - 10f, c.Y)
            };
            using (SolidBrush brush = new SolidBrush(YELLOW))
            {
                g.FillPolygon(brush, medallion);
            }

            g.DrawPolygon(Pens.White, medallion);
            using (SolidBrush brush = new SolidBrush(RED))
            {
                g.FillPolygon(brush, new PointF[]
                {
                    new PointF(c.X, c.Y - 6f), new PointF(c.X + 4.5f, c.Y), new PointF(c.X, c.Y + 6f), new PointF(c.X - 4.5f, c.Y)
                });
            }
        }

        return bitmap;
    }
}
'@

$resolved = [System.IO.Path]::GetFullPath($OutputPath)
$directory = [System.IO.Path]::GetDirectoryName($resolved)
if (-not (Test-Path $directory))
{
    New-Item -ItemType Directory -Path $directory | Out-Null
}

[CardArtGenerator]::Generate($resolved)
Write-Output "Card sheet written to $resolved"
