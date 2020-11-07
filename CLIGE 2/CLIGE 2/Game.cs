using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CLIGE_2
{
    public struct GamePixel
    {
        public string texture;
        public int foreground;
        public int background;
        public int width;

        public GamePixel(int foreground, int background)
        {
            texture = new string(' ', Game.pixelWidth);
            width = Game.pixelWidth;
            if (foreground > 15) { foreground = 15; }
            if (background > 15) { background = 15; }
            this.foreground = foreground;
            this.background = background;
        }

        public GamePixel(int width, int foreground, int background)
        {
            texture = new string(' ', width);
            if (width <= 0) { width = 1; }
            if (foreground > 15) { foreground = 15; }
            if (background > 15) { background = 15; }
            this.foreground = foreground;
            this.background = background;
            this.width = width;
        }

        public GamePixel(string texture, int foreground, int background)
        {
            this.texture = texture;
            if (foreground > 15) { foreground = 15; }
            if (background > 15) { background = 15; }
            this.foreground = foreground;
            this.background = background;
            width = texture.Length;
        }

        public Pixel[] ConvertToPixels()
        {
            Pixel[] pixels = new Pixel[width];

            for (int p = 0; p < width; p++)
            {
                pixels[p] = new Pixel(this, p);
            }

            return pixels;
        }
    }

    public class Game
    {
        public static GamePixel[][] grid;
        public static GamePixel background;

        public static Coord size;
        public static int pixelWidth;

        public static ScreenRegion gameRegion;
        public static Rect viewport;
        public static Rect scrollFreeZone;

        public static int edgeBumper;

        public static List<Entity> entities;

        public static List<int> excludedLayers;

        public static Ticker gameTicker;
        public static Ticker viewportTicker;

        public static bool paused;

        public static void Init(Coord windowSize, Coord gameSize, int pixelWidth)
        {
            Window.Init(windowSize);
            Game.pixelWidth = pixelWidth;
            background = new GamePixel(pixelWidth, 0, 0);
            if (gameSize.x <= 0 | gameSize.y <= 0)
            {
                gameSize = new Coord(Console.WindowWidth / pixelWidth, Console.WindowHeight);
            }
            size = gameSize;
            gameRegion = new ScreenRegion(new Coord(0, 0));
            InitGrid();
            InitViewport();
            edgeBumper = 2;
            entities = new List<Entity>();
            excludedLayers = new List<int>();
            gameTicker = new Ticker(0.1f);
            viewportTicker = gameTicker;
            paused = false;
        }

        public static void InitGrid()
        {
            grid = new GamePixel[size.y][];

            for (int y = 0; y < size.y; y++)
            {
                grid[y] = new GamePixel[size.x];

                for (int x = 0; x < size.x; x++)
                {
                    grid[y][x] = background;
                }
            }
        }

        public static void SetBackground(GamePixel background)
        {
            Game.background = background;
            InitGrid();
        }

        public static void SetBackground(GamePixel[][] grid)
        {
            size = new Coord(grid[0].Length, grid.Length);
            InitGrid();
            Game.grid = (GamePixel[][])grid.Clone();
        }

        public static void InitViewport()
        {
            Coord size = new Coord(Window.content.rect.size.x / pixelWidth, Window.content.rect.size.y);
            if (size.x > Game.size.x) { size.x = Game.size.x; }
            if (size.y > Game.size.y) { size.y = Game.size.y; }
            viewport = new Rect(new Coord(size, "centerongame"), size);
        }

        public static void InitViewport(Coord size)
        {
            if (size.x > 0 & size.y > 0)
            {
                viewport = new Rect(new Coord(size, "centerongame"), new Coord(size.x, size.y));
            }
            else
            {
                throw new Exception("Cannot initialize Viewport with a size parameter less than one.");
            }
        }

        public static void MoveViewport(Vector direction, bool delay)
        {
            if (viewport.position.x + direction.x >= 0 & viewport.position.x + viewport.size.x + direction.x <= size.x & viewport.position.y + direction.y >= 0 & viewport.position.y + viewport.size.y + direction.y <= size.y)
            {
                if (delay & viewportTicker.Check(true)) { viewport.position.Update(direction); }
                else { viewport.position.Update(direction); }
            }
        }

        public static void PreUpdate()
        {
            if (!paused)
            {
                SetScrollFreeZone();
                entities.Sort((e1, e2) => e1.layer.CompareTo(e2.layer));
                entities.Reverse();

                foreach (Entity entity in entities)
                {
                    if (entity.layer < 0)
                    {
                        entity.Move(false);
                    }
                }
            }
        }

        public static void Update(bool stopMotion)
        {
            gameTicker.Check(true);

            foreach (Entity entity in entities)
            {
                if (!excludedLayers.Exists(x => x == entity.layer) & entity.layer >= 0)
                {
                    if (entity.id == "player")
                    {
                        bool scroll = false;

                        if (entity.rect.left < scrollFreeZone.left | entity.rect.right > scrollFreeZone.right | entity.rect.top < scrollFreeZone.top | entity.rect.bottom > scrollFreeZone.bottom)
                        {
                            scroll = true;
                        }

                        entity.Move(stopMotion, scroll);
                    }
                    else
                    {
                        entity.Move(stopMotion);
                    }
                }
            }

            ConstructScene();
            Window.screenRegions.RemoveAt(0);
            Window.screenRegions.Insert(0, gameRegion);
            Window.Update();
        }

        private static void SetScrollFreeZone()
        {
            if (viewport.size.x > edgeBumper * 2 & viewport.size.y > edgeBumper * 2)
            {
                if (edgeBumper > 0)
                {
                    scrollFreeZone = new Rect(new Coord(viewport.size.x - edgeBumper, viewport.size.y - edgeBumper, "centerongame"), new Coord(viewport.size.x - edgeBumper, viewport.size.y - edgeBumper));
                }
                else
                {
                    scrollFreeZone = new Rect(new Coord(1, 1, "centerongame"), new Coord(1, 1));
                }
            }
            else
            {
                throw new Exception("Cannot Initialize a scroll free zone for a viewport this small and an edge bumper this large.");
            }
        }

        public static void ConstructScene()
        {
            GamePixel[][] scene = new GamePixel[viewport.size.y][];

            for (int y = 0; y < viewport.size.y; y++)
            {
                scene[y] = new GamePixel[viewport.size.x];

                for (int x = 0; x < viewport.size.x; x++)
                {
                    Coord gridPos = new Coord(x + viewport.position.x, y + viewport.position.y);

                    scene[y][x] = grid[gridPos.y][gridPos.x];

                    foreach (Entity entity in entities)
                    {
                        if (entity.rect.top <= gridPos.y & entity.rect.bottom >= gridPos.y & entity.rect.left <= gridPos.x & entity.rect.right >= gridPos.x)
                        {
                            if (entity.spriteSheet[entity.activeSpriteSet][entity.activeSprite].grid[gridPos.y - entity.rect.top][gridPos.x - entity.rect.left].foreground >= 0 & entity.spriteSheet[entity.activeSpriteSet][entity.activeSprite].grid[gridPos.y - entity.rect.top][gridPos.x - entity.rect.left].background >= 0)
                            {
                                scene[y][x] = entity.spriteSheet[entity.activeSpriteSet][entity.activeSprite].grid[gridPos.y - entity.rect.top][gridPos.x - entity.rect.left];
                            }
                        }
                    }
                }
            }

            gameRegion.SetContent(scene);
        }
    }
}
