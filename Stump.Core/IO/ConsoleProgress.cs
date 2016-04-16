using System;

namespace Stump.Core.IO
{
    public class ConsoleProgress
    {
        private object m_lastValue;
        public int PositionX
        {
            get;
            set;
        }
        public int PositionY
        {
            get;
            set;
        }

        public ConsoleProgress()
        {
            this.PositionX = Console.CursorLeft;
            this.PositionY = Console.CursorTop;
        }

        public ConsoleProgress(int positionX, int positionY)
        {
            this.PositionX = positionX;
            this.PositionY = positionY;
        }

        public void Update(int value)
        {
            if (!value.Equals(this.m_lastValue))
            {
                this.m_lastValue = value;
                int cursorLeft = Console.CursorLeft;
                int cursorTop = Console.CursorTop;
                Console.SetCursorPosition(this.PositionX, this.PositionY);
                Console.Write(value + "%");
                Console.SetCursorPosition(cursorLeft, cursorTop);
            }
        }

        public void Update(string value)
        {
            if (!value.Equals(this.m_lastValue))
            {
                int cursorLeft = Console.CursorLeft;
                int cursorTop = Console.CursorTop;
                Console.SetCursorPosition(this.PositionX, this.PositionY);
                Console.Write(value);
                Console.SetCursorPosition(cursorLeft, cursorTop);
            }
        }

        public void End()
        {
            string text = string.Empty;
            for (int i = 0; i < Console.BufferWidth - this.PositionX; i++)
            {
                text += " ";
            }
            int cursorLeft = Console.CursorLeft;
            int cursorTop = Console.CursorTop;
            Console.SetCursorPosition(this.PositionX, this.PositionY);
            Console.Write(text);
            Console.SetCursorPosition(cursorLeft, cursorTop);
        }
    }
}