using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{

    public class Point
    {
        public int X_position { get; private set; }
        public int Y_position { get; private set; }

        public Point(int x, int y)
        {
            this.X_position = x;
            this.Y_position = y;
        }

        public override string ToString()
        {
            return "(" + X_position + "," + Y_position + ")";
        }
        public void UpdatePosition(int x, int y)
        {
            X_position = x;
            Y_position = y;
        }
    }

    public class Line
    {
        public Point begin_point { get; private set; }
        public Point end_point { get; private set; }
        public Color line_colour { get; private set; }

        public Line(Point begin_Point, Point end_Point, Color line_colour)
        {
            this.begin_point = begin_Point;
            this.end_point = end_Point;
            this.line_colour = line_colour;
        }
        public void UpdateColor(Color new_color)
        {
            line_colour = new_color;
        }
        public override string ToString()
        {
            return "Line: begin_Point:" + begin_point.ToString() + " end_Point:" + end_point.ToString() + " color:" + line_colour.Name;
        }
    }
    class Program
    {
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
