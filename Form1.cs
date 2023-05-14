using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        private List<Line> lines = new List<Line>(); // List to store drawn lines
        private List<Point> clickedPoints = new List<Point>(); // List to store clicked points on the form
        private Color colorSelected = System.Drawing.Color.Black; // Default selected color is black

        public Form1()
        {
            InitializeComponent();
            pictureBox1.Paint += PictureBox1_Paint;
            dataGridView1.CellValueChanged += dataGridView1_CellValueChanged;
            pictureBox1.MouseUp += PictureBox1_MouseUp;
            dataGridView1.CellClick += dataGridView1_CellContentClick;
        }

        // Event handler when a cell is clicked
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // Ensure that the color dialog pops up only when the color cell is clicked
            if (e.RowIndex >= 0 && e.ColumnIndex == 4 && sender == dataGridView1)
            {
                try
                {
                    using (ColorDialog colorDialog = new ColorDialog())
                    {
                        if (colorDialog.ShowDialog() == DialogResult.OK)
                        {
                            Color newColor = colorDialog.Color;

                            // Get the line of the selected row and update its color
                            Line selectedLine = lines[e.RowIndex];
                            selectedLine.UpdateColor(newColor);

                            // Update the color value of dataGridView1
                            dataGridView1.Rows[e.RowIndex].Cells[4].Style.BackColor = newColor;
                            dataGridView1.Rows[e.RowIndex].Cells[4].Style.SelectionBackColor = newColor;

                            pictureBox1.Invalidate(); //Repaint pictureBox1
                        }
                    }
                }
                catch (ArgumentOutOfRangeException ex)
                {
                    MessageBox.Show("Enter the four coordinates first", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // Event handler when the value of a cell changes
        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            // The following code checks for the validity of the cell data and updates the line segment accordingly
            // If the data is not valid, it shows an error message
            // If the data is valid, it either updates an existing line segment or creates a new one, depending on the row index
            if (e.RowIndex >= 0)
            {
                if (dataGridView1.Rows[e.RowIndex].Cells[0].Value == null
                    || dataGridView1.Rows[e.RowIndex].Cells[1].Value == null
                    || dataGridView1.Rows[e.RowIndex].Cells[2].Value == null
                    || dataGridView1.Rows[e.RowIndex].Cells[3].Value == null
                    )
                {
                    dataGridView1.Rows[e.RowIndex].Cells[4].Style.BackColor = colorSelected;
                    dataGridView1.Rows[e.RowIndex].Cells[4].Style.SelectionBackColor = colorSelected;
                }
                if (dataGridView1.Rows[e.RowIndex].Cells[0].Value != null
                    && dataGridView1.Rows[e.RowIndex].Cells[1].Value != null
                    && dataGridView1.Rows[e.RowIndex].Cells[2].Value != null
                    && dataGridView1.Rows[e.RowIndex].Cells[3].Value != null)
                {
                    int firstX, firstY, secondX, secondY;

                    if (int.TryParse(dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString(), out firstX) &&
                        int.TryParse(dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString(), out firstY) &&
                        int.TryParse(dataGridView1.Rows[e.RowIndex].Cells[2].Value.ToString(), out secondX) &&
                        int.TryParse(dataGridView1.Rows[e.RowIndex].Cells[3].Value.ToString(), out secondY))
                    {
                        if (firstX < 0 || firstY < 0 || secondX < 0 || secondY < 0)
                        {
                            MessageBox.Show("Negative values are not allowed.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                        if (e.RowIndex < lines.Count)
                        {
                            // Update line's begin and end points
                            lines[e.RowIndex].begin_point.UpdatePosition(firstX, firstY);
                            lines[e.RowIndex].end_point.UpdatePosition(secondX, secondY);
                        }
                        else
                        {
                            Point begin_point = new Point(firstX, firstY);
                            Point end_point = new Point(secondX, secondY);
                            Line line = new Line(begin_point, end_point, colorSelected);
                            lines.Add(line);
                            dataGridView1.Rows[e.RowIndex].Cells[4].Style.BackColor = colorSelected;
                        }

                        pictureBox1.Invalidate(); 
                    }
                    else
                    {
                        MessageBox.Show("Some of the number fields do not contain integers", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
        // Event handler when the mouse button is released
        private void PictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            clickedPoints.Add(new Point(e.X, e.Y));

            if (clickedPoints.Count % 2 == 0)
            {
                Point startPoint = clickedPoints[clickedPoints.Count - 2];
                Point endPoint = clickedPoints[clickedPoints.Count - 1];
                Line line = new Line(startPoint, endPoint, colorSelected);

                lines.Add(line);
                pictureBox1.Invalidate();
                // Adds the new line segment's data to dataGridView1
                DataGridViewRow row = new DataGridViewRow();
                row.CreateCells(dataGridView1);

                row.Cells[0].Value = startPoint.X_position;
                row.Cells[1].Value = startPoint.Y_position;
                row.Cells[2].Value = endPoint.X_position;
                row.Cells[3].Value = endPoint.Y_position;
                row.Cells[4].Style.BackColor = colorSelected;

                dataGridView1.Rows.Add(row);
            }
        }

        private void Color_Button_Click(object sender, EventArgs e)
        {
            using (ColorDialog colorDialog = new ColorDialog())
            {
                if (colorDialog.ShowDialog() == DialogResult.OK)
                {
                    colorSelected = colorDialog.Color;
                    pictureBox1.Invalidate();
                }
            }
        }

        // Paint event handler for PictureBox
        private void PictureBox1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            foreach (Line line in lines)
            {
                Color color = line.line_colour;
                using (Pen pen = new Pen(color, 2))
                {
                    g.DrawLine(pen, line.begin_point.X_position, line.begin_point.Y_position, line.end_point.X_position, line.end_point.Y_position);
                }
            }
        }

        private void Remove_Click(object sender, EventArgs e)
        {
            int selectedRowIndex = -1;

            // Finds and removes the selected row in dataGridView1, and the corresponding line segment
            foreach (DataGridViewCell cell in dataGridView1.SelectedCells)
            {
                if (cell.ColumnIndex < 4) // Only consider the first four columns
                {
                    selectedRowIndex = cell.RowIndex;
                    break;
                }
            }

            if (selectedRowIndex >= 0)
            {
                try
                {
                    // clear the data of the row
                    dataGridView1.Rows.RemoveAt(selectedRowIndex);

                    // Removes the corresponding line from the list of lines and refreshes the PictureBox
                    if (selectedRowIndex < lines.Count)
                    {
                        lines.RemoveAt(selectedRowIndex);
                        pictureBox1.Invalidate();
                    }
                }
                catch (InvalidOperationException ex)
                {
                    MessageBox.Show("No line selected.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("No line selected.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public class LineSegment
        {
            public Point Start { get; set; }
            public Point End { get; set; }

            public LineSegment(Point start, Point end)
            {
                Start = start;
                End = end;
            }
        }

        private bool LineSegmentsIntersect(LineSegment a, LineSegment b, out PointF intersection)
        {
            float A1 = a.End.Y_position - a.Start.Y_position;
            float B1 = a.Start.X_position - a.End.X_position;
            float C1 = A1 * a.Start.X_position + B1 * a.Start.Y_position;

            float A2 = b.End.Y_position - b.Start.Y_position;
            float B2 = b.Start.X_position - b.End.X_position;
            float C2 = A2 * b.Start.X_position + B2 * b.Start.Y_position;

            float det = A1 * B2 - A2 * B1;

            if (Math.Abs(det) < 1E-9)
            {
                intersection = default(PointF);
                return false;
            }

            float x = (B2 * C1 - B1 * C2) / det;
            float y = (A1 * C2 - A2 * C1) / det;

            if (x >= Math.Min(a.Start.X_position, a.End.X_position) && x <= Math.Max(a.Start.X_position, a.End.X_position) &&
                x >= Math.Min(b.Start.X_position, b.End.X_position) && x <= Math.Max(b.Start.X_position, b.End.X_position) &&
                y >= Math.Min(a.Start.Y_position, a.End.Y_position) && y <= Math.Max(a.Start.Y_position, a.End.Y_position) &&
                y >= Math.Min(b.Start.Y_position, b.End.Y_position) && y <= Math.Max(b.Start.Y_position, b.End.Y_position))
            {
                intersection = new PointF(x, y);
                return true;
            }

            intersection = default(PointF);
            return false;
        }

        private bool LineSegmentsOverlap(LineSegment a, LineSegment b, out LineSegment overlap)
        {
            float A1 = a.End.Y_position - a.Start.Y_position;
            float B1 = a.Start.X_position - a.End.X_position;
            float C1 = A1 * a.Start.X_position + B1 * a.Start.Y_position;

            float A2 = b.End.Y_position - b.Start.Y_position;
            float B2 = b.Start.X_position - b.End.X_position;
            float C2 = A2 * b.Start.X_position + B2 * b.Start.Y_position;

            float det = A1 * B2 - A2 * B1;

            if (Math.Abs(det) < 1E-9)
            {
                if (Math.Max(a.Start.X_position, a.End.X_position) < Math.Min(b.Start.X_position, b.End.X_position) ||
                    Math.Max(b.Start.X_position, b.End.X_position) < Math.Min(a.Start.X_position, a.End.X_position) ||
                    Math.Max(a.Start.Y_position, a.End.Y_position) < Math.Min(b.Start.Y_position, b.End.Y_position) ||
                    Math.Max(b.Start.Y_position, b.End.Y_position) < Math.Min(a.Start.Y_position, a.End.Y_position))
                {
                    overlap = null;
                    return false;
                }

                Point topLeft = new Point(Math.Max(Math.Min(a.Start.X_position, a.End.X_position), Math.Min(b.Start.X_position, b.End.X_position)),
                                          Math.Max(Math.Min(a.Start.Y_position, a.End.Y_position), Math.Min(b.Start.Y_position, b.End.Y_position)));
                Point bottomRight = new Point(Math.Min(Math.Max(a.Start.X_position, a.End.X_position), Math.Max(b.Start.X_position, b.End.X_position)),
                                              Math.Min(Math.Max(a.Start.Y_position, a.End.Y_position), Math.Max(b.Start.Y_position, b.End.Y_position)));

                overlap = new LineSegment(topLeft, bottomRight);
                return true;
            }

            overlap = null;
            return false;
        }


        private void Find_intersections_Click(object sender, EventArgs e)
        {
            List<PointF> intersections = new List<PointF>();
            List<LineSegment> overlappingLines = new List<LineSegment>();

            for (int i = 0; i < lines.Count; i++)
            {
                for (int j = i + 1; j < lines.Count; j++)
                {
                    LineSegment segment1 = new LineSegment(lines[i].begin_point, lines[i].end_point);
                    LineSegment segment2 = new LineSegment(lines[j].begin_point, lines[j].end_point);

                    PointF intersection;
                    if (LineSegmentsIntersect(segment1, segment2, out intersection))
                    {
                        intersections.Add(intersection);
                    }
                    else
                    {
                        LineSegment overlap;
                        if (LineSegmentsOverlap(segment1, segment2, out overlap))
                        {
                            overlappingLines.Add(overlap);
                        }
                    }
                }
            }
            using (Graphics g = pictureBox1.CreateGraphics())
            {
                // Draw circles around intersections
                foreach (PointF intersection in intersections)
                {
                    g.DrawEllipse(new Pen(colorSelected, 2), intersection.X - 5, intersection.Y - 5, 10, 10);
                }

                // Draw overlapping lines with their original color
                foreach (LineSegment line in overlappingLines)
                {
                    g.DrawLine(new Pen(colorSelected, 2), line.Start.X_position, line.Start.Y_position, line.End.X_position, line.End.Y_position);
                }
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
