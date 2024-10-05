using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic;
using System.Threading;
using System.Drawing;
using System.Windows.Forms;
/*
 * Anima theme by Naywyn - HF
 * Converted to C# by UniveX
 * */

internal static class Helpers
{
    public static SizeF Size;

    public static Point MiddlePoint(Graphics G, string TargetText, Font TargetFont, Rectangle Rect)
    {
        Size = G.MeasureString(TargetText, TargetFont);
        return new Point((int)Rect.Width / 2 - (int)Size.Width / 2, (int)Rect.Height / 2 - (int)Size.Height / 2);
    }
}

public class AnimaExperimentalListView : Control
{
    public string[] Columns { get; set; }

    public ListViewItem[] Items { get; set; }

    public int ColumnWidth { get; set; } = 120;

    public int SelectedIndex { get; set; } = -1;

    public List<int> SelectedIndexes { get; set; } = new List<int>();

    public bool Multiselect { get; set; }

    public int Highlight { get; set; } = -1;

    public bool HandleItemsForeColor { get; set; }

    public bool Grid { get; set; }

    public int ItemSize { get; set; } = 16;

    public int SelectedCount
    {
        get
        {
            return SelectedIndexes.Count;
        }
    }

    public ListViewItem FocusedItem
    {
        get
        {
            if (SelectedIndexes == null)
                return new ListViewItem();

            return Items[SelectedIndexes[0]];
        }
    }

    public void Add(ListViewItem Item)
    {
        List<ListViewItem> ItemsList = Items.ToList();
        ItemsList.Add(Item);
        Items = ItemsList.ToArray();
        Invalidate();
    }

    private int BorderIndex = -1;

    public event SelectedIndexChangedEventHandler SelectedIndexChanged;

    public delegate void SelectedIndexChangedEventHandler(object Sender, int Index);

    public AnimaExperimentalListView()
    {
        DoubleBuffered = true;
        ForeColor = Color.FromArgb(200, 200, 200);
        Font = new Font("Segoe UI", 9);
        SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);
    }

    private SolidBrush ReturnForeFromItem(int I, ListViewItem Item)
    {
        if (SelectedIndexes.Contains(I))
            return new SolidBrush(Color.FromArgb(10, 10, 10));
        if (HandleItemsForeColor)
            return new SolidBrush(Item.ForeColor);
        else
            return new SolidBrush(ForeColor);
    }

    private SolidBrush ReturnForeFromSubItem(int I, ListViewItem.ListViewSubItem Item)
    {
        if (SelectedIndexes.Contains(I))
            return new SolidBrush(Color.FromArgb(10, 10, 10));
        if (HandleItemsForeColor)
            return new SolidBrush(Item.ForeColor);
        else
            return new SolidBrush(ForeColor);
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        e.Graphics.Clear(Color.FromArgb(50, 50, 53));

        using (SolidBrush Background = new SolidBrush(Color.FromArgb(55, 55, 58)))
        {
            e.Graphics.FillRectangle(Background, new Rectangle(1, 1, Width - 2, 26));
        }

        using (Pen Border = new Pen(Color.FromArgb(42, 42, 45)))
        using (Pen Shadow = new Pen(Color.FromArgb(65, 65, 68))
)
        {
            e.Graphics.DrawRectangle(Border, new Rectangle(0, 0, Width - 1, Height - 1));
            e.Graphics.DrawRectangle(Border, new Rectangle(0, 0, Width - 1, 26));
            e.Graphics.DrawLine(Shadow, 1, 1, Width - 2, 1);
        }

        if (Columns != null)
        {
            for (var I = 0; I <= Columns.Count() - 1; I++)
            {
                if (!(I == 0))
                {
                    using (SolidBrush Separator = new SolidBrush(Color.FromArgb(42, 42, 45)))
                    using (SolidBrush Shadow = new SolidBrush(Color.FromArgb(65, 65, 68))
)
                    {
                        e.Graphics.FillRectangle(Separator, new Rectangle(ColumnWidth * I, 1, 1, 26));
                        e.Graphics.FillRectangle(Shadow, new Rectangle(ColumnWidth * I - 1, 1, 1, 25));

                        if (Grid && Items != null)
                        {
                            e.Graphics.FillRectangle(Separator, new Rectangle(ColumnWidth * I, 1, 1, 26 + (Items.Count() * ItemSize)));
                            e.Graphics.FillRectangle(Shadow, new Rectangle(ColumnWidth * I - 1, 1, 1, 25 + (Items.Count() * ItemSize)));
                        }
                    }
                }

                using (SolidBrush Fore = new SolidBrush(ForeColor))
                {
                    e.Graphics.DrawString(Columns[I], Font, Fore, new Point((ColumnWidth * I) + 6, 4));
                }
            }
        }

        if (Items != null)
        {
            if (!(Highlight == -1))
            {
                using (SolidBrush Background = new SolidBrush(Color.FromArgb(66, 66, 69)))
                using (Pen Line = new Pen(Color.FromArgb(45, 45, 48))
)
                {
                    e.Graphics.FillRectangle(Background, new Rectangle(1, 26 + Highlight * ItemSize, Width - 2, ItemSize));
                    e.Graphics.DrawRectangle(Line, new Rectangle(1, 26 + Highlight * ItemSize, Width - 2, ItemSize));
                }
            }

            using (SolidBrush Selection = new SolidBrush(Color.FromArgb(41, 130, 232)))
            using (Pen Line = new Pen(Color.FromArgb(40, 40, 40))
)
            {
                if (Multiselect && !(SelectedIndexes.Count == 0))
                {
                    foreach (int Selected in SelectedIndexes)
                    {
                        e.Graphics.FillRectangle(Selection, new Rectangle(1, 26 + Selected * ItemSize, Width - 2, ItemSize));

                        if (Selected == 0 && SelectedIndexes.Count == 1)
                            e.Graphics.DrawLine(Line, 1, 26 + ItemSize, Width - 2, 26 + ItemSize);
                        else if (SelectedIndexes.Count == 1)
                            e.Graphics.DrawLine(Line, 1, 26 + ItemSize + Selected * ItemSize, Width - 2, 26 + ItemSize + Selected * ItemSize);
                    }
                }
                else if (!(SelectedIndexes.Count == 0))
                    e.Graphics.FillRectangle(Selection, new Rectangle(1, 26 + SelectedIndex * ItemSize, Width - 2, ItemSize));
            }

            if (SelectedIndexes.Count > 0)
                BorderIndex = SelectedIndexes.Max();

            for (var I = 0; I <= Items.Count() - 1; I++)
            {
                e.Graphics.DrawString(Items[I].Text, Font, ReturnForeFromItem(I, Items[I]), new Point(6, 26 + I * ItemSize + 2));

                if (Items[I].SubItems != null)
                {
                    for (var X = 0; X <= Items[I].SubItems.Count - 1; X++)
                    {
                        if (!(Items[I].SubItems[X].Text == Items[I].Text))
                            e.Graphics.DrawString(Items[I].SubItems[X].Text, Font, ReturnForeFromSubItem(I, Items[I].SubItems[X]), new Rectangle((ColumnWidth * X) + 6, 26 + I * ItemSize + 2, ColumnWidth - 8, 16));
                    }
                }
            }

            if (SelectedIndexes.Contains(BorderIndex))
            {
                using (Pen Line = new Pen(Color.FromArgb(40, 40, 40)))
                {
                    e.Graphics.DrawLine(Line, 1, 26 + ItemSize + BorderIndex * ItemSize, Width - 2, 26 + ItemSize + BorderIndex * ItemSize);
                }
            }
        }

        base.OnPaint(e);
    }
    public static bool IsControlDown()
    {
        return (Control.ModifierKeys & Keys.Control) != 0;
    }
    protected override void OnMouseUp(MouseEventArgs e)
    {
        int Selection = GetSelectedFromLocation(e.Location);

        if (Selection == -1 || !(e.Button == MouseButtons.Left))
            return;

        if (Multiselect && IsControlDown())
        {
            if (!SelectedIndexes.Contains(Selection))
                SelectedIndexes.Add(Selection);
            else
                SelectedIndexes.Remove(Selection);
        }
        else if (Multiselect && !IsControlDown())
            SelectedIndexes = new List<int>()
            {
                Selection
            };
        else
        {
            SelectedIndexes = new List<int>()
            {
                Selection
            };
            SelectedIndex = Selection;
        }

        if (Selection == -1)
            SelectedIndexes = new List<int>();

        Invalidate();

        SelectedIndexChanged?.Invoke(this, Selection);
        base.OnMouseUp(e);
    }

    private int GetSelectedFromLocation(Point P)
    {
        if (Items != null)
        {
            for (var I = 0; I <= Items.Count() - 1; I++)
            {
                if (new Rectangle(1, 26 + I * ItemSize, Width - 2, ItemSize).Contains(P))
                    return I;
            }
        }

        return -1;
    }
}

public class AnimaTextBox : Control
{
    private Graphics G;
    private TextBox _T;

    private TextBox T
    {
        [MethodImpl(MethodImplOptions.Synchronized)]
        get
        {
            return _T;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        set
        {
            if (_T != null)
            {
                _T.Enter -= _T_Enter;
                _T.KeyPress -= _T_KeyPress;
                _T.Leave -= _T_Leave;
                _T.TextChanged -= _T_TextChanged;
            }

            _T = value;
            if (_T != null)
            {
                _T.Enter += _T_Enter;
                _T.KeyPress += _T_KeyPress;
                _T.Leave += _T_Leave;
                _T.TextChanged += _T_TextChanged;
            }
        }
    }

    private void _T_TextChanged(object sender, EventArgs e)
    {
        OnTextChanged(EventArgs.Empty);
    }

    private void _T_Leave(object sender, EventArgs e)
    {
        AnimatingT2 = new Thread(UndoAnimation) { IsBackground = true };
        AnimatingT2.Start();
    }

    private void _T_KeyPress(object sender, KeyPressEventArgs e)
    {
        if (Numeric)
        {
            if ((int)(e.KeyChar) != 8)
            {
                if ((int)(e.KeyChar) < 48 | (int)(e.KeyChar) > 57)
                    e.Handled = true;
            }
        }
    }

    private void _T_Enter(object sender, EventArgs e)
    {
        if (!Block)
        {
            AnimatingT = new Thread(DoAnimation) { IsBackground = true };
            AnimatingT.Start();
        }
    }

    private Thread AnimatingT;
    private Thread AnimatingT2;

    private int[] RGB = new[] { 45, 45, 48 };
    private int RGB1 = 45;
    private bool Block;

    public bool Dark { get; set; }

    public bool Numeric { get; set; }

    public new string Text
    {
        get
        {
            return T.Text;
        }
        set
        {
            base.Text = value;
            T.Text = value;
            Invalidate();
        }
    }

    public int MaxLength
    {
        get
        {
            return T.MaxLength;
        }
        set
        {
            T.MaxLength = value;
            Invalidate();
        }
    }

    public bool UseSystemPasswordChar
    {
        get
        {
            return T.UseSystemPasswordChar;
        }
        set
        {
            T.UseSystemPasswordChar = value;
            Invalidate();
        }
    }

    public bool MultiLine
    {
        get
        {
            return T.Multiline;
        }
        set
        {
            T.Multiline = value;
            Helpers.Size = new Size(T.Width + 2, T.Height + 2);
            Invalidate();
        }
    }

    public bool ReadOnly
    {
        get
        {
            return T.ReadOnly;
        }
        set
        {
            T.ReadOnly = value;
            Invalidate();
        }
    }

    public AnimaTextBox()
    {
        DoubleBuffered = true;

        T = new TextBox() { BorderStyle = BorderStyle.None, ForeColor = Color.FromArgb(200, 200, 200), BackColor = Color.FromArgb(55, 55, 58), Location = new Point(6, 5), Multiline = false };

        Controls.Add(T);
    }

    protected override void CreateHandle()
    {
        if (Dark)
        {
            RGB = new[] { 42, 42, 45 };
            RGB1 = 45;
            T.BackColor = Color.FromArgb(45, 45, 48);
        }
        else
        {
            RGB = new[] { 70, 70, 70 };
            RGB1 = 70;
            T.BackColor = Color.FromArgb(55, 55, 58);
        }
        base.CreateHandle();
    }

    protected override void OnEnter(EventArgs e)
    {
        T.Select();
        base.OnEnter(e);
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        G = e.Graphics;

        if (Enabled)
        {
            if (Dark)
            {
                G.Clear(Color.FromArgb(45, 45, 48));

                using (Pen Border = new Pen(Color.FromArgb(RGB[0], RGB[1], RGB[2])))
                {
                    G.DrawRectangle(Border, new Rectangle(0, 0, Width - 1, Height - 1));
                }
            }
            else
            {
                G.Clear(Color.FromArgb(55, 55, 58));

                using (Pen Shadow = new Pen(Color.FromArgb(42, 42, 45)))
                using (Pen Border = new Pen(Color.FromArgb(RGB[0], RGB[1], RGB[2]))
)
                {
                    G.DrawRectangle(Border, new Rectangle(1, 1, Width - 3, Height - 3));
                    G.DrawRectangle(Shadow, new Rectangle(0, 0, Width - 1, Height - 1));
                }
            }
        }
        else
        {
            G.Clear(Color.FromArgb(50, 50, 53));
            T.BackColor = Color.FromArgb(50, 50, 53);

            using (Pen Border = new Pen(Color.FromArgb(42, 42, 45)))
            using (Pen Shadow = new Pen(Color.FromArgb(66, 66, 69))
)
            {
                e.Graphics.DrawRectangle(Border, new Rectangle(0, 0, Width - 1, Height - 1));
                e.Graphics.DrawRectangle(Shadow, new Rectangle(1, 1, Width - 3, Height - 3));
            }
        }

        base.OnPaint(e);
    }


    protected override void OnResize(EventArgs e)
    {
        if (MultiLine)
        {
            T.Size = new Size(Width - 7, Height - 7); Invalidate();
        }
        else
        {
            T.Size = new Size(Width - 8, Height - 15); Invalidate();
        }
        base.OnResize(e);
    }


    private void DoAnimation()
    {
        while (RGB[2] < 204 && !Block)
        {
            RGB[1] += 1;
            RGB[2] += 2;

            Invalidate();
            Thread.Sleep(5);
        }
    }

    private void UndoAnimation()
    {
        Block = true;

        while (RGB[2] > RGB1)
        {
            RGB[1] -= 1;
            RGB[2] -= 2;

            Invalidate();
            Thread.Sleep(5);
        }

        Block = false;
    }
}

public class AnimaProgressBar : ProgressBar
{
    private Graphics G;

    public AnimaProgressBar()
    {
        SetStyle(ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        G = e.Graphics;

        base.OnPaint(e);

        G.Clear(Color.FromArgb(37, 37, 40));

        using (Pen Border = new Pen(Color.FromArgb(35, 35, 38)))
        {
            G.DrawRectangle(Border, new Rectangle(0, 0, Width - 1, Height - 1));
        }
        int area = Convert.ToInt32((Value * Width) / Maximum);
        using (SolidBrush Background = new SolidBrush(Color.FromArgb(0, 122, 204)))
        {
            G.FillRectangle(Background, new Rectangle(1, 1, area - 2, Height - 2));
        }
    }
}

public class AnimaCheckBox : CheckBox
{
    private Graphics G;

    private Thread AnimatingT;
    private Thread AnimatingT2;

    private int[] RGB = new[] { 36, 36, 39 };

    private bool Block;

    public bool Radio { get; set; }

    public bool Caution { get; set; }

    private const string CheckedIcon = "iVBORw0KGgoAAAANSUhEUgAAAAsAAAAKCAMAAABVLlSxAAAASFBMVEUlJSYuLi8oKCmlpaXx8fGioqJoaGjOzs8+Pj/k5OTu7u5LS0zIyMiBgYKFhYXo6OhUVFWVlZW7u7t+fn7h4eE5OTlfX1+YmJn8uq7eAAAAA3RSTlMAAAD6dsTeAAAACXBIWXMAAABIAAAASABGyWs+AAAAO0lEQVQI12NgwAKYWVhhTDYWdkYok4OTixvCYGDiYeEFM/n4BQRZhCDywiz8XCKiDDAOixjcPGFxDCsASakBdDYGvzAAAAAldEVYdGRhdGU6Y3JlYXRlADIwMTYtMTItMTRUMTI6MDM6MjktMDY6MDB4J65tAAAAJXRFWHRkYXRlOm1vZGlmeQAyMDE2LTEyLTE0VDEyOjAzOjI5LTA2OjAwCXoW0QAAAABJRU5ErkJggg==";

    public AnimaCheckBox()
    {
        DoubleBuffered = true;
        Font = new Font("Segoe UI", 9);
        ForeColor = Color.FromArgb(200, 200, 200);
        SetStyle(ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        G = e.Graphics;
        G.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

        G.Clear(BackColor);

        using (SolidBrush Background = new SolidBrush(Color.FromArgb(38, 38, 41)))
        {
            G.FillRectangle(Background, new Rectangle(0, 0, 16, 16));
        }

        using (Pen Border = new Pen(Color.FromArgb(RGB[0], RGB[1], RGB[2])))
        {
            G.DrawRectangle(Border, new Rectangle(0, 0, 16, 16));
        }

        using (SolidBrush Fore = new SolidBrush(ForeColor))
        {
            G.DrawString(Text, Font, Fore, new Point(22, 0));
        }

        if (Checked)
        {
            using (Image Mark = Image.FromStream(new System.IO.MemoryStream(Convert.FromBase64String(CheckedIcon))))
            {
                G.DrawImage(Mark, new Point(2, 3));
            }
        }
    }

    protected override void OnMouseEnter(EventArgs e)
    {
        if (!Block)
        {
            AnimatingT = new Thread(DoAnimation) { IsBackground = true };
            AnimatingT.Start();
        }

        base.OnMouseEnter(e);
    }

    protected override void OnMouseLeave(EventArgs e)
    {
        AnimatingT2 = new Thread(UndoAnimation) { IsBackground = true };
        AnimatingT2.Start();

        base.OnMouseLeave(e);
    }

    protected override void OnMouseUp(MouseEventArgs e)
    {
        if (Radio)
        {
            foreach (AnimaCheckBox C in Parent.Controls.OfType<AnimaCheckBox>())
            {
                if (C.Radio)
                    C.Checked = false;
            }

        }

        base.OnMouseUp(e);
    }

    private void DoAnimation()
    {
        if (Caution)
        {
            while (RGB[2] < 130 && !Block)
            {
                RGB[1] += 1;
                RGB[2] += 1;

                Invalidate();
                Thread.Sleep(4);
            }
        }
        else
            while (RGB[2] < 204 && !Block)
            {
                RGB[1] += 1;
                RGB[2] += 2;

                Invalidate();
                Thread.Sleep(4);
            }
    }

    private void UndoAnimation()
    {
        Block = true;

        if (Caution)
        {
            while (RGB[2] > 42)
            {
                RGB[1] -= 1;
                RGB[2] -= 1;

                Invalidate();
                Thread.Sleep(4);
            }
        }
        else
            while (RGB[2] > 42)
            {
                RGB[1] -= 1;
                RGB[2] -= 2;

                Invalidate();
                Thread.Sleep(4);
            }

        Block = false;
    }
}

public class AnimaHeader : Control
{
    private Graphics G;

    public AnimaHeader()
    {
        DoubleBuffered = true;
        Font = new Font("Segoe UI", 9);
        Location = new Point(1, 1);
        Helpers.Size = new Size(Width - 2, 36);
        ForeColor = Color.FromArgb(200, 200, 200);
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        G = e.Graphics;

        using (SolidBrush Background = new SolidBrush(Color.FromArgb(55, 55, 58)))
        {
            G.FillRectangle(Background, new Rectangle(0, 0, Width - 1, Height - 1));
        }

        using (Pen Line = new Pen(Color.FromArgb(43, 43, 46)))
        {
            G.DrawLine(Line, 0, Height - 1, Width - 1, Height - 1);
        }

        using (SolidBrush Fore = new SolidBrush(ForeColor))
        {
            G.DrawString(Text, Font, Fore, new Point(6, 6));
        }

        base.OnPaint(e);
    }
}

public class AnimaForm : ContainerControl
{
    private Graphics G;

    public AnimaForm()
    {
        BackColor = Color.FromArgb(45, 45, 48);
        DoubleBuffered = true;
        Font = new Font("Segoe UI", 9);
        Dock = DockStyle.Fill;
        ForeColor = Color.FromArgb(200, 200, 200);
    }

    protected override void OnCreateControl()
    {
        ParentForm.FormBorderStyle = FormBorderStyle.None;
        base.OnCreateControl();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        G = e.Graphics;

        using (SolidBrush Background = new SolidBrush(Color.FromArgb(55, 55, 58)))
        {
            G.FillRectangle(Background, new Rectangle(0, 0, Width - 1, 36));
        }

        using (Pen Line = new Pen(Color.FromArgb(43, 43, 46)))
        {
            G.DrawLine(Line, 0, 36, Width - 1, 36);
        }

        using (SolidBrush Fore = new SolidBrush(ForeColor))
        {
            G.DrawString(Text, Font, Fore, new Point(10, 10));
        }

        using (Pen Border = new Pen(Color.FromArgb(35, 35, 38)))
        {
            e.Graphics.DrawRectangle(Border, new Rectangle(0, 0, Width - 1, Height - 1));
        }

        base.OnPaint(e);
    }

    private bool Drag;
    private Point MousePoint = new Point(), Temporary = new Point();

    protected override void OnMouseDown(MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Left && e.Y < 36)
        {
            Drag = true;
            MousePoint.X = e.X;
            MousePoint.Y = e.Y;
        }

        base.OnMouseDown(e);
    }

    protected override void OnMouseUp(MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Left)
            Drag = false;

        base.OnMouseUp(e);
    }

    protected override void OnMouseMove(MouseEventArgs e)
    {
        if (Drag)
        {
            Temporary.X = Parent.Location.X + (e.X - MousePoint.X);
            Temporary.Y = Parent.Location.Y + (e.Y - MousePoint.Y);
            Parent.Location = Temporary;
            Temporary = Point.Empty;
        }

        base.OnMouseMove(e);
    }
}

public class AnimaButton : Button
{
    private Graphics G;

    private Thread HoverAnim, CHoverAnim, DownAnimationT;

    private int[] RGB = new[] { 42, 42, 45 };

    private Point Loc = new Point();
    private Size RSize = new Size();

    private bool Block;

    private bool Animate;

    public Point ImageLocation { get; set; }

    public Size ImageSize { get; set; } = new Size(14, 14);

    public AnimaButton()
    {
        DoubleBuffered = true;
        Font = new Font("Segoe UI", 9);
        ForeColor = Color.FromArgb(200, 200, 200);
        SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
        ImageLocation = new Point(Width / 2 - 7, 6);
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        G = e.Graphics;

        G.Clear(BackColor);

        using (SolidBrush Background = new SolidBrush(Color.FromArgb(55, 55, 58)))
        {
            G.FillRectangle(Background, new Rectangle(0, 0, Width - 1, Height - 1));
        }

        if (Animate)
        {
            using (SolidBrush Background = new SolidBrush(Color.FromArgb(66, 66, 69)))
            {
                G.FillEllipse(Background, new Rectangle(Loc.X, -30, RSize.Width, 80));
            }
        }

        using (Pen Border = new Pen(Color.FromArgb(RGB[0], RGB[1], RGB[2])))
        {
            G.DrawRectangle(Border, new Rectangle(0, 0, Width - 1, Height - 1));
        }

        if (Image != null)
            G.DrawImage(Image, new Rectangle(ImageLocation, ImageSize));
        else
            using (SolidBrush Fore = new SolidBrush(ForeColor))
            {
                G.DrawString(Text, Font, Fore, Helpers.MiddlePoint(G, Text, Font, new Rectangle(0, 0, Width - 1, Height - 1)));
            }
    }

    protected override void OnMouseEnter(EventArgs e)
    {
        if (!Block)
        {
            HoverAnim = new Thread(DoAnimation) { IsBackground = true };
            HoverAnim.Start();
        }

        base.OnMouseEnter(e);
    }

    protected override void OnMouseLeave(EventArgs e)
    {
        CHoverAnim = new Thread(UndoAnimation) { IsBackground = true };
        CHoverAnim.Start();

        base.OnMouseLeave(e);
    }

    protected override void OnMouseDown(MouseEventArgs e)
    {
        DownAnimationT = new Thread(() => DoBackAnimation(e.Location)) { IsBackground = true };
        DownAnimationT.Start();

        base.OnMouseDown(e);
    }

    private void DoBackAnimation(Point P)
    {
        Loc = P;
        RSize = new Size();

        Animate = true; Invalidate();

        while (RSize.Width < Width * 3)
        {
            Loc.X -= 1;
            RSize.Width += 2;
            Thread.Sleep(4);
            Invalidate();
        }

        Animate = false; Invalidate();
    }

    private void DoAnimation()
    {
        while (RGB[2] < 204 && !Block)
        {
            RGB[1] += 1;
            RGB[2] += 2;

            Invalidate();
            Thread.Sleep(5);
        }
    }

    private void UndoAnimation()
    {
        Block = true;

        while (RGB[2] > 45)
        {
            RGB[1] -= 1;
            RGB[2] -= 2;

            Invalidate();
            Thread.Sleep(5);
        }

        Block = false;
    }
}

public class Renderer : ToolStripRenderer
{
    public event PaintMenuBackgroundEventHandler PaintMenuBackground;

    public delegate void PaintMenuBackgroundEventHandler(object sender, ToolStripRenderEventArgs e);

    public event PaintMenuBorderEventHandler PaintMenuBorder;

    public delegate void PaintMenuBorderEventHandler(object sender, ToolStripRenderEventArgs e);

    public event PaintMenuImageMarginEventHandler PaintMenuImageMargin;

    public delegate void PaintMenuImageMarginEventHandler(object sender, ToolStripRenderEventArgs e);

    public event PaintItemCheckEventHandler PaintItemCheck;

    public delegate void PaintItemCheckEventHandler(object sender, ToolStripItemImageRenderEventArgs e);

    public event PaintItemImageEventHandler PaintItemImage;

    public delegate void PaintItemImageEventHandler(object sender, ToolStripItemImageRenderEventArgs e);

    public event PaintItemTextEventHandler PaintItemText;

    public delegate void PaintItemTextEventHandler(object sender, ToolStripItemTextRenderEventArgs e);

    public event PaintItemBackgroundEventHandler PaintItemBackground;

    public delegate void PaintItemBackgroundEventHandler(object sender, ToolStripItemRenderEventArgs e);

    public event PaintItemArrowEventHandler PaintItemArrow;

    public delegate void PaintItemArrowEventHandler(object sender, ToolStripArrowRenderEventArgs e);

    public event PaintSeparatorEventHandler PaintSeparator;

    public delegate void PaintSeparatorEventHandler(object sender, ToolStripSeparatorRenderEventArgs e);

    protected override void OnRenderToolStripBackground(ToolStripRenderEventArgs e)
    {
        PaintMenuBackground?.Invoke(this, e);
    }

    protected override void OnRenderImageMargin(ToolStripRenderEventArgs e)
    {
        PaintMenuImageMargin?.Invoke(this, e);
    }

    protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e)
    {
        PaintMenuBorder?.Invoke(this, e);
    }

    protected override void OnRenderItemCheck(ToolStripItemImageRenderEventArgs e)
    {
        PaintItemCheck?.Invoke(this, e);
    }

    protected override void OnRenderItemImage(ToolStripItemImageRenderEventArgs e)
    {
        PaintItemImage?.Invoke(this, e);
    }

    protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
    {
        PaintItemText?.Invoke(this, e);
    }

    protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)
    {
        PaintItemBackground?.Invoke(this, e);
    }

    protected override void OnRenderArrow(ToolStripArrowRenderEventArgs e)
    {
        PaintItemArrow?.Invoke(this, e);
    }

    protected override void OnRenderSeparator(ToolStripSeparatorRenderEventArgs e)
    {
        PaintSeparator?.Invoke(this, e);
    }
}

public class AnimaContextMenuStrip : ContextMenuStrip
{
    private Graphics G;
    private Rectangle Rect;

    public AnimaContextMenuStrip()
    {
        Font = new Font("Segoe UI", 9);
        ForeColor = Color.FromArgb(200, 200, 200);
        Renderer Render = new Renderer();
        Render.PaintMenuBackground += Renderer_PaintMenuBackground;
        Render.PaintMenuBorder += Renderer_PaintMenuBorder;
        Render.PaintItemImage += Renderer_PaintItemImage;
        Render.PaintItemText += Renderer_PaintItemText;
        Render.PaintItemBackground += Renderer_PaintItemBackground;
        Render.PaintItemArrow += Rendered_PaintItemArrow;

        Renderer = Render;
    }

    private void Rendered_PaintItemArrow(object sender, ToolStripArrowRenderEventArgs e)
    {
        G = e.Graphics;

        using (Font F = new Font("Marlett", 10))
        using (SolidBrush Fore = new SolidBrush(Color.FromArgb(130, 130, 130))
)
        {
            G.DrawString("4", F, Fore, new Point(e.ArrowRectangle.X, e.ArrowRectangle.Y + 2));
        }
    }

    private void Renderer_PaintMenuBackground(object sender, ToolStripRenderEventArgs e)
    {
        G = e.Graphics;

        G.Clear(Color.FromArgb(55, 55, 58));
    }

    private void Renderer_PaintMenuBorder(object sender, ToolStripRenderEventArgs e)
    {
        G = e.Graphics;

        using (Pen Border = new Pen(Color.FromArgb(35, 35, 38)))
        {
            G.DrawRectangle(Border, new Rectangle(e.AffectedBounds.X, e.AffectedBounds.Y, e.AffectedBounds.Width - 1, e.AffectedBounds.Height - 1));
        }
    }

    private void Renderer_PaintItemImage(object sender, ToolStripItemImageRenderEventArgs e)
    {
        G = e.Graphics;

        G.DrawImage(e.Image, new Point(10, 1));
    }

    private void Renderer_PaintItemText(object sender, ToolStripItemTextRenderEventArgs e)
    {
        G = e.Graphics;

        using (SolidBrush Fore = new SolidBrush(e.TextColor))
        {
            G.DrawString(e.Text, Font, Fore, new Point(e.TextRectangle.X, e.TextRectangle.Y - 1));
        }
    }

    private void Renderer_PaintItemBackground(object sender, ToolStripItemRenderEventArgs e)
    {
        G = e.Graphics;

        Rect = e.Item.ContentRectangle;

        if (e.Item.Selected)
        {
            using (SolidBrush Background = new SolidBrush(Color.FromArgb(85, 85, 85)))
            {
                G.FillRectangle(Background, new Rectangle(Rect.X - 4, Rect.Y - 1, Rect.Width + 4, Rect.Height - 1));
            }
        }
    }
}

public class AnimaStatusBar : Control
{
    private Graphics G;

    private Color Body = Color.FromArgb(0, 122, 204);
    private Color Outline = Color.FromArgb(0, 126, 204);

    public Types Type { get; set; }

    public enum Types : byte
    {
        Basic = 0,
        Warning = 1,
        Wrong = 2,
        Success = 3
    }

    public AnimaStatusBar()
    {
        DoubleBuffered = true;
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        G = e.Graphics;

        switch (Type)
        {
            case Types.Basic:
                {
                    Body = Color.FromArgb(0, 122, 204);
                    Outline = Color.FromArgb(0, 126, 204);
                    break;
                }

            case Types.Warning:
                {
                    Body = Color.FromArgb(210, 143, 75);
                    Outline = Color.FromArgb(214, 147, 75);
                    break;
                }

            case Types.Wrong:
                {
                    Body = Color.FromArgb(212, 110, 110);
                    Outline = Color.FromArgb(216, 114, 114);
                    break;
                }

            default:
                {
                    Body = Color.FromArgb(45, 193, 90);
                    Outline = Color.FromArgb(45, 197, 90);
                    break;
                }
        }

        using (SolidBrush Background = new SolidBrush(Body))
        using (Pen Line = new Pen(Outline)
)
        {
            G.FillRectangle(Background, new Rectangle(0, 0, Width - 1, Height - 1));
            G.DrawLine(Line, 0, 0, Width - 2, 0);
        }

        using (SolidBrush Fore = new SolidBrush(Color.FromArgb(255, 255, 255)))
        using (Font Font = new Font("Segoe UI semibold", 8)
)
        {
            G.DrawString(Text, Font, Fore, new Point(4, 2));
        }

        base.OnPaint(e);
    }

    protected override void OnTextChanged(EventArgs e)
    {
        Invalidate();
        base.OnTextChanged(e);
    }
}

public class AnimaTabControl : TabControl
{
    private Graphics G;
    private Rectangle Rect;

    protected override void OnControlAdded(ControlEventArgs e)
    {
        e.Control.BackColor = Color.FromArgb(45, 45, 48);
        e.Control.ForeColor = Color.FromArgb(200, 200, 200);
        e.Control.Font = new Font("Segoe UI", 9);
        base.OnControlAdded(e);
    }

    public AnimaTabControl()
    {
        DoubleBuffered = true;
        Font = new Font("Segoe UI", 9);
        ForeColor = Color.FromArgb(200, 200, 200);
        ItemSize = new Size(18, 18);
        SizeMode = TabSizeMode.Fixed;
        Alignment = TabAlignment.Top;
        SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.Opaque | ControlStyles.OptimizedDoubleBuffer, true);
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        G = e.Graphics;

        G.Clear(Parent.BackColor);

        for (var I = 0; I <= TabPages.Count - 1; I++)
        {
            Rect = GetTabRect(I);

            if (SelectedIndex == I)
            {
                using (SolidBrush Background = new SolidBrush(Color.FromArgb(41, 130, 232)))
                using (Pen Border = new Pen(Color.FromArgb(38, 127, 229))
)
                {
                    G.FillRectangle(Background, new Rectangle(Rect.X + 5, Rect.Y + 2, 12, 12));
                    G.DrawRectangle(Border, new Rectangle(Rect.X + 5, Rect.Y + 2, 12, 12));
                }
            }
            else
                using (SolidBrush Background = new SolidBrush(Color.FromArgb(70, 70, 73)))
                using (Pen Border = new Pen(Color.FromArgb(42, 42, 45))
)
                {
                    G.FillRectangle(Background, new Rectangle(Rect.X + 5, Rect.Y + 2, 12, 12));
                    G.DrawRectangle(Border, new Rectangle(Rect.X + 5, Rect.Y + 2, 12, 12));
                }
        }

        base.OnPaint(e);
    }
}

public class AnimaGroupBox : Control
{
    public AnimaGroupBox()
    {
        DoubleBuffered = true;
        ForeColor = Color.FromArgb(200, 200, 200);
        BackColor = Color.FromArgb(50, 50, 53);
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        e.Graphics.Clear(BackColor);

        using (Pen Border = new Pen(Color.FromArgb(42, 42, 45)))
        using (SolidBrush HBackground = new SolidBrush(Color.FromArgb(60, 60, 63)))
        using (Pen Shadow = new Pen(Color.FromArgb(66, 66, 69))
)
        {
            e.Graphics.FillRectangle(HBackground, new Rectangle(1, 0, Width - 2, 26));
            e.Graphics.DrawRectangle(Border, new Rectangle(0, 0, Width - 1, 26));
            e.Graphics.DrawLine(Shadow, 1, 25, Width - 2, 25);
            e.Graphics.DrawRectangle(Border, new Rectangle(0, 0, Width - 1, Height - 1));
        }

        using (SolidBrush Fore = new SolidBrush(ForeColor))
        {
            e.Graphics.DrawString(Text, Font, Fore, new Point(4, 4));
        }

        base.OnPaint(e);
    }
}

public class AnimaExperimentalControlBox : Control
{
    public int TextHeight { get; set; } = 4;

    public int ComboHeight { get; set; } = 24;

    public string[] Items { get; set; }

    public int SelectedIndex { get; set; } = 0;

    public int ItemSize { get; set; } = 24;

    public string SelectedItem { get; set; }

    public AnimaGroupBox AnimaGroupBoxContainer { get; set; }

    public Point CurrentLocation { get; set; }

    public event SelectedIndexChangedEventHandler SelectedIndexChanged;

    public delegate void SelectedIndexChangedEventHandler();

    private bool Open;
    private int ItemsHeight = 0;
    private int Hover = -1;

    public AnimaExperimentalControlBox()
    {
        DoubleBuffered = true;
        Font = new Font("Segoe UI", 9);
        ForeColor = Color.FromArgb(200, 200, 200);
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        e.Graphics.Clear(Parent.BackColor);

        if (Enabled)
        {
            if (Items != null)
                ItemsHeight = Items.Count() * ItemSize;

            if (!DesignMode)
            {
                if (Open)
                    Height = ItemsHeight + ComboHeight + 5;
                else
                    Height = ComboHeight + 1;
            }

            using (SolidBrush Background = new SolidBrush(Color.FromArgb(55, 55, 58)))
            using (Pen Border = new Pen(Color.FromArgb(42, 42, 45)))
            using (Pen Shadow = new Pen(Color.FromArgb(66, 66, 69))
)
            {
                e.Graphics.FillRectangle(Background, new Rectangle(0, 0, Width - 1, ComboHeight - 1));
                e.Graphics.DrawRectangle(Border, new Rectangle(0, 0, Width - 1, ComboHeight - 1));
                e.Graphics.DrawRectangle(Shadow, new Rectangle(1, 1, Width - 3, ComboHeight - 3));
            }

            if (Items == null)
                SelectedIndex = -1;
            else if (!(SelectedIndex == -1))
            {
                using (SolidBrush Fore = new SolidBrush(ForeColor))
                {
                    e.Graphics.DrawString(Items[SelectedIndex], Font, Fore, new Point(4, 4));
                }
            }

            if (Open)
            {
                using (SolidBrush Background = new SolidBrush(Color.FromArgb(60, 60, 63)))
                using (Pen Border = new Pen(Color.FromArgb(41, 130, 232))
)
                {
                    e.Graphics.FillRectangle(Background, new Rectangle(1, ComboHeight, Width - 3, ItemsHeight));
                    e.Graphics.DrawRectangle(Border, new Rectangle(1, ComboHeight, Width - 3, ItemsHeight));
                }

                if (!(Hover == -1))
                {
                    using (SolidBrush Background = new SolidBrush(Color.FromArgb(41, 130, 232)))
                    using (Pen Border = new Pen(Color.FromArgb(40, 40, 40))
)
                    {
                        e.Graphics.FillRectangle(Background, new Rectangle(1, ComboHeight + Hover * ItemSize, Width - 2, ItemSize));
                        e.Graphics.DrawLine(Border, 1, ComboHeight + Hover * ItemSize + ItemSize, Width - 2, ComboHeight + Hover * ItemSize + ItemSize);
                    }
                }

                for (var I = 0; I <= Items.Count() - 1; I++)
                {
                    if (Hover == I)
                    {
                        using (SolidBrush Fore = new SolidBrush(Color.FromArgb(15, 15, 15)))
                        {
                            e.Graphics.DrawString(Items[I], Font, Fore, new Point(4, ComboHeight + TextHeight + I * ItemSize));
                        }
                    }
                    else
                        using (SolidBrush Fore = new SolidBrush(ForeColor))
                        {
                            e.Graphics.DrawString(Items[I], Font, Fore, new Point(4, ComboHeight + TextHeight + I * ItemSize));
                        }
                }
            }
        }
        else
        {
            using (SolidBrush Background = new SolidBrush(Color.FromArgb(50, 50, 53)))
            using (Pen Border = new Pen(Color.FromArgb(42, 42, 45)))
            using (Pen Shadow = new Pen(Color.FromArgb(66, 66, 69))
)
            {
                e.Graphics.FillRectangle(Background, new Rectangle(0, 0, Width - 1, ComboHeight - 1));
                e.Graphics.DrawRectangle(Border, new Rectangle(0, 0, Width - 1, ComboHeight - 1));
                e.Graphics.DrawRectangle(Shadow, new Rectangle(1, 1, Width - 3, ComboHeight - 3));
            }

            if (Items != null && !(SelectedIndex == -1))
            {
                using (SolidBrush Fore = new SolidBrush(Color.FromArgb(140, 140, 140)))
                {
                    e.Graphics.DrawString(Items[SelectedIndex], Font, Fore, new Point(4, 4));
                }
            }
        }

        base.OnPaint(e);
    }

    protected override void OnLostFocus(EventArgs e)
    {
        Open = false; Invalidate();
        base.OnLostFocus(e);
    }

    protected override void OnMouseDown(MouseEventArgs e)
    {

        // If Not Open Then Main.BTab1.SendToBack() : Else : BringToFront()

        if (Open && !(ItemsHeight == 0))
        {
            for (var I = 0; I <= Items.Count() - 1; I++)
            {
                if (new Rectangle(0, ComboHeight + I * ItemSize, Width - 1, ItemSize).Contains(e.Location))
                {
                    SelectedIndex = I; Invalidate();
                    SelectedItem = Items[SelectedIndex];
                    break;
                }
            }
        }

        if (!new Rectangle(0, 0, Width - 1, ComboHeight + 4).Contains(e.Location))
        {
            if (Open && !(SelectedIndex == -1))
                SelectedIndexChanged?.Invoke();
        }

        Open = !Open; Invalidate();
        this.Select();
        base.OnMouseDown(e);
    }

    protected override void OnMouseMove(MouseEventArgs e)
    {
        if (Open && !(ItemsHeight == 0))
        {
            for (var I = 0; I <= Items.Count() - 1; I++)
            {
                if (new Rectangle(0, ComboHeight + I * ItemSize, Width - 1, ItemSize).Contains(e.Location))
                {
                    Hover = I; Invalidate();
                    break;
                }
            }
        }

        base.OnMouseMove(e);
    }
}