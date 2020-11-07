using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLIGE_2
{
    public struct Sprite
    {
        public Coord size;
        public GamePixel[][] grid;

        public Sprite(int foreground, int background)
        {
            size = new Coord(1, 1);
            if (foreground > 15) { foreground = 15; }
            if (background > 15) { background = 15; }
            grid = new GamePixel[1][] { new GamePixel[1] { new GamePixel(foreground, background) } };
        }

        public Sprite(string texture, int foreground, int background)
        {
            size = new Coord(1, 1);
            if (foreground > 15) { foreground = 15; }
            if (background > 15) { background = 15; }
            grid = new GamePixel[1][] { new GamePixel[1] { new GamePixel(texture, foreground, background) } };
        }

        public Sprite(Coord size, string texture, int foreground, int background)
        {
            this.size = size;

            if (foreground > 15) { foreground = 15; }
            if (background > 15) { background = 15; }

            grid = new GamePixel[size.y][];

            for (int y = 0; y < size.y; y++)
            {
                grid[y] = new GamePixel[size.x];

                for (int x = 0; x < size.x; x++)
                {
                    grid[y][x] = new GamePixel(texture, foreground, background);
                }
            }
        }

        public Sprite(Coord size, string[][] texture, int foreground, int background)
        {
            if (texture.Length == size.y & texture[0].Length == size.x)
            {
                this.size = size;

                if (foreground > 15) { foreground = 15; }
                if (background > 15) { background = 15; }

                grid = new GamePixel[size.y][];

                for (int y = 0; y < size.y; y++)
                {
                    grid[y] = new GamePixel[size.x];

                    for (int x = 0; x < size.x; x++)
                    {
                        grid[y][x] = new GamePixel(texture[y][x], foreground, background);
                    }
                }
            }
            else
            {
                throw new Exception("Cannot instantiate Sprite because the specified size does not match the array sizes.");
            }
        }

        public Sprite(Coord size, string texture, int[][] foreground, int background)
        {
            if (foreground.Length == size.y & foreground[0].Length == size.x)
            {
                this.size = size;

                if (background > 15) { background = 15; }

                grid = new GamePixel[size.y][];

                for (int y = 0; y < size.y; y++)
                {
                    grid[y] = new GamePixel[size.x];

                    for (int x = 0; x < size.x; x++)
                    {
                        if (foreground[y][x] > 15) { foreground[y][x] = 15; }
                        grid[y][x] = new GamePixel(texture, foreground[y][x], background);
                    }
                }
            }
            else
            {
                throw new Exception("Cannot instantiate Sprite because the specified size does not match the array sizes.");
            }
        }

        public Sprite(Coord size, string texture, int[][] foreground, int[][] background)
        {
            if (foreground.Length == size.y & foreground[0].Length == size.x & background.Length == size.y & background[0].Length == size.x)
            {
                this.size = size;

                grid = new GamePixel[size.y][];

                for (int y = 0; y < size.y; y++)
                {
                    grid[y] = new GamePixel[size.x];

                    for (int x = 0; x < size.x; x++)
                    {
                        if (foreground[y][x] > 15) { foreground[y][x] = 15; }
                        if (background[y][x] > 15) { background[y][x] = 15; }
                        grid[y][x] = new GamePixel(texture, foreground[y][x], background[y][x]);
                    }
                }
            }
            else
            {
                throw new Exception("Cannot instantiate Sprite because the specified size does not match the array sizes.");
            }
        }

        public Sprite(Coord size, string[][] texture, int[][] foreground, int background)
        {
            if (texture.Length == size.y & texture[0].Length == size.x & foreground.Length == size.y & foreground[0].Length == size.x)
            {
                this.size = size;

                if (background > 15) { background = 15; }

                grid = new GamePixel[size.y][];

                for (int y = 0; y < size.y; y++)
                {
                    grid[y] = new GamePixel[size.x];

                    for (int x = 0; x < size.x; x++)
                    {
                        if (foreground[y][x] > 15) { foreground[y][x] = 15; }
                        grid[y][x] = new GamePixel(texture[y][x], foreground[y][x], background);
                    }
                }
            }
            else
            {
                throw new Exception("Cannot instantiate Sprite because the specified size does not match the array sizes.");
            }
        }

        public Sprite(Coord size, string[][] texture, int[][] foreground, int[][] background)
        {
            if (texture.Length == size.y & texture[0].Length == size.x & foreground.Length == size.y & foreground[0].Length == size.x & background.Length == size.y & background[0].Length == size.x)
            {
                this.size = size;

                grid = new GamePixel[size.y][];

                for (int y = 0; y < size.y; y++)
                {
                    grid[y] = new GamePixel[size.x];

                    for (int x = 0; x < size.x; x++)
                    {
                        if (foreground[y][x] > 15) { foreground[y][x] = 15; }
                        if (background[y][x] > 15) { background[y][x] = 15; }
                        grid[y][x] = new GamePixel(texture[y][x], foreground[y][x], background[y][x]);
                    }
                }
            }
            else
            {
                throw new Exception("Cannot instantiate Sprite because the specified size does not match the array sizes.");
            }
        }

        public Sprite(GamePixel[][] grid)
        {
            size = new Coord(grid[0].Length, grid.Length);

            this.grid = (GamePixel[][])grid.Clone();
        }
    }

    public struct Entity
    {
        public string id;
        public int layer;
        public int activeSpriteSet;
        public int activeSprite;
        public List<Sprite[]> spriteSheet;
        public Rect rect;
        public Vector direction;
        public Ticker moveDelay;

        public Entity(string id, Coord position)
        {
            if (Game.entities.Exists(x => x.id == id)) { this.id = id + "_2"; }
            else { this.id = id; }
            layer = 0;
            activeSpriteSet = 0;
            activeSprite = 0;
            spriteSheet = new List<Sprite[]>() { new Sprite[1] { new Sprite(new string(' ', Game.pixelWidth), 0, 15) } };
            rect = new Rect(position, new Coord(1, 1));
            direction = new Vector(0, 0, 0);
            moveDelay = new Ticker(Game.gameTicker.delay);
            Game.entities.Add(this);
        }

        public Entity(string id, int layer, Coord position)
        {
            if (Game.entities.Exists(x => x.id == id)) { this.id = id + "_2"; }
            else { this.id = id; }
            this.layer = layer;
            activeSpriteSet = 0;
            activeSprite = 0;
            spriteSheet = new List<Sprite[]>() { new Sprite[1] { new Sprite(new string(' ', Game.pixelWidth), 0, 15) } };
            rect = new Rect(position, new Coord(1, 1));
            direction = new Vector(0, 0, 0);
            moveDelay = new Ticker(Game.gameTicker.delay);
            Game.entities.Add(this);
        }

        public Entity(string id, int layer, Coord position, Sprite sprite)
        {
            if (Game.entities.Exists(x => x.id == id)) { this.id = id + "_2"; }
            else { this.id = id; }
            this.layer = layer;
            activeSpriteSet = 0;
            activeSprite = 0;
            spriteSheet = new List<Sprite[]>() { new Sprite[1] { sprite } };
            rect = new Rect(position, sprite.size);
            direction = new Vector(0, 0, 0);
            moveDelay = new Ticker(Game.gameTicker.delay);
            Game.entities.Add(this);
        }

        public Entity(string id, int layer, Coord position, Sprite[] spriteSet)
        {
            if (Game.entities.Exists(x => x.id == id)) { this.id = id + "_2"; }
            else { this.id = id; }

            bool sameSpriteSizes = true;

            Coord compareSize = spriteSet[0].size;
            foreach(Sprite sprite in spriteSet)
            {
                if (sprite.size != compareSize)
                {
                    sameSpriteSizes = false;
                }
            }

            if (sameSpriteSizes)
            {
                this.layer = layer;
                activeSpriteSet = 0;
                activeSprite = 0;
                this.spriteSheet = new List<Sprite[]>() { spriteSet };
                rect = new Rect(position, spriteSet[0].size);
                direction = new Vector(0, 0, 0);
                moveDelay = new Ticker(Game.gameTicker.delay);
                Game.entities.Add(this);
            }
            else
            {
                throw new Exception("Cannot instantiate Entity because not all of the sprites in the sprite set were the same size.");
            }
        }

        public void Refresh()
        {
            Entity This = this;
            Game.entities.RemoveAt(Game.entities.IndexOf(Game.entities.Find(x => x.id == This.id)));
            Game.entities.Add(this);
        }

        public void Move(bool thenStop)
        {
            if (moveDelay.Check(true) & !CheckForCollisions(true).collision)
            {
                rect.position.Update(direction);

                if (thenStop) { direction.v = 0; }
            }
        }

        public void Move(bool thenStop, bool moveViewport)
        {
            if (moveDelay.Check(true) & !CheckForCollisions(true).collision)
            {
                rect.position.Update(direction);
                Game.MoveViewport(direction, false);
                if (thenStop) { direction.v = 0; }
            }
        }

        private (bool collision, int entityIndex) CheckForCollisions(bool thisLayerOnly)
        {
            Entity This = this;

            bool collision = false;
            int entityIndex = 0;

            if (thisLayerOnly)
            {
                if (Game.entities.Exists(x => x.layer == This.layer & x.rect.top <= This.rect.bottom & This.rect.top <= x.rect.bottom & x.rect.left <= This.rect.right & This.rect.left <= x.rect.right))
                {
                    collision = false;

                    for (int y = rect.top; y <= rect.bottom; y++)
                    {
                        for (int x = rect.left; x <= rect.right; x++)
                        {
                            if (this.spriteSheet[activeSpriteSet][activeSprite].grid[y - rect.top][x - rect.left].foreground >= 0 & this.spriteSheet[activeSpriteSet][activeSprite].grid[y - rect.top][x - rect.left].background >= 0)
                            {
                                List<Entity> possibleCollisions = Game.entities.FindAll(e => e.layer == This.layer & y - e.rect.top >= 0 & e.rect.size.y > y - e.rect.top & x - e.rect.left >= 0 & e.rect.size.x > x - e.rect.left);

                                foreach (Entity e in possibleCollisions)
                                {
                                    if (e.spriteSheet[e.activeSpriteSet][e.activeSprite].grid[y - e.rect.top][x - e.rect.left].foreground >= 0 & e.spriteSheet[e.activeSpriteSet][e.activeSprite].grid[y - e.rect.top][x - e.rect.left].background >= 0) { collision = true; break; }
                                }

                                if (collision) { break; }
                            }
                        }

                        if (collision) { break; }
                    }

                    if (collision)
                    {
                        entityIndex = Game.entities.IndexOf(Game.entities.Find(x => x.layer == This.layer & x.rect.top <= This.rect.bottom & This.rect.top <= x.rect.bottom & x.rect.left <= This.rect.right & This.rect.left <= x.rect.right));
                    }
                }
            }
            else
            {
                if (Game.entities.Exists(x => x.rect.top <= This.rect.bottom & This.rect.top <= x.rect.bottom & x.rect.left <= This.rect.right & This.rect.left <= x.rect.right))
                {
                    collision = false;

                    for (int y = rect.top; y <= rect.bottom; y++)
                    {
                        for (int x = rect.left; x <= rect.right; x++)
                        {
                            if (this.spriteSheet[activeSpriteSet][activeSprite].grid[y - rect.top][x - rect.left].foreground >= 0 & this.spriteSheet[activeSpriteSet][activeSprite].grid[y - rect.top][x - rect.left].background >= 0 & Game.entities.Exists(e => y - e.rect.top >= 0 & e.rect.size.y > y - e.rect.top & x - e.rect.left >= 0 & e.rect.size.x > x - e.rect.left & e.spriteSheet[e.activeSpriteSet][e.activeSprite].grid[y - e.rect.top][x - e.rect.left].foreground >= 0 & e.spriteSheet[e.activeSpriteSet][e.activeSprite].grid[y - e.rect.top][x - e.rect.left].background >= 0))
                            {
                                collision = true;
                                break;
                            }
                        }

                        if (collision) { break; }
                    }

                    if (collision)
                    {
                        entityIndex = Game.entities.IndexOf(Game.entities.Find(x => x.rect.top <= This.rect.bottom & This.rect.top <= x.rect.bottom & x.rect.left <= This.rect.right & This.rect.left <= x.rect.right));
                    }
                }
            }

            return (collision, entityIndex);
        }
    }
}
