using System;
using System.Threading;
using System.Collections.Generic;
using System.Linq;


namespace SnakeGame
{
    class Game
    {
        static readonly int x = 40;
        static readonly int y = 15;
        private static int score = 0;

        static Walls walls;
        static Snake snake;
        static FoodKitchen _foodKitchen;
        static Timer time;
        static void Main()
        {
            Console.SetCursorPosition(1, y+1);
            Console.WriteLine("пельменей съето: " + score);
            Console.OutputEncoding = System.Text.Encoding.UTF8;  
            Console.CursorVisible = false;

            walls = new Walls(x, y, '#');
            snake = new Snake(x / 2, y / 2, 5);
            
            _foodKitchen = new FoodKitchen(x, y, '\u263A');
            _foodKitchen.CookFood();

            time = new Timer(Loop, null, 0, 130);

            while (true)
            {
                if (Console.KeyAvailable)
                {
                    ConsoleKeyInfo key = Console.ReadKey();
                    snake.Rotation(key.Key);
                }
            }
        }

        static void Loop(object obj)
        {
            if (walls.IsCrash(snake.GetHead()) || snake.IsHit(snake.GetHead()))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.SetCursorPosition(20, y+1);
                Console.WriteLine("Game over!" );
                time.Change(0, Timeout.Infinite);
            }
            else if (snake.Eat(_foodKitchen.food))
            {
                score = score + 1;
                Console.SetCursorPosition(1, y+1);
                Console.WriteLine("пельменей съето: " + score);
                _foodKitchen.CookFood();
            }
            else
            {
                snake.Move();
            }
        }
    }

    struct Point
    {
        public int x { get; set; }
        public int y { get; set; }
        public char ch { get; set; }
        
        public static implicit operator Point((int, int, char) value) => 
              new Point {x = value.Item1, y = value.Item2, ch = value.Item3};

        public static bool operator ==(Point a, Point b) => 
                (a.x == b.x && a.y == b.y);
        public static bool operator !=(Point a, Point b) => 
                (a.x != b.x || a.y != b.y);

        public void Draw()
        {
            DrawPoint(ch);
        }
        public void Clear()
        {
            DrawPoint(' ');
        }

        private void DrawPoint(char _ch)
        {
            Console.SetCursorPosition(x, y);
            Console.Write(_ch);
        }
    }

    class Walls
    {
        private char ch;
        private List<Point> wall = new List<Point>();

        public Walls(int x, int y, char ch)
        {
            this.ch = ch;
            Console.ForegroundColor = ConsoleColor.Yellow;
            DrawHorizontal(x, 0);
            DrawHorizontal(x, y);
            DrawHorizontal(x/3, y/2);
            DrawVertical(0, y);
            DrawVertical(x, y);
            DrawVertical(x/2, y/2);
            Console.ForegroundColor = ConsoleColor.DarkGreen;
        }

        private void DrawHorizontal(int x, int y)
        {
            for (int i = 0; i < x; i++)
            {
                Point p = (i, y, ch);
                p.Draw();
                wall.Add(p);
            }
        }

        private void DrawVertical(int x, int y)
        {
            for (int i = 0; i < y; i++)
            {
                Point p = (x, i, ch);
                p.Draw();
                wall.Add(p);
            }
        }

        public bool IsCrash(Point p)
        {
            foreach (var w in wall)
            {
                if (p == w)
                {
                    return true;
                }
            }
            return false;
        }
    }// class Walls

    enum Direction
    {
        LEFT,
        RIGHT,
        UP,
        DOWN
    }

    class Snake
    {
        private List<Point> snake;

        private Direction direction;
        private int step = 1;
        private Point tail;
        private Point head;

        bool rotate = true;

        public Snake(int x, int y, int length)
        {
            direction = Direction.RIGHT;

            snake = new List<Point>();
            for (int i = x - length; i < x; i++)
            {
                Point p = (i, y, '*');
                snake.Add(p);

                p.Draw();
            }
        }

        public Point GetHead() => snake.Last();

        public void Move()
        {
            head = GetNextPoint();
            snake.Add(head);

            tail = snake.First();
            snake.Remove(tail);

            tail.Clear();
            head.Draw();

            rotate = true;
        }

        public bool Eat(Point p)
        {
            head = GetNextPoint();
            if (head == p)
            {
                snake.Add(head);
                head.Draw();
                return true;
            }
            return false;
        }

    public Point GetNextPoint () 
    {
        Point p = GetHead ();

        switch (direction) 
        {
        case Direction.LEFT:
            p.x -= step;
            break;
        case Direction.RIGHT:
            p.x += step;
            break;
        case Direction.UP:
            p.y -= step;
            break;
        case Direction.DOWN:
            p.y += step;
            break;
        }
        return p;
    }

    public void Rotation (ConsoleKey key) 
    {
        if (rotate) 
        {
            switch (direction) 
            {
            case Direction.LEFT:
            case Direction.RIGHT:
                if (key == ConsoleKey.DownArrow)
                    direction = Direction.DOWN;
                else if (key == ConsoleKey.UpArrow)
                    direction = Direction.UP;
                break;
            case Direction.UP:
            case Direction.DOWN:
                if (key == ConsoleKey.LeftArrow)
                    direction = Direction.LEFT;
                else if (key == ConsoleKey.RightArrow)
                    direction = Direction.RIGHT;
                break;
            }
            rotate = false;
        }

    }

        public bool IsHit(Point p)
        {
            for (int i = snake.Count - 2; i > 0; i--)
            {
                if (snake[i] == p)
                {
                    return true;
                }
            }
            return false;
        }
    }//class Snake

    class FoodKitchen
    {
        int x;
        int y;
        char ch;
        public Point food { get; private set; }

        Random random = new Random();

        public FoodKitchen(int x, int y, char ch)
        {
            this.x = x;
            this.y = y;
            this.ch = ch;
        }

        public void CookFood()
        {
            while (true) {
                food = (random.Next(2, x - 2), random.Next(2, y - 2), ch);
                if ((food.x != x/2-4) && (food.y != y/2))
                {  
                    food.Draw();
                    break;
                }
            }
        }
    }
}