
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Final_minersweeper
{
    public partial class Form1 : Form
    {
        int height = -10;
        int width = -10;
        int offset = 25;
        int bombPercent = 5;
        bool isFirstClick = true;
        int cellsOpened = 0;
        int bombs = 0;

        FieldButton[,] field;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
        }
        void GenerateField()
        {

            Random random = new Random();
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    FieldButton newButton = new FieldButton();
                    newButton.Location = new Point(x * offset, y * offset);
                    newButton.Size = new Size(offset, offset);
                    newButton.isClickable = true;
                    if (random.Next(0, 100) < bombPercent) //бомба с вероятностью 20%
                    {
                        newButton.isBomb = true;
                        bombs++;
                    }
                    newButton.xCoord = x;
                    newButton.yCoord = y;
                    Controls.Add(newButton);
                    newButton.MouseUp += new MouseEventHandler(FieldButtonClick);
                    field[x, y] = newButton;
                }
            }
        }

        void FieldButtonClick(object sender, MouseEventArgs e) //для каждой кнопки при нажатии
        {
            FieldButton clickedButton = (FieldButton)sender;
            if (e.Button == MouseButtons.Left && clickedButton.isClickable)
            {
                if (clickedButton.isBomb)
                {
                    if (isFirstClick)
                    {
                        clickedButton.isBomb = false;
                        isFirstClick = false;
                        bombs--;
                        OpenRegion(clickedButton.xCoord, clickedButton.yCoord, clickedButton);
                    }
                    else
                    {
                        Explode();
                    }
                }
                else
                {
                    EmptyFieldButtonClick(clickedButton);
                }
                isFirstClick = false;
            }
            if (e.Button == MouseButtons.Right)
            {
                clickedButton.isClickable = !clickedButton.isClickable; //деактивация
                if (clickedButton.isClickable)
                {
                    clickedButton.Text = "B"; //бомба
                }
                else
                {
                    clickedButton.Text = ""; //не бомба
                }
            }
            CheckWin();
        }
        void Explode()
        {
            foreach (FieldButton button in field)
            {
                if (button.isBomb)
                {
                    button.Text = "*";
                }
            }
            MessageBox.Show("Вы проиграли");
            Application.Restart();


        }
        void EmptyFieldButtonClick(FieldButton clickedButton)
        {

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (field[x, y] == clickedButton)
                    {
                        OpenRegion(x, y, clickedButton);
                    }
                }
            }

        }

        void OpenRegion(int xCoord, int yCoord, FieldButton clickedButton)
        {
            Queue<FieldButton> queue = new Queue<FieldButton>();
            queue.Enqueue(clickedButton);
            clickedButton.wasAdded = true;

            while (queue.Count > 0)
            {
                FieldButton currentCell = queue.Dequeue();
                OpenCell(currentCell.xCoord, currentCell.yCoord, currentCell); //открытие поля
                cellsOpened++;
                if (CountBombsAround(currentCell.xCoord, currentCell.yCoord) == 0)
                {
                    for (int y = currentCell.yCoord - 1; y <= currentCell.yCoord + 1; y++) //проходим по полю 3 на 3 вокруг нажатой клетки
                    {
                        for (int x = currentCell.xCoord - 1; x <= currentCell.xCoord + 1; x++)
                        {
                            if (x == currentCell.xCoord && y == currentCell.yCoord)
                            {
                                continue;
                            }
                            if (x >= 0 && x < width && y >= 0 && y < height) //если не выходим за границы поля 3 на 3
                            {
                                if (!field[x, y].wasAdded)
                                {
                                    queue.Enqueue(field[x, y]);
                                    field[x, y].wasAdded = true;
                                }
                            }
                        }
                    }
                }
            }
            CheckWin();
        }
        void OpenCell(int x, int y, FieldButton clickedButton)
        {
            int bombsAround = CountBombsAround(x, y);

            if (bombsAround == 0)
            {

            }
            else
            {
                clickedButton.Text = "" + bombsAround;
            }
            clickedButton.Enabled = false; //если нет бомб, то деактивация
        }
        int CountBombsAround(int xCoord, int yCoord) //позиция кнопки в массиве
        {
            int bombsAround = 0;
            for (int x = xCoord - 1; x <= xCoord + 1; x++)
            {
                for (int y = yCoord - 1; y <= yCoord + 1; y++)
                {
                    if (x >= 0 && x < width && y >= 0 && y < height)
                    {
                        if (field[x, y].isBomb == true)
                        {
                            bombsAround++;
                        }
                    }
                }
            }
            return bombsAround;
        }
        void CheckWin()
        {
            int cells = width * height;
            int emptyCells = cells - bombs;
            if (cellsOpened >= emptyCells)
            {
                MessageBox.Show("Вы победили! :)");
            }
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            width = 10;
            height = 10;
            field = new FieldButton[width, height];            
            GenerateField();
            
        }

        private void toolStripMenuItem3_Click(object sender, EventArgs e)
        {
            width = 20;
            height = 20;
            field = new FieldButton[width, height];
            GenerateField();
        }

        private void toolStripMenuItem4_Click(object sender, EventArgs e)
        {
            width = 30;
            height = 30;
            field = new FieldButton[width, height];
            GenerateField();
        }

        private void настройкиToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }
    }
    class FieldButton : Button
    {
        public bool isBomb;
        public bool isClickable;
        public bool wasAdded;
        public int xCoord;
        public int yCoord;

    }
}




