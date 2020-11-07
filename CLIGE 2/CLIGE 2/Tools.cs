using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace CLIGE_2
{
    public struct Coord
    {
        public int x;
        public int y;

        public Coord(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public Coord(int x, int y, string position)
        {
            if (position.ToLower() == "centerongame")
            {
                this.x = (Game.size.x - x) / 2;
                this.y = (Game.size.y - y) / 2;
            }
            else if (position.ToLower() == "centeronscreen")
            {
                this.x = (Window.content.rect.size.x - x) / 2;
                this.y = (Window.content.rect.size.y - y) / 2;
            }
            else
            {
                this.x = 0;
                this.y = 0;
            }
        }

        public Coord(Coord size, string position)
        {
            if (position.ToLower() == "centerongame")
            {
                x = (Game.size.x - size.x) / 2;
                y = (Game.size.y - size.y) / 2;
            }
            else if (position.ToLower() == "centeronscreen")
            {
                x = (Window.content.rect.size.x - size.x) / 2;
                y = (Window.content.rect.size.y - size.y) / 2;
            }
            else
            {
                x = 0;
                y = 0;
            }
        }

        public void Update(Vector direction)
        {
            x += direction.x * direction.v;
            y += direction.y * direction.v;
        }

        public static bool operator ==(Coord c1, Coord c2) { return c1.Equals(c2); }
        public static bool operator !=(Coord c1, Coord c2) { return !c1.Equals(c2); }
        public override bool Equals(object obj) { return base.Equals(obj); }
        public override int GetHashCode() { return base.GetHashCode(); }
    }

    public struct Rect
    {
        public Coord position;
        public Coord size;

        public int top;
        public int bottom;
        public int left;
        public int right;

        public Rect(Coord position)
        {
            this.position = position;
            size = new Coord(0, 0);
            top = position.y;
            bottom = top;
            left = position.x;
            right = left;
        }

        public Rect(Coord position, Coord size)
        {
            if (size.x >= 0 & size.y >= 0)
            {
                this.position = position;
                this.size = size;
                top = position.y;
                bottom = top + size.y - 1;
                left = position.x;
                right = left + size.x - 1;
            }
            else
            {
                throw new Exception("Cannot instantiate Rect with negative size.");
            }
        }

        public void RefreshSides()
        {
            top = position.y;
            bottom = top + size.y - 1;
            left = position.x;
            right = left + size.x - 1;
        }
    }

    public struct Vector
    {
        public int x;
        public int y;
        public int v;

        public Vector(int x, int y)
        {
            this.x = x;
            this.y = y;
            v = 1;
        }

        public Vector(int x, int y, int velocity)
        {
            this.x = x;
            this.y = y;
            v = velocity;
        }

        public static bool operator ==(Vector v1, Vector v2) { return v1.Equals(v2); }
        public static bool operator !=(Vector v1, Vector v2) { return !v1.Equals(v2); }
        public override bool Equals(object obj) { return base.Equals(obj); }
        public override int GetHashCode() { return base.GetHashCode(); }
    }

    public struct Ticker
    {
        public float delay;
        public DateTime startTime;
        public DateTime lastUpdate;

        public Ticker(float delay)
        {
            if (delay >= 0)
            {
                this.delay = delay;
                startTime = DateTime.Now;
                lastUpdate = DateTime.Now;
            }
            else
            {
                throw new Exception("Cannot instantiate Ticker with a negative delay.");
            }
        }

        public bool Check(bool update)
        {
            bool result = false;

            if (DateTime.Now >= lastUpdate.AddSeconds(delay))
            {
                if (update) { lastUpdate = DateTime.Now; }
                result = true;
            }

            return result;
        }
    }

    public class Tools
    {
        public static Random rng = new Random();

        public static void Log(string message)
        {
            Debug.WriteLine(message);
        }

        public static void Error(string message)
        {
            Debug.WriteLine($"ERROR: {message}");
        }
    }
}
