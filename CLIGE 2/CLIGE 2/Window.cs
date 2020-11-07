using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CLIGE_2
{
    public struct Pixel
    {
        public char texture;
        public int foreground;
        public int background;

        public Pixel(int foreground, int background)
        {
            texture = ' ';
            if (foreground > 15) { foreground = 15; }
            if (background > 15) { background = 15; }
            this.foreground = foreground;
            this.background = background;
        }

        public Pixel(char texture, int foreground, int background)
        {
            this.texture = texture;
            if (foreground > 15) { foreground = 15; }
            if (background > 15) { background = 15; }
            this.foreground = foreground;
            this.background = background;
        }

        public Pixel(GamePixel pixel, int index)
        {
            try
            {
                texture = pixel.texture[index];
            }
            catch
            {
                texture = pixel.texture[0];
            }

            if (pixel.foreground > 15) { pixel.foreground = 15; }
            if (pixel.background > 15) { pixel.background = 15; }
            foreground = pixel.foreground;
            background = pixel.background;
        }
    }

    public struct ScreenRegion
    {
        public bool active;
        public Rect rect;
        public Pixel[][] grid;

        public ScreenRegion(Coord size)
        {
            if (size.x >= 0 & size.y >= 0)
            {
                active = false;

                rect = new Rect(new Coord(0, 0), size);

                grid = InitGrid(size);
            }
            else
            {
                throw new Exception("Cannot instantiate ScreenRegion with a negative size.");
            }
        }

        public ScreenRegion(Coord size, Coord position)
        {
            if (size.x >= 0 & size.y >= 0)
            {
                if (position.x >= 0 & position.y >= 0)
                {
                    active = false;

                    rect = new Rect(position, size);

                    grid = InitGrid(size);
                }
                else
                {
                    throw new Exception("Cannot instantiate ScreenRegion with a negative position.");
                }
            }
            else
            {
                throw new Exception("Cannot instantiate ScreenRegion with a negative size.");
            }
        }

        public static Pixel[][] InitGrid(Coord size)
        {
            Pixel[][] grid = new Pixel[size.y][];
            for(int y = 0; y < size.y; y++)
            {
                grid[y] = new Pixel[size.x];
                for(int x = 0; x < size.x; x++)
                {
                    grid[y][x] = new Pixel(0, 0);
                }
            }
            return grid;
        }

        public void SetContent(string content)
        {
            if (rect.size.x < content.Length) { rect.size.x = content.Length; }
            if (rect.size.y < 1) { rect.size.y = 1; }

            active = true;

            grid = new Pixel[rect.size.y][];

            for (int y = 0; y < rect.size.y; y++)
            {
                grid[y] = new Pixel[rect.size.x];

                for (int x = 0; x < rect.size.x; x++)
                {
                    if (y == rect.size.y / 2 & x >= (rect.size.x - content.Length) / 2 & x < ((rect.size.x - content.Length) / 2) + content.Length)
                    {
                        grid[y][x] = new Pixel(content[x - ((rect.size.x - content.Length) / 2)], 15, 0);
                    }
                    else
                    {
                        grid[y][x] = new Pixel(0, 0);
                    }
                }
            }
        }

        public void SetContent(string[] content)
        {
            if (rect.size.y < content.Length) { rect.size.y = content.Length; }

            int largestWidth = 0;

            for (int y = 0; y < content.Length; y++)
            {
                if (content[y].Length > largestWidth) { largestWidth = content[y].Length; }
            }

            if (rect.size.x < largestWidth) { rect.size.x = largestWidth; }

            active = true;

            grid = new Pixel[rect.size.y][];

            for (int y = 0; y < rect.size.y; y++)
            {
                grid[y] = new Pixel[rect.size.x];

                for (int x = 0; x < rect.size.x; x++)
                {
                    if (y >= (rect.size.y - content.Length) / 2 & y < ((rect.size.y - content.Length) / 2) + content.Length & x >= (rect.size.x - content[y].Length) / 2 & x < ((rect.size.x - content[y].Length) / 2) + content[y].Length)
                    {
                        grid[y][x] = new Pixel(content[y - (rect.size.y - content.Length) / 2][x - (rect.size.x - content[y].Length) / 2], 15, 0);
                    }
                    else
                    {
                        grid[y][x] = new Pixel(0, 0);
                    }
                }
            }
        }

        public void SetContent(Pixel[][] grid)
        {
            if (rect.size.x < grid[0].Length) { rect.size.x = grid[0].Length; }
            if (rect.size.y < grid.Length) { rect.size.y = grid.Length; }

            active = true;
            this.grid = (Pixel[][])grid.Clone();
        }

        public void SetContent(GamePixel[][] grid)
        {
            if (rect.size.x < grid[0].Length * grid[0][0].width) { rect.size.x = grid[0].Length * grid[0][0].width; }
            if (rect.size.y < grid.Length) { rect.size.y = grid.Length; }

            active = true;

            Coord size = new Coord(grid[0].Length, grid.Length);

            this.grid = new Pixel[size.y][];

            for (int y = 0; y < size.y; y++)
            {
                this.grid[y] = new Pixel[rect.size.x];

                for (int x = 0; x < size.x; x++)
                {
                    Pixel[] pa = grid[y][x].ConvertToPixels();

                    for (int p = 0; p < pa.Length; p++)
                    {
                        this.grid[y][(x * pa.Length) + p] = pa[p];
                    }
                }
            }
        }

        public void Clear()
        {
            active = false;

            grid = new Pixel[rect.size.y][];

            for (int y = 0; y < rect.size.y; y++)
            {
                grid[y] = new Pixel[rect.size.x];

                for (int x = 0; x < rect.size.x; x++)
                {
                    grid[y][x] = new Pixel(0, 0);
                }
            }
        }
    }

    public class Window
    {
        public static bool newWindow = true;

        public static ScreenRegion content;
        public static List<ScreenRegion> screenRegions;

        public static void Init(Coord size)
        {
            if (size.x >= 32 & size.y >= 16)
            {
                Console.SetWindowSize(size.x, size.y);
                Console.SetBufferSize(size.x, size.y);
                Console.SetWindowSize(size.x, size.y);
            }
            else
            {
                Interop.FullscreenWindow();
            }

            Interop.DisableResizing();
            Interop.SetConsoleHandle();

            content = new ScreenRegion(new Coord(Console.WindowWidth, Console.WindowHeight));
            screenRegions = new List<ScreenRegion>() { new ScreenRegion(new Coord(0, 0)) };
        }

        public static void Update()
        {
            CheckForSizeChange();
            content.Clear();

            for (int s = 0; s < screenRegions.Count(); s++)
            {
                ScreenRegion region = screenRegions[s];

                if (region.active)
                {
                    for (int y = 0; y < region.rect.size.y; y++)
                    {
                        for (int x = 0; x < region.rect.size.x; x++)
                        {
                            content.grid[region.rect.position.y + y][region.rect.position.x + x] = region.grid[y][x];
                        }
                    }
                }
            }

            Interop.Draw(content);
        }

        public static void CheckForSizeChange()
        {
            Console.CursorVisible = false;

            if (content.rect.size.x != Console.WindowWidth | content.rect.size.y != Console.WindowHeight | newWindow)
            {
                if(newWindow) { newWindow = false; }
                Console.SetWindowSize(content.rect.size.x, content.rect.size.y);
                Console.SetBufferSize(content.rect.size.x, content.rect.size.y);
            }
        }
    }
}
